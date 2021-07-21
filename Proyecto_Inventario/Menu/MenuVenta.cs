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
    public partial class MenuVenta : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int backMenu = 0;
        int back = 5;
        long idUsuario = 0;
        int rango = 0;
        int compra = 0;
        public MenuVenta(long _idUsuario, int _rango)
        {
            InitializeComponent();
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MenuVenta_Load(object sender, EventArgs e)
        {
            var Empleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);
            lblUser.Text = Empleados.NombreEmpleado + " " + Empleados.ApellidoEmpleado;
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            MNT_Ventas ventas = new MNT_Ventas(back, idUsuario, rango);
            this.Hide();
            ventas.Show();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            MNT_Productos productos = new MNT_Productos(compra, backMenu, back, idUsuario, rango);
            this.Hide();
            productos.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Dispose();
            login.Show();
        }

        private void btnDevoluciones_Click(object sender, EventArgs e)
        {
            ContDevoluciones devoluciones = new ContDevoluciones(back, idUsuario, rango);
            this.Hide();
            devoluciones.Show();
        }
    }
}
