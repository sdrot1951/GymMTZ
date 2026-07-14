using GymApp.DAO;
using GymApp.Entities;
using System;
using BCrypt.Net;
using System.Collections.Generic;

namespace GymApp.BLL
{
    public class EmpleadoBLL
    {
        private EmpleadoDAO _empleadoDAO = new EmpleadoDAO();
        public bool RegistrarNuevo(Empleado nuevoEmpleado)
        {
            // Validaciones básicas de negocio
            if (string.IsNullOrWhiteSpace(nuevoEmpleado.fcNombre))
                throw new Exception("El nombre del empleado es obligatorio.");

            // ENCRIPTACIÓN DE CONTRASEÑA
            // Si el empleado trae una contraseña, la encriptamos antes de mandarla al DAO
            if (!string.IsNullOrWhiteSpace(nuevoEmpleado.fcPassword))
            {
                // Usando BCrypt (El hash resultante siempre medirá 60 caracteres)
                nuevoEmpleado.fcPassword = BCrypt.Net.BCrypt.HashPassword(nuevoEmpleado.fcPassword);

                // NOTA: Si prefieres no usar NuGet y usar SHA256 nativo, avísame y te paso la clase Helper para eso.
            }

            EmpleadoDAO dao = new EmpleadoDAO();
            return dao.Insertar(nuevoEmpleado);
        }
    
        public List<Empleado> ObtenerTodos()
        {
            // Aquí el BLL actúa como puente. Pide los datos al DAO y se los pasa a la UI.
            return _empleadoDAO.ObtenerTodos();
        }

        public bool EliminarEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
            {
                throw new Exception("El ID del empleado no es válido para la eliminación.");
            }

            EmpleadoDAO dao = new EmpleadoDAO();
            return dao.Eliminar(idEmpleado);
        }

        public List<Empleado> Buscar(string textoBusqueda)
        {
            // 1. Validaciones de negocio preventivas
            // Si por alguna razón llega el texto por defecto o viene vacío, 
            // devolvemos la lista completa para proteger la base de datos de consultas innecesarias.
            if (string.IsNullOrWhiteSpace(textoBusqueda) || textoBusqueda == "🔍  Buscar...")
            {
                return ObtenerTodos();
            }

            // 2. Limpieza de datos (quitamos espacios accidentales al inicio y final)
            string textoLimpio = textoBusqueda.Trim();

            // 3. Pasamos la petición a la capa de Acceso a Datos (DAO)
            EmpleadoDAO dao = new EmpleadoDAO();
            return dao.Buscar(textoLimpio);
        }


        public bool EditarEmpleado(Empleado empleado)
        {
            // Validaciones básicas de negocio
            if (empleado.fiEmpleado <= 0)
                throw new Exception("El ID del empleado no es válido para la edición.");

            if (string.IsNullOrWhiteSpace(empleado.fcNombre))
                throw new Exception("El nombre del empleado es obligatorio.");

            // Limpiamos espacios en blanco accidentales
            empleado.fcNombre = empleado.fcNombre.Trim();

            if (!string.IsNullOrWhiteSpace(empleado.fcEmail))
                empleado.fcEmail = empleado.fcEmail.Trim();

            // Mandamos a la capa de datos
            EmpleadoDAO dao = new EmpleadoDAO();
            return dao.EditarEmpleado(empleado);
        }
        public Empleado ObtenerPorId(int idEmpleado)
        {
            // Validación preventiva de seguridad
            if (idEmpleado <= 0)
                throw new Exception("El ID de empleado proporcionado no es válido.");

            EmpleadoDAO dao = new EmpleadoDAO();
            return dao.ObtenerPorId(idEmpleado);
        }
    }
}