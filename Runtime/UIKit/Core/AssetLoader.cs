using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Handles loading assets from remote URLs for UIKit templates
    /// </summary>
    public class AssetLoader
    {
        private readonly Dictionary<string, Texture2D> _memoryCache;
        private readonly Queue<AssetLoadRequest> _loadQueue;
        private readonly int _maxConcurrentLoads;
        private int _currentLoadCount;
        
        public AssetLoader(int maxConcurrentLoads = 3)
        {
            _memoryCache = new Dictionary<string, Texture2D>();
            _loadQueue = new Queue<AssetLoadRequest>();
            _maxConcurrentLoads = maxConcurrentLoads;
            _currentLoadCount = 0;
        }
        
        /// <summary>
        /// Loads a texture from a URL
        /// </summary>
        /// <param name="url">The URL to load the texture from</param>
        /// <param name="onComplete">Callback when loading completes</param>
        /// <param name="onProgress">Optional callback for progress updates</param>
        public void LoadTexture(string url, Action<Texture2D, string> onComplete, Action<float> onProgress = null)
        {
            // Check memory cache first
            if (_memoryCache.TryGetValue(url, out var cachedTexture))
            {
                onComplete?.Invoke(cachedTexture, null);
                return;
            }
            
            // Check if asset exists in cache
            var cache = UIKitManager.Instance.AssetCache;
            if (cache.TryGetCachedTexture(url, out var texture))
            {
                _memoryCache[url] = texture;
                onComplete?.Invoke(texture, null);
                return;
            }
            
            // Add to load queue
            var request = new AssetLoadRequest
            {
                Url = url,
                OnComplete = onComplete,
                OnProgress = onProgress
            };
            
            _loadQueue.Enqueue(request);
            ProcessLoadQueue();
        }
        
        /// <summary>
        /// Batch loads multiple textures
        /// </summary>
        /// <param name="urls">Array of URLs to load</param>
        /// <param name="onAllComplete">Callback when all textures are loaded</param>
        /// <param name="onProgress">Optional callback for overall progress</param>
        public void LoadTextures(string[] urls, Action<Dictionary<string, Texture2D>> onAllComplete, Action<float> onProgress = null)
        {
            var results = new Dictionary<string, Texture2D>();
            var loadedCount = 0;
            var totalCount = urls.Length;
            
            if (totalCount == 0)
            {
                onAllComplete?.Invoke(results);
                return;
            }
            
            foreach (var url in urls)
            {
                LoadTexture(url, (texture, error) =>
                {
                    if (texture != null)
                    {
                        results[url] = texture;
                    }
                    
                    loadedCount++;
                    onProgress?.Invoke((float)loadedCount / totalCount);
                    
                    if (loadedCount == totalCount)
                    {
                        onAllComplete?.Invoke(results);
                    }
                }, null);
            }
        }
        
        /// <summary>
        /// Preloads assets for a specific template
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <param name="assetUrls">Dictionary of asset URLs</param>
        /// <param name="onComplete">Callback when preloading completes</param>
        public void PreloadTemplateAssets(string templateId, Dictionary<string, object> assetUrls, Action<bool> onComplete)
        {
            var urlList = new List<string>();
            ExtractUrls(assetUrls, urlList);
            
            if (urlList.Count == 0)
            {
                onComplete?.Invoke(true);
                return;
            }
            
            LoadTextures(urlList.ToArray(), (results) =>
            {
                onComplete?.Invoke(results.Count == urlList.Count);
            });
        }
        
        private void ExtractUrls(Dictionary<string, object> data, List<string> urls)
        {
            foreach (var kvp in data)
            {
                if (kvp.Value is string url && IsValidUrl(url))
                {
                    urls.Add(url);
                }
                else if (kvp.Value is Dictionary<string, object> nestedDict)
                {
                    ExtractUrls(nestedDict, urls);
                }
            }
        }
        
        private bool IsValidUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && 
                   (url.StartsWith("http://") || url.StartsWith("https://"));
        }
        
        private void ProcessLoadQueue()
        {
            while (_loadQueue.Count > 0 && _currentLoadCount < _maxConcurrentLoads)
            {
                var request = _loadQueue.Dequeue();
                _currentLoadCount++;
                
                UIKitManager.Instance.StartCoroutine(LoadTextureCoroutine(request));
            }
        }
        
        private IEnumerator LoadTextureCoroutine(AssetLoadRequest request)
        {
            Debug.Log($"[UIKit AssetLoader] Starting texture download from: {request.Url}");
            
            using (var webRequest = UnityWebRequestTexture.GetTexture(request.Url))
            {
                webRequest.timeout = 30; // 30 second timeout
                
                var operation = webRequest.SendWebRequest();
                
                while (!operation.isDone)
                {
                    request.OnProgress?.Invoke(operation.progress);
                    yield return null;
                }
                
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var texture = DownloadHandlerTexture.GetContent(webRequest);
                    Debug.Log($"[UIKit AssetLoader] Successfully downloaded texture: {texture.width}x{texture.height} from {request.Url}");
                    
                    // Cache the texture
                    _memoryCache[request.Url] = texture;
                    UIKitManager.Instance.AssetCache.CacheTexture(request.Url, texture);
                    
                    request.OnComplete?.Invoke(texture, null);
                }
                else
                {
                    Debug.LogError($"[UIKit AssetLoader] Failed to load texture from {request.Url}: {webRequest.error}");
                    request.OnComplete?.Invoke(null, webRequest.error);
                }
            }
            
            _currentLoadCount--;
            ProcessLoadQueue();
        }
        
        /// <summary>
        /// Clears the memory cache
        /// </summary>
        public void ClearMemoryCache()
        {
            foreach (var texture in _memoryCache.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }
            _memoryCache.Clear();
        }
        
        /// <summary>
        /// Gets the current number of items in memory cache
        /// </summary>
        public int MemoryCacheCount => _memoryCache.Count;
        
        /// <summary>
        /// Gets the current number of items in the load queue
        /// </summary>
        public int LoadQueueCount => _loadQueue.Count;
        
        private class AssetLoadRequest
        {
            public string Url { get; set; }
            public Action<Texture2D, string> OnComplete { get; set; }
            public Action<float> OnProgress { get; set; }
        }
    }
}