using HardwareStore.Application.DTOs.Inventory;
using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages;

public sealed class IndexModel(IReportingService reportingService, IInventoryService inventoryService) : PageModel
{
    public DailySalesReportDto DailySales { get; private set; } = new();
    public IReadOnlyList<StockOnHandDto> LowStockItems { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        DailySales = await reportingService.GetDailySalesSummaryAsync(DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        LowStockItems = (await inventoryService.GetStockOnHandAsync(cancellationToken))
            .Where(x => x.IsLowStock)
            .Take(8)
            .ToList();
    }
}
