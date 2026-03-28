using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using HardwareStore.Application.Abstractions;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class ReportingService(IAppDbContext dbContext) : IReportingService
{
    public async Task<DailySalesReportDto> GetDailySalesSummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var startUtc = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endUtc = startUtc.AddDays(1);

        var sales = await dbContext.Sales
            .AsNoTracking()
            .Where(x => x.SaleDateUtc >= startUtc && x.SaleDateUtc < endUtc && x.Status == SaleStatus.Completed)
            .ToListAsync(cancellationToken);

        return new DailySalesReportDto
        {
            Date = date,
            SaleCount = sales.Count,
            GrossSales = sales.Sum(x => x.Subtotal),
            TaxAmount = sales.Sum(x => x.TaxAmount),
            DiscountAmount = sales.Sum(x => x.DiscountAmount),
            NetSales = sales.Sum(x => x.TotalAmount)
        };
    }
}
