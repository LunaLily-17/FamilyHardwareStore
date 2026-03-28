using System.Globalization;
using System.Text;
using HardwareStore.Application.DTOs.Exports;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using HardwareStore.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace HardwareStore.Infrastructure.Services;

public sealed class CsvExportService(IOptions<StorageOptions> storageOptions) : IExportService
{
    private readonly StorageOptions _options = storageOptions.Value;

    public async Task<ExportFileDto> ExportProductsCsvAsync(
        IReadOnlyList<ProductListItemDto> products,
        CancellationToken cancellationToken = default)
    {
        var lines = new List<string>
        {
            "ပစ္စည်းအမည်,မြန်မာအမည်,ပစ္စည်းကုဒ်,ဘားကုဒ်,အမျိုးအစား,အမျိုးအစား(မြန်မာ),အတိုင်းအတာ,ရောင်းဈေး,လက်ကျန်,ပြန်ဖြည့်ရန်အဆင့်"
        };

        lines.AddRange(products.Select(product => string.Join(",",
            Escape(product.Name),
            Escape(product.NameMm),
            Escape(product.Sku),
            Escape(product.Barcode),
            Escape(product.CategoryName),
            Escape(product.CategoryNameMm),
            Escape(product.UnitType.ToString()),
            Escape(product.SalePrice.ToString("0.##", CultureInfo.InvariantCulture)),
            Escape(product.StockOnHand.ToString("0.##", CultureInfo.InvariantCulture)),
            Escape(product.ReorderLevel.ToString("0.##", CultureInfo.InvariantCulture)))));

        return await SaveAsync(
            $"products-{DateTime.UtcNow:yyyy-MM-dd-HHmmss}.csv",
            string.Join(Environment.NewLine, lines),
            cancellationToken);
    }

    public async Task<ExportFileDto> ExportDailySalesCsvAsync(
        DailySalesReportDto report,
        CancellationToken cancellationToken = default)
    {
        var lines = new List<string>
        {
            "နေ့စွဲ,အရောင်းအရေအတွက်,မူလရောင်းအား,လျှော့ဈေး,အခွန်,စုစုပေါင်း ရောင်းအား",
            string.Join(",",
                Escape(report.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                Escape(report.SaleCount.ToString(CultureInfo.InvariantCulture)),
                Escape(report.GrossSales.ToString("0.##", CultureInfo.InvariantCulture)),
                Escape(report.DiscountAmount.ToString("0.##", CultureInfo.InvariantCulture)),
                Escape(report.TaxAmount.ToString("0.##", CultureInfo.InvariantCulture)),
                Escape(report.NetSales.ToString("0.##", CultureInfo.InvariantCulture)))
        };

        return await SaveAsync(
            $"daily-sales-{report.Date:yyyy-MM-dd}.csv",
            string.Join(Environment.NewLine, lines),
            cancellationToken);
    }

    private async Task<ExportFileDto> SaveAsync(string fileName, string csvContent, CancellationToken cancellationToken)
    {
        var exportDirectory = ResolvePath(_options.ExportDirectory);
        Directory.CreateDirectory(exportDirectory);

        var utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var content = utf8Bom.GetBytes(csvContent);
        var savedPath = Path.Combine(exportDirectory, fileName);

        await File.WriteAllBytesAsync(savedPath, content, cancellationToken);

        return new ExportFileDto
        {
            FileName = fileName,
            Content = content,
            SavedPath = savedPath
        };
    }

    private static string ResolvePath(string relativeOrAbsolute)
    {
        if (Path.IsPathRooted(relativeOrAbsolute))
        {
            return relativeOrAbsolute;
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeOrAbsolute));
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "\"\"";
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
