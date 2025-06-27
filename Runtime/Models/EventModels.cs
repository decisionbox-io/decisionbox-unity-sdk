using System;
using System.Collections.Generic;

#nullable enable

namespace DecisionBox.Models
{
    /// <summary>
    /// Base class for all event data
    /// </summary>
    public abstract class EventData
    {
        public string UserId { get; }
        public abstract string EventType { get; }

        protected EventData(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        /// <summary>
        /// Get the metadata dictionary for this event
        /// </summary>
        public abstract Dictionary<string, object> GetMetadata();
    }

    /// <summary>
    /// Game started event
    /// </summary>
    public class GameStartedEvent : EventData
    {
        public override string EventType => "GameStarted";
        public int? InitialSoftCurrency { get; }
        public int? InitialHardCurrency { get; }
        public int? InitialPremiumCurrency { get; }

        public GameStartedEvent(string userId, int? initialSoftCurrency = null, int? initialHardCurrency = null, int? initialPremiumCurrency = null)
            : base(userId)
        {
            InitialSoftCurrency = initialSoftCurrency;
            InitialHardCurrency = initialHardCurrency;
            InitialPremiumCurrency = initialPremiumCurrency;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>();
            if (InitialSoftCurrency.HasValue) metadata["initialSoftCurrency"] = InitialSoftCurrency.Value;
            if (InitialHardCurrency.HasValue) metadata["initialHardCurrency"] = InitialHardCurrency.Value;
            if (InitialPremiumCurrency.HasValue) metadata["initialPremiumCurrency"] = InitialPremiumCurrency.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Level started event
    /// </summary>
    public class LevelStartedEvent : EventData
    {
        public override string EventType => "LevelStarted";
        public int LevelNumber { get; }
        public string? LevelName { get; }
        public string? Difficulty { get; }

        public LevelStartedEvent(string userId, int levelNumber, string? levelName = null, string? difficulty = null)
            : base(userId)
        {
            LevelNumber = levelNumber;
            LevelName = levelName;
            Difficulty = difficulty;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber
            };
            if (!string.IsNullOrEmpty(LevelName)) metadata["levelName"] = LevelName;
            if (!string.IsNullOrEmpty(Difficulty)) metadata["difficulty"] = Difficulty;
            return metadata;
        }
    }

    /// <summary>
    /// Score updated event
    /// </summary>
    public class ScoreUpdatedEvent : EventData
    {
        public override string EventType => "ScoreUpdated";
        public int LevelNumber { get; }
        public int CurrentScore { get; }
        public int? OldScore { get; }
        public int? ScoreDelta { get; }
        public int ComboMultiplier { get; }

        public ScoreUpdatedEvent(string userId, int levelNumber, int currentScore, int? oldScore = null, int? scoreDelta = null, int comboMultiplier = 1)
            : base(userId)
        {
            LevelNumber = levelNumber;
            CurrentScore = currentScore;
            OldScore = oldScore;
            ScoreDelta = scoreDelta;
            ComboMultiplier = comboMultiplier;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["currentScore"] = CurrentScore,
                ["comboMultiplier"] = ComboMultiplier
            };
            if (OldScore.HasValue) metadata["oldScore"] = OldScore.Value;
            if (ScoreDelta.HasValue) metadata["scoreDelta"] = ScoreDelta.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Power-up collected event
    /// </summary>
    public class PowerUpCollectedEvent : EventData
    {
        public override string EventType => "PowerUpCollected";
        public int LevelNumber { get; }
        public string PowerUpType { get; }
        public int Quantity { get; }
        public double? Duration { get; }

        public PowerUpCollectedEvent(string userId, int levelNumber, string powerUpType, int quantity, double? duration = null)
            : base(userId)
        {
            LevelNumber = levelNumber;
            PowerUpType = powerUpType ?? throw new ArgumentNullException(nameof(powerUpType));
            Quantity = quantity;
            Duration = duration;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["powerUpType"] = PowerUpType,
                ["quantity"] = Quantity
            };
            if (Duration.HasValue) metadata["duration"] = Duration.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Currency balance updated event
    /// </summary>
    public class CurrencyBalanceUpdatedEvent : EventData
    {
        public override string EventType => "CurrencyBalanceUpdated";
        public CurrencyType CurrencyType { get; }
        public int? OldBalance { get; }
        public int CurrentBalance { get; }
        public int? Delta { get; }
        public CurrencyUpdateReason UpdateReason { get; }

        public CurrencyBalanceUpdatedEvent(string userId, CurrencyType currencyType, int? oldBalance, int currentBalance, int? delta, CurrencyUpdateReason updateReason)
            : base(userId)
        {
            CurrencyType = currencyType;
            OldBalance = oldBalance;
            CurrentBalance = currentBalance;
            Delta = delta;
            UpdateReason = updateReason;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["currencyType"] = CurrencyType.ToString(),
                ["currentBalance"] = CurrentBalance,
                ["updateReason"] = UpdateReason.ToString()
            };
            if (OldBalance.HasValue) metadata["oldBalance"] = OldBalance.Value;
            if (Delta.HasValue) metadata["delta"] = Delta.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Booster offered event
    /// </summary>
    public class BoosterOfferedEvent : EventData
    {
        public override string EventType => "BoosterOffered";
        public int LevelNumber { get; }
        public BoosterType BoosterType { get; }
        public OfferMethod OfferMethod { get; }
        public int RequiredCurrency { get; }

        public BoosterOfferedEvent(string userId, int levelNumber, BoosterType boosterType, OfferMethod offerMethod, int requiredCurrency)
            : base(userId)
        {
            LevelNumber = levelNumber;
            BoosterType = boosterType;
            OfferMethod = offerMethod;
            RequiredCurrency = requiredCurrency;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["boosterType"] = BoosterType.ToString(),
                ["offerMethod"] = OfferMethod.ToString(),
                ["requiredCurrency"] = RequiredCurrency
            };
        }
    }

    /// <summary>
    /// Booster accepted event
    /// </summary>
    public class BoosterAcceptedEvent : EventData
    {
        public override string EventType => "BoosterAccepted";
        public int LevelNumber { get; }
        public BoosterType BoosterType { get; }
        public AcceptMethod AcceptMethod { get; }
        public int SpentAmount { get; }

        public BoosterAcceptedEvent(string userId, int levelNumber, BoosterType boosterType, AcceptMethod acceptMethod, int spentAmount)
            : base(userId)
        {
            LevelNumber = levelNumber;
            BoosterType = boosterType;
            AcceptMethod = acceptMethod;
            SpentAmount = spentAmount;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["boosterType"] = BoosterType.ToString(),
                ["acceptMethod"] = AcceptMethod.ToString(),
                ["spentAmount"] = SpentAmount
            };
        }
    }

    /// <summary>
    /// Booster declined event
    /// </summary>
    public class BoosterDeclinedEvent : EventData
    {
        public override string EventType => "BoosterDeclined";
        public int LevelNumber { get; }
        public BoosterType BoosterType { get; }
        public DeclineReason DeclineReason { get; }

        public BoosterDeclinedEvent(string userId, int levelNumber, BoosterType boosterType, DeclineReason declineReason)
            : base(userId)
        {
            LevelNumber = levelNumber;
            BoosterType = boosterType;
            DeclineReason = declineReason;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["boosterType"] = BoosterType.ToString(),
                ["declineReason"] = DeclineReason.ToString()
            };
        }
    }

    /// <summary>
    /// Action outcome recorded event
    /// </summary>
    public class ActionOutcomeRecordedEvent : EventData
    {
        public override string EventType => "ActionOutcomeRecorded";
        public string RulesetId { get; }
        public OfferMethod OfferMethod { get; }
        public bool IsPositive { get; }
        public string Outcome { get; }
        public double? Value { get; }

        public ActionOutcomeRecordedEvent(string userId, string rulesetId, OfferMethod offerMethod, bool isPositive, string outcome, double? value = null)
            : base(userId)
        {
            RulesetId = rulesetId ?? throw new ArgumentNullException(nameof(rulesetId));
            OfferMethod = offerMethod;
            IsPositive = isPositive;
            Outcome = outcome ?? throw new ArgumentNullException(nameof(outcome));
            Value = value;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["rulesetId"] = RulesetId,
                ["offerMethod"] = OfferMethod.ToString(),
                ["isPositive"] = IsPositive,
                ["outcome"] = Outcome
            };
            if (Value.HasValue) metadata["value"] = Value.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Level finished event
    /// </summary>
    public class LevelFinishedEvent : EventData
    {
        public override string EventType => "LevelFinished";
        public int LevelNumber { get; }

        public LevelFinishedEvent(string userId, int levelNumber)
            : base(userId)
        {
            LevelNumber = levelNumber;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber
            };
        }
    }

    /// <summary>
    /// Level success event
    /// </summary>
    public class LevelSuccessEvent : EventData
    {
        public override string EventType => "LevelSuccess";
        public int LevelNumber { get; }

        public LevelSuccessEvent(string userId, int levelNumber)
            : base(userId)
        {
            LevelNumber = levelNumber;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber
            };
        }
    }

    /// <summary>
    /// Level failed event
    /// </summary>
    public class LevelFailedEvent : EventData
    {
        public override string EventType => "LevelFailed";
        public int LevelNumber { get; }
        public FailureReason FailureReason { get; }

        public LevelFailedEvent(string userId, int levelNumber, FailureReason failureReason)
            : base(userId)
        {
            LevelNumber = levelNumber;
            FailureReason = failureReason;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["failureReason"] = FailureReason.ToString()
            };
        }
    }

    /// <summary>
    /// Game ended event
    /// </summary>
    public class GameEndedEvent : EventData
    {
        public override string EventType => "GameEnded";
        public int CurrentScore { get; }
        public int? FinalSoftCurrency { get; }
        public int? FinalHardCurrency { get; }
        public int? FinalPremiumCurrency { get; }

        public GameEndedEvent(string userId, int currentScore, int? finalSoftCurrency = null, int? finalHardCurrency = null, int? finalPremiumCurrency = null)
            : base(userId)
        {
            CurrentScore = currentScore;
            FinalSoftCurrency = finalSoftCurrency;
            FinalHardCurrency = finalHardCurrency;
            FinalPremiumCurrency = finalPremiumCurrency;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["currentScore"] = CurrentScore
            };
            if (FinalSoftCurrency.HasValue) metadata["finalSoftCurrency"] = FinalSoftCurrency.Value;
            if (FinalHardCurrency.HasValue) metadata["finalHardCurrency"] = FinalHardCurrency.Value;
            if (FinalPremiumCurrency.HasValue) metadata["finalPremiumCurrency"] = FinalPremiumCurrency.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Game over event
    /// </summary>
    public class GameOverEvent : EventData
    {
        public override string EventType => "GameOver";
        public int LevelNumber { get; }
        public int CurrentScore { get; }
        public FailureReason GameOverReason { get; }

        public GameOverEvent(string userId, int levelNumber, int currentScore, FailureReason gameOverReason)
            : base(userId)
        {
            LevelNumber = levelNumber;
            CurrentScore = currentScore;
            GameOverReason = gameOverReason;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["currentScore"] = CurrentScore,
                ["gameOverReason"] = GameOverReason.ToString()
            };
        }
    }

    /// <summary>
    /// Metric recorded event
    /// </summary>
    public class MetricRecordedEvent : EventData
    {
        public override string EventType => "MetricRecorded";
        public int LevelNumber { get; }
        public MetricType Metric { get; }
        public float MetricValue { get; }

        public MetricRecordedEvent(string userId, int levelNumber, MetricType metric, float metricValue)
            : base(userId)
        {
            LevelNumber = levelNumber;
            Metric = metric;
            MetricValue = metricValue;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["metric"] = Metric.ToString(),
                ["metricValue"] = MetricValue
            };
        }
    }

    /// <summary>
    /// Session started event (internal use)
    /// </summary>
    public class SessionStartedEvent : EventData
    {
        public override string EventType => "SessionStarted";
        public string GameVersion { get; }
        public PlatformType Platform { get; }
        public string DeviceModel { get; }

        public SessionStartedEvent(string userId, string gameVersion, PlatformType platform, string deviceModel)
            : base(userId)
        {
            GameVersion = gameVersion ?? throw new ArgumentNullException(nameof(gameVersion));
            Platform = platform;
            DeviceModel = deviceModel ?? throw new ArgumentNullException(nameof(deviceModel));
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["gameVersion"] = GameVersion,
                ["platform"] = Platform.ToString(),
                ["deviceModel"] = DeviceModel
            };
        }
    }

    /// <summary>
    /// Session ended event (internal use)
    /// </summary>
    public class SessionEndedEvent : EventData
    {
        public override string EventType => "SessionEnded";
        public string Reason { get; }

        public SessionEndedEvent(string userId, string reason)
            : base(userId)
        {
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["reason"] = Reason
            };
        }
    }

    /// <summary>
    /// User location text event
    /// </summary>
    public class UserLocationTextEvent : EventData
    {
        public override string EventType => "UserLocationText";
        public string Location { get; }

        public UserLocationTextEvent(string userId, string location)
            : base(userId)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["location"] = Location
            };
        }
    }

    /// <summary>
    /// User location lat/lng event
    /// </summary>
    public class UserLocationLatLngEvent : EventData
    {
        public override string EventType => "UserLocationLatLng";
        public double Latitude { get; }
        public double Longitude { get; }

        public UserLocationLatLngEvent(string userId, double latitude, double longitude)
            : base(userId)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["latitude"] = Latitude,
                ["longitude"] = Longitude
            };
        }
    }

    /// <summary>
    /// User location IP event
    /// </summary>
    public class UserLocationIPEvent : EventData
    {
        public override string EventType => "UserLocationIP";
        public string IpAddress { get; }

        public UserLocationIPEvent(string userId, string ipAddress)
            : base(userId)
        {
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["ipAddress"] = IpAddress
            };
        }
    }

    /// <summary>
    /// User touch event
    /// </summary>
    public class UserTouchEvent : EventData
    {
        public override string EventType => "UserTouch";
        public int FingerId { get; }
        public string Phase { get; }
        public float NormalizedX { get; }
        public float NormalizedY { get; }
        public float RawX { get; }
        public float RawY { get; }
        public float Timestamp { get; }

        public UserTouchEvent(string userId, int fingerId, string phase, float normalizedX, float normalizedY, float rawX, float rawY, float timestamp)
            : base(userId)
        {
            FingerId = fingerId;
            Phase = phase ?? throw new ArgumentNullException(nameof(phase));
            NormalizedX = normalizedX;
            NormalizedY = normalizedY;
            RawX = rawX;
            RawY = rawY;
            Timestamp = timestamp;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["fingerId"] = FingerId,
                ["phase"] = Phase,
                ["normalizedX"] = NormalizedX,
                ["normalizedY"] = NormalizedY,
                ["rawX"] = RawX,
                ["rawY"] = RawY,
                ["timestamp"] = Timestamp
            };
        }
    }
}