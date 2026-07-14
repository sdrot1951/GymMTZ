using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public class DescuentoDAO : ConexionDB
    {
        public DataTable ConsultarDescuentos(string filtro, DateTime? desde, DateTime? hasta)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ConsultarDescuentos", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");
                        cmd.Parameters.AddWithValue("@FechaDesde", desde.HasValue ? (object)desde.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaHasta", hasta.HasValue ? (object)hasta.Value : DBNull.Value);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BD al consultar los descuentos: " + ex.Message);
            }
            return dt;
        }
    }
}