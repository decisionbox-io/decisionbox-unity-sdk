using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DecisionBox.Models;
using DecisionBox.UIKit.Components;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Main manager for the DecisionBox UIKit system.
    /// Handles template loading, rendering, and lifecycle management.
    /// </summary>
    public class UIKitManager : MonoBehaviour
    {
        private static UIKitManager _instance;
        
        /// <summary>
        /// Singleton instance of the UIKitManager
        /// </summary>
        public static UIKitManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("[DecisionBox UIKit Manager]");
                    _instance = go.AddComponent<UIKitManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private AssetLoader _assetLoader;
        private AssetCache _assetCache;
        private TemplateRenderer _templateRenderer;
        private LayoutManager _layoutManager;
        private ComponentFactory _componentFactory;
        private Dictionary<string, DynamicTemplate> _activeTemplates;
        
        private bool _isInitialized = false;
        
        /// <summary>
        /// Event fired when UIKit is initialized
        /// </summary>
        public event Action OnUIKitInitialized;
        
        /// <summary>
        /// Event fired when a template is shown
        /// </summary>
        public event Action<string> OnTemplateShown;
        
        /// <summary>
        /// Event fired when a template is hidden
        /// </summary>
        public event Action<string> OnTemplateHidden;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }
        
        /// <summary>
        /// Initializes the UIKit system
        /// </summary>
        private void Initialize()
        {
            if (_isInitialized) return;
            
            _assetLoader = new AssetLoader();
            _assetCache = new AssetCache();
            _templateRenderer = new TemplateRenderer();
            _layoutManager = new LayoutManager();
            _componentFactory = new ComponentFactory();
            _activeTemplates = new Dictionary<string, DynamicTemplate>();
            
            _isInitialized = true;
            OnUIKitInitialized?.Invoke();
            
            Debug.Log("[DecisionBox UIKit] Initialized successfully");
        }
        
        /// <summary>
        /// Processes a WebSocket message and renders UI if it contains template data
        /// </summary>
        /// <param name="message">The WebSocket message to process</param>
        /// <returns>True if the message was handled by UIKit, false otherwise</returns>
        public bool ProcessMessage(WebSocketMessage message)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("[DecisionBox UIKit] Not initialized. Call Initialize() first.");
                return false;
            }
            
            // Check if this message should use a template
            if (!message.UseTemplate || message.TemplateData == null)
            {
                return false;
            }
            
            try
            {
                // Generate a unique template ID
                var templateId = GenerateTemplateId(message);
                
                // Create or update the template
                if (_activeTemplates.TryGetValue(templateId, out var existingTemplate))
                {
                    // Update existing template
                    existingTemplate.UpdateWithMessage(message);
                }
                else
                {
                    // Create new template
                    var template = CreateDynamicTemplate(templateId, message);
                    if (template != null)
                    {
                        _activeTemplates[templateId] = template;
                        template.Show();
                        OnTemplateShown?.Invoke(templateId);
                    }
                }
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DecisionBox UIKit] Failed to process template message: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Creates a dynamic template from a WebSocket message
        /// </summary>
        private DynamicTemplate CreateDynamicTemplate(string templateId, WebSocketMessage message)
        {
            // Get layout dimensions
            var layoutSize = message.LayoutSize ?? "MediumModal";
            var layoutDimensions = _layoutManager.GetLayoutDimensions(layoutSize);
            
            // Create template container
            var container = _templateRenderer.CreateTemplateContainer(templateId);
            
            // Create dynamic template
            var template = new DynamicTemplate(templateId, container, layoutDimensions);
            
            // Set template data
            template.SetTemplateData(message.TemplateData);
            template.SetConfig(message.TemplateConfig);
            
            // Build the template
            template.Build(_componentFactory, _assetLoader);
            
            return template;
        }
        
        /// <summary>
        /// Generates a unique template ID from a message
        /// </summary>
        private string GenerateTemplateId(WebSocketMessage message)
        {
            // Use action/type + session ID for uniqueness
            var baseId = !string.IsNullOrEmpty(message.Action) ? message.Action : message.type;
            return $"{baseId}_{message.SessionId}_{DateTime.UtcNow.Ticks}";
        }
        
        /// <summary>
        /// Hides a specific template
        /// </summary>
        /// <param name="templateId">The template ID to hide</param>
        public void HideTemplate(string templateId)
        {
            if (_activeTemplates.TryGetValue(templateId, out var template))
            {
                template.Hide(() =>
                {
                    _activeTemplates.Remove(templateId);
                    template.Dispose();
                    OnTemplateHidden?.Invoke(templateId);
                });
            }
        }
        
        /// <summary>
        /// Hides all active templates
        /// </summary>
        public void HideAllTemplates()
        {
            var templateIds = new List<string>(_activeTemplates.Keys);
            foreach (var templateId in templateIds)
            {
                HideTemplate(templateId);
            }
        }
        
        /// <summary>
        /// Gets the asset loader instance
        /// </summary>
        public AssetLoader AssetLoader => _assetLoader;
        
        /// <summary>
        /// Gets the asset cache instance
        /// </summary>
        public AssetCache AssetCache => _assetCache;
        
        /// <summary>
        /// Gets the template renderer instance
        /// </summary>
        public TemplateRenderer TemplateRenderer => _templateRenderer;
        
        /// <summary>
        /// Gets the layout manager instance
        /// </summary>
        public LayoutManager LayoutManager => _layoutManager;
        
        /// <summary>
        /// Gets the component factory instance
        /// </summary>
        public ComponentFactory ComponentFactory => _componentFactory;
        
        /// <summary>
        /// Checks if a template is currently active
        /// </summary>
        public bool IsTemplateActive(string templateId)
        {
            return _activeTemplates.ContainsKey(templateId);
        }
        
        /// <summary>
        /// Gets all active template IDs
        /// </summary>
        public string[] GetActiveTemplateIds()
        {
            return _activeTemplates.Keys.ToArray();
        }
        
        /// <summary>
        /// Event handler for template action callbacks
        /// </summary>
        public event Action<string, ComponentAction> OnTemplateAction;
        
        /// <summary>
        /// Handles template action callbacks
        /// </summary>
        internal void HandleTemplateAction(string templateId, ComponentAction action)
        {
            OnTemplateAction?.Invoke(templateId, action);
            
            // Handle built-in actions
            if (action.Type == "Close")
            {
                HideTemplate(templateId);
            }
        }
        
        private void OnDestroy()
        {
            _assetCache?.Dispose();
            HideAllTemplates();
        }
    }
}