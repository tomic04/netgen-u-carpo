using System;
using System.Windows.Forms;

namespace Carpo.UI.Forms
{
    public partial class SearchDocFilter : DevExpress.XtraEditors.XtraForm
    {
        public string FilterText
        {
            get { return txtPojam.Text.Trim(); }
        }

        public SearchDocFilter(string name)
        {
            InitializeComponent();

            TranslateControls( name);
        }

        private void TranslateControls(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.Text = Common.Translator.TranslateString("Pronađi dokument");

            }
            else
            {
                
                this.Text = Common.Translator.TranslateString(name);

            }
            lblPojam.Text = Common.Translator.TranslateString("Pojam", true);

        }

        private void SearchDocFilter_Load(object sender, EventArgs e)
        {
            txtPojam.Select();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            Ok();
        }

        private void SearchDocFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Ok();
            else if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        private void Ok()
        {
            if (txtPojam.Text.Trim() == "")
                txtPojam.Select();
            else
                DialogResult = DialogResult.OK;
        }
    }
}