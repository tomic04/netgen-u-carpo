namespace Carpo.Common.Forms
{
    partial class frmError
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
            this.btnOKCentral = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrikazi = new DevExpress.XtraEditors.CheckButton();
            this.txtBoxError = new System.Windows.Forms.TextBox();
            this.tlpIconError = new System.Windows.Forms.TableLayoutPanel();
            this.imgError = new System.Windows.Forms.PictureBox();
            this.tblErrText = new System.Windows.Forms.TableLayoutPanel();
            this.btnOKRight = new DevExpress.XtraEditors.SimpleButton();
            this.tblParent = new System.Windows.Forms.TableLayoutPanel();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.tlpIconError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgError)).BeginInit();
            this.tblErrText.SuspendLayout();
            this.tblParent.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOKCentral
            // 
            this.btnOKCentral.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOKCentral.Location = new System.Drawing.Point(179, 3);
            this.btnOKCentral.Name = "btnOKCentral";
            this.btnOKCentral.Size = new System.Drawing.Size(75, 23);
            this.btnOKCentral.TabIndex = 1;
            this.btnOKCentral.Text = "&OK";
            this.btnOKCentral.Visible = false;
            this.btnOKCentral.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPrikazi
            // 
            this.btnPrikazi.Location = new System.Drawing.Point(23, 3);
            this.btnPrikazi.Name = "btnPrikazi";
            this.btnPrikazi.Size = new System.Drawing.Size(75, 23);
            this.btnPrikazi.TabIndex = 7;
            this.btnPrikazi.Text = "&Prikaži više";
            this.btnPrikazi.CheckedChanged += new System.EventHandler(this.checkButton1_CheckedChanged);
            // 
            // txtBoxError
            // 
            this.txtBoxError.BackColor = System.Drawing.SystemColors.Control;
            this.txtBoxError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBoxError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxError.Location = new System.Drawing.Point(3, 13);
            this.txtBoxError.Multiline = true;
            this.txtBoxError.Name = "txtBoxError";
            this.txtBoxError.ReadOnly = true;
            this.txtBoxError.Size = new System.Drawing.Size(372, 26);
            this.txtBoxError.TabIndex = 0;
            // 
            // tlpIconError
            // 
            this.tlpIconError.ColumnCount = 3;
            this.tlpIconError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlpIconError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tlpIconError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpIconError.Controls.Add(this.imgError, 1, 0);
            this.tlpIconError.Controls.Add(this.tblErrText, 2, 0);
            this.tlpIconError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpIconError.Location = new System.Drawing.Point(0, 9);
            this.tlpIconError.Margin = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.tlpIconError.Name = "tlpIconError";
            this.tlpIconError.RowCount = 1;
            this.tlpIconError.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpIconError.Size = new System.Drawing.Size(434, 42);
            this.tlpIconError.TabIndex = 2;
            // 
            // imgError
            // 
            this.imgError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgError.Location = new System.Drawing.Point(13, 3);
            this.imgError.Name = "imgError";
            this.imgError.Size = new System.Drawing.Size(40, 36);
            this.imgError.TabIndex = 1;
            this.imgError.TabStop = false;
            // 
            // tblErrText
            // 
            this.tblErrText.ColumnCount = 1;
            this.tblErrText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblErrText.Controls.Add(this.txtBoxError, 0, 1);
            this.tblErrText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblErrText.Location = new System.Drawing.Point(56, 0);
            this.tblErrText.Margin = new System.Windows.Forms.Padding(0);
            this.tblErrText.Name = "tblErrText";
            this.tblErrText.RowCount = 2;
            this.tblErrText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tblErrText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblErrText.Size = new System.Drawing.Size(378, 42);
            this.tblErrText.TabIndex = 2;
            // 
            // btnOKRight
            // 
            this.btnOKRight.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOKRight.Location = new System.Drawing.Point(335, 3);
            this.btnOKRight.Name = "btnOKRight";
            this.btnOKRight.Size = new System.Drawing.Size(75, 23);
            this.btnOKRight.TabIndex = 8;
            this.btnOKRight.Text = "&OK";
            this.btnOKRight.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tblParent
            // 
            this.tblParent.BackColor = System.Drawing.SystemColors.Control;
            this.tblParent.ColumnCount = 1;
            this.tblParent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblParent.Controls.Add(this.tblButtons, 0, 1);
            this.tblParent.Controls.Add(this.tlpIconError, 0, 0);
            this.tblParent.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblParent.Location = new System.Drawing.Point(0, 0);
            this.tblParent.Margin = new System.Windows.Forms.Padding(0);
            this.tblParent.Name = "tblParent";
            this.tblParent.RowCount = 3;
            this.tblParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tblParent.Size = new System.Drawing.Size(434, 91);
            this.tblParent.TabIndex = 9;
            // 
            // tblButtons
            // 
            this.tblButtons.ColumnCount = 7;
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tblButtons.Controls.Add(this.btnPrikazi, 1, 0);
            this.tblButtons.Controls.Add(this.btnOKRight, 5, 0);
            this.tblButtons.Controls.Add(this.btnOKCentral, 3, 0);
            this.tblButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblButtons.Location = new System.Drawing.Point(0, 53);
            this.tblButtons.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblButtons.Size = new System.Drawing.Size(434, 29);
            this.tblButtons.TabIndex = 10;
            // 
            // txtDetails
            // 
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.Location = new System.Drawing.Point(0, 91);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.Size = new System.Drawing.Size(434, 0);
            this.txtDetails.TabIndex = 10;
            this.txtDetails.Text = "";
            // 
            // frmError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton = this.btnOKCentral;
            this.ClientSize = new System.Drawing.Size(434, 91);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.tblParent);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 130);
            this.Name = "frmError";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            this.tlpIconError.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgError)).EndInit();
            this.tblErrText.ResumeLayout(false);
            this.tblErrText.PerformLayout();
            this.tblParent.ResumeLayout(false);
            this.tblButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnOKCentral;
        private DevExpress.XtraEditors.CheckButton btnPrikazi;
        private System.Windows.Forms.TextBox txtBoxError;
        private System.Windows.Forms.TableLayoutPanel tlpIconError;
        private DevExpress.XtraEditors.SimpleButton btnOKRight;
        private System.Windows.Forms.PictureBox imgError;
        private System.Windows.Forms.TableLayoutPanel tblErrText;
        private System.Windows.Forms.TableLayoutPanel tblParent;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
        private System.Windows.Forms.RichTextBox txtDetails;
    }
}