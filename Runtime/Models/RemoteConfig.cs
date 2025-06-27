using Newtonsoft.Json;

#nullable enable

namespace DecisionBox.Models
{
    /// <summary>
    /// Remote configuration fetched from the API
    /// </summary>
    [System.Serializable]
    public class RemoteConfig
    {
        // Match actual API response format
        [JsonProperty("enabled")]
        public bool enabled { get; set; } = true;

        [JsonProperty("max_session_duration")]
        public int max_session_duration { get; set; } = 30; // in minutes

        [JsonProperty("video_recording_enabled")]
        public bool video_recording_enabled { get; set; } = false;

        // Additional SDK settings with defaults (not from API)
        [JsonIgnoreAttribute]
        public bool websocketEnabled { get; set; } = true;

        [JsonIgnoreAttribute]
        public string? websocketUrl { get; set; } = "wss://eventapi.dev.decisionbox.io/ws";

        [JsonIgnoreAttribute]
        public long maxBackgroundDuration { get; set; } = 300000; // 5 minutes in milliseconds

        [JsonIgnoreAttribute]
        public int eventBatchSize { get; set; } = 10;

        [JsonIgnoreAttribute]
        public long eventFlushInterval { get; set; } = 30000; // 30 seconds in milliseconds

        [JsonIgnoreAttribute]
        public bool enableLogging { get; set; } = false;

        [JsonIgnoreAttribute]
        public int apiTimeout { get; set; } = 30000; // 30 seconds in milliseconds

        [JsonIgnoreAttribute]
        public int retryAttempts { get; set; } = 3;

        [JsonIgnoreAttribute]
        public int retryDelayMs { get; set; } = 1000; // 1 second

        // Computed properties for SDK compatibility
        [JsonIgnoreAttribute]
        public bool sdkEnabled => enabled;

        [JsonIgnoreAttribute]
        public long maxSessionDuration => max_session_duration * 60 * 1000; // Convert minutes to milliseconds
    }

    /// <summary>
    /// Authentication response from the API
    /// </summary>
    [System.Serializable]
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; } = "";

        [JsonProperty("token_type")]
        public string? token_type { get; set; }

        [JsonProperty("expires_in")]
        public string expires_in { get; set; } = "3600"; // API returns this as string timestamp

        [JsonProperty("scope")]
        public string? scope { get; set; }
        
        /// <summary>
        /// Get expires_in as seconds until expiry
        /// </summary>
        [JsonIgnoreAttribute]
        public int ExpiresInSeconds
        {
            get
            {
                // If expires_in is a timestamp, calculate seconds until expiry
                if (long.TryParse(expires_in, out long timestamp))
                {
                    if (timestamp > 1000000000000) // Likely a millisecond timestamp
                    {
                        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        return (int)((timestamp - currentTime) / 1000);
                    }
                    else if (timestamp > 1000000000) // Likely a second timestamp
                    {
                        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        return (int)(timestamp - currentTime);
                    }
                    else // Already in seconds
                    {
                        return (int)timestamp;
                    }
                }
                return 3600; // Default to 1 hour
            }
        }
    }

    /// <summary>
    /// WebSocket message structure
    /// </summary>
    [System.Serializable]
    public class WebSocketMessage
    {
        [JsonProperty("type")]
        public string? type { get; set; }

        [JsonProperty("data")]
        public object? data { get; set; }

        [JsonProperty("timestamp")]
        public long timestamp { get; set; }

        [JsonProperty("session_id")]
        public string? session_id { get; set; }
    }

    /// <summary>
    /// API error response
    /// </summary>
    [System.Serializable]
    public class ApiErrorResponse
    {
        [JsonProperty("error")]
        public string? error { get; set; }

        [JsonProperty("error_description")]
        public string? error_description { get; set; }

        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("message")]
        public string? message { get; set; }
    }
}