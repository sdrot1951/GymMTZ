using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using GymApp.Entities;
using Microsoft.Data.SqlClient;


namespace GymApp.DAO
{
    public class VentaDAO : ConexionDB // O la clase de donde heredes GetConnection()
    {


        public bool ProcesarVentaOmni(int idCliente, int idVendedor, decimal total, decimal montoPagado, int idTipoPago, string tipoVenta, List<VentaDetalle> carrito, 
                                     bool aplicaDescuento = false, string descDescripcion = null, decimal descMonto = 0, int descAutoriza = 0)

        {
            var xmlElements = from item in carrito
                              select new XElement("item",
                                  new XAttribute("fiProducto", item.fiProducto),
                                  new XAttribute("fiConceptoOmni", item.fiConceptoOmni), // ✨ Novedad
                                  new XAttribute("fiCantidad", item.fiCantidad),
                                  new XAttribute("fiPrecio", item.fiPrecio)
                              );
            string stringXml = new XElement("detalles", xmlElements).ToString();

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ProcesarVentaOmni", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros originales
                    cmd.Parameters.AddWithValue("@fiCliente", idCliente);
                    cmd.Parameters.AddWithValue("@fiVendedor", idVendedor);
                    cmd.Parameters.AddWithValue("@fmMontoTotal", total);
                    cmd.Parameters.AddWithValue("@fmMontoPagado", montoPagado);

                    cmd.Parameters.AddWithValue("@fiTipoPago", idTipoPago);
                    cmd.Parameters.AddWithValue("@fcTipoVenta", tipoVenta);
                    cmd.Parameters.AddWithValue("@fcDetalleXml", stringXml);

                    // Nuevos parámetros para el descuento
                    cmd.Parameters.AddWithValue("@AplicaDescuento", aplicaDescuento);

                    cmd.Parameters.AddWithValue("@fcDescDescripcion",
                        string.IsNullOrEmpty(descDescripcion) ? (object)DBNull.Value : descDescripcion);

                    cmd.Parameters.AddWithValue("@fiMontoDescuento",
                        descMonto > 0 ? (object)descMonto : DBNull.Value);

                    cmd.Parameters.AddWithValue("@fiAutorizaDescuento",
                        descAutoriza > 0 ? (object)descAutoriza : DBNull.Value);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }


        public DataTable BuscarPorDescripcion(string texto, bool buscarMembresia)
        {
            var dt = new DataTable();
           // string sp = buscarMembresia ? "sp_BuscarMembresiasPorDesc" : "sp_BuscarProductosPorDesc";

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarMembresiasPorDesc", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", texto);
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }

        public DataTable BuscarProducto(string filtro)
        {
            var dt = new DataTable();
            // string sp = buscarMembresia ? "sp_BuscarMembresiasPorDesc" : "sp_BuscarProductosPorDesc";

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarProducto", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", filtro);
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }

                }
            }
            return dt;
        }

        public DataTable BuscarProductoCompra(string filtro)
        {
            var dt = new DataTable();
            // string sp = buscarMembresia ? "sp_BuscarMembresiasPorDesc" : "sp_BuscarProductosPorDesc";

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarProductoCompra", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", filtro);
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }

                }
            }
            return dt;
        }


        public DataTable ObtenerTiposPago()
        {
            var dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerTiposPago", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }

        public DataTable BuscarClientes(string texto)
        {
            var dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarClientePorNombre", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", texto);
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }

        // Actualiza tu ProcesarVentaOmni para incluir fiConceptoOmni en el XML
        public bool ProcesarVentaOmni(int idCliente, int idVendedor, decimal total, int idTipoPago, string tipoVenta, List<VentaDetalle> carrito)
        {
            var xmlElements = from item in carrito
                              select new XElement("item",
                                  new XAttribute("fiProducto", item.fiProducto),
                                  new XAttribute("fiConceptoOmni", item.fiConceptoOmni), // ✨ Novedad
                                  new XAttribute("fiCantidad", item.fiCantidad),
                                  new XAttribute("fiPrecio", item.fiPrecio)
                              );
            string stringXml = new XElement("detalles", xmlElements).ToString();

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ProcesarVentaOmni", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiCliente", idCliente);
                    cmd.Parameters.AddWithValue("@fiVendedor", idVendedor);
                    cmd.Parameters.AddWithValue("@fmMontoTotal", total);
                    cmd.Parameters.AddWithValue("@fiTipoPago", idTipoPago);
                    cmd.Parameters.AddWithValue("@fcTipoVenta", tipoVenta);
                    cmd.Parameters.AddWithValue("@fcDetalleXml", stringXml);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable BuscarVisitas(string texto)
        {
            var dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarVisitasPorDesc", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", texto);
                    conexion.Open();
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }


        public DataTable ConsultarVentas(string texto, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            DataTable dt = new DataTable();
            try
            {
                // Reemplaza "GymDBConn" con el nombre de tu cadena de conexión si es distinto
                string conexionString = System.Configuration.ConfigurationManager.ConnectionStrings["GymDBConn"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(conexionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.sp_ConsultarVentas", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FiltroTexto", texto.Trim());

                        // Si la fecha tiene valor la enviamos, si no, enviamos DBNull
                        cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde.HasValue ? (object)fechaDesde.Value.Date : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta.HasValue ? (object)fechaHasta.Value.Date : DBNull.Value);

                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar las ventas: " + ex.Message);
            }
            return dt;
        }



    }
}