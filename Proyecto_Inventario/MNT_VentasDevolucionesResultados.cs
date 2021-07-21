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
    public partial class MNT_VentasDevolucionesResultados : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idUsuario = 0;
        int rango = 0;
        int back = 0;
        public MNT_VentasDevolucionesResultados(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_VentasDevolucionesResultados_Load(object sender, EventArgs e)
        {
            var tDevolucion = from d in entitiesFact.Ventas_Devoluciones
                              join dd in entitiesFact.Ventas_Devoluciones_Detalles
                              on d.PKVentaDevolucionID equals dd.FKDevolucionID
                              join p in entitiesFact.Productos
                              on dd.FKProductosID equals p.PKProductoID
                              select new
                              {
                                  p.PKProductoID,
                                  p.DescProducto,
                                  d.FKVentaID,
                                  d.FechaDevolucion,
                              };

            dgvProductos.DataSource = tDevolucion.CopyAnonymusToDataTable();
            dgvProductos.Columns[0].HeaderCell.Value = "ID";
            dgvProductos.Columns[1].HeaderCell.Value = "Producto";
            dgvProductos.Columns[2].HeaderCell.Value = "Venta #";
            dgvProductos.Columns[3].HeaderCell.Value = "Fecha devolución";
            dgvProductos.AutoResizeColumns();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MNT_VentasDevoluciones form = new MNT_VentasDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }
    }
}
