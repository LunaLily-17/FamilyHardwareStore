using HardwareStore.Application.DTOs.Purchases;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Purchases;

public sealed class IndexModel(IPurchaseService purchaseService) : PageModel
{
    public IReadOnlyList<PurchaseListItemDto> Purchases { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Purchases = await purchaseService.GetPurchasesAsync(cancellationToken);
    }
}
