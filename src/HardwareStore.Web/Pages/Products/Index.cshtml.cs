using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Products;

public sealed class IndexModel(IProductService productService) : PageModel
{
    public IReadOnlyList<ProductListItemDto> Products { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Products = await productService.GetProductsAsync(cancellationToken);
    }
}
