using HardwareStore.Application.DTOs.Auth;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using HardwareStore.Desktop.Infrastructure;
using System.Windows.Input;

namespace HardwareStore.Desktop.ViewModels;

public sealed class LoginViewModel(IAuthService authService) : ObservableObject
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string? _errorMessage;
    private bool _isBusy;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public Guid UserId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    public ICommand LoginCommand => new RelayCommand(() => { }, () => !IsBusy);

    public async Task<bool> LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var result = await authService.LoginAsync(new LoginRequestDto
            {
                Username = Username,
                Password = Password
            });

            if (!result.Succeeded)
            {
                ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "ဝင်ရောက်မှု မအောင်မြင်ပါ။"
                    : result.ErrorMessage;
                return false;
            }

            UserId = result.UserId;
            DisplayName = result.DisplayName;
            Role = result.Role;
            return true;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
