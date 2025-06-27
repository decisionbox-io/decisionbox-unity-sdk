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

## 📦 Installation

### Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **"Add package from git URL"**
3. Enter: `https://github.com/decisionbox-io/decisionbox-unity-sdk.git`
4. Click **Add**

## 🎯 Quick Start

### 1. Setup SDK

Create a GameObject with the DecisionBoxSDK component:

```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameManager : MonoBehaviour
{
    private DecisionBoxSDK sdk;
    
    private async void Start()
    {
        // Find or create SDK GameObject
        var sdkObject = GameObject.Find("DecisionBoxSDK");
        if (sdkObject == null)
        {
            sdkObject = new GameObject("DecisionBoxSDK");
            DontDestroyOnLoad(sdkObject);
        }
        
        // Add SDK component
        sdk = sdkObject.GetComponent<DecisionBoxSDK>();
        if (sdk == null)
        {
            sdk = sdkObject.AddComponent<DecisionBoxSDK>();
        }
        
        // Initialize SDK (null userId will generate a GUID)
        bool initialized = await sdk.InitializeAsync(userId: null);
        
        if (initialized)
        {
            Debug.Log("DecisionBox SDK initialized!");
            // SDK automatically sends SessionStarted event
        }
    }
}
```

### 2. Configure in Inspector

1. Select the DecisionBoxSDK GameObject
2. Configure in the Inspector:
   - **App ID**: Your DecisionBox application ID
   - **App Secret**: Your DecisionBox application secret  
   - **API URL**: `https://api.decisionbox.io` (default)
   - **Enable Logging**: Check for debug logs

### 3. Send Strongly-Typed Events

All events use enums and strongly-typed parameters:

```csharp
// Game events
await sdk.SendGameStartedAsync(
    initialSoftCurrency: 100,
    initialHardCurrency: 50
);

// Level events with enums
await sdk.SendLevelStartedAsync(
    levelNumber: 1,
    levelName: "Tutorial",
    difficulty: "Easy"
);

await sdk.SendLevelFailedAsync(
    levelNumber: 1,
    failureReason: FailureReason.TimeOut  // Enum, not string!
);

// Currency events with strongly-typed enums
await sdk.SendCurrencyBalanceUpdatedAsync(
    currencyType: CurrencyType.Soft,     // Enum
    oldBalance: 100,
    currentBalance: 150,
    delta: 50,
    updateReason: CurrencyUpdateReason.Reward  // Enum
);

// Booster events
await sdk.SendBoosterOfferedAsync(
    levelNumber: 1,
    boosterType: BoosterType.SpeedBoost,  // Enum
    offerMethod: OfferMethod.WatchAd,     // Enum
    requiredCurrency: 0
);

// Metrics
await sdk.SendMetricRecordedAsync(
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
- `SendScoreUpdatedAsync` - Score changed
- `SendPowerUpCollectedAsync` - Power-up collected
- `SendCurrencyBalanceUpdatedAsync` - Currency balance changed
- `SendBoosterOfferedAsync` - Booster offered to player
- `SendBoosterAcceptedAsync` - Booster accepted
- `SendBoosterDeclinedAsync` - Booster declined
- `SendMetricRecordedAsync` - Custom metric recorded
- `SendUserLocationTextAsync` - Text location
- `SendUserLocationLatLngAsync` - GPS coordinates
- `SendUserLocationIPAsync` - IP address location

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

// Booster types
BoosterType.SpeedBoost
BoosterType.Shield
BoosterType.ExtraLife
// ... and more

// Metrics
MetricType.Score
MetricType.LivesRemaining
MetricType.EnemiesDefeated
// ... and more
```

## 🔌 WebSocket Support

Real-time bidirectional communication:

```csharp
// Register for custom events
sdk.On("offer_available", (message) => {
    Debug.Log($"Received offer: {message}");
});

// Unregister when done
sdk.Off("offer_available", callback);
```

## 🎮 Complete Example

```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameController : MonoBehaviour
{
    private DecisionBoxSDK sdk;
    private int currentLevel = 1;
    
    private async void Start()
    {
        // Initialize SDK
        var sdkObject = new GameObject("DecisionBoxSDK");
        sdk = sdkObject.AddComponent<DecisionBoxSDK>();
        
        bool initialized = await sdk.InitializeAsync();
        if (!initialized) return;
        
        // Start game
        await sdk.SendGameStartedAsync(
            initialSoftCurrency: 100,
            initialHardCurrency: 10
        );
        
        StartLevel();
    }
    
    private async void StartLevel()
    {
        await sdk.SendLevelStartedAsync(
            levelNumber: currentLevel,
            levelName: $"Level {currentLevel}",
            difficulty: currentLevel < 5 ? "Easy" : "Hard"
        );
    }
    
    public async void OnLevelComplete(int score)
    {
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
        await sdk.SendLevelFailedAsync(
            levelNumber: currentLevel,
            failureReason: reason
        );
    }
    
    public async void OnBoosterPurchased(BoosterType booster)
    {
        await sdk.SendBoosterAcceptedAsync(
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