namespace GymApp.Entities
{
    public class CompraDetalle
    {
        public int fiProducto { get; set; }
        public string fcDescripcion { get; set; } // Auxiliar visual
        public int fiCantidad { get; set; }
        public decimal fmCostoUnitario { get; set; }
        public decimal fmSubtotal => fiCantidad * fmCostoUnitario;
    }
}