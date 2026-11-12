namespace Carpo.TestShell
{
    partial class Main
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
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Carpo.TestShell.Forms.WaitForm1), true, true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.pnlShell = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cmdGen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdPostavke = new System.Windows.Forms.ToolStripButton();
            this.lblKonekcija = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // pnlShell
            // 
            this.pnlShell.BackColor = System.Drawing.Color.Transparent;
            this.pnlShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShell.Location = new System.Drawing.Point(0, 25);
            this.pnlShell.Name = "pnlShell";
            this.pnlShell.Size = new System.Drawing.Size(885, 448);
            this.pnlShell.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdGen,
            this.toolStripSeparator1,
            this.cmdPostavke,
            this.lblKonekcija,
            this.toolStripSeparator2,
            this.cmdHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(885, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cmdGen
            // 
            this.cmdGen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdGen.Enabled = false;
            this.cmdGen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmdGen.Image = ((System.Drawing.Image)(resources.GetObject("cmdGen.Image")));
            this.cmdGen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdGen.Name = "cmdGen";
            this.cmdGen.Size = new System.Drawing.Size(39, 22);
            this.cmdGen.Text = "&Start";
            this.cmdGen.Click += new System.EventHandler(this.cmdGen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // cmdPostavke
            // 
            this.cmdPostavke.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdPostavke.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.cmdPostavke.Image = ((System.Drawing.Image)(resources.GetObject("cmdPostavke.Image")));
            this.cmdPostavke.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdPostavke.Name = "cmdPostavke";
            this.cmdPostavke.Size = new System.Drawing.Size(58, 22);
            this.cmdPostavke.Text = "&Postavke";
            this.cmdPostavke.Click += new System.EventHandler(this.cmdPostavke_Click);
            // 
            // lblKonekcija
            // 
            this.lblKonekcija.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblKonekcija.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblKonekcija.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblKonekcija.Name = "lblKonekcija";
            this.lblKonekcija.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // cmdHelp
            // 
            this.cmdHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdHelp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmdHelp.Image = ((System.Drawing.Image)(resources.GetObject("cmdHelp.Image")));
            this.cmdHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdHelp.Name = "cmdHelp";
            this.cmdHelp.Size = new System.Drawing.Size(36, 22);
            this.cmdHelp.Text = "&Help";
            this.cmdHelp.Click += new System.EventHandler(this.cmdHelp_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 473);
            this.Controls.Add(this.pnlShell);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "TestShell";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlShell;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cmdGen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton cmdPostavke;
        private System.Windows.Forms.ToolStripLabel lblKonekcija;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cmdHelp;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}

