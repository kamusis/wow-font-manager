using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for detecting and managing WoW client installations
/// </summary>
public interface IWoWClientService
{
    /// <summary>
    /// Auto-detect WoW installations on the system
    /// </summary>
    /// <returns>List of detected WoW client configurations</returns>
    Task<List<WoWClientConfiguration>> DetectClientsAsync();

    /// <summary>
    /// Validate and configure a WoW client from a given path
    /// </summary>
    /// <param name="path">Root directory path of WoW installation</param>
    /// <returns>Validated client configuration or null if invalid</returns>
    Task<WoWClientConfiguration?> ValidateClientPathAsync(string path);

    /// <summary>
    /// Get font file mapping for a specific locale and category
    /// </summary>
    /// <param name="locale">Client locale (e.g., enUS, zhCN, zhTW)</param>
    /// <param name="category">Replacement category</param>
    /// <returns>List of target font filenames to replace</returns>
    List<string> GetFontMappingForLocale(string locale, ReplacementCategory category);

    /// <summary>
    /// Read the client locale from WTF/Config.wtf
    /// </summary>
    /// <param name="clientPath">Root directory of WoW installation</param>
    /// <returns>Detected locale string or system default</returns>
    Task<string> GetClientLocaleAsync(string clientPath);

    /// <summary>
    /// Check if WoW is currently running
    /// </summary>
    /// <param name="clientPath">Root directory of WoW installation</param>
    /// <returns>True if WoW process is detected</returns>
    bool IsWoWRunning(string clientPath);

    /// <summary>
    /// Get the display name for a client type
    /// </summary>
    /// <param name="clientType">Client type</param>
    /// <returns>User-friendly display name</returns>
    string GetClientDisplayName(WoWClientType clientType);
}
