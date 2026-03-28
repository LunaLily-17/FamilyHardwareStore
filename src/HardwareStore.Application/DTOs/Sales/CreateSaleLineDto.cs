namespace HardwareStore.Application.DTOs.Sales;

public sealed class CreateSaleLineDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
}
