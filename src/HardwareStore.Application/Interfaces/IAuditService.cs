using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.Interfaces;

public interface IAuditService
{
    Task WriteAsync(
        AuditActionType actionType,
        string entityName,
        string? entityId,
        Guid? userId,
        string description,
        string? metadataJson = null,
        CancellationToken cancellationToken = default);
}
