// using System;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using UnityEngine;

// namespace DecisionBox.Models
// {
    

//     /// <summary>
//     /// Session information
//     /// </summary>
//     [Serializable]
//     public class SessionInfo
//     {
//         public string SessionId { get; set; }
//         public string UserId { get; set; }
//         public DateTime StartTime { get; set; }
//         public DateTime? EndTime { get; set; }
//         public PlatformType Platform { get; set; }
//         public string DeviceModel { get; set; }
//         public string OperatingSystem { get; set; }
//         public string AppVersion { get; set; }
//         public Dictionary<string, object> CustomProperties { get; set; }

//         public SessionInfo()
//         {
//             SessionId = Guid.NewGuid().ToString();
//             StartTime = DateTime.UtcNow;
//             Platform = GetCurrentPlatform();
//             DeviceModel = SystemInfo.deviceModel;
//             OperatingSystem = SystemInfo.operatingSystem;
//             AppVersion = Application.version;
//             CustomProperties = new Dictionary<string, object>();
//         }

//         private static PlatformType GetCurrentPlatform()
//         {
//             switch (Application.platform)
//             {
//                 case RuntimePlatform.Android:
//                     return PlatformType.Android;
//                 case RuntimePlatform.IPhonePlayer:
//                     return PlatformType.iOS;
//                 case RuntimePlatform.WindowsPlayer:
//                 case RuntimePlatform.WindowsEditor:
//                     return PlatformType.Windows;
//                 case RuntimePlatform.OSXPlayer:
//                 case RuntimePlatform.OSXEditor:
//                     return PlatformType.MacOS;
//                 case RuntimePlatform.LinuxPlayer:
//                 case RuntimePlatform.LinuxEditor:
//                     return PlatformType.Linux;
//                 case RuntimePlatform.WebGLPlayer:
//                     return PlatformType.WebGL;
//                 case RuntimePlatform.PS4:
//                 case RuntimePlatform.PS5:
//                     return PlatformType.PlayStation;
//                 case RuntimePlatform.XboxOne:
//                     return PlatformType.Xbox;
//                 case RuntimePlatform.Switch:
//                     return PlatformType.Nintendo;
//                 default:
//                     return Application.isEditor ? PlatformType.Editor : PlatformType.NotSet;
//             }
//         }

//         public TimeSpan GetDuration()
//         {
//             var endTime = EndTime ?? DateTime.UtcNow;
//             return endTime - StartTime;
//         }
//     }
// }