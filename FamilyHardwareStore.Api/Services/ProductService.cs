using System;
using System.Data.Entity;
using FamilyHardwareStore.Api.Data;
using FamilyHardwareStore.Api.Domain.Entities;
using FamilyHardwareStore.Api.Dtos;
using FamilyHardwareStore.Api.Interfaces;

namespace FamilyHardwareStore.Api.Controllers
{
    public class ProductService : IProductService
    {
        private readonly HardwareStoreDbContext _db;
        public ProductService(HardwareStoreDbContext db) { _db = db; }

        public async Task<Product?> GetByIdAsync(int id) => await _db.Products.FindAsync(id);

        public async Task<List<Product>> SearchAsync(string? search, int page = 1, int pageSize = 20)
        {
            var q = _db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(p => p.NameEn.Contains(search) || (p.NameMm != null && p.NameMm.Contains(search)) || p.Sku.Contains(search));
            }
            return await q.OrderBy(p => p.NameEn).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Product> CreateAsync(CreateProductRequest dto)
        {
            var p = new Product
            {
                Sku = dto.Sku,
                NameEn = dto.NameEn,
                NameMm = dto.NameMm,
                Barcode = dto.Barcode,
                CostPrice = dto.CostPrice,
                SellPrice = dto.SellPrice,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                ReorderLevel = dto.ReorderLevel,
            };
            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            // initial stock movement if stock > 0
            if (p.StockQuantity > 0)
            {
                _db.StockMovements.Add(new Domain.Entities.StockMovement
                {
                    ProductId = p.Id,
                    MovementType = Domain.Enums.StockMovementType.PurchaseReceive,
                    Quantity = p.StockQuantity,
                    Reference = "InitialStock"
                });
                await _db.SaveChangesAsync();
            }
            return p;
        }

        public async Task UpdateAsync(int id, CreateProductRequest dto)
        {
            var p = await _db.Products.FindAsync(id) ?? throw new Exception("Product not found");
            p.NameEn = dto.NameEn;
            p.NameMm = dto.NameMm;
            p.Barcode = dto.Barcode;
            p.CostPrice = dto.CostPrice;
            p.SellPrice = dto.SellPrice;
            p.Sku = dto.Sku;
            p.ReorderLevel = dto.ReorderLevel;
            // update stock quantity carefully if needed (we'll keep simple)
            p.StockQuantity = dto.StockQuantity;
            p.CategoryId = dto.CategoryId;
            await _db.SaveChangesAsync();
        }
    }
}

