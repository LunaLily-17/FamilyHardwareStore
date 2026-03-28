using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HardwareStore.Application.DTOs.Auth;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Account;

public sealed class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

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

        var result = await authService.LoginAsync(new LoginRequestDto
        {
            Username = Input.Username,
            Password = Input.Password
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new(ClaimTypes.Name, result.DisplayName),
            new(ClaimTypes.Role, result.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToPage("/Index");
    }

    public sealed class LoginInput
    {
        [Required]
        [Display(Name = "အသုံးပြုသူအမည်")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "လျှို့ဝှက်နံပါတ်")]
        public string Password { get; set; } = string.Empty;
    }
}
