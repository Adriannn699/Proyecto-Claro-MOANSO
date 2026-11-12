CREATE DATABASE ClaroGestionDB;
GO

USE ClaroGestionDB;
GO

CREATE SCHEMA personal;
GO

CREATE TABLE personal.Colaborador (
    id_colaborador INT IDENTITY(1,1) PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    dni CHAR(8) NOT NULL UNIQUE,
    correo VARCHAR(150),
    telefono VARCHAR(15),
    estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE personal.PeriodoProgramacion (
    id_periodo INT IDENTITY(1,1) PRIMARY KEY,
    nombre_periodo VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Abierto',

    CONSTRAINT CK_Periodo_Fechas
        CHECK (fecha_fin >= fecha_inicio),

    CONSTRAINT CK_Periodo_Estado
        CHECK (estado IN ('Abierto', 'Cerrado'))
);
GO

CREATE TABLE personal.Disponibilidad (
    id_disponibilidad INT IDENTITY(1,1) PRIMARY KEY,
    id_colaborador INT NOT NULL,
    id_periodo INT NOT NULL,
    dia_semana TINYINT NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,
    estado BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Disponibilidad_Colaborador
        FOREIGN KEY (id_colaborador)
        REFERENCES personal.Colaborador(id_colaborador),

    CONSTRAINT FK_Disponibilidad_Periodo
        FOREIGN KEY (id_periodo)
        REFERENCES personal.PeriodoProgramacion(id_periodo),

    CONSTRAINT CK_Disponibilidad_Dia
        CHECK (dia_semana BETWEEN 1 AND 7),

    CONSTRAINT CK_Disponibilidad_Horas
        CHECK (hora_fin > hora_inicio)
);
GO