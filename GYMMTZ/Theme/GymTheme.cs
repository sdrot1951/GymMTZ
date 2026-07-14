using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMMTZ.Theme
{
    internal class GymTheme
    {
        // === PALETA DE COLORES GYM ===
        public static readonly Color Background = Color.FromArgb(18, 18, 20);      // Negro carbón
        public static readonly Color Surface = Color.FromArgb(26, 26, 30);      // Gris oscuro
        public static readonly Color SurfaceElevated = Color.FromArgb(34, 34, 40);      // Panel elevado
        public static readonly Color Accent = Color.FromArgb(255, 69, 0);      // Naranja fuego
        public static readonly Color AccentHover = Color.FromArgb(255, 100, 30);    // Naranja claro
        public static readonly Color AccentDark = Color.FromArgb(180, 45, 0);      // Naranja oscuro
        public static readonly Color Gold = Color.FromArgb(255, 200, 0);     // Dorado
        public static readonly Color TextPrimary = Color.FromArgb(240, 240, 245);   // Blanco suave
        public static readonly Color TextSecondary = Color.FromArgb(150, 150, 160);   // Gris claro
        public static readonly Color TextMuted = Color.FromArgb(90, 90, 100);     // Gris muted
        public static readonly Color Border = Color.FromArgb(48, 48, 56);      // Borde sutil
        public static readonly Color BorderAccent = Color.FromArgb(255, 69, 0);      // Borde naranja
        public static readonly Color Success = Color.FromArgb(34, 197, 94);     // Verde
        public static readonly Color Warning = Color.FromArgb(234, 179, 8);     // Amarillo
        public static readonly Color Danger = Color.FromArgb(239, 68, 68);     // Rojo
        public static readonly Color MenuBackground = Color.FromArgb(22, 22, 26);      // Menú fondo
        public static readonly Color MenuHover = Color.FromArgb(255, 69, 0, 30);  // Menú hover
        public static readonly Color MenuSelected = Color.FromArgb(255, 69, 0);      // Menú seleccionado
        public static readonly Color HeaderBackground = Color.FromArgb(20, 20, 24);      // Header

        // === TIPOGRAFÍA ===
        public static readonly Font FontTitle = new Font("Segoe UI", 22f, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 13f, FontStyle.Bold);
        public static readonly Font FontBody = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontSmall = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font FontBold = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontMono = new Font("Consolas", 9f, FontStyle.Regular);
        public static readonly Font FontMenuIcon = new Font("Segoe UI", 14f, FontStyle.Regular);
        public static readonly Font FontMenuItem = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontMenuSec = new Font("Segoe UI", 8f, FontStyle.Regular);

        // === RADIOS Y MEDIDAS ===
        public const int Radius = 8;
        public const int RadiusLarge = 12;
        public const int MenuWidth = 220;
        public const int HeaderHeight = 64;
        public const int Padding = 20;
    }
}
