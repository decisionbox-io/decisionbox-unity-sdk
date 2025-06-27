using System;
using UnityEngine;

namespace DecisionBox.Core
{
    /// <summary>
    /// Configuration settings for DecisionBox SDK
    /// </summary>
    [Serializable]
    public class DecisionBoxConfig
    {
        [Header("Authentication")]
        [Tooltip("Your DecisionBox application ID")]
        public string AppId;
        
        [Tooltip("Your DecisionBox application secret")]
        public string AppSecret;
        
        [Header("API Settings")]
        [Tooltip("DecisionBox API URL")]
        public string ApiUrl = "https://api.decisionbox.io";
        
        [Tooltip("Request timeout in milliseconds")]
        public int TimeoutMs = 30000;
        
        [Header("Debug Settings")]
        [Tooltip("Enable debug logging")]
        public bool EnableDebugLogs = false;
        
        [Tooltip("Enable detailed request/response logging")]
        public bool EnableVerboseLogging = false;

        [Header("Session Settings")]
        [Tooltip("Automatically end session when app loses focus")]
        public bool AutoEndSessionOnFocusLost = false;
        
        [Tooltip("Session timeout in minutes (0 = no timeout)")]
        public int SessionTimeoutMinutes = 0;

        [Header("Event Batching")]
        [Tooltip("Enable event batching for better performance")]
        public bool EnableEventBatching = true;
        
        [Tooltip("Maximum number of events in a batch")]
        public int MaxBatchSize = 50;
        
        [Tooltip("Maximum time to wait before sending batch (seconds)")]
        public float BatchTimeoutSeconds = 30f;

        [Header("Offline Support")]
        [Tooltip("Cache events when offline")]
        public bool EnableOfflineSupport = true;
        
        [Tooltip("Maximum number of cached events")]
        public int MaxCachedEvents = 1000;

        /// <summary>
        /// Validate the configuration
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AppId) && 
                   !string.IsNullOrEmpty(AppSecret) && 
                   !string.IsNullOrEmpty(ApiUrl);
        }

        /// <summary>
        /// Get validation errors
        /// </summary>
        public string GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();
            
            if (string.IsNullOrEmpty(AppId))
                errors.Add("App ID is required");
            
            if (string.IsNullOrEmpty(AppSecret))
                errors.Add("App Secret is required");
            
            if (string.IsNullOrEmpty(ApiUrl))
                errors.Add("API URL is required");
            
            if (TimeoutMs <= 0)
                errors.Add("Timeout must be positive");
            
            if (EnableEventBatching && MaxBatchSize <= 0)
                errors.Add("Max batch size must be positive when batching is enabled");
            
            if (EnableOfflineSupport && MaxCachedEvents <= 0)
                errors.Add("Max cached events must be positive when offline support is enabled");

            return string.Join("; ", errors);
        }
    }
}