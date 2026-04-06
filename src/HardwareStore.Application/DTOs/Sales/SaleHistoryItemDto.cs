namespace HardwareStore.Application.DTOs.Sales;

public sealed class SaleHistoryItemDto
{
    public Guid SaleId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime SaleDateUtc { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
