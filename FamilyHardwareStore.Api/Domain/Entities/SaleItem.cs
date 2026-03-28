using System;
namespace FamilyHardwareStore.Api.Domain.Entities
{
	public class SaleItem
	{
		public int id { get; set; }

		// Product
		public int ProductId { get; set; }
		public Product Product { get; set; } = default!;

		// Price at time of sale (don't rely on Product.SellPrice)
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal Subtotal => UnitPrice * Quantity;

		// Sale relation
		public int SaleId { get; set; }
		public Sale Sale { get; set; } = default!;
	}
}

