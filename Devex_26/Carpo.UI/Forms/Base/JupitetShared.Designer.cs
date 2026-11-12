namespace Carpo.UI.Forms.Base
{
    partial class JupiterShared 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JupiterShared));
            this.barManagerMain = new DevExpress.XtraBars.BarManager(this.components);
            this.statusBar = new DevExpress.XtraBars.Bar();
            this.bbiVerzija = new DevExpress.XtraBars.BarStaticItem();
            this.bbiLog = new DevExpress.XtraBars.BarEditItem();
            this.repPicEditLog = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.bbiBrojSlogova = new DevExpress.XtraBars.BarStaticItem();
            this.bbiBrojSlogovaMasterGrid = new DevExpress.XtraBars.BarStaticItem();
            this.bbiBrojSlogovaDetailsGrid = new DevExpress.XtraBars.BarStaticItem();
            this.bbiLabel = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTopMain = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottomMain = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeftMain = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRightMain = new DevExpress.XtraBars.BarDockControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Carpo.UI.Forms.WaitForm1), true, true);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPicEditLog)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerMain
            // 
            this.barManagerMain.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.statusBar});
            this.barManagerMain.DockControls.Add(this.barDockControlTopMain);
            this.barManagerMain.DockControls.Add(this.barDockControlBottomMain);
            this.barManagerMain.DockControls.Add(this.barDockControlLeftMain);
            this.barManagerMain.DockControls.Add(this.barDockControlRightMain);
            this.barManagerMain.Form = this;
            this.barManagerMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbiLog,
            this.bbiVerzija,
            this.bbiBrojSlogova,
            this.bbiBrojSlogovaMasterGrid,
            this.bbiBrojSlogovaDetailsGrid,
            this.bbiLabel});
            this.barManagerMain.MaxItemId = 8;
            this.barManagerMain.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repPicEditLog});
            this.barManagerMain.StatusBar = this.statusBar;
            // 
            // statusBar
            // 
            this.statusBar.BarName = "Status bar";
            this.statusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.statusBar.DockCol = 0;
            this.statusBar.DockRow = 0;
            this.statusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.statusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiVerzija),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiLog),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiBrojSlogova),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiBrojSlogovaMasterGrid),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiBrojSlogovaDetailsGrid),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiLabel)});
            this.statusBar.OptionsBar.AllowQuickCustomization = false;
            this.statusBar.OptionsBar.DrawDragBorder = false;
            this.statusBar.OptionsBar.UseWholeRow = true;
            this.statusBar.Text = "Status bar";
            // 
            // bbiVerzija
            // 
            this.bbiVerzija.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.bbiVerzija.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bbiVerzija.Caption = "v.";
            this.bbiVerzija.Id = 3;
            this.bbiVerzija.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.bbiVerzija.ItemAppearance.Normal.ForeColor = System.Drawing.Color.DimGray;
            this.bbiVerzija.ItemAppearance.Normal.Options.UseFont = true;
            this.bbiVerzija.ItemAppearance.Normal.Options.UseForeColor = true;
            this.bbiVerzija.Name = "bbiVerzija";
            // 
            // bbiLog
            // 
            this.bbiLog.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.bbiLog.Caption = "Log";
            this.bbiLog.Edit = this.repPicEditLog;
            this.bbiLog.EditWidth = 30;
            this.bbiLog.Id = 2;
            this.bbiLog.Name = "bbiLog";
            // 
            // repPicEditLog
            // 
            this.repPicEditLog.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.repPicEditLog.Appearance.Options.UseBackColor = true;
            this.repPicEditLog.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repPicEditLog.InitialImageOptions.Image = global::Carpo.UI.Properties.Resources.circle_gray_16;
            this.repPicEditLog.Name = "repPicEditLog";
            this.repPicEditLog.NullText = "...";
            // 
            // bbiBrojSlogova
            // 
            this.bbiBrojSlogova.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bbiBrojSlogova.Caption = "Broj slogova:";
            this.bbiBrojSlogova.Id = 4;
            this.bbiBrojSlogova.Name = "bbiBrojSlogova";
            // 
            // bbiBrojSlogovaMasterGrid
            // 
            this.bbiBrojSlogovaMasterGrid.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.bbiBrojSlogovaMasterGrid.Id = 5;
            this.bbiBrojSlogovaMasterGrid.Name = "bbiBrojSlogovaMasterGrid";
            // 
            // bbiBrojSlogovaDetailsGrid
            // 
            this.bbiBrojSlogovaDetailsGrid.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.bbiBrojSlogovaDetailsGrid.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bbiBrojSlogovaDetailsGrid.Id = 6;
            this.bbiBrojSlogovaDetailsGrid.Name = "bbiBrojSlogovaDetailsGrid";
            // 
            // bbiLabel
            // 
            this.bbiLabel.Id = 7;
            this.bbiLabel.Name = "bbiLabel";
            this.bbiLabel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // barDockControlTopMain
            // 
            this.barDockControlTopMain.CausesValidation = false;
            this.barDockControlTopMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTopMain.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTopMain.Manager = this.barManagerMain;
            this.barDockControlTopMain.Size = new System.Drawing.Size(492, 0);
            // 
            // barDockControlBottomMain
            // 
            this.barDockControlBottomMain.CausesValidation = false;
            this.barDockControlBottomMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottomMain.Location = new System.Drawing.Point(0, 286);
            this.barDockControlBottomMain.Manager = this.barManagerMain;
            this.barDockControlBottomMain.Size = new System.Drawing.Size(492, 22);
            // 
            // barDockControlLeftMain
            // 
            this.barDockControlLeftMain.CausesValidation = false;
            this.barDockControlLeftMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeftMain.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeftMain.Manager = this.barManagerMain;
            this.barDockControlLeftMain.Size = new System.Drawing.Size(0, 286);
            // 
            // barDockControlRightMain
            // 
            this.barDockControlRightMain.CausesValidation = false;
            this.barDockControlRightMain.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRightMain.Location = new System.Drawing.Point(492, 0);
            this.barDockControlRightMain.Manager = this.barManagerMain;
            this.barDockControlRightMain.Size = new System.Drawing.Size(0, 286);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // JupiterShared
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 308);
            this.Controls.Add(this.barDockControlLeftMain);
            this.Controls.Add(this.barDockControlRightMain);
            this.Controls.Add(this.barDockControlBottomMain);
            this.Controls.Add(this.barDockControlTopMain);

            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "JupiterShared";
            this.Text = "JupitetShared";
            
            this.Shown += new System.EventHandler(this.JupiterShared_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JupiterShared_KeyDown);
            this.Controls.SetChildIndex(this.barDockControlTopMain, 0);
            this.Controls.SetChildIndex(this.barDockControlBottomMain, 0);
            this.Controls.SetChildIndex(this.barDockControlRightMain, 0);
            this.Controls.SetChildIndex(this.barDockControlLeftMain, 0);

            ((System.ComponentModel.ISupportInitialize)(this.barManagerMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPicEditLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.BarDockControl barDockControlTopMain;
        private DevExpress.XtraBars.BarDockControl barDockControlBottomMain;
        private DevExpress.XtraBars.BarDockControl barDockControlLeftMain;
        private DevExpress.XtraBars.BarDockControl barDockControlRightMain;
        internal DevExpress.XtraBars.BarManager barManagerMain;
        protected DevExpress.XtraBars.BarStaticItem bbiVerzija;
        protected DevExpress.XtraBars.BarEditItem bbiLog;
        protected DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repPicEditLog;
        protected DevExpress.XtraBars.BarStaticItem bbiBrojSlogovaMasterGrid;
        protected DevExpress.XtraBars.BarStaticItem bbiBrojSlogovaDetailsGrid;
        protected DevExpress.XtraBars.BarStaticItem bbiBrojSlogova;
        internal DevExpress.XtraBars.Bar statusBar;
        internal DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        internal DevExpress.XtraBars.BarStaticItem bbiLabel;
    }
}