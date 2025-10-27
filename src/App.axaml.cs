using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using WowFontManager.Services;
using WowFontManager.ViewModels;
using WowFontManager.Views;

namespace WowFontManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // Initialize services (prepare for DI container in future)
            var fontDiscoveryService = new FontDiscoveryService();
            var fontMetadataService = new FontMetadataService();
            var renderingService = new RenderingService();
            var fontPreviewService = new FontPreviewService(renderingService);
            var fontCategoryService = new FontCategoryService();

            // Create ViewModels
            var fontBrowserViewModel = new FontBrowserViewModel(
                fontDiscoveryService,
                fontMetadataService,
                fontPreviewService,
                fontCategoryService);

            // Create and configure MainWindow
            var mainWindow = new MainWindow();
            mainWindow.FontBrowserView.DataContext = fontBrowserViewModel;
            
            desktop.MainWindow = mainWindow;

            // Auto-load fonts on startup
            _ = fontBrowserViewModel.LoadFontsCommand.ExecuteAsync(null);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}