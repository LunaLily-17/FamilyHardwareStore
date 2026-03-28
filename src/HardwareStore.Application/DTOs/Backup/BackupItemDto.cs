namespace HardwareStore.Application.DTOs.Backup;

public sealed class BackupItemDto
{
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public long SizeBytes { get; set; }
}
