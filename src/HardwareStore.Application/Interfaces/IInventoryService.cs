using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Inventory;

namespace HardwareStore.Application.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<StockOnHandDto>> GetStockOnHandAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> AdjustStockAsync(StockAdjustmentDto request, CancellationToken cancellationToken = default);
}
