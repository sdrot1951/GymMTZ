using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Entities
{
    public class Empleado
    {
        public int fiEmpleado { get; set; }
        public string fcNombre { get; set; } = string.Empty;
        public string fcApeMat { get; set; } = string.Empty;
        public string fcApePat { get; set; } = string.Empty;
        public DateTime fdFechaNac { get; set; }
        public DateTime fdFechaReg { get; set; }
        public string fcDireccion { get; set; } = string.Empty;
        public long fiTelefono { get; set; }
        public string fcEmail { get; set; } = string.Empty;
        public bool flActivo { get; set; }
        public int fiPuesto { get; set; }
        public string fcNombrePuesto { get; set; } = string.Empty;

        public string fcUsuario { get; set; }
        public string fcPassword { get; set; }
    }
}
