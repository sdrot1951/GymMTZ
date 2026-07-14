using System;
using System.Drawing;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmLoginBiometrico : Form
    {
        // Variable para recordar a qué empleado le vamos a asignar la huella
        private int _idEmpleado;

        // Controles de la UI
        private PictureBox picHuella;
        private Label lblInstrucciones;
        private Label lblEstado;
        private Button btnGuardar;
        private Button btnOmitir;

        // Constructor que recibe el ID del empleado recién guardado
        public FrmLoginBiometrico(int idEmpleado = 0)
        {
            _idEmpleado = idEmpleado;
            ConstruirUI();
        }

        private void ConstruirUI()
        {
            // 1. Configuración base de la ventana (Tema Oscuro)
            this.Text = "Captura Biométrica";
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.ForeColor = Color.White;
            this.Size = new Size(380, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 2. Título superior
            Label lblTitulo = new Label()
            {
                Text = "Registro de Huella",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(90, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // 3. Recuadro para mostrar la imagen de la huella en tiempo real
            picHuella = new PictureBox()
            {
                Location = new Point(90, 70),
                Size = new Size(180, 220),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 40),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(picHuella);

            // 4. Texto de Instrucciones
            lblInstrucciones = new Label()
            {
                Text = "Coloque su dedo sobre el lector para escanear y registrar la huella biométrica.",
                Location = new Point(20, 310),
                Size = new Size(320, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblInstrucciones);

            // 5. Estado del lector (Ej: "Conectando...", "Toque 1 de 4", etc.)
            lblEstado = new Label()
            {
                Text = "Esperando lector...",
                Location = new Point(20, 355),
                Size = new Size(320, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Orange,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            this.Controls.Add(lblEstado);

            // 6. Botón de Omitir (Camino Alterno)
            btnOmitir = new Button()
            {
                Text = "✖ Omitir",
                Location = new Point(30, 400),
                Size = new Size(140, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(50, 50, 50),
                Cursor = Cursors.Hand
            };
            btnOmitir.FlatAppearance.BorderSize = 0;
            // Si le da omitir, devolvemos Cancel y cerramos
            btnOmitir.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnOmitir);

            // 7. Botón de Guardar (Camino Feliz)
            btnGuardar = new Button()
            {
                Text = "💾 Guardar",
                Location = new Point(190, 400),
                Size = new Size(140, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 69, 0), // Tono anaranjado/rojo de tu tema
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false // Empieza apagado hasta que el lector capture bien la huella
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Aquí irá el código para mandar el arreglo de bytes (byte[]) de la huella a tu EmpleadoBLL
            // ...

            // Simulamos éxito y cerramos la ventana
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}