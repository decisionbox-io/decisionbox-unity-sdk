using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Spacer component for adding space in layouts
    /// </summary>
    public class SpacerComponent : BaseComponent
    {
        private LayoutElement _layoutElement;
        
        public SpacerComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Add LayoutElement for controlling space
            _layoutElement = _gameObject.AddComponent<LayoutElement>();
            
            // Default to flexible space
            _layoutElement.flexibleWidth = 1;
            _layoutElement.flexibleHeight = 1;
        }
        
        public override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            
            if (properties == null) return;
            
            // Handle fixed width
            if (properties.TryGetValue("width", out var width))
            {
                _layoutElement.preferredWidth = System.Convert.ToSingle(width);
                _layoutElement.flexibleWidth = 0;
            }
            
            // Handle fixed height
            if (properties.TryGetValue("height", out var height))
            {
                _layoutElement.preferredHeight = System.Convert.ToSingle(height);
                _layoutElement.flexibleHeight = 0;
            }
            
            // Handle minimum width
            if (properties.TryGetValue("minWidth", out var minWidth))
            {
                _layoutElement.minWidth = System.Convert.ToSingle(minWidth);
            }
            
            // Handle minimum height
            if (properties.TryGetValue("minHeight", out var minHeight))
            {
                _layoutElement.minHeight = System.Convert.ToSingle(minHeight);
            }
            
            // Handle flexible width
            if (properties.TryGetValue("flexibleWidth", out var flexWidth))
            {
                _layoutElement.flexibleWidth = System.Convert.ToSingle(flexWidth);
            }
            
            // Handle flexible height
            if (properties.TryGetValue("flexibleHeight", out var flexHeight))
            {
                _layoutElement.flexibleHeight = System.Convert.ToSingle(flexHeight);
            }
            
            // Handle type
            if (properties.TryGetValue("type", out var type))
            {
                SetSpacerType(type.ToString());
            }
        }
        
        private void SetSpacerType(string type)
        {
            switch (type?.ToLower())
            {
                case "fixed":
                    // Use preferred width/height
                    _layoutElement.flexibleWidth = 0;
                    _layoutElement.flexibleHeight = 0;
                    break;
                    
                case "flexible":
                    // Use flexible width/height
                    _layoutElement.flexibleWidth = 1;
                    _layoutElement.flexibleHeight = 1;
                    break;
                    
                case "horizontal":
                    // Flexible horizontal, fixed vertical
                    _layoutElement.flexibleWidth = 1;
                    _layoutElement.flexibleHeight = 0;
                    _layoutElement.preferredHeight = 0;
                    break;
                    
                case "vertical":
                    // Fixed horizontal, flexible vertical
                    _layoutElement.flexibleWidth = 0;
                    _layoutElement.preferredWidth = 0;
                    _layoutElement.flexibleHeight = 1;
                    break;
            }
        }
    }
}