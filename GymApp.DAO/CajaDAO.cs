using GymApp.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace GymApp.DAO
{
    public class CajaDAO : ConexionDB
    {
        public DataTable ObtenerMovimientos( DateTime desde, DateTime hasta)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_ConsultarMovimientosCaja", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@FiltroTexto"); // Nuevo parámetro
                    cmd.Parameters.AddWithValue("@FechaDesde", desde);
                    cmd.Parameters.AddWithValue("@FechaHasta", hasta);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable BuscarMovimientosTexto(string filtro)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_BuscarMovimientosCajaTexto", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FiltroTexto", filtro);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        
        //Corte Diario
        public int RegistrarCorte(int idEmpleado, decimal fondo, decimal declarado, string obs)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarCorteTurno", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);
                    cmd.Parameters.AddWithValue("@fiFondoInicial", fondo);
                    cmd.Parameters.AddWithValue("@fiMontoDeclarado", declarado);
                    cmd.Parameters.AddWithValue("@fcObservaciones", obs);

                    conexion.Open();
                    // ExecuteScalar atrapa el SCOPE_IDENTITY() del SQL
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public decimal ObtenerTotalEsperadoDia(int empleadoTurno)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ObtenerTotalTurno", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiEmpleado", empleadoTurno);
                    conexion.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToDecimal(resultado) : 0m;
                }
            }
        }

        public DataTable ObtenerDetalleMovimientos(DateTime inicio, DateTime fin, int empleadoturno)
        {
            DataTable dt = new DataTable();

            // GetConnection() proviene de tu clase base ConexionDB
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ConsultarDetalleMovimientosCorte", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pasamos los parámetros tal como los definimos en el SP
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = inicio;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fin;
                    cmd.Parameters.Add("@idEmpleado", SqlDbType.Int).Value = empleadoturno;

                    try
                    {
                        conexion.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // Rellenamos el DataTable con las filas devueltas por el SP
                            da.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Lanza la excepción para que sea capturada y mostrada en la UI
                        throw new Exception("Error en CajaDAO.ObtenerDetalleMovimientos: " + ex.Message, ex);
                    }
                }
            }

            return dt;
        }

        public DataTable ConsultarCortesTurno(DateTime fechaDesde, DateTime fechaHasta)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarCortes", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.Date);
                    cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.Date);

                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable ConsultarCortesDiarios(DateTime fechaDesde, DateTime fechaHasta)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarCortesDiarios", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.Date);
                    cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.Date);

                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public (decimal Entradas, decimal Salidas) PrevisualizarCorte()
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_PrevisualizarCorte", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (Convert.ToDecimal(reader["TotalEntradas"]), Convert.ToDecimal(reader["TotalSalidas"]));
                        }
                    }
                }
            }
            return (0, 0);
        }


        //Corte Diario
        public int GenerarCorte(int idEmpleado, decimal fondoInicial, decimal montoDeclarado, string observaciones)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_GenerarCorteCaja", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);
                    cmd.Parameters.AddWithValue("@fiFondoInicial", fondoInicial);
                    cmd.Parameters.AddWithValue("@fiMontoDeclarado", montoDeclarado);
                    cmd.Parameters.AddWithValue("@fcObservaciones", string.IsNullOrEmpty(observaciones) ? (object)DBNull.Value : observaciones);


                    conexion.Open();
                    // ExecuteScalar atrapa el SCOPE_IDENTITY() del SQL
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }


    }
}