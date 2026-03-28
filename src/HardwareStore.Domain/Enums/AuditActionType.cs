namespace HardwareStore.Domain.Enums;

public enum AuditActionType
{
    LoginAttempt = 1,
    ProductPriceChanged = 2,
    StockAdjustment = 3,
    SaleVoided = 4,
    SettingsChanged = 5,
    BackupCreated = 6,
    BackupRestored = 7
}
