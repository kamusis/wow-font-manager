using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for mapping font files to WoW usage categories using FontMappings.json
/// </summary>
public class FontCategoryService : IFontCategoryService
{
    private readonly Dictionary<string, LocaleFontMappings> _fontMappings;

    public FontCategoryService()
    {
        _fontMappings = LoadFontMappings();
    }

    /// <inheritdoc/>
    public IReadOnlyList<FontCategory> GetCategoriesForFont(string fileName, string locale)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(locale))
        {
            return Array.Empty<FontCategory>();
        }

        if (!_fontMappings.TryGetValue(locale, out var localeMappings))
        {
            return Array.Empty<FontCategory>();
        }

        var categories = new List<FontCategory>();

        if (localeMappings.MainUI?.Contains(fileName, StringComparer.OrdinalIgnoreCase) == true)
        {
            categories.Add(FontCategory.MainUI);
        }

        if (localeMappings.Chat?.Contains(fileName, StringComparer.OrdinalIgnoreCase) == true)
        {
            categories.Add(FontCategory.Chat);
        }

        if (localeMappings.Damage?.Contains(fileName, StringComparer.OrdinalIgnoreCase) == true)
        {
            categories.Add(FontCategory.Damage);
        }

        // If font is in any specific category, it's also in "All"
        if (categories.Count > 0 || localeMappings.All?.Contains(fileName, StringComparer.OrdinalIgnoreCase) == true)
        {
            categories.Add(FontCategory.All);
        }

        return categories;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetFontsForCategory(FontCategory category, string locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
        {
            return Array.Empty<string>();
        }

        if (!_fontMappings.TryGetValue(locale, out var localeMappings))
        {
            return Array.Empty<string>();
        }

        return category switch
        {
            FontCategory.All => (IReadOnlyList<string>)(localeMappings.All ?? new List<string>()),
            FontCategory.MainUI => (IReadOnlyList<string>)(localeMappings.MainUI ?? new List<string>()),
            FontCategory.Chat => (IReadOnlyList<string>)(localeMappings.Chat ?? new List<string>()),
            FontCategory.Damage => (IReadOnlyList<string>)(localeMappings.Damage ?? new List<string>()),
            _ => Array.Empty<string>()
        };
    }

    /// <inheritdoc/>
    public bool IsFontInCategory(string fileName, FontCategory category, string locale)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(locale))
        {
            return false;
        }

        var fonts = GetFontsForCategory(category, locale);
        return fonts.Contains(fileName, StringComparer.OrdinalIgnoreCase);
    }

    private Dictionary<string, LocaleFontMappings> LoadFontMappings()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "WowFontManager.Resources.FontMappings.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return new Dictionary<string, LocaleFontMappings>();
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var mappings = JsonSerializer.Deserialize<Dictionary<string, LocaleFontMappings>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return mappings ?? new Dictionary<string, LocaleFontMappings>();
        }
        catch
        {
            return new Dictionary<string, LocaleFontMappings>();
        }
    }

    private class LocaleFontMappings
    {
        public List<string>? All { get; set; }
        public List<string>? MainUI { get; set; }
        public List<string>? Chat { get; set; }
        public List<string>? Damage { get; set; }
    }
}
