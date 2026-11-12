namespace Carpo.UI.Components
{
    partial class JStandardButtons
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
            Carpo.UI.Components.Base.ComponentButton componentButton1 = new Carpo.UI.Components.Base.ComponentButton();
            Carpo.UI.Components.Base.ComponentButton componentButton2 = new Carpo.UI.Components.Base.ComponentButton();
            this.cmdStandardIspisKontrola = new Carpo.UI.Components.JButtonDropDownControl(this.components);
            this.popupIspisKontrola = new DevExpress.XtraBars.PopupMenu(this.components);
            this.bbiPrintGridMaster = new DevExpress.XtraBars.BarButtonItem();
            this.bbiPrintGridDetalji = new DevExpress.XtraBars.BarButtonItem();
            this.bbiPrintGridTree = new DevExpress.XtraBars.BarButtonItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.cmdStandardHelp = new Carpo.UI.Components.JButtonDropDownControl(this.components);
            this.pnlHelpSeparator = new Carpo.UI.Components.JSeparatorVert();
            ((System.ComponentModel.ISupportInitialize)(this.popupIspisKontrola)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdStandardIspisKontrola
            // 
            this.cmdStandardIspisKontrola.Appearance.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            this.cmdStandardIspisKontrola.Appearance.Options.UseFont = true;
            this.cmdStandardIspisKontrola.Appearance.Options.UseTextOptions = true;
            this.cmdStandardIspisKontrola.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            componentButton1.Action = "";
            componentButton1.Action2 = "";
            componentButton1.ActionEvent = "";
            componentButton1.form = null;
            componentButton1.SearchText = null;
            this.cmdStandardIspisKontrola.ComponentExt = componentButton1;
            this.cmdStandardIspisKontrola.DataRowRef = null;
            this.cmdStandardIspisKontrola.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmdStandardIspisKontrola.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdStandardIspisKontrola.DropDownControl = this.popupIspisKontrola;
            this.cmdStandardIspisKontrola.Location = new System.Drawing.Point(0, 0);
            this.cmdStandardIspisKontrola.Name = "cmdStandardIspisKontrola";
            this.cmdStandardIspisKontrola.Size = new System.Drawing.Size(90, 47);
            this.cmdStandardIspisKontrola.TabIndex = 0;
            this.cmdStandardIspisKontrola.Text = "ISPIS    SUČELJA";
            this.cmdStandardIspisKontrola.Visible = false;
            // 
            // popupIspisKontrola
            // 
            this.popupIspisKontrola.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiPrintGridMaster),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiPrintGridDetalji),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiPrintGridTree)});
            this.popupIspisKontrola.Manager = this.barManager1;
            this.popupIspisKontrola.Name = "popupIspisKontrola";
            this.popupIspisKontrola.BeforePopup += new System.ComponentModel.CancelEventHandler(this.popupIspisKontrola_BeforePopup);
            // 
            // bbiPrintGridMaster
            // 
            this.bbiPrintGridMaster.Caption = "Master grid";
            this.bbiPrintGridMaster.Glyph = global::Carpo.UI.Properties.Resources.grid_16;
            this.bbiPrintGridMaster.Id = 0;
            this.bbiPrintGridMaster.Name = "bbiPrintGridMaster";
            this.bbiPrintGridMaster.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiPrintGridMaster_ItemClick);
            // 
            // bbiPrintGridDetalji
            // 
            this.bbiPrintGridDetalji.Caption = "Detalj grid";
            this.bbiPrintGridDetalji.Glyph = global::Carpo.UI.Properties.Resources.grid_16;
            this.bbiPrintGridDetalji.Id = 1;
            this.bbiPrintGridDetalji.Name = "bbiPrintGridDetalji";
            this.bbiPrintGridDetalji.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiPrintGridDetalji_ItemClick);
            // 
            // bbiPrintGridTree
            // 
            this.bbiPrintGridTree.Caption = "Tree";
            this.bbiPrintGridTree.Glyph = global::Carpo.UI.Properties.Resources.flow_16;
            this.bbiPrintGridTree.Id = 2;
            this.bbiPrintGridTree.Name = "bbiPrintGridTree";
            this.bbiPrintGridTree.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiPrintGridTree_ItemClick);
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbiPrintGridMaster,
            this.bbiPrintGridDetalji,
            this.bbiPrintGridTree});
            this.barManager1.MaxItemId = 3;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(126, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 47);
            this.barDockControlBottom.Size = new System.Drawing.Size(126, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 47);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(126, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 47);
            // 
            // cmdStandardHelp
            // 
            componentButton2.Action = "";
            componentButton2.Action2 = "";
            componentButton2.ActionEvent = "";
            componentButton2.form = null;
            componentButton2.SearchText = null;
            this.cmdStandardHelp.ComponentExt = componentButton2;
            this.cmdStandardHelp.DataRowRef = null;
            this.cmdStandardHelp.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmdStandardHelp.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdStandardHelp.Enabled = false;
            this.cmdStandardHelp.Image = global::Carpo.UI.Properties.Resources.help_24;
            this.cmdStandardHelp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdStandardHelp.Location = new System.Drawing.Point(94, 0);
            this.cmdStandardHelp.Name = "cmdStandardHelp";
            this.cmdStandardHelp.Size = new System.Drawing.Size(32, 47);
            this.cmdStandardHelp.TabIndex = 11;
            this.cmdStandardHelp.ToolTip = "Help";
            // 
            // pnlHelpSeparator
            // 
            this.pnlHelpSeparator.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlHelpSeparator.Location = new System.Drawing.Point(90, 0);
            this.pnlHelpSeparator.Name = "pnlHelpSeparator";
            this.pnlHelpSeparator.Size = new System.Drawing.Size(4, 47);
            this.pnlHelpSeparator.TabIndex = 22;
            this.pnlHelpSeparator.Visible = false;
            // 
            // JStandardButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.cmdStandardHelp);
            this.Controls.Add(this.pnlHelpSeparator);
            this.Controls.Add(this.cmdStandardIspisKontrola);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "JStandardButtons";
            this.Size = new System.Drawing.Size(126, 47);
            ((System.ComponentModel.ISupportInitialize)(this.popupIspisKontrola)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.PopupMenu popupIspisKontrola;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbiPrintGridMaster;
        private DevExpress.XtraBars.BarButtonItem bbiPrintGridDetalji;
        private DevExpress.XtraBars.BarButtonItem bbiPrintGridTree;
        public JButtonDropDownControl cmdStandardIspisKontrola;
        public JButtonDropDownControl cmdStandardHelp;
        public JSeparatorVert pnlHelpSeparator;
    }
}
