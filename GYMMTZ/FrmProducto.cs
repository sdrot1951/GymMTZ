using System;
using System.Data;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using GymApp.BLL;
using GymApp.Entities;

namespace GYMMTZ
{
    public partial class FrmProducto : Form
    {
        private int? _idProductoEditando;
        private Label lblTitulo;
        private TextBox txtDescripcion, txtPrecio, txtCosto;
        private ComboBox cmbRubro; // ✨ ¡Ahora es un ComboBox!
        private Button btnGuardar, btnCancelar;

        private void FrmProducto_Load(object sender, EventArgs e)
        {

        }

     

        public FrmProducto()
        {
            InitializeComponent();
            ConstruirUI();
            CargarRubros();
            _idProductoEditando = null;
        }

        public FrmProducto(int idProducto)
        {
            InitializeComponent();
            ConstruirUI();
            CargarRubros(); // Primero cargamos el origen de datos del ComboBox

            _idProductoEditando = idProducto;
            lblTitulo.Text = "✏️ Editar Producto";
            btnGuardar.Text = "💾 Guardar Cambios";

            // Recuperar los datos actuales y llenar la UI
            var bll = new ProductoBLL();
            var prod = bll.ObtenerPorId(idProducto);
            if (prod != null)
            {
                txtDescripcion.Text = prod.fcDescripcion;
                txtPrecio.Text = prod.fiPrecio.ToString("0.00");
                txtCosto.Text = prod.fiCosto.ToString("0.00");
                cmbRubro.SelectedValue = prod.fiRubro; // Selecciona el rubro correspondiente
            }
        }



        private void ConstruirUI()
        {
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.DarkGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(480, 360);
            this.Text = "Administración de Producto";

            lblTitulo = new Label { Text = "📦 Registrar Nuevo Producto", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTitulo);

            txtDescripcion = CrearCampo("Descripción del Producto *", 20, 70, 420);
            txtPrecio = CrearCampo("Precio Venta ($) *", 20, 140, 200);
            txtCosto = CrearCampo("Costo Base ($) *", 240, 140, 200);

            Label lblRubro = new Label { Text = "Rubro / Categoría *", Location = new Point(20, 210), AutoSize = true, Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lblRubro);

            cmbRubro = new ComboBox
            {
                Location = new Point(20, 230),
                Size = new Size(420, 25),
                BackColor = Color.FromArgb(35, 35, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbRubro);

            btnCancelar = new Button { Text = "✖ Cancelar", Location = new Point(20, 280), Size = new Size(200, 38), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            btnGuardar = new Button { Text = "💾 Guardar (Stock: 0)", Location = new Point(240, 280), Size = new Size(200, 38), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }
        private TextBox CrearCampo(string etiqueta, int x, int y, int ancho)
        {
            Label lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lbl);
            TextBox txt = new TextBox { Location = new Point(x, y + 20), Size = new Size(ancho, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txt);
            return txt;
        }

        private void CargarRubros()
        {
            try
            {
                var bll = new ProductoBLL();
                DataTable dtRubros = bll.ObtenerRubros();

                // Conectamos los datos visuales y los ocultos
                cmbRubro.DataSource = dtRubros;
                cmbRubro.DisplayMember = "fcDescripcion"; // Lo que lee el usuario
                cmbRubro.ValueMember = "fiRubro";         // El ID oculto que va a la base de datos
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*        private TextBox CrearCampo(string etiqueta, int x, int y, int ancho)
                {
                    Label lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
                    this.Controls.Add(lbl);
                    TextBox txt = new TextBox { Location = new Point(x, y + 20), Size = new Size(ancho, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
                    this.Controls.Add(txt);
                    return txt;
                }*/


        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDescripcion.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtCosto.Text) || cmbRubro.SelectedValue == null)
                {
                    MessageBox.Show("Todos los campos marcados con * son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Producto prod = new Producto
                {
                    fcDescripcion = txtDescripcion.Text.Trim(),
                    fiPrecio = Convert.ToDecimal(txtPrecio.Text),
                    fiCosto = Convert.ToDecimal(txtCosto.Text),
                    fiRubro = Convert.ToInt32(cmbRubro.SelectedValue)
                };

                var bll = new ProductoBLL();
                bool exito = false;

                if (_idProductoEditando == null)
                {
                    prod.fiCantidad = 0; // Inserción forzada a stock 0
                    exito = bll.RegistrarNuevo(prod);
                }
                else
                {
                    prod.fiProducto = _idProductoEditando.Value; // Asignamos el ID
                    exito = bll.Editar(prod); // Ejecutamos la actualización
                }

                if (exito)
                {
                    MessageBox.Show("¡Datos del catálogo guardados correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico:\n\n" + ex.Message, "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}