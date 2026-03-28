using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.DTOs.Sales;

public sealed class CreateSaleDto
{
    public Guid CashierId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Notes { get; set; }
    public IList<CreateSaleLineDto> Items { get; set; } = new List<CreateSaleLineDto>();
}
