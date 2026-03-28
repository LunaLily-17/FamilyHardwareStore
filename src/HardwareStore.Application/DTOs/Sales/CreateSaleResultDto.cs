namespace HardwareStore.Application.DTOs.Sales;

public sealed class CreateSaleResultDto
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ReceiptNumber { get; init; }

    public static CreateSaleResultDto Success(string receiptNumber) => new() { Succeeded = true, ReceiptNumber = receiptNumber };
    public static CreateSaleResultDto Failure(string errorMessage) => new() { Succeeded = false, ErrorMessage = errorMessage };
}
