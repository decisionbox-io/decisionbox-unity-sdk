# Type-Safe Socket Notifications for DecisionBox Unity SDK

## Executive Summary

This document outlines a type-safe notification system specifically designed for DecisionBox's LTV optimization platform. The system provides strongly-typed handlers for ruleset actions while maintaining backward compatibility with the existing `.On()` method.

## Core Value Proposition

DecisionBox optimizes player LTV through real-time personalization. Our type-safe notifications enable:
- **Immediate player assistance** when struggling (retention)
- **Perfectly-timed monetization** opportunities (revenue) 
- **Dynamic difficulty adjustment** (engagement)
- **Personalized rewards** based on behavior (progression)

## Focused Action Types

This implementation focuses on 7 core action types that directly impact LTV:
1. **BoosterAwardedPayload** - Free boosters to help struggling players
2. **HintProvidedPayload** - Strategic hints for progression
3. **DifficultyAdjustedPayload** - Dynamic difficulty changes
4. **OfferPresentedPayload** - Time-sensitive monetization offers
5. **UIHighlightPayload** - UI element highlighting for engagement
6. **ProgressionBoostPayload** - Temporary progression multipliers
7. **RewardGrantedPayload** - Direct rewards for achievements

## Architecture Overview

### Design Principles
1. **LTV-Focused**: Every notification type directly impacts retention, monetization, or engagement
2. **Type Safety**: Compile-time validation prevents runtime errors
3. **Backward Compatible**: Existing `.On(string, Action<object>)` continues working
4. **Ruleset Aligned**: Maps directly to DecisionBox action payloads
5. **Performance Optimized**: Minimal overhead, efficient deserialization
6. **Customer Extensible**: Customers can define custom actions in DecisionBox UI and export as C# classes

### Implementation Approach
```csharp
// Current approach (maintained)
socket.On("STRUGGLING_PLAYER_BOOST", (data) => { 
    // Manual parsing required
});

// New type-safe approach
socket.OnAction(ActionType.BoosterAwarded, (BoosterAwardedPayload payload) => {
    // Strongly-typed, IntelliSense-supported
    inventory.AddBooster(payload.BoosterType, payload.Amount);
});
```

## Notification Action Types

Based on DecisionBox's ruleset system and LTV optimization goals, we define the following action types:

### 1. Player Assistance Actions (Retention)

#### ActionType.BoosterAwarded
Sent when a player receives free boosters to overcome difficulty.
```csharp
public class BoosterAwardedPayload : IActionPayload
{
    public string ActionId { get; set; }           // Ruleset action identifier
    public string RulesetId { get; set; }          // Which rule triggered this
    public BoosterType BoosterType { get; set; }   // Type from existing enum
    public int Amount { get; set; }                // Quantity awarded
    public string Message { get; set; }            // Optional display message
    public string Title { get; set; }              // Optional notification title
    public GrantReason Reason { get; set; }        // Why this was granted
    public Dictionary<string, object> CustomData { get; set; }
}

public enum GrantReason
{
    StrugglingPlayer,
    LevelDifficulty,
    FirstTimePlayer,
    ReturningPlayer,
    Milestone,
    Compensation,
    Promotion
}
```

#### ActionType.HintProvided
Strategic hints to help players progress.
```csharp
public class HintProvidedPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public string HintText { get; set; }           // The hint message
    public string Title { get; set; }              // e.g., "Pro Tip"
    public HintType Type { get; set; }             // Strategy, Warning, Tip
    public int DisplayDuration { get; set; }       // Seconds to display
    public Priority Priority { get; set; }         // Low, Normal, High, Critical
    public string TargetElement { get; set; }      // UI element to highlight
    public int? LevelNumber { get; set; }          // Associated level
}

public enum HintType
{
    Strategy,       // How to approach the level
    Warning,        // Upcoming difficulty
    Tip,           // General gameplay tip
    Tutorial,      // Teaching mechanic
    Encouragement  // Motivational message
}
```

#### ActionType.DifficultyAdjusted
Dynamic difficulty changes based on player performance.
```csharp
public class DifficultyAdjustedPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public DifficultyChange ChangeType { get; set; }
    public float OldMultiplier { get; set; }       // Previous difficulty
    public float NewMultiplier { get; set; }       // New difficulty
    public string Reason { get; set; }             // Why adjusted
    public int Duration { get; set; }              // Seconds (0 = permanent)
    public AdjustmentTarget Target { get; set; }   // What was adjusted
}

public enum DifficultyChange
{
    Decreased,
    Increased,
    Normalized
}

public enum AdjustmentTarget
{
    EnemyHealth,
    EnemyDamage,
    EnemySpeed,
    PlayerHealth,
    PlayerDamage,
    ResourceGeneration,
    TimeLimit,
    ObjectiveRequirement
}
```

### 2. Monetization Actions (Revenue)

#### ActionType.OfferPresented
Time-sensitive offers based on player behavior.
```csharp
public class OfferPresentedPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public string OfferId { get; set; }            // Unique offer identifier
    public string OfferName { get; set; }          // Display name
    public OfferType Type { get; set; }            // Flash, Limited, Special
    public OfferContent[] Contents { get; set; }   // What's included
    public decimal OriginalPrice { get; set; }     
    public decimal DiscountedPrice { get; set; }
    public int DiscountPercentage { get; set; }
    public string Currency { get; set; }           // USD, EUR, etc.
    public DateTime ExpiresAt { get; set; }        // When offer expires
    public int? MaxPurchases { get; set; }         // Purchase limit
    public OfferTriggerReason TriggerReason { get; set; }
    public string DeepLink { get; set; }           // Navigation link
}

public class OfferContent
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public ItemType Type { get; set; }
    public int Quantity { get; set; }
    public ItemRarity Rarity { get; set; }
    public decimal Value { get; set; }             // Perceived value
}

public enum OfferType
{
    Flash,          // Very short duration
    Limited,        // Time-limited
    Special,        // Event-based
    Personalized,   // AI-driven
    Bundle,         // Multiple items
    Starter,        // New player
    Comeback        // Returning player
}

public enum OfferTriggerReason
{
    LowCurrency,
    HighEngagement,
    FailureStreak,
    WinStreak,
    SessionLength,
    FirstPurchase,
    RepeatPurchase,
    Milestone,
    ReturnAfterAbsence
}
```

#### ActionType.UIHighlight
Highlight UI elements to drive attention.
```csharp
public class UIHighlightPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public string ElementId { get; set; }          // UI element to highlight
    public HighlightType Type { get; set; }        // Glow, Pulse, Arrow
    public string Message { get; set; }            // Associated message
    public int Duration { get; set; }              // Display duration
    public Priority Priority { get; set; }
    public HighlightReason Reason { get; set; }
    public string DeepLink { get; set; }           // Where to navigate
}

public enum HighlightType
{
    Glow,
    Pulse,
    Arrow,
    Badge,
    Shake,
    Sparkle
}

public enum HighlightReason
{
    NewContent,
    SpecialOffer,
    Tutorial,
    Achievement,
    Reward,
    Warning
}
```

### 3. Progression Actions (Engagement)

#### ActionType.ProgressionBoost
Temporary boosts to accelerate progression.
```csharp
public class ProgressionBoostPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public BoostType Type { get; set; }
    public float Multiplier { get; set; }          // e.g., 2.0 for double
    public int Duration { get; set; }              // Seconds
    public string Message { get; set; }
    public string IconUrl { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BoostReason Reason { get; set; }
}

public enum BoostType
{
    ExperienceMultiplier,
    CurrencyMultiplier,
    ScoreMultiplier,
    SpeedBoost,
    DamageBoost,
    LuckBoost,
    DropRateBoost
}

public enum BoostReason
{
    StreakReward,
    MilestoneReward,
    EventBonus,
    CompensationBonus,
    WelcomeBack,
    HappyHour,
    WeekendBonus
}
```

#### ActionType.RewardGranted
Direct rewards based on player actions.
```csharp
public class RewardGrantedPayload : IActionPayload
{
    public string ActionId { get; set; }
    public string RulesetId { get; set; }
    public RewardItem[] Rewards { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public RewardReason Reason { get; set; }
    public bool AutoClaimed { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class RewardItem
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public RewardType Type { get; set; }
    public int Quantity { get; set; }
    public ItemRarity Rarity { get; set; }
    public string IconUrl { get; set; }
}

public enum RewardType
{
    Currency,
    Booster,
    Cosmetic,
    Experience,
    UnlockableContent,
    LootBox,
    Energy
}

public enum RewardReason
{
    Achievement,
    Milestone,
    DailyBonus,
    StreakBonus,
    EventReward,
    Compensation,
    Tutorial,
    Challenge
}
```


## Custom Action Definition System

### Overview
Customers can define their own action types in the DecisionBox dashboard and export them as C# classes for use in their Unity projects. This provides complete flexibility while maintaining type safety.

### Dashboard Action Builder
In the DecisionBox UI, customers can:
1. Create custom action types with a unique identifier
2. Define payload fields with types and validation rules
3. Generate C# code for their custom actions
4. Export a complete action definition file

### Custom Action Definition Example

#### Step 1: Define in Dashboard
Customer creates a "SPECIAL_REWARD" action with custom fields:
```json
{
  "actionType": "SPECIAL_REWARD",
  "description": "Awards special rewards based on player achievements",
  "fields": [
    {
      "name": "rewardTier",
      "type": "string",
      "required": true,
      "enum": ["Bronze", "Silver", "Gold", "Platinum"]
    },
    {
      "name": "bonusMultiplier",
      "type": "float",
      "required": true,
      "min": 1.0,
      "max": 10.0
    },
    {
      "name": "unlockedFeatures",
      "type": "string[]",
      "required": false
    },
    {
      "name": "expirationTime",
      "type": "DateTime",
      "required": false
    }
  ]
}
```

#### Step 2: Export as C# Class
DecisionBox generates the following C# code:
```csharp
// Generated by DecisionBox on 2025-01-29
// Action Type: SPECIAL_REWARD
// Version: 1.0.0

using System;
using System.Collections.Generic;
using DecisionBox.Notifications;

namespace MyGame.Notifications.Custom
{
    /// <summary>
    /// Awards special rewards based on player achievements
    /// Generated from DecisionBox ruleset action definition
    /// </summary>
    [DecisionBoxAction("SPECIAL_REWARD")]
    public class SpecialRewardPayload : ICustomActionPayload
    {
        // Base properties (inherited from ICustomActionPayload)
        public string ActionId { get; set; }
        public string RulesetId { get; set; }
        public string ActionType => "SPECIAL_REWARD";
        
        // Custom properties
        public RewardTier RewardTier { get; set; }
        public float BonusMultiplier { get; set; }
        public string[] UnlockedFeatures { get; set; }
        public DateTime? ExpirationTime { get; set; }
        
        // Custom validation
        public bool Validate()
        {
            if (BonusMultiplier < 1.0f || BonusMultiplier > 10.0f)
                return false;
            return true;
        }
    }
    
    public enum RewardTier
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }
}
```

#### Step 3: Register and Use in Unity
```csharp
public class CustomActionManager : MonoBehaviour
{
    void Start()
    {
        var sdk = DecisionBoxSDK.Instance;
        
        // Register custom action handler
        sdk.RegisterCustomAction<SpecialRewardPayload>("SPECIAL_REWARD", OnSpecialReward);
    }
    
    private void OnSpecialReward(SpecialRewardPayload payload)
    {
        // Handle custom action with full type safety
        Debug.Log($"Special reward received: {payload.RewardTier} with {payload.BonusMultiplier}x bonus!");
        
        // Apply bonus multiplier
        rewardManager.ApplyMultiplier(payload.BonusMultiplier);
        
        // Unlock features if any
        if (payload.UnlockedFeatures != null)
        {
            foreach (var feature in payload.UnlockedFeatures)
            {
                featureManager.Unlock(feature);
            }
        }
        
        // Handle expiration
        if (payload.ExpirationTime.HasValue)
        {
            scheduler.ScheduleExpiration(payload.ActionId, payload.ExpirationTime.Value);
        }
    }
}
```

### Integration with DecisionBox SDK

```csharp
public partial class DecisionBoxSDK
{
    private readonly Dictionary<string, Type> customActionTypes = new();
    private readonly Dictionary<string, Delegate> customHandlers = new();
    
    /// <summary>
    /// Register a custom action type with its handler
    /// </summary>
    public void RegisterCustomAction<T>(string actionName, Action<T> handler) 
        where T : ICustomActionPayload
    {
        customActionTypes[actionName] = typeof(T);
        customHandlers[actionName] = handler;
        
        // Register with socket
        socket.On(actionName, (data) => ProcessCustomAction(actionName, data));
    }
    
    private void ProcessCustomAction(string actionName, object data)
    {
        try
        {
            if (customActionTypes.TryGetValue(actionName, out var payloadType))
            {
                // Deserialize to custom type
                var payload = JsonConvert.DeserializeObject(data.ToString(), payloadType);
                
                // Validate if implements IValidatable
                if (payload is IValidatable validatable && !validatable.Validate())
                {
                    logger.LogWarning($"Custom action {actionName} failed validation");
                    return;
                }
                
                // Execute handler
                if (customHandlers.TryGetValue(actionName, out var handler))
                {
                    handler.DynamicInvoke(payload);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to process custom action {actionName}: {ex.Message}");
        }
    }
}
```

### Export Package Structure

When customers export their custom actions from DecisionBox dashboard, they receive:
```
DecisionBoxCustomActions/
├── CustomActions.cs           // All custom action payload classes
├── CustomEnums.cs            // Related enums
├── Integration.md            // Integration instructions
└── Examples.cs               // Usage examples
```

### Benefits of Custom Actions

1. **Full Control**: Customers define exactly what data they need
2. **Type Safety**: Generated C# classes provide compile-time validation
3. **Consistency**: Actions defined in dashboard match Unity implementation
4. **Versioning**: Export includes version info for compatibility tracking
5. **No Manual Parsing**: Strongly-typed payloads eliminate parsing errors

## SDK Implementation

### Core Interfaces

```csharp
public interface IActionPayload
{
    string ActionId { get; set; }
    string RulesetId { get; set; }
}

public interface ICustomActionPayload : IActionPayload
{
    string ActionType { get; }
}

public interface INotificationHandler
{
    void RegisterHandler<T>(ActionType action, Action<T> handler) where T : IActionPayload;
    void UnregisterHandler(ActionType action);
    void ProcessNotification(string action, object data);
}
```

### NotificationManager Class

```csharp
public class NotificationManager : INotificationHandler
{
    private readonly Dictionary<ActionType, Delegate> handlers = new();
    private readonly ISocketManager socket;
    private readonly ILogger logger;
    
    public NotificationManager(ISocketManager socket, ILogger logger)
    {
        this.socket = socket;
        this.logger = logger;
    }
    
    public void RegisterHandler<T>(ActionType action, Action<T> handler) where T : IActionPayload
    {
        handlers[action] = handler;
        
        // Register with underlying socket
        string actionName = GetActionName(action);
        socket.On(actionName, (data) => ProcessNotification(actionName, data));
    }
    
    public void ProcessNotification(string action, object data)
    {
        try
        {
            // Extract system context
            var context = ExtractContext(data);
            
            // Determine action type
            ActionType actionType = ParseActionType(action);
            
            if (handlers.TryGetValue(actionType, out var handler))
            {
                // Deserialize to appropriate type
                var payloadType = GetPayloadType(actionType);
                var payload = DeserializePayload(data, payloadType);
                
                // Inject context
                InjectContext(payload, context);
                
                // Execute handler
                handler.DynamicInvoke(payload);
                
                // Track outcome
                TrackActionOutcome(context, true);
            }
            else
            {
                // Fall back to generic handler if registered
                OnUnhandledAction(action, data);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to process notification {action}: {ex.Message}");
            TrackActionOutcome(ExtractContext(data), false, ex.Message);
        }
    }
    
    private SystemContext ExtractContext(object data)
    {
        // Extract DECISIONBOX_RS_ID, app_id, session_id
        var json = JObject.Parse(data.ToString());
        return new SystemContext
        {
            RulesetId = json["DECISIONBOX_RS_ID"]?.ToString(),
            AppId = json["app_id"]?.ToString(),
            SessionId = json["session_id"]?.ToString(),
            ActionId = json["action"]?.ToString()
        };
    }
    
    private void TrackActionOutcome(SystemContext context, bool success, string error = null)
    {
        // Send ActionOutcomeRecorded event
        DecisionBoxSDK.Instance.SendActionOutcomeAsync(
            context.RulesetId,
            context.ActionId,
            success,
            error
        );
    }
}
```

### SDK Extension Methods

```csharp
public partial class DecisionBoxSDK
{
    private NotificationManager notificationManager;
    
    /// <summary>
    /// Register a type-safe notification handler
    /// </summary>
    public void OnAction<T>(ActionType action, Action<T> handler) where T : IActionPayload
    {
        notificationManager.RegisterHandler(action, handler);
    }
    
    /// <summary>
    /// Unregister a notification handler
    /// </summary>
    public void OffAction(ActionType action)
    {
        notificationManager.UnregisterHandler(action);
    }
    
    /// <summary>
    /// Check if handler exists for action
    /// </summary>
    public bool HasHandler(ActionType action)
    {
        return notificationManager.HasHandler(action);
    }
    
    /// <summary>
    /// Maintain backward compatibility
    /// </summary>
    public void On(string action, Action<object> handler)
    {
        socket.On(action, handler);
    }
}
```

## Combined Usage: Predefined + Custom Actions

### Example: Using Both Action Types Together
```csharp
public class GameNotificationManager : MonoBehaviour
{
    void Start()
    {
        var sdk = DecisionBoxSDK.Instance;
        
        // Register predefined action handlers
        sdk.OnAction(ActionType.BoosterAwarded, OnBoosterAwarded);
        sdk.OnAction(ActionType.OfferPresented, OnOfferPresented);
        sdk.OnAction(ActionType.DifficultyAdjusted, OnDifficultyAdjusted);
        
        // Register custom action handlers
        sdk.RegisterCustomAction<SpecialRewardPayload>("SPECIAL_REWARD", OnSpecialReward);
        sdk.RegisterCustomAction<BossEventPayload>("BOSS_EVENT_STARTED", OnBossEventStarted);
        
        // Still support legacy string-based handlers for backward compatibility
        sdk.On("LEGACY_EVENT", OnLegacyEvent);
    }
    
    // Predefined action handler
    private void OnBoosterAwarded(BoosterAwardedPayload payload)
    {
        inventory.AddBooster(payload.BoosterType, payload.Amount);
        UI.ShowNotification($"Received {payload.Amount}x {payload.BoosterType}!");
    }
    
    // Custom action handler
    private void OnSpecialReward(SpecialRewardPayload payload)
    {
        rewardManager.ProcessSpecialReward(payload.RewardTier, payload.BonusMultiplier);
    }
    
    // Legacy handler
    private void OnLegacyEvent(object data)
    {
        // Manual parsing for backward compatibility
        var json = JObject.Parse(data.ToString());
        Debug.Log($"Legacy event received: {json}");
    }
}
```

## Usage Examples

### Example 1: Handling Booster Awards

```csharp
public class GameManager : MonoBehaviour
{
    void Start()
    {
        var sdk = DecisionBoxSDK.Instance;
        
        // Register type-safe handler
        sdk.OnAction(ActionType.BoosterAwarded, OnBoosterAwarded);
    }
    
    private void OnBoosterAwarded(BoosterAwardedPayload payload)
    {
        // Add to inventory
        inventory.AddBooster(payload.BoosterType, payload.Amount);
        
        // Show notification
        if (!string.IsNullOrEmpty(payload.Message))
        {
            notificationUI.Show(payload.Title ?? "Reward!", payload.Message);
        }
        
        // Track analytics
        Analytics.TrackEvent("booster_awarded", new {
            booster_type = payload.BoosterType.ToString(),
            amount = payload.Amount,
            reason = payload.Reason.ToString(),
            ruleset_id = payload.RulesetId
        });
        
        // Play celebration effect
        if (payload.Amount > 1)
        {
            effectsManager.PlayCelebration();
        }
    }
}
```

### Example 2: Monetization Opportunities

```csharp
public class ShopManager : MonoBehaviour
{
    void Start()
    {
        var sdk = DecisionBoxSDK.Instance;
        
        sdk.OnAction(ActionType.OfferPresented, OnOfferPresented);
        sdk.OnAction(ActionType.UIHighlight, OnUIHighlight);
    }
    
    private void OnOfferPresented(OfferPresentedPayload payload)
    {
        // Check if player can afford it
        bool canAfford = playerWallet.GetBalance(payload.Currency) >= payload.DiscountedPrice;
        
        // Create offer UI
        var offerPanel = Instantiate(offerPrefab);
        offerPanel.SetOffer(payload);
        offerPanel.SetAffordable(canAfford);
        
        // Set expiration timer
        offerPanel.StartCountdown(payload.ExpiresAt);
        
        // Track impression
        Analytics.TrackOfferImpression(payload.OfferId, payload.TriggerReason);
        
        // Highlight if high value
        if (payload.DiscountPercentage >= 50)
        {
            offerPanel.AddHighValueBadge();
        }
    }
    
    private void OnUIHighlight(UIHighlightPayload payload)
    {
        var element = UI.FindElement(payload.ElementId);
        if (element != null)
        {
            // Apply highlight effect
            switch (payload.Type)
            {
                case HighlightType.Pulse:
                    element.StartPulsing(payload.Duration);
                    break;
                case HighlightType.Glow:
                    element.AddGlow(payload.Duration);
                    break;
                case HighlightType.Arrow:
                    UI.ShowArrowPointing(element, payload.Duration);
                    break;
            }
            
            // Show message if provided
            if (!string.IsNullOrEmpty(payload.Message))
            {
                UI.ShowTooltip(element, payload.Message);
            }
        }
    }
}
```

### Example 3: Dynamic Difficulty

```csharp
public class DifficultyManager : MonoBehaviour
{
    void Start()
    {
        DecisionBoxSDK.Instance.OnAction(ActionType.DifficultyAdjusted, OnDifficultyAdjusted);
    }
    
    private void OnDifficultyAdjusted(DifficultyAdjustedPayload payload)
    {
        // Apply difficulty change
        switch (payload.Target)
        {
            case AdjustmentTarget.EnemyHealth:
                EnemyManager.SetHealthMultiplier(payload.NewMultiplier);
                break;
            case AdjustmentTarget.EnemyDamage:
                EnemyManager.SetDamageMultiplier(payload.NewMultiplier);
                break;
            case AdjustmentTarget.ResourceGeneration:
                ResourceManager.SetGenerationRate(payload.NewMultiplier);
                break;
        }
        
        // Show notification based on change type
        if (payload.ChangeType == DifficultyChange.Decreased)
        {
            UI.ShowEncouragement("We've made things a bit easier for you!");
        }
        
        // Schedule restoration if temporary
        if (payload.Duration > 0)
        {
            StartCoroutine(RestoreDifficulty(payload.Target, payload.OldMultiplier, payload.Duration));
        }
    }
}
```

## Benefits

### For Developers
- **Type Safety**: Compile-time validation prevents runtime errors
- **IntelliSense**: Full autocomplete for all payload properties
- **Clear Intent**: Action types clearly indicate purpose
- **Reduced Bugs**: No manual parsing or type casting needed

### For Business (LTV Impact)
- **Retention**: Timely assistance keeps players engaged
- **Monetization**: Perfectly-timed offers increase conversion
- **Engagement**: Dynamic events maintain interest
- **Personalization**: Tailored experiences per player segment

### For Maintenance
- **Versioning**: Easy to add new fields to payloads
- **Debugging**: Strongly-typed data simplifies troubleshooting
- **Testing**: Mock payloads for unit tests
- **Analytics**: Consistent data structure for tracking

## Migration Path

### Phase 1: Add Alongside (No Breaking Changes)
```csharp
// Both work simultaneously
sdk.On("BOOSTER_GRANTED", HandleBoosterOld);  // Old way
sdk.OnAction(ActionType.BoosterAwarded, HandleBoosterNew);  // New way
```

### Phase 2: Gradual Adoption
- Update documentation with type-safe examples
- Provide migration guide
- Add deprecation notices (but maintain compatibility)

### Phase 3: Full Integration
- Dashboard shows available action types
- Auto-generate TypeScript definitions for web dashboard
- Provide validation in ruleset editor

## Performance Considerations

### Serialization
- Use MessagePack for binary serialization (faster than JSON)
- Cache deserialized types
- Pool payload objects to reduce GC

### Memory Management
```csharp
public class PayloadPool<T> where T : IActionPayload, new()
{
    private readonly Stack<T> pool = new();
    
    public T Rent()
    {
        return pool.Count > 0 ? pool.Pop() : new T();
    }
    
    public void Return(T payload)
    {
        payload.Clear();
        pool.Push(payload);
    }
}
```

## Testing Strategy

### Unit Tests
```csharp
[Test]
public void TestBoosterAwardedPayload()
{
    var payload = new BoosterAwardedPayload
    {
        ActionId = "TEST_ACTION",
        RulesetId = "TEST_RULESET",
        BoosterType = BoosterType.ExtraLife,
        Amount = 3,
        Reason = GrantReason.StrugglingPlayer
    };
    
    var json = JsonConvert.SerializeObject(payload);
    var deserialized = JsonConvert.DeserializeObject<BoosterAwardedPayload>(json);
    
    Assert.AreEqual(payload.BoosterType, deserialized.BoosterType);
    Assert.AreEqual(payload.Amount, deserialized.Amount);
}
```

### Integration Tests
- Test with real WebSocket connections
- Verify payload compatibility with server format
- Test backward compatibility with string-based handlers
- Load test with high notification volume

## Conclusion

This type-safe notification system aligns perfectly with DecisionBox's mission to optimize player LTV through intelligent, real-time personalization. By providing strongly-typed handlers for common game optimization scenarios, we enable developers to implement sophisticated retention, monetization, and engagement strategies with confidence and ease.

The system maintains full backward compatibility while offering a superior developer experience that reduces bugs, improves maintainability, and accelerates implementation of LTV optimization strategies.