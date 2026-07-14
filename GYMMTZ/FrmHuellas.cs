using System;
using System.Drawing;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;

namespace GYMMTZ
{
    public partial class FrmHuellas : Form, DPFP.Capture.EventHandler
    {
        private Capture Capturer;
        private DPFP.Processing.Enrollment Enroller;
        private int _idCliente;

        // --- Controles Visuales ---
        private Label lblStatus;
        private Label lblIcono;
        private Label[] semaforos = new Label[4];

        public FrmHuellas(int idCliente)
        {
            _idCliente = idCliente;

            ConfigurarUI();

            Enroller = new DPFP.Processing.Enrollment();
            Capturer = new Capture();
            Capturer.EventHandler = this;


            try
            {
                Capturer.StartCapture();
            }
            catch
            {
                lblStatus.Text = "Lector no conectado. Verifique el USB.";
                lblStatus.ForeColor = Color.FromArgb(255, 69, 0); // Naranja error
            }
        }

        private void ConfigurarUI()
        {
            // ===== MAGIA VISUAL: SIN BORDES =====
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(24, 24, 24); // Fondo oscuro tipo login
            this.Size = new Size(320, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            // ====================================

            // Borde sutil opcional (para que no se pierda en fondos oscuros)
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            Label lblTitulo = new Label
            {
                Text = "REGISTRO BIOMÉTRICO",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 30),
                Width = this.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblIcono = new Label
            {
                Text = "👆",
                Font = new Font("Segoe UI", 60f),
                ForeColor = Color.White,
                Location = new Point(0, 80),
                Width = this.Width,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Panel panelSemaforo = new Panel
            {
                Location = new Point((this.Width - 160) / 2 - 8, 200),
                Width = 160,
                Height = 40,
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 4; i++)
            {
                semaforos[i] = new Label
                {
                    Text = "⬤",
                    Font = new Font("Segoe UI", 18f),
                    ForeColor = Color.FromArgb(45, 45, 45),
                    AutoSize = true,
                    Location = new Point(i * 40, 2)
                };
                panelSemaforo.Controls.Add(semaforos[i]);
            }

            lblStatus = new Label
            {
                Text = "Coloque su dedo en el lector 4 veces",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.DarkGray,
                Location = new Point(0, 250),
                Width = this.Width,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ===== BOTÓN ESTILO LOGIN =====
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 69, 0), // Naranja GYM MTZ
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Width = 200,
                Height = 35,
                Location = new Point((this.Width - 200) / 2, 300),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblIcono);
            this.Controls.Add(panelSemaforo);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnCancelar);
        }

        // --- ESTE ES EL MOMENTO DONDE GUARDA ---
        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null)
            {
                Enroller.AddFeatures(features); // Agrega la lectura actual

                this.BeginInvoke((Action)(() => {
                    ActualizarSemaforo((int)Enroller.FeaturesNeeded);

                    // ¿Ya tocó 4 veces y la huella es válida?
                    if (Enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
                    {
                        lblStatus.Text = "¡Huella capturada con éxito!";
                        lblStatus.ForeColor = Color.LimeGreen;

                        byte[] templateData = null;
                        Enroller.Template.Serialize(ref templateData);

                        // AQUÍ SE DISPARA EL GUARDADO A SQL
                        GuardarEnBaseDeDatos(templateData);
                    }
                }));
            }
        }

        private void ActualizarSemaforo(int lecturasFaltantes)
        {
            int toquesDados = 4 - lecturasFaltantes;
            for (int i = 0; i < 4; i++)
            {
                if (i < toquesDados)
                    semaforos[i].ForeColor = Color.LimeGreen;
                else
                    semaforos[i].ForeColor = Color.FromArgb(45, 45, 45);
            }
        }

        private FeatureSet ExtractFeatures(Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();
            CaptureFeedback feedback = CaptureFeedback.None;
            FeatureSet features = new FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            return (feedback == CaptureFeedback.Good) ? features : null;
        }

        private void GuardarEnBaseDeDatos(byte[] templateBytes)
        {
            try
            {
                var bll = new GymApp.BLL.BiometriaBLL();
                if (bll.RegistrarHuella(_idCliente, templateBytes))
                {
                    MessageBox.Show("Huella guardada exitosamente.", "Registro Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close(); // Cierra la ventana sin bordes
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }

        private void FrmHuellas_Load(object sender, EventArgs e) { }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Capturer != null)
            {
                try
                {
                    Capturer.StopCapture();
                    Capturer.EventHandler = null; // 1. Desvincula el hardware del formulario
                    Capturer = null; // 2. Libera el puerto USB por completo
                }
                catch { }
            }
            base.OnFormClosing(e);
        }
    }
}