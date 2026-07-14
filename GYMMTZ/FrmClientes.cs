using GymApp.BLL;
using GymApp.Entities;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmClientes : Form
    {
        private int? _idClienteEditando;

        // Controles
        private Label lblTitulo;
        private TextBox txtNombre, txtApePat, txtApeMat, txtTelefono, txtDireccion, txtEmail, txtemergencia, txtObservaciones;
        private Button btnGuardar, btnCancelar;
        private DateTimePicker dtpFechaNac;
        private void FrmClientes_Load(object sender, EventArgs e)
        {

        }

        // Constructor 1: Nuevo Cliente
        public FrmClientes()
        {
            ConstruirUI();
            _idClienteEditando = null;
        }




        // Constructor 2: Editar Cliente (Para cuando actives ese botón)
        public FrmClientes(int idCliente)
        {
            ConstruirUI();
            _idClienteEditando = idCliente;
            lblTitulo.Text = "✏️ Editar Cliente";

            // Traemos los datos de la base de datos a los TextBox
            var bll = new ClienteBLL();
            var cli = bll.ObtenerPorId(idCliente);
            if (cli != null)
            {
                txtNombre.Text = cli.fcNombre;
                txtApePat.Text = cli.fcApePat;
                txtApeMat.Text = cli.fcApeMat;
                txtTelefono.Text = cli.fiTelefono == 0 ? "" : cli.fiTelefono.ToString();
                txtDireccion.Text = cli.fcDireccion;
                txtEmail.Text = cli.fcEmail;
                txtemergencia.Text = cli.fcEmergencia;
                dtpFechaNac.Value = cli.fdFechaNac;
                txtObservaciones.Text = cli.fcObservaciones;
            }
        }

        private void ConstruirUI()
        {
            // Configuración general de la ventana (Más ancha y más corta)
            this.BackColor = Color.FromArgb(20, 20, 20); // Un negro un poco más profundo
            this.ForeColor = Color.DarkGray;
            this.FormBorderStyle = FormBorderStyle.Sizable; // Para que tenga la barra superior clásica de Windows como en tu captura
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(500, 750);
            this.Text = "FrmNuevoCliente"; // Título de la ventana de Windows

            // Título Principal
            lblTitulo = new Label { Text = "👥 Alta de Cliente", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTitulo);

            // --- FILA 1: Nombre completo (Ocupa todo el ancho) ---
            txtNombre = CrearCampo("Nombre(s)", 30, 80, 420);

            // --- FILA 2: Apellidos (Dos columnas) ---
            txtApePat = CrearCampo("Apellido Paterno", 30, 150, 200);
            txtApeMat = CrearCampo("Apellido Materno", 250, 150, 200);

            // --- FILA 3: Contacto (Dos columnas) ---
            txtTelefono = CrearCampo("Teléfono", 30, 220, 200);
            txtEmail = CrearCampo("Correo Electrónico", 250, 220, 200);

            // --- FILA 4: Detalles (Dos columnas) ---
            txtDireccion = CrearCampo("Direcion", 30, 290, 200);
            txtemergencia = CrearCampo("Contacto de emergencia", 30, 360, 200);

            // Fecha de Nacimiento (Acomodada a la derecha en la Fila 4)
            Label lblFecha = new Label { Text = "Fecha Nac.", 
                Location = new Point(250, 290), AutoSize = true, 
                Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lblFecha);
            dtpFechaNac = new DateTimePicker { Location = new Point(250, 310), Size = new Size(200, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpFechaNac);


            this.Height = 635;

            // 2. Empujamos la etiqueta más abajo (Y = 380)
            Label lblObservaciones = new Label
            {
                Text = "Observaciones (Condición médica, notas, etc.):",
                Location = new Point(20, 410), // <--- CAMBIO AQUÍ
                AutoSize = true,
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 9)
            };

            // 3. Empujamos el cuadro de texto más abajo (Y = 405)
            txtObservaciones = new TextBox
            {
                Name = "txtObservaciones",
                Location = new Point(20, 440), // <--- CAMBIO AQUÍ
                Width = 430,
                Height = 80,
                Multiline = true,
                MaxLength = 500,
                BackColor = Color.FromArgb(35, 35, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(lblObservaciones);
            this.Controls.Add(txtObservaciones);


            // --- BOTONES ---
            // Botón Cancelar (Estilo oscuro)
            btnCancelar = new Button { Text = "✖ Cancelar", 
            Location = new Point(30, 540), Size = new Size(200, 40), BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            // Botón Guardar (Estilo Naranja)
            btnGuardar = new Button { Text = "💾 Guardar", 
            Location = new Point(250, 540), Size = new Size(200, 40), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private TextBox CrearCampo(string etiqueta, int x, int y, int ancho)
        {
            Label lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            this.Controls.Add(lbl);

            TextBox txt = new TextBox
            {
                Location = new Point(x, y + 20),
                Size = new Size(ancho, 25),
                BackColor = Color.FromArgb(35, 35, 40), // Color de fondo ligeramente morado oscuro
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txt);
            return txt;
        }
        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números y la tecla de retroceso (Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Si es letra o símbolo, bloqueamos la pulsación
            }
        }
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApePat.Text))
                {
                    MessageBox.Show("El nombre y apellido paterno son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                long telefonoCliente = 0;
                string telTexto = txtTelefono.Text.Trim();

                if (!string.IsNullOrWhiteSpace(telTexto))
                {
                    // Verificamos la longitud exacta
                    if (telTexto.Length != 10)
                    {
                        MessageBox.Show("El número de teléfono debe tener exactamente 10 dígitos.",
                                        "Error de Validación",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        txtTelefono.Focus();
                        return; // Detenemos el guardado
                    }

                    // Convertimos a BIGINT (long) de forma segura
                    if (!long.TryParse(telTexto, out telefonoCliente))
                    {
                        MessageBox.Show("El teléfono debe contener solo números enteros.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTelefono.Focus();
                        return;
                    }
                }

                // =========================================================
                // VALIDACIÓN DEL FORMATO DE CORREO ELECTRÓNICO
                // =========================================================
                string emailIngresado = txtEmail.Text.Trim(); // Cambia txtEmail por el nombre real de tu TextBox

         /*       if (!string.IsNullOrWhiteSpace(emailIngresado))
                {
                    // Este es el "molde" matemático para un correo: texto@texto.texto
                    string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                    if (!Regex.IsMatch(emailIngresado, patronCorreo))
                    {
                        MessageBox.Show("El formato del correo electrónico no es válido.\n\nEjemplo correcto: usuario@dominio.com",
                                        "Error de Validación",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return; // Detenemos el guardado
                    }
                }*/
                // =========================================================




                Cliente nuevoCli = new Cliente
                {
                    fcNombre = txtNombre.Text.Trim(),
                    fcApePat = txtApePat.Text.Trim(),
                    fcApeMat = txtApeMat.Text.Trim(),
                    fiTelefono = telefonoCliente,
                    fcDireccion = txtDireccion.Text.Trim(),
                    fcEmail = txtEmail.Text.Trim(),
                    fcEmergencia = txtemergencia.Text.Trim(),
                    fdFechaNac = dtpFechaNac.Value.Date,
                    fcObservaciones = txtObservaciones.Text.Trim()
                };

                var bll = new ClienteBLL();
                int idClienteProcesado = 0;
                bool esNuevoRegistro = (_idClienteEditando == null);

                // 1. GUARDAMOS EN BASE DE DATOS
                if (esNuevoRegistro)
                {
                    // Atrapamos el nuevo ID que nos devuelve SQL
                    idClienteProcesado = bll.RegistrarNuevo(nuevoCli);
                }
                else
                {
                    // Si es edición, asumo que Editar() sigue devolviendo un booleano (true/false)
                    nuevoCli.fiCliente = _idClienteEditando.Value;
                    bool editado = bll.Editar(nuevoCli);
                    if (editado) idClienteProcesado = nuevoCli.fiCliente;
                }

                // 2. SI TODO SALIÓ BIEN, ACTIVAMOS EL FLUJO DE VENTANAS
                if (idClienteProcesado > 0)
                {
                    MessageBox.Show(esNuevoRegistro ? "¡Cliente registrado correctamente!" : "¡Cliente actualizado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // =============================================================
                    // MAGIA: FLUJO "SIGUIENTE" (Solo para clientes nuevos)
                    // =============================================================
                    if (esNuevoRegistro)
                    {

                        FrmTomarFoto frmFoto = new FrmTomarFoto();
                        if (frmFoto.ShowDialog() == DialogResult.OK)
                        {
                            byte[] fotoAguardar = frmFoto.FotoCapturadaBytes;

                            // Aquí mandas llamar a un método de tu ClienteBLL para guardar el "fotoAguardar" 
                            // usando un UPDATE a la tabla de clientes mediante su ID.
                            var bllCli = new GymApp.BLL.ClienteBLL();
                            bllCli.GuardarFoto(idClienteProcesado, fotoAguardar);
                        }
                        // A. Abrimos el registro de huellas (El usuario puede omitirlo dándole a Cancelar)
                        FrmHuellas frmHuella = new FrmHuellas(idClienteProcesado);
                        frmHuella.ShowDialog();

                        // B. Abrimos Punto de Venta para la Membresía
                        // Recuerda que en FrmVenta debes tener el constructor que recibe ID y Nombre que creamos antes
                        string nombreCompleto = $"{nuevoCli.fcNombre} {nuevoCli.fcApePat}";
                        FrmVenta frmVenta = new FrmVenta(idClienteProcesado, nombreCompleto);
                        frmVenta.ShowDialog();
                    }
                    // =============================================================

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al guardar el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió una excepción:\n\n" + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}