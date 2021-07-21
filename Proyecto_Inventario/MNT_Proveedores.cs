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
    public partial class MNT_Proveedores : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        long idProveedor = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";

        public MNT_Proveedores(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_Proveedores_Load(object sender, EventArgs e)
        {
            var tProveedores = from p in entitiesFact.Proveedores
                                 select new
                                 {
                                     p.PKProveedorID,
                                     p.NombreProveedor,
                                     p.NombreContacto,
                                     p.TelefonoContacto1,
                                     p.TelefonoContacto2,
                                     p.EmailContacto1,
                                     p.EmailContacto2,
                                     p.Direccion,
                                     Est = p.Estado == true ? "Activo" : "Inactivo"
                                 };
            dgvProveedores.DataSource = tProveedores.CopyAnonymusToDataTable();
            dgvProveedores.Columns[0].HeaderCell.Value = "ID";
            dgvProveedores.Columns[1].HeaderCell.Value = "Nombre del Proveedor";
            dgvProveedores.Columns[2].HeaderCell.Value = "Contacto";
            dgvProveedores.Columns[3].HeaderCell.Value = "Teléfono";
            dgvProveedores.Columns[4].HeaderCell.Value = "Teléfono Opcional";
            dgvProveedores.Columns[5].HeaderCell.Value = "Correo Electrónico";
            dgvProveedores.Columns[6].HeaderCell.Value = "Correco Electrónico Opcional";
            dgvProveedores.Columns[7].HeaderCell.Value = "Dirección";
            dgvProveedores.Columns[8].HeaderCell.Value = "Estado";
            dgvProveedores.AutoResizeColumns();

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Nombre Proveedor");
            cmbBuscar.Items.Add("Contacto");
            cmbBuscar.Items.Add("Teléfono");
            cmbBuscar.Items.Add("Correo Electrónico");
            cmbBuscar.Items.Add("Estado");

            btnCancelar.Text = "Nuevo";
        }

        private void dgvProveedores_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProveedores.RowCount > 0)
            {
                try
                {
                    idProveedor = Convert.ToInt64(dgvProveedores.SelectedCells[0].Value);
                    var tProveedores = entitiesFact.Proveedores.FirstOrDefault(x => x.PKProveedorID == idProveedor);
                    txtNombreProveedor.Text = tProveedores.NombreProveedor;
                    txtContacto.Text = tProveedores.NombreContacto;
                    txtTelefono1.Text = Convert.ToString(tProveedores.TelefonoContacto1);
                    txtTelefono2.Text = Convert.ToString(tProveedores.TelefonoContacto2);
                    txtCorreo1.Text = tProveedores.EmailContacto1;
                    txtCorreo2.Text = tProveedores.EmailContacto2;
                    txtDireccion.Text = tProveedores.Direccion;
                    cbEstado.Checked = tProveedores.Estado;
                    editar = true;

                }
                catch (Exception)
                {

                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtDireccion.Text.Equals("") && txtContacto.Text.Equals("") && txtTelefono1.Text.Equals("") && txtCorreo1.Text.Equals("") && txtDireccion.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thProveedores = entitiesFact.Proveedores.FirstOrDefault(x => x.PKProveedorID == idProveedor);
                thProveedores.NombreProveedor = txtNombreProveedor.Text;
                thProveedores.NombreContacto = txtContacto.Text;
                thProveedores.TelefonoContacto1 = Convert.ToInt32(txtTelefono1.Text);
                if (txtTelefono2.Text != "") { thProveedores.TelefonoContacto2 = Convert.ToInt32(txtTelefono2.Text); } else { thProveedores.TelefonoContacto2 = null; }
                thProveedores.EmailContacto1 = txtCorreo1.Text;
                if (txtCorreo2.Text != "") { thProveedores.EmailContacto2 = txtCorreo2.Text; } else { thProveedores.EmailContacto2 = null; }
                if (txtDireccion.Text != "") { thProveedores.Direccion = txtDireccion.Text; } else { thProveedores.Direccion = null; }
                thProveedores.Estado = cbEstado.Checked;

                entitiesFact.SaveChanges();
            }
            else
            {

                Proveedores tbProveedores = new Proveedores();
                tbProveedores.NombreProveedor = txtNombreProveedor.Text;
                tbProveedores.NombreContacto = txtContacto.Text;
                tbProveedores.TelefonoContacto1 = Convert.ToInt32(txtTelefono1.Text);
                if (txtTelefono2.Text != "") { tbProveedores.TelefonoContacto2 = Convert.ToInt32(txtTelefono2.Text); } else { tbProveedores.TelefonoContacto2 = null; }
                tbProveedores.EmailContacto1 = txtCorreo1.Text;
                if (txtCorreo2.Text != "") { tbProveedores.EmailContacto2 = txtCorreo2.Text; } else { tbProveedores.EmailContacto2 = null; }
                if (txtDireccion.Text != "") { tbProveedores.Direccion = txtDireccion.Text; } else { tbProveedores.Direccion = null; }
                tbProveedores.Estado = cbEstado.Checked;
                entitiesFact.Proveedores.Add(tbProveedores);

                entitiesFact.SaveChanges();
            }
            txtNombreProveedor.Text = "";
            txtContacto.Text = "";
            txtTelefono1.Text = "";
            txtTelefono2.Text = "";
            txtCorreo1.Text = "";
            txtCorreo2.Text = "";
            txtDireccion.Text = "";
            cbEstado.Checked = false;
            idProveedor = 0;
            editar = false;

            var tProveedores = from p in entitiesFact.Proveedores
                                select new
                                {
                                    p.PKProveedorID,
                                    p.NombreProveedor,
                                    p.NombreContacto,
                                    p.TelefonoContacto1,
                                    p.TelefonoContacto2,
                                    p.EmailContacto1,
                                    p.EmailContacto2,
                                    p.Direccion,
                                    Est = p.Estado == true ? "Activo" : "Inactivo"
                                };
            dgvProveedores.DataSource = tProveedores.CopyAnonymusToDataTable();

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
                txtNombreProveedor.Text = "";
                txtContacto.Text = "";
                txtTelefono1.Text = "";
                txtTelefono2.Text = "";
                txtCorreo1.Text = "";
                txtCorreo2.Text = "";
                txtDireccion.Text = "";
                cbEstado.Checked = false;
                editar = false;
            }
        }

        private void txtNombreProveedor_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtContacto_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtTelefono1_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "" || txtTelefono1.Text.Length != 8)
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "" && txtTelefono1.Text.Length == 8)
            {
                btnGuardar.Enabled = true;
            }
            if (txtTelefono1.Text.Length > 8) 
            {
                MessageBox.Show("El número de teléfono debe de llevar 8 dígitos como máximo.");
            }
        }

        private void txtTelefono2_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "" || txtTelefono1.Text.Length != 8)
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "" && txtTelefono1.Text.Length == 8)
            {
                btnGuardar.Enabled = true;
            }
            if (txtTelefono2.Text.Length > 8)
            {
                MessageBox.Show("El número de teléfono debe de llevar 8 dígitos como máximo.");
            }
        }

        private void txtCorreo1_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtCorreo2_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtDireccion_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void cbEstado_CheckedChanged(object sender, EventArgs e)
        {
            if (txtNombreProveedor.Text == "" && txtContacto.Text == "" && txtTelefono1.Text == "" && txtTelefono2.Text == "" && txtCorreo1.Text == "" && txtCorreo2.Text == "" && txtDireccion.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreProveedor.Text != "" || txtContacto.Text != "" || txtTelefono1.Text != "" || txtTelefono2.Text != "" || txtCorreo1.Text != "" || txtCorreo2.Text != "" || txtDireccion.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreProveedor.Text == "" || txtContacto.Text == "" || txtTelefono1.Text == "" || txtCorreo1.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreProveedor.Text != "" && txtContacto.Text != "" && txtTelefono1.Text != "" && txtCorreo1.Text != "")
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
                        var tID = from p in entitiesFact.Proveedores
                                         where p.PKProveedorID == id
                                         select new
                                         {
                                             p.PKProveedorID,
                                             p.NombreProveedor,
                                             p.NombreContacto,
                                             p.TelefonoContacto1,
                                             p.TelefonoContacto2,
                                             p.EmailContacto1,
                                             p.EmailContacto2,
                                             p.Direccion,
                                             Est = p.Estado == true ? "Activo" : "Inactivo"
                                         };
                        dgvProveedores.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from p in entitiesFact.Proveedores
                                            select new
                                            {
                                                p.PKProveedorID,
                                                p.NombreProveedor,
                                                p.NombreContacto,
                                                p.TelefonoContacto1,
                                                p.TelefonoContacto2,
                                                p.EmailContacto1,
                                                p.EmailContacto2,
                                                p.Direccion,
                                                Est = p.Estado == true ? "Activo" : "Inactivo"
                                            };
                        dgvProveedores.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvProveedores.AutoResizeColumns();
                    return;
                case "Nombre Proveedor":
                    var tProveedor = from p in entitiesFact.Proveedores
                                       where p.NombreProveedor.Contains(busqueda)
                                       select new
                                       {
                                           p.PKProveedorID,
                                           p.NombreProveedor,
                                           p.NombreContacto,
                                           p.TelefonoContacto1,
                                           p.TelefonoContacto2,
                                           p.EmailContacto1,
                                           p.EmailContacto2,
                                           p.Direccion,
                                           Est = p.Estado == true ? "Activo" : "Inactivo"
                                       };
                    dgvProveedores.DataSource = tProveedor.CopyAnonymusToDataTable();
                    dgvProveedores.AutoResizeColumns();
                    return;
                case "Contacto":
                    var tContacto = from p in entitiesFact.Proveedores
                                     where p.NombreContacto.Contains(busqueda)
                                    select new
                                     {
                                         p.PKProveedorID,
                                         p.NombreProveedor,
                                         p.NombreContacto,
                                         p.TelefonoContacto1,
                                         p.TelefonoContacto2,
                                         p.EmailContacto1,
                                         p.EmailContacto2,
                                         p.Direccion,
                                         Est = p.Estado == true ? "Activo" : "Inactivo"
                                     };
                    dgvProveedores.DataSource = tContacto.CopyAnonymusToDataTable();
                    dgvProveedores.AutoResizeColumns();
                    return;
                case "Teléfono":
                    if (busqueda != "")
                    {
                        int num = 0;
                        bool numeric = int.TryParse(busqueda, out num);
                        if (numeric == true)
                        {
                            int telefono = Convert.ToInt32(busqueda);
                            var tTelefono = from p in entitiesFact.Proveedores
                                            where p.TelefonoContacto1 == telefono || p.TelefonoContacto2 == telefono
                                            select new
                                            {
                                                p.PKProveedorID,
                                                p.NombreProveedor,
                                                p.NombreContacto,
                                                p.TelefonoContacto1,
                                                p.TelefonoContacto2,
                                                p.EmailContacto1,
                                                p.EmailContacto2,
                                                p.Direccion,
                                                Est = p.Estado == true ? "Activo" : "Inactivo"
                                            };
                            dgvProveedores.DataSource = tTelefono.CopyAnonymusToDataTable();
                            dgvProveedores.AutoResizeColumns();
                        }
                    }
                    else
                    {
                        var tProveedores = from p in entitiesFact.Proveedores
                                           select new
                                           {
                                               p.PKProveedorID,
                                               p.NombreProveedor,
                                               p.NombreContacto,
                                               p.TelefonoContacto1,
                                               p.TelefonoContacto2,
                                               p.EmailContacto1,
                                               p.EmailContacto2,
                                               p.Direccion,
                                               Est = p.Estado == true ? "Activo" : "Inactivo"
                                           };
                        dgvProveedores.DataSource = tProveedores.CopyAnonymusToDataTable();
                        dgvProveedores.AutoResizeColumns();
                    }
                    return;
                case "Correo Electrónico":
                    var tCorreo = from p in entitiesFact.Proveedores
                                    where p.EmailContacto1.Contains(busqueda) || p.EmailContacto2.Contains(busqueda)
                                  select new
                                    {
                                        p.PKProveedorID,
                                        p.NombreProveedor,
                                        p.NombreContacto,
                                        p.TelefonoContacto1,
                                        p.TelefonoContacto2,
                                        p.EmailContacto1,
                                        p.EmailContacto2,
                                        p.Direccion,
                                        Est = p.Estado == true ? "Activo" : "Inactivo"
                                    };
                    dgvProveedores.DataSource = tCorreo.CopyAnonymusToDataTable();
                    dgvProveedores.AutoResizeColumns();
                    return;
                
                case "Estado":
                    bool est = false;
                    if (busqueda == "Activo" || busqueda == "activo" || busqueda == "ACTIVO" || busqueda == "act" || busqueda == "Act" || busqueda == "ACT") { est = true; } else if (busqueda == "Inactivo" || busqueda == "inactivo" || busqueda == "INACTIVO" || busqueda == "ina" || busqueda == "Ina" || busqueda == "INA") { est = false; }
                    var tEstado = from p in entitiesFact.Proveedores
                                    where p.Estado == est
                                    select new
                                    {
                                        p.PKProveedorID,
                                        p.NombreProveedor,
                                        p.NombreContacto,
                                        p.TelefonoContacto1,
                                        p.TelefonoContacto2,
                                        p.EmailContacto1,
                                        p.EmailContacto2,
                                        p.Direccion,
                                        Est = p.Estado == true ? "Activo" : "Inactivo"
                                    };
                    dgvProveedores.DataSource = tEstado.CopyAnonymusToDataTable();
                    dgvProveedores.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    if (cmbBuscar.Text == "Mostrar Todo" || cmbBuscar.Text == "")
                    {
                        var tProveedores = from p in entitiesFact.Proveedores
                                        select new
                                        {
                                            p.PKProveedorID,
                                            p.NombreProveedor,
                                            p.NombreContacto,
                                            p.TelefonoContacto1,
                                            p.TelefonoContacto2,
                                            p.EmailContacto1,
                                            p.EmailContacto2,
                                            p.Direccion,
                                            Est = p.Estado == true ? "Activo" : "Inactivo"
                                        };
                        dgvProveedores.DataSource = tProveedores.CopyAnonymusToDataTable();
                        dgvProveedores.AutoResizeColumns();
                    }
                    return;
                default:
                    var tProveedores2 = from p in entitiesFact.Proveedores
                                       select new
                                       {
                                           p.PKProveedorID,
                                           p.NombreProveedor,
                                           p.NombreContacto,
                                           p.TelefonoContacto1,
                                           p.TelefonoContacto2,
                                           p.EmailContacto1,
                                           p.EmailContacto2,
                                           p.Direccion,
                                           Est = p.Estado == true ? "Activo" : "Inactivo"
                                       };
                    dgvProveedores.DataSource = tProveedores2.CopyAnonymusToDataTable();
                    dgvProveedores.AutoResizeColumns();
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
                var tProveedores = from p in entitiesFact.Proveedores
                                   select new
                                   {
                                       p.PKProveedorID,
                                       p.NombreProveedor,
                                       p.NombreContacto,
                                       p.TelefonoContacto1,
                                       p.TelefonoContacto2,
                                       p.EmailContacto1,
                                       p.EmailContacto2,
                                       p.Direccion,
                                       Est = p.Estado == true ? "Activo" : "Inactivo"
                                   };
                dgvProveedores.DataSource = tProveedores.CopyAnonymusToDataTable();
                dgvProveedores.AutoResizeColumns();
            } else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
    }
}
