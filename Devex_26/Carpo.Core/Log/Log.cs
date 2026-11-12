using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.Core.Log
{
    public partial class Log : DevExpress.XtraEditors.XtraForm
    {
        private string action = "";

        private Logger logger;
        //private List<Models.LoggerDef> logList;

        public bool ParentFormExists { get; set; } = false;
        public List<Structure.Models.JControl> ControlList { get; set; }
        public List<Structure.Models.JControl> ParentControlList { get; set; }
        public Dictionary<string, object> FormVars { get; set; }
        public Dictionary<string, object> ParentFormVars { get; set; }
        public Dictionary<string, object> GlobalVars { get; set; }
        public Dictionary<string, object> InitVars { get; set; }
        public string Version { get; set; }
        public string ActiveFormName { get; set; }

        //public Log(List<Models.LoggerDef> logList)
        //{
        //    action = "init";
        //    InitializeComponent();

        //    this.logList = logList;
        //    action = "";
        //}

        public Log(Logger logger)
        {
            action = "init";
            InitializeComponent();

            this.logger = logger;
            action = "";
        }

        private void Settings()
        {
            if (ParentFormExists == true)
                chkParentForm.Enabled = true;
        }

        private void Log_Load(object sender, EventArgs e)
        {
            Settings();
            Populate();
            SetVerzija();

            chkMode.BackColor = System.Drawing.ColorTranslator.FromHtml("#E1E2E7");
        }

        private void SetVerzija()
        {
            string verzija = "nedefinirano";
            if (!string.IsNullOrEmpty(Version))
                verzija = Version;

            lblVerzija.Text = string.Format("Verzija granule: <b>{0}</b>", verzija);
        }

        private void Populate()
        {
            PopulateLogTab(true);
            PopulateFormTab();
            PopulateDatalistInfoTab(logger.DatalistInfo);

            PopulateVars(GlobalVars, gcGlobalVariables, "global.$");
            gvGlobalVariables.BestFitColumns();

            PopulateVars(InitVars, gcGlobal, "");
            //gvGlobal.BestFitColumns();
            gvGlobal.Columns[0].BestFit();
            gvGlobal.Columns[0].OptionsColumn.FixedWidth = true;
            gvGlobal.Columns[1].BestFit();
            gvGlobal.Columns[1].OptionsColumn.FixedWidth = true;

            this.gvVarijable.ViewCaption = "Varijable forme" + " - " + ActiveFormName + GetFormDimensions(this.Owner);
        }

        private void PopulateDatalistInfoTab(DataTable dt)
        {
            gcDatalistInfo.DataSource = null;
            gcDatalistInfo.DataSource = dt;
            chkMode.EditValue = false;
        }

        private void PopulateLogTab(bool populateGrid)
        {
            var errNum = logger.LogList.Where(x => (x.LogLevel == LogLevel.Error)).Count();
            var warnNum = logger.LogList.Where(x => (x.LogLevel == LogLevel.Warn)).Count();
            var dataNum = logger.LogList.Where(x => (x.LogLevel == LogLevel.InfoData)).Count();

            chkLevels.Items["Error"].Description = "<color=" + GetHexColorLogLevel("Error") + ">" + chkLevels.Items["Error"].Tag + GenerateCountString(errNum) + "</color>";
            chkLevels.Items["Warn"].Description = "<color=" + GetHexColorLogLevel("Warn") + ">" + chkLevels.Items["Warn"].Tag + GenerateCountString(warnNum) + "</color>";
            chkLevels.Items["InfoData"].Description = chkLevels.Items["InfoData"].Tag + GenerateCountString(dataNum);
            chkLevels.Items["Info"].Description = "<color=" + GetHexColorLogLevel("Info") + ">" + chkLevels.Items["Info"].Tag + "</color>";
            chkLevels.Items["InfoCore"].Description = "<color=" + GetHexColorLogLevel("InfoCore") + ">" + chkLevels.Items["InfoCore"].Tag + "</color>";

            if (populateGrid == true)
                PopulateLogGrid();
        }

        private void PopulateFormTab()
        {
            gcVarijable.DataSource = null;
            gcFields.DataSource = null;
            gcKontrole.DataSource = null;

            string globalReferenca = GetParentReferenca();

            PopulateVars(chkParentForm.Checked ? ParentFormVars : FormVars, gcVarijable, globalReferenca + "$");
            gvVarijable.BestFitColumns();

            PopulateControls();
            gvKontrole.BestFitColumns();
        }

        private string GetHexColorLogLevel(string logLevel)
        {
            if (logLevel == "Error")
                return "#990000";
            else if (logLevel == "Warn")
                return "#00007f";
            else if (logLevel == "Info")
                return "#999999";
            else if (logLevel == "InfoCore")
                return "#005900";
            return "black";
        }

        private string GenerateCountString(int cnt)
        {
            if (cnt > 0)
            {
                return " (<b>" + cnt + "</b>)";
            }
            return " (0)";
        }

        private void PopulateLogGrid()
        {
            try
            {
                if (action == "")
                {
                    gvGla.Columns.AddField("Duration");
                    gvGla.Columns[4].Visible = true;
                    gvGla.Columns[4].Width = 50;
                    gcGla.DataSource = GetLogDataTable();
                    gvGla.BestFitMaxRowCount = 100;
                    gvGla.Columns[0].BestFit();
                    gvGla.Columns[0].OptionsColumn.FixedWidth = true;
                    gvGla.Columns[1].BestFit();
                    gvGla.Columns[1].OptionsColumn.FixedWidth = true;
                    gvGla.Columns[2].Width = 200;
                    //gvGla.BestFitColumns();
                }
            }
            catch { }
        }

        private void PopulateVars(Dictionary<string, object> vars, GridControl gc, string prefix)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Naziv", typeof(string));
                dt.Columns.Add("Tip", typeof(string));
                dt.Columns.Add("Vrijednost", typeof(string));
                if (!string.IsNullOrEmpty(prefix))
                    dt.Columns.Add("Referenca", typeof(string));
                if (vars != null && vars.Count > 0)
                {
                    foreach (KeyValuePair<string, object> var in vars)
                    {
                        DataRow dr = dt.NewRow();

                        string tip = "";
                        string value = "null";

                        if (var.Value != null)
                        {
                            // napravi string array ako je table parametar
                            if (var.Value.GetType() == typeof(System.Data.DataTable))
                                value = Carpo.Data.Common.GetDataTableValueString("", "", var);
                            else
                                value = var.Value.ToString();

                            tip = var.Value.GetType().ToString();
                        }

                        string key = var.Key;

                        dr["Naziv"] = key;
                        dr["Tip"] = tip;
                        dr["Vrijednost"] = value;
                        if (!string.IsNullOrEmpty(prefix))
                            dr["Referenca"] = prefix + key;

                        dt.Rows.Add(dr);
                    }
                }

                gc.DataSource = dt;
            }
            catch { }
        }

        private void PopulateControls()
        {
            List<Structure.Models.JControl> controlList = GetControlList();

            if (controlList != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Naziv kontrole", typeof(string));
                dt.Columns.Add("Tip", typeof(string));
                dt.Columns.Add("Vrijednost", typeof(string));

                dt.Columns.Add("Referenca", typeof(string));

                string globalReferenca = GetParentReferenca();

                foreach (var c in controlList)
                {
                    if (c != null)
                    {
                        DataRow dr = dt.NewRow();

                        string naziv = "nedefinirano";
                        string referenca = "";
                        if (c.IsNameDefinied)
                        {
                            naziv = c.Control.Name;
                            referenca = globalReferenca + "#" + naziv;
                        }

                        string[] tipSpl = c.ControlType.ToString().Split('.');
                        string tip = tipSpl[tipSpl.Length - 1];

                        string value = "";
                        if (c.Control is BaseEdit)
                        {
                            value = "null";
                            object valueObj = (c.Control as BaseEdit).EditValue;
                            if (c.ControlType.Name == "JSearchControl")
                            {
                                object valueObjSearch = (c.Control as Structure.Interfaces.IJEditor).GetValue("");
                                if (valueObjSearch != null)
                                    value = valueObjSearch.ToString();
                            }
                            else if (c.ControlType.Name == "JLookUpControl" && string.IsNullOrEmpty(c.Control.Text.ToString().Trim()))
                            { }
                            else if (valueObj != null)
                                value = valueObj.ToString();
                        }

                        dr["Naziv kontrole"] = naziv;
                        dr["Tip"] = tip;
                        dr["Vrijednost"] = value;
                        dr["Referenca"] = referenca;
                        dt.Rows.Add(dr);
                    }
                }

                gcKontrole.DataSource = dt;

                FormatDetailDataTable(new string[] { "Naziv kontrole", "Vrijednost" }, gvKontrole);
            }
        }

        private DataTable GetLogDataTable()
        {
            var infoList = logger.LogList.Where(x => (x.LogLevel == LogLevel.Error && chkLevels.Items["Error"].CheckState == CheckState.Checked)
                            || (x.LogLevel == LogLevel.Warn && chkLevels.Items["Warn"].CheckState == CheckState.Checked)
                            || (x.LogLevel == LogLevel.Info && chkLevels.Items["Info"].CheckState == CheckState.Checked)
                            || (x.LogLevel == LogLevel.InfoCore && chkLevels.Items["InfoCore"].CheckState == CheckState.Checked)
                            || (x.LogLevel == LogLevel.Test)
                            || (x.LogLevel == LogLevel.InfoData && chkLevels.Items["InfoData"].CheckState == CheckState.Checked));


            DataTable dt = new DataTable();
            try
            {
                dt.Columns.Add("Vrijeme", typeof(DateTime));
                dt.Columns.Add("LogLevel", typeof(string));
                dt.Columns.Add("Opis", typeof(string));
                dt.Columns.Add("Tekst", typeof(string));
                dt.Columns.Add("Args", typeof(object));
                dt.Columns.Add("Duration", typeof(string));
                foreach (Models.LoggerDef info in infoList)
                {
                    DataRow dr = dt.NewRow();
                    dr["Vrijeme"] = info.Time;
                    dr["LogLevel"] = info.LogLevel.ToString();
                    dr["Opis"] = info.Opis;
                    dr["Tekst"] = info.Text;
                    dr["Args"] = info.Args;
                    dt.Rows.Add(dr);
                    if ((dr["LogLevel"].ToString() == "InfoData"|| dr["LogLevel"].ToString() == "Test") && info.Args != null && ((object[])info.Args).Length > 0)
                    {
                        try
                        {
                            DateTime timeStart = (DateTime)((object[])info.Args)[0];
                            DateTime timeEnd = info.Time;


                            dr["Duration"] = string.Format("{0:n4} sec", ((timeEnd - timeStart).TotalSeconds));
                        }
                        catch (Exception e)
                        { }

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw e;
            }
            return dt;
        }

        private DataTable GetVariables()
        {
            return null;
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Naziv", typeof(string));
            //dt.Columns.Add("Tip", typeof(string));
            //dt.Columns.Add("Vrijednost", typeof(string));
            //foreach (Structure.Interfaces.IJControl info in ControlList)
            //{
            //    DataRow dr = dt.NewRow();
            //    dr["Vrijeme"] = info.;
            //    dr["LogLevel"] = info.LogLevel.ToString();
            //    dr["LogType"] = info.LogType.ToString();
            //    dr["Tekst"] = info.Text;
            //    dr["Args"] = info.Args;
            //    dt.Rows.Add(dr);
            //}
            //return dt;

            //foreach (var item in ControlList)
            //{
            //    item.ControlName =
            //}

            //gcVarijable.DataSource =
        }

        private void gvGla_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                try
                {
                    SetDetails();
                    //propertyGrid.SelectedObject = gvGla.GetDataRow(e.FocusedRowHandle)["Args"];
                }
                catch { }
                finally { }
            }
        }

        private void SetDetails()
        {
            object args = gvGla.GetDataRow(gvGla.FocusedRowHandle)["Args"];
            GetDetailDataTable(args);
        }

        private void GetDetailDataTable(object args)
        {
            if (args == null || args == DBNull.Value)
            {
                gcDetails.DataSource = null;
                return;
            }

            DataRow drGla = gvGla.GetDataRow(gvGla.FocusedRowHandle);

            if (drGla["LogLevel"].ToString() == "InfoData" && ((object[])args).Length > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("TimeStart", typeof(DateTime));
                dt.Columns.Add("TimeEnd", typeof(DateTime));
                dt.Columns.Add("Duration", typeof(string));

                DateTime timeStart = (DateTime)((object[])args)[0];
                DateTime timeEnd = (DateTime)drGla["Vrijeme"];

                DataRow dr = dt.NewRow();
                dr["Description"] = "početak i kraj okidanja";
                dr["TimeStart"] = timeStart;
                dr["TimeEnd"] = timeEnd;
                dr["Duration"] = string.Format("{0:n4} sec", ((timeEnd - timeStart).TotalSeconds));
                dt.Rows.Add(dr);
                gcDetails.DataSource = dt;
                FormatDetailDataTable(new string[] { "Duration" }, gvDetails);
                gvDetails.BestFitColumns();
            }
            else
            {
                gvDetails.Columns.Clear();
                gcDetails.DataSource = null;
            }
        }

        private void FormatDetailDataTable(string[] fieldsBold)
        {
            foreach (GridColumn col in gvDetails.Columns)
            {
                switch (Type.GetTypeCode(col.ColumnType))
                {
                    case TypeCode.DateTime:
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        col.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm:ss.fff";
                        break;
                }
                if (fieldsBold.Contains(col.FieldName))
                    col.AppearanceCell.Font = new Font(col.AppearanceCell.Font, FontStyle.Bold);
            }
        }

        private void FormatDetailDataTable(string[] fieldsBold, GridView gridView)
        {
            foreach (GridColumn col in gridView.Columns)
            {
                switch (Type.GetTypeCode(col.ColumnType))
                {
                    case TypeCode.DateTime:
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        col.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm:ss.fff";
                        break;
                }
                if (fieldsBold.Contains(col.FieldName))
                    col.AppearanceCell.Font = new Font(col.AppearanceCell.Font, FontStyle.Bold);
            }
        }

        private void chkLevels_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            PopulateLogGrid();
        }

        private void gvGla_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string logLevel = gvGla.GetDataRow(e.RowHandle)["LogLevel"].ToString();
                    string hexColor = GetHexColorLogLevel(logLevel);
                    e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml(hexColor);
                }
            }
            catch { }
        }

        private void cmdLogClear_Click(object sender, EventArgs e)
        {
            gcDetails.DataSource = null;
            gcGla.DataSource = null;

            logger.LogList.Clear();
            logger.ErrorExists = false;
            logger.WarningExists = false;
            PopulateLogTab(false);
            logger.SetIcon();
        }

        private void gvKontrole_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                List<Structure.Models.JControl> controlList = GetControlList();

                string type = gvKontrole.GetDataRow(e.FocusedRowHandle)["Tip"].ToString();
                string naziv = gvKontrole.GetDataRow(e.FocusedRowHandle)["Naziv kontrole"].ToString();
                if (type == "JGridControl")
                {
                    var c = controlList.Where(x => x.Control.Name == naziv).FirstOrDefault();
                    if (c.Control is GridControl)
                    {
                        GridControl gc = c.Control as GridControl;
                        if (gc.DataSource != null && gc.DataSource is DataTable)
                        {
                            if (!(gc.DefaultView is GridView))
                                return;

                            GridView gv;
                            gv = gc.DefaultView as GridView;

                            DataRow dr = null;
                            if (gv.FocusedRowHandle >= 0)
                                dr = gv.GetDataRow(gv.FocusedRowHandle);

                            gcFields.DataSource = GetMasterDataSource(gc.DataSource as DataTable, dr);
                            FormatDetailDataTable(new string[] { "Naziv polja", "Vrijednost" }, gvFields);
                            gvFields.BestFitColumns();
                        }
                        else
                            gcFields.DataSource = null;
                    }
                }
                else if (type == "JTreeListControl")
                {
                    var c = controlList.Where(x => x.Control.Name == naziv).FirstOrDefault();
                    if (c.Control is TreeList)
                    {
                        TreeList gc = c.Control as TreeList;
                        if (gc.DataSource != null)
                        {
                            if (gc.DataSource is List<Carpo.Structure.Models.Dates.TreeMonths>)
                            {
                                Carpo.Structure.Models.Dates.TreeMonths obj = gc.GetDataRecordByNode(gc.FocusedNode) as Carpo.Structure.Models.Dates.TreeMonths;
                                gcFields.DataSource = GetMasterDataSource(gc.DataSource as DataTable, obj);
                            }
                            else
                            {
                                DataRow dr = null;
                                if (gc.FocusedNode != null && gc.FocusedNode != gc.Nodes.AutoFilterNode)
                                {
                                    if (gc.GetDataRecordByNode(gc.FocusedNode) is DataRowView)
                                        dr = (gc.GetDataRecordByNode(gc.FocusedNode) as DataRowView).Row as DataRow;
                                }
                                gcFields.DataSource = GetMasterDataSource(gc.DataSource as DataTable, dr);
                            }

                            FormatDetailDataTable(new string[] { "Naziv polja", "Vrijednost" }, gvFields);
                            gvFields.BestFitColumns();
                        }
                    }
                }
                else
                {
                    gvFields.Columns.Clear();
                    gcFields.DataSource = null;
                }
            }
            catch { }
        }

        private List<Structure.Models.JControl> GetControlList()
        {
            if (chkParentForm.Checked == true)
                return ParentControlList;
            return ControlList;
        }

        private string GetParentReferenca()
        {
            if (chkParentForm.Checked == true)
                return "parent.";
            else
                return "";
        }

        private DataTable GetMasterDataSource(DataTable dtSource, DataRow drFocused)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Naziv polja", typeof(string));
            dt.Columns.Add("Tip", typeof(string));
            dt.Columns.Add("Vrijednost", typeof(string));
            dt.Columns.Add("Referenca", typeof(string));

            string referenca = "";

            if (gvKontrole.FocusedRowHandle >= 0)
            {
                referenca = gvKontrole.GetDataRow(gvKontrole.FocusedRowHandle)["Referenca"].ToString();
            }

            foreach (DataColumn col in dtSource.Columns)
            {
                string vrijednost = "";
                if (drFocused != null)
                {
                    if (drFocused[col.Caption] != null)
                    {
                        vrijednost = drFocused[col.Caption].ToString();
                    }
                }

                DataRow dr = dt.NewRow();
                dr["Naziv polja"] = col.Caption;
                dr["Tip"] = col.DataType.ToString();
                dr["Vrijednost"] = vrijednost;
                dr["Referenca"] = referenca + ".@" + col.Caption;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private DataTable GetMasterDataSource(DataTable dtSource, Carpo.Structure.Models.Dates.TreeMonths model)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Naziv polja", typeof(string));
            dt.Columns.Add("Tip", typeof(string));
            dt.Columns.Add("Vrijednost", typeof(string));
            dt.Columns.Add("Referenca", typeof(string));

            //parant = gvKontrole.GetDataRow(gvKontrole.FocusedRowHandle)["Naziv"]);

            PopulateDtFromModel(dt, GetModelValue(model.DatumDo), "DatumDo");
            PopulateDtFromModel(dt, GetModelValue(model.DatumOd), "DatumOd");
            PopulateDtFromModel(dt, GetModelValue(model.Id), "Id");
            PopulateDtFromModel(dt, GetModelValue(model.Month), "Month");
            PopulateDtFromModel(dt, GetModelValue(model.Name), "Name");
            PopulateDtFromModel(dt, GetModelValue(model.ParentId), "ParentId");
            PopulateDtFromModel(dt, GetModelValue(model.ValueId), "ValueId");
            PopulateDtFromModel(dt, GetModelValue(model.Year), "Year");

            return dt;
        }

        private void PopulateDtFromModel(DataTable dt, object field, string fieldName)
        {
            DataRow dr = dt.NewRow();
            dr["Naziv polja"] = fieldName;
            dr["Tip"] = field.GetType().ToString();
            if (field != null)
                dr["Vrijednost"] = field.ToString();
            else
                dr["Vrijednost"] = "null";
            dt.Rows.Add(dr);
        }

        private string GetModelValue(object value)
        {
            if (value == null)
                return "null";

            return value.ToString();
        }

        private void gvGla_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string logLevel = gvGla.GetDataRow(e.RowHandle)["LogLevel"].ToString();
                    if (logLevel == "Test") { e.Appearance.ForeColor = Color.Orange; return; }

                    string hexColor = GetHexColorLogLevel(logLevel);
                    e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml(hexColor);
                }
            }
            catch { }
        }

        private void chkForm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.CheckEdit checkControl = (sender as DevExpress.XtraEditors.CheckEdit);

                if (checkControl.Checked)
                {
                    checkControl.Properties.Appearance.Font = new Font(checkControl.Properties.Appearance.Font, FontStyle.Bold);
                    PopulateFormTab();
                }
                else
                {
                    checkControl.Properties.Appearance.Font = new Font(checkControl.Properties.Appearance.Font, FontStyle.Regular);
                }

                if (chkParentForm.Checked == true)
                    this.gvVarijable.ViewCaption = "Varijable forme" + " - " + this.Owner.Owner.Name + GetFormDimensions(this.Owner.Owner);
                else
                    this.gvVarijable.ViewCaption = "Varijable forme" + " - " + ActiveFormName + GetFormDimensions(this.Owner);
            }
            catch (Exception ex)
            {
                this.logger.Error("chkForm_CheckedChanged", ex.Message, null);
            }
        }

        private string GetFormDimensions(Form form)
        {
            return " (" + form.Width.ToString() + " x " + form.Height.ToString() + ")";
        }

        private void Log_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

        #region DatalistInfo

        private void chkMode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMode.Checked == true)    // copy mode, sakrij detalje
                {
                    var dt = CreateDistinctTable();
                    gcDatalistInfo.DataSource = dt;
                    if (dt != null)
                        HideColumns();
                    browserDatalist.Visible = false;
                    splitterDatalist.Visible = false;
                }
                else
                {   // info mode, pokazi sve detalje - store (i duple) i uvjete na parametrima
                    ShowColumns();
                    gcDatalistInfo.DataSource = logger.DatalistInfo;
                    browserDatalist.Visible = true;
                    splitterDatalist.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("chkMode_CheckedChanged", ex.Message, null);
            }
        }

        private void HideColumns()
        {
            for (int i = 0; i < 5; i++)
            {
                gvDatalistInfo.Columns[i + 1].Visible = false;
            }
        }

        private void ShowColumns()
        {
            List<string> listColumns = new List<string>() { "Type", "Form", "Name", "Scope", "Parameters" };
            int i = 0;
            foreach (var str in listColumns)
            {
                gvDatalistInfo.Columns[str].VisibleIndex = i + 1;
                gvDatalistInfo.Columns[str].Visible = true;
                i++;
            }
        }

        private DataTable CreateDistinctTable()
        {
            try
            {
                var UniqueRecords = new List<DataRow>();
                var DuplicateRecords = new List<DataRow>();
                var distinctDatatable = logger.DatalistInfo.Clone();
                var spRecords = new List<DataRow>();

                foreach (DataRow row in logger.DatalistInfo.Rows)
                {
                    // kopiraj tablicu u distinct tablicu, kasnije ćemo maknuti duple redove
                    DataRow dr = distinctDatatable.NewRow();
                    for (int i = 0; i < 5; i++)
                    {
                        dr[i] = row[i].ToString();
                    }

                    distinctDatatable.Rows.Add(dr);
                }

                foreach (DataRow dRow in distinctDatatable.Rows) // razdvoji distinct imena stori
                {
                    // ako uniqueRecords lista sadrži datarow kojemu je column[0] jednak kao i dRow[0] (ime store) gurni ju u duple redove, u suprotnom je unique
                    if (UniqueRecords.Where(x => x.ItemArray[0].ToString().ToLower() == dRow.ItemArray[0].ToString().ToLower()).FirstOrDefault() != null)
                        DuplicateRecords.Add(dRow);
                    else if (dRow.ItemArray[1].ToString() != "stored procedure")  //ako je kolona reda tipa "stored procedure" onda dodaj red u Listu spRecords
                    {
                        spRecords.Add(dRow);
                    }
                    else
                        UniqueRecords.Add(dRow);
                }

                foreach (DataRow dRow in DuplicateRecords)  // makni redove s ponavlajucim imenima store kako bi dobili distinct tablicu
                {
                    distinctDatatable.Rows.Remove(dRow);
                }

                foreach (DataRow row in spRecords)
                {
                    distinctDatatable.Rows.Remove(row);
                }

                return distinctDatatable;
            }
            catch
            {
                return null;
            }
        }

        private void ExportSelectedProceduresToTxt(String parameter)
        {
            ArrayList rows = new ArrayList();
            Int32[] selectedRowHandles = gvDatalistInfo.GetSelectedRows();

            for (int i = 0; i < selectedRowHandles.Length; i++)
            {
                int selectedRowHandle = selectedRowHandles[i];
                if (selectedRowHandle >= 0)
                    rows.Add(gvDatalistInfo.GetDataRow(selectedRowHandle));
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = @"C:\",
                Title = "Save script",
                DefaultExt = "sql",
                Filter = "SQL (*.sql)|*.sql",
                FilterIndex = 2,
                RestoreDirectory = true,
                CheckFileExists = false,
                CheckPathExists = false
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    for (int i = 0; i < rows.Count; i++)
                    {
                        // kreiranje definicijske tablice za helper metodu ExecuteDatasetTypedParams
                        DataTable table = new DataTable();

                        // kreiranje parametara
                        table.Columns.Add("storedProcedure", typeof(string));
                        table.Columns.Add("action", typeof(string));

                        // popunjavanje parametara sa vrijednostima
                        DataRow defRow = table.NewRow();

                        DataRow row = rows[i] as DataRow;  // dohvaća trenutni red
                        defRow["storedProcedure"] = row[0].ToString().Replace("dbo.", String.Empty); // dohvaća ime store iz grida
                        defRow["action"] = parameter;

                        // dodavanje parametara u tablicu
                        table.Rows.Add(defRow);

                        DataSet storaDataSet = Carpo.Data.SqlHelper.ExecuteDatasetTypedParams(Carpo.Data.Connection.ConnectionString, "spCarpoScriptGenBck", table.Rows[0]);
                        DataTable storaTbl = storaDataSet.Tables[0];
                        string storaTxt = storaTbl.Rows[0][0].ToString();   // vraća text store

                        sw.WriteLine(storaTxt);
                    }

                    MessageBox.Show("Skripta uspješno kreirana.");
                }
            }
        }

        private void gvDatalistInfo_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var XmlString = "";
            var dataName = "";

            try
            {
                if (gvDatalistInfo.FocusedRowHandle >= 0)
                {
                    dataName = gvDatalistInfo.GetDataRow(gvDatalistInfo.FocusedRowHandle)["Name"].ToString();
                    var sp = gvDatalistInfo.GetDataRow(gvDatalistInfo.FocusedRowHandle)["StoredProcedure"].ToString();

                    foreach (var list in Carpo.Common.Helpers.SpecificElementList.DatalistModelList)
                    {
                        var moduleId = InitVars.Where(x => x.Key == "@@moduleid").FirstOrDefault().Value;

                        var temp = list.Where(x => x.Name == dataName && x.StoredProcedure == sp && x.ModuleId == moduleId.ToString()).FirstOrDefault();
                        if (temp != null)
                        {
                            XmlString = temp.XMLvalue;
                            break;
                        }
                    }

                    System.Xml.Xsl.XslCompiledTransform xTrans = new System.Xml.Xsl.XslCompiledTransform();

                    using (var reader = new StringReader(Properties.Resources.defaultss))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(reader))
                        {
                            xTrans.Load(xmlReader);
                            //xTrans.Transform(fileName, arguments, xmlTextWriter);
                        }
                    }

                    // Read the xml string data into an XML reader object
                    StringReader sr = new StringReader(XmlString);
                    XmlReader xReader = XmlReader.Create(sr);
                    //Apply / transform the XML data
                    Stream ms = new MemoryStream();
                    ms.Position = 0;
                    xTrans.Transform(xReader, null, ms);
                    // Reset the position
                    ms.Position = 0;
                    // Set to the document stream
                    browserDatalist.DocumentStream = ms;

                    browserDatalist.Visible = true;
                }
                else
                    browserDatalist.Visible = false;
            }
            catch (Exception ex)
            {
                this.logger.Error("gvDatalistInfo_FocusedRowChanged", ex.Message, null);
            }
        }

        #endregion DatalistInfo

        private void gvDatalistInfo_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (chkMode.Checked == true)
                browserDatalist.Visible = false;
        }

        private void gvDatalistInfo_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            // Check whether a row is right-clicked.
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row && chkMode.Checked == true)
            {
                int rowHandle = e.HitInfo.RowHandle;
                if (rowHandle < 0)
                    return;
                // Delete existing menu items, if any.
                e.Menu.Items.Clear();
                // Add a submenu with a single menu item.
                e.Menu.Items.Add(CreateRowSubMenu(view, rowHandle));
            }
        }

        private DXMenuItem CreateRowSubMenu(GridView view, int rowHandle)
        {
            DXSubMenuItem subMenu = new DXSubMenuItem("Generate script");

            DXMenuItem ExportToTxtItem = new DXMenuItem("&CREATE To", new EventHandler(OnExportToTxt), null);
            ExportToTxtItem.Tag = new RowInfo(view, rowHandle);
            subMenu.Items.Add(ExportToTxtItem);

            DXMenuItem ExportIfExists = new DXMenuItem("&DROP And CREATE To", new EventHandler(OnExportIfExists), null);
            ExportIfExists.Tag = new RowInfo(view, rowHandle);
            subMenu.Items.Add(ExportIfExists);

            return subMenu;
        }

        private void OnExportToTxt(object sender, EventArgs e)
        {
            ExportSelectedProceduresToTxt("alter");
        }

        private void OnExportIfExists(object sender, EventArgs e)
        {
            ExportSelectedProceduresToTxt("drop");
        }

        private class RowInfo
        {
            public RowInfo(GridView view, int rowHandle)
            {
                this.RowHandle = rowHandle;
                this.View = view;
            }

            public GridView View;
            public int RowHandle;
        }
    }
}