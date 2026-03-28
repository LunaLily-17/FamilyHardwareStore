using System;
namespace FamilyHardwareStore.Api.Dtos
{
    public class CreateSaleItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // price charged per unit
    }
}

