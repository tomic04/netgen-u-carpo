namespace Carpo.Designer.Forms
{
    partial class CheckXml
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
            this.gcCheck = new DevExpress.XtraGrid.GridControl();
            this.gvCheck = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblGreske = new DevExpress.XtraEditors.LabelControl();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvCheck)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gcCheck
            // 
            this.gcCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcCheck.Location = new System.Drawing.Point(4, 4);
            this.gcCheck.MainView = this.gvCheck;
            this.gcCheck.Name = "gcCheck";
            this.gcCheck.Size = new System.Drawing.Size(664, 370);
            this.gcCheck.TabIndex = 0;
            this.gcCheck.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvCheck});
            // 
            // gvCheck
            // 
            this.gvCheck.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gvCheck.GridControl = this.gcCheck;
            this.gvCheck.Name = "gvCheck";
            this.gvCheck.OptionsCustomization.AllowFilter = false;
            this.gvCheck.OptionsCustomization.AllowGroup = false;
            this.gvCheck.OptionsView.ShowGroupPanel = false;
            this.gvCheck.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvCheck_RowCellStyle);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Element";
            this.gridColumn1.FieldName = "Element";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Atribut";
            this.gridColumn2.FieldName = "Attribute";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Poruka";
            this.gridColumn3.FieldName = "Message";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblGreske);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 374);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(664, 25);
            this.panel1.TabIndex = 1;
            // 
            // lblGreske
            // 
            this.lblGreske.AllowHtmlString = true;
            this.lblGreske.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.lblGreske.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGreske.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblGreske.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblGreske.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGreske.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblGreske.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGreske.Location = new System.Drawing.Point(0, 0);
            this.lblGreske.Name = "lblGreske";
            this.lblGreske.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblGreske.Size = new System.Drawing.Size(664, 25);
            this.lblGreske.TabIndex = 0;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "File";
            this.gridColumn4.FieldName = "File";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // CheckXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 403);
            this.Controls.Add(this.gcCheck);
            this.Controls.Add(this.panel1);
            this.Name = "CheckXml";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analiza Carpo XML-a";
            this.Load += new System.EventHandler(this.CheckXml_Load);
            this.Shown += new System.EventHandler(this.CheckXml_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gcCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvCheck)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraGrid.Views.Grid.GridView gvCheck;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        public DevExpress.XtraGrid.GridControl gcCheck;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl lblGreske;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
    }
}