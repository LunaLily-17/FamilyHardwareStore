namespace HardwareStore.Application.DTOs.Reports;

public sealed class DailySalesReportDto
{
    public DateOnly Date { get; set; }
    public int SaleCount { get; set; }
    public decimal GrossSales { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetSales { get; set; }
}
