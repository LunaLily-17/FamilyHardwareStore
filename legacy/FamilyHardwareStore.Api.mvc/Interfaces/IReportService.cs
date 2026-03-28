using System;
using FamilyHardwareStore.Api.Dtos;

namespace FamilyHardwareStore.Api.Interfaces
{
	public interface IReportService
    { 
        Task<DailySalesReportDto> GetDailySalesAsync(DateTime date);
        Task<List<LowStockItemDto>> GetLowStockAsync();
    }
}

