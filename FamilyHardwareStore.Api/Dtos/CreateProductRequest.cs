using System;
namespace FamilyHardwareStore.Api.Dtos
{
    public class CreateProductRequest
    {
        public string Sku { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string? NameMm { get; set; }
        public string? Barcode { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int StockQuantity { get; set; }
        public int? CategoryId { get; set; }
        public int ReorderLevel { get; set; }
    }
}

