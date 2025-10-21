using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of cache service with LRU eviction policy
/// </summary>
public class CacheService : ICacheService
{
    private readonly LRUCache<string, SKTypeface> _typefaceCache;
    private readonly LRUCache<string, SKBitmap> _thumbnailCache;
    private readonly LRUCache<string, FontMetadata> _metadataCache;
    private readonly string _diskCachePath;
    
    private int _typefaceHits = 0;
    private int _typefaceMisses = 0;
    private int _thumbnailHits = 0;
    private int _thumbnailMisses = 0;
    private int _metadataHits = 0;
    private int _metadataMisses = 0;

    public CacheService()
    {
        // LRU cache for SKTypeface (50 items, memory-only)
        _typefaceCache = new LRUCache<string, SKTypeface>(50, OnTypefaceEvicted);
        
        // LRU cache for thumbnails (200 items, disk + memory)
        _thumbnailCache = new LRUCache<string, SKBitmap>(200, OnBitmapEvicted);
        
        // LRU cache for metadata (500 items, disk + memory)
        _metadataCache = new LRUCache<string, FontMetadata>(500, null);
        
        // Set up disk cache directory
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _diskCachePath = Path.Combine(appData, "WowFontManager", "Cache");
        Directory.CreateDirectory(_diskCachePath);
    }

    /// <inheritdoc/>
    public SKTypeface GetOrAddTypeface(string filePath, Func<SKTypeface> factory)
    {
        var cacheKey = GenerateCacheKey(filePath);
        
        if (_typefaceCache.TryGetValue(cacheKey, out var cachedTypeface))
        {
            _typefaceHits++;
            return cachedTypeface;
        }

        _typefaceMisses++;
        var typeface = factory();
        if (typeface != null)
        {
            _typefaceCache.Add(cacheKey, typeface);
        }
        return typeface;
    }

    /// <inheritdoc/>
    public async Task<SKBitmap?> GetOrAddThumbnailAsync(string key, Func<Task<SKBitmap?>> factory)
    {
        var cacheKey = key;
        
        // Check memory cache first
        if (_thumbnailCache.TryGetValue(cacheKey, out var cachedBitmap))
        {
            _thumbnailHits++;
            return cachedBitmap;
        }

        // Check disk cache
        var diskPath = GetDiskCachePath("thumbnails", cacheKey);
        if (File.Exists(diskPath))
        {
            try
            {
                var bitmap = SKBitmap.Decode(diskPath);
                if (bitmap != null)
                {
                    _thumbnailHits++;
                    _thumbnailCache.Add(cacheKey, bitmap);
                    return bitmap;
                }
            }
            catch
            {
                // Corrupted cache file, delete it
                File.Delete(diskPath);
            }
        }

        // Generate new thumbnail
        _thumbnailMisses++;
        var newBitmap = await factory();
        if (newBitmap != null)
        {
            _thumbnailCache.Add(cacheKey, newBitmap);
            
            // Save to disk cache
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(diskPath)!);
                using var image = SKImage.FromBitmap(newBitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 80);
                using var stream = File.OpenWrite(diskPath);
                data.SaveTo(stream);
            }
            catch
            {
                // Ignore disk cache errors
            }
        }
        
        return newBitmap;
    }

    /// <inheritdoc/>
    public async Task<FontMetadata?> GetOrAddMetadataAsync(string filePath, Func<Task<FontMetadata?>> factory)
    {
        var cacheKey = GenerateCacheKey(filePath);
        
        // Check memory cache first
        if (_metadataCache.TryGetValue(cacheKey, out var cachedMetadata))
        {
            _metadataHits++;
            return cachedMetadata;
        }

        // Check disk cache
        var diskPath = GetDiskCachePath("metadata", cacheKey + ".json");
        if (File.Exists(diskPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(diskPath);
                var metadata = JsonSerializer.Deserialize<FontMetadata>(json);
                if (metadata != null)
                {
                    _metadataHits++;
                    _metadataCache.Add(cacheKey, metadata);
                    return metadata;
                }
            }
            catch
            {
                // Corrupted cache file, delete it
                File.Delete(diskPath);
            }
        }

        // Load new metadata
        _metadataMisses++;
        var newMetadata = await factory();
        if (newMetadata != null)
        {
            _metadataCache.Add(cacheKey, newMetadata);
            
            // Save to disk cache
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(diskPath)!);
                var json = JsonSerializer.Serialize(newMetadata, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(diskPath, json);
            }
            catch
            {
                // Ignore disk cache errors
            }
        }
        
        return newMetadata;
    }

    /// <inheritdoc/>
    public void InvalidateFile(string filePath)
    {
        var cacheKey = GenerateCacheKey(filePath);
        _typefaceCache.Remove(cacheKey);
        _metadataCache.Remove(cacheKey);
        
        // Remove from disk cache
        try
        {
            var metadataPath = GetDiskCachePath("metadata", cacheKey + ".json");
            if (File.Exists(metadataPath))
                File.Delete(metadataPath);
        }
        catch
        {
            // Ignore errors
        }
    }

    /// <inheritdoc/>
    public void ClearAll()
    {
        _typefaceCache.Clear();
        _thumbnailCache.Clear();
        _metadataCache.Clear();
        
        // Clear disk cache
        try
        {
            if (Directory.Exists(_diskCachePath))
                Directory.Delete(_diskCachePath, true);
            Directory.CreateDirectory(_diskCachePath);
        }
        catch
        {
            // Ignore errors
        }
    }

    /// <inheritdoc/>
    public CacheStatistics GetStatistics()
    {
        return new CacheStatistics
        {
            TypefaceHits = _typefaceHits,
            TypefaceMisses = _typefaceMisses,
            ThumbnailHits = _thumbnailHits,
            ThumbnailMisses = _thumbnailMisses,
            MetadataHits = _metadataHits,
            MetadataMisses = _metadataMisses,
            TypefaceCacheSize = _typefaceCache.Count,
            ThumbnailCacheSize = _thumbnailCache.Count,
            MetadataCacheSize = _metadataCache.Count
        };
    }

    /// <summary>
    /// Generates cache key from file path with modification timestamp
    /// </summary>
    private string GenerateCacheKey(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            var timestamp = fileInfo.LastWriteTimeUtc.Ticks;
            return $"{Path.GetFileName(filePath)}_{timestamp}";
        }
        catch
        {
            return Path.GetFileName(filePath);
        }
    }

    /// <summary>
    /// Gets disk cache file path
    /// </summary>
    private string GetDiskCachePath(string category, string key)
    {
        // Use hash to avoid file system path length issues
        var hash = key.GetHashCode().ToString("X8");
        return Path.Combine(_diskCachePath, category, hash);
    }

    /// <summary>
    /// Called when a typeface is evicted from cache
    /// </summary>
    private void OnTypefaceEvicted(SKTypeface typeface)
    {
        typeface?.Dispose();
    }

    /// <summary>
    /// Called when a bitmap is evicted from cache
    /// </summary>
    private void OnBitmapEvicted(SKBitmap bitmap)
    {
        bitmap?.Dispose();
    }

    public void Dispose()
    {
        _typefaceCache.Dispose();
        _thumbnailCache.Dispose();
        _metadataCache.Dispose();
    }
}

/// <summary>
/// LRU (Least Recently Used) cache implementation
/// </summary>
internal class LRUCache<TKey, TValue> : IDisposable where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cache;
    private readonly LinkedList<CacheItem> _lruList;
    private readonly Action<TValue>? _evictionCallback;

    public int Count => _cache.Count;

    public LRUCache(int capacity, Action<TValue>? evictionCallback = null)
    {
        _capacity = capacity;
        _cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
        _lruList = new LinkedList<CacheItem>();
        _evictionCallback = evictionCallback;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_cache.TryGetValue(key, out var node))
        {
            // Move to front (most recently used)
            _lruList.Remove(node);
            _lruList.AddFirst(node);
            value = node.Value.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public void Add(TKey key, TValue value)
    {
        if (_cache.TryGetValue(key, out var existingNode))
        {
            // Update existing item
            _lruList.Remove(existingNode);
            _cache.Remove(key);
            _evictionCallback?.Invoke(existingNode.Value.Value);
        }
        else if (_cache.Count >= _capacity)
        {
            // Evict least recently used item
            var lruNode = _lruList.Last!;
            _lruList.RemoveLast();
            _cache.Remove(lruNode.Value.Key);
            _evictionCallback?.Invoke(lruNode.Value.Value);
        }

        // Add new item
        var cacheItem = new CacheItem { Key = key, Value = value };
        var node = _lruList.AddFirst(cacheItem);
        _cache[key] = node;
    }

    public bool Remove(TKey key)
    {
        if (_cache.TryGetValue(key, out var node))
        {
            _lruList.Remove(node);
            _cache.Remove(key);
            _evictionCallback?.Invoke(node.Value.Value);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        if (_evictionCallback != null)
        {
            foreach (var node in _lruList)
            {
                _evictionCallback(node.Value);
            }
        }
        
        _lruList.Clear();
        _cache.Clear();
    }

    public void Dispose()
    {
        Clear();
    }

    private class CacheItem
    {
        public TKey Key { get; set; } = default!;
        public TValue Value { get; set; } = default!;
    }
}
