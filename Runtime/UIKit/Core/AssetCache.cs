using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DecisionBox.UIKit.Core
{
    /// <summary>
    /// Manages disk caching of UIKit assets
    /// </summary>
    public class AssetCache : IDisposable
    {
        private readonly string _cacheBasePath;
        private readonly long _maxCacheSize;
        private readonly Dictionary<string, CacheEntry> _cacheIndex;
        private long _currentCacheSize;
        
        private const string CACHE_INDEX_FILE = "cache_index.json";
        private const long DEFAULT_MAX_CACHE_SIZE = 100 * 1024 * 1024; // 100MB
        
        public AssetCache(long maxCacheSize = DEFAULT_MAX_CACHE_SIZE)
        {
            _maxCacheSize = maxCacheSize;
            _cacheBasePath = Path.Combine(Application.persistentDataPath, "DecisionBoxUIKit", "Cache");
            _cacheIndex = new Dictionary<string, CacheEntry>();
            
            InitializeCache();
        }
        
        /// <summary>
        /// Initializes the cache directory and loads the cache index
        /// </summary>
        private void InitializeCache()
        {
            if (!Directory.Exists(_cacheBasePath))
            {
                Directory.CreateDirectory(_cacheBasePath);
            }
            
            LoadCacheIndex();
            ValidateCache();
        }
        
        /// <summary>
        /// Caches a texture to disk
        /// </summary>
        /// <param name="url">The URL of the texture</param>
        /// <param name="texture">The texture to cache</param>
        public void CacheTexture(string url, Texture2D texture)
        {
            if (texture == null) return;
            
            var hash = GetUrlHash(url);
            var filePath = Path.Combine(_cacheBasePath, hash + ".png");
            
            try
            {
                var bytes = texture.EncodeToPNG();
                var fileSize = bytes.Length;
                
                // Check if we need to make space
                EnsureCacheSpace(fileSize);
                
                File.WriteAllBytes(filePath, bytes);
                
                // Update cache index
                var entry = new CacheEntry
                {
                    Url = url,
                    Hash = hash,
                    FilePath = filePath,
                    FileSize = fileSize,
                    LastAccessTime = DateTime.UtcNow,
                    CreatedTime = DateTime.UtcNow
                };
                
                _cacheIndex[url] = entry;
                _currentCacheSize += fileSize;
                
                SaveCacheIndex();
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIKit AssetCache] Failed to cache texture: {e.Message}");
            }
        }
        
        /// <summary>
        /// Tries to get a cached texture
        /// </summary>
        /// <param name="url">The URL of the texture</param>
        /// <param name="texture">The loaded texture if found</param>
        /// <returns>True if the texture was found and loaded</returns>
        public bool TryGetCachedTexture(string url, out Texture2D texture)
        {
            texture = null;
            
            if (!_cacheIndex.TryGetValue(url, out var entry))
            {
                return false;
            }
            
            if (!File.Exists(entry.FilePath))
            {
                // File was deleted, remove from index
                _cacheIndex.Remove(url);
                SaveCacheIndex();
                return false;
            }
            
            try
            {
                var bytes = File.ReadAllBytes(entry.FilePath);
                texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                
                // Update last access time
                entry.LastAccessTime = DateTime.UtcNow;
                SaveCacheIndex();
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIKit AssetCache] Failed to load cached texture: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Ensures there's enough space in the cache for a new file
        /// </summary>
        /// <param name="requiredSize">The size needed in bytes</param>
        private void EnsureCacheSpace(long requiredSize)
        {
            if (_currentCacheSize + requiredSize <= _maxCacheSize)
            {
                return;
            }
            
            // Need to free up space - remove least recently used items
            var sortedEntries = _cacheIndex.Values
                .OrderBy(e => e.LastAccessTime)
                .ToList();
            
            foreach (var entry in sortedEntries)
            {
                if (_currentCacheSize + requiredSize <= _maxCacheSize)
                {
                    break;
                }
                
                RemoveCacheEntry(entry);
            }
        }
        
        /// <summary>
        /// Removes a cache entry
        /// </summary>
        private void RemoveCacheEntry(CacheEntry entry)
        {
            try
            {
                if (File.Exists(entry.FilePath))
                {
                    File.Delete(entry.FilePath);
                }
                
                _cacheIndex.Remove(entry.Url);
                _currentCacheSize -= entry.FileSize;
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIKit AssetCache] Failed to remove cache entry: {e.Message}");
            }
        }
        
        /// <summary>
        /// Clears all cached assets
        /// </summary>
        public void ClearCache()
        {
            foreach (var entry in _cacheIndex.Values.ToList())
            {
                RemoveCacheEntry(entry);
            }
            
            _cacheIndex.Clear();
            _currentCacheSize = 0;
            SaveCacheIndex();
        }
        
        /// <summary>
        /// Loads the cache index from disk
        /// </summary>
        private void LoadCacheIndex()
        {
            var indexPath = Path.Combine(_cacheBasePath, CACHE_INDEX_FILE);
            
            if (!File.Exists(indexPath))
            {
                return;
            }
            
            try
            {
                var json = File.ReadAllText(indexPath);
                var data = JsonUtility.FromJson<CacheIndexData>(json);
                
                if (data?.entries != null)
                {
                    _cacheIndex.Clear();
                    _currentCacheSize = 0;
                    
                    foreach (var entry in data.entries)
                    {
                        _cacheIndex[entry.Url] = entry;
                        _currentCacheSize += entry.FileSize;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIKit AssetCache] Failed to load cache index: {e.Message}");
            }
        }
        
        /// <summary>
        /// Saves the cache index to disk
        /// </summary>
        private void SaveCacheIndex()
        {
            try
            {
                var data = new CacheIndexData
                {
                    entries = _cacheIndex.Values.ToArray()
                };
                
                var json = JsonUtility.ToJson(data, true);
                var indexPath = Path.Combine(_cacheBasePath, CACHE_INDEX_FILE);
                
                File.WriteAllText(indexPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIKit AssetCache] Failed to save cache index: {e.Message}");
            }
        }
        
        /// <summary>
        /// Validates the cache by checking if all indexed files exist
        /// </summary>
        private void ValidateCache()
        {
            var entriesToRemove = new List<string>();
            
            foreach (var kvp in _cacheIndex)
            {
                if (!File.Exists(kvp.Value.FilePath))
                {
                    entriesToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var url in entriesToRemove)
            {
                _cacheIndex.Remove(url);
            }
            
            if (entriesToRemove.Count > 0)
            {
                RecalculateCacheSize();
                SaveCacheIndex();
            }
        }
        
        /// <summary>
        /// Recalculates the total cache size
        /// </summary>
        private void RecalculateCacheSize()
        {
            _currentCacheSize = 0;
            
            foreach (var entry in _cacheIndex.Values)
            {
                if (File.Exists(entry.FilePath))
                {
                    var fileInfo = new FileInfo(entry.FilePath);
                    entry.FileSize = fileInfo.Length;
                    _currentCacheSize += entry.FileSize;
                }
            }
        }
        
        /// <summary>
        /// Gets a hash for a URL to use as a filename
        /// </summary>
        private string GetUrlHash(string url)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(url);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        
        /// <summary>
        /// Gets the current cache size in bytes
        /// </summary>
        public long CurrentCacheSize => _currentCacheSize;
        
        /// <summary>
        /// Gets the maximum cache size in bytes
        /// </summary>
        public long MaxCacheSize => _maxCacheSize;
        
        /// <summary>
        /// Gets the number of cached items
        /// </summary>
        public int CachedItemCount => _cacheIndex.Count;
        
        public void Dispose()
        {
            SaveCacheIndex();
        }
        
        [Serializable]
        private class CacheEntry
        {
            public string Url;
            public string Hash;
            public string FilePath;
            public long FileSize;
            public DateTime LastAccessTime;
            public DateTime CreatedTime;
        }
        
        [Serializable]
        private class CacheIndexData
        {
            public CacheEntry[] entries;
        }
    }
}