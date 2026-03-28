using System;
namespace FamilyHardwareStore.Api.Dtos
{
	public class DailySalesReportDto
	{
		public DateTime Date { get; set; }
		public decimal TotalSales { get; set; }
		public decimal TotalProfit { get; set; }
		public int TotalTransactions { get; set; }

		public List<DailySalesItemDto> Items { get; set; } = new();
	}
}

