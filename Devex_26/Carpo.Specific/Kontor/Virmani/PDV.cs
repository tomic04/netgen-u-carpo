using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.Specific.Kontor.Virmani
{
    public partial class PDV : XtraForm
    {
        public int VirmaniId { get; set; }
        public int OperateriId { get; set; }
        public string ConnectionString { get; set; }
        public string sqlText = "";
        public PDV()
        {
            InitializeComponent();
        }

        public void run (string connectionstring,int virmaniid, int operateriid)
        {
            VirmaniId = virmaniid;
            ConnectionString = connectionstring;
            OperateriId = operateriid;
            ShowDialog();
        }
        private void PDV_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtGetPDV = Data.Virmani.GetDataPDV(ConnectionString, 1, VirmaniId, OperateriId, out sqlText).Tables[0];
                //Logger.InfoData("Virmani: VirmaniLoad (dtGetPDV) ", sqlText);

                txtIznosRacuna.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["IznosRacuna"]);
                txtOsnovica0.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Osnovica0"]);
                txtProlazne.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["ProlazneStavke"]);
                txtNePod.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["NePod"]);
                txtOsnovica25.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Osnovica25"]);
                txtOdbiv25.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Odbiv25"]);
                txtNeOdbiv25.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["NeOdbiv25"]);
                txtOsnovica13.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Osnovica13"]);
                txtOdbiv13.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Odbiv13"]);
                txtNeOdbiv13.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["NeOdbiv13"]);
                txtOsnovica5.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Osnovica5"]);
                txtOdbiv5.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["Odbiv5"]);
                txtNeOdbiv5.EditValue = Convert.ToDecimal(dtGetPDV.Rows[0]["NeOdbiv5"]);

                Dictionary<int, string> pdv37List = new Dictionary<int, string>();
                pdv37List.Add(1, "30%");
                pdv37List.Add(4, "50%");
                pdv37List.Add(2, "70%");
                pdv37List.Add(3, "0%");
                var dataList = new List<KeyValuePair<int, string>>(pdv37List);
                cmbPDV.Properties.DataSource = dataList;
                cmbPDV.Properties.DisplayMember = "Value";
                cmbPDV.Properties.ValueMember = "Key";
                cmbPDV.Properties.Columns[0].Visible = false;
                if ((dtGetPDV.Rows[0]["pdv37id"]) != DBNull.Value) cmbPDV.EditValue = Convert.ToInt32(dtGetPDV.Rows[0]["pdv37id"]);
                // FormatTextControl(txtIznosRacuna, "N2");
                if (Convert.ToInt32(dtGetPDV.Rows[0]["ObvOstvarenjePlana"]) == 0) 
                    cheObavezno.EditValue = false;
                else
                    cheObavezno.EditValue = true;
            }
            catch (Exception ex)
            {
                throw;
                //Logger.Error("Virmani: VirmaniLoad (dtGetPDV)" + ex.Message, "");
            }
        }
        private void FormatTextControl(DevExpress.XtraEditors.TextEdit kontrola, string format)
        {
            //kontrola.Properties.AppearanceDisabled.ForeColor = Color.Black;
            Common.Helpers.Formats.FormatTextEdit(format, kontrola.Properties);
            kontrola.Enter += (s, e) =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    kontrola.SelectionStart = 0;
                    kontrola.SelectionLength = kontrola.Text.Length;
                }));
            };
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            try
            {
                int obavezno;
                obavezno = ((bool)cheObavezno.EditValue == true)? 1 : 0;

                Data.Virmani.SpremiVirmanPDV(ConnectionString, 1, VirmaniId, Convert.ToDecimal(txtIznosRacuna.EditValue), Convert.ToDecimal(txtOsnovica0.EditValue), Convert.ToDecimal(txtProlazne.EditValue), Convert.ToDecimal(txtNePod.EditValue),
                    Convert.ToDecimal(txtOsnovica25.EditValue), Convert.ToDecimal(txtOdbiv25.EditValue), Convert.ToDecimal(txtNeOdbiv25.EditValue), Convert.ToDecimal(txtOsnovica13.EditValue), Convert.ToDecimal(txtOdbiv13.EditValue), Convert.ToDecimal(txtNeOdbiv13.EditValue),
                    Convert.ToDecimal(txtOsnovica5.EditValue), Convert.ToDecimal(txtOdbiv5.EditValue), Convert.ToDecimal(txtNeOdbiv5.EditValue), OperateriId, out sqlText, obavezno);
                
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) 
            {
                throw;
                //Logger.Error("Virmani: VirmaniPDV (SpremiVirmanPDV)" + ex.Message, "");
            }
        }
        private void txtIznosRacuna_Leave(object sender, EventArgs e)
        {
            Postimaj("iznosspdv");
        }
        private void txtOsnovica0_Leave(object sender, EventArgs e)
        {
            Postimaj("neoporezivo");
        }
        private void txtProlazne_Leave(object sender, EventArgs e)
        {
            Postimaj("neoporezivo");
        }
        private void txtNePod_Leave(object sender, EventArgs e)
        {
            Postimaj("neoporezivo");
        }
        private void txtOsnovica25_Leave(object sender, EventArgs e)
        {
            Postimaj("osnovica25");
        }
        private void txtOsnovica13_Leave(object sender, EventArgs e)
        {
            Postimaj("osnovica13");
        }
        private void txtOsnovica5_Leave(object sender, EventArgs e)
        {
            Postimaj("osnovica5");
        }
        private void txtOdbiv25_Leave(object sender, EventArgs e)
        {
            Postimaj("odbiv25");
        }
        private void txtOdbiv13_Leave(object sender, EventArgs e)
        {
            Postimaj("odbiv13");
        }
        private void txtOdbiv5_Leave(object sender, EventArgs e)
        {
            Postimaj("odbiv5");
        }
        private void txtNeOdbiv25_Leave(object sender, EventArgs e)
        {
            Postimaj("neodbiv25");
        }
        private void txtNeOdbiv13_Leave(object sender, EventArgs e)
        {
            Postimaj("neodbiv13");
        }
        private void txtNeOdbiv5_Leave(object sender, EventArgs e)
        {
            Postimaj("neodbiv5");
        }
        private void cmbPDV_Leave(object sender, EventArgs e)
        {
            Postimaj("pdv37");
        }
        private void cmbPDV_Clear(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Clear)
            {
                this.cmbPDV.EditValue = null; // Explicitly set null
            }
        }

        private void Postimaj(string action)
        {
            //int? pdv37id = (cmbPDV.EditValue == null)? (int?)null : Convert.ToInt32(cmbPDV.EditValue);

            DataTable dtPostimaj = Data.Virmani.PostimajKontrolePDV(ConnectionString, action, Convert.ToDecimal(txtIznosRacuna.EditValue), 
                Convert.ToDecimal(txtOsnovica0.EditValue), Convert.ToDecimal(txtProlazne.EditValue), Convert.ToDecimal(txtNePod.EditValue),
                    Convert.ToDecimal(txtOsnovica25.EditValue), Convert.ToDecimal(txtOdbiv25.EditValue), Convert.ToDecimal(txtNeOdbiv25.EditValue), 
                    Convert.ToDecimal(txtOsnovica13.EditValue), Convert.ToDecimal(txtOdbiv13.EditValue), Convert.ToDecimal(txtNeOdbiv13.EditValue),
                    Convert.ToDecimal(txtOsnovica5.EditValue), Convert.ToDecimal(txtOdbiv5.EditValue), Convert.ToDecimal(txtNeOdbiv5.EditValue),
                    (int?)cmbPDV.EditValue, out sqlText).Tables[0];
            SetKontrolePDV(dtPostimaj);
        }

        private void SetKontrolePDV(DataTable dt)
        {
            if (dt.Columns.Contains("iznosspdv")) txtIznosRacuna.EditValue = Convert.ToDecimal(dt.Rows[0]["iznosspdv"]);
            if (dt.Columns.Contains("neoporezivo")) txtOsnovica0.EditValue = Convert.ToDecimal(dt.Rows[0]["neoporezivo"]);
            if (dt.Columns.Contains("prolaznestavke")) txtProlazne.EditValue = Convert.ToDecimal(dt.Rows[0]["prolaznestavke"]);
            if (dt.Columns.Contains("nepod")) txtNePod.EditValue = Convert.ToDecimal(dt.Rows[0]["nepod"]);
            if (dt.Columns.Contains("osnovica25")) txtOsnovica25.EditValue = Convert.ToDecimal(dt.Rows[0]["osnovica25"]);
            if (dt.Columns.Contains("odbiv25")) txtOdbiv25.EditValue = Convert.ToDecimal(dt.Rows[0]["odbiv25"]);
            if (dt.Columns.Contains("neodbiv25")) txtNeOdbiv25.EditValue = Convert.ToDecimal(dt.Rows[0]["neodbiv25"]);
            if (dt.Columns.Contains("osnovica13")) txtOsnovica13.EditValue = Convert.ToDecimal(dt.Rows[0]["osnovica13"]);
            if (dt.Columns.Contains("odbiv13")) txtOdbiv13.EditValue = Convert.ToDecimal(dt.Rows[0]["odbiv13"]);
            if (dt.Columns.Contains("neodbiv13")) txtNeOdbiv13.EditValue = Convert.ToDecimal(dt.Rows[0]["neodbiv13"]);
            if (dt.Columns.Contains("osnovica5")) txtOsnovica5.EditValue = Convert.ToDecimal(dt.Rows[0]["osnovica5"]);
            if (dt.Columns.Contains("odbiv5")) txtOdbiv5.EditValue = Convert.ToDecimal(dt.Rows[0]["odbiv5"]);
            if (dt.Columns.Contains("neodbiv5")) txtNeOdbiv5.EditValue = Convert.ToDecimal(dt.Rows[0]["neodbiv5"]);
        }
    }
}
