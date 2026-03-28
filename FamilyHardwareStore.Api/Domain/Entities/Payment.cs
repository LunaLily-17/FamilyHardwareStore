using System;
using FamilyHardwareStore.Api.Domain.Enums;
namespace FamilyHardwareStore.Api.Domain.Entities
{
	public class Payment
	{
		public int Id { get; set; }

		public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } // "Cash", "KPay", "Wave"

		public DateTime PaidAt { get; set; } = DateTime.UtcNow;

		// Relation
		public int SaleId { get; set; }
		public Sale Sale { get; set; } = default!;
	}
}

