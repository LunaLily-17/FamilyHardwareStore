using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Inventory;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class InventoryService(IAppDbContext dbContext, IAuditService auditService) : IInventoryService
{
    public async Task<IReadOnlyList<StockOnHandDto>> GetStockOnHandAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .Select(x => new StockOnHandDto
            {
                ProductId = x.Id,
                ProductName = x.Name,
                ProductNameMm = x.NameMm,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                CategoryNameMm = x.Category != null ? x.Category.NameMm : null,
                StockOnHand = x.StockOnHand,
                ReorderLevel = x.ReorderLevel
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult> AdjustStockAsync(StockAdjustmentDto request, CancellationToken cancellationToken = default)
    {
        if (request.ProductId == Guid.Empty || request.NewQuantity < 0 || string.IsNullOrWhiteSpace(request.Reason))
        {
            return ServiceResult.Failure("Product, quantity, and reason are required.");
        }

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);
        if (product is null)
        {
            return ServiceResult.Failure("Product not found.");
        }

        var previousQuantity = product.StockOnHand;
        var delta = request.NewQuantity - previousQuantity;
        product.StockOnHand = request.NewQuantity;
        product.UpdatedUtc = DateTime.UtcNow;
        dbContext.Update(product);

        await dbContext.AddAsync(new StockAdjustment
        {
            ProductId = product.Id,
            PreviousQuantity = previousQuantity,
            AdjustedQuantity = delta,
            NewQuantity = request.NewQuantity,
            Reason = request.Reason.Trim(),
            AdjustedByUserId = request.AdjustedByUserId
        }, cancellationToken);

        await dbContext.AddAsync(new InventoryMovement
        {
            ProductId = product.Id,
            MovementType = delta >= 0 ? InventoryMovementType.AdjustmentIn : InventoryMovementType.AdjustmentOut,
            Quantity = delta,
            BalanceAfter = request.NewQuantity,
            Notes = request.Reason.Trim()
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await auditService.WriteAsync(
            AuditActionType.StockAdjustment,
            nameof(Product),
            product.Id.ToString(),
            request.AdjustedByUserId,
            $"Stock adjusted for {product.Name} from {previousQuantity} to {request.NewQuantity}.",
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
