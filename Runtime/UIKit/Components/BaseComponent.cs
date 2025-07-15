using System;
using System.Collections.Generic;
using UnityEngine;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Base implementation for UIKit components
    /// </summary>
    public abstract class BaseComponent : IUIKitComponent
    {
        protected readonly string _id;
        protected readonly GameObject _gameObject;
        protected readonly RectTransform _rectTransform;
        protected readonly UIKitComponent _componentData;
        
        public string Id => _id;
        public GameObject GameObject => _gameObject;
        public virtual bool IsVisible => _gameObject.activeSelf;
        
        protected BaseComponent(UIKitComponent componentData, GameObject parent)
        {
            _componentData = componentData;
            _id = componentData.Id;
            
            // Create GameObject
            _gameObject = new GameObject($"UIKit_{componentData.Type}_{_id}");
            _gameObject.transform.SetParent(parent.transform, false);
            
            // Add RectTransform
            _rectTransform = _gameObject.AddComponent<RectTransform>();
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            // Initialize component
            Initialize();
        }
        
        /// <summary>
        /// Initialize the component
        /// </summary>
        protected abstract void Initialize();
        
        /// <summary>
        /// Applies theme settings to the component
        /// </summary>
        public virtual void ApplyTheme(Dictionary<string, object> theme)
        {
            // Base implementation - override in derived classes
        }
        
        /// <summary>
        /// Updates component properties
        /// </summary>
        public virtual void UpdateProperties(Dictionary<string, object> properties)
        {
            // Base implementation - override in derived classes
            if (properties == null) return;
            
            // Handle common properties
            if (properties.TryGetValue("visible", out var visible))
            {
                SetVisible(Convert.ToBoolean(visible));
            }
            
            if (properties.TryGetValue("opacity", out var opacity))
            {
                SetOpacity(Convert.ToSingle(opacity));
            }
        }
        
        /// <summary>
        /// Sets the component's visibility
        /// </summary>
        public virtual void SetVisible(bool visible)
        {
            _gameObject.SetActive(visible);
        }
        
        /// <summary>
        /// Sets the component's opacity
        /// </summary>
        protected virtual void SetOpacity(float opacity)
        {
            var canvasGroup = _gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = _gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = Mathf.Clamp01(opacity);
        }
        
        /// <summary>
        /// Disposes the component
        /// </summary>
        public virtual void Dispose()
        {
            if (_gameObject != null)
            {
                UnityEngine.Object.Destroy(_gameObject);
            }
        }
        
        /// <summary>
        /// Helper method to parse color from various formats
        /// </summary>
        protected Color ParseColor(object colorValue, Color defaultColor = default)
        {
            if (colorValue == null) return defaultColor;
            
            if (colorValue is string colorString)
            {
                // Handle hex colors
                if (colorString.StartsWith("#"))
                {
                    if (ColorUtility.TryParseHtmlString(colorString, out var color))
                    {
                        return color;
                    }
                }
                // Handle named colors
                else if (Enum.TryParse<KnownColor>(colorString, true, out var knownColor))
                {
                    return GetKnownColor(knownColor);
                }
            }
            else if (colorValue is Dictionary<string, object> colorDict)
            {
                // Handle RGB/RGBA dictionary
                float r = GetFloatValue(colorDict, "r", 0);
                float g = GetFloatValue(colorDict, "g", 0);
                float b = GetFloatValue(colorDict, "b", 0);
                float a = GetFloatValue(colorDict, "a", 1);
                return new Color(r, g, b, a);
            }
            
            return defaultColor;
        }
        
        /// <summary>
        /// Helper method to get float value from dictionary
        /// </summary>
        protected float GetFloatValue(Dictionary<string, object> dict, string key, float defaultValue = 0)
        {
            if (dict != null && dict.TryGetValue(key, out var value))
            {
                return Convert.ToSingle(value);
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Helper method to get string value from dictionary
        /// </summary>
        protected string GetStringValue(Dictionary<string, object> dict, string key, string defaultValue = "")
        {
            if (dict != null && dict.TryGetValue(key, out var value))
            {
                return value?.ToString() ?? defaultValue;
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Helper method to get bool value from dictionary
        /// </summary>
        protected bool GetBoolValue(Dictionary<string, object> dict, string key, bool defaultValue = false)
        {
            if (dict != null && dict.TryGetValue(key, out var value))
            {
                return Convert.ToBoolean(value);
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Gets a known color value
        /// </summary>
        private Color GetKnownColor(KnownColor color)
        {
            return color switch
            {
                KnownColor.White => Color.white,
                KnownColor.Black => Color.black,
                KnownColor.Red => Color.red,
                KnownColor.Green => Color.green,
                KnownColor.Blue => Color.blue,
                KnownColor.Yellow => Color.yellow,
                KnownColor.Cyan => Color.cyan,
                KnownColor.Magenta => Color.magenta,
                KnownColor.Gray => Color.gray,
                KnownColor.Clear => Color.clear,
                _ => Color.white
            };
        }
        
        private enum KnownColor
        {
            White, Black, Red, Green, Blue, Yellow, Cyan, Magenta, Gray, Clear
        }
    }
}