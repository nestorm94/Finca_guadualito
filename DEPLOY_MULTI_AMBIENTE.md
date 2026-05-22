# Despliegue multi-ambiente — Registro Ganadero

Dos ambientes **completamente separados**: rutas, bases de datos, App Pools, cookies y claves de Data Protection.

| Concepto | Pruebas (Staging) | Producción |
|----------|-------------------|------------|
| `ASPNETCORE_ENVIRONMENT` | `Staging` | `Production` |
| Sitio IIS | `RegistroGanaderoTest` | `RegistroGanadero` |
| App Pool | `RegistroGanaderoTestPool` | `RegistroGanaderoPool` |
| Ruta publicación | `C:\Hosting\RegistroGanaderoTest` | `C:\Hosting\RegistroGanadero` |
| Base de datos | `DBRegistroGanadero_Test` | `DBRegistroGanadero` |
| Puerto HTTP | **8080** | **80** |
| Uploads | `C:\Hosting\Uploads\RegistroGanaderoTest` | `C:\Hosting\Uploads\RegistroGanadero` |
| Data Protection | `C:\Hosting\Data\RegistroGanaderoTest\...` | `C:\Hosting\Data\RegistroGanadero\...` |
| Cookie auth | `RegistroGanadero.Test.Auth` | `RegistroGanadero.Auth` |

**No se usa** `ObservatorioDB` ni rutas `Observatorio*`.

---

## 1. Archivos de configuración

| Archivo | Uso |
|---------|-----|
| `appsettings.json` | Valores base (logging, hosts) |
| `appsettings.Development.json` | `dotnet run` local → LocalDB |
| `appsettings.Staging.json` | IIS pruebas → `DBRegistroGanadero_Test` |
| `appsettings.Production.json` | IIS producción → `DBRegistroGanadero` |

ASP.NET Core carga `appsettings.{Environment}.json` según `ASPNETCORE_ENVIRONMENT`.

---

## 2. Publicar

### Pruebas (Staging)

```powershell
cd deploy
.\publish-staging.ps1
```

### Producción

```powershell
cd deploy
.\publish-production.ps1
```

Cada script:

- Publica en su carpeta (`C:\Hosting\...`)
- Crea carpetas `logs`, `Uploads` y `DataProtection-Keys` del ambiente
- Escribe `ASPNETCORE_ENVIRONMENT` en el `web.config` del destino (`Staging` o `Production`)

---

## 3. SQL Server (`.\SQLEXPRESS`)

### Permisos App Pool (ejecutar una vez por ambiente)

```powershell
sqlcmd -S .\SQLEXPRESS -i Scripts\IIS\001_AppPool_Permissions_Staging.sql
sqlcmd -S .\SQLEXPRESS -i Scripts\IIS\002_AppPool_Permissions_Production.sql
```

### Esquema en base de pruebas

Si `DBRegistroGanadero_Test` está vacía, copie datos desde producción (backup/restore) o ejecute en orden sobre **`DBRegistroGanadero_Test`**:

1. `Scripts/001_AddPasswordHash.sql` (cambiar `USE` a la base test)
2. `Scripts/002_ModuloReproductivo.sql`
3. `Scripts/003_HojaVida_TareasMasivas.sql`

**Importante:** edite la línea `USE` de cada script a `DBRegistroGanadero_Test` antes de ejecutar en pruebas.

---

## 4. Configurar IIS

Requisitos: **ASP.NET Core Hosting Bundle 10.x**, App Pools con **.NET CLR = No Managed Code**.

### Ambiente pruebas

1. **Application Pool** `RegistroGanaderoTestPool`
   - .NET CLR: *No Managed Code*
   - Identity: `ApplicationPoolIdentity`
   - Variable opcional (redundante si está en web.config): `ASPNETCORE_ENVIRONMENT` = `Staging`

2. **Sitio** `RegistroGanaderoTest`
   - Physical path: `C:\Hosting\RegistroGanaderoTest`
   - Binding: `http`, puerto **8080**, host name vacío (o el que use Cloudflare Tunnel)
   - App Pool: `RegistroGanaderoTestPool`

3. **Permisos NTFS** para `IIS AppPool\RegistroGanaderoTestPool`:
   - Lectura/ejecución: `C:\Hosting\RegistroGanaderoTest`
   - Modificar: `C:\Hosting\RegistroGanaderoTest\logs`
   - Modificar: `C:\Hosting\Data\RegistroGanaderoTest\DataProtection-Keys`
   - Modificar: `C:\Hosting\Uploads\RegistroGanaderoTest`

### Ambiente producción

1. **Application Pool** `RegistroGanaderoPool`
   - .NET CLR: *No Managed Code*
   - `ASPNETCORE_ENVIRONMENT` = `Production` (opcional en App Pool; ya va en web.config)

2. **Sitio** `RegistroGanadero`
   - Physical path: `C:\Hosting\RegistroGanadero`
   - Binding: `http`, puerto **80**
   - App Pool: `RegistroGanaderoPool`

3. **Permisos NTFS** para `IIS AppPool\RegistroGanaderoPool`:
   - Lectura/ejecución: `C:\Hosting\RegistroGanadero`
   - Modificar: `C:\Hosting\RegistroGanadero\logs`
   - Modificar: `C:\Hosting\Data\RegistroGanadero\DataProtection-Keys`
   - Modificar: `C:\Hosting\Uploads\RegistroGanadero`

---

## 5. Cloudflare Tunnel (opcional)

| Ambiente | URL local sugerida |
|----------|-------------------|
| Pruebas | `http://127.0.0.1:8080` |
| Producción | `http://127.0.0.1:80` |

Puede usar dos túneles o un solo hostname con reglas distintas; no mezcle bases ni carpetas.

---

## 6. Verificación rápida

```powershell
# Pruebas
$env:ASPNETCORE_ENVIRONMENT="Staging"
cd C:\Hosting\RegistroGanaderoTest
dotnet InventarioGanadero.Api.dll
# Debe conectar a DBRegistroGanadero_Test

# Producción
$env:ASPNETCORE_ENVIRONMENT="Production"
cd C:\Hosting\RegistroGanadero
dotnet InventarioGanadero.Api.dll
# Debe conectar a DBRegistroGanadero
```

URLs locales:

- Pruebas: http://localhost:8080
- Producción: http://localhost

---

## 7. Desarrollo local (`dotnet run`)

Sigue usando **Development** + LocalDB (`appsettings.Development.json`). No afecta a los sitios IIS.

```powershell
cd src\InventarioGanadero.Api
dotnet run
```

---

## 8. Reglas para no mezclar ambientes

1. Nunca publique pruebas en `C:\Hosting\RegistroGanadero` ni producción en `RegistroGanaderoTest`.
2. No reutilice la misma base: `_Test` solo en Staging, `DBRegistroGanadero` solo en Production.
3. No comparta carpetas `DataProtection-Keys` ni `Uploads`.
4. Use el script de publicación correcto (`publish-staging` vs `publish-production`).
5. Cookies distintas permiten estar logueado en ambos sitios en el mismo PC sin conflicto.

---

## 9. Comandos PowerShell IIS (referencia)

Ejecutar como **Administrador** si crea sitios por consola:

```powershell
Import-Module WebAdministration

# --- Pruebas ---
New-WebAppPool -Name "RegistroGanaderoTestPool" -Force
Set-ItemProperty "IIS:\AppPools\RegistroGanaderoTestPool" -Name managedRuntimeVersion -Value ""
New-Website -Name "RegistroGanaderoTest" -PhysicalPath "C:\Hosting\RegistroGanaderoTest" -Port 8080 -ApplicationPool "RegistroGanaderoTestPool" -Force

# --- Producción ---
New-WebAppPool -Name "RegistroGanaderoPool" -Force
Set-ItemProperty "IIS:\AppPools\RegistroGanaderoPool" -Name managedRuntimeVersion -Value ""
New-Website -Name "RegistroGanadero" -PhysicalPath "C:\Hosting\RegistroGanadero" -Port 80 -ApplicationPool "RegistroGanaderoPool" -Force
```

Si el puerto 80 está ocupado por **Default Web Site**, deténgalo o cambie su binding antes de crear `RegistroGanadero`.
