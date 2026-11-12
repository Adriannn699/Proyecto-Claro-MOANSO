
using System.Data;
using System.Data.SqlClient;
using System;

namespace Proyecto_Claro_MOANSO
{
    internal class DisponibilidadDatos
    {
        public static DataTable Listar()
        {
            string sql = @"
                SELECT
                    d.id_disponibilidad AS ID,
                    c.nombres + ' ' + c.apellidos AS Colaborador,
                    p.nombre_periodo AS Periodo,
                    CASE d.dia_semana
                        WHEN 1 THEN 'Lunes'
                        WHEN 2 THEN 'Martes'
                        WHEN 3 THEN 'Miércoles'
                        WHEN 4 THEN 'Jueves'
                        WHEN 5 THEN 'Viernes'
                        WHEN 6 THEN 'Sábado'
                        WHEN 7 THEN 'Domingo'
                    END AS Dia,
                    CONVERT(VARCHAR(5), d.hora_inicio, 108) AS Inicio,
                    CONVERT(VARCHAR(5), d.hora_fin, 108) AS Fin,
                    CASE
                        WHEN d.estado = 1 THEN 'Activo'
                        ELSE 'Anulado'
                    END AS Estado
                FROM personal.Disponibilidad d
                INNER JOIN personal.Colaborador c
                    ON d.id_colaborador = c.id_colaborador
                INNER JOIN personal.PeriodoProgramacion p
                    ON d.id_periodo = p.id_periodo
                ORDER BY d.id_disponibilidad DESC";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                SqlDataAdapter adaptador =
                    new SqlDataAdapter(sql, conexion);

                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                return tabla;
            }
        }


        public static bool Registrar(
            int idColaborador,
            int idPeriodo,
            int diaSemana,
            TimeSpan horaInicio,
            TimeSpan horaFin)
        {
            string sql = @"
            INSERT INTO personal.Disponibilidad
            (id_colaborador, id_periodo, dia_semana,
             hora_inicio, hora_fin)
            SELECT
                @Colaborador, @Periodo, @Dia,
                @Inicio, @Fin
            WHERE
            EXISTS (
                SELECT 1
                FROM personal.Colaborador
                WHERE id_colaborador = @Colaborador
                AND estado = 1
            )
            AND EXISTS (
                SELECT 1
                FROM personal.PeriodoProgramacion
                WHERE id_periodo = @Periodo
                AND estado = 'Abierto'
            )
            AND NOT EXISTS (
                SELECT 1
                FROM personal.Disponibilidad
                WHERE id_colaborador = @Colaborador
                AND id_periodo = @Periodo
                AND dia_semana = @Dia
                AND estado = 1
                AND @Inicio < hora_fin
                AND @Fin > hora_inicio
            )";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand comando =
                       new SqlCommand(sql, conexion))
                {
                    comando.Parameters.Add("@Colaborador",
                        SqlDbType.Int).Value = idColaborador;

                    comando.Parameters.Add("@Periodo",
                        SqlDbType.Int).Value = idPeriodo;

                    comando.Parameters.Add("@Dia",
                        SqlDbType.Int).Value = diaSemana;

                    comando.Parameters.Add("@Inicio",
                        SqlDbType.Time).Value = horaInicio;

                    comando.Parameters.Add("@Fin",
                        SqlDbType.Time).Value = horaFin;

                    return comando.ExecuteNonQuery() == 1;
                }
            }
        }

        public static bool Modificar(
            int idDisponibilidad,
            int idColaborador,
            int idPeriodo,
            int diaSemana,
            TimeSpan horaInicio,
            TimeSpan horaFin)
        {
            string sql = @"
            UPDATE d
            SET
            id_colaborador = @Colaborador,
            id_periodo = @Periodo,
            dia_semana = @Dia,
            hora_inicio = @Inicio,
            hora_fin = @Fin
            FROM personal.Disponibilidad d
            WHERE d.id_disponibilidad = @Id
              AND d.estado = 1

              AND EXISTS (
              SELECT 1
              FROM personal.Colaborador
              WHERE id_colaborador = @Colaborador
                AND estado = 1
              )

              AND EXISTS (
                  SELECT 1
                  FROM personal.PeriodoProgramacion
                  WHERE id_periodo = @Periodo
                    AND estado = 'Abierto'
              )

              AND NOT EXISTS (
              SELECT 1
              FROM personal.Disponibilidad
              WHERE id_disponibilidad <> @Id
                AND id_colaborador = @Colaborador
                AND id_periodo = @Periodo
                AND dia_semana = @Dia
                AND estado = 1
                AND @Inicio < hora_fin
                AND @Fin > hora_inicio
                )";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand comando =
                       new SqlCommand(sql, conexion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = idDisponibilidad;

                    comando.Parameters.Add("@Colaborador", SqlDbType.Int)
                        .Value = idColaborador;

                    comando.Parameters.Add("@Periodo", SqlDbType.Int)
                        .Value = idPeriodo;

                    comando.Parameters.Add("@Dia", SqlDbType.Int)
                        .Value = diaSemana;

                    comando.Parameters.Add("@Inicio", SqlDbType.Time)
                        .Value = horaInicio;

                    comando.Parameters.Add("@Fin", SqlDbType.Time)
                        .Value = horaFin;

                    return comando.ExecuteNonQuery() == 1;
                }
            }
        }


        public static bool Anular(int idDisponibilidad)
        {
            string sql = @"
              UPDATE personal.Disponibilidad
              SET estado = 0
              WHERE id_disponibilidad = @Id
              AND estado = 1
              AND EXISTS (
              SELECT 1
              FROM personal.PeriodoProgramacion p
              WHERE p.id_periodo =
                  personal.Disponibilidad.id_periodo
                AND p.estado = 'Abierto'
               )";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlCommand comando =
                       new SqlCommand(sql, conexion))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = idDisponibilidad;

                    return comando.ExecuteNonQuery() == 1;
                }
            }
        }


        public static DataTable ListarColaboradores()
        {
            string sql = @"
        SELECT id_colaborador,
               nombres + ' ' + apellidos AS nombre
        FROM personal.Colaborador
        WHERE estado = 1
        ORDER BY nombres, apellidos";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                SqlDataAdapter adaptador =
                    new SqlDataAdapter(sql, conexion);

                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                return tabla;
            }
        }

        public static DataTable ListarPeriodos()
        {
            string sql = @"
        SELECT id_periodo, nombre_periodo
        FROM personal.PeriodoProgramacion
        WHERE estado = 'Abierto'
        ORDER BY fecha_inicio";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            {
                SqlDataAdapter adaptador =
                    new SqlDataAdapter(sql, conexion);

                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                return tabla;
            }
        }

    }
}
