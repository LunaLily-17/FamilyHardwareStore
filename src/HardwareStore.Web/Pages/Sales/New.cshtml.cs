using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HardwareStore.Application.DTOs.Sales;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using HardwareStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Web.Pages.Sales;

[Authorize(Policy = "CashierOrAdmin")]
public sealed class NewModel(ISalesService salesService, AppDbContext dbContext) : PageModel
{
    [BindProperty]
    public CreateSaleInput Input { get; set; } = new();

    public List<SelectListItem> ProductOptions { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadProductsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadProductsAsync(cancellationToken);
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var cashierId))
        {
            ErrorMessage = "Unable to resolve the current cashier account.";
            return Page();
        }

        var result = await salesService.CreateAsync(new CreateSaleDto
        {
            CashierId = cashierId,
            PaymentMethod = Input.PaymentMethod,
            DiscountAmount = Input.DiscountAmount,
            TaxAmount = Input.TaxAmount,
            Notes = Input.Notes,
            Items =
            [
                new CreateSaleLineDto
                {
                    ProductId = Input.ProductId,
                    Quantity = Input.Quantity
                }
            ]
        }, cancellationToken);

        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.ReceiptNumber))
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Sales/Receipt", new { receiptNumber = result.ReceiptNumber });
    }

    private async Task LoadProductsAsync(CancellationToken cancellationToken)
    {
        ProductOptions = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(
                !string.IsNullOrWhiteSpace(x.NameMm) ? $"{x.Name} / {x.NameMm}" : x.Name,
                x.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    public sealed class CreateSaleInput
    {
        [Display(Name = "Product")]
        public Guid ProductId { get; set; }

        public decimal Quantity { get; set; } = 1;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string? Notes { get; set; }
    }
}
