USE ClaroGestionDB;
GO

IF OBJECT_ID('dbo.HistorialMigraciones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.HistorialMigraciones (
        id_migracion INT IDENTITY(1,1) PRIMARY KEY,
        nombre_archivo VARCHAR(200) NOT NULL UNIQUE,
        fecha_ejecucion DATETIME2 NOT NULL
            DEFAULT SYSDATETIME()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM dbo.HistorialMigraciones
    WHERE nombre_archivo = '001_CrearHistorialMigraciones.sql'
)
BEGIN
    INSERT INTO dbo.HistorialMigraciones (nombre_archivo)
    VALUES ('001_CrearHistorialMigraciones.sql');
END;
GO