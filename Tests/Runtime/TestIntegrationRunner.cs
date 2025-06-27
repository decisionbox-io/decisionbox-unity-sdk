using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using DecisionBox.Core;
using DecisionBox.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DecisionBox.Tests.Integration
{
    /// <summary>
    /// Manual integration test runner to verify API communication
    /// </summary>
    public class TestIntegrationRunner : MonoBehaviour
    {
        private const string APP_ID = "68077d9a65f9ed2ee2b45666";
        private const string APP_SECRET = "352bf0616107a3dc772ccd1f60574ebaf84e055a8d5fc78bf641d6ba7bc8ff28623e2a2cab32ef37ad35e76cc9f4a4ca";
        
        private void Start()
        {
            StartCoroutine(RunIntegrationTests());
        }
        
        private IEnumerator RunIntegrationTests()
        {
            Debug.Log("=== DecisionBox Unity SDK Integration Test ===");
            
            // Test 1: Authentication
            Debug.Log("\n1. Testing Authentication...");
            yield return TestAuthentication();
            
            // Test 2: Config Fetch
            Debug.Log("\n2. Testing Config Fetch...");
            yield return TestConfigFetch();
            
            // Test 3: Send Event
            Debug.Log("\n3. Testing Event Sending...");
            yield return TestEventSending();
            
            Debug.Log("\n=== Integration Tests Complete ===");
        }
        
        private IEnumerator TestAuthentication()
        {
            var authRequest = new
            {
                app_id = APP_ID,
                client_secret = APP_SECRET
            };
            
            string json = JsonConvert.SerializeObject(authRequest);
            byte[] bodyData = Encoding.UTF8.GetBytes(json);
            
            using (var request = new UnityWebRequest("https://eventapi.dev.decisionbox.io/oauth/token", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✓ Authentication successful");
                    Debug.Log($"Response: {request.downloadHandler.text}");
                    
                    // Store token for next tests
                    var response = JsonConvert.DeserializeObject<AuthResponse>(request.downloadHandler.text);
                    PlayerPrefs.SetString("test_token", response.access_token);
                }
                else
                {
                    Debug.LogError($"✗ Authentication failed: {request.error}");
                    Debug.LogError($"Response Code: {request.responseCode}");
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.LogError($"Response: {request.downloadHandler.text}");
                    }
                }
            }
        }
        
        private IEnumerator TestConfigFetch()
        {
            string token = PlayerPrefs.GetString("test_token", "");
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("No token available for config test");
                yield break;
            }
            
            using (var request = new UnityWebRequest($"https://eventapi.dev.decisionbox.io/apps/config?appid={APP_ID}", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(new byte[0]);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✓ Config fetch successful");
                    Debug.Log($"Response: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"✗ Config fetch failed: {request.error}");
                    Debug.LogError($"Response Code: {request.responseCode}");
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.LogError($"Response: {request.downloadHandler.text}");
                    }
                }
            }
        }
        
        private IEnumerator TestEventSending()
        {
            string token = PlayerPrefs.GetString("test_token", "");
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("No token available for event test");
                yield break;
            }
            
            var eventData = new
            {
                user_id = "test_user_" + Guid.NewGuid().ToString(),
                session_id = Guid.NewGuid().ToString(),
                app_id = APP_ID,
                event_type = "LevelSuccess",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                metadata = new
                {
                    levelNumber = 1
                }
            };
            
            string json = JsonConvert.SerializeObject(eventData);
            byte[] bodyData = Encoding.UTF8.GetBytes(json);
            
            using (var request = new UnityWebRequest("https://eventapi.dev.decisionbox.io/events", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✓ Event sent successfully");
                    Debug.Log($"Response: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"✗ Event sending failed: {request.error}");
                    Debug.LogError($"Response Code: {request.responseCode}");
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.LogError($"Response: {request.downloadHandler.text}");
                    }
                }
            }
        }
    }
    
    [Serializable]
    public class AuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}