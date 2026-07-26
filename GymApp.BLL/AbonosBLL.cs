using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.BLL
{
    public class AbonosBLL
    {
        public bool RegistrarAbono(int idSaldo, decimal montoAbono, int idTipoPago, int idEmpleado)
        {
            // Reglas de negocio rápidas
            if (idSaldo <= 0)
                throw new Exception("Debe seleccionar una deuda válida para abonar.");

            if (montoAbono <= 0)
                throw new Exception("El monto del abono debe ser mayor a cero.");

            if (idTipoPago <= 0)
                throw new Exception("Debe seleccionar una forma de pago válida.");  

            var dao = new DAO.AbonosDAO();
            return dao.RegistrarAbono(idSaldo, montoAbono, idTipoPago, idEmpleado);
        }


        public DataTable ConsultarDeudas(string texto, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var dao = new GymApp.DAO.AbonosDAO();
            return dao.ConsultarDeudas(texto, fechaDesde, fechaHasta);
        }
    }

}