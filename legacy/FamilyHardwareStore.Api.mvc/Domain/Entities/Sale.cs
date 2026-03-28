using System;
namespace FamilyHardwareStore.Api.Domain.Entities
{
	// Domain/Entities/Sale.cs
	public class Sale
	{
		public int Id { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public decimal TotalAmount { get; set; }
		public decimal TotalCost { get; set; } // for profit calculation
		public decimal? Discount { get; set; }
		public string? Note { get; set; }

        // Navigation
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
}

