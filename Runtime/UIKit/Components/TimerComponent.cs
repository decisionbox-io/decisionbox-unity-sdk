using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DecisionBox.Models;
using DecisionBox.UIKit.Core;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Timer component for countdown displays
    /// </summary>
    public class TimerComponent : BaseComponent, ITextComponent
    {
        private TextMeshProUGUI _textMesh;
        private float _remainingTime;
        private bool _isRunning;
        private string _format = "{0:00}:{1:00}:{2:00}"; // HH:MM:SS
        private Coroutine _timerCoroutine;
        
        public event Action OnTimerComplete;
        
        public TimerComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Add TextMeshPro component
            _textMesh = _gameObject.AddComponent<TextMeshProUGUI>();
            _textMesh.alignment = TextAlignmentOptions.Center;
            _textMesh.fontSize = 20;
            _textMesh.color = Color.white;
            _textMesh.raycastTarget = false;
            
            // Set initial time if provided
            if (_componentData.Properties != null &&
                _componentData.Properties.TryGetValue("duration", out var duration))
            {
                _remainingTime = Convert.ToSingle(duration);
                UpdateDisplay();
            }
        }
        
        public void SetText(string text)
        {
            // For timer, this sets the format
            _format = text;
            UpdateDisplay();
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
            
            // Handle duration
            if (properties.TryGetValue("duration", out var duration))
            {
                _remainingTime = Convert.ToSingle(duration);
                UpdateDisplay();
            }
            
            // Handle format
            if (properties.TryGetValue("format", out var format))
            {
                _format = format.ToString();
                UpdateDisplay();
            }
            
            // Handle auto start
            if (properties.TryGetValue("autoStart", out var autoStart) && 
                Convert.ToBoolean(autoStart))
            {
                StartTimer();
            }
            
            // Handle text properties
            if (properties.TryGetValue("fontSize", out var fontSize))
            {
                SetFontSize(Convert.ToSingle(fontSize));
            }
            
            if (properties.TryGetValue("color", out var color))
            {
                SetTextColor(ParseColor(color, _textMesh.color));
            }
            
            if (properties.TryGetValue("alignment", out var alignment))
            {
                SetAlignment(alignment.ToString());
            }
            
            // Handle warning threshold
            if (properties.TryGetValue("warningThreshold", out var threshold))
            {
                // Could change color when below threshold
            }
        }
        
        private void SetAlignment(string alignment)
        {
            _textMesh.alignment = alignment?.ToLower() switch
            {
                "left" => TextAlignmentOptions.Left,
                "center" => TextAlignmentOptions.Center,
                "right" => TextAlignmentOptions.Right,
                _ => TextAlignmentOptions.Center
            };
        }
        
        public void StartTimer()
        {
            if (_isRunning) return;
            
            _isRunning = true;
            _timerCoroutine = UIKitManager.Instance.StartCoroutine(TimerRoutine());
        }
        
        public void StopTimer()
        {
            _isRunning = false;
            if (_timerCoroutine != null)
            {
                UIKitManager.Instance.StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }
        
        public void ResetTimer(float duration)
        {
            StopTimer();
            _remainingTime = duration;
            UpdateDisplay();
        }
        
        private IEnumerator TimerRoutine()
        {
            while (_isRunning && _remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                UpdateDisplay();
                
                if (_remainingTime <= 0)
                {
                    _remainingTime = 0;
                    UpdateDisplay();
                    _isRunning = false;
                    OnTimerComplete?.Invoke();
                    
                    // Send timer complete action
                    var action = new ComponentAction 
                    { 
                        Type = "TimerComplete",
                        Data = new Dictionary<string, object> { { "componentId", Id } }
                    };
                    UIKitManager.Instance.HandleTemplateAction(_componentData.Id, action);
                }
                
                yield return null;
            }
        }
        
        private void UpdateDisplay()
        {
            var timeSpan = TimeSpan.FromSeconds(_remainingTime);
            
            // Try different format patterns
            try
            {
                if (_format.Contains("{0}") && _format.Contains("{1}") && _format.Contains("{2}"))
                {
                    // HH:MM:SS format
                    _textMesh.text = string.Format(_format, 
                        (int)timeSpan.TotalHours, 
                        timeSpan.Minutes, 
                        timeSpan.Seconds);
                }
                else if (_format.Contains("{0}") && _format.Contains("{1}"))
                {
                    // MM:SS format
                    _textMesh.text = string.Format(_format, 
                        (int)timeSpan.TotalMinutes, 
                        timeSpan.Seconds);
                }
                else if (_format.Contains("{0}"))
                {
                    // Seconds only
                    _textMesh.text = string.Format(_format, (int)_remainingTime);
                }
                else
                {
                    // Use format as is
                    _textMesh.text = _format.Replace("$time", timeSpan.ToString(@"hh\:mm\:ss"));
                }
            }
            catch
            {
                // Fallback to default format
                _textMesh.text = timeSpan.ToString(@"hh\:mm\:ss");
            }
        }
        
        public override void Dispose()
        {
            StopTimer();
            base.Dispose();
        }
    }
}