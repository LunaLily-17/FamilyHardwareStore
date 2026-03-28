using HardwareStore.Application.DTOs.Inventory;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.DTOs.Reports;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Desktop.ViewModels;

public sealed class ShellViewModel(
    IProductService productService,
    IInventoryService inventoryService,
    IReportingService reportingService) : ObservableObject
{
    private string _welcomeText = "မိသားစု ဟာ့ဒ်ဝဲဆိုင်";
    private string _statusText = "အသင့်ဖြစ်ပါပြီ";
    private UserRole _role;

    public string WelcomeText
    {
        get => _welcomeText;
        private set => SetProperty(ref _welcomeText, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public UserRole Role
    {
        get => _role;
        private set => SetProperty(ref _role, value);
    }

    public IReadOnlyList<ProductListItemDto> Products { get; private set; } = [];
    public IReadOnlyList<StockOnHandDto> StockItems { get; private set; } = [];
    public DailySalesReportDto DailySales { get; private set; } = new();

    public bool IsAdmin => Role == UserRole.Admin;

    public async Task LoadAsync(string displayName, UserRole role)
    {
        WelcomeText = $"{displayName} ဖြင့် ဝင်ရောက်ထားပါသည်";
        Role = role;
        DailySales = await reportingService.GetDailySalesSummaryAsync(DateOnly.FromDateTime(DateTime.UtcNow));
        Products = await productService.GetProductsAsync();
        StockItems = await inventoryService.GetStockOnHandAsync();
        StatusText = $"ပစ္စည်း {Products.Count} မျိုးကို ဖတ်ပြီးပါပြီ။";
    }
}
