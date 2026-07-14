using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GYMMTZ
{
    public partial class FrmMensaje : Form
    {
        public FrmMensaje(string mensaje)
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(350, 150);
            this.StartPosition = FormStartPosition.CenterParent;

            Label lbl = new Label() { Text = mensaje, ForeColor = Color.White, Location = new Point(20, 30), AutoSize = true };
            this.Controls.Add(lbl);

            Button btnAceptar = new Button() { Text = "Aceptar", Location = new Point(130, 90), BackColor = Color.FromArgb(255, 69, 0), FlatStyle = FlatStyle.Flat };
            btnAceptar.Click += (s, e) => this.Close();
            this.Controls.Add(btnAceptar);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMensaje));
            this.SuspendLayout();
            // 
            // FrmMensaje
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMensaje";
            this.Load += new System.EventHandler(this.FrmMensaje_Load);
            this.ResumeLayout(false);

        }

        private void FrmMensaje_Load(object sender, EventArgs e)
        {

        }
    }
}
