using HardwareStore.Domain.Common;

namespace HardwareStore.Domain.Entities;

public sealed class SaleItem : EntityBase
{
    public Guid SaleId { get; set; }
    public Sale? Sale { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
