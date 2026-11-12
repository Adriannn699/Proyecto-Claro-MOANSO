using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Claro_MOANSO.Cores
{
    public partial class FrmProgramarHorarios : Form
    {
        public FrmProgramarHorarios()
        {
            InitializeComponent();
            cboColaborador.Items.AddRange(new object[]
            {
                "Juan Perez",
                "Maria Lopez",
                "Carlos Ramirez"
            });

            cboPeriodo.Items.AddRange(new object[]
            {
                "Semana 16 al 22 de noviembre",
                "Semana 23 al 29 de noviembre"
            });

            cboTurno.Items.AddRange(new object[]
            {
                "Mañana (08:00 - 14:00)",
                "Tarde (14:00 - 20:00)",
                "Noche (20:00 - 02:00)"
            });

            dgvHorarios.Columns.Clear();

            dgvHorarios.Columns.Add("Colaborador", "Colaborador");
            dgvHorarios.Columns.Add("Fecha", "Fecha");
            dgvHorarios.Columns.Add("Turno", "Turno");
            dgvHorarios.Columns.Add("Estado", "Estado");

            // Propiedades de la tabla
            dgvHorarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHorarios.AllowUserToAddRows = false;
            dgvHorarios.ReadOnly = true;
            dgvHorarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Datos de ejemplo
            dgvHorarios.Rows.Add(
                "Juan Perez",
                "16/11/2026",
                "Mañana (08:00 - 14:00)",
                "Programado"
            );

            dgvHorarios.Rows.Add(
                "Maria Lopez",
                "17/11/2026",
                "Tarde (14:00 - 20:00)",
                "Programado"
            );

            dgvHorarios.Rows.Add(
                "Carlos Ramirez",
                "18/11/2026",
                "Mañana (08:00 - 14:00)",
                "Pendiente"
            );
        }

        private void FrmProgramarHorarios_Load(object sender, EventArgs e)
        {

        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            // Validar que se hayan seleccionado los campos
            if (cboColaborador.SelectedIndex == -1 ||
                cboPeriodo.SelectedIndex == -1 ||
                cboTurno.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Seleccione un colaborador, periodo y turno.",
                    "Campos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Verificar que la fecha corresponda al periodo seleccionado
            DateTime inicioPeriodo = cboPeriodo.SelectedIndex == 0
                ? new DateTime(2026, 11, 16)
                : new DateTime(2026, 11, 23);

            DateTime finPeriodo = inicioPeriodo.AddDays(6);

            if (dtpFecha.Value.Date < inicioPeriodo ||
                dtpFecha.Value.Date > finPeriodo)
            {
                MessageBox.Show(
                    "La fecha seleccionada no pertenece al periodo.",
                    "Fecha incorrecta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string colaborador = cboColaborador.Text;
            string fecha = dtpFecha.Value.ToString("dd/MM/yyyy");

            // Comprobar los horarios de ejemplo
            foreach (DataGridViewRow fila in dgvHorarios.Rows)
            {
                if (fila.Cells["Colaborador"].Value?.ToString() == colaborador &&
                    fila.Cells["Fecha"].Value?.ToString() == fecha)
                {
                    MessageBox.Show(
                        "El colaborador ya tiene un horario asignado para esta fecha.",
                        "Horario encontrado (simulación)",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            MessageBox.Show(
                "No se encontró una asignación para esta fecha en los datos de ejemplo.",
                "Verificación simulada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validar campos obligatorios
            if (cboColaborador.SelectedIndex == -1 ||
                cboPeriodo.SelectedIndex == -1 ||
                cboTurno.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Complete todos los campos antes de agregar.",
                    "Campos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Validar que la fecha pertenezca al periodo
            DateTime inicioPeriodo = cboPeriodo.SelectedIndex == 0
                ? new DateTime(2026, 11, 16)
                : new DateTime(2026, 11, 23);

            DateTime finPeriodo = inicioPeriodo.AddDays(6);

            if (dtpFecha.Value.Date < inicioPeriodo ||
                dtpFecha.Value.Date > finPeriodo)
            {
                MessageBox.Show(
                    "La fecha seleccionada no pertenece al periodo.",
                    "Fecha incorrecta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string colaborador = cboColaborador.Text;
            string fecha = dtpFecha.Value.ToString("dd/MM/yyyy");
            string turno = cboTurno.Text;

            // Evitar asignaciones duplicadas en la misma fecha
            foreach (DataGridViewRow fila in dgvHorarios.Rows)
            {
                if (fila.Cells["Colaborador"].Value?.ToString() == colaborador &&
                    fila.Cells["Fecha"].Value?.ToString() == fecha)
                {
                    MessageBox.Show(
                        "El colaborador ya tiene una asignación para esta fecha.",
                        "Asignación duplicada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            // Agregar asignación simulada a la tabla
            dgvHorarios.Rows.Add(
                colaborador,
                fecha,
                turno,
                "Pendiente"
            );

            MessageBox.Show(
                "Asignación agregada correctamente (simulación).",
                "Operación realizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnPublicar_Click(object sender, EventArgs e)
        {
            int pendientes = 0;

            // Contar las asignaciones pendientes
            foreach (DataGridViewRow fila in dgvHorarios.Rows)
            {
                if (fila.Cells["Estado"].Value?.ToString() == "Pendiente")
                {
                    pendientes++;
                }
            }

            if (pendientes == 0)
            {
                MessageBox.Show(
                    "No existen horarios pendientes de publicación.",
                    "Sin pendientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Solicitar confirmación
            DialogResult respuesta = MessageBox.Show(
                $"¿Desea publicar las {pendientes} asignaciones pendientes?",
                "Confirmar publicación (simulación)",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
                return;

            // Publicar las asignaciones
            foreach (DataGridViewRow fila in dgvHorarios.Rows)
            {
                if (fila.Cells["Estado"].Value?.ToString() == "Pendiente")
                {
                    fila.Cells["Estado"].Value = "Programado";
                }
            }

            MessageBox.Show(
                "Los horarios se publicaron correctamente (simulación).",
                "Publicación completada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar las selecciones
            cboColaborador.SelectedIndex = -1;
            cboPeriodo.SelectedIndex = -1;
            cboTurno.SelectedIndex = -1;

            // Restablecer la fecha de ejemplo
            dtpFecha.Value = new DateTime(2026, 11, 16);

            // Quitar la selección de la tabla
            dgvHorarios.ClearSelection();

            // Colocar el cursor en el primer campo
            cboColaborador.Focus();
        }
    }
}
