using System;
using NUnit.Framework;
using Newtonsoft.Json;
using DecisionBox.Models;

namespace DecisionBox.Tests
{
    [TestFixture]
    public class RemoteConfigTests
    {
        #region RemoteConfig Tests

        [Test]
        public void RemoteConfig_HasDefaultValues()
        {
            var config = new RemoteConfig();
            
            Assert.AreEqual(false, config.sdkEnabled);
            Assert.AreEqual(false, config.websocketEnabled);
            Assert.IsNull(config.websocketUrl);
            Assert.AreEqual(1800000, config.maxSessionDuration); // 30 minutes
            Assert.AreEqual(300000, config.maxBackgroundDuration); // 5 minutes
            Assert.AreEqual(10, config.eventBatchSize);
            Assert.AreEqual(30000, config.eventFlushInterval); // 30 seconds
            Assert.AreEqual(false, config.enableLogging);
            Assert.AreEqual(30000, config.apiTimeout); // 30 seconds
            Assert.AreEqual(3, config.retryAttempts);
            Assert.AreEqual(1000, config.retryDelayMs); // 1 second
        }

        [Test]
        public void RemoteConfig_DeserializesCorrectly()
        {
            var json = @"{
                ""sdkEnabled"": true,
                ""websocketEnabled"": true,
                ""websocketUrl"": ""wss://ws.decisionbox.io"",
                ""maxSessionDuration"": 3600000,
                ""maxBackgroundDuration"": 600000,
                ""eventBatchSize"": 20,
                ""eventFlushInterval"": 60000,
                ""enableLogging"": true,
                ""apiTimeout"": 60000,
                ""retryAttempts"": 5,
                ""retryDelayMs"": 2000
            }";

            var config = JsonConvert.DeserializeObject<RemoteConfig>(json);
            
            Assert.IsNotNull(config);
            Assert.AreEqual(true, config!.sdkEnabled);
            Assert.AreEqual(true, config.websocketEnabled);
            Assert.AreEqual("wss://ws.decisionbox.io", config.websocketUrl);
            Assert.AreEqual(3600000, config.maxSessionDuration);
            Assert.AreEqual(600000, config.maxBackgroundDuration);
            Assert.AreEqual(20, config.eventBatchSize);
            Assert.AreEqual(60000, config.eventFlushInterval);
            Assert.AreEqual(true, config.enableLogging);
            Assert.AreEqual(60000, config.apiTimeout);
            Assert.AreEqual(5, config.retryAttempts);
            Assert.AreEqual(2000, config.retryDelayMs);
        }

        [Test]
        public void RemoteConfig_HandlesPartialJson()
        {
            var json = @"{
                ""sdkEnabled"": true,
                ""websocketUrl"": ""wss://ws.test.com""
            }";

            var config = JsonConvert.DeserializeObject<RemoteConfig>(json);
            
            Assert.IsNotNull(config);
            Assert.AreEqual(true, config!.sdkEnabled);
            Assert.AreEqual("wss://ws.test.com", config.websocketUrl);
            
            // Check defaults are preserved
            Assert.AreEqual(1800000, config.maxSessionDuration);
            Assert.AreEqual(10, config.eventBatchSize);
        }

        #endregion

        #region AuthResponse Tests

        [Test]
        public void AuthResponse_HasDefaultValues()
        {
            var response = new AuthResponse();
            
            Assert.AreEqual("", response.access_token);
            Assert.AreEqual("", response.token_type);
            Assert.AreEqual(0, response.expires_in);
            Assert.IsNull(response.scope);
        }

        [Test]
        public void AuthResponse_DeserializesCorrectly()
        {
            var json = @"{
                ""access_token"": ""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"",
                ""token_type"": ""Bearer"",
                ""expires_in"": 3600,
                ""scope"": ""read write""
            }";

            var response = JsonConvert.DeserializeObject<AuthResponse>(json);
            
            Assert.IsNotNull(response);
            Assert.AreEqual("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", response!.access_token);
            Assert.AreEqual("Bearer", response.token_type);
            Assert.AreEqual(3600, response.expires_in);
            Assert.AreEqual("read write", response.scope);
        }

        #endregion

        #region WebSocketMessage Tests

        [Test]
        public void WebSocketMessage_HasDefaultValues()
        {
            var message = new WebSocketMessage();
            
            Assert.IsNull(message.type);
            Assert.IsNull(message.data);
            Assert.AreEqual(0, message.timestamp);
            Assert.IsNull(message.session_id);
        }

        [Test]
        public void WebSocketMessage_DeserializesCorrectly()
        {
            var json = @"{
                ""type"": ""offer_available"",
                ""data"": { ""offer_id"": ""123"", ""value"": 9.99 },
                ""timestamp"": 1640000000000,
                ""session_id"": ""session_123""
            }";

            var message = JsonConvert.DeserializeObject<WebSocketMessage>(json);
            
            Assert.IsNotNull(message);
            Assert.AreEqual("offer_available", message!.type);
            Assert.IsNotNull(message.data);
            Assert.AreEqual(1640000000000, message.timestamp);
            Assert.AreEqual("session_123", message.session_id);
        }

        [Test]
        public void WebSocketMessage_HandlesComplexData()
        {
            var json = @"{
                ""type"": ""game_event"",
                ""data"": {
                    ""event_type"": ""level_complete"",
                    ""level"": 5,
                    ""score"": 12500,
                    ""rewards"": [
                        { ""type"": ""currency"", ""amount"": 100 },
                        { ""type"": ""item"", ""id"": ""sword_01"" }
                    ]
                },
                ""timestamp"": 1640000000000
            }";

            var message = JsonConvert.DeserializeObject<WebSocketMessage>(json);
            
            Assert.IsNotNull(message);
            Assert.AreEqual("game_event", message!.type);
            Assert.IsNotNull(message.data);
            
            // Verify the data object can be accessed
            var dataJson = JsonConvert.SerializeObject(message.data);
            Assert.IsTrue(dataJson.Contains("level_complete"));
            Assert.IsTrue(dataJson.Contains("rewards"));
        }

        #endregion

        #region ApiErrorResponse Tests

        [Test]
        public void ApiErrorResponse_HasDefaultValues()
        {
            var error = new ApiErrorResponse();
            
            Assert.IsNull(error.error);
            Assert.IsNull(error.error_description);
            Assert.AreEqual(0, error.code);
            Assert.IsNull(error.message);
        }

        [Test]
        public void ApiErrorResponse_DeserializesCorrectly()
        {
            var json = @"{
                ""error"": ""invalid_token"",
                ""error_description"": ""The access token is expired"",
                ""code"": 401,
                ""message"": ""Authentication failed""
            }";

            var error = JsonConvert.DeserializeObject<ApiErrorResponse>(json);
            
            Assert.IsNotNull(error);
            Assert.AreEqual("invalid_token", error!.error);
            Assert.AreEqual("The access token is expired", error.error_description);
            Assert.AreEqual(401, error.code);
            Assert.AreEqual("Authentication failed", error.message);
        }

        [Test]
        public void ApiErrorResponse_HandlesPartialError()
        {
            var json = @"{
                ""error"": ""server_error"",
                ""code"": 500
            }";

            var error = JsonConvert.DeserializeObject<ApiErrorResponse>(json);
            
            Assert.IsNotNull(error);
            Assert.AreEqual("server_error", error!.error);
            Assert.AreEqual(500, error.code);
            Assert.IsNull(error.error_description);
            Assert.IsNull(error.message);
        }

        #endregion

        #region Serialization Tests

        [Test]
        public void RemoteConfig_SerializesCorrectly()
        {
            var config = new RemoteConfig
            {
                sdkEnabled = true,
                websocketEnabled = true,
                websocketUrl = "wss://test.com",
                maxSessionDuration = 7200000,
                enableLogging = true
            };

            var json = JsonConvert.SerializeObject(config);
            var deserialized = JsonConvert.DeserializeObject<RemoteConfig>(json);
            
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(config.sdkEnabled, deserialized!.sdkEnabled);
            Assert.AreEqual(config.websocketEnabled, deserialized.websocketEnabled);
            Assert.AreEqual(config.websocketUrl, deserialized.websocketUrl);
            Assert.AreEqual(config.maxSessionDuration, deserialized.maxSessionDuration);
            Assert.AreEqual(config.enableLogging, deserialized.enableLogging);
        }

        [Test]
        public void AuthResponse_SerializesCorrectly()
        {
            var response = new AuthResponse
            {
                access_token = "test_token",
                token_type = "Bearer",
                expires_in = 7200,
                scope = "full_access"
            };

            var json = JsonConvert.SerializeObject(response);
            var deserialized = JsonConvert.DeserializeObject<AuthResponse>(json);
            
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(response.access_token, deserialized!.access_token);
            Assert.AreEqual(response.token_type, deserialized.token_type);
            Assert.AreEqual(response.expires_in, deserialized.expires_in);
            Assert.AreEqual(response.scope, deserialized.scope);
        }

        #endregion
    }
}