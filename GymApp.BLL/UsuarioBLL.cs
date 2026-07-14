using System;
using System.Data;
using GymApp.DAO;
using GymApp.Entities;
// Asegúrate de tener el using de BCrypt si es necesario

namespace GymApp.BLL
{
    public class UsuarioBLL
    {
        public Empleado ValidarLogin(string usernameInput, string passwordInput)
        {
            UsuarioDAO dao = new UsuarioDAO();
            DataTable dtUsuario = dao.ObtenerPorUsername(usernameInput);

            // 1. Validar si el usuario realmente existe en la BD
            if (dtUsuario.Rows.Count == 0)
            {
                return null; // ¡Cambiado! Retornamos null porque el usuario no existe
            }

            // Guardamos la fila en una variable para no escribir "dtUsuario.Rows[0]" tantas veces
            DataRow fila = dtUsuario.Rows[0];

            // 2. Extraer el hash de la base de datos
            string hashAlmacenado = fila["fcPassword"].ToString();

            // 3. Verificar la contraseña usando BCrypt
            bool esValido = BCrypt.Net.BCrypt.Verify(passwordInput, hashAlmacenado);

            // 4. Si la contraseña es correcta, empacamos todos los datos del empleado
            if (esValido)
            {
                Empleado empleadoLogueado = new Empleado
                {
                    fiEmpleado = Convert.ToInt32(fila["fiEmpleado"]),
                    fcNombre = fila["fcNombre"].ToString(),
                    fcApePat = fila["fcApePat"].ToString(),
                    fcApeMat = fila["fcApeMat"] != DBNull.Value ? fila["fcApeMat"].ToString() : "",
                    fiPuesto = Convert.ToInt32(fila["fiPuesto"]),

                    // Asegúrate de que el DAO traiga el nombre del puesto, si no lo trae, puedes dejarlo vacío por ahora
                    fcNombrePuesto = fila.Table.Columns.Contains("NombrePuesto") ? fila["NombrePuesto"].ToString() : "Usuario"
                };

                return empleadoLogueado; // ¡Regresamos al Empleado para que inicie sesión!
            }

            // Si la contraseña fue incorrecta, llegamos hasta aquí y regresamos nulo
            return null;
        }
    }
}