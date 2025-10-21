using System.Collections.Generic;

namespace WowFontManager.Models;

/// <summary>
/// Represents a font replacement operation
/// </summary>
public class FontReplacementOperation
{
    /// <summary>
    /// Path to the font file being used for replacement
    /// </summary>
    public required string SourceFontPath { get; set; }

    /// <summary>
    /// Target WoW client configuration
    /// </summary>
    public required WoWClientConfiguration TargetClient { get; set; }

    /// <summary>
    /// Selected replacement categories
    /// </summary>
    public required List<ReplacementCategory> Categories { get; set; }

    /// <summary>
    /// Calculated list of target files to replace
    /// </summary>
    public List<string> TargetFiles { get; set; } = new();

    /// <summary>
    /// Directory where backups will be stored
    /// </summary>
    public string? BackupPath { get; set; }

    /// <summary>
    /// Operation status
    /// </summary>
    public OperationStatus Status { get; set; }

    /// <summary>
    /// Number of files successfully replaced
    /// </summary>
    public int ReplacedCount { get; set; }

    /// <summary>
    /// Any errors encountered during the operation
    /// </summary>
    public List<string> ErrorMessages { get; set; } = new();
}

/// <summary>
/// Font replacement categories for WoW
/// </summary>
public enum ReplacementCategory
{
    All,
    MainUI,
    Chat,
    Damage
}

/// <summary>
/// Operation status
/// </summary>
public enum OperationStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    PartiallyCompleted
}
