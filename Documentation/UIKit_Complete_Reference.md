# DecisionBox UIKit - Complete Reference Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Installation & Setup](#installation--setup)
4. [Core Concepts](#core-concepts)
5. [WebSocket Message Structure](#websocket-message-structure)
6. [Layout System](#layout-system)
7. [Component System](#component-system)
8. [Asset Management](#asset-management)
9. [Theme System](#theme-system)
10. [Action Handling](#action-handling)
11. [API Reference](#api-reference)
12. [Examples](#examples)
13. [Best Practices](#best-practices)
14. [Troubleshooting](#troubleshooting)
15. [Performance Optimization](#performance-optimization)

---

## Overview

DecisionBox UIKit is a dynamic UI rendering system for Unity that allows you to create, update, and manage UI elements through WebSocket messages. Instead of hardcoding UI in your Unity scenes, you can send JSON configurations that describe the UI structure, and UIKit will render them dynamically.

### Key Benefits
- **No App Updates Required**: Change UI without releasing new app versions
- **A/B Testing**: Test different UI variations easily
- **Personalization**: Show different UIs to different user segments
- **Rapid Iteration**: Designers can modify UI without developer involvement
- **Consistent Design**: Enforce design standards through centralized templates
- **Cross-Platform**: Same JSON works across all Unity-supported platforms

### How It Works
1. Server sends WebSocket message with `useTemplate: true`
2. DecisionBoxSDK detects UIKit message and routes to UIKitManager
3. UIKitManager parses template data and creates UI components
4. Assets are loaded from URLs and cached locally
5. User interactions trigger actions that can be handled by your game

---

## Architecture

### System Components

```
DecisionBoxSDK
    ├── WebSocket Handler
    │   └── UIKit Detection → UIKitManager
    │
    └── UIKitManager (Singleton)
        ├── TemplateRenderer
        │   ├── Canvas Management
        │   └── Template Lifecycle
        ├── LayoutManager
        │   └── Size Constraints
        ├── ComponentFactory
        │   └── Dynamic Component Creation
        ├── AssetLoader
        │   ├── URL Loading
        │   └── Progress Tracking
        └── AssetCache
            ├── Memory Cache
            └── Disk Cache
```

### Core Classes

#### UIKitManager
- **Purpose**: Central manager for all UIKit operations
- **Responsibilities**: Message processing, template lifecycle, event dispatch
- **Singleton**: Accessed via `UIKitManager.Instance`

#### DynamicTemplate
- **Purpose**: Represents a rendered template instance
- **Responsibilities**: Component management, layout application, event handling

#### LayoutManager
- **Purpose**: Manages layout constraints and responsive sizing
- **Responsibilities**: Calculate dimensions, apply constraints, handle orientation

#### ComponentFactory
- **Purpose**: Creates UI components from JSON data
- **Responsibilities**: Component instantiation, property application, hierarchy building

#### AssetLoader
- **Purpose**: Loads remote assets asynchronously
- **Responsibilities**: URL fetching, progress tracking, error handling

#### AssetCache
- **Purpose**: Caches loaded assets for performance
- **Responsibilities**: Disk storage, memory management, cache invalidation

---

## Installation & Setup

### Prerequisites
- Unity 2020.3 or higher
- TextMeshPro package
- Newtonsoft.Json (included with DecisionBox SDK)

### Integration Steps

1. **Automatic Integration**
   - UIKit is automatically initialized when DecisionBoxSDK starts
   - No additional setup required

2. **Manual Initialization (Optional)**
   ```csharp
   // Force UIKit initialization
   var uiKit = UIKitManager.Instance;
   ```

3. **Event Subscription**
   ```csharp
   void Start()
   {
       UIKitManager.Instance.OnUIKitInitialized += OnUIKitReady;
       UIKitManager.Instance.OnTemplateShown += OnTemplateShown;
       UIKitManager.Instance.OnTemplateHidden += OnTemplateHidden;
       UIKitManager.Instance.OnTemplateAction += OnTemplateAction;
   }
   
   void OnUIKitReady()
   {
       Debug.Log("UIKit is ready to receive templates");
   }
   
   void OnTemplateShown(string templateId)
   {
       Debug.Log($"Template shown: {templateId}");
   }
   
   void OnTemplateHidden(string templateId)
   {
       Debug.Log($"Template hidden: {templateId}");
   }
   
   void OnTemplateAction(string templateId, ComponentAction action)
   {
       Debug.Log($"Action from {templateId}: {action.Type}");
       // Handle the action based on type
   }
   ```

---

## Core Concepts

### Dynamic Templates
Templates are UI layouts defined entirely in JSON. They consist of:
- **Layout Size**: Predefined container size
- **Components**: UI elements arranged hierarchically
- **Theme**: Visual styling rules
- **Actions**: User interaction handlers

### Component Hierarchy
Components can be nested to create complex layouts:
```
Container (root)
├── Image (hero banner)
├── Container (content area)
│   ├── Text (title)
│   ├── Text (description)
│   └── Container (button group)
│       ├── Button (primary)
│       └── Button (secondary)
└── Timer (countdown)
```

### Responsive Design
UIKit uses percentage-based layouts and predefined size constraints to ensure UI looks good on all screen sizes.

---

## WebSocket Message Structure

### Complete Message Format

```json
{
  // Standard DecisionBox fields
  "action": "YOUR_ACTION_TYPE",
  "type": "sdk-notification",
  "app_id": "your-app-id",
  "session_id": "unique-session-id",
  "DECISIONBOX_RS_ID": "ruleset-id",
  
  // UIKit specific fields
  "useTemplate": true,              // Required: Enables UIKit processing
  "layoutSize": "MediumModal",      // Required: Layout constraint
  
  // Template definition
  "templateData": {
    "components": [               // Required: Array of UI components
      {
        "type": "Container",
        "id": "root",
        "properties": {...},
        "children": [...]
      }
    ],
    "theme": {                    // Optional: Theme overrides
      "primaryColor": "#007AFF",
      "textColor": "#333333"
    },
    "animations": {               // Optional: Animation settings
      "showAnimation": "Fade",
      "hideAnimation": "Scale",
      "duration": 0.3
    }
  },
  
  // Template configuration
  "templateConfig": {             // Optional: Runtime configuration
    "autoClose": true,
    "autoCloseDelay": 5000,
    "blockInput": true
  },
  
  // Custom data
  "payload": {                    // Optional: Your custom data
    "productId": "item_123",
    "price": 9.99
  }
}
```

### Field Descriptions

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `useTemplate` | boolean | Yes | Must be `true` to trigger UIKit processing |
| `layoutSize` | string | Yes | One of the predefined layout sizes |
| `templateData` | object | Yes | Contains components, theme, and animations |
| `templateData.components` | array | Yes | Array of component definitions |
| `templateData.theme` | object | No | Theme overrides for this template |
| `templateData.animations` | object | No | Animation configuration |
| `templateConfig` | object | No | Runtime behavior configuration |
| `payload` | object | No | Custom data for your game logic |

---

## Layout System

### Layout Sizes

UIKit provides 9 predefined layout sizes that automatically adapt to different screen sizes:

| Layout Size | Description | Dimensions | Use Case |
|-------------|-------------|------------|----------|
| `FullScreen` | Covers entire screen | 100% × 100% | Splash screens, full-page offers |
| `LargeModal` | Large centered modal | 90% × 80% | Complex forms, detailed content |
| `MediumModal` | Medium centered modal | 80% × 60% | Standard dialogs, offers |
| `SmallModal` | Small centered modal | 70% × 40% | Simple confirmations |
| `TopBanner` | Top notification | 100% × 15% | Notifications, alerts |
| `BottomSheet` | Bottom sliding panel | 100% × 50% | Action sheets, options |
| `CenterSquare` | Square in center | 80% of min(w,h) | Profile pictures, badges |
| `FloatingButton` | Fixed floating button | 80×80 dp | FAB, quick actions |
| `SidePanel` | Side panel | 30% × 100% | Navigation, filters |

### Layout Properties

Each layout has default properties:
- **Anchor**: Where the layout is anchored on screen
- **Pivot**: The pivot point for positioning
- **Padding**: Default padding inside the layout

### Responsive Behavior

Layouts automatically adjust for:
- **Screen Orientation**: Portrait vs Landscape
- **Safe Areas**: Notches, home indicators
- **Different Resolutions**: Phones, tablets, etc.

Example layout application:
```csharp
// Internally handled by UIKit
var dimensions = layoutManager.GetLayoutDimensions("MediumModal");
rectTransform.anchorMin = dimensions.Anchor - dimensions.Size/2;
rectTransform.anchorMax = dimensions.Anchor + dimensions.Size/2;
```

---

## Component System

### Component Types

#### 1. Image Component
Displays images loaded from URLs with caching support.

```json
{
  "type": "Image",
  "id": "heroImage",
  "size": "Hero",              // Tiny|Small|Medium|Large|Hero|Banner|Fill
  "assetUrl": "https://example.com/image.png",
  "properties": {
    "scaleMode": "aspectFit",  // fill|fit|aspectFit|aspectFill
    "preserveAspect": true,
    "tint": "#FF0000",         // Color tint
    "opacity": 0.8
  }
}
```

**Size Options**:
- `Tiny`: 32×32 pixels
- `Small`: 64×64 pixels
- `Medium`: 128×128 pixels
- `Large`: 256×256 pixels
- `ExtraLarge`: 384×384 pixels
- `Hero`: 16:9 aspect ratio
- `Banner`: 3:1 aspect ratio
- `Fill`: Fills container

#### 2. Text Component
Rich text display with TextMeshPro support.

```json
{
  "type": "Text",
  "id": "title",
  "size": "Large",
  "content": "Welcome!",
  "properties": {
    "fontSize": 24,
    "color": "#333333",
    "fontWeight": "bold",        // normal|bold|italic|bolditalic
    "alignment": "center",       // left|center|right|justified
    "wrap": true,
    "overflow": "ellipsis",      // overflow|truncate|ellipsis
    "lineSpacing": 1.2,
    "letterSpacing": 0.5
  }
}
```

#### 3. Button Component
Interactive button with customizable appearance and actions.

```json
{
  "type": "Button",
  "id": "purchaseBtn",
  "size": "Large",
  "content": "Buy Now",
  "action": {
    "type": "Purchase",
    "data": {
      "productId": "item_123",
      "price": 9.99
    }
  },
  "properties": {
    "backgroundColor": "#007AFF",
    "textColor": "#FFFFFF",
    "fontSize": 18,
    "fontWeight": "bold",
    "cornerRadius": 8,
    "padding": {
      "left": 20,
      "right": 20,
      "top": 10,
      "bottom": 10
    },
    "enabled": true
  }
}
```

#### 4. Container Component
Groups components with layout management.

```json
{
  "type": "Container",
  "id": "contentArea",
  "size": "Fill",
  "properties": {
    "layout": "vertical",         // vertical|horizontal|grid|none
    "spacing": 10,
    "padding": 20,
    "backgroundColor": "#F5F5F5",
    "cornerRadius": 12,
    "borderWidth": 1,
    "borderColor": "#E0E0E0",
    "scrollable": true,
    "childAlignment": "middlecenter"
  },
  "children": [...]
}
```

**Layout Types**:
- `vertical`: Stack children vertically
- `horizontal`: Arrange children horizontally
- `grid`: Grid layout with rows/columns
- `none`: Manual positioning

**Grid-Specific Properties**:
```json
{
  "layout": "grid",
  "constraint": "fixedColumnCount",  // flexible|fixedColumnCount|fixedRowCount
  "constraintCount": 3,
  "cellWidth": 100,
  "cellHeight": 100,
  "spacingX": 10,
  "spacingY": 10
}
```

#### 5. Timer Component
Countdown timer with formatting options.

```json
{
  "type": "Timer",
  "id": "countdown",
  "properties": {
    "duration": 3600,             // Seconds
    "format": "{0:00}:{1:00}:{2:00}",  // HH:MM:SS
    "autoStart": true,
    "fontSize": 16,
    "color": "#E74C3C",
    "alignment": "center",
    "warningThreshold": 60,       // Change color when below threshold
    "warningColor": "#FF0000"
  }
}
```

**Format Patterns**:
- `{0:00}:{1:00}:{2:00}` - HH:MM:SS
- `{0:00}:{1:00}` - MM:SS
- `{0} seconds` - Seconds only
- Custom string with `$time` placeholder

#### 6. ProgressBar Component
Visual progress indicator.

```json
{
  "type": "ProgressBar",
  "id": "loadingProgress",
  "properties": {
    "value": 0.7,                 // 0.0 to 1.0
    "min": 0,
    "max": 100,
    "height": 20,
    "backgroundColor": "#E0E0E0",
    "fillColor": "#4CAF50",
    "direction": "lefttoright",   // lefttoright|righttoleft|bottomtotop|toptobottom
    "animated": true,
    "animationSpeed": 1.0,
    "showLabel": true,
    "labelFormat": "{0:0}%",
    "labelColor": "#FFFFFF"
  }
}
```

#### 7. Spacer Component
Adds space in layouts.

```json
{
  "type": "Spacer",
  "id": "flexSpace",
  "properties": {
    "type": "flexible",           // flexible|fixed|horizontal|vertical
    "width": 100,                 // For fixed spacers
    "height": 20,                 // For fixed spacers
    "flexibleWidth": 1.0,         // Weight for flexible space
    "flexibleHeight": 1.0
  }
}
```

### Component Properties

#### Common Properties
All components support these base properties:

| Property | Type | Description |
|----------|------|-------------|
| `visible` | boolean | Show/hide component |
| `opacity` | float | 0.0 to 1.0 transparency |
| `transform` | object | Position, rotation, scale |

#### Transform Properties
```json
{
  "transform": {
    "position": {"x": 0, "y": 0},
    "rotation": 0,                // Degrees
    "scale": {"x": 1, "y": 1}
  }
}
```

### Component Positioning

Components can be positioned using:

1. **Layout-based** (Recommended)
   - Use Container with layout
   - Components auto-position

2. **Absolute Positioning**
   ```json
   {
     "position": {
       "x": 100,
       "y": 50,
       "alignment": "topLeft",
       "anchor": "0.5,0.5"
     }
   }
   ```

---

## Asset Management

### Asset Loading

UIKit automatically loads assets from URLs with:
- **Progressive Loading**: UI shows while assets load
- **Placeholder Support**: Show default content during load
- **Error Handling**: Fallback on load failure
- **Retry Logic**: Automatic retry on network errors

### Asset URLs

Assets can be specified as:

1. **Direct URL**
   ```json
   "assetUrl": "https://cdn.example.com/image.png"
   ```

2. **Resolution-Specific URLs**
   ```json
   "assetUrl": {
     "1x": "https://cdn.example.com/image.png",
     "2x": "https://cdn.example.com/image@2x.png",
     "3x": "https://cdn.example.com/image@3x.png"
   }
   ```

3. **Platform-Specific URLs**
   ```json
   "assetUrl": {
     "ios": "https://cdn.example.com/ios/image.png",
     "android": "https://cdn.example.com/android/image.png",
     "default": "https://cdn.example.com/image.png"
   }
   ```

### Caching System

#### Memory Cache
- **Purpose**: Fast access to recently used assets
- **Size**: Automatic management
- **Lifetime**: Until app closes or memory pressure

#### Disk Cache
- **Purpose**: Persistent storage across sessions
- **Size**: 100MB default (configurable)
- **Location**: `Application.persistentDataPath/DecisionBoxUIKit/Cache`
- **Cleanup**: LRU (Least Recently Used) eviction

#### Cache Configuration
```csharp
// Configure cache size (optional)
var cache = UIKitManager.Instance.AssetCache;
cache.MaxCacheSize = 200 * 1024 * 1024; // 200MB
```

### Asset Specifications

Each template can define required assets:
```json
{
  "assets": {
    "hero": {
      "url": "https://cdn.example.com/hero.png",
      "preload": true,
      "required": true,
      "maxSize": 2097152,         // 2MB in bytes
      "dimensions": {
        "width": 1200,
        "height": 800
      }
    }
  }
}
```

### Preloading Assets

For better performance, preload assets before showing templates:
```csharp
// Assets are automatically preloaded when template is received
// But you can manually preload if needed:
var assetLoader = UIKitManager.Instance.AssetLoader;
assetLoader.LoadTexture(url, (texture, error) => {
    if (texture != null) {
        Debug.Log("Asset preloaded successfully");
    }
});
```

---

## Theme System

### Global Theme

Define consistent styling across all components:

```json
{
  "theme": {
    // Colors
    "primaryColor": "#007AFF",
    "secondaryColor": "#5856D6",
    "backgroundColor": "#FFFFFF",
    "surfaceColor": "#F2F2F7",
    "errorColor": "#FF3B30",
    "warningColor": "#FF9500",
    "successColor": "#34C759",
    
    // Text
    "textColor": "#000000",
    "secondaryTextColor": "#8E8E93",
    "fontSize": 16,
    "fontFamily": "Helvetica",
    
    // Components
    "buttonColor": "#007AFF",
    "buttonTextColor": "#FFFFFF",
    "buttonCornerRadius": 8,
    
    // Spacing
    "spacing": 8,
    "padding": 16,
    
    // Borders
    "borderColor": "#C6C6C8",
    "borderWidth": 1,
    "cornerRadius": 12
  }
}
```

### Theme Inheritance

Components inherit theme values hierarchically:
1. Global theme (lowest priority)
2. Template theme
3. Container theme
4. Component properties (highest priority)

### Theme Variables

Use theme variables in components:
```json
{
  "type": "Text",
  "properties": {
    "color": "@theme.primaryColor",      // Reference theme color
    "fontSize": "@theme.fontSize * 1.5"  // Calculate from theme
  }
}
```

### Dark Mode Support

Provide alternate themes:
```json
{
  "theme": {
    "light": {
      "backgroundColor": "#FFFFFF",
      "textColor": "#000000"
    },
    "dark": {
      "backgroundColor": "#000000",
      "textColor": "#FFFFFF"
    }
  }
}
```

---

## Action Handling

### Action Types

| Action Type | Description | Required Data | Example Use |
|-------------|-------------|---------------|-------------|
| `None` | No action | - | Decorative elements |
| `Close` | Close template | - | Dismiss buttons |
| `OpenUrl` | Open external URL | `url` | Links, store pages |
| `SendEvent` | Send custom event | `eventType`, custom | Game events |
| `Navigate` | Open another template | `templateId` | Multi-page flows |
| `Purchase` | IAP purchase | `productId`, `price` | Buy buttons |
| `Share` | Share content | `content`, `url` | Social sharing |
| `PlaySound` | Play sound effect | `soundId` | Feedback |
| `Custom` | Custom action | Any | Game-specific |

### Action Data Structure

```json
{
  "action": {
    "type": "SendEvent",
    "data": {
      "eventType": "ITEM_SELECTED",
      "itemId": "sword_001",
      "category": "weapons",
      "customField": "value"
    }
  }
}
```

### Handling Actions in Code

```csharp
public class UIActionHandler : MonoBehaviour
{
    void Start()
    {
        UIKitManager.Instance.OnTemplateAction += HandleAction;
    }
    
    void HandleAction(string templateId, ComponentAction action)
    {
        switch (action.Type)
        {
            case "Close":
                // Already handled by UIKit
                break;
                
            case "Purchase":
                HandlePurchase(action.Data);
                break;
                
            case "SendEvent":
                HandleCustomEvent(action.Data);
                break;
                
            case "OpenUrl":
                Application.OpenURL(action.Data["url"].ToString());
                break;
                
            case "Custom":
                HandleCustomAction(action.Data);
                break;
        }
    }
    
    void HandlePurchase(Dictionary<string, object> data)
    {
        string productId = data["productId"].ToString();
        float price = Convert.ToSingle(data["price"]);
        
        // Trigger your IAP system
        IAPManager.Instance.Purchase(productId);
    }
    
    void HandleCustomEvent(Dictionary<string, object> data)
    {
        string eventType = data["eventType"].ToString();
        
        // Track analytics
        Analytics.TrackEvent(eventType, data);
        
        // Handle game logic
        switch (eventType)
        {
            case "REWARD_CLAIMED":
                ClaimReward(data);
                break;
        }
    }
}
```

### Action Callbacks

Get notified when actions complete:
```csharp
// Send action result back to server
void HandleAction(string templateId, ComponentAction action)
{
    // Process action...
    
    // Report result
    DecisionBoxSDK.Instance.TrackEvent(new Dictionary<string, object>
    {
        { "event_type", "template_action" },
        { "template_id", templateId },
        { "action_type", action.Type },
        { "action_data", action.Data },
        { "result", "success" }
    });
}
```

---

## API Reference

### UIKitManager

#### Properties
```csharp
// Singleton instance
public static UIKitManager Instance { get; }

// Sub-systems
public AssetLoader AssetLoader { get; }
public AssetCache AssetCache { get; }
public TemplateRenderer TemplateRenderer { get; }
public LayoutManager LayoutManager { get; }
public ComponentFactory ComponentFactory { get; }
```

#### Methods
```csharp
// Process a WebSocket message
public bool ProcessMessage(WebSocketMessage message)

// Hide a specific template
public void HideTemplate(string templateId)

// Hide all active templates
public void HideAllTemplates()

// Check if template is active
public bool IsTemplateActive(string templateId)

// Get all active template IDs
public string[] GetActiveTemplateIds()
```

#### Events
```csharp
// Fired when UIKit initializes
public event Action OnUIKitInitialized

// Fired when template is shown
public event Action<string> OnTemplateShown

// Fired when template is hidden
public event Action<string> OnTemplateHidden

// Fired when user interacts with template
public event Action<string, ComponentAction> OnTemplateAction
```

### AssetLoader

#### Methods
```csharp
// Load single texture
public void LoadTexture(
    string url, 
    Action<Texture2D, string> onComplete, 
    Action<float> onProgress = null)

// Load multiple textures
public void LoadTextures(
    string[] urls, 
    Action<Dictionary<string, Texture2D>> onAllComplete, 
    Action<float> onProgress = null)

// Preload template assets
public void PreloadTemplateAssets(
    string templateId, 
    Dictionary<string, object> assetUrls, 
    Action<bool> onComplete)

// Clear memory cache
public void ClearMemoryCache()
```

#### Properties
```csharp
// Number of items in memory cache
public int MemoryCacheCount { get; }

// Number of items in load queue
public int LoadQueueCount { get; }
```

### AssetCache

#### Methods
```csharp
// Cache a texture
public void CacheTexture(string url, Texture2D texture)

// Try to get cached texture
public bool TryGetCachedTexture(string url, out Texture2D texture)

// Clear all cached assets
public void ClearCache()
```

#### Properties
```csharp
// Current cache size in bytes
public long CurrentCacheSize { get; }

// Maximum cache size in bytes
public long MaxCacheSize { get; set; }

// Number of cached items
public int CachedItemCount { get; }
```

### LayoutManager

#### Methods
```csharp
// Get layout dimensions by enum
public LayoutDimensions GetLayoutDimensions(LayoutSize layoutSize)

// Get layout dimensions by string
public LayoutDimensions GetLayoutDimensions(string layoutSize)

// Get component dimensions
public ComponentDimensions GetComponentDimensions(ComponentSize size, ComponentType type)

// Get component dimensions by string
public ComponentDimensions GetComponentDimensions(string size, string type)

// Convert alignment to anchor
public static Vector2 GetAnchor(Alignment alignment)
```

### Component Interfaces

#### IUIKitComponent
```csharp
public interface IUIKitComponent : IDisposable
{
    string Id { get; }
    GameObject GameObject { get; }
    bool IsVisible { get; }
    
    void ApplyTheme(Dictionary<string, object> theme);
    void UpdateProperties(Dictionary<string, object> properties);
    void SetVisible(bool visible);
}
```

#### IAssetComponent
```csharp
public interface IAssetComponent
{
    void SetAsset(Texture2D texture);
    Texture2D GetAsset();
}
```

#### IInteractableComponent
```csharp
public interface IInteractableComponent
{
    event Action<ComponentAction> OnAction;
    bool IsInteractable { get; }
    
    void SetInteractable(bool interactable);
}
```

#### ITextComponent
```csharp
public interface ITextComponent
{
    void SetText(string text);
    string GetText();
    void SetTextColor(Color color);
    void SetFontSize(float size);
}
```

#### IContainerComponent
```csharp
public interface IContainerComponent
{
    void AddChild(IUIKitComponent child);
    void RemoveChild(IUIKitComponent child);
    IUIKitComponent[] GetChildren();
    void ClearChildren();
}
```

---

## Examples

### Example 1: Simple Notification

```json
{
  "action": "ACHIEVEMENT_UNLOCKED",
  "type": "sdk-notification",
  "useTemplate": true,
  "layoutSize": "TopBanner",
  "templateData": {
    "components": [
      {
        "type": "Container",
        "id": "notification",
        "size": "Fill",
        "properties": {
          "layout": "horizontal",
          "backgroundColor": "#4CAF50",
          "padding": 15,
          "spacing": 10
        },
        "children": [
          {
            "type": "Image",
            "id": "icon",
            "size": "Small",
            "assetUrl": "https://example.com/achievement-icon.png"
          },
          {
            "type": "Text",
            "id": "message",
            "content": "Achievement Unlocked: First Victory!",
            "properties": {
              "color": "#FFFFFF",
              "fontSize": 16
            }
          }
        ]
      }
    ]
  },
  "templateConfig": {
    "autoClose": true,
    "autoCloseDelay": 5000
  }
}
```

### Example 2: Purchase Dialog

```json
{
  "action": "SPECIAL_OFFER",
  "type": "sdk-notification",
  "useTemplate": true,
  "layoutSize": "MediumModal",
  "templateData": {
    "components": [
      {
        "type": "Container",
        "id": "dialog",
        "size": "Fill",
        "properties": {
          "layout": "vertical",
          "backgroundColor": "#FFFFFF",
          "cornerRadius": 16,
          "padding": 24,
          "spacing": 16
        },
        "children": [
          {
            "type": "Image",
            "id": "productImage",
            "size": "Hero",
            "assetUrl": "https://example.com/product-hero.jpg",
            "properties": {
              "cornerRadius": 8
            }
          },
          {
            "type": "Text",
            "id": "title",
            "size": "Large",
            "content": "Limited Time Offer!",
            "properties": {
              "fontSize": 24,
              "fontWeight": "bold",
              "color": "#333333"
            }
          },
          {
            "type": "Text",
            "id": "description",
            "content": "Get the Mega Bundle for 50% off. This offer expires soon!",
            "properties": {
              "color": "#666666",
              "wrap": true
            }
          },
          {
            "type": "Timer",
            "id": "countdown",
            "properties": {
              "duration": 3600,
              "format": "Expires in: {0:00}:{1:00}:{2:00}",
              "autoStart": true,
              "color": "#FF5722"
            }
          },
          {
            "type": "Container",
            "id": "priceContainer",
            "properties": {
              "layout": "horizontal",
              "spacing": 12,
              "childAlignment": "middlecenter"
            },
            "children": [
              {
                "type": "Text",
                "id": "originalPrice",
                "content": "$19.99",
                "properties": {
                  "fontWeight": "strikethrough",
                  "color": "#999999"
                }
              },
              {
                "type": "Text",
                "id": "salePrice",
                "content": "$9.99",
                "properties": {
                  "fontSize": 28,
                  "fontWeight": "bold",
                  "color": "#4CAF50"
                }
              }
            ]
          },
          {
            "type": "Spacer",
            "id": "spacer",
            "properties": {
              "height": 8
            }
          },
          {
            "type": "Button",
            "id": "purchaseButton",
            "size": "Large",
            "content": "Buy Now",
            "action": {
              "type": "Purchase",
              "data": {
                "productId": "mega_bundle_sale",
                "price": 9.99,
                "currency": "USD"
              }
            },
            "properties": {
              "backgroundColor": "#4CAF50",
              "textColor": "#FFFFFF",
              "fontSize": 18,
              "fontWeight": "bold",
              "cornerRadius": 24
            }
          },
          {
            "type": "Button",
            "id": "dismissButton",
            "content": "Maybe Later",
            "action": {
              "type": "Close"
            },
            "properties": {
              "backgroundColor": "transparent",
              "textColor": "#999999",
              "fontSize": 14
            }
          }
        ]
      }
    ],
    "theme": {
      "primaryColor": "#4CAF50",
      "textColor": "#333333"
    }
  }
}
```

### Example 3: Multi-Page Onboarding

```json
{
  "action": "ONBOARDING_STEP_1",
  "type": "sdk-notification",
  "useTemplate": true,
  "layoutSize": "LargeModal",
  "templateData": {
    "components": [
      {
        "type": "Container",
        "id": "onboarding",
        "size": "Fill",
        "properties": {
          "layout": "vertical",
          "backgroundColor": "#F8F9FA",
          "padding": 32
        },
        "children": [
          {
            "type": "ProgressBar",
            "id": "progress",
            "properties": {
              "value": 0.33,
              "height": 4,
              "backgroundColor": "#E0E0E0",
              "fillColor": "#007AFF"
            }
          },
          {
            "type": "Spacer",
            "id": "topSpace",
            "properties": { "height": 24 }
          },
          {
            "type": "Image",
            "id": "illustration",
            "size": "Large",
            "assetUrl": "https://example.com/onboarding-1.png"
          },
          {
            "type": "Text",
            "id": "title",
            "size": "Large",
            "content": "Welcome to the Game!",
            "properties": {
              "fontSize": 28,
              "fontWeight": "bold",
              "alignment": "center"
            }
          },
          {
            "type": "Text",
            "id": "description",
            "content": "Embark on an epic adventure and become the ultimate hero.",
            "properties": {
              "alignment": "center",
              "color": "#666666",
              "wrap": true
            }
          },
          {
            "type": "Spacer",
            "id": "flexSpace",
            "properties": { "type": "flexible" }
          },
          {
            "type": "Container",
            "id": "navigation",
            "properties": {
              "layout": "horizontal",
              "spacing": 16
            },
            "children": [
              {
                "type": "Button",
                "id": "skipButton",
                "content": "Skip",
                "action": {
                  "type": "SendEvent",
                  "data": {
                    "eventType": "ONBOARDING_SKIP",
                    "step": 1
                  }
                },
                "properties": {
                  "backgroundColor": "transparent",
                  "textColor": "#999999"
                }
              },
              {
                "type": "Spacer",
                "id": "navSpace",
                "properties": { "type": "flexible" }
              },
              {
                "type": "Button",
                "id": "nextButton",
                "content": "Next",
                "action": {
                  "type": "Navigate",
                  "data": {
                    "templateId": "ONBOARDING_STEP_2"
                  }
                },
                "properties": {
                  "backgroundColor": "#007AFF",
                  "textColor": "#FFFFFF",
                  "cornerRadius": 20,
                  "padding": {
                    "left": 32,
                    "right": 32,
                    "top": 12,
                    "bottom": 12
                  }
                }
              }
            ]
          }
        ]
      }
    ]
  }
}
```

---

## Best Practices

### 1. Performance Optimization

#### Asset Optimization
- **Use appropriate image formats**: JPEG for photos, PNG for transparency
- **Optimize image sizes**: Don't use 4K images for small icons
- **Provide multiple resolutions**: @1x, @2x, @3x for different screen densities
- **Compress images**: Use tools like TinyPNG or ImageOptim
- **Limit file sizes**: Keep individual assets under 2MB

#### Loading Strategy
- **Preload critical assets**: Load hero images and important icons first
- **Use progressive loading**: Show UI with placeholders while assets load
- **Implement lazy loading**: Load off-screen content as needed
- **Cache aggressively**: Leverage both memory and disk cache

#### Template Optimization
- **Limit component count**: Keep under 50 components per template
- **Use efficient layouts**: Prefer simple layouts over complex nesting
- **Minimize transparency**: Opaque components render faster
- **Batch similar components**: Group similar items in containers

### 2. Design Guidelines

#### Responsive Design
- **Use relative sizes**: Percentages instead of fixed pixels
- **Test on multiple devices**: Phones, tablets, different aspects
- **Handle orientation changes**: Portrait and landscape layouts
- **Consider safe areas**: Account for notches and system UI

#### Visual Hierarchy
- **Clear focal points**: Guide user attention
- **Consistent spacing**: Use theme spacing values
- **Readable text**: Minimum 14pt font size
- **Sufficient contrast**: WCAG AA compliance

#### User Experience
- **Clear actions**: Obvious primary and secondary actions
- **Feedback**: Visual feedback for interactions
- **Loading states**: Show progress for long operations
- **Error handling**: Graceful degradation on failures

### 3. Template Design

#### Structure
```
Root Container
├── Header Section
│   ├── Close Button (optional)
│   └── Title
├── Content Section
│   ├── Primary Content
│   └── Supporting Content
└── Action Section
    ├── Primary Action
    └── Secondary Action
```

#### Naming Conventions
- **IDs**: Use camelCase (e.g., `purchaseButton`)
- **Types**: Use PascalCase (e.g., `Container`)
- **Actions**: Use UPPER_SNAKE_CASE (e.g., `PURCHASE_ITEM`)

#### State Management
- **Loading States**: Show skeletons or spinners
- **Empty States**: Handle no-content scenarios
- **Error States**: Display helpful error messages
- **Success States**: Confirm successful actions

### 4. Integration Best Practices

#### Event Handling
```csharp
public class TemplateEventHandler : MonoBehaviour
{
    private Dictionary<string, Action<ComponentAction>> _handlers;
    
    void Start()
    {
        _handlers = new Dictionary<string, Action<ComponentAction>>
        {
            { "Purchase", HandlePurchase },
            { "SendEvent", HandleEvent },
            { "Custom", HandleCustom }
        };
        
        UIKitManager.Instance.OnTemplateAction += OnAction;
    }
    
    void OnAction(string templateId, ComponentAction action)
    {
        if (_handlers.TryGetValue(action.Type, out var handler))
        {
            handler(action);
        }
    }
}
```

#### Analytics Integration
```csharp
void TrackTemplateEvent(string templateId, string event_type, Dictionary<string, object> data)
{
    var eventData = new Dictionary<string, object>
    {
        { "template_id", templateId },
        { "event_type", event_type },
        { "timestamp", DateTime.UtcNow.ToUnixTimeSeconds() }
    };
    
    foreach (var kvp in data)
    {
        eventData[kvp.Key] = kvp.Value;
    }
    
    Analytics.TrackEvent("uikit_event", eventData);
}
```

#### Error Handling
```csharp
void Start()
{
    UIKitManager.Instance.OnTemplateAction += SafeHandleAction;
}

void SafeHandleAction(string templateId, ComponentAction action)
{
    try
    {
        HandleAction(templateId, action);
    }
    catch (Exception e)
    {
        Debug.LogError($"Error handling UIKit action: {e.Message}");
        
        // Report error to analytics
        TrackError("uikit_action_error", e);
        
        // Show user-friendly error if needed
        ShowErrorNotification("Something went wrong. Please try again.");
    }
}
```

### 5. Security Considerations

#### Asset Loading
- **Validate URLs**: Only load from trusted domains
- **Use HTTPS**: Always use secure connections
- **Verify checksums**: Validate asset integrity
- **Sanitize filenames**: Prevent directory traversal

#### Data Validation
- **Validate JSON**: Check structure before processing
- **Sanitize text**: Prevent XSS in dynamic content
- **Limit sizes**: Enforce maximum template size
- **Type checking**: Validate data types

#### Action Security
```csharp
bool IsActionAllowed(ComponentAction action)
{
    // Whitelist allowed action types
    var allowedActions = new[] { "Close", "SendEvent", "Purchase" };
    
    if (!allowedActions.Contains(action.Type))
    {
        Debug.LogWarning($"Blocked unauthorized action: {action.Type}");
        return false;
    }
    
    // Additional validation based on action type
    switch (action.Type)
    {
        case "OpenUrl":
            return IsUrlSafe(action.Data["url"].ToString());
            
        case "Purchase":
            return IsValidProduct(action.Data["productId"].ToString());
    }
    
    return true;
}
```

---

## Troubleshooting

### Common Issues

#### 1. Template Not Showing

**Symptoms**: ProcessMessage returns true but no UI appears

**Causes & Solutions**:
- **Missing canvas**: UIKit creates its own canvas, check hierarchy
- **Layout size issue**: Verify valid layout size string
- **Component errors**: Check console for component creation errors
- **Hidden behind**: Check canvas sort order

**Debug Code**:
```csharp
// Check if template is active
Debug.Log($"Template active: {UIKitManager.Instance.IsTemplateActive(templateId)}");

// List all active templates
var activeTemplates = UIKitManager.Instance.GetActiveTemplateIds();
Debug.Log($"Active templates: {string.Join(", ", activeTemplates)}");

// Check canvas
var canvas = GameObject.Find("[DecisionBox UIKit Canvas]");
Debug.Log($"UIKit Canvas exists: {canvas != null}");
```

#### 2. Assets Not Loading

**Symptoms**: Images show as white/missing

**Causes & Solutions**:
- **Invalid URLs**: Check URL accessibility
- **CORS issues**: Ensure proper CORS headers
- **Network timeout**: Check internet connection
- **Cache corruption**: Clear cache and retry

**Debug Code**:
```csharp
// Monitor asset loading
var loader = UIKitManager.Instance.AssetLoader;
loader.LoadTexture(url, (texture, error) =>
{
    if (error != null)
    {
        Debug.LogError($"Asset load failed: {error}");
        // Check specific error type
    }
    else
    {
        Debug.Log($"Asset loaded: {texture.width}x{texture.height}");
    }
}, 
progress =>
{
    Debug.Log($"Loading progress: {progress:P}");
});
```

#### 3. Layout Issues

**Symptoms**: Components overlapping or misaligned

**Causes & Solutions**:
- **Missing layout type**: Specify layout in container
- **Size conflicts**: Check size constraints
- **Anchor issues**: Verify anchor settings
- **Parent constraints**: Check parent container size

**Debug Code**:
```csharp
// Debug component hierarchy
void DebugTemplate(string templateId)
{
    var template = GameObject.Find($"Template_{templateId}");
    if (template != null)
    {
        DebugTransform(template.transform, 0);
    }
}

void DebugTransform(Transform t, int depth)
{
    var indent = new string(' ', depth * 2);
    var rect = t.GetComponent<RectTransform>();
    
    Debug.Log($"{indent}{t.name}: " +
              $"Pos={rect.anchoredPosition} " +
              $"Size={rect.sizeDelta} " +
              $"Anchor=({rect.anchorMin},{rect.anchorMax})");
    
    foreach (Transform child in t)
    {
        DebugTransform(child, depth + 1);
    }
}
```

#### 4. Performance Issues

**Symptoms**: UI lag, slow loading, high memory usage

**Causes & Solutions**:
- **Too many components**: Simplify template structure
- **Large assets**: Optimize image sizes
- **Memory leaks**: Ensure proper cleanup
- **Excessive updates**: Batch UI updates

**Performance Monitoring**:
```csharp
public class UIKitPerformanceMonitor : MonoBehaviour
{
    void Start()
    {
        UIKitManager.Instance.OnTemplateShown += OnTemplateShown;
        UIKitManager.Instance.OnTemplateHidden += OnTemplateHidden;
    }
    
    void OnTemplateShown(string templateId)
    {
        StartCoroutine(MonitorPerformance(templateId));
    }
    
    IEnumerator MonitorPerformance(string templateId)
    {
        var startTime = Time.realtimeSinceStartup;
        var startMemory = System.GC.GetTotalMemory(false);
        
        yield return new WaitForSeconds(0.5f);
        
        var loadTime = Time.realtimeSinceStartup - startTime;
        var memoryUsed = System.GC.GetTotalMemory(false) - startMemory;
        
        Debug.Log($"Template {templateId} - " +
                  $"Load time: {loadTime:F2}s, " +
                  $"Memory: {memoryUsed / 1024 / 1024:F1}MB");
        
        // Check cache status
        var cache = UIKitManager.Instance.AssetCache;
        Debug.Log($"Cache: {cache.CachedItemCount} items, " +
                  $"{cache.CurrentCacheSize / 1024 / 1024:F1}MB used");
    }
}
```

### Debug Mode

Enable verbose logging:
```csharp
public static class UIKitDebug
{
    public static bool EnableVerboseLogging = true;
    
    public static void Log(string message)
    {
        if (EnableVerboseLogging)
        {
            Debug.Log($"[UIKit] {message}");
        }
    }
}
```

### Error Recovery

Implement fallback behavior:
```csharp
public class UIKitErrorHandler : MonoBehaviour
{
    void Start()
    {
        UIKitManager.Instance.OnTemplateAction += HandleActionSafely;
    }
    
    void HandleActionSafely(string templateId, ComponentAction action)
    {
        try
        {
            // Process action
            ProcessAction(action);
        }
        catch (Exception e)
        {
            Debug.LogError($"UIKit action error: {e}");
            
            // Fallback behavior
            if (action.Type == "Purchase")
            {
                ShowErrorDialog("Purchase temporarily unavailable");
            }
            
            // Hide problematic template
            UIKitManager.Instance.HideTemplate(templateId);
            
            // Report error
            ReportError(templateId, action, e);
        }
    }
}
```

---

## Performance Optimization

### Memory Management

#### Asset Memory
- **Texture compression**: Use compressed formats
- **Mipmap generation**: Disable for UI textures
- **Texture atlasing**: Combine small images
- **Memory limits**: Set max cache size

```csharp
// Configure texture import settings
void OptimizeTexture(Texture2D texture)
{
    texture.compress = true;
    texture.mipmapCount = 1;
    texture.anisoLevel = 1;
    texture.filterMode = FilterMode.Bilinear;
}
```

#### Component Pooling
UIKit automatically pools certain components:
- Template containers
- Common UI elements
- Reusable layouts

#### Garbage Collection
Minimize allocations:
```csharp
// Bad: Creates garbage
void UpdateEveryFrame()
{
    text.text = "Score: " + score.ToString();
}

// Good: Reuse StringBuilder
private StringBuilder _sb = new StringBuilder();
void UpdateEveryFrame()
{
    _sb.Clear();
    _sb.Append("Score: ");
    _sb.Append(score);
    text.text = _sb.ToString();
}
```

### Rendering Optimization

#### Batch Draw Calls
- **Use same materials**: Share materials across components
- **Minimize transparency**: Opaque renders faster
- **Order matters**: Render opaque first, then transparent
- **Canvas splitting**: Separate static and dynamic content

#### Reduce Overdraw
- **Occlude hidden areas**: Don't render behind opaque elements
- **Optimize hierarchy**: Flatten when possible
- **Cull efficiently**: Disable off-screen components

### Network Optimization

#### Asset Loading
```csharp
// Parallel loading
var urls = new[] { url1, url2, url3 };
AssetLoader.LoadTextures(urls, (textures) =>
{
    // All loaded
});

// Progressive loading with priority
AssetLoader.LoadTexture(heroImageUrl, OnHeroLoaded, priority: High);
AssetLoader.LoadTexture(backgroundUrl, OnBackgroundLoaded, priority: Low);
```

#### Bandwidth Management
- **Asset compression**: Use WebP or compressed formats
- **CDN usage**: Serve from edge locations
- **Caching headers**: Set appropriate cache times
- **Delta updates**: Only send changed data

### Profiling Tools

#### Unity Profiler Integration
```csharp
using UnityEngine.Profiling;

public class UIKitProfiler
{
    public static void ProfileTemplateCreation(string templateId)
    {
        Profiler.BeginSample($"UIKit.CreateTemplate.{templateId}");
        // Template creation code
        Profiler.EndSample();
    }
    
    public static void ProfileAssetLoad(string url)
    {
        Profiler.BeginSample($"UIKit.LoadAsset");
        // Asset loading code
        Profiler.EndSample();
    }
}
```

#### Custom Metrics
```csharp
public class UIKitMetrics
{
    public static void TrackTemplateMetrics(string templateId)
    {
        var metrics = new Dictionary<string, object>
        {
            { "template_id", templateId },
            { "load_time_ms", loadTime },
            { "component_count", componentCount },
            { "asset_count", assetCount },
            { "memory_mb", memoryUsed / 1024f / 1024f },
            { "draw_calls", drawCalls }
        };
        
        Analytics.TrackEvent("uikit_performance", metrics);
    }
}
```

---

## Migration Guide

### From Static UI to UIKit

#### Step 1: Identify UI Patterns
```csharp
// Before: Static UI
public class ShopDialog : MonoBehaviour
{
    public Text titleText;
    public Text priceText;
    public Button buyButton;
    public Image productImage;
    
    public void Show(ShopItem item)
    {
        titleText.text = item.name;
        priceText.text = $"${item.price}";
        productImage.sprite = item.sprite;
        gameObject.SetActive(true);
    }
}

// After: Dynamic UIKit
// Send this JSON from server:
{
  "useTemplate": true,
  "layoutSize": "MediumModal",
  "templateData": {
    "components": [
      {
        "type": "Container",
        "id": "shopDialog",
        "children": [
          {
            "type": "Text",
            "id": "title",
            "content": "{{item.name}}"
          },
          {
            "type": "Text",
            "id": "price",
            "content": "${{item.price}}"
          },
          {
            "type": "Image",
            "id": "productImage",
            "assetUrl": "{{item.imageUrl}}"
          },
          {
            "type": "Button",
            "id": "buyButton",
            "content": "Buy",
            "action": {
              "type": "Purchase",
              "data": {
                "itemId": "{{item.id}}"
              }
            }
          }
        ]
      }
    ]
  }
}
```

#### Step 2: Create Template Definitions
1. Document existing UI patterns
2. Create JSON templates for each pattern
3. Define reusable components
4. Set up theme system

#### Step 3: Implement Handlers
```csharp
public class UIKitMigrationHandler : MonoBehaviour
{
    void Start()
    {
        // Replace static UI calls with UIKit
        UIKitManager.Instance.OnTemplateAction += HandleAction;
    }
    
    void HandleAction(string templateId, ComponentAction action)
    {
        // Route to existing game logic
        switch (action.Type)
        {
            case "Purchase":
                var itemId = action.Data["itemId"].ToString();
                ShopManager.Instance.PurchaseItem(itemId);
                break;
        }
    }
}
```

#### Step 4: Gradual Migration
1. Start with simple dialogs
2. Move to complex screens
3. Keep fallbacks during transition
4. Monitor performance and errors

### Backward Compatibility

Maintain compatibility during migration:
```csharp
public class HybridUIManager : MonoBehaviour
{
    public bool UseUIKit = true;
    
    public void ShowShopDialog(ShopItem item)
    {
        if (UseUIKit && UIKitManager.Instance != null)
        {
            // Use UIKit
            SendShopTemplate(item);
        }
        else
        {
            // Fall back to static UI
            ShowStaticShopDialog(item);
        }
    }
}
```

---

## Appendix

### Glossary

| Term | Definition |
|------|------------|
| **Template** | A JSON-defined UI layout |
| **Component** | Individual UI element (text, image, button, etc.) |
| **Layout Size** | Predefined container dimensions |
| **Asset** | Remote resource (image, font, etc.) |
| **Action** | User interaction handler |
| **Theme** | Visual styling configuration |

### File Structure

```
DecisionBoxSDK/
├── Runtime/
│   ├── UIKit/
│   │   ├── Core/
│   │   │   ├── UIKitManager.cs
│   │   │   ├── DynamicTemplate.cs
│   │   │   ├── TemplateRenderer.cs
│   │   │   ├── LayoutManager.cs
│   │   │   ├── AssetLoader.cs
│   │   │   ├── AssetCache.cs
│   │   │   └── UIKitEnums.cs
│   │   ├── Components/
│   │   │   ├── IUIKitComponent.cs
│   │   │   ├── BaseComponent.cs
│   │   │   ├── ComponentFactory.cs
│   │   │   ├── ImageComponent.cs
│   │   │   ├── TextComponent.cs
│   │   │   ├── ButtonComponent.cs
│   │   │   ├── ContainerComponent.cs
│   │   │   ├── TimerComponent.cs
│   │   │   ├── ProgressBarComponent.cs
│   │   │   └── SpacerComponent.cs
│   │   └── Assets/
│   │       └── (Reserved for future use)
│   └── Models/
│       └── WebSocketMessage.cs (Extended)
├── Resources/
│   └── UIKit/
│       └── Examples/
│           ├── flash_sale_example.json
│           ├── daily_reward_example.json
│           └── simple_notification_example.json
└── Documentation/
    ├── UIKit_Usage_Guide.md
    └── UIKit_Complete_Reference.md
```

### Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024 | Initial release with core components |

### Support

For issues and questions:
- GitHub Issues: [DecisionBox Unity SDK](https://github.com/decisionbox/unity-sdk)
- Documentation: This guide
- Examples: See Resources/UIKit/Examples/

---

*This documentation is part of DecisionBox Unity SDK. For the latest updates, visit the official repository.*