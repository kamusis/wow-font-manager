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
        { "enUS", "The quick brown fox jumps over the lazy dog.\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n0123456789" },
        { "zhCN", "快速的棕色狐狸跳过懒狗。\n魔兽世界字体管理器\n简体中文测试文本" },
        { "zhTW", "快速的棕色狐狸跳過懶狗。\n魔獸世界字體管理器\n繁體中文測試文本" },
        { "koKR", "재빠른 갈색 여우가 게으른 개를 뛰어넘습니다.\n월드 오브 워크래프트\n한국어 테스트 텍스트" },
        { "ruRU", "Быстрая коричневая лиса перепрыгивает через ленивую собаку.\nWorld of Warcraft\nРусский тестовый текст" },
        { "jaJP", "素早い茶色のキツネが怠け者の犬を飛び越えます。\nワールド・オブ・ウォークラフト\n日本語テストテキスト" }
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

            // Create paint with anti-aliasing and subpixel rendering
            using var paint = new SKPaint
            {
                Typeface = typeface,
                TextSize = config.FontSize,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                Color = config.TextColor,
                Style = SKPaintStyle.Fill
            };

            // Measure text to determine canvas size
            var textBounds = MeasureText(config.SampleText, typeface, config.FontSize);
            var canvasWidth = Math.Max((int)Math.Ceiling(textBounds.Width) + 40, config.Width);
            var canvasHeight = Math.Max((int)Math.Ceiling(textBounds.Height) + 40, config.Height);

            // Create bitmap and canvas
            var bitmap = new SKBitmap(canvasWidth, canvasHeight);
            using var canvas = new SKCanvas(bitmap);

            // Fill background
            canvas.Clear(config.BackgroundColor);

            // Calculate starting position (centered)
            var x = 20f;
            var y = 20f - textBounds.Top;

            // Render text with layout
            RenderTextLayout(canvas, config.SampleText, typeface, config.FontSize, new SKPoint(x, y), paint);

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
