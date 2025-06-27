using System;
using System.Collections.Generic;
using NUnit.Framework;
using DecisionBox.Models;

namespace DecisionBox.Tests
{
    [TestFixture]
    public class EventModelTests
    {
        private const string TEST_USER_ID = "test_user_123";

        #region GameStartedEvent Tests

        [Test]
        public void GameStartedEvent_ConstructsCorrectly()
        {
            var evt = new GameStartedEvent(TEST_USER_ID, 100, 50, 10);
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("GameStarted", evt.EventType);
            Assert.AreEqual(100, evt.InitialSoftCurrency);
            Assert.AreEqual(50, evt.InitialHardCurrency);
            Assert.AreEqual(10, evt.InitialPremiumCurrency);
        }

        [Test]
        public void GameStartedEvent_GetMetadata_WithAllValues()
        {
            var evt = new GameStartedEvent(TEST_USER_ID, 100, 50, 10);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(3, metadata.Count);
            Assert.AreEqual(100, metadata["initialSoftCurrency"]);
            Assert.AreEqual(50, metadata["initialHardCurrency"]);
            Assert.AreEqual(10, metadata["initialPremiumCurrency"]);
        }

        [Test]
        public void GameStartedEvent_GetMetadata_WithNullValues()
        {
            var evt = new GameStartedEvent(TEST_USER_ID, null, null, null);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void GameStartedEvent_ThrowsOnNullUserId()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStartedEvent(null!, 100, 50, 10));
        }

        #endregion

        #region LevelStartedEvent Tests

        [Test]
        public void LevelStartedEvent_ConstructsCorrectly()
        {
            var evt = new LevelStartedEvent(TEST_USER_ID, 5, "Boss Level", "Hard");
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("LevelStarted", evt.EventType);
            Assert.AreEqual(5, evt.LevelNumber);
            Assert.AreEqual("Boss Level", evt.LevelName);
            Assert.AreEqual("Hard", evt.Difficulty);
        }

        [Test]
        public void LevelStartedEvent_GetMetadata_WithAllValues()
        {
            var evt = new LevelStartedEvent(TEST_USER_ID, 5, "Boss Level", "Hard");
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(3, metadata.Count);
            Assert.AreEqual(5, metadata["levelNumber"]);
            Assert.AreEqual("Boss Level", metadata["levelName"]);
            Assert.AreEqual("Hard", metadata["difficulty"]);
        }

        [Test]
        public void LevelStartedEvent_GetMetadata_WithOptionalNull()
        {
            var evt = new LevelStartedEvent(TEST_USER_ID, 5, null, null);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(1, metadata.Count);
            Assert.AreEqual(5, metadata["levelNumber"]);
        }

        #endregion

        #region ScoreUpdatedEvent Tests

        [Test]
        public void ScoreUpdatedEvent_ConstructsCorrectly()
        {
            var evt = new ScoreUpdatedEvent(TEST_USER_ID, 1, 1500, 1000, 500, 2);
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("ScoreUpdated", evt.EventType);
            Assert.AreEqual(1, evt.LevelNumber);
            Assert.AreEqual(1500, evt.CurrentScore);
            Assert.AreEqual(1000, evt.OldScore);
            Assert.AreEqual(500, evt.ScoreDelta);
            Assert.AreEqual(2, evt.ComboMultiplier);
        }

        [Test]
        public void ScoreUpdatedEvent_GetMetadata_WithAllValues()
        {
            var evt = new ScoreUpdatedEvent(TEST_USER_ID, 1, 1500, 1000, 500, 2);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(5, metadata.Count);
            Assert.AreEqual(1, metadata["levelNumber"]);
            Assert.AreEqual(1500, metadata["currentScore"]);
            Assert.AreEqual(1000, metadata["oldScore"]);
            Assert.AreEqual(500, metadata["scoreDelta"]);
            Assert.AreEqual(2, metadata["comboMultiplier"]);
        }

        [Test]
        public void ScoreUpdatedEvent_DefaultComboMultiplier()
        {
            var evt = new ScoreUpdatedEvent(TEST_USER_ID, 1, 1500);
            
            Assert.AreEqual(1, evt.ComboMultiplier);
        }

        #endregion

        #region PowerUpCollectedEvent Tests

        [Test]
        public void PowerUpCollectedEvent_ConstructsCorrectly()
        {
            var evt = new PowerUpCollectedEvent(TEST_USER_ID, 1, "SpeedBoost", 2, 30.5);
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("PowerUpCollected", evt.EventType);
            Assert.AreEqual(1, evt.LevelNumber);
            Assert.AreEqual("SpeedBoost", evt.PowerUpType);
            Assert.AreEqual(2, evt.Quantity);
            Assert.AreEqual(30.5, evt.Duration);
        }

        [Test]
        public void PowerUpCollectedEvent_ThrowsOnNullPowerUpType()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new PowerUpCollectedEvent(TEST_USER_ID, 1, null!, 1));
        }

        #endregion

        #region CurrencyBalanceUpdatedEvent Tests

        [Test]
        public void CurrencyBalanceUpdatedEvent_ConstructsCorrectly()
        {
            var evt = new CurrencyBalanceUpdatedEvent(
                TEST_USER_ID, 
                CurrencyType.Hard, 
                100, 
                150, 
                50, 
                CurrencyUpdateReason.Reward
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("CurrencyBalanceUpdated", evt.EventType);
            Assert.AreEqual(CurrencyType.Hard, evt.CurrencyType);
            Assert.AreEqual(100, evt.OldBalance);
            Assert.AreEqual(150, evt.CurrentBalance);
            Assert.AreEqual(50, evt.Delta);
            Assert.AreEqual(CurrencyUpdateReason.Reward, evt.UpdateReason);
        }

        [Test]
        public void CurrencyBalanceUpdatedEvent_GetMetadata_EnumsAsStrings()
        {
            var evt = new CurrencyBalanceUpdatedEvent(
                TEST_USER_ID, 
                CurrencyType.Premium, 
                null, 
                100, 
                null, 
                CurrencyUpdateReason.Purchase
            );
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual("Premium", metadata["currencyType"]);
            Assert.AreEqual("Purchase", metadata["updateReason"]);
        }

        #endregion

        #region BoosterOfferedEvent Tests

        [Test]
        public void BoosterOfferedEvent_ConstructsCorrectly()
        {
            var evt = new BoosterOfferedEvent(
                TEST_USER_ID,
                1,
                BoosterType.DoublePoints,
                OfferMethod.WatchAd,
                0
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("BoosterOffered", evt.EventType);
            Assert.AreEqual(1, evt.LevelNumber);
            Assert.AreEqual(BoosterType.DoublePoints, evt.BoosterType);
            Assert.AreEqual(OfferMethod.WatchAd, evt.OfferMethod);
            Assert.AreEqual(0, evt.RequiredCurrency);
        }

        #endregion

        #region BoosterAcceptedEvent Tests

        [Test]
        public void BoosterAcceptedEvent_ConstructsCorrectly()
        {
            var evt = new BoosterAcceptedEvent(
                TEST_USER_ID,
                1,
                BoosterType.Shield,
                AcceptMethod.SpendCurrency,
                100
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("BoosterAccepted", evt.EventType);
            Assert.AreEqual(1, evt.LevelNumber);
            Assert.AreEqual(BoosterType.Shield, evt.BoosterType);
            Assert.AreEqual(AcceptMethod.SpendCurrency, evt.AcceptMethod);
            Assert.AreEqual(100, evt.SpentAmount);
        }

        #endregion

        #region BoosterDeclinedEvent Tests

        [Test]
        public void BoosterDeclinedEvent_ConstructsCorrectly()
        {
            var evt = new BoosterDeclinedEvent(
                TEST_USER_ID,
                1,
                BoosterType.ExtraLife,
                DeclineReason.TooExpensive
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("BoosterDeclined", evt.EventType);
            Assert.AreEqual(1, evt.LevelNumber);
            Assert.AreEqual(BoosterType.ExtraLife, evt.BoosterType);
            Assert.AreEqual(DeclineReason.TooExpensive, evt.DeclineReason);
        }

        #endregion

        #region LevelFailedEvent Tests

        [Test]
        public void LevelFailedEvent_ConstructsCorrectly()
        {
            var evt = new LevelFailedEvent(TEST_USER_ID, 3, FailureReason.NotEnoughMoves);
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("LevelFailed", evt.EventType);
            Assert.AreEqual(3, evt.LevelNumber);
            Assert.AreEqual(FailureReason.NotEnoughMoves, evt.FailureReason);
        }

        [Test]
        public void LevelFailedEvent_GetMetadata_WithEnumAsString()
        {
            var evt = new LevelFailedEvent(TEST_USER_ID, 3, FailureReason.TimeOut);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual("TimeOut", metadata["failureReason"]);
        }

        #endregion

        #region SessionStartedEvent Tests

        [Test]
        public void SessionStartedEvent_ConstructsCorrectly()
        {
            var evt = new SessionStartedEvent(
                TEST_USER_ID,
                "1.0.0",
                PlatformType.iOS,
                "iPhone 12"
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("SessionStarted", evt.EventType);
            Assert.AreEqual("1.0.0", evt.GameVersion);
            Assert.AreEqual(PlatformType.iOS, evt.Platform);
            Assert.AreEqual("iPhone 12", evt.DeviceModel);
        }

        [Test]
        public void SessionStartedEvent_ThrowsOnNullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new SessionStartedEvent(TEST_USER_ID, null!, PlatformType.Android, "Device"));
            
            Assert.Throws<ArgumentNullException>(() => 
                new SessionStartedEvent(TEST_USER_ID, "1.0.0", PlatformType.Android, null!));
        }

        #endregion

        #region UserLocationLatLngEvent Tests

        [Test]
        public void UserLocationLatLngEvent_ConstructsCorrectly()
        {
            var evt = new UserLocationLatLngEvent(TEST_USER_ID, 37.7749, -122.4194);
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("UserLocationLatLng", evt.EventType);
            Assert.AreEqual(37.7749, evt.Latitude);
            Assert.AreEqual(-122.4194, evt.Longitude);
        }

        [Test]
        public void UserLocationLatLngEvent_GetMetadata_HasCoordinates()
        {
            var evt = new UserLocationLatLngEvent(TEST_USER_ID, 37.7749, -122.4194);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(2, metadata.Count);
            Assert.AreEqual(37.7749, metadata["latitude"]);
            Assert.AreEqual(-122.4194, metadata["longitude"]);
        }

        #endregion

        #region UserTouchEvent Tests

        [Test]
        public void UserTouchEvent_ConstructsCorrectly()
        {
            var evt = new UserTouchEvent(
                TEST_USER_ID,
                0,
                "Began",
                0.5f,
                0.75f,
                540f,
                810f,
                123.45f
            );
            
            Assert.AreEqual(TEST_USER_ID, evt.UserId);
            Assert.AreEqual("UserTouch", evt.EventType);
            Assert.AreEqual(0, evt.FingerId);
            Assert.AreEqual("Began", evt.Phase);
            Assert.AreEqual(0.5f, evt.NormalizedX);
            Assert.AreEqual(0.75f, evt.NormalizedY);
            Assert.AreEqual(540f, evt.RawX);
            Assert.AreEqual(810f, evt.RawY);
            Assert.AreEqual(123.45f, evt.Timestamp);
        }

        [Test]
        public void UserTouchEvent_GetMetadata_AllFields()
        {
            var evt = new UserTouchEvent(TEST_USER_ID, 1, "Moved", 0.25f, 0.5f, 270f, 540f, 456.78f);
            var metadata = evt.GetMetadata();
            
            Assert.AreEqual(7, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("fingerId"));
            Assert.IsTrue(metadata.ContainsKey("phase"));
            Assert.IsTrue(metadata.ContainsKey("normalizedX"));
            Assert.IsTrue(metadata.ContainsKey("normalizedY"));
            Assert.IsTrue(metadata.ContainsKey("rawX"));
            Assert.IsTrue(metadata.ContainsKey("rawY"));
            Assert.IsTrue(metadata.ContainsKey("timestamp"));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void AllEvents_RequireNonNullUserId()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStartedEvent(null!));
            Assert.Throws<ArgumentNullException>(() => new LevelStartedEvent(null!, 1));
            Assert.Throws<ArgumentNullException>(() => new ScoreUpdatedEvent(null!, 1, 100));
            Assert.Throws<ArgumentNullException>(() => new PowerUpCollectedEvent(null!, 1, "type", 1));
            Assert.Throws<ArgumentNullException>(() => new LevelSuccessEvent(null!, 1));
            Assert.Throws<ArgumentNullException>(() => new LevelFailedEvent(null!, 1, FailureReason.TimeOut));
            Assert.Throws<ArgumentNullException>(() => new GameEndedEvent(null!, 100));
            Assert.Throws<ArgumentNullException>(() => new GameOverEvent(null!, 1, 100, FailureReason.NoLives));
            Assert.Throws<ArgumentNullException>(() => new MetricRecordedEvent(null!, 1, MetricType.Score, 100));
            Assert.Throws<ArgumentNullException>(() => new SessionStartedEvent(null!, "1.0", PlatformType.iOS, "device"));
            Assert.Throws<ArgumentNullException>(() => new SessionEndedEvent(null!, "reason"));
            Assert.Throws<ArgumentNullException>(() => new UserLocationTextEvent(null!, "location"));
            Assert.Throws<ArgumentNullException>(() => new UserLocationLatLngEvent(null!, 0, 0));
            Assert.Throws<ArgumentNullException>(() => new UserLocationIPEvent(null!, "127.0.0.1"));
            Assert.Throws<ArgumentNullException>(() => new UserTouchEvent(null!, 0, "phase", 0, 0, 0, 0, 0));
        }

        #endregion
    }
}