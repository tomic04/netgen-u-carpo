namespace Carpo.Designer.Modules
{
    partial class ucXmlNode
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
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.mnuTreeEdit = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeAddInSameLevel = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeDelete = new DevExpress.XtraBars.BarButtonItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.treeXml = new DevExpress.XtraTreeList.TreeList();
            this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            this.vGrdAttr = new DevExpress.XtraVerticalGrid.VGridControl();
            this.repositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEditNumbers = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemMRUEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit();
            this.txtAttributeMemo = new DevExpress.XtraEditors.MemoEdit();
            this.popupTree = new DevExpress.XtraBars.PopupMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeXml)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
            this.splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vGrdAttr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEditNumbers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttributeMemo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupTree)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.mnuTreeEdit,
            this.mnuTreeAddInSameLevel,
            this.mnuTreeDelete});
            this.barManager1.MaxItemId = 9;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(4, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(780, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(4, 401);
            this.barDockControlBottom.Size = new System.Drawing.Size(780, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(4, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 397);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(784, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 397);
            // 
            // mnuTreeEdit
            // 
            this.mnuTreeEdit.Caption = "Uredi";
            this.mnuTreeEdit.Id = 5;
            this.mnuTreeEdit.Name = "mnuTreeEdit";
            this.mnuTreeEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeAddInside_ItemClick);
            // 
            // mnuTreeAddInSameLevel
            // 
            this.mnuTreeAddInSameLevel.Id = 8;
            this.mnuTreeAddInSameLevel.Name = "mnuTreeAddInSameLevel";
            // 
            // mnuTreeDelete
            // 
            this.mnuTreeDelete.Caption = "Obriši";
            this.mnuTreeDelete.Id = 7;
            this.mnuTreeDelete.Name = "mnuTreeDelete";
            this.mnuTreeDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeDelete_ItemClick);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(4, 4);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.treeXml);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(780, 397);
            this.splitContainerControl1.SplitterPosition = 375;
            this.splitContainerControl1.TabIndex = 16;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // treeXml
            // 
            this.treeXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeXml.Location = new System.Drawing.Point(0, 0);
            this.treeXml.Name = "treeXml";
            this.treeXml.OptionsBehavior.Editable = false;
            this.treeXml.OptionsBehavior.EnableFiltering = true;
            this.treeXml.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            this.treeXml.OptionsClipboard.CopyNodeHierarchy = DevExpress.Utils.DefaultBoolean.True;
            this.treeXml.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Extended;
            this.treeXml.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeXml.OptionsView.ShowAutoFilterRow = true;
            this.treeXml.Size = new System.Drawing.Size(375, 397);
            this.treeXml.TabIndex = 17;
            this.treeXml.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeXml_FocusedNodeChanged);
            this.treeXml.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeXml_PopupMenuShowing);
            // 
            // splitContainerControl2
            // 
            this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl2.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl2.Horizontal = false;
            this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl2.Name = "splitContainerControl2";
            this.splitContainerControl2.Panel1.Controls.Add(this.vGrdAttr);
            this.splitContainerControl2.Panel1.Text = "Panel1";
            this.splitContainerControl2.Panel2.Controls.Add(this.txtAttributeMemo);
            this.splitContainerControl2.Panel2.Text = "Panel2";
            this.splitContainerControl2.Size = new System.Drawing.Size(400, 397);
            this.splitContainerControl2.TabIndex = 12;
            this.splitContainerControl2.Text = "splitContainerControl2";
            // 
            // vGrdAttr
            // 
            this.vGrdAttr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vGrdAttr.Location = new System.Drawing.Point(0, 0);
            this.vGrdAttr.Name = "vGrdAttr";
            this.vGrdAttr.OptionsBehavior.ResizeRowHeaders = false;
            this.vGrdAttr.RecordWidth = 200;
            this.vGrdAttr.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit,
            this.repositoryItemTextEdit,
            this.repositoryItemTextEditNumbers,
            this.repositoryItemMRUEdit});
            this.vGrdAttr.RowHeaderWidth = 200;
            this.vGrdAttr.ShowButtonMode = DevExpress.XtraVerticalGrid.ShowButtonModeEnum.ShowAlways;
            this.vGrdAttr.Size = new System.Drawing.Size(400, 292);
            this.vGrdAttr.TabIndex = 11;
            this.vGrdAttr.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(this.vGrdAttr_FocusedRowChanged);
            this.vGrdAttr.RecordCellStyle += new DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventHandler(this.vGrdAttr_RecordCellStyle);
            this.vGrdAttr.CustomRecordCellEdit += new DevExpress.XtraVerticalGrid.Events.GetCustomRowCellEditEventHandler(this.vGrdAttr_CustomRecordCellEdit);
            this.vGrdAttr.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.vGrdAttr_CellValueChanged);
            // 
            // repositoryItemButtonEdit
            // 
            this.repositoryItemButtonEdit.AutoHeight = false;
            this.repositoryItemButtonEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repositoryItemButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemButtonEdit.Name = "repositoryItemButtonEdit";
            this.repositoryItemButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit_ButtonClick);
            // 
            // repositoryItemTextEdit
            // 
            this.repositoryItemTextEdit.AutoHeight = false;
            this.repositoryItemTextEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repositoryItemTextEdit.Name = "repositoryItemTextEdit";
            // 
            // repositoryItemTextEditNumbers
            // 
            this.repositoryItemTextEditNumbers.AutoHeight = false;
            this.repositoryItemTextEditNumbers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repositoryItemTextEditNumbers.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.repositoryItemTextEditNumbers.Name = "repositoryItemTextEditNumbers";
            // 
            // repositoryItemMRUEdit
            // 
            this.repositoryItemMRUEdit.AutoHeight = false;
            this.repositoryItemMRUEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repositoryItemMRUEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemMRUEdit.Name = "repositoryItemMRUEdit";
            // 
            // txtAttributeMemo
            // 
            this.txtAttributeMemo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAttributeMemo.Location = new System.Drawing.Point(0, 0);
            this.txtAttributeMemo.MenuManager = this.barManager1;
            this.txtAttributeMemo.Name = "txtAttributeMemo";
            this.txtAttributeMemo.Properties.ReadOnly = true;
            this.txtAttributeMemo.Size = new System.Drawing.Size(400, 100);
            this.txtAttributeMemo.TabIndex = 0;
            // 
            // popupTree
            // 
            this.popupTree.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeDelete, true)});
            this.popupTree.Manager = this.barManager1;
            this.popupTree.Name = "popupTree";
            // 
            // ucXmlNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "ucXmlNode";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(788, 405);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeXml)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
            this.splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vGrdAttr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEditNumbers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttributeMemo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraTreeList.TreeList treeXml;
        private DevExpress.XtraVerticalGrid.VGridControl vGrdAttr;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private DevExpress.XtraEditors.MemoEdit txtAttributeMemo;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEditNumbers;
        private DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit repositoryItemMRUEdit;
        private DevExpress.XtraBars.BarButtonItem mnuTreeEdit;
        private DevExpress.XtraBars.BarButtonItem mnuTreeAddInSameLevel;
        private DevExpress.XtraBars.BarButtonItem mnuTreeDelete;
        private DevExpress.XtraBars.PopupMenu popupTree;
    }
}

