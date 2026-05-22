# Publica ambiente PRUEBAS (Staging) -> C:\Hosting\RegistroGanaderoTest
# IIS: RegistroGanaderoTest / RegistroGanaderoTestPool / puerto 8080

$ErrorActionPreference = "Stop"

$OutputPath = "C:\Hosting\RegistroGanaderoTest"
$EnvironmentName = "Staging"
$AppPoolName = "RegistroGanaderoTestPool"

$root = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $root "src\InventarioGanadero.Api\InventarioGanadero.Api.csproj"

$folders = @(
    $OutputPath,
    "$OutputPath\logs",
    "C:\Hosting\Uploads\RegistroGanaderoTest",
    "C:\Hosting\Data\RegistroGanaderoTest\DataProtection-Keys"
)

foreach ($f in $folders) {
    if (-not (Test-Path $f)) {
        New-Item -ItemType Directory -Path $f -Force | Out-Null
        Write-Host "Creada: $f"
    }
}

Write-Host "Publicando STAGING -> $OutputPath"
dotnet publish $apiProject -c Release -o $OutputPath --no-self-contained

# web.config Staging: entorno + logs (plantilla explícita evita conflictos XML con el handler)
$webConfig = Join-Path $OutputPath "web.config"
@'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\InventarioGanadero.Api.dll"
                  stdoutLogEnabled="true"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="InProcess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Staging" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
'@ | Set-Content -Path $webConfig -Encoding UTF8

Write-Host ""
Write-Host "=== Ambiente PRUEBAS publicado ==="
Write-Host "Ruta:           $OutputPath"
Write-Host "Entorno:        $EnvironmentName"
Write-Host "Base de datos:  DBRegistroGanadero_Test"
Write-Host "Puerto IIS:     8080"
Write-Host "Sitio IIS:      RegistroGanaderoTest"
Write-Host "App Pool:       $AppPoolName"
Write-Host ""
Write-Host "SQL Server:     .\SQLEXPRESS2025 (temporal)"
Write-Host "Siguiente: ejecutar Scripts\IIS\001_AppPool_Permissions_Staging.sql en .\SQLEXPRESS2025"
