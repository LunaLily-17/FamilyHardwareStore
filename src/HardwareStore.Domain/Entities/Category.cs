using HardwareStore.Domain.Common;

namespace HardwareStore.Domain.Entities;

public sealed class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? NameMm { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
