using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Suppliers;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class SupplierService(IAppDbContext dbContext) : ISupplierService
{
    public async Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                ContactPerson = x.ContactPerson,
                Phone = x.Phone,
                Address = x.Address,
                Email = x.Email,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult> UpsertAsync(SupplierDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult.Failure("Supplier name is required.");
        }

        Supplier entity;
        if (request.Id.HasValue)
        {
            entity = await dbContext.Suppliers.FirstAsync(x => x.Id == request.Id.Value, cancellationToken);
            entity.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new Supplier();
            await dbContext.AddAsync(entity, cancellationToken);
        }

        entity.Name = request.Name.Trim();
        entity.ContactPerson = request.ContactPerson?.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Address = request.Address?.Trim();
        entity.Email = request.Email?.Trim();
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }
}
