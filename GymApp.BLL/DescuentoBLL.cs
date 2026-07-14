using System;
using System.Data;
using GymApp.DAO;

namespace GymApp.BLL
{
    public class DescuentoBLL
    {
        public DataTable ConsultarDescuentos(string filtro, DateTime? desde, DateTime? hasta)
        {
            try
            {
                string filtroLimpio = string.IsNullOrWhiteSpace(filtro) ? "" : filtro.Trim();

                // Validamos que las fechas tengan sentido
                if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                {
                    throw new Exception("La fecha de inicio no puede ser mayor que la fecha final.");
                }

                DescuentoDAO dao = new DescuentoDAO();
                return dao.ConsultarDescuentos(filtroLimpio, desde, hasta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la consulta de descuentos: " + ex.Message);
            }
        }
    }
}