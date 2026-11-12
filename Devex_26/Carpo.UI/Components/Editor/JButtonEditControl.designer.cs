namespace Carpo.UI.Components
{
    partial class JButtonEditControl
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
            this.cmd = new DevExpress.XtraEditors.SimpleButton();
            this.txt = new DevExpress.XtraEditors.TextEdit();
            this.pnl = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.txt.Properties)).BeginInit();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmd
            // 
            this.cmd.Appearance.Options.UseTextOptions = true;
            this.cmd.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.cmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmd.Location = new System.Drawing.Point(0, 0);
            this.cmd.LookAndFeel.SkinMaskColor = System.Drawing.Color.Red;
            this.cmd.Name = "cmd";
            this.cmd.Size = new System.Drawing.Size(95, 28);
            this.cmd.TabIndex = 0;
            // 
            // txt
            // 
            this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt.Location = new System.Drawing.Point(0, 0);
            this.txt.Name = "txt";
            this.txt.Properties.Appearance.Options.UseTextOptions = true;
            this.txt.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txt.Properties.Mask.EditMask = "n2";
            this.txt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txt.Properties.Mask.SaveLiteral = false;
            this.txt.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txt.Size = new System.Drawing.Size(95, 20);
            this.txt.TabIndex = 1;
            // 
            // pnl
            // 
            this.pnl.AutoSize = true;
            this.pnl.Controls.Add(this.cmd);
            this.pnl.Controls.Add(this.txt);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(95, 28);
            this.pnl.TabIndex = 2;
            // 
            // JButtonEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnl);
            this.Name = "JButtonEditControl";
            this.Size = new System.Drawing.Size(95, 28);
            ((System.ComponentModel.ISupportInitialize)(this.txt.Properties)).EndInit();
            this.pnl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.SimpleButton cmd;
        public DevExpress.XtraEditors.TextEdit txt;
        private System.Windows.Forms.Panel pnl;
    }
}
