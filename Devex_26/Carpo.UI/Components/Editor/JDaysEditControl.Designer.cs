namespace Carpo.UI.Components
{
    partial class JDaysEditControl
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
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.txtDan = new DevExpress.XtraEditors.TextEdit();
            this.cmdDatum = new DevExpress.XtraEditors.DropDownButton();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDan.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.txtDan);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl1.Location = new System.Drawing.Point(153, 1);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.panelControl1.Size = new System.Drawing.Size(33, 20);
            this.panelControl1.TabIndex = 1;
            // 
            // txtDan
            // 
            this.txtDan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDan.Location = new System.Drawing.Point(2, 0);
            this.txtDan.Name = "txtDan";
            this.txtDan.Properties.EditFormat.FormatString = "F0";
            this.txtDan.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDan.Properties.Mask.EditMask = "F0";
            this.txtDan.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtDan.Properties.Mask.ShowPlaceHolders = false;
            this.txtDan.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtDan.Size = new System.Drawing.Size(31, 20);
            this.txtDan.TabIndex = 0;
            // 
            // cmdDatum
            // 
            this.cmdDatum.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmdDatum.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdDatum.Location = new System.Drawing.Point(0, 1);
            this.cmdDatum.Name = "cmdDatum";
            this.cmdDatum.Size = new System.Drawing.Size(153, 20);
            this.cmdDatum.TabIndex = 2;
            this.cmdDatum.TabStop = false;
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.MaxItemId = 0;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 1);
            this.barDockControlTop.Size = new System.Drawing.Size(186, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 21);
            this.barDockControlBottom.Size = new System.Drawing.Size(186, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 1);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 20);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(186, 1);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 20);
            // 
            // JDaysEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.cmdDatum);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.MaximumSize = new System.Drawing.Size(0, 22);
            this.MinimumSize = new System.Drawing.Size(0, 22);
            this.Name = "JDaysEditControl";
            this.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.Size = new System.Drawing.Size(186, 22);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDan.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        internal DevExpress.XtraEditors.TextEdit txtDan;
        internal DevExpress.XtraEditors.DropDownButton cmdDatum;
        internal DevExpress.XtraBars.BarManager barManager1;
    }
}
