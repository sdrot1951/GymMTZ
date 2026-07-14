using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using GymApp.Entities;
using Microsoft.Data.SqlClient;


namespace GymApp.DAO
{
    public class AbonosDAO : ConexionDB // O la clase de donde heredes GetConnection()
    {
        public bool RegistrarAbono(int idSaldo, decimal montoAbono, int idTipoPago, int idEmpleado)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_RegistrarAbono", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@fiSaldo", idSaldo);
                    cmd.Parameters.AddWithValue("@fmMontoAbono", montoAbono);
                    cmd.Parameters.AddWithValue("@fiTipoPago", idTipoPago);
                    cmd.Parameters.AddWithValue("@fiVendedor", idEmpleado);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable ConsultarDeudas( string texto, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            DataTable dt = new DataTable();
            // Usa el nombre correcto de tu cadena de conexión
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarDeudas", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FiltroTexto", texto ?? "");
                    cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.HasValue ? (object)fechaDesde.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.HasValue ? (object)fechaHasta.Value.Date : DBNull.Value);

                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }
    }
}