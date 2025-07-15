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

        /// <summary>
        /// Indicates whether this message should use UIKit template rendering
        /// </summary>
        [JsonProperty("useTemplate")]
        public bool UseTemplate { get; set; } = false;

        /// <summary>
        /// The layout size for UIKit templates
        /// </summary>
        [JsonProperty("layoutSize")]
        public string? LayoutSize { get; set; }

        /// <summary>
        /// Template data containing component definitions for UIKit
        /// </summary>
        [JsonProperty("templateData")]
        public TemplateData? TemplateData { get; set; }

        /// <summary>
        /// Configuration overrides for the template
        /// </summary>
        [JsonProperty("templateConfig")]
        public Dictionary<string, object>? TemplateConfig { get; set; }
    }

    /// <summary>
    /// Template data structure for UIKit dynamic templates
    /// </summary>
    [Serializable]
    public class TemplateData
    {
        /// <summary>
        /// Array of UI components to render
        /// </summary>
        [JsonProperty("components")]
        public List<UIKitComponent> Components { get; set; } = new List<UIKitComponent>();

        /// <summary>
        /// Theme configuration for the template
        /// </summary>
        [JsonProperty("theme")]
        public Dictionary<string, object>? Theme { get; set; }

        /// <summary>
        /// Animation settings for the template
        /// </summary>
        [JsonProperty("animations")]
        public Dictionary<string, object>? Animations { get; set; }
    }

    /// <summary>
    /// Base class for UIKit components in templates
    /// </summary>
    [Serializable]
    public class UIKitComponent
    {
        /// <summary>
        /// Component type (image, text, button, container, etc.)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        /// <summary>
        /// Unique identifier for the component
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = "";

        /// <summary>
        /// Size constraint for the component
        /// </summary>
        [JsonProperty("size")]
        public string? Size { get; set; }

        /// <summary>
        /// Position configuration
        /// </summary>
        [JsonProperty("position")]
        public ComponentPosition? Position { get; set; }

        /// <summary>
        /// Component-specific properties
        /// </summary>
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Style overrides for the component
        /// </summary>
        [JsonProperty("style")]
        public Dictionary<string, object>? Style { get; set; }

        /// <summary>
        /// Asset URL for components that need remote assets
        /// </summary>
        [JsonProperty("assetUrl")]
        public string? AssetUrl { get; set; }

        /// <summary>
        /// Content for text-based components
        /// </summary>
        [JsonProperty("content")]
        public string? Content { get; set; }

        /// <summary>
        /// Action to perform when component is interacted with
        /// </summary>
        [JsonProperty("action")]
        public ComponentAction? Action { get; set; }

        /// <summary>
        /// Child components for container types
        /// </summary>
        [JsonProperty("children")]
        public List<UIKitComponent>? Children { get; set; }
    }

    /// <summary>
    /// Position configuration for components
    /// </summary>
    [Serializable]
    public class ComponentPosition
    {
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("alignment")]
        public string? Alignment { get; set; }

        [JsonProperty("anchor")]
        public string? Anchor { get; set; }
    }

    /// <summary>
    /// Action configuration for interactive components
    /// </summary>
    [Serializable]
    public class ComponentAction
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("data")]
        public Dictionary<string, object>? Data { get; set; }
    }
}