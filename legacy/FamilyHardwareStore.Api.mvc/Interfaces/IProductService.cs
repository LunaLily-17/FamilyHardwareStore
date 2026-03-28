using System;
using FamilyHardwareStore.Api.Domain.Entities;
using FamilyHardwareStore.Api.Dtos;

namespace FamilyHardwareStore.Api.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> SearchAsync(string? search, int page = 1, int pageSize = 20);
        Task<Product> CreateAsync(CreateProductRequest dto);
        Task UpdateAsync(int id, CreateProductRequest dto);
    }
}

