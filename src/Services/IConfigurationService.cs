using System.Collections.Generic;
using System.Threading.Tasks;

namespace WowFontManager.Services;

/// <summary>
/// Service for managing application configuration and settings
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Loads application settings from storage
    /// </summary>
    Task<AppSettings> LoadSettingsAsync();

    /// <summary>
    /// Saves application settings to storage
    /// </summary>
    /// <param name="settings">Settings to save</param>
    Task SaveSettingsAsync(AppSettings settings);

    /// <summary>
    /// Gets the current settings (cached)
    /// </summary>
    AppSettings GetSettings();

    /// <summary>
    /// Updates a specific setting and auto-saves
    /// </summary>
    Task UpdateSettingAsync<T>(string key, T value);
}

/// <summary>
/// Application settings
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Last directory browsed for fonts
    /// </summary>
    public string? LastDirectory { get; set; }

    /// <summary>
    /// Recently accessed directories
    /// </summary>
    public List<string> RecentDirectories { get; set; } = new();

    /// <summary>
    /// Maximum number of recent directories to remember
    /// </summary>
    public int MaxRecentDirectories { get; set; } = 10;

    /// <summary>
    /// Default preview font size
    /// </summary>
    public float PreviewSize { get; set; } = 24.0f;

    /// <summary>
    /// Window width
    /// </summary>
    public int WindowWidth { get; set; } = 1200;

    /// <summary>
    /// Window height
    /// </summary>
    public int WindowHeight { get; set; } = 800;

    /// <summary>
    /// Whether window is maximized
    /// </summary>
    public bool WindowMaximized { get; set; } = false;

    /// <summary>
    /// Application theme (Light/Dark)
    /// </summary>
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// Last selected WoW client path
    /// </summary>
    public string? LastWoWClientPath { get; set; }

    /// <summary>
    /// Enable auto-detect WoW clients on startup
    /// </summary>
    public bool AutoDetectClients { get; set; } = true;

    /// <summary>
    /// Show preview metrics overlay
    /// </summary>
    public bool ShowPreviewMetrics { get; set; } = false;

    /// <summary>
    /// Enable anti-aliasing in previews
    /// </summary>
    public bool EnableAntiAliasing { get; set; } = true;

    /// <summary>
    /// Enable subpixel rendering
    /// </summary>
    public bool EnableSubpixelRendering { get; set; } = true;
}
