using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Backup;

namespace HardwareStore.Application.Interfaces;

public interface IBackupService
{
    Task<IReadOnlyList<BackupItemDto>> GetBackupsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> CreateBackupAsync(CancellationToken cancellationToken = default);
}
