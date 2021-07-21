using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Proyecto_Inventario
{
    public partial class Login : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        public Login()
        {
            InitializeComponent();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            if(txtUser.Text == "" || txtPass.Text == "")
            {
                btnAcceder.Enabled = false;
            }
            else if(txtUser.Text != "" && txtPass.Text != "")
            {
                btnAcceder.Enabled = true;
            }
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            if (txtUser.Text == "" || txtPass.Text == "")
            {
                btnAcceder.Enabled = false;
            }
            else if (txtUser.Text != "" && txtPass.Text != "")
            {
                btnAcceder.Enabled = true;
            }
        }

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text;
            //string pass = txtPass.Text;
            string pass = Hash.obtenerHash256(txtPass.Text); ;
            
            var tUsuarios = entitiesFact.Empleados.FirstOrDefault(x => x.NombreUsuario == user && x.Contrasena == pass);
            
            if(tUsuarios == null)
            {
                MessageBox.Show("Usuario o contraseña incorrecta.");
                txtPass.Text = "";
                return;
            }
            else
            {
                switch (tUsuarios.FKRango)
                {
                    case 1:
                        MenuSU menuSU = new MenuSU(tUsuarios.PKEmpleadoID, tUsuarios.FKRango);
                        this.Hide();
                        menuSU.Show();
                        return;
                    case 2:
                        MenuGerente menuGerente = new MenuGerente(tUsuarios.PKEmpleadoID, tUsuarios.FKRango);
                        this.Hide();
                        menuGerente.Show();
                        return;
                    case 3:
                        MenuAdmin menuAdmin = new MenuAdmin(tUsuarios.PKEmpleadoID, tUsuarios.FKRango);
                        this.Hide();
                        menuAdmin.Show();
                        return;
                    case 4:
                        MenuCompra menuCompra = new MenuCompra(tUsuarios.PKEmpleadoID, tUsuarios.FKRango);
                        this.Hide();
                        menuCompra.Show();
                        return;
                    case 5:
                        MenuVenta menuVenta = new MenuVenta(tUsuarios.PKEmpleadoID, tUsuarios.FKRango);
                        this.Hide();
                        menuVenta.Show();
                        return;
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            btnAcceder.Enabled = false;
            //MessageBox.Show("El menú principal no está finalizado :c , pero puede revisar los mantenimientos individualmente.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public class Hash
    {
        public static string obtenerHash256(string text)
        {

            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashString = new SHA256Managed();

            byte[] hash = hashString.ComputeHash(bytes);
            string hashStr = string.Empty;

            foreach (byte x in hash)
            {
                hashStr += String.Format("{0:x2}", x);
            }

            return hashStr;

        }
    }
}
