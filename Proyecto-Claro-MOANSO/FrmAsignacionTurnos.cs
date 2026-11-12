
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_Claro_MOANSO
{
    public partial class FrmAsignacionTurnos : Form
    {
        private ComboBox cboColaborador;
        private ComboBox cboPeriodo;
        private DateTimePicker dtpFecha;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFin;
        private DataGridView dgvTurnos;
        private TextBox txtMotivo;
        private Label lblSeleccion;

        private int idSeleccionado = 0;

        public FrmAsignacionTurnos()
        {
            InitializeComponent();
            CrearInterfaz();

            try
            {
                CargarColaboradores();
                CargarPeriodos();
                CargarTurnos();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void CrearInterfaz()
        {
            Text = "Gestión de Asignaciones de Turnos";
            Size = new Size(1100, 730);
            MinimumSize = new Size(950, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 75;
            encabezado.BackColor = Color.FromArgb(139, 0, 0);
            Controls.Add(encabezado);

            Label titulo = new Label();
            titulo.Text = "GESTIÓN DE ASIGNACIONES DE TURNOS";
            titulo.ForeColor = Color.White;
            titulo.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            titulo.AutoSize = true;
            titulo.Location = new Point(25, 20);
            encabezado.Controls.Add(titulo);

            TableLayoutPanel principal = new TableLayoutPanel();
            principal.Dock = DockStyle.Fill;
            principal.Padding = new Padding(20);
            principal.ColumnCount = 1;
            principal.RowCount = 4;

            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            principal.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Controls.Add(principal);
            principal.BringToFront();

            TableLayoutPanel campos = new TableLayoutPanel();
            campos.Dock = DockStyle.Fill;
            campos.ColumnCount = 3;
            campos.RowCount = 4;

            for (int i = 0; i < 3; i++)
                campos.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.33f));

            for (int i = 0; i < 4; i++)
                campos.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 25));

            principal.Controls.Add(campos, 0, 0);

            cboColaborador = new ComboBox();
            cboColaborador.DropDownStyle = ComboBoxStyle.DropDownList;
            cboColaborador.Dock = DockStyle.Fill;

            cboPeriodo = new ComboBox();
            cboPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPeriodo.Dock = DockStyle.Fill;

            dtpFecha = new DateTimePicker();
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Dock = DockStyle.Fill;

            dtpInicio = CrearSelectorHora();
            dtpFin = CrearSelectorHora();

            campos.Controls.Add(CrearEtiqueta("Colaborador"), 0, 0);
            campos.Controls.Add(CrearEtiqueta("Periodo"), 1, 0);
            campos.Controls.Add(CrearEtiqueta("Fecha del turno"), 2, 0);

            campos.Controls.Add(cboColaborador, 0, 1);
            campos.Controls.Add(cboPeriodo, 1, 1);
            campos.Controls.Add(dtpFecha, 2, 1);

            campos.Controls.Add(CrearEtiqueta("Hora de inicio"), 0, 2);
            campos.Controls.Add(CrearEtiqueta("Hora de fin"), 1, 2);

            campos.Controls.Add(dtpInicio, 0, 3);
            campos.Controls.Add(dtpFin, 1, 3);

            lblSeleccion = CrearEtiqueta("Ningún turno seleccionado");
            campos.Controls.Add(lblSeleccion, 2, 3);

            FlowLayoutPanel botones = new FlowLayoutPanel();
            botones.Dock = DockStyle.Fill;
            botones.WrapContents = true;
            principal.Controls.Add(botones, 0, 1);

            AgregarBoton(botones, "Registrar", Color.SeaGreen,
                (s, e) => RegistrarTurno());

            AgregarBoton(botones, "Modificar", Color.SteelBlue,
                (s, e) => ModificarTurno());

            AgregarBoton(botones, "Cancelar", Color.IndianRed,
                (s, e) => CancelarTurno());

            AgregarBoton(botones, "Actualizar", Color.DimGray,
                (s, e) => EjecutarSeguro(CargarTurnos));

            AgregarBoton(botones, "Ver historial", Color.DarkSlateBlue,
                (s, e) => VerHistorial());

            dgvTurnos = new DataGridView();
            dgvTurnos.Dock = DockStyle.Fill;
            dgvTurnos.ReadOnly = true;
            dgvTurnos.AllowUserToAddRows = false;
            dgvTurnos.AllowUserToDeleteRows = false;
            dgvTurnos.MultiSelect = false;
            dgvTurnos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.BackgroundColor = Color.White;
            dgvTurnos.RowHeadersVisible = false;

            dgvTurnos.SelectionChanged += SeleccionarTurno;

            principal.Controls.Add(dgvTurnos, 0, 3);

            TableLayoutPanel inferior = new TableLayoutPanel();
            inferior.Dock = DockStyle.Fill;
            inferior.RowCount = 2;
            inferior.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 30));
            inferior.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            inferior.Controls.Add(
                CrearEtiqueta("Motivo de cancelación"), 0, 0);

            txtMotivo = new TextBox();
            txtMotivo.Dock = DockStyle.Fill;
            txtMotivo.MaxLength = 300;
            inferior.Controls.Add(txtMotivo, 0, 1);

            principal.Controls.Add(inferior, 0, 2);

            dtpInicio.Value = DateTime.Today.AddHours(8);
            dtpFin.Value = DateTime.Today.AddHours(17);
        }

        private Label CrearEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(3, 7, 3, 3)
            };
        }

        private DateTimePicker CrearSelectorHora()
        {
            return new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Dock = DockStyle.Fill
            };
        }

        private void AgregarBoton(
            FlowLayoutPanel panel,
            string texto,
            Color color,
            EventHandler accion)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Width = 155;
            boton.Height = 40;
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.Click += accion;
            panel.Controls.Add(boton);
        }

        private void CargarColaboradores()
        {
            string sql = @"
                SELECT id_colaborador,
                       nombres + ' ' + apellidos AS nombre
                FROM personal.Colaborador
                ORDER BY nombres, apellidos";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            using (SqlDataAdapter adaptador =
                   new SqlDataAdapter(sql, conexion))
            {
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                cboColaborador.DataSource = tabla;
                cboColaborador.DisplayMember = "nombre";
                cboColaborador.ValueMember = "id_colaborador";
            }
        }

        private void CargarPeriodos()
        {
            string sql = @"
                SELECT id_periodo, nombre_periodo
                FROM personal.PeriodoProgramacion
                WHERE estado = 'Abierto'
                ORDER BY fecha_inicio DESC";

            using (SqlConnection conexion =
                   ConexionBD.ObtenerConexion())
            using (SqlDataAdapter adaptador =
                   new SqlDataAdapter(sql, conexion))
            {
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                cboPeriodo.DataSource = tabla;
                cboPeriodo.DisplayMember = "nombre_periodo";
                cboPeriodo.ValueMember = "id_periodo";
            }
        }

        private void CargarTurnos()
        {
            dgvTurnos.SelectionChanged -= SeleccionarTurno;
            dgvTurnos.DataSource = AsignacionTurnoDatos.Listar();
            dgvTurnos.ClearSelection();
            dgvTurnos.SelectionChanged += SeleccionarTurno;

            idSeleccionado = 0;
            lblSeleccion.Text = "Ningún turno seleccionado";
        }

        private void SeleccionarTurno(object sender, EventArgs e)
        {
            if (dgvTurnos.SelectedRows.Count == 0)
                return;

            DataGridViewRow fila = dgvTurnos.SelectedRows[0];

            if (fila.Cells["ID"].Value == null)
                return;

            idSeleccionado = Convert.ToInt32(
                fila.Cells["ID"].Value);

            dtpFecha.Value = Convert.ToDateTime(
                fila.Cells["Fecha"].Value);

            TimeSpan inicio = TimeSpan.Parse(
                fila.Cells["Inicio"].Value.ToString());

            TimeSpan fin = TimeSpan.Parse(
                fila.Cells["Fin"].Value.ToString());

            dtpInicio.Value = DateTime.Today.Add(inicio);
            dtpFin.Value = DateTime.Today.Add(fin);

            lblSeleccion.Text = "Turno seleccionado: " +
                                idSeleccionado;
        }

        private TimeSpan HoraInicio()
        {
            return dtpInicio.Value.TimeOfDay;
        }

        private TimeSpan HoraFin()
        {
            return dtpFin.Value.TimeOfDay;
        }

        private void RegistrarTurno()
        {
            EjecutarSeguro(() =>
            {
                if (cboColaborador.SelectedValue == null ||
                    cboPeriodo.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Selecciona un colaborador y un periodo.");
                    return;
                }

                bool resultado = AsignacionTurnoDatos.Registrar(
                    Convert.ToInt32(cboColaborador.SelectedValue),
                    Convert.ToInt32(cboPeriodo.SelectedValue),
                    dtpFecha.Value.Date,
                    HoraInicio(),
                    HoraFin());

                MostrarResultado(resultado, "registrado");
            });
        }

        private void ModificarTurno()
        {
            EjecutarSeguro(() =>
            {
                if (idSeleccionado == 0)
                {
                    MessageBox.Show("Selecciona un turno de la tabla.");
                    return;
                }

                bool resultado = AsignacionTurnoDatos.Modificar(
                    idSeleccionado,
                    dtpFecha.Value.Date,
                    HoraInicio(),
                    HoraFin());

                MostrarResultado(resultado, "modificado");
            });
        }

        private void CancelarTurno()
        {
            EjecutarSeguro(() =>
            {
                if (idSeleccionado == 0)
                {
                    MessageBox.Show("Selecciona un turno de la tabla.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show(
                        "Escribe el motivo de cancelación.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Confirmas la cancelación del turno?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;

                bool resultado = AsignacionTurnoDatos.Cancelar(
                    idSeleccionado, txtMotivo.Text.Trim());

                MostrarResultado(resultado, "cancelado");
            });
        }

        private void VerHistorial()
        {
            EjecutarSeguro(() =>
            {
                Form ventana = new Form();
                ventana.Text = "Historial de asignaciones";
                ventana.Size = new Size(1050, 500);
                ventana.StartPosition =
                    FormStartPosition.CenterParent;

                DataGridView tabla = new DataGridView();
                tabla.Dock = DockStyle.Fill;
                tabla.ReadOnly = true;
                tabla.AllowUserToAddRows = false;
                tabla.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                tabla.DataSource =
                    AsignacionTurnoDatos.ListarHistorial();

                ventana.Controls.Add(tabla);
                ventana.ShowDialog(this);
            });
        }

        private void MostrarResultado(bool resultado, string accion)
        {
            if (resultado)
            {
                MessageBox.Show(
                    "Turno " + accion + " correctamente.",
                    "Operación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarTurnos();
                txtMotivo.Clear();
            }
            else
            {
                MessageBox.Show(
                    "No se pudo completar la operación. " +
                    "Verifica el periodo, la disponibilidad " +
                    "y que no existan turnos superpuestos.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void EjecutarSeguro(Action accion)
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void MostrarError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void FrmAsignacionTurnos_Load(object sender, EventArgs e)
        {

        }
    }
}
