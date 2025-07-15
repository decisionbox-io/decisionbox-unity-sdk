using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Handles rendering of UIKit templates in Unity UI
    /// </summary>
    public class TemplateRenderer
    {
        private Canvas _uiCanvas;
        private GameObject _canvasRoot;
        private readonly Dictionary<string, GameObject> _templateInstances;
        private readonly Stack<GameObject> _pooledContainers;
        
        private const string CANVAS_NAME = "[DecisionBox UIKit Canvas]";
        private const int CANVAS_SORT_ORDER = 1000;
        
        public TemplateRenderer()
        {
            _templateInstances = new Dictionary<string, GameObject>();
            _pooledContainers = new Stack<GameObject>();
            
            InitializeCanvas();
        }
        
        /// <summary>
        /// Initializes the UI canvas for rendering templates
        /// </summary>
        private void InitializeCanvas()
        {
            // Find or create the UIKit canvas
            var existingCanvas = GameObject.Find(CANVAS_NAME);
            if (existingCanvas != null)
            {
                _canvasRoot = existingCanvas;
                _uiCanvas = _canvasRoot.GetComponent<Canvas>();
            }
            else
            {
                _canvasRoot = new GameObject(CANVAS_NAME);
                _uiCanvas = _canvasRoot.AddComponent<Canvas>();
                _uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _uiCanvas.sortingOrder = CANVAS_SORT_ORDER;
                
                // Add CanvasScaler for responsive UI
                var scaler = _canvasRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                // Add GraphicRaycaster for UI interaction
                _canvasRoot.AddComponent<GraphicRaycaster>();
                
                // Make the canvas persistent
                UnityEngine.Object.DontDestroyOnLoad(_canvasRoot);
            }
        }
        
        /// <summary>
        /// Creates a container for a template
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <returns>The created container GameObject</returns>
        public GameObject CreateTemplateContainer(string templateId)
        {
            GameObject container;
            
            // Try to get a pooled container
            if (_pooledContainers.Count > 0)
            {
                container = _pooledContainers.Pop();
                container.name = $"Template_{templateId}";
                container.SetActive(true);
            }
            else
            {
                container = new GameObject($"Template_{templateId}");
                container.transform.SetParent(_uiCanvas.transform, false);
                
                // Add RectTransform and configure for full screen
                var rectTransform = container.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                
                // Add CanvasGroup for fade animations
                container.AddComponent<CanvasGroup>();
            }
            
            _templateInstances[templateId] = container;
            return container;
        }
        
        
        /// <summary>
        /// Shows a template with optional animation
        /// </summary>
        /// <param name="templateId">The template ID to show</param>
        /// <param name="animated">Whether to animate the show</param>
        /// <param name="duration">Animation duration</param>
        public void ShowTemplate(string templateId, bool animated = true, float duration = 0.3f)
        {
            if (!_templateInstances.TryGetValue(templateId, out var container))
            {
                Debug.LogError($"[UIKit TemplateRenderer] Template not found: {templateId}");
                return;
            }
            
            container.SetActive(true);
            
            if (animated)
            {
                var canvasGroup = container.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    UIKitManager.Instance.StartCoroutine(
                        AnimationHelper.FadeIn(canvasGroup, duration)
                    );
                }
            }
        }
        
        /// <summary>
        /// Hides a template with optional animation
        /// </summary>
        /// <param name="templateId">The template ID to hide</param>
        /// <param name="animated">Whether to animate the hide</param>
        /// <param name="duration">Animation duration</param>
        /// <param name="onComplete">Callback when hide completes</param>
        public void HideTemplate(string templateId, bool animated = true, float duration = 0.3f, Action onComplete = null)
        {
            if (!_templateInstances.TryGetValue(templateId, out var container))
            {
                onComplete?.Invoke();
                return;
            }
            
            if (animated)
            {
                var canvasGroup = container.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    UIKitManager.Instance.StartCoroutine(
                        AnimationHelper.FadeOut(canvasGroup, duration, () =>
                        {
                            HideTemplateImmediate(templateId);
                            onComplete?.Invoke();
                        })
                    );
                    return;
                }
            }
            
            HideTemplateImmediate(templateId);
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// Immediately hides a template without animation
        /// </summary>
        private void HideTemplateImmediate(string templateId)
        {
            if (_templateInstances.TryGetValue(templateId, out var container))
            {
                container.SetActive(false);
                ClearContainer(container);
                
                // Return to pool
                _templateInstances.Remove(templateId);
                _pooledContainers.Push(container);
            }
        }
        
        /// <summary>
        /// Clears all content from a container
        /// </summary>
        private void ClearContainer(GameObject container)
        {
            foreach (Transform child in container.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// Destroys a template instance
        /// </summary>
        /// <param name="templateId">The template ID to destroy</param>
        public void DestroyTemplate(string templateId)
        {
            if (_templateInstances.TryGetValue(templateId, out var container))
            {
                _templateInstances.Remove(templateId);
                UnityEngine.Object.Destroy(container);
            }
        }
        
        /// <summary>
        /// Gets the container for a template
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <returns>The container GameObject if found</returns>
        public GameObject GetTemplateContainer(string templateId)
        {
            _templateInstances.TryGetValue(templateId, out var container);
            return container;
        }
        
        /// <summary>
        /// Checks if a template is currently visible
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <returns>True if the template is visible</returns>
        public bool IsTemplateVisible(string templateId)
        {
            if (_templateInstances.TryGetValue(templateId, out var container))
            {
                return container.activeInHierarchy;
            }
            return false;
        }
        
        /// <summary>
        /// Gets the UIKit canvas
        /// </summary>
        public Canvas Canvas => _uiCanvas;
        
        /// <summary>
        /// Gets the canvas root GameObject
        /// </summary>
        public GameObject CanvasRoot => _canvasRoot;
    }
    
    /// <summary>
    /// Helper class for UI animations
    /// </summary>
    internal static class AnimationHelper
    {
        public static System.Collections.IEnumerator FadeIn(CanvasGroup canvasGroup, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
            onComplete?.Invoke();
        }
        
        public static System.Collections.IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            onComplete?.Invoke();
        }
    }
}