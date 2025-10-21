using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for replacing WoW fonts with backup and restore capabilities
/// </summary>
public interface IFontReplacementService
{
    /// <summary>
    /// Replaces WoW fonts with the specified source font
    /// </summary>
    /// <param name="operation">Font replacement operation details</param>
    /// <param name="progress">Progress reporter (0-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Replacement result with details</returns>
    Task<FontReplacementResult> ReplaceFontAsync(
        FontReplacementOperation operation,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores fonts from a backup
    /// </summary>
    /// <param name="client">WoW client configuration</param>
    /// <param name="backupInfo">Backup to restore</param>
    /// <param name="progress">Progress reporter (0-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restore result with details</returns>
    Task<RestoreResult> RestoreBackupAsync(
        WoWClientConfiguration client,
        BackupInfo backupInfo,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all available backups for a WoW client
    /// </summary>
    /// <param name="client">WoW client configuration</param>
    /// <returns>List of available backups</returns>
    Task<List<BackupInfo>> ListBackupsAsync(WoWClientConfiguration client);

    /// <summary>
    /// Deletes old backups, keeping only the most recent ones
    /// </summary>
    /// <param name="client">WoW client configuration</param>
    /// <param name="keepCount">Number of recent backups to keep (default: 5)</param>
    /// <returns>Number of backups deleted</returns>
    Task<int> CleanupOldBackupsAsync(WoWClientConfiguration client, int keepCount = 5);

    /// <summary>
    /// Validates that font replacement can proceed
    /// </summary>
    /// <param name="operation">Font replacement operation</param>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateOperationAsync(FontReplacementOperation operation);
}

/// <summary>
/// Result of a font replacement operation
/// </summary>
public class FontReplacementResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public BackupInfo? BackupInfo { get; set; }
    public List<string> ReplacedFiles { get; set; } = new();
    public List<string> FailedFiles { get; set; } = new();
}

/// <summary>
/// Result of a backup restore operation
/// </summary>
public class RestoreResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> RestoredFiles { get; set; } = new();
    public List<string> FailedFiles { get; set; } = new();
}

/// <summary>
/// Result of operation validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
