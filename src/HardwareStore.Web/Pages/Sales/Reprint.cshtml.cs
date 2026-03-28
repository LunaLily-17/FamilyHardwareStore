using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Sales;

[Authorize(Policy = "CashierOrAdmin")]
public sealed class ReprintModel : PageModel
{
    [BindProperty]
    [Display(Name = "ဘောင်ချာနံပါတ်")]
    public string ReceiptNumber { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(ReceiptNumber))
        {
            return Page();
        }

        return RedirectToPage("/Sales/Receipt", new { receiptNumber = ReceiptNumber.Trim() });
    }
}
