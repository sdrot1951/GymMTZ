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
    public partial class FrmVenta : Form
    {
        private Label lblTitulo, lblTotal, lblCliente, lblBuscador;
        private TextBox txtBuscadorCliente, txtBuscadorProducto, txtCantidad;
        private CheckBox rdoVentaMostrador, rdoMembresia, rdoVisita;



        private ListBox lstResultadosClientes, lstResultadosProductos;
        private ComboBox cmbTipoPago, cmbBuscadorProducto;
       // private Button btnAgregar, btnCobrar, btnCancelar;
        private Button btnAgregar, btnCobrar, btnCancelar, btnRestar; // <-- Agregamos btnRestar
        private DataGridView dgvCarrito;

        private CheckBox chkAplicaDescuento;
        private TextBox txtDescripcionDescuento;
        private TextBox txtMontoDescuento;

        // ====== NUEVAS VARIABLES PARA EL CARGO ======
        private CheckBox chkAplicaCargo;
        private TextBox txtDescripcionCargo;
        private TextBox txtMontoCargo;

        private List<VentaDetalle> listaCarrito = new List<VentaDetalle>();
        private decimal sumaTotalVenta = 0;

        private int idClienteSeleccionado = 1; // Público General por defecto
        private int idConceptoSeleccionado = 0;
        private int idProductoEquivalente = 0;
        private decimal precioSeleccionado = 0;

        private string descripcionSeleccionada = "";

        private Label lblMontoPagado;
        private TextBox txtMontoPagado;

        public FrmVenta()
        {
            InitializeComponent();
            ConstruirUI();
            CargarFormasPago();
           // AlternarModoVenta();
        }

       

        private void FrmVenta_Load(object sender, EventArgs e)
        {
            // Método requerido por el diseñador de Visual Studio
        }

        private void ConstruirUI()
        {
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.DarkGray;
            this.Size = new Size(720, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Punto de Venta Unificado";

            lblTitulo = new Label { Text = "🛒 Caja Registradora Omni", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), AutoSize = true };
            this.Controls.Add(lblTitulo);

            // ── RADIO BUTTONS DE MODO ──
          /*  rdoVentaMostrador = new CheckBox { Text = "🏪 Venta Mostrador", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(255, 69, 0), Checked = true };
            rdoMembresia = new CheckBox { Text = "💳 Alta Membresía", Location = new Point(200, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Cyan };
            rdoVisita = new CheckBox { Text = "🏃 Pago Visita", Location = new Point(380, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.YellowGreen };
*/
            rdoVentaMostrador = new CheckBox { Text = "🏪 Venta Mostrador", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(255, 69, 0) };
            rdoMembresia = new CheckBox { Text = "💳 Alta Membresía", Location = new Point(200, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Cyan };
            rdoVisita = new CheckBox { Text = "🏃 Pago Visita", Location = new Point(380, 60), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.YellowGreen };

            rdoVentaMostrador.CheckedChanged += (s, e) => AlternarModoVenta();
            rdoMembresia.CheckedChanged += (s, e) => AlternarModoVenta();
            rdoVisita.CheckedChanged += (s, e) => AlternarModoVenta();
            this.Controls.Add(rdoVentaMostrador); this.Controls.Add(rdoMembresia); this.Controls.Add(rdoVisita);

            // ── BUSCADOR PREDICTIVO DE CLIENTES ──
            lblCliente = new Label { Text = "Buscar Cliente (Nombre o Apellido):", Location = new Point(20, 105), AutoSize = true };
            txtBuscadorCliente = new TextBox { Location = new Point(20, 125), Size = new Size(300, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtBuscadorCliente.TextChanged += TxtBuscadorCliente_TextChanged;

            lstResultadosClientes = new ListBox { Location = new Point(20, 151), Size = new Size(300, 80), BackColor = Color.FromArgb(30, 30, 35), ForeColor = Color.White, Visible = false, BorderStyle = BorderStyle.FixedSingle };
            lstResultadosClientes.Click += LstResultadosClientes_Click;

            this.Controls.Add(lblCliente); this.Controls.Add(txtBuscadorCliente); this.Controls.Add(lstResultadosClientes);

            // ── BUSCADOR PREDICTIVO DE PRODUCTOS / SERVICIOS ──
            lblBuscador = new Label { Text = "Buscar Artículo / Concepto:", Location = new Point(20, 165), AutoSize = true };
            this.Controls.Add(lblBuscador);

            // Instanciar y agregar el TextBox (Mostrador)
            txtBuscadorProducto = new TextBox { Location = new Point(20, 185), Size = new Size(460, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtBuscadorProducto.TextChanged += TxtBuscadorProducto_TextChanged;
            this.Controls.Add(txtBuscadorProducto); // <-- Asegúrate de que tenga esta línea



            cmbBuscadorProducto = new ComboBox { Location = new Point(20, 185), Size = new Size(460, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, Visible = false };
            cmbBuscadorProducto.SelectedIndexChanged += CmbBuscadorProducto_SelectedIndexChanged;
            this.Controls.Add(cmbBuscadorProducto);



            lstResultadosProductos = new ListBox { Location = new Point(20, 211), Size = new Size(460, 100), BackColor = Color.FromArgb(30, 30, 35), ForeColor = Color.White, Visible = false, BorderStyle = BorderStyle.FixedSingle };
            lstResultadosProductos.Click += LstResultadosProductos_Click;

            txtCantidad = new TextBox { Location = new Point(495, 185), Size = new Size(60, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = "1", TextAlign = HorizontalAlignment.Center };
            Label lblCant = new Label { Text = "Cant.", Location = new Point(495, 165), AutoSize = true };

            btnAgregar = new Button { Text = "➕", Location = new Point(565, 184), Size = new Size(40, 27), BackColor = Color.FromArgb(45, 45, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAgregar.Click += BtnAgregar_Click;

            // ── BOTÓN DISMINUIR / ELIMINAR ──
            btnRestar = new Button
            {
                Text = "➖", // Lo dejamos solo con el icono para que sea un cuadrado perfecto
                Location = new Point(610, 184), // Lo alineamos en la misma altura (Y=184) y a 5 pixeles del botón '+'
                Size = new Size(40, 27), // Mismo tamaño exacto que btnAgregar
                BackColor = Color.FromArgb(45, 45, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRestar.Click += BtnRestar_Click;
            this.Controls.Add(btnRestar);


            this.Controls.Add(lblBuscador); this.Controls.Add(txtBuscadorProducto); this.Controls.Add(lstResultadosProductos);
            this.Controls.Add(lblCant); this.Controls.Add(txtCantidad); this.Controls.Add(btnAgregar);

            // ── GRID DEL CARRITO ──
            dgvCarrito = new DataGridView { Location = new Point(20, 230), Size = new Size(660, 190), BackgroundColor = Color.FromArgb(30, 30, 30), AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ForeColor = Color.Black };
            dgvCarrito.Columns.Add("ID", "SKU"); dgvCarrito.Columns.Add("Desc", "Descripción"); dgvCarrito.Columns.Add("Cant", "Cant."); dgvCarrito.Columns.Add("Pre", "Precio U."); dgvCarrito.Columns.Add("Sub", "Subtotal");
            this.Controls.Add(dgvCarrito);

            // ── FOOTER ──
            Label lblPago = new Label { Text = "Forma de Pago:", Location = new Point(20, 440), AutoSize = true };
            cmbTipoPago = new ComboBox { Location = new Point(20, 460), Size = new Size(200, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            this.Controls.Add(lblPago); this.Controls.Add(cmbTipoPago);

            lblTotal = new Label { Text = "TOTAL: $0.00", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.LimeGreen, Location = new Point(460, 450), AutoSize = true };
            this.Controls.Add(lblTotal);
            // 1. Aumentamos la altura de la ventana a 640
            this.Size = new Size(720, 640);

            // 2. Bajamos los botones principales a Y = 550
            btnCancelar = new Button { Text = "✖ Cancelar", Location = new Point(20, 550), Size = new Size(130, 35), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            btnCobrar = new Button { Text = "💳 PROCESAR COBRO", Location = new Point(490, 550), Size = new Size(190, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCobrar.Click += BtnCobrar_Click;
            this.Controls.Add(btnCobrar);

            // ====== CONTROLES DE DESCUENTO (Se quedan en Y=440 y 460) ======
            chkAplicaDescuento = new CheckBox { Text = "Aplicar Descuento", Location = new Point(240, 440), AutoSize = true, ForeColor = Color.Yellow };
            txtDescripcionDescuento = new TextBox { Location = new Point(240, 460), Size = new Size(130, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Enabled = false };
            txtMontoDescuento = new TextBox { Location = new Point(380, 460), Size = new Size(60, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Enabled = false, Text = "0" };

            chkAplicaDescuento.CheckedChanged += (s, e) => {
                txtDescripcionDescuento.Enabled = chkAplicaDescuento.Checked;
                txtMontoDescuento.Enabled = chkAplicaDescuento.Checked;
                if (!chkAplicaDescuento.Checked) { txtDescripcionDescuento.Clear(); txtMontoDescuento.Text = "0"; }
                CalcularGranTotal();
            };
            txtMontoDescuento.TextChanged += (s, e) => CalcularGranTotal();

            this.Controls.Add(chkAplicaDescuento); this.Controls.Add(txtDescripcionDescuento); this.Controls.Add(txtMontoDescuento);

            // ====== NUEVOS CONTROLES DE CARGO EXTRA (En Y=490 y 510) ======
            chkAplicaCargo = new CheckBox { Text = "Aplicar Cargo Extra", Location = new Point(240, 490), AutoSize = true, ForeColor = Color.Orange };
            txtDescripcionCargo = new TextBox { Location = new Point(240, 510), Size = new Size(130, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Enabled = false };
            txtMontoCargo = new TextBox { Location = new Point(380, 510), Size = new Size(60, 25), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Enabled = false, Text = "0" };

            chkAplicaCargo.CheckedChanged += (s, e) => {
                txtDescripcionCargo.Enabled = chkAplicaCargo.Checked;
                txtMontoCargo.Enabled = chkAplicaCargo.Checked;
                if (!chkAplicaCargo.Checked) { txtDescripcionCargo.Clear(); txtMontoCargo.Text = "0"; }
                CalcularGranTotal();
            };
            txtMontoCargo.TextChanged += (s, e) => CalcularGranTotal();

            this.Controls.Add(chkAplicaCargo); this.Controls.Add(txtDescripcionCargo); this.Controls.Add(txtMontoCargo);

            // ====== MONTO PAGADO ======
            lblMontoPagado = new Label { Text = "Monto Recibido:", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Location = new Point(450, 415), AutoSize = true };
            txtMontoPagado = new TextBox { Location = new Point(600, 415), Size = new Size(80, 25), Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Right };

            this.Controls.Add(lblMontoPagado);
            this.Controls.Add(txtMontoPagado);





        }

        private void CargarFormasPago()
        {
            try
            {
                var bll = new GymApp.BLL.VentaBLL(); // O VentaBLL
                cmbTipoPago.DataSource = bll.ObtenerTiposPago();
                cmbTipoPago.DisplayMember = "Descripcion";
                cmbTipoPago.ValueMember = "ID";
            }
            catch { }
        }

        private void AlternarModoVenta()
        {
            // Evaluamos si los tres checkboxes están desmarcados (Estado Inicial / Reset)
            bool estadoInicial = !rdoVentaMostrador.Checked && !rdoMembresia.Checked && !rdoVisita.Checked;

            if (estadoInicial)
            {
                // 1. Mostrar las 3 opciones para que el usuario pueda elegir
                rdoVentaMostrador.Visible = true;
                rdoMembresia.Visible = true;
                rdoVisita.Visible = true;
            }
            else
            {
                // 2. Ocultar los que NO están seleccionados
                rdoVentaMostrador.Visible = rdoVentaMostrador.Checked;
                rdoMembresia.Visible = rdoMembresia.Checked;
                rdoVisita.Visible = rdoVisita.Checked;
            }

            bool esVisita = rdoVisita.Checked;
            bool esMembresia = rdoMembresia.Checked;
            bool usarCombo = esVisita || esMembresia;

            // ====== LA CORRECCIÓN ESTÁ AQUÍ ======
            // El cliente será visible SI es Membresía, o SI estamos en el estado inicial (todos desmarcados)
            lblCliente.Visible = esMembresia || estadoInicial;
            txtBuscadorCliente.Visible = esMembresia || estadoInicial;

            // Si estamos en Visita o Mostrador, forzamos el reset del cliente al Genérico
            if (!esMembresia && !estadoInicial)
            {
                txtBuscadorCliente.Clear();
                idClienteSeleccionado = 1;
            }

            // El resto de la lógica de productos se queda igual
            txtCantidad.Text = "1";
            //txtCantidad.Enabled = !usarCombo;

            txtBuscadorProducto.Visible = !usarCombo;
            cmbBuscadorProducto.Visible = usarCombo;

            lblBuscador.Text = usarCombo ? "Seleccionar Artículo / Concepto:" : "Buscar Artículo / Concepto:";
            lstResultadosProductos.Visible = false;

            if (usarCombo)
            {
                CargarComboConceptos(esMembresia);
            }
            else
            {
                txtBuscadorProducto.Clear();
                idConceptoSeleccionado = 0;
                idProductoEquivalente = 0;
            }

            if (listaCarrito.Count > 0)
            {
                listaCarrito.Clear();
                dgvCarrito.Rows.Clear();
                CalcularGranTotal();
            }

            if (listaCarrito.Count > 0)
            {
                listaCarrito.Clear();
                dgvCarrito.Rows.Clear();
                CalcularGranTotal();
            }

            // ====== LÓGICA DE DESCUENTOS Y CARGOS ======
            // Ocultamos todo este bloque si es venta de mostrador
            bool permiteDescuento = rdoMembresia.Checked || rdoVisita.Checked;

            chkAplicaDescuento.Visible = permiteDescuento;
            txtDescripcionDescuento.Visible = permiteDescuento;
            txtMontoDescuento.Visible = permiteDescuento;

            chkAplicaCargo.Visible = permiteDescuento;
            txtDescripcionCargo.Visible = permiteDescuento;
            txtMontoCargo.Visible = permiteDescuento;

            // Si cambiaron a Mostrador, desmarcamos ambos por seguridad
            if (!permiteDescuento)
            {
                chkAplicaDescuento.Checked = false;
                chkAplicaCargo.Checked = false;
            }


        }

        private void TxtBuscadorCliente_TextChanged(object sender, EventArgs e)
        {
            string texto = txtBuscadorCliente.Text.Trim();
            if (texto.Length < 3) { lstResultadosClientes.Visible = false; return; }

            try
            {
                var bll = new GymApp.BLL.VentaBLL(); // Reemplaza por ClienteBLL
                DataTable dt = bll.BuscarClientes(texto);

                if (dt.Rows.Count > 0)
                {
                    lstResultadosClientes.DataSource = dt;
                    lstResultadosClientes.DisplayMember = "Descripcion";
                    lstResultadosClientes.ValueMember = "ID";
                    lstResultadosClientes.Visible = true;
                    lstResultadosClientes.BringToFront();
                }
                else { lstResultadosClientes.Visible = false; }
            }
            catch { }
        }

        private void LstResultadosClientes_Click(object sender, EventArgs e)
        {
            if (lstResultadosClientes.SelectedValue == null) return;
            DataRowView drv = (DataRowView)lstResultadosClientes.SelectedItem;
            idClienteSeleccionado = Convert.ToInt32(drv["ID"]);
            txtBuscadorCliente.Text = drv["Descripcion"].ToString();
            lstResultadosClientes.Visible = false;
        }

        private void TxtBuscadorProducto_TextChanged(object sender, EventArgs e)
        {
            string texto = txtBuscadorProducto.Text.Trim();

            // Si el usuario borra el texto, ocultamos la lista
            if (string.IsNullOrEmpty(texto) || texto.Length < 2)
            {
                lstResultadosProductos.Visible = false;
                return;
            }

            try
            {
                var bll = new GymApp.BLL.VentaBLL();
                DataTable dt = bll.ConsultarProductos(texto);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Limpiamos bindings previos
                    lstResultadosProductos.DataSource = null;

                    // Asignamos los datos
                    lstResultadosProductos.DataSource = dt;

                    // Verificamos que las columnas existan antes de asignar
                    if (dt.Columns.Contains("fiProducto") && dt.Columns.Contains("fcDescripcion"))
                    {
                        lstResultadosProductos.DisplayMember = "fcDescripcion";
                        lstResultadosProductos.ValueMember = "fiProducto";
                    }
                    else
                    {
                        MessageBox.Show("Error: Las columnas 'fcDescripcion' o 'fiProducto' no existen en el resultado de SQL.");
                    }

                    lstResultadosProductos.Visible = true;
                    lstResultadosProductos.BringToFront();
                }
                else
                {
                    lstResultadosProductos.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar productos: " + ex.Message);
            }
        }

        private void LstResultadosProductos_Click(object sender, EventArgs e)
        {
            if (lstResultadosProductos.SelectedValue == null) return;
            DataRowView drv = (DataRowView)lstResultadosProductos.SelectedItem;

            idConceptoSeleccionado = Convert.ToInt32(drv["fiProducto"]);
            precioSeleccionado = Convert.ToDecimal(drv["fiPrecio"]);
            txtBuscadorProducto.Text = drv["fcDescripcion"].ToString();

            // Atrapamos el ID Genérico si estamos en Membresía o Visita
            if (rdoMembresia.Checked || rdoVisita.Checked)
                idProductoEquivalente = Convert.ToInt32(drv["ProductoEquivalente"]);
            else
                idProductoEquivalente = idConceptoSeleccionado; // Si es mostrador, el SKU es el ID mismo.

            lstResultadosProductos.Visible = false;
        }
        private void CargarComboConceptos(bool esMembresia)
        {
            try
            {
                var bll = new GymApp.BLL.VentaBLL();
                DataTable dt;

                if (esMembresia)
                    dt = bll.BuscarPorDescripcion("", true);
                else
                    dt = bll.BuscarVisitas("");

                cmbBuscadorProducto.DataSource = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    cmbBuscadorProducto.SelectedIndexChanged -= CmbBuscadorProducto_SelectedIndexChanged;

                    cmbBuscadorProducto.DataSource = dt;
                    cmbBuscadorProducto.DisplayMember = "Descripcion";
                    cmbBuscadorProducto.ValueMember = "ID";
                    cmbBuscadorProducto.SelectedIndex = -1;

                    cmbBuscadorProducto.SelectedIndexChanged += CmbBuscadorProducto_SelectedIndexChanged;
                }
            }
            catch { }
        }


        private void CmbBuscadorProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBuscadorProducto.SelectedIndex == -1 || cmbBuscadorProducto.SelectedItem == null) return;

            DataRowView drv = (DataRowView)cmbBuscadorProducto.SelectedItem;

            idConceptoSeleccionado = Convert.ToInt32(drv["ID"]);
            precioSeleccionado = Convert.ToDecimal(drv["Precio"]);
            idProductoEquivalente = Convert.ToInt32(drv["ProductoEquivalente"]);

            // ====== NUEVA LÍNEA: Guardamos el nombre para el Grid ======
            descripcionSeleccionada = drv["Descripcion"].ToString();
        }
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (idConceptoSeleccionado == 0 || string.IsNullOrWhiteSpace(txtCantidad.Text)) return;

            int cantidad = Convert.ToInt32(txtCantidad.Text);
            bool usarCombo = rdoMembresia.Checked;  
            //|| rdoVisita.Checked; Ya se puedem agregar difereentes tipos de visitas

            if (usarCombo)
            {
                if (listaCarrito.Count > 0) { MessageBox.Show("Las membresías y visitas se facturan en un ticket individual.", "Aviso"); return; }
            }

            string nombreArticulo = usarCombo ? descripcionSeleccionada : txtBuscadorProducto.Text;

            // ====== LÓGICA DE AGRUPACIÓN (NUEVA) ======

            // 1. Buscamos si el producto ya existe en el carrito
            var itemExistente = listaCarrito.FirstOrDefault(x => x.fiConceptoOmni == idConceptoSeleccionado);

            if (itemExistente != null)
            {
                // Si el producto ya está, solo aumentamos la cantidad
                itemExistente.fiCantidad += cantidad;

                // Ojo: Si en tu clase VentaDetalle 'fmSubtotal' NO se calcula automáticamente, 
                // tendrías que actualizarlo aquí manualmente así:
                //itemExistente.fmSubtotal = itemExistente.fiCantidad * itemExistente.fiPrecio;
            }
            else
            {
                // Si no existe, lo agregamos como un producto nuevo a la lista
                var item = new VentaDetalle
                {
                    fiProducto = idProductoEquivalente,
                    fiConceptoOmni = idConceptoSeleccionado,
                    fcDescripcion = nombreArticulo,
                    fiCantidad = cantidad,
                    fiPrecio = precioSeleccionado
                };
                listaCarrito.Add(item);
            }

            // ====== REFRESCO DEL GRID ======

            // Limpiamos el grid y lo volvemos a dibujar desde la lista para garantizar sincronía
            dgvCarrito.Rows.Clear();
            foreach (var det in listaCarrito)
            {
                dgvCarrito.Rows.Add(det.fiProducto, det.fcDescripcion, det.fiCantidad, det.fiPrecio.ToString("$#,##0.00"), det.fmSubtotal.ToString("$#,##0.00"));
            }

            CalcularGranTotal();

            // ====== LIMPIEZA PARA EL SIGUIENTE PRODUCTO ======
            txtBuscadorProducto.Clear();
            if (usarCombo) cmbBuscadorProducto.SelectedIndex = -1;

            idConceptoSeleccionado = 0;
            idProductoEquivalente = 0;
            precioSeleccionado = 0;
            descripcionSeleccionada = "";

            // UX Extra: Regresamos la cantidad a 1 y el foco al buscador por defecto
            txtCantidad.Text = "1";
            if (!usarCombo) txtBuscadorProducto.Focus();
        }

        private void BtnRestar_Click(object sender, EventArgs e)
        {
            // 1. Validamos que haya una fila seleccionada en el Grid
            if (dgvCarrito.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un producto del carrito haciendo clic en él.", "Aviso");
                return;
            }

            // 2. La magia: El índice del Grid es exactamente el mismo que el de tu Lista
            int index = dgvCarrito.SelectedRows[0].Index;
            var item = listaCarrito[index];

            // 3. Restamos la cantidad
            item.fiCantidad--;

            // Si tu entidad no calcula el subtotal sola, lo forzamos aquí:
            //item.fmSubtotal = item.fiCantidad * item.fiPrecio;

            // 4. Si la cantidad llega a cero (o menos), lo borramos de la lista
            if (item.fiCantidad <= 0)
            {
                listaCarrito.RemoveAt(index);
            }

            // 5. Sincronizamos la parte visual (Borramos y redibujamos)
            dgvCarrito.Rows.Clear();
            foreach (var det in listaCarrito)
            {
                dgvCarrito.Rows.Add(det.fiProducto, det.fcDescripcion, det.fiCantidad, det.fiPrecio.ToString("$#,##0.00"), det.fmSubtotal.ToString("$#,##0.00"));
            }

            // 6. Recalculamos la suma de dinero
            CalcularGranTotal();
        }
        private void CalcularGranTotal()
        {
            sumaTotalVenta = 0;

            // 1. Sumamos todos los productos del grid
            foreach (var item in listaCarrito)
            {
                sumaTotalVenta += item.fmSubtotal;
            }

            // 2. Restamos el descuento
            if (chkAplicaDescuento.Checked && decimal.TryParse(txtMontoDescuento.Text, out decimal descuento))
            {
                if (descuento > sumaTotalVenta) descuento = sumaTotalVenta;
                sumaTotalVenta -= descuento;
            }

            // 3. SUMAMOS EL CARGO EXTRA (¡La magia de este requerimiento!)
            if (chkAplicaCargo.Checked && decimal.TryParse(txtMontoCargo.Text, out decimal cargo))
            {
                sumaTotalVenta += cargo;
            }

            // 4. Imprimimos el resultado final
            lblTotal.Text = "TOTAL: " + sumaTotalVenta.ToString("$#,##0.00");
            txtMontoPagado.Text = sumaTotalVenta.ToString("0.00"); // Autocompletar
        }

        //private void BtnCobrar_Click(object sender, EventArgs e)
        //{
        //    if (listaCarrito.Count == 0) return;

        //   // if (listaCarrito.Count == 0) return;

        //    // ====== NUEVA VALIDACIÓN: CLIENTE OBLIGATORIO PARA MEMBRESÍAS ======
        //    // Asumimos que el ID 1 es "Público General". Si es 1 o 0, bloqueamos la venta.
        //    if (rdoMembresia.Checked && idClienteSeleccionado <= 1)
        //    {
        //        MessageBox.Show("Para dar de alta una membresía, es obligatorio buscar y seleccionar un cliente registrado.", "Cliente Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        txtBuscadorCliente.Focus();
        //        return; // Detenemos el proceso de cobro aquí mismo
        //    }
        //    // ===================================================================

        //    try
        //    {

        //        if (!decimal.TryParse(txtMontoPagado.Text, out decimal montoPagado))
        //        {
        //            MessageBox.Show("Por favor ingrese un monto de pago numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }


        //        int idPago = Convert.ToInt32(cmbTipoPago.SelectedValue);
        //        int idVendedor = GymApp.Core.SesionGlobal.IdEmpleado;

        //        string tipoVenta = "MOSTRADOR";
        //        if (rdoMembresia.Checked) tipoVenta = "MEMBRESIA";
        //        else if (rdoVisita.Checked) tipoVenta = "VISITA";

        //        // Capturar datos del descuento
        //        bool aplicaDesc = chkAplicaDescuento.Checked;
        //        string descripcionDesc = txtDescripcionDescuento.Text.Trim();
        //        decimal montoDesc = 0;

        //        if (aplicaDesc)
        //        {
        //            decimal.TryParse(txtMontoDescuento.Text, out montoDesc);
        //        }

        //        decimal totalBruto = 0;
        //        foreach (var item in listaCarrito)
        //        {
        //            totalBruto += item.fmSubtotal;
        //        }

        //        var bll = new GymApp.BLL.VentaBLL();

        //        // Pasamos las nuevas variables al final del método (aplicaDesc, descripcionDesc, montoDesc, idVendedor)
        //        //if (bll.ProcesarVentaOmni(idClienteSeleccionado, idVendedor, sumaTotalVenta, idPago, tipoVenta, listaCarrito, aplicaDesc, descripcionDesc, montoDesc, idVendedor))
        //        if (bll.ProcesarVentaOmni(idClienteSeleccionado, idVendedor, sumaTotalVenta, montoPagado, idPago, tipoVenta, listaCarrito, aplicaDesc, descripcionDesc, montoDesc, idVendedor))
        //            {
        //            MessageBox.Show("¡Venta procesada con éxito!", "Éxito");
        //            this.DialogResult = DialogResult.OK;
        //            this.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Validación de Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            if (listaCarrito.Count == 0) return;

            // ====== NUEVA VALIDACIÓN: CLIENTE OBLIGATORIO PARA MEMBRESÍAS ======
            if (rdoMembresia.Checked && idClienteSeleccionado <= 1)
            {
                MessageBox.Show("Para dar de alta una membresía, es obligatorio buscar y seleccionar un cliente registrado.", "Cliente Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscadorCliente.Focus();
                return;
            }
            // ===================================================================

            try
            {
                if (!decimal.TryParse(txtMontoPagado.Text, out decimal montoPagado))
                {
                    MessageBox.Show("Por favor ingrese un monto de pago numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idPago = Convert.ToInt32(cmbTipoPago.SelectedValue);
                int idVendedor = GymApp.Core.SesionGlobal.IdEmpleado;

                string tipoVenta = "MOSTRADOR";
                if (rdoMembresia.Checked) tipoVenta = "MEMBRESIA";
                else if (rdoVisita.Checked) tipoVenta = "VISITA";

                // ====== SOLUCIÓN: CALCULAMOS EL TOTAL BRUTO PRIMERO ======
                decimal totalBruto = 0;
                foreach (var item in listaCarrito)
                {
                    totalBruto += item.fmSubtotal;
                }

                // Capturar datos del descuento
                bool aplicaDesc = chkAplicaDescuento.Checked;
                string descripcionDesc = txtDescripcionDescuento.Text.Trim();
                decimal montoDesc = 0;

                if (aplicaDesc)
                {
                    decimal.TryParse(txtMontoDescuento.Text, out montoDesc);

                    // === AHORA SÍ: Comparamos el descuento contra el total ORIGINAL sin afectar ===
                    if (montoDesc == totalBruto && totalBruto > 0)
                    {
                        var confirmacion = MessageBox.Show(
                            "Se está aplicando un descuento del 100% (Pago Cero). ¿Desea continuar con el descuento del 100%?",
                            "Confirmación de Descuento",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (confirmacion == DialogResult.No)
                        {
                            return; // Si el cajero dice que No, detenemos todo el proceso aquí.
                        }
                    }
                }

                var bll = new GymApp.BLL.VentaBLL();

                // Pasamos las variables al final del método (usando tu sumaTotalVenta que ya lleva las restas/sumas finales)
                if (bll.ProcesarVentaOmni(idClienteSeleccionado, idVendedor, sumaTotalVenta, montoPagado, idPago, tipoVenta, listaCarrito, aplicaDesc, descripcionDesc, montoDesc, idVendedor))
                {
                    MessageBox.Show("¡Venta procesada con éxito!", "Éxito");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validación de Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // NUEVO CONSTRUCTOR PARA FLUJO AUTOMÁTICO
        public FrmVenta(int idClientePrecargado, string nombreCliente) : this() // El ': this()' ejecuta el constructor normal para dibujar la pantalla
        {
            // 1. Asignamos el cliente que nos enviaron
            idClienteSeleccionado = idClientePrecargado;

            // 2. Lo mostramos en la caja de búsqueda para que el cajero lo vea
            txtBuscadorCliente.Text = nombreCliente;

            // 3. Seleccionamos automáticamente el modo "Membresía"
            rdoMembresia.Checked = true;

            // 4. Enfocamos la caja de artículos para que el cajero solo busque qué membresía venderle
            txtBuscadorProducto.Focus();
        }
    }
}