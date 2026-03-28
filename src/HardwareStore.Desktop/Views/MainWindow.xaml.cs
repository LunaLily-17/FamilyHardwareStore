using System.Windows;
using HardwareStore.Desktop.ViewModels;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly ShellViewModel _viewModel;

    public MainWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    public async Task InitializeAsync(string displayName, UserRole role)
    {
        await _viewModel.LoadAsync(displayName, role);
        AdminModulesButton.Visibility = role == UserRole.Admin ? Visibility.Visible : Visibility.Collapsed;
    }
}
