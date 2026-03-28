using System;
using FamilyHardwareStore.Api.Domain.Enums;

namespace FamilyHardwareStore.Api.Dtos
{
    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
    }
}

