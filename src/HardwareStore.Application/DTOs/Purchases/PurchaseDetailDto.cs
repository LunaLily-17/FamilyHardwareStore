using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Purchases;

public sealed class PurchaseDetailDto
{
    public Guid Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
    public PurchaseStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public IList<PurchaseDetailLineDto> Items { get; set; } = new List<PurchaseDetailLineDto>();
}
