using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of font preview service using SkiaSharp and Avalonia
/// </summary>
public class FontPreviewService : IFontPreviewService
{
    private readonly IRenderingService _renderingService;

    public FontPreviewService(IRenderingService renderingService)
    {
        _renderingService = renderingService;
    }

    /// <inheritdoc/>
    public async Task<Bitmap?> RenderPreviewAsync(FontInfo fontInfo, string? sampleText = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Determine sample text
            var text = sampleText ?? GetDefaultSampleText(fontInfo.FilePath);

            // Create preview configuration
            var config = new PreviewConfiguration
            {
                FilePath = fontInfo.FilePath,
                SampleText = text,
                FontSize = 32.0f,
                Width = 800,
                Height = 400,
                BackgroundColor = SKColors.White,
                TextColor = SKColors.Black,
                AntiAliasing = true,
                SubpixelRendering = true,
                DpiScale = 1.25f  // Hardcoded DPI scaling for 125% display scaling
            };

            // Render using the rendering service
            var skBitmap = await _renderingService.RenderPreviewAsync(config, cancellationToken);
            if (skBitmap == null)
                return null;

            // Convert SKBitmap to Avalonia Bitmap
            using (skBitmap)
            {
                return ConvertToAvaloniaBitmap(skBitmap);
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public string GetDefaultSampleText(string fontPath)
    {
        // Detect locale from path
        var pathLower = fontPath.ToLowerInvariant();

        if (pathLower.Contains("enus") || pathLower.Contains("en_us"))
            return _renderingService.GetDefaultSampleText("enUS");
        
        if (pathLower.Contains("zhcn") || pathLower.Contains("zh_cn"))
            return _renderingService.GetDefaultSampleText("zhCN");
        
        if (pathLower.Contains("zhtw") || pathLower.Contains("zh_tw"))
            return _renderingService.GetDefaultSampleText("zhTW");
        
        if (pathLower.Contains("kokr") || pathLower.Contains("ko_kr"))
            return _renderingService.GetDefaultSampleText("koKR");
        
        if (pathLower.Contains("ruru") || pathLower.Contains("ru_ru"))
            return _renderingService.GetDefaultSampleText("ruRU");
        
        if (pathLower.Contains("jajp") || pathLower.Contains("ja_jp"))
            return _renderingService.GetDefaultSampleText("jaJP");
        
        // Default to Simplified Chinese
        return _renderingService.GetDefaultSampleText("zhCN");
    }

    private Bitmap? ConvertToAvaloniaBitmap(SKBitmap skBitmap)
    {
        try
        {
            // Encode SKBitmap to PNG in memory
            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            
            data.SaveTo(stream);
            stream.Position = 0;
            
            // Create Avalonia Bitmap from stream
            return new Bitmap(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
