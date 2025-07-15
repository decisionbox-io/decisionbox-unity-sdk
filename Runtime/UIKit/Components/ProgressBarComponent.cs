using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DecisionBox.Models;
using DecisionBox.UIKit.Core;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Progress bar component for showing progress or loading states
    /// </summary>
    public class ProgressBarComponent : BaseComponent
    {
        private Slider _slider;
        private Image _backgroundImage;
        private Image _fillImage;
        private TextMeshProUGUI _labelText;
        private float _targetValue;
        private float _animationSpeed = 1f;
        private bool _showLabel = false;
        
        public float Value => _slider.value;
        
        public ProgressBarComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Create background
            _backgroundImage = _gameObject.AddComponent<Image>();
            _backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            _backgroundImage.raycastTarget = false;
            
            // Create fill area
            var fillAreaObject = new GameObject("Fill Area");
            fillAreaObject.transform.SetParent(_gameObject.transform, false);
            var fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 5);
            fillAreaRect.offsetMax = new Vector2(-5, -5);
            
            // Create fill
            var fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(fillAreaObject.transform, false);
            var fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            
            _fillImage = fillObject.AddComponent<Image>();
            _fillImage.color = new Color(0.2f, 0.6f, 1f, 1f);
            _fillImage.raycastTarget = false;
            
            // Add Slider component
            _slider = _gameObject.AddComponent<Slider>();
            _slider.interactable = false;
            _slider.fillRect = fillRect;
            _slider.direction = Slider.Direction.LeftToRight;
            _slider.minValue = 0;
            _slider.maxValue = 1;
            _slider.value = 0;
            
            // Set default size
            _rectTransform.sizeDelta = new Vector2(200, 20);
        }
        
        public override void UpdateProperties(Dictionary<string, object> properties)
        {
            base.UpdateProperties(properties);
            
            if (properties == null) return;
            
            // Handle value
            if (properties.TryGetValue("value", out var value))
            {
                SetValue(Convert.ToSingle(value));
            }
            
            // Handle min/max
            if (properties.TryGetValue("min", out var min))
            {
                _slider.minValue = Convert.ToSingle(min);
            }
            
            if (properties.TryGetValue("max", out var max))
            {
                _slider.maxValue = Convert.ToSingle(max);
            }
            
            // Handle colors
            if (properties.TryGetValue("backgroundColor", out var bgColor))
            {
                _backgroundImage.color = ParseColor(bgColor, _backgroundImage.color);
            }
            
            if (properties.TryGetValue("fillColor", out var fillColor))
            {
                _fillImage.color = ParseColor(fillColor, _fillImage.color);
            }
            
            // Handle animation
            if (properties.TryGetValue("animated", out var animated))
            {
                _animationSpeed = Convert.ToBoolean(animated) ? 1f : 1000f;
            }
            
            if (properties.TryGetValue("animationSpeed", out var speed))
            {
                _animationSpeed = Convert.ToSingle(speed);
            }
            
            // Handle label
            if (properties.TryGetValue("showLabel", out var showLabel))
            {
                _showLabel = Convert.ToBoolean(showLabel);
                UpdateLabel();
            }
            
            if (properties.TryGetValue("labelFormat", out var format))
            {
                UpdateLabel();
            }
            
            // Handle direction
            if (properties.TryGetValue("direction", out var direction))
            {
                SetDirection(direction.ToString());
            }
            
            // Handle height
            if (properties.TryGetValue("height", out var height))
            {
                var size = _rectTransform.sizeDelta;
                size.y = Convert.ToSingle(height);
                _rectTransform.sizeDelta = size;
            }
        }
        
        private void SetValue(float value)
        {
            _targetValue = Mathf.Clamp(value, _slider.minValue, _slider.maxValue);
            
            if (_animationSpeed >= 1000f)
            {
                // Instant update
                _slider.value = _targetValue;
                UpdateLabel();
            }
            else
            {
                // Animated update
                UIKitManager.Instance.StartCoroutine(AnimateValue());
            }
        }
        
        private System.Collections.IEnumerator AnimateValue()
        {
            float startValue = _slider.value;
            float elapsed = 0;
            
            while (elapsed < 1f / _animationSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed * _animationSpeed;
                _slider.value = Mathf.Lerp(startValue, _targetValue, t);
                UpdateLabel();
                yield return null;
            }
            
            _slider.value = _targetValue;
            UpdateLabel();
        }
        
        private void SetDirection(string direction)
        {
            _slider.direction = direction?.ToLower() switch
            {
                "righttoleft" => Slider.Direction.RightToLeft,
                "bottomtotop" => Slider.Direction.BottomToTop,
                "toptobottom" => Slider.Direction.TopToBottom,
                _ => Slider.Direction.LeftToRight
            };
        }
        
        private void UpdateLabel()
        {
            if (!_showLabel)
            {
                if (_labelText != null)
                {
                    _labelText.gameObject.SetActive(false);
                }
                return;
            }
            
            // Create label if needed
            if (_labelText == null)
            {
                var labelObject = new GameObject("Label");
                labelObject.transform.SetParent(_gameObject.transform, false);
                
                var labelRect = labelObject.AddComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
                
                _labelText = labelObject.AddComponent<TextMeshProUGUI>();
                _labelText.alignment = TextAlignmentOptions.Center;
                _labelText.fontSize = 12;
                _labelText.color = Color.white;
                _labelText.raycastTarget = false;
            }
            
            _labelText.gameObject.SetActive(true);
            
            // Update label text
            float percentage = (_slider.value - _slider.minValue) / (_slider.maxValue - _slider.minValue) * 100;
            
            var format = GetStringValue(_componentData.Properties, "labelFormat", "{0:0}%");
            try
            {
                _labelText.text = string.Format(format, percentage);
            }
            catch
            {
                _labelText.text = $"{percentage:0}%";
            }
        }
        
        public override void ApplyTheme(Dictionary<string, object> theme)
        {
            base.ApplyTheme(theme);
            
            if (theme == null) return;
            
            if (theme.TryGetValue("progressBackgroundColor", out var bgColor))
            {
                _backgroundImage.color = ParseColor(bgColor, _backgroundImage.color);
            }
            
            if (theme.TryGetValue("progressFillColor", out var fillColor))
            {
                _fillImage.color = ParseColor(fillColor, _fillImage.color);
            }
        }
    }
}