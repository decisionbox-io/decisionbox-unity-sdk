# DecisionBox SDK Connection Callbacks

## Overview
The DecisionBox Unity SDK now provides comprehensive connection state management and callbacks to let you know when the WebSocket is connected and authenticated.

## Connection States
The SDK goes through the following connection states:

```csharp
public enum ConnectionState
{
    Disconnected,      // Not connected
    Initializing,      // SDK is initializing
    Connecting,        // WebSocket is connecting
    Connected,         // WebSocket connected but not authenticated
    Authenticating,    // Sending authentication to server
    Authenticated,     // Fully authenticated and ready
    Reconnecting,      // Attempting to reconnect
    Failed            // Connection failed
}
```

## Available Properties

```csharp
// Check if SDK is initialized
bool isInitialized = DecisionBoxSDK.Instance.IsInitialized;

// Check if WebSocket is connected
bool isConnected = DecisionBoxSDK.Instance.IsWebSocketConnected;

// Check if WebSocket is authenticated
bool isAuthenticated = DecisionBoxSDK.Instance.IsWebSocketAuthenticated;

// Check if SDK is fully ready (initialized, connected, and authenticated)
bool isReady = DecisionBoxSDK.Instance.IsReady;

// Get current connection state
var currentState = DecisionBoxSDK.Instance.CurrentConnectionState;
```

## Event Callbacks

### 1. Connection State Changed
Triggered whenever the connection state changes:

```csharp
void Start()
{
    var sdk = DecisionBoxSDK.Instance;
    
    // Subscribe to connection state changes
    sdk.ConnectionStateChanged += OnConnectionStateChanged;
}

private void OnConnectionStateChanged(DecisionBoxSDK.ConnectionState previousState, 
                                     DecisionBoxSDK.ConnectionState newState, 
                                     string message)
{
    Debug.Log($"Connection state changed from {previousState} to {newState}");
    if (!string.IsNullOrEmpty(message))
    {
        Debug.Log($"Message: {message}");
    }
    
    // Update UI based on state
    UpdateConnectionUI(newState);
}
```

### 2. SDK Ready Callback
Triggered when the SDK is fully initialized, WebSocket is connected and authenticated:

```csharp
void Start()
{
    var sdk = DecisionBoxSDK.Instance;
    
    // Subscribe to SDK ready event
    sdk.SDKReady += OnSDKReady;
    
    // Initialize SDK
    StartCoroutine(InitializeSDK());
}

private void OnSDKReady(string sessionId, string userId)
{
    Debug.Log($"SDK is ready! Session: {sessionId}, User: {userId}");
    
    // SDK is now fully ready to receive notifications
    // Register your notification handlers here
    sdk.On("BOOSTER_GRANTED", HandleBoosterGranted);
    sdk.On("OFFER_AVAILABLE", HandleOfferAvailable);
    
    // Enable game features that require SDK
    EnableOnlineFeatures();
}
```

### 3. Connection Error Callback
Triggered when a connection error occurs:

```csharp
void Start()
{
    var sdk = DecisionBoxSDK.Instance;
    
    // Subscribe to connection errors
    sdk.ConnectionError += OnConnectionError;
}

private void OnConnectionError(string error, DecisionBoxSDK.ConnectionState state)
{
    Debug.LogError($"Connection error in state {state}: {error}");
    
    // Handle error based on state
    switch (state)
    {
        case DecisionBoxSDK.ConnectionState.Initializing:
            ShowError("Failed to initialize SDK. Check your credentials.");
            break;
        case DecisionBoxSDK.ConnectionState.Connecting:
            ShowError("Failed to connect. Check your internet connection.");
            break;
        case DecisionBoxSDK.ConnectionState.Authenticating:
            ShowError("Authentication failed. Please try again.");
            break;
        default:
            ShowError($"Connection error: {error}");
            break;
    }
}
```

## Complete Example

```csharp
using UnityEngine;
using DecisionBox.Core;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private DecisionBoxSDK sdk;
    private bool isSDKReady = false;
    
    void Start()
    {
        sdk = DecisionBoxSDK.Instance;
        
        // Subscribe to all events
        sdk.ConnectionStateChanged += OnConnectionStateChanged;
        sdk.SDKReady += OnSDKReady;
        sdk.ConnectionError += OnConnectionError;
        
        // Initialize SDK
        StartCoroutine(InitializeSDK());
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (sdk != null)
        {
            sdk.ConnectionStateChanged -= OnConnectionStateChanged;
            sdk.SDKReady -= OnSDKReady;
            sdk.ConnectionError -= OnConnectionError;
        }
    }
    
    IEnumerator InitializeSDK()
    {
        // Wait for SDK to be available
        while (DecisionBoxSDK.Instance == null)
        {
            yield return null;
        }
        
        // Initialize with user ID
        string userId = GetOrCreateUserId();
        bool success = await sdk.InitializeAsync(userId);
        
        if (!success)
        {
            Debug.LogError("Failed to initialize DecisionBox SDK");
        }
    }
    
    private void OnConnectionStateChanged(DecisionBoxSDK.ConnectionState previousState, 
                                         DecisionBoxSDK.ConnectionState newState, 
                                         string message)
    {
        Debug.Log($"[DecisionBox] State: {previousState} -> {newState}");
        
        // Update UI to show connection status
        UpdateConnectionStatusUI(newState);
    }
    
    private void OnSDKReady(string sessionId, string userId)
    {
        Debug.Log($"[DecisionBox] Ready! Session: {sessionId}, User: {userId}");
        isSDKReady = true;
        
        // Register notification handlers
        RegisterNotificationHandlers();
        
        // Enable online features
        EnableOnlineFeatures();
    }
    
    private void OnConnectionError(string error, DecisionBoxSDK.ConnectionState state)
    {
        Debug.LogError($"[DecisionBox] Error in state {state}: {error}");
        
        // Show error to player
        ShowConnectionError(error);
        
        // Retry logic if needed
        if (shouldRetry)
        {
            StartCoroutine(RetryConnection());
        }
    }
    
    private void RegisterNotificationHandlers()
    {
        // Register handlers for ruleset actions
        sdk.On("BOOSTER_GRANTED", (data) => {
            Debug.Log($"Booster received: {data}");
            // Handle booster award
        });
        
        sdk.On("OFFER_AVAILABLE", (data) => {
            Debug.Log($"Offer available: {data}");
            // Show offer UI
        });
        
        sdk.On("DIFFICULTY_ADJUSTED", (data) => {
            Debug.Log($"Difficulty adjusted: {data}");
            // Apply difficulty changes
        });
    }
    
    private void UpdateConnectionStatusUI(DecisionBoxSDK.ConnectionState state)
    {
        // Update your UI based on connection state
        switch (state)
        {
            case DecisionBoxSDK.ConnectionState.Disconnected:
                connectionIcon.color = Color.red;
                connectionText.text = "Offline";
                break;
            case DecisionBoxSDK.ConnectionState.Connecting:
            case DecisionBoxSDK.ConnectionState.Authenticating:
                connectionIcon.color = Color.yellow;
                connectionText.text = "Connecting...";
                break;
            case DecisionBoxSDK.ConnectionState.Authenticated:
                connectionIcon.color = Color.green;
                connectionText.text = "Connected";
                break;
            case DecisionBoxSDK.ConnectionState.Failed:
                connectionIcon.color = Color.red;
                connectionText.text = "Connection Failed";
                break;
        }
    }
}
```

## Checking Connection State

You can check the connection state at any time:

```csharp
void Update()
{
    // Check if SDK is ready before sending events
    if (DecisionBoxSDK.Instance.IsReady)
    {
        // Safe to send events
        SendGameEvents();
    }
    
    // Or check specific states
    if (DecisionBoxSDK.Instance.CurrentConnectionState == DecisionBoxSDK.ConnectionState.Authenticated)
    {
        // Fully connected and authenticated
    }
}
```

## Best Practices

1. **Always wait for SDKReady event** before registering notification handlers
2. **Check IsReady property** before sending events
3. **Handle connection errors gracefully** with retry logic
4. **Unsubscribe from events** when your component is destroyed
5. **Update UI based on connection state** to keep players informed

## State Flow

Normal connection flow:
```
Disconnected -> Initializing -> Connecting -> Connected -> Authenticating -> Authenticated
```

On error:
```
Any State -> Failed
```

On disconnect:
```
Any State -> Disconnected
```