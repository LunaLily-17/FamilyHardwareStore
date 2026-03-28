using System.ComponentModel.DataAnnotations;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using HardwareStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Web.Pages.Products;

public sealed class CreateModel(IProductService productService, AppDbContext dbContext) : PageModel
{
    [BindProperty]
    public CreateProductInput Input { get; set; } = new();

    public List<SelectListItem> CategoryOptions { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(cancellationToken);
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await productService.UpsertAsync(new ProductUpsertDto
        {
            Name = Input.Name,
            NameMm = Input.NameMm,
            Sku = Input.Sku,
            Barcode = Input.Barcode,
            Description = Input.Description,
            CategoryId = Input.CategoryId,
            UnitType = Input.UnitType,
            CostPrice = Input.CostPrice,
            SalePrice = Input.SalePrice,
            ReorderLevel = Input.ReorderLevel
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Products/Index");
    }

    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        CategoryOptions = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    public sealed class CreateProductInput
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Name (Myanmar)")]
        public string? NameMm { get; set; }

        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        public UnitType UnitType { get; set; } = UnitType.Piece;
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ReorderLevel { get; set; }
    }
}
