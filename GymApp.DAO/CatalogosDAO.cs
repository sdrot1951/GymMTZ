using Microsoft.Data.SqlClient;
using System.Data;
using System;

namespace GymApp.DAO
{
    public class CatalogosDAO : ConexionDB // Asegúrate de heredar de tu BaseDAO para la conexión
    {
        public bool InsertarMembresia(string nombre, int dias, decimal precio)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand("sp_InsertarMembresia", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcNombre", nombre);
                cmd.Parameters.AddWithValue("@fiDiasDuracion", dias);
                cmd.Parameters.AddWithValue("@fmPrecio", precio);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool InsertarVisita(string descripcion, decimal precio)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand("sp_InsertarVisita", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                cmd.Parameters.AddWithValue("@fmPrecio", precio);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable ConsultarCatalogo(string tipo)
        {
            DataTable dt = new DataTable();
            string query = "";

            // Elegimos la consulta dependiendo del panel que se está dibujando
            switch (tipo)
            {
                case "Gasto": query = "SELECT fiTipoGasto AS ID, fcDescripcion AS Descripción FROM Cat_Gastos"; break;
                case "Pago": query = "SELECT fiPago AS ID, fcDescripcion AS Descripción FROM Cat_Pagos"; break;
                case "Rubro": query = "SELECT fiRubro AS ID, fcDescripcion AS Descripción FROM Cat_Rubros"; break;
                case "Visita": query = "SELECT fiVisita AS ID, fcDescripcion AS Descripción, fmPrecio AS Precio FROM catalogo_visitas WHERE flActivo = 1"; break;
                case "Membresia": query = "SELECT fiMembresia AS ID, fcNombre AS Nombre, fiDiasDuracion AS Dias, fmPrecio AS Precio FROM membresia WHERE flActivo = 1"; break;
            }

            if (string.IsNullOrEmpty(query)) return dt;

            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
            {
                con.Open();
                using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public bool InsertarCatalogoGenerico(string tablaDestino, string descripcion)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand("sp_InsertarCatalogoGenerico", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcTabla", tablaDestino);
                cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==============================================================
        // 1. MÉTODOS PARA EDITAR Y ELIMINAR CATÁLOGOS GENÉRICOS
        // ==============================================================
        public bool EditarCatalogoGenerico(string tablaDestino, int id, string descripcion)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EditarCatalogoGenerico", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcTabla", tablaDestino);
                cmd.Parameters.AddWithValue("@fiID", id);
                cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EliminarCatalogoGenerico(string tablaDestino, int id)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EliminarCatalogoGenerico", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fcTabla", tablaDestino);
                cmd.Parameters.AddWithValue("@fiID", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==============================================================
        // 2. MÉTODOS PARA EDITAR Y ELIMINAR MEMBRESÍAS
        // ==============================================================
        public bool EditarMembresia(int id, string nombre, int dias, decimal precio)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EditarMembresia", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fiMembresia", id);
                cmd.Parameters.AddWithValue("@fcNombre", nombre);
                cmd.Parameters.AddWithValue("@fiDias", dias);
                cmd.Parameters.AddWithValue("@fmPrecio", precio);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EliminarMembresia(int id)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EliminarMembresia", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fiMembresia", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==============================================================
        // 3. MÉTODOS PARA EDITAR Y ELIMINAR VISITAS/PASES
        // ==============================================================
        public bool EditarVisita(int id, string descripcion, decimal precio)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EditarVisita", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fiVisita", id);
                cmd.Parameters.AddWithValue("@fcDescripcion", descripcion);
                cmd.Parameters.AddWithValue("@fmPrecio", precio);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EliminarVisita(int id)
        {
            using (var con = GetConnection())
            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("sp_EliminarVisita", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fiVisita", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}