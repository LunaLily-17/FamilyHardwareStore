using System.ComponentModel.DataAnnotations;
using HardwareStore.Application.DTOs.Categories;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Categories;

public sealed class CreateModel(ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public CreateCategoryInput Input { get; set; } = new();

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

        var result = await categoryService.UpsertAsync(new CategoryDto
        {
            Name = Input.Name,
            NameMm = Input.NameMm,
            Description = Input.Description,
            IsActive = Input.IsActive
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }

    public sealed class CreateCategoryInput
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Name (Myanmar)")]
        public string? NameMm { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
