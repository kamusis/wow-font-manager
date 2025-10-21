using SkiaSharp;

namespace WowFontManager.Models;

/// <summary>
/// Configuration for font preview rendering
/// </summary>
public class PreviewConfiguration
{
    /// <summary>
    /// Path to the font file being previewed
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Text to render in the preview
    /// </summary>
    public required string SampleText { get; set; }

    /// <summary>
    /// Font size in points
    /// </summary>
    public float FontSize { get; set; } = 24.0f;

    /// <summary>
    /// Selected writing system/script
    /// </summary>
    public Script Script { get; set; } = Script.Latin;

    /// <summary>
    /// Whether to display baseline and metrics overlay
    /// </summary>
    public bool ShowMetrics { get; set; } = false;

    /// <summary>
    /// Enable anti-aliasing
    /// </summary>
    public bool AntiAliasing { get; set; } = true;

    /// <summary>
    /// Enable subpixel rendering
    /// </summary>
    public bool SubpixelRendering { get; set; } = true;

    /// <summary>
    /// Background color for preview
    /// </summary>
    public SKColor BackgroundColor { get; set; } = SKColors.White;

    /// <summary>
    /// Text color for preview
    /// </summary>
    public SKColor TextColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Width of the preview canvas in pixels
    /// </summary>
    public int Width { get; set; } = 800;

    /// <summary>
    /// Height of the preview canvas in pixels
    /// </summary>
    public int Height { get; set; } = 200;
}

/// <summary>
/// Writing system/script types
/// </summary>
public enum Script
{
    Latin,
    ChineseSimplified,
    ChineseTraditional,
    Japanese,
    Korean,
    Numbers,
    Symbols,
    Mixed
}
