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
    public partial class MNT_RangosModulos : Form
    {
        FactEntities2 entitiesFact = new FactEntities2();
        long idRM = 0;
        bool retornar = false;
        bool editar = false;
        string campoBuscar = "";
        public MNT_RangosModulos()
        {
            InitializeComponent();
        }

        private void MNT_RangosModulos_Load(object sender, EventArgs e)
        {
            var tRM = from rm in entitiesFact.RangosModulos
                      join r in entitiesFact.Rangos
                      on rm.FKRangosID equals r.PKRangoID
                      join m in entitiesFact.Modulos
                      on rm.FKModulosID equals m.PKModulosID
                      select new
                      {
                          rm.PKRangosModulos,
                          r.DescripcionRango,
                          m.DescripcionModulo
                      };

            dgvRM.DataSource = tRM.CopyAnonymusToDataTable();
            dgvRM.Columns[0].HeaderCell.Value = "Rango";
            dgvRM.Columns[1].HeaderCell.Value = "Modulo";
            dgvRM.AutoResizeColumns();

            var tRangos = from r in entitiesFact.Rangos
                             select new
                             {
                                 r.PKRangoID,
                                 r.DescripcionRango
                             };

            DataTable dtRango = tRangos.CopyAnonymusToDataTable();
            cmbRango.DataSource = dtRango;
            cmbRango.DisplayMember = dtRango.Columns[1].ColumnName;
            cmbRango.ValueMember = dtRango.Columns[0].ColumnName;

            var tModulos = from m in entitiesFact.Modulos
                          select new
                          {
                             m.PKModulosID,
                             m.DescripcionModulo
                          };

            DataTable dtModulo = tModulos.CopyAnonymusToDataTable();
            cmbModulo.DataSource = dtModulo;
            cmbModulo.DisplayMember = dtModulo.Columns[1].ColumnName;
            cmbModulo.ValueMember = dtModulo.Columns[0].ColumnName;

            cmbBuscar.Items.Add("Mostrar Todo");
            cmbBuscar.Items.Add("Rango");
            cmbBuscar.Items.Add("Módulo");

            btnCancelar.Text = "Nuevo";
        }

        private void dgvRM_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRM.RowCount > 0)
            {
                try
                {
                    idRM = Convert.ToInt64(dgvRM.SelectedCells[0].Value);
                    var dtRM = entitiesFact.RangosModulos.FirstOrDefault(x => x.PKRangosModulos == idRM);
                    cmbRango.Text = dtRM.Rangos.DescripcionRango;
                    cmbModulo.Text = dtRM.Modulos.DescripcionModulo;

                    cmbRango.DisplayMember = dtRM.Rangos.DescripcionRango;
                    cmbRango.ValueMember = dtRM.FKRangosID.ToString();
                    
                    cmbModulo.DisplayMember = dtRM.Modulos.DescripcionModulo;
                    cmbModulo.ValueMember = dtRM.FKModulosID.ToString();

                    editar = true;
                }
                catch (Exception)
                {

                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (retornar)
            {
                this.Dispose();
                editar = false;
            }
            else
            {
                cmbRango.Text = "";
                cmbModulo.Text = "";
                editar = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbRango.Text.Equals("") && cmbModulo.Text.Equals(""))
            {
                MessageBox.Show("Por favor ingresar toda la información requerida.");
                return;
            }

            if (editar)
            {
                var thRM = entitiesFact.RangosModulos.FirstOrDefault(x => x.PKRangosModulos == idRM);

                thRM.FKRangosID = Convert.ToInt16(cmbRango.SelectedValue);
                thRM.FKModulosID = Convert.ToInt16(cmbModulo.SelectedValue);

                entitiesFact.SaveChanges();
            }
            else
            {
                RangosModulos thRM = new RangosModulos();

                thRM.FKRangosID = Convert.ToInt16(cmbRango.SelectedValue);
                thRM.FKModulosID = Convert.ToInt16(cmbModulo.SelectedValue);

                entitiesFact.RangosModulos.Add(thRM);

                entitiesFact.SaveChanges();
            }
            cmbRango.SelectedIndex = -1;
            cmbRango.Text = "";
            cmbModulo.SelectedIndex = -1;
            cmbModulo.Text = "";
            editar = false;

            var tRM = from rm in entitiesFact.RangosModulos
                      join r in entitiesFact.Rangos
                      on rm.FKRangosID equals r.PKRangoID
                      join m in entitiesFact.Modulos
                      on rm.FKModulosID equals m.PKModulosID
                      select new
                      {
                          rm.PKRangosModulos,
                          r.DescripcionRango,
                          m.DescripcionModulo
                      };

            dgvRM.DataSource = tRM.CopyAnonymusToDataTable();

            MessageBox.Show("Informacion guardada!");
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text;

            switch (campoBuscar)
            {
                case "Rango":
                    var tRango = from rm in entitiesFact.RangosModulos
                              join r in entitiesFact.Rangos
                              on rm.FKRangosID equals r.PKRangoID
                              join m in entitiesFact.Modulos
                              on rm.FKModulosID equals m.PKModulosID
                              where r.DescripcionRango.Contains(busqueda)
                              select new
                              {
                                  rm.PKRangosModulos,
                                  r.DescripcionRango,
                                  m.DescripcionModulo
                              };
                    dgvRM.DataSource = tRango.CopyAnonymusToDataTable();
                    dgvRM.AutoResizeColumns();
                    return;
                case "Módulo":
                    var tModulo = from rm in entitiesFact.RangosModulos
                                 join r in entitiesFact.Rangos
                                 on rm.FKRangosID equals r.PKRangoID
                                 join m in entitiesFact.Modulos
                                 on rm.FKModulosID equals m.PKModulosID
                                 where m.DescripcionModulo.Contains(busqueda)
                                 select new
                                 {
                                     rm.PKRangosModulos,
                                     r.DescripcionRango,
                                     m.DescripcionModulo
                                 };
                    dgvRM.DataSource = tModulo.CopyAnonymusToDataTable();
                    dgvRM.AutoResizeColumns();
                    return;
                case "Mostrar Todo":
                    var tRM = from rm in entitiesFact.RangosModulos
                              join r in entitiesFact.Rangos
                              on rm.FKRangosID equals r.PKRangoID
                              join m in entitiesFact.Modulos
                              on rm.FKModulosID equals m.PKModulosID
                              select new
                              {
                                  rm.PKRangosModulos,
                                  r.DescripcionRango,
                                  m.DescripcionModulo
                              };
                    dgvRM.DataSource = tRM.CopyAnonymusToDataTable();
                    dgvRM.AutoResizeColumns();
                    return;
                default:
                    var tRM2 = from rm in entitiesFact.RangosModulos
                              join r in entitiesFact.Rangos
                              on rm.FKRangosID equals r.PKRangoID
                              join m in entitiesFact.Modulos
                              on rm.FKModulosID equals m.PKModulosID
                              select new
                              {
                                  rm.PKRangosModulos,
                                  r.DescripcionRango,
                                  m.DescripcionModulo
                              };
                    dgvRM.DataSource = tRM2.CopyAnonymusToDataTable();
                    dgvRM.AutoResizeColumns();
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
                var tRM = from rm in entitiesFact.RangosModulos
                          join r in entitiesFact.Rangos
                          on rm.FKRangosID equals r.PKRangoID
                          join m in entitiesFact.Modulos
                          on rm.FKModulosID equals m.PKModulosID
                          select new
                          {
                              rm.PKRangosModulos,
                              r.DescripcionRango,
                              m.DescripcionModulo
                          };
                dgvRM.DataSource = tRM.CopyAnonymusToDataTable();
                dgvRM.AutoResizeColumns();
            }
            else { txtBuscar.ReadOnly = false; }
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }

        private void cmbRango_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRango.DisplayMember == "" && cmbModulo.DisplayMember == "")
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (cmbRango.DisplayMember != "" || cmbModulo.DisplayMember != "" )
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
        }

        private void cmbModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRango.DisplayMember == "" && cmbModulo.DisplayMember == "")
            {
                btnCancelar.Text = "Regresar";
                retornar = true;
            }
            else if (cmbRango.DisplayMember != "" || cmbModulo.DisplayMember != "")
            {
                btnCancelar.Text = "Limpiar";
                retornar = false;
            }
        }
    }
}
