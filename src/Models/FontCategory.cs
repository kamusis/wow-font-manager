namespace WowFontManager.Models;

/// <summary>
/// Represents WoW font usage categories
/// </summary>
public enum FontCategory
{
    /// <summary>
    /// All UI fonts (applies to all categories)
    /// </summary>
    All,

    /// <summary>
    /// Main UI fonts (menus, tooltips, general UI)
    /// </summary>
    MainUI,

    /// <summary>
    /// Chat window fonts
    /// </summary>
    Chat,

    /// <summary>
    /// Damage number fonts (floating combat text)
    /// </summary>
    Damage
}
