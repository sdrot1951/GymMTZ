using System;
using System.Drawing;
using System.Windows.Forms;
using GymApp.BLL;

namespace GYMMTZ
{
    public partial class FrmAbonos : Form
    {
        private int _idSaldo;
        private decimal _saldoPendiente;

        private ComboBox cmbTipoPago;
        private TextBox txtMonto;

        public FrmAbonos(int idSaldo, string nombreCliente, decimal saldoPendiente)
        {
            _idSaldo = idSaldo;
            _saldoPendiente = saldoPendiente;

            // Configuración visual básica (Dark Theme)
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.Size = new Size(350, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Registrar Abono";

            Label lblTitulo = new Label { Text = $"Cobro a: {nombreCliente}", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            Label lblDeuda = new Label { Text = $"Saldo Restante: {saldoPendiente:C2}", Location = new Point(20, 55), AutoSize = true, ForeColor = Color.FromArgb(255, 69, 0) };

            Label lblMonto = new Label { Text = "Monto a abonar:", Location = new Point(20, 100), AutoSize = true };
            txtMonto = new TextBox { Location = new Point(20, 120), Width = 290, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
            txtMonto.Text = saldoPendiente.ToString("0.00"); // Sugerimos liquidar todo

            Label lblPago = new Label { Text = "Forma de Pago:", Location = new Point(20, 160), AutoSize = true };
            cmbTipoPago = new ComboBox { Location = new Point(20, 180), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White };

            // Cargar tipos de pago (Igual que en tu FrmVenta)
            cmbTipoPago.DataSource = new VentaBLL().ObtenerTiposPago();
            cmbTipoPago.DisplayMember = "Descripcion";
            cmbTipoPago.ValueMember = "ID";

            Button btnGuardar = new Button { Text = "💾 Procesar Abono", Location = new Point(20, 220), Width = 290, Height = 35, BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardar.Click += BtnGuardar_Click;

            this.Controls.AddRange(new Control[] { lblTitulo, lblDeuda, lblMonto, txtMonto, lblPago, cmbTipoPago, btnGuardar });
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMonto.Text, out decimal abono) || abono <= 0)
            {
                MessageBox.Show("Monto inválido."); return;
            }

            try
            {
                int idPago = Convert.ToInt32(cmbTipoPago.SelectedValue);
                int idVendedor = GymApp.Core.SesionGlobal.IdEmpleado; // Usamos el ID global de la RAM

                var bll = new AbonosBLL();
                if (bll.RegistrarAbono(_idSaldo, abono, idPago, idVendedor))
                {
                    MessageBox.Show("Abono registrado con éxito en Caja.", "Éxito");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FrmAbonos_Load(object sender, EventArgs e)
        {

        }
    }
}