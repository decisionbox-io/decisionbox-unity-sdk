using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Button component for interactive elements
    /// </summary>
    public class ButtonComponent : BaseComponent, IInteractableComponent, ITextComponent, IAssetComponent
    {
        private Button _button;
        private Image _backgroundImage;
        private TextMeshProUGUI _textMesh;
        private ComponentAction _action;
        private Texture2D _backgroundTexture;
        
        public event Action<ComponentAction> OnAction;
        public bool IsInteractable => _button.interactable;
        
        public ButtonComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Add Image for background
            _backgroundImage = _gameObject.AddComponent<Image>();
            _backgroundImage.raycastTarget = true;
            
            // If we have an asset URL, prepare for image loading
            if (!string.IsNullOrEmpty(_componentData.AssetUrl))
            {
                // Set color to white so the image shows properly
                _backgroundImage.color = Color.white;
                Debug.Log($"[UIKit] Button {_componentData.Id} has asset URL, setting background color to white");
            }
            else
            {
                // Default blue background only if no asset URL
                _backgroundImage.color = new Color(0.2f, 0.6f, 1f, 1f);
            }
            
            // Add Button component
            _button = _gameObject.AddComponent<Button>();
            _button.targetGraphic = _backgroundImage;
            
            // Create text child
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(_gameObject.transform, false);
            
            var textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);
            
            _textMesh = textObject.AddComponent<TextMeshProUGUI>();
            _textMesh.alignment = TextAlignmentOptions.Center;
            _textMesh.fontSize = 16;
            _textMesh.color = Color.white;
            _textMesh.raycastTarget = false;
            
            // Set initial content
            if (!string.IsNullOrEmpty(_componentData.Content))
            {
                SetText(_componentData.Content);
            }
            
            // Set up action
            _action = _componentData.Action ?? new ComponentAction { Type = "None" };
            
            // Add click handler
            _button.onClick.AddListener(HandleClick);
            
            // Add hover effects
            AddHoverEffects();
        }
        
        private void HandleClick()
        {
            OnAction?.Invoke(_action);
        }
        
        private void AddHoverEffects()
        {
            var colors = _button.colors;
            
            // If we have an asset URL, use different hover colors to preserve the image
            if (!string.IsNullOrEmpty(_componentData.AssetUrl))
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                colors.selectedColor = colors.highlightedColor;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                colors.colorMultiplier = 1f;
                colors.fadeDuration = 0.1f;
            }
            else
            {
                // Default colors for buttons without background images
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                colors.selectedColor = colors.highlightedColor;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                colors.colorMultiplier = 1f;
                colors.fadeDuration = 0.1f;
            }
            
            _button.colors = colors;
        }
        
        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }
        
        public void SetText(string text)
        {
            _textMesh.text = text ?? "";
        }
        
        public string GetText()
        {
            return _textMesh.text;
        }
        
        public void SetTextColor(Color color)
        {
            _textMesh.color = color;
        }
        
        public void SetFontSize(float size)
        {
            _textMesh.fontSize = size;
        }
        
        public override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            
            if (properties == null) return;
            
            // Handle text
            if (properties.TryGetValue("text", out var text))
            {
                SetText(text.ToString());
            }
            
            // Handle background color (only if no background texture is set)
            if (properties.TryGetValue("backgroundColor", out var bgColor) && _backgroundTexture == null)
            {
                _backgroundImage.color = ParseColor(bgColor, _backgroundImage.color);
            }
            
            // Handle text color
            if (properties.TryGetValue("textColor", out var textColor))
            {
                SetTextColor(ParseColor(textColor, _textMesh.color));
            }
            
            // Handle font size
            if (properties.TryGetValue("fontSize", out var fontSize))
            {
                SetFontSize(Convert.ToSingle(fontSize));
            }
            
            // Handle enabled state
            if (properties.TryGetValue("enabled", out var enabled))
            {
                SetInteractable(Convert.ToBoolean(enabled));
            }
            
            // Handle corner radius
            if (properties.TryGetValue("cornerRadius", out var radius))
            {
                ApplyCornerRadius(Convert.ToSingle(radius));
            }
            
            // Handle padding
            if (properties.TryGetValue("padding", out var padding))
            {
                ApplyPadding(padding);
            }
        }
        
        public override void ApplyTheme(Dictionary<string, object> theme)
        {
            base.ApplyTheme(theme);
            
            if (theme == null) return;
            
            // Apply button-specific theme settings
            if (theme.TryGetValue("buttonColor", out var buttonColor))
            {
                _backgroundImage.color = ParseColor(buttonColor, _backgroundImage.color);
            }
            
            if (theme.TryGetValue("buttonTextColor", out var textColor))
            {
                SetTextColor(ParseColor(textColor, _textMesh.color));
            }
            
            if (theme.TryGetValue("buttonFontSize", out var fontSize))
            {
                SetFontSize(Convert.ToSingle(fontSize));
            }
            
            if (theme.TryGetValue("buttonCornerRadius", out var radius))
            {
                ApplyCornerRadius(Convert.ToSingle(radius));
            }
        }
        
        private void ApplyCornerRadius(float radius)
        {
            // TODO: Implement rounded corners using shader or 9-slice sprite
        }
        
        private void ApplyPadding(object paddingValue)
        {
            var textRect = _textMesh.GetComponent<RectTransform>();
            
            if (paddingValue is float padding)
            {
                textRect.offsetMin = new Vector2(padding, padding);
                textRect.offsetMax = new Vector2(-padding, -padding);
            }
            else if (paddingValue is Dictionary<string, object> paddingDict)
            {
                float left = GetFloatValue(paddingDict, "left", 10);
                float right = GetFloatValue(paddingDict, "right", 10);
                float top = GetFloatValue(paddingDict, "top", 5);
                float bottom = GetFloatValue(paddingDict, "bottom", 5);
                
                textRect.offsetMin = new Vector2(left, bottom);
                textRect.offsetMax = new Vector2(-right, -top);
            }
        }
        
        public void SetAsset(Texture2D texture)
        {
            if (texture == null || _backgroundImage == null) return;
            
            _backgroundTexture = texture;
            
            // Convert texture to sprite
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f // pixels per unit
            );
            
            _backgroundImage.sprite = sprite;
            _backgroundImage.type = Image.Type.Simple;
            
            // Set color to white to show the texture properly
            _backgroundImage.color = Color.white;
            
            // Ensure the image fills the button
            _backgroundImage.preserveAspect = false;
            
            // Update button colors to use white for normal state with image
            var colors = _button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            _button.colors = colors;
            
            Debug.Log($"[UIKit] Button {_componentData.Id} background image set: {texture.width}x{texture.height}, sprite: {sprite != null}");
        }
        
        public Texture2D GetAsset()
        {
            return _backgroundTexture;
        }
        
        public override void Dispose()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
            
            if (_backgroundTexture != null && _backgroundImage.sprite != null)
            {
                UnityEngine.Object.Destroy(_backgroundImage.sprite);
            }
            
            base.Dispose();
        }
    }
}