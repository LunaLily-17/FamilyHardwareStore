namespace HardwareStore.Application.DTOs.Inventory;

public sealed class StockAdjustmentDto
{
    public Guid ProductId { get; set; }
    public decimal NewQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid AdjustedByUserId { get; set; }
}
