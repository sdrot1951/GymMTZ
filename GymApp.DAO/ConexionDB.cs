using System.Configuration;
using Microsoft.Data.SqlClient;

namespace GymApp.DAO
{
    public abstract class ConexionDB
    {
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["GymDBConn"].ConnectionString);
        }
    }
}