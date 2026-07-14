using System;

namespace GymApp.Core
{
    // Clase modelo para la caché en memoria RAM
    public class RegistroBiometricoCache
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public byte[] TemplateHuella { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string EstadoMembresia { get; set; }
        public int DiasRestantes { get; set; }

        public byte[] FotoBytes { get; set; }
    }
}