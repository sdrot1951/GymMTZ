using System.Data;
using GymApp.DAO;


namespace GymApp.BLL
{
    public class PuestoBLL
    {
        private PuestoDAO _puestoDAO = new PuestoDAO();
        public DataTable ObtenerCatalogoPuestos()
        {
            return _puestoDAO.ObtenerPuestos();
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


    }



}
