namespace Carpo.TestShell.Forms
{
    partial class XmlHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlHistory));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnZatvori = new DevExpress.XtraEditors.SimpleButton();
            this.btnPotvrdi = new DevExpress.XtraEditors.SimpleButton();
            this.btnObrisiPovijest = new DevExpress.XtraEditors.SimpleButton();
            this.chkbKoristiKonekciju = new DevExpress.XtraEditors.CheckButton();
            this.gcHistory = new DevExpress.XtraGrid.GridControl();
            this.gvHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel2.Controls.Add(this.btnZatvori, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPotvrdi, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnObrisiPovijest, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkbKoristiKonekciju, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 430);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(648, 39);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnZatvori
            // 
            this.btnZatvori.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnZatvori.Location = new System.Drawing.Point(570, 3);
            this.btnZatvori.Name = "btnZatvori";
            this.btnZatvori.Size = new System.Drawing.Size(75, 33);
            this.btnZatvori.TabIndex = 0;
            this.btnZatvori.Text = "&Zatvori";
            this.btnZatvori.Click += new System.EventHandler(this.btnOdustani_Click);
            // 
            // btnPotvrdi
            // 
            this.btnPotvrdi.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnPotvrdi.Appearance.Options.UseFont = true;
            this.btnPotvrdi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPotvrdi.Location = new System.Drawing.Point(489, 3);
            this.btnPotvrdi.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnPotvrdi.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPotvrdi.Name = "btnPotvrdi";
            this.btnPotvrdi.Size = new System.Drawing.Size(75, 33);
            this.btnPotvrdi.TabIndex = 1;
            this.btnPotvrdi.Text = "&Potvrdi";
            this.btnPotvrdi.Click += new System.EventHandler(this.btnPotvrdi_Click);
            // 
            // btnObrisiPovijest
            // 
            this.btnObrisiPovijest.Appearance.Options.UseTextOptions = true;
            this.btnObrisiPovijest.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnObrisiPovijest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnObrisiPovijest.Image = ((System.Drawing.Image)(resources.GetObject("btnObrisiPovijest.Image")));
            this.btnObrisiPovijest.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
            this.btnObrisiPovijest.Location = new System.Drawing.Point(98, 3);
            this.btnObrisiPovijest.Name = "btnObrisiPovijest";
            this.btnObrisiPovijest.Size = new System.Drawing.Size(79, 33);
            this.btnObrisiPovijest.TabIndex = 3;
            this.btnObrisiPovijest.Text = "Obriši povijest";
            this.btnObrisiPovijest.Click += new System.EventHandler(this.btnObrisiPovijest_Click);
            // 
            // chkbKoristiKonekciju
            // 
            this.chkbKoristiKonekciju.Appearance.Options.UseTextOptions = true;
            this.chkbKoristiKonekciju.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.chkbKoristiKonekciju.Checked = true;
            this.chkbKoristiKonekciju.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkbKoristiKonekciju.Image = ((System.Drawing.Image)(resources.GetObject("chkbKoristiKonekciju.Image")));
            this.chkbKoristiKonekciju.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
            this.chkbKoristiKonekciju.Location = new System.Drawing.Point(3, 3);
            this.chkbKoristiKonekciju.Name = "chkbKoristiKonekciju";
            this.chkbKoristiKonekciju.Size = new System.Drawing.Size(89, 33);
            this.chkbKoristiKonekciju.TabIndex = 4;
            this.chkbKoristiKonekciju.Text = "Koristi s konekcijom";
            // 
            // gcHistory
            // 
            this.gcHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHistory.Location = new System.Drawing.Point(0, 0);
            this.gcHistory.MainView = this.gvHistory;
            this.gcHistory.Name = "gcHistory";
            this.gcHistory.Size = new System.Drawing.Size(648, 430);
            this.gcHistory.TabIndex = 4;
            this.gcHistory.ToolTipController = this.toolTipController1;
            this.gcHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHistory});
            // 
            // gvHistory
            // 
            this.gvHistory.GridControl = this.gcHistory;
            this.gvHistory.Name = "gvHistory";
            this.gvHistory.OptionsBehavior.Editable = false;
            this.gvHistory.OptionsCustomization.AllowFilter = false;
            this.gvHistory.OptionsCustomization.AllowGroup = false;
            this.gvHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvHistory.OptionsView.ColumnAutoWidth = false;
            this.gvHistory.OptionsView.ShowAutoFilterRow = true;
            this.gvHistory.OptionsView.ShowGroupPanel = false;
            this.gvHistory.DoubleClick += new System.EventHandler(this.gvHistory_DoubleClick);
            // 
            // toolTipController1
            // 
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            // 
            // XmlHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 469);
            this.Controls.Add(this.gcHistory);
            this.Controls.Add(this.tableLayoutPanel2);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XmlHistory";
            this.ShowIcon = false;
            this.Text = "History";
            this.Load += new System.EventHandler(this.XmlHistory_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XmlHistory_KeyDown);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraEditors.SimpleButton btnZatvori;
        private DevExpress.XtraEditors.SimpleButton btnPotvrdi;
        private DevExpress.XtraGrid.GridControl gcHistory;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHistory;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.SimpleButton btnObrisiPovijest;
        private DevExpress.XtraEditors.CheckButton chkbKoristiKonekciju;
    }
}