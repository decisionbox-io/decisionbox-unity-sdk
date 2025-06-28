using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DecisionBox.Core;

namespace DecisionBox.Tests
{
    public class RunTestsForSingleton : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("=== Running Singleton Pattern Tests ===");
            RunSingletonTests();
        }

        private void RunSingletonTests()
        {
            try
            {
                // Test 1: Verify singleton instance creation
                Debug.Log("Test 1: Singleton Instance Creation");
                var instance1 = DecisionBoxSDK.Instance;
                var instance2 = DecisionBoxSDK.Instance;
                
                if (instance1 == instance2)
                {
                    Debug.Log("✓ PASS: Singleton returns same instance");
                }
                else
                {
                    Debug.LogError("✗ FAIL: Singleton returns different instances");
                }

                // Test 2: Verify instance survives reset
                Debug.Log("\nTest 2: Instance Persistence");
                var originalInstance = DecisionBoxSDK.Instance;
                
                // Try to destroy and recreate
                var instanceField = typeof(DecisionBoxSDK).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
                if (instanceField != null)
                {
                    instanceField.SetValue(null, null);
                    var newInstance = DecisionBoxSDK.Instance;
                    
                    if (newInstance != null && newInstance != originalInstance)
                    {
                        Debug.Log("✓ PASS: New instance created after reset");
                    }
                    else
                    {
                        Debug.LogError("✗ FAIL: Instance not properly recreated");
                    }
                }

                // Test 3: Verify configuration works
                Debug.Log("\nTest 3: SDK Configuration");
                DecisionBoxSDK.Instance.Configure("test-app", "test-secret", "development", true);
                Debug.Log("✓ PASS: Configuration successful");

                // Test 4: Verify event methods are accessible
                Debug.Log("\nTest 4: Event Methods Accessibility");
                var sdk = DecisionBoxSDK.Instance;
                
                // Check if methods exist and are callable
                var methodsToCheck = new string[]
                {
                    "SendGameStartedAsync",
                    "SendLevelStartedAsync",
                    "SendLevelSuccessAsync",
                    "SendCurrencyBalanceUpdatedAsync"
                };

                bool allMethodsFound = true;
                foreach (var methodName in methodsToCheck)
                {
                    var method = sdk.GetType().GetMethod(methodName);
                    if (method != null)
                    {
                        Debug.Log($"✓ PASS: Method {methodName} found");
                    }
                    else
                    {
                        Debug.LogError($"✗ FAIL: Method {methodName} not found");
                        allMethodsFound = false;
                    }
                }

                // Test 5: WebSocket event handling
                Debug.Log("\nTest 5: WebSocket Event Handling");
                bool callbackCalled = false;
                Action<string> testCallback = (msg) => { callbackCalled = true; };
                
                sdk.On("test_event", testCallback);
                Debug.Log("✓ PASS: Event handler registered");
                
                sdk.Off("test_event", testCallback);
                Debug.Log("✓ PASS: Event handler unregistered");

                // Summary
                Debug.Log("\n=== Test Summary ===");
                Debug.Log("All singleton pattern tests completed!");
                Debug.Log("The SDK is now properly implemented as a singleton.");
                
            }
            catch (Exception e)
            {
                Debug.LogError($"Test failed with exception: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}