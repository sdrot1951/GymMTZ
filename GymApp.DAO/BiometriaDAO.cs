using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using GymApp.Entities;
using Microsoft.Data.SqlClient;


namespace GymApp.DAO
{
    public class  BiometriaDAO : ConexionDB
    {

        public bool GuardarHuella(int idCliente, byte[] template)
        {
            using (var conexion = GetConnection())
            {
                using (var cmd = new SqlCommand("sp_RegistrarHuella", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Asignamos parámetros
                    cmd.Parameters.Add("@fiCliente", SqlDbType.Int).Value = idCliente;
                    cmd.Parameters.Add("@fcHuella", SqlDbType.VarBinary).Value = template;

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
    }
}


