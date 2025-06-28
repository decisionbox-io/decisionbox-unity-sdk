// namespace DecisionBox.Events
// {
//     /// <summary>
//     /// Core analytics event types
//     /// </summary>
//     // public static class CoreEvents
//     // {
//     //     public const string SessionStarted = "SessionStarted";
//     //     public const string SessionEnded = "SessionEnded";
//     //     public const string UserLocationText = "UserLocationText";
//     //     public const string UserLocationLatLng = "UserLocationLatLng";
//     //     public const string UserLocationIP = "UserLocationIP";
//     //     public const string ActionOutcomeRecorded = "ActionOutcomeRecorded";
//     // }

//     // /// <summary>
//     // /// Gaming-specific event types
//     // /// </summary>
//     // public static class GamingEvents
//     // {
//     //     // Level Events
//     //     public const string LevelStarted = "LevelStarted";
//     //     public const string LevelCompleted = "LevelCompleted";
//     //     public const string LevelFailed = "LevelFailed";
//     //     public const string LevelSkipped = "LevelSkipped";
//     //     public const string LevelRestarted = "LevelRestarted";

//     //     // Progress Events
//     //     public const string PlayerLevelUp = "PlayerLevelUp";
//     //     public const string XpGained = "XpGained";
//     //     public const string AchievementUnlocked = "AchievementUnlocked";
//     //     public const string QuestCompleted = "QuestCompleted";
//     //     public const string QuestStarted = "QuestStarted";

//     //     // Economy Events
//     //     public const string CurrencyEarned = "CurrencyEarned";
//     //     public const string CurrencySpent = "CurrencySpent";
//     //     public const string ItemPurchased = "ItemPurchased";
//     //     public const string ItemUsed = "ItemUsed";
//     //     public const string ItemCrafted = "ItemCrafted";
//     //     public const string PurchaseMade = "PurchaseMade";

//     //     // Gameplay Events
//     //     public const string GameStarted = "GameStarted";
//     //     public const string GameCompleted = "GameCompleted";
//     //     public const string GamePaused = "GamePaused";
//     //     public const string GameResumed = "GameResumed";
//     //     public const string PlayerDied = "PlayerDied";
//     //     public const string PlayerRespawned = "PlayerRespawned";

//     //     // Social Events
//     //     public const string PlayerInvited = "PlayerInvited";
//     //     public const string PlayerJoined = "PlayerJoined";
//     //     public const string PlayerLeft = "PlayerLeft";
//     //     public const string MessageSent = "MessageSent";
//     //     public const string FriendAdded = "FriendAdded";

//     //     // Monetization Events
//     //     public const string AdViewed = "AdViewed";
//     //     public const string AdClicked = "AdClicked";
//     //     public const string AdSkipped = "AdSkipped";
//     //     public const string IapPurchaseStarted = "IapPurchaseStarted";
//     //     public const string IapPurchaseCompleted = "IapPurchaseCompleted";
//     //     public const string IapPurchaseFailed = "IapPurchaseFailed";

//     //     // Tutorial Events
//     //     public const string TutorialStarted = "TutorialStarted";
//     //     public const string TutorialCompleted = "TutorialCompleted";
//     //     public const string TutorialSkipped = "TutorialSkipped";
//     //     public const string TutorialStepCompleted = "TutorialStepCompleted";

//     //     // Power-up and Booster Events
//     //     public const string BoosterActivated = "BoosterActivated";
//     //     public const string BoosterExpired = "BoosterExpired";
//     //     public const string PowerUpCollected = "PowerUpCollected";
//     //     public const string PowerUpUsed = "PowerUpUsed";

//     //     // Performance Events
//     //     public const string PerformanceMetric = "PerformanceMetric";
//     //     public const string ErrorOccurred = "ErrorOccurred";
//     //     public const string CrashReported = "CrashReported";
//     // }

//     /// <summary>
//     /// Gaming event enums for type safety
//     /// </summary>
//     public static class GamingEnums
//     {
//         public enum BoosterType
//         {
//             NotSet,
//             ScoreMultiplier,
//             TimeExtension,
//             ExtraLives,
//             DoubleCoins,
//             ShieldProtection,
//             SpeedBoost,
//             ExtraMoves,
//             HintReveal,
//             AutoComplete,
//             ResourceMultiplier
//         }

//         public enum CurrencyType
//         {
//             NotSet,
//             Coins,
//             Gems,
//             Energy,
//             Lives,
//             Tokens,
//             Points,
//             Stars,
//             Crystals,
//             Gold,
//             Silver
//         }

//         public enum CurrencyUpdateReason
//         {
//             NotSet,
//             Purchase,
//             Reward,
//             LevelComplete,
//             DailyBonus,
//             Achievement,
//             Gift,
//             Refund,
//             AdminAdjustment,
//             EventReward,
//             Consumed
//         }

//         public enum FailureReason
//         {
//             NotSet,
//             OutOfTime,
//             NotEnoughMoves,
//             PlayerDied,
//             ObjectiveFailed,
//             Cancelled,
//             NetworkError,
//             InsufficientResources,
//             SkillCheck,
//             RngFailure,
//             UserAbandoned
//         }

//         public enum MetricType
//         {
//             NotSet,
//             FrameRate,
//             LoadTime,
//             MemoryUsage,
//             BatteryUsage,
//             NetworkLatency,
//             ErrorCount,
//             CrashCount,
//             SessionDuration,
//             ClickRate,
//             CompletionRate
//         }

//         public enum OfferMethod
//         {
//             NotSet,
//             Popup,
//             Banner,
//             InGameStore,
//             LevelComplete,
//             DailyReward,
//             PushNotification,
//             EmailCampaign,
//             SocialShare,
//             Video,
//             Interstitial
//         }
//     }
// }