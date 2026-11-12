using System;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.Common.Forms
{
    public partial class frmError : Form
    {
        public frmError(Exception err, string errText, string errCaption)
        {
            InitializeComponent();

            // dodao boju da se izjednači što je više moguće sa XtraMessageBox porukama
            this.BackColor = imgError.BackColor = txtBoxError.BackColor = tblParent.BackColor = tblErrText.BackColor = tblButtons.BackColor
                = System.Drawing.ColorTranslator.FromHtml("#EBECEF");
            imgError.Image = System.Drawing.SystemIcons.Error.ToBitmap();

            this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Height, 130);

            Text = errCaption;
            txtBoxError.Text = errText;

            if (err != null)
            {
                txtDetails.Text = err.ToString();
                btnOKRight.Select();
            }
            else
            {
                btnOKCentral.Visible = true;
                btnOKRight.Visible = false;
                btnPrikazi.Visible = false;
                btnOKCentral.Select();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void checkButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (btnPrikazi.Checked == true)
            {
                btnPrikazi.Text = Translator.TranslateString("Prikaži manje");
                txtDetails.Visible = true;
                this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Height);
                Height = 350;
            }
            else
            {
                btnPrikazi.Text = Translator.TranslateString("Prikaži više");
                txtDetails.Visible = false;
                this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Height, 130);
                Height = 130;
            }
        }
    }
}