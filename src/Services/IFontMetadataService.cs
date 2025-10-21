using System.Collections.Generic;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for extracting and analyzing font metadata using SkiaSharp
/// </summary>
public interface IFontMetadataService
{
    /// <summary>
    /// Load font and extract complete metadata
    /// </summary>
    /// <param name="filePath">Path to font file</param>
    /// <returns>Font metadata or null if failed</returns>
    Task<FontMetadata?> LoadFontAsync(string filePath);

    /// <summary>
    /// Get list of font families in a font file (for TTC files)
    /// </summary>
    /// <param name="filePath">Path to font file</param>
    /// <returns>List of font family names</returns>
    Task<List<string>> GetFontFamiliesAsync(string filePath);

    /// <summary>
    /// Detect Unicode character coverage ranges
    /// </summary>
    /// <param name="filePath">Path to font file</param>
    /// <returns>List of Unicode ranges with coverage information</returns>
    Task<List<UnicodeRange>> DetectCoverageAsync(string filePath);

    /// <summary>
    /// Validate font file integrity and WoW compatibility
    /// </summary>
    /// <param name="filePath">Path to font file</param>
    /// <returns>Validation result with compatibility status</returns>
    Task<FontValidationResult> ValidateFontAsync(string filePath);

    /// <summary>
    /// Update existing FontFileEntry with detailed metadata
    /// </summary>
    /// <param name="entry">Font file entry to update</param>
    /// <returns>Updated entry with metadata</returns>
    Task<FontFileEntry> EnrichFontEntryAsync(FontFileEntry entry);
}

/// <summary>
/// Result of font validation
/// </summary>
public class FontValidationResult
{
    public bool IsValid { get; set; }
    public bool IsCompatibleWithWoW { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
