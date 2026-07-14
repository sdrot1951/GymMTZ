using System;
using System.Data;
using GymApp.DAO;

namespace GymApp.BLL
{
    public class GastosBLL
    {
        public DataTable ConsultarGastos(string filtro, DateTime? desde, DateTime? hasta)
        {
            try
            {
                string filtroLimpio = string.IsNullOrWhiteSpace(filtro) ? "" : filtro.Trim();

                // Regla de Negocio: Validar consistencia del calendario
                if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                {
                    throw new Exception("La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.");
                }

                GastosDAO dao = new GastosDAO();
                return dao.ConsultarGastos(filtroLimpio, desde, hasta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la solicitud de gastos: " + ex.Message);
            }
        }

        public DataTable ObtenerCategorias()
        {
            var dao = new GastosDAO();
            return dao.ObtenerCategorias();
        }

        public bool RegistrarGasto(string descripcion, decimal monto, int tipoGasto, int idEmpleado)
        {
            // Validaciones estrictas antes de tocar la base de datos
            if (string.IsNullOrWhiteSpace(descripcion)) throw new Exception("La descripción del gasto es obligatoria.");
            if (monto <= 0) throw new Exception("El monto debe ser mayor a cero.");
            if (tipoGasto <= 0) throw new Exception("Debe seleccionar una categoría válida.");
            if (idEmpleado <= 0) throw new Exception("Error de sesión: No se detectó un empleado válido.");

            var dao = new GastosDAO();
            return dao.InsertarGasto(descripcion.Trim(), monto, tipoGasto, idEmpleado);
        }



        public DataTable ConsultarGastosMensuales(string filtro, DateTime? desde, DateTime? hasta)
        {
            try
            {
                string filtroLimpio = string.IsNullOrWhiteSpace(filtro) ? "" : filtro.Trim();

                // Regla de Negocio: Validar consistencia del calendario
                if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                {
                    throw new Exception("La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.");
                }

                GastosDAO dao = new GastosDAO();
                return dao.ConsultarGastosMensual(filtroLimpio, desde, hasta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la solicitud de gastos: " + ex.Message);
            }
                }

        public bool RegistrarGastoMensual(string descripcion, decimal monto, int tipoGasto, int idEmpleado)
        {
            // Validaciones estrictas antes de tocar la base de datos
            if (string.IsNullOrWhiteSpace(descripcion)) throw new Exception("La descripción del gasto es obligatoria.");
            if (monto <= 0) throw new Exception("El monto debe ser mayor a cero.");
            if (tipoGasto <= 0) throw new Exception("Debe seleccionar una categoría válida.");
            if (idEmpleado <= 0) throw new Exception("Error de sesión: No se detectó un empleado válido.");

            var dao = new GastosDAO();
            return dao.InsertarGastoMensual(descripcion.Trim(), monto, tipoGasto, idEmpleado);
        }


    }
}