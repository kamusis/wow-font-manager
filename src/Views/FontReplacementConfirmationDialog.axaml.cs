using Avalonia.Controls;
using Avalonia.Interactivity;
using WowFontManager.ViewModels;

namespace WowFontManager.Views;

public partial class FontReplacementConfirmationDialog : Window
{
    public FontReplacementConfirmationDialog()
    {
        InitializeComponent();
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FontReplacementConfirmationViewModel viewModel)
        {
            viewModel.IsConfirmed = true;
        }
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FontReplacementConfirmationViewModel viewModel)
        {
            viewModel.IsConfirmed = false;
        }
        Close(false);
    }
}
