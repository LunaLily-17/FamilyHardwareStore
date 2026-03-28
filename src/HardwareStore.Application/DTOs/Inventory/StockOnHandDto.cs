namespace HardwareStore.Application.DTOs.Inventory;

public sealed class StockOnHandDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductNameMm { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryNameMm { get; set; }
    public decimal StockOnHand { get; set; }
    public decimal ReorderLevel { get; set; }
    public bool IsLowStock => StockOnHand <= ReorderLevel;
}
