$dbPath = ".\src\HardwareStore.Web\App_Data\hardwarestore.db"
$backupDir = ".\src\HardwareStore.Web\backups"

if (-not (Test-Path $backupDir)) {
    New-Item -Path $backupDir -ItemType Directory | Out-Null
}

if (Test-Path $dbPath) {
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    Copy-Item $dbPath (Join-Path $backupDir "hardwarestore-$timestamp.db")
    Write-Host "Backup completed."
}
else {
    Write-Host "Database file not found: $dbPath"
}
