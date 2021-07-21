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
    public partial class MNT_VentasDetallesResultados : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idUsuario = 0;
        int rango = 0;
        int back = 0;
        public MNT_VentasDetallesResultados(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_VentasDetallesResultados_Load(object sender, EventArgs e)
        {
            var tVentas = from c in entitiesFact.Ventas
                          select new
                          {
                              c.PKVentaID,
                          };

            DataTable dtVentas = tVentas.CopyAnonymusToDataTable();
            cmbVenta.DataSource = dtVentas;
            cmbVenta.DisplayMember = dtVentas.Columns[0].ColumnName;
            cmbVenta.ValueMember = dtVentas.Columns[0].ColumnName;

            cmbVenta.SelectedIndex = -1;
        }

        private void cmbVenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                long venta = Convert.ToInt32(cmbVenta.SelectedValue);
                var tVenta = from v in entitiesFact.Ventas
                             join vd in entitiesFact.Ventas_Detalles
                             on v.PKVentaID equals vd.FKVentaID
                             join p in entitiesFact.Productos
                             on vd.FKProductoID equals p.PKProductoID
                             join d in entitiesFact.Descuentos
                             on vd.FKDescuentoID equals d.PKDescuentoID
                             where v.PKVentaID == venta
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 vd.Estatus,
                                 vd.PrecioUnidad,
                                 vd.Cantidad,
                                 d.NombreDescuento,
                                 vd.TotalProducto
                             };

                dgvProductos.DataSource = tVenta.CopyAnonymusToDataTable();
                dgvProductos.Columns[0].HeaderCell.Value = "ID";
                dgvProductos.Columns[1].HeaderCell.Value = "Producto";
                dgvProductos.Columns[2].HeaderCell.Value = "Estatus";
                dgvProductos.Columns[3].HeaderCell.Value = "Precio";
                dgvProductos.Columns[4].HeaderCell.Value = "Cantidad";
                dgvProductos.Columns[5].HeaderCell.Value = "Descuento";
                dgvProductos.Columns[6].HeaderCell.Value = "Total del producto";
                dgvProductos.AutoResizeColumns();

                var fechaVenta = entitiesFact.Ventas.FirstOrDefault(x => x.PKVentaID == venta);
                if (cmbVenta.SelectedIndex != -1)
                {
                    lblfecha.Text = fechaVenta.Fecha.ToShortDateString();
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MNT_Ventas form = new MNT_Ventas(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
