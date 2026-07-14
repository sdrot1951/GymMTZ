using GymApp.DAO;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

public class PuestoDAO : ConexionDB // Asegúrate de heredar de tu clase de conexión
{
    public DataTable ObtenerPuestos()
    {
        DataTable dt = new DataTable();

        using (var conexion = GetConnection())
        {
            using (var comando = new SqlCommand("sp_ObtenerPuestos", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                try
                {
                    conexion.Open();
                    SqlDataAdapter da = new SqlDataAdapter(comando);
                    da.Fill(dt);
                }
                catch (SqlException ex)
                {
                    // Aquí podrías loguear el error o lanzarlo a la capa BLL
                    throw new Exception("Error al cargar puestos desde la BD", ex);
                }
            }
        }
        return dt;
    }
}