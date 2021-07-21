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
    public partial class MenuCompra : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idUsuario = 0;
        int back = 4;
        int backMenu = 0;
        int rango = 0;
        int compra = 0;
        public MenuCompra(long _idUsuario, int _rango)
        {
            InitializeComponent();
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MenuCompra_Load(object sender, EventArgs e)
        {
            var Empleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);
            lblUser.Text = Empleados.NombreEmpleado + " " + Empleados.ApellidoEmpleado;
            lblFecha.Text = DateTime.Now.ToShortDateString();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Dispose();
            login.Show();
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            MNT_Compras compras = new MNT_Compras(back, idUsuario, rango);
            this.Hide();
            compras.Show();
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            MNT_Proveedores proveedores = new MNT_Proveedores(back, idUsuario, rango);
            this.Hide();
            proveedores.Show();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            MNT_Productos productos = new MNT_Productos(compra, backMenu, back, idUsuario, rango);
            this.Hide();
            productos.Show();
        }

        private void btnDevoluciones_Click(object sender, EventArgs e)
        {
            ContDevoluciones devoluciones = new ContDevoluciones(back, idUsuario, rango);
            this.Hide();
            devoluciones.Show();
        }
    }
}
