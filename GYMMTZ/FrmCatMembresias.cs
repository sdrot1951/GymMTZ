using GymApp.BLL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmCatMembresias : Form
    {
        private string _modo; // "Membresia" o "Visita"
        private TextBox txtNombre, txtDias, txtPrecio;

        private int _idEditar;

        public FrmCatMembresias(string modo, int idEditar = 0, string nombre = "", decimal precio = 0, int dias = 0)
        {
            _modo = modo;
            _idEditar = idEditar;
            ConfigurarUI();

            if (_idEditar > 0)
            {
                this.Text = $"Editar {_modo}";
                txtNombre.Text = nombre;
                txtPrecio.Text = precio.ToString("0.00");
                if (_modo == "Membresia") txtDias.Text = dias.ToString();
            }
        }

        private void FrmCatMembresias_Load(object sender, EventArgs e)
        {

        }

        private void ConfigurarUI()
        {
            this.Size = new Size(400, _modo == "Membresia" ? 380 : 300);
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = _modo == "Membresia" ? "Nueva Membresía" : "Nueva Visita/Pase";

            Label lblTitulo = new Label { Text = this.Text, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTitulo);

            // Campos compartidos
            Label lblNom = new Label { Text = "Nombre / Descripción:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.DarkGray };
            txtNombre = new TextBox { Location = new Point(20, 95), Width = 340, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(lblNom); this.Controls.Add(txtNombre);

            Label lblPre = new Label { Text = "Precio de Venta ($):", Location = new Point(20, 140), AutoSize = true, ForeColor = Color.DarkGray };
            txtPrecio = new TextBox { Location = new Point(20, 165), Width = 160, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(lblPre); this.Controls.Add(txtPrecio);

            // Campo exclusivo de Membresía
            int btnY = 220;
            if (_modo == "Membresia")
            {
                Label lblDias = new Label { Text = "Días de Duración:", Location = new Point(200, 140), AutoSize = true, ForeColor = Color.DarkGray };
                txtDias = new TextBox { Location = new Point(200, 165), Width = 160, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
                this.Controls.Add(lblDias); this.Controls.Add(txtDias);
                btnY = 280;
            }

            Button btnGuardar = new Button { Text = "💾 Guardar", Location = new Point(200, btnY), Size = new Size(160, 40), BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal precio = Convert.ToDecimal(txtPrecio.Text);
                var bll = new CatalogosBLL();

                if (_modo == "Membresia")
                {
                    int dias = Convert.ToInt32(txtDias.Text);
                    if (_idEditar > 0) bll.EditarMembresia(_idEditar, txtNombre.Text, dias, precio);
                    else bll.GuardarMembresia(txtNombre.Text, dias, precio);
                }
                else
                {
                    if (_idEditar > 0) bll.EditarVisita(_idEditar, txtNombre.Text, precio);
                    else bll.GuardarVisita(txtNombre.Text, precio);
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }
    }
}