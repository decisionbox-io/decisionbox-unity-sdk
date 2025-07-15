using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Image component for displaying textures and sprites
    /// </summary>
    public class ImageComponent : BaseComponent, IAssetComponent
    {
        private Image _image;
        private Texture2D _currentTexture;
        private AspectRatioFitter _aspectRatioFitter;
        
        public ImageComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Add Image component
            _image = _gameObject.AddComponent<Image>();
            _image.raycastTarget = false;
            
            // Set initial content if provided
            if (!string.IsNullOrEmpty(_componentData.Content))
            {
                // Content could be a placeholder color
                _image.color = ParseColor(_componentData.Content, Color.gray);
            }
        }
        
        public void SetAsset(Texture2D texture)
        {
            if (texture == null) return;
            
            _currentTexture = texture;
            
            // Convert to sprite
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            
            _image.sprite = sprite;
            _image.preserveAspect = true;
            
            // Update aspect ratio if needed
            UpdateAspectRatio();
        }
        
        public Texture2D GetAsset()
        {
            return _currentTexture;
        }
        
        public override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            
            if (properties == null) return;
            
            // Handle scale mode
            if (properties.TryGetValue("scaleMode", out var scaleMode))
            {
                SetScaleMode(scaleMode.ToString());
            }
            
            // Handle preserve aspect
            if (properties.TryGetValue("preserveAspect", out var preserveAspect))
            {
                _image.preserveAspect = Convert.ToBoolean(preserveAspect);
            }
            
            // Handle tint color
            if (properties.TryGetValue("tint", out var tint))
            {
                _image.color = ParseColor(tint, Color.white);
            }
            
            // Handle raycast target
            if (properties.TryGetValue("raycastTarget", out var raycastTarget))
            {
                _image.raycastTarget = Convert.ToBoolean(raycastTarget);
            }
        }
        
        public override void ApplyTheme(Dictionary<string, object> theme)
        {
            base.ApplyTheme(theme);
            
            if (theme == null) return;
            
            // Apply image-specific theme settings
            if (theme.TryGetValue("imageTint", out var tint))
            {
                _image.color = ParseColor(tint, _image.color);
            }
            
            if (theme.TryGetValue("imageAlpha", out var alpha))
            {
                var color = _image.color;
                color.a = Convert.ToSingle(alpha);
                _image.color = color;
            }
        }
        
        private void SetScaleMode(string mode)
        {
            switch (mode?.ToLower())
            {
                case "fill":
                    _image.preserveAspect = false;
                    RemoveAspectRatioFitter();
                    break;
                    
                case "fit":
                    _image.preserveAspect = true;
                    RemoveAspectRatioFitter();
                    break;
                    
                case "aspectfit":
                    _image.preserveAspect = true;
                    EnsureAspectRatioFitter();
                    _aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    break;
                    
                case "aspectfill":
                    _image.preserveAspect = true;
                    EnsureAspectRatioFitter();
                    _aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                    break;
            }
        }
        
        private void UpdateAspectRatio()
        {
            if (_currentTexture != null && _aspectRatioFitter != null)
            {
                _aspectRatioFitter.aspectRatio = (float)_currentTexture.width / _currentTexture.height;
            }
        }
        
        private void EnsureAspectRatioFitter()
        {
            if (_aspectRatioFitter == null)
            {
                _aspectRatioFitter = _gameObject.AddComponent<AspectRatioFitter>();
            }
            UpdateAspectRatio();
        }
        
        private void RemoveAspectRatioFitter()
        {
            if (_aspectRatioFitter != null)
            {
                UnityEngine.Object.Destroy(_aspectRatioFitter);
                _aspectRatioFitter = null;
            }
        }
        
        public override void Dispose()
        {
            if (_currentTexture != null && _image.sprite != null)
            {
                UnityEngine.Object.Destroy(_image.sprite);
            }
            base.Dispose();
        }
    }
}