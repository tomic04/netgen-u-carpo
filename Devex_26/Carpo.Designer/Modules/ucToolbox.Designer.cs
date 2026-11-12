namespace Carpo.Designer.Modules
{
    partial class ucToolbox {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucToolbox));
            this.navBar = new DevExpress.XtraNavBar.NavBarControl();
            this.toolboxImages = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.navBar)).BeginInit();
            this.SuspendLayout();
            // 
            // navBar
            // 
            this.navBar.ActiveGroup = null;
            this.navBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBar.DragDropFlags = ((DevExpress.XtraNavBar.NavBarDragDrop)(((DevExpress.XtraNavBar.NavBarDragDrop.AllowDrag | DevExpress.XtraNavBar.NavBarDragDrop.AllowDrop) 
            | DevExpress.XtraNavBar.NavBarDragDrop.AllowOuterDrop)));
            this.navBar.Location = new System.Drawing.Point(0, 0);
            this.navBar.Margin = new System.Windows.Forms.Padding(2);
            this.navBar.Name = "navBar";
            this.navBar.OptionsNavPane.ExpandedWidth = 139;
            this.navBar.Size = new System.Drawing.Size(139, 373);
            this.navBar.SmallImages = this.toolboxImages;
            this.navBar.StoreDefaultPaintStyleName = true;
            this.navBar.TabIndex = 3;
            this.navBar.Text = "navBarControl1";
            // 
            // toolboxImages
            // 
            this.toolboxImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolboxImages.ImageStream")));
            this.toolboxImages.TransparentColor = System.Drawing.Color.Magenta;
            this.toolboxImages.Images.SetKeyName(0, "Label_684.png");
            this.toolboxImages.Images.SetKeyName(1, "Button_668.png");
            this.toolboxImages.Images.SetKeyName(2, "CheckBox_669.png");
            this.toolboxImages.Images.SetKeyName(3, "RadioButton_701.png");
            this.toolboxImages.Images.SetKeyName(4, "ComboBox_672.png");
            this.toolboxImages.Images.SetKeyName(5, "ListBox_686.png");
            this.toolboxImages.Images.SetKeyName(6, "CheckBoxList_727.png");
            this.toolboxImages.Images.SetKeyName(7, "DataGrid_674.png");
            this.toolboxImages.Images.SetKeyName(8, "ListView_687.png");
            this.toolboxImages.Images.SetKeyName(9, "SplitterControl_705.png");
            this.toolboxImages.Images.SetKeyName(10, "SplitContainerControl_704.png");
            this.toolboxImages.Images.SetKeyName(11, "Panel_694.png");
            this.toolboxImages.Images.SetKeyName(12, "TableLayoutPanel_6241.png");
            this.toolboxImages.Images.SetKeyName(13, "GroupBox_680.png");
            this.toolboxImages.Images.SetKeyName(14, "TabControl_707.png");
            this.toolboxImages.Images.SetKeyName(15, "LabelControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(16, "SimpleButton_DX.bmp");
            this.toolboxImages.Images.SetKeyName(17, "CheckButton_DX.bmp");
            this.toolboxImages.Images.SetKeyName(18, "ToggleSwitch_DX.bmp");
            this.toolboxImages.Images.SetKeyName(19, "CheckEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(20, "RadioGroup_DX.bmp");
            this.toolboxImages.Images.SetKeyName(21, "TextEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(22, "MemoEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(23, "ButtonEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(24, "SpinEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(25, "CalcEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(26, "ColorEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(27, "DateControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(28, "DateEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(29, "TimeEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(30, "LookUpEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(31, "ListBoxControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(32, "PictureEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(33, "RangeControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(34, "XtraTabControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(35, "BarManager_DX.bmp");
            this.toolboxImages.Images.SetKeyName(36, "DocumentManager_DX.bmp");
            this.toolboxImages.Images.SetKeyName(37, "PopupMenu_DX.bmp");
            this.toolboxImages.Images.SetKeyName(38, "RadialMenu_DX.bmp");
            this.toolboxImages.Images.SetKeyName(39, "TileControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(40, "GridControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(41, "GridLookUpEdit_DX.bmp");
            this.toolboxImages.Images.SetKeyName(42, "PdfViewer_DX.bmp");
            this.toolboxImages.Images.SetKeyName(43, "ChartControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(44, "SpreadsheetControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(45, "SchedulerControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(46, "PivotGridControl_DX.bmp");
            this.toolboxImages.Images.SetKeyName(47, "MapControl_DX.bmp");
            // 
            // ucToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.navBar);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ucToolbox";
            this.Size = new System.Drawing.Size(139, 373);
            ((System.ComponentModel.ISupportInitialize)(this.navBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraNavBar.NavBarControl navBar;
        private System.Windows.Forms.ImageList toolboxImages;
    }
}
