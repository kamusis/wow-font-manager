using System;

namespace WowFontManager.Models;

/// <summary>
/// Represents a World of Warcraft client installation configuration
/// </summary>
public class WoWClientConfiguration
{
    /// <summary>
    /// Type of WoW client (Retail, Classic, Classic Era)
    /// </summary>
    public required WoWClientType ClientType { get; set; }

    /// <summary>
    /// Root directory of WoW installation
    /// </summary>
    public required string InstallationPath { get; set; }

    /// <summary>
    /// Full path to the Fonts directory
    /// </summary>
    public required string FontsPath { get; set; }

    /// <summary>
    /// Client locale (e.g., enUS, zhCN, zhTW)
    /// </summary>
    public required string Locale { get; set; }

    /// <summary>
    /// Whether the Fonts directory is accessible and valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// WoW client version string (optional)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// User-friendly display name for the client
    /// </summary>
    public string DisplayName => $"WoW {ClientType} ({Locale})";

    /// <summary>
    /// Whether backups exist for this client
    /// </summary>
    public bool HasBackups { get; set; }

    /// <summary>
    /// Most recent backup timestamp
    /// </summary>
    public DateTime? LastBackupDate { get; set; }
}

/// <summary>
/// WoW client types
/// </summary>
public enum WoWClientType
{
    Retail,
    Classic,
    ClassicEra
}
