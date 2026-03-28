using System;
using FamilyHardwareStore.Api.Domain.Entities;
using FamilyHardwareStore.Api.Dtos;

namespace FamilyHardwareStore.Api.Interfaces
{
	public interface ISalesService
	{
        Task<Sale> CreateSaleAsync(CreateSaleRequest request);
        Task RefundAsync(int saleId); // simplified
    }
}

