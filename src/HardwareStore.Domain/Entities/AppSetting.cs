using HardwareStore.Domain.Common;

namespace HardwareStore.Domain.Entities;

public sealed class AppSetting : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
}
