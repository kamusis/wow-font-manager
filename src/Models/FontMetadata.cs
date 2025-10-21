using System;
using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Detailed font metadata extracted from font files
/// </summary>
public class FontMetadata
{
    // Identification
    public required string FullName { get; set; }
    public required string FontFamily { get; set; }
    public required string FontSubfamily { get; set; }
    public string? PostScriptName { get; set; }

    // Technical Details
    public required FontFormat Format { get; set; }
    public string? Version { get; set; }
    public EmbeddingRights EmbeddingRights { get; set; }

    // Metrics
    public int UnitsPerEm { get; set; }
    public int Ascent { get; set; }
    public int Descent { get; set; }
    public int LineGap { get; set; }

    // Glyphs and Coverage
    public int GlyphCount { get; set; }
    public List<UnicodeRange> CoverageRanges { get; set; } = new();

    // Designer Information
    public string? Designer { get; set; }
    public string? Copyright { get; set; }
    public string? Trademark { get; set; }

    // File Information
    public long FileSize { get; set; }
    public required string FilePath { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Font embedding rights/permissions
/// </summary>
public enum EmbeddingRights
{
    Unknown,
    Installable,
    Editable,
    PreviewPrint,
    Restricted
}
