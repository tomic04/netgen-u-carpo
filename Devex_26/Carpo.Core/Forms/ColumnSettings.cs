using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Carpo.Core.Forms
{
    public partial class ColumnSettings : DevExpress.XtraEditors.XtraForm
    {
        // TO DO - logeru zabilježit
        private string selectedColumns = "";

        private Carpo.Structure.Models.Core.GridTemplatesModel templatesModel;
        private DataTable dt;
        private InsertEdit insertEdit;
        private int? carpoGridGlaId = null;
        private List<string> dataColumns = new List<string>();
        private Dictionary<string, string> dc = new Dictionary<string, string>();
        private GridView gv;
        private MemoryStream ms;

        private enum ColumnsState
        {
            Enabled,
            Disabled
        };

        private enum InsertEdit
        {
            Insert,
            Edit
        };

        public ColumnSettings(Carpo.Structure.Models.Core.GridTemplatesModel TemplatesModel, GridView Gv, MemoryStream Ms
            , Dictionary<string, string> DataColumns)
        {
            InitializeComponent();

            templatesModel = TemplatesModel;
            //dataColumns = DataColumns;
            dc = DataColumns;
            gv = Gv;
            ms = Ms;

            foreach (var obj in dc)
            {
                dataColumns.Add(obj.Key.ToString());
            }

            SetColumnsState(ColumnsState.Disabled);
            //SetComboProps();
            SetComboData(false);
            TranslateControls();
        }

        #region Methods

        //private void SetComboProps()
        //{
        //    lePredlosci.Properties.DisplayMember = "Naziv";
        //    lePredlosci.Properties.ValueMember = "CarpoGridGlaId";

        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CarpoGridGlaId") { Visible = false });
        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Naziv"));
        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Operater"));
        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Za ispis"));
        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Javan"));
        //    lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CarpoGridGlaLnId") { Visible = false });
        //}
        private void SetComboData(bool setId, bool resetiraj = false)
        {
            try
            {
                bool doSet = true;
                dt = Carpo.Data.Get.GridTemplates.GetTemplatesQuery(templatesModel.ConnStr, templatesModel.OperateriId, templatesModel.ModuleId, templatesModel.FormName, templatesModel.ComponentName);
                int rowHandleBeforeRefresh = gvPredlosci.FocusedRowHandle;
                int carpoGridGlaIdBeforeRefresh = Convert.ToInt32(carpoGridGlaId);
                gcPredlosci.DataSource = dt;

                if (dt.Rows.Count > 0)
                {
                    if (setId)
                    {
                        int id = Convert.ToInt32(gvPredlosci.GetRowCellValue(rowHandleBeforeRefresh, "CarpoGridGlaId"));
                        if (id != carpoGridGlaIdBeforeRefresh)
                        {
                            if (insertEdit == InsertEdit.Insert)
                                id = carpoGridGlaIdBeforeRefresh;

                            for (int i = 0; i < gvPredlosci.RowCount; i++)
                            {
                                if (Convert.ToInt32(gvPredlosci.GetRowCellValue(i, "CarpoGridGlaId")) == id)
                                {
                                    gvPredlosci.FocusedRowHandle = i;
                                    break;
                                }
                            }
                        }
                        else
                            SetBtnPostavi();

                        doSet = false;
                    }
                    else
                        gvPredlosci.FocusedRowHandle = 0; // u svakom slučaju postavljam prvi
                }
                else
                {
                    EnablePostavi(false, false);
                }

                if (doSet)
                    SetBtnPostavi();

                if (resetiraj)
                    ResetirajKoloneNaGridView();
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        private void SetColumnsState(ColumnsState state)
        {
            if (state == ColumnsState.Disabled)
            {
                btnDodajKolonu.Enabled = btnMakniKolonu.Enabled
                    = btnPomjeriKolonuDole.Enabled = btnPomjeriKolonuGore.Enabled = false;
            }
            else
            {
                btnDodajKolonu.Enabled = btnMakniKolonu.Enabled
                    = btnPomjeriKolonuDole.Enabled = btnPomjeriKolonuGore.Enabled = true;
                listOut.SelectedIndex = 0;
                listIn.SelectedIndex = 0;
            }
        }

        private void ResetirajKoloneNaGridView()
        {
            ms.Seek(0, SeekOrigin.Begin);
            gv.RestoreLayoutFromStream(ms);
        }

        private void SetBtnPostavi()
        {
            string postavljen = "";
            DataTable datatable = gcPredlosci.DataSource as DataTable;
            if (gvPredlosci.FocusedRowHandle >= 0)
            {
                postavljen = gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "CarpoGridGlaLnId").ToString();
                //chkJavanPredlosci.Checked = ((DataRowView)row)["Javan"].ToString() == "Da" ? true : false;
                //chkZaIspisPredlosci.Checked = ((DataRowView)row)["Za ispis"].ToString() == "Da" ? true : false;
            }
            else if (datatable.Rows.Count > 0 && gvPredlosci.FocusedRowHandle >= 0)
            {
                postavljen = dt.Rows[0]["CarpoGridGlaLnId"].ToString();
                //chkJavanPredlosci.Checked = dt.Rows[0]["Javan"].ToString() == "Da" ? true : false;
                //chkZaIspisPredlosci.Checked = dt.Rows[0]["Za ispis"].ToString() == "Da" ? true : false;
            }

            if (!string.IsNullOrEmpty(postavljen))
                EnablePostavi(false, true);
            else if (gvPredlosci.FocusedRowHandle >= 0 && gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "Za ispis").ToString() == "Da")
                EnablePostavi(true, false);
            else if (datatable != null && datatable.Rows.Count == 0 && gvPredlosci.FocusedRowHandle < 0)
                EnablePostavi(false, false);
            else
                EnablePostavi(true, false);
        }

        private void EnablePostavi(bool enabled1, bool enebled2)
        {
            btnPostaviPredlozak.Enabled = enabled1;
            btnUkloniPredlozak.Enabled = enebled2;
        }

        private void ShowPanel1(bool setCombo)
        {
            splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
            groupBox1.Text = "Predlošci";
            SetColumnsState(ColumnsState.Disabled);

            if (setCombo)
                SetComboData(true);
            else
                SetBtnPostavi();

            SetColumnListBox(InsertEdit.Edit);
        }

        private void ShowPanel2(InsertEdit InsertEdit, string caption)
        {
            insertEdit = InsertEdit;
            splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
            groupBox1.Text = caption;

            if (insertEdit == InsertEdit.Insert)
            {
                txtNoviPredlozak.Text = "";
                chkJavan.Checked = chkZaIspis.Checked = false;
            }
            else if (insertEdit == InsertEdit.Edit)
            {
                txtNoviPredlozak.Text = gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "Naziv").ToString();
                chkJavan.Checked = gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "Javan").ToString() == "Da" ? true : false;
                chkZaIspis.Checked = gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "Za ispis").ToString() == "Da" ? true : false;
            }

            SetColumnsState(ColumnsState.Enabled);
            SetColumnListBox(insertEdit);

            EnablePostavi(false, false);
        }

        private void SetColumnListBox(InsertEdit insertEdit)
        {
            listOut.Items.Clear();
            listIn.Items.Clear();

            if (insertEdit == InsertEdit.Insert)
            {
                foreach (var column in dc)
                {
                    listOut.Items.Add(column.Value);
                }
            }
            else
            {
                try
                {
                    List<string> columnsOut = new List<string>();
                    List<string> columnsIn = new List<string>();
                    List<string> columnsInFromBase =
                        Data.Get.GridTemplates.GetTemplateColumns(templatesModel.ConnStr, Convert.ToInt32(carpoGridGlaId));

                    foreach (var col in columnsInFromBase)
                    {
                        if (!(dataColumns.FindIndex(x => x.Equals(col, StringComparison.OrdinalIgnoreCase)) == -1))
                        {
                            string value = "";
                            dc.TryGetValue(col, out value);

                            columnsIn.Add(col);
                            listIn.Items.Add(value);
                        }
                    }

                    foreach (var col in dataColumns)
                    {
                        if (columnsIn.FindIndex(x => x.Equals(col, StringComparison.OrdinalIgnoreCase)) == -1)
                        {
                            string value = "";
                            dc.TryGetValue(col, out value);

                            columnsOut.Add(value);
                            listOut.Items.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, ex.Message);
                }
            }
        }

        private void SetSelectedColumnsString()
        {
            selectedColumns = "";
            for (int i = 0; i < listIn.Items.Count; i++)
            {
                //selectedColumns += listIn.Items[i];

                foreach (var obj in dc)
                {
                    if (obj.Value == listIn.Items[i].ToString())
                    {
                        selectedColumns += obj.Key.ToString();
                        break;
                    }
                }

                if (i < listIn.Items.Count - 1)
                {
                    selectedColumns += ",";
                }
            }
        }

        private void TranslateControls()
        {
            this.Text = Common.Translator.TranslateString("Postavke kolona grida");
            groupBox1.Text = Common.Translator.TranslateString("Predlošci:");
            groupBox2.Text = Common.Translator.TranslateString("Kolone:");
            btnDodajPredlozak.Text = Common.Translator.TranslateString("Dodaj");
            btnIzmijeniPredlozak.Text = Common.Translator.TranslateString("Izmijeni");
            btnObrisiPredlozak.Text = Common.Translator.TranslateString("Obriši");
            chkZaIspis.Text = Common.Translator.TranslateString("za ispis", true);
            chkJavan.Text = Common.Translator.TranslateString("javan", true);
            btnSpremiPredlozak.Text = Common.Translator.TranslateString("Spremi");
            btnOdustani.Text = Common.Translator.TranslateString("Odustani");
            btnPostaviPredlozak.Text = Common.Translator.TranslateString("POSTAVI PREDLOŽAK");
            btnUkloniPredlozak.Text = Common.Translator.TranslateString("UKLONI PREDLOŽAK");
            btnIzadji.Text = Common.Translator.TranslateString("ODUSTANI");
        }

        #endregion Methods

        #region Events

        // PANEL - PREDLOŠCI
        private void btnDodajPredlozak_Click(object sender, EventArgs e)
        {
            EnablePostavi(false, false);
            ShowPanel2(InsertEdit.Insert, Common.Translator.TranslateString("Novi predložak"));
            tableLayoutPanel1.RowStyles[0].Height = 50;
            txtNoviPredlozak.Select();
        }

        private void btnIzmijeniPredlozak_Click(object sender, EventArgs e)
        {
            if (gvPredlosci.FocusedRowHandle >= 0)
            {
                EnablePostavi(false, false);
                ShowPanel2(InsertEdit.Edit, Common.Translator.TranslateString("Postojeći predložak"));
                tableLayoutPanel1.RowStyles[0].Height = 50;
                txtNoviPredlozak.Select();
            }
        }

        private void btnObrisiPredlozak_Click(object sender, EventArgs e)
        {
            if (gvPredlosci.FocusedRowHandle >= 0)
            {
                var resetiraj = btnUkloniPredlozak.Enabled;
                string msg = "Želite li sigurno obrisati predložak?";
                DialogResult dialogResult =
                    Common.Helpers.JConfirm.Confirm(msg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        string sqlText = "";
                        int i = Data.Set.GridTemplates.DeleteTemplate(templatesModel.ConnStr, templatesModel.OperateriId, Convert.ToInt32(carpoGridGlaId), out sqlText);

                        if (i == 0)
                        {
                            Common.Helpers.JMessages.Warning("Javni predložak kojeg koristi drugi korisnik ne može se obrisati!");
                            return;
                        }

                        SetComboData(false, resetiraj);

                        gvPredlosci.FocusedRowHandle = -1;

                        if (gvPredlosci.FocusedRowHandle >= 0)
                            carpoGridGlaId = Convert.ToInt32(gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "CarpoGridGlaId"));
                    }
                    catch (Exception ex)
                    {
                        Common.Helpers.JMessages.Error(ex, ex.Message);
                    }
                }
            }
        }

        private void gvPredlosci_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            SetBtnPostavi();
            carpoGridGlaId = gvPredlosci.FocusedRowHandle < 0 ?
                null : gvPredlosci.GetRowCellValue(gvPredlosci.FocusedRowHandle, "CarpoGridGlaId") as int?;
            SetColumnListBox(InsertEdit.Edit);
        }

        private void btnPostaviPredlozak_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlText = "";
                Data.Set.GridTemplates.SetTemplateForUse(
                    templatesModel.ConnStr, templatesModel.OperateriId, templatesModel.ModuleId
                    , templatesModel.FormName, templatesModel.ComponentName, Convert.ToInt32(carpoGridGlaId), out sqlText);

                SetComboData(true);
                Carpo.Common.Helpers.GridTemplates.SetGridColumnsFromTemplate(GetTemplateVisibleColumns(), gv);
                //Common.Helpers.JMessages.Info("Predložak uspješno postavljen");
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        // PANEL - PREDLOŽAK EDIT
        private void btnSpremiPredlozak_Click(object sender, EventArgs e)
        {
            //string msg = insertEdit == InsertEdit.Insert ? "Predložak uspješno kreiran!" : "Predložak uspješno izmijenjen!";
            SetSelectedColumnsString();
            if (string.IsNullOrEmpty(txtNoviPredlozak.Text))
            {
                Common.Helpers.JMessages.Warning("Predložak mora sadržavati naziv!");
                return;
            }
            else if (string.IsNullOrEmpty(selectedColumns))
            {
                Common.Helpers.JMessages.Warning("Predložak mora sadržavati barem jednu kolonu!");
                return;
            }

            try
            {
                if (insertEdit == InsertEdit.Insert)
                {
                    string sqlText = "";
                    carpoGridGlaId = Data.Set.GridTemplates.InsertTemplate(templatesModel.ConnStr, txtNoviPredlozak.Text
                        , templatesModel.OperateriId, templatesModel.ModuleId, templatesModel.FormName, templatesModel.ComponentName
                        , chkZaIspis.Checked, chkJavan.Checked, out sqlText);
                }
                else if (insertEdit == InsertEdit.Edit)
                {
                    string sqlText = "";
                    int success = Data.Set.GridTemplates.EditTemplate(templatesModel.ConnStr, txtNoviPredlozak.Text
                        , templatesModel.OperateriId, chkZaIspis.Checked, chkJavan.Checked, Convert.ToInt32(carpoGridGlaId)
                        , out sqlText);

                    if (success == 0)
                    {
                        Common.Helpers.JMessages.Warning("Javni predložak kojeg koristi drugi korisnik ne može postati privatan!");
                        return;
                    }
                }

                string sqlTextC = "";
                if (!string.IsNullOrEmpty(selectedColumns))
                    Data.Set.GridTemplates.SetColumnsForTemplate(templatesModel.ConnStr, selectedColumns
                        , Convert.ToInt32(carpoGridGlaId), out sqlTextC);

                if (insertEdit == InsertEdit.Edit)
                    Carpo.Common.Helpers.GridTemplates.SetGridColumnsFromTemplate(GetTemplateVisibleColumns(), gv);

                //Common.Helpers.JMessages.Info(msg);
                ShowPanel1(true);
                tableLayoutPanel1.RowStyles[0].Height = 150;
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            ShowPanel1(false);
            tableLayoutPanel1.RowStyles[0].Height = 150;
        }

        private void btnDodajKolonu_Click(object sender, EventArgs e)
        {
            if (listOut.SelectedItems != null && listOut.SelectedItems.Count > 0)
            {
                var listToRemove = new List<object>();
                foreach (var item in listOut.SelectedItems)
                {
                    listIn.Items.Add(item.ToString());
                    listToRemove.Add(item);
                }

                foreach (var item in listToRemove)
                {
                    listOut.Items.Remove(item);
                }
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (!btnDodajKolonu.Enabled || !btnMakniKolonu.Enabled)
                return;

            var listBox = sender as DevExpress.XtraEditors.ListBoxControl;
            string columnName = listBox.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(columnName))
            {
                if (listBox.Name == "listOut")
                {
                    listIn.Items.Add(columnName);
                    listOut.Items.Remove(columnName);
                }
                else if (listBox.Name == "listIn")
                {
                    listOut.Items.Add(columnName);
                    listIn.Items.Remove(columnName);
                }
            }
        }

        private void btnIzadji_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnUkloniPredlozak_Click(object sender, EventArgs e)
        {
            //string msg = "Želite li sigurno ukloniti predložak za kontrolu '" + templatesModel.ComponentName + "'?";
            //DialogResult dialogResult =
            //    Common.Helpers.JConfirm.Confirm(msg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //if (dialogResult == DialogResult.Yes)
            //{
            try
            {
                string sqlText = "";
                Carpo.Data.Set.GridTemplates.StopUsingTemplate(templatesModel.ConnStr, templatesModel.OperateriId, "", "", ""
                    , carpoGridGlaId, out sqlText);
                //Common.Helpers.JMessages.Info("Postavljeni predložak za komponentu '" + templatesModel.ComponentName
                //+ "' uspješno uklonjen!");

                SetComboData(true);
                ResetirajKoloneNaGridView();
                btnUkloniPredlozak.Enabled = false; // za svaki slučaj disejblam jer nam negdje bježi enable, teško za istestirat
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        private void btnMakniKolonu_Click(object sender, EventArgs e)
        {
            if (listIn.SelectedItems != null && listIn.SelectedItems.Count > 0)
            {
                var listToRemove = new List<object>();
                foreach (var item in listIn.SelectedItems)
                {
                    listOut.Items.Add(item.ToString());
                    listToRemove.Add(item);
                }

                foreach (var item in listToRemove)
                {
                    listIn.Items.Remove(item);
                }
            }
        }

        private void btnPomjeriKolonuGore_Click(object sender, EventArgs e)
        {
            if (listIn.SelectedItems != null && listIn.SelectedItems.Count == 1)
            {
                int index = listIn.SelectedIndex;
                if (index > 0)
                {
                    object data = listIn.SelectedItem;
                    listIn.Items.Remove(data);
                    listIn.Items.Insert(index - 1, data);
                    listIn.SelectedItem = data;
                }
            }
        }

        private void btnPomjeriKolonuDole_Click(object sender, EventArgs e)
        {
            if (listIn.SelectedItems != null && listIn.SelectedItems.Count == 1)
            {
                int index = listIn.SelectedIndex;
                if (index < listIn.ItemCount - 1)
                {
                    object data = listIn.SelectedItem;
                    listIn.Items.Remove(data);
                    listIn.Items.Insert(index + 1, data);
                    listIn.SelectedItem = data;
                }
            }
        }

        // FORMA
        private void ColumnSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        #endregion Events

        public DataTable GetTemplateVisibleColumns()
        {
            var dt = new DataTable();

            try
            {
                dt = Carpo.Data.Get.GridTemplates.GetVisibleColumns(templatesModel.ConnStr, templatesModel.OperateriId
                    , templatesModel.ModuleId, templatesModel.FormName, templatesModel.ComponentName);
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
                dt = null;
            }

            return dt;
        }

        private void gvPredlosci_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                var postavljen = gvPredlosci.GetRowCellValue(e.RowHandle, "CarpoGridGlaLnId").ToString();
                if (!string.IsNullOrEmpty(postavljen))
                {
                    var font = new System.Drawing.Font(gvPredlosci.Appearance.Row.Font.FontFamily,
                        gvPredlosci.Appearance.Row.Font.Size, System.Drawing.FontStyle.Bold);
                    e.Appearance.Font = font;
                }
            }
        }
    }
}