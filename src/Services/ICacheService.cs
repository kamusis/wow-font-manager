using System;
using System.Threading.Tasks;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for caching font-related data to improve performance
/// </summary>
public interface ICacheService : IDisposable
{
    /// <summary>
    /// Gets or adds a typeface to the cache
    /// </summary>
    /// <param name="filePath">Font file path</param>
    /// <param name="factory">Factory function to create typeface if not cached</param>
    /// <returns>Cached or newly created typeface</returns>
    SKTypeface? GetOrAddTypeface(string filePath, Func<SKTypeface?> factory);

    /// <summary>
    /// Gets or adds a thumbnail bitmap to the cache
    /// </summary>
    /// <param name="key">Cache key (typically file path + size)</param>
    /// <param name="factory">Factory function to create bitmap if not cached</param>
    /// <returns>Cached or newly created bitmap</returns>
    Task<SKBitmap?> GetOrAddThumbnailAsync(string key, Func<Task<SKBitmap?>> factory);

    /// <summary>
    /// Gets or adds font metadata to the cache
    /// </summary>
    /// <param name="filePath">Font file path</param>
    /// <param name="factory">Factory function to create metadata if not cached</param>
    /// <returns>Cached or newly loaded metadata</returns>
    Task<FontMetadata?> GetOrAddMetadataAsync(string filePath, Func<Task<FontMetadata?>> factory);

    /// <summary>
    /// Invalidates cache entry for a specific file
    /// </summary>
    /// <param name="filePath">Font file path</param>
    void InvalidateFile(string filePath);

    /// <summary>
    /// Clears all caches
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    CacheStatistics GetStatistics();
}

/// <summary>
/// Cache hit/miss statistics
/// </summary>
public class CacheStatistics
{
    public int TypefaceHits { get; set; }
    public int TypefaceMisses { get; set; }
    public int ThumbnailHits { get; set; }
    public int ThumbnailMisses { get; set; }
    public int MetadataHits { get; set; }
    public int MetadataMisses { get; set; }
    public int TypefaceCacheSize { get; set; }
    public int ThumbnailCacheSize { get; set; }
    public int MetadataCacheSize { get; set; }
}
