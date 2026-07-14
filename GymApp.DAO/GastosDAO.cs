using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public class GastosDAO : ConexionDB
    {
        public DataTable ConsultarGastos(string filtro, DateTime? desde, DateTime? hasta)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ConsultarGastos", conexion))
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
                throw new Exception("Error en BD al consultar el histórico de gastos: " + ex.Message);
            }
            return dt;
        }

        public DataTable ObtenerCategorias()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ObtenerCatGastos", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al cargar categorías: " + ex.Message); }
            return dt;
        }

        public bool InsertarGasto(string descripcion, decimal monto, int tipoGasto, int idEmpleado)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertarGasto", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                        cmd.Parameters.AddWithValue("@fiMonto", monto);
                        cmd.Parameters.AddWithValue("@fiTipoGasto", tipoGasto);
                        cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar en BD: " + ex.Message); }
        }


        public DataTable ConsultarGastosMensual(string filtro, DateTime? desde, DateTime? hasta)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ConsultarGastosMensual", conexion))
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
                throw new Exception("Error en BD al consultar el histórico de gastos: " + ex.Message);
            }
            return dt;
        }


        public bool InsertarGastoMensual(string descripcion, decimal monto, int tipoGasto, int idEmpleado)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertarGastoMensual", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                        cmd.Parameters.AddWithValue("@fiMonto", monto);
                        cmd.Parameters.AddWithValue("@fiTipoGasto", tipoGasto);
                        cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar en BD: " + ex.Message); }
        }



    }
}