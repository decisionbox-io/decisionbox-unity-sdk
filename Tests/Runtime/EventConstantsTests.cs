using System;
using System.Reflection;
using NUnit.Framework;
using DecisionBox.Models;

namespace DecisionBox.Tests
{
    [TestFixture]
    public class EventConstantsTests
    {
        #region Event Name Constants Tests

        [Test]
        public void EventConstants_Events_HasAllEventNames()
        {
            Assert.AreEqual("GameStarted", EventConstants.Events.GameStarted);
            Assert.AreEqual("LevelStarted", EventConstants.Events.LevelStarted);
            Assert.AreEqual("ScoreUpdated", EventConstants.Events.ScoreUpdated);
            Assert.AreEqual("PowerUpCollected", EventConstants.Events.PowerUpCollected);
            Assert.AreEqual("CurrencyBalanceUpdated", EventConstants.Events.CurrencyBalanceUpdated);
            Assert.AreEqual("BoosterOffered", EventConstants.Events.BoosterOffered);
            Assert.AreEqual("BoosterAccepted", EventConstants.Events.BoosterAccepted);
            Assert.AreEqual("BoosterDeclined", EventConstants.Events.BoosterDeclined);
            Assert.AreEqual("ActionOutcomeRecorded", EventConstants.Events.ActionOutcomeRecorded);
            Assert.AreEqual("LevelFinished", EventConstants.Events.LevelFinished);
            Assert.AreEqual("LevelSuccess", EventConstants.Events.LevelSuccess);
            Assert.AreEqual("LevelFailed", EventConstants.Events.LevelFailed);
            Assert.AreEqual("GameEnded", EventConstants.Events.GameEnded);
            Assert.AreEqual("GameOver", EventConstants.Events.GameOver);
            Assert.AreEqual("MetricRecorded", EventConstants.Events.MetricRecorded);
            Assert.AreEqual("SessionStarted", EventConstants.Events.SessionStarted);
            Assert.AreEqual("SessionEnded", EventConstants.Events.SessionEnded);
            Assert.AreEqual("UserLocationText", EventConstants.Events.UserLocationText);
            Assert.AreEqual("UserLocationLatLng", EventConstants.Events.UserLocationLatLng);
            Assert.AreEqual("UserLocationIP", EventConstants.Events.UserLocationIP);
            Assert.AreEqual("UserTouch", EventConstants.Events.UserTouch);
        }

        [Test]
        public void EventConstants_Events_AllFieldsAreStrings()
        {
            var type = typeof(EventConstants.Events);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            
            foreach (var field in fields)
            {
                Assert.AreEqual(typeof(string), field.FieldType, $"Field {field.Name} should be string");
                Assert.IsNotNull(field.GetValue(null), $"Field {field.Name} should not be null");
            }
        }

        #endregion

        #region Currency Constants Tests

        [Test]
        public void EventConstants_Currency_HasAllTypes()
        {
            Assert.AreEqual(CurrencyType.Soft, EventConstants.Currency.Soft);
            Assert.AreEqual(CurrencyType.Hard, EventConstants.Currency.Hard);
            Assert.AreEqual(CurrencyType.Premium, EventConstants.Currency.Premium);
        }

        [Test]
        public void EventConstants_CurrencyUpdateReasons_HasAllReasons()
        {
            Assert.AreEqual(CurrencyUpdateReason.NotSpecified, EventConstants.CurrencyUpdateReasons.NotSpecified);
            Assert.AreEqual(CurrencyUpdateReason.Purchase, EventConstants.CurrencyUpdateReasons.Purchase);
            Assert.AreEqual(CurrencyUpdateReason.Reward, EventConstants.CurrencyUpdateReasons.Reward);
            Assert.AreEqual(CurrencyUpdateReason.Spending, EventConstants.CurrencyUpdateReasons.Spending);
            Assert.AreEqual(CurrencyUpdateReason.Refund, EventConstants.CurrencyUpdateReasons.Refund);
            Assert.AreEqual(CurrencyUpdateReason.Bonus, EventConstants.CurrencyUpdateReasons.Bonus);
            Assert.AreEqual(CurrencyUpdateReason.Achievement, EventConstants.CurrencyUpdateReasons.Achievement);
            Assert.AreEqual(CurrencyUpdateReason.DailyLogin, EventConstants.CurrencyUpdateReasons.DailyLogin);
            Assert.AreEqual(CurrencyUpdateReason.AdReward, EventConstants.CurrencyUpdateReasons.AdReward);
            Assert.AreEqual(CurrencyUpdateReason.CurrencyExchange, EventConstants.CurrencyUpdateReasons.CurrencyExchange);
            Assert.AreEqual(CurrencyUpdateReason.SystemAdjustment, EventConstants.CurrencyUpdateReasons.SystemAdjustment);
        }

        #endregion

        #region Booster Constants Tests

        [Test]
        public void EventConstants_Boosters_HasCommonTypes()
        {
            Assert.AreEqual(BoosterType.SpeedBoost, EventConstants.Boosters.SpeedBoost);
            Assert.AreEqual(BoosterType.Shield, EventConstants.Boosters.Shield);
            Assert.AreEqual(BoosterType.DoublePoints, EventConstants.Boosters.DoublePoints);
            Assert.AreEqual(BoosterType.ExtraLife, EventConstants.Boosters.ExtraLife);
            Assert.AreEqual(BoosterType.Invincibility, EventConstants.Boosters.Invincibility);
            Assert.AreEqual(BoosterType.Magnet, EventConstants.Boosters.Magnet);
            Assert.AreEqual(BoosterType.ComboMultiplier, EventConstants.Boosters.ComboMultiplier);
            Assert.AreEqual(BoosterType.DamageBoost, EventConstants.Boosters.DamageBoost);
            Assert.AreEqual(BoosterType.HealthRegen, EventConstants.Boosters.HealthRegen);
            Assert.AreEqual(BoosterType.CriticalHitBoost, EventConstants.Boosters.CriticalHitBoost);
            Assert.AreEqual(BoosterType.ScoreMultiplier, EventConstants.Boosters.ScoreMultiplier);
            Assert.AreEqual(BoosterType.TimeExtension, EventConstants.Boosters.TimeExtension);
            Assert.AreEqual(BoosterType.CoinMultiplier, EventConstants.Boosters.CoinMultiplier);
            Assert.AreEqual(BoosterType.GemFinder, EventConstants.Boosters.GemFinder);
        }

        #endregion

        #region Method Constants Tests

        [Test]
        public void EventConstants_OfferMethods_HasCommonMethods()
        {
            Assert.AreEqual(OfferMethod.WatchAd, EventConstants.OfferMethods.WatchAd);
            Assert.AreEqual(OfferMethod.SpendCurrency, EventConstants.OfferMethods.SpendCurrency);
            Assert.AreEqual(OfferMethod.InAppPurchase, EventConstants.OfferMethods.InAppPurchase);
            Assert.AreEqual(OfferMethod.CompleteChallenge, EventConstants.OfferMethods.CompleteChallenge);
            Assert.AreEqual(OfferMethod.DailyBonus, EventConstants.OfferMethods.DailyBonus);
            Assert.AreEqual(OfferMethod.RewardedVideo, EventConstants.OfferMethods.RewardedVideo);
            Assert.AreEqual(OfferMethod.SocialShare, EventConstants.OfferMethods.SocialShare);
        }

        [Test]
        public void EventConstants_AcceptMethods_HasCommonMethods()
        {
            Assert.AreEqual(AcceptMethod.WatchAd, EventConstants.AcceptMethods.WatchAd);
            Assert.AreEqual(AcceptMethod.SpendCurrency, EventConstants.AcceptMethods.SpendCurrency);
            Assert.AreEqual(AcceptMethod.InAppPurchase, EventConstants.AcceptMethods.InAppPurchase);
            Assert.AreEqual(AcceptMethod.Reward, EventConstants.AcceptMethods.Reward);
            Assert.AreEqual(AcceptMethod.SocialShare, EventConstants.AcceptMethods.SocialShare);
            Assert.AreEqual(AcceptMethod.DirectAccept, EventConstants.AcceptMethods.DirectAccept);
        }

        [Test]
        public void EventConstants_DeclineReasons_HasCommonReasons()
        {
            Assert.AreEqual(DeclineReason.NotInterested, EventConstants.DeclineReasons.NotInterested);
            Assert.AreEqual(DeclineReason.InsufficientFunds, EventConstants.DeclineReasons.InsufficientFunds);
            Assert.AreEqual(DeclineReason.AlreadyActive, EventConstants.DeclineReasons.AlreadyActive);
            Assert.AreEqual(DeclineReason.AdUnavailable, EventConstants.DeclineReasons.AdUnavailable);
            Assert.AreEqual(DeclineReason.AdSkipped, EventConstants.DeclineReasons.AdSkipped);
            Assert.AreEqual(DeclineReason.CancelledByUser, EventConstants.DeclineReasons.CancelledByUser);
            Assert.AreEqual(DeclineReason.TimeExpired, EventConstants.DeclineReasons.TimeExpired);
            Assert.AreEqual(DeclineReason.TechnicalIssue, EventConstants.DeclineReasons.TechnicalIssue);
        }

        #endregion

        #region Failure and Metric Constants Tests

        [Test]
        public void EventConstants_FailureReasons_HasCommonReasons()
        {
            Assert.AreEqual(FailureReason.TimeOut, EventConstants.FailureReasons.TimeOut);
            Assert.AreEqual(FailureReason.NoLives, EventConstants.FailureReasons.NoLives);
            Assert.AreEqual(FailureReason.CollisionWithObstacle, EventConstants.FailureReasons.CollisionWithObstacle);
            Assert.AreEqual(FailureReason.PuzzleNotSolved, EventConstants.FailureReasons.PuzzleNotSolved);
            Assert.AreEqual(FailureReason.ObjectiveNotCompleted, EventConstants.FailureReasons.ObjectiveNotCompleted);
            Assert.AreEqual(FailureReason.EnemyDefeatedPlayer, EventConstants.FailureReasons.EnemyDefeatedPlayer);
            Assert.AreEqual(FailureReason.OverwhelmedByEnemies, EventConstants.FailureReasons.OverwhelmedByEnemies);
            Assert.AreEqual(FailureReason.WrongMove, EventConstants.FailureReasons.WrongMove);
            Assert.AreEqual(FailureReason.NotEnoughMoves, EventConstants.FailureReasons.NotEnoughMoves);
        }

        [Test]
        public void EventConstants_Metrics_HasCommonTypes()
        {
            Assert.AreEqual(MetricType.Score, EventConstants.Metrics.Score);
            Assert.AreEqual(MetricType.MovesMade, EventConstants.Metrics.MovesMade);
            Assert.AreEqual(MetricType.MovesRemaining, EventConstants.Metrics.MovesRemaining);
            Assert.AreEqual(MetricType.LivesRemaining, EventConstants.Metrics.LivesRemaining);
            Assert.AreEqual(MetricType.LivesLost, EventConstants.Metrics.LivesLost);
            Assert.AreEqual(MetricType.EnemiesDefeated, EventConstants.Metrics.EnemiesDefeated);
            Assert.AreEqual(MetricType.ItemsCollected, EventConstants.Metrics.ItemsCollected);
            Assert.AreEqual(MetricType.PowerUpsUsed, EventConstants.Metrics.PowerUpsUsed);
            Assert.AreEqual(MetricType.TimePlayed, EventConstants.Metrics.TimePlayed);
            Assert.AreEqual(MetricType.LevelsCompleted, EventConstants.Metrics.LevelsCompleted);
        }

        #endregion

        #region Platform Constants Tests

        [Test]
        public void EventConstants_Platforms_HasAllPlatforms()
        {
            Assert.AreEqual(PlatformType.Android, EventConstants.Platforms.Android);
            Assert.AreEqual(PlatformType.iOS, EventConstants.Platforms.iOS);
            Assert.AreEqual(PlatformType.Web, EventConstants.Platforms.Web);
        }

        [Test]
        public void EventConstants_Platforms_HasNoPlatformNotSet()
        {
            var type = typeof(EventConstants.Platforms);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            
            foreach (var field in fields)
            {
                var value = field.GetValue(null);
                Assert.IsNotNull(value);
                Assert.AreNotEqual("NotSet", value.ToString());
                Assert.AreNotEqual("None", value.ToString());
            }
        }

        #endregion

        #region Type Safety Tests

        [Test]
        public void EventConstants_AllClassesAreStatic()
        {
            Assert.IsTrue(typeof(EventConstants).IsAbstract && typeof(EventConstants).IsSealed);
            Assert.IsTrue(typeof(EventConstants.Events).IsAbstract && typeof(EventConstants.Events).IsSealed);
            Assert.IsTrue(typeof(EventConstants.Currency).IsAbstract && typeof(EventConstants.Currency).IsSealed);
            Assert.IsTrue(typeof(EventConstants.CurrencyUpdateReasons).IsAbstract && typeof(EventConstants.CurrencyUpdateReasons).IsSealed);
            Assert.IsTrue(typeof(EventConstants.Boosters).IsAbstract && typeof(EventConstants.Boosters).IsSealed);
            Assert.IsTrue(typeof(EventConstants.OfferMethods).IsAbstract && typeof(EventConstants.OfferMethods).IsSealed);
            Assert.IsTrue(typeof(EventConstants.AcceptMethods).IsAbstract && typeof(EventConstants.AcceptMethods).IsSealed);
            Assert.IsTrue(typeof(EventConstants.DeclineReasons).IsAbstract && typeof(EventConstants.DeclineReasons).IsSealed);
            Assert.IsTrue(typeof(EventConstants.FailureReasons).IsAbstract && typeof(EventConstants.FailureReasons).IsSealed);
            Assert.IsTrue(typeof(EventConstants.Metrics).IsAbstract && typeof(EventConstants.Metrics).IsSealed);
            Assert.IsTrue(typeof(EventConstants.Platforms).IsAbstract && typeof(EventConstants.Platforms).IsSealed);
        }

        [Test]
        public void EventConstants_AllFieldsAreReadOnly()
        {
            CheckFieldsAreReadOnly(typeof(EventConstants.Currency));
            CheckFieldsAreReadOnly(typeof(EventConstants.CurrencyUpdateReasons));
            CheckFieldsAreReadOnly(typeof(EventConstants.Boosters));
            CheckFieldsAreReadOnly(typeof(EventConstants.OfferMethods));
            CheckFieldsAreReadOnly(typeof(EventConstants.AcceptMethods));
            CheckFieldsAreReadOnly(typeof(EventConstants.DeclineReasons));
            CheckFieldsAreReadOnly(typeof(EventConstants.FailureReasons));
            CheckFieldsAreReadOnly(typeof(EventConstants.Metrics));
            CheckFieldsAreReadOnly(typeof(EventConstants.Platforms));
        }

        private void CheckFieldsAreReadOnly(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (!field.FieldType.IsEnum) continue;
                
                Assert.IsTrue(field.IsInitOnly || field.IsLiteral, 
                    $"Field {field.Name} in {type.Name} should be readonly");
            }
        }

        #endregion
    }
}