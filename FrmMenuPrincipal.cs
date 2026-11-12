using Proyecto_Claro_MOANSO.Cores;
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
    public partial class FrmMenuPrincipal : Form
    {
        public FrmMenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnDisponibilidad_Click(object sender, EventArgs e)
        {
            using (Form1 formulario = new Form1())
            {
                formulario.ShowDialog(this);
            }
        }

        private void btnProgramarHorarios_Click(object sender, EventArgs e)
        {
            using (FrmProgramarHorarios formulario = new FrmProgramarHorarios())
            {
                formulario.ShowDialog(this);
            }
        }
    }
}
