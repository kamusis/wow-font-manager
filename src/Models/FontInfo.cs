namespace WowFontManager.Models;

/// <summary>
/// Simplified font information for UI display in the font browser
/// </summary>
public class FontInfo
{
    /// <summary>
    /// Absolute path to the font file
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// Primary font family name (e.g., "Arial", "Microsoft YaHei")
    /// </summary>
    public required string FamilyName { get; set; }

    /// <summary>
    /// Style variant (e.g., "Regular", "Bold", "Italic")
    /// </summary>
    public string SubfamilyName { get; set; } = "Regular";

    /// <summary>
    /// File name with extension
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Folder path relative to fonts root (e.g., "enUS", "zhCN")
    /// </summary>
    public string? FolderPath { get; set; }

    /// <summary>
    /// Display name for the font (combines family and subfamily)
    /// </summary>
    public string DisplayName => $"{FamilyName} {SubfamilyName}".Trim();
}
