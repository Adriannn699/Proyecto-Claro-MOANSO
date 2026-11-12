using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Proyecto_Claro_MOANSO
{
    public partial class Form1 : Form
    {
        private int idDisponibilidadSeleccionada = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // CARGAR COLABORADORES
                cboColaborador.DisplayMember = "nombre";
                cboColaborador.ValueMember = "id_colaborador";
                cboColaborador.DataSource =
                    DisponibilidadNegocio.ListarColaboradores();

                // CARGAR PERIODOS
                cboPeriodo.DisplayMember = "nombre_periodo";
                cboPeriodo.ValueMember = "id_periodo";
                cboPeriodo.DataSource =
                    DisponibilidadNegocio.ListarPeriodos();

                // CARGAR DIAS DE LA SEMANA
                cboDia.Items.Clear();

                cboDia.Items.AddRange(new string[]
                {
                "Lunes",
                "Martes",
                "Miércoles",
                "Jueves",
                "Viernes",
                "Sábado",
                "Domingo"
                });

                cboDia.SelectedIndex = 0;

                // CARGAR TABLA DE DISPONIBILIDADES
                dgvDisponibilidades.DataSource =
                    DisponibilidadNegocio.Listar();

                dgvDisponibilidades.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los datos: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {

            try
            {
                if (cboColaborador.SelectedValue == null ||
                    cboPeriodo.SelectedValue == null ||
                    cboDia.SelectedIndex == -1)
                {
                    MessageBox.Show("Complete todos los campos.");
                    return;
                }

                int idColaborador = Convert.ToInt32(cboColaborador.SelectedValue);
                int idPeriodo = Convert.ToInt32(cboPeriodo.SelectedValue);
                int diaSemana = cboDia.SelectedIndex + 1;

                TimeSpan horaInicio = dtpInicio.Value.TimeOfDay;
                TimeSpan horaFin = dtpFin.Value.TimeOfDay;

                bool registrado = DisponibilidadNegocio.Registrar(
                    idColaborador,
                    idPeriodo,
                    diaSemana,
                    horaInicio,
                    horaFin
                );

                if (registrado)
                {
                    MessageBox.Show(
                        "Disponibilidad registrada correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dgvDisponibilidades.DataSource =
                        DisponibilidadNegocio.Listar();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo registrar. Verifique que el colaborador esté activo, " +
                        "el periodo esté abierto y no exista un horario superpuesto.",
                        "Registro no realizado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvDisponibilidades_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila =
                dgvDisponibilidades.Rows[e.RowIndex];

            idDisponibilidadSeleccionada =
                Convert.ToInt32(fila.Cells["ID"].Value);

            cboColaborador.SelectedIndex =
                cboColaborador.FindStringExact(
                    fila.Cells["Colaborador"].Value.ToString());

            cboPeriodo.SelectedIndex =
                cboPeriodo.FindStringExact(
                    fila.Cells["Periodo"].Value.ToString());

            cboDia.SelectedIndex =
                cboDia.FindStringExact(
                    fila.Cells["Dia"].Value.ToString());

            dtpInicio.Value = DateTime.Today.Add(
                TimeSpan.Parse(fila.Cells["Inicio"].Value.ToString()));

            dtpFin.Value = DateTime.Today.Add(
                TimeSpan.Parse(fila.Cells["Fin"].Value.ToString()));
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            try
            {
                if (idDisponibilidadSeleccionada <= 0)
                {
                    MessageBox.Show(
                        "Seleccione una disponibilidad de la tabla.");
                    return;
                }

                if (cboColaborador.SelectedValue == null ||
                    cboPeriodo.SelectedValue == null ||
                    cboDia.SelectedIndex == -1)
                {
                    MessageBox.Show("Complete todos los campos.");
                    return;
                }

                int idColaborador =
                    Convert.ToInt32(cboColaborador.SelectedValue);

                int idPeriodo =
                    Convert.ToInt32(cboPeriodo.SelectedValue);

                int diaSemana = cboDia.SelectedIndex + 1;

                TimeSpan horaInicio = dtpInicio.Value.TimeOfDay;
                TimeSpan horaFin = dtpFin.Value.TimeOfDay;

                bool modificado = DisponibilidadNegocio.Modificar(
                    idDisponibilidadSeleccionada,
                    idColaborador,
                    idPeriodo,
                    diaSemana,
                    horaInicio,
                    horaFin
                );

                if (modificado)
                {
                    MessageBox.Show(
                        "Disponibilidad modificada correctamente.",
                        "Modificación exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dgvDisponibilidades.DataSource =
                        DisponibilidadNegocio.Listar();

                    idDisponibilidadSeleccionada = 0;
                    dgvDisponibilidades.ClearSelection();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo modificar. Verifique que el registro esté activo, " +
                        "el periodo esté abierto y no existan horarios superpuestos.",
                        "Modificación no realizada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnAnular_Click(object sender, EventArgs e)
        {

            try
            {
                if (idDisponibilidadSeleccionada <= 0)
                {
                    MessageBox.Show(
                        "Seleccione una disponibilidad de la tabla.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Está seguro de que desea anular esta disponibilidad?",
                    "Confirmar anulación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (respuesta != DialogResult.Yes)
                    return;

                bool anulado = DisponibilidadNegocio.Anular(
                    idDisponibilidadSeleccionada
                );

                if (anulado)
                {
                    MessageBox.Show(
                        "Disponibilidad anulada correctamente.",
                        "Anulación exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dgvDisponibilidades.DataSource =
                        DisponibilidadNegocio.Listar();

                    idDisponibilidadSeleccionada = 0;
                    dgvDisponibilidades.ClearSelection();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo anular. Verifique que la disponibilidad " +
                        "esté activa y que el periodo continúe abierto.",
                        "Anulación no realizada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al anular: " + ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Quitar el registro seleccionado
            idDisponibilidadSeleccionada = 0;

            // Restablecer los ComboBox
            if (cboColaborador.Items.Count > 0)
                cboColaborador.SelectedIndex = 0;

            if (cboPeriodo.Items.Count > 0)
                cboPeriodo.SelectedIndex = 0;

            if (cboDia.Items.Count > 0)
                cboDia.SelectedIndex = 0;

            // Restablecer las horas
            dtpInicio.Value = DateTime.Today.AddHours(8);
            dtpFin.Value = DateTime.Today.AddHours(14);

            // Quitar selección de la tabla
            dgvDisponibilidades.ClearSelection();

            // Colocar el cursor en colaborador
            cboColaborador.Focus();
        }

        private void btnAsignaciones_Click(object sender, EventArgs e)
        {
            FrmAsignacionTurnos formulario = new FrmAsignacionTurnos();
            formulario.ShowDialog();
        }
    }
}
