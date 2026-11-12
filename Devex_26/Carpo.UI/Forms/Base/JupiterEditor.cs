using Carpo.Core;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components;
using Carpo.UI.Forms.Edit;
using Carpo.UI.Logic;
using Carpo.UI.Logic.Layout;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Export;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Carpo.UI.Forms.Base
{
    public partial class JupiterEditor : JupiterShared //, IJFormJupiterEditor
    {
        private bool disableMaximizeBoxClick = false;
        public List<LayoutControl> LayoutControlList { get; set; } = new List<LayoutControl>();
        public List<Components.JCustomUC> CustomControls { get; set; } = new List<Components.JCustomUC>();
        //public static List<Components.JCustomUC> CustomControls { get; set; } = new List<Components.JCustomUC>();

        //čuva inicijalnu veličinu forme
        //ako se, pri izlasku, veličina forme promijenila, vrijednost se sprema u CarpoVars
        //sprječavanje spremanja veličine forme > saveformsize="false" u form elementu
        public Size InitFormSize { get; set; }

        // služi za provjeravanje atributa type="collapsed" koji se definira na layoutu u splitter formi (DevEx dozvoljava samo 1 collapsabilni panel)
        public int collapsedPanelCount = 0;

        public JupiterEditor()
        {
            InitializeComponent();
            //SetEvents(DefinitionForm.Event);
        }

        //direktan poziv
        public JupiterEditor(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId)
        {
            FormName = definition.Name;

            ModuleId = moduleId;

            DefinitionRoot = definitionRoot;
            BeforeInit(definition, logger, connectionString);
            InitializeComponent();

            SetFooter("masterForm");

            IsMaster = true;
            SetEvents(definition.Event);
            SetMethods(this);

            
        }

        //sub form
        public JupiterEditor(Forms.Base.JupiterShared form, JForm definition)
        {
            FormName = definition.Name;

            this.CarpoVarsSession = form.CarpoVarsSession; //proslijedi parametre
            this.CarpoVarsApp = form.CarpoVarsApp;
            this.VariablesGlobal = form.VariablesGlobal; //proslijedi globalne varijable
            this.PermissionList = form.PermissionList; //proslijedi permission
            this.ModuleId = form.ModuleId;
            BeforeInit(definition, form.Logger, form.ConnectionString);
            InitializeComponent();

            SetFooter("");

            //this.StartPosition = FormStartPosition.CenterParent;
            SetMethods(FormsVar.GetActiveForm(form));
            SetControlAppValues();


        }

        private void SetFooter(string form)
        {
            if (form != "masterForm")
            {
                bbiVerzija.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                bbiLog.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                if (DefinitionForm.StatusBar != true)
                    statusBar.Visible = false;
            }

            bbiBrojSlogova.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bbiBrojSlogovaMasterGrid.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bbiBrojSlogovaDetailsGrid.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            bbiLabel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!(string.IsNullOrEmpty(FormName)))
                this.Name = FormName;

            if (!DesignMode)
            {
                SetForm();
                //ako su kontrole u tabovima handle ne može biti kreiran prije load eventa!
                //layoutControl.BeginUpdate();
                try
                {
                    foreach (Structure.Models.JControl c in ControlList.Where(x => x.Control.Visible == false))
                    {
                        if (!c.Control.IsHandleCreated)
                            c.Control.Visible = true;
                    }
                }
                catch
                { }

                try
                {
                    foreach (LayoutControl layoutControl in LayoutControlList)
                    {
                        layoutControl.EndUpdate();
                    }
                    LayoutControlList[0].TabIndex = 0;
                }
                catch
                { }

                try
                {
                    SetFormSize(true);
                    base.OnLoad(e);
                }
                catch
                { }

                try
                {
                    var searchControls = ControlList.Where(ctrl => ctrl.ControlType == typeof(Components.JSearchControl));
                    if (searchControls != null)
                    {
                        foreach (var control in searchControls)
                        {
                            (control.Control as Components.JSearchControl).ComponentExt.SetValue(control.Control as Components.JSearchControl);
                        }
                    }
                }
                catch { }

                //nekada bilo unutar SetFormSize
                //zbog blicanja premiješteno na kraj load forme
                if (DefinitionForm != null && DefinitionForm.FixedSize == true)
                {
                    disableMaximizeBoxClick = true;
                    this.MinimumSize = this.MaximumSize = new Size(this.Size.Width, this.Size.Height);
                    //this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    //this.MaximizeBox = false;
                    //this.MinimizeBox = false;
                }
                //if (DefinitionForm.Maximized != true)
                SetFormSize(false); //za određivanje minimum veličine
                this.StartPosition = FormStartPosition.CenterParent;

            }
        }

        //protected override void OnShown(EventArgs e)
        //{
        //    //    int i = 0;

        //    //    if (CusControlList != null)
        //    //    {
        //    //        foreach (JLayout jLayout in this.DefinitionForm.Layout)
        //    //        {
        //    //            ControlInit controlInit = new ControlInit(this, LayoutControlList[i], jLayout, ConnectionString, CusControlList);
        //    //            {
        //    //                controlInit.CreateCustomControls();
        //    //                i++;
        //    //            }
        //    //        }
        //    //    }
        //}

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x0112)
        //    {
        //        if ((m.WParam.ToInt32() & 0xFFF0) == 0xF030 && DefinitionForm.FixedSize == true)
        //            return;
        //    }
        //    base.WndProc(ref m);
        //}

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (IsMaster)
            {
                using (var cls = new Carpo.UI.Logic.Layout.CustomControlInit())
                {
                    cls.DisposeCustomControls();
                }
            }

            try
            {
                if (DefinitionForm.SaveFormSize != false && this.Size != InitFormSize)
                {
                    Logic.VarsLayout layout = new Logic.VarsLayout(this);
                    string formSize = layout.GetFormSizeStr();
                    if (!string.IsNullOrEmpty(formSize))
                    {
                        var srcFrm = this as Carpo.UI.Forms.Search;

                        if (srcFrm == null)
                            layout.SaveValue("formSize", formSize, "form");
                        else if (srcFrm != null)
                            layout.SaveValue(srcFrm.ComponentName, formSize, "grid - search", true);
                    }
                }
            }
            catch { }

            try
            {
                var saveValueControlsSession = this.ControlList.Where(x => x.SaveValueSession == true);
                var saveValueControlsApp = this.ControlList.Where(x => x.SaveValueApp == true);

                foreach (var item in saveValueControlsApp)
                {
                    //var itemSaved = CarpoVarsApp.FirstOrDefault(x => x.FormName == this.Name && x.Name == item.ControlName);
                    if (item.Control is IJEditor)
                    {
                        string value = "";
                        string valueText = "";
                        System.Type valueType = typeof(string);

                        if (item.Control is JButtonControl || item.Control is JButtonEditControl || item.Control is JDaysEditControl
                            || item.Control is JLabelEditControl || item.Control is JGridControl || item.Control is JTreeListControl)
                            continue;

                        if ((item.Control as IJEditor).GetValue("") != null)
                            value = (item.Control as IJEditor).GetValue("").ToString();
                        else
                            value = "null";

                        if (item.Control is JCheckEditControl || item.Control is JRadioEditControl)
                            valueType = typeof(bool);
                        else if (item.Control is JSearchControl)
                        {
                            var control = item.Control as JSearchControl;
                            valueText = control.GetValueText();
                            valueType = typeof(int);
                        }
                        else if (item.Control is JLookUpControl)
                        {
                            var control = item.Control as JLookUpControl;
                            valueType = (control.ComponentExt.ValueFieldColumnType);
                        }
                        else if (item.Control is JAutoSuggestControl)
                        {
                            var control = item.Control as JAutoSuggestControl;
                            valueType = (control.ComponentExt.ValueFieldColumnType);
                        }
                        else if (item.Control is JTreeLookUpControl)
                        {
                            var control = item.Control as JTreeLookUpControl;
                            valueType = (control.ComponentExt.ValueFieldColumnType);
                        }
                        else if (item.Control is JDateEditControl)
                            valueType = typeof(DateTime);

                        string sqlText = "";
                        Carpo.Data.Set.Vars.SetAppControlValues(this.ConnectionString, Core.PubVars.OperateriId, this.ModuleId, this.FormName, item.ControlName,
                            value, valueType.Name, valueText, out sqlText);
                        Logger.InfoData("Spremanje vrijednosti kontroli '" + item.ControlName + "'", sqlText, DateTime.Now);
                    }
                }
                CarpoVarsApp.Clear();

                foreach (var item in saveValueControlsSession)
                {
                    var itemSaved = CarpoVarsSession.FirstOrDefault(x => x.FormName == this.Name && x.Name == item.ControlName);
                    if (item.Control is IJEditor)
                    {
                        string text = null;
                        if (item.Control is JSearchControl)
                            text = (item.Control as JSearchControl).GetValueText();

                        if (itemSaved == null)
                            CarpoVarsSession.Add(new Structure.Models.Vars() { FormName = this.Name, Name = item.ControlName, ValueObj = (item.Control as IJEditor).GetValue(""), ValueText = text });
                        else
                        {
                            itemSaved.ValueObj = (item.Control as IJEditor).GetValue("");
                            itemSaved.ValueText = text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("JupiterEditor: OnFormClosing", ex.Message, ex);
            }
        }

        //protected override void OnShown(form e)
        //{
        //    base.OnFormClosing(e);

        //    try
        //    {
        //        if (DefinitionForm.SaveFormSize != false && this.Size != InitFormSize)
        //        {
        //            Logic.VarsLayout layout = new Logic.VarsLayout(this);
        //            string formSize = layout.GetFormSizeStr();
        //            if (!string.IsNullOrEmpty(formSize))
        //                layout.SaveValue("formSize", formSize, "form");
        //        }
        //    }
        //    catch { }

        //}

        private void BeforeInit(JForm definition, Logger logger, string connectionString)
        {
            this.DoubleBuffered = true;
            //this.SetStyle(ControlStyles.UserPaint |
            //              ControlStyles.AllPaintingInWmPaint |
            //              ControlStyles.ResizeRedraw |
            //              ControlStyles.ContainerControl |
            //              ControlStyles.OptimizedDoubleBuffer |
            //              ControlStyles.SupportsTransparentBackColor
            //              , true);

            this.InitLogger(logger);
            this.ConnectionString = connectionString;
            this.DefinitionForm = definition;

            Logic.Permissions.PermissionsAreSet = false;
            Logic.Permissions.SetPermissions(this, this.ModuleId);


            SetControlsLayout();
        }

        private void SetControlsLayout()
        {
            try
            {
                VarsLayout varsLayout = new VarsLayout(this);
                varsLayout.SetLayout();
            }
            catch (Exception ex)
            {
                Logger.Error("JupiterEditor: SetControlLayout", ex.Message, ex);
            }
        }

        public void SetForm()
        {
            if (this is Forms.Edit.Splitter && DefinitionForm.SplitterPosition != null)
                (this as Edit.Splitter).splitter1.SplitterPosition = (int)DefinitionForm.SplitterPosition;
        }

        public void SetControlAppValues(string moduleId = "")
        {
            string modId = moduleId;
            if (string.IsNullOrEmpty(moduleId))
                modId = this.ModuleId;

            try
            {
                string sqlText = "";
                DataTable dt = Carpo.Data.Set.Vars.GetAppControlValues(this.ConnectionString, Core.PubVars.OperateriId, modId, this.FormName, out sqlText).Tables[0];
               // Logger.InfoData("Dohvat spremljenih vrijednosti kontrola", sqlText, DateTime.Now);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        object value = null;
                        if (dt.Rows[i]["Value"] != DBNull.Value)
                            value = dt.Rows[i]["Value"];

                        if (value != null && !(string.IsNullOrEmpty(value.ToString())))
                        {
                            switch (dt.Rows[i]["ValueType"].ToString())
                            {
                                case "String":
                                    value = value.ToString();
                                    break;

                                case "Int32":
                                    value = Convert.ToInt32(value);
                                    break;

                                case "DateTime":
                                    value = Convert.ToDateTime(value);
                                    break;

                                case "Boolean":
                                case "bool":
                                    value = Convert.ToBoolean(value);
                                    break;
                            }
                        }

                        CarpoVarsApp.Add(new Structure.Models.Vars()
                        {
                            FormName = dt.Rows[i]["Form"].ToString(),
                            Name = dt.Rows[i]["ComponentName"].ToString(),
                            ValueObj = value,
                            ValueText = dt.Rows[i]["ValueText"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("JupiterEditor: SetControlAppValues", ex.Message, ex);
            }
        }

        public void InitializeCustomControls()
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

        public void SetLayout()
        {
            //layoutControl.BeginUpdate();
            IsSettingLayout = true;

            //ControlHelper.SuspendRedraw(layoutControl);

            try
            {
                int index = 0;
                foreach (JLayout jLayout in this.DefinitionForm.Layout)
                {
                    CustomLayoutControl layoutControl = new CustomLayoutControl();

                    layoutControl.BeginUpdate();

                    layoutControl.CustomDrawBackground += (s, e) =>
                    {
                        layoutControl_CustomDrawBackground(s, e);
                    };

                    LayoutControlList.Add(layoutControl);

                    InitLayout(layoutControl, jLayout, index);

                    ControlInit controlInit = new ControlInit(this, layoutControl, jLayout, ConnectionString);
                    controlInit.CreateLayout();

                    index += 1;
                }
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                IsSettingLayout = false;
            }

            if (this.DefinitionForm.PanelButtons == null && this.DefinitionForm.ActionPanel == null)
                pnlButtons.Visible = false;
            else
            {
                if (DefinitionForm.ActionPanel != null)
                    pnlButtons.Definition = this.DefinitionForm.ActionPanel;
                else
                    pnlButtons.Definition = this.DefinitionForm.PanelButtons;

                pnlButtons.Init(false);
            }
        }

        private void InitLayout(LayoutControl layoutControl, JLayout jLayout, int index)
        {
            try
            {
                if (jLayout.FocusDirection == "downthenacross")
                    layoutControl.OptionsFocus.MoveFocusDirection = MoveFocusDirection.DownThenAcross;

                if (jLayout.AutoTabOrder == false)
                    layoutControl.OptionsFocus.EnableAutoTabOrder = false;

                layoutControl.AllowCustomization = false;

                layoutControl.OptionsFocus.AllowFocusTabbedGroups = false;
                layoutControl.OptionsFocus.AllowFocusControlOnActivatedTabPage = true;

                layoutControl.Root.EnableIndentsWithoutBorders = DefaultBoolean.True;
                layoutControl.Root.TextVisible = false;
                layoutControl.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(4);
                layoutControl.Root.GroupBordersVisible = false;

                layoutControl.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
                layoutControl.Root.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                layoutControl.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();

                //if (string.IsNullOrEmpty(jLayout.Type))
                //{
                if (this is Forms.Edit.NavBar)
                {
                    if (jLayout.Name == "panel1")
                        (this as Forms.Edit.NavBar).navBarGroupContainer1.Controls.Add(layoutControl);
                    else
                        (this as Forms.Edit.NavBar).splitter1.Panel2.Controls.Add(layoutControl);
                }
                else if (this is Forms.Edit.Splitter)
                {
                    Forms.Edit.Splitter spliterForm = this as Forms.Edit.Splitter;

                    if (jLayout.Name == "panel1")
                        spliterForm.splitter1.Panel1.Controls.Add(layoutControl);
                    else if (jLayout.Name == "panel2")
                        spliterForm.splitter1.Panel2.Controls.Add(layoutControl);

                    if (jLayout.Type == "collapsable")
                    {
                        if (!string.IsNullOrEmpty(jLayout.Name))
                        {
                            if (jLayout.Name == "panel1")
                            {
                                spliterForm.splitter1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
                                collapsedPanelCount++;
                            }
                            else if (jLayout.Name == "panel2")
                            {
                                spliterForm.splitter1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
                                collapsedPanelCount++;
                            }
                        }
                    }

                    if (collapsedPanelCount > 1)
                        Logger.Warn("event: InitLayout", "Samo jedan layout može biti collapsabilan; ", "");

                    if (DefinitionForm.Template.ToLower().Contains("vertical"))
                        spliterForm.splitter1.Horizontal = false;

                    if (DefinitionForm.SplitterFixed == true)
                        spliterForm.splitter1.IsSplitterFixed = true;

                    //SetPanelsMinSize(jLayout, spliterForm);             prebačeno na SetFormSize zbog 'anomalija' kada se postavi MinSize u inicijalizaciji svakog panela
                }
                else if (this is Forms.Edit.Standard)
                {
                    this.Controls.Add(layoutControl);
                }
                else if (this is Forms.Info || this is Forms.Edit.SideBar)
                {
                    SetTab(this as JupiterInfo, layoutControl, jLayout, index);
                }

                layoutControl.BringToFront();
                layoutControl.Dock = DockStyle.Fill;
            }
            catch (Exception)
            {

                throw;
            }


            

            //}
        }

        private void SetTab(JupiterInfo jupiterInfoForm, LayoutControl layoutControl, JLayout jLayout, int index)
        {
            // PERMISSION
            bool permissionEnabled = true;
            bool permissionVisible = true;
            if (!string.IsNullOrEmpty(jLayout.Function))
            {
                for (int i = 0; i < PermissionList.Count; i++)
                {
                    if (PermissionList[i].Function == jLayout.Function)
                    {
                        permissionEnabled = PermissionList[i].Enabled;
                        permissionVisible = PermissionList[i].Visible;
                        break;
                    }
                }
            }

            





            DevExpress.XtraTab.XtraTabPage page = new DevExpress.XtraTab.XtraTabPage();
            page.Text = jLayout.Caption;
            if (jupiterInfoForm is SideBar)
            {
                (jupiterInfoForm as SideBar).tabs.TabPages.Add(page);
            } 
            else if (jupiterInfoForm is Info)
            {
                (jupiterInfoForm as Info).tabs.TabPages.Add(page);
            }

            page.Controls.Add(layoutControl);
            page.Show();

            if (permissionVisible == false)
            {
                jupiterInfoForm.TabPagesVisible.Add(index);
            }
            else if (permissionEnabled == false)
            {
                jupiterInfoForm.TabPagesEnabled.Add(index);
            }
        }

        private void SetPanelsMinSize(Forms.Edit.Splitter spliterForm)
        {
            if (DefinitionForm.Layout == null)
                return;

            foreach (JLayout jLayout in DefinitionForm.Layout)
            {
                if (jLayout.Name != null)
                {
                    if (jLayout.MinSize != null)
                    {
                        if (jLayout.Name.ToLower() == "panel2")
                            spliterForm.splitter1.Panel2.MinSize = (int)jLayout.MinSize;
                        else if (jLayout.Name.ToLower() == "panel1")
                            spliterForm.splitter1.Panel1.MinSize = (int)jLayout.MinSize;
                    }
                    else
                    {
                        if (jLayout.Name.ToLower() == "panel2")
                        {
                            if (spliterForm.splitter1.Horizontal == true)
                                spliterForm.splitter1.Panel2.MinSize = LayoutControlList[1].Root.MinSize.Width;
                            else
                                spliterForm.splitter1.Panel2.MinSize = LayoutControlList[1].Root.MinSize.Height;
                        }
                        else if (jLayout.Name.ToLower() == "panel1")
                        {
                            if (spliterForm.splitter1.Horizontal == true)
                                spliterForm.splitter1.Panel1.MinSize = LayoutControlList[0].Root.MinSize.Width;
                            else
                                spliterForm.splitter1.Panel1.MinSize = LayoutControlList[0].Root.MinSize.Height;
                        }
                    }
                }
            }
        }

        public void SetFormSize(bool beforeLoad)
        {
            if (this is Forms.SearchDoc)
                return;

            // postavi minimalnu veličinu panela u spliter formi
            if (this is Forms.Edit.Splitter)
            {
                Forms.Edit.Splitter spliterForm = (this as Forms.Edit.Splitter);
                SetPanelsMinSize(spliterForm);
            }

            #region RacunicaVisina

            int height;
            int minimumHeight = 100;
            int minimumHeightReal = 0;
            if (LayoutControlList.Count != 0)
            {
                if (this is Forms.Edit.Splitter)
                {
                    Forms.Edit.Splitter spliterForm = (this as Forms.Edit.Splitter);

                    if (spliterForm.splitter1.Horizontal == false)
                    {
                        height = this.Height;
                        foreach (LayoutControl lc in LayoutControlList)
                        {
                            height -= lc.Height;
                            minimumHeightReal += lc.Root.MinSize.Height;
                        }
                        minimumHeightReal += height;

                        if (minimumHeightReal > minimumHeight)
                            minimumHeight = minimumHeightReal;
                    }
                    else if (spliterForm.splitter1.Horizontal == true)
                    {
                        int maxInt = 0;
                        int tempMinSize = 0;
                        for (int i = 0; i < LayoutControlList.Count; i++)
                        {
                            if (LayoutControlList[i].Root.MinSize.Height > tempMinSize)
                            {
                                tempMinSize = LayoutControlList[i].Root.MinSize.Height;
                                maxInt = i;
                            }
                        }

                        height = this.Height - LayoutControlList[maxInt].Height;
                        minimumHeight = LayoutControlList[maxInt].Root.MinSize.Height + height;
                    }
                }
                else
                {
                    height = this.Height - LayoutControlList[0].Height;
                    minimumHeight = LayoutControlList[0].Root.MinSize.Height + height;

                    //if (DefinitionForm.PanelButtons == null)
                    //    minimumHeight -= pnlButtons.Height;
                }

                if (DefinitionForm.Type == "master" || DefinitionForm.StatusBar == true)
                    minimumHeight += GetStatusBarSize();

                if (DefinitionForm.PanelButtons == null && DefinitionForm.ActionPanel == null)
                    minimumHeight -= pnlButtons.Height;
            }

            //if (afterLoad == false)
            //{
            if (DefinitionForm != null && DefinitionForm.Height != null)
                height = (int)DefinitionForm.Height;
            //this.Height = (int)DefinitionForm.Height;
            else
                height = minimumHeight;
            //this.Height = minimumHeight;

            //}

            #endregion RacunicaVisina

            #region RacunicaSirina

            int width;
            int minimumWidth = 320;
            int minimumWidthReal = -1;

            if (LayoutControlList.Count != 0)
            {
                if (this is Forms.Edit.Splitter)
                {
                    minimumWidthReal = 0;
                    Forms.Edit.Splitter spliterForm = (this as Forms.Edit.Splitter);

                    if (spliterForm.splitter1.Horizontal == true)
                    {
                        width = this.Width;
                        foreach (LayoutControl lc in LayoutControlList)
                        {
                            width -= lc.Width;
                            minimumWidthReal += lc.Root.MinSize.Width;
                        }
                        minimumWidthReal += width;
                    }
                    else if (spliterForm.splitter1.Horizontal == false)
                    {
                        int maxInt = 0;
                        int tempMinSize = 0;
                        for (int i = 0; i < LayoutControlList.Count; i++)
                        {
                            if (LayoutControlList[i].Root.MinSize.Width > tempMinSize)
                            {
                                tempMinSize = LayoutControlList[i].Root.MinSize.Width;
                                maxInt = i;
                            }
                        }
                        width = this.Width - LayoutControlList[maxInt].Width;
                        minimumWidthReal = LayoutControlList[maxInt].Root.MinSize.Width + width;
                    }
                }
                else
                {
                    width = this.Width - LayoutControlList[0].Width;
                    minimumWidthReal = LayoutControlList[0].Root.MinSize.Width + width;
                }
                if (minimumWidthReal > minimumWidth)
                    minimumWidth = minimumWidthReal;
            }

            //if (afterLoad == false)
            //{
            if (DefinitionForm != null && DefinitionForm.Width != null && DefinitionForm.Width > minimumWidthReal)
                if ((int)DefinitionForm.Width < minimumWidthReal)
                    //this.Width = minimumWidthReal;
                    width = minimumWidthReal;
                else
                    width = (int)DefinitionForm.Width;
            //this.Width = (int)DefinitionForm.Width;
            else
                width = minimumWidth;
            //this.Width = minimumWidth;
            //}

            #endregion RacunicaSirina

            bool varSizeExists = false;
            bool varMaximized = false;

            if (!(this is Carpo.UI.Forms.Search))
            {
                if (DefinitionForm.SaveFormSize != false)
                {
                    VarsLayout layout = new VarsLayout(this);
                    var formSize = layout.SetFormSize();

                    if (formSize != null)
                    {
                        if (formSize is string && formSize.ToString() == "maximized")
                        {
                            varMaximized = true;
                        }
                        else if (formSize is Size)
                        {
                            varSizeExists = true;
                            this.Size = (Size)formSize;
                        }
                    }
                }

                if (varSizeExists == false)
                    this.Size = new Size(width, height);

                if (beforeLoad == true)
                {
                    int minimumW = minimumWidthReal;
                    if (minimumW < 0)
                        minimumW = minimumWidth;

                    this.MinimumSize = new Size(minimumWidth, minimumHeight);
                }

                if (beforeLoad == false)
                {
                    if (varMaximized == true || (DefinitionForm != null && DefinitionForm.Maximized == true))
                    {
                        this.WindowState = FormWindowState.Maximized;
                    }
                    this.InitFormSize = this.Size;
                }
            }
        }

        public bool Validation()
        {
            Form form = FindForm();
            if (form is Forms.Base.JupiterEditor)
            {
                return dxValidationProvider1.Validate();
            }
            return true;
        }

        public void layoutControl_CustomDrawBackground(object sender, Components.CustomDrawEventArgs e)
        {
            if (e.Args.Owner.GetType() == typeof(DevExpress.XtraLayout.LayoutControlGroup))
            {
                Color color = (e.Args.Owner as DevExpress.XtraLayout.LayoutControlGroup).AppearanceGroup.BackColor;
                //if ((e.Args.Owner as LayoutControlGroup).AppearanceGroup.BackColor
                //if ((e.Args.Owner as DevExpress.XtraLayout.LayoutControlGroup).AppearanceGroup.BackColor == Color.White)
                //{
                Rectangle bounds = ((GroupObjectInfoArgs)(e.Args.BorderInfo)).ClientBounds;
                e.Args.Graphics.FillRectangle(new SolidBrush(color), bounds);
                e.Handled = true;
                //}
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //if (e.KeyCode == Keys.F8)
            //{
            //    Xml jRootXml = new Xml(AssemblyFolder, AssemblyName);
            //    jRootXml.ExportXml(DefinitionForm);
            //}
        }

        // dodano zbog problema sa fixedsize="true". MimimizeBox, MaximizeBox i FixedSingle su stvarali problem
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == 0x0112 && msg.WParam == new IntPtr(0xF030) && disableMaximizeBoxClick) // SC_MAXIMIZE
                return;

            base.WndProc(ref msg);
            //this.MinimizeBox
        }

        //protected override void WndProc(ref Message msg)
        //{
        //    if (msg.Msg == 0x0112)
        //    {
        //        if ((msg.WParam.ToInt32() & 0xFFF0) == 0xF030) // SC_MAXIMIZE
        //        {
        //            if (disableMaximizeBoxClick)
        //                return;
        //        }
        //        else if ((msg.WParam.ToInt32() & 0xFFF0) == 0xF020) // SC_MINIMIZE
        //        {
        //            if (!this.IsMaster)
        //            {
        //                //MinimizeFormTest(this.Owner);
        //                return;
        //            }
        //        }
        //    }

        //    base.WndProc(ref msg);
        //}

        //private void MinimizeFormTest(Form ownerForm)
        //{
        //    if ((ownerForm as Carpo.UI.Forms.Edit.Standard).IsMaster)
        //    {
        //        //this.WindowState = FormWindowState.Minimized;
        //        ownerForm.WindowState = FormWindowState.Minimized;
        //    }
        //    else
        //        MinimizeFormTest(ownerForm);
        //}

        private void JupiterEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ControlList.Count > 0)
            {
                foreach (JControl c in ControlList)
                {
                    if (c.Control is Carpo.UI.Components.JGridControl)
                    {
                        Carpo.UI.Components.JGridControl gridControl = c.Control as Carpo.UI.Components.JGridControl;

                        Logic.VarsLayout layout = new Logic.VarsLayout(this, gridControl.Definition.LayoutVersion);
                        if (gridControl.Definition.SaveLayout == true)
                        {
                            if ((gridControl.Views[0] as DevExpress.XtraGrid.Views.Grid.GridView).Columns.Count != 0)
                            {
                                if (!(string.IsNullOrEmpty(gridControl.Definition.ClearGridFilter)) && gridControl.Definition.ClearGridFilter.ToLower() == "true")
                                {
                                    try
                                    {
                                        (gridControl.Views[0] as DevExpress.XtraGrid.Views.Grid.GridView).ClearColumnsFilter();
                                    }
                                    catch (Exception)
                                    {
                                        Logger.Warn(gridControl.Definition.Name + " - ClearColumnsFilter", "Greška čišćenja filtera grida");
                                    }
                                }
                                layout.SaveXml(gridControl.Views[0], "grid - layout", null, gridControl.Definition.LayoutName);
                            }
                        }
                    }
                }
            }
        }

        private int GetStatusBarSize()
        {
            int i = 0;
            try
            {
                barManagerMain.ForceInitialize();
                FieldInfo fi = statusBar.GetType().GetField("barControl", BindingFlags.NonPublic | BindingFlags.Instance);
                DevExpress.XtraBars.Controls.CustomBarControl barControl = fi.GetValue(statusBar) as DevExpress.XtraBars.Controls.CustomBarControl;
                if (barControl != null)
                    i = barControl.Height;
            }
            catch { }

            return i;
        }

        private void JupiterEditor_Load(object sender, EventArgs e)
        {

        }
    }
}