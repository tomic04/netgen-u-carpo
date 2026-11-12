namespace Carpo.UI.Forms
{
    partial class SearchDocFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDocFilter));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPojam = new System.Windows.Forms.Label();
            this.txtPojam = new DevExpress.XtraEditors.TextEdit();
            this.cmdOk = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPojam.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Controls.Add(this.lblPojam, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPojam, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmdOk, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(306, 32);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblPojam
            // 
            this.lblPojam.AutoSize = true;
            this.lblPojam.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblPojam.Location = new System.Drawing.Point(3, 0);
            this.lblPojam.Name = "lblPojam";
            this.lblPojam.Size = new System.Drawing.Size(40, 32);
            this.lblPojam.TabIndex = 0;
            this.lblPojam.Text = "Pojam:";
            this.lblPojam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPojam
            // 
            this.txtPojam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPojam.EditValue = "";
            this.txtPojam.Location = new System.Drawing.Point(49, 3);
            this.txtPojam.Name = "txtPojam";
            this.txtPojam.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtPojam.Properties.Appearance.Options.UseFont = true;
            this.txtPojam.Properties.AutoHeight = false;
            this.txtPojam.Size = new System.Drawing.Size(214, 26);
            this.txtPojam.TabIndex = 1;
            // 
            // cmdOk
            // 
            this.cmdOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOk.Image = ((System.Drawing.Image)(resources.GetObject("cmdOk.Image")));
            this.cmdOk.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdOk.Location = new System.Drawing.Point(269, 3);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(34, 26);
            this.cmdOk.TabIndex = 2;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // SearchDocFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 40);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "SearchDocFilter";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pronađi dokument";
            this.Load += new System.EventHandler(this.SearchDocFilter_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchDocFilter_KeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPojam.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPojam;
        private DevExpress.XtraEditors.TextEdit txtPojam;
        private DevExpress.XtraEditors.SimpleButton cmdOk;
    }
}