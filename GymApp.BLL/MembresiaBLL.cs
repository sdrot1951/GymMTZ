using System;
using System.Data;
using GymApp.DAO;

namespace GymApp.BLL
{
    public class MembresiaBLL
    {
        public DataTable ConsultarMembresiasClientes(string filtro)
        {
            try
            {
                // Regla de limpieza: evitamos espacios en blanco accidentales que arruinen la búsqueda en SQL
                string filtroLimpio = string.IsNullOrWhiteSpace(filtro) ? "" : filtro.Trim();

                // Instanciamos el DAO y pasamos la petición
                MembresiaDAO dao = new MembresiaDAO();
                return dao.ConsultarMembresiasClientes(filtroLimpio);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la solicitud de membresías: " + ex.Message);
            }
        }
    }
}