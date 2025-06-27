using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using DecisionBox.Auth;
using DecisionBox.Core;
using DecisionBox.Models;

// Alias to avoid ambiguity with UnityEngine.Event
using DBEvent = DecisionBox.Models.Event;

namespace DecisionBox.Events
{
    /// <summary>
    /// Interface for sending events
    /// </summary>
    public interface IEventSender
    {
        void Initialize(DecisionBoxConfig config, IAuthManager authManager);
        Task SendEventAsync(DBEvent eventData);
        void SendEventBatch(List<DBEvent> events);
        void QueueEvent(DBEvent eventData);
        void FlushQueue();
    }

    /// <summary>
    /// Handles sending events to DecisionBox API using Unity's networking
    /// </summary>
    public class EventSender : MonoBehaviour, IEventSender
    {
        private DecisionBoxConfig _config;
        private IAuthManager _authManager;
        private Queue<DBEvent> _eventQueue;
        private List<DBEvent> _currentBatch;
        private bool _isSending;
        private float _lastBatchTime;

        public static EventSender Instance { get; private set; }

        public void Initialize(DecisionBoxConfig config, IAuthManager authManager)
        {
            _config = config;
            _authManager = authManager;
            _eventQueue = new Queue<DBEvent>();
            _currentBatch = new List<DBEvent>();
            _lastBatchTime = Time.time;

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (_config?.EnableEventBatching == true && 
                _currentBatch.Count > 0 && 
                Time.time - _lastBatchTime >= _config.BatchTimeoutSeconds)
            {
                FlushQueue();
            }

            ProcessEventQueue();
        }

        /// <summary>
        /// Send an event asynchronously
        /// </summary>
        public async Task SendEventAsync(DBEvent eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            if (_config.EnableEventBatching)
            {
                QueueEvent(eventData);
                return;
            }

            await SendSingleEventAsync(eventData);
        }

        /// <summary>
        /// Queue an event for batch sending
        /// </summary>
        public void QueueEvent(DBEvent eventData)
        {
            if (eventData == null) return;

            if (_config.EnableEventBatching)
            {
                _currentBatch.Add(eventData);
                
                if (_currentBatch.Count >= _config.MaxBatchSize)
                {
                    FlushQueue();
                }
            }
            else
            {
                _eventQueue.Enqueue(eventData);
            }
        }

        /// <summary>
        /// Process queued events
        /// </summary>
        private void ProcessEventQueue()
        {
            if (_isSending || _eventQueue.Count == 0) return;

            StartCoroutine(ProcessQueueCoroutine());
        }

        private IEnumerator ProcessQueueCoroutine()
        {
            _isSending = true;

            while (_eventQueue.Count > 0)
            {
                var eventData = _eventQueue.Dequeue();
                var task = SendSingleEventAsync(eventData);
                
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsFaulted && _config.EnableDebugLogs)
                {
                    Debug.LogError($"[DecisionBox] Failed to send event: {task.Exception?.GetBaseException().Message}");
                }

                yield return null; // Wait a frame between sends
            }

            _isSending = false;
        }

        /// <summary>
        /// Flush the current batch
        /// </summary>
        public void FlushQueue()
        {
            if (_currentBatch.Count == 0) return;

            var batchToSend = new List<DBEvent>(_currentBatch);
            _currentBatch.Clear();
            _lastBatchTime = Time.time;

            StartCoroutine(SendBatchCoroutine(batchToSend));
        }

        /// <summary>
        /// Send a batch of events
        /// </summary>
        public void SendEventBatch(List<DBEvent> events)
        {
            if (events == null || events.Count == 0) return;
            StartCoroutine(SendBatchCoroutine(events));
        }

        private IEnumerator SendBatchCoroutine(List<DBEvent> events)
        {
            var task = SendEventBatchAsync(events);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted && _config.EnableDebugLogs)
            {
                Debug.LogError($"[DecisionBox] Failed to send event batch: {task.Exception?.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// Send a single event to the API
        /// </summary>
        private async Task SendSingleEventAsync(DBEvent eventData)
        {
            try
            {
                var token = await _authManager.GetAccessTokenAsync();
                var json = JsonConvert.SerializeObject(eventData);
                var bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{_config.ApiUrl}/events", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {token}");
                    request.timeout = _config.TimeoutMs / 1000;

                    var operation = request.SendWebRequest();
                    
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        if (_config.EnableDebugLogs)
                            Debug.Log($"[DecisionBox] Event sent successfully: {eventData.EventType}");
                    }
                    else
                    {
                        var error = $"Failed to send event: {request.error}";
                        if (_config.EnableVerboseLogging)
                            Debug.LogError($"[DecisionBox] {error}\nResponse: {request.downloadHandler?.text}");
                        
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_config.EnableOfflineSupport)
                {
                    CacheEventForLater(eventData);
                }
                
                throw new Exception($"Failed to send event {eventData.EventType}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Send a batch of events to the API
        /// </summary>
        private async Task SendEventBatchAsync(List<DBEvent> events)
        {
            try
            {
                var token = await _authManager.GetAccessTokenAsync();
                var batchData = new { events = events };
                var json = JsonConvert.SerializeObject(batchData);
                var bodyData = Encoding.UTF8.GetBytes(json);

                using (var request = new UnityWebRequest($"{_config.ApiUrl}/events/batch", "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", $"Bearer {token}");
                    request.timeout = _config.TimeoutMs / 1000;

                    var operation = request.SendWebRequest();
                    
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        if (_config.EnableDebugLogs)
                            Debug.Log($"[DecisionBox] Event batch sent successfully: {events.Count} events");
                    }
                    else
                    {
                        var error = $"Failed to send event batch: {request.error}";
                        if (_config.EnableVerboseLogging)
                            Debug.LogError($"[DecisionBox] {error}\nResponse: {request.downloadHandler?.text}");
                        
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_config.EnableOfflineSupport)
                {
                    foreach (var evt in events)
                    {
                        CacheEventForLater(evt);
                    }
                }
                
                throw new Exception($"Failed to send event batch: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cache event for later sending when offline
        /// </summary>
        private void CacheEventForLater(DBEvent eventData)
        {
            try
            {
                int cachedEventCount = PlayerPrefs.GetInt("decisionbox_cached_event_count", 0);
                var key = $"decisionbox_cached_event_{cachedEventCount}";
                var json = JsonConvert.SerializeObject(eventData);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.SetInt("decisionbox_cached_event_count", cachedEventCount + 1);
                PlayerPrefs.Save();

                if (_config.EnableDebugLogs)
                    Debug.Log($"[DecisionBox] Event cached for later: {eventData.EventType}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DecisionBox] Failed to cache event: {ex.Message}");
            }
        }

        /// <summary>
        /// Send cached events when back online
        /// </summary>
        public void SendCachedEvents()
        {
            try
            {
                var cachedEvents = new List<DBEvent>();
                var keysToRemove = new List<string>();

                // Check for cached events (Unity doesn't have GetKeys, so we use a counter)
                int cachedEventCount = PlayerPrefs.GetInt("decisionbox_cached_event_count", 0);
                
                for (int i = 0; i < cachedEventCount; i++)
                {
                    string key = $"decisionbox_cached_event_{i}";
                    if (PlayerPrefs.HasKey(key))
                    {
                        var json = PlayerPrefs.GetString(key);
                        try
                        {
                            var evt = JsonConvert.DeserializeObject<DBEvent>(json);
                            cachedEvents.Add(evt);
                            keysToRemove.Add(key);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"[DecisionBox] Failed to deserialize cached event: {ex.Message}");
                            keysToRemove.Add(key); // Remove corrupted entries
                        }
                    }
                }

                // Send cached events
                if (cachedEvents.Count > 0)
                {
                    SendEventBatch(cachedEvents);
                    
                    // Remove cached events after sending
                    foreach (var key in keysToRemove)
                    {
                        PlayerPrefs.DeleteKey(key);
                    }
                    PlayerPrefs.SetInt("decisionbox_cached_event_count", 0);
                    PlayerPrefs.Save();

                    if (_config.EnableDebugLogs)
                        Debug.Log($"[DecisionBox] Sent {cachedEvents.Count} cached events");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DecisionBox] Failed to send cached events: {ex.Message}");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) // Resumed
            {
                SendCachedEvents();
            }
            else // Paused
            {
                FlushQueue();
            }
        }

        private void OnDestroy()
        {
            FlushQueue();
        }
    }
}