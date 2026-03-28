using HardwareStore.Domain.Common;

namespace HardwareStore.Domain.Entities;

public sealed class PurchaseItem : EntityBase
{
    public Guid PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }
}
