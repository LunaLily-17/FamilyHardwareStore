using HardwareStore.Domain.Common;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Domain.Entities;

public sealed class Product : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? NameMm { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public UnitType UnitType { get; set; } = UnitType.Piece;
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockOnHand { get; set; }
    public decimal ReorderLevel { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
