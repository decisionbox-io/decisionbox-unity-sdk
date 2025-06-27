#nullable enable

namespace DecisionBox.Models
{
    /// <summary>
    /// Static constants for all events and enums to ensure customers don't use strings
    /// </summary>
    public static class EventConstants
    {
        /// <summary>
        /// Event type constants
        /// </summary>
        public static class Events
        {
            public const string GameStarted = "GameStarted";
            public const string LevelStarted = "LevelStarted";
            public const string ScoreUpdated = "ScoreUpdated";
            public const string PowerUpCollected = "PowerUpCollected";
            public const string CurrencyBalanceUpdated = "CurrencyBalanceUpdated";
            public const string BoosterOffered = "BoosterOffered";
            public const string BoosterAccepted = "BoosterAccepted";
            public const string BoosterDeclined = "BoosterDeclined";
            public const string ActionOutcomeRecorded = "ActionOutcomeRecorded";
            public const string LevelFinished = "LevelFinished";
            public const string LevelSuccess = "LevelSuccess";
            public const string LevelFailed = "LevelFailed";
            public const string GameEnded = "GameEnded";
            public const string GameOver = "GameOver";
            public const string MetricRecorded = "MetricRecorded";
            public const string SessionStarted = "SessionStarted";
            public const string SessionEnded = "SessionEnded";
            public const string UserLocationText = "UserLocationText";
            public const string UserLocationLatLng = "UserLocationLatLng";
            public const string UserLocationIP = "UserLocationIP";
            public const string UserTouch = "UserTouch";
        }

        /// <summary>
        /// Currency type constants
        /// </summary>
        public static class Currency
        {
            public static readonly CurrencyType Soft = CurrencyType.Soft;
            public static readonly CurrencyType Hard = CurrencyType.Hard;
            public static readonly CurrencyType Premium = CurrencyType.Premium;
        }

        /// <summary>
        /// Currency update reason constants
        /// </summary>
        public static class CurrencyUpdateReasons
        {
            public static readonly CurrencyUpdateReason NotSpecified = CurrencyUpdateReason.NotSpecified;
            public static readonly CurrencyUpdateReason Purchase = CurrencyUpdateReason.Purchase;
            public static readonly CurrencyUpdateReason Reward = CurrencyUpdateReason.Reward;
            public static readonly CurrencyUpdateReason Spending = CurrencyUpdateReason.Spending;
            public static readonly CurrencyUpdateReason Refund = CurrencyUpdateReason.Refund;
            public static readonly CurrencyUpdateReason Bonus = CurrencyUpdateReason.Bonus;
            public static readonly CurrencyUpdateReason Achievement = CurrencyUpdateReason.Achievement;
            public static readonly CurrencyUpdateReason DailyLogin = CurrencyUpdateReason.DailyLogin;
            public static readonly CurrencyUpdateReason AdReward = CurrencyUpdateReason.AdReward;
            public static readonly CurrencyUpdateReason CurrencyExchange = CurrencyUpdateReason.CurrencyExchange;
            public static readonly CurrencyUpdateReason SystemAdjustment = CurrencyUpdateReason.SystemAdjustment;
        }

        /// <summary>
        /// Booster type constants
        /// </summary>
        public static class Boosters
        {
            public static readonly BoosterType SpeedBoost = BoosterType.SpeedBoost;
            public static readonly BoosterType Shield = BoosterType.Shield;
            public static readonly BoosterType DoublePoints = BoosterType.DoublePoints;
            public static readonly BoosterType ExtraLife = BoosterType.ExtraLife;
            public static readonly BoosterType Invincibility = BoosterType.Invincibility;
            public static readonly BoosterType Magnet = BoosterType.Magnet;
            public static readonly BoosterType ComboMultiplier = BoosterType.ComboMultiplier;
            public static readonly BoosterType DamageBoost = BoosterType.DamageBoost;
            public static readonly BoosterType HealthRegen = BoosterType.HealthRegen;
            public static readonly BoosterType CriticalHitBoost = BoosterType.CriticalHitBoost;
            public static readonly BoosterType ScoreMultiplier = BoosterType.ScoreMultiplier;
            public static readonly BoosterType TimeExtension = BoosterType.TimeExtension;
            public static readonly BoosterType CoinMultiplier = BoosterType.CoinMultiplier;
            public static readonly BoosterType GemFinder = BoosterType.GemFinder;
            // Add more as needed...
        }

        /// <summary>
        /// Offer method constants
        /// </summary>
        public static class OfferMethods
        {
            public static readonly OfferMethod WatchAd = OfferMethod.WatchAd;
            public static readonly OfferMethod SpendCurrency = OfferMethod.SpendCurrency;
            public static readonly OfferMethod InAppPurchase = OfferMethod.InAppPurchase;
            public static readonly OfferMethod CompleteChallenge = OfferMethod.CompleteChallenge;
            public static readonly OfferMethod DailyBonus = OfferMethod.DailyBonus;
            public static readonly OfferMethod RewardedVideo = OfferMethod.RewardedVideo;
            public static readonly OfferMethod SocialShare = OfferMethod.SocialShare;
            // Add more as needed...
        }

        /// <summary>
        /// Accept method constants
        /// </summary>
        public static class AcceptMethods
        {
            public static readonly AcceptMethod WatchAd = AcceptMethod.WatchAd;
            public static readonly AcceptMethod SpendCurrency = AcceptMethod.SpendCurrency;
            public static readonly AcceptMethod InAppPurchase = AcceptMethod.InAppPurchase;
            public static readonly AcceptMethod Reward = AcceptMethod.Reward;
            public static readonly AcceptMethod SocialShare = AcceptMethod.SocialShare;
            public static readonly AcceptMethod DirectAccept = AcceptMethod.DirectAccept;
            // Add more as needed...
        }

        /// <summary>
        /// Decline reason constants
        /// </summary>
        public static class DeclineReasons
        {
            public static readonly DeclineReason NotInterested = DeclineReason.NotInterested;
            public static readonly DeclineReason InsufficientFunds = DeclineReason.InsufficientFunds;
            public static readonly DeclineReason AlreadyActive = DeclineReason.AlreadyActive;
            public static readonly DeclineReason AdUnavailable = DeclineReason.AdUnavailable;
            public static readonly DeclineReason AdSkipped = DeclineReason.AdSkipped;
            public static readonly DeclineReason CancelledByUser = DeclineReason.CancelledByUser;
            public static readonly DeclineReason TimeExpired = DeclineReason.TimeExpired;
            public static readonly DeclineReason TechnicalIssue = DeclineReason.TechnicalIssue;
            // Add more as needed...
        }

        /// <summary>
        /// Failure reason constants
        /// </summary>
        public static class FailureReasons
        {
            public static readonly FailureReason TimeOut = FailureReason.TimeOut;
            public static readonly FailureReason NoLives = FailureReason.NoLives;
            public static readonly FailureReason CollisionWithObstacle = FailureReason.CollisionWithObstacle;
            public static readonly FailureReason PuzzleNotSolved = FailureReason.PuzzleNotSolved;
            public static readonly FailureReason ObjectiveNotCompleted = FailureReason.ObjectiveNotCompleted;
            public static readonly FailureReason EnemyDefeatedPlayer = FailureReason.EnemyDefeatedPlayer;
            public static readonly FailureReason OverwhelmedByEnemies = FailureReason.OverwhelmedByEnemies;
            public static readonly FailureReason WrongMove = FailureReason.WrongMove;
            public static readonly FailureReason NotEnoughMoves = FailureReason.NotEnoughMoves;
            // Add more as needed...
        }

        /// <summary>
        /// Metric type constants
        /// </summary>
        public static class Metrics
        {
            public static readonly MetricType Score = MetricType.Score;
            public static readonly MetricType MovesMade = MetricType.MovesMade;
            public static readonly MetricType MovesRemaining = MetricType.MovesRemaining;
            public static readonly MetricType LivesRemaining = MetricType.LivesRemaining;
            public static readonly MetricType LivesLost = MetricType.LivesLost;
            public static readonly MetricType EnemiesDefeated = MetricType.EnemiesDefeated;
            public static readonly MetricType ItemsCollected = MetricType.ItemsCollected;
            public static readonly MetricType PowerUpsUsed = MetricType.PowerUpsUsed;
            public static readonly MetricType TimePlayed = MetricType.TimePlayed;
            public static readonly MetricType LevelsCompleted = MetricType.LevelsCompleted;
            // Add more as needed...
        }

        /// <summary>
        /// Platform type constants
        /// </summary>
        public static class Platforms
        {
            public static readonly PlatformType Android = PlatformType.Android;
            public static readonly PlatformType iOS = PlatformType.iOS;
            public static readonly PlatformType Web = PlatformType.Web;
        }
    }
}