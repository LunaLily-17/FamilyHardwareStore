using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Suppliers;

namespace HardwareStore.Application.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> UpsertAsync(SupplierDto request, CancellationToken cancellationToken = default);
}
