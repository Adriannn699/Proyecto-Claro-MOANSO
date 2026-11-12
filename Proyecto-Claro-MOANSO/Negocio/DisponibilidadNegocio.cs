using System.Data;
using System;

namespace Proyecto_Claro_MOANSO
{
    internal class DisponibilidadNegocio
    {
        public static bool Registrar(
            int idColaborador,
            int idPeriodo,
            int diaSemana,
            TimeSpan horaInicio,
            TimeSpan horaFin)
        {
            // Validar colaborador
            if (idColaborador <= 0)
            {
                throw new Exception(
                    "Debe seleccionar un colaborador.");
            }

            // Validar periodo
            if (idPeriodo <= 0)
            {
                throw new Exception(
                    "Debe seleccionar un periodo.");
            }

            // Validar día
            if (diaSemana < 1 || diaSemana > 7)
            {
                throw new Exception(
                    "Debe seleccionar un día válido.");
            }

            // Validar horas
            if (horaInicio >= horaFin)
            {
                throw new Exception(
                    "La hora de inicio debe ser menor que la hora de fin.");
            }

            // Enviar los datos a la capa de datos
            return DisponibilidadDatos.Registrar(
                idColaborador,
                idPeriodo,
                diaSemana,
                horaInicio,
                horaFin
            );
        }


        public static bool Modificar(
            int idDisponibilidad,
            int idColaborador,
            int idPeriodo,
            int diaSemana,
            TimeSpan horaInicio,
            TimeSpan horaFin)
        {
            if (idDisponibilidad <= 0)
            {
                throw new Exception(
                    "Debe seleccionar una disponibilidad.");
            }

            if (idColaborador <= 0)
            {
                throw new Exception(
                    "Debe seleccionar un colaborador.");
            }

            if (idPeriodo <= 0)
            {
                throw new Exception(
                    "Debe seleccionar un periodo.");
            }

            if (diaSemana < 1 || diaSemana > 7)
            {
                throw new Exception(
                    "Debe seleccionar un día válido.");
            }

            if (horaInicio >= horaFin)
            {
                throw new Exception(
                    "La hora de inicio debe ser menor que la hora de fin.");
            }

            return DisponibilidadDatos.Modificar(
                idDisponibilidad,
                idColaborador,
                idPeriodo,
                diaSemana,
                horaInicio,
                horaFin
            );
        }


        public static bool Anular(int idDisponibilidad)
        {
            if (idDisponibilidad <= 0)
            {
                throw new Exception(
                    "Debe seleccionar una disponibilidad para anular.");
            }

            return DisponibilidadDatos.Anular(idDisponibilidad);
        }


        public static DataTable ListarColaboradores()
        {
            return DisponibilidadDatos.ListarColaboradores();
        }

        public static DataTable ListarPeriodos()
        {
            return DisponibilidadDatos.ListarPeriodos();
        }


        public static DataTable Listar()
        {
            return DisponibilidadDatos.Listar();
        }

    }
}
