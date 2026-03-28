using HardwareStore.Application.DTOs.Sales;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Sales;

[Authorize(Policy = "CashierOrAdmin")]
public sealed class ReceiptModel(ISalesService salesService, ISettingsService settingsService) : PageModel
{
    public SaleReceiptDto? Receipt { get; private set; }
    public string ShopName { get; private set; } = "Family Hardware Store";
    public string? ShopNameMm { get; private set; }
    public string? ShopAddress { get; private set; }
    public string? ShopPhone { get; private set; }
    public string? ReceiptFooter { get; private set; }

    public async Task OnGetAsync(string receiptNumber, CancellationToken cancellationToken)
    {
        Receipt = await salesService.GetReceiptAsync(receiptNumber, cancellationToken);
        var settings = await settingsService.GetAllAsync(cancellationToken);
        ShopName = settings.FirstOrDefault(x => x.Key == "ShopName")?.Value ?? ShopName;
        ShopNameMm = settings.FirstOrDefault(x => x.Key == "ShopNameMm")?.Value;
        ShopAddress = settings.FirstOrDefault(x => x.Key == "ShopAddress")?.Value;
        ShopPhone = settings.FirstOrDefault(x => x.Key == "ShopPhone")?.Value;
        ReceiptFooter = settings.FirstOrDefault(x => x.Key == "ReceiptFooter")?.Value;
    }
}
