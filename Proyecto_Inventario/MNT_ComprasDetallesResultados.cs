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
    public partial class MNT_ComprasDetallesResultados : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idUsuario = 0;
        int rango = 0;
        int back = 0;
        public MNT_ComprasDetallesResultados(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_ComprasDetallesResultados_Load(object sender, EventArgs e)
        {
            var tVentas = from c in entitiesFact.Compras
                          select new
                          {
                              c.PKCompraID,
                          };

            DataTable dtVentas = tVentas.CopyAnonymusToDataTable();
            cmbVenta.DataSource = dtVentas;
            cmbVenta.DisplayMember = dtVentas.Columns[0].ColumnName;
            cmbVenta.ValueMember = dtVentas.Columns[0].ColumnName;

            cmbVenta.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MNT_Compras form = new MNT_Compras(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void cmbVenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                long compra = Convert.ToInt32(cmbVenta.SelectedValue);
                var tVenta = from v in entitiesFact.Compras
                             join vd in entitiesFact.Compras_Detalles
                             on v.PKCompraID equals vd.FKCompraID
                             join p in entitiesFact.Productos
                             on vd.FKProductoID equals p.PKProductoID
                             where v.PKCompraID == compra
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 vd.Estatus,
                                 vd.PrecioUnidad,
                                 vd.Cantidad,
                                 vd.TotalProducto
                             };

                dgvProductos.DataSource = tVenta.CopyAnonymusToDataTable();
                dgvProductos.Columns[0].HeaderCell.Value = "ID";
                dgvProductos.Columns[1].HeaderCell.Value = "Producto";
                dgvProductos.Columns[2].HeaderCell.Value = "Estatus";
                dgvProductos.Columns[3].HeaderCell.Value = "Precio";
                dgvProductos.Columns[4].HeaderCell.Value = "Cantidad";
                dgvProductos.Columns[5].HeaderCell.Value = "Total";
                dgvProductos.AutoResizeColumns();

                var fechaCompra = entitiesFact.Compras.FirstOrDefault(x => x.PKCompraID == compra);
                if (cmbVenta.SelectedIndex != -1)
                {
                    lblfecha.Text = fechaCompra.Fecha.ToShortDateString();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
