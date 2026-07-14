using System;
using System.Drawing;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;
using GymApp.BLL; // Tu capa de negocio

namespace GYMMTZ
{
    public partial class FrmChecador : Form, DPFP.Capture.EventHandler
    {
        private Capture Capturer;
        private AccesoBLL _accesoBLL;
        private PictureBox picFotoCliente;

        // Controles de la UI
        private Label lblReloj;
        private Label lblFecha;
        private Label lblNombre;
        private Label lblMensaje;
        private Panel pnlEstado;
        private Label lblIcono;

        // Timers
        private Timer tmrReloj;
        private Timer tmrLimpiar;

        private TextBox txtBuscadorManual;
        private ListBox lstResultadosBusqueda;

        public FrmChecador()
        {
            // 1. Cargamos el motor de base de datos a la RAM
            _accesoBLL = new AccesoBLL();

            // 2. Construimos la Interfaz Gráfica al vuelo
            ConfigurarUI();

            // 3. Encendemos el lector
            IniciarLector();
        }

        private void ConfigurarUI()
        {
            // Ventana en pantalla completa, sin bordes y fondo oscuro
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(18, 18, 20); // El color oscuro de tu menú principal

            // Botón discreto para cerrar el checador (solo administradores)
            Button btnCerrar = new Button
            {
                Text = "✖ Salir del Checador",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.DarkGray,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10f),
                Size = new Size(160, 40),
                Location = new Point(Screen.PrimaryScreen.Bounds.Width - 180, 20),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);

            // Reloj Digital Gigante
            lblReloj = new Label
            {
                Text = "00:00",
                Font = new Font("Segoe UI", 80f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, 120),
                Location = new Point(0, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblReloj);

            // Fecha debajo del reloj
            lblFecha = new Label
            {
                Text = "Cargando fecha...",
                Font = new Font("Segoe UI", 20f),
                ForeColor = Color.FromArgb(255, 69, 0), // Naranja Gym MTZ
                AutoSize = false,
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, 40),
                Location = new Point(0, 200),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblFecha);

            // ==========================================
            // PANEL DE ESTADO (Ajustado para foto grande)
            // ==========================================
            pnlEstado = new Panel
            {
                Size = new Size(800, 450), // Ampliamos la altura de 350 a 450
                Location = new Point((Screen.PrimaryScreen.Bounds.Width - 800) / 2, 280), // Lo subimos ligeramente
                BackColor = Color.FromArgb(30, 30, 35) // Gris un poco más claro
            };

            lblIcono = new Label
            {
                Text = "👆",
                Font = new Font("Segoe UI", 70f),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(800, 100),
                Location = new Point(0, 100), // Bajamos el icono a la altura del rostro
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlEstado.Controls.Add(lblIcono);

            picFotoCliente = new PictureBox
            {
                Size = new Size(240, 240), // Tamaño Premium
                Location = new Point((pnlEstado.Width - 240) / 2, 40), // Centrado perfecto
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                Visible = false
            };

            // Corte circular
            picFotoCliente.Paint += (s, ev) => {
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, picFotoCliente.Width - 1, picFotoCliente.Height - 1);
                picFotoCliente.Region = new Region(gp);
            };
            pnlEstado.Controls.Add(picFotoCliente);

            lblNombre = new Label
            {
                Text = "Esperando Huella...",
                Font = new Font("Segoe UI", 28f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false, // Magia para el centrado
                Size = new Size(800, 50),
                Location = new Point(0, 300), // Empujado debajo de la foto (40 + 240 + 20)
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlEstado.Controls.Add(lblNombre);

            lblMensaje = new Label
            {
                Text = "Coloque su dedo sobre el lector biométrico",
                Font = new Font("Segoe UI", 18f),
                ForeColor = Color.DarkGray,
                AutoSize = false,
                Size = new Size(800, 60),
                Location = new Point(0, 360), // Empujado debajo del nombre
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };
            pnlEstado.Controls.Add(lblMensaje);

            int buscadorWidth = 600; // Lo hacemos el doble de ancho
            int buscadorX = (Screen.PrimaryScreen.Bounds.Width - buscadorWidth) / 2; // Centrado perfecto
            int buscadorYBase = pnlEstado.Location.Y + pnlEstado.Height + 40; // 40 píxeles debajo del panel de la huella

            Label lblBuscar = new Label
            {
                Text = "🔍 Búsqueda de Miembro (Ingreso Manual):",
                Location = new Point(buscadorX, buscadorYBase),
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold), // Letra más grande y en negrita
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblBuscar);

            txtBuscadorManual = new TextBox
            {
                Location = new Point(buscadorX, buscadorYBase + 30),
                Width = buscadorWidth,
                Font = new Font("Segoe UI", 18f), // Letra enorme para fácil lectura
                BackColor = Color.FromArgb(35, 35, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBuscadorManual.TextChanged += TxtBuscadorManual_TextChanged;
            this.Controls.Add(txtBuscadorManual);

            // Ajustamos la lista desplegable al nuevo ancho y posición
            lstResultadosBusqueda = new ListBox
            {
                // Se coloca justo debajo del TextBox, calculando su altura dinámica
                Location = new Point(buscadorX, txtBuscadorManual.Location.Y + txtBuscadorManual.Height + 2),
                Width = buscadorWidth,
                Height = 220,
                Font = new Font("Segoe UI", 14f),
                BackColor = Color.FromArgb(30, 30, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                DisplayMember = "NombreCliente",
                Cursor = Cursors.Hand
            };
            lstResultadosBusqueda.Click += LstResultadosBusqueda_Click;
            this.Controls.Add(lstResultadosBusqueda);

            // Aseguramos que los controles no queden ocultos detrás de nada
            lblBuscar.BringToFront();
            txtBuscadorManual.BringToFront();
            lstResultadosBusqueda.BringToFront();



            this.Controls.Add(pnlEstado);

            // ==========================================
            // TIMERS
            // ==========================================
            tmrReloj = new Timer { Interval = 1000 };
            tmrReloj.Tick += (s, e) => {
                lblReloj.Text = DateTime.Now.ToString("HH:mm:ss");
                lblFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy").ToUpper();
            };
            tmrReloj.Start();

            // Configurar Timer para Limpiar Pantalla (se activa después de leer una huella)
            tmrLimpiar = new Timer { Interval = 4000 }; // 4 segundos de mensaje
            tmrLimpiar.Tick += (s, e) => ResetearPantalla();
        }

        private void IniciarLector()
        {
            try
            {
                Capturer = new Capture();
                Capturer.EventHandler = this;
                Capturer.StartCapture();
            }
            catch
            {
                lblNombre.Text = "ERROR DE HARDWARE";
                lblMensaje.Text = "Lector no conectado. Verifique el puerto USB.";
                pnlEstado.BackColor = Color.DarkRed;
            }
        }

        // --- EVENTO: CUANDO ALGUIEN PONE EL DEDO ---
        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            // Detenemos la limpieza automática si alguien pone el dedo muy rápido
            this.BeginInvoke((Action)(() => tmrLimpiar.Stop()));

            // 1. Extraemos características en MODO VERIFICACIÓN
            FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            if (features != null)
            {
                // 2. Mandamos la huella a la memoria RAM para buscar al cliente
                var resultado = _accesoBLL.EvaluarAcceso(features);

                // 3. Actualizamos la pantalla con el resultado
                this.BeginInvoke((Action)(() => ProcesarResultadoVisual(resultado)));
            }
        }

        private void ProcesarResultadoVisual(ResultadoAcceso resultado)
        {

            if (picFotoCliente.Image != null)
            {
                picFotoCliente.Image.Dispose();
                picFotoCliente.Image = null;
            }

            if (resultado.FotoBytes != null && resultado.FotoBytes.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(resultado.FotoBytes))
                {
                    picFotoCliente.Image = Image.FromStream(ms);
                }
                picFotoCliente.Visible = true;
                lblIcono.Visible = false;
            }
            else
            {
                picFotoCliente.Visible = false;
                lblIcono.Visible = true;
            }

            if (resultado.Encontrado)
            {
                lblNombre.Text = resultado.NombreCliente.ToUpper();
                lblMensaje.Text = resultado.Mensaje;

                switch (resultado.Estatus)
                {
                    case EstatusAcceso.PermitidoOK:
                        pnlEstado.BackColor = Color.LimeGreen;
                        lblNombre.ForeColor = Color.Black;
                        lblMensaje.ForeColor = Color.Black;
                        lblIcono.Text = "✅";
                        break;

                    case EstatusAcceso.PermitidoPorVencer:
                        pnlEstado.BackColor = Color.Gold;
                        lblNombre.ForeColor = Color.Black;
                        lblMensaje.ForeColor = Color.Black;
                        lblIcono.Text = "⚠️";
                        break;

                    case EstatusAcceso.DenegadoMembresiaVencida:
                        pnlEstado.BackColor = Color.Crimson;
                        lblNombre.ForeColor = Color.White;
                        lblMensaje.ForeColor = Color.White;
                        lblIcono.Text = "❌";
                        break;
                }
            }
            else
            {
                // Cliente no está en la base de datos
                pnlEstado.BackColor = Color.FromArgb(45, 45, 50);
                lblIcono.Text = "❓";
                lblNombre.Text = "HUELLA NO RECONOCIDA";
                lblNombre.ForeColor = Color.White;
                lblMensaje.Text = "Por favor, acuda a recepción para registrarse.";
                lblMensaje.ForeColor = Color.DarkGray;
            }

            // Iniciamos la cuenta regresiva para borrar el nombre
            tmrLimpiar.Start();
        }

        private void ResetearPantalla()
        {
            tmrLimpiar.Stop();
            pnlEstado.BackColor = Color.FromArgb(30, 30, 35);
            lblIcono.Text = "👆";
            lblIcono.Visible = true;       // <-- Vuelve el dedito
            picFotoCliente.Visible = false; // <-- Se oculta la foto
            lblNombre.Text = "Esperando Huella...";
            lblNombre.ForeColor = Color.White;
            lblMensaje.Text = "Coloque su dedo sobre el lector biométrico";
            lblMensaje.ForeColor = Color.DarkGray;
        }

        private FeatureSet ExtractFeatures(Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();
            CaptureFeedback feedback = CaptureFeedback.None;
            FeatureSet features = new FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            return (feedback == CaptureFeedback.Good) ? features : null;
        }

        // Interfaces obligatorias del SDK
        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }

        // Apagar lector y reloj al cerrar para limpiar RAM

        private void FrmChecador_Load(object sender, EventArgs e)
        {

        }

        private void TxtBuscadorManual_TextChanged(object sender, EventArgs e)
        {
            string texto = txtBuscadorManual.Text.Trim();
            if (texto.Length >= 3) // Evitar búsquedas con 1 sola letra
            {
                var bll = new GymApp.BLL.AccesoBLL();
                var resultados = bll.BuscarClientesManual(texto);

                if (resultados.Count > 0)
                {
                    lstResultadosBusqueda.DataSource = resultados;
                    lstResultadosBusqueda.Visible = true;
                }
                else
                {
                    lstResultadosBusqueda.Visible = false;
                }
            }
            else
            {
                lstResultadosBusqueda.Visible = false;
            }
        }

        private void LstResultadosBusqueda_Click(object sender, EventArgs e)
        {
            if (lstResultadosBusqueda.SelectedItem != null)
            {
                // 1. Extraemos al cliente seleccionado
                var clienteSeleccionado = (GymApp.BLL.ResultadoAcceso)lstResultadosBusqueda.SelectedItem;

                // 2. Lo pasamos por el semáforo y registramos asistencia
                var bll = new GymApp.BLL.AccesoBLL();
                var resultadoFinal = bll.EvaluarAccesoManual(clienteSeleccionado);

                // 3. ¡Magia! Se lo mandamos a tu función visual que ya hace el cambio de panel y foto gigante
                ProcesarResultadoVisual(resultadoFinal);

                // 4. Limpiamos y ocultamos el buscador
                txtBuscadorManual.Text = "";
                lstResultadosBusqueda.Visible = false;

                // Volvemos a enfocar el foco principal al formulario
                this.Focus();
            }
        }

        // ===== NUEVO MÉTODO PARA DESTRUIR EL LECTOR =====
        public void ApagarLector()
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                    Capturer.EventHandler = null; // Desconecta los eventos
                    Capturer = null; // Libera el USB
                }
                catch { }
            }
            if (tmrReloj != null) tmrReloj.Stop();
            if (tmrLimpiar != null) tmrLimpiar.Stop();
        }

        // Apagar lector y reloj al cerrar para limpiar RAM
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ApagarLector(); // Llamamos al método de arriba
            base.OnFormClosing(e);
        }
    }
}