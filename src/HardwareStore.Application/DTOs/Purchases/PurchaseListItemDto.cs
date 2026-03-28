using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Purchases;

public sealed class PurchaseListItemDto
{
    public Guid Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
    public PurchaseStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
