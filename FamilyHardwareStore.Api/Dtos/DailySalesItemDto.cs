using System;
namespace FamilyHardwareStore.Api.Dtos
{
    public class DailySalesItemDto
    {
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Profit { get; set; }
    }
}

