using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WowFontManager.Models;

namespace WowFontManager.Services;

/// <summary>
/// Implementation of Google Fonts API integration service
/// </summary>
public class GoogleFontsService : IGoogleFontsService
{
    private readonly IConfigurationService _configurationService;
    private readonly HttpClient _httpClient;
    private readonly string _cacheDirectory;
    private readonly string _cacheFilePath;
    private readonly string _fontsDirectory;

    // Locale to subset mapping
    private static readonly Dictionary<string, string[]> LocaleToSubsets = new()
    {
        { "enUS", new[] { "latin", "latin-ext" } },
        { "zhCN", new[] { "chinese-simplified", "latin", "latin-ext" } },
        { "zhTW", new[] { "chinese-traditional", "latin", "latin-ext" } },
        { "jaJP", new[] { "japanese", "latin", "latin-ext" } },
        { "koKR", new[] { "korean", "latin", "latin-ext" } }
    };

    // Subset priority for folder determination (CJK takes precedence over latin)
    private static readonly string[] SubsetPriority = new[]
    {
        "chinese-simplified",
        "chinese-traditional",
        "chinese-hongkong",
        "japanese",
        "korean",
        "latin",
        "latin-ext"
    };

    public GoogleFontsService(IConfigurationService configurationService, HttpClient? httpClient = null)
    {
        _configurationService = configurationService;
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // Set up cache directory
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _cacheDirectory = Path.Combine(localAppData, "WowFontManager");
        _cacheFilePath = Path.Combine(_cacheDirectory, "google-fonts-cache.json");

        // Set up fonts directory
        _fontsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fonts");

        // Ensure directories exist
        Directory.CreateDirectory(_cacheDirectory);
        Directory.CreateDirectory(_fontsDirectory);
    }

    public async Task<List<GoogleFontFamily>> GetAllFontsAsync(bool includeCache = true, CancellationToken cancellationToken = default)
    {
        if (includeCache)
        {
            var cache = await LoadCacheAsync(cancellationToken);
            if (cache != null && cache.IsValid)
            {
                // Background refresh if needed (fire and forget)
                if (cache.ShouldBackgroundRefresh)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await FetchFromApiAsync(CancellationToken.None);
                        }
                        catch
                        {
                            // Silently fail background refresh
                        }
                    }, CancellationToken.None);
                }

                return cache.Fonts;
            }
        }

        // Fetch fresh data from API
        return await FetchFromApiAsync(cancellationToken);
    }

    public async Task<List<GoogleFontFamily>> GetFontsBySubsetAsync(string subset, CancellationToken cancellationToken = default)
    {
        var allFonts = await GetAllFontsAsync(includeCache: true, cancellationToken);
        return allFonts.Where(f => f.Subsets.Contains(subset, StringComparer.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<GoogleFontFamily>> GetFontsByLocaleAsync(string locale, CancellationToken cancellationToken = default)
    {
        if (!LocaleToSubsets.TryGetValue(locale, out var subsets))
        {
            return new List<GoogleFontFamily>();
        }

        var allFonts = await GetAllFontsAsync(includeCache: true, cancellationToken);

        // Return fonts that support any of the locale's subsets
        return allFonts.Where(f => f.Subsets.Any(s => subsets.Contains(s, StringComparer.OrdinalIgnoreCase))).ToList();
    }

    public async Task<string> DownloadFontAsync(GoogleFontFamily fontFamily, string variant, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        if (!fontFamily.Files.TryGetValue(variant, out var downloadUrl))
        {
            throw new InvalidOperationException($"Variant '{variant}' not found for font '{fontFamily.Family}'");
        }

        // Determine target folder
        var targetLocale = DetermineTargetFolder(fontFamily);
        var targetDir = Path.Combine(_fontsDirectory, targetLocale);
        Directory.CreateDirectory(targetDir);

        // Generate file name
        var fileName = $"{fontFamily.Family.Replace(" ", "")}-{VariantToFileName(variant)}.ttf";
        var targetPath = Path.Combine(targetDir, fileName);

        // Handle file name collision
        if (File.Exists(targetPath))
        {
            var counter = 1;
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            while (File.Exists(targetPath))
            {
                fileName = $"{fileNameWithoutExt} ({counter}).ttf";
                targetPath = Path.Combine(targetDir, fileName);
                counter++;
            }
        }

        // Download file with progress reporting
        progress?.Report(0);

        using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        var canReportProgress = totalBytes != -1;

        using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            totalRead += bytesRead;

            if (canReportProgress)
            {
                var progressPercentage = (int)((totalRead * 100) / totalBytes);
                progress?.Report(progressPercentage);
            }
        }

        progress?.Report(100);

        // Validate file size
        var fileInfo = new FileInfo(targetPath);
        if (fileInfo.Length == 0)
        {
            File.Delete(targetPath);
            throw new IOException("Downloaded file is empty");
        }

        // Validate file size threshold (50MB)
        if (fileInfo.Length > 50 * 1024 * 1024)
        {
            File.Delete(targetPath);
            throw new IOException("Downloaded file exceeds size threshold (50MB)");
        }

        return targetPath;
    }

    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await FetchFromApiAsync(cancellationToken);
    }

    public async Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = _configurationService.GetSettings();
            if (string.IsNullOrWhiteSpace(settings.GoogleFontsApiKey))
            {
                return false;
            }

            var url = $"https://www.googleapis.com/webfonts/v1/webfonts?key={settings.GoogleFontsApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public string DetermineTargetFolder(GoogleFontFamily fontFamily)
    {
        // Find the highest priority subset that the font supports
        foreach (var subset in SubsetPriority)
        {
            if (fontFamily.Subsets.Contains(subset, StringComparer.OrdinalIgnoreCase))
            {
                // Map subset to locale
                foreach (var kvp in LocaleToSubsets)
                {
                    if (kvp.Value.Contains(subset, StringComparer.OrdinalIgnoreCase))
                    {
                        return kvp.Key;
                    }
                }
            }
        }

        // Default to enUS if no match found
        return "enUS";
    }

    private async Task<GoogleFontsCache?> LoadCacheAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_cacheFilePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_cacheFilePath, cancellationToken);
            var cache = JsonSerializer.Deserialize<GoogleFontsCache>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cache;
        }
        catch
        {
            // Cache corrupted, delete it
            try
            {
                File.Delete(_cacheFilePath);
            }
            catch
            {
                // Ignore deletion errors
            }

            return null;
        }
    }

    private async Task<List<GoogleFontFamily>> FetchFromApiAsync(CancellationToken cancellationToken)
    {
        var settings = _configurationService.GetSettings();
        if (string.IsNullOrWhiteSpace(settings.GoogleFontsApiKey))
        {
            throw new InvalidOperationException("Google Fonts API key not configured");
        }

        var url = $"https://www.googleapis.com/webfonts/v1/webfonts?key={settings.GoogleFontsApiKey}";

        // Retry logic with exponential backoff
        var retryDelays = new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) };
        Exception? lastException = null;

        for (int attempt = 0; attempt < retryDelays.Length + 1; attempt++)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonSerializer.Deserialize<GoogleFontsApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null || apiResponse.Items == null)
                {
                    throw new InvalidOperationException("Invalid API response");
                }

                // Save to cache
                var cache = new GoogleFontsCache
                {
                    Fonts = apiResponse.Items,
                    LastCacheUpdate = DateTime.Now,
                    CacheExpiryHours = settings.GoogleFontsCacheExpiryHours
                };

                await SaveCacheAsync(cache, cancellationToken);

                // Update last refresh time in settings
                await _configurationService.UpdateSettingAsync("GoogleFontsLastRefresh", DateTime.Now);

                return apiResponse.Items;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("Invalid Google Fonts API key", ex);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new InvalidOperationException("Rate limit exceeded, please try again later", ex);
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt < retryDelays.Length)
                {
                    await Task.Delay(retryDelays[attempt], cancellationToken);
                }
            }
        }

        throw new InvalidOperationException("Unable to connect to Google Fonts API after multiple retries", lastException);
    }

    private async Task SaveCacheAsync(GoogleFontsCache cache, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(cache, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_cacheFilePath, json, cancellationToken);
        }
        catch
        {
            // Silently fail cache save - not critical
        }
    }

    private static string VariantToFileName(string variant)
    {
        // Convert variant name to filename-friendly format
        return variant switch
        {
            "regular" => "Regular",
            "italic" => "Italic",
            "bold" => "Bold",
            "bolditalic" => "BoldItalic",
            _ => char.ToUpper(variant[0]) + variant[1..] // Capitalize first letter
        };
    }

    // Helper class for deserializing API response
    private class GoogleFontsApiResponse
    {
        public List<GoogleFontFamily>? Items { get; set; }
    }
}
