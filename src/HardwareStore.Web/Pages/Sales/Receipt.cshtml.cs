using HardwareStore.Application.DTOs.Sales;
using HardwareStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HardwareStore.Web.Pages.Sales;

[Authorize(Policy = "CashierOrAdmin")]
public sealed class ReceiptModel(ISalesService salesService, ISettingsService settingsService) : PageModel
{
    private const string DefaultShopName = "မိသားစု အိမ်ဆောက်ပစ္စည်းဆိုင်";
    private const string DefaultShopAddress = "ရန်ကုန်-မန္တလေးလမ်းဟောင်း နှစ်ဆောင်မြိုင် (ရွှေဝါဆီဆိုင်ရှေ့) တောင်ငူမြို့";
    private static readonly string[] DefaultPhoneNumbers = ["095344098", "09752344098", "09752744098", "09688373092"];

    [BindProperty(SupportsGet = true)]
    public bool AutoPrint { get; set; }

    public SaleReceiptDto? Receipt { get; private set; }
    public string ShopName { get; private set; } = DefaultShopName;
    public string ShopAddress { get; private set; } = DefaultShopAddress;
    public IReadOnlyList<string> ShopPhoneNumbers { get; private set; } = DefaultPhoneNumbers;
    public string? ReceiptFooter { get; private set; }

    public async Task OnGetAsync(string receiptNumber, CancellationToken cancellationToken)
    {
        Receipt = await salesService.GetReceiptAsync(receiptNumber, cancellationToken);
        var settings = await settingsService.GetAllAsync(cancellationToken);
        ShopName = NormalizeShopName(settings.FirstOrDefault(x => x.Key == "ShopName")?.Value);
        ShopAddress = NormalizeShopAddress(settings.FirstOrDefault(x => x.Key == "ShopAddress")?.Value);
        ShopPhoneNumbers = NormalizePhones(settings.FirstOrDefault(x => x.Key == "ShopPhone")?.Value);
        ReceiptFooter = settings.FirstOrDefault(x => x.Key == "ReceiptFooter")?.Value;
    }

    private static string NormalizeShopName(string? configuredValue)
    {
        if (string.IsNullOrWhiteSpace(configuredValue) ||
            string.Equals(configuredValue.Trim(), "မိသားစု ဟာ့ဒ်ဝဲဆိုင်", StringComparison.Ordinal))
        {
            return DefaultShopName;
        }

        return configuredValue.Trim();
    }

    private static string NormalizeShopAddress(string? configuredValue)
    {
        if (string.IsNullOrWhiteSpace(configuredValue) ||
            string.Equals(configuredValue.Trim(), "Main Street", StringComparison.OrdinalIgnoreCase))
        {
            return DefaultShopAddress;
        }

        return configuredValue.Trim();
    }

    private static IReadOnlyList<string> NormalizePhones(string? configuredValue)
    {
        if (string.IsNullOrWhiteSpace(configuredValue) ||
            string.Equals(configuredValue.Trim(), "+44 0000 000000", StringComparison.OrdinalIgnoreCase))
        {
            return DefaultPhoneNumbers;
        }

        var values = configuredValue
            .Split(['\r', '\n', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return values.Length > 0 ? values : DefaultPhoneNumbers;
    }
}
