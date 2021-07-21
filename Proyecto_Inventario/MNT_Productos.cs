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
    public partial class MNT_Productos : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        int backMenu = 0;
        long idUsuario = 0;
        int rango = 0;
        int compra = 0;
        long idProducto = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";

        public MNT_Productos(int _compra, int _backMenu, int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            backMenu = _backMenu;
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
            compra = _compra;
        }

        private void MNT_Productos_Load(object sender, EventArgs e)
        {
            var tProductos = from p in entitiesFact.Productos
                             join c in entitiesFact.Productos_Categorias
                             on p.FKCategoria equals c.PKCategoriaID
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 p.Existencia,
                                 p.ExistenciaMinima,
                                 c.NombreCategoria,
                                 p.PrecioUnidadVenta,
                                 p.PrecioUnidadCompra,
                                 p.ISV,
                                 Est = p.Estado == true ? "Activo" : "Inactivo"
                             };

            dgvProductos.DataSource = tProductos.CopyAnonymusToDataTable();
            dgvProductos.Columns[0].HeaderCell.Value = "ID";
            dgvProductos.Columns[1].HeaderCell.Value = "Producto";
            dgvProductos.Columns[2].HeaderCell.Value = "Existencia";
            dgvProductos.Columns[3].HeaderCell.Value = "Existencia mínima";
            dgvProductos.Columns[4].HeaderCell.Value = "Categoría";
            dgvProductos.Columns[5].HeaderCell.Value = "Precio de venta";
            dgvProductos.Columns[6].HeaderCell.Value = "Precio de compra";
            dgvProductos.Columns[7].HeaderCell.Value = "ISV";
            dgvProductos.Columns[8].HeaderCell.Value = "Estado";
            dgvProductos.AutoResizeColumns();

            var tCategorias = from c in entitiesFact.Productos_Categorias
                              select new
                              {
                                  c.PKCategoriaID,
                                  c.NombreCategoria
                              };

            DataTable dtCategoria = tCategorias.CopyAnonymusToDataTable();
            cmbCat.DataSource = dtCategoria;
            cmbCat.DisplayMember = dtCategoria.Columns[1].ColumnName;
            cmbCat.ValueMember = dtCategoria.Columns[0].ColumnName;

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Nombre del producto");
            cmbBuscar.Items.Add("Categoría");
            cmbBuscar.Items.Add("ISV");
            cmbBuscar.Items.Add("Estado");

            cmbISV.Items.Add("Excento");
            cmbISV.Items.Add("15%");
            cmbISV.Items.Add("18%");

            btnCancelar.Text = "Nuevo";
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.RowCount > 0)
            {
                try
                {
                    idProducto = Convert.ToInt64(dgvProductos.SelectedCells[0].Value);
                    var tProductos = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);
                    var tCategoria = entitiesFact.Productos_Categorias.FirstOrDefault(x => x.PKCategoriaID == tProductos.FKCategoria);

                    txtDesc.Text = tProductos.DescProducto;
                    txtExistencia.Text = tProductos.Existencia.ToString();
                    txtExistenciaMinima.Text = tProductos.ExistenciaMinima.ToString();
                    txtPrecioVenta.Text = tProductos.PrecioUnidadVenta.ToString();
                    txtPrecioCompra.Text = tProductos.PrecioUnidadCompra.ToString();
                    cmbCat.Text = tCategoria.NombreCategoria;
                    cmbCat.DisplayMember = tCategoria.NombreCategoria;
                    cmbISV.Text = tProductos.ISV;
                    cmbISV.DisplayMember = tProductos.ISV;
                    cmbCat.ValueMember = Convert.ToString(tCategoria.PKCategoriaID);
                    cmbISV.Text = tProductos.ISV;
                    cmbISV.DisplayMember = tProductos.ISV;
                    switch (tProductos.ISV)
                    {
                        case "E":
                            cmbISV.ValueMember = "0";
                            return;
                        case "15%":
                            cmbISV.ValueMember = "1";
                            return;
                        case "18%":
                            cmbISV.ValueMember = "2";
                            return;
                    }
                    cbEstado.Checked = tProductos.Estado;

                }
                catch (Exception)
                {

                }
            }
            editar = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtDesc.Text.Equals("") && txtExistencia.Text.Equals("") && txtExistenciaMinima.Text.Equals("") && txtPrecioVenta.Text.Equals("") && txtPrecioCompra.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            long idd = idProducto;

            if (editar)
            {
                var thProductos = entitiesFact.Productos.FirstOrDefault(x => x.PKProductoID == idProducto);

                thProductos.DescProducto = txtDesc.Text;
                thProductos.Existencia = Convert.ToInt32(txtExistencia.Text);
                thProductos.ExistenciaMinima = Convert.ToInt32(txtExistenciaMinima.Text);
                thProductos.PrecioUnidadVenta = Convert.ToDecimal(txtPrecioVenta.Text);
                thProductos.PrecioUnidadCompra = Convert.ToDecimal(txtPrecioCompra.Text);
                thProductos.ISV = cmbISV.SelectedItem.ToString();
                thProductos.FKCategoria = Convert.ToByte(cmbCat.SelectedIndex + 1);
                thProductos.Estado = cbEstado.Checked;

                entitiesFact.SaveChanges();
            }
            else
            {

                Productos tbProductos = new Productos();

                tbProductos.DescProducto = txtDesc.Text;
                tbProductos.Existencia = Convert.ToInt32(txtExistencia.Text);
                tbProductos.ExistenciaMinima = Convert.ToInt32(txtExistenciaMinima.Text);
                tbProductos.PrecioUnidadVenta = Convert.ToDecimal(txtPrecioVenta.Text);
                tbProductos.PrecioUnidadCompra = Convert.ToDecimal(txtPrecioCompra.Text);
                tbProductos.ISV = cmbISV.SelectedItem.ToString();
                tbProductos.FKCategoria = Convert.ToByte(cmbCat.SelectedIndex + 1);
                tbProductos.Estado = cbEstado.Checked;

                entitiesFact.Productos.Add(tbProductos);

                entitiesFact.SaveChanges();
            }
            txtDesc.Text = "";
            txtExistencia.Text = "";
            txtExistenciaMinima.Text = "";
            txtPrecioVenta.Text = "";
            txtPrecioCompra.Text = "";
            cmbCat.SelectedIndex = -1;
            cmbISV.SelectedIndex = -1;
            cbEstado.Checked = false;
            idProducto = 0;
            editar = false;

            var tProductos = from p in entitiesFact.Productos
                             join c in entitiesFact.Productos_Categorias
                             on p.FKCategoria equals c.PKCategoriaID
                             select new
                             {
                                 p.PKProductoID,
                                 p.DescProducto,
                                 p.Existencia,
                                 p.ExistenciaMinima,
                                 c.NombreCategoria,
                                 p.PrecioUnidadVenta,
                                 p.PrecioUnidadCompra,
                                 p.ISV,
                                 Est = p.Estado == true ? "Activo" : "Inactivo"
                             };
            dgvProductos.DataSource = tProductos.CopyAnonymusToDataTable();

            MessageBox.Show("Informacion guardada!");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (retornar)
            {
                editar = false;
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
                    case 6:
                        MNT_ComprasDetalles form6 = new MNT_ComprasDetalles(compra, backMenu, back, idUsuario, rango);
                        this.Dispose();
                        form6.Show();
                        return;
                }
            }
            else
            {
                txtDesc.Text = "";
                txtExistencia.Text = "";
                txtExistenciaMinima.Text = "";
                txtPrecioVenta.Text = "";
                txtPrecioCompra.Text = "";
                cmbCat.SelectedIndex = -1;
                cmbISV.SelectedIndex = -1;
                cbEstado.Checked = false;
                editar = false;
                retornar = true;
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioVenta.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioVenta.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtExistencia.Text == "" || txtExistenciaMinima.Text == "" || txtPrecioVenta.Text == "" || txtPrecioCompra.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtExistencia.Text != "" && txtExistenciaMinima.Text != "" && txtPrecioVenta.Text != "" && txtPrecioCompra.Text != "") 
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtExistencia_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioVenta.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioVenta.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtExistencia.Text == "" || txtExistenciaMinima.Text == "" || txtPrecioVenta.Text == "" || txtPrecioCompra.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtExistencia.Text != "" && txtExistenciaMinima.Text != "" && txtPrecioVenta.Text != "" && txtPrecioCompra.Text != "")
            {
                btnGuardar.Enabled = true;
            }

            int existenciaminima = 0;
            int existencia = 0;
            if (txtExistenciaMinima.Text != "")
            {
                existenciaminima = Convert.ToInt32(txtExistenciaMinima.Text);
            }
            if (txtExistencia.Text != "")
            {
                existencia = Convert.ToInt32(txtExistencia.Text);
            }
            //Validación de existencias
            if (txtDesc.Text != "")
            {
                if (existenciaminima >= existencia)
                {
                    btnGuardar.Enabled = false;
                    MessageBox.Show("El valor en la existencia debe ser mayor que la existencia mínima.");
                }
                else
                {
                    btnGuardar.Enabled = true;
                }
            }
        }

        private void txtExistenciaMinima_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioVenta.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioVenta.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtExistencia.Text == "" || txtExistenciaMinima.Text == "" || txtPrecioVenta.Text == "" || txtPrecioCompra.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtExistencia.Text != "" && txtExistenciaMinima.Text != "" && txtPrecioVenta.Text != "" && txtPrecioCompra.Text != "")
            {
                btnGuardar.Enabled = true;
            }

            int existenciaminima = 0;
            int existencia = 0;
            if (txtExistenciaMinima.Text != "")
            {
                existenciaminima = Convert.ToInt32(txtExistenciaMinima.Text);
            }
            if(txtExistencia.Text != "")
            {
                existencia = Convert.ToInt32(txtExistencia.Text);
            }
            //Validación de existencias
            if (txtDesc.Text != "")
            {
                if (existenciaminima >= existencia)
                {
                    btnGuardar.Enabled = false;
                    MessageBox.Show("El valor en la existencia debe ser mayor que la existencia mínima.");
                }
                else
                {
                    btnGuardar.Enabled = true;
                }
            }
        }

        private void txtPrecioVenta_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioVenta.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioVenta.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
        }

        private void txtPrecioCompra_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtPrecioCompra.Text != "")
            {
                txtPrecioVenta.Text = (Convert.ToDouble(txtPrecioCompra.Text) + (Convert.ToDouble(txtPrecioCompra.Text) * 0.2)).ToString();
            }
            if (txtDesc.Text == "" || txtExistencia.Text == "" || txtExistenciaMinima.Text == "" || txtPrecioVenta.Text == "" || txtPrecioCompra.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtExistencia.Text != "" && txtExistenciaMinima.Text != "" && txtPrecioVenta.Text != "" && txtPrecioCompra.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void cbEstado_CheckedChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtExistencia.Text == "" && txtExistenciaMinima.Text == "" && txtPrecioVenta.Text == "" && txtPrecioCompra.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtExistencia.Text != "" || txtExistenciaMinima.Text != "" || txtPrecioVenta.Text != "" || txtPrecioCompra.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtExistencia.Text == "" || txtExistenciaMinima.Text == "" || txtPrecioVenta.Text == "" || txtPrecioCompra.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtExistencia.Text != "" && txtExistenciaMinima.Text != "" && txtPrecioVenta.Text != "" && txtPrecioCompra.Text != "")
            {
                btnGuardar.Enabled = true;
            }
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
                        var tID = from p in entitiesFact.Productos
                                      join c in entitiesFact.Productos_Categorias
                                      on p.FKCategoria equals c.PKCategoriaID
                                      where p.PKProductoID == id
                                      select new
                                      {
                                          p.PKProductoID,
                                          p.DescProducto,
                                          p.Existencia,
                                          p.ExistenciaMinima,
                                          c.NombreCategoria,
                                          p.PrecioUnidadVenta,
                                          p.PrecioUnidadCompra,
                                          p.ISV,
                                          Est = p.Estado == true ? "Activo" : "Inactivo"
                                      };
                        dgvProductos.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from p in entitiesFact.Productos
                                          join c in entitiesFact.Productos_Categorias
                                          on p.FKCategoria equals c.PKCategoriaID
                                          select new
                                          {
                                              p.PKProductoID,
                                              p.DescProducto,
                                              p.Existencia,
                                              p.ExistenciaMinima,
                                              c.NombreCategoria,
                                              p.PrecioUnidadVenta,
                                              p.PrecioUnidadCompra,
                                              p.ISV,
                                              Est = p.Estado == true ? "Activo" : "Inactivo"
                                          };
                        dgvProductos.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Nombre del producto":
                    var tNombre = from p in entitiesFact.Productos
                                  join c in entitiesFact.Productos_Categorias
                                  on p.FKCategoria equals c.PKCategoriaID
                                  where p.DescProducto.Contains(busqueda)
                                  select new
                                  {
                                      p.PKProductoID,
                                      p.DescProducto,
                                      p.Existencia,
                                      p.ExistenciaMinima,
                                      c.NombreCategoria,
                                      p.PrecioUnidadVenta,
                                      p.PrecioUnidadCompra,
                                      p.ISV,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvProductos.DataSource = tNombre.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Categoría":
                    var tCategoria = from p in entitiesFact.Productos
                                     join c in entitiesFact.Productos_Categorias
                                     on p.FKCategoria equals c.PKCategoriaID
                                     where c.NombreCategoria.Contains(busqueda)
                                     select new
                                     {
                                         p.PKProductoID,
                                         p.DescProducto,
                                         p.Existencia,
                                         p.ExistenciaMinima,
                                         c.NombreCategoria,
                                         p.PrecioUnidadVenta,
                                         p.PrecioUnidadCompra,
                                         p.ISV,
                                         Est = p.Estado == true ? "Activo" : "Inactivo"
                                     };
                    dgvProductos.DataSource = tCategoria.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                case "ISV":
                    var tISV = from p in entitiesFact.Productos
                                  join c in entitiesFact.Productos_Categorias
                                  on p.FKCategoria equals c.PKCategoriaID
                                  where p.ISV.Contains(busqueda)
                                  select new
                                  {
                                      p.PKProductoID,
                                      p.DescProducto,
                                      p.Existencia,
                                      p.ExistenciaMinima,
                                      c.NombreCategoria,
                                      p.PrecioUnidadVenta,
                                      p.PrecioUnidadCompra,
                                      p.ISV,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvProductos.DataSource = tISV.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Estado":
                    bool est = false;
                    if (busqueda == "Activo" || busqueda == "activo" || busqueda == "ACTIVO" || busqueda == "act" || busqueda == "Act" || busqueda == "ACT") { est = true; } else if (busqueda == "Inactivo" || busqueda == "inactivo" || busqueda == "INACTIVO" || busqueda == "ina" || busqueda == "Ina" || busqueda == "INA") { est = false; }
                    var tEstado = from p in entitiesFact.Productos
                                  join c in entitiesFact.Productos_Categorias
                                  on p.FKCategoria equals c.PKCategoriaID
                                  where p.Estado == est
                                  select new
                                  {
                                      p.PKProductoID,
                                      p.DescProducto,
                                      p.Existencia,
                                      p.ExistenciaMinima,
                                      c.NombreCategoria,
                                      p.PrecioUnidadVenta,
                                      p.PrecioUnidadCompra,
                                      p.ISV,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvProductos.DataSource = tEstado.CopyAnonymusToDataTable();
                    dgvProductos.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    var tProductos = from p in entitiesFact.Productos
                                         join c in entitiesFact.Productos_Categorias
                                         on p.FKCategoria equals c.PKCategoriaID
                                         select new
                                         {
                                             p.PKProductoID,
                                             p.DescProducto,
                                             p.Existencia,
                                             p.ExistenciaMinima,
                                             c.NombreCategoria,
                                             p.PrecioUnidadVenta,
                                             p.PrecioUnidadCompra,
                                             p.ISV,
                                             Est = p.Estado == true ? "Activo" : "Inactivo"
                                         };
                        dgvProductos.DataSource = tProductos.CopyAnonymusToDataTable();
                        dgvProductos.AutoResizeColumns();
                    return;
                default:
                    var tProductos2 = from p in entitiesFact.Productos
                                      join c in entitiesFact.Productos_Categorias
                                      on p.FKCategoria equals c.PKCategoriaID
                                      select new
                                      {
                                          p.PKProductoID,
                                          p.DescProducto,
                                          p.Existencia,
                                          p.ExistenciaMinima,
                                          c.NombreCategoria,
                                          p.PrecioUnidadVenta,
                                          p.PrecioUnidadCompra,
                                          p.ISV,
                                          Est = p.Estado == true ? "Activo" : "Inactivo"
                                      };
                    dgvProductos.DataSource = tProductos2.CopyAnonymusToDataTable();
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
                var tProductos = from p in entitiesFact.Productos
                                 join c in entitiesFact.Productos_Categorias
                                 on p.FKCategoria equals c.PKCategoriaID
                                 select new
                                 {
                                     p.PKProductoID,
                                     p.DescProducto,
                                     p.Existencia,
                                     p.ExistenciaMinima,
                                     c.NombreCategoria,
                                     p.PrecioUnidadVenta,
                                     p.PrecioUnidadCompra,
                                     p.ISV,
                                     Est = p.Estado == true ? "Activo" : "Inactivo"
                                 };
                dgvProductos.DataSource = tProductos.CopyAnonymusToDataTable();
                dgvProductos.AutoResizeColumns();
            } else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }

        private void cmbISV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbISV.Text == "Excento")
            {
                cmbISV.Text = "E";
            }
        }
    }
}
