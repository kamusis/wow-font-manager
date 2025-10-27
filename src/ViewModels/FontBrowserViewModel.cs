using System;
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

    public FontBrowserViewModel(
        IFontDiscoveryService fontDiscoveryService,
        IFontMetadataService fontMetadataService,
        IFontPreviewService fontPreviewService,
        IFontCategoryService fontCategoryService)
    {
        _fontDiscoveryService = fontDiscoveryService;
        _fontMetadataService = fontMetadataService;
        _fontPreviewService = fontPreviewService;
        _fontCategoryService = fontCategoryService;
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
    private void ApplyFont(FontCategory category)
    {
        // Placeholder implementation - to be completed in future change
        // This will eventually:
        // 1. Get the list of WoW font files for the category
        // 2. Copy the selected font to replace those files
        // 3. Create backups before replacement
        // 4. Update status message
        
        if (_selectedFont == null)
        {
            return;
        }

        StatusMessage = $"Font replacement for {category} category will be implemented in a future update";
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

            foreach (var font in folderGroup.OrderBy(f => f.DisplayName))
            {
                var fontNode = new FontTreeNode
                {
                    Name = font.DisplayName,
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
}
