using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.BLL
{
    public class ProductoBLL
    {
        public List<Entities.Producto> ObtenerTodos()
        {
            var dao = new DAO.ProductoDAO();
            return dao.ObtenerTodos();
        }
        public bool RegistrarNuevo(Entities.Producto prod)
        {
            var dao = new DAO.ProductoDAO();
            return dao.Insertar(prod);
        }

        public bool RegistrarOrdenCompra(decimal total, List<Entities.CompraDetalle> detalles)
        {
            var dao = new DAO.ProductoDAO();
            return dao.RegistrarOrdenCompra(total, detalles);
        }

        // Asegúrate de tener: using System.Data; hasta arriba

        public DataTable ObtenerRubros()
        {
            var dao = new DAO.ProductoDAO();
            return dao.ObtenerRubros();
        }

        public Entities.Producto ObtenerPorId(int id)
        {
            var dao = new DAO.ProductoDAO();
            return dao.ObtenerPorId(id);
        }

        public bool Editar(Entities.Producto prod)
        {
            var dao = new DAO.ProductoDAO();
            return dao.Editar(prod);
        }

        public bool Eliminar(int id)
        {
            var dao = new DAO.ProductoDAO();
            return dao.Eliminar(id);
        }

        public List<Entities.Producto> Buscar(string texto)
        {
            var dao = new DAO.ProductoDAO();
            return dao.Buscar(texto);
        }
    }
}