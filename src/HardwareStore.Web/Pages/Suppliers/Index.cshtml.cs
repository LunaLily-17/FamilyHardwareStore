using HardwareStore.Application.DTOs.Suppliers;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Suppliers;

public sealed class IndexModel(ISupplierService supplierService) : PageModel
{
    public IReadOnlyList<SupplierDto> Suppliers { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Suppliers = await supplierService.GetSuppliersAsync(cancellationToken);
    }
}
