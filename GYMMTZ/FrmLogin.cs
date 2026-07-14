using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GymApp.BLL;
using GymApp.Core;
using GymApp.Entities;
namespace GYMMTZ
{
    public partial class FrmLogin : Form
    {

        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnIngresar;
        private Button btnCancelar;

 
        public FrmLogin()
        {
            InitializeComponent();
            AplicarEstiloOscuro();
           

        }

    

        private void AgregarEtiquetas()
        {
            // Etiqueta Usuario
            Label lblUser = new Label()
            {
                Text = "USUARIO",
                Location = new Point(txtUser.Left, txtUser.Top - 25),
                ForeColor = Color.White, // Cambié de DarkGray a White para probar el contraste
                Font = new Font("Segoe UI", 9, FontStyle.Bold), // Subí a 9pt para mejor lectura
                AutoSize = true,
                BackColor = Color.Transparent // IMPORTANTE: Que no tengan fondo propio
            };
            this.Controls.Add(lblUser);
            lblUser.BringToFront(); // <--- ESTO FUERZA A LA ETIQUETA A DIBUJARSE ENCIMA DE TODO

            // Etiqueta Contraseña
            Label lblPass = new Label()
            {
                Text = "CONTRASEÑA",
                Location = new Point(txtPass.Left, txtPass.Top - 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblPass);
            lblPass.BringToFront(); // <--- ESTO FUERZA A LA ETIQUETA A DIBUJARSE ENCIMA DE TODO
        }
        private void AplicarEstiloOscuro()
        {
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 300); // Tamaño fijo para el login

            // Crear txtUser
            txtUser = new TextBox() { Location = new Point(100, 80), Size = new Size(200, 25) };
            EstilizarTextBox(txtUser);
            this.Controls.Add(txtUser);

            // Crear txtPass
            txtPass = new TextBox() { Location = new Point(100, 140), Size = new Size(200, 25), PasswordChar = '●' };
            EstilizarTextBox(txtPass);
            this.Controls.Add(txtPass);

            // Crear Botón
            btnIngresar = new Button() { Text = "Ingresar", Location = new Point(100, 180), Size = new Size(100, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnIngresar.Click += btnIngresar_Click; // Reconecta el evento
            this.Controls.Add(btnIngresar);

            // Crear Botón
            btnCancelar = new Button() { Text = "Cancelar", Location = new Point(210, 180), Size = new Size(100, 35), BackColor = Color.FromArgb(255, 69, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.Click += (s, e) => { this.Close(); }; // Directo y limpio
            this.Controls.Add(btnCancelar);

            // LLAMA AQUÍ A LAS ETIQUETAS
            AgregarEtiquetas();
        }

        private void EstilizarTextBox(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(40, 40, 40);
            txt.ForeColor = Color.White;
            txt.BorderStyle = BorderStyle.None;
            txt.Padding = new Padding(5);
        }
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            var bll = new GymApp.BLL.UsuarioBLL();

            // Ahora esperamos que nos devuelva al Empleado con todos sus datos
            //var empleadoLogueado = bll.ValidarLogin(txtUser.Text, txtPass.Text);
            var empleadoLogueado = bll.ValidarLogin(txtUser.Text, txtPass.Text);

            if (empleadoLogueado != null) // Si no es nulo, las credenciales son correctas
            {
                // ✨ ¡MAGIA! Guardamos los datos en la memoria RAM global
                GymApp.Core.SesionGlobal.IniciarSesion(empleadoLogueado);

                this.Hide();
                FrmMenuPrincipal menu = new FrmMenuPrincipal();

                // Cuando el menú se cierre, cerramos toda la aplicación
                menu.FormClosed += (s, args) => this.Close();
                menu.Show();
            }
            else
            {
                FrmMensaje errorMsg = new FrmMensaje("Usuario o contraseña incorrectos.");
                errorMsg.ShowDialog();
            }
        }


        private void FrmLogin_Load(object sender, EventArgs e)
        {
            //AgregarEtiquetas();
        }

        private void FrmLogin_Load_1(object sender, EventArgs e)
        {

        }
    }
}
