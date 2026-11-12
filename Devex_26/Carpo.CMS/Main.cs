using System.Windows.Forms;

namespace Carpo.CMS
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        public Main()
        {
            InitializeComponent();
            //Icon = Properties.Resources.CMS;
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
        }

        private void nbiExamples_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Form f = Application.OpenForms["Examples"];
            if (f != null)
                f.BringToFront();
            else
            {
                Forms.Examples frm = new Forms.Examples();
                frm.MdiParent = this;
                frm.Show();
            }
        }

        private void nbiElements_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Form f = Application.OpenForms["Elements"];
            if (f != null)
                f.BringToFront();
            else
            {
                Forms.Elements frm = new Forms.Elements();
                frm.MdiParent = this;
                frm.Show();
            }
        }

        private void nbiStruktura_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Form f = Application.OpenForms["ElementsStructure"];
            if (f != null)
                f.BringToFront();
            else
            {
                Forms.ElementsStructure frm = new Forms.ElementsStructure();
                frm.MdiParent = this;
                frm.Show();
            }
        }
    }
}