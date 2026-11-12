using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Claro_MOANSO
{
    public partial class FrmNuevaSolicitud : System.Windows.Forms.Form
    {
        public FrmNuevaSolicitud()
        {
            InitializeComponent();

            CargarMateriales();

            btnAgregarMaterial.Click += btnAgregarMaterial_Click;
            dgvDetalle.CellContentClick += dgvDetalle_CellContentClick;
        }

        private void CargarMateriales()
        {
            try
            {
                SolicitudMaterialDAO dao = new SolicitudMaterialDAO();

                cmbMaterial.DataSource = dao.ObtenerMaterialesActivos();

                cmbMaterial.DisplayMember = "nombre";
                cmbMaterial.ValueMember = "id_material";

                cmbMaterial.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los materiales: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnAgregarMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMaterial.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione un material.");
                    return;
                }

                if (nudCantidad.Value <= 0)
                {
                    MessageBox.Show("La cantidad debe ser mayor que cero.");
                    return;
                }

                DataRowView material =
                    cmbMaterial.SelectedItem as DataRowView;

                if (material == null)
                    return;

                int idMaterial = Convert.ToInt32(
                    material["id_material"]);

                string codigo = material["codigo"].ToString();
                string nombre = material["nombre"].ToString();
                string unidad = material["unidad_medida"].ToString();

                decimal cantidad = nudCantidad.Value;

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    if (Convert.ToInt32(fila.Tag) == idMaterial)
                    {
                        decimal cantidadActual =
                            Convert.ToDecimal(fila.Cells["colCantidad"].Value);

                        fila.Cells["colCantidad"].Value =
                            cantidadActual + cantidad;

                        MessageBox.Show(
                            "Se actualizó la cantidad del material.");

                        return;
                    }
                }

                int indice = dgvDetalle.Rows.Add(
                    codigo,
                    nombre,
                    unidad,
                    cantidad,
                    "Quitar"
                );

                dgvDetalle.Rows[indice].Tag = idMaterial;

                nudCantidad.Value = 1;

                MessageBox.Show(
                    "Material agregado correctamente.",
                    "Tiendas Claro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al agregar el material: " + ex.Message);
            }
        }

        private void dgvDetalle_CellContentClick(
    object sender,
    DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvDetalle.Columns[e.ColumnIndex].Name == "colQuitar")
            {
                dgvDetalle.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void btnAgregarMaterial1_Click(object sender, EventArgs e)
        {

        }

        private void cmbMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
