using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for rendering font previews with SkiaSharp
/// </summary>
public interface IRenderingService
{
    /// <summary>
    /// Renders a font preview asynchronously
    /// </summary>
    /// <param name="config">Preview configuration including font, text, size, etc.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered image as SKBitmap</returns>
    Task<SKBitmap?> RenderPreviewAsync(PreviewConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Measures the dimensions of rendered text
    /// </summary>
    /// <param name="text">Text to measure</param>
    /// <param name="typeface">Font typeface</param>
    /// <param name="fontSize">Font size in points</param>
    /// <returns>Measured text bounds</returns>
    SKRect MeasureText(string text, SKTypeface typeface, float fontSize);

    /// <summary>
    /// Renders text with advanced layout options
    /// </summary>
    /// <param name="canvas">Canvas to draw on</param>
    /// <param name="text">Text to render</param>
    /// <param name="typeface">Font typeface</param>
    /// <param name="fontSize">Font size in points</param>
    /// <param name="position">Drawing position</param>
    /// <param name="paint">Paint configuration</param>
    void RenderTextLayout(SKCanvas canvas, string text, SKTypeface typeface, float fontSize, SKPoint position, SKPaint paint);

    /// <summary>
    /// Gets default sample text for a given locale
    /// </summary>
    /// <param name="locale">WoW locale (enUS, zhCN, zhTW, koKR, ruRU)</param>
    /// <returns>Sample text string</returns>
    string GetDefaultSampleText(string locale);
}
