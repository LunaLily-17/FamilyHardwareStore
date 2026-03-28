using HardwareStore.Domain.Common;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Domain.Entities;

public sealed class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Cashier;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginUtc { get; set; }
}
