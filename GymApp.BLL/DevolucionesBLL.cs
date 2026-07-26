using GymApp.DAO;
using System;
using System.Data;

namespace GymApp.BLL
{
    public class DevolucionesBLL
    {
        private readonly DevolucionesDAO _dao = new DevolucionesDAO();

        public void RegistrarDevolucion(int idVenta, decimal montoDevuelto, string motivo)
        {
            // 1. VALIDACIÓN ESTRICTA DE SEGURIDAD (Jerarquía)
            string puesto = GymApp.Core.SesionGlobal.NombrePuesto?.ToUpper();

            if (puesto != "ADMINISTRADOR" && puesto != "GERENTE")
            {
                throw new Exception("ACCESO DENEGADO: No tienes los permisos suficientes para autorizar cancelaciones o devoluciones de efectivo. Se requiere la autorización de un Administrador o Gerente.");
            }

            // 2. Validaciones de negocio
            if (idVenta <= 0) throw new Exception("Debe especificar un folio de venta válido.");
            if (montoDevuelto <= 0) throw new Exception("El monto a devolver debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 5) throw new Exception("Debe especificar un motivo válido y descriptivo para la auditoría.");

            // 3. Procesar en base de datos
            int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
            _dao.RegistrarDevolucion(idVenta, idEmpleadoLogueado, motivo, montoDevuelto);
        }

        public DataTable ConsultarDevoluciones(string filtro, DateTime desde, DateTime hasta)
        {
            if (desde > hasta) throw new Exception("La fecha 'Desde' no puede ser mayor que 'Hasta'.");
            return _dao.ConsultarDevoluciones(filtro, desde, hasta);
        }


        public DataTable ConsultarTicket(int folio)
        {
            if (folio <= 0) throw new Exception("El folio ingresado no es válido.");
            return _dao.ConsultarTicketParaCancelacion(folio);
        }
    }
}