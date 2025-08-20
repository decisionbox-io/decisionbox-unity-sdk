using System;
using System.Collections.Generic;
using DecisionBox.Core;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable

namespace DecisionBox.Models
{
    /// <summary>
    /// Base class for all event data
    /// </summary>
    public abstract class EventData
    {
        [JsonProperty("user_id")]
        public string UserId { get; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("event_type")]
        public abstract string EventType { get; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        private Dictionary<string, object>? _metadata;

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata 
        { 
            get
            {
                if (_metadata == null)
                {
                    _metadata = GetMetadata();
                    // Ensure all events include version, platform, and deviceModel
                    if (!_metadata.ContainsKey("version"))
                        _metadata["version"] = Application.version;
                    if (!_metadata.ContainsKey("platform"))
                        _metadata["platform"] = DecisionBoxSDK.Instance.GetPlatformType().ToString();
                    if (!_metadata.ContainsKey("deviceModel"))
                        _metadata["deviceModel"] = SystemInfo.deviceModel;
                }
                return _metadata;
            }
            set => _metadata = value;
        }

        protected EventData(string userId, string sessionId, string appId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            SessionId = sessionId;
            AppId = appId ?? "default_app_id";
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }


        /// <summary>
        /// Get the metadata dictionary for this event
        /// </summary>
        public abstract Dictionary<string, object> GetMetadata();
    }

    /// <summary>
    /// User Device Token model
    /// </summary>
    [Serializable]
    public class UserDeviceToken
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("device_token")]
        public string DeviceToken { get; set; } 

        public UserDeviceToken(string userId, string platform, string deviceToken)
        {
            UserId = userId;
            Platform = platform;
            DeviceToken = deviceToken;
        }
    }

    /// <summary>
    /// Game started event
    /// </summary>
    public class GameStartedEvent : EventData
    {
        public override string EventType => "GameStarted";
        [JsonIgnore]
        public int? InitialSoftCurrency { get; }
        [JsonIgnore]
        public int? InitialHardCurrency { get; }
        [JsonIgnore]
        public int? InitialPremiumCurrency { get; }

        public GameStartedEvent(string userId, int? initialSoftCurrency = null, int? initialHardCurrency = null, int? initialPremiumCurrency = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public string? LevelName { get; }
        [JsonIgnore]
        public string? Difficulty { get; }

        public LevelStartedEvent(string userId, int levelNumber, string? levelName = null, string? difficulty = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public int CurrentScore { get; }
        [JsonIgnore]
        public int? OldScore { get; }
        [JsonIgnore]
        public int? ScoreDelta { get; }
        [JsonIgnore]
        public int ComboMultiplier { get; }

        public ScoreUpdatedEvent(string userId, int levelNumber, int currentScore, int? oldScore = null, int? scoreDelta = null, int comboMultiplier = 1)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public string PowerUpType { get; }
        [JsonIgnore]
        public int Quantity { get; }
        [JsonIgnore]
        public double? Duration { get; }

        public PowerUpCollectedEvent(string userId, int levelNumber, string powerUpType, int quantity, double? duration = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public CurrencyType CurrencyType { get; }
        [JsonIgnore]
        public int? OldBalance { get; }
        [JsonIgnore]
        public int CurrentBalance { get; }
        [JsonIgnore]
        public int? Delta { get; }
        [JsonIgnore]
        public CurrencyUpdateReason UpdateReason { get; }

        public CurrencyBalanceUpdatedEvent(string userId, CurrencyType currencyType, int? oldBalance, int currentBalance, int? delta, CurrencyUpdateReason updateReason)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public BoosterType BoosterType { get; }
        [JsonIgnore]
        public OfferMethod OfferMethod { get; }
        [JsonIgnore]
        public int RequiredCurrency { get; }

        public BoosterOfferedEvent(string userId, int levelNumber, BoosterType boosterType, OfferMethod offerMethod, int requiredCurrency)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public BoosterType BoosterType { get; }
        [JsonIgnore]
        public AcceptMethod AcceptMethod { get; }
        [JsonIgnore]
        public int SpentAmount { get; }

        public BoosterAcceptedEvent(string userId, int levelNumber, BoosterType boosterType, AcceptMethod acceptMethod, int spentAmount)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public BoosterType BoosterType { get; }
        [JsonIgnore]
        public DeclineReason DeclineReason { get; }

        public BoosterDeclinedEvent(string userId, int levelNumber, BoosterType boosterType, DeclineReason declineReason)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
    /// Booster used event - tracks when a player uses a booster from inventory
    /// </summary>
    public class BoosterUsedEvent : EventData
    {
        public override string EventType => "BoosterUsed";
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public BoosterType BoosterType { get; }
        [JsonIgnore]
        public BoosterSource Source { get; }
        [JsonIgnore]
        public int Quantity { get; }
        [JsonIgnore]
        public int? RemainingCount { get; }

        public BoosterUsedEvent(string userId, int levelNumber, BoosterType boosterType, BoosterSource source = BoosterSource.Inventory, int quantity = 1, int? remainingCount = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
        {
            LevelNumber = levelNumber;
            BoosterType = boosterType;
            Source = source;
            Quantity = quantity;
            RemainingCount = remainingCount;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["boosterType"] = BoosterType.ToString(),
                ["source"] = Source.ToString(),
                ["quantity"] = Quantity
            };
            if (RemainingCount.HasValue) metadata["remainingCount"] = RemainingCount.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Action outcome recorded event
    /// </summary>
    public class ActionOutcomeRecordedEvent : EventData
    {
        public override string EventType => "ActionOutcomeRecorded";
        [JsonIgnore]
        public string RulesetId { get; }
        [JsonIgnore]
        public OfferMethod OfferMethod { get; }
        [JsonIgnore]
        public string Outcome { get; }
        [JsonIgnore]
        public bool IsPositive { get; }
        [JsonIgnore]
        public double? Value { get; }

        public ActionOutcomeRecordedEvent(string userId, string rulesetId, OfferMethod offerMethod, AcceptMethod acceptMethod, bool isPositive, double? value = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
        {
            RulesetId = rulesetId ?? throw new ArgumentNullException(nameof(rulesetId));
            OfferMethod = offerMethod;
            Outcome = acceptMethod.ToString();
            IsPositive = isPositive;
            Value = value;
        }

        public ActionOutcomeRecordedEvent(string userId, string rulesetId, OfferMethod offerMethod, DeclineReason declineReason, bool isPositive, double? value = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
        {
            RulesetId = rulesetId ?? throw new ArgumentNullException(nameof(rulesetId));
            OfferMethod = offerMethod;
            Outcome = declineReason.ToString();
            IsPositive = isPositive;
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
        [JsonIgnore]
        public int LevelNumber { get; }

        public LevelFinishedEvent(string userId, int levelNumber)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }

        public LevelSuccessEvent(string userId, int levelNumber)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public FailureReason FailureReason { get; }

        public LevelFailedEvent(string userId, int levelNumber, FailureReason failureReason)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
    /// Level restarted event
    /// </summary>
    public class LevelRestartedEvent : EventData
    {
        public override string EventType => "LevelRestarted";
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public int? AttemptNumber { get; }
        [JsonIgnore]
        public RestartReason RestartReason { get; }

        public LevelRestartedEvent(string userId, int levelNumber, RestartReason restartReason = RestartReason.PlayerChoice, int? attemptNumber = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
        {
            LevelNumber = levelNumber;
            AttemptNumber = attemptNumber;
            RestartReason = restartReason;
        }

        public override Dictionary<string, object> GetMetadata()
        {
            var metadata = new Dictionary<string, object>
            {
                ["levelNumber"] = LevelNumber,
                ["restartReason"] = RestartReason.ToString()
            };
            if (AttemptNumber.HasValue) metadata["attemptNumber"] = AttemptNumber.Value;
            return metadata;
        }
    }

    /// <summary>
    /// Game ended event
    /// </summary>
    public class GameEndedEvent : EventData
    {
        public override string EventType => "GameEnded";
        [JsonIgnore]
        public int CurrentScore { get; }
        [JsonIgnore]
        public int? FinalSoftCurrency { get; }
        [JsonIgnore]
        public int? FinalHardCurrency { get; }
        [JsonIgnore]
        public int? FinalPremiumCurrency { get; }

        public GameEndedEvent(string userId, int currentScore, int? finalSoftCurrency = null, int? finalHardCurrency = null, int? finalPremiumCurrency = null)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public int CurrentScore { get; }
        [JsonIgnore]
        public FailureReason GameOverReason { get; }

        public GameOverEvent(string userId, int levelNumber, int currentScore, FailureReason gameOverReason)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public int LevelNumber { get; }
        [JsonIgnore]
        public MetricType Metric { get; }
        [JsonIgnore]
        public float MetricValue { get; }

        public MetricRecordedEvent(string userId, int levelNumber, MetricType metric, float metricValue)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public string GameVersion { get; }
        [JsonIgnore]
        public PlatformType Platform { get; }
        [JsonIgnore]
        public string DeviceModel { get; }

        public SessionStartedEvent(string userId, string gameVersion, PlatformType platform, string deviceModel = "Unknown")
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
        {
            GameVersion = gameVersion ?? throw new ArgumentNullException(nameof(gameVersion));
            Platform = platform;
            DeviceModel = deviceModel ?? throw new ArgumentNullException(nameof(deviceModel));
        }

        public override Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["version"] = GameVersion,
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
        [JsonIgnore]
        public string Reason { get; }

        public SessionEndedEvent(string userId, string reason)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public string Location { get; }

        public UserLocationTextEvent(string userId, string location)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public double Latitude { get; }
        [JsonIgnore]
        public double Longitude { get; }

        public UserLocationLatLngEvent(string userId, double latitude, double longitude)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
        [JsonIgnore]
        public string IpAddress { get; }

        public UserLocationIPEvent(string userId, string ipAddress)
            : base(userId, DecisionBoxSDK.Instance.CurrentSessionId, DecisionBoxSDK.Instance.GetAppID())
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
}