using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using GymApp.Entities;

namespace GymApp.DAO
{
    public class ClienteDAO : ConexionDB
    {

        public PerfilClienteDTO ObtenerPerfilCliente(int idCliente)
        {
            PerfilClienteDTO perfil = null;
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ObtenerPerfilCliente", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiCliente", idCliente);
                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            perfil = new PerfilClienteDTO
                            {
                                IdCliente = Convert.ToInt32(dr["fiCliente"]),
                                NombreCompleto = dr["NombreCompleto"].ToString(),
                                Telefono = Convert.ToInt64(dr["Telefono"]),
                                Email = dr["Email"].ToString(),
                                Foto = dr["fbFoto"] != DBNull.Value ? (byte[])dr["fbFoto"] : null,
                                Membresia = dr["MembresiaDesc"].ToString(),
                                FechaInicio = dr["fdFechaInicio"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaInicio"]) : (DateTime?)null,
                                fcObservaciones = dr["fcObservaciones"] != DBNull.Value ? dr["fcObservaciones"].ToString(): string.Empty,
                                FechaVencimiento = dr["fdFechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaVencimiento"]) : (DateTime?)null,
                                DiasRestantes = Convert.ToInt32(dr["DiasRestantes"]),
                                EstadoMembresia = dr["EstadoMembresia"].ToString(),
                                DeudaTotal = Convert.ToDecimal(dr["DeudaTotal"]),
                                IdDeudaPendiente = dr["IdDeudaPendiente"] != DBNull.Value ? Convert.ToInt32(dr["IdDeudaPendiente"]) : (int?)null
                            };
                        }
                    }
                }
            }
            return perfil;
        }
        public int Insertar(Cliente cli)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fcNombre", cli.fcNombre);
                        cmd.Parameters.AddWithValue("@fcApePat", cli.fcApePat);
                        cmd.Parameters.AddWithValue("@fcApeMat", string.IsNullOrWhiteSpace(cli.fcApeMat) ? (object)DBNull.Value : cli.fcApeMat);
                        cmd.Parameters.Add("@fdFechaNac", SqlDbType.Date).Value = cli.fdFechaNac.Date;
                        cmd.Parameters.AddWithValue("@fcDireccion", string.IsNullOrWhiteSpace(cli.fcDireccion) ? (object)DBNull.Value : cli.fcDireccion);
                        cmd.Parameters.AddWithValue("@fiTelefono", cli.fiTelefono == 0 ? (object)DBNull.Value : (long)cli.fiTelefono);
                        cmd.Parameters.AddWithValue("@fcEmergencias", string.IsNullOrWhiteSpace(cli.fcEmergencia) ? (object)DBNull.Value : cli.fcEmergencia);
                        cmd.Parameters.AddWithValue("@fcEmail", string.IsNullOrWhiteSpace(cli.fcEmail) ? (object)DBNull.Value : cli.fcEmail);
                        cmd.Parameters.AddWithValue("@fcObservaciones", string.IsNullOrWhiteSpace(cli.fcObservaciones) ? (object)DBNull.Value : cli.fcObservaciones);

                        conexion.Open();
                        //int filasAfectadas = cmd.ExecuteNonQuery();

                        int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                        return nuevoId;
                    }
                }
            }
            catch (SqlException ex)
            {
                // 🔴 ESTO ES CLAVE: Si explota, nos dirá exactamente por qué
                throw new Exception("Error de SQL: " + ex.Message);
            }
        }

        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();

            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_ObtenerClientes", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conexion.Open();

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new Cliente
                                {
                                    fiCliente = Convert.ToInt32(dr["fiCliente"]),
                                    fcNombre = dr["fcNombre"].ToString(),
                                    fcApePat = dr["fcApePat"].ToString(),
                                    fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : string.Empty,
                                    fiTelefono = dr["fiTelefono"] != DBNull.Value ? Convert.ToInt64(dr["fiTelefono"]) : 0,
                                    fcEmail = dr["fcEmail"] != DBNull.Value ? dr["fcEmail"].ToString() : string.Empty,
                                    fcEmergencia = dr["fiEmergencias"] != DBNull.Value ? dr["fiEmergencias"].ToString() : string.Empty,
                                    fdFechaNac = dr["fdFechaNac"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaNac"]) : DateTime.MinValue,
                                    fcObservaciones = dr["fcObservaciones"] != DBNull.Value ? dr["fcObservaciones"].ToString() : string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los clientes desde la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener la lista de clientes.", ex);
            }

            return lista;
        }

        public bool GuardarFoto(int idCliente, byte[] fotoBytes)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_GuardarFotoCliente", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiCliente", idCliente);

                    // Tratamiento especial para datos binarios (Imágenes)
                    if (fotoBytes == null || fotoBytes.Length == 0)
                    {
                        cmd.Parameters.Add("@fbFoto", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@fbFoto", System.Data.SqlDbType.VarBinary).Value = fotoBytes;
                    }

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Retorna true si se actualizó al menos 1 cliente
                    return filasAfectadas > 0;
                }
            }
        }
        public Cliente ObtenerPorId(int id)
        {
            Cliente cli = null;
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerClientePorId", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiCliente", id);
                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cli = new Cliente
                            {
                                fiCliente = Convert.ToInt32(dr["fiCliente"]),
                                fcNombre = dr["fcNombre"].ToString(),
                                fcApePat = dr["fcApePat"].ToString(),
                                fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : "",
                                fiTelefono = dr["fiTelefono"] != DBNull.Value ? Convert.ToInt64(dr["fiTelefono"]) : 0,
                                fcEmail = dr["fcEmail"] != DBNull.Value ? dr["fcEmail"].ToString() : "",
                                fcEmergencia = dr["fiEmergencias"] != DBNull.Value ? dr["fiEmergencias"].ToString() : "",
                                fcDireccion = dr["fcDireccion"] != DBNull.Value ? dr["fcDireccion"].ToString() : "",
                                
                                fdFechaNac = dr["fdFechaNac"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaNac"]) : new DateTime(1990, 1, 1),
                                fcObservaciones = dr["fcObservaciones"] != DBNull.Value ? dr["fcObservaciones"].ToString() : "",
                            };
                        }
                    }
                }
            }
            return cli;
        }

        public bool Editar(Cliente cli)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_EditarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fiCliente", cli.fiCliente);
                        cmd.Parameters.AddWithValue("@fcNombre", cli.fcNombre);
                        cmd.Parameters.AddWithValue("@fcApePat", cli.fcApePat);
                        cmd.Parameters.AddWithValue("@fcApeMat", string.IsNullOrWhiteSpace(cli.fcApeMat) ? (object)DBNull.Value : cli.fcApeMat);
                        cmd.Parameters.AddWithValue("@fiTelefono", cli.fiTelefono == 0 ? (object)DBNull.Value : (long)cli.fiTelefono);
                        cmd.Parameters.AddWithValue("@fcEmail", string.IsNullOrWhiteSpace(cli.fcEmail) ? (object)DBNull.Value : cli.fcEmail);
                        cmd.Parameters.AddWithValue("@fiEmergencias", string.IsNullOrWhiteSpace(cli.fcEmergencia) ? (object)DBNull.Value : cli.fcEmergencia);
                        cmd.Parameters.Add("@fdFechaNac", SqlDbType.Date).Value = cli.fdFechaNac.Date;
                        cmd.Parameters.AddWithValue("@fcObservaciones", string.IsNullOrWhiteSpace(cli.fcObservaciones) ? (object)DBNull.Value : cli.fcObservaciones);


                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar: " + ex.Message);
            }
        }

        public bool Eliminar(int id)
        {
            try
            {
                using (var conexion = GetConnection())
                {
                    using (var cmd = new SqlCommand("sp_EliminarCliente", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fiCliente", id);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de baja: " + ex.Message);
            }
        }

        public List<Cliente> Buscar(string texto)
        {
            var lista = new List<Cliente>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarCliente", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@texto", texto);

                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                fiCliente = Convert.ToInt32(dr["fiCliente"]),
                                fcNombre = dr["fcNombre"].ToString(),
                                fcApePat = dr["fcApePat"].ToString(),
                                fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : "",
                                fiTelefono = dr["fiTelefono"] != DBNull.Value ? Convert.ToInt64(dr["fiTelefono"]) : 0,
                                fcEmail = dr["fcEmail"] != DBNull.Value ? dr["fcEmail"].ToString() : "",
                                fcEmergencia = dr["fiEmergencias"] != DBNull.Value ? dr["fiEmergencias"].ToString() : "",
                                fdFechaNac = dr["fdFechaNac"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaNac"]) : new DateTime(1990, 1, 1),
                                fcObservaciones = dr["fcObservaciones"] != DBNull.Value ? dr["fcObservaciones"].ToString() : "",
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Cliente> ObtenerClientesSinHuella()
        {
            List<Cliente> lista = new List<Cliente>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerClientesSinHuella", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Cliente
                            {
                                fiCliente = Convert.ToInt32(dr["fiCliente"]),
                                fcNombre = dr["fcNombre"].ToString(),
                                fcApePat = dr["fcApePat"].ToString(),
                                fcApeMat = dr["fcApeMat"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}