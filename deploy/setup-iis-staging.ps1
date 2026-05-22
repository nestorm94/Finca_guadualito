# Ejecutar PowerShell COMO ADMINISTRADOR
# Crea sitio RegistroGanaderoTest en puerto 8080

$ErrorActionPreference = "Stop"
$OutputPath = "C:\Hosting\RegistroGanaderoTest"
$SiteName = "RegistroGanaderoTest"
$PoolName = "RegistroGanaderoTestPool"
$Port = 8080

Import-Module WebAdministration

if (-not (Test-Path $OutputPath)) {
    throw "No existe $OutputPath. Ejecute primero publish-staging.ps1"
}

if (-not (Test-Path "IIS:\AppPools\$PoolName")) {
    New-WebAppPool -Name $PoolName | Out-Null
    Set-ItemProperty "IIS:\AppPools\$PoolName" -Name managedRuntimeVersion -Value ""
    Write-Host "App Pool creado: $PoolName"
}

if (Test-Path "IIS:\Sites\$SiteName") {
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name physicalPath -Value $OutputPath
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name applicationPool -Value $PoolName
    Write-Host "Sitio actualizado: $SiteName"
} else {
    New-Website -Name $SiteName -PhysicalPath $OutputPath -Port $Port -ApplicationPool $PoolName | Out-Null
    Write-Host "Sitio creado: $SiteName en puerto $Port"
}

# Permisos NTFS para el App Pool
$aclIdentity = "IIS AppPool\$PoolName"
$paths = @(
    $OutputPath,
    "$OutputPath\logs",
    "C:\Hosting\Data\RegistroGanaderoTest\DataProtection-Keys",
    "C:\Hosting\Uploads\RegistroGanaderoTest"
)
foreach ($p in $paths) {
    if (-not (Test-Path $p)) { New-Item -ItemType Directory -Path $p -Force | Out-Null }
    icacls $p /grant "${aclIdentity}:(OI)(CI)M" /T 2>&1 | Out-Null
}

Start-WebAppPool -Name $PoolName
Write-Host "Listo. Probar: http://localhost:$Port"
