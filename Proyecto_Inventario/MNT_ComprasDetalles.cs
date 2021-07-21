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
    public partial class MNT_ComprasDetalles : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 6;
        int backMenu = 0;
        long idUsuario = 0;
        int rango = 0;
        int compra = 0;

        long idProducto = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_ComprasDetalles(int _compra, int _backMenu, int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            backMenu = _backMenu;
            idUsuario = _idUsuario;
            rango = _rango;
            compra = _compra;
        }

        private void MNT_ComprasDetalles_Load(object sender, EventArgs e)
        {
            var tProductos = from p in entitiesFact.Productos
                             join c in entitiesFact.Productos_Categorias
                             on p.FKCategoria equals c.PKCategoriaID
                             where p.Estado == true
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                             };

            dgvProductos.DataSource = tProductos.CopyAnonymusToDataTable();
            dgvProductos.Columns[0].HeaderCell.Value = "ID";
            dgvProductos.Columns[1].HeaderCell.Value = "Producto";
            dgvProductos.AutoResizeColumns();

            lblCompra.Text = compra.ToString();

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Nombre producto");
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.RowCount > 0)
            {
                try
                {
                    idProducto = Convert.ToInt64(dgvProductos.SelectedCells[0].Value);
                    var tComprasDetalles = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);

                    txtProducto.Text = tComprasDetalles.DescProducto;
                    txtPrecio.Text = tComprasDetalles.PrecioUnidadCompra.ToString();
                }
                catch (Exception)
                {

                }
                editar = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtProducto.Text == "" && txtPrecio.Text == "" && txtCantidad.Text == "")
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }
            else
            {
                //Guardado de compra detalle
                Compras_Detalles tbCompras = new Compras_Detalles();

                int cantidad = Convert.ToInt32(txtCantidad.Text);
                double precio = Convert.ToDouble(txtPrecio.Text);
                int comp = Convert.ToInt32(lblCompra.Text);
                tbCompras.FKCompraID = comp;
                tbCompras.FKProductoID = idProducto;
                tbCompras.Estatus = "Comprado";
                tbCompras.PrecioUnidad = Convert.ToDecimal(precio);
                tbCompras.Cantidad = cantidad;
                tbCompras.TotalProducto = Convert.ToDecimal(cantidad * precio);

                entitiesFact.Compras_Detalles.Add(tbCompras);
                entitiesFact.SaveChanges();

                //Actualización del producto
                var thProductos = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);

                thProductos.PrecioUnidadCompra = Convert.ToDecimal(precio);
                thProductos.Existencia += cantidad;

                decimal ganancia = Convert.ToDecimal(precio * 0.2);

                if (txtPrecio.Text != "")
                {
                    thProductos.PrecioUnidadVenta = (Convert.ToDecimal(precio) + ganancia);
                }

                entitiesFact.SaveChanges();

                txtProducto.Text = "";
                txtPrecio.Text = "";
                txtCantidad.Text = "";
                txtTotal.Text = "";
                idProducto = 0;
                editar = false;

                MessageBox.Show("Informacion guardada!");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtProducto.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text;

            switch (campoBuscar)
            {
                case "ID":
                    int id = 1;
                    if (busqueda != "")
                    {
                        id = Convert.ToInt32(busqueda);
                        var tID = from d in entitiesFact.Productos
                                        where d.PKProductoID == id && d.Estado == true
                                        select new
                                        {
                                            d.PKProductoID,
                                            d.DescProducto,
                                        };
                        dgvProductos.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from d in entitiesFact.Productos
                                   where d.Estado == true
                                   select new
                                  {
                                      d.PKProductoID,
                                      d.DescProducto,
                                  };
                        dgvProductos.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Nombre producto":
                    var tNombre = from d in entitiesFact.Productos
                                    where d.DescProducto.Contains(txtBuscar.Text) && d.Estado == true
                                  select new
                                    {
                                        d.PKProductoID,
                                        d.DescProducto,
                                    };
                    dgvProductos.DataSource = tNombre.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    var tProducto = from d in entitiesFact.Productos
                                    where d.Estado == true
                                    select new
                                  {
                                      d.PKProductoID,
                                      d.DescProducto,
                                  };
                    dgvProductos.DataSource = tProducto.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                default:
                    var tProducto2 = from d in entitiesFact.Productos
                                     where d.Estado == true
                                     select new
                                    {
                                        d.PKProductoID,
                                        d.DescProducto,
                                    };
                    dgvProductos.DataSource = tProducto2.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
            }
        }

        private void cmbBuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            campoBuscar = "";
            campoBuscar = cmbBuscar.SelectedItem.ToString();
            if (campoBuscar == "Mostrar Todo") 
            { 
                txtBuscar.ReadOnly = true;
                var tProducto = from d in entitiesFact.Productos
                                where d.Estado == true
                                select new
                                {
                                    d.PKProductoID,
                                    d.DescProducto,
                                };
                dgvProductos.DataSource = tProducto.CopyAnonymusToDataTable();
                dgvProductos.AutoResizeColumns();
            } 
            else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            MNT_Productos productos = new MNT_Productos(compra, backMenu, back, idUsuario, rango);
            this.Hide();
            productos.Show();
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {
            if (txtProducto.Text == "" || txtPrecio.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtProducto.Text != "" && txtPrecio.Text != "" && txtCantidad.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            txtTotal.Text = "";
            if (txtProducto.Text == "" || txtPrecio.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtProducto.Text != "" && txtPrecio.Text != "" && txtCantidad.Text != "")
            {
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                double precio = Convert.ToDouble(txtPrecio.Text);
                btnGuardar.Enabled = true;
                txtTotal.Text = (Convert.ToDecimal(cantidad) * Convert.ToDecimal(precio)).ToString();
            }
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            txtTotal.Text = "";
            if (txtProducto.Text == "" || txtPrecio.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtProducto.Text != "" && txtPrecio.Text != "" && txtCantidad.Text != "")
            {
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                double precio = Convert.ToDouble(txtPrecio.Text);
                btnGuardar.Enabled = true;
                txtTotal.Text = (Convert.ToDecimal(cantidad) * Convert.ToDecimal(precio)).ToString();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            var CompraCompleta = entitiesFact.Compras.FirstOrDefault(x => x.PKCompraID == compra);
            CompraCompleta.Estatus = "Completo";
            entitiesFact.SaveChanges();

            MNT_Compras form = new MNT_Compras(backMenu, idUsuario, rango);
            this.Hide();
            form.Show();
        }
    }
}
