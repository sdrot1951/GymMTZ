using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GymApp.BLL;
using GymApp.DAO;
using GymApp.Entities;
using GYMMTZ.Controls;
using GYMMTZ.Theme;
using System.Text.RegularExpressions;

namespace GYMMTZ
{
    public partial class FrmNuevoEmpleado : Form
    {
        // Controles de la UI
        private GymTextBox txtNombre;
        private GymTextBox txtApePat;
        private GymTextBox txtApeMat;
        private GymTextBox txtTelefono;
        private GymTextBox txtEmail;
        private GymTextBox txtDireccion;
        private ComboBox cmbPuesto;
        private DateTimePicker dtpFechaNac;
        private Panel pnlAcceso;
        private GymTextBox txtUsuario;
        private GymTextBox txtPassword;
        private GymButton btnGuardar;
        private GymButton btnCancelar;
        private int? _idEmpleadoEditando = null; // Si es null = Modo Nuevo, si tiene número = Modo Editar
        private Label lblTitulo;
        private Label lblPuesto;
        public FrmNuevoEmpleado()
        {
            ConstruirUI();
            InitializeComponent();
            _idEmpleadoEditando = null;
        }

        // Constructor 2 (Sobrecargado): Se usa para EDICIÓN
        public FrmNuevoEmpleado(int idEmpleado)
        {
            ConstruirUI();
            InitializeComponent();
            _idEmpleadoEditando = idEmpleado;
        }
        private void CargarPuestos()
        {
            try
            {
                PuestoBLL bll = new PuestoBLL();
                DataTable dt = bll.ObtenerCatalogoPuestos();


                cmbPuesto.ValueMember = "fiPuesto";
                cmbPuesto.DisplayMember = "fcDescripcion";
                cmbPuesto.DataSource = dt;

                if (cmbPuesto.Items.Count > 0)
                    cmbPuesto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar puestos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbPuesto_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (_idEmpleadoEditando.HasValue) return;

            if (cmbPuesto.SelectedValue == null) return;

            // Validación segura del ID
            if (!int.TryParse(cmbPuesto.SelectedValue.ToString(), out int idSeleccionado)) return;

            bool requiereAcceso = (idSeleccionado == 1 || idSeleccionado == 2 || idSeleccionado == 10 || idSeleccionado == 3);

            pnlAcceso.Visible = requiereAcceso;

            if (requiereAcceso)
            {
                // Expandir el formulario
                this.ClientSize = new Size(480, 650);

                // Mover botones hacia abajo
                btnCancelar.Location = new Point(20, 560);
                btnGuardar.Location = new Point(240, 560);

                // Mover el panel de acceso arriba de los botones
                pnlAcceso.Location = new Point(20, 470);
            }
            else
            {
                // Formulario compacto
                this.ClientSize = new Size(480, 550);

                // Mover botones hacia arriba
                btnCancelar.Location = new Point(20, 470);
                btnGuardar.Location = new Point(240, 470);

                // Ocultar panel
                pnlAcceso.Location = new Point(20, 440);
            }
        }

        private void ConstruirUI()
        {
            // Configuración base del Formulario
            this.Text = "Registrar Nuevo Empleado";
            this.ClientSize = new Size(480, 550); // Usar ClientSize en lugar de Size
            this.BackColor = GymTheme.Background;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ========== TÍTULO ==========
            lblTitulo = new Label
            {
                Text = "👥 Alta de Empleado",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = GymTheme.TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitulo);

            // ========== CAMPOS DE EMPLEADO ==========
            txtNombre = new GymTextBox { LabelText = "Nombre(s)", Location = new Point(20, 70), Width = 420 };
            txtApePat = new GymTextBox { LabelText = "Apellido Paterno", Location = new Point(20, 140), Width = 200 };
            txtApeMat = new GymTextBox { LabelText = "Apellido Materno", Location = new Point(240, 140), Width = 200 };
            txtTelefono = new GymTextBox { LabelText = "Teléfono", Location = new Point(20, 210), Width = 200 };
            txtEmail = new GymTextBox { LabelText = "Correo Electrónico", Location = new Point(240, 210), Width = 200 };
            txtDireccion = new GymTextBox { LabelText = "Dirección", Location = new Point(20, 280), Width = 420 };

            // Fecha de Nacimiento
            var lblFecha = new Label
            {
                Text = "Fecha Nac.",
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextSecondary,
                Location = new Point(20, 360),
                AutoSize = true
            };

            dtpFechaNac = new DateTimePicker
            {
                Location = new Point(20, 380),
                Width = 200,
                Font = GymTheme.FontBody,
                Format = DateTimePickerFormat.Short
            };

            // Puesto
            lblPuesto = new Label
            {
                Text = "Puesto",
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextSecondary,
                Location = new Point(240, 360),
                AutoSize = true
            };

            cmbPuesto = new ComboBox
            {
                Location = new Point(240, 380),
                Width = 200,
                Font = GymTheme.FontBody,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = GymTheme.SurfaceElevated,
                ForeColor = GymTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat
            };

            // ========== PANEL DE ACCESO (Usuario/Contraseña) ==========
            pnlAcceso = new Panel();
            pnlAcceso.Location = new Point(20, 440);
            pnlAcceso.Size = new Size(440, 70);
            pnlAcceso.Visible = false; // Inicialmente oculto
            pnlAcceso.BackColor = Color.Transparent;

            txtUsuario = new GymTextBox { LabelText = "Usuario", Location = new Point(0, 0), Width = 200 };
            txtPassword = new GymTextBox { LabelText = "Contraseña", Location = new Point(220, 0), Width = 200, PasswordChar = '●' };

            pnlAcceso.Controls.Add(txtUsuario);
            pnlAcceso.Controls.Add(txtPassword);

            // ========== BOTONES ==========
            btnGuardar = new GymButton
            {
                Text = "💾 Guardar",
                Style = GymButton.ButtonStyle.Primary,
                Location = new Point(240, 470),
                Width = 200
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new GymButton
            {
                Text = "❌ Cancelar",
                Style = GymButton.ButtonStyle.Secondary,
                Location = new Point(20, 470),
                Width = 200
            };
            btnCancelar.Click += (s, e) => this.Close();

            // ========== AGREGAR CONTROLES AL FORM ==========
            this.Controls.Add(txtNombre);
            this.Controls.Add(txtApePat);
            this.Controls.Add(txtApeMat);
            this.Controls.Add(txtTelefono);
            this.Controls.Add(txtEmail);
            this.Controls.Add(txtDireccion);
            this.Controls.Add(lblFecha);
            this.Controls.Add(dtpFechaNac);
            this.Controls.Add(lblPuesto);
            this.Controls.Add(cmbPuesto);
            this.Controls.Add(pnlAcceso);  // Panel primero (para orden de capas)
            this.Controls.Add(btnCancelar);
            this.Controls.Add(btnGuardar);

            // ========== CARGAR DATOS Y CONFIGURAR EVENTOS ==========
            CargarPuestos();
            cmbPuesto.SelectedIndexChanged += cmbPuesto_SelectedIndexChanged;

            // Forzar el evento para ajustar posiciones iniciales
            this.Load += (s, e) =>
            {
                if (cmbPuesto.Items.Count > 0)
                    cmbPuesto_SelectedIndexChanged(cmbPuesto, EventArgs.Empty);
            };
            this.Refresh();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtApePat.Text))
                {
                    MessageBox.Show("El apellido paterno es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtApePat.Focus();
                    return;
                }

                if (cmbPuesto.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un puesto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // =========================================================
                // VALIDACIÓN DEL FORMATO DE CORREO ELECTRÓNICO
                // =========================================================
                string emailIngresado = txtEmail.Text.Trim(); // Cambia txtEmail por el nombre real de tu TextBox

                if (!string.IsNullOrWhiteSpace(emailIngresado))
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
                }

                // =========================================================
                // VALIDACIÓN DE TELÉFONO (EXACTAMENTE 10 DÍGITOS)
                // =========================================================
                long telefonoEmpleado = 0;
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
                    if (!long.TryParse(telTexto, out telefonoEmpleado))
                    {
                        MessageBox.Show("El teléfono debe contener solo números enteros.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTelefono.Focus();
                        return;
                    }
                }
                // =========================================================



                // 1. Recoger datos de la UI
                var nuevoEmpleado = new Empleado
                {
                    fcNombre = txtNombre.Text.Trim(),
                    fcApePat = txtApePat.Text.Trim(),
                    fcApeMat = txtApeMat.Text.Trim(),
                    fiTelefono = telefonoEmpleado,
                    fcDireccion = txtDireccion.Text.Trim(),
                    fcEmail = txtEmail.Text.Trim(),
                    fdFechaNac = dtpFechaNac.Value,
                    fiPuesto = Convert.ToInt32(cmbPuesto.SelectedValue)
                };

                // 2. Recoger credenciales si el puesto lo requiere
                if (pnlAcceso.Visible)
                {
                    nuevoEmpleado.fcUsuario = txtUsuario.Text.Trim();
                    nuevoEmpleado.fcPassword = txtPassword.Text.Trim();

                    // Validación extra: no dejar que guarden vacíos si requieren acceso
                    if (string.IsNullOrEmpty(nuevoEmpleado.fcUsuario) || string.IsNullOrEmpty(nuevoEmpleado.fcPassword))
                    {
                        MessageBox.Show("El usuario y contraseña son obligatorios para este puesto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
            


                var bll = new EmpleadoBLL();

                // 2. ⚡ SELECCIÓN DE ACCIÓN SEGÚN EL CONTEXTO ⚡
                if (_idEmpleadoEditando.HasValue)
                {
                    // MODO EDICIÓN: Inyectamos el ID que recuperamos al dar clic en la tabla
                    nuevoEmpleado.fiEmpleado = _idEmpleadoEditando.Value;

                    if (bll.EditarEmpleado(nuevoEmpleado)) // <-- Ahora sí llama a Editar
                    {
                        MessageBox.Show("Los datos del empleado se actualizaron correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK; // Indica a la pantalla principal que debe refrescar el grid
                        this.Close();
                    }
                }
                else
                {

                    int idPuesto = Convert.ToInt32(cmbPuesto.SelectedValue);
                    bool requiereAcceso = (idPuesto == 1 || idPuesto == 2 || idPuesto == 10 || idPuesto == 3);
                    // MODO NUEVO REGISTRO

                    if (requiereAcceso)
                    {
                        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                        {
                            MessageBox.Show("Debe ingresar un nombre de usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUsuario.Focus();
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            MessageBox.Show("Debe ingresar una contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPassword.Focus();
                            return;
                        }

                        nuevoEmpleado.fcUsuario = txtUsuario.Text.Trim();
                        nuevoEmpleado.fcPassword = txtPassword.Text.Trim();
                    }

                    if (bll.RegistrarNuevo(nuevoEmpleado))
                    {
                        MessageBox.Show("Empleado registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string HashPassword(string password)
        {
            // Implementa tu método de hashing aquí
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void FrmNuevoEmpleado_Load(object sender, EventArgs e)
        {
            //CargarComboPuestos(); // Tu método existente para llenar los puestos
            //CargarPuestos();
            // Si tenemos un ID, significa que es Modo Edición
            if (_idEmpleadoEditando.HasValue)
            {
                this.Text = "Modificar Datos del Empleado";
                lblTitulo.Text = "✏️ Editar Empleado";
                btnGuardar.Text = "💾 Actualizar";

                // ── NUEVA LÓGICA DE CORRECCIÓN VISUAL PARA EDICIÓN ──
                lblPuesto.Visible = false;
                cmbPuesto.Visible = false;
                pnlAcceso.Visible = false;

                // Movemos los botones arriba (Y=430) justo debajo de los calendarios de fechas
                btnCancelar.Location = new Point(20, 430);
                btnGuardar.Location = new Point(240, 430);

                // Reducimos el tamaño de la ventana para que luzca compacta y estética
                this.ClientSize = new Size(480, 510);

                CargarDatosParaEditar(_idEmpleadoEditando.Value);
            }

        }

        private void CargarDatosParaEditar(int id)
        {
            try
            {
                var bll = new EmpleadoBLL();
                Empleado emp = bll.ObtenerPorId(id);

                if (emp != null)
                {
                    txtNombre.Text = emp.fcNombre;
                    txtApePat.Text = emp.fcApePat;
                    txtApeMat.Text = emp.fcApeMat;
                    dtpFechaNac.Value = emp.fdFechaNac;
                    //dtpFechaReg.Value = emp.fdFechaReg;
                    txtDireccion.Text = emp.fcDireccion;
                    txtTelefono.Text = emp.fiTelefono == 0 ? "" : emp.fiTelefono.ToString();
                    txtEmail.Text = emp.fcEmail;

                    // El valor se selecciona en segundo plano para que el objeto mapee correctamente al guardar
                    cmbPuesto.SelectedValue = emp.fiPuesto;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del empleado: " + ex.Message);
            }
        }
    }
}