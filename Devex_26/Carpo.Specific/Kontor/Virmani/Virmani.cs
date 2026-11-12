using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.Kontor.Virmani
{
    public partial class Virmani : Shared.Forms.Jupiter
    {
        private enum DtControls
        { ValutaPlacanja, ValutaPokrica, SifraNamjene, PartnerPlatitelj, PartnerPrimatelj }

        private List<string> listaBicPolja = new List<string>() {
                    { "txtBank1" }, { "txtBank2" }, { "txtBank3" }, { "txtBank4" }, { "txtBank5" },
                    { "rdoFizicka" }, { "rdoPravna" }, { "cmbValutaPokrica" },{ "rdoOUR" }, { "rdoBEN" }, { "rdoSHA" }
                };

        private List<string> listaVrsteVirmana = new List<string>() {
                    { "txtNaziv" }, { "lblNaziv" }, { "txtOznaka" }, { "lblOznaka" }
                };

        private List<string> listaPonistiPrekoTrazilice = new List<string>() {
                    { "txtPlatitelj2" }, { "txtPlatitelj3" }, { "txtPrimatelj2" }, { "txtPrimatelj3" }
                };

        private List<string> listaVisible = new List<string>() {
                    { "srcPlatitelj" }, { "txtPlatitelj2" }, { "txtPlatitelj3" },
                    { "srcPrimatelj" }, { "txtPrimatelj2" }, { "txtPrimatelj3" },
                    { "txtPlatiteljSU1" }, { "txtPlatiteljSU2" }, { "txtPlatiteljSU3" },
                    { "txtPrimateljSU1" }, { "txtPrimateljSU2" }, { "txtPrimateljSU3" },
                    { "cmbIBANPlatitelj" }, { "txtIBANPlatitelj" },
                    { "cmbIBANPrimatelj" }, { "txtIBANPrimatelj" }
                };

        private int action = 0, hitno = 0, indprimateljtip = 2, indtroskovna = 3, indPlatitelj = 0, indPrimatelj = 0, izbornik = 2;

        private int? platiteljid = null, platiteljziroid = null, primateljid = null, primateljziroid = null, virmaniid = null, virmanivrsteid = null,
                        valutaplacanjaid = null, valutapokricaid = null, sifranamjeneid = null, bankazemlja = null;

        private string platitelj1 = "", platitelj2 = "", platitelj3 = "", platiteljziro = "", primatelj1 = "", primatelj2 = "", primatelj3 = "",
                        primateljziro = "", banka1 = "", banka2 = "", banka3 = "", banka4 = "", sqlText = "", bicText = "", spreman = "0";

        private bool isInit = false;
        private decimal? iznos = null;
        private DateTime? datum = null;
        private DataTable dtGet = null;
        private DateTime startTime;

        public string ConnectionString { get; set; }
        public int SSOperateriId { get; set; } = 0;

        public int VirmaniId { get; set; } = 0;
        public int VirmaniVrsteId { get; set; } = 0;

        public string server = "", database = "", username = "", password = "";
        public int Action // 0 - unos, 1 - izmijena
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        public int Izbornik // 1 - Vrste virmana, 2 - Virman
        {
            get
            {
                return izbornik;
            }
            set
            {
                izbornik = value;
            }
        }

        #region INIT/LOAD/SHOWN

        public Virmani()
        {
            InitializeComponent();
            FormatTextControl(txtIznos, "N2");
            FormatTextControl(txtBank5, "N0");
            //srcPlatitelj.Properties.RepositoryItems
        }

        private void Virmani_Load(object sender, EventArgs e)
        {
            string sqlText = "";
            int id, query_action;
            if (izbornik == 1)
            {
                ManageControls(listaVrsteVirmana, "visible", true);
                btnDimenzije.Visible = false;
                id = VirmaniVrsteId;
                query_action = 1;
            }
            else
            {
                id = VirmaniId;
                query_action = 0;
            }

            ControlsDatasetGet();
            datDatumIzvrsenja.EditValue = DateTime.Now;

            //IZMJENA(OBJE)
            if (action == 1)
            {
                try
                {
                    dtGet = Data.Virmani.GetData(ConnectionString, query_action, id, out sqlText).Tables[0];
                    Logger.InfoData("Virmani: VirmaniLoad (dtGet) ", sqlText, startTime);
                }
                catch (Exception ex)
                {
                    Logger.Error("Virmani: VirmaniLoad (dtGet)" + ex.Message, "");
                }
                if (dtGet.Rows.Count > 0)
                    LoadDataOnControls(true);
            }
            // UNOS
            else
            {
                if (izbornik == 2) // virman
                {
                    try
                    {
                        dtGet = Data.Virmani.GetData(ConnectionString, 1, VirmaniVrsteId, out sqlText).Tables[0];
                        Logger.InfoData("Virmani: VirmaniLoad (dtGet) ", sqlText, startTime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Virmani: VirmaniLoad (dtGet)" + ex.Message, "");
                    }
                    LoadDataOnControls(false);
                }
                else // virman vrste
                    chkSpreman.Checked = true;
            }
        }

        private void Virmani_Shown(object sender, EventArgs e)
        {
            SetControlFocus();
        }

        #endregion INIT/LOAD/SHOWN

        #region DATA

        private void LoadDataOnControls(bool isIzmjena)
        {
            isInit = true;
            /////////////////////
            ///// PLATITELJ /////
            /////////////////////
            if ((int)dtGet.Rows[0]["IndPlatitelj"] == 1) //slobodan unos
            {
                chkSlobodanUnosPlatitelj.Checked = true;
                SetControlValue(txtPlatiteljSU1, typeof(string), dtGet, "Platitelj1");
                SetControlValue(txtPlatiteljSU2, typeof(string), dtGet, "Platitelj2");
                SetControlValue(txtPlatiteljSU3, typeof(string), dtGet, "Platitelj3");

                string iban = Convert.ToString(dtGet.Rows[0]["NalogodavacZiro"]);
                if (!String.IsNullOrEmpty(iban))
                {
                    //RefreshIBANControlDataSource(cmbIBANPlatitelj, 1, iban, 0, "Platitelj");
                    SetControlValue(txtIBANPlatitelj, typeof(string), dtGet, "NalogodavacZiro");
                }
            }
            else
            {
                if (dtGet.Rows[0]["NalogodavacId"] != null)
                {
                    SetControlValue(srcPlatitelj, typeof(int), dtGet, "NalogodavacId");

                    string iban = Convert.ToString(dtGet.Rows[0]["NalogodavacZiro"]);
                    if (!String.IsNullOrEmpty(iban) && (srcPlatitelj.EditValue != null))
                    {
                        RefreshIBANControlDataSource(cmbIBANPlatitelj, 1, iban, (int)srcPlatitelj.EditValue, "Platitelj");
                        SetControlValue(cmbIBANPlatitelj, typeof(string), dtGet, "NalogodavacZiro");
                    }
                }
            }
            /////////////////////
            ///// PRIMATELJ /////
            /////////////////////
            if ((int)dtGet.Rows[0]["IndPrimatelj"] == 1) //slobodan unos
            {
                chkSlobodanUnosPrimatelj.Checked = true;
                SetControlValue(txtPrimateljSU1, typeof(string), dtGet, "Primatelj1");
                SetControlValue(txtPrimateljSU2, typeof(string), dtGet, "Primatelj2");
                SetControlValue(txtPrimateljSU3, typeof(string), dtGet, "Primatelj3");
                string iban = Convert.ToString(dtGet.Rows[0]["PartneriZiro"]);
                if (!String.IsNullOrEmpty(iban))
                {
                    //RefreshIBANControlDataSource(cmbIBANPrimatelj, 1, iban, 0, "Primatelj");
                    SetControlValue(txtIBANPrimatelj, typeof(string), dtGet, "PartneriZiro");
                }
            }
            else
            {
                if (dtGet.Rows[0]["PartneriId"] != null)
                {
                    SetControlValue(srcPrimatelj, typeof(int), dtGet, "PartneriId");
                    string iban = Convert.ToString(dtGet.Rows[0]["PartneriZiro"]);
                    if (!String.IsNullOrEmpty(iban) && srcPrimatelj.EditValue != null)
                    {
                        RefreshIBANControlDataSource(cmbIBANPrimatelj, 1, iban, (int)srcPrimatelj.EditValue, "Primatelj");
                        SetControlValue(cmbIBANPrimatelj, typeof(string), dtGet, "PartneriZiro");
                    }
                }
            }
            /////////////////////
            /////// OSTALO //////
            /////////////////////
            if ((int)dtGet.Rows[0]["IndHitno"] == 1)
                chkHitno.Checked = true;
            else
                chkHitno.Checked = false;

            if ((string)dtGet.Rows[0]["Spreman"] == "1")
                chkSpreman.Checked = true;
            else
                chkSpreman.Checked = false;

            // DORADE iz CASE-a 33153 (pri unosu novog virmana postavit defaultnu vrijednost valute na HRK)
            if (isIzmjena == false)
            {
                string outVirmaniSqlString = "";
                cmbValutaPlacanja.EditValue = Convert.ToInt32(Data.Virmani.GetHrkId(ConnectionString, out outVirmaniSqlString).ToString());
                Logger.InfoData("Virmani: GetHrkId (defaultna valuta) ", outVirmaniSqlString, startTime);
            }
            else
                SetControlValue(cmbValutaPlacanja, typeof(int), dtGet, "ValuteId");
            SetControlValue(txtModelPlatitelj, typeof(string), dtGet, "PzvbrZad1");
            SetControlValue(txtPozivNaBrojPlatitelj, typeof(string), dtGet, "PzvbrZad2");

            SetControlValue(txtIznos, typeof(string), dtGet, "Iznos");

            SetControlValue(txtModelPrimatelj, typeof(string), dtGet, "PzvbrOd1");
            SetControlValue(txtPozivNaBrojPrimatelj, typeof(string), dtGet, "PzvbrOd2");
            SetControlValue(cmbSifraNamjene, typeof(int), dtGet, "SifraNamjeneId");
            SetControlValue(datDatumIzvrsenja, typeof(DateTime), dtGet, "Datum");
            SetControlValue(txtOpisPlacanja1, typeof(string), dtGet, "Svrha1");
            SetControlValue(txtOpisPlacanja2, typeof(string), dtGet, "Svrha2");
            SetControlValue(txtOpisPlacanja3, typeof(string), dtGet, "Svrha3");
            SetControlValue(txtOpisPlacanja4, typeof(string), dtGet, "Svrha4");

            /////////////////////
            /////// BIC //////
            /////////////////////
            if (indPrimatelj == 1 ||
                (cmbIBANPrimatelj.EditValue != null && !cmbIBANPrimatelj.EditValue.ToString().ToLower().StartsWith("hr")))
            {
                SetControlValue(txtBank1, typeof(string), dtGet, "BIC");
                SetControlValue(txtBank2, typeof(string), dtGet, "BankaNaziv");
                SetControlValue(txtBank3, typeof(string), dtGet, "BankaAdresa");
                SetControlValue(txtBank4, typeof(string), dtGet, "BankaMjesto");
                SetControlValue(txtBank5, typeof(int), dtGet, "BankaZemlja");
                SetControlValue(cmbValutaPokrica, typeof(int), dtGet, "ValutePokricaId");
                SetBicRadioButtons(dtGet);
            }

            if (isIzmjena == true && izbornik == 1)
            {
                SetControlValue(txtNaziv, typeof(string), dtGet, "Naziv");
                SetControlValue(txtOznaka, typeof(string), dtGet, "Oznaka");
            }
            isInit = false;
        }

        private void SetParameterValues()
        {
            if (izbornik == 1) // Virmani vrste (unos/izmjena)
            {
                if (action == 0) virmanivrsteid = null;
                else virmanivrsteid = VirmaniVrsteId;
            }
            else // Virman (unos/izmjena)
            {
                if (action == 0)
                {
                    virmaniid = null;
                    virmanivrsteid = VirmaniVrsteId;
                }
                else
                {
                    virmaniid = VirmaniId;
                    virmanivrsteid = null;
                }
            }

            // Hitno
            if (chkHitno.Checked) hitno = 1;
            else hitno = 0;

            // Spreman
            if (chkSpreman.Checked) spreman = "1";
            else spreman = "0";

            // Slobodan unos platitelj
            if (chkSlobodanUnosPlatitelj.Checked)
            {
                indPlatitelj = 1;
                platitelj1 = txtPlatiteljSU1.Text;
                platitelj2 = txtPlatiteljSU2.Text;
                platitelj3 = txtPlatiteljSU3.Text;
                platiteljziroid = null;
                platiteljziro = txtIBANPlatitelj.Text;
            }
            else
            {
                GetVariableValue(srcPlatitelj, out platiteljid, null);
                //GetVariableValue(cmbIBANPlatitelj, out platiteljziroid, null);
                platiteljziroid = GetSpecificColumnValue(cmbIBANPlatitelj);
                platiteljziro = cmbIBANPlatitelj.Text;
                platitelj1 = srcPlatitelj.Text;
                platitelj2 = txtPlatitelj2.Text;
                platitelj3 = txtPlatitelj3.Text;
            }

            // Slobodan unos primatelj
            if (chkSlobodanUnosPrimatelj.Checked)
            {
                indPrimatelj = 1;
                primatelj1 = txtPrimateljSU1.Text;
                primatelj2 = txtPrimateljSU2.Text;
                primatelj3 = txtPrimateljSU3.Text;
                primateljziroid = null;
                primateljziro = txtIBANPrimatelj.Text;
            }
            else
            {
                GetVariableValue(srcPrimatelj, out primateljid, Convert.ToString(srcPrimatelj.EditValue) != "");
                //GetVariableValue(cmbIBANPrimatelj, out primateljziroid, Convert.ToString(cmbIBANPrimatelj.EditValue) != "");
                primateljziroid = GetSpecificColumnValue(cmbIBANPrimatelj);
                primateljziro = cmbIBANPrimatelj.Text;
                primatelj1 = srcPrimatelj.Text;
                primatelj2 = txtPrimatelj2.Text;
                primatelj3 = txtPrimatelj3.Text;
            }

            if (txtBank1.Enabled)
            {
                banka1 = txtBank1.Text;
                banka2 = txtBank2.Text;
                banka3 = txtBank3.Text;
                banka4 = txtBank4.Text;

                // Pravna ili fizička osoba
                if (rdoPravna.Checked) indprimateljtip = 2;
                else if (rdoFizicka.Checked) indprimateljtip = 1;

                // Troškovna opcija
                if (rdoOUR.Checked) indtroskovna = 1;
                else if (rdoBEN.Checked) indtroskovna = 2;
                else if (rdoSHA.Checked) indtroskovna = 3;
            }

            if (txtIznos.Text != "")
                iznos = Convert.ToDecimal(txtIznos.Text);
            else
                iznos = null;

            GetVariableValue(cmbValutaPlacanja, out valutaplacanjaid, null);
            GetVariableValue(cmbValutaPokrica, out valutapokricaid, null);
            GetVariableValue(cmbSifraNamjene, out sifranamjeneid, null);
            GetVariableValue(txtBank5, out bankazemlja, null);

            if (chkDatumIzvrsenja.Checked && datDatumIzvrsenja.EditValue != null)
                datum = Convert.ToDateTime(datDatumIzvrsenja.EditValue);
            else
                datum = null;
        }

        #endregion DATA

        #region EVENTS

        private void Virmani_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        private void searchEdit_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit srcSender = sender as SearchLookUpEdit;
            string nastavak = srcSender.Name.Substring(3);

            if ((Convert.ToString(srcSender.EditValue) != "") && ((int)srcSender.EditValue > 0))
            {
                using (DataTable dtAdresa = GetAdresaDataSet((int)srcSender.EditValue, nastavak))
                {
                    foreach (Control c in panel2.Controls)
                    {
                        if (c.Name == "txt" + nastavak + "2")
                            SetControlValue((c as BaseEdit), typeof(string), dtAdresa, "Adresa");
                        else if (c.Name == "txt" + nastavak + "3")
                            SetControlValue((c as BaseEdit), typeof(string), dtAdresa, "PP");
                        else if (c.Name == "cmbIBAN" + nastavak)
                        {
                            LookUpEdit le = c as LookUpEdit;
                            RefreshIBANControlDataSource(le, 0, "", (int)srcSender.EditValue, nastavak);

                            if (isInit == false) // ************ ako ima defaultni postavi ga odmah
                            {
                                sqlText = "";
                                DataTable dtDefaultIbanGet = Data.Virmani.GetIBANComboData(ConnectionString, 2, "", (int)srcSender.EditValue, out sqlText).Tables[0];

                                if (dtDefaultIbanGet.Rows.Count > 0)
                                {
                                    string iban = Convert.ToString(dtDefaultIbanGet.Rows[0]["Iban"]);
                                    if (!string.IsNullOrEmpty(iban))
                                        SetControlValue(le, typeof(string), dtDefaultIbanGet, "Iban");
                                }
                            }
                            le.ProcessNewValue += new ProcessNewValueEventHandler(Properties_ProcessNewValue);
                        }
                    }
                }

                if (nastavak == "Platitelj")
                    txtPlatiteljSU2.Focus();
                else
                    txtPrimateljSU2.Focus();
            }
            else
                ResetControls(nastavak);
        }

        private void btnPDV_Click(object sender, EventArgs e)
        {
            //provjeri je li ručni, ako nema fakturedobid, onda otvori
            sqlText = "";
            if (Convert.ToInt32(Data.Virmani.GetEnabledPDV(ConnectionString,0,VirmaniId,out sqlText))==1)
            {
                try 
                { 
                    PDV pdv = new PDV();
                    pdv.run(ConnectionString, VirmaniId, SSOperateriId);
                }
                catch
                {
                    MessageBox.Show(@"Došlo je do pogreške u pokretanju PDV-a", @"Greška");
                }
                finally { DialogResult = DialogResult.None; }
            }
        }

        private void searchEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            SearchLookUpEdit search = sender as SearchLookUpEdit;
            EditorButton button = e.Button;
            EditorButtonClick(search, e, button);
        }

        private void btnDimenzije_Click(object sender, EventArgs e)
        {
            string tip = "VirmaniId";
            int id = (action == 0) ? 0 : (VirmaniId);
            int glaid = (action == 0) ? 1 : 0;
            int partneriid = 0;//(PartneriId == null) ? 0 : (int)PartneriId;
            if (SetConnStr(ConnectionString))
            {
                try
                {
                    DimenzijeNet.Dimenzije dim = new DimenzijeNet.Dimenzije();
                    //nemam glaid pa fejkam s jedinicom
                    dim.run(server, database, username, password, SSOperateriId, 0, 0, 0, "", tip, id, glaid, partneriid);
                }
                catch
                {
                    MessageBox.Show(@"Došlo je do pogreške u pokretanju dimenzija", @"Greška");
                }
                finally { DialogResult = DialogResult.None; }//ako ovog nema zatvori se i frmIzmijeni
            }
        }

        private bool SetConnStr(string cs)
        {
            try
            {
                string[] connStrings = cs.Split(';');
                server = connStrings.FirstOrDefault(s => s.Contains("Source"));
                database = connStrings.FirstOrDefault(s => s.Contains("Catalog"));
                server = server.Substring(server.IndexOf("=") + 1);
                database = database.Substring(database.IndexOf("=") + 1);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void cmbIBANPrimatelj_EditValueChanged(object sender, EventArgs e)
        {
            if (cmbIBANPrimatelj.EditValue != null && !cmbIBANPrimatelj.EditValue.ToString().ToLower().StartsWith("hr"))
            {
                ManageControls(listaBicPolja, "enabled", true);
                string sqlText = "";

                DataTable dt;
                try
                {
                    dt = Data.Virmani.GetBicData(ConnectionString, Convert.ToInt32(srcPrimatelj.EditValue),
                        (int)GetIbanId("ParZiroId"), out sqlText).Tables[0];
                    Logger.InfoData("Virmani: GetBicData ", sqlText, startTime);
                }
                catch (Exception ex)
                {
                    Logger.Error("Virmani: GetBicData ", ex.Message, ex);
                    return;
                }

                SetControlValue(txtBank1, typeof(string), dt, "SWIFT");
                SetControlValue(txtBank2, typeof(string), dt, "BankaNaziv");
                SetControlValue(txtBank3, typeof(string), dt, "BankaAdresa");
                SetControlValue(txtBank4, typeof(string), dt, "BankaMjesto");
                SetControlValue(txtBank5, typeof(int), dt, "BankaZemlja");
                //SetControlValue(cmbValutaPokrica, typeof(int), dt, "ValutePokricaId");
                //SetBicRadioButtons(dt);
            }
            else
                ResetBicControls();
        }

        private void Properties_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            LookUpEdit le = sender as LookUpEdit;
            if ((string)e.DisplayValue != "") // ako smo unijeli novu vrijednost koja nije prazan string
            {
                if (le == cmbIBANPlatitelj)
                    ValidateNewIBAN((string)e.DisplayValue, cmbIBANPlatitelj);
                else if (le == cmbIBANPrimatelj)
                    ValidateAndOrSaveNewIBAN((string)e.DisplayValue, cmbIBANPrimatelj);
            }
        }

        private void txtBank1_Enter(object sender, EventArgs e)
        {
            bicText = txtBank1.Text;
        }

        private void txtBank1_Leave(object sender, EventArgs e)
        {
            // samo ako se promijenio podatak u polju update-aj SWIFT te ako nismo na slobodnom unosu
            if ((txtBank1.Text != bicText) && (chkSlobodanUnosPrimatelj.Checked == false))
            {
                int? pzid = null;
                string column = "ParZiroId";

                try
                {
                    pzid = GetIbanId(column);
                    if (pzid == null)
                        return;
                }
                catch (Exception ex)
                {
                    Logger.Error("Virmani: BIC Update", ex.Message);
                    return;
                }

                Data.Virmani.SwiftUpdate(ConnectionString, txtBank1.Text, (int)pzid, out sqlText);
            }
            else
                return;
        }

        private int? GetIbanId(string column)
        {
            DataRowView rowView = (DataRowView)cmbIBANPrimatelj.GetSelectedDataRow();
            if (rowView != null)
            {
                DataRow row = rowView.Row;
                if (row.Table.Columns.Contains(column))
                    return Convert.ToInt32(row[column]);
                else
                    return null;
            }
            else
                return null;
        }

        private void chkDatumIzvrsenja_CheckedChanged_1(object sender, EventArgs e)
        {
            datDatumIzvrsenja.Enabled = chkDatumIzvrsenja.Checked;
        }

        private void chkSlobodanUnos_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit chk = sender as CheckEdit;
            string temp = chk.Name.ToString().Substring(15);
            SlobodanUnosKontrole(temp, chk.Checked);

            if (temp == "Platitelj")
            {
                if (chkSlobodanUnosPlatitelj.Checked)
                    indPlatitelj = 1;
                else
                    indPlatitelj = 0;
            }
            else if (temp == "Primatelj")
            {
                if (chkSlobodanUnosPrimatelj.Checked)
                    indPrimatelj = 1;
                else
                    indPrimatelj = 0;
            }
        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            SetParameterValues();

            if (izbornik == 1) // Vrste virmana
            {
                try
                {
                    sqlText = "";
                    startTime = DateTime.Now;

                    Data.Virmani.SpremiVirmaniVrste(ConnectionString, action, virmanivrsteid, txtNaziv.Text, txtOznaka.Text, platiteljid, platiteljziroid, platiteljziro,
                                                    txtModelPlatitelj.Text, txtPozivNaBrojPlatitelj.Text, txtOpisPlacanja1.Text, txtOpisPlacanja2.Text, txtOpisPlacanja3.Text,
                                                    txtOpisPlacanja4.Text, iznos, primateljziroid, primateljziro, primateljid, txtModelPrimatelj.Text,
                                                    txtPozivNaBrojPrimatelj.Text, datum, SSOperateriId, spreman, valutaplacanjaid, hitno, indprimateljtip, valutapokricaid,
                                                    indtroskovna, sifranamjeneid, banka1, banka2, banka3, banka4, bankazemlja, indPrimatelj, indPlatitelj,
                                                    platitelj1, platitelj2, platitelj3, primatelj1, primatelj2, primatelj3, out sqlText);

                    Logger.InfoData("Virmani: Spremi vrsta virmana ", sqlText, startTime);
                }
                catch (Exception ex)
                {
                    Logger.InfoData("Virmani: Spremi vrsta virmana ", sqlText, startTime);
                    Logger.Error("Greška pri spremanju Vrste virmana", ex.Message, ex);
                    //Common.Helpers.JMessages.Warning(ex.Message);
                    return;
                }
            }
            else // Virman
            {
                try
                {
                    sqlText = "";
                    startTime = DateTime.Now;

                    Data.Virmani.SpremiVirman(ConnectionString, action, virmaniid, virmanivrsteid, platiteljid, platiteljziroid, platiteljziro, txtModelPlatitelj.Text,
                                                txtPozivNaBrojPlatitelj.Text, txtOpisPlacanja1.Text, txtOpisPlacanja2.Text, txtOpisPlacanja3.Text, txtOpisPlacanja4.Text,
                                                iznos, primateljziroid, primateljziro, primateljid, txtModelPrimatelj.Text,
                                                txtPozivNaBrojPrimatelj.Text, datum, SSOperateriId, spreman, valutaplacanjaid, hitno, indprimateljtip, valutapokricaid,
                                                indtroskovna, sifranamjeneid, banka1, banka2, banka3, banka4, bankazemlja, indPrimatelj, indPlatitelj,
                                                platitelj1, platitelj2, platitelj3, primatelj1, primatelj2, primatelj3, out sqlText);

                    Logger.InfoData("Virmani: Spremi virman ", sqlText, startTime);
                }
                catch (Exception ex)
                {
                    Logger.InfoData("Virmani: Spremi virman ", sqlText, startTime);
                    Logger.Error("Greška pri spremanju Virmana", ex.Message, ex);

                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void EditorButtonClick(SearchLookUpEdit sender, ButtonPressedEventArgs bpea, EditorButton eb)
        {
            if (eb.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                sender.EditValue = null;
        }

        #endregion EVENTS

        #region FUNCTIONS

        private void ManageControls(List<string> lista, string prop, bool yesNo)
        {
            foreach (Control c in panel2.Controls)
            {
                if (lista.Contains(c.Name))
                {
                    if (prop == "visible")
                        c.Visible = yesNo;
                    else if (prop == "enabled")
                        c.Enabled = yesNo;
                }
            }
        }

        private void ResetControls(string nastavak)
        {
            foreach (Control c in panel2.Controls)
            {
                if (listaPonistiPrekoTrazilice.Contains(c.Name))
                {
                    if (c.Name.Contains(nastavak) && c is TextEdit)
                        c.Text = "";
                }
            }
            if (nastavak == "Platitelj")
                cmbIBANPlatitelj.Properties.DataSource = null;
            else
            {
                cmbIBANPrimatelj.Properties.DataSource = null;
                cmbIBANPrimatelj.EditValue = null;
                //ResetBicControls();
            }
        }

        private void ResetBicControls()
        {
            cmbValutaPokrica.EditValue = null;
            txtBank1.EditValue = "";
            txtBank2.EditValue = "";
            txtBank3.EditValue = "";
            txtBank4.EditValue = "";
            txtBank5.EditValue = "";
            ManageControls(listaBicPolja, "enabled", false);
        }

        private void ValidateNewIBAN(string value, Control control)
        {
            int ibanReturn = -1;

            try
            {
                ibanReturn = Data.Virmani.CheckAndOrAddIban(ConnectionString, 10, value, 0, out sqlText);
            }
            catch
            {
                XtraMessageBox.Show("Unešen je neispravan IBAN !");
                return;
            }

            if (ibanReturn == -1) // ako je neispravan IBAN stora vrati -1
            {
                XtraMessageBox.Show("Unešen je neispravan IBAN !");
                return;
            }
            else
            {
                if (control == cmbIBANPlatitelj)
                    txtModelPlatitelj.Select();
                else if (control == cmbIBANPrimatelj)
                    txtModelPrimatelj.Select();
            }
        }

        private void ValidateAndOrSaveNewIBAN(string value, Control control)
        {
            int ibanReturn = -1;
            int partneriid = 0;

            if (chkSlobodanUnosPrimatelj.Checked)
                ValidateNewIBAN(value, cmbIBANPrimatelj);
            else
            {
                if (srcPrimatelj.EditValue != null)
                    partneriid = (int)srcPrimatelj.EditValue;

                try
                {
                    ibanReturn = Data.Virmani.CheckAndOrAddIban(ConnectionString, 0, value, partneriid, out sqlText);
                }
                catch
                {
                    XtraMessageBox.Show("Unešen je neispravan IBAN !");
                    return;
                }

                if (ibanReturn == -1) // ako je neispravan IBAN stora vrati -1
                {
                    XtraMessageBox.Show("Unešen je neispravan IBAN !");
                    //return;
                }
                else
                {
                    RefreshIBANControlDataSource(cmbIBANPrimatelj, 1, "", (int)srcPrimatelj.EditValue, "Primatelj");
                    cmbIBANPrimatelj.EditValue = value;

                    //txtModelPrimatelj.Select();
                }
                if (!value.ToLower().StartsWith("hr"))
                    ManageControls(listaBicPolja, "enabled", true);
            }
        }

        private void ControlsDatasetGet() // puni tražilice i comboboxe
        {
            cmbValutaPlacanja.Properties.DataSource = PopulateControls(DtControls.ValutaPlacanja);
            cmbValutaPokrica.Properties.DataSource = PopulateControls(DtControls.ValutaPokrica);
            cmbSifraNamjene.Properties.DataSource = PopulateControls(DtControls.SifraNamjene);
            srcPlatitelj.Properties.View.Columns.AddField("Naziv").Visible = true;
            srcPlatitelj.Properties.DataSource = PopulateControls(DtControls.PartnerPlatitelj);
            srcPrimatelj.Properties.View.Columns.AddField("Naziv").Visible = true;
            srcPrimatelj.Properties.DataSource = PopulateControls(DtControls.PartnerPrimatelj);
        }

        private DataTable PopulateControls(DtControls control)
        {
            DataTable dt = null;
            sqlText = "";
            startTime = DateTime.Now;
            try
            {
                if (control == DtControls.ValutaPlacanja || control == DtControls.ValutaPokrica)
                    dt = Data.Virmani.GetValute(ConnectionString, out sqlText).Tables[0];
                else if (control == DtControls.SifraNamjene)
                    dt = Data.Virmani.GetSifraNamjene(ConnectionString, out sqlText).Tables[0];
                else if (control == DtControls.PartnerPlatitelj || control == DtControls.PartnerPrimatelj)
                    dt = Data.Virmani.GetPartneri(ConnectionString, out sqlText).Tables[0];
                Logger.InfoData("Virmani: Populate " + control.ToString(), sqlText, startTime);
            }
            catch (Exception ex)
            {
                Logger.Error("Virmani: Populate " + control.ToString(), ex.Message, ex);
            }
            return dt;
        }

        private void SetBicRadioButtons(DataTable dtBICGet)
        {
            try
            {
                if (Convert.ToInt32(dtBICGet.Rows[0]["IndPrimateljTip"]) == 2)
                    rdoPravna.Checked = true;
                else if (Convert.ToInt32(dtBICGet.Rows[0]["IndPrimateljTip"]) == 1)
                    rdoFizicka.Checked = true;

                if (Convert.ToInt32(dtBICGet.Rows[0]["IndTroskovna"]) == 1)
                    rdoOUR.Checked = true;
                else if (Convert.ToInt32(dtBICGet.Rows[0]["IndTroskovna"]) == 2)
                    rdoBEN.Checked = true;
                else if (Convert.ToInt32(dtBICGet.Rows[0]["IndTroskovna"]) == 3)
                    rdoSHA.Checked = true;
            }
            catch
            {
                MessageBox.Show("SetBicRadioButtons()", "Greška");
            }
        }

        private void SetControlValue(DevExpress.XtraEditors.BaseEdit control, Type type, DataTable dtConvert, string field)
        {
            if (dtConvert.Columns.Contains(field))
            {
                object value = dtConvert.Rows[0][field];

                if (value != DBNull.Value)
                {
                    if (control == datDatumIzvrsenja)
                        chkDatumIzvrsenja.Checked = true;

                    if (type == typeof(string))
                    {
                        string tekst = Carpo.Common.Helpers.Conversion.Convert(value, type, null).ToString();
                        control.EditValue = tekst.Trim();
                    }
                    else
                        control.EditValue = Carpo.Common.Helpers.Conversion.Convert(value, type, null);
                }
                else if (value == DBNull.Value)
                {
                    if (control == datDatumIzvrsenja)
                        chkDatumIzvrsenja.Checked = false;
                    else if (type == typeof(string) && control.EditValue != null)
                        control.EditValue = null;
                }
            }
            else
                Logger.Warn("Virmani: SetControlValue", "U datasetu ne postoji kolona '" + field + "'!");
            //XtraMessageBox.Show("U datasetu ne postoji kolona '" + field + "'!");
        }

        private object ConvertFromControl(Control c, Type type)
        {
            if ((c as BaseEdit).EditValue != null)
                return Carpo.Common.Helpers.Conversion.Convert((c as BaseEdit).EditValue, type, null);
            else return null;
        }

        public static object SendIntParamToDatabase(object objekt)
        {
            if (objekt == null)
                return DBNull.Value;
            else
                return objekt;
        }

        public static object SendStringParamToDatabase(string tekst, int duzina)
        {
            string param = "";
            if (tekst.Length > duzina)
                param = tekst.Substring(0, duzina);
            else
                param = tekst;

            if (param == "")
                return DBNull.Value;
            else
                return param;
        }

        private void GetVariableValue(Control control, out int? variable, bool? condition)
        {
            BaseEdit be = control as BaseEdit;

            if (be.EditValue != null && (condition == null || condition == true))
            {
                try
                {
                    variable = Convert.ToInt32(be.EditValue);
                }
                catch
                {
                    variable = null;
                }
            }
            else
                variable = null;
        }

        private int? GetSpecificColumnValue(LookUpEdit le)
        {
            int? value = null;
            object row = le.Properties.GetDataSourceRowByKeyValue(le.EditValue);
            if (row != null)
            {
                try
                {
                    value = Convert.ToInt32(((DataRowView)row)["ParZiroId"]);
                }
                catch
                {
                    return null;
                }
            }
            return value;
        }

        private void RefreshIBANControlDataSource(LookUpEdit le, int action, string iban, int partneriid, string kontrola)
        {
            DataTable dt = null;
            sqlText = "";
            startTime = DateTime.Now;
            try
            {
                dt = Data.Virmani.GetIBANComboData(ConnectionString, action, iban, partneriid, out sqlText).Tables[0];
                Logger.InfoData("Virmani: Populate Iban" + kontrola, sqlText, startTime);
            }
            catch (Exception ex)
            {
                Logger.Error("Virmani: Populate Iban" + kontrola, ex.Message, ex);
                return;
            }
            le.Properties.DataSource = dt;
            le.EditValue = null;
        }

        private void FormatTextControl(TextEdit kontrola, string format)
        {
            //kontrola.Properties.AppearanceDisabled.ForeColor = Color.Black;
            Common.Helpers.Formats.FormatTextEdit(format, kontrola.Properties);
            kontrola.Enter += (s, e) =>
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    kontrola.SelectionStart = 0;
                    kontrola.SelectionLength = txtIznos.Text.Length;
                }));
            };
        }

        private void SlobodanUnosKontrole(string grupa_kontrola, bool checked_status)
        {
            foreach (Control c in panel2.Controls)
            {
                if (listaVisible.Contains(c.Name) && c.Name.Contains(grupa_kontrola))
                    c.Visible = !c.Visible;
            }

            if (grupa_kontrola == "Primatelj")
                ManageControls(listaBicPolja, "enabled", checked_status);
        }

        private DataTable GetAdresaDataSet(int partneriid, string kontrola)
        {
            DataTable dt = null;
            sqlText = "";
            startTime = DateTime.Now;
            try
            {
                dt = Data.Virmani.GetPartnerAdress(ConnectionString, partneriid, out sqlText).Tables[0];
                Logger.InfoData("Virmani: Populate Adresa" + kontrola, sqlText, startTime);
            }
            catch (Exception ex)
            {
                Logger.Error("Virmani: Populate Adresa" + kontrola, ex.Message, ex);
                return null;
            }
            return dt;
        }

        private void SetControlFocus()
        {
            if (izbornik == 2) // VIRMAN
            {
                if (action == 0) // unos
                {
                    if (dtGet != null && dtGet.Rows[0]["NalogodavacId"] != null)
                        txtIznos.Select();
                    else
                    {
                        if (chkSlobodanUnosPlatitelj.Checked)
                            txtPlatiteljSU1.Focus();
                        else
                            srcPlatitelj.Select();
                    }
                }
                else
                    txtIznos.Select();
            }
            else // VRSTE VIRMANA
                txtNaziv.Focus();
        }

        #endregion FUNCTIONS
    }
}