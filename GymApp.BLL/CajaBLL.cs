using GymApp.DAO;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymApp.BLL
{
    public class CajaBLL
    {
        private readonly DAO.CajaDAO _dao = new DAO.CajaDAO();

        public DataTable ConsultarCaja(DateTime desde, DateTime hasta)
        {
            // Limpiamos el texto por si mandan la marca de agua del buscador
            //string filtroLimpio = filtro == "🔍  Buscar..." ? "" : filtro.Trim();
            return _dao.ObtenerMovimientos( desde, hasta);
        }

        public DataTable BuscarCajaTexto(string filtro)
        {
            string filtroLimpio = filtro == "🔍  Buscar..." ? "" : filtro.Trim();
            return _dao.BuscarMovimientosTexto(filtroLimpio);
        }
        public int GenerarCorte(int idEmpleado, decimal fondo, decimal declarado, string obs)
        {
            var dao = new CajaDAO();
            return dao.GenerarCorte(idEmpleado, fondo, declarado, obs);
        }


        public DataTable ObtenerDetalleMovimientos(DateTime inicio, DateTime fin, int empleadoturno)
        {
            // Validaciones de negocio preventivas
            if (inicio > fin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }

            // Solicitamos los datos a la capa DAO
            return _dao.ObtenerDetalleMovimientos(inicio, fin, empleadoturno);
        }

        public decimal ObtenerTotalEsperadoDia(int empleadoTurno)
        {
            var dao = new CajaDAO(); // Asumiendo que tu DAO de caja se llama así
            return dao.ObtenerTotalEsperadoDia(empleadoTurno);
        }

        public DataTable ConsultarCortesTurno(DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaDesde > fechaHasta) throw new Exception("La fecha de inicio no puede ser mayor a la fecha final.");
            var dao = new DAO.CajaDAO();
            return dao.ConsultarCortesTurno(fechaDesde, fechaHasta);
        }

        public DataTable ConsultarCortesDiarios(DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaDesde > fechaHasta) throw new Exception("La fecha de inicio no puede ser mayor a la fecha final.");
            var dao = new DAO.CajaDAO();
            return dao.ConsultarCortesDiarios(fechaDesde, fechaHasta);
        }

        public (decimal Entradas, decimal Salidas) PrevisualizarCorte()
        {
            var dao = new DAO.CajaDAO(); // Ajusta a tu namespace
            return dao.PrevisualizarCorte();
        }

        //Corte Turno
        public int ProcesarCorteCaja(int idEmpleado, decimal fondoInicial, decimal montoDeclarado, string observaciones)
        {
            if (idEmpleado <= 0) throw new Exception("Error de sesión: No se identificó al empleado.");
            if (montoDeclarado < 0) throw new Exception("El monto declarado no puede ser negativo.");

            var dao = new DAO.CajaDAO();
            return dao.RegistrarCorte(idEmpleado, fondoInicial, montoDeclarado, observaciones);
        }


    }
}