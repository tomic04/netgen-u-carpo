using Carpo.Data;
using Carpo.Specific.UgovoriONajmu.Helpers.Models;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.UgovoriONajmu
{
    public partial class UgovoriDetail : XtraUserControl, IDisposable
    {
        public int GlaId { get; set; }
        public int Operateriid { get; set; }
        public bool UtvrdenaImovinaInd { get; set; }
        public bool UtvrdenaImovinaBoolValue { get; set; }
        public string UtvrdenaImovinaOpis { get; set; }
        public int VrstaUgovoraId { get; set; }
        public string VrstaUgovoraOznaka { get; set; }
        public DataTable DT { get; set; }
        public bool disableAllControls { get; set; }
        public DateTime ProcDatumPrestankaVazenjaNajma { get; set; }
        private object imovinaParamsSet { get; set; } = null;
        private DataRow glavaPoljaDR { get; set; }
        private int? lastKnownPartneriId { get; set; }
        private string lastKnownPartneriText { get; set; }
        private int? lastKnownOrgShemaId { get; set; }
        private string lastKnownOrgShemaText { get; set; }
        private int detid;

        public int DetId
        {
            get { return detid; }
            set
            {
                detid = value;
                glavaPoljaDR = GetGlavaPolja().Rows[0];
                this.VrstaUgovoraId = Convert.ToInt32(GetGlavaPolja().Rows[0]["ugovorinajamvrsteid"]);
                SetGrid();
                InitForm();
            }
        }

        public bool IsprazniKontrole
        {
            get
            {
                return true;
            }
            set
            {
                try
                {
                    this.isprazniKontrole();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private string requiredString()
        {
            return " <color=200,0,0>*</color>";
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    if (DT == null)
                        return false;

                    if (validateRequiredFields() == false)
                        return false;

                    if (Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'")))
                    {
                        if (GetFieldValue("ugovoreniiznos") == DBNull.Value
                            || string.IsNullOrEmpty(GetFieldValue("ugovoreniiznos").ToString())
                            || Convert.ToDecimal(GetFieldValue("ugovoreniiznos")) == 0)
                        {
                            XtraMessageBox.Show("Odabrali ste vrstu troška najam, a ugovoreni iznos je 0.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }

                    if (((GetFieldValue("placanjenalogomind") != DBNull.Value) && (Convert.ToInt32(GetFieldValue("placanjenalogomind")) == 1))
                        && ((GetFieldValue("ugovorinajammjernamjestatipplacanjaid") != DBNull.Value) && Convert.ToInt32(GetFieldValue("ugovorinajammjernamjestatipplacanjaid")) == 0))
                    {
                        XtraMessageBox.Show("Odabrali ste opciju plaćanja nalogom i tip plaćanja : Uključeno u najam.", "Neispravni podaci", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if ((Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'")))
                        && (string.IsNullOrEmpty(this.UtvrdenaImovinaOpis) || this.UtvrdenaImovinaInd != true))
                    {
                        XtraMessageBox.Show("Niste popunili podatke kod utvrđivanja imovine.", "Neispravni podaci", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    XtraMessageBox.Show("Niste popunili sve obavezne podatke.", "Neispravni podaci", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
        }

        public UgovoriDetail()
        {
            InitializeComponent();
        }

        #region Inits

        private void SetGrid()
        {
            try
            {
                vGridControl2.OptionsView.MinRowAutoHeight = 22;
                vGridControl2.RowHeaderWidth = 90;
                vGridControl2.Width = 700;

                RepositoryItem repVrstaTroska = SetRepositoryLookUpEdit(GetVrstaTroska(), "ugovorinajamvrstetroskovaid", "naziv");
                RepositoryItem repPartner = SetRepositorySearchEdit("partneriid", "nazivid");
                //RepositoryItem repPartner = SetRepositoryLookUpEdit(GetPartneri(), "partneriid", "nazivid");
                RepositoryItem repUtvrdenaImovina = SetRepositoryButtonEdit("Utvrđena imovina");
                RepositoryItem repIFRS16 = SetRepositoryCheckEdit("ifrs16");
                RepositoryItem repIdStavke = SetRepositoryTextEdit(false);
                RepositoryItem repProcijenjeniDatumPrestankaVazenjaUgovora = SetRepositoryDateEdit("procijenjenidatumprestankavazenjaugovora");
                RepositoryItem repOpis = SetRepositoryMemoEdit();
                RepositoryItem repSiteId = SetRepositorySearchLookUpEdit(GetDimenzije(), "dimenzijeid", "oznaka");
                RepositoryItem repNazivSiteIda = SetRepositoryTextEdit(false, true);
                RepositoryItem repProductCarRegistrationId = SetRepositorySearchLookUpEdit(GetProductCarRegistrationId(), "dimenzijeid", "oznaka");
                RepositoryItem repDatumPrimopredaje = SetRepositoryDateEdit("datumprimopredaje");
                RepositoryItem repPocetakIFRS16Kalkulacije = SetRepositoryDateEdit("pocetakifrs16kalkulacije");
                RepositoryItem repOpcija = SetRepositoryLookUpEdit(GetOpcija(0), "ugovorinajamopcijeid", "sifra");
                RepositoryItem repNazivOpcije = SetRepositoryTextEdit(false, true);

                RepositoryItem repUgovoreniIznos = SetRepositoryTextEdit(true);
                RepositoryItem repValuta = SetRepositoryLookUpEdit(GetValuta(), "valuteid", "naziv");
                RepositoryItem repTecaj = SetRepositoryLookUpEdit(GetValuta(), "tecajvaluteid", "naziv");
                RepositoryItem repVrstaTecaja = SetRepositoryLookUpEdit(GetVrsteTecaja(), "vrstatecajavalue", "vrstatecajastring");
                RepositoryItem repBanka = SetRepositoryLookUpEdit(GetBanke(), "partneriid", "bankanaziv");
                RepositoryItem repDatumPocetkaPlacanja = SetRepositoryDateEdit("datumpocetkaplacanja");
                RepositoryItem repNacinPlacanjaTip = SetRepositoryRadioGroup(new List<RadioGroupModel>()
                    { new RadioGroupModel() { Value = 1, Description = "Unaprijed" },  new RadioGroupModel() { Value = 2, Description = "Unazad" }});
                RepositoryItem repPeriodPlacanja = SetRepositoryLookUpEdit(GetPeriodiPlacanja(0), "ugovorinajamperiodiplacanjaid", "naziv");
                RepositoryItem repPrviDatumDospijeca = SetRepositoryDateEdit("prvidatumdospijeca");
                RepositoryItem repIndeksi = SetRepositoryLookUpEdit(GetIndeksi(), "ugovorinajamindeksiid", "naziv");
                RepositoryItem repDatumSlijedeceIndeksacije = SetRepositoryDateEdit("datumslijedeceindeksacije");
                RepositoryItem repVrijednostIndeksa = SetRepositorySpinEdit();
                RepositoryItem repDatumZadnjeIndeksacije = SetRepositoryDateEdit("datumzadnjeindeksacije");
                RepositoryItem repPlacanjeNalogom = SetRepositoryCheckEdit("placanjenalogomind");
                RepositoryItem repDatumDospijecaSlijedecegUgovora = SetRepositoryDateEdit("datumdospijecaslijedecegugovora");
                RepositoryItem repStatus = SetRepositoryCheckEdit("status", "Aktivan");
                RepositoryItem repDatumDeaktivacijeStavke = SetRepositoryDateEdit("datumdeaktivacijestavke");
                RepositoryItem repbrojobracunskogmjesta = SetRepositoryLookUpEdit(GetMjernaMjesta(0), "ugovorinajammjernamjestaid", "brojmjernogmjesta", new List<string>() { "adresa", "mjesto" });
                RepositoryItem repAdresaObracunskogMjesta = SetRepositoryTextEdit(false, true);
                RepositoryItem repSerijskiBrojBrojila = SetRepositoryTextEdit(false, true);
                RepositoryItem repNacinObracuna = SetRepositoryLookUpEdit(GetNacinObracuna(), "ugovorinajammjernamjestanacinobracunaid", "naziv");
                RepositoryItem repTipPlacanja = SetRepositoryLookUpEdit(GetTipPlacanja(), "ugovorinajammjernamjestatipplacanjaid", "naziv");

                RepositoryItem repProstorUnajmu = SetRepositoryTextEdit(true);
                RepositoryItem repRegOznaka = SetRepositoryTextEdit(false);
                RepositoryItem repSerijskiBrojUredaja = SetRepositoryTextEdit(false);
                RepositoryItem repIznosPenala = SetRepositoryTextEdit(true);
                RepositoryItem repPlaceniNajamPrijePocetkaVazenjaUgovora = SetRepositoryTextEdit(true);
                RepositoryItem repInicijalniDirektniTroskovi = SetRepositoryTextEdit(true);
                RepositoryItem repOrganizacijskaJedinica = SetRepositorySearchEdit("orgshemaid", "orgshema");

                CategoryRow cr = AddCategoryRow("Osnovni podaci");
                cr.ChildRows.Add(AddEditorRow("ugovorinajamvrstetroskovaid", "Vrsta troška" + requiredString(), repVrstaTroska));
                cr.ChildRows.Add(AddEditorRow("nazivid", "Partner" + requiredString(), repPartner));
                cr.ChildRows.Add(AddEditorRow("utvrdenaimovina", "", repUtvrdenaImovina));
                cr.ChildRows.Add(AddEditorRow("ifrs16", "IFRS 16", repIFRS16));
                cr.ChildRows.Add(AddEditorRow("procijenjenidatumprestankavazenjaugovora", "Proc. datum prestanka važenja najma" + requiredString(), repProcijenjeniDatumPrestankaVazenjaUgovora));
                cr.ChildRows.Add(AddEditorRow("idstavke", "ID Stavke", repIdStavke));
                cr.ChildRows.Add(AddEditorRow("opis", "Opis", repOpis));
                if ((!string.IsNullOrEmpty(GetPubVar("'UgovoriNajamVrste_VrstaTroskaVozila'")) && (Convert.ToInt32(this.GetGlavaPolja().Rows[0]["ugovorinajamvrsteid"]) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaVozila'"))))
                    || !string.IsNullOrEmpty(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraOstalaOprema'")) && (Convert.ToInt32(this.GetGlavaPolja().Rows[0]["ugovorinajamvrsteid"]) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraOstalaOprema'"))))
                { cr.ChildRows.Add(AddEditorRow("dimenzijeid", "Site ID", repSiteId)); }
                else
                { cr.ChildRows.Add(AddEditorRow("dimenzijeid", "Site ID" + requiredString(), repSiteId)); }
                cr.ChildRows.Add(AddEditorRow("siteidlabel", "Site ID/Lokacija", repNazivSiteIda));
                cr.ChildRows.Add(AddEditorRow("productcarregistrationid", "Product/CarRegistration", repProductCarRegistrationId));
                cr.ChildRows.Add(AddEditorRow("datumprimopredaje", "Datum primopredaje", repDatumPrimopredaje));
                cr.ChildRows.Add(AddEditorRow("pocetakifrs16kalkulacije", "Početak IFRS16 kalkulacije", repPocetakIFRS16Kalkulacije));

                cr.ChildRows.Add(AddEditorRow("ugovorinajamopcijeid", "Opcija", repOpcija));
                cr.ChildRows.Add(AddEditorRow("opcijalabel", "Naziv opcije", repNazivOpcije));

                cr = AddCategoryRow("Plaćanje");
                cr.ChildRows.Add(AddEditorRow("ugovoreniiznos", "Ugovoreni iznos", repUgovoreniIznos));
                cr.ChildRows.Add(AddEditorRow("devvaluteid", "Valuta", repValuta));
                cr.ChildRows.Add(AddEditorRow("datumpocetkaplacanja", "Datum prvog obračuna", repDatumPocetkaPlacanja));
                cr.ChildRows.Add(AddEditorRow("nacinplacanjatip", "Način obračuna" + requiredString(), repNacinPlacanjaTip));
                cr.ChildRows.Add(AddEditorRow("ugovorinajamperiodiplacanjaid", "Period plaćanja" + requiredString(), repPeriodPlacanja));
                cr.ChildRows.Add(AddEditorRow("prvidatumdospijeca", "Prvi datum dospijeća", repPrviDatumDospijeca));
                cr.ChildRows.Add(AddEditorRow("tecajid", "Valuta plaćanja", repTecaj));
                cr.ChildRows.Add(AddEditorRow("vrstatecaja", "Vrsta tečaja", repVrstaTecaja));
                cr.ChildRows.Add(AddEditorRow("bankeid", "Banka", repBanka));
                cr.ChildRows.Add(AddEditorRow("ugovorinajamindeksiid", "Indeksi", repIndeksi));
                cr.ChildRows.Add(AddEditorRow("datumsljedeceindeksacije", "Datum slijedeće indeksacije", repDatumSlijedeceIndeksacije));
                cr.ChildRows.Add(AddEditorRow("vrijednostindeksa", "Vrijednost indeksa" + requiredString(), repVrijednostIndeksa));
                cr.ChildRows.Add(AddEditorRow("datumzadnjeindeksacije", "Datum zadnje indeksacije", repDatumZadnjeIndeksacije));
                cr.ChildRows.Add(AddEditorRow("placanjenalogomind", "Plaćanje nalogom", repPlacanjeNalogom));
                cr.ChildRows.Add(AddEditorRow("datumdospijecasljedecegracuna", "Datum dospijeća slijedećeg računa", repDatumDospijecaSlijedecegUgovora));
                cr.ChildRows.Add(AddEditorRow("status", "Status", repStatus));
                cr.ChildRows.Add(AddEditorRow("datumdeaktivacijestavke", "Datum deaktivacije stavke", repDatumDeaktivacijeStavke));
                cr.ChildRows.Add(AddEditorRow("ugovorinajammjernamjestaid", "Broj obračunskog mjesta" + requiredString(), repbrojobracunskogmjesta));
                cr.ChildRows.Add(AddEditorRow("adresaobracunskogmjesta", "Adresa obračunskog mjesta", repAdresaObracunskogMjesta));
                cr.ChildRows.Add(AddEditorRow("serijskibrojbrojila", "Serijski broj brojila", repSerijskiBrojBrojila));
                cr.ChildRows.Add(AddEditorRow("ugovorinajammjernamjestanacinobracunaid", "Način obračuna" + requiredString(), repNacinObracuna));
                cr.ChildRows.Add(AddEditorRow("ugovorinajammjernamjestatipplacanjaid", "Tip plaćanja", repTipPlacanja));

                cr = AddCategoryRow("Ostali podaci");
                cr.ChildRows.Add(AddEditorRow("prostorunajmu", "Prostor u najmu (m2)", repProstorUnajmu));
                cr.ChildRows.Add(AddEditorRow("regoznaka", "Broj šasije", repRegOznaka));
                cr.ChildRows.Add(AddEditorRow("serbr", "Serijski broj uređaja", repSerijskiBrojUredaja));
                cr.ChildRows.Add(AddEditorRow("iznospenala", "Iznos penala", repIznosPenala));
                cr.ChildRows.Add(AddEditorRow("placnajamprijepocetka", "Plaćeni najam prije početka važenja ugovora", repPlaceniNajamPrijePocetkaVazenjaUgovora));
                cr.ChildRows.Add(AddEditorRow("inicijalnidirektnitroskovi", "Inicijalni direktni troškovi", repInicijalniDirektniTroskovi));
                cr.ChildRows.Add(AddEditorRow("orgshema", "Organizacijska jedinica", repOrganizacijskaJedinica));

                DT = GetDateSource();
                vGridControl2.DataSource = DT;
                if (DetId == 0)
                {
                    DataRow toInsert = DT.NewRow();
                    DT.Rows.InsertAt(toInsert, 0);
                }
                setInitProperties();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void setInitProperties()
        {
            this.VrstaUgovoraOznaka = this.GetGlavaPolja().Rows[0]["oznaka"].ToString();

            if (this.DetId != 0)
            {
                if (GetFieldValue("dimenzijeid") != DBNull.Value)
                {
                    var dimenzijeid = Convert.ToInt32(GetFieldValue("dimenzijeid"));

                    var kontrola = (vGridControl2.GetRowByFieldName("ugovorinajamopcijeid") as EditorRow);
                    var cmbOpcija = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                    cmbOpcija.DataSource = GetOpcija(dimenzijeid);

                    kontrola = (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestaid") as EditorRow);
                    var cmbMjernaMjesta = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                    cmbMjernaMjesta.DataSource = GetMjernaMjesta(dimenzijeid);
                }

                if (GetFieldValue("ugovorinajammjernamjestatipplacanjaid") != DBNull.Value)
                {
                    SetValue(new List<string>() { "ugovorinajammjernamjestatipplacanjaid" }, Convert.ToInt32(DT.Rows[0]["ugovorinajammjernamjestatipplacanjaid"]));
                }
                if (GetFieldValue("utvrdenaimovinaboolvalue") != DBNull.Value)
                {
                    this.UtvrdenaImovinaBoolValue = (bool)GetFieldValue("utvrdenaimovinaboolvalue");
                    this.UtvrdenaImovinaOpis = GetFieldValue("utvrdenaimovinarazlog").ToString();
                    if (!string.IsNullOrEmpty(UtvrdenaImovinaOpis))
                        this.UtvrdenaImovinaInd = true;
                }
                if (Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'")))
                {
                    SetValue(new List<string>() { "ifrs16" }, false);
                    EnableDisableRow(new List<string>() { "ifrs16" }, false);
                }
                if (GetFieldValue("ugovorinajamvrstetroskovaid") != DBNull.Value
                && (Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'"))))
                {
                    if (GetFieldValue("pocetakifrs16kalkulacije") == DBNull.Value && GetFieldValue("datumprimopredaje") != DBNull.Value)
                        SetValue(new List<string>() { "pocetakifrs16kalkulacije" }, Convert.ToDateTime(GetFieldValue("datumprimopredaje")));
                }
                else
                    EnableDisableRow(new List<string> { "pocetakifrs16kalkulacije" }, false);

                if (enableMjernaMjesta() == false)
                    EnableDisableRow(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, false);

                this.lastKnownPartneriText = GetFieldValue("nazivid").ToString();
                this.lastKnownPartneriId = Convert.ToInt32(GetFieldValue("partneriid"));
                this.lastKnownOrgShemaText = GetFieldValue("orgshema").ToString();
                this.lastKnownOrgShemaId = Convert.ToInt32(GetFieldValue("orgshemaid"));

                EnableDisableRow(new List<string> { "ugovorinajamvrstetroskovaid" }, false);
            }

            EnableDisableRow(new List<string>() { "idstavke", "datumzadnjeindeksacije", "datumdospijecasljedecegracuna", "opcijalabel",
                    "serijskibrojbrojila", "adresaobracunskogmjesta", "siteidlabel"}, false);

            if ((GetFieldValue("ugovorinajamvrstetroskovaid") != DBNull.Value
                && Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'"))))
                EnableDisableRow(new List<string>() { "utvrdenaimovina" }, true);
            else
                EnableDisableRow(new List<string>() { "utvrdenaimovina" }, false);

            if (this.VrstaUgovoraId != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraBazneStanice'"))
                && (this.VrstaUgovoraId != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraVodovi'"))))
            {
                EnableDisableRow(new List<string>() { "ugovorinajamopcijeid", "opcijalabel" }, false);
            }

            if (!GetPubVar("'UgovoriNajamVrste_VrstaTroskaProstor'").Split(',').ToArray().Contains(VrstaUgovoraId.ToString()))
            {   //ako je UgovoriVrste = Uredi, dućani ili poslovni prostori enableaj ovu kontrolu
                EnableDisableRow(new List<string>() { "prostorunajmu" }, false);
            }
            if (!GetPubVar("'UgovoriNajamVrste_VrstaTroskaVozila'").Split(',').ToArray().Contains(VrstaUgovoraId.ToString()))
            {   //ako je UgovoriVrste = vozila enableaj ovu kontrolu
                EnableDisableRow(new List<string>() { "regoznaka" }, false);
            }
            if (GetFieldValue("ugovorinajamvrstetroskovaid") != DBNull.Value
                && (Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'"))))
                EnableDisableRow(new List<string>() { "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi" }, false);

            EnableDisableRow(new List<string>() { "vrijednostindeksa" }, false);

            if (disableAllControls && this.DetId != 0)
            {
                DisableAllControls();
                return;
            }

            if (this.DetId == 0)
            {
                SetValue(new List<string>() { "vrijednostindeksa" }, 1);
                SetValue(new List<string>() { "nacinplacanjatip" }, 2);
                SetValue(new List<string>() { "datumpocetkaplacanja" }, glavaPoljaDR["datumpocetkavazenjaugovora"]);
                SetValue(new List<string>() { "ugovorinajamperiodiplacanjaid" }, DBNull.Value);
                SetValue(new List<string>() { "status" }, true);
                SetValue(new List<string>() { "ifrs16", "placanjenalogomind" }, false);
                SetValue(new List<string>() { "prvidatumdospijeca" }, DateTime.Today);
                EnableDisableRow(new List<string> { "pocetakifrs16kalkulacije" }, false);

                SetValue(new List<string>() { "orgshema" }, glavaPoljaDR["orgshema"], 3);
                SetValue(new List<string>() { "orgshemaid" }, glavaPoljaDR["orgshemaid"], 3);
                this.lastKnownOrgShemaText = GetFieldValue("orgshema").ToString();
                this.lastKnownOrgShemaId = Convert.ToInt32(GetFieldValue("orgshemaid"));

                if (GetPartnerIzZaglavlja() != null)
                {
                    SetValue(new List<string>() { "nazivid" }, GetPartnerIzZaglavlja()[0].ToString(), 1);
                    SetValue(new List<string>() { "partneriid" }, Convert.ToInt32(GetPartnerIzZaglavlja()[1]), 1);

                    this.lastKnownPartneriText = GetFieldValue("nazivid").ToString();
                    this.lastKnownPartneriId = Convert.ToInt32(GetFieldValue("partneriid"));
                }

                EnableDisableRow(new List<string>() { "ifrs16", "procijenjenidatumprestankavazenjaugovora" }, false);
                var kontrola = (vGridControl2.GetRowByFieldName("status") as EditorRow);
                var chkStatus = (kontrola.Properties.RowEdit as RepositoryItemCheckEdit);
                chkStatus.ReadOnly = true;  // ako je unos stavke, mora biti aktiviran status, zato read only

                EnableDisableRow(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, false);
            }

            if (GetFieldValue("status") != null && GetFieldValue("status") != DBNull.Value
                    && (bool)GetFieldValue("status") == true)
                EnableDisableRow(new List<string> { "datumdeaktivacijestavke" }, false);
            else
            {
                var datumDeaktivacijeStavke = GetFieldValue("datumdeaktivacijestavke");
                datumDeaktivacijeStavke = datumDeaktivacijeStavke == null || datumDeaktivacijeStavke == DBNull.Value ? DateTime.Today : datumDeaktivacijeStavke;
                SetValue(new List<string>() { "datumdeaktivacijestavke" }, datumDeaktivacijeStavke, 2);
            }

            handleMjernaMjestaPoljaRequired();  // required podaci o mjernom mjestu - samo ako su ugovori za struju i vrsta troska struja, u suprotnom nisu required
            handleDatumPrimopredajePoljeRequired();
            EnableLookupEditClearButtons();
            handleProcijenjeniDatumPrestankaVazenjaNajma(GetFieldValue("ifrs16"));
            handleTecaj_VrstaTecaja_Banke_Polja(GetFieldValue("devvaluteid"));
        }

        private void EnableLookupEditClearButtons()
        {
            foreach (RepositoryItem rep in vGridControl2.RepositoryItems)
            {
                if (rep is RepositoryItemLookUpEdit)
                {
                    var controlValue = DT.Rows[0][(rep as RepositoryItemLookUpEdit).Name];
                    if (controlValue != DBNull.Value && !string.IsNullOrEmpty(controlValue.ToString()))
                        EnableDisableClearButton((rep as RepositoryItemLookUpEdit), true, "combo");
                }
                else if (rep is RepositoryItemButtonEdit)
                {
                    var controlValue = DT.Rows[0][(rep as RepositoryItemButtonEdit).Name];
                    if (controlValue != DBNull.Value && !string.IsNullOrEmpty(controlValue.ToString()))
                        EnableDisableClearButton((rep as RepositoryItemButtonEdit), true, "search");
                }
            }
        }

        private void isprazniKontrole()
        {
            SetValue(new List<string>() { "idstavke", "opis", "regoznaka", "serbr", "siteidlabel", "opcijalabel" }, string.Empty);
            SetValue(new List<string>() { "ugovoreniiznos", "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi" }, 0);
            SetValue(new List<string>() { "utvrdenaimovinaboolvalue", "dimenzijeid", "ugovorinajamopcijeid", "devvaluteid",
            "ugovorinajamperiodiplacanjaid", "ugovorinajamindeksiid", "datumzadnjeindeksacije",
            "datumdospijecasljedecegracuna","datumdeaktivacijestavke", "ugovorinajammjernamjestaid", "ugovorinajammjernamjestatipplacanjaid", "prostorunajmu" }, DBNull.Value);
            SetValue(new List<string>() { "ifrs16", "placanjenalogomind" }, false);
            SetValue(new List<string>() { "datumprimopredaje", "pocetakifrs16kalkulacije", "datumsljedeceindeksacije", "prvidatumdospijeca" }, DateTime.Today);
            SetValue(new List<string>() { "vrijednostindeksa", "nacinplacanjatip" }, 1);
            SetValue(new List<string>() { "datumpocetkaplacanja" }, glavaPoljaDR["datumpocetkavazenjaugovora"]);
            SetValue(new List<string>() { "status" }, true);

            SetValue(new List<string>() { "orgshema" }, glavaPoljaDR["orgshema"], 3);
            SetValue(new List<string>() { "orgshemaid" }, glavaPoljaDR["orgshemaid"], 3);
            this.lastKnownOrgShemaText = GetFieldValue("orgshema").ToString();
            this.lastKnownOrgShemaId = Convert.ToInt32(GetFieldValue("orgshemaid"));
        }

        private void InitForm()
        {
        }

        private CategoryRow AddCategoryRow(string caption)
        {
            CategoryRow cr = new CategoryRow();
            cr.Properties.Caption = caption;

            this.vGridControl2.Rows.Add(cr);
            return cr;
        }

        private EditorRow AddEditorRow(string fieldName, string caption, RepositoryItem repositoryItem, string properties = "")
        {
            EditorRow er = new EditorRow();
            repositoryItem.Name = fieldName;
            er.Properties.FieldName = fieldName;
            er.Properties.Caption = caption;
            er.Properties.RowEdit = repositoryItem;
            return er;
        }

        private bool enableMjernaMjesta()
        {
            // polja za mjerna mjesta su ENABLED ako :
            //  - je vrsta ugovora bazne stanice,vodovi ili ugovori za struju IIIIII vrsta troska MORA biti odabrana i mora biti struja ili mrezarina

            if (this.VrstaUgovoraId != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraBazneStanice'"))
                && this.VrstaUgovoraId != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraVodovi'"))
                && this.VrstaUgovoraId != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraUgovoriZaStruju'")))
            { return false; }

            if (GetFieldValue("ugovorinajamvrstetroskovaid") == DBNull.Value
                || (GetFieldValue("ugovorinajamvrstetroskovaid") == null))
            { return false; }

            if (Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaStruja'"))
                && Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaPrikljucenje'"))
                && Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid")) != Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaMrezarina'")))
            { return false; }

            return true;
        }

        private void DisableAllControls()
        {
            EnableDisableRow(new List<string>()
            {"ugovorinajamvrstetroskovaid", "nazivid", "utvrdenaimovina", "ifrs16", "idstavke", "opis", "dimenzijeid", "siteidlabel", "productcarregistrationid",
            "productcarregistrationid", "datumprimopredaje", "pocetakifrs16kalkulacije", "ugovorinajamopcijeid", "opcijalabel",
            "ugovoreniiznos", "devvaluteid", "datumpocetkaplacanja", "nacinplacanjatip", "ugovorinajamperiodiplacanjaid", "prvidatumdospijeca", "tecajid",
            "vrstatecaja", "bankeid", "ugovorinajamindeksiid", "datumsljedeceindeksacije", "vrijednostindeksa", "datumzadnjeindeksacije", "placanjenalogomind",
            "datumdospijecasljedecegracuna", "status", "datumdeaktivacijestavke", "ugovorinajammjernamjestaid", "adresaobracunskogmjesta", "serijskibrojbrojila",
            "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid", "procijenjenidatumprestankavazenjaugovora",
            "prostorunajmu", "regoznaka", "serbr", "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi", "orgshema"}
            , false);
        }

        #endregion Inits

        #region Events

        private void EnableDisableClearButton(object rep, bool enabled, string controlType)
        {
            if (controlType == "search")
                (rep as RepositoryItemButtonEdit).Buttons[0].Enabled = enabled;
            else if (controlType == "combo")
                (rep as RepositoryItemLookUpEdit).Buttons[0].Enabled = enabled;
            else if (controlType == "searchCombo")
                (rep as RepositoryItemSearchLookUpEdit).Buttons[0].Enabled = enabled;
        }

        private void vGridControl2_RecordCellStyle(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        }

        #endregion Events

        #region SetRepository

        #region SearchLookupEditRepo

        private RepositoryItem SetRepositorySearchLookUpEdit(DataTable dataSource, string valueMember, string displayMember, List<string> columns = null, object initValue = null)
        {
            RepositoryItemSearchLookUpEdit rep = new RepositoryItemSearchLookUpEdit();
            rep.DataSource = dataSource;
            rep.PopulateViewColumns();
            rep.DisplayMember = displayMember;
            rep.ValueMember = valueMember;
            rep.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            rep.View.OptionsView.ShowColumnHeaders = false;

            rep.View.Columns["naziv"].Visible = false;
            foreach (GridColumn col in rep.View.Columns)
            {
                if (col.Name.ToLower().EndsWith("id"))
                    col.Visible = false;
            }

            rep.NullText = "...";
            rep.NullValuePromptShowForEmptyValue = true;
            rep.AppearanceDropDown.FontSizeDelta = 2;
            rep.EditValueChanged += (s, e) =>
            {
                EventSearchLookUpEditValueChanged(s, e, displayMember);
            };

            rep.Closed += (s, e) =>
            {
                EnableDisableClearButton(
                   (vGridControl2.RepositoryItems[(s as SearchLookUpEdit).Properties.Name] as RepositoryItemSearchLookUpEdit),
                   ((s as SearchLookUpEdit).EditValue != null && (s as SearchLookUpEdit).EditValue != DBNull.Value) ? true : false,
                   "searchCombo");
            };

            createUkloniLookupButton(rep as RepositoryItemSearchLookUpEdit, true);
            vGridControl2.RepositoryItems.Add(rep);

            return rep;
        }

        private void EventSearchLookUpEditValueChanged(object s, EventArgs e, string displayMember)
        {
            try
            {
                var valueMember = (s as DevExpress.XtraEditors.SearchLookUpEdit).Properties.ValueMember;
                // drugi dio uvjeta dodajem jer dimenzijeid valuemember imam i na productcarregistrationid kontroli
                if (valueMember == "dimenzijeid"
                    && (!string.IsNullOrEmpty((s as SearchLookUpEdit).Properties.Name)) && (s as SearchLookUpEdit).Properties.Name == "dimenzijeid")
                {
                    object dimenzijeid = (s as SearchLookUpEdit).EditValue;

                    if (dimenzijeid != null && dimenzijeid != DBNull.Value)
                    {
                        var newDimenzijeId = Convert.ToInt32(dimenzijeid);
                        var dimenzijeOpis = GetDimenzijeOpis(newDimenzijeId).Rows[0][0].ToString();

                        SetValue(new List<string>() { "opcijalabel", "adresaobracunskogmjesta", "serijskibrojbrojila" }, "");
                        SetValue(new List<string>() { "siteidlabel" }, dimenzijeOpis);
                        SetValue(new List<string>() { "ugovorinajamopcijeid" }, DBNull.Value);
                        var kontrola = (vGridControl2.GetRowByFieldName("ugovorinajamopcijeid") as EditorRow);
                        var cmbOpcija = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                        cmbOpcija.DataSource = GetOpcija(newDimenzijeId);

                        kontrola = (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestaid") as EditorRow);
                        var cmbMjernaMjesta = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                        SetValue(new List<string> { "ugovorinajammjernamjestaid" }, DBNull.Value);
                        cmbMjernaMjesta.DataSource = GetMjernaMjesta(newDimenzijeId);
                    }
                    else
                    {
                        var kontrola = (vGridControl2.GetRowByFieldName("ugovorinajamopcijeid") as EditorRow);
                        var cmbOpcija = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                        EnableDisableClearButton(cmbOpcija, false, "combo");
                        cmbOpcija.DataSource = null;
                        kontrola = (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestaid") as EditorRow);
                        var cmbMjernaMjesta = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
                        EnableDisableClearButton(cmbMjernaMjesta, false, "combo");
                        cmbMjernaMjesta.DataSource = null;

                        SetValue(new List<string>() { "opcijalabel", "adresaobracunskogmjesta", "serijskibrojbrojila" }, string.Empty);
                        SetValue(new List<string>() { "siteidlabel" }, string.Empty);
                        SetValue(new List<string> { "ugovorinajammjernamjestaid" }, DBNull.Value);
                    }
                }
            }
            catch (Exception)
            { }
        }

        #endregion SearchLookupEditRepo

        #region loookupEditRepo

        private RepositoryItem SetRepositoryLookUpEdit(DataTable dataSource, string valueMember, string displayMember, List<string> columns = null, object initValue = null)
        {
            RepositoryItemLookUpEdit rep = new RepositoryItemLookUpEdit();
            rep.DataSource = dataSource;
            rep.DisplayMember = displayMember;
            rep.ValueMember = valueMember;
            rep.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

            rep.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = displayMember });
            var popupWidth = 150;
            if (columns != null && columns.Count > 0)
            {
                popupWidth = columns.Count * 250;
                foreach (var col in columns)
                {
                    rep.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = col });
                }
            }

            rep.PopupFormMinSize = new System.Drawing.Size(popupWidth, 300);
            rep.ShowHeader = false;
            rep.NullText = "...";
            rep.NullValuePromptShowForEmptyValue = true;
            rep.AppearanceDropDown.FontSizeDelta = 2;
            rep.EditValueChanged += (s, e) =>
            {
                EventLookUpEditValueChanged(s, e, displayMember);
            };

            rep.Closed += (s, e) =>
            {
                EnableDisableClearButton(
                   (vGridControl2.RepositoryItems[(s as LookUpEdit).Properties.Name] as RepositoryItemLookUpEdit),
                   ((s as LookUpEdit).EditValue != null && (s as LookUpEdit).EditValue != DBNull.Value) ? true : false,
                   "combo");
            };

            createUkloniLookupButton(rep as RepositoryItemLookUpEdit);
            vGridControl2.RepositoryItems.Add(rep);

            return rep;
        }

        private void EventLookUpEditValueChanged(object s, EventArgs e, string displayMember)
        {
            try
            {
                var valueMember = (s as DevExpress.XtraEditors.LookUpEdit).Properties.ValueMember;

                if (valueMember == "ugovorinajamvrstetroskovaid")
                {
                    object vrsteTroskovaId = (s as LookUpEdit).EditValue;

                    if (vrsteTroskovaId != null && vrsteTroskovaId != DBNull.Value)
                    {
                        var tempVrsteTroskovaId = Convert.ToInt32(vrsteTroskovaId);

                        if ((tempVrsteTroskovaId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaStruja'"))
                                || tempVrsteTroskovaId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaPrikljucenje'"))
                                || tempVrsteTroskovaId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaMrezarina'")))
                            &&
                            (
                              (VrstaUgovoraId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraBazneStanice'")))
                              || (VrstaUgovoraId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraVodovi'")))
                              || (VrstaUgovoraId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraUgovoriZaStruju'")))
                            )
                           )
                        {
                            EnableDisableRow(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, true);
                        }
                        else
                        {
                            EnableDisableRow(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, false);
                            SetValue(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, DBNull.Value);
                            SetValue(new List<string>() { "adresaobracunskogmjesta", "serijskibrojbrojila" }, "", 2);
                        }

                        if (tempVrsteTroskovaId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'")))
                            EnableDisableRow(new List<string>() { "utvrdenaimovina", "ifrs16", "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi" }, true);
                        else
                        {
                            SetValue(new List<string>() { "utvrdenaimovinaboolvalue", "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi" }, DBNull.Value);
                            SetValue(new List<string>() { "ifrs16" }, false);
                            EnableDisableRow(new List<string>() { "utvrdenaimovina", "ifrs16", "iznospenala", "placnajamprijepocetka", "inicijalnidirektnitroskovi", "pocetakifrs16kalkulacije" }, false);
                        }
                    }
                    else
                    {
                        EnableDisableRow(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, false);
                        SetValue(new List<string>() { "ugovorinajammjernamjestaid", "ugovorinajammjernamjestanacinobracunaid", "ugovorinajammjernamjestatipplacanjaid" }, DBNull.Value);
                        //SetValue(new List<string>() { "utvrdenaimovinaboolvalue" }, DBNull.Value);
                        SetValue(new List<string>() { "ifrs16" }, false);
                        EnableDisableRow(new List<string>() { "utvrdenaimovina", "ifrs16" }, false);
                    }
                    handleDatumPrimopredajePoljeRequired(Convert.ToInt32(vrsteTroskovaId));
                    handleMjernaMjestaPoljaRequired(Convert.ToInt32(vrsteTroskovaId));  // required podaci o mjernom mjestu - samo ako su ugovori za struju, u suprotnom nisu required
                }
                else if (valueMember == "ugovorinajammjernamjestaid")
                {
                    object mjernomjestoid = (s as LookUpEdit).EditValue;

                    if (mjernomjestoid != null && mjernomjestoid != DBNull.Value)
                    {
                        SetValue(new List<string>() { "adresaobracunskogmjesta" }, GetAdresaObracunskogMjesta(Convert.ToInt32(mjernomjestoid)).Rows[0][0].ToString(), 2);
                        SetValue(new List<string>() { "serijskibrojbrojila" }, GetBrojBrojila(Convert.ToInt32(((s as DevExpress.XtraEditors.LookUpEdit).GetSelectedDataRow() as DataRowView)["ugovorinajammjernamjestaid"])), 2);
                    }
                    else
                    {
                        SetValue(new List<string>() { "adresaobracunskogmjesta" }, string.Empty, 2);
                        SetValue(new List<string>() { "serijskibrojbrojila" }, string.Empty, 2);
                    }
                }
                else if (valueMember == "ugovorinajamopcijeid")
                {
                    object opcijaid = (s as LookUpEdit).EditValue;

                    if (opcijaid != null && opcijaid != DBNull.Value)
                        SetValue(new List<string>() { "opcijalabel" }, GetOpcijaString(Convert.ToInt32(opcijaid)), 1);
                    else
                        SetValue(new List<string>() { "opcijalabel" }, string.Empty, 1);
                }
                else if (valueMember == "ugovorinajamperiodiplacanjaid")
                {
                    object periodPlacanjaid = (s as LookUpEdit).EditValue;

                    if (periodPlacanjaid != DBNull.Value && periodPlacanjaid != null && Convert.ToInt32(periodPlacanjaid) == Convert.ToInt32(GetPubVar("'UgovoriNajamPeriodPlacanja_Nepravilno'")))
                    {
                        SetValue(new List<string>() { "ugovoreniiznos", "prvidatumdospijeca", "datumdospijecasljedecegracuna" }, DBNull.Value, 2);
                        EnableDisableRow(new List<string>() { "ugovoreniiznos", "prvidatumdospijeca", "datumdospijecasljedecegracuna" }, false);
                    }
                    else
                        EnableDisableRow(new List<string>() { "ugovoreniiznos", "prvidatumdospijeca", "datumdospijecasljedecegracuna" }, true);

                    if (GetFieldValue("prvidatumdospijeca") != DBNull.Value &&
                        ((GetFieldValue("placanjenalogomind") != DBNull.Value) && (Convert.ToInt32(GetFieldValue("placanjenalogomind")) == 1)))
                    {
                        object periodiplacanjaid = (s as LookUpEdit).EditValue;

                        if (periodiplacanjaid != null && periodiplacanjaid != DBNull.Value)
                        {
                            var value = GetDatumDospijecaSlijedecegRacuna(Convert.ToDateTime(GetFieldValue("prvidatumdospijeca")), Convert.ToInt32(periodiplacanjaid));
                            SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, value, 2);
                        }
                        else
                            SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, DBNull.Value, 2);
                    }
                }
                else if (valueMember == "ugovorinajamindeksiid")
                {
                    if ((s as LookUpEdit).EditValue != null)
                    {
                        if (GetFieldValue("datumsljedeceindeksacije") == DBNull.Value || GetFieldValue("datumsljedeceindeksacije") == null)
                            SetValue(new List<string>() { "datumsljedeceindeksacije" }, DateTime.Today, 2);

                        var indeks = GetVrijednostIndeksa(Convert.ToInt32((s as LookUpEdit).EditValue),
                            Convert.ToDateTime(GetFieldValue("datumsljedeceindeksacije")));

                        if (indeks != null)
                        {
                            var vrijednostIndeksa = indeks["vrijednost"];

                            if (vrijednostIndeksa != null)
                                SetValue(new List<string>() { "vrijednostindeksa" }, Convert.ToDecimal(vrijednostIndeksa), 2);
                        }
                        else
                            SetValue(new List<string>() { "vrijednostindeksa" }, 1, 2);
                    }
                    else
                        SetValue(new List<string>() { "vrijednostindeksa" }, 1, 2);
                }
                else if (valueMember == "valuteid")
                {
                    object valutaid = (s as LookUpEdit).EditValue;
                    handleTecaj_VrstaTecaja_Banke_Polja(valutaid);
                }
            }
            catch (Exception)
            { }
        }

        private void handleMjernaMjestaPoljaRequired(int? vrstaTroskaid = null)
        {
            // ovo šaljem iz valuechanged eventa tog comboa jer drukčije ne mogu dobiti pravo stanje comboa (dobijam prethodno, nije završio update)
            int? vrstaTroskaCmb = null;

            // proslijedio sam combo value, usporedi s tom vrijednošću
            if (vrstaTroskaid != null)
                vrstaTroskaCmb = vrstaTroskaid;
            else
            {
                if (GetFieldValue("ugovorinajamvrstetroskovaid") != null && GetFieldValue("ugovorinajamvrstetroskovaid") != DBNull.Value)
                    vrstaTroskaCmb = Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid"));
            }

            if (vrstaTroskaCmb != null
                && (vrstaTroskaCmb == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaStruja'")) || vrstaTroskaCmb == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaMrezarina'")))
                && VrstaUgovoraId == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraUgovoriZaStruju'")))
            {
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestaid") as EditorRow).Properties.Caption = "Broj obračunskog mjesta" + requiredString();
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestatipplacanjaid") as EditorRow).Properties.Caption = "Tip plaćanja" + requiredString();
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestanacinobracunaid") as EditorRow).Properties.Caption = "Način obračuna" + requiredString();
            }
            else
            {
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestaid") as EditorRow).Properties.Caption = "Broj obračunskog mjesta";
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestatipplacanjaid") as EditorRow).Properties.Caption = "Tip plaćanja";
                (vGridControl2.GetRowByFieldName("ugovorinajammjernamjestanacinobracunaid") as EditorRow).Properties.Caption = "Način obračuna";
            }
        }

        private void handleDatumPrimopredajePoljeRequired(int? vrstaTroskaid = null)
        {
            // ovo šaljem iz valuechanged eventa tog comboa jer drukčije ne mogu dobiti pravo stanje comboa (dobijam prethodno, nije završio update)
            int? vrstaTroskaCmb = null;

            // proslijedio sam combo value, usporedi s tom vrijednošću
            if (vrstaTroskaid != null)
                vrstaTroskaCmb = vrstaTroskaid;
            else
            {
                if (GetFieldValue("ugovorinajamvrstetroskovaid") != null && GetFieldValue("ugovorinajamvrstetroskovaid") != DBNull.Value)
                    vrstaTroskaCmb = Convert.ToInt32(GetFieldValue("ugovorinajamvrstetroskovaid"));
            }

            if (vrstaTroskaCmb != null && vrstaTroskaCmb == Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaTroskaNajam'")))
                (vGridControl2.GetRowByFieldName("datumprimopredaje") as EditorRow).Properties.Caption = "Datum primopredaje" + requiredString();
            else
                (vGridControl2.GetRowByFieldName("datumprimopredaje") as EditorRow).Properties.Caption = "Datum primopredaje";
        }

        private void createUkloniLookupButton(RepositoryItem rep, bool searchLookupEdit = false)
        {
            DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
            clearButton.IsDefaultButton = false;
            clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            clearButton.ToolTip = "Ukloni";
            clearButton.Enabled = false;

            if (searchLookupEdit)
            {
                (rep as RepositoryItemSearchLookUpEdit).Buttons.Insert(0, clearButton);
                (rep as RepositoryItemSearchLookUpEdit).ActionButtonIndex = 1;
                (rep as RepositoryItemSearchLookUpEdit).ButtonClick += (s, e) =>
                {
                    SearchEditLookupEditButtonClick(s, e, rep, "searchCombo");
                };
            }
            else
            {
                (rep as RepositoryItemLookUpEdit).Buttons.Insert(0, clearButton);
                (rep as RepositoryItemLookUpEdit).ActionButtonIndex = 1;
                (rep as RepositoryItemLookUpEdit).ButtonClick += (s, e) =>
                {
                    SearchEditLookupEditButtonClick(s, e, rep, "combo");
                };
            }
        }

        #endregion loookupEditRepo

        #region searchEditRepo

        private RepositoryItem SetRepositorySearchEdit(string valueMember, string displayMember)
        {
            RepositoryItemButtonEdit rep = new RepositoryItemButtonEdit();
            rep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.Name = displayMember;
            vGridControl2.RepositoryItems.Add(rep);

            rep.KeyDown += (s, e) =>
            {
                OnKeyDown(s, e, valueMember, displayMember);
            };

            rep.Leave += (s, e) =>
            {
                OnLeave(s, e, valueMember, displayMember);
            };

            createUkloniSearchEditButton(rep as RepositoryItemButtonEdit, valueMember, displayMember);
            vGridControl2.RepositoryItems.Add(rep);

            return rep;
        }

        private void SetSearchEditValues(string valueMember, string displayMember, string textValue, object idValue, int rowIndex)
        {
            bool orgshema = displayMember.ToLower().Contains("orgshema");

            SetValue(new List<string>() { displayMember }, textValue, rowIndex);
            SetValue(new List<string>() { valueMember }, idValue == null ? DBNull.Value : idValue, rowIndex);

            if (orgshema)
                lastKnownOrgShemaText = textValue;
            else
                lastKnownPartneriText = textValue;

            if (idValue == DBNull.Value)
            {
                if (orgshema)
                    lastKnownOrgShemaId = null;
                else
                    lastKnownPartneriId = null;
            }
            else
            {
                if (orgshema)
                    lastKnownOrgShemaId = Convert.ToInt32(idValue);
                else
                    lastKnownPartneriId = Convert.ToInt32(idValue);
            }
        }

        private void OnLeave(object s, EventArgs e, string valueMember, string displayMember)
        {
            try
            {
                bool orgshema = valueMember.ToLower().Contains("orgshema");

                // nije došao iz metode tražilice (tab ili clear)
                if ((s as ButtonEdit).Tag.ToString() == "")
                {
                    // ako je prije bio prazan string - (samo prelazim preko kontrole (tab))
                    if (GetFieldValue(valueMember) == DBNull.Value && !string.IsNullOrEmpty((s as ButtonEdit).Text.ToString()))
                    {
                        FilterDTandOpenSearchForm(s, valueMember, displayMember);
                        OnLeave(s, e, valueMember, displayMember);
                        //return;
                    }
                    // ako je prije bila postavljena vrijednost
                    else
                    {
                        // clear
                        if ((s as ButtonEdit).EditValue.ToString() == "")
                            SetSearchEditValues(valueMember, displayMember, string.Empty, DBNull.Value, orgshema ? 3 : 1); // - dinamički index
                        // tab
                        else
                            SetSearchEditValues(valueMember, displayMember,
                                orgshema ? lastKnownOrgShemaText : lastKnownPartneriText,
                                orgshema ? lastKnownOrgShemaId : lastKnownPartneriId,
                                orgshema ? 3 : 1); // - dinamički index
                    }
                }
                // bio je u tražilici
                else if ((s as ButtonEdit).Tag.ToString() != "")
                {
                    //nije našao Id u tražilici
                    if ((s as ButtonEdit).Tag.ToString().ToLower() == "nosearchid")
                    {
                        // ako je prije bio prazan string
                        if (GetFieldValue(valueMember) == DBNull.Value)
                            SetSearchEditValues(valueMember, displayMember, string.Empty, DBNull.Value, orgshema ? 3 : 1); // - dinamički index
                                                                                                                           // ako je prije bila postavljena vrijednost
                        else
                            SetSearchEditValues(valueMember, displayMember,
                                orgshema ? lastKnownOrgShemaText : lastKnownPartneriText,
                                orgshema ? lastKnownOrgShemaId : lastKnownPartneriId,
                                orgshema ? 3 : 1); // - dinamički index
                    }
                    //našao je Id u tražilici -> samo odi dalje
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("OnLeave(): " + ex.Message, "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                EnableDisableClearButton(
                    (vGridControl2.RepositoryItems[(s as ButtonEdit).Properties.Name] as RepositoryItemButtonEdit),
                    string.IsNullOrEmpty(GetFieldValue(displayMember).ToString()) ? false : true, "search");

                if (string.IsNullOrEmpty(GetFieldValue(displayMember).ToString()))
                    vGridControl2.EditingValue = "";
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e, string idName, string stringName)
        {
            if (e.KeyData == Keys.Enter && (sender as TextEdit).Text.Length > 0)
            {
                FilterDTandOpenSearchForm(sender, idName, stringName);
            }
        }

        //private void EventButtonEditSearchButtonClick(object sender, ButtonPressedEventArgs e, string idName, string stringName)
        //{
        //    FilterDTandOpenSearchForm(sender, idName, stringName);
        //}

        private void FilterDTandOpenSearchForm(object sender, string idName, string stringName)
        {
            try
            {
                if (vGridControl2.FocusedRow.Name.ToLower() == "row" + stringName)
                {
                    if (!string.IsNullOrEmpty((sender as ButtonEdit).Text))
                    {
                        bool orgshema = stringName.ToLower().Contains("orgshema");

                        var tblFiltered = orgshema ? TraziOrgShemu((sender as ButtonEdit).Text) : TraziPartneri((sender as ButtonEdit).Text);

                        Forms.Search searchForm = new Forms.Search(tblFiltered, "", idName);
                        searchForm.ShowDialog();

                        if (searchForm.DialogResult == DialogResult.OK && searchForm.SearchId != null)
                        {
                            // setiraj textualnu vrijednost kada nađem partnera/orgshemu po zatvaranju search forme
                            (sender as ButtonEdit).EditValue = tblFiltered.AsEnumerable()
                                .Where(row => row.Field<Int32>(idName) == searchForm.SearchId).FirstOrDefault()[stringName].ToString();
                            // setiraj id partnera kada nađem partnera/orgshemu po zatvaranju search forme
                            SetValue(new List<string>() { idName }, searchForm.SearchId, orgshema ? 3 : 1); // -- dinamički row index
                            // tag - indikator da li je našao partnera/orgshemu
                            (sender as ButtonEdit).Tag = (sender as ButtonEdit).EditValue.ToString();

                            if (idName.ToLower() == "partneriid")
                            {
                                this.lastKnownPartneriId = Convert.ToInt32(searchForm.SearchId);
                                this.lastKnownPartneriText = (sender as ButtonEdit).EditValue.ToString();
                            }
                            else if (idName.ToLower().Contains("orgshema"))
                            {
                                this.lastKnownOrgShemaId = Convert.ToInt32(searchForm.SearchId);
                                this.lastKnownOrgShemaText = (sender as ButtonEdit).EditValue.ToString();
                            }
                        }
                        else
                        {
                            //SetValue(new List<string>() { idName }, DBNull.Value, 1);
                            (sender as ButtonEdit).Tag = "noSearchId";
                        }
                    }
                    else
                        XtraMessageBox.Show("Niste unijeli pojam pretraživanja.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch { }
        }

        private void createUkloniSearchEditButton(RepositoryItem rep, string idName, string stringName)
        {
            (rep as RepositoryItemButtonEdit).Buttons[0].Kind = ButtonPredefines.Search;
            (rep as RepositoryItemButtonEdit).Buttons[0].IsDefaultButton = true;

            DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
            clearButton.IsDefaultButton = false;
            clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            clearButton.ToolTip = "Ukloni";
            clearButton.Enabled = false;

            (rep as RepositoryItemButtonEdit).Buttons.Insert(0, clearButton);
            (rep as RepositoryItemButtonEdit).ButtonClick += (s, e) =>
            {
                SearchEditLookupEditButtonClick(s, e, rep, "search", idName, stringName);
            };

            (rep as RepositoryItemButtonEdit).Buttons[1].IsDefaultButton = true;
        }

        private void SearchEditLookupEditButtonClick(object sender, ButtonPressedEventArgs e, RepositoryItem rep, string controlType,
           string idName = "", string stringName = "")
        {
            if (e.Button.Kind == ButtonPredefines.Search)
            {
                FilterDTandOpenSearchForm(sender, idName, stringName);
            }
            else if (e.Button.Kind == ButtonPredefines.Delete)
            {
                vGridControl2.EditingValue = controlType == "search" ? string.Empty : null; // ako sam poslao idName onda je search, moram u null
                EnableDisableClearButton(rep, false, controlType);
            }
        }

        #endregion searchEditRepo

        #region dateEditRepo

        private RepositoryItem SetRepositoryDateEdit(string name)
        {
            RepositoryItemDateEdit rep = new RepositoryItemDateEdit();
            rep.EditValueChanged += (s, e) =>
            {
                EventDateEditValueChanged(s, e);
            };
            rep.Name = name;
            vGridControl2.RepositoryItems.Add(rep);
            return rep;
        }

        private void EventDateEditValueChanged(object s, EventArgs e, bool ind = false)
        {
            var kontrola = (s as DateEdit);
            if (kontrola.Properties.Name == "prvidatumdospijeca")
            {
                if (kontrola.EditValue != null &&
                    ((GetFieldValue("placanjenalogomind") != DBNull.Value) && (Convert.ToInt32(GetFieldValue("placanjenalogomind")) == 1)))
                {
                    var periodiplacanjaid = Convert.ToInt32(GetFieldValue("ugovorinajamperiodiplacanjaid"));
                    var value = GetDatumDospijecaSlijedecegRacuna(Convert.ToDateTime((s as DateEdit).EditValue), periodiplacanjaid);
                    SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, value, 2);
                }
                else if (kontrola.EditValue == null)
                {
                    SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, DBNull.Value, 2);
                }
            }
            else if (kontrola.Properties.Name == "datumprimopredaje")
            {
                if ((kontrola as DateEdit).EditValue == null)
                    SetValue(new List<string>() { "pocetakifrs16kalkulacije" }, DBNull.Value, 0);
                else
                    SetValue(new List<string>() { "pocetakifrs16kalkulacije" }, Convert.ToDateTime((kontrola as DateEdit).EditValue), 0);
            }
            else if (kontrola.Properties.Name == "datumsljedeceindeksacije")
            {
                if ((s as DateEdit).EditValue != null)
                {
                    if (GetFieldValue("ugovorinajamindeksiid") != DBNull.Value)
                    {
                        var indeks = GetVrijednostIndeksa(Convert.ToInt32(GetFieldValue("ugovorinajamindeksiid")),
                            Convert.ToDateTime((s as DateEdit).EditValue));

                        if (indeks != null)
                        {
                            var vrijednostIndeksa = indeks["vrijednost"];
                            //var indeksiid = indeks["ugovorinajamindeksiid"];

                            if (vrijednostIndeksa != null)
                                SetValue(new List<string>() { "vrijednostindeksa" }, Convert.ToDecimal(vrijednostIndeksa), 2);

                            //if (indeksiid != null)
                            //    SetValue(new List<string>() { "ugovorinajamindeksiid" }, Convert.ToInt32(indeksiid), 2);
                        }
                        else
                        {
                            SetValue(new List<string>() { "vrijednostindeksa" }, 1, 2);
                            //SetValue(new List<string>() { "ugovorinajamindeksiid" }, DBNull.Value, 2);
                        }
                    }
                    else
                    {
                        SetValue(new List<string>() { "vrijednostindeksa" }, 1, 2);
                        //SetValue(new List<string>() { "ugovorinajamindeksiid" }, DBNull.Value, 2);
                    }
                }
                else
                {
                    SetValue(new List<string>() { "vrijednostindeksa" }, 1, 2);
                    //SetValue(new List<string>() { "ugovorinajamindeksiid" }, DBNull.Value, 2);
                }
            }
        }

        #endregion dateEditRepo

        #region checkEditRepo

        private RepositoryItem SetRepositoryCheckEdit(string name, string caption = "")
        {
            RepositoryItemCheckEdit rep = new RepositoryItemCheckEdit();
            rep.GlyphAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.Caption = caption;
            //rep.Name = name;
            rep.DisplayValueUnchecked = "...";
            vGridControl2.RepositoryItems.Add(rep);
            rep.CheckedChanged += (s, e) =>
            {
                EventCheckEditCheckedChanged(s, e);
            };
            return rep;
        }

        private void EventCheckEditCheckedChanged(object s, EventArgs e)
        {
            if (vGridControl2.FocusedRow.Name.ToLower() == "rowplacanjenalogomind")
            {
                if ((bool)(s as CheckEdit).EditValue == true)
                {
                    if (GetFieldValue("prvidatumdospijeca") != DBNull.Value && (GetFieldValue("ugovorinajamperiodiplacanjaid") != DBNull.Value))
                    {
                        var periodiplacanjaid = Convert.ToInt32(GetFieldValue("ugovorinajamperiodiplacanjaid"));
                        var value = GetDatumDospijecaSlijedecegRacuna(Convert.ToDateTime(GetFieldValue("prvidatumdospijeca")), periodiplacanjaid);
                        SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, value, 1);
                    }
                }
                else
                    SetValue(new List<string>() { "datumdospijecasljedecegracuna" }, DBNull.Value, 1);
            }
            else if (vGridControl2.FocusedRow.Name.ToLower() == "rowstatus")
            {
                if ((s as CheckEdit).EditValue != null && GetFieldValue("status") != DBNull.Value
                    && (bool)(s as CheckEdit).EditValue == true)
                {
                    SetValue(new List<string>() { "datumdeaktivacijestavke" }, DBNull.Value, 2);
                    EnableDisableRow(new List<string> { "datumdeaktivacijestavke" }, false);
                }
                else
                {
                    EnableDisableRow(new List<string> { "datumdeaktivacijestavke" }, true);
                    SetValue(new List<string>() { "datumdeaktivacijestavke" }, DateTime.Today, 2);
                }
            }
            else if (vGridControl2.FocusedRow.Name.ToLower() == "rowifrs16")
                handleProcijenjeniDatumPrestankaVazenjaNajma((s as CheckEdit).EditValue);
        }

        #endregion checkEditRepo

        #region buttonEditRepo

        private RepositoryItem SetRepositoryButtonEdit(string caption)
        {
            RepositoryItemButtonEdit rep = new RepositoryItemButtonEdit();
            rep.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
            rep.Buttons[0].Caption = caption;
            rep.Buttons[0].Appearance.Font = new System.Drawing.Font(rep.Buttons[0].Appearance.Font, System.Drawing.FontStyle.Bold);
            rep.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;

            rep.Click += (s, e) =>
            {
                EventButtonEditClick(s, e);
            };
            return rep;
        }

        private void EventButtonEditClick(object s, EventArgs e)
        {
            UtvrdenaImovinaPopUpForm popup = new UtvrdenaImovinaPopUpForm();
            popup.UtvrdenaImovinaInd = this.UtvrdenaImovinaInd;
            popup.UtvrdenaImovinaOpis = this.UtvrdenaImovinaOpis;
            if (this.DetId != 0 || (imovinaParamsSet != null && (bool)imovinaParamsSet))
                popup.izmjenaInd = true;

            if (popup.izmjenaInd != null && (bool)popup.izmjenaInd)
                popup.UtvrdenaImovinaBoolValue = this.UtvrdenaImovinaBoolValue;

            popup.ShowDialog();
            if (popup.DialogResult == DialogResult.OK)
            {
                this.UtvrdenaImovinaInd = popup.UtvrdenaImovinaInd;
                this.UtvrdenaImovinaBoolValue = popup.UtvrdenaImovinaBoolValue;
                this.UtvrdenaImovinaOpis = popup.UtvrdenaImovinaOpis;
                popup.izmjenaInd = true;
                imovinaParamsSet = true;
            }
        }

        #endregion buttonEditRepo

        private RepositoryItem SetRepositorySpinEdit()
        {
            RepositoryItemSpinEdit rep = new RepositoryItemSpinEdit();
            rep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            vGridControl2.RepositoryItems.Add(rep);
            rep.EditValueChanged += (s, e) =>
            {
                //EventSpinEditEditValueChanged(s, e);
            };
            return rep;
        }

        private RepositoryItem SetRepositoryTextEdit(bool moneyFormat, bool labelEdit = false)
        {
            RepositoryItemTextEdit rep = new RepositoryItemTextEdit();
            rep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            vGridControl2.RepositoryItems.Add(rep);

            if (moneyFormat)
            {
                rep.Mask.EditMask = "n2";
                rep.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                rep.Mask.UseMaskAsDisplayFormat = true;
            }

            if (labelEdit)
                rep.ReadOnly = true;

            return rep;
        }

        private RepositoryItem SetRepositoryMemoEdit()
        {
            RepositoryItemTextEdit rep = new RepositoryItemMemoEdit();
            vGridControl2.RepositoryItems.Add(rep);
            return rep;
        }

        private RepositoryItem SetRepositoryRadioGroup(List<RadioGroupModel> radioButtons)
        {
            RepositoryItemRadioGroup rep = new RepositoryItemRadioGroup();
            foreach (RadioGroupModel radioButton in radioButtons)
            {
                rep.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
                    new DevExpress.XtraEditors.Controls.RadioGroupItem(radioButton.Value, radioButton.Description)
                });
            }

            vGridControl2.RepositoryItems.Add(rep);
            return rep;
        }

        #endregion SetRepository

        #region DataSources

        private DataTable GetGlavaPolja()
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetGlavaPolja " + GlaId.ToString();
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetGlavaPolja " + GlaId.ToString(), "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetDateSource()
        {
            try
            {
                string sql = "exec spugovorinajamdet_PuniDetaljFormu " + DetId.ToString();
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spugovorinajamdet_PuniDetaljFormu " + DetId.ToString(), "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetVrstaTroska()
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetVrstaTroska";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetVrstaTroska", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable TraziPartneri(string searchString)
        {
            try
            {
                string sql = "exec spTraziPartneri @Action='Partneri', @broj_lokacija=1, @Uvjet='" + searchString + "'";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spTraziPartneri @Action='Partneri'", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable TraziOrgShemu(string searchString)
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_GetOrgShema @searchstring='" + searchString + "'" + ", @operateriid=" + Operateriid.ToString();
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetOrgShema @searchstring='" + searchString + "'", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetPartneri()
        {
            try
            {
                string sql = "exec spTraziPartneri @Action='Partneri'";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spTraziPartneri @Action='Partneri'", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetDimenzije()
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_Dimenzije_Query @action=1, @DimVrsteId=4";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_Dimenzije_Query @action=1, @DimVrsteId=4", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetProductCarRegistrationId()
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_Dimenzije_Query @action=1, @DimVrsteId=6";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_Dimenzije_Query @action=1, @DimVrsteId=6", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetOpcija(int dimenzijeid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetOpcije @dimenzijeid=" + dimenzijeid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetOpcije @dimenzijeid=" + dimenzijeid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetVrsteTecaja()
        {
            try
            {
                string sql = "select 'Kupovni' as vrstatecajavalue, 'Kupovni' as vrstatecajastring union all select 'Prodajni' as vrstatecajavalue, 'Prodajni' as vrstatecajastring union all select 'Srednji' as vrstatecajavalue, 'Srednji' as vrstatecajastring";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: vrstetecaja", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetBanke()
        {
            try
            {
                string sql = "exec spUgovoriNajam_getBanke";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajam_getBanke", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetValuta()
        {
            try
            {
                string sql = "exec spUgovoriNajamTipOpcija_getValute";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spNG_Combo @Action=valute, @Ind=1, @Oznaka=OznakaNaziv", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetPeriodiPlacanja(int dimenzijeid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetPeriodPlacanja @dimenzijeid=" + dimenzijeid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetPeriodPlacanja @dimenzijeid=" + dimenzijeid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetIndeksi()
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetIndeksi";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetIndeksi", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetAdresaObracunskogMjesta(int mjernomjestoid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetAdresaObracunskogMjesta " + mjernomjestoid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetAdresaObracunskogMjesta " + mjernomjestoid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetMjernaMjesta(int dimenzijeid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetMjernaMjesta " + dimenzijeid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetMjernaMjesta " + dimenzijeid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        //private DataTable GetNajamId()
        //{
        //    try
        //    {
        //        string sql = "select VarSet as IdNajma from pubvars where VarIme='UgovoriNajamVrste_VrstaTroskaNajam'";
        //        DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
        //        return FormatDataColumns(dt);
        //    }
        //    catch (Exception)
        //    {
        //         XtraMessageBox.Show("Greška dohvata podataka: select VarSet as IdNajma from pubvars where VarIme='UgovoriNajamVrste_VrstaTroskaNajam'", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return null;
        //    }
        //}

        private DataTable GetNacinObracuna()
        {
            try
            {
                string sql = "select 'Kn' as Naziv, 1 as UgovoriNajamMjernaMjestaNacinObracunaId union all select 'kWh' as Naziv, 0 as UgovoriNajamMjernaMjestaNacinObracunaId";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select 'Kn' as Naziv, 1 as UgovoriNajamMjernaMjestaNacinObracunaId union all select 'kWh' as Naziv, 0 as UgovoriNajamMjernaMjestaNacinObracunaId", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetTipPlacanja()
        {
            try
            {
                string sql = "select 'Uključeno u najam' as Naziv, 0 as ugovorinajammjernamjestatipplacanjaid  union all select 'Odvojeni trošak' as Naziv, 1 as ugovorinajammjernamjestatipplacanjaid";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select 'Uključeno u najam' as Naziv, 0 as ugovorinajammjernamjestatipplacanjaid  union all select 'Odvojeni trošak' as Naziv, 1 as ugovorinajammjernamjestatipplacanjaid", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetOrgShema()
        {
            try
            {
                //TODO: maknuti searchstring i riješiti dinamički operateriid
                string sql = "exec spUgovoriNajamGla_GetOrgShema @searchstring='%', @operateriid=" + Operateriid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetOrgShema @searchstring='%', @operateriid=" + Operateriid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable FormatDataColumns(DataTable dt)
        {
            try
            {
                foreach (DataColumn col in dt.Columns)
                {
                    col.ColumnName = col.ColumnName.ToLower();
                }
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FormatDataColumns(), " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetDimenzijeOpis(int dimenzijeid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetDimenzijaOpis " + dimenzijeid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetDimenzijaOpis " + dimenzijeid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private string GetBrojBrojila(int ugovorinajammjernamjestaid)
        {
            try
            {
                string sql = "exec spUgovoriNajamDet_GetBrojBrojila " + ugovorinajammjernamjestaid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
                else
                    return "";
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamDet_GetBrojBrojila " + ugovorinajammjernamjestaid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }
        }

        private string GetOpcijaString(int ugovorinajamopcijeid)
        {
            try
            {
                string sql = "select naziv from UgovoriNajamOpcije where UgovoriNajamOpcijeId = " + ugovorinajamopcijeid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
                else
                    return "";
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select naziv from UgovoriNajamOpcije where UgovoriNajamOpcijeId = " + ugovorinajamopcijeid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }
        }

        private object GetDatumDospijecaSlijedecegRacuna(DateTime prvidatumdospijeca, int ugovorinajamperiodiplacanjaid)
        {
            try
            {
                string sql = "select BrojMjeseci from UgovoriNajamPeriodiPlacanja where UgovoriNajamPeriodiPlacanjaID = " + ugovorinajamperiodiplacanjaid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return prvidatumdospijeca.AddMonths(Convert.ToInt32(dt.Rows[0][0]));
                else
                    return prvidatumdospijeca;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select BrojMjeseci from UgovoriNajamPeriodiPlacanja where UgovoriNajamPeriodiPlacanjaID = " + ugovorinajamperiodiplacanjaid, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private string GetPubVar(string varIme)
        {
            try
            {
                string sql = "select varset from pubvars where varime = " + varIme;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
                else
                    return "";
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select varset from pubvars where varime = " + varIme, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }
        }

        private DataRow GetPartnerIzZaglavlja()
        {
            try
            {
                string sql = "select top 1 p.Naziv, ln.partneriid from UgovoriNajamGla_LnPartneri ln left join Partneri p on p.PartneriId = ln.PartneriId where UgovoriNajamGlaId = " + this.GlaId;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                else
                    return null;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: select top 1 partneriid from UgovoriNajamGla_LnPartneri where UgovoriNajamGlaId = " + this.GlaId, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataRow GetVrijednostIndeksa(int ugovorinajamindeksiid, DateTime datumSljedeceIndeksacije)
        {
            try
            {
                var datumSljedeceIndeksacijeSQL = datumSljedeceIndeksacije.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sql = "exec spUgovoriNajamDet_GetVrijednostIndeksa @ugovorinajamindeksiid=" + ugovorinajamindeksiid + ", @DatumSljedeceIndeksacije='" + datumSljedeceIndeksacijeSQL + "'";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                else
                    return null;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Greška dohvata podataka: spUgovoriNajamDet_GetVrijednostIndeksa", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        #endregion DataSources

        private void EnableDisableRow(List<string> fields, bool enabled)
        {
            foreach (var field in fields)
            {
                (vGridControl2.GetRowByFieldName(field) as EditorRow).Enabled = enabled;
            }
        }

        private void SetValue(List<string> fields, object value, int? rowIndex = null)
        {
            // int rowIndex - služi kada ne želi refreshati value kontrole na editvaluechanged,
            // s njime forsiram refresh value. Šaljem index kategorije u kojoj je taj repo
            foreach (var field in fields)
            {
                DT.Rows[0][field] = value;
                if (rowIndex != null)
                {
                    var index = rowIndex;
                    RepositoryItem repoItem = vGridControl2.RepositoryItems[field];
                    vGridControl2.SetCellValue(vGridControl2.Rows[Convert.ToInt32(index)], 0, value);
                }
            }
        }

        private object GetFieldValue(string fieldName)
        {
            if (DT != null && DT.Rows[0][fieldName] != null)
                return DT.Rows[0][fieldName];
            else
                return null;
        }

        #region helperMethods

        private bool validateRequiredFields()
        {
            foreach (CategoryRow cr in this.vGridControl2.Rows)
            {
                foreach (EditorRow row in cr.ChildRows)
                {
                    if (row.Properties.Caption.Contains(requiredString())
                        && (vGridControl2.GetRowByFieldName(row.Properties.FieldName) as EditorRow).Enabled == true)
                    {
                        if (GetFieldValue(row.Properties.FieldName) == null
                            || GetFieldValue(row.Properties.FieldName) == DBNull.Value
                            || string.IsNullOrEmpty(GetFieldValue(row.Properties.FieldName).ToString()))
                        {
                            XtraMessageBox.Show("Niste popunili sva obavezna polja.", "Validacija podataka", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            // focus obavezno polje
                            vGridControl2.FocusedRow = (vGridControl2.GetRowByFieldName(row.Properties.FieldName) as EditorRow);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void handleTecaj_VrstaTecaja_Banke_Polja(object valutaid)
        {
            if (valutaid != null && valutaid != DBNull.Value)
            {
                EnableDisableRow(new List<string>() { "tecajid", "vrstatecaja", "bankeid" }, true);
                (vGridControl2.GetRowByFieldName("vrstatecaja") as EditorRow).Properties.Caption = "Vrsta tečaja" + requiredString();
                (vGridControl2.GetRowByFieldName("bankeid") as EditorRow).Properties.Caption = "Banka" + requiredString();
            }
            else
            {
                SetValue(new List<string>() { "tecajid", "vrstatecaja", "bankeid" }, DBNull.Value, 1);
                (vGridControl2.GetRowByFieldName("vrstatecaja") as EditorRow).Properties.Caption = "Vrsta tečaja";
                (vGridControl2.GetRowByFieldName("bankeid") as EditorRow).Properties.Caption = "Banka";
                EnableDisableRow(new List<string>() { "tecajid", "vrstatecaja", "bankeid" }, false);
            }
        }

        private void handleProcijenjeniDatumPrestankaVazenjaNajma(object chkIfrs16)
        {
            if (chkIfrs16 != null && chkIfrs16 != DBNull.Value && Convert.ToBoolean(chkIfrs16) == true)
            {
                EnableDisableRow(new List<string> { "pocetakifrs16kalkulacije", "procijenjenidatumprestankavazenjaugovora" }, true);

                // ako je unos stavke želim samo popuniti procijenjenidatumprestankavazenjaugovora s glave
                if (this.DetId == 0)
                {
                    SetValue(new List<string> { "procijenjenidatumprestankavazenjaugovora" }, this.ProcDatumPrestankaVazenjaNajma, 0);
                    return;
                }

                // uvjet dodan da ne pregazim vrijednost kod inicijalnog učitavanja
                if (GetFieldValue("procijenjenidatumprestankavazenjaugovora") == null || GetFieldValue("procijenjenidatumprestankavazenjaugovora") == DBNull.Value)
                    SetValue(new List<string> { "procijenjenidatumprestankavazenjaugovora" }, this.ProcDatumPrestankaVazenjaNajma, 0);
            }
            else
            {
                SetValue(new List<string> { "procijenjenidatumprestankavazenjaugovora" }, DBNull.Value, 0);
                EnableDisableRow(new List<string> { "pocetakifrs16kalkulacije", "procijenjenidatumprestankavazenjaugovora" }, false);
            }
        }

        #endregion helperMethods
    }
}