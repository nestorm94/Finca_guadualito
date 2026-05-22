# Prueba login en http://localhost:8080 (requiere sitio IIS activo)
$ErrorActionPreference = "Stop"
$base = "http://localhost:8080"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

$loginPage = Invoke-WebRequest -Uri "$base/Account/Login" -WebSession $session -UseBasicParsing
$token = if ($loginPage.InputFields) {
    ($loginPage.InputFields | Where-Object { $_.name -eq '__RequestVerificationToken' }).value
} else {
    [regex]::Match($loginPage.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value
}

if (-not $token) { throw "No se encontró token antiforgery" }

$body = @{
    Usuario              = "admin"
    Password             = "Admin123!"
    ReturnUrl            = "/"
    __RequestVerificationToken = $token
}

$response = Invoke-WebRequest -Uri "$base/Account/Login" -Method POST -Body $body -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 302) {
    Write-Host "LOGIN OK -> $($response.Headers.Location)"
} else {
    Write-Host "Status: $($response.StatusCode)"
    if ($response.Content -match "incorrectos") { Write-Host "Mensaje: Usuario o contraseña incorrectos" }
}
