using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Reports;

public sealed class DailySalesModel(IReportingService reportingService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public DailySalesReportDto Report { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Report = await reportingService.GetDailySalesSummaryAsync(Date, cancellationToken);
    }
}
