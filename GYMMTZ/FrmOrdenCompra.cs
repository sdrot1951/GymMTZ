using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GymApp.BLL;
using GymApp.Entities;

namespace GYMMTZ
{
    public partial class FrmOrdenCompra : Form
    {
        private Label lblTitulo, lblTotal;
        private TextBox txtIdProducto, txtCantidad, txtCostoUnitario;
        private Button btnAgregar, btnGuardarOrden, btnCancelar;
        private DataGridView dgvDetalle;

        // Lista dinámica que funciona como el carrito de compras
        private List<CompraDetalle> listaCarrito = new List<CompraDetalle>();
        private decimal sumaTotalOrden = 0;

        public FrmOrdenCompra()
        {
            InitializeComponent();
            ConstruirUI();
            CalcularGranTotal();
        }

        private void ConstruirUI()
        {
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.DarkGray;
            this.Size = new Size(650, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Entrada de Almacén - Órdenes de Compra";

            lblTitulo = new Label { Text = "📝 Generar Orden de Compra (Entrada Stock)", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), AutoSize = true };
            this.Controls.Add(lblTitulo);

            // Campos superiores en fila para agregar artículos rápidamente
            txtIdProducto = CrearCampoSimple("ID Prod.", 20, 60, 80);
            txtCantidad = CrearCampoSimple("Cantidad", 110, 60, 90);
            txtCostoUnitario = CrearCampoSimple("Costo Unit. ($)", 210, 60, 100);

            btnAgregar = new Button { Text = "➕ Añadir", Location = new Point(325, 78), Size = new Size(100, 26), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            // DataGridView del Carrito
            dgvDetalle = new DataGridView { Location = new Point(20, 130), Size = new Size(590, 240), BackgroundColor = Color.FromArgb(30, 30, 30), AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvDetalle.Columns.Add("ID", "ID");
            dgvDetalle.Columns.Add("Cant", "Cantidad");
            dgvDetalle.Columns.Add("Costo", "Costo U.");
            dgvDetalle.Columns.Add("Sub", "Subtotal");
            this.Controls.Add(dgvDetalle);

            // Label de Gran Total
            lblTotal = new Label { Text = "TOTAL ORDEN: $0.00", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(255, 69, 0), Location = new Point(420, 385), AutoSize = true };
            this.Controls.Add(lblTotal);

            // Botones de salida y procesamiento
            btnCancelar = new Button { Text = "Cancelar", Location = new Point(20, 410), Size = new Size(150, 35), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            btnGuardarOrden = new Button { Text = "💾 Procesar Entrada", Location = new Point(440, 410), Size = new Size(170, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardarOrden.Click += BtnGuardarOrden_Click;
            this.Controls.Add(btnGuardarOrden);
        }

        private TextBox CrearCampoSimple(string etiqueta, int x, int y, int ancho)
        {
            Label lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
            this.Controls.Add(lbl);
            TextBox txt = new TextBox { Location = new Point(x, y + 18), Size = new Size(ancho, 23), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txt);
            return txt;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIdProducto.Text) || string.IsNullOrWhiteSpace(txtCantidad.Text) || string.IsNullOrWhiteSpace(txtCostoUnitario.Text)) return;

                var item = new CompraDetalle
                {
                    fiProducto = Convert.ToInt32(txtIdProducto.Text),
                    fiCantidad = Convert.ToInt32(txtCantidad.Text),
                    fmCostoUnitario = Convert.ToDecimal(txtCostoUnitario.Text)
                };

                listaCarrito.Add(item);
                dgvDetalle.Rows.Add(item.fiProducto, item.fiCantidad, item.fmCostoUnitario.ToString("C"), item.fmSubtotal.ToString("C"));

                CalcularGranTotal();

                // Limpieza de inputs rápidos para meter el siguiente
                txtIdProducto.Clear();
                txtCantidad.Clear();
                txtCostoUnitario.Clear();
                txtIdProducto.Focus();
            }
            catch { MessageBox.Show("Verifica que los formatos numéricos ingresados sean correctos.", "Formato Inválido"); }
        }

        private void CalcularGranTotal()
        {
            sumaTotalOrden = 0;
            foreach (var item in listaCarrito) sumaTotalOrden += item.fmSubtotal;
            lblTotal.Text = "TOTAL ORDEN: " + sumaTotalOrden.ToString("C");
        }

        private void BtnGuardarOrden_Click(object sender, EventArgs e)
        {
            if (listaCarrito.Count == 0)
            {
                MessageBox.Show("El detalle de la orden de compra está vacío.", "Aviso");
                return;
            }

            try
            {
                var bll = new ProductoBLL();
                // Mandamos la lista completa de golpe al SP transaccional
                if (bll.RegistrarOrdenCompra(sumaTotalOrden, listaCarrito))
                {
                    MessageBox.Show("¡Orden de compra procesada con éxito!\nLas unidades han sido cargadas al inventario de forma segura.", "Éxito");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Si el SP truena por alguna FK o dato inválido, saltará directo aquí mostrando la reversión completa de la BD
                MessageBox.Show(ex.Message, "Fallo Transaccional Detectado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}