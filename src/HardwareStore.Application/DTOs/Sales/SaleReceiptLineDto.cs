namespace HardwareStore.Application.DTOs.Sales;

public sealed class SaleReceiptLineDto
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductNameMm { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
