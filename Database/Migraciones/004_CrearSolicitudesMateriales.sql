
USE ClaroGestionDB;
GO

SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.HistorialMigraciones', 'U') IS NULL
        THROW 50001, 'No existe el historial de migraciones.', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.HistorialMigraciones
        WHERE nombre_archivo =
            '004_CrearSolicitudesMateriales.sql'
    )
        THROW 50002, 'La migracion 004 ya esta registrada.', 1;

    IF SCHEMA_ID('materiales') IS NULL
        EXEC('CREATE SCHEMA materiales');

    IF OBJECT_ID('materiales.SolicitudMaterial', 'U') IS NOT NULL
       OR OBJECT_ID('materiales.DetalleSolicitudMaterial', 'U') IS NOT NULL
        THROW 50003, 'Ya existen tablas de esta migracion.', 1;

    
    CREATE TABLE materiales.SolicitudMaterial
    (
        id_solicitud INT IDENTITY(1,1) NOT NULL,

        codigo AS (
            'SOL-' +
            RIGHT('000000' + CONVERT(VARCHAR(6), id_solicitud), 6)
        ) PERSISTED,

        id_colaborador INT NOT NULL,

        fecha_creacion DATETIME2 NOT NULL
            DEFAULT SYSDATETIME(),

        estado VARCHAR(15) NOT NULL
            DEFAULT 'Borrador',

        CONSTRAINT PK_SolicitudMaterial
            PRIMARY KEY (id_solicitud),

        CONSTRAINT FK_Solicitud_Colaborador
            FOREIGN KEY (id_colaborador)
            REFERENCES personal.Colaborador(id_colaborador),

        CONSTRAINT CK_Solicitud_Estado
            CHECK (estado IN ('Borrador', 'Cancelada', 'Enviada'))
    );

    CREATE TABLE materiales.DetalleSolicitudMaterial
    (
        id_detalle INT IDENTITY(1,1) NOT NULL,
        id_solicitud INT NOT NULL,
        id_material INT NOT NULL,
        cantidad DECIMAL(10,2) NOT NULL,

        CONSTRAINT PK_DetalleSolicitudMaterial
            PRIMARY KEY (id_detalle),

        CONSTRAINT FK_Detalle_Solicitud
            FOREIGN KEY (id_solicitud)
            REFERENCES materiales.SolicitudMaterial(id_solicitud),

        CONSTRAINT FK_Detalle_Material
            FOREIGN KEY (id_material)
            REFERENCES inventario.Material(id_material),

        CONSTRAINT CK_Detalle_Cantidad
            CHECK (cantidad > 0),

        CONSTRAINT UQ_Detalle_Solicitud_Material
            UNIQUE (id_solicitud, id_material)
    );

    INSERT INTO dbo.HistorialMigraciones(nombre_archivo)
    VALUES ('004_CrearSolicitudesMateriales.sql');

    COMMIT TRANSACTION;

    PRINT 'Migracion 004 ejecutada correctamente.';

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;
GO
