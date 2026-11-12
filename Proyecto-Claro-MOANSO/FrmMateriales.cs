using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_Claro_MOANSO
{
    public partial class FrmMateriales : Form
    {

        private int idSeleccionado = 0;
        private bool seleccionadoActivo = true;

        public FrmMateriales()
        {
            InitializeComponent();

            dgvMateriales.ReadOnly = true;
            dgvMateriales.AllowUserToAddRows = false;
            dgvMateriales.AllowUserToDeleteRows = false;
            dgvMateriales.MultiSelect = false;
            dgvMateriales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMateriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMateriales.RowHeadersVisible = false;

            cboFiltroEstado.SelectedIndex = 0;

            btnNuevo.Click += (s, e) => LimpiarFormulario();
            btnGuardar.Click += (s, e) => Guardar();
            btnCambiarEstado.Click += (s, e) => CambiarEstado();
            txtBuscar.TextChanged += (s, e) => CargarMateriales();
            cboFiltroEstado.SelectedIndexChanged += (s, e) => CargarMateriales();
            dgvMateriales.CellClick += DgvMateriales_CellClick;

            Load += (s, e) =>
            {
                CargarMateriales();
                LimpiarFormulario();
            };
        }


        private void CargarMateriales()
        {
            int filtro = cboFiltroEstado.SelectedIndex == 1 ? 1
                       : cboFiltroEstado.SelectedIndex == 2 ? 0
                       : -1;

            string sql = @"SELECT id_material AS Id,
                                  codigo AS [Código],
                                  nombre AS Nombre,
                                  categoria AS [Categoría],
                                  unidad_medida AS Unidad,
                                  CASE WHEN estado = 1 THEN 'Activo' ELSE 'Inhabilitado' END AS Estado,
                                  descripcion AS [Descripción]
                           FROM inventario.Material
                           WHERE (codigo LIKE @b OR nombre LIKE @b OR ISNULL(categoria, '') LIKE @b)
                             AND (@estado = -1 OR estado = @estado)
                           ORDER BY nombre";
            try
            {
                using (var cn = ConexionBD.ObtenerConexion())
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@b", "%" + txtBuscar.Text.Trim() + "%");
                    da.SelectCommand.Parameters.AddWithValue("@estado", filtro);
                    var tabla = new DataTable();
                    da.Fill(tabla);
                    dgvMateriales.DataSource = tabla;
                    dgvMateriales.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el catálogo: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMateriales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = dgvMateriales.Rows[e.RowIndex];

            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtCodigo.Text = fila.Cells["Código"].Value.ToString();
            txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
            cboCategoria.Text = fila.Cells["Categoría"].Value.ToString();
            cboUnidad.Text = fila.Cells["Unidad"].Value.ToString();
            txtDescripcion.Text = fila.Cells["Descripción"].Value.ToString();

            seleccionadoActivo = fila.Cells["Estado"].Value.ToString() == "Activo";
            btnCambiarEstado.Text = seleccionadoActivo ? "Inhabilitar" : "Reactivar";
            btnCambiarEstado.Enabled = true;
        }

        private object ONulo(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? (object)DBNull.Value : texto.Trim();
        }

        private void Guardar()
        {
            string codigo = txtCodigo.Text.Trim().ToUpper();
            string nombre = txtNombre.Text.Trim();

            if (codigo == "" || nombre == "" || cboUnidad.SelectedIndex < 0)
            {
                MessageBox.Show("Completa el código, el nombre y la unidad de medida.", "Faltan datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var cn = ConexionBD.ObtenerConexion())
                {
                    cn.Open();

                    using (var cmdDup = new SqlCommand(
                        "SELECT COUNT(*) FROM inventario.Material WHERE codigo = @codigo AND id_material <> @id", cn))
                    {
                        cmdDup.Parameters.AddWithValue("@codigo", codigo);
                        cmdDup.Parameters.AddWithValue("@id", idSeleccionado);
                        if ((int)cmdDup.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Ya existe un material con el código " + codigo + ".", "Código duplicado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string sql = idSeleccionado == 0
                        ? @"INSERT INTO inventario.Material (codigo, nombre, descripcion, categoria, unidad_medida)
                            VALUES (@codigo, @nombre, @descripcion, @categoria, @unidad)"
                        : @"UPDATE inventario.Material
                            SET codigo = @codigo, nombre = @nombre, descripcion = @descripcion,
                                categoria = @categoria, unidad_medida = @unidad,
                                fecha_modificacion = GETDATE()
                            WHERE id_material = @id";

                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@descripcion", ONulo(txtDescripcion.Text));
                        cmd.Parameters.AddWithValue("@categoria", ONulo(cboCategoria.Text));
                        cmd.Parameters.AddWithValue("@unidad", cboUnidad.Text);
                        cmd.Parameters.AddWithValue("@id", idSeleccionado);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(idSeleccionado == 0 ? "Material registrado." : "Material actualizado.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarMateriales();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CambiarEstado()
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un material de la tabla.", "Sin selección",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (seleccionadoActivo)
            {
                var resp = MessageBox.Show("¿Inhabilitar el material " + txtCodigo.Text + "?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resp != DialogResult.Yes) return;
            }

            try
            {
                using (var cn = ConexionBD.ObtenerConexion())
                using (var cmd = new SqlCommand(
                    @"UPDATE inventario.Material
                      SET estado = @estado, fecha_modificacion = GETDATE()
                      WHERE id_material = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@estado", seleccionadoActivo ? 0 : 1);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                CargarMateriales();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            seleccionadoActivo = true;
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            cboCategoria.Text = "";
            cboUnidad.SelectedIndex = -1;
            btnCambiarEstado.Text = "Inhabilitar";
            btnCambiarEstado.Enabled = false;
            txtCodigo.Focus();
        }
    }
}