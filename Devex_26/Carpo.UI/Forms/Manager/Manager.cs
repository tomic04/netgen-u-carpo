using Carpo.Common.Helpers;
using Carpo.Core;
using Carpo.Data.Data.Context;
using Carpo.Data.Data.Service;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components;
using Carpo.UI.Components.Container;
using Carpo.UI.Connector;
using Carpo.UI.Interfaces;
using Carpo.UI.Logic;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Forms.Master
{
    public partial class Manager : Base.JupiterManager, ISettingsForm
    {
        #region Publics

        public SettingsMaster JSettings { get; set; }

        #endregion Publics

        #region VisibleVariables

        public bool TreeControlVisible { get; set; } = true;
        public bool MasterGridVisible { get; set; } = true;
        public bool DetailGridVisible { get; set; } = true;

        private List<Structure.Models.JControl> tempTabGridList = new List<Structure.Models.JControl>();
        public JXtraTabControl tabControl;

        public List<JButtonControl> CrudButtons { get; set; } = new List<JButtonControl>();
        public List<JButtonControl> SelfDragButtons { get; set; } = new List<JButtonControl>();
        public List<string> additionalMasterTabList = new List<string>();
        public List<string> additionalDetailTabList = new List<string>();

        private bool treeFilterVisible = true;

        #endregion VisibleVariables

        #region PrivateVariables

        //indikator statusCollapse statusa
        private bool isTreeCollapsed = true;

        //private string assemblyName, assemblyFolder;
        private JData jDataMaster;

        private const string cntMasterGrid = "master grid: <b>##</b>";
        private const string cntDetailGrid = "detalj grid: <b>##</b>";

        #endregion PrivateVariables

        #region InitForm

        //public Manager()
        //{
        //    DevExpress.Skins.SkinManager.DisableFormSkins();
        //    InitializeComponent();

        //    JSettings = new SettingsMaster();
        //}
        Dictionary<string, byte[]> settings = new Dictionary<string, byte[]>();

        private const string EnvFilterMenuId = "CARPO_FILTER_MENU_ID";
        private const byte FilterLayoutsFileVersion = 1;
        private const string FilterLayoutsPayloadFileName = "filterlayouts.v1.bin";

        private static string NumericMenuIdFromEnvironment()
        {
            try
            {
                var v = Environment.GetEnvironmentVariable(EnvFilterMenuId);
                if (string.IsNullOrWhiteSpace(v)) return "";
                v = v.Trim();
                if (v.Length == 0) return "";
                for (int i = 0; i < v.Length; i++)
                {
                    if (!char.IsDigit(v[i])) return "";
                }
                return v;
            }
            catch { return ""; }
        }

        private static bool IsAsciiDigitMenuIdString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i])) return false;
            }
            return true;
        }

        /// <summary>Prvo <see cref="PubVars.MenuID"/> (samo cifre), inače env <c>CARPO_FILTER_MENU_ID</c>.</summary>
        private static string GetEffectiveMenuIdForFilterPostavke()
        {
            if (!string.IsNullOrEmpty(PubVars.MenuID))
            {
                string m = PubVars.MenuID.Trim();
                if (IsAsciiDigitMenuIdString(m))
                    return m;
            }
            return NumericMenuIdFromEnvironment();
        }

        private static string TryGetInitialCatalogFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return "";
            try
            {
                var csb = new SqlConnectionStringBuilder(connectionString);
                return (csb.InitialCatalog ?? "").Trim();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>%AppData%\Roaming\Carpo\FilterLayouts\{hash16}\</summary>
        private string GetFilterLayoutsAppDataFolderPath()
        {
            string db = TryGetInitialCatalogFromConnectionString(ConnectionString ?? "");
            string composite = PubVars.OperateriId.ToString() + "|" + db + "|" + (ModuleId ?? "") + "|" + (FormName ?? "");
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(composite));
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Carpo", "FilterLayouts",
                    BitConverter.ToString(hash, 0, 16).Replace("-", ""));
            }
        }

        private void MergeFilterLayoutsFromAppDataFileIntoSettings()
        {
            try
            {
                string path = Path.Combine(GetFilterLayoutsAppDataFolderPath(), FilterLayoutsPayloadFileName);
                if (!File.Exists(path)) return;
                using (var fs = File.OpenRead(path))
                using (var r = new BinaryReader(fs, Encoding.UTF8, leaveOpen: false))
                {
                    if (r.BaseStream.Length < 5) return;
                    if (r.ReadByte() != FilterLayoutsFileVersion) return;
                    int n = r.ReadInt32();
                    if (n < 0 || n > 100000) return;
                    for (int i = 0; i < n; i++)
                    {
                        string cvor = r.ReadString();
                        int len = r.ReadInt32();
                        if (len < 0 || len > 100_000_000) return;
                        byte[] blob = r.ReadBytes(len);
                        if (blob.Length != len) return;
                        settings[cvor] = blob;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Manager > MergeFilterLayoutsFromAppDataFileIntoSettings", ex.Message, ex);
            }
        }

        private void PersistFilterLayoutsToAppDataFile()
        {
            try
            {
                string dir = GetFilterLayoutsAppDataFolderPath();
                Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, FilterLayoutsPayloadFileName);
                using (var fs = File.Create(path))
                using (var w = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false))
                {
                    w.Write(FilterLayoutsFileVersion);
                    w.Write(settings.Count);
                    foreach (var kv in settings)
                    {
                        w.Write(kv.Key);
                        w.Write(kv.Value.Length);
                        w.Write(kv.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Manager > PersistFilterLayoutsToAppDataFile", ex.Message, ex);
            }
        }

        private void TryDeleteFilterLayoutsAppDataFile()
        {
            try
            {
                string path = Path.Combine(GetFilterLayoutsAppDataFolderPath(), FilterLayoutsPayloadFileName);
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Logger?.Error("Manager > TryDeleteFilterLayoutsAppDataFile", ex.Message, ex);
            }
        }

        private void SnapshotCurrentFilterTreeNodeLayoutToSettings()
        {
            if (trMain == null || gvGla == null || trMain.FocusedNode == null) return;
            var data = (DataRowView)trMain.GetDataRecordByNode(trMain.FocusedNode);
            if (data == null) return;
            string id = $"{data.Row.Field<string>("name")}";
            using (var stream = new MemoryStream())
            {
                gvGla.SaveLayoutToStream(stream);
                settings[id] = stream.ToArray();
            }
        }

        private void MergeCurrentFilterLayoutIntoSettingsBeforeClose()
        {
            if (!string.Equals((string)bbiLookUpFilter.EditValue, "Filter", StringComparison.CurrentCultureIgnoreCase))
                return;
            SnapshotCurrentFilterTreeNodeLayoutToSettings();
        }

        public Manager()
        {
            InitializeComponent();
        }
        SettingsService service;
        public Manager(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId)
        {       
            var startTime1 = DateTime.Now;
            FormName = definition.Name;
            // ovo smo morali ovdje za svaki slučaj setati jer je dolazilo do problema sa parametar formama koje su se pokretale
            // na init eventu master forme koji tek kasnije seta moduleId pa child forma nije mogla u loger postaviti @@moduleid
            this.ModuleId = moduleId;
            InitLogger(logger);
            this.DefinitionRoot = definitionRoot;
            this.DefinitionForm = definition;
            this.Logger = logger;
            this.ConnectionString = connectionString;

            service = new SettingsService(new SqlContext(connectionString));

            try
            {
                IsMaster = true;
                //DevExpress.Skins.SkinManager.DisableFormSkins();
                DateTime startTime = DateTime.Now;
                InitializeComponent();

                string menuIdEff = GetEffectiveMenuIdForFilterPostavke();
                try
                {
                    if (!string.IsNullOrEmpty(menuIdEff))
                    {
                        var data = service.LoadSettings(PubVars.OperateriId, menuIdEff);
                        if (data != null)
                        {
                            foreach (DataRow r in data.Rows)
                                settings[r.Field<string>("cvorid")] = r.Field<byte[]>("postavke");
                        }
                    }
                    else
                        MergeFilterLayoutsFromAppDataFileIntoSettings();
                }
                catch (Exception exLoad)
                {
                    Logger?.Error("Manager > LoadSettings (spCarpoFilterPostavke ctor)", exLoad.Message, exLoad);
                }

                if (!definition.exportButtonVisible)
                    bbiGridExport.Visibility = BarItemVisibility.Never;
                Logger.InfoData("Inicijalizacija manager forme", "Inicijalizacija manager forme", startTime);
                JSettings = new SettingsMaster();

                //bbiLog.EditValue = Properties.Resources.circle_gray_16;
                //repPicEditLog.ContextMenuStrip = new ContextMenuStrip(); //onemogućuje desni klik

                //Logger.Imginfo = bbiLog;

                if (SetDefinition() == false)
                    return;

                try
                {
                    Logic.Permissions.SetPermissions(this as Forms.Base.JupiterShared, moduleId);
                }
                catch (Exception ex)
                {
                    Logger.Error("Master > Manager", ex.Message, ex);
                }

                SetControlsVisibility();
                //DateTime startTime2 = DateTime.Now;
                SetEvents(DefinitionForm.Event);
                //Logger.InfoData("SetEvents manager forme", "Inicijalizacija manager forme", startTime2);
                DateTime startTime3 = DateTime.Now;
                SetMethods(this);
                // Logger.InfoData("SetMethods manager forme", "Inicijalizacija manager forme", startTime3);
                //DateTime startTime4 = DateTime.Now;
                SetControls();
                // Logger.InfoData("SetControls manager forme", "Inicijalizacija manager forme", startTime4);
                // DateTime startTime5 = DateTime.Now;
                InitControls();
                // Logger.InfoData("InitControls manager forme", "Inicijalizacija manager forme", startTime5);
                //DateTime startTime6 = DateTime.Now;
                SetManagerLayoutTabs();
                // Logger.InfoData("SetManagerLayoutTabs manager forme", "Inicijalizacija manager forme", startTime6);
                TranslateCaptions();
                Logger.InfoTest("ManagerCtor", "manager forma, translator, kontrole i eventovi", startTime1);
            }
            catch (Exception ex)
            {
                Logger.Error("event: InitForm", ex.Message, ex);
            }
        }

        string lastSelected = "";
        private void TrMain_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)//gruiranje
        {
            if (!string.Equals((string)bbiLookUpFilter.EditValue, "Filter", StringComparison.CurrentCultureIgnoreCase)) return;

            gvGla.ClearGrouping();

            var data = (DataRowView)e.Node.TreeList.GetDataRecordByNode(e.Node);
            var id = $"{data.Row.Field<string>("name")}";

            if (!settings.ContainsKey(id)) return;

            var layout = new MemoryStream(settings[id]);
            gvGla.RestoreLayoutFromStream(layout);
        }
        public void ResetSettings()
        {
            if (!string.Equals((string)bbiLookUpFilter.EditValue, "Filter", StringComparison.CurrentCultureIgnoreCase)) return;
            gvGla.ClearGrouping();
            settings.Clear();
            string menuIdEff = GetEffectiveMenuIdForFilterPostavke();
            if (!string.IsNullOrEmpty(menuIdEff))
                service.DeleteSettings(PubVars.OperateriId, menuIdEff);
            else
                TryDeleteFilterLayoutsAppDataFile();
        }

        #endregion InitForm

        #region Load

        private void TreeWithGrids_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;//zakomentirati kod js10

            if (!(string.IsNullOrEmpty(FormName)))
                this.Name = FormName;

            if (!DesignMode)
            {
                try
                {
                    PopulateControls();

                    PopulateControlList();

                    InitializeCustomControls();
                    trMain.FocusedNodeChanged += TrMain_FocusedNodeChanged;
                    gvGla.ColumnPositionChanged += GvGla_ColumnPositionChanged;
                    gvGla.ColumnFilterChanged += GvGla_ColumnFilterChanged;
                    gvGla.ColumnWidthChanged += GvGla_ColumnWidthChanged;
                }
                catch (Exception ex)
                {
                    JMessages.Error(ex, ex.Message);
                }
            }
        }

        private void GvGla_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            SaveSettings();
        }

        private void GvGla_ColumnFilterChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void GvGla_ColumnPositionChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void SaveSettings()
        {
            if (!string.Equals((string)bbiLookUpFilter.EditValue, "Filter", StringComparison.CurrentCultureIgnoreCase)) return;
            var data = (DataRowView)trMain.GetDataRecordByNode(trMain.FocusedNode);

            string id = "";
            if (data != null)
            {
                id = $"{data.Row.Field<string>("name")}";

                var stream = new MemoryStream();
                if (gvGla == null) return;
                gvGla.SaveLayoutToStream(stream);

                settings[id] = stream.ToArray();
            }
        }

        #endregion Load

        #region InitSettings

        private void SetControls()
        {
            if (MasterGridVisible == true && JSettings.GridMaster.Data.Count > 0)
                jDataMaster = JSettings.GridMaster.Data[0];

            pnlButtons.Init();
            pnlButtonsTree.Init();

            SetControlList();

            //****begin Grids
            if (MasterGridVisible == false)
            {
                gcGla.Visible = false;
                //pnlButtons.Parent = pnlButtonsTree;
                splitMain.PanelVisibility = SplitPanelVisibility.Panel1;
            }
            else
                gvGla.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;

            if (treeFilterVisible == false)
            {
                bbiLookUpFilter.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiTreeCaption.Caption = JSettings.TreeMaster.Caption;
            }
            else
            {
                bbiTreeCaption.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            if (TreeControlVisible == false)
            {
                trMain.Visible = false;
            }
            else
            {
                //trMain.OptionsView.ShowColumns = false;
                //if (JSettings.TreeMaster.DragNodes == true)
                //    trMain.DragNodes = true;
                if (MasterGridVisible == true)
                    trMain.ComponentExt.DetailControl = gcGla;
            }

            if (DetailGridVisible == false)
            {
                splitGrid.PanelVisibility = SplitPanelVisibility.Panel1;
            }
            else
            {
                if (MasterGridVisible == true)
                {
                    gcGla.ComponentExt.DetailControl = gcDet;
                    gcDet.ComponentExt.MasterControl = gcGla;
                }
            }

            if (TreeControlVisible == false)
                splitMain.PanelVisibility = SplitPanelVisibility.Panel2;

            //definira visinu toolbar-a; širina se zna poremetiti u designeru - zato iz inita;
            barEditFake.Width = 1;
            barEditIFakeRight.Width = 1;

            ////ToDo
            //pnlCmdIspisi.CustomName = "ispis";
            //pnlCmdIspisi.ButtonClick += new EventHandler(ButtonClick);
        }

        private void SetControlList()
        {
            if (TreeControlVisible == true && !string.IsNullOrEmpty(JSettings.TreeMaster.Name))
            {
                trMain.Name = JSettings.TreeMaster.Name.Trim();
                JControlList.Add(new Structure.Models.JControlDefinition() { JControl = trMain, CustomName = trMain.Name });
            }
            if (!string.IsNullOrEmpty(JSettings.GridMaster.Name))
            {
                gcGla.ComponentExt.lblCountCaption = cntMasterGrid;
                gcGla.ComponentExt.lblCount = bbiBrojSlogovaMasterGrid;
                gcGla.Name = JSettings.GridMaster.Name.Trim();
                bbiBrojSlogovaMasterGrid.Caption = cntMasterGrid.Replace("##", "0");
                JControlList.Add(new Structure.Models.JControlDefinition() { JControl = gcGla, CustomName = gcGla.Name });
            }
            if (DetailGridVisible == true && !string.IsNullOrEmpty(JSettings.GridDetail.Name))
            {
                gcDet.ComponentExt.lblCountCaption = cntDetailGrid;
                gcDet.ComponentExt.lblCount = bbiBrojSlogovaDetailsGrid;
                gcDet.Name = JSettings.GridDetail.Name.Trim();
                bbiBrojSlogovaDetailsGrid.Caption = cntDetailGrid.Replace("##", "0");
                JControlList.Add(new Structure.Models.JControlDefinition() { JControl = gcDet, CustomName = gcDet.Name });
            }
            if (treeFilterVisible == true && !string.IsNullOrEmpty(JSettings.ComboEdit.Name))
            {
                cmbComboEditFilter.Name = JSettings.ComboEdit.Name.Trim();
                JControlList.Add(new Structure.Models.JRepositoryDefinition() { JControl = cmbComboEditFilter, CustomName = cmbComboEditFilter.Name });
            }
        }

        private void PopulateControlList()
        {
            if (TreeControlVisible == true)
            {
                PopulateControlList(JSettings.TreeMaster.Name, trMain);
            }
            if (MasterGridVisible == true)
            {
                PopulateControlList(JSettings.GridMaster.Name, gcGla);
            }
            if (DetailGridVisible == true)
            {
                PopulateControlList(JSettings.GridDetail.Name, gcDet);
                PopulateTabGridsList(null, true);   // dodaj tab gridove u kontrol list u pravilnom redoslijedu
            }
        }

        private void PopulateControlList(string controlName, Control control)
        {
            bool isNameDefined = false;
            if (!string.IsNullOrEmpty(controlName))
                isNameDefined = true;
            ControlList.Add(new Structure.Models.JControl() { Control = control, ControlName = control.Name, ControlType = control.GetType(), IsNameDefinied = isNameDefined });
        }

        // dodao Tomislav: sada puni i tab gridove u logger... tempList - radi redoslijeda gridova u logger tablici
        private void PopulateTabGridsList(Control control, bool copyListToControlList = false)
        {
            try
            {
                if (copyListToControlList == false)
                    tempTabGridList.Add(new Structure.Models.JControl() { Control = control, ControlName = control.Name, ControlType = control.GetType() });
                else
                {
                    foreach (var tabGrid in tempTabGridList)
                    {
                        ControlList.Add(tabGrid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("method: PopulateTabGridsList", ex.Message);
            }
        }

        private void InitControls()
        {
            //if (TreeControlVisible == true)
            //    trMain.ComponentExt.Init();
        }

        private void InitializeCustomControls()
        {
            if (CustomControlsElement != null && CustomControlsElement.Count > 0)
            {
                foreach (var controlElement in CustomControlsElement)
                {
                    if ((!string.IsNullOrEmpty(controlElement.InitOnMaster)) && controlElement.InitOnMaster == "true")
                    {
                        using (var cls = new Carpo.UI.Logic.Layout.CustomControlInit())
                        {
                            cls.StartInit(controlElement, DateTime.Now, Logger);
                        }
                    }
                }
            }
        }

        #endregion InitSettings

        #region Data

        private void PopulateControls()
        {
            if (treeFilterVisible == true)
                PopulateTreeFilter();
            else if (TreeControlVisible == true)
                CheckTreelistAutoPopulate();
            else if (gcGla.ComponentExt.AutoPopulate != false)
                gcGla.ComponentExt.Populate(null, false, null, FocusType.Default);
        }

        private void PopulateTreeFilter()
        {
            cmbComboEditFilter.Definition = JSettings.ComboEdit;
            cmbComboEditFilter.CurrentForm = this;
            cmbComboEditFilter.Init();

            object value = cmbComboEditFilter.Items[0];

            if (cmbComboEditFilter.Definition.SaveValue == "app")
                SetTreeFilterValue(value);
            else
                bbiLookUpFilter.EditValue = value;
        }

        private void SetTreeFilterValue(object value)
        {
            var treeFilterValue = GetTreeFilterValue();

            if (treeFilterValue != null)
            {
                bool itemExist = cmbComboEditFilter.Definition.ComboItemList.Any(x => x.Value == treeFilterValue);

                if (itemExist)
                {
                    var item = cmbComboEditFilter.Definition.ComboItemList.First(x => x.Value == treeFilterValue);
                    bbiLookUpFilter.EditValue = item.Value;
                }
                else
                {
                    bbiLookUpFilter.EditValue = value;
                    Logger.Warn("Nepostojeći zapis u datasetu kontrole", "Kontrola '" + cmbComboEditFilter.Definition.Name +
                        "' u svome datasetu ne sadrži vrijednost '" + treeFilterValue + "'");
                }
            }
            else if (cmbComboEditFilter.Definition.ComboItemList.Count > 0)
                bbiLookUpFilter.EditValue = cmbComboEditFilter.Definition.ComboItemList[0].Value;
        }

        private string GetTreeFilterValue() // nije objekt jer combo vraća string vrijednost
        {
            string val = "";
            string sqlText;
            DateTime startTime = DateTime.Now;
            try
            {
                DataSet ds = Carpo.Data.Set.Vars.GetAppControlValues(ConnectionString, Core.PubVars.OperateriId, ModuleId, FormName, out sqlText);
                //DataSet ds = Carpo.Data.Get.Vars.GetBySource(ConnectionString, Core.PubVars.OperateriId, ModuleId, FormName, out sqlText);
                Logger.InfoData("data reader - punjenje vrijednosti filtera drveta; CarpoVars", sqlText, startTime);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dataRow = (from row in ds.Tables[0].AsEnumerable()
                                   where row.Field<string>("ComponentName") == cmbComboEditFilter.Definition.Name
                                   select row).FirstOrDefault();
                    return val = dataRow["ValueText"].ToString();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logger.Error("SetTreeFilterValue", ex.Message, ex);
                return "";
            }
        }

        #endregion Data

        #region TreeControls

        private void bbiTreeExpand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (isTreeCollapsed)
            {
                trMain.ExpandAll();
                isTreeCollapsed = false;
            }
            else
            {
                trMain.CollapseAll();
                isTreeCollapsed = true;
            }
        }

        #endregion TreeControls

        #region SetDefinition

        private bool SetDefinition()
        {
            JForm jFormMaster = DefinitionRoot.Form.FirstOrDefault(x => string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase));
            if (jFormMaster != null)
            {
                try
                {
                    if (jFormMaster.ManagerLayout.LookUpList != null)
                        JSettings.ComboEdit = jFormMaster.ManagerLayout.LookUpList.FirstOrDefault();
                    if (jFormMaster.ManagerLayout.TreeList != null && jFormMaster.ManagerLayout.TreeList.Count > 0)
                        JSettings.TreeMaster = jFormMaster.ManagerLayout.TreeList.FirstOrDefault();

                    if (jFormMaster.ManagerLayout.Tabs.Count > 0) // ako postoji barem jedna tab definicija idem tamo
                    {
                        if (!SetTabSettings(jFormMaster, jFormMaster.ManagerLayout.Tabs.Count))
                            return false;
                    }
                    else
                    {
                        var gridMaster = jFormMaster.ManagerLayout.GridList.Where(x => string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (gridMaster == null)
                        {
                            if (JSettings.TreeMaster == null)
                            {
                                Common.Helpers.JMessages.Warning(@"Nisu pronađeni master grid ni drvo!");
                                return false;
                            }
                        }
                        else
                        {
                            JSettings.GridMaster = gridMaster;

                            var tabsDetail = jFormMaster.ManagerLayout.GridList.Where(x => !string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (tabsDetail != null)
                                JSettings.GridDetail = tabsDetail;
                        }
                    }

                    if (jFormMaster.ActionPanel != null && jFormMaster.ActionPanel.ActionItems != null && jFormMaster.ActionPanel.ActionItems.Count > 0)
                        JSettings.PanelButtons = jFormMaster.ActionPanel;
                    else
                        JSettings.PanelButtons = jFormMaster.PanelButtons;

                    this.DefinitionForm = jFormMaster;
                    this.Text = Common.Translator.TranslateString(jFormMaster.Caption);
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, "SetDefinition()");
                }
            }
            else
            {
                Common.Helpers.JMessages.Warning(@"Master forma nije pronađena. Dodijelite formi atribut 'type=""master""'");
                return false;
            }

            return true;
        }

        private bool SetTabSettings(JForm jFormMaster, int tabsCount)
        {
            try
            {
                if (tabsCount == 1)
                {
                    // master tab + detalj grid
                    var tabsMaster = jFormMaster.ManagerLayout.Tabs.Where(x => string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (tabsMaster != null)
                    {
                        SetMasterTab(tabsMaster);

                        var gridDetail = jFormMaster.ManagerLayout.GridList.Where(x => x.Type == null).FirstOrDefault();
                        if (gridDetail != null)
                            JSettings.GridDetail = gridDetail;
                    }
                    else // mastergrid + detalj tab
                    {
                        var masterGrid = jFormMaster.ManagerLayout.GridList.Where(x => string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (masterGrid != null)
                        {
                            JSettings.GridMaster = masterGrid;

                            var tabsDetail = jFormMaster.ManagerLayout.Tabs.Where(x => string.IsNullOrEmpty(x.Type)).FirstOrDefault();
                            if (tabsDetail != null)
                                SetDetailTab(tabsDetail);
                        }
                        else
                        {
                            Common.Helpers.JMessages.Warning(@"U xml-u nisu pronađeni mastergrid ni mastertab");
                            return false;
                        }
                    }
                }
                else if (tabsCount == 2)
                {
                    var tabsMaster = jFormMaster.ManagerLayout.Tabs.Where(x => string.Equals(x.Type, "master", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (tabsMaster != null)
                    {
                        // setam master tab
                        SetMasterTab(tabsMaster);

                        //setam detalj tab
                        var tabsDetail = jFormMaster.ManagerLayout.Tabs.Where(x => string.IsNullOrEmpty(x.Type)).FirstOrDefault();
                        if (tabsDetail != null)
                            SetDetailTab(tabsDetail);
                    }
                    else
                    {
                        Common.Helpers.JMessages.Warning(@"U xml-u nije pronađen tab container sa atributom 'type=""master""'");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, "SetTabSettings");
                return false;
            }
        }

        private void SetMasterTab(JTabs tabsMaster)
        {
            JSettings.TabsMaster = tabsMaster;
            for (int i = 0; i < JSettings.TabsMaster.TabList.Count; i++)
            {
                if (JSettings.TabsMaster.TabList[i].Grid != null)
                {
                    JSettings.GridMaster = JSettings.TabsMaster.TabList[i].Grid;
                    break;
                }
            }
        }

        private void SetDetailTab(JTabs tabsDetail)
        {
            JSettings.TabsDetail = tabsDetail;
            for (int i = 0; i < JSettings.TabsDetail.TabList.Count; i++)
            {
                if (JSettings.TabsDetail.TabList[i].Grid != null)
                {
                    JSettings.GridDetail = JSettings.TabsDetail.TabList[i].Grid;
                    break;
                }
            }
        }

        private void SetControlsVisibility()
        {
            if (JSettings.Forms != null)
            {
                DefinitionRoot.Form.AddRange(JSettings.Forms);
            }

            if (JSettings.ComboEdit == null || (JSettings.ComboEdit.Data == null && JSettings.ComboEdit.Data == null && JSettings.ComboEdit.ComboItemList == null))
                treeFilterVisible = false;

            if (JSettings.TreeMaster == null || JSettings.TreeMaster.Data == null)
            {
                TreeControlVisible = false;
                bbiSideBar.Visibility = BarItemVisibility.Never;
            }

            if (JSettings.GridDetail == null || JSettings.GridDetail.Data == null)
            {
                DetailGridVisible = false;
            }

            if (JSettings.GridMaster == null || JSettings.GridMaster.Data == null)
            {
                MasterGridVisible = false;
            }

            if (TreeControlVisible == true)
                trMain.SetControl(JSettings.TreeMaster);
            if (MasterGridVisible == true)
                gcGla.SetControl(JSettings.GridMaster);
            if (DetailGridVisible == true)
                gcDet.SetControl(JSettings.GridDetail);
            if (JSettings.PanelButtons != null)
            {
                pnlButtons.Definition = JSettings.PanelButtons;
                pnlButtonsTree.Definition = JSettings.PanelButtons;
            }

            SetTreeEditing();
        }

        private void TranslateCaptions()
        {
            // Raširi / Sažmi
            bbiTreeExpand.Caption = Common.Translator.TranslateString("Raširi / Sažmi");
            bbiTreeExpand.Tag = Common.Translator.TranslateString("Raširi / Sažmi");
            // Sakrij / Prikaži side-bar
            bbiSideBar.Caption = Common.Translator.TranslateString("Sakrij / Prikaži Side Bar");
            bbiSideBar.Tag = Common.Translator.TranslateString("Sakrij / Prikaži Side Bar");
            // Raširi / Sažmi
            bbiGridExpand.Caption = Common.Translator.TranslateString("Raširi / Sažmi");
            bbiGridExpand.Tag = Common.Translator.TranslateString("Raširi / Sažmi");
            // Resetiraj grid
            bbiGridReset.Caption = Common.Translator.TranslateString("Resetiraj grid");
            bbiGridReset.Tag = Common.Translator.TranslateString("Resetiraj grid");
            // Postavke
            barSubItem1.Caption = Common.Translator.TranslateString("Postavke");
            barSubItem1.Tag = Common.Translator.TranslateString("Postavke");
            //  * Prikaži sumu grupa
            barSubItem2.Caption = Common.Translator.TranslateString("Prikaži sumu grupa");
            //      ** Suma ispod grupe
            bbiOptionGroupSumUnder.Caption = Common.Translator.TranslateString("Suma ispod grupe");
            //      ** Suma inline
            bbiOptionGroupSumInline.Caption = Common.Translator.TranslateString("Suma inline");
            //  * prikaži red sveukupno
            bbiOptionSum.Caption = Common.Translator.TranslateString("Prikaži red sveukupno");
            // Export
            bbiGridExport.Caption = Common.Translator.TranslateString("Export");

            // Broj slogova
            bbiBrojSlogova.Caption = Common.Translator.TranslateString("Broj slogova:");
        }

        private void SetManagerLayoutTabs()
        {
            SetTabControl(JSettings.TabsMaster, splitGrid.Panel1, false);
            SetTabControl(JSettings.TabsDetail, splitGrid.Panel2, true);
        }

        private void SetTabControl(JTabs jTabs, SplitGroupPanel panel, bool isDetail)
        {
            try
            {
                if (jTabs == null)
                    return;

                tabControl = new JXtraTabControl(jTabs);

                if (jTabs.Event != null)
                {
                    var onSelectedPageChanged = jTabs.Event.Where(x => x.Type.Contains("selectedtabchanged")).FirstOrDefault();
                    tabControl.SelectedPageChanged += (s, e) =>
                    {
                        using (Logic.Events ev = new Logic.Events(onSelectedPageChanged, null, this))
                        {
                            ev.RunNotification();
                        }
                    };
                }

                panel.Controls.Add(tabControl);

                int index = 0;

                foreach (var tab in jTabs.TabList)
                {
                    bool checkIfResult = true;
                    if (Conditions.CheckCondition(ref checkIfResult, tab, this, null) == false)
                        continue;

                    JGridControl gridControl;
                    if (jTabs.TabList.Count == 1)
                    {
                        tabControl.Visible = false;
                        gridControl = SetTabGrid(panel, jTabs.TabList[0].Grid, index == 0, isDetail);
                    }
                    else
                    {
                        DevExpress.XtraTab.XtraTabPage tabPage = tabControl.CreateTabPage(tab);
                        gridControl = SetTabGrid(tabPage, tab.Grid, index == 0, isDetail);
                        tabControl.TabPagesBugWorkAround(tabPage);
                        gridControl.ComponentExt.tabPage = tabPage;
                        gridControl.ComponentExt.tabPageCaption = Carpo.Common.Translator.TranslateString(tab.Caption);

                        if (isDetail)
                            gridControl.ComponentExt.MasterControl = gcGla;
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SetTabControl", ex.Message, ex);
            }
            //tabControl.TabPagesBugWorkAround();
        }

        private JGridControl SetTabGrid(Control parentControl, JGrid gridDefinition, bool mainGrid, bool isDetail)
        {
            try
            {
                JGridControl grid;
                if (mainGrid == true)
                {
                    grid = isDetail ? gcDet : gcGla;
                }
                else
                {
                    grid = CreateGrid(gridDefinition);
                    JControlList.Add(new Structure.Models.JControlDefinition() { JControl = grid, CustomName = gridDefinition.Name });
                    if (!isDetail)
                        additionalMasterTabList.Add(gridDefinition.Name);
                    else
                        additionalDetailTabList.Add(gridDefinition.Name);
                    PopulateTabGridsList(grid);
                }
                parentControl.Controls.Add(grid);
                return grid;
            }
            catch (Exception ex)
            {
                Logger.Error("SetTabGrid", ex.Message, ex);
                return null;
            }
        }

        private JGridControl CreateGrid(JGrid gridDefinition)
        {
            JGridControl grid = new JGridControl(gridDefinition);
            grid.Dock = DockStyle.Fill;
            return grid;
        }

        #endregion SetDefinition

        #region SaveLayoutToXml

        private void TreeWithGrids_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                Xml jRootXml = new Xml(AssemblyFolder, AssamblyName);
                jRootXml.ExportXml(DefinitionRoot);
            }
        }

        #endregion SaveLayoutToXml

        private void bbiGridRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcGla.ComponentExt.GridExpand();
        }

        #region BarManager/Items

        private void bbiSideBar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (splitMain.Panel1.Visible)
            {
                splitMain.PanelVisibility = SplitPanelVisibility.Panel2;
                bbiSideBar.Glyph = Properties.Resources.right_16;
            }
            else
            {
                splitMain.PanelVisibility = SplitPanelVisibility.Both;
                bbiSideBar.Glyph = Properties.Resources.left_16;
            }
        }

        string previous = "";
        MemoryStream nonFilterLayout = new MemoryStream();

        private void bbiLookUpFilter_EditValueChanged(object sender, EventArgs e)//promjena
        {
            if (string.Equals(previous, "Filter", StringComparison.CurrentCultureIgnoreCase))
            {
                gvGla.ClearGrouping();
                //gcGla.ComponentExt.ForceRestoreLayout();
                var array = nonFilterLayout.ToArray();
                nonFilterLayout.Seek(0, SeekOrigin.Begin);
                if (array.Length != 0) gvGla.RestoreLayoutFromStream(nonFilterLayout);

            }
            else if (!string.Equals(previous, "Filter", StringComparison.CurrentCultureIgnoreCase) && gvGla.Columns.Count > 0)
            {
                nonFilterLayout.SetLength(0);
                nonFilterLayout.Position = 0;
                gvGla.SaveLayoutToStream(nonFilterLayout);
            }

            //if (string.Equals((string)bbiLookUpFilter.EditValue, "Filter", StringComparison.CurrentCultureIgnoreCase)) TODO15: dodaj da se loada grid
            object value = bbiLookUpFilter.EditValue;
            SetControlValue(cmbComboEditFilter.Name, value);

            //var it = list.Find(x => x.JControl.Name == "filter")
            //var texti = JControlList..Select(item => item.ToString);

            //set treeListsearchfield
            var itemDefinition = cmbComboEditFilter.itemList.Where(x => x.Value.ToString().ToLower() == value.ToString().ToLower()).FirstOrDefault();
            if (itemDefinition != null && itemDefinition.Definition != null)
            {
                string searchField = itemDefinition.Definition.SearchField;
                trMain.SetSearchField(searchField);
            }
            string itemName = bbiLookUpFilter.EditValue.ToString();

            var valuechanged = JSettings.ComboEdit.Event.Where(x => x.Type == "valuechanged").FirstOrDefault();
            if (valuechanged != null)
            {
                cmbComboEditFilter.EventEditValueChanged();
            }

            trMain.ComponentExt.ParameterControlValue = itemName;
            CheckTreelistAutoPopulate();

            previous = (string)bbiLookUpFilter.EditValue;
        }

        private void CheckTreelistAutoPopulate()
        {
            if (trMain.ComponentExt.AutoPopulate != false)
                trMain.ComponentExt.PopulateTreeList(false, false, trMain.Definition != null && trMain.Definition.Async == true);
        }

        #region GridOptions

        public void HandleBbiCheck(BarCheckItem bbi, bool value)
        {
            // kada je ifexpanded bbi je null
            if (bbi == null)
            {
                bbiOptionGroupSumUnder.CheckedChanged -= this.bbiOptionGroupSumUnder_CheckedChanged;
                bbiOptionGroupSumUnder.Checked = value;
                bbiOptionGroupSumUnder.CheckedChanged += this.bbiOptionGroupSumUnder_CheckedChanged;
                bbiOptionGroupSumInline.CheckedChanged -= this.bbiOptionGroupSumInline_CheckedChanged;
                bbiOptionGroupSumInline.Checked = value;
                bbiOptionGroupSumInline.CheckedChanged += this.bbiOptionGroupSumInline_CheckedChanged;
            }
            else if (bbi.Name == "bbiOptionGroupSumInline")
            {
                bbi.CheckedChanged -= this.bbiOptionGroupSumInline_CheckedChanged;
                bbi.Checked = value;
                bbi.CheckedChanged += this.bbiOptionGroupSumInline_CheckedChanged;

                if (value)
                {
                    var bbi2 = bbiOptionGroupSumUnder;
                    UncheckOtherBBI(bbi2);
                }
            }
            else if (bbi.Name == "bbiOptionGroupSumUnder")
            {
                bbi.CheckedChanged -= this.bbiOptionGroupSumUnder_CheckedChanged;
                bbi.Checked = value;
                bbi.CheckedChanged += this.bbiOptionGroupSumUnder_CheckedChanged;

                if (value)
                {
                    var bbi2 = bbiOptionGroupSumInline;
                    UncheckOtherBBI(bbi2);
                }
            }
        }

        private void UncheckOtherBBI(BarCheckItem bbi)
        {
            if (bbi.Name == "bbiOptionGroupSumUnder")
            {
                bbi.CheckedChanged -= this.bbiOptionGroupSumUnder_CheckedChanged;
                bbi.Checked = false;
                bbi.CheckedChanged += this.bbiOptionGroupSumUnder_CheckedChanged;
            }
            else if (bbi.Name == "bbiOptionGroupSumInline")
            {
                bbi.CheckedChanged -= this.bbiOptionGroupSumInline_CheckedChanged;
                bbi.Checked = false;
                bbi.CheckedChanged += this.bbiOptionGroupSumInline_CheckedChanged;
            }
        }

        private void bbiOptionSum_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcGla.ComponentExt.GridOptionSum();
        }

        public void bbiOptionGroupSumInline_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcGla.ComponentExt.GridGroupSumsInline();
        }

        public void bbiOptionGroupSumUnder_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcGla.ComponentExt.GridGroupSumsUnder();
        }

        private void gvGla_EndGrouping(object sender, EventArgs e)
        {
            if (gvGla.GroupCount > 0)
                bbiGridExpand.Enabled = true;
            else
                bbiGridExpand.Enabled = false;
        }

        private void bbiGridExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (e.Item.Tag != null)
            {
                string ext = e.Item.Tag as string;
                gcGla.ComponentExt.GridExport(ext);
            }
        }

        #endregion GridOptions

        #endregion BarManager/Items

        #region Buttons

        private void ButtonClick(object sender, EventArgs e)
        {
            //switch (((UserControls.ButtonMaster)sender).CustomName)
            //{
            //    case "ispis":
            //        Edit.Standard frm = new Forms.Edit.Standard(((Base.JupiterShared)FindForm()).ConnectionString);
            //        frm.ConnectionString = ConnectionString;
            //        frm.ShowDialog();
            //        break;
            //    default:
            //        break;
            //}
        }

        #endregion Buttons

        private void Manager_FormClosing(object sender, FormClosingEventArgs e)//zatvaranje i spremanje
        {
            MergeCurrentFilterLayoutIntoSettingsBeforeClose();

            string menuIdEff = GetEffectiveMenuIdForFilterPostavke();
            if (!string.IsNullOrEmpty(menuIdEff))
            {
                foreach (var item in settings)
                    service.SaveSettings(item.Key, PubVars.OperateriId, menuIdEff, item.Value);
            }
            else if (settings.Count > 0)
                PersistFilterLayoutsToAppDataFile();

            using (var cls = new Carpo.UI.Logic.Layout.CustomControlInit())
            {
                cls.DisposeCustomControls();
            }

            try
            {
                // Tree-based managers (e.g. Djelatnosti) have no grid definition; guard every dereference so
                // closing the form never logs an NRE for layout that does not exist.
                var glaDef = gcGla?.Definition;
                var detDef = gcDet?.Definition;
                Logic.VarsLayout layout = new Logic.VarsLayout(this, glaDef?.LayoutVersion);
                if (string.Equals("Filter", bbiLookUpFilter?.EditValue as string, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (gvGla != null)
                        layout.SaveXml(gvGla, "grid - manager gla", null, glaDef?.PreviousLayoutName, nonFilterLayout);
                    return;
                }

                if (splitGrid.Panel1.Visible == true)
                {
                    if (glaDef?.SaveLayout == true && (gcGla.Views.Count > 0 ? gcGla.Views[0] as GridView : null)?.Columns.Count > 0)
                    {
                        if (!(string.IsNullOrEmpty(glaDef.ClearGridFilter)) && glaDef.ClearGridFilter.ToLower() == "true")
                        {
                            try
                            {
                                gvGla.ClearColumnsFilter();
                            }
                            catch (Exception)
                            { }
                        }
                        layout.SaveXml(gvGla, "grid - manager gla", null, glaDef.PreviousLayoutName, nonFilterLayout);
                    }

                    // ovdje tab gridove mastera
                    try
                    {
                        foreach (var additionalTab in JControlList)
                        {
                            var tabobj = additionalTab as JControlDefinition;
                            if (tabobj != null)
                                if (additionalMasterTabList.Contains(tabobj.CustomName))
                                {
                                    layout.SaveXml((tabobj.JControl as DevExpress.XtraGrid.GridControl).Views[0], tabobj.CustomName);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Manager_FormClosing", "Problem sa spremanjem layouta grida : " + ex.Message, ex);
                    }
                }

                if (splitGrid.Panel2.Visible == true)
                {
                    if (detDef?.SaveLayout == true && (gcDet.Views.Count > 0 ? gcDet.Views[0] as GridView : null)?.Columns.Count > 0)
                    {
                        if (!(string.IsNullOrEmpty(detDef.ClearGridFilter)) && detDef.ClearGridFilter.ToLower() == "true")
                        {
                            try
                            {
                                gvDet.ClearColumnsFilter();
                            }
                            catch (Exception)
                            { }
                        }
                        layout.SaveXml(gvDet, "grid - manager det", null, glaDef?.PreviousLayoutName);
                    }

                    // ovdje tab gridove detalja
                    try
                    {
                        foreach (var additionalTab in JControlList)
                        {
                            var tabobj = additionalTab as JControlDefinition;
                            if (tabobj != null)
                                if (additionalDetailTabList.Contains(tabobj.CustomName))
                                {
                                    layout.SaveXml(tabobj.JControl, tabobj.CustomName);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Manager_FormClosing", "Problem sa spremanjem layouta grida : " + ex.Message, ex);
                    }
                }
                if (splitMain.PanelVisibility == SplitPanelVisibility.Both)
                    layout.SaveValue(splitMain.Name, splitMain.SplitterPosition, "spliter - manager");

                if (splitGrid.PanelVisibility == SplitPanelVisibility.Both)
                    layout.SaveValue(splitGrid.Name, splitGrid.SplitterPosition, "spliter - grids");

                if (cmbComboEditFilter?.Definition != null && cmbComboEditFilter.Definition.SaveValue == "app")
                {
                    string sqlText = "";
                    Carpo.Data.Set.Vars.SetAppControlValues(ConnectionString, Core.PubVars.OperateriId, ModuleId, FormName,
                        cmbComboEditFilter.Definition.Name, "", "String", bbiLookUpFilter?.EditValue?.ToString() ?? string.Empty, out sqlText);
                    Logger.InfoData("Spremanje vrijednosti kontroli '" + cmbComboEditFilter.Definition.Name + "'", sqlText, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Neuspješno spremanje layout grida", ex.Message, ex);
            }
        }

        #region Onemogući PopupMenu na toolbaru

        private void barManager1_ShowToolbarsContextMenu(object sender, ShowToolbarsContextMenuEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void barManager2_ShowToolbarsContextMenu(object sender, ShowToolbarsContextMenuEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        #endregion Onemogući PopupMenu na toolbaru

        private void chkTreeReorder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTreeReorder.Checked)
                SetTreeEditing(true, "chk");
            else
                SetTreeEditing(false, "chk");
        }

        private void SetTreeEditing(bool isEditMode, string sender)
        {
            pnlTreeSpec.Visible = isEditMode;
            bool allowSelfDrag = trMain.ComponentExt.AllowSelfDrag;

            trMain.ComponentExt.SelfDragging = (isEditMode == true && allowSelfDrag == true);

            if (isEditMode == true && allowSelfDrag == true)
            {
                if (trMain.ComponentExt.AllowDragMultiple == true)
                {
                    trMain.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
                    trMain.OptionsSelection.MultiSelect = true;
                }
                else
                {
                    trMain.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
                    trMain.OptionsSelection.MultiSelect = false;
                }
            }
            else
            {
                trMain.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.None;
            }

            SetTreeStatusBar(!isEditMode);

            //splitMain.Panel2.Enabled = false;
            if (isEditMode == true)
                splitMain.PanelVisibility = SplitPanelVisibility.Panel1;
            else
                splitMain.PanelVisibility = SplitPanelVisibility.Both;

            if (isEditMode == true)
                trMain.ComponentExt.Action = "reordering";
            else
            {
                trMain.ComponentExt.Action = "";
                if (sender == "chk")
                    trMain.ComponentExt.FocusedNodeChanged(false);
            }
        }

        public void RePopulateTreeFilter()
        {
            cmbComboEditFilter.PopulateItems();
        }

        private void SetTreeEditing()
        {
            if (TreeControlVisible == true)
            {
                if (MasterGridVisible == false && DetailGridVisible == false)
                {
                    chkTreeReorder.Visible = false;
                    SetTreeEditing(true, "");
                }
            }
        }

        private void SetTreeStatusBar(bool visible)
        {
            bbiBrojSlogova.Visibility = GetBarItemVisibilityState(visible);
            bbiBrojSlogovaMasterGrid.Visibility = GetBarItemVisibilityState(visible);
            bbiBrojSlogovaDetailsGrid.Visibility = GetBarItemVisibilityState(visible);
        }

        private BarItemVisibility GetBarItemVisibilityState(bool visible)
        {
            if (visible == true)
                return BarItemVisibility.Always;
            return BarItemVisibility.Never;
        }

        private void bbiGridReset_ItemClick(object sender, ItemClickEventArgs e)
        {
            gcGla.ComponentExt.GridReset();
        }

        private async void Manager_Shown(object sender, EventArgs e)
        {
            await Task.Delay(200);
            this.Opacity = 100;
        }
    }
}