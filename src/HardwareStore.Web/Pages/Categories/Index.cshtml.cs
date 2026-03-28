using HardwareStore.Application.DTOs.Categories;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Categories;

public sealed class IndexModel(ICategoryService categoryService) : PageModel
{
    public IReadOnlyList<CategoryDto> Categories { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Categories = await categoryService.GetCategoriesAsync(cancellationToken);
    }
}
