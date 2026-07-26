using GymApp.Core;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public class DevolucionesDAO : ConexionDB
    {
        public void RegistrarDevolucion(int idVenta, int idEmpleadoAutoriza, string motivo, decimal montoDevuelto)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarDevolucion", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiVenta", idVenta);
                    cmd.Parameters.AddWithValue("@fiEmpleadoAutoriza", idEmpleadoAutoriza);
                    cmd.Parameters.AddWithValue("@fcMotivo", motivo);
                    cmd.Parameters.AddWithValue("@fiMontoDevuelto", montoDevuelto);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable ConsultarDevoluciones(string filtro, DateTime desde, DateTime hasta)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ConsultarDevoluciones", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaDesde", desde.Date);
                    cmd.Parameters.AddWithValue("@FechaHasta", hasta.Date);
                    cmd.Parameters.AddWithValue("@Filtro", string.IsNullOrEmpty(filtro) ? "" : filtro);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable ConsultarTicketParaCancelacion(int folio)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ConsultarTicketParaCancelacion", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Folio", folio);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

    }
}