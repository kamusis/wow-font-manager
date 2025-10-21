using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Service for discovering and scanning font files in directories
/// </summary>
public class FontDiscoveryService : IFontDiscoveryService
{
    private static readonly string[] SupportedExtensions = { ".ttf", ".otf", ".ttc", ".woff", ".woff2" };

    /// <inheritdoc/>
    public async IAsyncEnumerable<FontFileEntry> ScanDirectoryAsync(
        string directoryPath,
        bool recursive = true,
        IProgress<int>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(directoryPath))
            yield break;

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var allFiles = new List<string>();

        // Collect all font files first to calculate progress
        try
        {
            foreach (var ext in SupportedExtensions)
            {
                var files = Directory.EnumerateFiles(directoryPath, $"*{ext}", searchOption);
                allFiles.AddRange(files);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip inaccessible directories
            yield break;
        }
        catch (Exception)
        {
            // Skip on other errors
            yield break;
        }

        var totalFiles = allFiles.Count;
        var processedFiles = 0;

        foreach (var filePath in allFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entry = await CreateFontFileEntryAsync(filePath);
            if (entry != null)
            {
                yield return entry;
            }

            processedFiles++;
            if (progress != null && totalFiles > 0)
            {
                var percentage = (int)((processedFiles / (double)totalFiles) * 100);
                progress.Report(percentage);
            }
        }
    }

    /// <inheritdoc/>
    public List<string> GetSupportedExtensions()
    {
        return SupportedExtensions.ToList();
    }

    /// <inheritdoc/>
    public bool IsFontFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return SupportedExtensions.Contains(extension);
    }

    private async Task<FontFileEntry?> CreateFontFileEntryAsync(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                return null;

            var format = GetFontFormat(fileInfo.Extension);

            var entry = new FontFileEntry
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime,
                Format = format,
                IsValid = true, // Will be validated by metadata service
                IsCompatibleWithWoW = format == FontFormat.TrueType || format == FontFormat.OpenType
            };

            return await Task.FromResult(entry);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private FontFormat GetFontFormat(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".ttf" => FontFormat.TrueType,
            ".otf" => FontFormat.OpenType,
            ".ttc" => FontFormat.Collection,
            ".woff" => FontFormat.Woff,
            ".woff2" => FontFormat.Woff2,
            _ => FontFormat.Unknown
        };
    }
}
