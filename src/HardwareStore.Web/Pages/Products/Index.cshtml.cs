using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Products;

public sealed class IndexModel(IProductService productService, IExportService exportService) : PageModel
{
    public IReadOnlyList<ProductListItemDto> Products { get; private set; } = [];
    public bool CanEditProducts => User.IsInRole(UserRole.Admin.ToString());

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Products = await productService.GetProductsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostExportCsvAsync(CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(cancellationToken);
        var exportFile = await exportService.ExportProductsCsvAsync(products, cancellationToken);

        return File(exportFile.Content, exportFile.ContentType, exportFile.FileName);
    }
}
