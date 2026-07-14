using System;
namespace GymApp.Entities
{
    public class Cliente
    {
        public int fiCliente { get; set; }
        public string fcNombre { get; set; } = string.Empty;
        public string fcApeMat { get; set; } = string.Empty;
        public string fcApePat { get; set; } = string.Empty;
        public DateTime fdFechaNac { get; set; }
        public DateTime fdFechaReg { get; set; }
        public string fcDireccion { get; set; } = string.Empty;
        public long fiTelefono { get; set; }
        public string fcEmergencia { get; set; } = string.Empty;
        public string fcEmail { get; set; } = string.Empty;
        public bool flActivo { get; set; }

        public string fcObservaciones { get; set; }
    }

    public class PerfilClienteDTO
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
        public long Telefono { get; set; }
        public string Email { get; set; }
        public byte[] Foto { get; set; }

        public string Membresia { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int DiasRestantes { get; set; }
        public string EstadoMembresia { get; set; }

        public decimal DeudaTotal { get; set; }
        public int? IdDeudaPendiente { get; set; }

        public string fcObservaciones { get; set; }
    }
}