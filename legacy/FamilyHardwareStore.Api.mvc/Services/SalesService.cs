using System;
using System.Data.Entity;
using FamilyHardwareStore.Api.Data;
using FamilyHardwareStore.Api.Domain.Entities;
using FamilyHardwareStore.Api.Domain.Enums;
using FamilyHardwareStore.Api.Dtos;
using FamilyHardwareStore.Api.Interfaces;

namespace FamilyHardwareStore.Api.Controllers
{
    public class SalesService : ISalesService
    {
        private readonly HardwareStoreDbContext _db;
        public SalesService(HardwareStoreDbContext db) { _db = db; }

        public async Task<Sale> CreateSaleAsync(CreateSaleRequest request)
        {
            if (request.Items == null || request.Items.Count == 0) throw new ArgumentException("Sale must have items");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var sale = new Sale
                {
                    CreatedAt = DateTime.UtcNow,
                    Discount = request.Discount,
                    Note = request.Note
                };

                decimal total = 0m;
                foreach (var item in request.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId) ??
                        throw new Exception($"Product {item.ProductId} not found");
                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for product {product.NameEn}");

                    var si = new SaleItem
                    {
                        ProductId = product.Id,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    };
                    sale.Items.Add(si);

                    // decrement stock
                    product.StockQuantity -= item.Quantity;

                    // create stock movement record
                    _db.StockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        MovementType = StockMovementType.Sale,
                        Quantity = item.Quantity,
                        Reference = null
                    });

                    total += item.UnitPrice * item.Quantity;
                }

                if (request.Discount.HasValue)
                {
                    total -= request.Discount.Value;
                }
                sale.TotalAmount = total;

                _db.Sales.Add(sale);
                await _db.SaveChangesAsync();

                // payments
                foreach (var p in request.Payments)
                {
                    var payment = new Payment
                    {
                        Amount = p.Amount,
                        Method = p.Method,
                        SaleId = sale.Id,
                        PaidAt = DateTime.UtcNow
                    };
                    _db.Payments.Add(payment);
                }
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return sale;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task RefundAsync(int saleId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var sale = await _db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == saleId) ?? throw new Exception("Sale not found");
                // For simple refund: increment product stock and create stock movement
                foreach (var item in sale.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId) ?? throw new Exception("Product missing");
                    product.StockQuantity += item.Quantity;
                    _db.StockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        MovementType = StockMovementType.Refund,
                        Quantity = item.Quantity,
                        Reference = $"Refund:{saleId}"
                    });
                }

                // you might also create a Refund record or mark sale refunded. For now we just record stock and leave sale as-is.
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}

