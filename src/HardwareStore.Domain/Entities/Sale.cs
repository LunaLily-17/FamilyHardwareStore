using HardwareStore.Domain.Common;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Domain.Entities;

public sealed class Sale : EntityBase
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid CashierId { get; set; }
    public User? Cashier { get; set; }
    public DateTime SaleDateUtc { get; set; } = DateTime.UtcNow;
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public DateTime? VoidedUtc { get; set; }
    public Guid? VoidedByUserId { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
