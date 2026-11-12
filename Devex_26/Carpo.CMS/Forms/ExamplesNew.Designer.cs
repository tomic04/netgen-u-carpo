namespace Carpo.CMS.Forms
{
    partial class ExamplesNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExamplesNew));
            this.txtNaziv = new DevExpress.XtraEditors.TextEdit();
            this.cmdOk = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtNaziv.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNaziv
            // 
            this.txtNaziv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNaziv.Location = new System.Drawing.Point(12, 12);
            this.txtNaziv.Name = "txtNaziv";
            this.txtNaziv.Properties.AutoHeight = false;
            this.txtNaziv.Properties.NullValuePrompt = "Primjer - naziv";
            this.txtNaziv.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtNaziv.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtNaziv.Size = new System.Drawing.Size(208, 23);
            this.txtNaziv.TabIndex = 0;
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.Image = ((System.Drawing.Image)(resources.GetObject("cmdOk.Image")));
            this.cmdOk.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdOk.Location = new System.Drawing.Point(226, 12);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(36, 23);
            this.cmdOk.TabIndex = 1;
            this.cmdOk.Text = "Potvrdi";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // ExamplesNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 45);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.txtNaziv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "ExamplesNew";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Novi primjer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ExamplesNew_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtNaziv.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtNaziv;
        private DevExpress.XtraEditors.SimpleButton cmdOk;
    }
}