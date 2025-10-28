using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WowFontManager.Models;
using WowFontManager.Services;

namespace WowFontManager.ViewModels;

/// <summary>
/// ViewModel for the font browser UI
/// </summary>
public partial class FontBrowserViewModel : ViewModelBase
{
    private readonly IFontDiscoveryService _fontDiscoveryService;
    private readonly IFontMetadataService _fontMetadataService;
    private readonly IFontPreviewService _fontPreviewService;
    private readonly IFontCategoryService _fontCategoryService;
    private readonly IFontReplacementService _fontReplacementService;
    private readonly IWoWClientService _wowClientService;
    private readonly WoWConfigurationService _wowConfigurationService;
    private readonly IConfigurationService _configurationService;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private ObservableCollection<FontTreeNode> _fontTree = new();

    [ObservableProperty]
    private object? _selectedItem;

    private FontInfo? _selectedFont;

    [ObservableProperty]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "No fonts loaded";

    [ObservableProperty]
    private string _wowInstallPath = string.Empty;

    public FontBrowserViewModel(
        IFontDiscoveryService fontDiscoveryService,
        IFontMetadataService fontMetadataService,
        IFontPreviewService fontPreviewService,
        IFontCategoryService fontCategoryService,
        IFontReplacementService fontReplacementService,
        IWoWClientService wowClientService,
        WoWConfigurationService wowConfigurationService,
        IConfigurationService configurationService)
    {
        _fontDiscoveryService = fontDiscoveryService;
        _fontMetadataService = fontMetadataService;
        _fontPreviewService = fontPreviewService;
        _fontCategoryService = fontCategoryService;
        _fontReplacementService = fontReplacementService;
        _wowClientService = wowClientService;
        _wowConfigurationService = wowConfigurationService;
        _configurationService = configurationService;
        
        // Initialize WoW installation path
        WowInstallPath = _wowConfigurationService.GetWoWInstallationPath();
    }

    /// <summary>
    /// Loads fonts from the specified directory
    /// </summary>
    [RelayCommand]
    private async Task LoadFontsAsync(string? directoryPath = null)
    {
        // Cancel any ongoing operation
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            IsLoading = true;
            StatusMessage = "Discovering fonts...";
            FontTree.Clear();
            SelectedItem = null;
            _selectedFont = null;
            PreviewImage = null;

            // Use default fonts directory if not specified
            var fontsPath = directoryPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fonts");
            
            if (!Directory.Exists(fontsPath))
            {
                StatusMessage = $"Fonts directory not found: {fontsPath}";
                return;
            }

            // Collect all fonts first
            var allFonts = new System.Collections.Generic.List<FontInfo>();
            
            await foreach (var fontEntry in _fontDiscoveryService.ScanDirectoryAsync(
                fontsPath, 
                recursive: true, 
                cancellationToken: _cancellationTokenSource.Token))
            {
                // Extract metadata
                var metadata = await _fontMetadataService.LoadFontAsync(fontEntry.FilePath);
                
                // Determine folder path relative to fonts root
                var relativePath = Path.GetRelativePath(fontsPath, Path.GetDirectoryName(fontEntry.FilePath) ?? fontsPath);
                var folderPath = relativePath == "." ? "Root" : relativePath;
                
                // Create FontInfo for UI
                var fontInfo = new FontInfo
                {
                    FilePath = fontEntry.FilePath,
                    FamilyName = metadata?.FontFamily ?? fontEntry.FontFamily ?? Path.GetFileNameWithoutExtension(fontEntry.FileName),
                    SubfamilyName = metadata?.FontSubfamily ?? fontEntry.FontSubfamily ?? "Regular",
                    FileName = fontEntry.FileName,
                    FileSize = fontEntry.FileSize,
                    FolderPath = folderPath
                };

                allFonts.Add(fontInfo);
            }

            // Build tree structure
            BuildFontTree(allFonts);
            
            var folderCount = FontTree.Count;
            var fontCount = allFonts.Count;
            StatusMessage = $"Loaded {folderCount} folder{(folderCount != 1 ? "s" : "")} with {fontCount} font{(fontCount != 1 ? "s" : "")}";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Font discovery cancelled";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading fonts: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Called when the selected item changes
    /// </summary>
    partial void OnSelectedItemChanged(object? value)
    {
        if (value is FontTreeNode node && node.NodeType == FontTreeNodeType.Font && node.FontInfo != null)
        {
            _selectedFont = node.FontInfo;
            _ = RenderPreviewAsync(node.FontInfo);
        }
        else
        {
            _selectedFont = null;
            PreviewImage = null;
        }
        
        // Update command can-execute state
        ApplyFontCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Applies the selected font to the specified WoW font category
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanApplyFont))]
    private async Task ApplyFontAsync(FontCategory category)
    {
        if (_selectedFont == null)
        {
            return;
        }

        try
        {
            StatusMessage = "Preparing font replacement...";

            // Validate WoW installation
            var validation = _wowConfigurationService.ValidateInstallation();
            if (!validation.IsValid)
            {
                StatusMessage = $"Error: {string.Join("; ", validation.Errors)}";
                return;
            }

            // Detect locale
            var localeResult = await _wowConfigurationService.DetectLocaleAsync();
            if (!localeResult.Success && localeResult.Warning != null)
            {
                StatusMessage = $"Warning: {localeResult.Warning}";
                await Task.Delay(2000); // Show warning briefly
            }

            // Get client configuration
            var clientConfig = await _wowConfigurationService.GetClientConfigurationAsync();

            // Map FontCategory to ReplacementCategory
            var replacementCategory = category switch
            {
                FontCategory.All => ReplacementCategory.All,
                FontCategory.MainUI => ReplacementCategory.MainUI,
                FontCategory.Chat => ReplacementCategory.Chat,
                FontCategory.Damage => ReplacementCategory.Damage,
                _ => ReplacementCategory.All
            };

            // Get target font files based on category
            var targetFiles = _wowClientService.GetFontMappingForLocale(
                localeResult.Locale,
                replacementCategory);

            if (targetFiles == null || targetFiles.Count == 0)
            {
                StatusMessage = $"Error: No font mappings found for locale {localeResult.Locale}";
                return;
            }

            // Create operation
            var operation = new FontReplacementOperation
            {
                SourceFontPath = _selectedFont.FilePath,
                TargetClient = clientConfig,
                Categories = new List<ReplacementCategory> { replacementCategory },
                TargetFiles = targetFiles
            };

            // Show confirmation dialog
            var confirmationViewModel = new FontReplacementConfirmationViewModel();
            confirmationViewModel.Initialize(operation, AppDomain.CurrentDomain.BaseDirectory);

            var dialog = new Views.FontReplacementConfirmationDialog
            {
                DataContext = confirmationViewModel
            };

            var mainWindow = GetMainWindow();
            if (mainWindow == null)
            {
                StatusMessage = "Error: Could not find main window";
                return;
            }

            var result = await dialog.ShowDialog<bool?>(mainWindow);

            if (result != true || !confirmationViewModel.IsConfirmed)
            {
                StatusMessage = "Font replacement cancelled";
                return;
            }

            // Execute replacement
            StatusMessage = "Replacing fonts...";
            var replacementResult = await _fontReplacementService.ReplaceFontAsync(
                operation,
                new Progress<int>(percent => StatusMessage = $"Replacing fonts... {percent}%"));

            if (replacementResult.Success)
            {
                StatusMessage = $"Successfully replaced {replacementResult.ReplacedFiles.Count} fonts. Backup created at: {replacementResult.BackupInfo?.BackupDirectory}";
            }
            else if (replacementResult.FailedFiles.Count > 0 && replacementResult.ReplacedFiles.Count > 0)
            {
                StatusMessage = $"Replaced {replacementResult.ReplacedFiles.Count} of {replacementResult.ReplacedFiles.Count + replacementResult.FailedFiles.Count} fonts. Failed: {string.Join(", ", replacementResult.FailedFiles)}. Backup at: {replacementResult.BackupInfo?.BackupDirectory}";
            }
            else
            {
                StatusMessage = $"Font replacement failed: {replacementResult.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets the main window for dialog parent
    /// </summary>
    private Avalonia.Controls.Window? GetMainWindow()
    {
        return Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
    }

    /// <summary>
    /// Determines if a font can be applied (requires a font to be selected)
    /// </summary>
    private bool CanApplyFont()
    {
        return _selectedFont != null;
    }

    /// <summary>
    /// Builds the tree structure from a flat list of fonts
    /// </summary>
    private void BuildFontTree(System.Collections.Generic.List<FontInfo> fonts)
    {
        // Group fonts by folder
        var fontsByFolder = fonts.GroupBy(f => f.FolderPath ?? "Root")
                                  .OrderBy(g => g.Key);

        foreach (var folderGroup in fontsByFolder)
        {
            var folderNode = new FontTreeNode
            {
                Name = folderGroup.Key,
                NodeType = FontTreeNodeType.Folder,
                FolderPath = folderGroup.Key
            };

            foreach (var font in folderGroup.OrderBy(f => f.FileName))
            {
                var fontNode = new FontTreeNode
                {
                    Name = font.FileName,  // Optional DisplayName
                    NodeType = FontTreeNodeType.Font,
                    FontInfo = font
                };
                folderNode.Children.Add(fontNode);
            }

            FontTree.Add(folderNode);
        }
    }

    /// <summary>
    /// Renders a preview for the selected font
    /// </summary>
    private async Task RenderPreviewAsync(FontInfo fontInfo)
    {
        try
        {
            StatusMessage = $"Rendering preview for {fontInfo.DisplayName}...";
            
            var bitmap = await _fontPreviewService.RenderPreviewAsync(fontInfo);
            
            if (bitmap != null)
            {
                PreviewImage = bitmap;
                StatusMessage = $"Preview: {fontInfo.DisplayName}";
            }
            else
            {
                PreviewImage = null;
                StatusMessage = $"Failed to render preview for {fontInfo.DisplayName}";
            }
        }
        catch (Exception ex)
        {
            PreviewImage = null;
            StatusMessage = $"Error rendering preview: {ex.Message}";
        }
    }

    /// <summary>
    /// Opens a folder picker dialog to select WoW installation path
    /// </summary>
    [RelayCommand]
    private async Task SelectWoWInstallPathAsync()
    {
        try
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null)
            {
                StatusMessage = "Error: Could not find main window";
                return;
            }

            // Open folder picker dialog
            var folderPicker = await mainWindow.StorageProvider.OpenFolderPickerAsync(
                new Avalonia.Platform.Storage.FolderPickerOpenOptions
                {
                    Title = "Select WoW Installation Directory",
                    AllowMultiple = false
                });

            if (folderPicker.Count > 0)
            {
                var selectedPath = folderPicker[0].Path.LocalPath;
                
                // Validate the selected path
                if (ValidateWoWInstallPath(selectedPath))
                {
                    // Update the configuration service with the new path
                    _wowConfigurationService.SetWoWInstallationPath(selectedPath);
                    
                    // Update the UI display
                    WowInstallPath = selectedPath;
                    
                    // Save to persistent storage
                    await _configurationService.UpdateSettingAsync("LastWoWClientPath", selectedPath);
                    
                    StatusMessage = $"WoW installation path updated: {selectedPath}";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error selecting path: {ex.Message}";
        }
    }

    /// <summary>
    /// Validates that the selected path ends with a valid WoW client directory name
    /// </summary>
    private bool ValidateWoWInstallPath(string path)
    {
        var validClientDirs = new[] { "_retail_", "_classic_", "_classic_era_", "_ptr_", "_xptr_", "_beta_" };
        var dirName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        
        if (!validClientDirs.Contains(dirName, StringComparer.OrdinalIgnoreCase))
        {
            StatusMessage = $"Warning: Selected directory '{dirName}' does not appear to be a valid WoW client directory. Expected one of: {string.Join(", ", validClientDirs)}";
            return false;
        }

        return true;
    }
}
