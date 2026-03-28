using HardwareStore.Application.DTOs.Exports;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.DTOs.Reports;

namespace HardwareStore.Application.Interfaces;

public interface IExportService
{
    Task<ExportFileDto> ExportProductsCsvAsync(
        IReadOnlyList<ProductListItemDto> products,
        CancellationToken cancellationToken = default);

    Task<ExportFileDto> ExportDailySalesCsvAsync(
        DailySalesReportDto report,
        CancellationToken cancellationToken = default);
}
