using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for detecting and managing WoW client installations
/// </summary>
public class WoWClientService : IWoWClientService
{
    private readonly Dictionary<string, Dictionary<string, List<string>>> _fontMappings;

    public WoWClientService()
    {
        _fontMappings = LoadFontMappings();
    }

    /// <inheritdoc/>
    public async Task<List<WoWClientConfiguration>> DetectClientsAsync()
    {
        var clients = new List<WoWClientConfiguration>();
        var searchPaths = GetSearchPaths();

        foreach (var basePath in searchPaths)
        {
            if (!Directory.Exists(basePath))
                continue;

            // Check for each client type
            await TryDetectClient(clients, basePath, "_retail_", WoWClientType.Retail);
            await TryDetectClient(clients, basePath, "_classic_", WoWClientType.Classic);
            await TryDetectClient(clients, basePath, "_classic_era_", WoWClientType.ClassicEra);
        }

        return clients;
    }

    /// <inheritdoc/>
    public async Task<WoWClientConfiguration?> ValidateClientPathAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            return null;

        // Try to detect client type from path
        var clientType = DetectClientTypeFromPath(path);
        if (clientType == null)
            return null;

        var fontsPath = Path.Combine(path, "Fonts");
        if (!Directory.Exists(fontsPath))
            return null;

        var locale = await GetClientLocaleAsync(Path.GetDirectoryName(path) ?? path);

        return new WoWClientConfiguration
        {
            ClientType = clientType.Value,
            InstallationPath = path,
            FontsPath = fontsPath,
            Locale = locale,
            IsValid = true,
            HasBackups = CheckForBackups(fontsPath)
        };
    }

    /// <inheritdoc/>
    public List<string> GetFontMappingForLocale(string locale, ReplacementCategory category)
    {
        // Normalize locale
        var normalizedLocale = locale;
        if (!_fontMappings.ContainsKey(normalizedLocale))
        {
            // Try fallback to enUS
            normalizedLocale = "enUS";
        }

        if (!_fontMappings.ContainsKey(normalizedLocale))
            return new List<string>();

        var categoryKey = category.ToString();
        if (_fontMappings[normalizedLocale].TryGetValue(categoryKey, out var fonts))
        {
            return fonts.ToList();
        }

        return new List<string>();
    }

    /// <inheritdoc/>
    public async Task<string> GetClientLocaleAsync(string clientPath)
    {
        var configPath = Path.Combine(clientPath, "WTF", "Config.wtf");
        
        if (!File.Exists(configPath))
        {
            // Return system default or enUS as fallback
            return GetSystemLocale();
        }

        try
        {
            var content = await File.ReadAllTextAsync(configPath);
            var match = Regex.Match(content, @"SET\s+textLocale\s+""([^""]+)""", RegexOptions.IgnoreCase);
            
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }
        catch (Exception)
        {
            // If reading fails, return fallback
        }

        return GetSystemLocale();
    }

    /// <inheritdoc/>
    public bool IsWoWRunning(string clientPath)
    {
        var executableNames = new[] { "Wow.exe", "WowClassic.exe", "Wow-64.exe", "World of Warcraft" };

        try
        {
            var processes = Process.GetProcesses();
            
            foreach (var process in processes)
            {
                try
                {
                    if (executableNames.Any(name => 
                        process.ProcessName.Contains(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Check if the process is running from this client path
                        var processPath = process.MainModule?.FileName;
                        if (!string.IsNullOrEmpty(processPath) && 
                            processPath.Contains(clientPath, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Access denied or process exited, continue
                    continue;
                }
            }
        }
        catch (Exception)
        {
            // If we can't enumerate processes, assume not running
        }

        return false;
    }

    /// <inheritdoc/>
    public string GetClientDisplayName(WoWClientType clientType)
    {
        return clientType switch
        {
            WoWClientType.Retail => "World of Warcraft (Retail)",
            WoWClientType.Classic => "World of Warcraft Classic",
            WoWClientType.ClassicEra => "World of Warcraft Classic Era",
            _ => "Unknown"
        };
    }

    #region Private Helper Methods

    private List<string> GetSearchPaths()
    {
        var paths = new List<string>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows paths
            paths.Add(@"C:\Program Files (x86)\World of Warcraft");
            paths.Add(@"C:\Program Files\World of Warcraft");
            
            // Try to find Battle.net installation
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var battleNetPath = Path.Combine(programFiles, "Battle.net", "World of Warcraft");
            if (Directory.Exists(battleNetPath))
                paths.Add(battleNetPath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS paths
            paths.Add("/Applications/World of Warcraft");
            paths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                "Applications", "World of Warcraft"));
        }

        return paths;
    }

    private async Task TryDetectClient(List<WoWClientConfiguration> clients, string basePath, 
        string subFolder, WoWClientType clientType)
    {
        var clientPath = Path.Combine(basePath, subFolder);
        var client = await ValidateClientPathAsync(clientPath);
        
        if (client != null)
        {
            clients.Add(client);
        }
    }

    private WoWClientType? DetectClientTypeFromPath(string path)
    {
        var dirName = Path.GetFileName(path);
        
        return dirName?.ToLowerInvariant() switch
        {
            "_retail_" => WoWClientType.Retail,
            "_classic_" => WoWClientType.Classic,
            "_classic_era_" => WoWClientType.ClassicEra,
            _ => null
        };
    }

    private bool CheckForBackups(string fontsPath)
    {
        try
        {
            var backupDirs = Directory.GetDirectories(fontsPath, "FontBackup_*");
            return backupDirs.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private string GetSystemLocale()
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        
        // Map to WoW locale format
        return culture.Name switch
        {
            "zh-CN" => "zhCN",
            "zh-TW" => "zhTW",
            "ko-KR" => "koKR",
            "ru-RU" => "ruRU",
            "de-DE" => "deDE",
            "es-ES" => "esES",
            "fr-FR" => "frFR",
            "it-IT" => "itIT",
            "pt-BR" => "ptBR",
            _ => "enUS"
        };
    }

    private Dictionary<string, Dictionary<string, List<string>>> LoadFontMappings()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "WowFontManager.Resources.FontMappings.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                // Return default empty mapping
                return new Dictionary<string, Dictionary<string, List<string>>>();
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            
            var mappings = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(json);
            return mappings ?? new Dictionary<string, Dictionary<string, List<string>>>();
        }
        catch (Exception)
        {
            // Return empty mapping on error
            return new Dictionary<string, Dictionary<string, List<string>>>();
        }
    }

    #endregion
}
