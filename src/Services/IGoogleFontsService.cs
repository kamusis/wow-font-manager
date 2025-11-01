using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for integrating with Google Fonts API
/// </summary>
public interface IGoogleFontsService
{
    /// <summary>
    /// Retrieves complete font list from cache or API
    /// </summary>
    /// <param name="includeCache">If true, returns cached data if valid; if false, forces API call</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all available Google Fonts</returns>
    Task<List<GoogleFontFamily>> GetAllFontsAsync(bool includeCache = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters fonts by character subset
    /// </summary>
    /// <param name="subset">Subset name (e.g., "latin", "chinese-simplified")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of fonts supporting the specified subset</returns>
    Task<List<GoogleFontFamily>> GetFontsBySubsetAsync(string subset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters fonts by WoW locale mapping
    /// </summary>
    /// <param name="locale">WoW locale (e.g., "enUS", "zhCN", "zhTW")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of fonts supporting the specified locale</returns>
    Task<List<GoogleFontFamily>> GetFontsByLocaleAsync(string locale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a font to the appropriate subfolder
    /// </summary>
    /// <param name="fontFamily">Font family to download</param>
    /// <param name="variant">Variant to download (e.g., "regular", "bold")</param>
    /// <param name="progress">Progress reporter (0-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the downloaded font file</returns>
    Task<string> DownloadFontAsync(GoogleFontFamily fontFamily, string variant, IProgress<int>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces cache refresh from API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies API key validity by making a test API call
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if API key is valid, false otherwise</returns>
    Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines the target folder for a font based on its subsets
    /// </summary>
    /// <param name="fontFamily">Font family to check</param>
    /// <returns>Target folder name (e.g., "enUS", "zhCN")</returns>
    string DetermineTargetFolder(GoogleFontFamily fontFamily);
}
