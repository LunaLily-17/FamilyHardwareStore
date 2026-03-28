using HardwareStore.Domain.Common;

namespace HardwareStore.Domain.Entities;

public sealed class StockAdjustment : EntityBase
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public decimal PreviousQuantity { get; set; }
    public decimal AdjustedQuantity { get; set; }
    public decimal NewQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid AdjustedByUserId { get; set; }
    public User? AdjustedByUser { get; set; }
}
