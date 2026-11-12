
using System;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Claro_MOANSO
{
    internal class AsignacionTurnoDatos
    {
        public static DataTable Listar()
        {
            string sql = @"
                SELECT
                    a.id_asignacion AS ID,
                    c.nombres + ' ' + c.apellidos AS Colaborador,
                    p.nombre_periodo AS Periodo,
                    a.fecha_turno AS Fecha,
                    CONVERT(VARCHAR(5), a.hora_inicio, 108) AS Inicio,
                    CONVERT(VARCHAR(5), a.hora_fin, 108) AS Fin,
                    a.estado AS Estado
                FROM personal.AsignacionTurno a
                INNER JOIN personal.Colaborador c
                    ON a.id_colaborador = c.id_colaborador
                INNER JOIN personal.PeriodoProgramacion p
                    ON a.id_periodo = p.id_periodo
                ORDER BY a.id_asignacion DESC";

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
            DateTime fechaTurno,
            TimeSpan horaInicio,
            TimeSpan horaFin)
        {

            if (horaInicio >= horaFin)
            {
                return false;
            }

            int diaSemana = ((int)fechaTurno.DayOfWeek + 6) % 7 + 1;

            string sqlInsertar = @"
    INSERT INTO personal.AsignacionTurno
        (id_colaborador, id_periodo, fecha_turno,
         hora_inicio, hora_fin, estado)
    VALUES
        (@Colaborador, @Periodo, @Fecha,
         @Inicio, @Fin, 'Asignado');

    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                       conexion.BeginTransaction(IsolationLevel.Serializable))
                {

                    try
                    {

                        string verificarTurno = @"
    SELECT COUNT(*)
    FROM personal.AsignacionTurno WITH (UPDLOCK, HOLDLOCK)
    WHERE id_colaborador = @Colaborador
      AND fecha_turno = @Fecha
      AND estado = 'Asignado'
      AND @Inicio < hora_fin
      AND @Fin > hora_inicio";

                        using (SqlCommand verificar =
                               new SqlCommand(verificarTurno, conexion, transaccion))
                        {
                            verificar.Parameters.Add("@Colaborador", SqlDbType.Int)
                                .Value = idColaborador;

                            verificar.Parameters.Add("@Fecha", SqlDbType.Date)
                                .Value = fechaTurno.Date;

                            verificar.Parameters.Add("@Inicio", SqlDbType.Time)
                                .Value = horaInicio;

                            verificar.Parameters.Add("@Fin", SqlDbType.Time)
                                .Value = horaFin;

                            int repetidos = (int)verificar.ExecuteScalar();

                            if (repetidos > 0)
                            {
                                transaccion.Rollback();
                                return false;
                            }

                            string verificarDisponibilidad = @"
    SELECT COUNT(*)
    FROM personal.Disponibilidad
    WHERE id_colaborador = @Colaborador
      AND id_periodo = @Periodo
      AND dia_semana = @Dia
      AND hora_inicio <= @Inicio
      AND hora_fin >= @Fin
      AND estado = 1";

                            using (SqlCommand validar =
                                   new SqlCommand(verificarDisponibilidad, conexion, transaccion))
                            {
                                validar.Parameters.Add("@Colaborador", SqlDbType.Int).Value = idColaborador;
                                validar.Parameters.Add("@Periodo", SqlDbType.Int).Value = idPeriodo;
                                validar.Parameters.Add("@Dia", SqlDbType.TinyInt).Value = diaSemana;
                                validar.Parameters.Add("@Inicio", SqlDbType.Time).Value = horaInicio;
                                validar.Parameters.Add("@Fin", SqlDbType.Time).Value = horaFin;

                                if ((int)validar.ExecuteScalar() == 0)
                                {
                                    transaccion.Rollback();
                                    return false;
                                }
                            }

                        }


                        string verificarPeriodo = @"
    SELECT COUNT(*)
    FROM personal.PeriodoProgramacion
    WHERE id_periodo = @Periodo
      AND @Fecha BETWEEN fecha_inicio AND fecha_fin
      AND estado = 'Abierto'";

                        using (SqlCommand validarPeriodo =
                               new SqlCommand(verificarPeriodo, conexion, transaccion))
                        {
                            validarPeriodo.Parameters.Add("@Periodo", SqlDbType.Int).Value = idPeriodo;
                            validarPeriodo.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaTurno.Date;

                            if ((int)validarPeriodo.ExecuteScalar() == 0)
                            {
                                transaccion.Rollback();
                                return false;
                            }
                        }

                        using (SqlCommand comando =
                               new SqlCommand(sqlInsertar, conexion, transaccion))

                        {
                            comando.Parameters.Add("@Colaborador", SqlDbType.Int)
                                .Value = idColaborador;
                            comando.Parameters.Add("@Periodo", SqlDbType.Int)
                                .Value = idPeriodo;
                            comando.Parameters.Add("@Fecha", SqlDbType.Date)
                                .Value = fechaTurno.Date;
                            comando.Parameters.Add("@Inicio", SqlDbType.Time)
                                .Value = horaInicio;
                            comando.Parameters.Add("@Fin", SqlDbType.Time)
                                .Value = horaFin;

                            int idAsignacion = (int)comando.ExecuteScalar();

                            string sqlHistorial = @"
                    INSERT INTO personal.HistorialAsignacionTurno
                        (id_asignacion, accion, usuario_responsable,
                         detalle_nuevo)
                    VALUES
                        (@Id, 'Registrar', @Usuario, @Detalle)";

                            using (SqlCommand historial =
                                   new SqlCommand(sqlHistorial, conexion, transaccion))
                            {
                                historial.Parameters.Add("@Id", SqlDbType.Int)
                                    .Value = idAsignacion;
                                historial.Parameters.Add("@Usuario", SqlDbType.VarChar, 100)
                                    .Value = Environment.UserName;
                                historial.Parameters.Add("@Detalle", SqlDbType.VarChar, 300)
                                    .Value = fechaTurno.ToString("yyyy-MM-dd") + " " +
                                             horaInicio + " - " + horaFin;

                                historial.ExecuteNonQuery();
                            }
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }

        }

        public static bool Modificar(
            int idAsignacion,
            DateTime nuevaFecha,
            TimeSpan nuevaHoraInicio,
            TimeSpan nuevaHoraFin)
        {
            if (nuevaHoraInicio >= nuevaHoraFin)
                return false;

            int diaSemana = ((int)nuevaFecha.DayOfWeek + 6) % 7 + 1;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                       conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string sqlBuscar = @"
                    SELECT id_colaborador, id_periodo,
                           fecha_turno, hora_inicio, hora_fin
                    FROM personal.AsignacionTurno WITH (UPDLOCK, HOLDLOCK)
                    WHERE id_asignacion = @Id
                      AND estado = 'Asignado'";

                        int idColaborador;
                        int idPeriodo;
                        string detalleAnterior;

                        using (SqlCommand buscar =
                               new SqlCommand(sqlBuscar, conexion, transaccion))
                        {
                            buscar.Parameters.AddWithValue("@Id", idAsignacion);

                            using (SqlDataReader lector = buscar.ExecuteReader())
                            {
                                if (!lector.Read())
                                    return false;

                                idColaborador = lector.GetInt32(0);
                                idPeriodo = lector.GetInt32(1);

                                detalleAnterior =
                                    lector.GetDateTime(2).ToString("yyyy-MM-dd") + " " +
                                    lector.GetTimeSpan(3) + " - " +
                                    lector.GetTimeSpan(4);
                            }
                        }

                        string sqlValidar = @"
                    SELECT CASE WHEN
                        EXISTS (
                            SELECT 1
                            FROM personal.PeriodoProgramacion
                            WHERE id_periodo = @Periodo
                              AND estado = 'Abierto'
                              AND @Fecha BETWEEN fecha_inicio AND fecha_fin
                        )
                        AND EXISTS (
                            SELECT 1
                            FROM personal.Disponibilidad
                            WHERE id_colaborador = @Colaborador
                              AND id_periodo = @Periodo
                              AND dia_semana = @Dia
                              AND estado = 1
                              AND hora_inicio <= @Inicio
                              AND hora_fin >= @Fin
                        )
                        AND NOT EXISTS (
                            SELECT 1
                            FROM personal.AsignacionTurno WITH (UPDLOCK, HOLDLOCK)
                            WHERE id_colaborador = @Colaborador
                              AND fecha_turno = @Fecha
                              AND estado = 'Asignado'
                              AND id_asignacion <> @Id
                              AND @Inicio < hora_fin
                              AND @Fin > hora_inicio
                        )
                    THEN 1 ELSE 0 END";

                        using (SqlCommand validar =
                               new SqlCommand(sqlValidar, conexion, transaccion))
                        {
                            validar.Parameters.AddWithValue("@Id", idAsignacion);
                            validar.Parameters.AddWithValue("@Colaborador", idColaborador);
                            validar.Parameters.AddWithValue("@Periodo", idPeriodo);
                            validar.Parameters.AddWithValue("@Dia", diaSemana);
                            validar.Parameters.Add("@Fecha", SqlDbType.Date).Value = nuevaFecha.Date;
                            validar.Parameters.Add("@Inicio", SqlDbType.Time).Value = nuevaHoraInicio;
                            validar.Parameters.Add("@Fin", SqlDbType.Time).Value = nuevaHoraFin;

                            if ((int)validar.ExecuteScalar() == 0)
                                return false;
                        }

                        string sqlModificar = @"
                    UPDATE personal.AsignacionTurno
                    SET fecha_turno = @Fecha,
                        hora_inicio = @Inicio,
                        hora_fin = @Fin
                    WHERE id_asignacion = @Id
                      AND estado = 'Asignado'";

                        using (SqlCommand modificar =
                               new SqlCommand(sqlModificar, conexion, transaccion))
                        {
                            modificar.Parameters.AddWithValue("@Id", idAsignacion);
                            modificar.Parameters.Add("@Fecha", SqlDbType.Date).Value = nuevaFecha.Date;
                            modificar.Parameters.Add("@Inicio", SqlDbType.Time).Value = nuevaHoraInicio;
                            modificar.Parameters.Add("@Fin", SqlDbType.Time).Value = nuevaHoraFin;

                            if (modificar.ExecuteNonQuery() != 1)
                                return false;
                        }

                        string sqlHistorial = @"
                    INSERT INTO personal.HistorialAsignacionTurno
                        (id_asignacion, accion, usuario_responsable,
                         detalle_anterior, detalle_nuevo)
                    VALUES
                        (@Id, 'Modificar', @Usuario,
                         @Anterior, @Nuevo)";

                        using (SqlCommand historial =
                               new SqlCommand(sqlHistorial, conexion, transaccion))
                        {
                            historial.Parameters.AddWithValue("@Id", idAsignacion);
                            historial.Parameters.AddWithValue("@Usuario", Environment.UserName);
                            historial.Parameters.AddWithValue("@Anterior", detalleAnterior);
                            historial.Parameters.AddWithValue("@Nuevo",
                                nuevaFecha.ToString("yyyy-MM-dd") + " " +
                                nuevaHoraInicio + " - " + nuevaHoraFin);

                            historial.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool Cancelar(int idAsignacion, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                return false;

            if (motivo.Length > 300)
                return false;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                       conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string sqlBuscar = @"
                    SELECT fecha_turno, hora_inicio, hora_fin
                    FROM personal.AsignacionTurno WITH (UPDLOCK, HOLDLOCK)
                    WHERE id_asignacion = @Id
                      AND estado = 'Asignado'";

                        string detalleAnterior;

                        using (SqlCommand buscar =
                               new SqlCommand(sqlBuscar, conexion, transaccion))
                        {
                            buscar.Parameters.Add("@Id", SqlDbType.Int)
                                .Value = idAsignacion;

                            using (SqlDataReader lector = buscar.ExecuteReader())
                            {
                                if (!lector.Read())
                                    return false;

                                detalleAnterior =
                                    lector.GetDateTime(0).ToString("yyyy-MM-dd") + " " +
                                    lector.GetTimeSpan(1) + " - " +
                                    lector.GetTimeSpan(2);
                            }
                        }

                        string sqlCancelar = @"
                    UPDATE personal.AsignacionTurno
                    SET estado = 'Cancelado'
                    WHERE id_asignacion = @Id
                      AND estado = 'Asignado'";

                        using (SqlCommand cancelar =
                               new SqlCommand(sqlCancelar, conexion, transaccion))
                        {
                            cancelar.Parameters.Add("@Id", SqlDbType.Int)
                                .Value = idAsignacion;

                            if (cancelar.ExecuteNonQuery() != 1)
                                return false;
                        }

                        string sqlHistorial = @"
                    INSERT INTO personal.HistorialAsignacionTurno
                        (id_asignacion, accion, usuario_responsable,
                         detalle_anterior, detalle_nuevo, motivo)
                    VALUES
                        (@Id, 'Cancelar', @Usuario,
                         @Anterior, 'Cancelado', @Motivo)";

                        using (SqlCommand historial =
                               new SqlCommand(sqlHistorial, conexion, transaccion))
                        {
                            historial.Parameters.Add("@Id", SqlDbType.Int)
                                .Value = idAsignacion;

                            historial.Parameters.Add("@Usuario", SqlDbType.VarChar, 100)
                                .Value = Environment.UserName;

                            historial.Parameters.Add("@Anterior", SqlDbType.VarChar, 300)
                                .Value = detalleAnterior;

                            historial.Parameters.Add("@Motivo", SqlDbType.VarChar, 300)
                                .Value = motivo;

                            historial.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }
        
public static DataTable ListarHistorial()
{
    string sql = @"
        SELECT
            h.id_historial AS ID,
            h.id_asignacion AS Asignacion,
            c.nombres + ' ' + c.apellidos AS Colaborador,
            h.accion AS Accion,
            h.fecha_cambio AS FechaCambio,
            h.usuario_responsable AS Responsable,
            h.detalle_anterior AS DetalleAnterior,
            h.detalle_nuevo AS DetalleNuevo,
            h.motivo AS Motivo
        FROM personal.HistorialAsignacionTurno h
        INNER JOIN personal.AsignacionTurno a
            ON h.id_asignacion = a.id_asignacion
        INNER JOIN personal.Colaborador c
            ON a.id_colaborador = c.id_colaborador
        ORDER BY h.fecha_cambio DESC, h.id_historial DESC";

    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
    {
        using (SqlDataAdapter adaptador =
               new SqlDataAdapter(sql, conexion))
        {
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);

            return tabla;
        }
    }
}

    }
}
