using System;
using GymApp.Entities; // Asegúrate de referenciar donde vive tu entidad Empleado

namespace GymApp.Core // Ajusta el namespace según tu estructura
{
    public static class SesionGlobal
    {
        // Propiedades de solo lectura desde afuera, para que nadie las sobreescriba por accidente
        public static int IdEmpleado { get; private set; }
        public static string NombreCompleto { get; private set; }
        public static int IdPuesto { get; private set; }
        public static string NombrePuesto { get; private set; }

        // Método para cargar los datos en memoria al hacer Login exitoso
        public static void IniciarSesion(Empleado empleadoLogueado)
        {
            IdEmpleado = empleadoLogueado.fiEmpleado;

            // Concatenamos y limpiamos espacios extra
            NombreCompleto = $"{empleadoLogueado.fcNombre} {empleadoLogueado.fcApePat} {empleadoLogueado.fcApeMat}".Trim();

            IdPuesto = empleadoLogueado.fiPuesto;
            NombrePuesto = empleadoLogueado.fcNombrePuesto;
        }

        // Método para limpiar la memoria al "Cerrar Sesión" o bloquear el sistema
        public static void CerrarSesion()
        {
            IdEmpleado = 0;
            NombreCompleto = string.Empty;
            IdPuesto = 0;
            NombrePuesto = string.Empty;
        }

        // Bandera rápida para que tus pantallas validen si hay alguien adentro
        public static bool HaySesionActiva => IdEmpleado > 0;
    }
}