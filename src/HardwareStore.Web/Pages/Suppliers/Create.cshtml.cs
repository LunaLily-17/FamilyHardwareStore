using System.ComponentModel.DataAnnotations;
using HardwareStore.Application.DTOs.Suppliers;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Suppliers;

public sealed class CreateModel(ISupplierService supplierService) : PageModel
{
    [BindProperty]
    public CreateSupplierInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await supplierService.UpsertAsync(new SupplierDto
        {
            Name = Input.Name,
            ContactPerson = Input.ContactPerson,
            Phone = Input.Phone,
            Address = Input.Address,
            Email = Input.Email
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Suppliers/Index");
    }

    public sealed class CreateSupplierInput
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Contact person")]
        public string? ContactPerson { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
}
