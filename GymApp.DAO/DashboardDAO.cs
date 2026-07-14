using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public class DashboardDAO : ConexionDB
    {
        public DataSet ObtenerDatosDashboard()
        {
            DataSet ds = new DataSet();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ObtenerDashboard", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            // Fill llenará automáticamente ds.Tables[0] con los KPIs 
                            // y ds.Tables[1] con las últimas ventas
                            da.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BD al consultar el Dashboard: " + ex.Message);
            }
            return ds;
        }
    }
}