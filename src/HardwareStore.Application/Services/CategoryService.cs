using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Categories;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class CategoryService(IAppDbContext dbContext) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                NameMm = x.NameMm,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult> UpsertAsync(CategoryDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult.Failure("Category name is required.");
        }

        var duplicate = await dbContext.Categories.AnyAsync(
            x => x.Name == request.Name && x.Id != request.Id,
            cancellationToken);

        if (duplicate)
        {
            return ServiceResult.Failure("Category name must be unique.");
        }

        Category entity;
        if (request.Id.HasValue)
        {
            entity = await dbContext.Categories.FirstAsync(x => x.Id == request.Id.Value, cancellationToken);
            entity.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new Category();
            await dbContext.AddAsync(entity, cancellationToken);
        }

        entity.Name = request.Name.Trim();
        entity.NameMm = request.NameMm?.Trim();
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success();
    }
}
