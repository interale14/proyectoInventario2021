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
    public partial class ContDevoluciones : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        public ContDevoluciones(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void ContDevoluciones_Load(object sender, EventArgs e)
        {
            var TEmpleado = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);

            if(TEmpleado.FKRango == 4)
            {
                btnVentas.Enabled = false;
            }
            else if (TEmpleado.FKRango == 5)
            {
                btnCompras.Enabled = false;
            }
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            switch (back)
            {
                case 1:
                    MenuSU form1 = new MenuSU(idUsuario, rango);
                    this.Dispose();
                    form1.Show();
                    return;
                case 3:
                    MenuAdmin form3 = new MenuAdmin(idUsuario, rango);
                    this.Dispose();
                    form3.Show();
                    return;
                case 4:
                    MenuCompra form4 = new MenuCompra(idUsuario, rango);
                    this.Dispose();
                    form4.Show();
                    return;
                case 5:
                    MenuVenta form5 = new MenuVenta(idUsuario, rango);
                    this.Dispose();
                    form5.Show();
                    return;
            }
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            MNT_ComprasDevoluciones form = new MNT_ComprasDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            MNT_VentasDevoluciones form = new MNT_VentasDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }
    }
}
