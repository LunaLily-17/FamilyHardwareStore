using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Settings;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class SettingsService(IAppDbContext dbContext, IAuditService auditService) : ISettingsService
{
    public async Task<IReadOnlyList<AppSettingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AppSettings
            .AsNoTracking()
            .OrderBy(x => x.Key)
            .Select(x => new AppSettingDto
            {
                Key = x.Key,
                Value = x.Value,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult> SaveAsync(AppSettingDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
        {
            return ServiceResult.Failure("Setting key is required.");
        }

        var entity = await dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == request.Key, cancellationToken);
        if (entity is null)
        {
            entity = new AppSetting { Key = request.Key.Trim() };
            await dbContext.AddAsync(entity, cancellationToken);
        }

        entity.Value = request.Value?.Trim() ?? string.Empty;
        entity.Description = request.Description?.Trim();
        entity.UpdatedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditService.WriteAsync(
            AuditActionType.SettingsChanged,
            nameof(AppSetting),
            entity.Id.ToString(),
            null,
            $"Setting '{entity.Key}' updated.",
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
