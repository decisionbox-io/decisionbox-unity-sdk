using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using DecisionBox.Core;
using DecisionBox.Models;

namespace DecisionBox.Auth
{
    /// <summary>
    /// Interface for authentication management
    /// </summary>
    public interface IAuthManager
    {
        Task AuthenticateAsync();
        Task<string> GetAccessTokenAsync();
        string GetAuthorizationHeader();
    }

    /// <summary>
    /// Handles OAuth authentication for Unity
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly DecisionBoxConfig _config;
        private string _accessToken;
        private string _refreshToken;
        private DateTime _tokenExpiry;

        public AuthManager(DecisionBoxConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Authenticate with DecisionBox API
        /// </summary>
        public async Task AuthenticateAsync()
        {
            var authRequest = new AuthRequest
            {
                GrantType = "client_credentials",
                AppId = _config.AppId,
                AppSecret = _config.AppSecret
            };

            var json = JsonConvert.SerializeObject(authRequest);
            var bodyData = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest($"{_config.ApiUrl}/oauth/token", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = _config.TimeoutMs / 1000;

                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var responseText = request.downloadHandler.text;
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseText);
                    
                    _accessToken = authResponse.AccessToken;
                    _refreshToken = authResponse.RefreshToken;
                    _tokenExpiry = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn - 60); // Refresh 1 min early

                    if (_config.EnableDebugLogs)
                        Debug.Log("[DecisionBox] Authentication successful");
                }
                else
                {
                    var error = $"Authentication failed: {request.error}";
                    if (_config.EnableDebugLogs)
                        Debug.LogError($"[DecisionBox] {error}");
                    
                    throw new Exception(error);
                }
            }
        }

        /// <summary>
        /// Get a valid access token, refreshing if necessary
        /// </summary>
        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
            {
                if (!string.IsNullOrEmpty(_refreshToken))
                {
                    try
                    {
                        await RefreshTokenAsync();
                    }
                    catch
                    {
                        // If refresh fails, re-authenticate
                        await AuthenticateAsync();
                    }
                }
                else
                {
                    await AuthenticateAsync();
                }
            }

            return _accessToken;
        }

        /// <summary>
        /// Get authorization header value
        /// </summary>
        public string GetAuthorizationHeader()
        {
            if (string.IsNullOrEmpty(_accessToken))
                throw new InvalidOperationException("Not authenticated. Call AuthenticateAsync first.");

            return $"Bearer {_accessToken}";
        }

        /// <summary>
        /// Refresh the access token using refresh token
        /// </summary>
        private async Task RefreshTokenAsync()
        {
            var refreshRequest = new RefreshTokenRequest
            {
                GrantType = "refresh_token",
                RefreshToken = _refreshToken
            };

            var json = JsonConvert.SerializeObject(refreshRequest);
            var bodyData = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest($"{_config.ApiUrl}/oauth/token/refresh", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = _config.TimeoutMs / 1000;

                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var responseText = request.downloadHandler.text;
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseText);
                    
                    _accessToken = authResponse.AccessToken;
                    if (!string.IsNullOrEmpty(authResponse.RefreshToken))
                        _refreshToken = authResponse.RefreshToken;
                    _tokenExpiry = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn - 60);

                    if (_config.EnableDebugLogs)
                        Debug.Log("[DecisionBox] Token refreshed successfully");
                }
                else
                {
                    throw new Exception($"Token refresh failed: {request.error}");
                }
            }
        }
    }

    /// <summary>
    /// Authentication request model
    /// </summary>
    [Serializable]
    public class AuthRequest
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        
        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }
    }

    /// <summary>
    /// Refresh token request model
    /// </summary>
    [Serializable]
    public class RefreshTokenRequest
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Authentication response model
    /// </summary>
    [Serializable]
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}