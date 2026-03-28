using System;
using FamilyHardwareStore.Api.Domain.Enums;
namespace FamilyHardwareStore.Api.Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public StockMovementType MovementType { get; set; }
        public int Quantity { get; set; }    // positive numbers (debit/credit meaning depends on MovementType)
        public string? Reference { get; set; } // e.g. "Sale:123", "Purchase:456"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

