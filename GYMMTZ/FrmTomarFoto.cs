using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace GYMMTZ
{
    public partial class FrmTomarFoto : Form
    {
        private FilterInfoCollection MisDispositivos;
        private VideoCaptureDevice MiWebCam;

        // Propiedad pública donde guardaremos la foto para mandarla a la Base de Datos
        public byte[] FotoCapturadaBytes { get; private set; }

        // Controles UI
        private PictureBox picCamara;
        private ComboBox cmbCamaras;
        private Button btnCapturar, btnCancelar;

        public FrmTomarFoto()
        {
            ConfigurarUI();
            CargarDispositivos();
        }

        private void ConfigurarUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.Size = new Size(500, 560);
            this.StartPosition = FormStartPosition.CenterParent;

            // Borde sutil
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            Label lblTitulo = new Label { Text = "📷 Capturar Fotografía", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };

            cmbCamaras = new ComboBox { Location = new Point(20, 60), Width = 460, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, Font = new Font("Segoe UI", 10) };

            picCamara = new PictureBox { Location = new Point(20, 100), Size = new Size(460, 360), BackColor = Color.FromArgb(10, 10, 10), SizeMode = PictureBoxSizeMode.Zoom };

            btnCancelar = new Button { Text = "❌ Cancelar", Location = new Point(20, 480), Width = 150, Height = 45, BackColor = Color.FromArgb(60, 60, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            btnCapturar = new Button { Text = "📸 TOMAR FOTO", Location = new Point(190, 480), Width = 290, Height = 45, BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnCapturar.Click += BtnCapturar_Click;

            this.Controls.AddRange(new Control[] { lblTitulo, cmbCamaras, picCamara, btnCancelar, btnCapturar });
        }

        private void CargarDispositivos()
        {
            MisDispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (MisDispositivos.Count > 0)
            {
                foreach (FilterInfo dispositivo in MisDispositivos)
                {
                    cmbCamaras.Items.Add(dispositivo.Name);
                }
                cmbCamaras.SelectedIndex = 0; // Selecciona la Logitech por defecto
                EncenderCamara();
            }
            else
            {
                MessageBox.Show("No se detectó ninguna cámara web.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCapturar.Enabled = false;
            }
        }

        private void EncenderCamara()
        {
            if (MiWebCam != null && MiWebCam.IsRunning) return;

            MiWebCam = new VideoCaptureDevice(MisDispositivos[cmbCamaras.SelectedIndex].MonikerString);
            MiWebCam.NewFrame += MiWebCam_NewFrame;
            MiWebCam.Start();
        }

        // Evento que se dispara 30 veces por segundo (cada frame del video)
        private void MiWebCam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap imagen = (Bitmap)eventArgs.Frame.Clone();
            picCamara.Image = imagen;
        }

        private void BtnCapturar_Click(object sender, EventArgs e)
        {
            if (picCamara.Image != null)
            {
                // 1. Pausamos la cámara para que el usuario vea cómo quedó la foto
                if (MiWebCam != null && MiWebCam.IsRunning)
                {
                    MiWebCam.SignalToStop();
                    MiWebCam.WaitForStop();
                }

                // 2. Convertimos la imagen a arreglo de bytes (para SQL Server)
                using (MemoryStream ms = new MemoryStream())
                {
                    picCamara.Image.Save(ms, ImageFormat.Jpeg);
                    FotoCapturadaBytes = ms.ToArray();
                }

                DialogResult respuesta = MessageBox.Show("¿Desea guardar esta fotografía?", "Confirmar Fotografía", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Si no le gustó la foto, volvemos a encender la cámara
                    EncenderCamara();
                }
            }
        }

        // ==========================================
        // SÚPER IMPORTANTE: Apagar cámara al salir
        // ==========================================
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (MiWebCam != null && MiWebCam.IsRunning)
            {
                MiWebCam.SignalToStop();
                MiWebCam.WaitForStop();
                MiWebCam = null;
            }
            base.OnFormClosing(e);
        }
    }
}