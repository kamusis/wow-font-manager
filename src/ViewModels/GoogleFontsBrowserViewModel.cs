using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WowFontManager.Models;
using WowFontManager.Services;

namespace WowFontManager.ViewModels;

/// <summary>
/// ViewModel for the Google Fonts browser UI
/// </summary>
public partial class GoogleFontsBrowserViewModel : ViewModelBase
{
    private readonly IGoogleFontsService _googleFontsService;
    private readonly IFontPreviewService _fontPreviewService;
    private readonly IConfigurationService _configurationService;
    private CancellationTokenSource? _cancellationTokenSource;
    private List<GoogleFontFamily> _allFonts = new();
    private List<GoogleFontFamily> _filteredFonts = new();

    [ObservableProperty]
    private ObservableCollection<GoogleFontFamily> _googleFonts = new();

    [ObservableProperty]
    private GoogleFontFamily? _selectedGoogleFont;

    [ObservableProperty]
    private string _selectedVariant = "regular";

    [ObservableProperty]
    private bool _isLoadingFonts;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private int _downloadProgress;

    [ObservableProperty]
    private string _statusMessage = "No fonts loaded";

    [ObservableProperty]
    private string _selectedLocale = "enUS";

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private ObservableCollection<string> _availableLocales = new();

    [ObservableProperty]
    private ObservableCollection<string> _availableCategories = new();

    [ObservableProperty]
    private ObservableCollection<string> _availableVariants = new();

    public GoogleFontsBrowserViewModel(
        IGoogleFontsService googleFontsService,
        IFontPreviewService fontPreviewService,
        IConfigurationService configurationService)
    {
        _googleFontsService = googleFontsService;
        _fontPreviewService = fontPreviewService;
        _configurationService = configurationService;

        // Initialize available locales
        AvailableLocales = new ObservableCollection<string>(new[] { "All", "enUS", "zhCN", "zhTW", "jaJP", "koKR" });

        // Initialize available categories
        AvailableCategories = new ObservableCollection<string>(new[] { "All", "sans-serif", "serif", "monospace", "display", "handwriting" });

        // Load saved default locale
        var settings = _configurationService.GetSettings();
        if (!string.IsNullOrEmpty(settings.GoogleFontsDefaultLocale))
        {
            SelectedLocale = settings.GoogleFontsDefaultLocale;
        }
    }

    /// <summary>
    /// Loads Google Fonts from cache or API
    /// </summary>
    [RelayCommand]
    private async Task LoadGoogleFontsAsync()
    {
        // Cancel any ongoing operation
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            IsLoadingFonts = true;
            StatusMessage = "Loading Google Fonts...";
            GoogleFonts.Clear();

            // Check if API key is configured
            var settings = _configurationService.GetSettings();
            if (string.IsNullOrWhiteSpace(settings.GoogleFontsApiKey))
            {
                StatusMessage = "Google Fonts API key not configured. Please configure it in the Local Fonts tab.";
                return;
            }

            // Load fonts from cache or API
            _allFonts = await _googleFontsService.GetAllFontsAsync(includeCache: true, _cancellationTokenSource.Token);

            // Apply filters
            ApplyFilters();

            var fontCount = GoogleFonts.Count;
            StatusMessage = $"Loaded {fontCount} font{(fontCount != 1 ? "s" : "")} for {SelectedLocale}";
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("API key"))
        {
            StatusMessage = "Invalid Google Fonts API key. Please update it in the Local Fonts tab.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Font loading cancelled";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading fonts: {ex.Message}";
        }
        finally
        {
            IsLoadingFonts = false;
        }
    }

    /// <summary>
    /// Forces refresh from API
    /// </summary>
    [RelayCommand]
    private async Task RefreshGoogleFontsAsync()
    {
        // Cancel any ongoing operation
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            IsLoadingFonts = true;
            StatusMessage = "Refreshing Google Fonts...";

            // Check if API key is configured
            var settings = _configurationService.GetSettings();
            if (string.IsNullOrWhiteSpace(settings.GoogleFontsApiKey))
            {
                StatusMessage = "Google Fonts API key not configured. Please configure it in the Local Fonts tab.";
                return;
            }

            // Force refresh from API
            await _googleFontsService.RefreshCacheAsync(_cancellationTokenSource.Token);

            // Reload fonts
            _allFonts = await _googleFontsService.GetAllFontsAsync(includeCache: true, _cancellationTokenSource.Token);

            // Apply filters
            ApplyFilters();

            var fontCount = GoogleFonts.Count;
            StatusMessage = $"Font list updated ({fontCount} fonts available)";
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("API key"))
        {
            StatusMessage = "Invalid Google Fonts API key. Please update it in the Local Fonts tab.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Font refresh cancelled";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error refreshing fonts: {ex.Message}";
        }
        finally
        {
            IsLoadingFonts = false;
        }
    }

    /// <summary>
    /// Downloads the selected font variant
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDownloadFont))]
    private async Task DownloadFontAsync()
    {
        if (SelectedGoogleFont == null)
        {
            return;
        }

        try
        {
            IsDownloading = true;
            DownloadProgress = 0;
            StatusMessage = $"Downloading {SelectedGoogleFont.Family} - {SelectedVariant}...";

            var progress = new Progress<int>(percent =>
            {
                DownloadProgress = percent;
                StatusMessage = $"Downloading {SelectedGoogleFont.Family} - {SelectedVariant}... {percent}%";
            });

            var downloadedPath = await _googleFontsService.DownloadFontAsync(
                SelectedGoogleFont,
                SelectedVariant,
                progress,
                CancellationToken.None);

            // Get the folder name from the path
            var folder = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(downloadedPath));
            StatusMessage = $"Font downloaded to fonts/{folder}/";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Download failed: {ex.Message}";
        }
        finally
        {
            IsDownloading = false;
            DownloadProgress = 0;
            DownloadFontCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanDownloadFont()
    {
        return SelectedGoogleFont != null && !string.IsNullOrEmpty(SelectedVariant) && !IsDownloading;
    }

    /// <summary>
    /// Called when selected font changes
    /// </summary>
    partial void OnSelectedGoogleFontChanged(GoogleFontFamily? value)
    {
        if (value != null)
        {
            // Determine variants source: prefer Variants; fall back to Files keys
            var variantsSource = (value.Variants != null && value.Variants.Count > 0)
                ? value.Variants
                : (value.Files?.Keys?.ToList() ?? new List<string>());

            // Update available variants
            AvailableVariants = new ObservableCollection<string>(variantsSource);

            // Reset first to force UI re-selection
            SelectedVariant = string.Empty;

            // Prefer 'regular' when available; otherwise select the first variant
            if (variantsSource.Contains("regular", StringComparer.OrdinalIgnoreCase))
            {
                SelectedVariant = variantsSource.First(v => v.Equals("regular", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                SelectedVariant = variantsSource.FirstOrDefault() ?? "regular";
            }
        }
        else
        {
            AvailableVariants.Clear();
        }

        DownloadFontCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when selected locale changes
    /// </summary>
    partial void OnSelectedLocaleChanged(string value)
    {
        ApplyFilters();

        // Save preference
        _ = _configurationService.UpdateSettingAsync("GoogleFontsDefaultLocale", value);
    }

    /// <summary>
    /// Called when selected category changes
    /// </summary>
    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilters();
    }

    /// <summary>
    /// Applies locale and category filters to the font list
    /// </summary>
    private void ApplyFilters()
    {
        if (_allFonts.Count == 0)
        {
            GoogleFonts.Clear();
            return;
        }

        // Start with all fonts
        _filteredFonts = new List<GoogleFontFamily>(_allFonts);

        // Apply locale filter
        if (SelectedLocale != "All")
        {
            var localeSubsets = GetSubsetsForLocale(SelectedLocale);
            _filteredFonts = _filteredFonts.Where(f => f.Subsets.Any(s => localeSubsets.Contains(s, StringComparer.OrdinalIgnoreCase))).ToList();
        }

        // Apply category filter
        if (SelectedCategory != "All")
        {
            _filteredFonts = _filteredFonts.Where(f => f.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Update observable collection
        GoogleFonts.Clear();
        foreach (var font in _filteredFonts.OrderBy(f => f.Family))
        {
            GoogleFonts.Add(font);
        }

        // Update status
        var fontCount = GoogleFonts.Count;
        if (!IsLoadingFonts)
        {
            StatusMessage = $"Showing {fontCount} font{(fontCount != 1 ? "s" : "")} for {SelectedLocale}";
        }
    }

    private static string[] GetSubsetsForLocale(string locale)
    {
        return locale switch
        {
            // Keep latin + latin-ext for enUS
            "enUS" => new[] { "latin", "latin-ext" },
            // For CJK locales, filter strictly by the primary subset
            "zhCN" => new[] { "chinese-simplified" },
            "zhTW" => new[] { "chinese-traditional" },
            "jaJP" => new[] { "japanese" },
            "koKR" => new[] { "korean" },
            _ => Array.Empty<string>()
        };
    }
}
