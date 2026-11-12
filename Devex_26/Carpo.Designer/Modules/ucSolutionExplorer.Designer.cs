namespace Carpo.Designer.Modules
{
    partial class ucSolutionExplorer {
        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucSolutionExplorer));
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.iRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.fileTypeImages = new System.Windows.Forms.ImageList(this.components);
            this.iShow = new DevExpress.XtraBars.BarButtonItem();
            this.iProperties = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeViewRename = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeViewClose = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeViewOpen = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeViewDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.mnuTreeViewAdd = new DevExpress.XtraBars.BarSubItem();
            this.mnuTreeViewAddNewFile = new DevExpress.XtraBars.BarButtonItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView = new DevExpress.XtraTreeList.TreeList();
            this.mnuEditiranje = new DevExpress.XtraBars.PopupMenu(this.components);
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnuEditiranje)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager.Controller = this.barAndDockingController;
            this.barManager.DockControls.Add(this.barDockControl1);
            this.barManager.DockControls.Add(this.barDockControl2);
            this.barManager.DockControls.Add(this.barDockControl3);
            this.barManager.DockControls.Add(this.barDockControl4);
            this.barManager.Form = this;
            this.barManager.Images = this.fileTypeImages;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.iRefresh,
            this.iShow,
            this.iProperties,
            this.mnuTreeViewRename,
            this.mnuTreeViewClose,
            this.mnuTreeViewOpen,
            this.barButtonItem1,
            this.mnuTreeViewDelete,
            this.barButtonItem3,
            this.mnuTreeViewAdd,
            this.mnuTreeViewAddNewFile,
            this.barButtonItem2});
            this.barManager.MaxItemId = 9;
            // 
            // bar1
            // 
            this.bar1.BarName = "Explorer";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.FloatLocation = new System.Drawing.Point(53, 102);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.iRefresh)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Explorer";
            this.bar1.Visible = false;
            // 
            // iRefresh
            // 
            this.iRefresh.Caption = "Refresh";
            this.iRefresh.Hint = "Refresh";
            this.iRefresh.Id = 0;
            this.iRefresh.ImageIndex = 0;
            this.iRefresh.Name = "iRefresh";
            // 
            // barAndDockingController
            // 
            this.barAndDockingController.PropertiesBar.AllowLinkLighting = false;
            this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
            this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(288, 31);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 288);
            this.barDockControl2.Size = new System.Drawing.Size(288, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 31);
            this.barDockControl3.Size = new System.Drawing.Size(0, 257);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(288, 31);
            this.barDockControl4.Size = new System.Drawing.Size(0, 257);
            // 
            // fileTypeImages
            // 
            this.fileTypeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("fileTypeImages.ImageStream")));
            this.fileTypeImages.TransparentColor = System.Drawing.Color.Magenta;
            this.fileTypeImages.Images.SetKeyName(0, "refresh_16xLG.png");
            this.fileTypeImages.Images.SetKeyName(1, "ShowAllFiles_349.png");
            this.fileTypeImages.Images.SetKeyName(2, "Property_501.png");
            this.fileTypeImages.Images.SetKeyName(3, "Solution_8308.png");
            this.fileTypeImages.Images.SetKeyName(4, "CSharpProject_SolutionExplorerNode.png");
            this.fileTypeImages.Images.SetKeyName(5, "reference_16xLG.png");
            this.fileTypeImages.Images.SetKeyName(6, "FolderOpen.png");
            this.fileTypeImages.Images.SetKeyName(7, "Folder.png");
            this.fileTypeImages.Images.SetKeyName(8, "HiddenFolder_428.png");
            this.fileTypeImages.Images.SetKeyName(9, "HiddenFolder_427.png");
            this.fileTypeImages.Images.SetKeyName(10, "CSharpFile_SolutionExplorerNode.png");
            this.fileTypeImages.Images.SetKeyName(11, "dialog_16xLG.png");
            this.fileTypeImages.Images.SetKeyName(12, "UserControl.png");
            this.fileTypeImages.Images.SetKeyName(13, "Generatedfile_430.png");
            // 
            // iShow
            // 
            this.iShow.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.iShow.Caption = "Show All Files";
            this.iShow.Hint = "Show All Files";
            this.iShow.Id = 1;
            this.iShow.ImageIndex = 1;
            this.iShow.Name = "iShow";
            // 
            // iProperties
            // 
            this.iProperties.Caption = "Properties";
            this.iProperties.Hint = "Properties";
            this.iProperties.Id = 2;
            this.iProperties.ImageIndex = 2;
            this.iProperties.Name = "iProperties";
            // 
            // mnuTreeViewRename
            // 
            this.mnuTreeViewRename.Caption = "Rename";
            this.mnuTreeViewRename.Glyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewRename.Glyph")));
            this.mnuTreeViewRename.Id = 0;
            this.mnuTreeViewRename.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewRename.LargeGlyph")));
            this.mnuTreeViewRename.Name = "mnuTreeViewRename";
            this.mnuTreeViewRename.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeViewRename_ItemClick);
            // 
            // mnuTreeViewClose
            // 
            this.mnuTreeViewClose.Caption = "Close";
            this.mnuTreeViewClose.Glyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewClose.Glyph")));
            this.mnuTreeViewClose.Id = 1;
            this.mnuTreeViewClose.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewClose.LargeGlyph")));
            this.mnuTreeViewClose.Name = "mnuTreeViewClose";
            this.mnuTreeViewClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeViewClose_ItemClick);
            // 
            // mnuTreeViewOpen
            // 
            this.mnuTreeViewOpen.Caption = "Open";
            this.mnuTreeViewOpen.Glyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewOpen.Glyph")));
            this.mnuTreeViewOpen.Id = 2;
            this.mnuTreeViewOpen.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewOpen.LargeGlyph")));
            this.mnuTreeViewOpen.Name = "mnuTreeViewOpen";
            this.mnuTreeViewOpen.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeViewOpen_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 3;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // mnuTreeViewDelete
            // 
            this.mnuTreeViewDelete.Caption = "Delete";
            this.mnuTreeViewDelete.Glyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewDelete.Glyph")));
            this.mnuTreeViewDelete.Id = 4;
            this.mnuTreeViewDelete.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("mnuTreeViewDelete.LargeGlyph")));
            this.mnuTreeViewDelete.Name = "mnuTreeViewDelete";
            this.mnuTreeViewDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeViewDelete_ItemClick_1);
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "barButtonItem3";
            this.barButtonItem3.Id = 5;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // mnuTreeViewAdd
            // 
            this.mnuTreeViewAdd.Caption = "Add";
            this.mnuTreeViewAdd.Id = 6;
            this.mnuTreeViewAdd.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewAddNewFile),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2)});
            this.mnuTreeViewAdd.Name = "mnuTreeViewAdd";
            // 
            // mnuTreeViewAddNewFile
            // 
            this.mnuTreeViewAddNewFile.Caption = "New Form";
            this.mnuTreeViewAddNewFile.Id = 7;
            this.mnuTreeViewAddNewFile.Name = "mnuTreeViewAddNewFile";
            this.mnuTreeViewAddNewFile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.mnuTreeViewAddNewFile_ItemClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 257);
            this.panel1.TabIndex = 4;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(288, 257);
            this.treeView.StateImageList = this.fileTypeImages;
            this.treeView.TabIndex = 0;
            this.treeView.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeView_GetStateImage);
            this.treeView.HiddenEditor += new System.EventHandler(this.treeView_HiddenEditor);
            this.treeView.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeView_CellValueChanged);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            this.treeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDoubleClick);
            this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseUp);
            // 
            // mnuEditiranje
            // 
            this.mnuEditiranje.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewOpen),
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewRename),
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewClose),
            new DevExpress.XtraBars.LinkPersistInfo(this.mnuTreeViewDelete)});
            this.mnuEditiranje.Manager = this.barManager;
            this.mnuEditiranje.Name = "mnuEditiranje";
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "New Part";
            this.barButtonItem2.Id = 8;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // ucSolutionExplorer
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "ucSolutionExplorer";
            this.Size = new System.Drawing.Size(288, 288);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnuEditiranje)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarButtonItem iRefresh;
        private DevExpress.XtraBars.BarButtonItem iShow;
        private DevExpress.XtraBars.BarButtonItem iProperties;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraTreeList.TreeList treeView;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ImageList fileTypeImages;
        private DevExpress.XtraBars.PopupMenu mnuEditiranje;
        private DevExpress.XtraBars.BarButtonItem mnuTreeViewRename;
        private DevExpress.XtraBars.BarButtonItem mnuTreeViewClose;
        private DevExpress.XtraBars.BarButtonItem mnuTreeViewOpen;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem mnuTreeViewDelete;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarSubItem mnuTreeViewAdd;
        private DevExpress.XtraBars.BarButtonItem mnuTreeViewAddNewFile;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
    }
}
