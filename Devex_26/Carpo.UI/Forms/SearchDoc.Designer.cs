namespace Carpo.UI.Forms
{
    partial class SearchDoc
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
            Carpo.UI.Components.Base.ComponentButton componentButton1 = new Carpo.UI.Components.Base.ComponentButton();
            Carpo.UI.Components.Base.ComponentButton componentButton2 = new Carpo.UI.Components.Base.ComponentButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchDoc));
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.pnlButtonsFixed = new Carpo.UI.Components.JPanelButtonsControl();
            this.cmdPotvrdi = new Carpo.UI.Components.JButtonControl();
            this.jSplitterVertically1 = new Carpo.UI.Components.JSeparatorVert();
            this.cmdOdustani = new Carpo.UI.Components.JButtonControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.jSplitterVertically2 = new Carpo.UI.Components.JSeparatorVert();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtonsFixed)).BeginInit();
            this.pnlButtonsFixed.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1112, 504);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.DetailHeight = 431;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsFilter.AllowAutoFilterConditionChange = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // pnlButtonsFixed
            // 
            this.pnlButtonsFixed.Appearance.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlButtonsFixed.Appearance.Options.UseBackColor = true;
            this.pnlButtonsFixed.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlButtonsFixed.Controls.Add(this.cmdPotvrdi);
            this.pnlButtonsFixed.Controls.Add(this.jSplitterVertically1);
            this.pnlButtonsFixed.Controls.Add(this.cmdOdustani);
            this.pnlButtonsFixed.Definition = null;
            this.pnlButtonsFixed.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtonsFixed.Location = new System.Drawing.Point(0, 505);
            this.pnlButtonsFixed.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlButtonsFixed.Name = "pnlButtonsFixed";
            this.pnlButtonsFixed.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlButtonsFixed.Size = new System.Drawing.Size(1112, 48);
            this.pnlButtonsFixed.TabIndex = 9;
            // 
            // cmdPotvrdi
            // 
            this.cmdPotvrdi.ActAsDropDown = false;
            this.cmdPotvrdi.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.cmdPotvrdi.Appearance.Options.UseFont = true;
            componentButton1.Action = "";
            componentButton1.Action2 = "";
            componentButton1.ActionEvent = "";
            componentButton1.DefaultVisibility = null;
            componentButton1.form = null;
            componentButton1.PermissionEnabled = null;
            componentButton1.PermissionVisible = null;
            componentButton1.SearchText = null;
            this.cmdPotvrdi.ComponentExt = componentButton1;
            this.cmdPotvrdi.DataRowRef = null;
            this.cmdPotvrdi.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmdPotvrdi.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdPotvrdi.Location = new System.Drawing.Point(902, 5);
            this.cmdPotvrdi.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.cmdPotvrdi.LookAndFeel.UseDefaultLookAndFeel = false;
            this.cmdPotvrdi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdPotvrdi.Name = "cmdPotvrdi";
            this.cmdPotvrdi.Size = new System.Drawing.Size(100, 38);
            this.cmdPotvrdi.TabIndex = 2;
            this.cmdPotvrdi.Text = "Potvrdi";
            this.cmdPotvrdi.Click += new System.EventHandler(this.cmdPotvrdi_Click);
            // 
            // jSplitterVertically1
            // 
            this.jSplitterVertically1.Dock = System.Windows.Forms.DockStyle.Right;
            this.jSplitterVertically1.Location = new System.Drawing.Point(1002, 5);
            this.jSplitterVertically1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.jSplitterVertically1.Name = "jSplitterVertically1";
            this.jSplitterVertically1.Size = new System.Drawing.Size(5, 38);
            this.jSplitterVertically1.TabIndex = 1;
            // 
            // cmdOdustani
            // 
            this.cmdOdustani.ActAsDropDown = false;
            componentButton2.Action = "";
            componentButton2.Action2 = "";
            componentButton2.ActionEvent = "";
            componentButton2.DefaultVisibility = null;
            componentButton2.form = null;
            componentButton2.PermissionEnabled = null;
            componentButton2.PermissionVisible = null;
            componentButton2.SearchText = null;
            this.cmdOdustani.ComponentExt = componentButton2;
            this.cmdOdustani.DataRowRef = null;
            this.cmdOdustani.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmdOdustani.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdOdustani.Location = new System.Drawing.Point(1007, 5);
            this.cmdOdustani.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdOdustani.Name = "cmdOdustani";
            this.cmdOdustani.Size = new System.Drawing.Size(100, 38);
            this.cmdOdustani.TabIndex = 0;
            this.cmdOdustani.Text = "Odustani";
            this.cmdOdustani.Click += new System.EventHandler(this.cmdOdustani_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 504);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1112, 1);
            this.panel1.TabIndex = 10;
            // 
            // jSplitterVertically2
            // 
            this.jSplitterVertically2.Dock = System.Windows.Forms.DockStyle.Right;
            this.jSplitterVertically2.Location = new System.Drawing.Point(552, 4);
            this.jSplitterVertically2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.jSplitterVertically2.Name = "jSplitterVertically2";
            this.jSplitterVertically2.Size = new System.Drawing.Size(4, 31);
            this.jSplitterVertically2.TabIndex = 1;
            // 
            // SearchDoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1112, 629);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlButtonsFixed);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(9, 7, 9, 7);
            this.Name = "SearchDoc";
            this.Text = "Dokumenti";
            this.Load += new System.EventHandler(this.SearchDoc_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchDoc_KeyDown);
            this.Controls.SetChildIndex(this.pnlButtonsFixed, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.gridControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtonsFixed)).EndInit();
            this.pnlButtonsFixed.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private Components.JPanelButtonsControl pnlButtonsFixed;
        private System.Windows.Forms.Panel panel1;
        public DevExpress.XtraGrid.GridControl gridControl1;        
        private Components.JSeparatorVert jSplitterVertically2;
        private Components.JButtonControl cmdOdustani;
        private Components.JSeparatorVert jSplitterVertically1;
        private Components.JButtonControl cmdPotvrdi;
    }
}