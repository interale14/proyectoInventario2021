using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Proyecto_Inventario
{
    public partial class MNT_Empleados : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        long idEmpleado = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_Empleados(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_Empleados_Load(object sender, EventArgs e)
        {
            var tEmpleados = from p in entitiesFact.Empleados
                             join r in entitiesFact.Rangos
                             on p.FKRango equals r.PKRangoID
                             select new
                             {
                                 p.PKEmpleadoID,
                                 p.NombreEmpleado,
                                 p.ApellidoEmpleado,
                                 p.NombreUsuario,
                                 p.Correo,
                                 p.Contrasena,
                                 p.Genero,
                                 r.DescripcionRango,
                                 Est = p.Estado == true ? "Activo" : "Inactivo"
                             };

            var tRangos = from r in entitiesFact.Rangos
                          select new
                          {
                              r.PKRangoID,
                              r.DescripcionRango,
                          };

            dgvEmpleados.DataSource = tEmpleados.CopyAnonymusToDataTable();
            dgvEmpleados.Columns[0].HeaderCell.Value = "ID";
            dgvEmpleados.Columns[1].HeaderCell.Value = "Nombre del Empleado";
            dgvEmpleados.Columns[2].HeaderCell.Value = "Apellido del Empleado";
            dgvEmpleados.Columns[3].HeaderCell.Value = "Usuario";
            dgvEmpleados.Columns[4].HeaderCell.Value = "Correo";
            dgvEmpleados.Columns[5].HeaderCell.Value = "Contraseña";
            dgvEmpleados.Columns[6].HeaderCell.Value = "Género";
            dgvEmpleados.Columns[7].HeaderCell.Value = "Rango";
            dgvEmpleados.Columns[8].HeaderCell.Value = "Estado";
            dgvEmpleados.AutoResizeColumns();

            DataTable dtRango = tRangos.CopyAnonymusToDataTable();
            cmbRango.DataSource = dtRango;
            cmbRango.DisplayMember = dtRango.Columns[1].ColumnName;
            cmbRango.ValueMember = dtRango.Columns[0].ColumnName;

            cmbGenero.Items.Add("Masculino");
            cmbGenero.Items.Add("Femenino");

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("ID");
            cmbBuscar.Items.Add("Nombre");
            cmbBuscar.Items.Add("Apellido");
            cmbBuscar.Items.Add("Usuario");
            cmbBuscar.Items.Add("Correo Electrónico");
            cmbBuscar.Items.Add("Género");
            cmbBuscar.Items.Add("Rango");
            cmbBuscar.Items.Add("Estado");

            btnCancelar.Text = "Nuevo";
        }

        private void dgvEmpleados_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmpleados.RowCount > 0)
            {
                try
                {
                    idEmpleado = Convert.ToInt64(dgvEmpleados.SelectedCells[0].Value);
                    var tEmpleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idEmpleado);
                    var tRango = entitiesFact.Rangos.FirstOrDefault(x => x.PKRangoID == tEmpleados.FKRango);
                    txtNombreEmpleado.Text = tEmpleados.NombreEmpleado;
                    txtApellidoEmpleado.Text = tEmpleados.ApellidoEmpleado;
                    txtUsuario.Text = tEmpleados.NombreUsuario;
                    txtCorreo.Text = tEmpleados.Correo;
                    txtContrasena.Text = tEmpleados.Contrasena;
                    cmbGenero.Text = tEmpleados.Genero;
                    cmbGenero.DisplayMember = tEmpleados.Genero;
                    cmbRango.Text = tRango.DescripcionRango;
                    cmbRango.DisplayMember = tRango.DescripcionRango;
                    cmbRango.ValueMember = Convert.ToString(tRango.PKRangoID);
                    cbEstado.Checked = tEmpleados.Estado;

                }
                catch (Exception)
                {

                }
            }
            editar = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text.Equals("") && txtApellidoEmpleado.Text.Equals("") && txtUsuario.Text.Equals("") && txtCorreo.Text.Equals("") && txtContrasena.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thEmpleados = entitiesFact.Empleados.FirstOrDefault(x => x.PKEmpleadoID == idEmpleado);
                thEmpleados.NombreEmpleado = txtNombreEmpleado.Text;
                thEmpleados.ApellidoEmpleado = txtApellidoEmpleado.Text;
                thEmpleados.NombreUsuario = txtUsuario.Text;
                thEmpleados.Correo = txtCorreo.Text;
                thEmpleados.Contrasena = Hash.obtenerHash256(txtContrasena.Text);
                thEmpleados.Genero = cmbGenero.SelectedItem.ToString();
                thEmpleados.FKRango = Convert.ToByte(cmbRango.SelectedIndex + 1);
                thEmpleados.Estado = cbEstado.Checked;

                entitiesFact.SaveChanges();
            }
            else
            {

                Empleados tbEmpleados = new Empleados();
                tbEmpleados.NombreEmpleado = txtNombreEmpleado.Text;
                tbEmpleados.ApellidoEmpleado = txtApellidoEmpleado.Text;
                tbEmpleados.NombreUsuario = txtUsuario.Text;
                tbEmpleados.Correo = txtCorreo.Text;
                tbEmpleados.Contrasena = Hash.obtenerHash256(txtContrasena.Text);
                tbEmpleados.Genero = cmbGenero.SelectedItem.ToString();
                tbEmpleados.FKRango = Convert.ToByte(cmbRango.SelectedIndex + 1);
                tbEmpleados.Estado = cbEstado.Checked;
                entitiesFact.Empleados.Add(tbEmpleados);

                entitiesFact.SaveChanges();
            }
            txtNombreEmpleado.Text = "";
            txtApellidoEmpleado.Text = "";
            txtUsuario.Text = "";
            txtCorreo.Text = "";
            txtContrasena.Text = "";
            cmbGenero.Text = "";
            cmbRango.Text = "";
            cbEstado.Checked = false;
            idEmpleado = 0;
            editar = false;

            var tEmpleados = from p in entitiesFact.Empleados
                             join r in entitiesFact.Rangos
                             on p.FKRango equals r.PKRangoID
                             select new
                             {
                                 p.PKEmpleadoID,
                                 p.NombreEmpleado,
                                 p.ApellidoEmpleado,
                                 p.NombreUsuario,
                                 p.Correo,
                                 p.Contrasena,
                                 p.Genero,
                                 r.DescripcionRango,
                                 Est = p.Estado == true ? "Activo" : "Inactivo"
                             };
            dgvEmpleados.DataSource = tEmpleados.CopyAnonymusToDataTable();

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
                    case 2:
                        MenuGerente form2 = new MenuGerente(idUsuario, rango);
                        this.Dispose();
                        form2.Show();
                        return;
                    
                }
            }
            else
            {
                txtNombreEmpleado.Text = "";
                txtApellidoEmpleado.Text = "";
                txtUsuario.Text = "";
                txtCorreo.Text = "";
                txtContrasena.Text = "";
                cmbGenero.Text = "";
                cmbGenero.SelectedIndex = -1;
                cmbRango.Text = "";
                cmbRango.SelectedIndex = -1;
                cbEstado.Checked = false;
                editar = false;
                retornar = true;
            }
        }

        private void txtNombreEmpleado_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtContacto_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtContrasena_TextChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "" || txtContrasena.Text.Length < 6)
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "" && txtContrasena.Text.Length >= 6)
            {
                btnGuardar.Enabled = true;
            }
        }
        private void cmbGenero_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }
        private void cmbRango_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" && cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
            if (txtNombreEmpleado.Text == "" || txtApellidoEmpleado.Text == "" || txtUsuario.Text == "" || txtCorreo.Text == "" || txtContrasena.Text == "" || cmbGenero.Text == "" || cmbRango.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtNombreEmpleado.Text != "" && txtApellidoEmpleado.Text != "" && txtUsuario.Text != "" && txtCorreo.Text != "" && txtContrasena.Text != "" && cmbGenero.Text != "" && cmbRango.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void cbEstado_CheckedChanged(object sender, EventArgs e)
        {
            if (txtNombreEmpleado.Text == "" && txtApellidoEmpleado.Text == "" && txtUsuario.Text == "" && txtCorreo.Text == "" && txtContrasena.Text == "" &&cmbRango.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                btnGuardar.Enabled = false;
                retornar = true;
            }
            else if (txtNombreEmpleado.Text != "" || txtApellidoEmpleado.Text != "" || txtUsuario.Text != "" || txtCorreo.Text != "" || txtContrasena.Text != "" || cmbRango.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                btnGuardar.Enabled = true;
                retornar = false;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text;

            switch (campoBuscar)
            {
                case "ID":
                    int id = 1;
                    if(busqueda != "")
                    {
                        id = Convert.ToInt32(busqueda);
                        var tID = from p in entitiesFact.Empleados
                                  join r in entitiesFact.Rangos
                                  on p.FKRango equals r.PKRangoID
                                  where p.PKEmpleadoID == id
                                  select new
                                  {
                                      p.PKEmpleadoID,
                                      p.NombreEmpleado,
                                      p.ApellidoEmpleado,
                                      p.NombreUsuario,
                                      p.Correo,
                                      p.Contrasena,
                                      p.Genero,
                                      r.DescripcionRango,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                        dgvEmpleados.DataSource = tID.CopyAnonymusToDataTable();
                    }
                    else
                    {
                        var tID2 = from p in entitiesFact.Empleados
                                          join r in entitiesFact.Rangos
                                          on p.FKRango equals r.PKRangoID
                                          select new
                                          {
                                              p.PKEmpleadoID,
                                              p.NombreEmpleado,
                                              p.ApellidoEmpleado,
                                              p.NombreUsuario,
                                              p.Correo,
                                              p.Contrasena,
                                              p.Genero,
                                              r.DescripcionRango,
                                              Est = p.Estado == true ? "Activo" : "Inactivo"
                                          };
                        dgvEmpleados.DataSource = tID2.CopyAnonymusToDataTable();
                    }
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Nombre":
                    var tNombre = from p in entitiesFact.Empleados
                                  join r in entitiesFact.Rangos
                                  on p.FKRango equals r.PKRangoID
                                  where p.NombreEmpleado.Contains(busqueda)
                                  select new
                                  {
                                      p.PKEmpleadoID,
                                      p.NombreEmpleado,
                                      p.ApellidoEmpleado,
                                      p.NombreUsuario,
                                      p.Correo,
                                      p.Contrasena,
                                      p.Genero,
                                      r.DescripcionRango,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvEmpleados.DataSource = tNombre.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Apellido":
                    var tApellido = from p in entitiesFact.Empleados
                                    join r in entitiesFact.Rangos
                                    on p.FKRango equals r.PKRangoID
                                    where p.ApellidoEmpleado.Contains(busqueda)
                                    select new
                                    {
                                        p.PKEmpleadoID,
                                        p.NombreEmpleado,
                                        p.ApellidoEmpleado,
                                        p.NombreUsuario,
                                        p.Correo,
                                        p.Contrasena,
                                        p.Genero,
                                        r.DescripcionRango,
                                        Est = p.Estado == true ? "Activo" : "Inactivo"
                                    };
                    dgvEmpleados.DataSource = tApellido.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Usuario":
                    var tUsuario = from p in entitiesFact.Empleados
                                   join r in entitiesFact.Rangos
                                   on p.FKRango equals r.PKRangoID
                                   where p.NombreUsuario.Contains(busqueda)
                                   select new
                                   {
                                       p.PKEmpleadoID,
                                       p.NombreEmpleado,
                                       p.ApellidoEmpleado,
                                       p.NombreUsuario,
                                       p.Correo,
                                       p.Contrasena,
                                       p.Genero,
                                       r.DescripcionRango,
                                       Est = p.Estado == true ? "Activo" : "Inactivo"
                                   };
                    dgvEmpleados.DataSource = tUsuario.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Correo Electrónico":
                    var tCorreo = from p in entitiesFact.Empleados
                                  join r in entitiesFact.Rangos
                                  on p.FKRango equals r.PKRangoID
                                  where p.Correo.Contains(busqueda)
                                  select new
                                  {
                                      p.PKEmpleadoID,
                                      p.NombreEmpleado,
                                      p.ApellidoEmpleado,
                                      p.NombreUsuario,
                                      p.Correo,
                                      p.Contrasena,
                                      p.Genero,
                                      r.DescripcionRango,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvEmpleados.DataSource = tCorreo.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Género":
                    var tGenero = from p in entitiesFact.Empleados
                                  join r in entitiesFact.Rangos
                                  on p.FKRango equals r.PKRangoID
                                  where p.Genero.Contains(busqueda)
                                  select new
                                  {
                                      p.PKEmpleadoID,
                                      p.NombreEmpleado,
                                      p.ApellidoEmpleado,
                                      p.NombreUsuario,
                                      p.Correo,
                                      p.Contrasena,
                                      p.Genero,
                                      r.DescripcionRango,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvEmpleados.DataSource = tGenero.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Rango":
                    var tRangoEmpleado = from p in entitiesFact.Empleados
                                         join r in entitiesFact.Rangos
                                         on p.FKRango equals r.PKRangoID
                                         where r.DescripcionRango.Contains(busqueda)
                                         select new
                                         {
                                             p.PKEmpleadoID,
                                             p.NombreEmpleado,
                                             p.ApellidoEmpleado,
                                             p.NombreUsuario,
                                             p.Correo,
                                             p.Contrasena,
                                             p.Genero,
                                             r.DescripcionRango,
                                             Est = p.Estado == true ? "Activo" : "Inactivo"
                                         };
                    dgvEmpleados.DataSource = tRangoEmpleado.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Estado":
                    bool est = false;
                    if (busqueda == "Activo" || busqueda == "activo" || busqueda == "ACTIVO" || busqueda == "act" || busqueda == "Act" || busqueda == "ACT") { est = true; } else if (busqueda == "Inactivo" || busqueda == "inactivo" || busqueda == "INACTIVO" || busqueda == "ina" || busqueda == "Ina" || busqueda == "INA") { est = false; }
                    var tEstado = from p in entitiesFact.Empleados
                                  join r in entitiesFact.Rangos
                                  on p.FKRango equals r.PKRangoID
                                  where p.Estado == est
                                  select new
                                  {
                                      p.PKEmpleadoID,
                                      p.NombreEmpleado,
                                      p.ApellidoEmpleado,
                                      p.NombreUsuario,
                                      p.Correo,
                                      p.Contrasena,
                                      p.Genero,
                                      r.DescripcionRango,
                                      Est = p.Estado == true ? "Activo" : "Inactivo"
                                  };
                    dgvEmpleados.DataSource = tEstado.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    if (cmbBuscar.Text == "Mostrar Todo" || cmbBuscar.Text == "")
                    {
                        var tEmpleados = from p in entitiesFact.Empleados
                                         join r in entitiesFact.Rangos
                                         on p.FKRango equals r.PKRangoID
                                         select new
                                         {
                                             p.PKEmpleadoID,
                                             p.NombreEmpleado,
                                             p.ApellidoEmpleado,
                                             p.NombreUsuario,
                                             p.Correo,
                                             p.Contrasena,
                                             p.Genero,
                                             r.DescripcionRango,
                                             Est = p.Estado == true ? "Activo" : "Inactivo"
                                         };
                        dgvEmpleados.DataSource = tEmpleados.CopyAnonymusToDataTable();
                        dgvEmpleados.AutoResizeColumns();
                    }
                    return;
                default:
                    var tEmpleados2 = from p in entitiesFact.Empleados
                                      join r in entitiesFact.Rangos
                                      on p.FKRango equals r.PKRangoID
                                      select new
                                      {
                                          p.PKEmpleadoID,
                                          p.NombreEmpleado,
                                          p.ApellidoEmpleado,
                                          p.NombreUsuario,
                                          p.Correo,
                                          p.Contrasena,
                                          p.Genero,
                                          r.DescripcionRango,
                                          Est = p.Estado == true ? "Activo" : "Inactivo"
                                      };
                    dgvEmpleados.DataSource = tEmpleados2.CopyAnonymusToDataTable();
                    dgvEmpleados.AutoResizeColumns();
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
                var tEmpleados = from p in entitiesFact.Empleados
                                 join r in entitiesFact.Rangos
                                 on p.FKRango equals r.PKRangoID
                                 select new
                                 {
                                     p.PKEmpleadoID,
                                     p.NombreEmpleado,
                                     p.ApellidoEmpleado,
                                     p.NombreUsuario,
                                     p.Correo,
                                     p.Contrasena,
                                     p.Genero,
                                     r.DescripcionRango,
                                     Est = p.Estado == true ? "Activo" : "Inactivo"
                                 };
                dgvEmpleados.DataSource = tEmpleados.CopyAnonymusToDataTable();
                dgvEmpleados.AutoResizeColumns();
            } 
            else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }

    }
}
