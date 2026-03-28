using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Products;

public sealed class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameMm { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryNameMm { get; set; }
    public UnitType UnitType { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockOnHand { get; set; }
    public decimal ReorderLevel { get; set; }
}
