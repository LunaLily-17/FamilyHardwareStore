using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Purchases;

namespace HardwareStore.Application.Interfaces;

public interface IPurchaseService
{
    Task<IReadOnlyList<PurchaseListItemDto>> GetPurchasesAsync(CancellationToken cancellationToken = default);
    Task<PurchaseDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult> CreateAsync(CreatePurchaseDto request, CancellationToken cancellationToken = default);
}
