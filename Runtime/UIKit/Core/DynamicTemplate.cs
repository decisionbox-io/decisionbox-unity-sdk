using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DecisionBox.Models;
using DecisionBox.UIKit.Components;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Represents a dynamically created template from server data
    /// </summary>
    public class DynamicTemplate : IDisposable
    {
        private readonly string _templateId;
        private readonly GameObject _container;
        private readonly LayoutDimensions _layoutDimensions;
        private readonly Dictionary<string, IUIKitComponent> _components;
        
        private TemplateData _templateData;
        private Dictionary<string, object> _config;
        private bool _isVisible;
        private CanvasGroup _canvasGroup;
        
        public string TemplateId => _templateId;
        public bool IsVisible => _isVisible;
        
        public DynamicTemplate(string templateId, GameObject container, LayoutDimensions layoutDimensions)
        {
            _templateId = templateId;
            _container = container;
            _layoutDimensions = layoutDimensions;
            _components = new Dictionary<string, IUIKitComponent>();
            
            // Setup container
            SetupContainer();
        }
        
        /// <summary>
        /// Sets up the container with proper layout
        /// </summary>
        private void SetupContainer()
        {
            var rectTransform = _container.GetComponent<RectTransform>();
            
            // For percentage-based layouts, use different anchor setup
            if (_layoutDimensions.WidthPercent > 0 || _layoutDimensions.HeightPercent > 0)
            {
                // Special handling for edge-anchored layouts (TopBanner, BottomSheet, etc.)
                if (_layoutDimensions.Anchor.y >= 0.9f) // Top-anchored
                {
                    rectTransform.anchorMin = new Vector2(0, 1f - _layoutDimensions.HeightPercent);
                    rectTransform.anchorMax = new Vector2(1, 1);
                }
                else if (_layoutDimensions.Anchor.y <= 0.1f) // Bottom-anchored
                {
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, _layoutDimensions.HeightPercent);
                }
                else // Center-anchored (modals)
                {
                    rectTransform.anchorMin = new Vector2(
                        0.5f - _layoutDimensions.WidthPercent / 2f,
                        0.5f - _layoutDimensions.HeightPercent / 2f
                    );
                    rectTransform.anchorMax = new Vector2(
                        0.5f + _layoutDimensions.WidthPercent / 2f,
                        0.5f + _layoutDimensions.HeightPercent / 2f
                    );
                }
                
                // Reset offsets for percentage-based sizing
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
            else
            {
                // Fixed size layouts
                rectTransform.anchorMin = _layoutDimensions.Anchor;
                rectTransform.anchorMax = _layoutDimensions.Anchor;
                rectTransform.sizeDelta = new Vector2(_layoutDimensions.Width, _layoutDimensions.Height);
            }
            
            rectTransform.pivot = _layoutDimensions.Pivot;
            
            // Get or add CanvasGroup
            _canvasGroup = _container.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = _container.AddComponent<CanvasGroup>();
            }
            
            // Add background if needed
            AddBackgroundIfNeeded();
        }
        
        /// <summary>
        /// Adds a background panel if needed
        /// </summary>
        private void AddBackgroundIfNeeded()
        {
            // Add a semi-transparent background for modal layouts
            var layoutSizeStr = _layoutDimensions.WidthPercent < 1.0f || _layoutDimensions.HeightPercent < 1.0f 
                ? "Modal" : "FullScreen";
                
            if (layoutSizeStr.Contains("Modal"))
            {
                var bgObject = new GameObject("ModalBackground");
                bgObject.transform.SetParent(_container.transform.parent, false);
                bgObject.transform.SetSiblingIndex(_container.transform.GetSiblingIndex());
                
                var bgRect = bgObject.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.offsetMin = Vector2.zero;
                bgRect.offsetMax = Vector2.zero;
                
                var bgImage = bgObject.AddComponent<Image>();
                bgImage.color = new Color(0, 0, 0, 0.5f);
                bgImage.raycastTarget = true;
                
                // Add button to close on background click
                var bgButton = bgObject.AddComponent<Button>();
                bgButton.onClick.AddListener(() =>
                {
                    var closeAction = new ComponentAction { Type = "Close" };
                    UIKitManager.Instance.HandleTemplateAction(_templateId, closeAction);
                });
            }
        }
        
        /// <summary>
        /// Sets the template data
        /// </summary>
        public void SetTemplateData(TemplateData templateData)
        {
            _templateData = templateData;
        }
        
        /// <summary>
        /// Sets the configuration
        /// </summary>
        public void SetConfig(Dictionary<string, object> config)
        {
            _config = config ?? new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Builds the template UI from the template data
        /// </summary>
        public void Build(ComponentFactory componentFactory, AssetLoader assetLoader)
        {
            if (_templateData == null || _templateData.Components == null)
            {
                Debug.LogError($"[UIKit] No template data for template {_templateId}");
                return;
            }
            
            // Clear existing components
            ClearComponents();
            
            // Create components from template data
            foreach (var componentData in _templateData.Components)
            {
                try
                {
                    var component = componentFactory.CreateComponent(componentData, _container);
                    if (component != null)
                    {
                        _components[componentData.Id] = component;
                        
                        // Handle component actions
                        if (component is IInteractableComponent interactable)
                        {
                            interactable.OnAction += (action) =>
                            {
                                UIKitManager.Instance.HandleTemplateAction(_templateId, action);
                            };
                        }
                        
                        // Load assets if needed
                        if (!string.IsNullOrEmpty(componentData.AssetUrl) && component is IAssetComponent assetComponent)
                        {
                            Debug.Log($"[UIKit] Loading asset for {componentData.Id} from URL: {componentData.AssetUrl}");
                            assetLoader.LoadTexture(componentData.AssetUrl, (texture, error) =>
                            {
                                if (texture != null)
                                {
                                    Debug.Log($"[UIKit] Successfully loaded texture for {componentData.Id}: {texture.width}x{texture.height}");
                                    assetComponent.SetAsset(texture);
                                }
                                else
                                {
                                    Debug.LogError($"[UIKit] Failed to load asset for {componentData.Id}: {error}");
                                }
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[UIKit] Failed to create component {componentData.Id}: {e.Message}");
                }
            }
            
            // Apply theme if provided
            ApplyTheme();
        }
        
        /// <summary>
        /// Updates the template with new message data
        /// </summary>
        public void UpdateWithMessage(WebSocketMessage message)
        {
            if (message.TemplateData != null)
            {
                SetTemplateData(message.TemplateData);
            }
            
            if (message.TemplateConfig != null)
            {
                SetConfig(message.TemplateConfig);
            }
            
            // Rebuild the template
            Build(UIKitManager.Instance.ComponentFactory, UIKitManager.Instance.AssetLoader);
        }
        
        /// <summary>
        /// Applies theme configuration
        /// </summary>
        private void ApplyTheme()
        {
            if (_templateData?.Theme == null) return;
            
            // Apply theme to components
            foreach (var component in _components.Values)
            {
                component.ApplyTheme(_templateData.Theme);
            }
        }
        
        /// <summary>
        /// Shows the template
        /// </summary>
        public void Show()
        {
            _container.SetActive(true);
            _isVisible = true;
            
            // Animate in
            _canvasGroup.alpha = 0f;
            UIKitManager.Instance.StartCoroutine(
                AnimationHelper.FadeIn(_canvasGroup, 0.3f)
            );
        }
        
        /// <summary>
        /// Hides the template
        /// </summary>
        public void Hide(Action onComplete = null)
        {
            if (!_isVisible) 
            {
                onComplete?.Invoke();
                return;
            }
            
            _isVisible = false;
            
            // Animate out
            UIKitManager.Instance.StartCoroutine(
                AnimationHelper.FadeOut(_canvasGroup, 0.3f, () =>
                {
                    _container.SetActive(false);
                    onComplete?.Invoke();
                })
            );
        }
        
        /// <summary>
        /// Gets a component by ID
        /// </summary>
        public IUIKitComponent GetComponent(string componentId)
        {
            _components.TryGetValue(componentId, out var component);
            return component;
        }
        
        /// <summary>
        /// Clears all components
        /// </summary>
        private void ClearComponents()
        {
            foreach (var component in _components.Values)
            {
                component.Dispose();
            }
            _components.Clear();
            
            // Clear container children
            foreach (Transform child in _container.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// Disposes the template
        /// </summary>
        public void Dispose()
        {
            ClearComponents();
            
            if (_container != null)
            {
                UnityEngine.Object.Destroy(_container);
            }
        }
    }
}