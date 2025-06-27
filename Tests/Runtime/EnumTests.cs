using System;
using System.Linq;
using NUnit.Framework;
using DecisionBox.Models;

namespace DecisionBox.Tests
{
    [TestFixture]
    public class EnumTests
    {
        #region CurrencyType Tests

        [Test]
        public void CurrencyType_HasCorrectValues()
        {
            var values = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>().ToList();
            
            Assert.Contains(CurrencyType.Soft, values);
            Assert.Contains(CurrencyType.Hard, values);
            Assert.Contains(CurrencyType.Premium, values);
            Assert.AreEqual(3, values.Count);
        }

        [Test]
        public void CurrencyType_ToStringReturnsCorrectName()
        {
            Assert.AreEqual("Soft", CurrencyType.Soft.ToString());
            Assert.AreEqual("Hard", CurrencyType.Hard.ToString());
            Assert.AreEqual("Premium", CurrencyType.Premium.ToString());
        }

        #endregion

        #region CurrencyUpdateReason Tests

        [Test]
        public void CurrencyUpdateReason_HasAllExpectedValues()
        {
            var values = Enum.GetValues(typeof(CurrencyUpdateReason)).Cast<CurrencyUpdateReason>().ToList();
            
            Assert.Contains(CurrencyUpdateReason.NotSpecified, values);
            Assert.Contains(CurrencyUpdateReason.Purchase, values);
            Assert.Contains(CurrencyUpdateReason.Reward, values);
            Assert.Contains(CurrencyUpdateReason.Spending, values);
            Assert.Contains(CurrencyUpdateReason.Refund, values);
            Assert.Contains(CurrencyUpdateReason.Bonus, values);
            Assert.Contains(CurrencyUpdateReason.Achievement, values);
            Assert.Contains(CurrencyUpdateReason.DailyLogin, values);
            Assert.Contains(CurrencyUpdateReason.AdReward, values);
            Assert.Contains(CurrencyUpdateReason.CurrencyExchange, values);
            Assert.Contains(CurrencyUpdateReason.SystemAdjustment, values);
            Assert.AreEqual(11, values.Count);
        }

        #endregion

        #region BoosterType Tests

        [Test]
        public void BoosterType_HasCommonBoosters()
        {
            var values = Enum.GetValues(typeof(BoosterType)).Cast<BoosterType>().ToList();
            
            // Test a subset of important boosters
            Assert.Contains(BoosterType.SpeedBoost, values);
            Assert.Contains(BoosterType.Shield, values);
            Assert.Contains(BoosterType.DoublePoints, values);
            Assert.Contains(BoosterType.ExtraLife, values);
            Assert.Contains(BoosterType.Invincibility, values);
            Assert.Contains(BoosterType.Magnet, values);
            Assert.Contains(BoosterType.ComboMultiplier, values);
            
            // Verify we have many booster types
            Assert.Greater(values.Count, 40, "Should have many booster types");
        }

        #endregion

        #region OfferMethod Tests

        [Test]
        public void OfferMethod_HasCommonMethods()
        {
            var values = Enum.GetValues(typeof(OfferMethod)).Cast<OfferMethod>().ToList();
            
            Assert.Contains(OfferMethod.WatchAd, values);
            Assert.Contains(OfferMethod.SpendCurrency, values);
            Assert.Contains(OfferMethod.InAppPurchase, values);
            Assert.Contains(OfferMethod.CompleteChallenge, values);
            Assert.Contains(OfferMethod.DailyBonus, values);
            Assert.Contains(OfferMethod.RewardedVideo, values);
            Assert.Contains(OfferMethod.SocialShare, values);
            
            // Verify we have many offer methods
            Assert.Greater(values.Count, 20, "Should have many offer methods");
        }

        #endregion

        #region AcceptMethod Tests

        [Test]
        public void AcceptMethod_HasCommonMethods()
        {
            var values = Enum.GetValues(typeof(AcceptMethod)).Cast<AcceptMethod>().ToList();
            
            Assert.Contains(AcceptMethod.WatchAd, values);
            Assert.Contains(AcceptMethod.SpendCurrency, values);
            Assert.Contains(AcceptMethod.InAppPurchase, values);
            Assert.Contains(AcceptMethod.Reward, values);
            Assert.Contains(AcceptMethod.SocialShare, values);
            Assert.Contains(AcceptMethod.DirectAccept, values);
            
            // Verify we have many accept methods
            Assert.Greater(values.Count, 15, "Should have many accept methods");
        }

        #endregion

        #region DeclineReason Tests

        [Test]
        public void DeclineReason_HasCommonReasons()
        {
            var values = Enum.GetValues(typeof(DeclineReason)).Cast<DeclineReason>().ToList();
            
            Assert.Contains(DeclineReason.NotInterested, values);
            Assert.Contains(DeclineReason.InsufficientFunds, values);
            Assert.Contains(DeclineReason.AlreadyActive, values);
            Assert.Contains(DeclineReason.AdUnavailable, values);
            Assert.Contains(DeclineReason.AdSkipped, values);
            Assert.Contains(DeclineReason.CancelledByUser, values);
            Assert.Contains(DeclineReason.TimeExpired, values);
            Assert.Contains(DeclineReason.TechnicalIssue, values);
            Assert.Contains(DeclineReason.PreferToSaveCurrency, values);
            Assert.Contains(DeclineReason.TooExpensive, values);
            Assert.Contains(DeclineReason.AlreadyOwned, values);
            Assert.Contains(DeclineReason.LackOfUrgency, values);
            Assert.AreEqual(12, values.Count);
        }

        #endregion

        #region FailureReason Tests

        [Test]
        public void FailureReason_HasCommonReasons()
        {
            var values = Enum.GetValues(typeof(FailureReason)).Cast<FailureReason>().ToList();
            
            Assert.Contains(FailureReason.TimeOut, values);
            Assert.Contains(FailureReason.NoLives, values);
            Assert.Contains(FailureReason.CollisionWithObstacle, values);
            Assert.Contains(FailureReason.PuzzleNotSolved, values);
            Assert.Contains(FailureReason.ObjectiveNotCompleted, values);
            Assert.Contains(FailureReason.NotEnoughMoves, values);
            
            // Verify we have many failure reasons
            Assert.Greater(values.Count, 20, "Should have many failure reasons");
        }

        #endregion

        #region MetricType Tests

        [Test]
        public void MetricType_HasCommonMetrics()
        {
            var values = Enum.GetValues(typeof(MetricType)).Cast<MetricType>().ToList();
            
            Assert.Contains(MetricType.Score, values);
            Assert.Contains(MetricType.MovesMade, values);
            Assert.Contains(MetricType.MovesRemaining, values);
            Assert.Contains(MetricType.LivesRemaining, values);
            Assert.Contains(MetricType.LivesLost, values);
            Assert.Contains(MetricType.EnemiesDefeated, values);
            Assert.Contains(MetricType.ItemsCollected, values);
            Assert.Contains(MetricType.PowerUpsUsed, values);
            Assert.Contains(MetricType.TimePlayed, values);
            Assert.Contains(MetricType.LevelsCompleted, values);
            
            // Verify we have many metric types
            Assert.Greater(values.Count, 30, "Should have many metric types");
        }

        #endregion

        #region PlatformType Tests

        [Test]
        public void PlatformType_HasCorrectValues()
        {
            var values = Enum.GetValues(typeof(PlatformType)).Cast<PlatformType>().ToList();
            
            Assert.Contains(PlatformType.Android, values);
            Assert.Contains(PlatformType.iOS, values);
            Assert.Contains(PlatformType.Web, values);
            Assert.AreEqual(3, values.Count, "Should only have Android, iOS, and Web (no NotSet)");
        }

        [Test]
        public void PlatformType_DoesNotHaveNotSet()
        {
            var values = Enum.GetNames(typeof(PlatformType)).ToList();
            
            Assert.IsFalse(values.Contains("NotSet"), "PlatformType should not have NotSet value");
            Assert.IsFalse(values.Contains("None"), "PlatformType should not have None value");
        }

        #endregion

        #region Enum Usage in EventConstants Tests

        [Test]
        public void EventConstants_CurrencyValues_MatchEnum()
        {
            Assert.AreEqual(CurrencyType.Soft, EventConstants.Currency.Soft);
            Assert.AreEqual(CurrencyType.Hard, EventConstants.Currency.Hard);
            Assert.AreEqual(CurrencyType.Premium, EventConstants.Currency.Premium);
        }

        [Test]
        public void EventConstants_FailureReasonValues_MatchEnum()
        {
            Assert.AreEqual(FailureReason.TimeOut, EventConstants.FailureReasons.TimeOut);
            Assert.AreEqual(FailureReason.NoLives, EventConstants.FailureReasons.NoLives);
            Assert.AreEqual(FailureReason.NotEnoughMoves, EventConstants.FailureReasons.NotEnoughMoves);
        }

        [Test]
        public void EventConstants_MetricValues_MatchEnum()
        {
            Assert.AreEqual(MetricType.Score, EventConstants.Metrics.Score);
            Assert.AreEqual(MetricType.LivesRemaining, EventConstants.Metrics.LivesRemaining);
            Assert.AreEqual(MetricType.EnemiesDefeated, EventConstants.Metrics.EnemiesDefeated);
        }

        [Test]
        public void EventConstants_PlatformValues_MatchEnum()
        {
            Assert.AreEqual(PlatformType.Android, EventConstants.Platforms.Android);
            Assert.AreEqual(PlatformType.iOS, EventConstants.Platforms.iOS);
            Assert.AreEqual(PlatformType.Web, EventConstants.Platforms.Web);
        }

        #endregion
    }
}