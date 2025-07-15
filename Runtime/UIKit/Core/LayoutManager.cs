using UnityEngine;
using System;
using System.Collections.Generic;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Manages layout calculations and constraints for UIKit templates
    /// </summary>
    public class LayoutManager
    {
        private readonly Dictionary<LayoutSize, LayoutDimensions> _layoutDefinitions;
        private readonly Dictionary<ComponentSize, ComponentDimensions> _componentDefinitions;
        
        public LayoutManager()
        {
            _layoutDefinitions = InitializeLayoutDefinitions();
            _componentDefinitions = InitializeComponentDefinitions();
        }
        
        /// <summary>
        /// Gets the dimensions for a layout size
        /// </summary>
        public LayoutDimensions GetLayoutDimensions(LayoutSize layoutSize)
        {
            if (_layoutDefinitions.TryGetValue(layoutSize, out var dimensions))
            {
                return CalculateActualDimensions(dimensions);
            }
            
            // Default to medium modal if not found
            return CalculateActualDimensions(_layoutDefinitions[LayoutSize.MediumModal]);
        }
        
        /// <summary>
        /// Gets the dimensions for a layout size from string
        /// </summary>
        public LayoutDimensions GetLayoutDimensions(string layoutSize)
        {
            if (Enum.TryParse<LayoutSize>(layoutSize, true, out var size))
            {
                return GetLayoutDimensions(size);
            }
            
            // Default to medium modal if invalid
            return GetLayoutDimensions(LayoutSize.MediumModal);
        }
        
        /// <summary>
        /// Gets component dimensions based on size and type
        /// </summary>
        public ComponentDimensions GetComponentDimensions(ComponentSize size, ComponentType type)
        {
            if (_componentDefinitions.TryGetValue(size, out var dimensions))
            {
                return dimensions;
            }
            
            return _componentDefinitions[ComponentSize.Medium];
        }
        
        /// <summary>
        /// Gets component dimensions from string size
        /// </summary>
        public ComponentDimensions GetComponentDimensions(string size, string type)
        {
            ComponentSize componentSize = ComponentSize.Medium;
            ComponentType componentType = ComponentType.Container;
            
            Enum.TryParse<ComponentSize>(size, true, out componentSize);
            Enum.TryParse<ComponentType>(type, true, out componentType);
            
            return GetComponentDimensions(componentSize, componentType);
        }
        
        /// <summary>
        /// Calculates actual pixel dimensions based on screen size
        /// </summary>
        private LayoutDimensions CalculateActualDimensions(LayoutDimensions baseDimensions)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Account for safe area on mobile devices
            var safeArea = Screen.safeArea;
            if (safeArea.width > 0 && safeArea.height > 0)
            {
                screenWidth = safeArea.width;
                screenHeight = safeArea.height;
            }
            
            return new LayoutDimensions
            {
                Width = baseDimensions.WidthPercent > 0 
                    ? screenWidth * baseDimensions.WidthPercent 
                    : baseDimensions.Width,
                    
                Height = baseDimensions.HeightPercent > 0 
                    ? screenHeight * baseDimensions.HeightPercent 
                    : baseDimensions.Height,
                    
                WidthPercent = baseDimensions.WidthPercent,
                HeightPercent = baseDimensions.HeightPercent,
                Anchor = baseDimensions.Anchor,
                Pivot = baseDimensions.Pivot,
                Padding = baseDimensions.Padding
            };
        }
        
        /// <summary>
        /// Initializes layout definitions
        /// </summary>
        private Dictionary<LayoutSize, LayoutDimensions> InitializeLayoutDefinitions()
        {
            return new Dictionary<LayoutSize, LayoutDimensions>
            {
                [LayoutSize.FullScreen] = new LayoutDimensions
                {
                    WidthPercent = 1.0f,
                    HeightPercent = 1.0f,
                    Anchor = new Vector2(0.5f, 0.5f),
                    Pivot = new Vector2(0.5f, 0.5f)
                },
                
                [LayoutSize.LargeModal] = new LayoutDimensions
                {
                    WidthPercent = 0.9f,
                    HeightPercent = 0.8f,
                    Anchor = new Vector2(0.5f, 0.5f),
                    Pivot = new Vector2(0.5f, 0.5f),
                    Padding = new RectOffset(20, 20, 20, 20)
                },
                
                [LayoutSize.MediumModal] = new LayoutDimensions
                {
                    WidthPercent = 0.8f,
                    HeightPercent = 0.6f,
                    Anchor = new Vector2(0.5f, 0.5f),
                    Pivot = new Vector2(0.5f, 0.5f),
                    Padding = new RectOffset(20, 20, 20, 20)
                },
                
                [LayoutSize.SmallModal] = new LayoutDimensions
                {
                    WidthPercent = 0.7f,
                    HeightPercent = 0.4f,
                    Anchor = new Vector2(0.5f, 0.5f),
                    Pivot = new Vector2(0.5f, 0.5f),
                    Padding = new RectOffset(16, 16, 16, 16)
                },
                
                [LayoutSize.TopBanner] = new LayoutDimensions
                {
                    WidthPercent = 1.0f,
                    HeightPercent = 0.15f,
                    Anchor = new Vector2(0.5f, 1.0f),
                    Pivot = new Vector2(0.5f, 1.0f)
                },
                
                [LayoutSize.BottomSheet] = new LayoutDimensions
                {
                    WidthPercent = 1.0f,
                    HeightPercent = 0.5f,
                    Anchor = new Vector2(0.5f, 0.0f),
                    Pivot = new Vector2(0.5f, 0.0f)
                },
                
                [LayoutSize.CenterSquare] = new LayoutDimensions
                {
                    WidthPercent = 0.8f,
                    HeightPercent = 0.8f,
                    MaintainAspectRatio = true,
                    AspectRatio = 1.0f,
                    Anchor = new Vector2(0.5f, 0.5f),
                    Pivot = new Vector2(0.5f, 0.5f)
                },
                
                [LayoutSize.FloatingButton] = new LayoutDimensions
                {
                    Width = 80,
                    Height = 80,
                    Anchor = new Vector2(1.0f, 0.0f),
                    Pivot = new Vector2(1.0f, 0.0f),
                    Padding = new RectOffset(0, 20, 20, 0)
                },
                
                [LayoutSize.SidePanel] = new LayoutDimensions
                {
                    WidthPercent = 0.3f,
                    HeightPercent = 1.0f,
                    Anchor = new Vector2(0.0f, 0.5f),
                    Pivot = new Vector2(0.0f, 0.5f)
                }
            };
        }
        
        /// <summary>
        /// Initializes component definitions
        /// </summary>
        private Dictionary<ComponentSize, ComponentDimensions> InitializeComponentDefinitions()
        {
            return new Dictionary<ComponentSize, ComponentDimensions>
            {
                [ComponentSize.Tiny] = new ComponentDimensions
                {
                    ImageSize = 32,
                    FontSize = 12,
                    Padding = 4
                },
                
                [ComponentSize.Small] = new ComponentDimensions
                {
                    ImageSize = 64,
                    FontSize = 14,
                    Padding = 8
                },
                
                [ComponentSize.Medium] = new ComponentDimensions
                {
                    ImageSize = 128,
                    FontSize = 16,
                    Padding = 12
                },
                
                [ComponentSize.Large] = new ComponentDimensions
                {
                    ImageSize = 256,
                    FontSize = 20,
                    Padding = 16
                },
                
                [ComponentSize.ExtraLarge] = new ComponentDimensions
                {
                    ImageSize = 384,
                    FontSize = 24,
                    Padding = 20
                },
                
                [ComponentSize.Hero] = new ComponentDimensions
                {
                    AspectRatio = 16f / 9f,
                    ResponsiveToContainer = true,
                    Padding = 0
                },
                
                [ComponentSize.Banner] = new ComponentDimensions
                {
                    AspectRatio = 3f / 1f,
                    ResponsiveToContainer = true,
                    Padding = 0
                },
                
                [ComponentSize.Fill] = new ComponentDimensions
                {
                    FillContainer = true,
                    Padding = 0
                }
            };
        }
        
        /// <summary>
        /// Converts alignment enum to Unity anchor values
        /// </summary>
        public static Vector2 GetAnchor(Alignment alignment)
        {
            return alignment switch
            {
                Alignment.TopLeft => new Vector2(0, 1),
                Alignment.TopCenter => new Vector2(0.5f, 1),
                Alignment.TopRight => new Vector2(1, 1),
                Alignment.MiddleLeft => new Vector2(0, 0.5f),
                Alignment.MiddleCenter => new Vector2(0.5f, 0.5f),
                Alignment.MiddleRight => new Vector2(1, 0.5f),
                Alignment.BottomLeft => new Vector2(0, 0),
                Alignment.BottomCenter => new Vector2(0.5f, 0),
                Alignment.BottomRight => new Vector2(1, 0),
                _ => new Vector2(0.5f, 0.5f)
            };
        }
    }
    
    /// <summary>
    /// Layout dimensions configuration
    /// </summary>
    public class LayoutDimensions
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float WidthPercent { get; set; }
        public float HeightPercent { get; set; }
        public Vector2 Anchor { get; set; }
        public Vector2 Pivot { get; set; }
        public RectOffset Padding { get; set; } = new RectOffset(0, 0, 0, 0);
        public bool MaintainAspectRatio { get; set; }
        public float AspectRatio { get; set; } = 1.0f;
    }
    
    /// <summary>
    /// Component dimensions configuration
    /// </summary>
    public class ComponentDimensions
    {
        public float ImageSize { get; set; }
        public float FontSize { get; set; }
        public float Padding { get; set; }
        public float AspectRatio { get; set; }
        public bool ResponsiveToContainer { get; set; }
        public bool FillContainer { get; set; }
    }
}