# Finca Guadualito — Registro Ganadero

Sistema web **ASP.NET Core MVC** para inventario ganadero, vacunaciones y administración de catálogos.

- Base de datos: **SQL Server** (`DBRegistroGanadero`)
- Autenticación: cookies, **BCrypt**, roles (ADMINISTRADOR, OPERADOR, CONSULTA)
- Menú lateral con módulos: Inventario, Catálogos, Operaciones, Sistema

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server o LocalDB

## Configuración

- **Desarrollo:** `appsettings.Development.json` → LocalDB
- **Producción / IIS:** `appsettings.Production.json` → `.\SQLEXPRESS`
- **Despliegue IIS (pruebas + producción):** ver [DEPLOY_MULTI_AMBIENTE.md](DEPLOY_MULTI_AMBIENTE.md)

Script de columna `PasswordHash`: `Scripts/001_AddPasswordHash.sql`

## Ejecutar

```powershell
cd src/InventarioGanadero.Api
dotnet run
```

Abrir: http://127.0.0.1:5180

**Usuario inicial:** `admin` / `Admin123!` (se crea si no hay usuarios)

## Estructura

```
src/
  InventarioGanadero.Api/          → MVC, vistas, controladores
  InventarioGanadero.Infrastructure/ → Entidades, DbContext
Scripts/                           → SQL
```

## Repositorio

https://github.com/nestorm94/Finca_guadualito
