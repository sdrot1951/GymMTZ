using System;
using System.Collections.Generic;
using System.Data;
using GymApp.DAO;
using GymApp.Entities;

namespace GymApp.BLL
{
    public class VentaBLL
    {
       /* public bool ProcesarVentaOmni(int idCliente, int idVendedor, decimal total, int idTipoPago, string tipoVenta, List<VentaDetalle> carrito,
                                              bool aplicaDescuento = false, string descDescripcion = null, decimal descMonto = 0, int descAutoriza = 0)*/
        public bool ProcesarVentaOmni(int idCliente, int idVendedor, decimal total, decimal montoPagado, int idTipoPago, string tipoVenta, List<VentaDetalle> carrito, 
                                        bool aplicaDescuento = false, string descDescripcion = null, decimal descMonto = 0, int descAutoriza = 0)
        {
            // ==========================================
            // REGLAS DE NEGOCIO: VALIDACIÓN DE DESCUENTOS
            // ==========================================
            if (aplicaDescuento)
            {
                // 1. Validar que el tipo de venta permita descuento
                if (!tipoVenta.Equals("MEMBRESIA", StringComparison.OrdinalIgnoreCase) &&
                    !tipoVenta.Equals("VISITA", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Los descuentos solo aplican en la compra de Membresías o Visitas.");
                }

                // 2. Validar que el monto de descuento tenga lógica
                if (descMonto <= 0)
                {
                    throw new Exception("El monto del descuento debe ser mayor a cero.");
                }
           /*     if (descMonto > total)
                {
                    throw new Exception("El descuento no puede ser mayor o igual al total de la venta.");
                }*/


                // 3. Validar que exista una justificación
                if (string.IsNullOrWhiteSpace(descDescripcion))
                {
                    throw new Exception("Debe proporcionar un motivo o descripción para autorizar el descuento.");
                }

                if (montoPagado < 0)
                {
                    throw new Exception("El monto pagado no puede ser negativo.");
                }

                if (montoPagado > total)
                {
                    throw new Exception("El monto pagado no puede superar el total de la venta. Entregue el cambio al cliente e ingrese el monto exacto ingresado a caja.");
                }

                // Si el cliente paga menos del total, verificamos que NO sea Público General (ID 1)
                if (montoPagado < total && idCliente <= 1)
                {
                    throw new Exception("No se puede otorgar crédito o pagos parciales al 'Público General'. Por favor busque y seleccione un cliente registrado.");
                }
                // 4. APLICAR DESCUENTO: Restamos el monto al total original
                // total -= descMonto;
            }
            else
            {
                // Por seguridad, si la bandera es falsa, limpiamos las variables 
                // para que no se filtre basura a la base de datos.
                descMonto = 0;
                descDescripcion = null;
                descAutoriza = 0;
            }
            // ==========================================

            // Conectamos directamente con el método transaccional del DAO
            var dao = new DAO.VentaDAO();

            // Pasamos el "total" (que ya viene con la resta si hubo descuento) y los nuevos parámetros
      /*      return dao.ProcesarVentaOmni(idCliente, idVendedor, total, idTipoPago, tipoVenta, carrito,
                                         aplicaDescuento, descDescripcion, descMonto, descAutoriza);*/

            return dao.ProcesarVentaOmni(idCliente, idVendedor, total, montoPagado, idTipoPago, tipoVenta, carrito, aplicaDescuento, descDescripcion, descMonto, descAutoriza);
        }
        public DataTable ConsultarProductos(string filtro)
        {
            var dao = new VentaDAO();
            return dao.BuscarProducto(filtro);
        }

        public DataTable ConsultarProductosCompra(string filtro)
        {
            var dao = new VentaDAO();
            return dao.BuscarProductoCompra(filtro);
        }
        public DataTable BuscarPorDescripcion(string texto, bool buscarMembresia)
        {
            var dao = new DAO.VentaDAO();
            return dao.BuscarPorDescripcion(texto, buscarMembresia);
        }

        public DataTable ObtenerTiposPago()
        {
            var dao = new DAO.VentaDAO();
            return dao.ObtenerTiposPago();
        }

        public DataTable BuscarClientes(string texto)
        {
            var dao = new DAO.VentaDAO();
            return dao.BuscarClientes(texto);
        }

    

        public DataTable BuscarVisitas(string texto)
        {
            var dao = new DAO.VentaDAO();
            return dao.BuscarVisitas(texto);
        }

        public DataTable ConsultarVentas(string texto, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                // REGLA DE NEGOCIO: Validamos que las fechas tengan sentido antes de ir a la Base de Datos
                if (fechaDesde.HasValue && fechaHasta.HasValue)
                {
                    if (fechaDesde.Value.Date > fechaHasta.Value.Date)
                    {
                        throw new ArgumentException("La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.");
                    }
                }

                // Evitamos que manden valores nulos que puedan romper la búsqueda de texto
                string filtroLimpio = texto == null ? "" : texto.Trim();

                // Instanciamos el DAO (Data Access Object)
                var dao = new GymApp.DAO.VentaDAO(); // Ajusta el namespace si tu clase DAO se llama distinto

                // Llamamos al método que ya creaste en el DAO y retornamos el resultado
                return dao.ConsultarVentas(filtroLimpio, fechaDesde, fechaHasta);
            }
            catch (Exception ex)
            {
                // Si la validación falla o la base de datos devuelve error, lo enviamos al Formulario
                throw new Exception("Error al consultar el historial de ventas: " + ex.Message);
            }
        }
    }
}