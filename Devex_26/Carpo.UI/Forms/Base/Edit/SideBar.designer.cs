using Carpo.UI.Components.Container;

namespace Carpo.UI.Forms.Edit
{
    partial class SideBar
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
            Carpo.UI.Components.Base.ComponentTabs componentTabs1 = new Carpo.UI.Components.Base.ComponentTabs();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideBar));
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.tabs = new Carpo.UI.Components.Container.JXtraTabControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(35, 37, 35, 37);
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ribbonControl1.OptionsMenuMinWidth = 243;
            this.ribbonControl1.Size = new System.Drawing.Size(1107, 50);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            // 
            // panelControl4
            // 
            this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl4.Controls.Add(this.tabs);
            this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl4.Location = new System.Drawing.Point(0, 51);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelControl4.Size = new System.Drawing.Size(1107, 705);
            this.panelControl4.TabIndex = 10;
            // 
            // tabs
            // 
            this.tabs.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.tabs.AppearancePage.Header.ForeColor = System.Drawing.Color.DimGray;
            this.tabs.AppearancePage.Header.Options.UseForeColor = true;
            this.tabs.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tabs.AppearancePage.HeaderActive.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tabs.AppearancePage.HeaderActive.Options.UseFont = true;
            this.tabs.AppearancePage.HeaderActive.Options.UseForeColor = true;
            componentTabs1.Action = "";
            componentTabs1.Action2 = "";
            componentTabs1.ActionEvent = "";
            componentTabs1.DataColumns = new string[0];
            componentTabs1.DefaultVisibility = null;
            componentTabs1.form = null;
            componentTabs1.PermissionEnabled = null;
            componentTabs1.PermissionVisible = null;
            componentTabs1.ValueFieldColumnType = null;
            this.tabs.ComponentExt = componentTabs1;
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.RunActions = false;
            this.tabs.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            this.tabs.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
            this.tabs.JTabsInit = null;
            this.tabs.Location = new System.Drawing.Point(8, 8);
            this.tabs.MultiLine = DevExpress.Utils.DefaultBoolean.False;
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Windows.Forms.Padding(4);
            this.tabs.Size = new System.Drawing.Size(1091, 693);
            this.tabs.TabIndex = 5;
            this.tabs.TabNameHelper = null;
            this.tabs.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabs_SelectedPageChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(109)))), ((int)(((byte)(156)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 50);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1107, 1);
            this.panel3.TabIndex = 13;
            // 
            // SideBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 824);
            this.Controls.Add(this.panelControl4);
            this.Controls.Add(this.panel3);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("SideBar.IconOptions.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "SideBar";
            this.Ribbon = null;
            this.Text = "Info";
            this.Shown += new System.EventHandler(this.SideBar_Shown);
            this.Controls.SetChildIndex(this.ribbonControl1, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.panelControl4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        internal JXtraTabControl tabs;
    }
}