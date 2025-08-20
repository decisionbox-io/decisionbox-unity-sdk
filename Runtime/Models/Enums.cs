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
        ScoreEnhancer
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
        StarsAwarded
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