using HardwareStore.Application.DTOs.Backup;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Backup;

[Authorize(Policy = "AdminOnly")]
public sealed class IndexModel(IBackupService backupService) : PageModel
{
    public IReadOnlyList<BackupItemDto> Backups { get; private set; } = [];
    public string? StatusMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Backups = await backupService.GetBackupsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var result = await backupService.CreateBackupAsync(cancellationToken);
        StatusMessage = result.Succeeded ? "Backup created." : result.ErrorMessage;
        Backups = await backupService.GetBackupsAsync(cancellationToken);
        return Page();
    }
}
