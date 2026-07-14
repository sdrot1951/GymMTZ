namespace GymApp.Entities
{
    public class Producto
    {
        public int fiProducto { get; set; }
        public string fcDescripcion { get; set; }
        public decimal fiPrecio { get; set; }
        public decimal fiCosto { get; set; }
        public int fiRubro { get; set; }
        public string fcNombreRubro { get; set; }
        public int fiCantidad { get; set; } // Stock
    }
}