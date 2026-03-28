using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Sales;

namespace HardwareStore.Application.Interfaces;

public interface ISalesService
{
    Task<CreateSaleResultDto> CreateAsync(CreateSaleDto request, CancellationToken cancellationToken = default);
    Task<SaleReceiptDto?> GetReceiptAsync(string receiptNumber, CancellationToken cancellationToken = default);
    Task<ServiceResult> VoidAsync(string receiptNumber, Guid userId, CancellationToken cancellationToken = default);
}
