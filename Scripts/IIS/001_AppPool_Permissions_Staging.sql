/*
  Permisos SQL para ambiente PRUEBAS (Staging)
  App Pool:  RegistroGanaderoTestPool  ->  login IIS APPPOOL\RegistroGanaderoTestPool
  Base:      DBRegistroGanadero_Test
  Instancia: .\SQLEXPRESS2025 (temporal; producción sigue en instancia anterior)
*/
USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'DBRegistroGanadero_Test')
BEGIN
    CREATE DATABASE [DBRegistroGanadero_Test];
    PRINT 'Base DBRegistroGanadero_Test creada.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\RegistroGanaderoTestPool')
BEGIN
    CREATE LOGIN [IIS APPPOOL\RegistroGanaderoTestPool] FROM WINDOWS;
    PRINT 'Login de servidor creado para RegistroGanaderoTestPool.';
END
GO

USE [DBRegistroGanadero_Test];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\RegistroGanaderoTestPool')
BEGIN
    CREATE USER [IIS APPPOOL\RegistroGanaderoTestPool] FOR LOGIN [IIS APPPOOL\RegistroGanaderoTestPool];
    PRINT 'Usuario de base de datos creado.';
END
GO

IF IS_ROLEMEMBER('db_datareader', 'IIS APPPOOL\RegistroGanaderoTestPool') = 0
    ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\RegistroGanaderoTestPool];
IF IS_ROLEMEMBER('db_datawriter', 'IIS APPPOOL\RegistroGanaderoTestPool') = 0
    ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\RegistroGanaderoTestPool];
GO

PRINT 'Permisos Staging aplicados en DBRegistroGanadero_Test.';
PRINT 'Ejecute Scripts 001, 002, 003 sobre esta base si el esquema aun no existe.';
