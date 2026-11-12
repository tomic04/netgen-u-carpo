namespace Carpo.UI.Forms.Base
{
    partial class JupiterEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JupiterEditor));
            this.pnlButtons = new Carpo.UI.Components.JPanelButtonsControl();
            this.pnlSpace = new DevExpress.XtraEditors.PanelControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(35, 37, 35, 37);
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3);
            this.ribbonControl1.OptionsMenuMinWidth = 330;
            this.ribbonControl1.Size = new System.Drawing.Size(563, 58);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            // 
            // pnlButtons
            // 
            //this.pnlButtons.Appearance.BackColor = System.Drawing.Color.Gainsboro;
            //this.pnlButtons.Appearance.Options.UseBackColor = true;
            this.pnlButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlButtons.Definition = null;
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(3, 2);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(4);
            this.pnlButtons.Size = new System.Drawing.Size(558, 42);
            this.pnlButtons.TabIndex = 8;
            // 
            // pnlSpace
            // 
            this.pnlSpace.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSpace.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSpace.Location = new System.Drawing.Point(2, 2);
            this.pnlSpace.Name = "pnlSpace";
            this.pnlSpace.Size = new System.Drawing.Size(1, 42);
            this.pnlSpace.TabIndex = 0;
            this.pnlSpace.Visible = false;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pnlButtons);
            this.panelControl1.Controls.Add(this.pnlSpace);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 249);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(563, 46);
            this.panelControl1.TabIndex = 9;
            // 
            // JupiterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 317);
            this.Controls.Add(this.panelControl1);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("JupiterEditor.IconOptions.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "JupiterEditor";
            this.Text = "JupiterForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JupiterEditor_FormClosing);
            this.Load += new System.EventHandler(this.JupiterEditor_Load);
            this.Controls.SetChildIndex(this.ribbonControl1, 0);
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        internal Components.JPanelButtonsControl pnlButtons;
        public DevExpress.XtraEditors.PanelControl pnlSpace;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}