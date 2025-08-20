# DecisionBox Unity SDK

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![Package Manager](https://img.shields.io/badge/Package%20Manager-UPM-green.svg)](https://docs.unity3d.com/Manual/upm-ui.html)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE.md)

Advanced analytics and real-time insights for Unity games with strongly-typed events and automatic session management.

## 🚀 Features

- **Strongly-Typed Events**: No string-based event names - use enums and type-safe methods
- **Automatic Session Management**: Sessions start/end automatically based on app lifecycle
- **Remote Configuration**: SDK behavior controlled by remote config
- **Real-time WebSocket**: Bidirectional communication with authentication
- **Unity Native**: Built specifically for Unity with MonoBehaviour integration
- **Type-Safe Enums**: All event parameters use enums instead of strings
- **Automatic User ID**: Generates and persists user ID if not provided
- **Background Handling**: Proper session management on app pause/resume
- **Push Notifications**: Send device tokens for push notification capability

## 📦 Installation

### Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **"Add package from git URL"**
3. Enter: `https://github.com/decisionbox-io/decisionbox-unity-sdk.git`
4. Click **Add**

## 🎯 Quick Start

### 1. Initial Setup (One-time)

#### Option A: Using the Setup Window (Recommended)
1. Go to **DecisionBox > Setup SDK** in Unity menu
2. Enter your App ID and App Secret
3. Click "Create Configured SDK GameObject"

#### Option B: Manual Setup
1. Go to **DecisionBox > Create SDK GameObject** in Unity menu
2. Configure the SDK in the Inspector:
   - **App ID**: Your DecisionBox application ID
   - **App Secret**: Your DecisionBox application secret  
   - **Environment**: `production` or `development`
   - **Enable Logging**: Check for debug logs

The SDK GameObject will persist across scenes (DontDestroyOnLoad).

### 2. Access SDK from Anywhere

Once the SDK GameObject is created, you can access it directly from any script:

```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameManager : MonoBehaviour
{
    private async void Start()
    {
        // Access the SDK instance directly
        var sdk = DecisionBoxSDK.Instance;
        
        // Initialize SDK (only needed once, usually at game start)
        bool initialized = await sdk.InitializeAsync();
        
        if (initialized)
        {
            Debug.Log("DecisionBox SDK initialized!");
            // SDK automatically sends SessionStarted event
        }
    }
}
```

### 3. Use SDK Throughout Your Game

From any script in your game:

```csharp
// Send events from anywhere - no need to store reference
await DecisionBoxSDK.Instance.SendLevelStartedAsync(levelNumber: 1);
await DecisionBoxSDK.Instance.SendScoreUpdatedAsync(levelNumber: 1, currentScore: 100);
```

### 4. Send Strongly-Typed Events

All events use enums and strongly-typed parameters:

```csharp
// Game events
await DecisionBoxSDK.Instance.SendGameStartedAsync(
    initialSoftCurrency: 100,
    initialHardCurrency: 50
);

// Level events with enums
await DecisionBoxSDK.Instance.SendLevelStartedAsync(
    levelNumber: 1,
    levelName: "Tutorial",
    difficulty: "Easy"
);

await DecisionBoxSDK.Instance.SendLevelFailedAsync(
    levelNumber: 1,
    failureReason: FailureReason.TimeOut  // Enum, not string!
);

await DecisionBoxSDK.Instance.SendLevelRestartedAsync(
    levelNumber: 1,
    restartReason: RestartReason.AfterFailure,  // Why player restarted
    attemptNumber: 2  // Optional: which attempt this is
);

// Currency events with strongly-typed enums
await DecisionBoxSDK.Instance.SendCurrencyBalanceUpdatedAsync(
    currencyType: CurrencyType.Soft,     // Enum
    oldBalance: 100,
    currentBalance: 150,
    delta: 50,
    updateReason: CurrencyUpdateReason.Reward  // Enum
);

// Booster events
await DecisionBoxSDK.Instance.SendBoosterOfferedAsync(
    levelNumber: 1,
    boosterType: BoosterType.SpeedBoost,  // Enum
    offerMethod: OfferMethod.WatchAd,     // Enum
    requiredCurrency: 0
);

// Track when player uses a booster from inventory
await DecisionBoxSDK.Instance.SendBoosterUsedAsync(
    levelNumber: 1,
    boosterType: BoosterType.SpeedBoost,
    source: BoosterSource.Inventory,  // Where it was activated from
    quantity: 1,
    remainingCount: 3  // Optional: boosters left in inventory
);

// Metrics
await DecisionBoxSDK.Instance.SendMetricRecordedAsync(
    levelNumber: 1,
    metric: MetricType.Score,  // Enum
    metricValue: 1500f
);
```

## 📚 Event Types

### Available Events

All events are strongly-typed methods on the SDK:

- `SendGameStartedAsync` - Game session started
- `SendGameEndedAsync` - Game session ended  
- `SendLevelStartedAsync` - Level began
- `SendLevelSuccessAsync` - Level completed successfully
- `SendLevelFailedAsync` - Level failed
- `SendLevelRestartedAsync` - Level restarted
- `SendScoreUpdatedAsync` - Score changed
- `SendPowerUpCollectedAsync` - Power-up collected
- `SendCurrencyBalanceUpdatedAsync` - Currency balance changed
- `SendBoosterOfferedAsync` - Booster offered to player
- `SendBoosterAcceptedAsync` - Booster accepted
- `SendBoosterDeclinedAsync` - Booster declined
- `SendBoosterUsedAsync` - Booster used from inventory
- `SendMetricRecordedAsync` - Custom metric recorded
- `SendUserLocationTextAsync` - Text location
- `SendUserLocationLatLngAsync` - GPS coordinates
- `SendUserLocationIPAsync` - IP address location
- `SendUserDeviceTokenAsync` - Device token for push notifications

### Enums Reference

All enums are accessible via the `DecisionBox.Models` namespace:

```csharp
// Currency types
CurrencyType.Soft
CurrencyType.Hard  
CurrencyType.Premium

// Failure reasons
FailureReason.TimeOut
FailureReason.NoLives
FailureReason.NotEnoughMoves
// ... and more

// Restart reasons
RestartReason.PlayerChoice
RestartReason.AfterFailure
RestartReason.Practice
RestartReason.Achievement
// ... and more

// Booster types
BoosterType.SpeedBoost
BoosterType.Shield
BoosterType.ExtraLife
// ... and more

// Booster sources (where activated from)
BoosterSource.Inventory
BoosterSource.PreGame
BoosterSource.InGame
BoosterSource.Store
// ... and more

// Metrics
MetricType.Score
MetricType.LivesRemaining
MetricType.EnemiesDefeated
MetricType.TotalClicks
// ... and more
```

## 📱 Push Notifications

Send device tokens to enable push notifications:

```csharp
// Send device token for push notifications
await DecisionBoxSDK.Instance.SendUserDeviceTokenAsync(
    userId: "user123",  // Optional - uses current user ID if not provided
    deviceToken: "your-device-token-here"
);

// Example with Firebase/FCM
public async void OnTokenReceived(string token)
{
    await DecisionBoxSDK.Instance.SendUserDeviceTokenAsync(deviceToken: token);
}
```

## 🔌 WebSocket Support

Real-time bidirectional communication:

```csharp
// Register for custom events
DecisionBoxSDK.Instance.On("offer_available", (message) => {
    Debug.Log($"Received offer: {message}");
});

// Unregister when done
DecisionBoxSDK.Instance.Off("offer_available", callback);
```

## 🎮 Complete Example

```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameController : MonoBehaviour
{
    private int currentLevel = 1;
    
    private async void Start()
    {
        // Initialize SDK (assumes SDK GameObject already exists in scene)
        bool initialized = await DecisionBoxSDK.Instance.InitializeAsync();
        if (!initialized) 
        {
            Debug.LogError("Failed to initialize DecisionBox SDK!");
            return;
        }
        
        // Send device token if available (e.g., from Firebase)
        string deviceToken = GetDeviceToken(); // Your method to get token
        if (!string.IsNullOrEmpty(deviceToken))
        {
            await DecisionBoxSDK.Instance.SendUserDeviceTokenAsync(deviceToken: deviceToken);
        }
        
        // Start game
        await DecisionBoxSDK.Instance.SendGameStartedAsync(
            initialSoftCurrency: 100,
            initialHardCurrency: 10
        );
        
        StartLevel();
    }
    
    private async void StartLevel()
    {
        await DecisionBoxSDK.Instance.SendLevelStartedAsync(
            levelNumber: currentLevel,
            levelName: $"Level {currentLevel}",
            difficulty: currentLevel < 5 ? "Easy" : "Hard"
        );
    }
    
    public async void OnLevelComplete(int score)
    {
        var sdk = DecisionBoxSDK.Instance;
        
        // Send success event
        await sdk.SendLevelSuccessAsync(levelNumber: currentLevel);
        
        // Update score
        await sdk.SendScoreUpdatedAsync(
            levelNumber: currentLevel,
            currentScore: score
        );
        
        // Award currency
        await sdk.SendCurrencyBalanceUpdatedAsync(
            currencyType: CurrencyType.Soft,
            oldBalance: GetCurrency(CurrencyType.Soft),
            currentBalance: GetCurrency(CurrencyType.Soft) + 50,
            delta: 50,
            updateReason: CurrencyUpdateReason.Reward
        );
        
        currentLevel++;
        StartLevel();
    }
    
    public async void OnLevelFailed(FailureReason reason)
    {
        await DecisionBoxSDK.Instance.SendLevelFailedAsync(
            levelNumber: currentLevel,
            failureReason: reason
        );
    }
    
    public async void OnBoosterPurchased(BoosterType booster)
    {
        await DecisionBoxSDK.Instance.SendBoosterAcceptedAsync(
            levelNumber: currentLevel,
            boosterType: booster,
            acceptMethod: AcceptMethod.SpendCurrency,
            spentAmount: 100
        );
    }
    
    private int GetCurrency(CurrencyType type)
    {
        // Your currency logic
        return 100;
    }
    
    private string GetDeviceToken()
    {
        // Your device token retrieval logic (e.g., from Firebase)
        return "";
    }
}
```

## 🔧 Advanced Features

### Session Management

Sessions are managed automatically:
- Started when SDK initializes
- Ended on app quit
- New session after background timeout (configurable via remote config)
- Automatic session timeout handling

### Remote Configuration

The SDK fetches configuration from the server:
- SDK enabled/disabled state
- Session duration limits
- WebSocket settings
- Event batching configuration

### Unity Lifecycle

The SDK handles Unity lifecycle events:
- `OnApplicationPause` - Background/foreground handling
- `OnApplicationQuit` - Clean session termination
- `Update` - WebSocket message processing

## 📱 Platform Support

- **iOS**: Full support
- **Android**: Full support  
- **WebGL**: Full support
- **Windows/Mac/Linux**: Full support

## 🛠️ Requirements

- **Unity**: 2020.3 or later
- **C# Version**: C# 8.0+ (nullable reference types)
- **Dependencies**: 
  - Newtonsoft.Json
  - NativeWebSocket (for WebSocket support)

## 🔒 Security

- JWT token authentication
- HTTPS-only communication
- Automatic token refresh
- No personal data collection by default

## 📄 License

This SDK is proprietary software. Use requires an active DecisionBox subscription. See the [LICENSE.md](LICENSE.md) file for details.

---

<p align="center">
  <strong>Built with ❤️ for Unity developers by DecisionBox</strong>
</p>