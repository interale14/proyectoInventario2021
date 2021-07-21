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
    public partial class MNT_Compras : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        int compra = 0;
        long idCompra = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_Compras(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_Compras_Load(object sender, EventArgs e)
        {
            var tCompras = from c in entitiesFact.Compras
                             join em in entitiesFact.Empleados
                             on c.FKEmpleadoID equals em.PKEmpleadoID
                             join p in entitiesFact.Proveedores
                             on c.FKProveedorID equals p.PKProveedorID
                             select new
                             {
                                 c.PKCompraID,
                                 c.Fecha,
                                 nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                 p.NombreProveedor,
                                 c.Estatus,
                                 Est = c.Estado == true ? "Activo" : "Inactivo"
                             };

            dgvCompras.DataSource = tCompras.CopyAnonymusToDataTable();
            dgvCompras.Columns[0].HeaderCell.Value = "ID";
            dgvCompras.Columns[1].HeaderCell.Value = "Fecha";
            dgvCompras.Columns[2].HeaderCell.Value = "Empleado";
            dgvCompras.Columns[3].HeaderCell.Value = "Proveedor";
            dgvCompras.Columns[4].HeaderCell.Value = "Estatus";
            dgvCompras.Columns[5].HeaderCell.Value = "Estado";
            dgvCompras.AutoResizeColumns();

            var Empleado = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);
            lblUsuario.Text = Empleado.NombreEmpleado + " " + Empleado.ApellidoEmpleado;

            var tProveedores = from p in entitiesFact.Proveedores
                               where p.Estado == true
                             select new
                             {
                                 p.PKProveedorID,
                                 p.NombreProveedor
                             };

            DataTable dtProveedores = tProveedores.CopyAnonymusToDataTable();
            cmbProveedor.DataSource = dtProveedores;
            cmbProveedor.DisplayMember = dtProveedores.Columns[1].ColumnName;
            cmbProveedor.ValueMember = dtProveedores.Columns[0].ColumnName;

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Fecha");
            cmbBuscar.Items.Add("Empleado");
            cmbBuscar.Items.Add("Proveedor");
            cmbBuscar.Items.Add("Estatus");
            cmbBuscar.Items.Add("Estado");

            btnCancelar.Text = "Nuevo";
            fechaActual.Text = DateTime.Today.ToShortDateString();
            txtFechaI.Text = DateTime.Today.ToShortDateString();
            txtFechaI.ReadOnly = true;
            txtFechaF.Text = DateTime.Today.ToShortDateString();
            txtFechaF.ReadOnly = true;
        }

        private void dgvCompras_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCompras.RowCount > 0)
            {
                try
                {
                    idCompra = Convert.ToInt64(dgvCompras.SelectedCells[0].Value);
                    compra = Convert.ToInt32(idCompra);
                    var tCompras = entitiesFact.Compras.FirstOrDefault(x => x.PKCompraID == idCompra);
                    cbEstado.Checked = tCompras.Estado;
                    if (tCompras.Estatus != "Completo")
                    {
                        btnDetalles.Enabled = true;
                    }
                    else
                    {
                        btnDetalles.Enabled = false;
                    }
                    var tEmpleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == tCompras.FKEmpleadoID);
                    var tProveedor = entitiesFact.Proveedores.FirstOrDefault(x => x.PKProveedorID == tCompras.FKProveedorID);
                    cmbProveedor.Text = tProveedor.NombreProveedor;

                    cmbProveedor.DisplayMember = tProveedor.NombreContacto;
                    cmbProveedor.ValueMember = Convert.ToString(tProveedor.PKProveedorID);
                }
                catch (Exception)
                {

                }
                editar = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbProveedor.Text.ToString() == "")
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thCompras = entitiesFact.Compras.FirstOrDefault(x => x.PKCompraID == idCompra);

                thCompras.FKEmpleadoID = Convert.ToInt32(idUsuario);
                thCompras.Estatus = "En Proceso";
                thCompras.FKProveedorID = Convert.ToByte(cmbProveedor.SelectedIndex + 1);
                thCompras.Estado = cbEstado.Checked;
                thCompras.Fecha = DateTime.Now;

                entitiesFact.SaveChanges();
            }
            else
            {

                Compras tbCompras = new Compras();

                tbCompras.FKEmpleadoID = Convert.ToInt32(idUsuario);
                tbCompras.Estatus = "En Proceso";
                tbCompras.FKProveedorID = Convert.ToByte(cmbProveedor.SelectedIndex + 1);
                tbCompras.Estado = cbEstado.Checked;
                tbCompras.Fecha = DateTime.Now;

                entitiesFact.Compras.Add(tbCompras);

                entitiesFact.SaveChanges();
            }
            cmbProveedor.SelectedIndex = -1;
            cmbProveedor.Text = "";
            cbEstado.Checked = false;
            idCompra = 0;
            editar = false;

            var tCompras = from c in entitiesFact.Compras
                           join em in entitiesFact.Empleados
                           on c.FKEmpleadoID equals em.PKEmpleadoID
                           join p in entitiesFact.Proveedores
                           on c.FKProveedorID equals p.PKProveedorID
                           select new
                           {
                               c.PKCompraID,
                               c.Fecha,
                               nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                               p.NombreProveedor,
                               c.Estatus,
                               Est = c.Estado == true ? "Activo" : "Inactivo"
                           };
            dgvCompras.DataSource = tCompras.CopyAnonymusToDataTable();

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
                cmbProveedor.SelectedIndex = -1;
                cmbProveedor.Text = "";
                cbEstado.Checked = false;
                editar = false;
                retornar = true;
            }
        }

        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProveedor.Text.ToString() == "")
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (cmbProveedor.Text.ToString() != "")
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (cmbProveedor.Text.ToString() == "")
            {
                btnGuardar.Enabled = false;
            }
            else
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
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    int id = 1;
                    if (busqueda != "")
                    {
                        id = Convert.ToInt32(busqueda);
                        var tID = from c in entitiesFact.Compras
                                       join em in entitiesFact.Empleados
                                       on c.FKEmpleadoID equals em.PKEmpleadoID
                                       join p in entitiesFact.Proveedores
                                       on c.FKProveedorID equals p.PKProveedorID
                                       where c.PKCompraID == id
                                       select new
                                       {
                                           c.PKCompraID,
                                           c.Fecha,
                                           nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                           p.NombreProveedor,
                                           c.Estatus,
                                           Est = c.Estado == true ? "Activo" : "Inactivo"
                                       };
                        dgvCompras.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from c in entitiesFact.Compras
                                   join em in entitiesFact.Empleados
                                   on c.FKEmpleadoID equals em.PKEmpleadoID
                                   join p in entitiesFact.Proveedores
                                   on c.FKProveedorID equals p.PKProveedorID
                                   select new
                                   {
                                       c.PKCompraID,
                                       c.Fecha,
                                       nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                       p.NombreProveedor,
                                       c.Estatus,
                                       Est = c.Estado == true ? "Activo" : "Inactivo"
                                   };
                        dgvCompras.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvCompras.AutoResizeColumns();
                    return;
                case "Empleado":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tEmpleado = from c in entitiesFact.Compras
                              join em in entitiesFact.Empleados
                              on c.FKEmpleadoID equals em.PKEmpleadoID
                              join p in entitiesFact.Proveedores
                              on c.FKProveedorID equals p.PKProveedorID
                              where em.NombreEmpleado.Contains(busqueda) || em.ApellidoEmpleado.Contains(busqueda)
                              select new
                              {
                                  c.PKCompraID,
                                  c.Fecha,
                                  nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                  p.NombreProveedor,
                                  c.Estatus,
                                  Est = c.Estado == true ? "Activo" : "Inactivo"
                              };
                    dgvCompras.DataSource = tEmpleado.CopyAnonymusToDataTable();
                    dgvCompras.AutoResizeColumns();
                    return;
                case "Proveedor":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tProveedor = from c in entitiesFact.Compras
                                    join em in entitiesFact.Empleados
                                    on c.FKEmpleadoID equals em.PKEmpleadoID
                                    join p in entitiesFact.Proveedores
                                    on c.FKProveedorID equals p.PKProveedorID
                                    where p.NombreProveedor.Contains(busqueda)
                                    select new
                                    {
                                        c.PKCompraID,
                                        c.Fecha,
                                        nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                        p.NombreProveedor,
                                        c.Estatus,
                                        Est = c.Estado == true ? "Activo" : "Inactivo"
                                    };
                    dgvCompras.DataSource = tProveedor.CopyAnonymusToDataTable();
                    dgvCompras.AutoResizeColumns();
                    return;
                case "Estatus":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tEstatus = from c in entitiesFact.Compras
                                     join em in entitiesFact.Empleados
                                     on c.FKEmpleadoID equals em.PKEmpleadoID
                                     join p in entitiesFact.Proveedores
                                     on c.FKProveedorID equals p.PKProveedorID
                                     where c.Estatus.Contains(busqueda)
                                     select new
                                     {
                                         c.PKCompraID,
                                         c.Fecha,
                                         nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                         p.NombreProveedor,
                                         c.Estatus,
                                         Est = c.Estado == true ? "Activo" : "Inactivo"
                                     };
                    dgvCompras.DataSource = tEstatus.CopyAnonymusToDataTable();
                    dgvCompras.AutoResizeColumns();
                    return;
                case "Estado":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    bool est = false;
                    if (busqueda == "Activo" || busqueda == "activo" || busqueda == "ACTIVO" || busqueda == "act" || busqueda == "Act" || busqueda == "ACT") { est = true; } else if (busqueda == "Inactivo" || busqueda == "inactivo" || busqueda == "INACTIVO" || busqueda == "ina" || busqueda == "Ina" || busqueda == "INA") { est = false; }
                    var tEstado = from c in entitiesFact.Compras
                                  join em in entitiesFact.Empleados
                                  on c.FKEmpleadoID equals em.PKEmpleadoID
                                  join p in entitiesFact.Proveedores
                                  on c.FKProveedorID equals p.PKProveedorID
                                  where c.Estado == est
                                  select new
                                  {
                                      c.PKCompraID,
                                      c.Fecha,
                                      nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                      p.NombreProveedor,
                                      c.Estatus,
                                      Est = c.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvCompras.DataSource = tEstado.CopyAnonymusToDataTable();
                    dgvCompras.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tCompras2 = from c in entitiesFact.Compras
                                       join em in entitiesFact.Empleados
                                       on c.FKEmpleadoID equals em.PKEmpleadoID
                                       join p in entitiesFact.Proveedores
                                       on c.FKProveedorID equals p.PKProveedorID
                                       select new
                                       {
                                           c.PKCompraID,
                                           c.Fecha,
                                           nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                           p.NombreProveedor,
                                           c.Estatus,
                                           Est = c.Estado == true ? "Activo" : "Inactivo"
                                       };
                        dgvCompras.DataSource = tCompras2.CopyAnonymusToDataTable();
                        dgvCompras.AutoResizeColumns();
                    return;
                default:
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tCompras = from c in entitiesFact.Compras
                                   join em in entitiesFact.Empleados
                                   on c.FKEmpleadoID equals em.PKEmpleadoID
                                   join p in entitiesFact.Proveedores
                                   on c.FKProveedorID equals p.PKProveedorID
                                   select new
                                   {
                                       c.PKCompraID,
                                       c.Fecha,
                                       nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                       p.NombreProveedor,
                                       c.Estatus,
                                       Est = c.Estado == true ? "Activo" : "Inactivo"
                                   };
                    dgvCompras.DataSource = tCompras.CopyAnonymusToDataTable();
                    dgvCompras.AutoResizeColumns();
                    return;
            }
        }

        private void cmbBuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            campoBuscar = "";
            campoBuscar = cmbBuscar.SelectedItem.ToString();
            if (campoBuscar == "Mostrar Todo") { txtBuscar.ReadOnly = true; } else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
            if (campoBuscar == "Fecha")
            {
                lblFechaF.Visible = true;
                lblFechaI.Visible = true;
                txtFechaF.Visible = true;
                txtFechaI.Visible = true;
                btnFecha.Visible = true;
                txtBuscar.Visible = false;
            }
            else if (campoBuscar == "Mostrar Todo")
            {
                var tCompras2 = from c in entitiesFact.Compras
                                join em in entitiesFact.Empleados
                                on c.FKEmpleadoID equals em.PKEmpleadoID
                                join p in entitiesFact.Proveedores
                                on c.FKProveedorID equals p.PKProveedorID
                                select new
                                {
                                    c.PKCompraID,
                                    c.Fecha,
                                    nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                    p.NombreProveedor,
                                    c.Estatus,
                                    Est = c.Estado == true ? "Activo" : "Inactivo"
                                };
                dgvCompras.DataSource = tCompras2.CopyAnonymusToDataTable();
                dgvCompras.AutoResizeColumns();
            }
            else
            {
                lblFechaF.Visible = false;
                lblFechaI.Visible = false;
                txtFechaF.Visible = false;
                txtFechaI.Visible = false;
                btnFecha.Visible = false;
                txtBuscar.Visible = true;
            }
        }

        private void btnDetalles_Click(object sender, EventArgs e)
        {
            MNT_ComprasDetalles detalles = new MNT_ComprasDetalles(compra, back, back, idUsuario, rango);
            this.Hide();
            detalles.Show();
        }

        private void btnFecha_Click(object sender, EventArgs e)
        {
            DateTime fechaI = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            DateTime fechaF = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            if (txtFechaI.Text != "") { fechaI = Convert.ToDateTime(txtFechaI.Text); }
            if (txtFechaF.Text != "") { fechaF = Convert.ToDateTime(txtFechaF.Text); }

            if ((txtFechaI.Text != "" && txtFechaF.Text == "") || (txtFechaI.Text == "" && txtFechaF.Text != "") || (txtFechaI.Text != "" && txtFechaF.Text != ""))
            {
                var tFecha = from c in entitiesFact.Compras
                             join em in entitiesFact.Empleados
                             on c.FKEmpleadoID equals em.PKEmpleadoID
                             join p in entitiesFact.Proveedores
                             on c.FKProveedorID equals p.PKProveedorID
                             where c.Fecha >= fechaI && c.Fecha <= fechaF
                             select new
                             {
                                 c.PKCompraID,
                                 c.Fecha,
                                 nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                 p.NombreProveedor,
                                 c.Estatus,
                                 Est = c.Estado == true ? "Activo" : "Inactivo"
                             };
                dgvCompras.DataSource = tFecha.CopyAnonymusToDataTable();
            }
            else if (txtFechaI.Text == "" && txtFechaF.Text == "")
            {
                var tFecha2 = from c in entitiesFact.Compras
                             join em in entitiesFact.Empleados
                             on c.FKEmpleadoID equals em.PKEmpleadoID
                             join p in entitiesFact.Proveedores
                             on c.FKProveedorID equals p.PKProveedorID
                             select new
                             {
                                 c.PKCompraID,
                                 c.Fecha,
                                 nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                 p.NombreProveedor,
                                 c.Estatus,
                                 Est = c.Estado == true ? "Activo" : "Inactivo"
                             };
                dgvCompras.DataSource = tFecha2.CopyAnonymusToDataTable();
            }
            dgvCompras.AutoResizeColumns();
        }

        private void txtFechaI_Click(object sender, EventArgs e)
        {
            if (txtFechaI.Text == DateTime.Today.ToShortDateString())
            {
                txtFechaI.Text = "";
                txtFechaI.ReadOnly = false;
            }
        }

        private void txtFechaF_Click(object sender, EventArgs e)
        {
            if (txtFechaF.Text == DateTime.Today.ToShortDateString())
            {
                txtFechaF.Text = "";
                txtFechaF.ReadOnly = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MNT_ComprasDetallesResultados form = new MNT_ComprasDetallesResultados(back, idUsuario, rango);
            this.Hide();
            form.Show();
        }
    }
}
