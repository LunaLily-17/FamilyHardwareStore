namespace HardwareStore.Application.DTOs.Sales;

public sealed class SaleReceiptDto
{
    public Guid SaleId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime SaleDateUtc { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public IList<SaleReceiptLineDto> Items { get; set; } = new List<SaleReceiptLineDto>();
}
