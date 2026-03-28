using HardwareStore.Domain.Common;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Domain.Entities;

public sealed class InventoryMovement : EntityBase
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public InventoryMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime MovementDateUtc { get; set; } = DateTime.UtcNow;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
