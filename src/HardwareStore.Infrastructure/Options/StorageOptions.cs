namespace HardwareStore.Infrastructure.Options;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string DataDirectory { get; set; } = "App_Data";
    public string BackupDirectory { get; set; } = "backups";
    public string DatabaseFileName { get; set; } = "hardwarestore.db";
    public string LogDirectory { get; set; } = "logs";
}
