using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for managing WoW configuration and fixed installation path
/// </summary>
public class WoWConfigurationService
{
    // Fixed WoW installation path for this version
    private const string FixedWoWInstallPath = @"d:\Games\World of Warcraft\_retail_\";
    private const string DefaultLocale = "enUS";
    
    private readonly IWoWClientService _woWClientService;

    public WoWConfigurationService(IWoWClientService woWClientService)
    {
        _woWClientService = woWClientService;
    }

    /// <summary>
    /// Gets the fixed WoW installation path
    /// </summary>
    public string GetWoWInstallationPath() => FixedWoWInstallPath;

    /// <summary>
    /// Gets the Fonts folder path derived from WoW installation path
    /// </summary>
    public string GetFontsPath() => Path.Combine(FixedWoWInstallPath, "Fonts");

    /// <summary>
    /// Gets the Config.wtf path derived from WoW installation path
    /// </summary>
    public string GetConfigPath() => Path.Combine(FixedWoWInstallPath, "WTF", "Config.wtf");

    /// <summary>
    /// Detects the locale from Config.wtf file
    /// </summary>
    /// <returns>Detected locale or default enUS</returns>
    public async Task<LocaleDetectionResult> DetectLocaleAsync()
    {
        var configPath = GetConfigPath();
        
        if (!File.Exists(configPath))
        {
            return new LocaleDetectionResult
            {
                Locale = DefaultLocale,
                Success = false,
                Warning = $"Could not detect WoW locale, defaulting to {DefaultLocale}. Config file not found at: {configPath}"
            };
        }

        try
        {
            var lines = await File.ReadAllLinesAsync(configPath);
            
            // WoW uses "SET textLocale" in Config.wtf
            var localeLine = lines.FirstOrDefault(l => l.Contains("SET textLocale", StringComparison.OrdinalIgnoreCase));
            
            if (localeLine == null)
            {
                return new LocaleDetectionResult
                {
                    Locale = DefaultLocale,
                    Success = false,
                    Warning = $"Could not detect WoW locale, defaulting to {DefaultLocale}. Locale setting not found in Config.wtf"
                };
            }

            // Parse: SET textLocale "zhCN"
            var match = Regex.Match(localeLine, @"SET textLocale ""(\w+)""", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var detectedLocale = match.Groups[1].Value;
                
                // Validate that this locale is supported
                if (IsSupportedLocale(detectedLocale))
                {
                    return new LocaleDetectionResult
                    {
                        Locale = detectedLocale,
                        Success = true
                    };
                }
                else
                {
                    return new LocaleDetectionResult
                    {
                        Locale = DefaultLocale,
                        Success = false,
                        Warning = $"Detected unsupported locale '{detectedLocale}', defaulting to {DefaultLocale}"
                    };
                }
            }

            return new LocaleDetectionResult
            {
                Locale = DefaultLocale,
                Success = false,
                Warning = $"Could not parse locale from Config.wtf, defaulting to {DefaultLocale}"
            };
        }
        catch (Exception ex)
        {
            return new LocaleDetectionResult
            {
                Locale = DefaultLocale,
                Success = false,
                Warning = $"Error reading Config.wtf: {ex.Message}. Defaulting to {DefaultLocale}"
            };
        }
    }

    /// <summary>
    /// Validates that the fixed WoW installation exists
    /// </summary>
    public ValidationResult ValidateInstallation()
    {
        var result = new ValidationResult { IsValid = true };
        
        var installPath = GetWoWInstallationPath();
        if (!Directory.Exists(installPath))
        {
            result.IsValid = false;
            result.Errors.Add($"WoW installation not found at: {installPath}");
            return result;
        }

        var fontsPath = GetFontsPath();
        if (!Directory.Exists(fontsPath))
        {
            result.IsValid = false;
            result.Errors.Add($"WoW Fonts folder not found at: {fontsPath}");
            return result;
        }

        return result;
    }

    /// <summary>
    /// Creates a WoWClientConfiguration for the fixed installation
    /// </summary>
    public async Task<WoWClientConfiguration> GetClientConfigurationAsync()
    {
        var localeResult = await DetectLocaleAsync();
        
        return new WoWClientConfiguration
        {
            ClientType = WoWClientType.Retail,
            InstallationPath = GetWoWInstallationPath(),
            FontsPath = GetFontsPath(),
            Locale = localeResult.Locale,
            IsValid = ValidateInstallation().IsValid
        };
    }

    /// <summary>
    /// Checks if a locale is supported
    /// </summary>
    private bool IsSupportedLocale(string locale)
    {
        var supportedLocales = new[] { "enUS", "zhCN", "zhTW", "koKR", "ruRU", "jaJP" };
        return supportedLocales.Contains(locale, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Result of locale detection
/// </summary>
public class LocaleDetectionResult
{
    public required string Locale { get; set; }
    public bool Success { get; set; }
    public string? Warning { get; set; }
}
