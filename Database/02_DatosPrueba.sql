
USE ClaroGestionDB;
GO

-- =====================================
-- DATOS DE PRUEBA: COLABORADORES
-- =====================================

IF NOT EXISTS (
    SELECT 1
    FROM personal.Colaborador
    WHERE dni = '12345678'
)
BEGIN
    INSERT INTO personal.Colaborador
        (nombres, apellidos, dni, correo, telefono, estado)
    VALUES
        ('Juan', 'Perez', '12345678',
         'juan.perez@ejemplo.com', '987654321', 1);
END;
GO

-- =====================================
-- DATOS DE PRUEBA: PERIODOS
-- =====================================

IF NOT EXISTS (
    SELECT 1
    FROM personal.PeriodoProgramacion
    WHERE fecha_inicio = '2026-11-16'
      AND fecha_fin = '2026-11-22'
)
BEGIN
    INSERT INTO personal.PeriodoProgramacion
        (nombre_periodo, fecha_inicio, fecha_fin, estado)
    VALUES
        ('Semana 16 al 22 de noviembre',
         '2026-11-16',
         '2026-11-22',
         'Abierto');
END;
GO
