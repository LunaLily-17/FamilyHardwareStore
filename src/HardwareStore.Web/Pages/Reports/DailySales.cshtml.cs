using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Reports;

public sealed class DailySalesModel(IReportingService reportingService, IExportService exportService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    [Display(Name = "နေ့စွဲ")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public DailySalesReportDto Report { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Report = await reportingService.GetDailySalesSummaryAsync(Date, cancellationToken);
    }

    public async Task<IActionResult> OnPostExportCsvAsync(CancellationToken cancellationToken)
    {
        var report = await reportingService.GetDailySalesSummaryAsync(Date, cancellationToken);
        var exportFile = await exportService.ExportDailySalesCsvAsync(report, cancellationToken);

        return File(exportFile.Content, exportFile.ContentType, exportFile.FileName);
    }
}
