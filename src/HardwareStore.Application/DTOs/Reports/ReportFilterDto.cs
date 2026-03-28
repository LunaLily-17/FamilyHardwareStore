namespace HardwareStore.Application.DTOs.Reports;

public sealed class ReportFilterDto
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CashierId { get; set; }
    public string? PaymentMethod { get; set; }
    public bool IncludeVoided { get; set; }
}
