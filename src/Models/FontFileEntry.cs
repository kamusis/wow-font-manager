using System;
using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Represents a discovered font file with metadata
/// </summary>
public class FontFileEntry
{
    /// <summary>
    /// Absolute path to the font file
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// File name with extension
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Primary font family name
    /// </summary>
    public string? FontFamily { get; set; }

    /// <summary>
    /// Style variant (Regular, Bold, Italic, etc.)
    /// </summary>
    public string? FontSubfamily { get; set; }

    /// <summary>
    /// Font format type
    /// </summary>
    public FontFormat Format { get; set; }

    /// <summary>
    /// Whether the font loaded successfully
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Total number of glyphs in the font
    /// </summary>
    public int GlyphCount { get; set; }

    /// <summary>
    /// Supported Unicode blocks for character coverage
    /// </summary>
    public List<UnicodeRange> CoverageRanges { get; set; } = new();

    /// <summary>
    /// Whether the font is suitable for WoW (TTF format check)
    /// </summary>
    public bool IsCompatibleWithWoW { get; set; }

    /// <summary>
    /// Full font name (for display purposes)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// PostScript name identifier
    /// </summary>
    public string? PostScriptName { get; set; }
}

/// <summary>
/// Font file format types
/// </summary>
public enum FontFormat
{
    Unknown,
    TrueType,
    OpenType,
    Collection,
    Woff,
    Woff2
}
