using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


        namespace GymApp.BLL
    {
        public class BiometriaBLL
        {
            public bool RegistrarHuella(int idCliente, byte[] template)
            {
                if (idCliente <= 0) throw new Exception("Cliente no válido.");
                if (template == null || template.Length == 0) throw new Exception("No se detectó huella válida.");

                var dao = new DAO.BiometriaDAO();
                return dao.GuardarHuella(idCliente, template);
            }
        }
    }


