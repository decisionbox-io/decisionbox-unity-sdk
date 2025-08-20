#nullable enable

namespace DecisionBox.Models
{
    /// <summary>
    /// Types of currency in the game
    /// </summary>
    public enum CurrencyType
    {
        Soft,
        Hard,
        Premium
    }

    /// <summary>
    /// Reasons for currency balance updates
    /// </summary>
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

    /// <summary>
    /// Reasons for booster inventory changes
    /// </summary>
    public enum BoosterUpdateReason
    {
        NotSpecified,
        Purchase,
        Reward,
        Used,
        Expired,
        Refund,
        Achievement,
        DailyLogin,
        AdReward,
        EventReward,
        LevelCompletion,
        Gift,
        SeasonPass,
        Tutorial,
        SystemAdjustment
    }

    /// <summary>
    /// Types of boosters available in the game
    /// </summary>
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
        EnergyBoost,
        AttackBoost,
        DefenseBoost,
        BonusXP,
        BonusGold,
        SuperCharge,
        RapidFire,
        MegaBlast,
        Reflector,
        ExplosiveCharge,
        UltimateCombo,
        Fortify,
        Recovery,
        Sprint,
        HyperSpeed,
        Frenzy,
        MagicBoost,
        LuckBooster,
        ShieldRegeneration,
        ArmorBoost,
        CriticalChanceBoost,
        XPBoost,
        ResourceMultiplier,
        ElementalBoost,
        FireBoost,
        IceBoost,
        ElectricBoost,
        WindBoost,
        EarthBoost,
        WaterBoost,
        LightBoost,
        DarkBoost,
        ComboEnhancer,
        ScoreEnhancer,
        Hint
    }

    /// <summary>
    /// Methods for offering boosters or items
    /// </summary>
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
        Coupon,
        Gift,
        RedeemCode,
        LoyaltyReward,
        TimeLimitedOffer,
        PromotionalOffer,
        BonusQuest,
        SpecialEventOffer,
        NotificationOffer,
        ReferralOffer,
        VIPOffer,
        FlashSale
    }

    /// <summary>
    /// Methods for accepting offers
    /// </summary>
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
        DailyBonus,
        SpecialEvent,
        MysteryBox,
        TimedResponse,
        EmailConfirmation,
        SurveyCompletion
    }

    /// <summary>
    /// Source of booster activation
    /// </summary>
    public enum BoosterSource
    {
        Inventory,
        PreGame,
        InGame,
        Store,
        Reward,
        Achievement,
        LevelStart,
        PowerUpMenu,
        QuickAction,
        AutoActivated,
        Tutorial,
        EventReward,
        SeasonPass,
        DailyBonus,
        BattlePass
    }

    /// <summary>
    /// Reasons for declining offers
    /// </summary>
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

    /// <summary>
    /// Reasons for restarting a level
    /// </summary>
    public enum RestartReason
    {
        PlayerChoice,
        AfterFailure,
        Tutorial,
        Practice,
        Challenge,
        Improvement,
        MissedObjective,
        WrongStrategy,
        Accident,
        Retry,
        PowerUpTest,
        Achievement,
        Leaderboard,
        PerfectScore,
        BetterReward,
        UnlockContent,
        DailyChallenge,
        EventRequirement
    }

    /// <summary>
    /// Reasons for level or game failure
    /// </summary>
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
        TechnicalError,
        NoLivesRemaining,
        TimeExpired,
        AccumulatedLevelFailures,
        Bankruptcy,
        Disconnected,
        Quit,
        Overwhelmed,
        NotEnoughMoves
    }

    /// <summary>
    /// Types of metrics that can be recorded
    /// </summary>
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
        Jumps,
        AttacksMade,
        DamageTaken,
        CriticalHits,
        ComboCount,
        QuestsCompleted,
        AchievementsUnlocked,
        Deaths,
        MultiplayerWins,
        MultiplayerLosses,
        InGamePurchases,
        AdsWatched,
        ReferralInvitesSent,
        ReferralInvitesAccepted,
        EnergyConsumed,
        EnergyRemaining,
        EnergyRestored,
        HealthRestored,
        SkillUses,
        ResourcesGathered,
        ExplorationPoints,
        BonusPoints,
        TargetScore,
        StarsAwarded,
        TotalClicks
    }

    /// <summary>
    /// Placement locations for ads
    /// </summary>
    public enum AdPlacement
    {
        LevelStart,
        LevelEnd,
        LevelFailed,
        LevelSuccess,
        MainMenu,
        PauseMenu,
        Store,
        Settings,
        GameOver,
        Checkpoint,
        BetweenLevels,
        Tutorial,
        Achievement,
        DailyReward,
        Loading,
        Inventory,
        Leaderboard,
        BeforeMatch,
        AfterMatch,
        MidLevel,
        BossDefeat,
        QuestComplete,
        Other
    }

    /// <summary>
    /// Types of rewards from ads
    /// </summary>
    public enum AdRewardType
    {
        Coins,
        Gems,
        Lives,
        Moves,
        Time,
        Booster,
        PowerUp,
        Character,
        Skin,
        Item,
        Energy,
        Hint,
        Skip,
        Continue,
        Checkpoint,
        Multiplier,
        Shield,
        Revive,
        Unlock,
        Experience,
        Token,
        Ticket,
        Key,
        Chest,
        RandomReward,
        Other
    }

    /// <summary>
    /// Reasons for offering rewarded ads
    /// </summary>
    public enum AdOfferReason
    {
        ContinueLevel,
        ExtraLife,
        ExtraMoves,
        SkipLevel,
        UnlockContent,
        DoubleReward,
        FreeBooster,
        FreeCurrency,
        FreeItem,
        ReviveCharacter,
        RefillEnergy,
        SpeedUpTimer,
        UnlockCharacter,
        BonusReward,
        DailyReward,
        WheelSpin,
        MysteryBox,
        RemoveAds,
        ExtraAttempt,
        HintReveal,
        PowerUpActivation,
        CheckpointSave,
        ScoreMultiplier,
        Other
    }

    /// <summary>
    /// Results of rewarded ad offers
    /// </summary>
    public enum AdResult
    {
        Watched,
        Skipped,
        Failed,
        NotAvailable,
        Cancelled,
        NetworkError,
        Timeout
    }

    /// <summary>
    /// Screen types in the game
    /// </summary>
    public enum ScreenType
    {
        // Main Screens
        MainMenu,
        SplashScreen,
        LoadingScreen,
        HomeScreen,
        
        // Game Screens
        GamePlay,
        LevelSelect,
        WorldMap,
        Campaign,
        Tutorial,
        Practice,
        
        // Menu Screens
        Settings,
        Options,
        AudioSettings,
        VideoSettings,
        ControlSettings,
        LanguageSettings,
        
        // Store/Shop Screens
        Store,
        ItemShop,
        BoosterShop,
        CurrencyShop,
        BundleShop,
        SpecialOffers,
        SeasonPass,
        BattlePass,
        
        // Player Screens
        Profile,
        PlayerStats,
        Inventory,
        Collection,
        Achievements,
        Leaderboard,
        Friends,
        Guild,
        Clan,
        Team,
        
        // Progress Screens
        LevelComplete,
        LevelFailed,
        GameOver,
        Victory,
        Defeat,
        Results,
        Rewards,
        
        // Social Screens
        Chat,
        Messages,
        Notifications,
        FriendsList,
        Social,
        Multiplayer,
        Matchmaking,
        Lobby,
        
        // Economy Screens
        Bank,
        Wallet,
        Exchange,
        Trading,
        Auction,
        
        // Customization Screens
        Character,
        Avatar,
        Skins,
        Cosmetics,
        Loadout,
        Deck,
        Equipment,
        Upgrades,
        
        // Information Screens
        Help,
        FAQ,
        Support,
        About,
        Credits,
        PrivacyPolicy,
        TermsOfService,
        ReleaseNotes,
        News,
        
        // Event Screens
        Events,
        DailyChallenge,
        WeeklyChallenge,
        Tournament,
        Competition,
        SpecialEvent,
        LiveEvent,
        
        // Popup/Modal Screens
        Popup,
        Dialog,
        Confirmation,
        Warning,
        Error,
        RateApp,
        CrossPromotion,
        AdReward,
        
        // Other Screens
        Login,
        Registration,
        AccountRecovery,
        Onboarding,
        FirstTimeUser,
        WelcomeBack,
        WhatsNew,
        Maintenance,
        Unknown
    }

    /// <summary>
    /// Platform types
    /// </summary>
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
}