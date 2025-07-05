# DecisionBox Unity SDK - AI Integration Context

This document provides comprehensive information about the DecisionBox Unity SDK for AI-powered integration assistance. It contains all necessary details about SDK installation, configuration, methods, events, enums, and usage examples.

## Overview

DecisionBox Unity SDK is an advanced analytics and real-time insights solution for Unity games featuring:
- Strongly-typed events with enums instead of strings
- Automatic session management
- Singleton pattern access via `DecisionBoxSDK.Instance`
- WebSocket support for real-time communication
- Push notification support via device tokens
- Remote configuration
- Unity 2020.3+ support

## Installation

### Unity Package Manager (Git URL)
1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **"Add package from git URL"**
3. Enter: `https://github.com/decisionbox-io/decisionbox-unity-sdk.git`
4. Click **Add**

## Initial Setup

### Option A: Using the Setup Window (Recommended)
1. Go to **DecisionBox > Setup SDK** in Unity menu
2. Enter your App ID and App Secret
3. Click "Create Configured SDK GameObject"

### Option B: Manual Setup
1. Go to **DecisionBox > Create SDK GameObject** in Unity menu
2. Configure in Inspector:
   - **App ID**: Your DecisionBox application ID
   - **App Secret**: Your DecisionBox application secret
   - **Environment**: `production` or `development`
   - **Enable Logging**: Check for debug logs

The SDK GameObject persists across scenes (DontDestroyOnLoad).

## Basic Usage Pattern

```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameManager : MonoBehaviour
{
    private async void Start()
    {
        // Initialize SDK
        bool initialized = await DecisionBoxSDK.Instance.InitializeAsync();
        
        if (initialized)
        {
            Debug.Log("DecisionBox SDK initialized!");
            // SDK automatically sends SessionStarted event
        }
    }
}
```

## Complete API Reference

### Configuration Methods

#### Configure
```csharp
DecisionBoxSDK.Instance.Configure(string appId, string appSecret, string environment = "production", bool enableLogging = false)
```
Configure SDK credentials before initialization.

#### InitializeAsync
```csharp
Task<bool> InitializeAsync(string? userId = null)
```
Initialize the SDK. Returns true if successful. If userId is null, generates and persists a GUID.

#### GetAppID
```csharp
string GetAppID()
```
Get the configured App ID.

### Game Event Methods

#### SendGameStartedAsync
```csharp
Task<bool> SendGameStartedAsync(
    string? userId = null,
    int? initialSoftCurrency = null,
    int? initialHardCurrency = null,
    int? initialPremiumCurrency = null
)
```
Send when game session starts.

#### SendGameEndedAsync
```csharp
Task<bool> SendGameEndedAsync(
    string? userId = null,
    int currentScore = 0,
    int? finalSoftCurrency = null,
    int? finalHardCurrency = null,
    int? finalPremiumCurrency = null
)
```
Send when game session ends.

#### SendGameOverAsync
```csharp
Task<bool> SendGameOverAsync(
    string? userId = null,
    int levelNumber = 0,
    int currentScore = 0,
    FailureReason gameOverReason = FailureReason.TimeOut
)
```
Send when game is over due to failure.

### Level Event Methods

#### SendLevelStartedAsync
```csharp
Task<bool> SendLevelStartedAsync(
    string? userId = null,
    int levelNumber = 0,
    string? levelName = null,
    string? difficulty = null
)
```
Send when a level starts.

#### SendLevelSuccessAsync
```csharp
Task<bool> SendLevelSuccessAsync(
    string? userId = null,
    int levelNumber = 0
)
```
Send when a level is completed successfully.

#### SendLevelFailedAsync
```csharp
Task<bool> SendLevelFailedAsync(
    string? userId = null,
    int levelNumber = 0,
    FailureReason failureReason = FailureReason.TimeOut
)
```
Send when a level fails.

#### SendLevelFinishedAsync
```csharp
Task<bool> SendLevelFinishedAsync(
    string? userId = null,
    int levelNumber = 0
)
```
Send when a level is finished (regardless of outcome).

### Currency & Economy Methods

#### SendCurrencyBalanceUpdatedAsync
```csharp
Task<bool> SendCurrencyBalanceUpdatedAsync(
    string? userId = null,
    CurrencyType currencyType = CurrencyType.Soft,
    int? oldBalance = null,
    int currentBalance = 0,
    int? delta = null,
    CurrencyUpdateReason updateReason = CurrencyUpdateReason.NotSpecified
)
```
Send when currency balance changes.

### Booster Event Methods

#### SendBoosterOfferedAsync
```csharp
Task<bool> SendBoosterOfferedAsync(
    string? userId = null,
    int levelNumber = 0,
    BoosterType boosterType = BoosterType.SpeedBoost,
    OfferMethod offerMethod = OfferMethod.WatchAd,
    int requiredCurrency = 0
)
```
Send when a booster is offered to the player.

#### SendBoosterAcceptedAsync
```csharp
Task<bool> SendBoosterAcceptedAsync(
    string? userId = null,
    int levelNumber = 0,
    BoosterType boosterType = BoosterType.SpeedBoost,
    AcceptMethod acceptMethod = AcceptMethod.WatchAd,
    int spentAmount = 0
)
```
Send when a booster is accepted.

#### SendBoosterDeclinedAsync
```csharp
Task<bool> SendBoosterDeclinedAsync(
    string? userId = null,
    int levelNumber = 0,
    BoosterType boosterType = BoosterType.SpeedBoost,
    DeclineReason declineReason = DeclineReason.NotInterested
)
```
Send when a booster is declined.

### Gameplay Event Methods

#### SendScoreUpdatedAsync
```csharp
Task<bool> SendScoreUpdatedAsync(
    string? userId = null,
    int levelNumber = 0,
    int currentScore = 0,
    int? oldScore = null,
    int? scoreDelta = null,
    int comboMultiplier = 1
)
```
Send when score changes.

#### SendPowerUpCollectedAsync
```csharp
Task<bool> SendPowerUpCollectedAsync(
    string? userId = null,
    int levelNumber = 0,
    string powerUpType = "",
    int quantity = 1,
    double? duration = null
)
```
Send when a power-up is collected.

#### SendMetricRecordedAsync
```csharp
Task<bool> SendMetricRecordedAsync(
    string? userId = null,
    int levelNumber = 0,
    MetricType metric = MetricType.Score,
    float metricValue = 0f
)
```
Send custom metric recordings.

### Location Event Methods

#### SendUserLocationTextAsync
```csharp
Task<bool> SendUserLocationTextAsync(
    string? userId = null,
    string location = ""
)
```
Send text-based location (e.g., "New York, USA").

#### SendUserLocationLatLngAsync
```csharp
Task<bool> SendUserLocationLatLngAsync(
    string? userId = null,
    double latitude = 0.0,
    double longitude = 0.0
)
```
Send GPS coordinates.

#### SendUserLocationIPAsync
```csharp
Task<bool> SendUserLocationIPAsync(
    string? userId = null,
    string ipAddress = ""
)
```
Send IP-based location.

### Action Outcome Methods

#### SendPositiveActionOutcomeRecordedAsync
```csharp
Task<bool> SendPositiveActionOutcomeRecordedAsync(
    string? userId,
    string rulesetId,
    OfferMethod offerMethod,
    AcceptMethod acceptMethod,
    double value
)
```
Send positive action outcome (e.g., offer accepted).

#### SendNegativeActionOutcomeRecordedAsync
```csharp
Task<bool> SendNegativeActionOutcomeRecordedAsync(
    string? userId,
    string rulesetId,
    OfferMethod offerMethod,
    DeclineReason declineReason,
    double value
)
```
Send negative action outcome (e.g., offer declined).

### Device Token Method

#### SendUserDeviceTokenAsync
```csharp
Task<bool> SendUserDeviceTokenAsync(
    string? userId = null,
    string deviceToken = ""
)
```
Send device token for push notifications.

### WebSocket Methods

#### On
```csharp
void On(string eventName, Action<string> callback)
```
Register a callback for WebSocket events.

#### Off
```csharp
void Off(string eventName, Action<string> callback)
```
Unregister a WebSocket callback.

## Enum Reference

### CurrencyType
```csharp
public enum CurrencyType
{
    Soft,
    Hard,
    Premium
}
```

### CurrencyUpdateReason
```csharp
public enum CurrencyUpdateReason
{
    NotSpecified,
    Purchase,
    Reward,
    Spending,
    Refund,
    Bonus,
    Achievement,
    DailyLogin,
    AdReward,
    CurrencyExchange,
    SystemAdjustment
}
```

### BoosterType
```csharp
public enum BoosterType
{
    SpeedBoost,
    Shield,
    DoublePoints,
    ExtraLife,
    Invincibility,
    Magnet,
    ComboMultiplier,
    DamageBoost,
    HealthRegen,
    CriticalHitBoost,
    ScoreMultiplier,
    TimeExtension,
    CoinMultiplier,
    GemFinder,
    MysteryBooster,
    Teleportation,
    FreezeTime,
    HyperJump,
    PowerShot,
    StealthMode,
    LaserBeam,
    RocketLauncher,
    Barrier,
    MegaBomb,
    SlowMotion,
    FastForward,
    ReviveBooster,
    LuckyCharm,
    ExplosiveAmmo,
    HomingMissiles,
    ChainReaction,
    ElementalPower,
    SuperArmor,
    InfiniteAmmo,
    GravityControl,
    Radar,
    CloakingDevice,
    EnergyShield,
    TurboMode,
    MultiShot,
    PiercingShot,
    BounceShot,
    SplitShot,
    ChargedShot,
    AutoAim,
    DamageReflection,
    LifeSteal,
    ManaSiphon,
    ElementalShield,
    BerserkMode,
    FocusMode,
    CustomBooster1
}
```

### OfferMethod
```csharp
public enum OfferMethod
{
    WatchAd,
    SpendCurrency,
    InAppPurchase,
    CompleteChallenge,
    DailyBonus,
    RewardedVideo,
    SocialShare,
    Survey,
    LevelCompletion,
    InvitationBonus,
    SpinWheel,
    MysteryBox,
    Upgrade,
    FreeTrial,
    LimitedTimeOffer,
    BundleOffer,
    SpecialEvent,
    VIPMembership,
    Subscription,
    Referral,
    Exchange,
    Tournament,
    MiniGame,
    Achievement,
    Gifting,
    Auction,
    Crafting,
    Unknown
}
```

### AcceptMethod
```csharp
public enum AcceptMethod
{
    WatchAd,
    SpendCurrency,
    InAppPurchase,
    Reward,
    SocialShare,
    Referral,
    Achievement,
    DirectAccept,
    VIPAccess,
    LimitedTimeOffer,
    ExchangeCurrency,
    BundlePurchase,
    EventParticipation,
    SubscriptionUpgrade,
    FreeAccept,
    CompletedChallenge,
    CompletedSurvey,
    UsedRewardedVideo,
    Unknown
}
```

### DeclineReason
```csharp
public enum DeclineReason
{
    NotInterested,
    InsufficientFunds,
    AlreadyActive,
    AdUnavailable,
    AdSkipped,
    CancelledByUser,
    TimeExpired,
    TechnicalIssue,
    PreferToSaveCurrency,
    TooExpensive,
    AlreadyOwned,
    LackOfUrgency
}
```

### FailureReason
```csharp
public enum FailureReason
{
    TimeOut,
    NoLives,
    CollisionWithObstacle,
    PuzzleNotSolved,
    ObjectiveNotCompleted,
    EnemyDefeatedPlayer,
    OverwhelmedByEnemies,
    WrongMove,
    MistimedAction,
    EnvironmentalHazard,
    InsufficientResources,
    Inactivity,
    Disconnection,
    UserQuit,
    NotEnoughMoves,
    AllTargetsNotDestroyed,
    FailedChallenge,
    SpecialRuleBroken,
    MinimumScoreNotReached,
    NoPathAvailable,
    CriticalErrorOccurred,
    Unknown
}
```

### MetricType
```csharp
public enum MetricType
{
    MovesMade,
    MovesRemaining,
    LivesRemaining,
    LivesLost,
    EnemiesDefeated,
    ItemsCollected,
    PowerUpsUsed,
    TimePlayed,
    LevelsCompleted,
    Score,
    CoinsCollected,
    DiamondsCollected,
    BoostersActivated,
    DistanceTraveled,
    JumpsPerformed,
    StreakCount,
    AccuracyRate,
    AverageSpeed,
    CriticalHits,
    DamageTaken,
    DamageDealt,
    HealthRecovered,
    ManaSpent,
    ExperienceGained,
    SkillsUsed,
    QuestsCompleted,
    AchievementsUnlocked,
    BossesDefeated,
    ChestsOpened,
    SecretsFound,
    CombosPerformed,
    MaxComboCount,
    PerfectMoves,
    NearMisses,
    BlocksDestroyed,
    TilesMatched,
    ChainReactions,
    ResourcesHarvested,
    BuildingsConstructed,
    UnitsProduced,
    BattlesWon,
    BattlesLost,
    TournamentsEntered,
    RankingPosition,
    FriendsInvited
}
```

### PlatformType
```csharp
public enum PlatformType
{
    NotSet,
    Android,
    iOS,
    Windows,
    MacOS,
    Linux,
    WebGL,
    PlayStation,
    Xbox,
    Nintendo,
    VR,
    Editor,
    Web
}
```

## Usage Examples

### Basic Game Flow
```csharp
using UnityEngine;
using DecisionBox.Core;
using DecisionBox.Models;

public class GameController : MonoBehaviour
{
    private int currentLevel = 1;
    
    private async void Start()
    {
        // Initialize SDK
        bool initialized = await DecisionBoxSDK.Instance.InitializeAsync();
        if (!initialized) 
        {
            Debug.LogError("Failed to initialize DecisionBox SDK!");
            return;
        }
        
        // Send device token if available (e.g., from Firebase)
        string deviceToken = GetDeviceToken();
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
        // Send success event
        await DecisionBoxSDK.Instance.SendLevelSuccessAsync(levelNumber: currentLevel);
        
        // Update score
        await DecisionBoxSDK.Instance.SendScoreUpdatedAsync(
            levelNumber: currentLevel,
            currentScore: score
        );
        
        // Award currency
        await DecisionBoxSDK.Instance.SendCurrencyBalanceUpdatedAsync(
            currencyType: CurrencyType.Soft,
            oldBalance: 100,
            currentBalance: 150,
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
    
    private string GetDeviceToken()
    {
        // Your device token retrieval logic
        return "";
    }
}
```

### Currency Management
```csharp
public async void OnPurchaseCompleted(string itemId, int price, CurrencyType currency)
{
    int oldBalance = GetCurrentBalance(currency);
    int newBalance = oldBalance - price;
    
    await DecisionBoxSDK.Instance.SendCurrencyBalanceUpdatedAsync(
        currencyType: currency,
        oldBalance: oldBalance,
        currentBalance: newBalance,
        delta: -price,
        updateReason: CurrencyUpdateReason.Spending
    );
    
    UpdateBalance(currency, newBalance);
}

public async void OnRewardCollected(int amount, CurrencyType currency)
{
    int oldBalance = GetCurrentBalance(currency);
    int newBalance = oldBalance + amount;
    
    await DecisionBoxSDK.Instance.SendCurrencyBalanceUpdatedAsync(
        currencyType: currency,
        oldBalance: oldBalance,
        currentBalance: newBalance,
        delta: amount,
        updateReason: CurrencyUpdateReason.Reward
    );
    
    UpdateBalance(currency, newBalance);
}
```

### Booster System
```csharp
public async void OfferBooster(BoosterType booster, int price)
{
    await DecisionBoxSDK.Instance.SendBoosterOfferedAsync(
        levelNumber: currentLevel,
        boosterType: booster,
        offerMethod: price > 0 ? OfferMethod.SpendCurrency : OfferMethod.FreeTrial,
        requiredCurrency: price
    );
}

public async void OnBoosterPurchased(BoosterType booster, int price)
{
    await DecisionBoxSDK.Instance.SendBoosterAcceptedAsync(
        levelNumber: currentLevel,
        boosterType: booster,
        acceptMethod: AcceptMethod.SpendCurrency,
        spentAmount: price
    );
    
    ActivateBooster(booster);
}

public async void OnBoosterDeclined(BoosterType booster, DeclineReason reason)
{
    await DecisionBoxSDK.Instance.SendBoosterDeclinedAsync(
        levelNumber: currentLevel,
        boosterType: booster,
        declineReason: reason
    );
}
```

### Metrics Tracking
```csharp
public async void TrackGameplayMetrics()
{
    // Track moves made
    await DecisionBoxSDK.Instance.SendMetricRecordedAsync(
        levelNumber: currentLevel,
        metric: MetricType.MovesMade,
        metricValue: moveCount
    );
    
    // Track enemies defeated
    await DecisionBoxSDK.Instance.SendMetricRecordedAsync(
        levelNumber: currentLevel,
        metric: MetricType.EnemiesDefeated,
        metricValue: enemiesKilled
    );
    
    // Track accuracy
    float accuracy = (float)successfulHits / totalShots * 100f;
    await DecisionBoxSDK.Instance.SendMetricRecordedAsync(
        levelNumber: currentLevel,
        metric: MetricType.AccuracyRate,
        metricValue: accuracy
    );
}
```

### WebSocket Events
```csharp
private void SetupWebSocketListeners()
{
    // Listen for special offers
    DecisionBoxSDK.Instance.On("special_offer", (message) => {
        Debug.Log($"Received special offer: {message}");
        // Parse and display offer to player
    });
    
    // Listen for tournament invitations
    DecisionBoxSDK.Instance.On("tournament_invite", (message) => {
        Debug.Log($"Tournament invitation: {message}");
        // Show tournament UI
    });
}

private void CleanupWebSocketListeners()
{
    DecisionBoxSDK.Instance.Off("special_offer", OnSpecialOffer);
    DecisionBoxSDK.Instance.Off("tournament_invite", OnTournamentInvite);
}
```

### Push Notifications
```csharp
// Integration with Firebase
using Firebase.Messaging;

public class PushNotificationManager : MonoBehaviour
{
    private void Start()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }
    
    private async void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log($"Received device token: {token.Token}");
        
        // Send token to DecisionBox
        await DecisionBoxSDK.Instance.SendUserDeviceTokenAsync(
            deviceToken: token.Token
        );
    }
    
    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received push notification");
        // Handle push notification
    }
}
```

### Location Tracking
```csharp
// Text-based location
await DecisionBoxSDK.Instance.SendUserLocationTextAsync(
    location: "New York, USA"
);

// GPS coordinates
await DecisionBoxSDK.Instance.SendUserLocationLatLngAsync(
    latitude: 40.7128,
    longitude: -74.0060
);

// IP address
await DecisionBoxSDK.Instance.SendUserLocationIPAsync(
    ipAddress: "192.168.1.1"
);
```

### Action Outcomes
```csharp
// Positive outcome - offer accepted
await DecisionBoxSDK.Instance.SendPositiveActionOutcomeRecordedAsync(
    userId: currentUserId,
    rulesetId: "PROMO_2024_SPRING",
    offerMethod: OfferMethod.LimitedTimeOffer,
    acceptMethod: AcceptMethod.InAppPurchase,
    value: 9.99
);

// Negative outcome - offer declined
await DecisionBoxSDK.Instance.SendNegativeActionOutcomeRecordedAsync(
    userId: currentUserId,
    rulesetId: "PROMO_2024_SPRING",
    offerMethod: OfferMethod.LimitedTimeOffer,
    declineReason: DeclineReason.TooExpensive,
    value: 9.99
);
```

## Important Notes

1. **Singleton Pattern**: Always use `DecisionBoxSDK.Instance` to access the SDK
2. **Initialization**: Must call `InitializeAsync()` before using any other methods
3. **User ID**: If not provided, SDK automatically generates and persists a GUID
4. **Session Management**: Sessions are managed automatically by the SDK
5. **Background Handling**: SDK handles app pause/resume automatically
6. **Token Management**: JWT tokens are refreshed automatically
7. **Platform Detection**: SDK automatically detects the current platform
8. **Error Handling**: All methods return `Task<bool>` - check return value for success
9. **WebSocket**: Real-time events require WebSocket to be enabled in remote config
10. **Thread Safety**: WebSocket messages are dispatched on the main thread

## Configuration Options

### SDK Configuration (Set in Inspector or via Configure method)
- `appId`: Your DecisionBox application ID (required)
- `appSecret`: Your DecisionBox application secret (required)
- `environment`: "production" or "development" (default: "production")
- `enableLogging`: Enable debug logging (default: false)

### Remote Configuration (Fetched from server)
- `enabled`: SDK enabled/disabled
- `max_session_duration`: Maximum session duration in minutes
- `websocketEnabled`: WebSocket connection enabled
- `maxBackgroundDuration`: Time before new session (default: 5 minutes)
- `eventBatchSize`: Events per batch (default: 10)
- `eventFlushInterval`: Event flush interval (default: 30 seconds)

## Platform Support
- iOS: Full support
- Android: Full support
- WebGL: Full support (including WebSocket)
- Windows/Mac/Linux: Full support
- Unity Editor: Full support with debug features

## Requirements
- Unity 2020.3 or later
- C# 8.0+ (nullable reference types)
- Newtonsoft.Json package
- NativeWebSocket package (for WebSocket support)

## Best Practices

1. **Initialize Early**: Initialize SDK in your game's startup sequence
2. **Check Return Values**: Always check the boolean return value of async methods
3. **Use Enums**: Use the provided enums instead of magic strings
4. **Track Key Events**: Focus on tracking events that provide business value
5. **Batch Events**: SDK automatically batches events for efficiency
6. **Handle Failures**: Implement retry logic for critical events if needed
7. **Test Integration**: Use development environment for testing
8. **Monitor Logs**: Enable logging during development to debug issues
9. **Update Tokens**: For push notifications, update device tokens when they change
10. **Clean Up**: Unregister WebSocket callbacks when no longer needed

## Troubleshooting

1. **SDK Not Initializing**: Check App ID and App Secret are correct
2. **Events Not Sending**: Verify SDK is initialized and network is available
3. **WebSocket Not Connecting**: Check if WebSocket is enabled in remote config
4. **Push Notifications**: Ensure device token is valid and sent to DecisionBox
5. **Session Issues**: Check background duration settings in remote config

## Version
Current SDK Version: As per package.json in the repository

---

This document is designed to provide complete context for AI-powered integration assistance. For the latest updates, refer to the official DecisionBox Unity SDK repository.