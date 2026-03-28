using HardwareStore.Application.DTOs.Settings;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Settings;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel(ISettingsService settingsService) : PageModel
{
    public IReadOnlyList<AppSettingDto> Settings { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Settings = await settingsService.GetAllAsync(cancellationToken);
    }
}
