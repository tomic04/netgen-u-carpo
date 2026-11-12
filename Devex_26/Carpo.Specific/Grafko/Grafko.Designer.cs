namespace Carpo.Specific.Grafko
{
    partial class Grafko
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.diagram = new DevExpress.XtraDiagram.DiagramControl();
            ((System.ComponentModel.ISupportInitialize)(this.diagram)).BeginInit();
            this.SuspendLayout();
            // 
            // diagram
            // 
            this.diagram.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.None;
            this.diagram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagram.Location = new System.Drawing.Point(0, 0);
            this.diagram.Name = "diagram";
           //tomic 18 this.diagram.OptionsView.DrawGrid = false;
           //tomic 18 this.diagram.OptionsView.DrawRulers = false;
            this.diagram.OptionsView.PageSize = new System.Drawing.SizeF(800F, 600F);
            this.diagram.Size = new System.Drawing.Size(860, 649);
            this.diagram.TabIndex = 0;
            this.diagram.Text = "diagramControl1";
            // 
            // Grafko2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.diagram);
            this.Name = "Grafko2";
            this.Size = new System.Drawing.Size(860, 649);
            ((System.ComponentModel.ISupportInitialize)(this.diagram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraDiagram.DiagramControl diagram;
    }
}
