using System;
using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Information about a font backup
/// </summary>
public class BackupInfo
{
    /// <summary>
    /// When the backup was created
    /// </summary>
    public required DateTime BackupDate { get; set; }

    /// <summary>
    /// Path to the source font used for replacement
    /// </summary>
    public required string SourceFont { get; set; }

    /// <summary>
    /// List of original files that were backed up
    /// </summary>
    public required List<string> ReplacedFiles { get; set; }

    /// <summary>
    /// Client type at time of backup
    /// </summary>
    public required WoWClientType ClientType { get; set; }

    /// <summary>
    /// Client locale at time of backup
    /// </summary>
    public required string Locale { get; set; }

    /// <summary>
    /// Which categories were replaced
    /// </summary>
    public required List<ReplacementCategory> Categories { get; set; }

    /// <summary>
    /// Full path to the backup directory
    /// </summary>
    public required string BackupDirectory { get; set; }

    /// <summary>
    /// Display name for the backup
    /// </summary>
    public string DisplayName => $"{BackupDate:yyyy-MM-dd HH:mm:ss} - {string.Join(", ", Categories)}";
}
