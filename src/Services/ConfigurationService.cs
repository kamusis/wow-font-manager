using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of configuration service with platform-specific paths
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly string _settingsPath;
    private AppSettings _cachedSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigurationService()
    {
        _settingsPath = GetSettingsFilePath();
        _cachedSettings = new AppSettings();
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        // Ensure settings directory exists
        var directory = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <inheritdoc/>
    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                if (settings != null)
                {
                    _cachedSettings = settings;
                    return settings;
                }
            }
        }
        catch
        {
            // If loading fails, return default settings
        }

        _cachedSettings = new AppSettings();
        return _cachedSettings;
    }

    /// <inheritdoc/>
    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsPath, json);
            _cachedSettings = settings;
        }
        catch
        {
            // Ignore save errors
        }
    }

    /// <inheritdoc/>
    public AppSettings GetSettings()
    {
        return _cachedSettings;
    }

    /// <inheritdoc/>
    public async Task UpdateSettingAsync<T>(string key, T value)
    {
        var settings = GetSettings();
        var property = typeof(AppSettings).GetProperty(key);
        
        if (property != null && property.CanWrite)
        {
            property.SetValue(settings, value);
            await SaveSettingsAsync(settings);
        }
    }

    /// <summary>
    /// Gets platform-specific settings file path
    /// </summary>
    private static string GetSettingsFilePath()
    {
        string basePath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows: %LOCALAPPDATA%\WowFontManager
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS: ~/Library/Application Support/WowFontManager
            basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library",
                "Application Support");
        }
        else
        {
            // Linux: ~/.config/WowFontManager
            var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (string.IsNullOrEmpty(configHome))
            {
                configHome = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config");
            }
            basePath = configHome;
        }

        return Path.Combine(basePath, "WowFontManager", "settings.json");
    }
}
