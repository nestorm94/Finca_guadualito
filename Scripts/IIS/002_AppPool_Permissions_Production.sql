/*
  Permisos SQL para ambiente PRODUCCIÓN
  App Pool:  RegistroGanaderoPool  ->  login IIS APPPOOL\RegistroGanaderoPool
  Base:      DBRegistroGanadero
  Instancia: .\SQLEXPRESS
*/
USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'DBRegistroGanadero')
BEGIN
    RAISERROR('La base DBRegistroGanadero no existe. Restaure o cree la base antes de asignar permisos.', 16, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\RegistroGanaderoPool')
BEGIN
    CREATE LOGIN [IIS APPPOOL\RegistroGanaderoPool] FROM WINDOWS;
    PRINT 'Login de servidor creado para RegistroGanaderoPool.';
END
GO

USE [DBRegistroGanadero];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\RegistroGanaderoPool')
BEGIN
    CREATE USER [IIS APPPOOL\RegistroGanaderoPool] FOR LOGIN [IIS APPPOOL\RegistroGanaderoPool];
    PRINT 'Usuario de base de datos creado.';
END
GO

IF IS_ROLEMEMBER('db_datareader', 'IIS APPPOOL\RegistroGanaderoPool') = 0
    ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\RegistroGanaderoPool];
IF IS_ROLEMEMBER('db_datawriter', 'IIS APPPOOL\RegistroGanaderoPool') = 0
    ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\RegistroGanaderoPool];
GO

PRINT 'Permisos Production aplicados en DBRegistroGanadero.';
