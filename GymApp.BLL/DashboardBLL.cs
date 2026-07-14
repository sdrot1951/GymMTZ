using System;
using System.Data;
using GymApp.DAO;

namespace GymApp.BLL
{
    public class DashboardBLL
    {
        public DataSet ObtenerDatosDashboard()
        {
            try
            {
                var dao = new DashboardDAO();
                return dao.ObtenerDatosDashboard();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar los datos del Dashboard: " + ex.Message);
            }
        }
    }
}