namespace HardwareStore.Application.DTOs.Purchases;

public sealed class CreatePurchaseDto
{
    public Guid SupplierId { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Notes { get; set; }
    public IList<CreatePurchaseLineDto> Items { get; set; } = new List<CreatePurchaseLineDto>();
}
