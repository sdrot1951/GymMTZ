using GymApp.BLL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmCatalogoGenerico : Form
    {
        private string _tipo; // "Gasto", "Pago", "Rubro"
        private TextBox txtDesc;

        private int _idEditar;

        // Modificamos el constructor para recibir datos opcionales
        public FrmCatalogoGenerico(string tipo, int idEditar = 0, string descripcion = "")
        {
            _tipo = tipo;
            _idEditar = idEditar;
            ConfigurarUI();

            if (_idEditar > 0)
            {
                this.Text = $"Editar {_tipo}";
                txtDesc.Text = descripcion;
            }
        }

        private void ConfigurarUI()
        {
            this.Size = new Size(400, 230);
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = $"Nuevo Catálogo: {_tipo}";

            Label lblNom = new Label { Text = $"Descripción de {_tipo}:", Location = new Point(20, 30), AutoSize = true, ForeColor = Color.DarkGray };
            txtDesc = new TextBox { Location = new Point(20, 55), Width = 340, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(lblNom); this.Controls.Add(txtDesc);

            Button btnGuardar = new Button { Text = "💾 Guardar", Location = new Point(200, 110), Size = new Size(160, 40), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var bll = new CatalogosBLL();
                if (_idEditar > 0) bll.EditarCatalogoSimple(_tipo, _idEditar, txtDesc.Text);
                else bll.GuardarCatalogoSimple(_tipo, txtDesc.Text);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmCatalogoGenerico
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FrmCatalogoGenerico";
            this.Load += new System.EventHandler(this.FrmCatalogoGenerico_Load);
            this.ResumeLayout(false);

        }

        private void FrmCatalogoGenerico_Load(object sender, EventArgs e)
        {

        }
    }
}