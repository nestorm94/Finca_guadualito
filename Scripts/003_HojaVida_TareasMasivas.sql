/*
  Script 003: Hoja de vida, Palpaciones y Tareas masivas
  Base de datos: DBRegistroGanadero (NO ejecutar sin aprobación)

  Instrucciones:
  1. Revisar el script completo
  2. Ejecutar manualmente en SSMS o: sqlcmd -S "(localdb)\MSSQLLocalDB" -d DBRegistroGanadero -i Scripts\003_HojaVida_TareasMasivas.sql
*/
USE DBRegistroGanadero;
GO

-- ============================================================
-- TABLA: Palpaciones
-- ============================================================
IF OBJECT_ID('dbo.Palpaciones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Palpaciones (
        IdPalpacion          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Numero               INT NOT NULL,
        Anio                 INT NOT NULL,
        FechaPalpacion       DATE NOT NULL,
        Resultado            NVARCHAR(30) NOT NULL,
        MesesGestacion       INT NULL,
        MedicamentoAplicado  NVARCHAR(150) NULL,
        Dosis                NVARCHAR(80) NULL,
        Responsable          NVARCHAR(120) NULL,
        Observacion          NVARCHAR(400) NULL,
        FechaRegistro        DATETIME2 NOT NULL CONSTRAINT DF_Palpaciones_FechaRegistro DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_Palpaciones_Animal FOREIGN KEY (Numero, Anio) REFERENCES dbo.Animales(Numero, Anio)
    );
    CREATE INDEX IX_Palpaciones_Animal ON dbo.Palpaciones(Numero, Anio);
    CREATE INDEX IX_Palpaciones_Fecha ON dbo.Palpaciones(FechaPalpacion DESC);
END
GO

-- ============================================================
-- TABLA: TareasGanaderas
-- ============================================================
IF OBJECT_ID('dbo.TareasGanaderas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TareasGanaderas (
        IdTarea          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TipoTarea        NVARCHAR(30) NOT NULL,
        FechaTarea       DATE NOT NULL,
        IdLote           INT NULL,
        IdPropietario    INT NULL,
        Descripcion      NVARCHAR(300) NULL,
        Responsable      NVARCHAR(120) NULL,
        Observacion      NVARCHAR(400) NULL,
        NombreVacuna     NVARCHAR(120) NULL,
        Medicamento      NVARCHAR(150) NULL,
        Dosis            NVARCHAR(80) NULL,
        Motivo           NVARCHAR(200) NULL,
        IdLoteDestino    INT NULL,
        FechaRegistro    DATETIME2 NOT NULL CONSTRAINT DF_TareasGanaderas_FechaRegistro DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_TareasGanaderas_Lote FOREIGN KEY (IdLote) REFERENCES dbo.Lotes(IdLote),
        CONSTRAINT FK_TareasGanaderas_Propietario FOREIGN KEY (IdPropietario) REFERENCES dbo.Propietarios(IdPropietario),
        CONSTRAINT FK_TareasGanaderas_LoteDestino FOREIGN KEY (IdLoteDestino) REFERENCES dbo.Lotes(IdLote)
    );
    CREATE INDEX IX_TareasGanaderas_Fecha ON dbo.TareasGanaderas(FechaTarea DESC);
END
GO

-- Campos opcionales si la tabla ya existía sin ellos (vacuna/medicamento en tarea masiva)
IF COL_LENGTH('dbo.TareasGanaderas', 'NombreVacuna') IS NULL
    ALTER TABLE dbo.TareasGanaderas ADD NombreVacuna NVARCHAR(120) NULL;
IF COL_LENGTH('dbo.TareasGanaderas', 'Medicamento') IS NULL
    ALTER TABLE dbo.TareasGanaderas ADD Medicamento NVARCHAR(150) NULL;
IF COL_LENGTH('dbo.TareasGanaderas', 'Dosis') IS NULL
    ALTER TABLE dbo.TareasGanaderas ADD Dosis NVARCHAR(80) NULL;
IF COL_LENGTH('dbo.TareasGanaderas', 'Motivo') IS NULL
    ALTER TABLE dbo.TareasGanaderas ADD Motivo NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.TareasGanaderas', 'IdLoteDestino') IS NULL
    ALTER TABLE dbo.TareasGanaderas ADD IdLoteDestino INT NULL;
GO

-- ============================================================
-- TABLA: DetalleTareaGanadera
-- ============================================================
IF OBJECT_ID('dbo.DetalleTareaGanadera', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetalleTareaGanadera (
        IdDetalleTarea     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdTarea            INT NOT NULL,
        Numero             INT NOT NULL,
        Anio               INT NOT NULL,
        EstadoRegistro     NVARCHAR(30) NOT NULL CONSTRAINT DF_DetalleTarea_Estado DEFAULT (N'REGISTRADO'),
        IdRegistroOrigen   INT NULL,
        TipoRegistroOrigen NVARCHAR(30) NULL,
        Observacion        NVARCHAR(400) NULL,
        CONSTRAINT FK_DetalleTarea_Tarea FOREIGN KEY (IdTarea) REFERENCES dbo.TareasGanaderas(IdTarea) ON DELETE CASCADE,
        CONSTRAINT FK_DetalleTarea_Animal FOREIGN KEY (Numero, Anio) REFERENCES dbo.Animales(Numero, Anio)
    );
    CREATE INDEX IX_DetalleTarea_Tarea ON dbo.DetalleTareaGanadera(IdTarea);
    CREATE INDEX IX_DetalleTarea_Animal ON dbo.DetalleTareaGanadera(Numero, Anio);
END
GO

-- Índice para evitar vacunaciones duplicadas (misma fecha + vacuna + animal)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Vacunaciones_Animal_Fecha_Vacuna' AND object_id = OBJECT_ID('dbo.Vacunaciones'))
BEGIN
    CREATE UNIQUE INDEX UX_Vacunaciones_Animal_Fecha_Vacuna
        ON dbo.Vacunaciones(Numero, Anio, FechaVacunacion, NombreVacuna);
END
GO

PRINT 'Script 003 listo. Revise las tablas antes de usar la aplicación.';
GO
