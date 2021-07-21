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
    public partial class MNT_VentasDevoluciones : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        DataTable dtDevolucion = new DataTable();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        public MNT_VentasDevoluciones(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_VentasDevoluciones_Load(object sender, EventArgs e)
        {
            lblfecha.Text = DateTime.Now.ToShortDateString();

            var tcmbVenta = from c in entitiesFact.Ventas
                          select new
                          {
                              c.PKVentaID,
                          };

            dtDevolucion = tcmbVenta.CopyAnonymusToDataTable();
            cmbVenta.DataSource = dtDevolucion;
            cmbVenta.DisplayMember = dtDevolucion.Columns[0].ColumnName;
            cmbVenta.ValueMember = dtDevolucion.Columns[0].ColumnName;

            cmbVenta.Text = "";
            cmbVenta.SelectedIndex = -1;
        }

        private void cmbVentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                long venta = Convert.ToInt32(cmbVenta.SelectedValue);
                var tVenta = from v in entitiesFact.Ventas
                             join vd in entitiesFact.Ventas_Detalles
                             on v.PKVentaID equals vd.FKVentaID
                             join p in entitiesFact.Productos
                             on vd.FKProductoID equals p.PKProductoID
                             where v.PKVentaID == venta
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 vd.Estatus,
                             };

                dgvProductos.DataSource = tVenta.CopyAnonymusToDataTable();
                dgvProductos.Columns[0].HeaderCell.Value = "ID";
                dgvProductos.Columns[1].HeaderCell.Value = "Producto";
                dgvProductos.Columns[2].HeaderCell.Value = "Estatus";
                dgvProductos.AutoResizeColumns();
            }
            catch (Exception)
            {

            }
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            ContDevoluciones form = new ContDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                string estatus = dgvProductos.CurrentRow.Cells[2].Value.ToString();

                if (estatus != "Devuelto")
                {
                    int indice = dgvProductos.CurrentCell.RowIndex;
                    long idproducto = Convert.ToInt64(dgvProductos.Rows[indice].Cells[0].Value.ToString());
                    string nombreProducto = dgvProductos.Rows[indice].Cells[1].Value.ToString();

                    dgvDevoluciones.Rows.Add(idproducto.ToString(), nombreProducto);

                    dtDevolucion = (DataTable)dgvProductos.DataSource;
                    dtDevolucion.Rows.RemoveAt(indice);
                    dgvProductos.DataSource = dtDevolucion;
                }
                else
                {
                    MessageBox.Show("No puede devolver un producto ya devuelto.");
                }
            }
            else
            {
                MessageBox.Show("Seleccione el producto a devolver.");
            }
        }

        private void btnVenta_Click(object sender, EventArgs e)
        {
            if (dgvDevoluciones.SelectedRows.Count > 0)
            {

                int indice = dgvDevoluciones.CurrentCell.RowIndex;
                long idproducto = Convert.ToInt64(dgvDevoluciones.Rows[indice].Cells[0].Value.ToString());
                string nombreProducto = dgvDevoluciones.Rows[indice].Cells[1].Value.ToString();
                string estatus = "Vendido";

                dtDevolucion.Rows.Add(idproducto.ToString(), nombreProducto, estatus);

                dgvProductos.DataSource = dtDevolucion;
                dgvDevoluciones.Rows.RemoveAt(indice);
            }
            else
            {
                MessageBox.Show("Seleccione el producto a remover.");
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            long idVenta = Convert.ToInt64(cmbVenta.SelectedValue);
            if (dgvDevoluciones.SelectedRows.Count > 0)
            {
                Ventas_Devoluciones tDevolucion = new Ventas_Devoluciones();
                tDevolucion.Estado = true;
                tDevolucion.FKVentaID = idVenta;
                tDevolucion.FechaDevolucion = DateTime.Now;

                entitiesFact.Ventas_Devoluciones.Add(tDevolucion);
                entitiesFact.SaveChanges();

                Ventas_Detalles VentaDet = new Ventas_Detalles();
                Productos producto = new Productos();

                foreach (DataGridViewRow dr in dgvDevoluciones.Rows)
                {
                    int idProd = Convert.ToInt32(dr.Cells[0].Value);

                    Ventas_Devoluciones_Detalles tDet = new Ventas_Devoluciones_Detalles();
                    tDet.FKDevolucionID = tDevolucion.PKVentaDevolucionID;
                    tDet.FKProductosID = idProd;
                    entitiesFact.Ventas_Devoluciones_Detalles.Add(tDet);

                    var ProdDevolucion = entitiesFact.Ventas_Detalles.FirstOrDefault(x => x.FKProductoID == idProd && x.FKVentaID == idVenta);
                    var VentaDevolucion = entitiesFact.Ventas_Detalles.FirstOrDefault(x => x.PKVentaDetalleID == ProdDevolucion.PKVentaDetalleID);
                    VentaDevolucion.Estatus = "Devuelto";

                    if (VentaDevolucion.Estatus == "Devuelto")
                    {
                        var ProductoDevuelto = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProd);
                        ProductoDevuelto.Existencia += VentaDevolucion.Cantidad;
                    }
                    entitiesFact.SaveChanges();
                }

                MessageBox.Show("Devolución exitosa!");
                for (int x = 0; x < dgvDevoluciones.Rows.Count; x++)
                {
                    dgvDevoluciones.Rows.RemoveAt(x);
                }

                long venta = Convert.ToInt32(cmbVenta.SelectedValue);
                var tVenta = from v in entitiesFact.Ventas
                             join vd in entitiesFact.Ventas_Detalles
                             on v.PKVentaID equals vd.FKVentaID
                             join p in entitiesFact.Productos
                             on vd.FKProductoID equals p.PKProductoID
                             where v.PKVentaID == venta
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 vd.Estatus,
                             };

                dgvProductos.DataSource = tVenta.CopyAnonymusToDataTable();
                dgvProductos.Columns[0].HeaderCell.Value = "ID";
                dgvProductos.Columns[1].HeaderCell.Value = "Producto";
                dgvProductos.Columns[2].HeaderCell.Value = "Estatus";
                dgvProductos.AutoResizeColumns();
            }
            else
            {
                MessageBox.Show("Seleccione al menos un producto a devolver.");
            }
        }

        private void btnDetalles_Click(object sender, EventArgs e)
        {
            MNT_VentasDevolucionesResultados form = new MNT_VentasDevolucionesResultados(back, idUsuario, rango);
            this.Hide();
            form.Show();
        }
    }
}
