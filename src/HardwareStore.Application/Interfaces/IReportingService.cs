using HardwareStore.Application.DTOs.Reports;

namespace HardwareStore.Application.Interfaces;

public interface IReportingService
{
    Task<DailySalesReportDto> GetDailySalesSummaryAsync(DateOnly date, CancellationToken cancellationToken = default);
}
