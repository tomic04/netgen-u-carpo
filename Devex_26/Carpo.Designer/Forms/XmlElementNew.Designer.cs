namespace Carpo.Designer.Forms
{
    partial class XmlElementNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlElementNew));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.gcNewElements = new DevExpress.XtraGrid.GridControl();
            this.gvNewElements = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.gcExistingElements = new DevExpress.XtraGrid.GridControl();
            this.gvExistingElements = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnMoveNodeUp = new DevExpress.XtraEditors.SimpleButton();
            this.btnMoveNodeDown = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemoveNewNode = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcNewElements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvNewElements)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcExistingElements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvExistingElements)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 454);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(799, 30);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(553, 24);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "&SPREMI PROMJENE";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(562, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(234, 24);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&ODUSTANI";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.tableLayoutPanel2);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.tableLayoutPanel3);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(799, 454);
            this.splitContainerControl1.SplitterPosition = 255;
            this.splitContainerControl1.TabIndex = 13;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.gcNewElements, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(255, 454);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // gcNewElements
            // 
            this.gcNewElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcNewElements.Location = new System.Drawing.Point(3, 33);
            this.gcNewElements.MainView = this.gvNewElements;
            this.gcNewElements.Name = "gcNewElements";
            this.gcNewElements.Size = new System.Drawing.Size(249, 418);
            this.gcNewElements.TabIndex = 0;
            this.gcNewElements.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvNewElements});
            // 
            // gvNewElements
            // 
            this.gvNewElements.GridControl = this.gcNewElements;
            this.gvNewElements.Name = "gvNewElements";
            this.gvNewElements.OptionsBehavior.Editable = false;
            this.gvNewElements.OptionsView.ShowGroupPanel = false;
            this.gvNewElements.DoubleClick += new System.EventHandler(this.gvNewElements_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F);
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "Novi elementi:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(539, 454);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(533, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Postojeći elementi:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Controls.Add(this.gcExistingElements, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 30);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 424F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(539, 424);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // gcExistingElements
            // 
            this.gcExistingElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcExistingElements.Location = new System.Drawing.Point(3, 3);
            this.gcExistingElements.MainView = this.gvExistingElements;
            this.gcExistingElements.Name = "gcExistingElements";
            this.gcExistingElements.Size = new System.Drawing.Size(503, 418);
            this.gcExistingElements.TabIndex = 0;
            this.gcExistingElements.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvExistingElements});
            // 
            // gvExistingElements
            // 
            this.gvExistingElements.GridControl = this.gcExistingElements;
            this.gvExistingElements.Name = "gvExistingElements";
            this.gvExistingElements.OptionsBehavior.Editable = false;
            this.gvExistingElements.OptionsView.ShowGroupPanel = false;
            this.gvExistingElements.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvExistingElements_RowStyle);
            this.gvExistingElements.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvExistingElements_FocusedRowChanged);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.btnMoveNodeUp, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnMoveNodeDown, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.btnRemoveNewNode, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(509, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(30, 424);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // btnMoveNodeUp
            // 
            this.btnMoveNodeUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMoveNodeUp.Enabled = false;
            this.btnMoveNodeUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveNodeUp.Image")));
            this.btnMoveNodeUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnMoveNodeUp.Location = new System.Drawing.Point(3, 3);
            this.btnMoveNodeUp.Name = "btnMoveNodeUp";
            this.btnMoveNodeUp.Size = new System.Drawing.Size(24, 24);
            this.btnMoveNodeUp.TabIndex = 0;
            this.btnMoveNodeUp.Click += new System.EventHandler(this.btnMoveNodeUp_Click);
            // 
            // btnMoveNodeDown
            // 
            this.btnMoveNodeDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMoveNodeDown.Enabled = false;
            this.btnMoveNodeDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveNodeDown.Image")));
            this.btnMoveNodeDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnMoveNodeDown.Location = new System.Drawing.Point(3, 33);
            this.btnMoveNodeDown.Name = "btnMoveNodeDown";
            this.btnMoveNodeDown.Size = new System.Drawing.Size(24, 24);
            this.btnMoveNodeDown.TabIndex = 1;
            this.btnMoveNodeDown.Click += new System.EventHandler(this.btnMoveNodeDown_Click);
            // 
            // btnRemoveNewNode
            // 
            this.btnRemoveNewNode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveNewNode.Enabled = false;
            this.btnRemoveNewNode.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveNewNode.Image")));
            this.btnRemoveNewNode.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnRemoveNewNode.Location = new System.Drawing.Point(3, 63);
            this.btnRemoveNewNode.Name = "btnRemoveNewNode";
            this.btnRemoveNewNode.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveNewNode.TabIndex = 2;
            this.btnRemoveNewNode.Click += new System.EventHandler(this.btnRemoveNewNode_Click);
            // 
            // XmlElementNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 484);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "XmlElementNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Element </>";
            this.Load += new System.EventHandler(this.XmlElementNew_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcNewElements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvNewElements)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcExistingElements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvExistingElements)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl gcNewElements;
        private DevExpress.XtraGrid.Views.Grid.GridView gvNewElements;
        private DevExpress.XtraGrid.GridControl gcExistingElements;
        private DevExpress.XtraGrid.Views.Grid.GridView gvExistingElements;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private DevExpress.XtraEditors.SimpleButton btnMoveNodeUp;
        private DevExpress.XtraEditors.SimpleButton btnMoveNodeDown;
        private DevExpress.XtraEditors.SimpleButton btnRemoveNewNode;
    }
}