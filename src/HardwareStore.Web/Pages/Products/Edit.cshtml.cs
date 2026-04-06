using System.ComponentModel.DataAnnotations;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using HardwareStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Web.Pages.Products;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel(IProductService productService, AppDbContext dbContext) : PageModel
{
    [BindProperty]
    public EditProductInput Input { get; set; } = new();

    public List<SelectListItem> CategoryOptions { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(cancellationToken);

        var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            return RedirectToPage("/Products/Index");
        }

        Input = new EditProductInput
        {
            Id = product.Id,
            Name = product.Name,
            NameMm = product.NameMm,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Description = product.Description,
            CategoryId = product.CategoryId,
            UnitType = product.UnitType,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            ReorderLevel = product.ReorderLevel
        };

        return Page();
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
            Id = Input.Id,
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

    public sealed class EditProductInput
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "ပစ္စည်းအမည်")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ပစ္စည်းအမည် (မြန်မာ)")]
        public string? NameMm { get; set; }

        [Display(Name = "ပစ္စည်းကုဒ်")]
        public string? Sku { get; set; }

        [Display(Name = "ဘားကုဒ်")]
        public string? Barcode { get; set; }

        [Display(Name = "အသေးစိတ်")]
        public string? Description { get; set; }

        [Display(Name = "အမျိုးအစား")]
        public Guid CategoryId { get; set; }

        [Display(Name = "အတိုင်းအတာ")]
        public UnitType UnitType { get; set; } = UnitType.Piece;
        [Display(Name = "ဝယ်ဈေး")]
        public decimal CostPrice { get; set; }
        [Display(Name = "ရောင်းဈေး")]
        public decimal SalePrice { get; set; }
        [Display(Name = "ပြန်ဖြည့်ရန် အရေအတွက်")]
        public decimal ReorderLevel { get; set; }
    }
}
