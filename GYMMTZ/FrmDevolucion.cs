using GymApp.BLL;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmDevolucion : Form
    {
        private TextBox txtFolioVenta, txtMonto, txtMotivo, txtDetalle;
        private Button btnBuscar;

        public FrmDevolucion()
        {
            InitializeComponent();
            ConfigurarUI();
        }

        private void FrmDevolucion_Load(object sender, EventArgs e)
        {

        }

        private void ConfigurarUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            // 1. Hicimos la ventana más alta para que quepa el detalle (de 420 a 530)
            this.Size = new Size(380, 530);
            this.StartPosition = FormStartPosition.CenterParent;

            this.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(255, 69, 0), ButtonBorderStyle.Solid); };

            Label lblTitulo = new Label { Text = "⚠️ Cancelar Venta", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(255, 69, 0), Location = new Point(20, 20), AutoSize = true };
            Label lblAviso = new Label { Text = "Solo Administradores y Gerentes pueden\nautorizar esta operación de efectivo.", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 9), Location = new Point(23, 55), AutoSize = true };

            // Búsqueda del Ticket
            Label lblFolio = new Label { Text = "Folio del Ticket a cancelar:", Location = new Point(20, 100), AutoSize = true };
            txtFolioVenta = new TextBox { Location = new Point(20, 120), Width = 230, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, Font = new Font("Segoe UI", 12) };

            // ====== NUEVO BOTÓN DE BÚSQUEDA ======
            btnBuscar = new Button { Text = "🔍 Buscar", Location = new Point(260, 119), Width = 100, Height = 30, BackColor = Color.FromArgb(45, 45, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            // ====== NUEVO TEXTBOX DE DETALLES ======
            Label lblDetalle = new Label { Text = "Artículos en el ticket:", Location = new Point(20, 160), AutoSize = true };
            txtDetalle = new TextBox { Location = new Point(20, 180), Width = 340, Height = 70, Multiline = true, ReadOnly = true, BackColor = Color.FromArgb(15, 15, 15), ForeColor = Color.LightGray, ScrollBars = ScrollBars.Vertical };

            // Monto (Ahora desplazado hacia abajo y solo lectura)
            Label lblMonto = new Label { Text = "Monto a devolver a cliente ($):", Location = new Point(20, 260), AutoSize = true };
            txtMonto = new TextBox { Location = new Point(20, 280), Width = 340, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, Font = new Font("Segoe UI", 12, FontStyle.Bold), ReadOnly = true }; // <- PROTEGIDO

            // Motivo
            Label lblMotivo = new Label { Text = "Motivo de la cancelación (Auditoría):", Location = new Point(20, 320), AutoSize = true };
            txtMotivo = new TextBox { Location = new Point(20, 340), Width = 340, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White };

            // Botones Finales
            Button btnGuardar = new Button { Text = "⚠️ AUTORIZAR Y CANCELAR", Location = new Point(20, 420), Width = 340, Height = 40, BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            Button btnCancelar = new Button { Text = "Cerrar", Location = new Point(20, 470), Width = 340, Height = 35, BackColor = Color.FromArgb(45, 45, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, lblAviso, lblFolio, txtFolioVenta, btnBuscar, lblDetalle, txtDetalle, lblMonto, txtMonto, lblMotivo, txtMotivo, btnGuardar, btnCancelar });
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtFolioVenta.Text, out int folio))
                {
                    var bll = new DevolucionesBLL();
                    DataTable dt = bll.ConsultarTicket(folio);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string estado = dt.Rows[0]["Estado"].ToString();

                        // Validamos si el ticket ya había sido cancelado antes
                        if (estado.ToUpper() == "CANCELADA")
                        {
                            MessageBox.Show("Este ticket YA SE ENCUENTRA CANCELADO.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtMonto.Text = "0.00";
                            txtDetalle.Text = "ESTADO: CANCELADA";
                            txtMotivo.Enabled = false;
                            return;
                        }

                        // Llenamos el monto (El cajero no lo podrá modificar)
                        decimal total = Convert.ToDecimal(dt.Rows[0]["TotalVenta"]);
                        txtMonto.Text = total.ToString("0.00");

                        // Armamos la lista de los artículos para mostrarlos en pantalla
                        string descripcionArticulos = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            descripcionArticulos += $"- {row["Cantidad"]}x {row["Producto"]}\r\n";
                        }

                        txtDetalle.Text = descripcionArticulos;
                        txtMotivo.Enabled = true; // Rehabilitamos por si estaba bloqueado
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún ticket con ese folio.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtMonto.Text = "0.00";
                        txtDetalle.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Por favor ingresa un folio numérico válido.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el ticket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int folio = string.IsNullOrWhiteSpace(txtFolioVenta.Text) ? 0 : Convert.ToInt32(txtFolioVenta.Text);
                decimal monto = string.IsNullOrWhiteSpace(txtMonto.Text) ? 0 : Convert.ToDecimal(txtMonto.Text.Replace("$", ""));

                if (monto <= 0)
                {
                    MessageBox.Show("Busque un ticket válido antes de confirmar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"¿Estás seguro de cancelar el ticket #{folio} y registrar una salida en caja de {monto:C2}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var bll = new DevolucionesBLL();
                    bll.RegistrarDevolucion(folio, monto, txtMotivo.Text);

                    MessageBox.Show("Venta cancelada y efectivo descontado de la caja correctamente.", "Operación Autorizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}