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
    public partial class MNT_ProductosCategorias : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        long idCategoria = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_ProductosCategorias(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_ProductosCategorias_Load(object sender, EventArgs e)
        {
            var tCategorias = from c in entitiesFact.Productos_Categorias
                             select new
                             {
                                 c.PKCategoriaID,
                                 c.NombreCategoria,
                                 c.DescripcionCategoria
                             };

            dgvCategorias.DataSource = tCategorias.CopyAnonymusToDataTable();
            dgvCategorias.Columns[0].HeaderCell.Value = "ID";
            dgvCategorias.Columns[1].HeaderCell.Value = "Descripción";
            dgvCategorias.Columns[2].HeaderCell.Value = "Detalles";
            dgvCategorias.AutoResizeColumns();


            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("Descripcion");
            cmbBuscar.Items.Add("Detalles");

            btnCancelar.Text = "Nuevo";
        }

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.RowCount > 0)
            {
                try
                {
                    idCategoria = Convert.ToInt64(dgvCategorias.SelectedCells[0].Value);
                    var tEmpleados = entitiesFact.Productos_Categorias.FirstOrDefault(x => x.PKCategoriaID == idCategoria);

                    txtDesc.Text = tEmpleados.NombreCategoria;
                    txtDetalles.Text = tEmpleados.DescripcionCategoria;
                    editar = true;

                }
                catch (Exception)
                {

                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtDesc.Text.Equals("") && txtDetalles.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thCategorias = entitiesFact.Productos_Categorias.FirstOrDefault(x => x.PKCategoriaID == idCategoria);
                thCategorias.NombreCategoria = txtDesc.Text;
                thCategorias.DescripcionCategoria = txtDetalles.Text;

                entitiesFact.SaveChanges();
            }
            else
            {

                Productos_Categorias tbCategorias = new Productos_Categorias();
                tbCategorias.NombreCategoria = txtDesc.Text;
                tbCategorias.DescripcionCategoria = txtDetalles.Text;
                entitiesFact.Productos_Categorias.Add(tbCategorias);

                entitiesFact.SaveChanges();
            }
            txtDesc.Text = "";
            txtDetalles.Text = "";
            editar = false;

            var tCategorias = from c in entitiesFact.Productos_Categorias
                              select new
                              {
                                  c.PKCategoriaID,
                                  c.NombreCategoria,
                                  c.DescripcionCategoria
                              };
            dgvCategorias.DataSource = tCategorias.CopyAnonymusToDataTable();

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
                }
            }
            else
            {
                txtDesc.Text = "";
                txtDetalles.Text = "";
                editar = false;
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtDetalles.Text == "")
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtDetalles.Text != "")
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtDetalles.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtDetalles.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtDetalles_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtDetalles.Text == "")
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtDetalles.Text != "")
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtDetalles.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtDetalles.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text;

            switch (campoBuscar)
            {
                case "Descripcion":
                    var tDescripcion = from c in entitiesFact.Productos_Categorias
                                       where c.NombreCategoria.Contains(busqueda)
                                      select new
                                      {
                                          c.PKCategoriaID,
                                          c.NombreCategoria,
                                          c.DescripcionCategoria
                                      };
                    dgvCategorias.DataSource = tDescripcion.CopyAnonymusToDataTable();
                    return;
                case "Detalles":
                    var tDetalles = from c in entitiesFact.Productos_Categorias
                                       where c.DescripcionCategoria.Contains(busqueda)
                                    select new
                                       {
                                           c.PKCategoriaID,
                                           c.NombreCategoria,
                                           c.DescripcionCategoria
                                       };
                    dgvCategorias.DataSource = tDetalles.CopyAnonymusToDataTable();
                    dgvCategorias.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    if (cmbBuscar.Text == "Mostrar Todo" || cmbBuscar.Text == "")
                    {
                        var tCategoria = from c in entitiesFact.Productos_Categorias
                                        select new
                                        {
                                            c.PKCategoriaID,
                                            c.NombreCategoria,
                                            c.DescripcionCategoria
                                        };
                        dgvCategorias.DataSource = tCategoria.CopyAnonymusToDataTable();
                    }
                    return;
                default:
                    var tCategoria2 = from c in entitiesFact.Productos_Categorias
                                     select new
                                     {
                                         c.PKCategoriaID,
                                         c.NombreCategoria,
                                         c.DescripcionCategoria
                                     };
                    dgvCategorias.DataSource = tCategoria2.CopyAnonymusToDataTable();
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
                var tCategoria = from c in entitiesFact.Productos_Categorias
                                 select new
                                 {
                                     c.PKCategoriaID,
                                     c.NombreCategoria,
                                     c.DescripcionCategoria
                                 };
                dgvCategorias.DataSource = tCategoria.CopyAnonymusToDataTable();
            } else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
    }
}
