using HardwareStore.Application.DTOs.Sales;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Sales;

[Authorize(Policy = "CashierOrAdmin")]
public sealed class HistoryModel(ISalesService salesService) : PageModel
{
    public IReadOnlyList<SaleHistoryItemDto> Sales { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Sales = await salesService.GetRecentSalesAsync(cancellationToken: cancellationToken);
    }
}
