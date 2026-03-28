namespace HardwareStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseLineDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
}
