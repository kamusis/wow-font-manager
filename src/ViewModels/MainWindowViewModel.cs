using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using WowFontManager.Models;
using WowFontManager.Services;

namespace WowFontManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWoWClientService _wowClientService;
    private readonly IFontDiscoveryService _fontDiscoveryService;
    private readonly IFontMetadataService _fontMetadataService;
    private readonly IFontReplacementService _fontReplacementService;
    private readonly IRenderingService _renderingService;
    
    private WoWClientConfiguration? _selectedClient;
    private FontFileEntry? _selectedFont;
    private string _statusMessage = "Ready";
    private bool _isLoading;
    private string _searchText = string.Empty;
    private string _previewText = "The quick brown fox jumps over the lazy dog";
    private float _previewSize = 24.0f;

    public MainWindowViewModel()
    {
        // Initialize services
        _wowClientService = new WoWClientService();
        _fontDiscoveryService = new FontDiscoveryService();
        _fontMetadataService = new FontMetadataService();
        _renderingService = new RenderingService();
        _fontReplacementService = new FontReplacementService(_wowClientService);

        WoWClients = new ObservableCollection<WoWClientConfiguration>();
        AvailableFonts = new ObservableCollection<FontFileEntry>();
        Backups = new ObservableCollection<BackupInfo>();
        
        // Initialize commands
        DetectClientsCommand = ReactiveCommand.CreateFromTask(DetectClientsAsync);
        BrowseFontsCommand = ReactiveCommand.CreateFromTask(BrowseFontsAsync);
        ReplaceFontsCommand = ReactiveCommand.CreateFromTask(ReplaceFontsAsync, 
            this.WhenAnyValue(x => x.SelectedClient, x => x.SelectedFont, 
                (client, font) => client != null && font != null));
        LoadBackupsCommand = ReactiveCommand.CreateFromTask(LoadBackupsAsync);
        
        // Auto-detect clients on startup
        _ = DetectClientsAsync();
    }

    public ObservableCollection<WoWClientConfiguration> WoWClients { get; }
    public ObservableCollection<FontFileEntry> AvailableFonts { get; }
    public ObservableCollection<BackupInfo> Backups { get; }

    public WoWClientConfiguration? SelectedClient
    {
        get => _selectedClient;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedClient, value);
            if (value != null)
            {
                _ = LoadBackupsAsync();
                PreviewText = _renderingService.GetDefaultSampleText(value.Locale);
            }
        }
    }

    public FontFileEntry? SelectedFont
    {
        get => _selectedFont;
        set => this.RaiseAndSetIfChanged(ref _selectedFont, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            FilterFonts();
        }
    }

    public string PreviewText
    {
        get => _previewText;
        set => this.RaiseAndSetIfChanged(ref _previewText, value);
    }

    public float PreviewSize
    {
        get => _previewSize;
        set => this.RaiseAndSetIfChanged(ref _previewSize, value);
    }

    public ICommand DetectClientsCommand { get; }
    public ICommand BrowseFontsCommand { get; }
    public ICommand ReplaceFontsCommand { get; }
    public ICommand LoadBackupsCommand { get; }

    private async Task DetectClientsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Detecting WoW clients...";
            
            var clients = await _wowClientService.DetectClientsAsync();
            WoWClients.Clear();
            
            foreach (var client in clients)
            {
                WoWClients.Add(client);
            }
            
            if (WoWClients.Any())
            {
                SelectedClient = WoWClients.First();
                StatusMessage = $"Found {WoWClients.Count} WoW client(s)";
            }
            else
            {
                StatusMessage = "No WoW clients detected. Please select manually.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error detecting clients: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task BrowseFontsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Scanning for fonts...";
            
            // For demo, scan system fonts directory
            var fontsPath = Environment.OSVersion.Platform == PlatformID.Unix 
                ? "/usr/share/fonts" 
                : @"C:\Windows\Fonts";
            
            AvailableFonts.Clear();
            var fonts = await _fontDiscoveryService.GetFontsInDirectoryAsync(fontsPath);
            
            foreach (var font in fonts.Take(50)) // Limit to first 50 for performance
            {
                AvailableFonts.Add(font);
            }
            
            StatusMessage = $"Found {AvailableFonts.Count} fonts";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error browsing fonts: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ReplaceFontsAsync()
    {
        if (SelectedClient == null || SelectedFont == null)
            return;

        try
        {
            IsLoading = true;
            StatusMessage = "Replacing fonts...";

            var operation = new FontReplacementOperation
            {
                SourceFontPath = SelectedFont.FilePath,
                TargetClient = SelectedClient,
                Categories = new System.Collections.Generic.List<ReplacementCategory> 
                { 
                    ReplacementCategory.All 
                }
            };

            var result = await _fontReplacementService.ReplaceFontAsync(operation);
            
            if (result.Success)
            {
                StatusMessage = $"Successfully replaced {result.ReplacedFiles.Count} fonts";
                await LoadBackupsAsync();
            }
            else
            {
                StatusMessage = $"Font replacement failed: {result.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error replacing fonts: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadBackupsAsync()
    {
        if (SelectedClient == null)
            return;

        try
        {
            var backups = await _fontReplacementService.ListBackupsAsync(SelectedClient);
            Backups.Clear();
            
            foreach (var backup in backups)
            {
                Backups.Add(backup);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading backups: {ex.Message}";
        }
    }

    private void FilterFonts()
    {
        // Simple filter implementation
        // In a real app, this would filter the ObservableCollection
    }
}
