using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Purchases;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class PurchaseService(IAppDbContext dbContext, IAuditService auditService) : IPurchaseService
{
    public async Task<IReadOnlyList<PurchaseListItemDto>> GetPurchasesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Purchases
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Items)
            .OrderByDescending(x => x.PurchaseDateUtc)
            .Select(x => new PurchaseListItemDto
            {
                Id = x.Id,
                PurchaseNumber = x.PurchaseNumber,
                SupplierName = x.Supplier != null ? x.Supplier.Name : string.Empty,
                PurchaseDateUtc = x.PurchaseDateUtc,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                ItemCount = x.Items.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Purchases
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new PurchaseDetailDto
            {
                Id = x.Id,
                PurchaseNumber = x.PurchaseNumber,
                SupplierName = x.Supplier != null ? x.Supplier.Name : string.Empty,
                PurchaseDateUtc = x.PurchaseDateUtc,
                Status = x.Status,
                Subtotal = x.Subtotal,
                TaxAmount = x.TaxAmount,
                TotalAmount = x.TotalAmount,
                Notes = x.Notes,
                Items = x.Items.Select(i => new PurchaseDetailLineDto
                {
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceResult> CreateAsync(CreatePurchaseDto request, CancellationToken cancellationToken = default)
    {
        if (request.SupplierId == Guid.Empty || request.Items.Count == 0)
        {
            return ServiceResult.Failure("Supplier and at least one purchase item are required.");
        }

        await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        var purchase = new Purchase
        {
            PurchaseNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}",
            SupplierId = request.SupplierId,
            PurchaseDateUtc = DateTime.UtcNow,
            TaxAmount = request.TaxAmount,
            Notes = request.Notes?.Trim()
        };

        decimal subtotal = 0;
        foreach (var line in request.Items)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId, cancellationToken);
            if (product is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ServiceResult.Failure("One or more products were not found.");
            }

            var lineTotal = line.Quantity * line.UnitCost;
            subtotal += lineTotal;

            purchase.Items.Add(new PurchaseItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitCost = line.UnitCost,
                LineTotal = lineTotal
            });

            product.StockOnHand += line.Quantity;
            product.CostPrice = line.UnitCost;
            product.UpdatedUtc = DateTime.UtcNow;
            dbContext.Update(product);

            await dbContext.AddAsync(new InventoryMovement
            {
                ProductId = product.Id,
                MovementType = InventoryMovementType.Purchase,
                Quantity = line.Quantity,
                BalanceAfter = product.StockOnHand,
                ReferenceNumber = purchase.PurchaseNumber
            }, cancellationToken);
        }

        purchase.Subtotal = subtotal;
        purchase.TotalAmount = subtotal + request.TaxAmount;

        await dbContext.AddAsync(purchase, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await auditService.WriteAsync(
            AuditActionType.StockAdjustment,
            nameof(Purchase),
            purchase.Id.ToString(),
            null,
            $"Purchase {purchase.PurchaseNumber} posted.",
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
