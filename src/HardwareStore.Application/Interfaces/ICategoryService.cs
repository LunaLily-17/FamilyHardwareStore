using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Categories;

namespace HardwareStore.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> UpsertAsync(CategoryDto request, CancellationToken cancellationToken = default);
}
