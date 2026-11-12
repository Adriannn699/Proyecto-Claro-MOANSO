
using System;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Claro_MOANSO
{
    internal class SolicitudMaterialDAO
    {
        

        public DataTable ObtenerMaterialesActivos()
        {
            DataTable tabla = new DataTable();

            string consulta = @"
                SELECT
                    id_material,
                    codigo,
                    nombre,
                    unidad_medida
                FROM inventario.Material
                WHERE estado = 1
                ORDER BY nombre;
            ";

            using (SqlConnection conexion =
                ConexionBD.ObtenerConexion())
            {
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(consulta, conexion))
                {
                    adaptador.Fill(tabla);
                }
            }

            return tabla;
        }
    }
}
