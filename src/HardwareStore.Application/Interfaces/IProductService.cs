using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Products;

namespace HardwareStore.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> UpsertAsync(ProductUpsertDto request, CancellationToken cancellationToken = default);
}
