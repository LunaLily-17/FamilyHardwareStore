using System;
namespace FamilyHardwareStore.Api.Domain.Enums
{
	public enum PaymentMethod
    { 
        Cash = 0,
        BankTransfer = 1,
        Card = 2,
        MobileMoney = 3, // e.g. KBZ Pay, Wave
        Credit = 4
	}
}

