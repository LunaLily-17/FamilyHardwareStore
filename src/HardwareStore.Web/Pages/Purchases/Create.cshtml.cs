using System.ComponentModel.DataAnnotations;
using HardwareStore.Application.DTOs.Purchases;
using HardwareStore.Application.Interfaces;
using HardwareStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Web.Pages.Purchases;

public sealed class CreateModel(IPurchaseService purchaseService, AppDbContext dbContext) : PageModel
{
    [BindProperty]
    public CreatePurchaseInput Input { get; set; } = new();

    public List<SelectListItem> SupplierOptions { get; private set; } = [];
    public List<SelectListItem> ProductOptions { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await purchaseService.CreateAsync(new CreatePurchaseDto
        {
            SupplierId = Input.SupplierId,
            TaxAmount = Input.TaxAmount,
            Notes = Input.Notes,
            Items =
            [
                new CreatePurchaseLineDto
                {
                    ProductId = Input.ProductId,
                    Quantity = Input.Quantity,
                    UnitCost = Input.UnitCost
                }
            ]
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Purchases/Index");
    }

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        SupplierOptions = await dbContext.Suppliers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);

        ProductOptions = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    public sealed class CreatePurchaseInput
    {
        [Display(Name = "ပေးသွင်းသူ")]
        public Guid SupplierId { get; set; }

        [Display(Name = "ပစ္စည်း")]
        public Guid ProductId { get; set; }

        [Display(Name = "အရေအတွက်")]
        public decimal Quantity { get; set; }
        [Display(Name = "တစ်ခုဈေး")]
        public decimal UnitCost { get; set; }
        [Display(Name = "အခွန်")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "မှတ်ချက်")]
        public string? Notes { get; set; }
    }
}
