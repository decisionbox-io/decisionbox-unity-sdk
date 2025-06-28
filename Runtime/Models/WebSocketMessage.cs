using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable


namespace DecisionBox.Models
{
    /// <summary>
    /// WebSocket message structure for DecisionBox real-time communication
    /// </summary>
    [Serializable]
    public class WebSocketMessage
    {
        /// <summary>
        /// The type/action of the WebSocket message
        /// </summary>
        [JsonProperty("type")]
        public string type { get; set; } = "";

        /// <summary>
        /// Alternative property name for action (legacy support)
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; } = "";

        /// <summary>
        /// The app ID associated with this message
        /// </summary>
        [JsonProperty("app_id")]
        public string AppId { get; set; } = "";

        /// <summary>
        /// The session ID associated with this message
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; } = "";

        /// <summary>
        /// DecisionBox ruleset ID if applicable
        /// </summary>
        [JsonProperty("DECISIONBOX_RS_ID")]
        public string DecisionBoxRsId { get; set; } = "";

        /// <summary>
        /// Message payload containing custom data
        /// </summary>
        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Timestamp of when the message was created
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Any error message if applicable
        /// </summary>
        [JsonProperty("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Success status of the message
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; } = true;
    }
}