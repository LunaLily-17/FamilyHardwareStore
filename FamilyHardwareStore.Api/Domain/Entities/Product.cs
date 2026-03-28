using System;
namespace FamilyHardwareStore.Api.Domain.Entities
{
	// Domain/Entities/Product.cs
	public class Product
	{
		public int Id { get; set; }

		// Identifiers
		public string Sku { get; set; } = default!;		// required
		public string NameEn { get; set; } = default!;  // required
		public string? NameMm { get; set; }				// optional
		public string? Barcode { get; set; }			// optional

		// Stock
		public int StockQuantity { get; set; }
		public int ReorderLevel { get; set; }

        // Pricing
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }

		// Category relation (optional)
        public int? CategoryId { get; set; }
		public Category? Category { get; set; }

		public bool IsActive { get; set; } = true;

		// Navigation to SaleItems + PurchaseItems
		public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }

	
}

