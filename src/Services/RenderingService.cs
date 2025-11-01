using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of font rendering service using SkiaSharp
/// </summary>
public class RenderingService : IRenderingService
{
    private static readonly Dictionary<string, string> DefaultSampleTexts = new()
    {
        { "enUS", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\nThe quick brown fox jumps over the lazy dog.\nWorld of Warcraft Font Manager\nEnglish Test Text" },
        { "zhCN", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\n临兵斗者皆阵列在前\n魔兽世界字体管理器\n简体中文测试文本" },
        { "zhTW", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\n臨兵闘者皆陣列在前\n魔獸世界字體管理器\n繁體中文測試文本" },
        { "jaJP", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\n臨兵闘者皆陣列在前\nワールド・オブ・ウォークラフト\n日本語テストテキスト" },
        { "koKR", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\n재빠른 갈색 여우가 게으른 개를 뛰어넘습니다.\n월드 오브 워크래프트\n한국어 테스트 텍스트" },
        { "ruRU", "\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n.;,' \"(!?) +-*/=\n\nБыстрая коричневая лиса перепрыгивает через ленивую собаку.\nWorld of Warcraft\nРусский тестовый текст" }
    };

    /// <inheritdoc/>
    public async Task<SKBitmap?> RenderPreviewAsync(PreviewConfiguration config, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(config.FilePath) || !File.Exists(config.FilePath))
                return null;

            using var typeface = SKTypeface.FromFile(config.FilePath);
            if (typeface == null)
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            // Apply DPI scaling for high-resolution displays
            var dpiScale = config.DpiScale;
            var scaledFontSize = config.FontSize * dpiScale;

            // Create paint with anti-aliasing and subpixel rendering
            using var paint = new SKPaint
            {
                Typeface = typeface,
                TextSize = scaledFontSize,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                Color = config.TextColor,
                Style = SKPaintStyle.Fill
            };

            // Measure text to determine canvas size
            var textBounds = MeasureText(config.SampleText, typeface, scaledFontSize);
            var canvasWidth = (int)Math.Max(Math.Ceiling(textBounds.Width) + 40 * dpiScale, config.Width * dpiScale);
            var canvasHeight = (int)Math.Max(Math.Ceiling(textBounds.Height) + 40 * dpiScale, config.Height * dpiScale);

            // Create bitmap and canvas with DPI-scaled dimensions
            var bitmap = new SKBitmap(canvasWidth, canvasHeight);
            using var canvas = new SKCanvas(bitmap);

            // Fill background
            canvas.Clear(config.BackgroundColor);

            // Calculate starting position (centered) with DPI scaling
            var x = 20f * dpiScale;
            var y = (20f - textBounds.Top) * dpiScale;

            // Render text with layout
            RenderTextLayout(canvas, config.SampleText, typeface, scaledFontSize, new SKPoint(x, y), paint);

            return bitmap;
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public SKRect MeasureText(string text, SKTypeface typeface, float fontSize)
    {
        using var paint = new SKPaint
        {
            Typeface = typeface,
            TextSize = fontSize,
            IsAntialias = true,
            SubpixelText = true
        };

        var bounds = new SKRect();
        
        // Split text into lines and measure each
        var lines = text.Split('\n');
        float totalWidth = 0;
        float totalHeight = 0;
        float lineHeight = paint.FontSpacing;

        foreach (var line in lines)
        {
            var lineBounds = new SKRect();
            paint.MeasureText(line, ref lineBounds);
            totalWidth = Math.Max(totalWidth, lineBounds.Width);
            totalHeight += lineHeight;
        }

        bounds.Size = new SKSize(totalWidth, totalHeight);
        return bounds;
    }

    /// <inheritdoc/>
    public void RenderTextLayout(SKCanvas canvas, string text, SKTypeface typeface, float fontSize, SKPoint position, SKPaint paint)
    {
        var lines = text.Split('\n');
        var lineHeight = paint.FontSpacing;
        var currentY = position.Y;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                currentY += lineHeight;
                continue;
            }

            // Render the line
            canvas.DrawText(line, position.X, currentY, paint);
            currentY += lineHeight;
        }
    }

    /// <inheritdoc/>
    public string GetDefaultSampleText(string locale)
    {
        if (DefaultSampleTexts.TryGetValue(locale, out var sampleText))
            return sampleText;

        // Fallback to English
        return DefaultSampleTexts["enUS"];
    }
}
