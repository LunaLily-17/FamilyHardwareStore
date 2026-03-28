using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Application.Services;

public sealed class AuditService(IAppDbContext dbContext) : IAuditService
{
    public async Task WriteAsync(
        AuditActionType actionType,
        string entityName,
        string? entityId,
        Guid? userId,
        string description,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        var entry = new AuditLog
        {
            ActionType = actionType,
            EntityName = entityName,
            EntityId = entityId,
            UserId = userId,
            Description = description,
            MetadataJson = metadataJson
        };

        await dbContext.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
