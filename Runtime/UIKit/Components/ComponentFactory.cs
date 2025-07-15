using System;
using UnityEngine;
using DecisionBox.Models;
using DecisionBox.UIKit.Core;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Factory for creating UIKit components
    /// </summary>
    public class ComponentFactory
    {
        private readonly LayoutManager _layoutManager;
        
        public ComponentFactory()
        {
            _layoutManager = UIKitManager.Instance.LayoutManager;
        }
        
        /// <summary>
        /// Creates a component from component data
        /// </summary>
        public IUIKitComponent CreateComponent(UIKitComponent componentData, GameObject parent)
        {
            if (componentData == null || string.IsNullOrEmpty(componentData.Type))
            {
                Debug.LogError("[UIKit ComponentFactory] Invalid component data");
                return null;
            }
            
            // Parse component type
            if (!Enum.TryParse<ComponentType>(componentData.Type, true, out var componentType))
            {
                Debug.LogWarning($"[UIKit ComponentFactory] Unknown component type: {componentData.Type}");
                return null;
            }
            
            // Create component based on type
            IUIKitComponent component = componentType switch
            {
                ComponentType.Image => new ImageComponent(componentData, parent),
                ComponentType.Text => new TextComponent(componentData, parent),
                ComponentType.Button => new ButtonComponent(componentData, parent),
                ComponentType.Container => new ContainerComponent(componentData, parent),
                ComponentType.Timer => new TimerComponent(componentData, parent),
                ComponentType.ProgressBar => new ProgressBarComponent(componentData, parent),
                ComponentType.Spacer => new SpacerComponent(componentData, parent),
                _ => null
            };
            
            if (component == null)
            {
                Debug.LogWarning($"[UIKit ComponentFactory] Component type not yet implemented: {componentType}");
                return null;
            }
            
            // Apply common properties
            ApplyCommonProperties(component, componentData);
            
            // Apply size constraints
            ApplySizeConstraints(component, componentData);
            
            // Apply positioning
            ApplyPositioning(component, componentData);
            
            // Apply style
            if (componentData.Style != null)
            {
                component.ApplyTheme(componentData.Style);
            }
            
            // Apply properties
            if (componentData.Properties != null)
            {
                component.UpdateProperties(componentData.Properties);
            }
            
            // Handle container children
            if (component is IContainerComponent container && componentData.Children != null)
            {
                foreach (var childData in componentData.Children)
                {
                    var childComponent = CreateComponent(childData, component.GameObject);
                    if (childComponent != null)
                    {
                        container.AddChild(childComponent);
                    }
                }
            }
            
            return component;
        }
        
        /// <summary>
        /// Applies common properties to a component
        /// </summary>
        private void ApplyCommonProperties(IUIKitComponent component, UIKitComponent componentData)
        {
            // Set visibility
            if (componentData.Properties != null && 
                componentData.Properties.TryGetValue("visible", out var visible))
            {
                component.SetVisible(Convert.ToBoolean(visible));
            }
            
            // Set name
            component.GameObject.name = $"UIKit_{componentData.Type}_{componentData.Id}";
        }
        
        /// <summary>
        /// Applies size constraints to a component
        /// </summary>
        private void ApplySizeConstraints(IUIKitComponent component, UIKitComponent componentData)
        {
            if (string.IsNullOrEmpty(componentData.Size))
                return;
                
            var dimensions = _layoutManager.GetComponentDimensions(componentData.Size, componentData.Type);
            var rectTransform = component.GameObject.GetComponent<RectTransform>();
            
            if (dimensions.FillContainer)
            {
                // Fill container
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = new Vector2(dimensions.Padding, dimensions.Padding);
                rectTransform.offsetMax = new Vector2(-dimensions.Padding, -dimensions.Padding);
            }
            else if (dimensions.ResponsiveToContainer && dimensions.AspectRatio > 0)
            {
                // Maintain aspect ratio
                var aspectRatioFitter = component.GameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();
                aspectRatioFitter.aspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;
                aspectRatioFitter.aspectRatio = dimensions.AspectRatio;
            }
            else if (dimensions.ImageSize > 0)
            {
                // Fixed size
                rectTransform.sizeDelta = new Vector2(dimensions.ImageSize, dimensions.ImageSize);
            }
            
            // Apply padding
            if (dimensions.Padding > 0 && component is IContainerComponent)
            {
                var padding = component.GameObject.AddComponent<UnityEngine.UI.LayoutGroup>();
                if (padding is UnityEngine.UI.HorizontalOrVerticalLayoutGroup hvLayout)
                {
                    hvLayout.padding = new RectOffset(
                        (int)dimensions.Padding,
                        (int)dimensions.Padding,
                        (int)dimensions.Padding,
                        (int)dimensions.Padding
                    );
                }
            }
        }
        
        /// <summary>
        /// Applies positioning to a component
        /// </summary>
        private void ApplyPositioning(IUIKitComponent component, UIKitComponent componentData)
        {
            var rectTransform = component.GameObject.GetComponent<RectTransform>();
            
            if (componentData.Position != null)
            {
                // Apply position
                rectTransform.anchoredPosition = new Vector2(
                    componentData.Position.X,
                    componentData.Position.Y
                );
                
                // Apply alignment
                if (!string.IsNullOrEmpty(componentData.Position.Alignment))
                {
                    if (Enum.TryParse<Alignment>(componentData.Position.Alignment, true, out var alignment))
                    {
                        var anchor = LayoutManager.GetAnchor(alignment);
                        rectTransform.anchorMin = anchor;
                        rectTransform.anchorMax = anchor;
                        rectTransform.pivot = anchor;
                    }
                }
                
                // Apply anchor
                if (!string.IsNullOrEmpty(componentData.Position.Anchor))
                {
                    // Parse anchor string (e.g., "0.5,0.5")
                    var parts = componentData.Position.Anchor.Split(',');
                    if (parts.Length == 2 && 
                        float.TryParse(parts[0], out var x) && 
                        float.TryParse(parts[1], out var y))
                    {
                        rectTransform.anchorMin = new Vector2(x, y);
                        rectTransform.anchorMax = new Vector2(x, y);
                    }
                }
            }
        }
    }
}