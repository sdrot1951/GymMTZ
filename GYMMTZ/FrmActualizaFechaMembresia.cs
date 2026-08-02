using System;
using System.Drawing;
using System.Windows.Forms;
using GymApp.BLL;
using GYMMTZ.Theme; // Importamos tu tema oscuro

namespace GYMMTZ
{
    public partial class FrmActualizaFechaMembresia : Form
    {
        private int _idCliente;
        private string _nombreCliente;
        private DateTime _fechaActual;

        private DateTimePicker _dtpNuevaFecha;

        // Constructor que recibe los datos de la fila seleccionada
        public FrmActualizaFechaMembresia(int idCliente, string nombreCliente, DateTime fechaActual)
        {
            _idCliente = idCliente;
            _nombreCliente = nombreCliente;
            _fechaActual = fechaActual;

            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            // Propiedades del formulario
            this.Text = "Modificar Fecha de Membresía";
            this.Size = new Size(450, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = GymTheme.Background;
            this.ForeColor = GymTheme.TextPrimary;

            // Panel contenedor (Simulando un GymPanel)
            Panel pnlContent = new Panel
            {
                BackColor = GymTheme.SurfaceElevated,
                Location = new Point(12, 12),
                Size = new Size(410, 255),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Título
            Label lblTitulo = new Label
            {
                Text = "Actualización Manual de Membresía",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = GymTheme.TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Cliente
            Label lblCliente = new Label
            {
                Text = "Cliente: " + _nombreCliente,
                Font = GymTheme.FontBody,
                ForeColor = GymTheme.TextSecondary,
                Location = new Point(20, 60),
                AutoSize = true
            };

            // Fecha Actual
            Label lblFechaAnterior = new Label
            {
                Text = "Vencimiento actual: " + _fechaActual.ToString("dd/MM/yyyy"),
                Font = GymTheme.FontBody,
                ForeColor = Color.Gold,
                Location = new Point(20, 90),
                AutoSize = true
            };

            // Etiqueta de nueva fecha
            Label lblNueva = new Label
            {
                Text = "Seleccione la nueva fecha de vencimiento:",
                Font = GymTheme.FontBody,
                ForeColor = GymTheme.TextPrimary,
                Location = new Point(20, 135),
                AutoSize = true
            };

            // Control de Fecha (DateTimePicker)
            _dtpNuevaFecha = new DateTimePicker
            {
                Location = new Point(20, 160),
                Width = 200,
                Font = new Font("Segoe UI", 10f),
                Format = DateTimePickerFormat.Short,
                Value = _fechaActual // Arranca en la fecha que ya tiene
            };

            // Botón Cancelar
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(160, 205),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = GymTheme.TextSecondary,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderColor = GymTheme.Border;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // Botón Guardar
            Button btnGuardar = new Button
            {
                Text = "💾 Guardar",
                Location = new Point(270, 205),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = GymTheme.Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            // Ensamblaje
            pnlContent.Controls.Add(lblTitulo);
            pnlContent.Controls.Add(lblCliente);
            pnlContent.Controls.Add(lblFechaAnterior);
            pnlContent.Controls.Add(lblNueva);
            pnlContent.Controls.Add(_dtpNuevaFecha);
            pnlContent.Controls.Add(btnCancelar);
            pnlContent.Controls.Add(btnGuardar);

            this.Controls.Add(pnlContent);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            DateTime nuevaFecha = _dtpNuevaFecha.Value.Date;

            if (nuevaFecha == _fechaActual.Date)
            {
                MessageBox.Show("La nueva fecha debe ser diferente a la actual.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"¿Estás seguro de cambiar la fecha de vencimiento a: {nuevaFecha.ToString("dd/MM/yyyy")}?\n\nEste movimiento quedará registrado en la bitácora del sistema bajo su usuario.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    MembresiaBLL bll = new MembresiaBLL();
                    // Obtenemos el nombre del administrador logueado globalmente
                    string adminActual = GymApp.Core.SesionGlobal.IdEmpleado.ToString();

                    // IMPORTANTE: Asegúrate de que el SP que te pasé antes (sp_ActualizarFechaMembresia) 
                    // esté utilizando el ID del Cliente (IdCliente) como llave (WHERE IdCliente = @IdMembresia)
                    bll.ActualizarFechaMembresia(_idCliente, nuevaFecha, adminActual);

                    MessageBox.Show("Fecha actualizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // Esto avisa al FrmMenuPrincipal que debe recargar el grid
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FrmActualizaFechaMembresia_Load(object sender, EventArgs e)
        {

        }
    }
}