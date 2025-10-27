using System.Collections.Generic;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for mapping font files to WoW usage categories
/// </summary>
public interface IFontCategoryService
{
    /// <summary>
    /// Gets the categories that a font file belongs to based on FontMappings.json
    /// </summary>
    /// <param name="fileName">The font file name (e.g., "FRIZQT__.ttf")</param>
    /// <param name="locale">The locale (e.g., "enUS", "zhCN", "zhTW")</param>
    /// <returns>List of categories the font belongs to</returns>
    IReadOnlyList<FontCategory> GetCategoriesForFont(string fileName, string locale);

    /// <summary>
    /// Gets all font file names for a specific category and locale
    /// </summary>
    /// <param name="category">The font category</param>
    /// <param name="locale">The locale (e.g., "enUS", "zhCN", "zhTW")</param>
    /// <returns>List of font file names in the category</returns>
    IReadOnlyList<string> GetFontsForCategory(FontCategory category, string locale);

    /// <summary>
    /// Checks if a font file belongs to a specific category
    /// </summary>
    /// <param name="fileName">The font file name</param>
    /// <param name="category">The category to check</param>
    /// <param name="locale">The locale</param>
    /// <returns>True if the font belongs to the category</returns>
    bool IsFontInCategory(string fileName, FontCategory category, string locale);
}
