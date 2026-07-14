using System;
using System.Data;
using Microsoft.Data.SqlClient; // Asegúrate de usar tu namespace de conexión actual

namespace GymApp.DAO
{
    public class MembresiaDAO : ConexionDB // Asumo que heredas de tu clase de conexión
    {
        public DataTable ConsultarMembresiasClientes(string filtro)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ConsultarMembresiasClientes", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Si viene nulo, mandamos cadena vacía para que traiga todos los registros
                        cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Encapsulamos el error para saber exactamente en qué capa falló
                throw new Exception("Error en BD al consultar las membresías: " + ex.Message);
            }
            return dt;
        }
    }
}