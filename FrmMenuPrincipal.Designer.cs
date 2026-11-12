namespace Proyecto_Claro_MOANSO
{
    partial class FrmMenuPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnDisponibilidad = new System.Windows.Forms.Button();
            this.btnProgramarHorarios = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(158, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(528, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "SISTEMA DE GESTIÓN - TIENDAS CLARO";
            // 
            // btnDisponibilidad
            // 
            this.btnDisponibilidad.Location = new System.Drawing.Point(271, 122);
            this.btnDisponibilidad.Name = "btnDisponibilidad";
            this.btnDisponibilidad.Size = new System.Drawing.Size(258, 117);
            this.btnDisponibilidad.TabIndex = 1;
            this.btnDisponibilidad.Text = "Gestión de Disponibilidad Horaria";
            this.btnDisponibilidad.UseVisualStyleBackColor = true;
            this.btnDisponibilidad.Click += new System.EventHandler(this.btnDisponibilidad_Click);
            // 
            // btnProgramarHorarios
            // 
            this.btnProgramarHorarios.Location = new System.Drawing.Point(271, 305);
            this.btnProgramarHorarios.Name = "btnProgramarHorarios";
            this.btnProgramarHorarios.Size = new System.Drawing.Size(258, 117);
            this.btnProgramarHorarios.TabIndex = 2;
            this.btnProgramarHorarios.Text = "Programación de Horarios";
            this.btnProgramarHorarios.UseVisualStyleBackColor = true;
            this.btnProgramarHorarios.Click += new System.EventHandler(this.btnProgramarHorarios_Click);
            // 
            // FrmMenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 526);
            this.Controls.Add(this.btnProgramarHorarios);
            this.Controls.Add(this.btnDisponibilidad);
            this.Controls.Add(this.label1);
            this.Name = "FrmMenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmMenuPrincipal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDisponibilidad;
        private System.Windows.Forms.Button btnProgramarHorarios;
    }
}