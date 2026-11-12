
USE ClaroGestionDB;
GO

CREATE TABLE personal.AsignacionTurno (
    id_asignacion INT IDENTITY(1,1) PRIMARY KEY,
    id_colaborador INT NOT NULL,
    id_periodo INT NOT NULL,
    fecha_turno DATE NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Asignado',
    
CONSTRAINT FK_Asignacion_Colaborador
    FOREIGN KEY (id_colaborador)
    REFERENCES personal.Colaborador(id_colaborador),

CONSTRAINT FK_Asignacion_Periodo
    FOREIGN KEY (id_periodo)
    REFERENCES personal.PeriodoProgramacion(id_periodo),

CONSTRAINT CK_Asignacion_Horas
    CHECK (hora_fin > hora_inicio),

    CONSTRAINT CK_Asignacion_Estado
    CHECK (estado IN ('Asignado', 'Cancelado'))

);
GO

CREATE TABLE personal.HistorialAsignacionTurno (
    id_historial INT IDENTITY(1,1) PRIMARY KEY,
    id_asignacion INT NOT NULL,
    accion VARCHAR(20) NOT NULL,
    fecha_cambio DATETIME NOT NULL DEFAULT GETDATE(),
    usuario_responsable VARCHAR(100) NOT NULL,
    detalle_anterior VARCHAR(300),
    detalle_nuevo VARCHAR(300),
    motivo VARCHAR(300),

    CONSTRAINT FK_Historial_Asignacion
        FOREIGN KEY (id_asignacion)
        REFERENCES personal.AsignacionTurno(id_asignacion),

    CONSTRAINT CK_Historial_Accion
        CHECK (accion IN ('Registrar', 'Modificar', 'Cancelar'))
);
GO


