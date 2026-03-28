using System;
namespace FamilyHardwareStore.Api.Dtos
{
    public class CreateSaleRequest
    {
        public List<CreateSaleItemRequest> Items { get; set; } = new();
        public List<CreatePaymentRequest> Payments { get; set; } = new();
        public decimal? Discount { get; set; }
        public string? Note { get; set; }
    }
}


