using HardwareStore.Application.DTOs.Purchases;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Purchases;

public sealed class DetailsModel(IPurchaseService purchaseService) : PageModel
{
    public PurchaseDetailDto? Purchase { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Purchase = await purchaseService.GetByIdAsync(id, cancellationToken);
        return Page();
    }
}
