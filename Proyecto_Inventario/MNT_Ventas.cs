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
    public partial class MNT_Ventas : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        int venta = 0;
        long idVenta = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_Ventas(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_Ventas_Load(object sender, EventArgs e)
        {
            var tVentas = from c in entitiesFact.Ventas
                           join em in entitiesFact.Empleados
                           on c.FKEmpleadoID equals em.PKEmpleadoID
                           select new
                           {
                               c.PKVentaID,
                               c.Fecha,
                               nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                               c.Estatus,
                               Est = c.Estado == true ? "Activo" : "Inactivo"
                           };

            dgvVentas.DataSource = tVentas.CopyAnonymusToDataTable();
            dgvVentas.Columns[0].HeaderCell.Value = "ID";
            dgvVentas.Columns[1].HeaderCell.Value = "Fecha";
            dgvVentas.Columns[2].HeaderCell.Value = "Empleado";
            dgvVentas.Columns[3].HeaderCell.Value = "Estatus";
            dgvVentas.Columns[4].HeaderCell.Value = "Estado";
            dgvVentas.AutoResizeColumns();

            var Empleado = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idUsuario);
            lblUsuario.Text = Empleado.NombreEmpleado + " " + Empleado.ApellidoEmpleado;

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Fecha");
            cmbBuscar.Items.Add("Empleado");
            cmbBuscar.Items.Add("Estatus");
            cmbBuscar.Items.Add("Estado");

            btnCancelar.Text = "Nuevo";
            fechaActual.Text = DateTime.Today.ToShortDateString();
            txtFechaI.Text = DateTime.Today.ToShortDateString();
            txtFechaI.ReadOnly = true;
            txtFechaF.Text = DateTime.Today.ToShortDateString();
            txtFechaF.ReadOnly = true;
        }

        private void dgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                idVenta = Convert.ToInt64(dgvVentas.SelectedCells[0].Value);
                venta = Convert.ToInt32(idVenta);
                var tVentas = entitiesFact.Ventas.FirstOrDefault(x => x.PKVentaID == idVenta);
                cbEstado.Checked = tVentas.Estado;
                var tEmpleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == tVentas.FKEmpleadoID);

                if (tVentas.Estatus == "Completo")
                {
                    btnDetalles.Enabled = false;
                } else
                {
                    btnDetalles.Enabled = true;
                }

            }
            catch (Exception)
            {

            }
            editar = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            
            if (editar)
            {
                var thVentas = entitiesFact.Ventas.FirstOrDefault(x => x.PKVentaID == idVenta);

                thVentas.FKEmpleadoID = Convert.ToInt32(idUsuario);
                thVentas.Estatus = "En Proceso";
                thVentas.Estado = cbEstado.Checked;
                thVentas.Fecha = DateTime.Now;

                entitiesFact.SaveChanges();
            }
            else
            {

                Ventas tbVentas = new Ventas();

                tbVentas.FKEmpleadoID = Convert.ToInt32(idUsuario);
                tbVentas.Estatus = "En Proceso";
                tbVentas.Estado = cbEstado.Checked;
                tbVentas.Fecha = DateTime.Now;

                entitiesFact.Ventas.Add(tbVentas);

                entitiesFact.SaveChanges();
            }
            cbEstado.Checked = false;
            idVenta = 0;
            editar = false;

            var tVentas = from c in entitiesFact.Ventas
                           join em in entitiesFact.Empleados
                           on c.FKEmpleadoID equals em.PKEmpleadoID
                           select new
                           {
                               c.PKVentaID,
                               c.Fecha,
                               nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                               c.Estatus,
                               Est = c.Estado == true ? "Activo" : "Inactivo"
                           };
            dgvVentas.DataSource = tVentas.CopyAnonymusToDataTable();

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
                btnCancelar.Text = "Regresar";
                cbEstado.Checked = false;
                editar = false;
                retornar = true;
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
                        var tID = from c in entitiesFact.Ventas
                                  join em in entitiesFact.Empleados
                                  on c.FKEmpleadoID equals em.PKEmpleadoID
                                  where c.PKVentaID == id
                                  select new
                                  {
                                      c.PKVentaID,
                                      c.Fecha,
                                      nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                      c.Estatus,
                                      Est = c.Estado == true ? "Activo" : "Inactivo"
                                  };
                        dgvVentas.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from c in entitiesFact.Ventas
                                  join em in entitiesFact.Empleados
                                  on c.FKEmpleadoID equals em.PKEmpleadoID
                                  select new
                                  {
                                      c.PKVentaID,
                                      c.Fecha,
                                      nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                      c.Estatus,
                                      Est = c.Estado == true ? "Activo" : "Inactivo"
                                  };
                        dgvVentas.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvVentas.AutoResizeColumns();
                    return;
                case "Empleado":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tEmpleado = from c in entitiesFact.Ventas
                                    join em in entitiesFact.Empleados
                                    on c.FKEmpleadoID equals em.PKEmpleadoID
                                    where em.NombreEmpleado.Contains(busqueda) || em.ApellidoEmpleado.Contains(busqueda)
                                    select new
                                    {
                                        c.PKVentaID,
                                        c.Fecha,
                                        nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                        c.Estatus,
                                        Est = c.Estado == true ? "Activo" : "Inactivo"
                                    };
                    dgvVentas.DataSource = tEmpleado.CopyAnonymusToDataTable();
                    dgvVentas.AutoResizeColumns();
                    return;
                case "Estatus":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tEstatus = from c in entitiesFact.Ventas
                                   join em in entitiesFact.Empleados
                                   on c.FKEmpleadoID equals em.PKEmpleadoID
                                   where c.Estatus.Contains(busqueda)
                                   select new
                                   {
                                       c.PKVentaID,
                                       c.Fecha,
                                       nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                       c.Estatus,
                                       Est = c.Estado == true ? "Activo" : "Inactivo"
                                   };
                    dgvVentas.DataSource = tEstatus.CopyAnonymusToDataTable();
                    dgvVentas.AutoResizeColumns();
                    return;
                case "Estado":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    bool est = false;
                    if (busqueda == "Activo" || busqueda == "activo" || busqueda == "ACTIVO" || busqueda == "act" || busqueda == "Act" || busqueda == "ACT" || busqueda == "ac" || busqueda == "Ac" || busqueda == "AC") { est = true; } else if (busqueda == "Inactivo" || busqueda == "inactivo" || busqueda == "INACTIVO" || busqueda == "ina" || busqueda == "Ina" || busqueda == "INA" || busqueda == "ac" || busqueda == "Ac" || busqueda == "AC") { est = false; }
                    var tEstado = from c in entitiesFact.Ventas
                                  join em in entitiesFact.Empleados
                                  on c.FKEmpleadoID equals em.PKEmpleadoID
                                  where c.Estado == est
                                  select new
                                  {
                                      c.PKVentaID,
                                      c.Fecha,
                                      nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                      c.Estatus,
                                      Est = c.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvVentas.DataSource = tEstado.CopyAnonymusToDataTable();
                    dgvVentas.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    
                    return;
                default:
                    lblFechaF.Visible = false;
                    lblFechaI.Visible = false;
                    txtFechaF.Visible = false;
                    txtFechaI.Visible = false;
                    btnFecha.Visible = false;
                    txtBuscar.Visible = true;
                    var tVentas2 = from c in entitiesFact.Ventas
                                  join em in entitiesFact.Empleados
                                  on c.FKEmpleadoID equals em.PKEmpleadoID
                                  select new
                                  {
                                      c.PKVentaID,
                                      c.Fecha,
                                      nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                      c.Estatus,
                                      Est = c.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvVentas.DataSource = tVentas2.CopyAnonymusToDataTable();
                    dgvVentas.AutoResizeColumns();
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
                var tVentas = from c in entitiesFact.Ventas
                              join em in entitiesFact.Empleados
                              on c.FKEmpleadoID equals em.PKEmpleadoID
                              select new
                              {
                                  c.PKVentaID,
                                  c.Fecha,
                                  nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                  c.Estatus,
                                  Est = c.Estado == true ? "Activo" : "Inactivo"
                              };
                dgvVentas.DataSource = tVentas.CopyAnonymusToDataTable();
                dgvVentas.AutoResizeColumns();
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
            MNT_VentasDetalles detalles = new MNT_VentasDetalles(venta, back, idUsuario, rango);
            this.Dispose();
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
                var tFecha = from c in entitiesFact.Ventas
                             join em in entitiesFact.Empleados
                             on c.FKEmpleadoID equals em.PKEmpleadoID
                             where c.Fecha >= fechaI && c.Fecha <= fechaF
                             select new
                             {
                                 c.PKVentaID,
                                 c.Fecha,
                                 nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                 c.Estatus,
                                 Est = c.Estado == true ? "Activo" : "Inactivo"
                             };
                dgvVentas.DataSource = tFecha.CopyAnonymusToDataTable();
            }
            else if (txtFechaI.Text == "" && txtFechaF.Text == "")
            {
                var tFecha2 = from c in entitiesFact.Ventas
                              join em in entitiesFact.Empleados
                              on c.FKEmpleadoID equals em.PKEmpleadoID
                              select new
                              {
                                  c.PKVentaID,
                                  c.Fecha,
                                  nombre = em.NombreEmpleado + " " + em.ApellidoEmpleado,
                                  c.Estatus,
                                  Est = c.Estado == true ? "Activo" : "Inactivo"
                              };
                dgvVentas.DataSource = tFecha2.CopyAnonymusToDataTable();
            }
            dgvVentas.AutoResizeColumns();
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
            MNT_VentasDetallesResultados detalles = new MNT_VentasDetallesResultados(back, idUsuario, rango);
            this.Hide();
            detalles.Show();
        }
    }
}
