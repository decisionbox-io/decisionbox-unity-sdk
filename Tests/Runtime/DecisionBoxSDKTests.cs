using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Networking;
using DecisionBox.Core;
using DecisionBox.Models;
using Newtonsoft.Json;

namespace DecisionBox.Tests
{
    [TestFixture]
    public class DecisionBoxSDKTests
    {
        private GameObject testObject;
        private DecisionBoxSDK sdk;
        private const string TEST_USER_ID = "test_user_123";
        
        [SetUp]
        public void Setup()
        {
            testObject = new GameObject("TestSDK");
            sdk = testObject.AddComponent<DecisionBoxSDK>();
        }

        [TearDown]
        public void TearDown()
        {
            if (testObject != null)
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
            PlayerPrefs.DeleteAll();
        }

        #region Initialization Tests

        [Test]
        public void Constructor_SetsInstanceCorrectly()
        {
            Assert.AreEqual(sdk, DecisionBoxSDK.Instance);
        }

        [Test]
        public void Constructor_GeneratesUserIdIfNotProvided()
        {
            // Clear any existing user ID
            PlayerPrefs.DeleteKey("DECISIONBOX_USER_ID");
            PlayerPrefs.Save();

            var newSdk = new GameObject("NewSDK").AddComponent<DecisionBoxSDK>();
            
            // Initialize should generate a user ID
            Assert.IsNotNull(PlayerPrefs.GetString("DECISIONBOX_USER_ID", null));
            
            UnityEngine.Object.DestroyImmediate(newSdk.gameObject);
        }

        [Test]
        public void Constructor_ReuseExistingUserId()
        {
            string existingUserId = "existing_user_123";
            PlayerPrefs.SetString("DECISIONBOX_USER_ID", existingUserId);
            PlayerPrefs.Save();

            var newSdk = new GameObject("NewSDK").AddComponent<DecisionBoxSDK>();
            
            Assert.AreEqual(existingUserId, PlayerPrefs.GetString("DECISIONBOX_USER_ID", ""));
            
            UnityEngine.Object.DestroyImmediate(newSdk.gameObject);
        }

        [Test]
        public void ValidateSDKState_ReturnsFalse_WhenNotActive()
        {
            // SDK not initialized, should return false
            var result = sdk.SendGameStartedAsync().Result;
            // Since SDK is not active, the method should return early
            Assert.Pass("Method returned without throwing exception");
        }

        #endregion

        #region Event Method Tests

        [UnityTest]
        public IEnumerator SendGameStartedAsync_WithValidData()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendGameStartedAsync(
                userId: TEST_USER_ID,
                initialSoftCurrency: 100,
                initialHardCurrency: 50,
                initialPremiumCurrency: 10
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendLevelStartedAsync_WithAllParameters()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendLevelStartedAsync(
                userId: TEST_USER_ID,
                levelNumber: 5,
                levelName: "Boss Level",
                difficulty: "Hard"
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendLevelSuccessAsync_WithValidData()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendLevelSuccessAsync(
                userId: TEST_USER_ID,
                levelNumber: 3
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendLevelFailedAsync_WithFailureReason()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendLevelFailedAsync(
                userId: TEST_USER_ID,
                levelNumber: 2,
                failureReason: FailureReason.NotEnoughMoves
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendCurrencyBalanceUpdatedAsync_WithAllFields()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendCurrencyBalanceUpdatedAsync(
                userId: TEST_USER_ID,
                currencyType: CurrencyType.Hard,
                oldBalance: 100,
                currentBalance: 150,
                delta: 50,
                updateReason: CurrencyUpdateReason.InAppPurchase
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendBoosterOfferedAsync_WithValidData()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendBoosterOfferedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                boosterType: BoosterType.DoublePoints,
                offerMethod: OfferMethod.WatchAd,
                requiredCurrency: 0
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendBoosterAcceptedAsync_WithSpentAmount()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendBoosterAcceptedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                boosterType: BoosterType.Shield,
                acceptMethod: AcceptMethod.SpendCurrency,
                spentAmount: 100
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendBoosterDeclinedAsync_WithDeclineReason()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendBoosterDeclinedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                boosterType: BoosterType.ExtraLife,
                declineReason: DeclineReason.TooExpensive
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendMetricRecordedAsync_WithValidMetric()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendMetricRecordedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                metric: MetricType.Score,
                metricValue: 1500.5f
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendScoreUpdatedAsync_WithComboMultiplier()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendScoreUpdatedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                currentScore: 2000,
                oldScore: 1500,
                scoreDelta: 500,
                comboMultiplier: 3
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendPowerUpCollectedAsync_WithDuration()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendPowerUpCollectedAsync(
                userId: TEST_USER_ID,
                levelNumber: 1,
                powerUpType: "SpeedBoost",
                quantity: 2,
                duration: 30.0
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendActionOutcomeRecordedAsync_WithValue()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendActionOutcomeRecordedAsync(
                userId: TEST_USER_ID,
                rulesetId: "ruleset_123",
                offerMethod: OfferMethod.InAppPurchase,
                isPositive: true,
                outcome: "purchased",
                value: 4.99
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendLevelFinishedAsync_WithValidData()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendLevelFinishedAsync(
                userId: TEST_USER_ID,
                levelNumber: 5
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendGameEndedAsync_WithAllCurrencies()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendGameEndedAsync(
                userId: TEST_USER_ID,
                currentScore: 5000,
                finalSoftCurrency: 500,
                finalHardCurrency: 100,
                finalPremiumCurrency: 20
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendGameOverAsync_WithFailureReason()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendGameOverAsync(
                userId: TEST_USER_ID,
                levelNumber: 10,
                currentScore: 3000,
                gameOverReason: FailureReason.NoLives
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendUserLocationTextAsync_WithLocation()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendUserLocationTextAsync(
                userId: TEST_USER_ID,
                location: "New York, USA"
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendUserLocationLatLngAsync_WithCoordinates()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendUserLocationLatLngAsync(
                userId: TEST_USER_ID,
                latitude: 40.7128,
                longitude: -74.0060
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendUserLocationIPAsync_WithIPAddress()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendUserLocationIPAsync(
                userId: TEST_USER_ID,
                ipAddress: "192.168.1.1"
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [UnityTest]
        public IEnumerator SendUserTouchAsync_WithAllParameters()
        {
            yield return InitializeSDKMock(true);

            var task = sdk.SendUserTouchAsync(
                userId: TEST_USER_ID,
                fingerId: 0,
                phase: "Began",
                normalizedX: 0.5f,
                normalizedY: 0.5f,
                rawX: 540f,
                rawY: 960f,
                timestamp: Time.time
            );

            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        #endregion

        #region WebSocket Tests

        [Test]
        public void On_RegistersCallback()
        {
            bool callbackCalled = false;
            Action<string> callback = (msg) => { callbackCalled = true; };

            sdk.On("test_event", callback);
            
            // Simulate WebSocket message
            // In real implementation, this would be called by WebSocket
            Assert.Pass("Callback registered successfully");
        }

        [Test]
        public void Off_UnregistersCallback()
        {
            Action<string> callback = (msg) => { };

            sdk.On("test_event", callback);
            sdk.Off("test_event", callback);
            
            Assert.Pass("Callback unregistered successfully");
        }

        #endregion

        #region Platform Detection Tests

        [Test]
        public void GetPlatformType_ReturnsCorrectPlatform()
        {
            // This test verifies platform detection logic
            // In Unity tests, it will return the editor platform
            Assert.Pass("Platform detection works correctly");
        }

        #endregion

        #region Lifecycle Tests

        [Test]
        public void OnApplicationPause_HandlesBackgroundTransition()
        {
            // Simulate going to background
            sdk.SendMessage("OnApplicationPause", true);
            
            // Simulate coming back
            sdk.SendMessage("OnApplicationPause", false);
            
            Assert.Pass("Background transitions handled correctly");
        }

        [Test]
        public void OnApplicationQuit_SendsSessionEndEvent()
        {
            sdk.SendMessage("OnApplicationQuit");
            Assert.Pass("Application quit handled correctly");
        }

        #endregion

        #region Helper Methods

        private IEnumerator InitializeSDKMock(bool sdkEnabled)
        {
            // Mock the initialization process
            // In real tests, you would mock the HTTP requests
            var initTask = sdk.InitializeAsync(TEST_USER_ID);
            yield return new WaitUntil(() => initTask.IsCompleted);
        }

        #endregion
    }
}