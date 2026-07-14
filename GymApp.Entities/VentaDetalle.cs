using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Entities
{
    public class VentaDetalle
    {
        public int fiProducto { get; set; } // El SKU genérico para Detalle_Ventas
        public int fiConceptoOmni { get; set; } // El ID real de la membresía o visita
        public string fcDescripcion { get; set; }
        public int fiCantidad { get; set; }
        public decimal fiPrecio { get; set; }
        public decimal fmSubtotal => fiCantidad * fiPrecio;
    }
}
