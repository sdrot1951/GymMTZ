using GymApp.DAO;

namespace GymApp.BLL
{
    public class CatalogosBLL
    {
        private CatalogosDAO _dao = new CatalogosDAO();



        public bool GuardarMembresia(string nombre, int dias, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre) || dias <= 0 || precio < 0)
                throw new System.Exception("Datos de membresía inválidos.");
            return _dao.InsertarMembresia(nombre, dias, precio);
        }

        public System.Data.DataTable ObtenerDatosCatalogo(string tipo)
        {
            return _dao.ConsultarCatalogo(tipo);
        }
        public bool GuardarVisita(string descripcion, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(descripcion) || precio < 0)
                throw new System.Exception("Datos de visita inválidos.");
            return _dao.InsertarVisita(descripcion, precio);
        }

        public bool GuardarCatalogoSimple(string tipoCatalogo, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new System.Exception("La descripción no puede estar vacía.");

            // Enrutador de tablas
            string tabla = "";
            switch (tipoCatalogo)
            {
                case "Gasto": tabla = "Cat_Gastos"; break;
                case "Pago": tabla = "Cat_Pagos"; break;
                case "Rubro": tabla = "Cat_Rubros"; break;
                default: throw new System.Exception("Catálogo no reconocido.");
            }

            return _dao.InsertarCatalogoGenerico(tabla, descripcion);
        }

        public bool EditarCatalogoSimple(string tipoCatalogo, int id, string descripcion)
        {
            string tabla = tipoCatalogo == "Gasto" ? "Cat_Gastos" : tipoCatalogo == "Pago" ? "Cat_Pagos" : "Cat_Rubros";
            return _dao.EditarCatalogoGenerico(tabla, id, descripcion);
        }

        public bool EliminarCatalogo(string tipoCatalogo, int id)
        {
            if (tipoCatalogo == "Membresia") return _dao.EliminarMembresia(id);
            if (tipoCatalogo == "Visita") return _dao.EliminarVisita(id);

            string tabla = tipoCatalogo == "Gasto" ? "Cat_Gastos" : tipoCatalogo == "Pago" ? "Cat_Pagos" : "Cat_Rubros";
            return _dao.EliminarCatalogoGenerico(tabla, id);
        }

        public bool EditarMembresia(int id, string nombre, int dias, decimal precio)
        {
            return _dao.EditarMembresia(id, nombre, dias, precio);
        }

        public bool EditarVisita(int id, string descripcion, decimal precio)
        {
            return _dao.EditarVisita(id, descripcion, precio);
        }
    }
}