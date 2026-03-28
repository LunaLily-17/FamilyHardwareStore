using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Products;

public sealed class ProductUpsertDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameMm { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public UnitType UnitType { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal ReorderLevel { get; set; }
    public bool IsActive { get; set; } = true;
}
