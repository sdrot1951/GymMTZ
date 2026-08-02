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

        public void ActualizarFechaMembresia(int idMembresia, DateTime nuevaFecha, string usuarioModifica)
        {
            try
            {
                // Validación básica de regla de negocio
                if (idMembresia <= 0)
                    throw new ArgumentException("El ID de la membresía no es válido.");

                if (string.IsNullOrWhiteSpace(usuarioModifica))
                    throw new ArgumentException("Se requiere el usuario administrador para el registro.");

                // Instanciamos el DAO y pasamos la petición[cite: 1]
                MembresiaDAO dao = new MembresiaDAO();
                dao.ActualizarFechaMembresia(idMembresia, nuevaFecha, usuarioModifica.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la actualización de membresía: " + ex.Message);
            }
        }
    }
}