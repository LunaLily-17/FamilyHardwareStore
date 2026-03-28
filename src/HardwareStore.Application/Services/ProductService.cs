using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Products;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class ProductService(IAppDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .Select(x => new ProductListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                NameMm = x.NameMm,
                Sku = x.Sku,
                Barcode = x.Barcode,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                CategoryNameMm = x.Category != null ? x.Category.NameMm : null,
                UnitType = x.UnitType,
                SalePrice = x.SalePrice,
                StockOnHand = x.StockOnHand,
                ReorderLevel = x.ReorderLevel
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult> UpsertAsync(ProductUpsertDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult.Failure("Product name is required.");
        }

        if (request.SalePrice < 0 || request.CostPrice < 0)
        {
            return ServiceResult.Failure("Prices cannot be negative.");
        }

        var duplicateSku = !string.IsNullOrWhiteSpace(request.Sku)
            && await dbContext.Products.AnyAsync(
                x => x.Sku == request.Sku && x.Id != request.Id,
                cancellationToken);

        if (duplicateSku)
        {
            return ServiceResult.Failure("SKU must be unique.");
        }

        Product entity;
        if (request.Id.HasValue)
        {
            entity = await dbContext.Products.FirstAsync(x => x.Id == request.Id.Value, cancellationToken);
            entity.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new Product();
            await dbContext.AddAsync(entity, cancellationToken);
        }

        entity.Name = request.Name.Trim();
        entity.NameMm = request.NameMm?.Trim();
        entity.Sku = request.Sku?.Trim();
        entity.Barcode = request.Barcode?.Trim();
        entity.Description = request.Description?.Trim();
        entity.CategoryId = request.CategoryId;
        entity.UnitType = request.UnitType;
        entity.CostPrice = request.CostPrice;
        entity.SalePrice = request.SalePrice;
        entity.ReorderLevel = request.ReorderLevel;
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }
}
