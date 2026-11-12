namespace Carpo.Designer.Forms
{
    partial class frmCustomAttribute
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCustomAttribute));
            this.cmdOk = new DevExpress.XtraEditors.SimpleButton();
            this.txtLeft = new DevExpress.XtraEditors.TextEdit();
            this.txtTop = new DevExpress.XtraEditors.TextEdit();
            this.txtRight = new DevExpress.XtraEditors.TextEdit();
            this.txtBottom = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textEdit5 = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeft.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTop.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRight.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Image = ((System.Drawing.Image)(resources.GetObject("cmdOk.Image")));
            this.cmdOk.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdOk.Location = new System.Drawing.Point(215, 178);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(36, 23);
            this.cmdOk.TabIndex = 4;
            this.cmdOk.Text = "Potvrdi";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // txtLeft
            // 
            this.txtLeft.EditValue = 0;
            this.txtLeft.Location = new System.Drawing.Point(55, 76);
            this.txtLeft.Name = "txtLeft";
            this.txtLeft.Properties.AutoHeight = false;
            this.txtLeft.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtLeft.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtLeft.Properties.NullValuePrompt = "Primjer - naziv";
            this.txtLeft.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtLeft.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtLeft.Size = new System.Drawing.Size(28, 23);
            this.txtLeft.TabIndex = 0;
            // 
            // txtTop
            // 
            this.txtTop.EditValue = 0;
            this.txtTop.Location = new System.Drawing.Point(115, 33);
            this.txtTop.Name = "txtTop";
            this.txtTop.Properties.AutoHeight = false;
            this.txtTop.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtTop.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtTop.Properties.NullValuePrompt = "Primjer - naziv";
            this.txtTop.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTop.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTop.Size = new System.Drawing.Size(28, 23);
            this.txtTop.TabIndex = 1;
            // 
            // txtRight
            // 
            this.txtRight.EditValue = 0;
            this.txtRight.Location = new System.Drawing.Point(174, 76);
            this.txtRight.Name = "txtRight";
            this.txtRight.Properties.AutoHeight = false;
            this.txtRight.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtRight.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtRight.Properties.NullValuePrompt = "Primjer - naziv";
            this.txtRight.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtRight.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtRight.Size = new System.Drawing.Size(28, 23);
            this.txtRight.TabIndex = 2;
            // 
            // txtBottom
            // 
            this.txtBottom.EditValue = 0;
            this.txtBottom.Location = new System.Drawing.Point(114, 118);
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.Properties.AutoHeight = false;
            this.txtBottom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtBottom.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtBottom.Properties.NullValuePrompt = "Primjer - naziv";
            this.txtBottom.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtBottom.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtBottom.Size = new System.Drawing.Size(28, 23);
            this.txtBottom.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Top";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Bottom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Left";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(217, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Right";
            // 
            // textEdit5
            // 
            this.textEdit5.EditValue = "Content";
            this.textEdit5.Enabled = false;
            this.textEdit5.Location = new System.Drawing.Point(89, 67);
            this.textEdit5.Name = "textEdit5";
            this.textEdit5.Properties.Appearance.BackColor = System.Drawing.Color.Wheat;
            this.textEdit5.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.textEdit5.Properties.Appearance.Options.UseBackColor = true;
            this.textEdit5.Properties.Appearance.Options.UseFont = true;
            this.textEdit5.Properties.Appearance.Options.UseTextOptions = true;
            this.textEdit5.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.textEdit5.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.textEdit5.Properties.AutoHeight = false;
            this.textEdit5.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textEdit5.Properties.NullValuePrompt = "Primjer - naziv";
            this.textEdit5.Properties.NullValuePromptShowForEmptyValue = true;
            this.textEdit5.Properties.ShowNullValuePromptWhenFocused = true;
            this.textEdit5.Size = new System.Drawing.Size(78, 40);
            this.textEdit5.TabIndex = 12;
            // 
            // frmCustomAttribute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 212);
            this.Controls.Add(this.textEdit5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBottom);
            this.Controls.Add(this.txtRight);
            this.Controls.Add(this.txtTop);
            this.Controls.Add(this.txtLeft);
            this.Controls.Add(this.cmdOk);
            this.KeyPreview = true;
            this.Name = "frmCustomAttribute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Naslov";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCustomAttribute_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtLeft.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTop.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRight.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit5.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton cmdOk;
        private DevExpress.XtraEditors.TextEdit txtLeft;
        private DevExpress.XtraEditors.TextEdit txtTop;
        private DevExpress.XtraEditors.TextEdit txtRight;
        private DevExpress.XtraEditors.TextEdit txtBottom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraEditors.TextEdit textEdit5;
    }
}