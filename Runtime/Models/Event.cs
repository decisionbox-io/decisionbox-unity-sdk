using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DecisionBox.Models
{
    /// <summary>
    /// Represents an analytics event
    /// </summary>
    [Serializable]
    public class Event
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Create a new event with current timestamp
        /// </summary>
        public static Event Create(string userId, string sessionId, string appId, string eventType, Dictionary<string, object> metadata = null)
        {
            return new Event
            {
                UserId = userId,
                SessionId = sessionId,
                AppId = appId,
                EventType = eventType,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Metadata = metadata ?? new Dictionary<string, object>()
            };
        }

        /// <summary>
        /// Add metadata to the event
        /// </summary>
        public Event WithMetadata(string key, object value)
        {
            Metadata ??= new Dictionary<string, object>();
            Metadata[key] = value;
            return this;
        }

        /// <summary>
        /// Add multiple metadata entries
        /// </summary>
        public Event WithMetadata(Dictionary<string, object> metadata)
        {
            if (metadata == null) return this;
            
            Metadata ??= new Dictionary<string, object>();
            foreach (var kvp in metadata)
            {
                Metadata[kvp.Key] = kvp.Value;
            }
            return this;
        }

        /// <summary>
        /// Convert to JSON string
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Create from JSON string
        /// </summary>
        public static Event FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Event>(json);
        }
    }

    /// <summary>
    /// Session information
    /// </summary>
    [Serializable]
    public class SessionInfo
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public PlatformType Platform { get; set; }
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public string AppVersion { get; set; }
        public Dictionary<string, object> CustomProperties { get; set; }

        public SessionInfo()
        {
            SessionId = Guid.NewGuid().ToString();
            StartTime = DateTime.UtcNow;
            Platform = GetCurrentPlatform();
            DeviceModel = SystemInfo.deviceModel;
            OperatingSystem = SystemInfo.operatingSystem;
            AppVersion = Application.version;
            CustomProperties = new Dictionary<string, object>();
        }

        private static PlatformType GetCurrentPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return PlatformType.Android;
                case RuntimePlatform.IPhonePlayer:
                    return PlatformType.iOS;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return PlatformType.Windows;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return PlatformType.MacOS;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return PlatformType.Linux;
                case RuntimePlatform.WebGLPlayer:
                    return PlatformType.WebGL;
                case RuntimePlatform.PS4:
                case RuntimePlatform.PS5:
                    return PlatformType.PlayStation;
                case RuntimePlatform.XboxOne:
                    return PlatformType.Xbox;
                case RuntimePlatform.Switch:
                    return PlatformType.Nintendo;
                default:
                    return Application.isEditor ? PlatformType.Editor : PlatformType.NotSet;
            }
        }

        public TimeSpan GetDuration()
        {
            var endTime = EndTime ?? DateTime.UtcNow;
            return endTime - StartTime;
        }
    }
}