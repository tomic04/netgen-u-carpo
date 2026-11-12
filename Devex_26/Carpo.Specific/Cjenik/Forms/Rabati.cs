using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.Cjenik.Forms
{
    public partial class Rabati : Form
    {
        private DataTable dt;
        private int largestLabelLettersNo = 0;

        private string sqlText = "";
        private List<Control> controls = new List<Control>();

        public string ConnectionString { get; set; }
        public decimal RabatVrijednost1 { get; set; }
        public decimal RabatVrijednost2 { get; set; }
        public decimal RabatVrijednost3 { get; set; }
        public decimal RabatVrijednost4 { get; set; }
        public decimal RabatVrijednost5 { get; set; }
        public decimal ZbrojeniRabat { get; set; }

        public Rabati()
        {
            InitializeComponent();
            InitControls();
            SetRabatValues();
        }

        private void InitControls()
        {
            if (this.Controls.Count > 0)
            {
                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    controls.Add(control);
                }
            }

            FormatTextControls();
        }

        private void Rabati_Load(object sender, EventArgs e)
        {
            largestLabelLettersNo = lblRabat1Naziv.Text.Length;

            try
            {
                dt = Data.Ponude.Rabati.GetRabatiNaziv(ConnectionString, out sqlText).Tables[0];
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.ToString());
                DialogResult = DialogResult.Cancel;
            }

            SetLabelsText();

            if (LargestLabel().Text.Length > largestLabelLettersNo)
                this.Width += ((LargestLabel().Text.Length - largestLabelLettersNo) * 5);

            SetRabatValues();
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Rabati_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            ZbrojeniRabat = Data.Ponude.Rabati.ZbrojiRabate(ConnectionString, Convert.ToDecimal(txtRabat1.EditValue), Convert.ToDecimal(txtRabat2.EditValue),
                                Convert.ToDecimal(txtRabat3.EditValue), Convert.ToDecimal(txtRabat4.EditValue), Convert.ToDecimal(txtRabat5.EditValue), out sqlText);

            RabatVrijednost1 = Convert.ToDecimal(txtRabat1.Text);
            RabatVrijednost2 = Convert.ToDecimal(txtRabat2.Text);
            RabatVrijednost3 = Convert.ToDecimal(txtRabat3.Text);
            RabatVrijednost4 = Convert.ToDecimal(txtRabat4.Text);
            RabatVrijednost5 = Convert.ToDecimal(txtRabat5.Text);

            DialogResult = DialogResult.OK;
        }

        private void FormatTextControls()
        {
            var textControls = controls.OfType<TextEdit>();
            foreach (Control control in textControls)
            {
                TextEdit textEdit = (control as TextEdit);
                textEdit.Tag = textEdit.Name.Replace("txt", "");
                textEdit.EnterMoveNextControl = true;
                textEdit.Properties.AppearanceDisabled.ForeColor = Color.Black;
                Common.Helpers.Formats.FormatTextEdit("N4", textEdit.Properties);
                textEdit.Enter += (s, e) =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        textEdit.SelectionStart = 0;
                        textEdit.SelectionLength = textEdit.Text.Length;
                    }));
                };
            }
        }

        private void SetLabelsText()
        {
            lblRabat1Naziv.Text = dt.Rows[0]["Rabat1"].ToString();
            lblRabat2Naziv.Text = dt.Rows[0]["Rabat2"].ToString();
            lblRabat3Naziv.Text = dt.Rows[0]["Rabat3"].ToString();
            lblRabat4Naziv.Text = dt.Rows[0]["Rabat4"].ToString();
            lblRabat5Naziv.Text = dt.Rows[0]["Rabat5"].ToString();
        }

        private void SetRabatValues()
        {
            txtRabat1.EditValue = RabatVrijednost1;
            txtRabat2.EditValue = RabatVrijednost2;
            txtRabat3.EditValue = RabatVrijednost3;
            txtRabat4.EditValue = RabatVrijednost4;
            txtRabat5.EditValue = RabatVrijednost5;
        }

        private Label LargestLabel()
        {
            Label label = new Label();
            label.Text = "";
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is Label)
                {
                    if (c.Name.EndsWith("Naziv"))
                    {
                        if (c.Text.Length > label.Text.Length)
                            label = c as Label;
                    }
                }
            }
            return label;
        }
    }
}