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
    public partial class MNT_VentasDetalles : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        int venta = 0;
        long idProducto = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_VentasDetalles(int _venta, int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
            venta = _venta;
        }

        private void MNT_VentasDetalles_Load(object sender, EventArgs e)
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

            lblVenta.Text = venta.ToString();

            var tDescuentos = from em in entitiesFact.Descuentos
                          select new
                          {
                              em.PKDescuentoID,
                              em.NombreDescuento
                          };

            DataTable dtDescuentos = tDescuentos.CopyAnonymusToDataTable();
            cmbDescuento.Items.Add("Ninguno");
            cmbDescuento.DataSource = dtDescuentos;
            cmbDescuento.DisplayMember = dtDescuentos.Columns[1].ColumnName;
            cmbDescuento.ValueMember = dtDescuentos.Columns[0].ColumnName;

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
                    var tVentasDetalles = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);

                    txtProducto.Text = tVentasDetalles.DescProducto;
                    txtPrecio.Text = (tVentasDetalles.PrecioUnidadVenta).ToString();
                }
                catch (Exception)
                {

                }
                editar = true;
                txtCantidad.Text = "";
                txtSubT.Text = "";
                txtDescuento.Text = "";
                txtISV.Text = "";
                txtTotal.Text = "";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtProducto.Text == "" && txtPrecio.Text == "" && txtCantidad.Text == "" && cmbDescuento.Text == "")
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }
            else
            {
                var thProductos = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);

                if (thProductos.Existencia > thProductos.ExistenciaMinima)
                {
                    if (thProductos.Existencia > Convert.ToInt32(txtCantidad.Text))
                    {
                        //Guardado de venta detalle
                        Ventas_Detalles tbVentas = new Ventas_Detalles();

                        int cantidad = Convert.ToInt32(txtCantidad.Text);
                        double precio = Convert.ToDouble(txtPrecio.Text);

                        tbVentas.FKVentaID = Convert.ToInt64(lblVenta.Text);
                        tbVentas.FKProductoID = idProducto;
                        tbVentas.Estatus = "Vendido";
                        tbVentas.PrecioUnidad = Convert.ToDecimal(precio);
                        tbVentas.Cantidad = cantidad;
                        tbVentas.FKDescuentoID = Convert.ToByte(cmbDescuento.SelectedIndex + 1);
                        tbVentas.TotalProducto = Convert.ToDecimal(txtTotal.Text);

                        entitiesFact.Ventas_Detalles.Add(tbVentas);
                        entitiesFact.SaveChanges();

                        //Actualización del producto
                        thProductos.Existencia -= cantidad;

                        entitiesFact.SaveChanges();

                        txtProducto.Text = "";
                        txtPrecio.Text = "";
                        txtCantidad.Text = "";
                        txtSubT.Text = "";
                        txtDescuento.Text = "";
                        txtISV.Text = "";
                        txtTotal.Text = "";
                        idProducto = 0;
                        editar = false;

                        MessageBox.Show("Informacion guardada!");
                        if (thProductos.Existencia <= (thProductos.ExistenciaMinima + 5))
                        {
                            MessageBox.Show("Se está aproximando a la existencia mínima de este producto.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("La cantidad de la venta no puede ser igual o superior a la existencia de este producto. Actualmente se cuenta con " + thProductos.Existencia + " existencias.");
                    }
                }
                else
                {
                    MessageBox.Show("Ha alcanzado la existencia mínima de este producto. Venta bloqueada por seguridad.");
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtProducto.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "";
            txtSubT.Text = "";
            txtDescuento.Text = "";
            txtISV.Text = "";
            txtTotal.Text = "";
            cmbDescuento.SelectedIndex = 0;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
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

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            if (txtProducto.Text == "" || txtPrecio.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;

                txtSubT.Text = "";
                txtDescuento.Text = "";
                txtISV.Text = "";
                txtTotal.Text = "";
            }
            else if (txtProducto.Text != "" && txtPrecio.Text != "" && txtCantidad.Text != "")
            {
                btnGuardar.Enabled = true;

                var tISV= entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);
                var tDescuento = entitiesFact.Descuentos.FirstOrDefault(x => x.NombreDescuento == cmbDescuento.Text);

                int cantidad = Convert.ToInt32(txtCantidad.Text);



                double precio = Convert.ToDouble(txtPrecio.Text);
                double subtotal = Convert.ToDouble(cantidad) * Convert.ToDouble(precio);
                double isv = 0;
                double descuento = Convert.ToDouble(tDescuento.CantidadDescuento);
                double descuentoAplicado = 0;
                double total = 0;
                txtSubT.Text = (subtotal).ToString();

                switch (tISV.ISV)
                {
                    case "Excento":
                        isv = 0;
                        if (txtPrecio.Text != "") { txtISV.Text = "0"; }
                        total = subtotal + isv;
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                    case "15%":
                        isv = 0.15;
                        if (txtPrecio.Text != "") { txtISV.Text = ((precio * isv) * Convert.ToDouble(cantidad)).ToString(); }
                        total = subtotal + Convert.ToDouble(txtISV.Text);
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                    case "18%":
                        isv = 0.18;
                        if (txtPrecio.Text != "") { txtISV.Text = ((precio * isv) * Convert.ToDouble(cantidad)).ToString(); }
                        total = subtotal + Convert.ToDouble(txtISV.Text);
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            var VentaCompleta = entitiesFact.Ventas.First(x => x.PKVentaID == venta);
            VentaCompleta.Estatus = "Completo";
            entitiesFact.SaveChanges();

            MNT_Ventas form = new MNT_Ventas(back, idUsuario, rango);
            this.Dispose();
            form.Show();
        }

        private void cmbDescuento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtProducto.Text != "" && txtPrecio.Text != "" && txtCantidad.Text != "")
            {
                var tISV = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);
                var tDescuento = entitiesFact.Descuentos.FirstOrDefault(x => x.NombreDescuento == cmbDescuento.Text);

                int cantidad = Convert.ToInt32(txtCantidad.Text);
                double precio = Convert.ToDouble(txtPrecio.Text);
                double subtotal = Convert.ToDouble(cantidad) * Convert.ToDouble(precio);
                double isv = 0;
                double descuento = Convert.ToDouble(tDescuento.CantidadDescuento);
                double descuentoAplicado = 0;
                double total = 0;
                txtSubT.Text = (subtotal).ToString();

                switch (tISV.ISV)
                {
                    case "Excento":
                        isv = 0;
                        if (txtPrecio.Text != "") { txtISV.Text = "0"; }
                        total = subtotal + isv;
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                    case "15%":
                        isv = 0.15;
                        if (txtPrecio.Text != "") { txtISV.Text = ((precio * isv) * Convert.ToDouble(cantidad)).ToString(); }
                        total = subtotal + Convert.ToDouble(txtISV.Text);
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                    case "18%":
                        isv = 0.18;
                        if (txtPrecio.Text != "") { txtISV.Text = ((precio * isv) * Convert.ToDouble(cantidad)).ToString(); }
                        total = subtotal + Convert.ToDouble(txtISV.Text);
                        descuentoAplicado = total * descuento;
                        txtDescuento.Text = (descuentoAplicado).ToString();
                        txtTotal.Text = (total - descuentoAplicado).ToString();
                        return;
                }
            }
        }
    }
}
