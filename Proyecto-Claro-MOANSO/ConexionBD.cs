using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Proyecto_Claro_MOANSO
{
    internal class ConexionBD
    {
        public static SqlConnection ObtenerConexion()
        {
            string cadena = ConfigurationManager
                .ConnectionStrings["ConexionClaro"]
                .ConnectionString;

            return new SqlConnection(cadena);
        }
    }
}

