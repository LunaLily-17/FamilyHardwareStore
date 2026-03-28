using HardwareStore.Application.Common;
using HardwareStore.Application.DTOs.Backup;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Enums;
using HardwareStore.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace HardwareStore.Infrastructure.Services;

public sealed class BackupService(IOptions<StorageOptions> storageOptions, IAuditService auditService) : IBackupService
{
    private readonly StorageOptions _options = storageOptions.Value;

    public Task<IReadOnlyList<BackupItemDto>> GetBackupsAsync(CancellationToken cancellationToken = default)
    {
        var backupDirectory = ResolvePath(_options.BackupDirectory);
        Directory.CreateDirectory(backupDirectory);

        IReadOnlyList<BackupItemDto> backups = Directory.GetFiles(backupDirectory, "*.db")
            .Select(path => new FileInfo(path))
            .OrderByDescending(x => x.CreationTimeUtc)
            .Select(x => new BackupItemDto
            {
                FileName = x.Name,
                FullPath = x.FullName,
                CreatedUtc = x.CreationTimeUtc,
                SizeBytes = x.Length
            })
            .ToList();

        return Task.FromResult(backups);
    }

    public async Task<ServiceResult> CreateBackupAsync(CancellationToken cancellationToken = default)
    {
        var dataDirectory = ResolvePath(_options.DataDirectory);
        var backupDirectory = ResolvePath(_options.BackupDirectory);
        Directory.CreateDirectory(dataDirectory);
        Directory.CreateDirectory(backupDirectory);

        var sourcePath = Path.Combine(dataDirectory, _options.DatabaseFileName);
        if (!File.Exists(sourcePath))
        {
            return ServiceResult.Failure("No database file exists yet.");
        }

        var backupName = $"{Path.GetFileNameWithoutExtension(_options.DatabaseFileName)}-{DateTime.UtcNow:yyyyMMddHHmmss}.db";
        var destinationPath = Path.Combine(backupDirectory, backupName);
        File.Copy(sourcePath, destinationPath, overwrite: false);

        await auditService.WriteAsync(
            AuditActionType.BackupCreated,
            "Database",
            null,
            null,
            $"Backup created at '{destinationPath}'.",
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    private static string ResolvePath(string relativeOrAbsolute)
    {
        if (Path.IsPathRooted(relativeOrAbsolute))
        {
            return relativeOrAbsolute;
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeOrAbsolute));
    }
}
