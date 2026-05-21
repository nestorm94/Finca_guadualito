-- Módulo reproductivo: Partos, RelacionMadreCria, BimestreNacimiento en Animales
USE DBRegistroGanadero;
GO

-- Bimestre de nacimiento en Animales
IF COL_LENGTH('dbo.Animales', 'BimestreNacimiento') IS NULL
BEGIN
    ALTER TABLE dbo.Animales ADD BimestreNacimiento INT NULL;
END
GO

-- Ampliar tabla Partos
IF COL_LENGTH('dbo.Partos', 'TipoParto') IS NULL
    ALTER TABLE dbo.Partos ADD TipoParto NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.Partos', 'EstadoParto') IS NULL
    ALTER TABLE dbo.Partos ADD EstadoParto NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.Partos', 'FechaRegistro') IS NULL
    ALTER TABLE dbo.Partos ADD FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_Partos_FechaRegistro DEFAULT (SYSUTCDATETIME());
GO

UPDATE dbo.Partos SET TipoParto = N'NORMAL' WHERE TipoParto IS NULL;
UPDATE dbo.Partos SET EstadoParto = N'REGISTRADO' WHERE EstadoParto IS NULL;
GO

-- Tabla RelacionMadreCria
IF OBJECT_ID('dbo.RelacionMadreCria', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RelacionMadreCria (
        IdRelacion      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdParto         INT NOT NULL,
        NumeroMadre     INT NOT NULL,
        AnioMadre       INT NOT NULL,
        NumeroCria      INT NOT NULL,
        AnioCria        INT NOT NULL,
        CONSTRAINT FK_RelacionMadreCria_Parto
            FOREIGN KEY (IdParto) REFERENCES dbo.Partos(IdParto) ON DELETE CASCADE,
        CONSTRAINT FK_RelacionMadreCria_Madre
            FOREIGN KEY (NumeroMadre, AnioMadre) REFERENCES dbo.Animales(Numero, Anio),
        CONSTRAINT FK_RelacionMadreCria_Cria
            FOREIGN KEY (NumeroCria, AnioCria) REFERENCES dbo.Animales(Numero, Anio)
    );

    CREATE INDEX IX_RelacionMadreCria_Parto ON dbo.RelacionMadreCria(IdParto);
    CREATE INDEX IX_RelacionMadreCria_Madre ON dbo.RelacionMadreCria(NumeroMadre, AnioMadre);
END
GO
