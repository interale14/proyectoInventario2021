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
    public partial class MNT_ComprasDevoluciones : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        DataTable dtDevolucion = new DataTable();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        public MNT_ComprasDevoluciones(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_ComprasDevoluciones_Load(object sender, EventArgs e)
        {
            lblfecha.Text = DateTime.Now.ToShortDateString();

            var tcmbCompra = from c in entitiesFact.Compras
                            select new
                            {
                                c.PKCompraID,
                            };

            dtDevolucion = tcmbCompra.CopyAnonymusToDataTable();
            cmbCompra.DataSource = dtDevolucion;
            cmbCompra.DisplayMember = dtDevolucion.Columns[0].ColumnName;
            cmbCompra.ValueMember = dtDevolucion.Columns[0].ColumnName;

            cmbCompra.Text = "";
            cmbCompra.SelectedIndex = -1;
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            ContDevoluciones form = new ContDevoluciones(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void cmbCompra_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                long compra = Convert.ToInt32(cmbCompra.SelectedValue);
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
                                 vd.Estatus
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

        private void btnCompra_Click(object sender, EventArgs e)
        {
            if (dgvDevoluciones.SelectedRows.Count > 0)
            {

                int indice = dgvDevoluciones.CurrentCell.RowIndex;
                long idproducto = Convert.ToInt64(dgvDevoluciones.Rows[indice].Cells[0].Value.ToString());
                string nombreProducto = dgvDevoluciones.Rows[indice].Cells[1].Value.ToString();
                string estatus = "Comprado"; 

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
            long idCompra = Convert.ToInt64(cmbCompra.SelectedValue);
            if (dgvDevoluciones.SelectedRows.Count > 0)
            {
                Compras_Devoluciones tDevolucion = new Compras_Devoluciones();
                tDevolucion.Estado = true;
                tDevolucion.FKCompraID = idCompra;
                tDevolucion.FechaDevolucion = DateTime.Now;

                entitiesFact.Compras_Devoluciones.Add(tDevolucion);
                entitiesFact.SaveChanges();

                Compras_Detalles VentaDet = new Compras_Detalles();
                Productos producto = new Productos();

                foreach (DataGridViewRow dr in dgvDevoluciones.Rows)
                {
                    int idProd = Convert.ToInt32(dr.Cells[0].Value);

                    //Crea el registro de la devolucion detalle
                    Compras_Devoluciones_Detalles tDet = new Compras_Devoluciones_Detalles();
                    tDet.FKDevolucionID = tDevolucion.PKCompraDevolucionID;
                    tDet.FKProductosID = idProd;
                    entitiesFact.Compras_Devoluciones_Detalles.Add(tDet);
                    entitiesFact.SaveChanges();

                    //Devuelve los productos
                    var CompDetDevolucion = entitiesFact.Compras_Detalles.FirstOrDefault(x => x.FKProductoID == idProd && x.FKCompraID == idCompra);
                    var ProductoDevuelto = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProd);
                    CompDetDevolucion.Estatus = "Devuelto";
                    ProductoDevuelto.Existencia -= CompDetDevolucion.Cantidad;
                    entitiesFact.SaveChanges();
                }

                MessageBox.Show("Devolución exitosa!");
                for (int x = 0; x < dgvDevoluciones.Rows.Count; x++)
                {
                    dgvDevoluciones.Rows.RemoveAt(x);
                }

                long compra = Convert.ToInt32(cmbCompra.SelectedValue);
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
                                 vd.Estatus
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
            MNT_ComprasDevolucionesResultados form = new MNT_ComprasDevolucionesResultados(back, idUsuario, rango);
            this.Hide();
            form.Show();
        }
    }
}
