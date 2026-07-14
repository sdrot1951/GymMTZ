using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GymApp.Core;


namespace GymApp.DAO
{
    public class AccesoDAO : ConexionDB
    {
        
        public List<RegistroBiometricoCache> CargarCatalogoBiometrico()
        {
            var lista = new List<RegistroBiometricoCache>();
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ObtenerCatalogoAccesoBiometrico", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new RegistroBiometricoCache
                            {
                                IdCliente = Convert.ToInt32(dr["fiCliente"]),
                                NombreCliente = dr["fcClienteNombre"].ToString(),
                                TemplateHuella = (byte[])dr["fcHuella"],
                                FechaVencimiento = dr["fdFechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(dr["fdFechaVencimiento"]) : (DateTime?)null,
                                EstadoMembresia = dr["fcEstadoMembresia"].ToString(),
                                DiasRestantes = Convert.ToInt32(dr["fiDiasRestantes"]),

                                // ====== AQUÍ ADAPTAMOS TU LÓGICA DE LA FOTO ======
                                FotoBytes = dr["fbFoto"] != DBNull.Value ? (byte[])dr["fbFoto"] : null
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void RegistrarAsistencia(int idCliente, string tipoAcceso)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarAsistencia", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fiCliente", idCliente);
                    cmd.Parameters.AddWithValue("@fcTipo", tipoAcceso);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public System.Data.DataTable ConsultarAsistencias(string busqueda, DateTime desde, DateTime hasta)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (var conexion = GetConnection()) // Asumo que tu método de conexión se llama GetConnection()
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_ConsultarAsistencias", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fcBusqueda", busqueda);
                    cmd.Parameters.AddWithValue("@fdDesde", desde.Date);
                    cmd.Parameters.AddWithValue("@fdHasta", hasta.Date);

                    conexion.Open();
                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public System.Data.DataTable BuscarAccesoManual(string textoBusqueda)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (var conexion = GetConnection())
            {
                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_BuscarClienteAccesoManual", conexion))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fcBusqueda", textoBusqueda);

                    conexion.Open();
                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }


    }
}