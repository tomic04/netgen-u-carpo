namespace Carpo.Core.Forms
{
    partial class GridExport
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
            this.cmdFullGrid = new DevExpress.XtraEditors.SimpleButton();
            this.cmdSelectedRows = new DevExpress.XtraEditors.SimpleButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdFullGrid
            // 
            this.cmdFullGrid.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.cmdFullGrid.Appearance.Options.UseFont = true;
            this.cmdFullGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmdFullGrid.Location = new System.Drawing.Point(4, 4);
            this.cmdFullGrid.Name = "cmdFullGrid";
            this.cmdFullGrid.Size = new System.Drawing.Size(276, 58);
            this.cmdFullGrid.TabIndex = 0;
            this.cmdFullGrid.Text = "CIJELI GRID";
            this.cmdFullGrid.Click += new System.EventHandler(this.cmdFullGrid_Click);
            // 
            // cmdSelectedRows
            // 
            this.cmdSelectedRows.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.cmdSelectedRows.Appearance.Options.UseFont = true;
            this.cmdSelectedRows.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmdSelectedRows.Location = new System.Drawing.Point(4, 66);
            this.cmdSelectedRows.Name = "cmdSelectedRows";
            this.cmdSelectedRows.Size = new System.Drawing.Size(276, 58);
            this.cmdSelectedRows.TabIndex = 1;
            this.cmdSelectedRows.Text = "SELEKTIRANE STAVKE";
            this.cmdSelectedRows.Click += new System.EventHandler(this.cmdSelectedRows_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cmdSelectedRows);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.cmdFullGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(284, 128);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(4, 62);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(276, 4);
            this.panel2.TabIndex = 2;
            // 
            // GridExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 105);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "GridExport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Grid Export";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridExport_KeyDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        public DevExpress.XtraEditors.SimpleButton cmdFullGrid;
        public DevExpress.XtraEditors.SimpleButton cmdSelectedRows;
    }
}