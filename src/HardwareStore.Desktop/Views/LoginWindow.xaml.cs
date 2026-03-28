using System.Windows;
using HardwareStore.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareStore.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;
    private readonly IServiceProvider _services;

    public LoginWindow(LoginViewModel viewModel, IServiceProvider services)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _services = services;
        DataContext = _viewModel;
    }

    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordInput.Password;
    }

    private async void SignIn_OnClick(object sender, RoutedEventArgs e)
    {
        if (!await _viewModel.LoginAsync())
        {
            return;
        }

        var mainWindow = _services.GetRequiredService<MainWindow>();
        await mainWindow.InitializeAsync(_viewModel.DisplayName, _viewModel.Role);
        mainWindow.Show();
        Close();
    }
}
