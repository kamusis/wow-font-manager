using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for extracting font metadata using SkiaSharp
/// </summary>
public class FontMetadataService : IFontMetadataService
{
    /// <inheritdoc/>
    public async Task<FontMetadata?> LoadFontAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            using var typeface = SKTypeface.FromFile(filePath);
            if (typeface == null)
                return null;

            var fileInfo = new FileInfo(filePath);
            
            // Get font metrics using SKFont
            using var font = typeface.ToFont();
            var fontMetrics = font.Metrics;

            var metadata = new FontMetadata
            {
                FilePath = filePath,
                FullName = typeface.FamilyName,
                FontFamily = typeface.FamilyName,
                FontSubfamily = GetFontStyle(typeface),
                Format = DetermineFormat(Path.GetExtension(filePath)),
                UnitsPerEm = typeface.UnitsPerEm,
                Ascent = (int)fontMetrics.Ascent,
                Descent = (int)fontMetrics.Descent,
                LineGap = (int)fontMetrics.Leading,
                GlyphCount = typeface.GlyphCount,
                FileSize = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime
            };

            return await Task.FromResult(metadata);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetFontFamiliesAsync(string filePath)
    {
        var families = new List<string>();

        if (!File.Exists(filePath))
            return families;

        try
        {
            // For TTC files, we need to enumerate through font indices
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension == ".ttc")
            {
                // Try multiple indices (TTC files typically have 1-10 fonts)
                for (int index = 0; index < 20; index++)
                {
                    using var typeface = SKTypeface.FromFile(filePath, index);
                    if (typeface == null)
                        break;

                    var familyName = typeface.FamilyName;
                    if (!string.IsNullOrEmpty(familyName) && !families.Contains(familyName))
                    {
                        families.Add(familyName);
                    }
                }
            }
            else
            {
                using var typeface = SKTypeface.FromFile(filePath);
                if (typeface != null && !string.IsNullOrEmpty(typeface.FamilyName))
                {
                    families.Add(typeface.FamilyName);
                }
            }
        }
        catch (Exception)
        {
            // Return empty list on error
        }

        return await Task.FromResult(families);
    }

    /// <inheritdoc/>
    public async Task<List<UnicodeRange>> DetectCoverageAsync(string filePath)
    {
        var ranges = new List<UnicodeRange>();

        if (!File.Exists(filePath))
            return ranges;

        try
        {
            using var typeface = SKTypeface.FromFile(filePath);
            if (typeface == null)
                return ranges;

            // Define common Unicode blocks to check
            var unicodeBlocks = GetCommonUnicodeBlocks();

            foreach (var block in unicodeBlocks)
            {
                var supportedGlyphs = 0;
                var sampleSize = Math.Min(100, block.EndCodePoint - block.StartCodePoint + 1);
                var step = Math.Max(1, (block.EndCodePoint - block.StartCodePoint + 1) / sampleSize);

                for (int codePoint = block.StartCodePoint; codePoint <= block.EndCodePoint; codePoint += step)
                {
                    var glyphId = typeface.GetGlyph(codePoint);
                    if (glyphId != 0) // 0 means glyph not found
                    {
                        supportedGlyphs++;
                    }
                }

                if (supportedGlyphs > 0)
                {
                    var range = new UnicodeRange
                    {
                        BlockName = block.BlockName,
                        StartCodePoint = block.StartCodePoint,
                        EndCodePoint = block.EndCodePoint,
                        SupportedGlyphs = (int)((supportedGlyphs / (double)sampleSize) * (block.EndCodePoint - block.StartCodePoint + 1))
                    };
                    ranges.Add(range);
                }
            }
        }
        catch (Exception)
        {
            // Return partial results on error
        }

        return await Task.FromResult(ranges);
    }

    /// <inheritdoc/>
    public async Task<FontValidationResult> ValidateFontAsync(string filePath)
    {
        var result = new FontValidationResult();

        if (!File.Exists(filePath))
        {
            result.Errors.Add("File does not exist");
            return result;
        }

        try
        {
            using var typeface = SKTypeface.FromFile(filePath);
            if (typeface == null)
            {
                result.Errors.Add("Failed to load font file");
                return result;
            }

            result.IsValid = true;

            // Check WoW compatibility (prefers TrueType)
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            result.IsCompatibleWithWoW = extension == ".ttf" || extension == ".otf";

            if (!result.IsCompatibleWithWoW)
            {
                result.Warnings.Add($"Font format '{extension}' may not be compatible with WoW. TrueType (.ttf) is recommended.");
            }

            // Check if font has glyphs
            if (typeface.GlyphCount == 0)
            {
                result.Warnings.Add("Font contains no glyphs");
            }

            // Check for basic Latin support
            var hasBasicLatin = typeface.GetGlyph('A') != 0 && typeface.GetGlyph('a') != 0;
            if (!hasBasicLatin)
            {
                result.Warnings.Add("Font may not support basic Latin characters");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Validation error: {ex.Message}");
        }

        return await Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<FontFileEntry> EnrichFontEntryAsync(FontFileEntry entry)
    {
        try
        {
            var metadata = await LoadFontAsync(entry.FilePath);
            if (metadata != null)
            {
                entry.FontFamily = metadata.FontFamily;
                entry.FontSubfamily = metadata.FontSubfamily;
                entry.FullName = metadata.FullName;
                entry.PostScriptName = metadata.PostScriptName;
                entry.GlyphCount = metadata.GlyphCount;
                entry.IsValid = true;

                // Get coverage information
                var coverage = await DetectCoverageAsync(entry.FilePath);
                entry.CoverageRanges = coverage;
            }
            else
            {
                entry.IsValid = false;
            }
        }
        catch (Exception)
        {
            entry.IsValid = false;
        }

        return entry;
    }

    #region Private Helper Methods

    private string GetFontStyle(SKTypeface typeface)
    {
        var style = typeface.FontStyle;
        var parts = new List<string>();

        if (style.Weight >= (int)SKFontStyleWeight.Bold)
            parts.Add("Bold");
        if (style.Slant != SKFontStyleSlant.Upright)
            parts.Add("Italic");
        if (style.Width != (int)SKFontStyleWidth.Normal)
            parts.Add(style.Width.ToString());

        return parts.Count > 0 ? string.Join(" ", parts) : "Regular";
    }

    private FontFormat DetermineFormat(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".ttf" => FontFormat.TrueType,
            ".otf" => FontFormat.OpenType,
            ".ttc" => FontFormat.Collection,
            ".woff" => FontFormat.Woff,
            ".woff2" => FontFormat.Woff2,
            _ => FontFormat.Unknown
        };
    }

    private List<UnicodeRange> GetCommonUnicodeBlocks()
    {
        return new List<UnicodeRange>
        {
            new() { BlockName = "Basic Latin", StartCodePoint = 0x0020, EndCodePoint = 0x007F },
            new() { BlockName = "Latin-1 Supplement", StartCodePoint = 0x0080, EndCodePoint = 0x00FF },
            new() { BlockName = "Latin Extended-A", StartCodePoint = 0x0100, EndCodePoint = 0x017F },
            new() { BlockName = "CJK Unified Ideographs", StartCodePoint = 0x4E00, EndCodePoint = 0x9FFF },
            new() { BlockName = "CJK Extension A", StartCodePoint = 0x3400, EndCodePoint = 0x4DBF },
            new() { BlockName = "Hiragana", StartCodePoint = 0x3040, EndCodePoint = 0x309F },
            new() { BlockName = "Katakana", StartCodePoint = 0x30A0, EndCodePoint = 0x30FF },
            new() { BlockName = "Hangul Syllables", StartCodePoint = 0xAC00, EndCodePoint = 0xD7AF },
            new() { BlockName = "Cyrillic", StartCodePoint = 0x0400, EndCodePoint = 0x04FF },
            new() { BlockName = "Greek and Coptic", StartCodePoint = 0x0370, EndCodePoint = 0x03FF }
        };
    }

    #endregion
}
