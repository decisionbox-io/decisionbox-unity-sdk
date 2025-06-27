using UnityEngine;
using UnityEditor;
using DecisionBox.Core;

namespace DecisionBox.Editor
{
    public class DecisionBoxSDKSetup : EditorWindow
    {
        private string appId = "";
        private string appSecret = "";
        private bool useDevEnvironment = true;

        [MenuItem("DecisionBox/Setup SDK")]
        public static void ShowWindow()
        {
            GetWindow<DecisionBoxSDKSetup>("DecisionBox SDK Setup");
        }

        [MenuItem("DecisionBox/Create SDK GameObject")]
        public static void CreateSDKGameObject()
        {
            // Check if SDK already exists
            var existingSDK = FindObjectOfType<DecisionBoxSDK>();
            if (existingSDK != null)
            {
                Debug.LogWarning("DecisionBox SDK already exists in the scene!");
                Selection.activeGameObject = existingSDK.gameObject;
                return;
            }

            // Create new GameObject with SDK
            var sdkObject = new GameObject("DecisionBox SDK");
            var sdk = sdkObject.AddComponent<DecisionBoxSDK>();
            
            // Mark as DontDestroyOnLoad is handled by the SDK itself
            Debug.Log("DecisionBox SDK GameObject created! Configure it in the Inspector.");
            
            // Select the new object
            Selection.activeGameObject = sdkObject;
            
            // Focus the inspector
            EditorGUIUtility.PingObject(sdkObject);
        }

        private void OnGUI()
        {
            GUILayout.Label("DecisionBox SDK Setup", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Configure your DecisionBox SDK credentials below.", MessageType.Info);
            
            EditorGUILayout.Space();
            appId = EditorGUILayout.TextField("App ID", appId);
            appSecret = EditorGUILayout.PasswordField("App Secret", appSecret);
            useDevEnvironment = EditorGUILayout.Toggle("Use Development Environment", useDevEnvironment);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Configured SDK GameObject"))
            {
                if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
                {
                    EditorUtility.DisplayDialog("Missing Credentials", 
                        "Please enter both App ID and App Secret.", "OK");
                    return;
                }
                
                CreateConfiguredSDK();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("You can also use DecisionBox > Create SDK GameObject to create an unconfigured SDK.", MessageType.None);
        }

        private void CreateConfiguredSDK()
        {
            // Check if SDK already exists
            var existingSDK = FindObjectOfType<DecisionBoxSDK>();
            if (existingSDK != null)
            {
                if (EditorUtility.DisplayDialog("SDK Exists", 
                    "A DecisionBox SDK already exists in the scene. Replace it?", 
                    "Replace", "Cancel"))
                {
                    DestroyImmediate(existingSDK.gameObject);
                }
                else
                {
                    return;
                }
            }

            // Create the SDK GameObject
            var sdkObject = new GameObject("DecisionBox SDK");
            var sdk = sdkObject.AddComponent<DecisionBoxSDK>();
            
            // Configure via serialized properties
            var serializedObject = new SerializedObject(sdk);
            serializedObject.FindProperty("appId").stringValue = appId;
            serializedObject.FindProperty("appSecret").stringValue = appSecret;
            serializedObject.FindProperty("environment").stringValue = useDevEnvironment ? "development" : "production";
            serializedObject.FindProperty("enableLogging").boolValue = useDevEnvironment;
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log($"DecisionBox SDK created and configured for {(useDevEnvironment ? "development" : "production")} environment!");
            
            // Select and ping
            Selection.activeGameObject = sdkObject;
            EditorGUIUtility.PingObject(sdkObject);
            
            // Close window
            Close();
        }
    }

    // Custom inspector for better SDK configuration
    [CustomEditor(typeof(DecisionBoxSDK))]
    public class DecisionBoxSDKInspector : UnityEditor.Editor
    {
        private SerializedProperty appIdProp;
        private SerializedProperty appSecretProp;
        private SerializedProperty environmentProp;
        private SerializedProperty enableLoggingProp;

        private void OnEnable()
        {
            appIdProp = serializedObject.FindProperty("appId");
            appSecretProp = serializedObject.FindProperty("appSecret");
            environmentProp = serializedObject.FindProperty("environment");
            enableLoggingProp = serializedObject.FindProperty("enableLogging");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("SDK Configuration", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(appIdProp, new GUIContent("App ID"));
            EditorGUILayout.PropertyField(appSecretProp, new GUIContent("App Secret"));
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(environmentProp, new GUIContent("Environment"));
            EditorGUILayout.PropertyField(enableLoggingProp, new GUIContent("Enable Logging"));
            
            if (string.IsNullOrEmpty(appIdProp.stringValue) || string.IsNullOrEmpty(appSecretProp.stringValue))
            {
                EditorGUILayout.HelpBox("Please configure App ID and App Secret to use the SDK.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("SDK is configured and ready to use!", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}