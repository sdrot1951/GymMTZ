using GymApp.DAO;
using GymApp.Entities;
using System;
using System.Collections.Generic;

namespace GymApp.BLL
{
    public class ClienteBLL
    {


        public PerfilClienteDTO ObtenerPerfil(int idCliente)
        {
            var dao = new DAO.ClienteDAO(); // Ajusta a tu namespace
            return dao.ObtenerPerfilCliente(idCliente);
        }
        public int RegistrarNuevo(Cliente cli)
        {
            if (string.IsNullOrWhiteSpace(cli.fcNombre)) throw new Exception("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(cli.fcApePat)) throw new Exception("El apellido paterno es obligatorio.");

            if (!string.IsNullOrWhiteSpace(cli.fcObservaciones) && cli.fcObservaciones.Length > 500)
            {
                throw new Exception("Las observaciones no pueden exceder los 500 caracteres.");
            }

            var dao = new DAO.ClienteDAO();
            return dao.Insertar(cli); // Devuelve el nuevo ID
        }

        public bool GuardarFoto(int idCliente, byte[] fotoBytes)
        {
            // Validaciones de seguridad
            if (idCliente <= 0)
                throw new Exception("Error: El ID del cliente no es válido para guardar la fotografía.");

            if (fotoBytes == null || fotoBytes.Length == 0)
                throw new Exception("Error: La fotografía está vacía o el archivo está dañado.");

            var dao = new DAO.ClienteDAO(); // Ajusta el namespace si tu estructura de carpetas es diferente
            return dao.GuardarFoto(idCliente, fotoBytes);
        }
        public List<Cliente> ObtenerTodos()
        {
            var dao = new ClienteDAO();
            return dao.ObtenerTodos();
        }

        public Cliente ObtenerPorId(int id)
        {
            var dao = new ClienteDAO();
            return dao.ObtenerPorId(id);
        }

        public bool Editar(Cliente cli)
        {
            var dao = new ClienteDAO();
            return dao.Editar(cli);
        }


        public bool Eliminar(int id)
        {
            var dao = new ClienteDAO();
            return dao.Eliminar(id);
        }

        public List<Cliente> Buscar(string texto)
        {
            var dao = new ClienteDAO();
            return dao.Buscar(texto);
        }

        public List<Cliente> ObtenerClientesSinHuella()
        {
            var dao = new GymApp.DAO.ClienteDAO();
            return dao.ObtenerClientesSinHuella();
        }
    }
}