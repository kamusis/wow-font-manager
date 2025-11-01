using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Represents a font family from Google Fonts API
/// </summary>
public class GoogleFontFamily
{
    /// <summary>
    /// Font family name (e.g., "Roboto", "Noto Sans SC")
    /// </summary>
    public required string Family { get; set; }

    /// <summary>
    /// Available font variants/styles (e.g., "regular", "bold", "italic", "700")
    /// </summary>
    public List<string> Variants { get; set; } = new();

    /// <summary>
    /// Supported character subsets (e.g., "latin", "chinese-simplified", "japanese")
    /// </summary>
    public List<string> Subsets { get; set; } = new();

    /// <summary>
    /// Font version identifier (e.g., "v6")
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// ISO date when the font was last modified (YYYY-MM-DD format)
    /// </summary>
    public string LastModified { get; set; } = string.Empty;

    /// <summary>
    /// Font classification: "sans-serif", "serif", "monospace", "display", or "handwriting"
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Map of variant names to download URLs (TTF format from fonts.gstatic.com)
    /// </summary>
    public Dictionary<string, string> Files { get; set; } = new();

    /// <summary>
    /// URL to a preview/menu font file for displaying the font name
    /// </summary>
    public string Menu { get; set; } = string.Empty;

    /// <summary>
    /// Display name for UI (same as Family)
    /// </summary>
    public string DisplayName => Family;
}
