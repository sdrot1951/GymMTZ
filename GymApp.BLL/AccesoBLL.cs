using System;
using System.Collections.Generic;
using GymApp.Core;
using GymApp.DAO;
using DPFP;

namespace GymApp.BLL
{
    public class AccesoBLL
    {
        // Esta es la lista que vivirá en la RAM de la pantalla de entrada
        private List<RegistroBiometricoCache> _catalogoMemoria;
        private DPFP.Verification.Verification _verificador;

        public AccesoBLL()
        {
            _verificador = new DPFP.Verification.Verification();
            RecargarCatalogo();
        }

        public System.Data.DataTable ConsultarAsistencias(string busqueda, DateTime desde, DateTime hasta)
        {
        var dao = new AccesoDAO();
        return dao.ConsultarAsistencias(busqueda, desde, hasta);
        }

        // Se ejecuta al iniciar la app o cuando se registra un cliente nuevo para actualizar la RAM
        public void RecargarCatalogo()
        {
            var dao = new AccesoDAO();
            _catalogoMemoria = dao.CargarCatalogoBiometrico();
        }

        // Método clave: Identifica quién puso el dedo y evalúa su estatus financiero
        public ResultadoAcceso EvaluarAcceso(FeatureSet caracteristicasVerificacion)
        {
            var resultado = new ResultadoAcceso { Encontrado = false };
            var resultSDK = new DPFP.Verification.Verification.Result();

            // Recorremos la RAM buscando coincidencia exacta
            foreach (var cliente in _catalogoMemoria)
            {
                // Convertimos el byte[] almacenado de nuevo a un objeto Template del SDK
                Template templateData = new Template();
                templateData.DeSerialize(cliente.TemplateHuella);

                // El SDK compara las características del dedo contra el template de la RAM
                _verificador.Verify(caracteristicasVerificacion, templateData, ref resultSDK);

                if (resultSDK.Verified)
                {
                    resultado.Encontrado = true;
                    resultado.IdCliente = cliente.IdCliente;
                    resultado.NombreCliente = cliente.NombreCliente;
                    resultado.DiasRestantes = cliente.DiasRestantes;
                    resultado.FotoBytes = cliente.FotoBytes;

                    // --- LÓGICA DEL SEMÁFORO DE ACCESO ---
                    if (cliente.EstadoMembresia == "Vencida" || cliente.DiasRestantes < 0)
                    {
                        resultado.Estatus = EstatusAcceso.DenegadoMembresiaVencida;
                        resultado.Mensaje = "ACCESO DENEGADO\nMembresía Vencida";
                    }
                    else if (cliente.DiasRestantes <= 5)
                    {
                        resultado.Estatus = EstatusAcceso.PermitidoPorVencer;
                        resultado.Mensaje = $"ACCESO PERMITIDO\n¡Cuidado! Vence en {cliente.DiasRestantes} días";
                    }
                    else
                    {
                        resultado.Estatus = EstatusAcceso.PermitidoOK;
                        resultado.Mensaje = "ACCESO PERMITIDO\nCliente Activo";
                    }

                    // Guardar registro histórico en la Base de datos
                    var dao = new AccesoDAO();
                    dao.RegistrarAsistencia(cliente.IdCliente, resultado.Estatus.ToString());

                    return resultado; // Rompemos el bucle en la primera coincidencia exitosa
                }
            }

            return resultado; // Si terminó el bucle y no encontró coincidencia
        }

        public List<ResultadoAcceso> BuscarClientesManual(string busqueda)
        {
            var dao = new AccesoDAO();
            System.Data.DataTable dt = dao.BuscarAccesoManual(busqueda);
            var lista = new List<ResultadoAcceso>();

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                lista.Add(new ResultadoAcceso
                {
                    IdCliente = Convert.ToInt32(dr["fiCliente"]),
                    NombreCliente = dr["fcClienteNombre"].ToString(),
                    EstadoMembresia = dr["fcEstadoMembresia"].ToString(),
                    DiasRestantes = Convert.ToInt32(dr["fiDiasRestantes"]),
                    FotoBytes = dr["fbFoto"] != DBNull.Value ? (byte[])dr["fbFoto"] : null,
                    Encontrado = true // Si salió en la lista, existe
                });
            }

            return lista;
        }

        // 2. Método para procesar al cliente seleccionado en la lista
        public ResultadoAcceso EvaluarAccesoManual(ResultadoAcceso cliente)
        {
            // --- LÓGICA DEL SEMÁFORO DE ACCESO (Igual que la huella) ---
            if (cliente.EstadoMembresia == "Vencida" || cliente.EstadoMembresia == "Sin Membresía" || cliente.DiasRestantes < 0)
            {
                cliente.Estatus = EstatusAcceso.DenegadoMembresiaVencida;
                cliente.Mensaje = "ACCESO DENEGADO\nMembresía Vencida o Inexistente";
            }
            else if (cliente.DiasRestantes <= 5)
            {
                cliente.Estatus = EstatusAcceso.PermitidoPorVencer;
                cliente.Mensaje = $"ACCESO PERMITIDO\n¡Cuidado! Vence en {cliente.DiasRestantes} días";
            }
            else
            {
                cliente.Estatus = EstatusAcceso.PermitidoOK;
                cliente.Mensaje = "ACCESO PERMITIDO\nCliente Activo";
            }

            // Guardar registro histórico en la Base de datos
            var dao = new AccesoDAO();
            dao.RegistrarAsistencia(cliente.IdCliente, cliente.Estatus.ToString());

            return cliente;
        }



    }

    // Clases auxiliares para empaquetar la respuesta a la interfaz de usuario
    public class ResultadoAcceso
    {
        public bool Encontrado { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Mensaje { get; set; }
        public int DiasRestantes { get; set; }
        public EstatusAcceso Estatus { get; set; }
        public byte[] FotoBytes { get; set; }

        public string EstadoMembresia { get; set; }
    }

    public enum EstatusAcceso
    {
        PermitidoOK,
        PermitidoPorVencer,
        DenegadoMembresiaVencida
    }
}