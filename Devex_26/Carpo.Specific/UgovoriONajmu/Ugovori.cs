using Carpo.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.UgovoriONajmu
{
    public partial class Ugovori : XtraUserControl, IDisposable
    {
        public string LnPartneriIdArrayString { get; set; }
        public int Operateriid { get; set; }
        private int bazneStaniceId;
        private int vodoviId;
        private int automatskoProduljenjeId;
        private int? lastKnownOrgShemaId { get; set; }
        private string lastKnownOrgShemaText { get; set; }
        private int id;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                SetGrid();
                InitForm();
            }
        }

        public DataTable DT { get; set; }

        private string requiredString()
        {
            return " <color=200,0,0>*</color>";
        }

        public bool IsValid
        {
            get
            {
                if (DT == null)
                    return false;

                try
                {
                    if (validateRequiredFields() == false)
                        return false;

                    if (string.IsNullOrEmpty(LnPartneriIdArrayString))
                    {
                        XtraMessageBox.Show("Nije odabran niti jedan partner.", "Neispravni podatci", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    XtraMessageBox.Show("Došlo je do greške pri validaciji podataka.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
        }

        public Ugovori()
        {
            InitializeComponent();
            //Connection.ConnectionString = Constructor.SetConnectionString("10.156.197.94", "PPTele2", "", "");
        }

        #region Inits

        private void SetGrid()
        {
            try
            {
                vGridControl2.OptionsView.MinRowAutoHeight = 22;
                vGridControl2.RowHeaderWidth = 108;

                RepositoryItem repBrojUgovora = SetRepositoryTextEdit();
                RepositoryItem repPartneri = SetRepositoryButtonEdit("Odaberi partnere");
                RepositoryItem repUgovoriVrste = SetRepositoryLookUpEdit(GetUgovoriVrste(), "ugovorinajamvrsteid", "naziv", new List<string>() { "oznaka" });
                RepositoryItem repTipRazdobljaTrajanjaUgovora = SetRepositoryLookUpEdit(GetTipRazdobljaTrajanjaUgovora(), "ugovoritiptrajanjaid", "naziv");
                RepositoryItem repBrojProduljenja = SetRepositoryTextEdit();
                RepositoryItem repTrajanjeProduljenja = SetRepositoryTextEdit();

                RepositoryItem repDatumPotpisivanjaUgovora = SetRepositoryDateEdit("datumpotpisivanjaugovora");
                RepositoryItem repDatumPocetkaVazenjaUgovora = SetRepositoryDateEdit("datumpocetkavazenjaugovora");
                RepositoryItem repDatumprestankaVazenjaUgovora = SetRepositoryDateEdit("datumprestankavazenjaugovora");
                RepositoryItem repUgovorProduljenDo = SetRepositoryDateEdit("ugovorproduljendo");
                RepositoryItem repProcijenjeniDatumPrestankaVazenjaUgovora = SetRepositoryDateEdit("procijenjenidatumprestankavazenjaugovora");
                RepositoryItem repDatumTerminacijeUgovora = SetRepositoryDateEdit("datumterminacijeugovora");
                RepositoryItem repDatumPotpisivanjaAneksa = SetRepositoryDateEdit("datumpotpisivanjaaneksa");
                RepositoryItem repDatumPocetkaVazenjaAneksa = SetRepositoryDateEdit("datumpocetkavazenjaaneksa");
                RepositoryItem repOtkazniRokTele2 = SetRepositorySpinEdit();
                RepositoryItem repOtkazniRokPartnera = SetRepositorySpinEdit();
                RepositoryItem repObavijesniPeriod = SetRepositorySpinEdit();

                RepositoryItem repOdgovornaOsoba = SetRepositoryLookUpEdit(GetOdgovornaOsoba(), "osobeid", "osoba");
                RepositoryItem repOrganizacijskaJedinica = SetRepositorySearchEdit("orgshemaid", "orgshema");
                RepositoryItem repPosebanUgovorZaStruju = SetRepositoryCheckEdit();
                RepositoryItem repUgovorZaStruj = SetRepositoryLookUpEdit(null, "strujaugovorinajamglaid", "broj");
                RepositoryItem repBrojStarogUgovora = SetRepositoryTextEdit();

                CategoryRow cr = AddCategoryRow("Osnovni podaci");
                cr.ChildRows.Add(AddEditorRow("ugovorinajamvrsteid", "Vrsta ugovora", repUgovoriVrste));
                cr.ChildRows.Add(AddEditorRow("broj", "Broj ugovora", repBrojUgovora));
                cr.ChildRows.Add(AddEditorRow("partneri", "Partneri" + requiredString(), repPartneri));
                cr.ChildRows.Add(AddEditorRow("ugovoritiptrajanjaid", "Tip razdoblja trajanja ugovora" + requiredString(), repTipRazdobljaTrajanjaUgovora));
                cr.ChildRows.Add(AddEditorRow("brojproduljenja", "Broj produljenja" + requiredString(), repBrojProduljenja));
                cr.ChildRows.Add(AddEditorRow("trajanjeproduljenja", "Trajanje produljenja" + requiredString(), repTrajanjeProduljenja));

                cr = AddCategoryRow("Period");
                cr.ChildRows.Add(AddEditorRow("datumpotpisivanjaugovora", "Datum potpisivanja ugovora", repDatumPotpisivanjaUgovora));
                cr.ChildRows.Add(AddEditorRow("datumpocetkavazenjaugovora", "Datum početka važenja ugovora", repDatumPocetkaVazenjaUgovora));
                cr.ChildRows.Add(AddEditorRow("datumprestankavazenjaugovora", "Datum prestanka važenja ugovora", repDatumprestankaVazenjaUgovora));
                cr.ChildRows.Add(AddEditorRow("ugovorproduljendo", "Ugovor produljen do", repUgovorProduljenDo));
                cr.ChildRows.Add(AddEditorRow("procijenjenidatumprestanka", "Proc. datum prestanka važenja najma", repProcijenjeniDatumPrestankaVazenjaUgovora));
                cr.ChildRows.Add(AddEditorRow("datumterminacijeugovora", "Datum terminacije ugovora", repDatumTerminacijeUgovora));
                cr.ChildRows.Add(AddEditorRow("datumpotpisivanjaaneksa", "Datum potpisivanja aneksa", repDatumPotpisivanjaAneksa));
                cr.ChildRows.Add(AddEditorRow("datumpocetkavazenjaaneksa", "Datum početka važenja aneksa", repDatumPocetkaVazenjaAneksa));
                cr.ChildRows.Add(AddEditorRow("otkazniroktele2", "Otkazni rok Tele2 (u mjesecima)", repOtkazniRokTele2));
                cr.ChildRows.Add(AddEditorRow("otkaznirokpartner", "Otkazni rok partnera (u mjesecima)", repOtkazniRokPartnera));
                cr.ChildRows.Add(AddEditorRow("obavijesniperiod", "Obavijesni period (u mjesecima)", repObavijesniPeriod));
                cr = AddCategoryRow("Ostalo");
                cr.ChildRows.Add(AddEditorRow("osobeid", "Odgovorna osoba" + requiredString(), repOdgovornaOsoba));
                cr.ChildRows.Add(AddEditorRow("orgshema", "Organizacijska jedinica" + requiredString(), repOrganizacijskaJedinica));
                cr.ChildRows.Add(AddEditorRow("strujaugovorind", "Poseban ugovor za struju", repPosebanUgovorZaStruju));
                cr.ChildRows.Add(AddEditorRow("strujaugovorinajamglaid", "Ugovor za struju", repUgovorZaStruj));
                cr.ChildRows.Add(AddEditorRow("brojstarogugovora", "Broj eksternog ugovora", repBrojStarogUgovora));

                DT = GetDateSource();
                vGridControl2.DataSource = DT;
                if (Id == 0)
                {
                    DataRow toInsert = DT.NewRow();
                    DT.Rows.InsertAt(toInsert, 0);
                }

                setInitProperties();
                EnableLookupEditClearButtons();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Greška grida", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void setInitProperties()
        {
            this.bazneStaniceId = Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraBazneStanice'"));
            this.vodoviId = Convert.ToInt32(GetPubVar("'UgovoriNajamVrste_VrstaUgovoraVodovi'"));

            if (isAneks())
                (vGridControl2.GetRowByFieldName("datumpocetkavazenjaaneksa") as EditorRow).Properties.Caption = "Datum početka važenja aneksa" + requiredString();
            else
                (vGridControl2.GetRowByFieldName("datumpocetkavazenjaugovora") as EditorRow).Properties.Caption = "Datum početka važenja ugovora" + requiredString();

            if (Id == 0)
            {
                var odgOsobaIorgShemaDefaultValues = (GetOdgOsobaIorgShema() as DataTable);
                if (odgOsobaIorgShemaDefaultValues != null && odgOsobaIorgShemaDefaultValues.Rows.Count > 0)
                    SetValue(new List<string>() { "orgshemaid" }, odgOsobaIorgShemaDefaultValues.Rows[0]["orgshemaid"], 3);

                if (odgOsobaIorgShemaDefaultValues != null && odgOsobaIorgShemaDefaultValues.Rows.Count > 0)
                {
                    this.lastKnownOrgShemaText = odgOsobaIorgShemaDefaultValues.Rows[0]["orgshema"].ToString();
                    this.lastKnownOrgShemaId = Convert.ToInt32(odgOsobaIorgShemaDefaultValues.Rows[0]["orgshemaid"]);

                    SetValue(new List<string> { "ugovorinajamvrsteid" }, odgOsobaIorgShemaDefaultValues.Rows[0]["ugovorinajamvrsteid"]);
                    SetValue(new List<string> { "orgshema" }, odgOsobaIorgShemaDefaultValues.Rows[0]["orgshema"], 3);
                    SetValue(new List<string> { "osobeid" }, odgOsobaIorgShemaDefaultValues.Rows[0]["osobeid"]);
                }
                if (!DT.Columns.Contains("oznakaVrsteUgovora"))
                    DT.Columns.Add("oznakaVrsteUgovora");

                var kontrola = (vGridControl2.GetRowByFieldName("ugovorinajamvrsteid") as EditorRow);

                var dataRow = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit).GetDataSourceRowByKeyValue(kontrola.Properties.Value);
                var oznakaString = "";
                var a = DT.Rows[0]["ugovorinajamvrsteid"];
                if (dataRow != null)
                    oznakaString = (dataRow as DataRowView)["oznaka"].ToString();

                DT.Rows[0]["oznakaVrsteUgovora"] = oznakaString;

                SetValue(new List<string> { "datumpotpisivanjaugovora", "datumpocetkavazenjaugovora", "datumprestankavazenjaugovora", "procijenjenidatumprestanka" }, DateTime.Now);
                EnableDisableRow(new List<string>() { "ugovorproduljendo", "brojproduljenja", "trajanjeproduljenja", "strujaugovorinajamglaid" }, false);
                SetValue(new List<string> { "otkazniroktele2", "otkaznirokpartner", "obavijesniperiod" }, 0);
                SetValue(new List<string> { "strujaugovorind" }, DBNull.Value, 3);
                // get default org shema i odg osoba

                // datum pocetka vazenja aneksa treba biti enablean samo na izmjeni i ako je ugovor aneks
                EnableDisableRow(new List<string>() { "datumpocetkavazenjaaneksa", "datumpotpisivanjaaneksa" }, false);
            }
            else
            {
                this.lastKnownOrgShemaText = GetFieldValue("orgshema").ToString();
                this.lastKnownOrgShemaId = Convert.ToInt32(GetFieldValue("orgshemaid"));
                // prepiši partnere inicijalno u array
                this.LnPartneriIdArrayString = GetFieldValue("PartneriIdString").ToString();

                if (GetControlsEnabledInd() != null && (bool)GetControlsEnabledInd() == false)
                    DisableAllControls();
                else
                {
                    bool enabled = Convert.ToInt32(GetFieldValue("ugovoritiptrajanjaid")) == automatskoProduljenjeId ? true : false;
                    EnableDisableRow(new List<string>() { "brojproduljenja", "trajanjeproduljenja" }, enabled);
                    EnableDisableRow(new List<string>() { "ugovorproduljendo" }, false);
                    EnableDisableRow(new List<string>() { "ugovorinajamvrsteid" }, false);

                    if (Convert.ToInt32(GetFieldValue("ugovoritiptrajanjaid")) == Convert.ToInt32(GetPubVar("'UgovoriNajam_TipTrajanjaUgovora_DoDaljnjeg'")))
                    {   //ako je TipRazdobljaTrajanjaUgovora = Do daljnjeg
                        SetValue(new List<string> { "datumprestankavazenjaugovora" }, DBNull.Value);
                        EnableDisableRow(new List<string>() { "datumprestankavazenjaugovora" }, false);
                    }
                    if (GetFieldValue("strujaugovorind") != null && GetFieldValue("strujaugovorind") != DBNull.Value && Convert.ToBoolean(GetFieldValue("strujaugovorind")) == true)
                        EnableDisableRow(new List<string>() { "strujaugovorinajamglaid" }, true);
                    else
                        EnableDisableRow(new List<string>() { "strujaugovorinajamglaid" }, false);

                    if (isAneks())
                        EnableDisableRow(new List<string>() { "datumpocetkavazenjaugovora", "datumpotpisivanjaugovora" }, false);
                    else
                        EnableDisableRow(new List<string>() { "datumpocetkavazenjaaneksa", "datumpotpisivanjaaneksa" }, false);
                }
            }
        }

        private void EnableLookupEditClearButtons()
        {
            foreach (RepositoryItem rep in vGridControl2.RepositoryItems)
            {
                if (rep is RepositoryItemLookUpEdit && !string.IsNullOrEmpty((rep as RepositoryItemLookUpEdit).Name))
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

        private void DisableAllControls()
        {
            EnableDisableRow(new List<string>()
            { "ugovorinajamvrsteid", "broj", "partneri", "ugovoritiptrajanjaid", "brojproduljenja", "trajanjeproduljenja", "datumpotpisivanjaugovora",
                "datumpocetkavazenjaugovora", "datumprestankavazenjaugovora", "ugovorproduljendo", "procijenjenidatumprestanka","datumterminacijeugovora", "datumpocetkavazenjaaneksa"
                , "datumpotpisivanjaaneksa", "otkazniroktele2","otkaznirokpartner", "obavijesniperiod", "osobeid", "orgshema", "strujaugovorind", "strujaugovorinajamglaid"
                , "brojstarogugovora"}
            //, "datumprimopredaje"
            , false);
        }

        private void InitForm()
        {
            EnableDisableRow(new List<string> { "broj" }, false);
            if (GetFieldValue("ugovorinajamvrsteid") != DBNull.Value)
                ChangedUgovoriVrste(Convert.ToInt32(GetFieldValue("ugovorinajamvrsteid")));
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
            //er.Properties.Value = 20;
            return er;
        }

        #endregion Inits

        #region Events

        private void EnableDisableClearButton(object rep, bool enabled, string controlType)
        {
            if (controlType == "search")
                (rep as RepositoryItemButtonEdit).Buttons[0].Enabled = enabled;
            else if (controlType == "combo")
                (rep as RepositoryItemLookUpEdit).Buttons[0].Enabled = enabled;
        }

        private void vGridControl2_RecordCellStyle(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        }

        #endregion Events

        #region SetRepoistory

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
            PartneriPopUp popup = new PartneriPopUp();
            popup.LnPartneriIdArrayString = LnPartneriIdArrayString;
            if (this.Id != 0)
                popup.ugovorinajamglaid = this.Id;
            popup.ShowDialog();
            if (popup.DialogResult == DialogResult.OK)
            {
                LnPartneriIdArrayString = popup.LnPartneriIdArrayString;
            }
        }

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
            SetValue(new List<string>() { displayMember }, textValue, rowIndex);
            SetValue(new List<string>() { valueMember }, idValue == null ? DBNull.Value : idValue, rowIndex);

            lastKnownOrgShemaText = textValue;
            if (idValue == DBNull.Value)
                lastKnownOrgShemaId = null;
            else
                lastKnownOrgShemaId = Convert.ToInt32(idValue);
        }

        private void OnLeave(object s, EventArgs e, string valueMember, string displayMember)
        {
            try
            {
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
                            SetSearchEditValues(valueMember, displayMember, string.Empty, DBNull.Value, 3); // - dinamički index
                        // tab
                        else
                            SetSearchEditValues(valueMember, displayMember, lastKnownOrgShemaText, lastKnownOrgShemaId, 3); // - dinamički index
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
                            SetSearchEditValues(valueMember, displayMember, string.Empty, DBNull.Value, 3); // - dinamički index
                                                                                                            // ako je prije bila postavljena vrijednost
                        else
                            SetSearchEditValues(valueMember, displayMember, lastKnownOrgShemaText, lastKnownOrgShemaId, 3); // - dinamički index
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

        private void EventButtonEditSearchButtonClick(object sender, ButtonPressedEventArgs e, string idName, string stringName)
        {
            FilterDTandOpenSearchForm(sender, idName, stringName);
        }

        private void FilterDTandOpenSearchForm(object sender, string idName, string stringName)
        {
            try
            {
                if (vGridControl2.FocusedRow.Name.ToLower() == "row" + stringName)
                {
                    if (!string.IsNullOrEmpty((sender as ButtonEdit).Text))
                    {
                        var tblFiltered = TraziOrgShemu((sender as ButtonEdit).Text);

                        Forms.Search searchForm = new Forms.Search(tblFiltered, "", idName);
                        searchForm.ShowDialog();

                        if (searchForm.DialogResult == DialogResult.OK && searchForm.SearchId != null)
                        {
                            // setiraj textualnu vrijednost kada nađem partnera po zatvaranju search forme
                            (sender as ButtonEdit).EditValue = tblFiltered.AsEnumerable()
                                .Where(row => row.Field<Int32>(idName) == searchForm.SearchId).FirstOrDefault()[stringName].ToString();
                            // setiraj id partnera kada nađem partnera po zatvaranju search forme
                            SetValue(new List<string>() { idName }, searchForm.SearchId, 3); // -- dinamički row index
                            // tag - indikator da li je našao partnera
                            (sender as ButtonEdit).Tag = (sender as ButtonEdit).EditValue.ToString();

                            if (idName.ToLower() == "orgshemaid")
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
                SearchEditLookupEditButtonClick(s, e, rep, idName, stringName);
            };

            (rep as RepositoryItemButtonEdit).Buttons[1].IsDefaultButton = true;
        }

        private void SearchEditLookupEditButtonClick(object sender, ButtonPressedEventArgs e, RepositoryItem rep,
           string idName = "", string stringName = "")
        {
            if (e.Button.Kind == ButtonPredefines.Search)
            {
                FilterDTandOpenSearchForm(sender, idName, stringName);
            }
            else if (e.Button.Kind == ButtonPredefines.Delete)
            {
                try
                {
                    vGridControl2.EditingValue = !string.IsNullOrEmpty(idName) ? string.Empty : null; // ako sam poslao idName onda je search, moram u null
                }
                catch (Exception ex)    // dodan jer iz nepoznatog razloga ulazi u catch samo za Vrsta ugovora combo - clear nakon inicijalnog učitavanja. Sve dobro odrađuje ali baca obj ref not set...
                {
                    var a = ex.ToString();
                }

                EnableDisableClearButton(rep, false,
                    !string.IsNullOrEmpty(idName) ? "search" : "combo");
            }
        }

        #endregion searchEditRepo

        #region lookupEditRepo

        private RepositoryItem SetRepositoryLookUpEdit(DataTable dataSource, string valueMember, string displayMember, List<string> columns = null, object initValue = null)
        {
            RepositoryItemLookUpEdit rep = new RepositoryItemLookUpEdit();
            rep.DataSource = dataSource;
            rep.DisplayMember = displayMember;
            rep.ValueMember = valueMember;
            //rep.Name = displayMember;
            rep.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = displayMember });
            var popupWidth = 150;
            if (columns != null && columns.Count > 0)
            {
                popupWidth = columns.Count * 600;
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
                EventLookUpEditValueChanged(s, e);
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

        private void createUkloniLookupButton(RepositoryItem rep)
        {
            DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
            clearButton.IsDefaultButton = false;
            clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            clearButton.ToolTip = "Ukloni";
            clearButton.Enabled = false;

            (rep as RepositoryItemLookUpEdit).Buttons.Insert(0, clearButton);
            (rep as RepositoryItemLookUpEdit).ActionButtonIndex = 1;
            (rep as RepositoryItemLookUpEdit).ButtonClick += (s, e) =>
            {
                SearchEditLookupEditButtonClick(s, e, rep);
            };
        }

        private void ChangedUgovoriVrste(int? value)
        {
            if (value != null)
            {
                var kontrola = (vGridControl2.GetRowByFieldName("ugovorinajamvrsteid") as EditorRow);

                var dataRow = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit).GetDataSourceRowByKeyValue(value);
                var oznakaString = "";
                if (dataRow != null)
                    oznakaString = (dataRow as DataRowView)["oznaka"].ToString();

                if (!DT.Columns.Contains("oznakaVrsteUgovora"))
                    DT.Columns.Add("oznakaVrsteUgovora");

                DT.Rows[0]["oznakaVrsteUgovora"] = oznakaString;

                if (Id == 0)
                    SetValue(new List<string> { "strujaugovorind" }, DBNull.Value, 3);
                else
                {
                    var boolValue = (GetFieldValue("strujaugovorind") != DBNull.Value && (bool)GetFieldValue("strujaugovorind") == true) ? true : false;
                    SetValue(new List<string> { "strujaugovorind" }, boolValue, 3);
                    EnableDisableRow(new List<string> { "strujaugovorinajamglaid" },
                        (bool)GetFieldValue("strujaugovorind") == true
                        && (GetControlsEnabledInd() != null && (bool)GetControlsEnabledInd() == true)
                    );
                }

                var vrstaUgovoraId = (dataRow as DataRowView)["ugovorinajamvrsteid"];
                changeStrujaUgovorDatasource((Int32)vrstaUgovoraId);
            }
        }

        private void changeStrujaUgovorDatasource(int vrstaUgovoraId)
        {
            var kontrola = (vGridControl2.GetRowByFieldName("strujaugovorinajamglaid") as EditorRow);
            var cmbStrujaUgovor = (kontrola.Properties.RowEdit as RepositoryItemLookUpEdit);
            cmbStrujaUgovor.DataSource = GetUgovorZaStruju(vrstaUgovoraId);

            if (Id != 0 && (bool)GetFieldValue("strujaugovorind") == true && GetFieldValue("strujaugovorinajamglaid") != DBNull.Value)
                SetValue(new List<string> { "strujaugovorinajamglaid" }, (Int32)GetFieldValue("strujaugovorinajamglaid"), 2);
        }

        private void EventLookUpEditValueChanged(object s, EventArgs e)
        {
            var valueMember = (s as DevExpress.XtraEditors.LookUpEdit).Properties.ValueMember;

            if (valueMember == "ugovorinajamvrsteid")
            {
                int valueId = Convert.ToInt32(((s as DevExpress.XtraEditors.LookUpEdit).GetSelectedDataRow() as DataRowView)["ugovorinajamvrsteid"]);
                ChangedUgovoriVrste(valueId);
            }
            else if (valueMember == "ugovoritiptrajanjaid")
            {
                var value = (s as DevExpress.XtraEditors.LookUpEdit).EditValue;
                var text = (s as DevExpress.XtraEditors.LookUpEdit).Text.ToLower();
                bool enabled = false;
                if (text == "automatsko produljenje")
                    enabled = true;

                EnableDisableRow(new List<string>() { "brojproduljenja", "trajanjeproduljenja" }, enabled);
                if (enabled == false)
                    SetValue(new List<string>() { "brojproduljenja", "trajanjeproduljenja" }, DBNull.Value);
                else
                    SetValue(new List<string>() { "brojproduljenja" }, 1);

                if (Convert.ToInt32(value) == Convert.ToInt32(GetPubVar("'UgovoriNajam_TipTrajanjaUgovora_DoDaljnjeg'")))
                {   //ako je TipRazdobljaTrajanjaUgovora = Do daljnjeg
                    SetValue(new List<string> { "datumprestankavazenjaugovora" }, DBNull.Value);
                    EnableDisableRow(new List<string>() { "datumprestankavazenjaugovora" }, false);
                }
                else
                {
                    EnableDisableRow(new List<string>() { "datumprestankavazenjaugovora" }, true);
                    SetValue(new List<string> { "datumprestankavazenjaugovora" }, DateTime.Today);
                }
            }
        }

        #endregion lookupEditRepo

        #region DateEditRepo

        private RepositoryItem SetRepositoryDateEdit(string name)
        {
            RepositoryItemDateEdit rep = new RepositoryItemDateEdit();
            rep.Name = name;
            vGridControl2.RepositoryItems.Add(rep);
            rep.EditValueChanged += (s, e) =>
            {
                OnDateChanged(s, e, rep);
            };
            return rep;
        }

        private void OnDateChanged(object s, EventArgs e, RepositoryItemDateEdit rep)
        {
            if (rep.Name == "datumprestankavazenjaugovora" &&
                ((s as DateEdit).EditValue != null) && ((s as DateEdit).EditValue != DBNull.Value))
            {
                SetValue(new List<string>() { "procijenjenidatumprestanka" }, (s as DateEdit).EditValue);
            }
        }

        #endregion DateEditRepo

        #region SpinEditRepo

        private RepositoryItem SetRepositorySpinEdit()
        {
            RepositoryItemSpinEdit rep = new RepositoryItemSpinEdit();
            rep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            vGridControl2.RepositoryItems.Add(rep);
            rep.EditValueChanged += (s, e) =>
            {
                EventSpinEditEditValueChanged(s, e);
            };
            return rep;
        }

        private void EventSpinEditEditValueChanged(object s, EventArgs e)
        {
            int i;
            if ((s as DevExpress.XtraEditors.SpinEdit).EditValue != null)
            {
                string value = (s as DevExpress.XtraEditors.SpinEdit).EditValue.ToString();
                bool numeric = int.TryParse(value, out i);
                if (numeric)
                    (s as DevExpress.XtraEditors.TextEdit).EditValue = Convert.ToInt32(value) < 0 ? 0 : (s as DevExpress.XtraEditors.TextEdit).EditValue;
                else
                    (s as DevExpress.XtraEditors.TextEdit).EditValue = 0;
            }
            else
                return;
        }

        #endregion SpinEditRepo

        #region CheckEditRepo

        private RepositoryItem SetRepositoryCheckEdit()
        {
            RepositoryItemCheckEdit rep = new RepositoryItemCheckEdit();
            rep.GlyphAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.Caption = "";
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
            bool value = (bool)(s as DevExpress.XtraEditors.CheckEdit).EditValue;

            var enabled = value;
            EnableDisableRow(new List<string>() { "strujaugovorinajamglaid" }, enabled);

            if (enabled == false)
                SetValue(new List<string> { "strujaugovorinajamglaid" }, DBNull.Value);
        }

        #endregion CheckEditRepo

        #region TextEditRepo

        private RepositoryItem SetRepositoryTextEdit(string name = "")
        {
            RepositoryItemTextEdit rep = new RepositoryItemTextEdit();
            rep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            rep.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            if (!string.IsNullOrEmpty(name))
                rep.Name = name;

            vGridControl2.RepositoryItems.Add(rep);
            rep.EditValueChanged += (s, e) =>
            {
                EventTextEditEditValueChanged(s, e);
            };
            return rep;
        }

        private void EventTextEditEditValueChanged(object s, EventArgs e)
        {
            if (vGridControl2.FocusedRow.Name.ToLower().Contains("brojstarogugovora"))
                return;

            int i;
            string value = (s as DevExpress.XtraEditors.TextEdit).EditValue.ToString();
            bool numeric = int.TryParse(value, out i);
            if (numeric)
                (s as DevExpress.XtraEditors.TextEdit).EditValue = Convert.ToInt32(value) < 1 ? 1 : (s as DevExpress.XtraEditors.TextEdit).EditValue;
            else
                (s as DevExpress.XtraEditors.TextEdit).EditValue = 1;
        }

        #endregion TextEditRepo

        #endregion SetRepoistory

        #region DataSources

        private DataTable GetDateSource()
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_GetDetalji " + Id.ToString();
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetDetalji - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetUgovoriVrste()
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_GetUgovoriVrste";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetUgovoriVrste - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetTipRazdobljaTrajanjaUgovora()
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_TipRazdobljaTrajanjaUgovora";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];

                DataRow[] foundRows;
                foundRows = dt.Select("Naziv = 'Automatsko produljenje'");
                automatskoProduljenjeId = (int)foundRows[0][0];

                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_TipRazdobljaTrajanjaUgovora - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetOdgovornaOsoba()
        {
            try
            {
                string sql = "exec spTraziOsobe 'NaseOsobe'";
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spTraziOsobe - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetOrgShema()
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_GetOrgShema @searchstring='%', @operateriid=" + Operateriid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetOrgShema  - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private DataTable GetUgovorZaStruju(int? vrstaUgovoraId)
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_TrazilicaUgovoraPoVrsti " + vrstaUgovoraId;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_TrazilicaUgovoraPoVrsti - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private object GetOdgOsobaIorgShema()
        {
            try
            {
                //TODO:dinamički operateriid
                string sql = "exec spUgovoriNajamGla_GetDefaultValues " + Operateriid;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                return FormatDataColumns(dt);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetDefaultValues  - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata pubvars podataka - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetOrgShema @searchstring='" + searchString + "' - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private object GetControlsEnabledInd()
        {
            try
            {
                string sql = "exec spUgovoriNajamGla_GetDisableControlsInd " + Id;
                DataTable dt = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
                if (dt.Rows.Count > 0)
                    return (bool)dt.Rows[0][0];
                else
                    return false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Greška dohvata podataka: exec spUgovoriNajamGla_GetDisableControlsInd - " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                XtraMessageBox.Show("FormatDataColumns(), " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        #endregion DataSources

        private void EnableDisableRow(List<string> fields, bool enabled)
        {
            foreach (var field in fields)
            {
                (vGridControl2.GetRowByFieldName(field) as EditorRow).Enabled = enabled;
                ////*****postavljanje vrijednosti na null
                //dt.Rows[0][field] = DBNull.Value;
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
            return DT.Rows[0][fieldName];
        }

        #region helperMethods

        private bool validateRequiredFields()
        {
            // validate required fields
            foreach (CategoryRow cr in this.vGridControl2.Rows)
            {
                foreach (EditorRow row in cr.ChildRows)
                {
                    if ((row.Properties.FieldName.ToLower() != "partneri") &&   // partneri nisu u datasetu
                        row.Properties.Caption.Contains(requiredString())
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

        private bool isAneks()
        {
            return GetFieldValue("broj") != DBNull.Value && (GetFieldValue("aneksprethodniugovorid") != DBNull.Value);
        }

        #endregion helperMethods
    }
}