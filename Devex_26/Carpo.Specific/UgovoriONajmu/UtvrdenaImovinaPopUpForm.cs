using System;
using System.Windows.Forms;

namespace Carpo.Specific.UgovoriONajmu
{
    public partial class UtvrdenaImovinaPopUpForm : Form
    {
        public bool UtvrdenaImovinaBoolValue { get; set; }
        public string UtvrdenaImovinaOpis { get; set; } = "";
        public bool UtvrdenaImovinaInd { get; set; }
        public object izmjenaInd { get; set; }

        public UtvrdenaImovinaPopUpForm()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            UgovoriDetail det = new UgovoriDetail();
            if (this.radioGroup1.EditValue != null && !string.IsNullOrEmpty(this.memoEdit1.Text))
            {
                this.UtvrdenaImovinaBoolValue = (bool)this.radioGroup1.EditValue;
                this.UtvrdenaImovinaOpis = this.memoEdit1.Text;
                this.UtvrdenaImovinaInd = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Niste popunili sva polja.");
        }

        private void UtvrdenaImovinaPopUpForm_Load(object sender, EventArgs e)
        {
            this.memoEdit1.Text = this.UtvrdenaImovinaOpis;
            if (this.izmjenaInd != null && (bool)this.izmjenaInd)
                this.radioGroup1.EditValue = this.UtvrdenaImovinaBoolValue;
        }
    }
}