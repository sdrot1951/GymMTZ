
using System;
using System.Data;
using GymApp.Entities;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public class UsuarioDAO : ConexionDB
    {
        public DataTable ObtenerPorUsername(string username)
        {
            DataTable dt = new DataTable();
            using (var conexion = GetConnection())
            {
                using (var comando = new SqlCommand("sp_ObtenerUsuarioPorUsername", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@fcUsername", username);

                    SqlDataAdapter da = new SqlDataAdapter(comando);
                    da.Fill(dt);
                }
            }
            return dt;
        }


        public Empleado ValidarLogin(string usuario, string password)
        {
            Empleado emp = null; // Inicia nulo por si falla el login

            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_ValidarLogin", conexion)) // Tu SP de login
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Si encontró al usuario
                        {
                            emp = new Empleado
                            {
                                fiEmpleado = Convert.ToInt32(dr["fiEmpleado"]),
                                fcNombre = dr["fcNombre"].ToString(),
                                fcApePat = dr["fcApePat"].ToString(),
                                fcApeMat = dr["fcApeMat"] != DBNull.Value ? dr["fcApeMat"].ToString() : "",
                                fiPuesto = Convert.ToInt32(dr["fiPuesto"]),
                                fcNombrePuesto = dr["NombrePuesto"].ToString() // Asegúrate de que tu SP devuelva esto
                            };
                        }
                    }
                }
            }
            return emp; // Devuelve los datos o null si las credenciales fueron incorrectas
        }
    }
}