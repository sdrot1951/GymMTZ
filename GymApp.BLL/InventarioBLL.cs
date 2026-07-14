using GymApp.DAO;
using GymApp.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymApp.BLL
{
    public class InventarioBLL
    {
        /*public DataTable ConsultarInventario(string filtro)
        {
            try
            {
                string filtroLimpio = string.IsNullOrWhiteSpace(filtro) ? "" : filtro.Trim();
                var dao = new InventarioDAO();
                return dao.ConsultarInventario(filtroLimpio);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la solicitud de inventario: " + ex.Message);
            }
        }

        */
        public System.Data.DataTable ConsultarKardex(string busqueda, DateTime desde, DateTime hasta)
        {
            // Llamamos al DAO y le pasamos los parámetros
            var dao = new InventarioDAO();
            return dao.ConsultarKardex(busqueda, desde, hasta);
        }
        public bool RegistrarCompra(string folio, decimal total, int idEmpleado, List<CompraDetalle> carrito)
        {
            if (string.IsNullOrWhiteSpace(folio)) throw new Exception("El folio de la nota o factura es obligatorio.");
            if (carrito == null || carrito.Count == 0) throw new Exception("El carrito de compras está vacío.");
            if (total <= 0) throw new Exception("El total de la compra debe ser mayor a cero.");
            if (idEmpleado <= 0) throw new Exception("No se ha identificado al usuario activo.");

            var dao = new InventarioDAO();
            return dao.RegistrarCompra(folio, total, idEmpleado, carrito);
        }

        public DataTable ConsultarCompras(string filtro, DateTime desde, DateTime hasta)
        {
            var dao = new InventarioDAO();
            return dao.ConsultarCompras(filtro ?? "", desde, hasta);
        }
    }
}