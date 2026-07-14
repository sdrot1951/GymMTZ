using Microsoft.Data.SqlClient;
using System;
using GymApp.Entities;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace GymApp.DAO
{
  
public class ProductoDAO : ConexionDB
    {

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerProductos", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                fiProducto = Convert.ToInt32(dr["fiProducto"]),
                                fcDescripcion = dr["fcDescripcion"].ToString(),
                                fiPrecio = Convert.ToDecimal(dr["fiPrecio"]),
                                fiCosto = Convert.ToDecimal(dr["fiCosto"]),
                                fcNombreRubro = dr["Rubro"].ToString(),
                                fiCantidad = Convert.ToInt32(dr["Stock"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insertar(Producto prod)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_InsertarProducto", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fcDescripcion", prod.fcDescripcion);
                    cmd.Parameters.AddWithValue("@fiPrecio", prod.fiPrecio);
                    cmd.Parameters.AddWithValue("@fiCosto", prod.fiCosto);
                    cmd.Parameters.AddWithValue("@fiRubro", prod.fiRubro);
                    cmd.Parameters.AddWithValue("@fiCantidad", prod.fiCantidad);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
    
        public bool RegistrarOrdenCompra(decimal total, List<CompraDetalle> detalles)
    {
        try
        {
            // Convertimos la lista de C# a una cadena XML compatible con nuestro SP
            var xmlElements = from d in detalles
                              select new XElement("item",
                                  new XAttribute("fiProducto", d.fiProducto),
                                  new XAttribute("fiCantidad", d.fiCantidad),
                                  new XAttribute("fmCostoUnitario", d.fmCostoUnitario)
                              );
            var xmlRoot = new XElement("detalles", xmlElements);
            string stringXml = xmlRoot.ToString();

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarOrdenCompra", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fmTotal", total);
                    cmd.Parameters.AddWithValue("@fcDetalleXml", stringXml);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("La transacción de la Orden de Compra falló y fue revertida de la BD. Razón: " + ex.Message);
        }
    }

         public DataTable ObtenerRubros()
        {
            var dt = new DataTable();
            using (var conexion = GetConnection())
            {
                // ✨ Mandamos a llamar el SP en lugar del query directo
                using (var cmd = new SqlCommand("sp_ObtenerRubros", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {

                        conexion.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        // Aquí podrías loguear el error o lanzarlo a la capa BLL
                        throw new Exception("Error al cargar rubros desde la BD", ex);
                    }

                }
                
            }
            return dt;
        }
        public Producto ObtenerPorId(int id)
        {
            Producto prod = null;
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerProductoPorId", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiProducto", id);

                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            prod = new Producto
                            {
                                fiProducto = Convert.ToInt32(dr["fiProducto"]),
                                fcDescripcion = dr["fcDescripcion"].ToString(),
                                fiPrecio = Convert.ToDecimal(dr["fiPrecio"]),
                                fiCosto = Convert.ToDecimal(dr["fiCosto"]),
                                fiRubro = Convert.ToInt32(dr["fiRubro"])
                            };
                        }
                    }
                }
            }
            return prod;
        }

        public bool Editar(Producto prod)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_EditarProducto", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fiProducto", prod.fiProducto);
                        cmd.Parameters.AddWithValue("@fcDescripcion", prod.fcDescripcion);
                        cmd.Parameters.AddWithValue("@fiPrecio", prod.fiPrecio);
                        cmd.Parameters.AddWithValue("@fiCosto", prod.fiCosto);
                        cmd.Parameters.AddWithValue("@fiRubro", prod.fiRubro);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar en BD: " + ex.Message);
            }
        }

        public bool Eliminar(int id)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_EliminarProducto", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fiProducto", id);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
                catch (Exception ex)
                {
                    throw new Exception("Error al dar de baja el producto: " + ex.Message);
                }
            }

        public List<Producto> Buscar(string texto)
        {
            var lista = new List<Producto>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarProducto", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", texto);

                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto
                            {
                                fiProducto = Convert.ToInt32(dr["fiProducto"]),
                                fcDescripcion = dr["fcDescripcion"].ToString(),
                                fiPrecio = Convert.ToDecimal(dr["fiPrecio"]),
                                fiCosto = Convert.ToDecimal(dr["fiCosto"]),
                                fcNombreRubro = dr["Rubro"].ToString(),
                                fiCantidad = Convert.ToInt32(dr["Stock"])
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }


}
