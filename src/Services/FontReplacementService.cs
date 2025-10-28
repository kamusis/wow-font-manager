using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of font replacement service with backup and restore
/// </summary>
public class FontReplacementService : IFontReplacementService
{
    private readonly IWoWClientService _wowClientService;
    private const int MaxBackupsToKeep = 5;

    public FontReplacementService(IWoWClientService wowClientService)
    {
        _wowClientService = wowClientService;
    }

    /// <inheritdoc/>
    public async Task<FontReplacementResult> ReplaceFontAsync(
        FontReplacementOperation operation,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new FontReplacementResult();

        try
        {
            // Validate operation
            var validation = await ValidateOperationAsync(operation);
            if (!validation.IsValid)
            {
                result.Success = false;
                result.ErrorMessage = string.Join("; ", validation.Errors);
                return result;
            }

            progress?.Report(10);
            cancellationToken.ThrowIfCancellationRequested();

            // Get target font files based on all categories
            var allTargetFiles = new List<string>();
            foreach (var category in operation.Categories)
            {
                var files = _wowClientService.GetFontMappingForLocale(
                    operation.TargetClient.Locale,
                    category);
                if (files != null)
                {
                    allTargetFiles.AddRange(files);
                }
            }

            // Remove duplicates
            var targetFiles = allTargetFiles.Distinct().ToList();

            if (targetFiles.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = $"No font mappings found for locale {operation.TargetClient.Locale}";
                return result;
            }

            progress?.Report(20);
            cancellationToken.ThrowIfCancellationRequested();

            // Create backup for files that will be replaced
            var backupInfo = await CreateBackupAsync(operation.TargetClient, targetFiles);
            result.BackupInfo = backupInfo;

            progress?.Report(40);
            cancellationToken.ThrowIfCancellationRequested();

            // Replace fonts
            var totalFiles = targetFiles.Count;
            var processedFiles = 0;

            foreach (var targetFile in targetFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var targetPath = Path.Combine(operation.TargetClient.FontsPath, targetFile);
                
                try
                {
                    // Copy source font to target location
                    File.Copy(operation.SourceFontPath, targetPath, overwrite: true);
                    result.ReplacedFiles.Add(targetFile);
                }
                catch (Exception ex)
                {
                    result.FailedFiles.Add($"{targetFile}: {ex.Message}");
                }

                processedFiles++;
                var progressPercent = 40 + (int)((processedFiles / (double)totalFiles) * 60);
                progress?.Report(progressPercent);
            }

            // Save backup metadata
            await SaveBackupMetadataAsync(backupInfo, operation, result.ReplacedFiles);

            result.Success = result.FailedFiles.Count == 0;
            if (!result.Success)
            {
                result.ErrorMessage = $"Failed to replace {result.FailedFiles.Count} file(s)";
            }

            progress?.Report(100);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<RestoreResult> RestoreBackupAsync(
        WoWClientConfiguration client,
        BackupInfo backupInfo,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new RestoreResult();

        try
        {
            // Validate backup exists
            if (!Directory.Exists(backupInfo.BackupDirectory))
            {
                result.Success = false;
                result.ErrorMessage = "Backup directory not found";
                return result;
            }

            progress?.Report(10);
            cancellationToken.ThrowIfCancellationRequested();

            // Get all backup files
            var backupFiles = Directory.GetFiles(backupInfo.BackupDirectory);
            var totalFiles = backupFiles.Length;
            var processedFiles = 0;

            foreach (var backupFile in backupFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(backupFile);
                if (fileName == "backup.json")
                    continue; // Skip metadata file

                var targetPath = Path.Combine(client.FontsPath, fileName);

                try
                {
                    await Task.Run(() => File.Copy(backupFile, targetPath, overwrite: true), cancellationToken);
                    result.RestoredFiles.Add(fileName);
                }
                catch (Exception ex)
                {
                    result.FailedFiles.Add($"{fileName}: {ex.Message}");
                }

                processedFiles++;
                var progressPercent = 10 + (int)((processedFiles / (double)totalFiles) * 90);
                progress?.Report(progressPercent);
            }

            result.Success = result.FailedFiles.Count == 0;
            if (!result.Success)
            {
                result.ErrorMessage = $"Failed to restore {result.FailedFiles.Count} file(s)";
            }

            progress?.Report(100);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<BackupInfo>> ListBackupsAsync(WoWClientConfiguration client)
    {
        var backups = new List<BackupInfo>();
        
        // Check for backups in WoW installation directory (new location)
        var wowInstallPath = client.InstallationPath;
        if (Directory.Exists(wowInstallPath))
        {
            var wowBackupDirs = Directory.GetDirectories(wowInstallPath)
                .Where(d => Path.GetFileName(d).StartsWith("Fonts."))
                .OrderByDescending(d => Directory.GetCreationTime(d));

            foreach (var backupDir in wowBackupDirs)
            {
                var metadataPath = Path.Combine(backupDir, "backup.json");
                if (File.Exists(metadataPath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(metadataPath);
                        var backupInfo = JsonSerializer.Deserialize<BackupInfo>(json);
                        if (backupInfo != null)
                        {
                            backups.Add(backupInfo);
                        }
                    }
                    catch
                    {
                        // Ignore corrupted backup metadata
                    }
                }
                else
                {
                    // Try to create BackupInfo from directory
                    try
                    {
                        var dirInfo = new DirectoryInfo(backupDir);
                        var backupFiles = dirInfo.GetFiles("*.ttf").Concat(dirInfo.GetFiles("*.otf")).ToList();
                        if (backupFiles.Count > 0)
                        {
                            backups.Add(new BackupInfo
                            {
                                BackupDirectory = backupDir,
                                BackupDate = dirInfo.CreationTime,
                                SourceFont = "Unknown",
                                ReplacedFiles = backupFiles.Select(f => f.Name).ToList(),
                                ClientType = client.ClientType,
                                Locale = client.Locale,
                                Categories = new List<ReplacementCategory> { ReplacementCategory.All }
                            });
                        }
                    }
                    catch
                    {
                        // Ignore errors
                    }
                }
            }
        }
        
        // Also check old backup location (AppData) for backward compatibility
        var backupBaseDir = GetBackupBaseDirectory(client);
        if (Directory.Exists(backupBaseDir))
        {
            var backupDirs = Directory.GetDirectories(backupBaseDir)
                .Where(d => Path.GetFileName(d).StartsWith("FontBackup_"))
                .OrderByDescending(d => Directory.GetCreationTime(d));

            foreach (var backupDir in backupDirs)
            {
                var metadataPath = Path.Combine(backupDir, "backup.json");
                if (File.Exists(metadataPath))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(metadataPath);
                        var backupInfo = JsonSerializer.Deserialize<BackupInfo>(json);
                        if (backupInfo != null)
                        {
                            backups.Add(backupInfo);
                        }
                    }
                    catch
                    {
                        // Ignore corrupted backup metadata
                    }
                }
            }
        }

        return backups;
    }

    /// <inheritdoc/>
    public async Task<int> CleanupOldBackupsAsync(WoWClientConfiguration client, int keepCount = 5)
    {
        var backups = await ListBackupsAsync(client);
        var backupsToDelete = backups.Skip(keepCount).ToList();
        var deletedCount = 0;

        foreach (var backup in backupsToDelete)
        {
            try
            {
                if (Directory.Exists(backup.BackupDirectory))
                {
                    Directory.Delete(backup.BackupDirectory, recursive: true);
                    deletedCount++;
                }
            }
            catch
            {
                // Ignore deletion errors
            }
        }

        return deletedCount;
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateOperationAsync(FontReplacementOperation operation)
    {
        var result = new ValidationResult { IsValid = true };

        // Check if source font exists
        if (!File.Exists(operation.SourceFontPath))
        {
            result.IsValid = false;
            result.Errors.Add("Source font file does not exist");
        }

        // Check if WoW installation path is valid
        if (!Directory.Exists(operation.TargetClient.InstallationPath))
        {
            result.IsValid = false;
            result.Errors.Add("WoW installation directory does not exist");
        }

        // Check write permissions to WoW installation directory
        // Note: Fonts directory may not exist yet and will be created automatically
        try
        {
            var testDir = operation.TargetClient.FontsPath;
            
            // If Fonts directory doesn't exist, test write permission on parent directory
            if (!Directory.Exists(testDir))
            {
                testDir = operation.TargetClient.InstallationPath;
            }
            
            var testFile = Path.Combine(testDir, $".test_{Guid.NewGuid()}");
            await File.WriteAllTextAsync(testFile, "test");
            File.Delete(testFile);
        }
        catch
        {
            result.IsValid = false;
            result.Errors.Add("No write permission to WoW fonts directory");
        }

        // Check if WoW is running
        if (_wowClientService.IsWoWRunning(operation.TargetClient.InstallationPath))
        {
            result.IsValid = false;
            result.Errors.Add("World of Warcraft is currently running. Please close the game before replacing fonts.");
        }

        return result;
    }

    /// <summary>
    /// Creates a backup of specific font files that will be replaced
    /// </summary>
    private async Task<BackupInfo> CreateBackupAsync(WoWClientConfiguration client, List<string> targetFiles)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fontsPath = client.FontsPath;
        
        // If Fonts folder doesn't exist, create it
        if (!Directory.Exists(fontsPath))
        {
            Directory.CreateDirectory(fontsPath);
            
            return new BackupInfo
            {
                BackupDirectory = string.Empty,
                BackupDate = DateTime.Now,
                ClientType = client.ClientType,
                Locale = client.Locale,
                SourceFont = string.Empty,
                ReplacedFiles = new List<string>(),
                Categories = new List<ReplacementCategory>()
            };
        }

        // Determine backup folder name in WoW installation directory
        var wowInstallPath = client.InstallationPath;
        var backupFolderName = $"Fonts.{timestamp}";
        var backupDir = Path.Combine(wowInstallPath, backupFolderName);
        
        // Handle collision (unlikely but possible)
        var counter = 2;
        while (Directory.Exists(backupDir))
        {
            backupFolderName = $"Fonts.{timestamp}_{counter}";
            backupDir = Path.Combine(wowInstallPath, backupFolderName);
            counter++;
        }

        // Create backup directory
        Directory.CreateDirectory(backupDir);

        // Backup only the files that will be replaced
        var backedUpFiles = new List<string>();
        foreach (var targetFile in targetFiles)
        {
            var sourcePath = Path.Combine(fontsPath, targetFile);
            if (File.Exists(sourcePath))
            {
                var backupPath = Path.Combine(backupDir, targetFile);
                await Task.Run(() => File.Copy(sourcePath, backupPath, overwrite: false));
                backedUpFiles.Add(targetFile);
            }
        }

        var backupInfo = new BackupInfo
        {
            BackupDirectory = backupDir,
            BackupDate = DateTime.Now,
            ClientType = client.ClientType,
            Locale = client.Locale,
            SourceFont = string.Empty, // Will be set later
            ReplacedFiles = backedUpFiles,
            Categories = new List<ReplacementCategory>()
        };

        return backupInfo;
    }

    /// <summary>
    /// Saves backup metadata to JSON
    /// </summary>
    private async Task SaveBackupMetadataAsync(
        BackupInfo backupInfo,
        FontReplacementOperation operation,
        List<string> replacedFiles)
    {
        backupInfo.SourceFont = operation.SourceFontPath;
        backupInfo.Categories = operation.Categories;
        backupInfo.ReplacedFiles = replacedFiles;

        var metadataPath = Path.Combine(backupInfo.BackupDirectory, "backup.json");
        var json = JsonSerializer.Serialize(backupInfo, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(metadataPath, json);
    }

    /// <summary>
    /// Gets the base directory for backups
    /// </summary>
    private string GetBackupBaseDirectory(WoWClientConfiguration client)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var backupBase = Path.Combine(appData, "WowFontManager", "Backups", 
            $"{client.ClientType}_{client.Locale}");
        return backupBase;
    }
}
