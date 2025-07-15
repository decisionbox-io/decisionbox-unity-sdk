# DecisionBox UIKit Usage Guide

## Overview

DecisionBox UIKit is a dynamic UI rendering system that allows you to create and display UI templates through WebSocket messages. Instead of hardcoding UI elements in your Unity game, you can send JSON messages that describe the UI structure, and UIKit will render them dynamically.

## Key Features

- **Dynamic UI Creation**: Build UIs from JSON without code changes
- **Remote Asset Loading**: Load images and resources from URLs
- **Layout Constraints**: Predefined layout sizes for consistent UI across devices
- **Component System**: Modular components (Image, Text, Button, Container, etc.)
- **Theme Support**: Apply consistent styling across components
- **Action Handling**: Handle user interactions with built-in or custom actions

## How It Works

1. Your server sends a WebSocket message with `useTemplate: true`
2. The SDK detects this and passes it to UIKit
3. UIKit creates the UI based on the template data
4. Assets are loaded from provided URLs
5. User interactions trigger actions that can be handled by your game

## Message Structure

```json
{
  "action": "YOUR_ACTION_TYPE",
  "type": "sdk-notification",
  "useTemplate": true,              // Required: Enables UIKit processing
  "layoutSize": "MediumModal",      // Required: Layout constraint
  "templateData": {                 // Required: UI structure
    "components": [...],            // Array of UI components
    "theme": {...},                 // Optional: Theme overrides
    "animations": {...}             // Optional: Animation settings
  },
  "payload": {...}                  // Your custom data
}
```

## Layout Sizes

UIKit provides predefined layout sizes to ensure consistent UI across different screen sizes:

| Layout Size | Description | Dimensions |
|------------|-------------|------------|
| `FullScreen` | Takes entire screen | 100% × 100% |
| `LargeModal` | Large centered modal | 90% × 80% |
| `MediumModal` | Medium centered modal | 80% × 60% |
| `SmallModal` | Small centered modal | 70% × 40% |
| `TopBanner` | Top notification banner | 100% × 15% |
| `BottomSheet` | Bottom sliding panel | 100% × 50% |
| `CenterSquare` | Square in center | 80% of min dimension |
| `FloatingButton` | Fixed floating button | 80×80 dp |
| `SidePanel` | Side panel (tablets) | 30% × 100% |

## Component Types

### Image Component
Displays images loaded from URLs.

```json
{
  "type": "Image",
  "id": "heroImage",
  "size": "Hero",  // or: Tiny, Small, Medium, Large, Hero, Banner
  "assetUrl": "https://example.com/image.png",
  "properties": {
    "scaleMode": "aspectFit",  // fill, fit, aspectFit, aspectFill
    "tint": "#FF0000"
  }
}
```

### Text Component
Displays text with styling options.

```json
{
  "type": "Text",
  "id": "title",
  "size": "Large",
  "content": "Hello World",
  "properties": {
    "fontSize": 24,
    "color": "#333333",
    "fontWeight": "bold",      // normal, bold, italic, bolditalic
    "alignment": "center",      // left, center, right
    "wrap": true
  }
}
```

### Button Component
Interactive button with action handling.

```json
{
  "type": "Button",
  "id": "buyButton",
  "size": "Large",
  "content": "Buy Now",
  "action": {
    "type": "Purchase",         // or: Close, SendEvent, OpenUrl, Custom
    "data": {
      "productId": "item_123"
    }
  },
  "properties": {
    "backgroundColor": "#2ECC71",
    "textColor": "#FFFFFF",
    "fontSize": 18,
    "cornerRadius": 8
  }
}
```

### Container Component
Groups and layouts child components.

```json
{
  "type": "Container",
  "id": "mainContainer",
  "properties": {
    "layout": "vertical",       // vertical, horizontal, grid, none
    "spacing": 10,
    "padding": 20,
    "backgroundColor": "#FFFFFF",
    "childAlignment": "middlecenter"
  },
  "children": [...]
}
```

### Timer Component
Countdown timer with auto-start capability.

```json
{
  "type": "Timer",
  "id": "countdown",
  "properties": {
    "duration": 3600,           // Seconds
    "format": "Ends in {0:00}:{1:00}:{2:00}",
    "autoStart": true,
    "fontSize": 16,
    "color": "#E74C3C"
  }
}
```

### ProgressBar Component
Visual progress indicator.

```json
{
  "type": "ProgressBar",
  "id": "progress",
  "properties": {
    "value": 0.7,               // 0.0 to 1.0
    "min": 0,
    "max": 100,
    "height": 20,
    "backgroundColor": "#E0E0E0",
    "fillColor": "#4CAF50",
    "showLabel": true,
    "labelFormat": "{0:0}%"
  }
}
```

### Spacer Component
Adds flexible or fixed space in layouts.

```json
{
  "type": "Spacer",
  "id": "spacer1",
  "properties": {
    "type": "flexible",         // flexible, fixed, horizontal, vertical
    "height": 20                // For fixed spacers
  }
}
```

## Component Sizes

Components can use predefined size constraints:

- `Tiny`: 32×32 images, 12pt text
- `Small`: 64×64 images, 14pt text
- `Medium`: 128×128 images, 16pt text
- `Large`: 256×256 images, 20pt text
- `ExtraLarge`: 384×384 images, 24pt text
- `Hero`: 16:9 aspect ratio, responsive
- `Banner`: 3:1 aspect ratio, responsive
- `Fill`: Fills available space

## Action Types

UIKit supports various action types for user interactions:

| Action Type | Description | Required Data |
|------------|-------------|---------------|
| `Close` | Closes the template | None |
| `SendEvent` | Sends custom event to game | `eventType`, custom data |
| `Purchase` | Triggers in-app purchase | `productId`, price info |
| `OpenUrl` | Opens external URL | `url` |
| `Navigate` | Opens another template | `templateId` |
| `Custom` | Custom game action | Any custom data |

## Handling Actions in Your Game

```csharp
void Start()
{
    // Subscribe to UIKit actions
    UIKitManager.Instance.OnTemplateAction += HandleUIKitAction;
}

void HandleUIKitAction(string templateId, ComponentAction action)
{
    switch (action.Type)
    {
        case "Purchase":
            var productId = action.Data["productId"].ToString();
            // Handle purchase
            break;
            
        case "SendEvent":
            var eventType = action.Data["eventType"].ToString();
            // Handle custom event
            break;
    }
}
```

## Theme System

Apply consistent styling across all components:

```json
"theme": {
  "primaryColor": "#2ECC71",
  "secondaryColor": "#E74C3C",
  "backgroundColor": "#FFFFFF",
  "textColor": "#333333",
  "buttonColor": "#3498DB",
  "buttonTextColor": "#FFFFFF",
  "fontSize": 16
}
```

## Best Practices

1. **Asset Optimization**
   - Use appropriate image sizes for different resolutions
   - Compress images to reduce loading time
   - Provide @1x, @2x, @3x versions for different screen densities

2. **Layout Design**
   - Use appropriate layout sizes for your content
   - Test on different screen sizes and orientations
   - Use containers to group related components

3. **Performance**
   - Preload assets when possible
   - Reuse templates instead of creating new ones
   - Limit the number of simultaneous templates

4. **User Experience**
   - Provide clear close/dismiss options
   - Use appropriate animations
   - Ensure text is readable on all backgrounds

## Example: Flash Sale Template

```json
{
  "action": "FLASH_SALE",
  "type": "sdk-notification",
  "useTemplate": true,
  "layoutSize": "MediumModal",
  "templateData": {
    "components": [
      {
        "type": "Container",
        "id": "main",
        "size": "Fill",
        "properties": {
          "layout": "vertical",
          "backgroundColor": "#FFFFFF",
          "padding": 20,
          "spacing": 15
        },
        "children": [
          {
            "type": "Image",
            "id": "hero",
            "size": "Hero",
            "assetUrl": "https://cdn.example.com/sale-banner.png"
          },
          {
            "type": "Text",
            "id": "title",
            "content": "LIMITED TIME OFFER!",
            "properties": {
              "fontSize": 24,
              "color": "#E74C3C",
              "fontWeight": "bold",
              "alignment": "center"
            }
          },
          {
            "type": "Timer",
            "id": "countdown",
            "properties": {
              "duration": 3600,
              "autoStart": true,
              "format": "Ends in: {0:00}:{1:00}:{2:00}"
            }
          },
          {
            "type": "Button",
            "id": "buy",
            "content": "BUY NOW - $2.99",
            "action": {
              "type": "Purchase",
              "data": {"productId": "sale_item_001"}
            },
            "properties": {
              "backgroundColor": "#2ECC71",
              "fontSize": 18
            }
          }
        ]
      }
    ]
  }
}
```

## Debugging

Enable UIKit debug logs:
```csharp
Debug.Log("[DecisionBox UIKit] ...");
```

Common issues:
- **Assets not loading**: Check URLs are accessible and CORS headers are set
- **Layout issues**: Verify component sizes and container properties
- **Actions not firing**: Ensure action handlers are subscribed

## Migration from Static UI

To migrate existing static UI to UIKit:

1. Identify UI patterns in your game
2. Create JSON templates for each pattern
3. Replace static UI code with UIKit message triggers
4. Test across different devices and screen sizes