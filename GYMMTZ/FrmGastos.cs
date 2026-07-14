using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GymApp.BLL;

namespace GYMMTZ
{
    public partial class FrmGastos : Form
    {
        private ComboBox cmbCategoria;
        private TextBox txtDescripcion;
        private TextBox txtMonto;
        private Button btnGuardar, btnCancelar;

        public FrmGastos()
        {
            InitializeComponent();
            ConstruirUI();
            CargarCategorias();
        }

        private void ConstruirUI()
        {
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.Size = new Size(400, 360);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Registrar Nuevo Gasto";

            Label lblTitulo = new Label { Text = "💸 Registrar Salida de Efectivo", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(255, 69, 0), Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTitulo);

            // Categoría
            Label lblCat = new Label { Text = "Categoría del Gasto:", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.DarkGray };
            cmbCategoria = new ComboBox { Location = new Point(20, 90), Size = new Size(340, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            this.Controls.Add(lblCat); this.Controls.Add(cmbCategoria);

            // Descripción
            Label lblDesc = new Label { Text = "Concepto / Descripción:", Location = new Point(20, 130), AutoSize = true, ForeColor = Color.DarkGray };
            txtDescripcion = new TextBox { Location = new Point(20, 150), Size = new Size(340, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(lblDesc); this.Controls.Add(txtDescripcion);

            // Monto
            Label lblMonto = new Label { Text = "Monto Total ($):", Location = new Point(20, 190), AutoSize = true, ForeColor = Color.DarkGray };
            txtMonto = new TextBox { Location = new Point(20, 210), Size = new Size(150, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Validar que solo escriban números y puntos
            txtMonto.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.') e.Handled = true;
                if (e.KeyChar == '.' && (s as TextBox).Text.IndexOf('.') > -1) e.Handled = true;
            };
            this.Controls.Add(lblMonto); this.Controls.Add(txtMonto);

            // Botones
            btnCancelar = new Button { Text = "✖ Cancelar", Location = new Point(20, 270), Size = new Size(120, 35), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            btnGuardar = new Button { Text = "💾 REGISTRAR GASTO", Location = new Point(170, 270), Size = new Size(190, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void CargarCategorias()
        {
            try
            {
                var bll = new GastosBLL();
                cmbCategoria.DataSource = bll.ObtenerCategorias();
                cmbCategoria.DisplayMember = "Descripcion";
                cmbCategoria.ValueMember = "ID";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void FrmGastos_Load(object sender, EventArgs e)
        {

        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal.TryParse(txtMonto.Text, out decimal monto);
                int idCat = Convert.ToInt32(cmbCategoria.SelectedValue);
                int idEmpleado = GymApp.Core.SesionGlobal.IdEmpleado; // Tomamos el ID de la sesión actual

                var bll = new GastosBLL();
                if (bll.RegistrarGasto(txtDescripcion.Text, monto, idCat, idEmpleado))
                {
                    MessageBox.Show("Gasto registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // Retorna OK para que el menú sepa que debe recargar la tabla
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}