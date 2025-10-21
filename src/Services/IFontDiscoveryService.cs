using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for discovering and scanning font files
/// </summary>
public interface IFontDiscoveryService
{
    /// <summary>
    /// Scan a directory for font files
    /// </summary>
    /// <param name="directoryPath">Directory to scan</param>
    /// <param name="recursive">Whether to scan subdirectories</param>
    /// <param name="progress">Progress reporter (0-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Observable stream of discovered fonts</returns>
    IAsyncEnumerable<FontFileEntry> ScanDirectoryAsync(
        string directoryPath, 
        bool recursive = true,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get supported font file extensions
    /// </summary>
    /// <returns>List of supported extensions (with dot)</returns>
    List<string> GetSupportedExtensions();

    /// <summary>
    /// Quick check if a file is a font file based on extension
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <returns>True if the extension is supported</returns>
    bool IsFontFile(string filePath);

    /// <summary>
    /// Get all fonts in a directory (non-streaming)
    /// </summary>
    /// <param name="directoryPath">Directory to scan</param>
    /// <returns>List of font entries</returns>
    Task<List<FontFileEntry>> GetFontsInDirectoryAsync(string directoryPath);
}
