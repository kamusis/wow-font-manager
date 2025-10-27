using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for generating font preview images for the font browser UI
/// </summary>
public interface IFontPreviewService
{
    /// <summary>
    /// Renders a preview image for a font
    /// </summary>
    /// <param name="fontInfo">Font information</param>
    /// <param name="sampleText">Optional custom sample text (uses default if null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered preview as Avalonia Bitmap, or null if rendering fails</returns>
    Task<Bitmap?> RenderPreviewAsync(FontInfo fontInfo, string? sampleText = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets default sample text based on font file path (detects locale from path)
    /// </summary>
    /// <param name="fontPath">Path to font file</param>
    /// <returns>Appropriate sample text for the font's locale</returns>
    string GetDefaultSampleText(string fontPath);
}
