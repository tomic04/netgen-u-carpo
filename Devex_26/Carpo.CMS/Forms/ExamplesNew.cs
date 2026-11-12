using System;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class ExamplesNew : DevExpress.XtraEditors.XtraForm
    {
        public string TextName
        {
            get { return txtNaziv.Text; }
        }

        public ExamplesNew()
        {
            InitializeComponent();
            txtNaziv.Select();
        }

        public ExamplesNew(string name, string text)
        {
            InitializeComponent();
            txtNaziv.Text = name;
            this.Text = text;
            txtNaziv.Properties.NullValuePrompt = text;
        }

        private void ExamplesNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Potvrdi();
            else if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        private void Potvrdi()
        {
            if (txtNaziv.Text.Trim() != "")
                DialogResult = DialogResult.OK;
            else
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Obavezan unos");
                txtNaziv.Select();
            }
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            Potvrdi();
        }
    }
}