using HardwareStore.Domain.Common;
using HardwareStore.Domain.Enums;

namespace HardwareStore.Domain.Entities;

public sealed class AuditLog : EntityBase
{
    public AuditActionType ActionType { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? MetadataJson { get; set; }
}
