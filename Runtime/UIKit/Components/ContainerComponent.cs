using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Container component for grouping and laying out child components
    /// </summary>
    public class ContainerComponent : BaseComponent, IContainerComponent
    {
        private readonly List<IUIKitComponent> _children;
        private LayoutGroup _layoutGroup;
        private Image _backgroundImage;
        
        public ContainerComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
            _children = new List<IUIKitComponent>();
        }
        
        protected override void Initialize()
        {
            // Optionally add background
            if (_componentData.Properties != null && 
                _componentData.Properties.ContainsKey("backgroundColor"))
            {
                _backgroundImage = _gameObject.AddComponent<Image>();
                _backgroundImage.raycastTarget = false;
            }
            
            // Set up layout based on properties
            SetupLayout();
        }
        
        private void SetupLayout()
        {
            if (_componentData.Properties == null) return;
            
            var layoutType = GetStringValue(_componentData.Properties, "layout", "vertical");
            
            switch (layoutType.ToLower())
            {
                case "horizontal":
                    SetupHorizontalLayout();
                    break;
                    
                case "vertical":
                    SetupVerticalLayout();
                    break;
                    
                case "grid":
                    SetupGridLayout();
                    break;
                    
                case "none":
                default:
                    // No automatic layout
                    break;
            }
        }
        
        private void SetupHorizontalLayout()
        {
            var layout = _gameObject.AddComponent<HorizontalLayoutGroup>();
            _layoutGroup = layout;
            
            layout.spacing = GetFloatValue(_componentData.Properties, "spacing", 10);
            layout.childAlignment = GetAlignment();
            layout.childControlWidth = GetBoolValue(_componentData.Properties, "childControlWidth", true);
            layout.childControlHeight = GetBoolValue(_componentData.Properties, "childControlHeight", true);
            layout.childForceExpandWidth = GetBoolValue(_componentData.Properties, "childForceExpandWidth", false);
            layout.childForceExpandHeight = GetBoolValue(_componentData.Properties, "childForceExpandHeight", false);
            
            ApplyLayoutPadding(layout);
        }
        
        private void SetupVerticalLayout()
        {
            var layout = _gameObject.AddComponent<VerticalLayoutGroup>();
            _layoutGroup = layout;
            
            layout.spacing = GetFloatValue(_componentData.Properties, "spacing", 10);
            layout.childAlignment = GetAlignment();
            layout.childControlWidth = GetBoolValue(_componentData.Properties, "childControlWidth", true);
            layout.childControlHeight = GetBoolValue(_componentData.Properties, "childControlHeight", true);
            layout.childForceExpandWidth = GetBoolValue(_componentData.Properties, "childForceExpandWidth", false);
            layout.childForceExpandHeight = GetBoolValue(_componentData.Properties, "childForceExpandHeight", false);
            
            ApplyLayoutPadding(layout);
        }
        
        private void SetupGridLayout()
        {
            var layout = _gameObject.AddComponent<GridLayoutGroup>();
            _layoutGroup = layout;
            
            layout.spacing = new Vector2(
                GetFloatValue(_componentData.Properties, "spacingX", 10),
                GetFloatValue(_componentData.Properties, "spacingY", 10)
            );
            
            layout.cellSize = new Vector2(
                GetFloatValue(_componentData.Properties, "cellWidth", 100),
                GetFloatValue(_componentData.Properties, "cellHeight", 100)
            );
            
            layout.childAlignment = GetAlignment();
            
            var constraint = GetStringValue(_componentData.Properties, "constraint", "flexible");
            layout.constraint = constraint.ToLower() switch
            {
                "fixedcolumncount" => GridLayoutGroup.Constraint.FixedColumnCount,
                "fixedrowcount" => GridLayoutGroup.Constraint.FixedRowCount,
                _ => GridLayoutGroup.Constraint.Flexible
            };
            
            layout.constraintCount = (int)GetFloatValue(_componentData.Properties, "constraintCount", 2);
            
            ApplyLayoutPadding(layout);
        }
        
        private TextAnchor GetAlignment()
        {
            var alignment = GetStringValue(_componentData.Properties, "childAlignment", "upperleft");
            return alignment.ToLower() switch
            {
                "upperleft" => TextAnchor.UpperLeft,
                "uppercenter" => TextAnchor.UpperCenter,
                "upperright" => TextAnchor.UpperRight,
                "middleleft" => TextAnchor.MiddleLeft,
                "middlecenter" => TextAnchor.MiddleCenter,
                "middleright" => TextAnchor.MiddleRight,
                "lowerleft" => TextAnchor.LowerLeft,
                "lowercenter" => TextAnchor.LowerCenter,
                "lowerright" => TextAnchor.LowerRight,
                _ => TextAnchor.UpperLeft
            };
        }
        
        private void ApplyLayoutPadding(LayoutGroup layout)
        {
            if (_componentData.Properties.TryGetValue("padding", out var paddingValue))
            {
                if (paddingValue is float padding)
                {
                    layout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
                }
                else if (paddingValue is Dictionary<string, object> paddingDict)
                {
                    int left = (int)GetFloatValue(paddingDict, "left", 0);
                    int right = (int)GetFloatValue(paddingDict, "right", 0);
                    int top = (int)GetFloatValue(paddingDict, "top", 0);
                    int bottom = (int)GetFloatValue(paddingDict, "bottom", 0);
                    
                    layout.padding = new RectOffset(left, right, top, bottom);
                }
            }
        }
        
        public void AddChild(IUIKitComponent child)
        {
            if (child == null) return;
            
            _children.Add(child);
            child.GameObject.transform.SetParent(_gameObject.transform, false);
            
            // Force layout rebuild
            if (_layoutGroup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }
        
        public void RemoveChild(IUIKitComponent child)
        {
            if (child == null) return;
            
            _children.Remove(child);
            child.GameObject.transform.SetParent(null);
            
            // Force layout rebuild
            if (_layoutGroup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }
        
        public IUIKitComponent[] GetChildren()
        {
            return _children.ToArray();
        }
        
        public void ClearChildren()
        {
            foreach (var child in _children.ToList())
            {
                RemoveChild(child);
                child.Dispose();
            }
            _children.Clear();
        }
        
        public override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            
            if (properties == null) return;
            
            // Handle background color
            if (properties.TryGetValue("backgroundColor", out var bgColor))
            {
                if (_backgroundImage == null)
                {
                    _backgroundImage = _gameObject.AddComponent<Image>();
                    _backgroundImage.raycastTarget = false;
                }
                _backgroundImage.color = ParseColor(bgColor, _backgroundImage.color);
            }
            
            // Handle border
            if (properties.TryGetValue("borderWidth", out var borderWidth))
            {
                // TODO: Implement border rendering
            }
            
            // Handle corner radius
            if (properties.TryGetValue("cornerRadius", out var radius))
            {
                // TODO: Implement rounded corners
            }
            
            // Handle scroll
            if (properties.TryGetValue("scrollable", out var scrollable) && 
                Convert.ToBoolean(scrollable))
            {
                EnsureScrollRect();
            }
        }
        
        private void EnsureScrollRect()
        {
            if (_gameObject.GetComponent<ScrollRect>() == null)
            {
                var scrollRect = _gameObject.AddComponent<ScrollRect>();
                
                // Create viewport
                var viewport = new GameObject("Viewport");
                viewport.transform.SetParent(_gameObject.transform, false);
                var viewportRect = viewport.AddComponent<RectTransform>();
                viewportRect.anchorMin = Vector2.zero;
                viewportRect.anchorMax = Vector2.one;
                viewportRect.offsetMin = Vector2.zero;
                viewportRect.offsetMax = Vector2.zero;
                
                viewport.AddComponent<Image>().color = Color.clear;
                viewport.AddComponent<Mask>().showMaskGraphic = false;
                
                // Create content
                var content = new GameObject("Content");
                content.transform.SetParent(viewport.transform, false);
                var contentRect = content.AddComponent<RectTransform>();
                contentRect.anchorMin = new Vector2(0, 1);
                contentRect.anchorMax = new Vector2(1, 1);
                contentRect.pivot = new Vector2(0.5f, 1);
                
                // Move children to content
                foreach (Transform child in _gameObject.transform)
                {
                    if (child != viewport.transform)
                    {
                        child.SetParent(content.transform, false);
                    }
                }
                
                // Configure scroll rect
                scrollRect.viewport = viewportRect;
                scrollRect.content = contentRect;
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                
                // Move layout group to content
                if (_layoutGroup != null)
                {
                    UnityEngine.Object.Destroy(_layoutGroup);
                    SetupLayout(); // Re-setup on content
                }
            }
        }
        
        public override void Dispose()
        {
            ClearChildren();
            base.Dispose();
        }
    }
}