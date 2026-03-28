using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Settings;

namespace HardwareStore.Application.Interfaces;

public interface ISettingsService
{
    Task<IReadOnlyList<AppSettingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> SaveAsync(AppSettingDto request, CancellationToken cancellationToken = default);
}
