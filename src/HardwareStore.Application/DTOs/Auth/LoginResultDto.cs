using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Auth;

public sealed class LoginResultDto
{
    public bool Succeeded { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
