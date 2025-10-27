using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WowFontManager.Models;
using WowFontManager.Services;

namespace WowFontManager.ViewModels;

/// <summary>
/// ViewModel for the font replacement confirmation dialog
/// </summary>
public partial class FontReplacementConfirmationViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _sourceFontName = string.Empty;

    [ObservableProperty]
    private string _sourceFontPath = string.Empty;

    [ObservableProperty]
    private string _targetClientInfo = string.Empty;

    [ObservableProperty]
    private string _targetPath = string.Empty;

    [ObservableProperty]
    private string _backupLocation = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _commandPreview = new();

    [ObservableProperty]
    private bool _isConfirmed;

    /// <summary>
    /// Initializes the dialog with operation details
    /// </summary>
    public void Initialize(FontReplacementOperation operation, string appInstallDir)
    {
        SourceFontName = Path.GetFileName(operation.SourceFontPath);
        SourceFontPath = operation.SourceFontPath;
        TargetClientInfo = $"{operation.TargetClient.ClientType} ({operation.TargetClient.Locale})";
        TargetPath = operation.TargetClient.FontsPath;
        
        // Generate backup location
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var wowInstallPath = operation.TargetClient.InstallationPath;
        BackupLocation = Path.Combine(wowInstallPath, $"Fonts.{timestamp}");

        // Generate command preview
        CommandPreview.Clear();
        foreach (var targetFile in operation.TargetFiles)
        {
            var targetFullPath = Path.Combine(operation.TargetClient.FontsPath, targetFile);
            var command = $"copy \"{operation.SourceFontPath}\" \"{targetFullPath}\"";
            CommandPreview.Add(command);
        }
    }

    /// <summary>
    /// Confirms the font replacement operation
    /// </summary>
    [RelayCommand]
    private void Confirm()
    {
        IsConfirmed = true;
    }

    /// <summary>
    /// Cancels the font replacement operation
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
    }
}
