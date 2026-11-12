using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class ExamplesImage : DevExpress.XtraEditors.XtraForm
    {
        public int Id { get; set; }

        public string TextName
        {
            get { return txtNaziv.Text; }
        }

        public ExamplesImage()
        {
            InitializeComponent();
        }

        private void ExamplesImage_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        private void InitForm()
        {
            DataTable dt = Data.Examples.Image(Id);
            if (dt.Rows.Count > 0)
            {
                var name = dt.Rows[0]["Name"];

                Image image = Common.Helpers.Images.ByteArrayToImage((byte[])dt.Rows[0]["Content"]);
                pic.Image = image;
                if (name != null)
                    txtNaziv.Text = name.ToString();
            }
            txtNaziv.Select();
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