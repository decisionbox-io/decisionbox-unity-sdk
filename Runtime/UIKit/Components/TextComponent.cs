using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DecisionBox.Models;

namespace DecisionBox.UIKit.Components
{
    /// <summary>
    /// Text component for displaying text content
    /// </summary>
    public class TextComponent : BaseComponent, ITextComponent
    {
        private TextMeshProUGUI _textMesh;
        
        public TextComponent(UIKitComponent componentData, GameObject parent) 
            : base(componentData, parent)
        {
        }
        
        protected override void Initialize()
        {
            // Add TextMeshPro component
            _textMesh = _gameObject.AddComponent<TextMeshProUGUI>();
            _textMesh.raycastTarget = false;
            
            // Set default properties
            _textMesh.alignment = TextAlignmentOptions.Center;
            _textMesh.fontSize = 16;
            _textMesh.color = Color.white;
            
            // Set initial content
            if (!string.IsNullOrEmpty(_componentData.Content))
            {
                SetText(_componentData.Content);
            }
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
            
            // Handle text content
            if (properties.TryGetValue("text", out var text))
            {
                SetText(text.ToString());
            }
            
            // Handle font size
            if (properties.TryGetValue("fontSize", out var fontSize))
            {
                SetFontSize(Convert.ToSingle(fontSize));
            }
            
            // Handle text color
            if (properties.TryGetValue("color", out var color))
            {
                SetTextColor(ParseColor(color, _textMesh.color));
            }
            
            // Handle alignment
            if (properties.TryGetValue("alignment", out var alignment))
            {
                SetAlignment(alignment.ToString());
            }
            
            // Handle font weight
            if (properties.TryGetValue("fontWeight", out var fontWeight))
            {
                SetFontWeight(fontWeight.ToString());
            }
            
            // Handle wrapping
            if (properties.TryGetValue("wrap", out var wrap))
            {
                _textMesh.enableWordWrapping = Convert.ToBoolean(wrap);
            }
            
            // Handle overflow
            if (properties.TryGetValue("overflow", out var overflow))
            {
                SetOverflow(overflow.ToString());
            }
            
            // Handle line spacing
            if (properties.TryGetValue("lineSpacing", out var lineSpacing))
            {
                _textMesh.lineSpacing = Convert.ToSingle(lineSpacing);
            }
        }
        
        public override void ApplyTheme(Dictionary<string, object> theme)
        {
            base.ApplyTheme(theme);
            
            if (theme == null) return;
            
            // Apply text-specific theme settings
            if (theme.TryGetValue("textColor", out var textColor))
            {
                SetTextColor(ParseColor(textColor, _textMesh.color));
            }
            
            if (theme.TryGetValue("fontSize", out var fontSize))
            {
                SetFontSize(Convert.ToSingle(fontSize));
            }
            
            if (theme.TryGetValue("fontFamily", out var fontFamily))
            {
                // TODO: Load custom fonts
            }
        }
        
        private void SetAlignment(string alignment)
        {
            _textMesh.alignment = alignment?.ToLower() switch
            {
                "left" => TextAlignmentOptions.Left,
                "center" => TextAlignmentOptions.Center,
                "right" => TextAlignmentOptions.Right,
                "topleft" => TextAlignmentOptions.TopLeft,
                "top" => TextAlignmentOptions.Top,
                "topright" => TextAlignmentOptions.TopRight,
                "bottomleft" => TextAlignmentOptions.BottomLeft,
                "bottom" => TextAlignmentOptions.Bottom,
                "bottomright" => TextAlignmentOptions.BottomRight,
                "justified" => TextAlignmentOptions.Justified,
                _ => TextAlignmentOptions.Center
            };
        }
        
        private void SetFontWeight(string weight)
        {
            _textMesh.fontStyle = weight?.ToLower() switch
            {
                "bold" => FontStyles.Bold,
                "italic" => FontStyles.Italic,
                "bolditalic" => FontStyles.Bold | FontStyles.Italic,
                "underline" => FontStyles.Underline,
                "strikethrough" => FontStyles.Strikethrough,
                _ => FontStyles.Normal
            };
        }
        
        private void SetOverflow(string overflow)
        {
            _textMesh.overflowMode = overflow?.ToLower() switch
            {
                "truncate" => TextOverflowModes.Truncate,
                "ellipsis" => TextOverflowModes.Ellipsis,
                "masking" => TextOverflowModes.Masking,
                "scrollrect" => TextOverflowModes.ScrollRect,
                "page" => TextOverflowModes.Page,
                _ => TextOverflowModes.Overflow
            };
        }
    }
}