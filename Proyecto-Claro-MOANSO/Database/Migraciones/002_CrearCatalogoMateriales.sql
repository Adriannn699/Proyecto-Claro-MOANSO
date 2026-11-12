USE ClaroGestionDB;
GO

-- Esquema (se crea solo si no existe)
IF SCHEMA_ID('inventario') IS NULL
    EXEC('CREATE SCHEMA inventario');
GO

-- Tabla de materiales (se crea solo si no existe)
IF OBJECT_ID('inventario.Material', 'U') IS NULL
BEGIN
    CREATE TABLE inventario.Material (
        id_material        INT IDENTITY(1,1) PRIMARY KEY,
        codigo             VARCHAR(30)  NOT NULL UNIQUE,
        nombre             VARCHAR(120) NOT NULL,
        descripcion        VARCHAR(255) NULL,
        categoria          VARCHAR(60)  NULL,
        unidad_medida      VARCHAR(20)  NOT NULL,
        estado             BIT          NOT NULL DEFAULT 1,  -- 1 = Activo, 0 = Inhabilitado
        fecha_registro     DATETIME2    NOT NULL DEFAULT SYSDATETIME(),
        fecha_modificacion DATETIME2    NULL
    );
END;
GO

-- Datos de prueba (solo si el catálogo está vacío)
IF NOT EXISTS (SELECT 1 FROM inventario.Material)
BEGIN
    INSERT INTO inventario.Material (codigo, nombre, descripcion, categoria, unidad_medida) VALUES
    ('MAT-001', 'Papel bond A4',     'Resma de 500 hojas',  'Papelería',  'Paquete'),
    ('MAT-002', 'Lapicero azul',     'Punta fina',          'Papelería',  'Unidad'),
    ('MAT-003', 'Cinta de embalaje', 'Transparente 48 mm',  'Otros',      'Unidad'),
    ('MAT-004', 'Lejía',             'Galón de 4 litros',   'Limpieza',   'Litro'),
    ('MAT-005', 'Polo corporativo',  'Talla M',             'Uniformes',  'Unidad'),
    ('MAT-006', 'Cable HDMI',        '1.5 metros',          'Tecnología', 'Unidad'),
    ('MAT-007', 'Folder manila',     'Tamaño oficio',       'Papelería',  'Caja'),
    ('MAT-008', 'Mouse USB',         'Óptico',              'Tecnología', 'Unidad');
END;
GO

-- Registro en el historial de migraciones
IF NOT EXISTS (
    SELECT 1
    FROM dbo.HistorialMigraciones
    WHERE nombre_archivo = '002_CrearCatalogoMateriales.sql'
)
BEGIN
    INSERT INTO dbo.HistorialMigraciones (nombre_archivo)
    VALUES ('002_CrearCatalogoMateriales.sql');
END;
GO
