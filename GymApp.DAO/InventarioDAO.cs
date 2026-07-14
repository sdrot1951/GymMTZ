using GymApp.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using System.Linq;

namespace GymApp.DAO
{
    public class InventarioDAO : ConexionDB
    {
        /* public DataTable ConsultarInventario(string filtro)
         {
             DataTable dt = new DataTable();
             try
             {
                 using (var conexion = GetConnection())
                 {
                     using (var cmd = new SqlCommand("sp_ConsultarInventario", conexion))
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
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
                 throw new Exception("Error en BD al consultar el inventario: " + ex.Message);
             }
             return dt;
         }*/


        public System.Data.DataTable ConsultarKardex(string busqueda, DateTime desde, DateTime hasta)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarKardexInventario", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcBusqueda", busqueda);
                cmd.Parameters.AddWithValue("@fdDesde", desde.Date);
                cmd.Parameters.AddWithValue("@fdHasta", hasta.Date);

                con.Open();
                using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public bool RegistrarCompra(string folio, decimal total, int idEmpleado, List<CompraDetalle> carrito)
        {
            // Transformamos el carrito a XML
            var xmlElements = from item in carrito
                              select new XElement("item",
                                  new XAttribute("fiProducto", item.fiProducto),
                                  new XAttribute("fmCosto", item.fmCostoUnitario),
                                  new XAttribute("fiCantidad", item.fiCantidad)
                              );
            string stringXml = new XElement("detalles", xmlElements).ToString();

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarCompra", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fcFolio", folio);
                    cmd.Parameters.AddWithValue("@fmMontoTotal", total);
                    cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);
                    cmd.Parameters.AddWithValue("@fcDetalleXml", stringXml);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public DataTable ConsultarCompras(string filtro, DateTime desde, DateTime hasta)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarCompras", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Filtro", filtro.Trim());
                    cmd.Parameters.AddWithValue("@FechaDesde", desde.Date);
                    cmd.Parameters.AddWithValue("@FechaHasta", hasta.Date);

                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }
    }
}