using System;
namespace FamilyHardwareStore.Api.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string? NameMm { get; set; }
        public string? Barcode { get; set; }
        public int StockQuantity { get; set; }
        public decimal SellPrice { get; set; }
        public int ReorderLevel { get; set; }
    }
}

