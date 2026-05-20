USE DBRegistroGanadero;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Usuarios') AND name = N'PasswordHash'
)
BEGIN
    ALTER TABLE dbo.Usuarios ADD PasswordHash NVARCHAR(255) NULL;
END
GO

ALTER TABLE dbo.Usuarios ALTER COLUMN Clave NVARCHAR(200) NULL;
GO

-- Opcional: después de migrar todos los hashes con la app, ejecutar:
-- ALTER TABLE dbo.Usuarios DROP COLUMN Clave;
