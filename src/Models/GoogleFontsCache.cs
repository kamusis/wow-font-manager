using System;
using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Manages local caching of Google Fonts API responses
/// </summary>
public class GoogleFontsCache
{
    /// <summary>
    /// Cached list of Google Font families
    /// </summary>
    public List<GoogleFontFamily> Fonts { get; set; } = new();

    /// <summary>
    /// Timestamp of last cache update
    /// </summary>
    public DateTime LastCacheUpdate { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Cache validity period in hours (default: 24)
    /// </summary>
    public int CacheExpiryHours { get; set; } = 24;

    /// <summary>
    /// Checks if the cache is still valid based on expiry time
    /// </summary>
    public bool IsValid => DateTime.Now - LastCacheUpdate < TimeSpan.FromHours(CacheExpiryHours);

    /// <summary>
    /// Checks if cache should trigger background refresh (older than 12 hours)
    /// </summary>
    public bool ShouldBackgroundRefresh => DateTime.Now - LastCacheUpdate > TimeSpan.FromHours(12);
}
