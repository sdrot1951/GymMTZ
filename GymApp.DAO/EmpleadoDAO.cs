using Microsoft.Data.SqlClient;
using System;
using GymApp.Entities;
using System.Data;
using System.Collections.Generic;

namespace GymApp.DAO
{
    public class EmpleadoDAO : ConexionDB
    {
        public bool Insertar(Empleado emp)
        {
            using (var conexion = GetConnection())
            {
                using (var comando = new SqlCommand("sp_InsertarEmpleadoUsuario", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    // Parámetros del empleado
                    comando.Parameters.AddWithValue("@fcNombre", emp.fcNombre);
                    comando.Parameters.AddWithValue("@fcApePat", emp.fcApePat);
                    comando.Parameters.AddWithValue("@fcApeMat", string.IsNullOrWhiteSpace(emp.fcApeMat) ? (object)DBNull.Value : emp.fcApeMat);
                    comando.Parameters.AddWithValue("@fcDireccion", emp.fcDireccion);
                    comando.Parameters.AddWithValue("@fcEmail", emp.fcEmail);
                    comando.Parameters.AddWithValue("@fiTelefono", emp.fiTelefono);
                    comando.Parameters.AddWithValue("@fdFechaNac", emp.fdFechaNac);
                    comando.Parameters.AddWithValue("@fiPuesto", emp.fiPuesto);


                    // Parámetros del Usuario (Manejando nulos de forma segura)
                    if (!string.IsNullOrWhiteSpace(emp.fcUsuario) && !string.IsNullOrWhiteSpace(emp.fcPassword))
                    {
                        comando.Parameters.AddWithValue("@fcUsuario", emp.fcUsuario);
                        comando.Parameters.AddWithValue("@fcPassword", emp.fcPassword);
                    }
                    else
                    {
                        comando.Parameters.AddWithValue("@fcUsuario", DBNull.Value);
                        comando.Parameters.AddWithValue("@fcPassword", DBNull.Value);
                    }

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
        }

        public List<Empleado> ObtenerTodos()
        {
            var lista = new List<Empleado>();

            try
            {
                using (var conexion = GetConnection())
                {
                    conexion.Open();

                    // 1. Llamamos al Procedimiento Almacenado
                    using (var comando = new SqlCommand("sp_ObtenerEmpleados", conexion))
                    {
                        // 2. Indicamos que es un SP
                        comando.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var emp = new Empleado
                                {
                                    fiEmpleado = Convert.ToInt32(reader["fiEmpleado"]),
                                    fcNombre = reader["fcNombre"].ToString(),
                                    fcApePat = reader["fcApePat"].ToString(),
                                    fcApeMat = reader["fcApeMat"] != DBNull.Value ? reader["fcApeMat"].ToString() : "",
                                    fdFechaNac = Convert.ToDateTime(reader["fdFechaNac"]),
                                   // fdFechaReg = Convert.ToDateTime(reader["fdFechaReg"]),
                                    fcDireccion = reader["fcDireccion"] != DBNull.Value ? reader["fcDireccion"].ToString() : "",
                                    fiTelefono = reader["fiTelefono"] != DBNull.Value ? Convert.ToInt64(reader["fiTelefono"]) : 0,
                                    fcEmail = reader["fcEmail"] != DBNull.Value ? reader["fcEmail"].ToString() : "",
                                    flActivo = Convert.ToBoolean(reader["flActivo"]),
                                    fiPuesto = Convert.ToInt32(reader["fiPuesto"]),

                                    // 3. Guardamos el nombre del puesto en su propia variable limpia
                                    fcNombrePuesto = reader["NombrePuesto"].ToString()
                                };

                                lista.Add(emp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar empleados en BD: " + ex.Message);
            }

            return lista;
        }

        public bool Eliminar(int idEmpleado)
        {
            using (var conexion = GetConnection())
            {
                using (var comando = new SqlCommand("sp_EliminarEmpleado", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@fiEmpleado", idEmpleado);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    // Retorna true si al menos un registro fue actualizado
                    return filasAfectadas > 0;
                }
            }
        }

        public List<Empleado> Buscar(string texto)
        {
            List<Empleado> lista = new List<Empleado>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_BuscarEmpleado", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cTextoBusqueda", texto);
                    conexion.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Empleado
                            {
                                // Mapeo completo de las columnas de SQL a tu objeto C#
                                fiEmpleado = Convert.ToInt32(dr["fiEmpleado"]),
                                fcNombre = dr["fcNombre"].ToString(),

                                // Validamos DBNull por si algunos apellidos o correos vienen vacíos
                                fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : "",
                                fcApePat = dr["fcApePat"] != DBNull.Value ? dr["fcApePat"].ToString() : "",
                                fcNombrePuesto = dr["fcDescripcion"].ToString(),

                                // Telefono lo casteamos a Int64 (long) como lo tienes en tu entidad
                                fiTelefono = dr["fiTelefono"] != DBNull.Value ? Convert.ToInt64(dr["fiTelefono"]) : 0,

                                fcEmail = dr["fcEmail"] != DBNull.Value ? dr["fcEmail"].ToString() : "",

                                // La fecha real de registro
                                fdFechaNac = Convert.ToDateTime(dr["fdFechaNac"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool EditarEmpleado(Empleado empleado)
            {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_EditarEmpleado", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Llave primaria para saber a quién editar
                    cmd.Parameters.AddWithValue("@fiEmpleado", empleado.fiEmpleado);

                    // Campos a actualizar
                    cmd.Parameters.AddWithValue("@fcNombre", empleado.fcNombre);
                    cmd.Parameters.AddWithValue("@fcApeMat", string.IsNullOrWhiteSpace(empleado.fcApeMat) ? (object)DBNull.Value : empleado.fcApeMat);
                    cmd.Parameters.AddWithValue("@fcApePat", string.IsNullOrWhiteSpace(empleado.fcApePat) ? (object)DBNull.Value : empleado.fcApePat);
                    cmd.Parameters.AddWithValue("@fdFechaNac", empleado.fdFechaNac);
                   // cmd.Parameters.AddWithValue("@fdFechaReg", empleado.fdFechaReg);
                    cmd.Parameters.AddWithValue("@fcDireccion", string.IsNullOrWhiteSpace(empleado.fcDireccion) ? (object)DBNull.Value : empleado.fcDireccion);
                    cmd.Parameters.AddWithValue("@fiTelefono", empleado.fiTelefono == 0 ? (object)DBNull.Value : empleado.fiTelefono);
                    cmd.Parameters.AddWithValue("@fcEmail", string.IsNullOrWhiteSpace(empleado.fcEmail) ? (object)DBNull.Value : empleado.fcEmail);

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Retorna true si se actualizó al menos 1 fila
                    return filasAfectadas > 0;
                }
            }
        }

        public Empleado ObtenerPorId(int idEmpleado)
        {
            Empleado emp = null;
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ObtenerEmpleadoPorId", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiEmpleado", idEmpleado);
                    conexion.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Usamos 'if' porque solo esperamos 1 registro o ninguno
                        {
                            emp = new Empleado
                            {
                                fiEmpleado = Convert.ToInt32(dr["fiEmpleado"]),
                                fcNombre = dr["fcNombre"].ToString(),
                                fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : "",
                                fcApePat = dr["fcApePat"] != DBNull.Value ? dr["fcApePat"].ToString() : "",
                                fiPuesto = Convert.ToInt32(dr["fiPuesto"]), // ID numérico para el ComboBox
                                fdFechaNac = Convert.ToDateTime(dr["fdFechaNac"]),
                                //fdFechaReg = Convert.ToDateTime(dr["fdFechaReg"]),
                                fcDireccion = dr["fcDireccion"] != DBNull.Value ? dr["fcDireccion"].ToString() : "",
                                fiTelefono = dr["fiTelefono"] != DBNull.Value ? Convert.ToInt64(dr["fiTelefono"]) : 0,
                                fcEmail = dr["fcEmail"] != DBNull.Value ? dr["fcEmail"].ToString() : ""
                            };
                        }
                    }
                }
            }
            return emp;
        }

    }
}