namespace WowFontManager.Models;

/// <summary>
/// Represents a Unicode character block range
/// </summary>
public class UnicodeRange
{
    /// <summary>
    /// Unicode block name (e.g., "CJK Unified Ideographs")
    /// </summary>
    public required string BlockName { get; set; }

    /// <summary>
    /// Starting code point (e.g., 0x4E00)
    /// </summary>
    public int StartCodePoint { get; set; }

    /// <summary>
    /// Ending code point (e.g., 0x9FFF)
    /// </summary>
    public int EndCodePoint { get; set; }

    /// <summary>
    /// Count of supported glyphs in this range
    /// </summary>
    public int SupportedGlyphs { get; set; }

    /// <summary>
    /// Percentage of block that is supported
    /// </summary>
    public float CoveragePercentage => 
        (float)SupportedGlyphs / (EndCodePoint - StartCodePoint + 1) * 100f;

    /// <summary>
    /// Total possible glyphs in this range
    /// </summary>
    public int TotalGlyphs => EndCodePoint - StartCodePoint + 1;
}
