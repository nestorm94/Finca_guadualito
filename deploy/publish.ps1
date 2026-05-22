# Publica Registro Ganadero (ASP.NET Core MVC) para IIS
# Ejecutar en PowerShell como administrador si crea carpetas en C:\Hosting

param(
    [string]$OutputPath = "C:\Hosting\ObservatorioWEB",
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$apiProject = Join-Path $root "src\InventarioGanadero.Api\InventarioGanadero.Api.csproj"

$folders = @(
    "C:\Hosting\ObservatorioWEB",
    "C:\Hosting\ObservatorioAPI",
    "C:\Hosting\Uploads\RegistroGanadero",
    "C:\Hosting\Data\RegistroGanadero\DataProtection-Keys",
    "C:\Hosting\Logs\RegistroGanadero"
)

foreach ($f in $folders) {
    if (-not (Test-Path $f)) {
        New-Item -ItemType Directory -Path $f -Force | Out-Null
        Write-Host "Creada carpeta: $f"
    }
}

Write-Host "Publicando $apiProject -> $OutputPath"
dotnet publish $apiProject -c $Configuration -o $OutputPath --no-self-contained

# Carpeta ObservatorioAPI: este proyecto es MVC monolítico (no hay API separada).
# Opcional: copia espejo para convención de nombres del checklist.
$apiMirror = "C:\Hosting\ObservatorioAPI"
if ($OutputPath -ne $apiMirror) {
    Write-Host "Copia espejo en $apiMirror (opcional, misma app)"
    robocopy $OutputPath $apiMirror /MIR /NFL /NDL /NJH /NJS /nc /ns /np | Out-Null
}

Write-Host ""
Write-Host "Publicacion completada."
Write-Host "Siguiente: configurar sitio IIS apuntando a $OutputPath"
Write-Host "Variable de entorno del App Pool: ASPNETCORE_ENVIRONMENT=Production"
