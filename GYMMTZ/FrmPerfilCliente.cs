using GymApp.BLL;
using GymApp.Entities;
using System;
using System.Drawing;
using System.Windows.Forms;
// using GymApp.Entities; (Descomenta si pusiste PerfilClienteDTO en Entities)

namespace GYMMTZ
{
    public partial class FrmPerfilCliente : Form
    {
        private int _idCliente;
        private PerfilClienteDTO _perfil;

        // Controles de UI
        private PictureBox picFoto;
        private Label lblNombre, lblContacto, lblMemEstatus, lblMemDetalle, lblDeudaInfo;
        private Button btnAbonar;
        private TextBox txtObservacionesInfo;

        public FrmPerfilCliente(int idCliente)
        {
            _idCliente = idCliente;
            ConfigurarUI();
            CargarDatos();
        }

        private void ConfigurarUI()
        {
            // 1. Limpiamos y ajustamos el tamaño de la ventana (Un poco más ancha)
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Perfil del Cliente";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(700, 480); // <--- Medida final, única y limpia

            // --- ZONA IZQUIERDA (Foto y Biométricos) ---
            picFoto = new PictureBox { Location = new Point(20, 20), Size = new Size(160, 160), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(30, 30, 35) };
            picFoto.Paint += (s, ev) => {
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, picFoto.Width - 1, picFoto.Height - 1);
                picFoto.Region = new Region(gp);
            };
            this.Controls.Add(picFoto);

            Button btnFoto = new Button { Text = "📷 Actualizar Foto", Location = new Point(20, 190), Size = new Size(160, 35), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnFoto.FlatAppearance.BorderSize = 0;
            btnFoto.Click += BtnFoto_Click;
            this.Controls.Add(btnFoto);

            Button btnHuella = new Button { Text = "👆 Registrar Huella", Location = new Point(20, 235), Size = new Size(160, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnHuella.FlatAppearance.BorderSize = 0;
            btnHuella.Click += BtnHuella_Click;
            this.Controls.Add(btnHuella);

            // --- ZONA DERECHA (Información) ---
            lblNombre = new Label { Location = new Point(200, 20), AutoSize = true, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White };
            lblContacto = new Label { Location = new Point(200, 55), AutoSize = true, Font = new Font("Segoe UI", 10), ForeColor = Color.DarkGray };
            this.Controls.Add(lblNombre);
            this.Controls.Add(lblContacto);

            // Panel de Membresía (Ampliamos el ancho a 440)
            Panel pnlMem = new Panel { Location = new Point(200, 90), Size = new Size(470, 100), BackColor = Color.FromArgb(35, 35, 40) };
            Label lblMemTitulo = new Label { Text = "Membresía Actual:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DarkGray };
            lblMemEstatus = new Label { Location = new Point(10, 30), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
            lblMemDetalle = new Label { Location = new Point(10, 60), AutoSize = true, Font = new Font("Segoe UI", 10), ForeColor = Color.White };
            pnlMem.Controls.AddRange(new Control[] { lblMemTitulo, lblMemEstatus, lblMemDetalle });
            this.Controls.Add(pnlMem);

            // Panel Financiero (Deudas) (Ampliamos el ancho a 440)
            Panel pnlFin = new Panel { Location = new Point(200, 200), Size = new Size(470, 90), BackColor = Color.FromArgb(35, 35, 40) };
            Label lblFinTitulo = new Label { Text = "Estado Financiero:", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DarkGray };
            lblDeudaInfo = new Label { Location = new Point(10, 35), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold) };

            // Empujamos el botón verde más a la derecha (X=280) para que no choque con la etiqueta
            btnAbonar = new Button { Text = "💵 Abonar / Pagar", Location = new Point(310, 25), Size = new Size(150, 40), BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Visible = false };
            btnAbonar.FlatAppearance.BorderSize = 0;
            btnAbonar.Click += BtnAbonar_Click;

            pnlFin.Controls.AddRange(new Control[] { lblFinTitulo, lblDeudaInfo, btnAbonar });
            this.Controls.Add(pnlFin);

            // --- Panel de Observaciones ---
            Label lblObsTitulo = new Label { Text = "📝 Observaciones:", Location = new Point(200, 300), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DarkGray };
            txtObservacionesInfo = new TextBox
            {
                Location = new Point(200, 325),
                Size = new Size(470, 60), // Alineado a los 440px de los otros paneles
                Multiline = true,
                ReadOnly = true, // Evita que se edite desde aquí
                BackColor = Color.FromArgb(30, 30, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };
            this.Controls.Add(lblObsTitulo);
            this.Controls.Add(txtObservacionesInfo);

            // Botón Cerrar (Alineado a la derecha con los paneles nuevos)
            Button btnCerrar = new Button { Text = "✖ Cerrar Perfil", Location = new Point(490, 395), Size = new Size(150, 40), BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }

        private void FrmPerfilCliente_Load(object sender, EventArgs e)
        {

        }

        private void CargarDatos()
        {
            var bll = new ClienteBLL(); // O el BLL donde pusiste el método
            _perfil = bll.ObtenerPerfil(_idCliente);

            if (_perfil != null)
            {
                lblNombre.Text = _perfil.NombreCompleto;
                lblContacto.Text = $"📞 {_perfil.Telefono}   |   ✉️ {_perfil.Email}";

                txtObservacionesInfo.Text = _perfil.fcObservaciones;

                // 1. Cargar Foto
                if (picFoto.Image != null) picFoto.Image.Dispose();
                if (_perfil.Foto != null && _perfil.Foto.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream(_perfil.Foto)) { picFoto.Image = Image.FromStream(ms); }
                }
                else { picFoto.Image = null; } // Puedes poner un logo por defecto aquí

                // 2. Semáforo de Membresía
                lblMemDetalle.Text = $"{_perfil.Membresia} | Vence: {(_perfil.FechaVencimiento.HasValue ? _perfil.FechaVencimiento.Value.ToString("dd/MM/yyyy") : "N/A")}";

                if (_perfil.EstadoMembresia == "Activa")
                {
                    lblMemEstatus.Text = $"✅ ACTIVA ({_perfil.DiasRestantes} días restantes)";
                    lblMemEstatus.ForeColor = Color.LimeGreen;
                    if (_perfil.DiasRestantes <= 5) lblMemEstatus.ForeColor = Color.Yellow;
                }
                else
                {
                    lblMemEstatus.Text = $"❌ {_perfil.EstadoMembresia.ToUpper()}";
                    lblMemEstatus.ForeColor = Color.Crimson;
                }

                // 3. Control de Deudas
                if (_perfil.DeudaTotal > 0)
                {
                    lblDeudaInfo.Text = $"⚠️ Saldo Pendiente: {_perfil.DeudaTotal.ToString("$#,##0.00")}";
                    lblDeudaInfo.ForeColor = Color.Crimson;
                    btnAbonar.Visible = true;
                }
                else
                {
                    lblDeudaInfo.Text = "✅ Al corriente (Sin adeudos)";
                    lblDeudaInfo.ForeColor = Color.LimeGreen;
                    btnAbonar.Visible = false;
                }



            }
        }

        private void BtnFoto_Click(object sender, EventArgs e)
        {
            FrmTomarFoto frmFoto = new FrmTomarFoto();
            if (frmFoto.ShowDialog() == DialogResult.OK)
            {
                var bll = new ClienteBLL();
                if (bll.GuardarFoto(_idCliente, frmFoto.FotoCapturadaBytes)) CargarDatos(); // Recarga la UI
            }
        }

        private void BtnHuella_Click(object sender, EventArgs e)
        {
            FrmHuellas frmHuella = new FrmHuellas(_idCliente);
            frmHuella.ShowDialog(); // El FrmHuellas ya maneja sus propios avisos
        }

        private void BtnAbonar_Click(object sender, EventArgs e)
        {
            if (_perfil.IdDeudaPendiente.HasValue)
            {
                FrmAbonos frm = new FrmAbonos(_perfil.IdDeudaPendiente.Value, _perfil.NombreCompleto, _perfil.DeudaTotal);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarDatos(); // Si el cliente paga, la UI se recarga y el botón verde desaparece
                }
            }
        }
    }
}