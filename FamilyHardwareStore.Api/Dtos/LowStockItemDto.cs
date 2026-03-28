using System;
namespace FamilyHardwareStore.Api.Dtos
{
	public class LowStockItemDto
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = default!;
		public int StockQuantity { get; set; }
		public int ReorderLevel { get; set; }
	}
}

