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
    public partial class MNT_ComprasDevolucionesResultados : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idUsuario = 0;
        int rango = 0;
        int back = 0;
        public MNT_ComprasDevolucionesResultados(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_ComprasDevolucionesResultados_Load(object sender, EventArgs e)
        {
            var tDevolucion = from d in entitiesFact.Compras_Devoluciones
                              join dd in entitiesFact.Compras_Devoluciones_Detalles
                              on d.PKCompraDevolucionID equals dd.FKDevolucionID
                              join p in entitiesFact.Productos
                              on dd.FKProductosID equals p.PKProductoID
                         select new
                         {
                             p.PKProductoID,
                             p.DescProducto,
                             d.FKCompraID,
                             d.FechaDevolucion,
                         };

            dgvProductos.DataSource = tDevolucion.CopyAnonymusToDataTable();
            dgvProductos.Columns[0].HeaderCell.Value = "ID";
            dgvProductos.Columns[1].HeaderCell.Value = "Producto";
            dgvProductos.Columns[2].HeaderCell.Value = "Compra #";
            dgvProductos.Columns[3].HeaderCell.Value = "Fecha devolución";
            dgvProductos.AutoResizeColumns();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MNT_ComprasDevoluciones form = new MNT_ComprasDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }
    }
}
