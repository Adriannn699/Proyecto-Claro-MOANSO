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
    public partial class FrmSolicitudes : Form
    {
        public FrmSolicitudes()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblEmpresa_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevaSolicitud_Click(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                using (FrmNuevaSolicitud formulario =
                    new FrmNuevaSolicitud())
                {
                    formulario.ShowDialog();
                }
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }
    }
}
