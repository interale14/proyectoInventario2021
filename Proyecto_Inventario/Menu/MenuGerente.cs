using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Inventario
{
    public partial class MenuGerente : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 2;
        long idUsuario = 0;
        int rango = 0;
        public MenuGerente(long _idUsuario, int _rango)
        {
            InitializeComponent();
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            var Empleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);
            lblUser.Text = Empleados.NombreEmpleado + " " + Empleados.ApellidoEmpleado;
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            MNT_Empleados form = new MNT_Empleados(back, idUsuario, rango);
            this.Hide();
            form.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Dispose();
            login.Show();
        }
    }
}
