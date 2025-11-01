using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WowFontManager.ViewModels;

namespace WowFontManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Subscribe to SelectionChanged and guard against bubbling from inner controls.
        MainTabs.SelectionChanged += OnTabsSelectionChanged;
    }

    private async void OnTabsSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            // Only handle when the TabControl itself raised the event
            if (!ReferenceEquals(e.Source, MainTabs))
            {
                return;
            }

            if (MainTabs.SelectedItem is TabItem tab && tab.Content is FontBrowserView view && view.DataContext is FontBrowserViewModel vm)
            {
                await vm.LoadFontsCommand.ExecuteAsync(null);
            }
        }
        catch
        {
            // Ignore refresh errors; UI remains usable
        }
    }
}