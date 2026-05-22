# Publica ambiente PRODUCCIÓN -> C:\Hosting\RegistroGanadero
# IIS: RegistroGanadero / RegistroGanaderoPool / puerto 80

$ErrorActionPreference = "Stop"

$OutputPath = "C:\Hosting\RegistroGanadero"
$EnvironmentName = "Production"
$AppPoolName = "RegistroGanaderoPool"

$root = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $root "src\InventarioGanadero.Api\InventarioGanadero.Api.csproj"

$folders = @(
    $OutputPath,
    "$OutputPath\logs",
    "C:\Hosting\Uploads\RegistroGanadero",
    "C:\Hosting\Data\RegistroGanadero\DataProtection-Keys"
)

foreach ($f in $folders) {
    if (-not (Test-Path $f)) {
        New-Item -ItemType Directory -Path $f -Force | Out-Null
        Write-Host "Creada: $f"
    }
}

Write-Host "Publicando PRODUCTION -> $OutputPath"
dotnet publish $apiProject -c Release -o $OutputPath --no-self-contained

$webConfig = Join-Path $OutputPath "web.config"
[xml]$xml = Get-Content $webConfig
$aspNetCore = $xml.configuration.location.system.webServer.aspNetCore
if (-not $aspNetCore.environmentVariables) {
    $envVars = $xml.CreateElement("environmentVariables")
    $aspNetCore.AppendChild($envVars) | Out-Null
}
$existing = $aspNetCore.environmentVariables.environmentVariable | Where-Object { $_.name -eq "ASPNETCORE_ENVIRONMENT" }
if ($existing) {
    $existing.value = $EnvironmentName
} else {
    $var = $xml.CreateElement("environmentVariable")
    $var.SetAttribute("name", "ASPNETCORE_ENVIRONMENT")
    $var.SetAttribute("value", $EnvironmentName)
    $aspNetCore.environmentVariables.AppendChild($var) | Out-Null
}
$xml.Save($webConfig)

Write-Host ""
Write-Host "=== Ambiente PRODUCCIÓN publicado ==="
Write-Host "Ruta:           $OutputPath"
Write-Host "Entorno:        $EnvironmentName"
Write-Host "Base de datos:  DBRegistroGanadero"
Write-Host "Puerto IIS:     80"
Write-Host "Sitio IIS:      RegistroGanadero"
Write-Host "App Pool:       $AppPoolName"
Write-Host ""
Write-Host "Siguiente: ejecutar Scripts\IIS\002_AppPool_Permissions_Production.sql en .\SQLEXPRESS"
