namespace WowFontManager.Models;

/// <summary>
/// Tracks font download operations
/// </summary>
public class FontDownloadInfo
{
    /// <summary>
    /// Name of the font family being downloaded
    /// </summary>
    public required string FontFamily { get; set; }

    /// <summary>
    /// Specific variant being downloaded (e.g., "regular", "bold")
    /// </summary>
    public required string Variant { get; set; }

    /// <summary>
    /// Source URL for the font download
    /// </summary>
    public required string DownloadUrl { get; set; }

    /// <summary>
    /// Local file system destination path
    /// </summary>
    public required string TargetPath { get; set; }

    /// <summary>
    /// Primary subset for folder organization (e.g., "chinese-simplified", "latin")
    /// </summary>
    public string Subset { get; set; } = string.Empty;

    /// <summary>
    /// Download progress percentage (0-100)
    /// </summary>
    public int Progress { get; set; } = 0;
}
