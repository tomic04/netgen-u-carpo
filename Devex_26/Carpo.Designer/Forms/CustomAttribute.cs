using System;
using System.Windows.Forms;

namespace Carpo.Designer.Forms
{
    public partial class frmCustomAttribute : Form
    {
        public string Value
        {
            get { return txtTop.Text + "," + txtBottom.Text + "," + txtLeft.Text + "," + txtRight.Text; }
        }

        public frmCustomAttribute()
        {
            InitializeComponent();
        }

        public frmCustomAttribute(string value)
        {
            InitializeComponent();
            if (!String.IsNullOrEmpty(value))
                parseValue(value);
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void parseValue(string value)
        {
            //"1,1,1,1" =  top , bottom, left,right
            char[] delimiterChars = { ',' };

            string[] values = value.Split(delimiterChars);

            //set values

            txtTop.EditValue = values[0];
            txtBottom.EditValue = values[1];
            txtLeft.EditValue = values[2];
            txtRight.EditValue = values[3];
        }

        private void frmCustomAttribute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
            else if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}