using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using DecisionBox.Models;
using NativeWebSocket;

#nullable enable

namespace DecisionBox.Core
{
    /// <summary>
    /// Main DecisionBox SDK for Unity
    /// </summary>
    public class DecisionBoxSDK : MonoBehaviour
    {
        [Header("SDK Configuration")]
        [SerializeField] private string appId = "";
        [SerializeField] private string appSecret = "";
        [SerializeField] private string environment = "production"; // "production" or "development"
        [SerializeField] private bool enableLogging = false;

        public static DecisionBoxSDK Instance { get; private set; } = null!;
            
        private string EventApiUrl => environment == "development" 
            ? "https://eventapi.dev.decisionbox.io" 
            : "https://eventapi.decisionbox.io";

        private string websocketUrl => environment == "development"
            ? "wss://ws.dev.decisionbox.io/ws"
            : "wss://ws.decisionbox.io/ws";

        // Private fields
        private bool sdkActive = false;
        private string? currentUserId;
        public string CurrentSessionId;
        private string jwtToken = "";
        private long tokenExpiry = 0;
        private RemoteConfig? remoteConfig;
        private WebSocket? websocket;
        private Dictionary<string, List<Action<string>>> websocketHandlers = new();
        private Queue<EventData> pendingEvents = new();
        private long sessionStartTime = 0;
        private long backgroundStartTime = 0;
        private bool hasSentSessionEndEvent = false;

        #region Unity Lifecycle

        public string GetAppID()
        {
            return appId;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SDKLog("DecisionBox SDK initialized");
            }
            else
            {
                SDKLogWarning("Another DecisionBox SDK instance already exists. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!sdkActive) return;

            if (pauseStatus)
            {
                backgroundStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                SDKLog("App going to background");
            }
            else
            {
                SDKLog("App resumed from background");
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                
                if (backgroundStartTime != 0 && remoteConfig != null)
                {
                    long backgroundDuration = currentTime - backgroundStartTime;
                    if (backgroundDuration > remoteConfig.maxBackgroundDuration)
                    {
                        SDKLog("Background duration exceeded. Starting new session");
                        EndCurrentSession("background_timeout");
                        StartNewSession();
                    }
                }
                backgroundStartTime = 0;
            }
        }

        private void OnApplicationQuit()
        {
            if (sdkActive && !hasSentSessionEndEvent)
            {
                EndCurrentSession("quit");
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Configure the SDK credentials. Can be called before InitializeAsync.
        /// </summary>
        public void Configure(string appId, string appSecret, string environment = "production", bool enableLogging = false)
        {
            this.appId = appId;
            this.appSecret = appSecret;
            this.environment = environment;
            this.enableLogging = enableLogging;
            SDKLog("SDK configured with new credentials");
        }

        /// <summary>
        /// Initialize the SDK. Must be called first.
        /// </summary>
        public async Task<bool> InitializeAsync(string? userId = null)
        {
            try
            {
                SDKLog("Initializing SDK...");

                // Validate configuration
                if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
                {
                    SDKLogError("SDK not configured. Please set App ID and App Secret in the Inspector or call Configure() method.");
                    return false;
                }

                // Authenticate and get token
                if (!await AuthenticateAsync())
                {
                    SDKLogError("Authentication failed");
                    return false;
                }

                // Fetch remote configuration
                remoteConfig = await FetchRemoteConfigAsync();
                if (remoteConfig == null || !remoteConfig.sdkEnabled)
                {
                    SDKLog("SDK disabled by remote config");
                    return false;
                }

                // Set user ID
                currentUserId = userId ?? GenerateGuidUserId();
                
                // SDK is now active
                sdkActive = true;
                
                // Start session
                StartNewSession();

                SDKLog($"SDK initialized successfully for user: {currentUserId}");
                return true;
            }
            catch (Exception ex)
            {
                SDKLogError($"SDK initialization failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Register a callback for WebSocket events
        /// </summary>
        public void On(string eventName, Action<string> callback)
        {
            if (!websocketHandlers.ContainsKey(eventName))
            {
                websocketHandlers[eventName] = new List<Action<string>>();
            }
            websocketHandlers[eventName].Add(callback);
            SDKLog($"Registered callback for: {eventName}");
        }

        /// <summary>
        /// Unregister a WebSocket callback
        /// </summary>
        public void Off(string eventName, Action<string> callback)
        {
            if (websocketHandlers.ContainsKey(eventName))
            {
                websocketHandlers[eventName].Remove(callback);
            }
        }

        #endregion

        #region Event Sending Methods

        /// <summary>
        /// Send GameStarted event
        /// </summary>
        public async Task<bool> SendGameStartedAsync(string? userId = null, int? initialSoftCurrency = null, int? initialHardCurrency = null, int? initialPremiumCurrency = null)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new GameStartedEvent(
                userId ?? currentUserId!,
                initialSoftCurrency,
                initialHardCurrency,
                initialPremiumCurrency
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send LevelStarted event
        /// </summary>
        public async Task<bool> SendLevelStartedAsync(string? userId = null, int levelNumber = 0, string? levelName = null, string? difficulty = null)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new LevelStartedEvent(
                userId ?? currentUserId!,
                levelNumber,
                levelName,
                difficulty
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send LevelSuccess event
        /// </summary>
        public async Task<bool> SendLevelSuccessAsync(string? userId = null, int levelNumber = 0)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new LevelSuccessEvent(
                userId ?? currentUserId!,
                levelNumber
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send LevelFailed event
        /// </summary>
        public async Task<bool> SendLevelFailedAsync(string? userId = null, int levelNumber = 0, FailureReason failureReason = FailureReason.TimeOut)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new LevelFailedEvent(
                userId ?? currentUserId!,
                levelNumber,
                failureReason
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send CurrencyBalanceUpdated event
        /// </summary>
        public async Task<bool> SendCurrencyBalanceUpdatedAsync(string? userId = null, CurrencyType currencyType = CurrencyType.Soft, int? oldBalance = null, int currentBalance = 0, int? delta = null, CurrencyUpdateReason updateReason = CurrencyUpdateReason.NotSpecified)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new CurrencyBalanceUpdatedEvent(
                userId ?? currentUserId!,
                currencyType,
                oldBalance,
                currentBalance,
                delta,
                updateReason
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send BoosterOffered event
        /// </summary>
        public async Task<bool> SendBoosterOfferedAsync(string? userId = null, int levelNumber = 0, BoosterType boosterType = BoosterType.SpeedBoost, OfferMethod offerMethod = OfferMethod.WatchAd, int requiredCurrency = 0)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new BoosterOfferedEvent(
                userId ?? currentUserId!,
                levelNumber,
                boosterType,
                offerMethod,
                requiredCurrency
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send BoosterAccepted event
        /// </summary>
        public async Task<bool> SendBoosterAcceptedAsync(string? userId = null, int levelNumber = 0, BoosterType boosterType = BoosterType.SpeedBoost, AcceptMethod acceptMethod = AcceptMethod.WatchAd, int spentAmount = 0)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new BoosterAcceptedEvent(
                userId ?? currentUserId!,
                levelNumber,
                boosterType,
                acceptMethod,
                spentAmount
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send BoosterDeclined event
        /// </summary>
        public async Task<bool> SendBoosterDeclinedAsync(string? userId = null, int levelNumber = 0, BoosterType boosterType = BoosterType.SpeedBoost, DeclineReason declineReason = DeclineReason.NotInterested)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new BoosterDeclinedEvent(
                userId ?? currentUserId!,
                levelNumber,
                boosterType,
                declineReason
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send MetricRecorded event
        /// </summary>
        public async Task<bool> SendMetricRecordedAsync(string? userId = null, int levelNumber = 0, MetricType metric = MetricType.Score, float metricValue = 0f)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new MetricRecordedEvent(
                userId ?? currentUserId!,
                levelNumber,
                metric,
                metricValue
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send UserLocationText event
        /// </summary>
        public async Task<bool> SendUserLocationTextAsync(string? userId = null, string location = "")
        {
            if (!ValidateSDKState()) return false;

            var eventData = new UserLocationTextEvent(
                userId ?? currentUserId!,
                location
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send UserLocationLatLng event
        /// </summary>
        public async Task<bool> SendUserLocationLatLngAsync(string? userId = null, double latitude = 0.0, double longitude = 0.0)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new UserLocationLatLngEvent(
                userId ?? currentUserId!,
                latitude,
                longitude
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send UserLocationIP event
        /// </summary>
        public async Task<bool> SendUserLocationIPAsync(string? userId = null, string ipAddress = "")
        {
            if (!ValidateSDKState()) return false;

            var eventData = new UserLocationIPEvent(
                userId ?? currentUserId!,
                ipAddress
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send ScoreUpdated event
        /// </summary>
        public async Task<bool> SendScoreUpdatedAsync(string? userId = null, int levelNumber = 0, int currentScore = 0, int? oldScore = null, int? scoreDelta = null, int comboMultiplier = 1)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new ScoreUpdatedEvent(
                userId ?? currentUserId!,
                levelNumber,
                currentScore,
                oldScore,
                scoreDelta,
                comboMultiplier
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send PowerUpCollected event
        /// </summary>
        public async Task<bool> SendPowerUpCollectedAsync(string? userId = null, int levelNumber = 0, string powerUpType = "", int quantity = 1, double? duration = null)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new PowerUpCollectedEvent(
                userId ?? currentUserId!,
                levelNumber,
                powerUpType,
                quantity,
                duration
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send ActionOutcomeRecorded Positive event
        /// </summary>
        public async Task<bool> SendPositiveActionOutcomeRecordedAsync(string? userId, string rulesetId, OfferMethod offerMethod, AcceptMethod acceptMethod,  double value)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new ActionOutcomeRecordedEvent(
                userId ?? currentUserId!,
                rulesetId,
                offerMethod,
                acceptMethod,
                true,
                value
            );
            return await SendEventAsync(eventData);
        }

                /// <summary>
        /// Send ActionOutcomeRecorded Positive event
        /// </summary>
        public async Task<bool> SendNegativeActionOutcomeRecordedAsync(string? userId, string rulesetId, OfferMethod offerMethod, DeclineReason declineReason,  double value)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new ActionOutcomeRecordedEvent(
                userId ?? currentUserId!,
                rulesetId,
                offerMethod,
                declineReason,
                false,
                value
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send LevelFinished event
        /// </summary>
        public async Task<bool> SendLevelFinishedAsync(string? userId = null, int levelNumber = 0)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new LevelFinishedEvent(
                userId ?? currentUserId!,
                levelNumber
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send GameEnded event
        /// </summary>
        public async Task<bool> SendGameEndedAsync(string? userId = null, int currentScore = 0, int? finalSoftCurrency = null, int? finalHardCurrency = null, int? finalPremiumCurrency = null)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new GameEndedEvent(
                userId ?? currentUserId!,
                currentScore,
                finalSoftCurrency,
                finalHardCurrency,
                finalPremiumCurrency
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send GameOver event
        /// </summary>
        public async Task<bool> SendGameOverAsync(string? userId = null, int levelNumber = 0, int currentScore = 0, FailureReason gameOverReason = FailureReason.TimeOut)
        {
            if (!ValidateSDKState()) return false;

            var eventData = new GameOverEvent(
                userId ?? currentUserId!,
                levelNumber,
                currentScore,
                gameOverReason
            );
            return await SendEventAsync(eventData);
        }

        /// <summary>
        /// Send User Device Token 
        /// </summary>
        public async Task<bool> SendUserDeviceTokenAsync(string? userId = null, string deviceToken = "")
        {
            if (!ValidateSDKState()) return false;

            var deviceTokenModel = new UserDeviceToken(
                userId ?? currentUserId!,
                GetPlatformType().ToString(),
                deviceToken
            );
            return await SendDeviceTokenAsync(deviceTokenModel);
        }


        #endregion

        #region Private Methods

        private bool ValidateSDKState()
        {
            if (!sdkActive)
            {
                SDKLogWarning("SDK not active. Call InitializeAsync first.");
                return false;
            }
            return true;
        }

        private string GenerateGuidUserId()
        {
            string userId = PlayerPrefs.GetString("DECISIONBOX_USER_ID", "");
            if (string.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("DECISIONBOX_USER_ID", userId);
                PlayerPrefs.Save();
            }
            return userId;
        }

        private void StartNewSession()
        {
            CurrentSessionId = Guid.NewGuid().ToString();
            sessionStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            hasSentSessionEndEvent = false;

            PlayerPrefs.SetString("DECISIONBOX_SESSION_ID", CurrentSessionId);
            PlayerPrefs.Save();

            // Send SessionStarted event
            _ = SendSessionStartedEventAsync();

            // Connect WebSocket
            if (remoteConfig?.websocketEnabled == true)
            {
                _ = ConnectWebSocketAsync();
            }

            SDKLog($"New session started: {CurrentSessionId}");
        }

        private void EndCurrentSession(string reason)
        {
            if (hasSentSessionEndEvent) return;

            _ = SendSessionEndedEventAsync(reason);
            hasSentSessionEndEvent = true;

            // Close WebSocket
            CloseWebSocket();

            SDKLog($"Session ended: {reason}");
        }

        private async Task<bool> AuthenticateAsync()
        {
            try
            {
                var authRequest = new
                {
                    app_id = appId,
                    client_secret = appSecret
                };

                string json = JsonConvert.SerializeObject(authRequest);
                byte[] bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{EventApiUrl}/oauth/token", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonConvert.DeserializeObject<AuthResponse>(request.downloadHandler.text);
                        if (response != null)
                        {
                            jwtToken = response.access_token;
                            tokenExpiry = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (response.ExpiresInSeconds * 1000);
                            SDKLog($"Token expires in {response.ExpiresInSeconds} seconds");
                        }
                        else
                        {
                            SDKLogError("Invalid authentication response");
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        SDKLogError($"Authentication failed: {request.error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"Authentication error: {ex.Message}");
                return false;
            }
        }

        private async Task<RemoteConfig?> FetchRemoteConfigAsync()
        {
            try
            {
                using (var request = new UnityWebRequest($"{EventApiUrl}/apps/config?appid={appId}", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(new byte[0]); // Empty body for POST
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        return JsonConvert.DeserializeObject<RemoteConfig>(request.downloadHandler.text);
                    }
                    else
                    {
                        SDKLogError($"Failed to fetch remote config: {request.error}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"Remote config fetch error: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> SendEventAsync(EventData eventData)
        {
            try
            {
                // Check if token needs refresh
                if (IsTokenExpired())
                {
                    if (!await RefreshTokenAsync())
                    {
                        SDKLogError("Failed to refresh token");
                        return false;
                    }
                }

                string json = JsonConvert.SerializeObject(eventData);
                byte[] bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{EventApiUrl}/events", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        SDKLog($"Event sent successfully: {eventData.EventType}");
                        return true;
                    }
                    else
                    {
                        SDKLogError($"Failed to send event: {request.error} - {request.responseCode}");
                        if (!string.IsNullOrEmpty(request.downloadHandler.text))
                        {
                            SDKLogError($"Response: {request.downloadHandler.text}");
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"Event sending error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendDeviceTokenAsync(UserDeviceToken deviceToken)
        {
            try
            {
                // Check if token needs refresh
                if (IsTokenExpired())
                {
                    if (!await RefreshTokenAsync())
                    {
                        SDKLogError("Failed to refresh token");
                        return false;
                    }
                }

                string json = JsonConvert.SerializeObject(deviceToken);
                byte[] bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{EventApiUrl}/users/token", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        SDKLog($"Device token sent successfully: {deviceToken.DeviceToken}");
                        return true;
                    }
                    else
                    {
                        SDKLogError($"Failed to send device token: {request.error} - {request.responseCode}");
                        if (!string.IsNullOrEmpty(request.downloadHandler.text))
                        {
                            SDKLogError($"Response: {request.downloadHandler.text}");
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"Device token sending error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendSessionStartedEventAsync()
        {
            var eventData = new SessionStartedEvent(
                currentUserId!,
                Application.version,
                GetPlatformType(),
                SystemInfo.deviceModel
            );
            return await SendEventAsync(eventData);
        }

        private async Task<bool> SendSessionEndedEventAsync(string reason)
        {
            var eventData = new SessionEndedEvent(currentUserId!, reason);
            return await SendEventAsync(eventData);
        }

        private PlatformType GetPlatformType()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return PlatformType.Android;
                case RuntimePlatform.IPhonePlayer:
                    return PlatformType.iOS;
                case RuntimePlatform.WebGLPlayer:
                    return PlatformType.Web;
                default:
                    return PlatformType.Web; // Default to Web for unsupported platforms
            }
        }

        private async Task ConnectWebSocketAsync()
        {
            try
            {
                if (websocketUrl == null) return;

                string wsUrl = $"{websocketUrl}?session_id={CurrentSessionId}";
                websocket = new WebSocket(wsUrl);

                websocket.OnOpen += () =>
                {
                    SDKLog("WebSocket connected");
                    // Send auth message
                    var authMessage = new
                    {
                        type = "auth",
                        token = jwtToken,
                        session_id = CurrentSessionId
                    };
                    websocket.SendText(JsonConvert.SerializeObject(authMessage));
                };

                websocket.OnMessage += (byte[] data) =>
                {
                    string message = Encoding.UTF8.GetString(data);
                    HandleWebSocketMessage(message);
                };

                websocket.OnError += (string error) =>
                {
                    SDKLogError($"WebSocket error: {error}");
                };

                websocket.OnClose += (WebSocketCloseCode closeCode) =>
                {
                    SDKLog($"WebSocket closed: {closeCode}");
                };

                await websocket.Connect();
            }
            catch (Exception ex)
            {
                SDKLogError($"WebSocket connection error: {ex.Message}");
            }
        }

        private void HandleWebSocketMessage(string message)
        {
            try
            {
                var wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
                if (wsMessage?.type != null && websocketHandlers.ContainsKey(wsMessage.type))
                {
                    foreach (var handler in websocketHandlers[wsMessage.type])
                    {
                        handler(message);
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"WebSocket message handling error: {ex.Message}");
            }
        }

        private void CloseWebSocket()
        {
            if (websocket != null)
            {
                _ = websocket.Close();
                websocket = null;
            }
        }

        private void SDKLog(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[DecisionBox] {message}");
            }
        }

        private void SDKLogWarning(string message)
        {
            if (enableLogging)
            {
                Debug.LogWarning($"[DecisionBox] {message}");
            }
        }

        private void SDKLogError(string message)
        {
            if (enableLogging)
            {
                Debug.LogError($"[DecisionBox] {message}");
            }
        }

        private bool IsTokenExpired()
        {
            // Check if token expires in next 5 minutes
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 300000 > tokenExpiry;
        }

        private async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var refreshRequest = new
                {
                    access_token = jwtToken
                };

                string json = JsonConvert.SerializeObject(refreshRequest);
                byte[] bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{EventApiUrl}/oauth/token/refresh", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        var response = JsonConvert.DeserializeObject<AuthResponse>(request.downloadHandler.text);
                        if (response != null)
                        {
                            jwtToken = response.access_token;
                            tokenExpiry = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (response.ExpiresInSeconds * 1000);
                            SDKLog($"Token refreshed successfully, expires in {response.ExpiresInSeconds} seconds");
                        }
                        else
                        {
                            SDKLogError("Invalid token refresh response");
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        SDKLogError($"Token refresh failed: {request.error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SDKLogError($"Token refresh error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Update Loop

        private void Update()
        {
            // Dispatch WebSocket events on main thread
            if (websocket != null)
            {
                websocket.DispatchMessageQueue();
            }

            // Check session duration
            if (sdkActive && remoteConfig != null && sessionStartTime != 0)
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (currentTime - sessionStartTime > remoteConfig.maxSessionDuration)
                {
                    SDKLog("Session duration exceeded. Starting new session");
                    EndCurrentSession("session_timeout");
                    StartNewSession();
                }
            }
        }

        #endregion
    }
}