// using System;
// using UnityEngine;
// using DecisionBox.Models;

// namespace DecisionBox.Session
// {
//     /// <summary>
//     /// Interface for session management
//     /// </summary>
//     public interface ISessionManager
//     {
//         string UserId { get; }
//         string SessionId { get; }
//         SessionInfo CurrentSession { get; }
//         bool IsActive { get; }
        
//         void StartSession(string userId);
//         void EndSession();
//         void UpdateSessionProperty(string key, object value);
//     }

//     /// <summary>
//     /// Manages user sessions in Unity
//     /// </summary>
//     public class SessionManager : ISessionManager
//     {
//         private SessionInfo _currentSession;

//         public string UserId => _currentSession?.UserId;
//         public string SessionId => _currentSession?.SessionId;
//         public SessionInfo CurrentSession => _currentSession;
//         public bool IsActive => _currentSession != null && _currentSession.EndTime == null;

//         /// <summary>
//         /// Start a new session for the user
//         /// </summary>
//         public void StartSession(string userId)
//         {
//             if (string.IsNullOrEmpty(userId))
//                 throw new ArgumentException("User ID cannot be empty", nameof(userId));

//             // End current session if active
//             if (IsActive)
//                 EndSession();

//             _currentSession = new SessionInfo
//             {
//                 UserId = userId,
//                 SessionId = GenerateSessionId()
//             };

//             Debug.Log($"[DecisionBox] Session started for user: {userId}, Session ID: {SessionId}");
//         }

//         /// <summary>
//         /// End the current session
//         /// </summary>
//         public void EndSession()
//         {
//             if (_currentSession != null && _currentSession.EndTime == null)
//             {
//                 _currentSession.EndTime = DateTime.UtcNow;
//                 Debug.Log($"[DecisionBox] Session ended. Duration: {_currentSession.GetDuration():hh\\:mm\\:ss}");
//             }
//         }

//         /// <summary>
//         /// Update a custom session property
//         /// </summary>
//         public void UpdateSessionProperty(string key, object value)
//         {
//             if (_currentSession?.CustomProperties != null)
//             {
//                 _currentSession.CustomProperties[key] = value;
//             }
//         }

//         /// <summary>
//         /// Generate a unique session ID
//         /// </summary>
//         private string GenerateSessionId()
//         {
//             var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//             var random = UnityEngine.Random.Range(1000, 9999);
//             return $"unity_{timestamp}_{random}";
//         }

//         /// <summary>
//         /// Get session duration in seconds
//         /// </summary>
//         public float GetSessionDurationSeconds()
//         {
//             if (_currentSession == null) return 0f;
            
//             var endTime = _currentSession.EndTime ?? DateTime.UtcNow;
//             return (float)(endTime - _currentSession.StartTime).TotalSeconds;
//         }

//         /// <summary>
//         /// Get session metadata for events
//         /// </summary>
//         public System.Collections.Generic.Dictionary<string, object> GetSessionMetadata()
//         {
//             if (_currentSession == null) return new System.Collections.Generic.Dictionary<string, object>();

//             return new System.Collections.Generic.Dictionary<string, object>
//             {
//                 ["platform"] = _currentSession.Platform.ToString(),
//                 ["device_model"] = _currentSession.DeviceModel,
//                 ["operating_system"] = _currentSession.OperatingSystem,
//                 ["app_version"] = _currentSession.AppVersion,
//                 ["session_duration"] = GetSessionDurationSeconds()
//             };
//         }
//     }
// }