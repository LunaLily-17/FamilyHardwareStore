namespace HardwareStore.Application.DTOs.Purchases;

public sealed class PurchaseDetailLineDto
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
}
