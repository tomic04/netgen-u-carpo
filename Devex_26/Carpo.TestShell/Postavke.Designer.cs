namespace Carpo.TestShell
{
    partial class Postavke
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Postavke));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            this.txtXml = new DevExpress.XtraEditors.ButtonEdit();
            this.cmdOk = new DevExpress.XtraEditors.SimpleButton();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.pnlBaza = new DevExpress.XtraEditors.GroupControl();
            this.cmbBaza = new DevExpress.XtraEditors.ComboBoxEdit();
            this.GroupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.rdoAuth = new DevExpress.XtraEditors.RadioGroup();
            this.txtLogin = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.GroupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.cmbServer = new DevExpress.XtraEditors.MRUEdit();
            this.pnlOperater = new DevExpress.XtraEditors.GroupControl();
            this.lblOperaterErr = new DevExpress.XtraEditors.LabelControl();
            this.cmbOperater = new DevExpress.XtraEditors.LookUpEdit();
            this.groupControl5 = new DevExpress.XtraEditors.GroupControl();
            this.chkXmlGen = new System.Windows.Forms.CheckBox();
            this.memoXmlGenParams = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtXml.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaza)).BeginInit();
            this.pnlBaza.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBaza.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupControl2)).BeginInit();
            this.GroupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdoAuth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupControl1)).BeginInit();
            this.GroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlOperater)).BeginInit();
            this.pnlOperater.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbOperater.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).BeginInit();
            this.groupControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoXmlGenParams.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtXml
            // 
            this.txtXml.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXml.EnterMoveNextControl = true;
            this.dxValidationProvider1.SetIconAlignment(this.txtXml, System.Windows.Forms.ErrorIconAlignment.MiddleRight);
            this.dxErrorProvider1.SetIconAlignment(this.txtXml, System.Windows.Forms.ErrorIconAlignment.MiddleRight);
            this.txtXml.Location = new System.Drawing.Point(5, 23);
            this.txtXml.Name = "txtXml";
            this.txtXml.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("txtXml.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "History", null, null, true)});
            this.txtXml.Size = new System.Drawing.Size(296, 22);
            this.txtXml.TabIndex = 2;
            conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule1.ErrorText = "Obavezan unos";
            conditionValidationRule1.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            conditionValidationRule1.Value1 = "";
            this.dxValidationProvider1.SetValidationRule(this.txtXml, conditionValidationRule1);
            this.txtXml.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtXml_ButtonClick);
            this.txtXml.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtXml_KeyDown);
            // 
            // cmdOk
            // 
            this.cmdOk.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.cmdOk.Appearance.Options.UseFont = true;
            this.cmdOk.Location = new System.Drawing.Point(12, 440);
            this.cmdOk.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(306, 36);
            this.cmdOk.TabIndex = 6;
            this.cmdOk.Text = "O K";
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // pnlBaza
            // 
            this.pnlBaza.Controls.Add(this.cmbBaza);
            this.pnlBaza.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("Osvježi", null)});
            this.pnlBaza.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.pnlBaza.Location = new System.Drawing.Point(12, 257);
            this.pnlBaza.Name = "pnlBaza";
            this.pnlBaza.Size = new System.Drawing.Size(306, 49);
            this.pnlBaza.TabIndex = 32;
            this.pnlBaza.Text = "Dostupne baze";
            this.pnlBaza.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.pnlBaza_CustomButtonClick);
            // 
            // cmbBaza
            // 
            this.cmbBaza.Location = new System.Drawing.Point(5, 24);
            this.cmbBaza.Name = "cmbBaza";
            this.cmbBaza.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbBaza.Size = new System.Drawing.Size(296, 20);
            this.cmbBaza.TabIndex = 36;
            this.cmbBaza.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbBaza_KeyDown);
            // 
            // GroupControl2
            // 
            this.GroupControl2.Controls.Add(this.rdoAuth);
            this.GroupControl2.Controls.Add(this.txtLogin);
            this.GroupControl2.Controls.Add(this.labelControl5);
            this.GroupControl2.Controls.Add(this.labelControl6);
            this.GroupControl2.Controls.Add(this.txtPassword);
            this.GroupControl2.Location = new System.Drawing.Point(12, 122);
            this.GroupControl2.Name = "GroupControl2";
            this.GroupControl2.Size = new System.Drawing.Size(306, 129);
            this.GroupControl2.TabIndex = 31;
            this.GroupControl2.Text = "Autentikacija";
            // 
            // rdoAuth
            // 
            this.rdoAuth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdoAuth.EditValue = 0;
            this.rdoAuth.Location = new System.Drawing.Point(5, 24);
            this.rdoAuth.Name = "rdoAuth";
            this.rdoAuth.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.rdoAuth.Properties.Appearance.Options.UseFont = true;
            this.rdoAuth.Properties.Columns = 1;
            this.rdoAuth.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(0, " Windows authentication"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(1, " SQL Server authentication")});
            this.rdoAuth.Size = new System.Drawing.Size(296, 48);
            this.rdoAuth.TabIndex = 2;
            this.rdoAuth.EditValueChanged += new System.EventHandler(this.rdoAuth_EditValueChanged);
            // 
            // txtLogin
            // 
            this.txtLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogin.EditValue = "sa";
            this.txtLogin.Enabled = false;
            this.txtLogin.Location = new System.Drawing.Point(79, 78);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtLogin.Properties.Appearance.Options.UseFont = true;
            this.txtLogin.Size = new System.Drawing.Size(222, 20);
            this.txtLogin.TabIndex = 3;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelControl5.Location = new System.Drawing.Point(8, 81);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(65, 14);
            this.labelControl5.TabIndex = 20;
            this.labelControl5.Text = "Logn name:";
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelControl6.Location = new System.Drawing.Point(8, 105);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(55, 14);
            this.labelControl6.TabIndex = 21;
            this.labelControl6.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(79, 102);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(222, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // GroupControl1
            // 
            this.GroupControl1.Controls.Add(this.cmbServer);
            this.GroupControl1.Location = new System.Drawing.Point(12, 67);
            this.GroupControl1.Name = "GroupControl1";
            this.GroupControl1.Size = new System.Drawing.Size(306, 49);
            this.GroupControl1.TabIndex = 30;
            this.GroupControl1.Text = "SQL server";
            // 
            // cmbServer
            // 
            this.cmbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServer.Location = new System.Drawing.Point(5, 23);
            this.cmbServer.Name = "cmbServer";
            this.cmbServer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbServer.Size = new System.Drawing.Size(296, 20);
            this.cmbServer.TabIndex = 33;
            // 
            // pnlOperater
            // 
            this.pnlOperater.Controls.Add(this.lblOperaterErr);
            this.pnlOperater.Controls.Add(this.cmbOperater);
            this.pnlOperater.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("Osvježi", null)});
            this.pnlOperater.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.pnlOperater.Location = new System.Drawing.Point(12, 312);
            this.pnlOperater.Name = "pnlOperater";
            this.pnlOperater.Size = new System.Drawing.Size(306, 49);
            this.pnlOperater.TabIndex = 33;
            this.pnlOperater.Text = "Operater";
            this.pnlOperater.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.pnlOperater_CustomButtonClick);
            // 
            // lblOperaterErr
            // 
            this.lblOperaterErr.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblOperaterErr.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblOperaterErr.Location = new System.Drawing.Point(6, 28);
            this.lblOperaterErr.Name = "lblOperaterErr";
            this.lblOperaterErr.Size = new System.Drawing.Size(121, 13);
            this.lblOperaterErr.TabIndex = 35;
            this.lblOperaterErr.Text = "Neispravna konekcija";
            // 
            // cmbOperater
            // 
            this.cmbOperater.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOperater.Location = new System.Drawing.Point(5, 23);
            this.cmbOperater.Name = "cmbOperater";
            this.cmbOperater.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbOperater.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("OperatoriId", "OperatoriId"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Naziv", "Naziv")});
            this.cmbOperater.Properties.DisplayMember = "Naziv";
            this.cmbOperater.Properties.NullText = "";
            this.cmbOperater.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cmbOperater.Properties.ValueMember = "OperateriId";
            this.cmbOperater.Size = new System.Drawing.Size(296, 20);
            this.cmbOperater.TabIndex = 34;
            this.cmbOperater.Visible = false;
            // 
            // groupControl5
            // 
            this.groupControl5.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupControl5.AppearanceCaption.Options.UseFont = true;
            this.groupControl5.Controls.Add(this.txtXml);
            this.groupControl5.Location = new System.Drawing.Point(12, 12);
            this.groupControl5.Name = "groupControl5";
            this.groupControl5.Size = new System.Drawing.Size(306, 49);
            this.groupControl5.TabIndex = 34;
            this.groupControl5.Text = "XML";
            // 
            // chkXmlGen
            // 
            this.chkXmlGen.AutoSize = true;
            this.chkXmlGen.Location = new System.Drawing.Point(13, 367);
            this.chkXmlGen.Name = "chkXmlGen";
            this.chkXmlGen.Size = new System.Drawing.Size(61, 17);
            this.chkXmlGen.TabIndex = 35;
            this.chkXmlGen.Text = "XmlGen";
            this.chkXmlGen.UseVisualStyleBackColor = true;
            this.chkXmlGen.CheckedChanged += new System.EventHandler(this.chkXmlGen_CheckedChanged);
            // 
            // memoXmlGenParams
            // 
            this.memoXmlGenParams.Enabled = false;
            this.memoXmlGenParams.Location = new System.Drawing.Point(12, 390);
            this.memoXmlGenParams.Name = "memoXmlGenParams";
            this.memoXmlGenParams.Size = new System.Drawing.Size(306, 44);
            this.memoXmlGenParams.TabIndex = 36;
            // 
            // Postavke
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 488);
            this.Controls.Add(this.memoXmlGenParams);
            this.Controls.Add(this.chkXmlGen);
            this.Controls.Add(this.groupControl5);
            this.Controls.Add(this.pnlOperater);
            this.Controls.Add(this.pnlBaza);
            this.Controls.Add(this.GroupControl2);
            this.Controls.Add(this.GroupControl1);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Postavke";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Postavke";
            this.Load += new System.EventHandler(this.Postavke_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Postavke_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtXml.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBaza)).EndInit();
            this.pnlBaza.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbBaza.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupControl2)).EndInit();
            this.GroupControl2.ResumeLayout(false);
            this.GroupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdoAuth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupControl1)).EndInit();
            this.GroupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlOperater)).EndInit();
            this.pnlOperater.ResumeLayout(false);
            this.pnlOperater.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbOperater.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).EndInit();
            this.groupControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoXmlGenParams.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ButtonEdit txtXml;
        private DevExpress.XtraEditors.SimpleButton cmdOk;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        internal DevExpress.XtraEditors.GroupControl pnlOperater;
        private DevExpress.XtraEditors.LookUpEdit cmbOperater;
        internal DevExpress.XtraEditors.GroupControl pnlBaza;
        internal DevExpress.XtraEditors.GroupControl GroupControl2;
        public DevExpress.XtraEditors.RadioGroup rdoAuth;
        public DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        public DevExpress.XtraEditors.TextEdit txtPassword;
        internal DevExpress.XtraEditors.GroupControl GroupControl1;
        private DevExpress.XtraEditors.MRUEdit cmbServer;
        internal DevExpress.XtraEditors.GroupControl groupControl5;
        private DevExpress.XtraEditors.ComboBoxEdit cmbBaza;
        private DevExpress.XtraEditors.LabelControl lblOperaterErr;
        private DevExpress.XtraEditors.MemoEdit memoXmlGenParams;
        private System.Windows.Forms.CheckBox chkXmlGen;
    }
}