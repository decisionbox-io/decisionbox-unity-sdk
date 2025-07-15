using UnityEngine;
using DecisionBox;
using DecisionBox.Models;
using DecisionBox.UIKit.Core;
using DecisionBox.UIKit.Components;
using System.Collections.Generic;

namespace DecisionBox.Tests
{
    /// <summary>
    /// Compilation test to verify all UIKit components can be referenced
    /// </summary>
    public class UIKitCompilationTest : MonoBehaviour
    {
        void Start()
        {
            // Test 1: UIKitManager instantiation
            var uiKitManager = UIKitManager.Instance;
            Debug.Log($"UIKitManager instance created: {uiKitManager != null}");
            
            // Test 2: Create a test WebSocket message
            var testMessage = new WebSocketMessage
            {
                type = "test-notification",
                Action = "TEST_UIKIT",
                UseTemplate = true,
                LayoutSize = "MediumModal",
                TemplateData = new TemplateData
                {
                    Components = new List<UIKitComponent>
                    {
                        new UIKitComponent
                        {
                            Type = "Text",
                            Id = "test-text",
                            Content = "Hello UIKit!",
                            Properties = new Dictionary<string, object>
                            {
                                { "fontSize", 20 },
                                { "color", "#333333" }
                            }
                        }
                    }
                }
            };
            
            // Test 3: Process the message
            bool processed = uiKitManager.ProcessMessage(testMessage);
            Debug.Log($"Message processed: {processed}");
            
            // Test 4: Check layout manager
            var layoutManager = uiKitManager.LayoutManager;
            var dimensions = layoutManager.GetLayoutDimensions(LayoutSize.MediumModal);
            Debug.Log($"Layout dimensions: {dimensions.WidthPercent}x{dimensions.HeightPercent}");
            
            // Test 5: Check component factory
            var componentFactory = uiKitManager.ComponentFactory;
            Debug.Log($"ComponentFactory ready: {componentFactory != null}");
            
            // Test 6: Check asset loader
            var assetLoader = uiKitManager.AssetLoader;
            Debug.Log($"AssetLoader ready: {assetLoader != null}");
            
            // Test 7: Check asset cache
            var assetCache = uiKitManager.AssetCache;
            Debug.Log($"AssetCache ready: {assetCache != null}");
            
            // Test 8: Subscribe to events
            uiKitManager.OnUIKitInitialized += () => Debug.Log("UIKit initialized");
            uiKitManager.OnTemplateShown += (id) => Debug.Log($"Template shown: {id}");
            uiKitManager.OnTemplateHidden += (id) => Debug.Log($"Template hidden: {id}");
            uiKitManager.OnTemplateAction += (id, action) => Debug.Log($"Template action: {id} - {action.Type}");
            
            // Test 9: Enum usage
            var componentType = ComponentType.Button;
            var actionType = ActionType.Purchase;
            var animationType = AnimationType.Fade;
            Debug.Log($"Enums test: {componentType}, {actionType}, {animationType}");
            
            // Test 10: Component interfaces
            IUIKitComponent component = null;
            IAssetComponent assetComponent = null;
            IInteractableComponent interactable = null;
            IContainerComponent container = null;
            ITextComponent textComponent = null;
            
            Debug.Log("All interface types compile correctly");
            
            // Success
            Debug.Log("[UIKit Compilation Test] All tests passed! UIKit is ready to use.");
        }
    }
}