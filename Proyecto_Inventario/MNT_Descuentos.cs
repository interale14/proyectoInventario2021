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
    public partial class MNT_Descuentos : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        int back = 0;
        long idUsuario = 0;
        int rango = 0;
        long idDescuento = 0;
        bool retornar = false;
        bool editar = false;
        public MNT_Descuentos(int _back, long _idUsuario, int _rango)
        {
            InitializeComponent();
            back = _back;
            idUsuario = _idUsuario;
            rango = _rango;
        }

        private void MNT_Descuentos_Load(object sender, EventArgs e)
        {
            var tDescuentos = from d in entitiesFact.Descuentos
                             select new
                             {
                                 d.PKDescuentoID,
                                 d.NombreDescuento,
                                 d.CantidadDescuento,
                                 Est = d.Estado == true ? "Activo" : "Inactivo"
                             };

            dgvDescuentos.DataSource = tDescuentos.CopyAnonymusToDataTable();
            dgvDescuentos.Columns[0].HeaderCell.Value = "ID";
            dgvDescuentos.Columns[1].HeaderCell.Value = "Descripción";
            dgvDescuentos.Columns[2].HeaderCell.Value = "Cantidad";
            dgvDescuentos.Columns[3].HeaderCell.Value = "Estado";
            dgvDescuentos.AutoResizeColumns();

            btnCancelar.Text = "Nuevo";
        }

        private void dgvDescuentos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDescuentos.RowCount > 0)
            {
                try
                {
                    idDescuento = Convert.ToInt64(dgvDescuentos.SelectedCells[0].Value);
                    var tDescuento = entitiesFact.Descuentos.FirstOrDefault(x => x.PKDescuentoID == idDescuento);

                    txtDesc.Text = tDescuento.NombreDescuento;
                    txtCantidad.Text = tDescuento.CantidadDescuento;
                    cbEstado.Checked = tDescuento.Estado;
                    editar = true;
                }
                catch (Exception)
                {

                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtDesc.Text.Equals("") && txtCantidad.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thDescuentos = entitiesFact.Descuentos.FirstOrDefault(x => x.PKDescuentoID == idDescuento);
                thDescuentos.NombreDescuento = txtDesc.Text;
                thDescuentos.CantidadDescuento = txtCantidad.Text;
                thDescuentos.Estado = cbEstado.Checked;

                entitiesFact.SaveChanges();
            }
            else
            {

                Descuentos tbDescuentos = new Descuentos();
                tbDescuentos.NombreDescuento = txtDesc.Text;
                tbDescuentos.CantidadDescuento = txtCantidad.Text;
                tbDescuentos.Estado = cbEstado.Checked;
                entitiesFact.Descuentos.Add(tbDescuentos);

                entitiesFact.SaveChanges();
            }
            txtDesc.Text = "";
            txtCantidad.Text = "";
            cbEstado.Checked = false;
            editar = false;

            var tDescuentos = from d in entitiesFact.Descuentos
                              select new
                              {
                                  d.PKDescuentoID,
                                  d.NombreDescuento,
                                  d.CantidadDescuento,
                                  Est = d.Estado == true ? "Activo" : "Inactivo"
                              };
            dgvDescuentos.DataSource = tDescuentos.CopyAnonymusToDataTable();

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
                txtCantidad.Text = "";
                cbEstado.Checked = false;
                editar = false;
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtCantidad.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtCantidad.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtCantidad.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtCantidad.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtCantidad.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtCantidad.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void cbEstado_CheckedChanged(object sender, EventArgs e)
        {
            if (txtDesc.Text == "" && txtCantidad.Text == "" && cbEstado.Checked == false)
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (txtDesc.Text != "" || txtCantidad.Text != "" || cbEstado.Checked == true)
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
            if (txtDesc.Text == "" || txtCantidad.Text == "")
            {
                btnGuardar.Enabled = false;
            }
            else if (txtDesc.Text != "" && txtCantidad.Text != "")
            {
                btnGuardar.Enabled = true;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscar.Text != "")
            {
                var tBusqueda = from d in entitiesFact.Descuentos
                                where d.NombreDescuento.Contains(txtBuscar.Text)
                                select new
                                {
                                    d.PKDescuentoID,
                                    d.NombreDescuento,
                                    d.CantidadDescuento,
                                    Est = d.Estado == true ? "Activo" : "Inactivo"
                                };
                dgvDescuentos.DataSource = tBusqueda.CopyAnonymusToDataTable();
            }
            else
            {
                var tBusqueda = from d in entitiesFact.Descuentos
                                select new
                                {
                                    d.PKDescuentoID,
                                    d.NombreDescuento,
                                    d.CantidadDescuento,
                                    Est = d.Estado == true ? "Activo" : "Inactivo"
                                };
                dgvDescuentos.DataSource = tBusqueda.CopyAnonymusToDataTable();
            }
        }
    }
}
