using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DecisionBox.Auth;
using DecisionBox.Events;
using DecisionBox.Session;
using DecisionBox.Models;

// Alias to avoid ambiguity with UnityEngine.Event
using DBEvent = DecisionBox.Models.Event;

namespace DecisionBox.Core
{
    /// <summary>
    /// Main DecisionBox SDK client for Unity
    /// </summary>
    public class DecisionBoxClient : MonoBehaviour
    {
        [Header("DecisionBox Configuration")]
        [SerializeField] private string _appId = "";
        [SerializeField] private string _appSecret = "";
        [SerializeField] private string _apiUrl = "https://api.decisionbox.io";
        [SerializeField] private bool _enableDebugLogs = false;
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private string _defaultUserId = "";

        private DecisionBoxConfig _config;
        private IAuthManager _authManager;
        private ISessionManager _sessionManager;
        private IEventSender _eventSender;
        private bool _initialized;

        public static DecisionBoxClient Instance { get; private set; }

        public bool IsInitialized => _initialized;
        public string CurrentUserId => _sessionManager?.UserId;
        public string CurrentSessionId => _sessionManager?.SessionId;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeComponents();
                
                if (_autoInitialize && !string.IsNullOrEmpty(_defaultUserId))
                {
                    StartCoroutine(AutoInitialize());
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeComponents()
        {
            _config = new DecisionBoxConfig
            {
                AppId = _appId,
                AppSecret = _appSecret,
                ApiUrl = _apiUrl,
                EnableDebugLogs = _enableDebugLogs
            };

            _authManager = new AuthManager(_config);
            _sessionManager = new SessionManager();
            
            // EventSender is a MonoBehaviour, so it needs to be added to a GameObject
            var eventSenderObject = new GameObject("DecisionBoxEventSender");
            eventSenderObject.transform.SetParent(transform);
            _eventSender = eventSenderObject.AddComponent<EventSender>();
            _eventSender.Initialize(_config, _authManager);

            if (_enableDebugLogs)
            {
                Debug.Log("[DecisionBox] SDK components initialized");
            }
        }

        private IEnumerator AutoInitialize()
        {
            yield return new WaitForSeconds(0.1f); // Wait a frame
            yield return StartCoroutine(InitializeCoroutine(_defaultUserId));
        }

        /// <summary>
        /// Initialize the SDK with a user ID (Async)
        /// </summary>
        public async Task InitializeAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            try
            {
                if (_enableDebugLogs)
                    Debug.Log($"[DecisionBox] Initializing SDK for user: {userId}");

                await _authManager.AuthenticateAsync();
                _sessionManager.StartSession(userId);
                _initialized = true;

                await SendEventAsync(CoreEvents.SessionStarted);

                if (_enableDebugLogs)
                    Debug.Log($"[DecisionBox] SDK initialized successfully for user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DecisionBox] Failed to initialize SDK: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Initialize the SDK with a user ID (Coroutine)
        /// </summary>
        public IEnumerator InitializeCoroutine(string userId)
        {
            var task = InitializeAsync(userId);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError($"[DecisionBox] Initialization failed: {task.Exception?.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// Send an event (Async)
        /// </summary>
        public async Task SendEventAsync(string eventType, Dictionary<string, object> metadata = null)
        {
            if (!_initialized)
            {
                Debug.LogWarning("[DecisionBox] SDK not initialized. Call InitializeAsync first.");
                return;
            }

            try
            {
                var evt = new DBEvent
                {
                    UserId = _sessionManager.UserId,
                    SessionId = _sessionManager.SessionId,
                    AppId = _config.AppId,
                    EventType = eventType,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Metadata = metadata
                };

                await _eventSender.SendEventAsync(evt);

                if (_enableDebugLogs)
                    Debug.Log($"[DecisionBox] Event sent: {eventType}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DecisionBox] Failed to send event {eventType}: {ex.Message}");
            }
        }

        /// <summary>
        /// Send an event (Coroutine)
        /// </summary>
        public IEnumerator SendEventCoroutine(string eventType, Dictionary<string, object> metadata = null)
        {
            var task = SendEventAsync(eventType, metadata);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted && _enableDebugLogs)
            {
                Debug.LogError($"[DecisionBox] Event sending failed: {task.Exception?.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// Send an event with Unity-specific timing
        /// </summary>
        public void SendEvent(string eventType, Dictionary<string, object> metadata = null)
        {
            StartCoroutine(SendEventCoroutine(eventType, metadata));
        }

        /// <summary>
        /// End the current session
        /// </summary>
        public async Task EndSessionAsync()
        {
            if (!_initialized)
                return;

            try
            {
                await SendEventAsync(CoreEvents.SessionEnded);
                _sessionManager.EndSession();
                _initialized = false;

                if (_enableDebugLogs)
                    Debug.Log("[DecisionBox] Session ended");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DecisionBox] Failed to end session: {ex.Message}");
            }
        }

        /// <summary>
        /// End the current session (Coroutine)
        /// </summary>
        public IEnumerator EndSessionCoroutine()
        {
            var task = EndSessionAsync();
            yield return new WaitUntil(() => task.IsCompleted);
        }

        /// <summary>
        /// Track player level progression
        /// </summary>
        public void TrackLevelStart(int levelNumber, string levelName = null)
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = levelNumber,
                ["levelName"] = levelName ?? $"Level {levelNumber}",
                ["startTime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            SendEvent(GamingEvents.LevelStarted, metadata);
        }

        /// <summary>
        /// Track player level completion
        /// </summary>
        public void TrackLevelCompleted(int levelNumber, float completionTime, int score = 0)
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = levelNumber,
                ["completionTime"] = completionTime,
                ["score"] = score,
                ["endTime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            SendEvent(GamingEvents.LevelCompleted, metadata);
        }

        /// <summary>
        /// Track in-app purchases
        /// </summary>
        public void TrackPurchase(string itemId, string itemName, float price, string currency = "USD")
        {
            var metadata = new Dictionary<string, object>
            {
                ["itemId"] = itemId,
                ["itemName"] = itemName,
                ["price"] = price,
                ["currency"] = currency,
                ["purchaseTime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            SendEvent(GamingEvents.PurchaseMade, metadata);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_initialized)
            {
                var metadata = new Dictionary<string, object>
                {
                    ["paused"] = pauseStatus,
                    ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                SendEvent(pauseStatus ? "game_paused" : "game_resumed", metadata);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_initialized)
            {
                var metadata = new Dictionary<string, object>
                {
                    ["focused"] = hasFocus,
                    ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                SendEvent(hasFocus ? "game_focused" : "game_unfocused", metadata);
            }
        }

        private void OnDestroy()
        {
            if (_initialized && this == Instance)
            {
                StartCoroutine(EndSessionCoroutine());
            }
        }

        private void OnApplicationQuit()
        {
            if (_initialized)
            {
                // Use synchronous approach for app quit to ensure event is sent
                var task = EndSessionAsync();
                task.Wait(TimeSpan.FromSeconds(2)); // Wait max 2 seconds
            }
        }
    }
}