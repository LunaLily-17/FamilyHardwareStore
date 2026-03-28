using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Sales;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class SalesService(IAppDbContext dbContext, IAuditService auditService) : ISalesService
{
    public async Task<CreateSaleResultDto> CreateAsync(CreateSaleDto request, CancellationToken cancellationToken = default)
    {
        if (request.CashierId == Guid.Empty || request.Items.Count == 0)
        {
            return CreateSaleResultDto.Failure("Cashier and sale items are required.");
        }

        await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        var sale = new Sale
        {
            CashierId = request.CashierId,
            PaymentMethod = request.PaymentMethod,
            DiscountAmount = request.DiscountAmount,
            TaxAmount = request.TaxAmount,
            ReceiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Notes = request.Notes?.Trim(),
            SaleDateUtc = DateTime.UtcNow
        };

        decimal subtotal = 0;
        foreach (var line in request.Items)
        {
            if (line.Quantity <= 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return CreateSaleResultDto.Failure("Item quantity must be greater than zero.");
            }

            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId, cancellationToken);
            if (product is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return CreateSaleResultDto.Failure("One or more products were not found.");
            }

            if (product.StockOnHand < line.Quantity)
            {
                await transaction.RollbackAsync(cancellationToken);
                return CreateSaleResultDto.Failure($"Insufficient stock for {product.Name}.");
            }

            var lineTotal = product.SalePrice * line.Quantity;
            subtotal += lineTotal;

            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.SalePrice,
                LineTotal = lineTotal
            });

            product.StockOnHand -= line.Quantity;
            product.UpdatedUtc = DateTime.UtcNow;
            dbContext.Update(product);

            await dbContext.AddAsync(new InventoryMovement
            {
                ProductId = product.Id,
                MovementType = InventoryMovementType.Sale,
                Quantity = -line.Quantity,
                BalanceAfter = product.StockOnHand,
                ReferenceNumber = sale.ReceiptNumber
            }, cancellationToken);
        }

        sale.Subtotal = subtotal;
        sale.TotalAmount = subtotal - request.DiscountAmount + request.TaxAmount;

        await dbContext.AddAsync(sale, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreateSaleResultDto.Success(sale.ReceiptNumber);
    }

    public async Task<SaleReceiptDto?> GetReceiptAsync(string receiptNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.Sales
            .AsNoTracking()
            .Include(x => x.Cashier)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .Where(x => x.ReceiptNumber == receiptNumber)
            .Select(x => new SaleReceiptDto
            {
                SaleId = x.Id,
                ReceiptNumber = x.ReceiptNumber,
                SaleDateUtc = x.SaleDateUtc,
                CashierName = x.Cashier != null ? x.Cashier.DisplayName : string.Empty,
                TotalAmount = x.TotalAmount,
                Items = x.Items.Select(i => new SaleReceiptLineDto
                {
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    ProductNameMm = i.Product != null ? i.Product.NameMm : null,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceResult> VoidAsync(string receiptNumber, Guid userId, CancellationToken cancellationToken = default)
    {
        var sale = await dbContext.Sales
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.ReceiptNumber == receiptNumber, cancellationToken);

        if (sale is null)
        {
            return ServiceResult.Failure("Sale not found.");
        }

        if (sale.Status == SaleStatus.Voided)
        {
            return ServiceResult.Failure("Sale is already voided.");
        }

        await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        foreach (var item in sale.Items)
        {
            var product = await dbContext.Products.FirstAsync(x => x.Id == item.ProductId, cancellationToken);
            product.StockOnHand += item.Quantity;
            product.UpdatedUtc = DateTime.UtcNow;
            dbContext.Update(product);

            await dbContext.AddAsync(new InventoryMovement
            {
                ProductId = product.Id,
                MovementType = InventoryMovementType.Return,
                Quantity = item.Quantity,
                BalanceAfter = product.StockOnHand,
                ReferenceNumber = sale.ReceiptNumber,
                Notes = "Sale voided"
            }, cancellationToken);
        }

        sale.Status = SaleStatus.Voided;
        sale.VoidedUtc = DateTime.UtcNow;
        sale.VoidedByUserId = userId;
        sale.UpdatedUtc = DateTime.UtcNow;
        dbContext.Update(sale);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await auditService.WriteAsync(
            AuditActionType.SaleVoided,
            nameof(Sale),
            sale.Id.ToString(),
            userId,
            $"Sale {sale.ReceiptNumber} was voided.",
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
