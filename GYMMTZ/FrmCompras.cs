using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GymApp.BLL;
using GymApp.Entities;

namespace GYMMTZ
{
    public partial class FrmCompras : Form
    {
        private TextBox txtFolio, txtBuscadorProducto, txtCantidad, txtCosto;
        private ListBox lstResultadosProductos;
        private DataGridView dgvCarrito;
        private Label lblTotal;
        private Button btnAgregar, btnRestar, btnGuardar, btnCancelar;

        private List<CompraDetalle> listaCarrito = new List<CompraDetalle>();
        private int idProductoSeleccionado = 0;
        private decimal sumaTotalCompra = 0;

        public FrmCompras()
        {
            ConfigurarUI();
        }

        private void ConfigurarUI()
        {
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.Size = new Size(720, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Ingreso de Mercancía (Compras)";

            Label title = new Label { Text = "📦 Registro de Compras", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };

            // Folio
            Label lFolio = new Label { Text = "Folio / Factura:", Location = new Point(20, 65), AutoSize = true };
            txtFolio = new TextBox { Location = new Point(20, 85), Size = new Size(180, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Buscador de Producto
            Label lProd = new Label { Text = "Buscar Producto:", Location = new Point(20, 125), AutoSize = true };

            txtBuscadorProducto = new TextBox { Location = new Point(20, 145), Size = new Size(350, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtBuscadorProducto.TextChanged += TxtBuscadorProducto_TextChanged;

      

            lstResultadosProductos = new ListBox { Location = new Point(20, 170), Size = new Size(350, 80), BackColor = Color.FromArgb(30, 30, 35), ForeColor = Color.White, Visible = false, BorderStyle = BorderStyle.FixedSingle };
            lstResultadosProductos.Click += LstResultadosProductos_Click;

            // Costo
            Label lCosto = new Label { Text = "Costo Unit:", Location = new Point(380, 125), AutoSize = true };
            txtCosto = new TextBox { Location = new Point(380, 145), Size = new Size(80, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.Yellow, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Right };

            // Cantidad
            Label lCant = new Label { Text = "Cant:", Location = new Point(470, 125), AutoSize = true };
            txtCantidad = new TextBox { Location = new Point(470, 145), Size = new Size(60, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = "1", TextAlign = HorizontalAlignment.Center };

            // Botones Agregar / Restar
            btnAgregar = new Button { Text = "➕", Location = new Point(540, 144), Size = new Size(40, 27), BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat };
            btnAgregar.Click += BtnAgregar_Click;

            btnRestar = new Button { Text = "➖", Location = new Point(585, 144), Size = new Size(40, 27), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRestar.Click += BtnRestar_Click;

            // Grid
            dgvCarrito = new DataGridView { Location = new Point(20, 200), Size = new Size(660, 240), BackgroundColor = Color.FromArgb(30, 30, 30), AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ForeColor = Color.Black };
            dgvCarrito.Columns.Add("ID", "SKU"); dgvCarrito.Columns.Add("Desc", "Descripción"); dgvCarrito.Columns.Add("Costo", "Costo U."); dgvCarrito.Columns.Add("Cant", "Cant."); dgvCarrito.Columns.Add("Sub", "Subtotal");

            // Total y Guardar
            lblTotal = new Label { Text = "TOTAL: $0.00", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.LimeGreen, Location = new Point(460, 460), AutoSize = true };

            btnCancelar = new Button { Text = "✖ Cancelar", Location = new Point(20, 500), Size = new Size(130, 35), FlatStyle = FlatStyle.Flat };
            btnCancelar.Click += (s, e) => this.Close();

            btnGuardar = new Button { Text = "💾 INGRESAR COMPRA", Location = new Point(490, 500), Size = new Size(190, 35), BackColor = Color.Cyan, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardar.Click += BtnGuardar_Click;

            this.Controls.AddRange(new Control[] { title, lFolio, txtFolio, lProd, txtBuscadorProducto, lstResultadosProductos, lCosto, txtCosto, lCant, txtCantidad, btnAgregar, btnRestar, dgvCarrito, lblTotal, btnCancelar, btnGuardar });
        }

        private void TxtBuscadorProducto_TextChanged(object sender, EventArgs e)
        {
            string texto = txtBuscadorProducto.Text.Trim();
            if (texto.Length < 2) { lstResultadosProductos.Visible = false; return; }

            try
            {
                // Reutilizamos el buscador que ya tienes en Ventas
                var bll = new VentaBLL();
                DataTable dt = bll.ConsultarProductosCompra(texto);

                if (dt != null && dt.Rows.Count > 0)
                {
                    lstResultadosProductos.DataSource = dt;
                    lstResultadosProductos.DisplayMember = "fcDescripcion";
                    lstResultadosProductos.ValueMember = "fiProducto";
                    lstResultadosProductos.Visible = true;
                    lstResultadosProductos.BringToFront();
                }
                else { lstResultadosProductos.Visible = false; }
            }
            catch { }
        }

        private void FrmCompras_Load(object sender, EventArgs e)
        {

        }

        private void LstResultadosProductos_Click(object sender, EventArgs e)
        {
            if (lstResultadosProductos.SelectedValue == null) return;
            DataRowView drv = (DataRowView)lstResultadosProductos.SelectedItem;

            idProductoSeleccionado = Convert.ToInt32(drv["fiProducto"]);
            txtBuscadorProducto.Text = drv["fcDescripcion"].ToString();

            // Sugerimos el último costo registrado
            txtCosto.Text = Convert.ToDecimal(drv["fiCosto"]).ToString("0.00");

            lstResultadosProductos.Visible = false;
            txtCantidad.Focus();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0) return;
            if (!decimal.TryParse(txtCosto.Text, out decimal costo) || costo < 0) { MessageBox.Show("Costo inválido"); return; }
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0) { MessageBox.Show("Cantidad inválida"); return; }

            var itemExistente = listaCarrito.FirstOrDefault(x => x.fiProducto == idProductoSeleccionado);
            if (itemExistente != null)
            {
                itemExistente.fiCantidad += cantidad;
                itemExistente.fmCostoUnitario = costo; // Actualiza el costo al último digitado
            }
            else
            {
                listaCarrito.Add(new CompraDetalle
                {
                    fiProducto = idProductoSeleccionado,
                    fcDescripcion = txtBuscadorProducto.Text,
                    fmCostoUnitario = costo,
                    fiCantidad = cantidad
                });
            }

            RefrescarGrid();
            LimpiarBuscador();
        }

        private void BtnRestar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.SelectedRows.Count == 0) return;
            int index = dgvCarrito.SelectedRows[0].Index;
            var item = listaCarrito[index];

            item.fiCantidad--;
            if (item.fiCantidad <= 0) listaCarrito.RemoveAt(index);
            RefrescarGrid();
        }

        private void RefrescarGrid()
        {
            dgvCarrito.Rows.Clear();
            sumaTotalCompra = 0;
            foreach (var det in listaCarrito)
            {
                dgvCarrito.Rows.Add(det.fiProducto, det.fcDescripcion, det.fmCostoUnitario.ToString("$#,##0.00"), det.fiCantidad, det.fmSubtotal.ToString("$#,##0.00"));
                sumaTotalCompra += det.fmSubtotal;
            }
            lblTotal.Text = "TOTAL: " + sumaTotalCompra.ToString("$#,##0.00");
        }

        private void LimpiarBuscador()
        {
            txtBuscadorProducto.Clear();
            txtCosto.Clear();
            txtCantidad.Text = "1";
            idProductoSeleccionado = 0;
            txtBuscadorProducto.Focus();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (listaCarrito.Count == 0) { MessageBox.Show("Agregue productos a la compra."); return; }
                if (string.IsNullOrWhiteSpace(txtFolio.Text)) { MessageBox.Show("Ingrese el número de nota o factura."); txtFolio.Focus(); return; }

                int idEmpleado = GymApp.Core.SesionGlobal.IdEmpleado;
                var bll = new InventarioBLL();

                if (bll.RegistrarCompra(txtFolio.Text.Trim(), sumaTotalCompra, idEmpleado, listaCarrito))
                {
                    MessageBox.Show("Compra registrada y stock actualizado con éxito.", "Inventario Cargado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al Procesar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

 
    }
}