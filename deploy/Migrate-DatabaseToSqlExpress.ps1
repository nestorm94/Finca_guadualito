# Migra DBRegistroGanadero de LocalDB a SQL Server Express (.\SQLEXPRESS)
# Requiere: sqlcmd en PATH, instancia SQLEXPRESS en ejecución, permisos de sysadmin o backup/restore.

param(
    [string]$DatabaseName = "DBRegistroGanadero",
    [string]$LocalDbInstance = "(localdb)\MSSQLLocalDB",
    [string]$ExpressInstance = ".\SQLEXPRESS",
    [string]$BackupFolder = "C:\Hosting\Backups"
)

$ErrorActionPreference = "Stop"
New-Item -ItemType Directory -Path $BackupFolder -Force | Out-Null

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$bak = Join-Path $BackupFolder "$DatabaseName`_$timestamp.bak"

Write-Host "1. Backup desde LocalDB..."
sqlcmd -S $LocalDbInstance -Q "BACKUP DATABASE [$DatabaseName] TO DISK = N'$bak' WITH INIT, FORMAT;" 
if ($LASTEXITCODE -ne 0) { throw "Fallo backup LocalDB. ¿Existe la base $DatabaseName en LocalDB?" }

Write-Host "2. Restaurar en $ExpressInstance..."
$sql = @"
IF DB_ID(N'$DatabaseName') IS NOT NULL
BEGIN
    ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$DatabaseName];
END
RESTORE DATABASE [$DatabaseName] FROM DISK = N'$bak'
WITH MOVE N'$DatabaseName' TO N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\$DatabaseName.mdf',
     MOVE N'${DatabaseName}_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\${DatabaseName}_log.ldf',
     REPLACE;
"@
# NOTA: Ajuste rutas MOVE según nombres lógicos reales: 
#   RESTORE FILELISTONLY FROM DISK = N'$bak'
Write-Host "IMPORTANTE: Ejecute RESTORE FILELISTONLY para obtener nombres lógicos y ajuste MOVE en este script."
Write-Host "Backup guardado en: $bak"
Write-Host ""
Write-Host "Alternativa manual en SSMS: Backup LocalDB -> Restore en .\SQLEXPRESS"
Write-Host "Luego ejecute Scripts\001, 002, 003 en la base restaurada si faltan tablas."
