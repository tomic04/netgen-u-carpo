using Carpo.Structure.Components;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using Carpo.UI.Logic;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components.Base
{
    public class ComponentBase
    {
        protected Control control;
        protected SimpleLabelItem simpleLabelItem;
        protected BaseEdit baseEdit;
        protected GridView gridView;
        protected TreeList treeList;
        protected XtraTabControl tabs;
        protected JEvent eventValueChanged;
        protected JEvent focusedRowChanged;
        protected JEvent selectionChanged;
        public JEvent cellValueChanged;
        public JEvent eventDragOver; //obrađujem u ComponentTree klasi (za sada)
        public JEvent eventDragOverSelf; //obrađujem u ComponentTree klasi (za sada)
        public JEvent eventBeforeDragNode; //obrađujem u ComponentTree klasi (za sada)
        public JEvent eventRowClick;
        public JEvent eventShowingEditor;

        public bool? PermissionEnabled { get; set; } = null;
        public bool? PermissionVisible { get; set; } = null;

        protected string connectionString;
        public   Forms.Base.JupiterShared form { get; set; }
        protected Structure.Models.Core.Base.JModelEditorDataBase definition;

        private int menuSort = 1; //sort za menuItem
        private bool comboMultiHelperReturn = false;

        protected Dictionary<string, object> propertyList = new Dictionary<string, object>();

        public string KeyCode = "";

        public string Action { get; set; } = "";
        public string Action2 { get; set; } = ""; //za grid -- dvije akcije paralelno
        public string ActionEvent { get; set; } = "";
        public bool DesignMode { get; private set; }

        public bool? DefaultVisibility { get; set; } = null; //radi panel buttonsa;

        public bool? PreventTabChanging { get; set; } = false;   

        public ComponentBase()
        {
        }

        public ComponentBase(Control control, object def)
        {
            if (!DesignMode)
            {
                this.control = control;
                if (control is BaseEdit)
                    baseEdit = control as BaseEdit;

                if (control is GridControl)
                    gridView = (control as GridControl).MainView as GridView;

                if (control is TreeList)
                   treeList = (control as TreeList) as TreeList;

                if (control is XtraTabControl)
                    tabs = (control as XtraTabControl) as XtraTabControl;

                this.form = (Forms.Base.JupiterShared)control.FindForm();
                this.connectionString = form.ConnectionString;

                //if (control is BaseEdit)
                //    baseEdit = (control as BaseEdit);

                SetDefinition(def);
                Init();
            }
        }

        private void SetPermissionVars()
        {
            if (definition == null)
                return;

            if (string.IsNullOrEmpty(definition.Function))
                return;

            string formName = "";
            if (form.DefinitionForm != null && !string.IsNullOrEmpty(form.DefinitionForm.Name))
                formName = form.DefinitionForm.Name;
            else
                return;

            //var permission = form.PermissionList.Where(x => x.Function == definition.Function && x.FormName == formName).FirstOrDefault();
            //var permission = form.PermissionList.Where(x => x.Function == definition.Function && x.FormName == formName).FirstOrDefault();
            var permission = form.PermissionList.Where(x => x.Function.ToLower() == definition.Function.ToLower()).FirstOrDefault();
            //var permission = form.PermissionList.Where(x => (x.Function ?? string.Empty).ToLower().Contains((definition.Function ?? string.Empty).ToLower()));

            if (permission != null)
            {
                PermissionEnabled = permission.Enabled;
                PermissionVisible = permission.Visible;
            }
        }

        public ComponentBase(LayoutControl layoutControl, SimpleLabelItem simpleLabelItem, object def)
        {
            this.simpleLabelItem = simpleLabelItem;

            this.form = (layoutControl.FindForm() as Forms.Base.JupiterShared);
            this.connectionString = form.ConnectionString;

            //SetDefinition(def);
            Init();
        }

        #region SetProperties

        private void SetDefinition(object def)
        {
            if (def is JModelEditorDataBase)
            {
                definition = def as JModelEditorDataBase;

                if (!string.IsNullOrEmpty(definition.ToolTip))
                {
                    if (baseEdit != null)
                        baseEdit.ToolTip = definition.ToolTip;
                    else if (this is ComponentButton)
                        (control as JButtonControl).ToolTip = definition.ToolTip;
                }

                //SetDefinition(definition);
            }
        }

        //public void SetDefinition(JModelEditorDataBase def)
        //{
        //    if (definition.Properties == null || definition.Properties.Count == 0)
        //        return;
        //    foreach (JModelEditorDataBase editorDef in definition.Properties)
        //    {
        //        SetDefinitionProperty(editorDef);
        //    }
        //}

        //private void SetDefinitionProperty(JModelEditorDataBase editorDef)
        //{
        //    Logic.Statemants statemants = new Logic.Statemants(editorDef.If, form, null);
        //    if (statemants.CheckIf() == true)
        //    {
        //        // Get property array
        //        var properties = GetProperties(editorDef);

        //        foreach (var p in properties)
        //        {
        //            try
        //            {
        //                string name = p.Name;
        //                var value = p.GetValue(editorDef, null);
        //                if (value != null)
        //                {
        //                    Type type = definition.GetType();
        //                    PropertyInfo pinfo = type.GetProperty(p.Name);

        //                    if (pinfo != null)
        //                    {
        //                        pinfo.SetValue(definition, value, null);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                form.Logger.Error("Method: SetDefinitionProperty", ex.Message, ex);
        //            }
        //        }
        //    }
        //}

        //private static PropertyInfo[] GetProperties(object obj)
        //{
        //    return obj.GetType().GetProperties();
        //}

        ////private void GetPropertyValue(string propertyName)
        ////{
        ////    if (definition == null)
        ////        return;

        ////    if (definition.Properties != null)
        ////    {
        ////        foreach (JModelEditorDataBase editorDef in definition.Properties)
        ////        {
        ////            if (definition.
        ////            Logic.Statemants statemants = new Logic.Statemants(editorDef.If, form, null);
        ////            if (statemants.CheckIf() == true)
        ////            {
        ////            }
        ////        }
        ////    }
        ////}

        #endregion SetProperties

        private void Init()
        {
            SetPermissionVars();

            if (control != null)
            {
                if (!DesignMode)
                    InitControl();
            }
            else if (simpleLabelItem != null)
                InitSimpleLabel();

            string visibleStr = "true";
            if (!string.IsNullOrEmpty(definition?.Visible))
            {
                using (Parser parser = new Parser(form, null))
                {
                    visibleStr = parser.GetLogicPartValue(definition.Visible).ToString();
                }
            }

            if (visibleStr == "false" || PermissionVisible == false)
            {
                DefaultVisibility = false;
                SetVisibility(false);
            }
        }

        private void InitControl()
        {

            if (definition == null)
                return;

            DateTime startTime = DateTime.Now;
            if (DesignMode)
                return;

            if (definition.EnterMoveNextControl == false)
                baseEdit.EnterMoveNextControl = false;
            else if ((baseEdit != null && !(control is ButtonEdit) && !(control is MemoEdit)) ||
                (control is JHyperLinkEditControl))
                baseEdit.EnterMoveNextControl = true;
            else if (control is MemoEdit && definition.EnterMoveNextControl == true)
                baseEdit.EnterMoveNextControl = true;

            if (!string.IsNullOrEmpty(definition.Name))
                control.Name = definition.Name;
            else
                control.Name = Guid.NewGuid().ToString();

            if (definition.Enabled == false || PermissionEnabled == false)
                control.Enabled = false;

            //if (definition.Visible == false || PermissionVisible == false)
            //    control.Visible = false;

            if (!string.IsNullOrEmpty(definition.FontColor))
            {
                if (baseEdit != null)
                    baseEdit.Properties.Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(definition.FontColor, baseEdit.Properties.Appearance.ForeColor);
                else if (control is JLabelEditControl)
                    (control as JLabelEditControl).Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(definition.FontColor, (control as JLabelEditControl).Appearance.ForeColor);
            }

            if (definition.FontBold == true)
            {
                if (baseEdit != null)
                    baseEdit.Properties.Appearance.Font = new Font(baseEdit.Font, FontStyle.Bold);
                else if (control is JLabelEditControl)
                    (control as JLabelEditControl).Appearance.Font = new Font((control as JLabelEditControl).Appearance.Font, FontStyle.Bold);
            }

            if (definition.FontSizeDelta != null)
            {
                if (baseEdit != null)
                    baseEdit.Properties.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;
                else if (control is JLabelEditControl)
                    (control as JLabelEditControl).Appearance.FontSizeDelta = (int)definition.FontSizeDelta;
            }

            //readonliy
            if (definition.ReadOnly == true && baseEdit != null)
            {
                baseEdit.Properties.ReadOnly = true;
                baseEdit.TabStop = false;
            }

            if (definition.TabStop == false)
            {
                if (baseEdit != null)
                    baseEdit.TabStop = false;
                else
                    control.TabStop = false;
            }

            if (!string.IsNullOrEmpty(definition.TabIndex)) // neće vrijediti ako na layoutu nije isključen 'autotaborder'
            {
                int n;
                bool isNumeric = int.TryParse(definition.TabIndex, out n);
                if (isNumeric)
                {
                    if (baseEdit != null)
                        baseEdit.TabIndex = n;
                    else
                        control.TabIndex = n;
                }
            }

            //širina kontrole (label, button i baseedit)
            if (definition.Width != null)
            {
                if (control is JLabelEditControl)
                    (control as JLabelEditControl).Width = (int)definition.Width;
                else if (baseEdit != null)
                    baseEdit.Width = (int)definition.Width;
            }
            else if (control is JButtonControl)
            {
                control.Width = 90;
            }

            SetValidation();

            //form.Logger.InfoData("Init base kontrole", definition.Name, startTime);
        }

        

        private void InitSimpleLabel()
        {
            if (!string.IsNullOrEmpty(definition.Name))
                simpleLabelItem.Name = definition.Name;
            else
                simpleLabelItem.Name = Guid.NewGuid().ToString();

            if (definition.Enabled == false)
                simpleLabelItem.Enabled = false;
        }

        #region validate

        private void SetValidation(bool attributeRequiredChanged = false)
        {
            if (!(string.IsNullOrEmpty(definition.Required)) && form is Forms.Base.JupiterEditor)
            {
                DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 =
                        new Carpo.Common.Helpers.Validator.MyConditionValidationRule();

                if (definition.Required.ToLower() == "true")
                {
                    //control.BackColor = Color.FromArgb(255,255,200); //pozadinska boja; obevezan unos
                    conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
                    conditionValidationRule1.ErrorText = Common.Translator.TranslateString("Obavezan unos");
                    conditionValidationRule1.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                }
                else
                {
                    //control.BackColor = Color.FromArgb(255,255,200); //pozadinska boja; obevezan unos
                    conditionValidationRule1.ConditionOperator = 0;
                    conditionValidationRule1.ErrorText = null;
                    conditionValidationRule1.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
                    //form.Controls
                }

                if (attributeRequiredChanged)
                    SetItemCaption(null, true);

                (form as Forms.Base.JupiterEditor).dxValidationProvider1.SetValidationRule(control, conditionValidationRule1);
            }
        }

        #endregion validate

        #region CreateEvents

        public void SetEvents(JEvent jEvent)
        {
            if (jEvent == null)
                return;
            List<JEvent> events = new List<JEvent>() { jEvent };
        }

        public virtual void SetEvents(List<JEvent> events)
        {
            if (events == null || events.Count == 0)
                return;

            cellValueChanged = events.Where(x => x.Type.StartsWith("cellvaluechanged")).FirstOrDefault();
            if (cellValueChanged != null)
            {
                OnCellValueChanged();
            }

            var valueChanged = events.Where(x => x.Type.StartsWith("valuechanged")).FirstOrDefault();
            if (valueChanged != null)
            {
                this.eventValueChanged = valueChanged;
                if (!(control is JSearchControl)) //search control ide direkt preko ValuChanged
                    OnValueChanged(valueChanged);
            }

            focusedRowChanged = events.Where(x => x.Type.Contains("focusedrowchanged")).FirstOrDefault();
            if (focusedRowChanged != null)
            {
                OnFocusedRowChanged();
            }

            selectionChanged = events.Where(x => x.Type.Contains("selectionchanged")).FirstOrDefault();
            if (selectionChanged != null)
            {
                OnSelectionChanged();
            }

            eventShowingEditor = events.Where(x => x.Type.Contains("showingeditor")).FirstOrDefault();

            var doubleclick = events.Where(x => x.Type.Contains("doubleclick")).FirstOrDefault();
            if (doubleclick != null)
            {
                OnDoubleClick(doubleclick);
            }
            var doublecltree = events.Where(x => x.Type.Contains("doublecltree")).FirstOrDefault();
            if (doublecltree != null)
            {
                OnDoubleClickTree(doublecltree);
            }

            var checkedChanged = events.Where(x => x.Type.Contains("checkedchanged")).FirstOrDefault();
            if (checkedChanged != null)
            {
                OnCheckedChanged(checkedChanged);
            }

            var leave = events.Where(x => x.Type.Contains("leave")).FirstOrDefault();
            if (leave != null)
                OnLeave(leave);

            var click = events.Where(x => x.Type.StartsWith("click")).FirstOrDefault();
            if (click != null)
                OnClick(click);


            var tabChanging = events.Where(x => x.Type.StartsWith("tabchanging")).FirstOrDefault();
            if (tabChanging != null)
                OnTabChanging(tabChanging);

            var tabChaned = events.Where(x => x.Type.StartsWith("tabchanged")).FirstOrDefault();
            if (tabChaned != null)
                OnTabChanged(tabChaned);


            var keydown = events.Where(x => x.Type.StartsWith("keydown")).FirstOrDefault();
            if (keydown != null)
                OnKeyDown(keydown);

            eventRowClick = events.Where(x => x.Type.Contains("rowclick")).FirstOrDefault();
            if (eventRowClick != null)
                OnRowClick(eventRowClick);

            eventDragOver = events.Where(x => x.Type.Contains("dragover")).FirstOrDefault();
            eventDragOverSelf = events.Where(x => x.Type.Contains("dragoverself")).FirstOrDefault();
            eventBeforeDragNode = events.Where(x => x.Type.Contains("dragnode")).FirstOrDefault();
        }

        private void OnDoubleClickTree(JEvent doubleclicktree)
        {
            treeList.DoubleClick += (s, e) =>
            {
                using (Logic.Events ev = new Logic.Events(doubleclicktree, control, form))
                {
                    ev.RunNotification();
                }
            };

            




        }

        //izdvojeno radi searc kontrole
        protected void ValueChanged()
        {
            // dodano zbog buga koji devexova kontrola CheckedComboBoxEdit ima da okida 2x valuechanged
            // V.P. zakomentirao posto se dva puta okidalo samo kada se kontrola praznila - praznila se sa postavljanjem na null sto je okidalo dva puta valuechanged
            //if (control is JComboMultiEditControl && comboMultiHelperReturn == true)
            //{
            //    comboMultiHelperReturn = false;
            //    return;
            //}

            try
            {
                if (eventValueChanged != null)
                {
                    if (eventValueChanged.RaiseOnLoad == false && form.IsLoadEvent == true)
                        return;

                    object value = null;
                    if (control is JSearchControl)
                    {
                        if ((form as Forms.Info)?.SearchType == Forms.Info.SearchTypeEnums.Default)
                        {
                            return;
                        }

                        //value = (control as JSearchControl).ComponentExt.Id;
                        value = (control as JSearchControl).DataRowRef;
                        if (value == null) //edititarnje
                            value = (control as JSearchControl).ComponentExt.Id;
                    }
                    else if (control is JDaysEditControl)
                        value = (control as JDaysEditControl).txtDan.Text;
                    else if (control is JAutoSuggestControl)
                    {
                        if ((form as Forms.Info).SearchType == Forms.Info.SearchTypeEnums.Popup)
                        {
                            return;
                        }

                        //value = (control as JSearchControl).ComponentExt.Id;
                        if ((control as JAutoSuggestControl).EditValue is DataRowView) {
                            value = ((control as JAutoSuggestControl).EditValue as DataRowView).Row[0];
                        }
                    }
                    else if (baseEdit != null)
                        value = baseEdit.EditValue;

                    Logic.Events jEvent = new Logic.Events(eventValueChanged, control, form, value);
                    jEvent.RunNotification();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentBase-ValueChanged", ex.Message, ex);
            }

            comboMultiHelperReturn = true;
        }

        private void OnValueChanged(JEvent jEvent)
        {
            baseEdit.EditValueChanged += (s, e) =>
            {
                if (!(jEvent.RaiseOnLoad == true && form.IsLoadEvent == true))
                    ValueChanged();
            };
        }

        private void OnCheckedChanged(JEvent jEvent)
        {
            if (control is DevExpress.XtraEditors.CheckEdit)
            {
                (control as CheckEdit).CheckedChanged += (s, e) =>
                {
                    using (Logic.Events ev = new Logic.Events(jEvent, control, form, baseEdit.EditValue))
                    {
                        ev.RunNotification();
                    }
                };
            }
        }

        private void OnClick(JEvent jEvent)
        {
            //direktni insert preko button definicije
            //<button><menuitem control="#gridKontrola"></menuitem></button>
            if (definition is JButton && (definition as JButton).MenuItem != null && PermissionEnabled != false && PermissionVisible != false)
            {
                string menuItemControl = (definition as JButton).MenuItem.Control;
                string menuItemSort = (definition as JButton).MenuItem.Sort;
                string menuItemType = (definition as JButton).MenuItem.Type;

                if (!string.IsNullOrEmpty(menuItemControl))
                {
                    Control menuControl = Controls.GetControl(form, menuItemControl);
                    if (menuControl != null)
                    {
                        JMenuItem menuItem = new JMenuItem() { Caption = definition.Caption, Event = new List<JEvent>() { jEvent }, Sort = menuItemSort, Type = menuItemType };
                        JMenuItemList menuItemList = new JMenuItemList();
                        JMenuItemControl m = AddItemMenuButton(menuControl, menuItem, GetMenuItemSort(menuItem.Sort), "control");
                        menuItemList.Item.Add(m);
                        (menuControl as IControlMenu).MenuItems.Add(menuItemList);
                    }
                    else
                    {
                        form.Logger.Warn("MenuItem - '" + definition.Caption + "'", "Kontrola '" + menuItemControl + "' nije pronađena.");
                    }
                }
            };

            //if (definition is JButton && string.IsNullOrEmpty(definition.DataName))
            //{
            //    control.Click += (s, e) =>
            //    {
            //        OnClick(jEvent, s);
            //    };
            //}
            if (definition is JButton)
            {
                control.Click += (s, e) =>
                {
                    OnClick(jEvent, s);
                };
            } 
        }

        // menu definiran kroz XML
        public void CreateMenu()
        {
            DateTime startTime = DateTime.Now;
            //JMenuItemList menuItemList = new JMenuItemList();

            if ((control as IControlMenu).MenuItems == null)
                (control as IControlMenu).MenuItems = new List<JMenuItemList>();
            else
                (control as IControlMenu).MenuItems.Clear();

            //bool checkIfResult = true;

            if (definition.Menu != null && definition.Menu.Count > 0)
            {
                foreach (JMenu menu in definition.Menu)
                {
                    JMenuItemList menuItemList = new JMenuItemList();
                    menuItemList.Type = menu.Type;
                    menuItemList.ShowAlways = menu.ShowAlways;
                    foreach (JMenuItem menuItem in menu.MenuItem)
                    {
                        // IF
                        //if (Conditions.CheckCondition(ref checkIfResult, menuItem, form, "CreateMenu", false) == false)
                        //    continue;

                        // PERMISSION
                        //bool mPermissionEnabled = true;
                        //bool mPermissionVisible = true;

                        //for (int i = 0; i < form.PermissionList.Count; i++)
                        //{
                        //    if (form.PermissionList[i].Function == menuItem.Function)
                        //    {
                        //        mPermissionEnabled = form.PermissionList[i].Enabled;
                        //        mPermissionVisible = form.PermissionList[i].Visible;
                        //        break;
                        //    }
                        //}

                        //if (mPermissionEnabled == false || mPermissionVisible == false)
                        //    continue;

                        string sort = GetMenuItemSort(menuItem.Sort);

                        if (menuItem.Type == "separator")
                        {
                            ToolStripSeparator m = AddItemMenuSeparator(control, sort);
                            menuItemList.Item.Add(m);
                        }
                        else
                        {
                            JMenuItemControl m = AddItemMenuButton(control, menuItem, sort, menu.Type);
                            menuItemList.Item.Add(m);

                            for (int i = 0; i < form.PermissionList.Count; i++)
                            {
                                if (form.PermissionList[i].Function != menuItem.Function) continue;

                                m.IsEnabled = form.PermissionList[i].Enabled;
                                m.IsVisible = form.PermissionList[i].Visible;
                                break;
                            }
                        }
                    }

                    if (menuItemList.Item.Count > 0)
                        (control as IControlMenu).MenuItems.Add(menuItemList);
                }
            }
            //form.Logger.InfoData("Create menu ", definition.Name, startTime);
        }

        private JMenuItemControl AddItemMenuButton(Control menuControl, JMenuItem menuItem, string sort, string groupType)
        {
            if (menuControl != null && menuControl is Structure.Interfaces.IControlMenu)
            {
                Structure.Components.JMenuItemControl menuItemControl = new Structure.Components.JMenuItemControl();

                if (menuItem.Event != null && menuItem.Event.Count > 0)
                {
                    menuItemControl.Click += (s, e) =>
                    {
                        OnClick(menuItem.Event[0], s);
                    };
                }
                menuItemControl.Text = Common.Translator.TranslateString(menuItem.Caption);

                menuItemControl.Image = Properties.Resources.right_menu_16;
                menuItemControl.Type = groupType;
                menuItemControl.Tag = sort;

                if (!string.IsNullOrEmpty(menuItem.Name))
                {
                    menuItemControl.Name = menuItem.Name;
                    form.MenuItems.Add(menuItemControl);
                }
                return menuItemControl;
                //(menuControl as IControlMenu).MenuItems.Add(menuItemControl);
            }
            return null;
        }

        public ToolStripSeparator AddItemMenuSeparator(Control menuControl, string sort)
        {
            if (menuControl != null && menuControl is Structure.Interfaces.IControlMenu)
            {
                ToolStripSeparator separator = new ToolStripSeparator();
                separator.Tag = sort;
                return separator;
                //(menuControl as IControlMenu).MenuItems.Add(separator);
            }
            return null;
        }

        // defaultni menu
        public void PopulateMenuItems(string type, string kontrola, bool xmlMenuItemsEnabled)
        {
            //if (xmlMenuItemsEnabled == true)
            //    CreateMenu();
            DateTime startTime = DateTime.Now;
            try
            {
                var controlMenu = (control as IControlMenu);
                if (controlMenu != null)
                {
                    int cnt = 0;
                    string typeSearch = type == "" ? null : type;
                    controlMenu.ControlMenu.Items.Clear();
                    if (controlMenu.MenuItems != null && (xmlMenuItemsEnabled == true ||
                        (controlMenu.MenuItems.Count > 0 && controlMenu.MenuItems[0].ShowAlways == true)))
                    {
                        // dosad smo sortirali po stringu, odsad po int
                        var m = controlMenu.MenuItems.Where(x => x.Type == typeSearch).SelectMany(x => x.Item);
                        m.OrderBy(x => Convert.ToInt32(x.Tag));

                        foreach (ToolStripItem item in m)
                        {
                            cnt++;
                            if (control is JTreeListControl && type == "drop")
                                item.Tag = (control as JTreeListControl).ComponentExt.DropNode;

                            controlMenu.ControlMenu.Items.Add(item);

                            if (item is JMenuItemControl it)
                            {
                                if (!it.IsVisible) cnt--;
                                it.Enabled = it.IsEnabled;
                                it.Available = it.IsVisible;
                            }
                        }
                    }

                    if (control is INotifyPrint && type == "")
                    {
                        if ((definition as JModelDataControlsBase).ShowMenuTools != false && kontrola == "grid")
                        {
                            if (cnt > 0)
                            {
                                ToolStripSeparator mm = AddItemMenuSeparator(control, "94");
                                controlMenu.ControlMenu.Items.Add(mm);
                            }

                            JMenuItemControl gridToolsMenu = GridToolsMenu(); // Alati grida dodani
                            controlMenu.ControlMenu.Items.Add(gridToolsMenu);
                            cnt++;
                        }

                        if (kontrola == "grid")
                        {
                            if (cnt > 0)
                            {
                                ToolStripSeparator mm = AddItemMenuSeparator(control, "96");
                                controlMenu.ControlMenu.Items.Add(mm);

                                JMenuItemControl gridToolsMenu = GridTemplatesMenu();

                                controlMenu.ControlMenu.Items.Add(gridToolsMenu);

                                cnt++;
                            }
                        }

                        if ((definition as JModelDataControlsBase).ShowMenuPrintItem != false)
                        {
                            if (cnt > 0)
                            {
                                ToolStripSeparator mm = AddItemMenuSeparator(control, "98");
                                controlMenu.ControlMenu.Items.Add(mm);
                            }

                            JMenuItemControl m1 = AddPrintMenuButton(kontrola);
                            if (kontrola == "grid")
                            {
                                JMenuItemControl sm1 = AddPrintSubMenuButton("Sve");
                                JMenuItemControl sm2 = AddPrintSubMenuButton("Samo označeno"); // ako se mijenja ime onda se mora promijeniti na još jednom IF-u
                                m1.DropDown.Items.Add(sm1);
                                m1.DropDown.Items.Add(sm2);
                            }

                            //JMenuItemControl m1 = AddPrintMenuButton();
                            controlMenu.ControlMenu.Items.Add(m1);
                        }
                    }

                    if (kontrola != "grid" && (controlMenu.MenuItems == null || controlMenu.ControlMenu.Items.Count == 0))
                    {
                        form.Logger.Warn("PopUpMenu - kontrola: '" + (control as JTreeListControl).Name + "'", @"Drop akcija zahtjeva menu iteme za drop akciju unutar <menu type=""drop"">");
                        (control as JTreeListControl).ComponentExt.DropNode = null;
                        (control as JTreeListControl).Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentBase-PopulateMenuItems", ex.Message, ex);
            }
            //finally
            //{
            //    if (control is JTreeListControl)
            //        (control as JTreeListControl).ComponentExt.DropNode = null;
            //}
            form.Logger.InfoData("PopulateMenuItems", definition.Name, startTime);
        }

        private JMenuItemControl AddPrintMenuButton(string kontrola)
        {
            Structure.Components.JMenuItemControl menuItemControl = new Structure.Components.JMenuItemControl();

            if (kontrola == "tree")
            {
                menuItemControl.Click += (s, e) =>
                {
                    (control as INotifyPrint).PrintControl(false);
                };
            }

            menuItemControl.Text = Common.Translator.TranslateString("Ispiši listu");
            menuItemControl.Image = Properties.Resources.right_menu_16;
            menuItemControl.Type = "";
            menuItemControl.Tag = "99";

            return menuItemControl;
        }

        private JMenuItemControl AddPrintSubMenuButton(string caption)
        {
            bool selected = false;
            if (caption == "Samo označeno")
                selected = true;
            Structure.Components.JMenuItemControl menuItemControl = new Structure.Components.JMenuItemControl();

            menuItemControl.Click += (s, e) =>
            {
                (control as INotifyPrint).PrintControl(selected);
            };

            menuItemControl.Text = Common.Translator.TranslateString(caption);
            menuItemControl.Image = Properties.Resources.right_menu_16;
            menuItemControl.Type = "";

            return menuItemControl;
        }

        private JMenuItemControl AddGridToolsMenuButton()
        {
            Structure.Components.JMenuItemControl menuItemControl = new Structure.Components.JMenuItemControl();

            menuItemControl.Text = Common.Translator.TranslateString("Alati");
            menuItemControl.Image = Properties.Resources.right_menu_16;
            menuItemControl.Type = "";
            menuItemControl.Tag = "95";

            return menuItemControl;
        }

        private JMenuItemControl AddGridToolsSubMenuButton(string caption, string helper)
        {
            JGridControl kontrola = control as JGridControl;
            Structure.Components.JMenuItemControl menuItemControl = new Structure.Components.JMenuItemControl();

            menuItemControl.Click += (s, e) =>
            {
                switch (helper)
                {
                    //case "Raširi/Sažmi":
                    case "rasiriSazmi":
                        kontrola.ComponentExt.GridExpand();
                        break;
                    //case "Resetiraj grid":
                    case "resetirajGrid":
                        kontrola.ComponentExt.GridReset();
                        break;
                    //case "Prikaži red sveukupno":
                    case "prikaziRedSveukupno":
                        kontrola.ComponentExt.GridOptionSum();
                        break;
                    //case "Suma ispod grupe":
                    case "sumaIspodGrupe":
                        kontrola.ComponentExt.SetGroupSummaryStyle("under");
                        break;
                    //case "Suma inline":
                    case "sumaInline":
                        kontrola.ComponentExt.SetGroupSummaryStyle("inline");
                        break;

                    case "XLSX":
                        kontrola.ComponentExt.GridExport("xlsx");
                        break;

                    case "XLS":
                        kontrola.ComponentExt.GridExport("xls");
                        break;

                    case "PDF":
                        kontrola.ComponentExt.GridExport("pdf");
                        break;

                    case "HTML":
                        kontrola.ComponentExt.GridExport("html");
                        break;

                    case "RTF":
                        kontrola.ComponentExt.GridExport("rtf");
                        break;

                    case "CSV":
                        kontrola.ComponentExt.GridExport("csv");
                        break;

                    default:
                        break;
                }
            };

            menuItemControl.Text = Common.Translator.TranslateString(caption);

            switch (helper)
            {
                case "rasiriSazmi":
                    menuItemControl.Image = Properties.Resources.expand_16;
                    break;

                case "resetirajGrid":
                    menuItemControl.Image = Properties.Resources.refresh_16;
                    break;

                case "export":
                    menuItemControl.Image = Properties.Resources.excel_16;
                    break;

                case "XLSX":
                    menuItemControl.Image = Properties.Resources.ExportToXLSX_16x16;
                    break;

                case "XLS":
                    menuItemControl.Image = Properties.Resources.ExportToXLS_16x16;
                    break;

                case "PDF":
                    menuItemControl.Image = Properties.Resources.ExportToPDF_16x16;
                    break;

                case "HTML":
                    menuItemControl.Image = Properties.Resources.ExportToHTML_16x16;
                    break;

                case "RTF":
                    menuItemControl.Image = Properties.Resources.ExportToRTF_16x16;
                    break;

                case "CSV":
                    menuItemControl.Image = Properties.Resources.ExportToCSV_16x16;
                    break;

                default:
                    break;
            }
            menuItemControl.Type = "";

            return menuItemControl;
        }

        private ToolStripDropDownButton AddMenuHeader(string caption)
        {
            var menuHeader = new ToolStripDropDownButton();
            menuHeader.Text = Common.Translator.TranslateString(caption);
            menuHeader.Image = Properties.Resources.settings_16;
            menuHeader.Dock = DockStyle.Top;
            menuHeader.BackColor = Color.FromArgb(240, 240, 240);
            menuHeader.Enabled = false;

            return menuHeader;
        }

        private JMenuItemControl GridToolsMenu()
        {
            DateTime startTime = DateTime.Now;
            JMenuItemControl gridToolsMenu = AddGridToolsMenuButton();

            JMenuItemControl gridToolsSubMenu1 = AddGridToolsSubMenuButton("Raširi / Sažmi", "rasiriSazmi");
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu1);

            JMenuItemControl gridToolsSubMenu2 = AddGridToolsSubMenuButton(Carpo.Common.Translator.TranslateString("Resetiraj grid"), Carpo.Common.Translator.TranslateString("resetirajGrid"));
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu2);

            JMenuItemControl gridToolsSubMenu4 = AddGridToolsSubMenuButton("Export", "export");
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu4);

            JMenuItemControl gridToolsSubMenuSubSub1 = AddGridToolsSubMenuButton("XLSX", "XLSX");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub1);
            JMenuItemControl gridToolsSubMenuSubSub2 = AddGridToolsSubMenuButton("XLS", "XLS");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub2);
            JMenuItemControl gridToolsSubMenuSubSub3 = AddGridToolsSubMenuButton("PDF", "PDF");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub3);
            JMenuItemControl gridToolsSubMenuSubSub4 = AddGridToolsSubMenuButton("HTML", "HTML");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub4);
            JMenuItemControl gridToolsSubMenuSubSub5 = AddGridToolsSubMenuButton("RTF", "RTF");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub5);
            JMenuItemControl gridToolsSubMenuSubSub6 = AddGridToolsSubMenuButton("CSV", "CSV");
            gridToolsSubMenu4.DropDown.Items.Add(gridToolsSubMenuSubSub6);

            ToolStripSeparator mm = AddItemMenuSeparator(control, "108");
            gridToolsMenu.DropDown.Items.Add(mm);

            gridToolsMenu.DropDown.Items.Add(AddMenuHeader("Postavke"));

            JMenuItemControl gridToolsSubMenu5Sub1 = AddGridToolsSubMenuButton("Suma ispod grupe", "sumaIspodGrupe");
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu5Sub1);
            JMenuItemControl gridToolsSubMenu5Sub2 = AddGridToolsSubMenuButton("Suma inline", "sumaInline");
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu5Sub2);
            JMenuItemControl gridToolsSubMenu5Sub3 = AddGridToolsSubMenuButton("Prikaži red sveukupno", "prikaziRedSveukupno");
            gridToolsMenu.DropDown.Items.Add(gridToolsSubMenu5Sub3);
            form.Logger.InfoData("GridToolsMenu ", definition.Name, startTime);
            return gridToolsMenu;
        }

        private JMenuItemControl GridTemplatesMenu()
        {
            JMenuItemControl gridTemplates = new Structure.Components.JMenuItemControl();

            gridTemplates.Text = Common.Translator.TranslateString("Predlošci");
            gridTemplates.Image = Properties.Resources.right_menu_16;
            gridTemplates.Type = "";
            gridTemplates.Tag = "97";
            gridTemplates.Click += (s, e) =>
            {
                (control as JGridControl).ComponentExt.MenuItemPredlosciClick();
            };

            return gridTemplates;
        }

        //public void AddItemMenu(JEvent jEvent, string caption, string sort)
        //{
        //    AddItemMenu(this.control, jEvent, caption, sort);
        //}

        //public void AddItemMenu(Control menuControl, JEvent jEvent, string caption, string sort)
        //{
        //    if (menuControl != null && menuControl is Structure.Interfaces.IControlMenu)
        //    {
        //        Structure.Components.JMenuItemControl menuItem = new Structure.Components.JMenuItemControl();

        //        if (jEvent != null)
        //        {
        //            menuItem.Click += (s, e) =>
        //            {
        //                OnClick(jEvent, s);
        //            };
        //        }
        //        menuItem.Text = caption;
        //        menuItem.Image = Properties.Resources.right_menu_16;
        //        menuItem.Type = "control";
        //        if (string.IsNullOrEmpty(sort))
        //            sort = "99";
        //        menuItem.Tag = sort;
        //        (menuControl as IControlMenu).MenuItems.Add(menuItem);
        //        //(menuControl as IControlMenu).ControlMenu.Items.Add(menuItem);
        //        //(menuControl as IControlMenu).SetMenu();
        //    }
        //}

        //public void AddItemMenuSeparator(Control menuControl, string sort)
        //{
        //    if (menuControl != null && menuControl is Structure.Interfaces.IControlMenu)
        //    {
        //        ToolStripSeparator separator = new ToolStripSeparator();
        //        separator.Tag = sort;
        //        (menuControl as IControlMenu).MenuItems.Add(separator);
        //    }
        //}

        public string GetMenuItemSort(string sort)
        {
            int n;
            bool isNumeric = int.TryParse(sort, out n);

            if (isNumeric == false)
                n = menuSort;

            menuSort += 1;
            return n.ToString();
        }

        public void OnClick(JEvent jEvent, object sender)
        {
            if (sender is BarButtonItem && (sender as BarButtonItem).Tag is Structure.Models.JBarButtonItemTag)
            {
                Structure.Models.JBarButtonItemTag tagDef = ((sender as BarButtonItem).Tag as Structure.Models.JBarButtonItemTag);

                if (tagDef.ControlRef is JButtonControl)
                {
                    (tagDef.ControlRef as JButtonControl).DataRowRef = tagDef.DataRowRef;
                }
            }
            if (sender is BarSubItem && (sender as BarSubItem).Tag is Structure.Models.JBarButtonItemTag)
            {
                Structure.Models.JBarButtonItemTag tagDef = ((sender as BarSubItem).Tag as Structure.Models.JBarButtonItemTag);

                if (tagDef.ControlRef is JButtonControl)
                {
                    (tagDef.ControlRef as JButtonControl).DataRowRef = tagDef.DataRowRef;
                }
            }

            if (sender is JButtonControl && !string.IsNullOrEmpty(definition.DataName))
            {
                (sender as JButtonControl).ComponentExt.SetDropDownMenu(true);
                return;
            }

            using (Logic.Events ev = new Logic.Events(jEvent, control, form))
            {
                var menuItemControl = sender as JMenuItemControl;
                try
                {
                    if (control is JTreeListControl && menuItemControl != null && menuItemControl.Tag is TreeListNode)
                        (control as JTreeListControl).ComponentExt.DropNode = menuItemControl.Tag as TreeListNode;
                    ev.RunNotification();
                }
                catch (Exception ex)
                {
                    form.Logger.Error("event: ComponentBase-OnClick", ex.Message, ex);
                }
                finally
                {
                    if (control is JTreeListControl)
                    {
                        (control as JTreeListControl).ComponentExt.DropNode = null;
                        (control as JTreeListControl).Refresh();
                    }
                }
            }
        }

        public void OnRowClick(JEvent jEvent)
        {
            try
            {
                if (jEvent.Actions.Count > 0 && jEvent.Actions[0].Type.ToLower() == "disallow")
                {
                    if (control is JTreeLookUpControl)
                    {
                        (this as ComponentTreeLookUpEdit).BeforeFocusing = true;
                        (this as ComponentTreeLookUpEdit).DisableNodeEvent = jEvent;

                        (control as TreeListLookUpEdit).Properties.TreeList.BeforeFocusNode += (s, e) =>
                        {
                            treeList_BeforeFocusNode(s, e, jEvent.Actions[0]);
                        };
                    }
                    else if (control is JTreeListControl)
                    {
                        (this as ComponentTree).BeforeFocusing = true;
                        (this as ComponentTree).DisableNodeEvent = jEvent;

                        (control as TreeList).BeforeFocusNode += (s, e) =>
                        {
                            treeList_BeforeFocusNode(s, e, jEvent.Actions[0]);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentBase-OnRowClick", ex.Message, ex);
            }
        }

        private void treeList_BeforeFocusNode(object sender, DevExpress.XtraTreeList.BeforeFocusNodeEventArgs e, JEventAction action)
        {
            try
            {
                if (this is ComponentTree)
                    (this as ComponentTree).BeforeFocusNode = e.Node;
                else
                    (this as ComponentTreeLookUpEdit).BeforeFocusNode = e.Node;

                bool condition = true;
                Conditions.CheckCondition(ref condition, action, form, control);

                e.CanFocus = !condition;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (this is ComponentTree)
                    (this as ComponentTree).BeforeFocusNode = null;
                else
                    (this as ComponentTreeLookUpEdit).BeforeFocusNode = null;
            }
        }

        private void OnLeave(JEvent jEvent)
        {
            DateTime startTime = DateTime.Now;
            control.Leave += (s, e) =>
            {
                using (Logic.Events ev = new Logic.Events(jEvent, control, form, baseEdit.EditValue))
                {
                    ev.RunNotification();
                }
            };
            //form.Logger.InfoData("OnLeave event ", definition.Name, startTime);
        }

        private void OnKeyDown(JEvent jEvent)
        {
            int num;
            object value;

            if (!string.IsNullOrEmpty(jEvent.Value)) // sa value-om
            {
                using (Parser parser = new Parser(form, null))
                {
                    string valueStr = jEvent.Value;
                    value = parser.GetLogicPartValue(valueStr);
                }
                if (value != null)
                {
                    bool isNumeric = int.TryParse(value.ToString(), out num);
                    if (isNumeric == true)
                    {
                        control.KeyDown += (s, e) =>
                        {
                            KeyDown(true, num, e, jEvent);
                        };
                    }
                    else
                        form.Logger.Warn("event: OnKeyDown", "Vrijednost tipke na koju se odnosi event nije dobro definirana; ");
                }
                else
                    form.Logger.Warn("event: OnKeyDown", "Neuspješno parsiranje vrijednosti; " + jEvent.Value.ToString());
            }
            else // bez value-a
            {
                control.KeyDown += (s, e) =>
                {
                    KeyDown(false, null, e, jEvent);
                };
            }
        }

        private void KeyDown(bool eventValueExist, int? num, KeyEventArgs e, JEvent jEvent)
        {
            if (e.Modifiers != Keys.None && (e.KeyValue == 16 || e.KeyValue == 17 || e.KeyValue == 18))
                KeyCode = "";
            else
                KeyCode = e.KeyValue.ToString();

            if (eventValueExist)
            {
                if (num > 0)
                {
                    if (e.KeyValue == num)
                    {
                        using (Logic.Events ev = new Logic.Events(jEvent, control, form, null))
                        {
                            ev.RunNotification();
                        }
                    }
                }
            }
            else
            {
                using (Logic.Events ev = new Logic.Events(jEvent, control, form, null))
                {
                    ev.RunNotification();
                }
            }

            KeyCode = "";
        }

        private void OnFocusedRowChanged()
        {
            if (control is TreeList)
            {
                (control as TreeList).FocusedNodeChanged += (s, e) =>
                {
                    if (Action != "populating" && Action != "reordering")
                    {
                        FocusedNodeChanged(false);
                    }
                };
            }
            else if (gridView != null)
            {
                gridView.FocusedRowChanged += (s, e) =>
                {
                    if (gridView.FocusedRowHandle < 0 && ((control as JGridControl).Definition.PreventFocusGroup != false))
                        return;

                    if ((control as JGridControl).ComponentExt == null || (control as JGridControl).ComponentExt.blockEvents == true)
                        return;

                    //if (Action != "populating" && Action2 != "ignorefocus")
                    if (Action != "populating")
                    {
                        try
                        {
                            FocusedRowChanged(e);
                        }
                        finally
                        {
                            (control as JGridControl).ComponentExt.KeyFocusedValue = null;
                        }
                    }
                };
            }
        }

        public void OnSelectionChanged()
        {
            if (gridView != null)
            {
                gridView.SelectionChanged += (s, e) =>
                {
                    OnSelectionChangedExtracted();
                };
            }
        }

        public void OnSelectionChangedExtracted()
        {
            //if (Action != "populating" && Action2 != "ignorefocus")
            if (Action != "populating")
            {
                if (control is JGridControl)
                {
                    if ((control as JGridControl).ComponentExt == null || (control as JGridControl).ComponentExt.blockEvents == true)
                        return;
                }
                using (Logic.Events ev = new Logic.Events(selectionChanged, control, form))
                {
                    ev.RunNotification();
                }
            }
        }

        private void OnCellValueChanged()
        {
            gridView.CellValueChanged += (s, e) =>
            {
                try
                {
                    if (ActionEvent != "cellvaluechanging")
                    {
                        ActionEvent = "cellvaluechanging";
                        JGridControl gridControl = null;
                        if (gridView.GridControl is JGridControl)
                            gridControl = (gridView.GridControl as JGridControl);

                        if (gridControl != null)
                        {
                            try
                            {
                                using (Logic.Events ev = new Logic.Events(cellValueChanged, control, form))
                                {
                                    ev.RunNotification();
                                }
                            }
                            catch (Exception ex)
                            {
                                form.Logger.Error("event: ComponentBase-OnCellValueChanged", ex.Message, ex);
                            }
                        }
                        //gridView.SetRowCellValue(gridView.FocusedRowHandle, gridView.FocusedColumn, 22);
                    }
                }
                catch (Exception ex)
                {
                    form.Logger.Error("event: ComponentBase-OnCellValueChanged", ex.Message, ex);
                }
                finally { ActionEvent = ""; }
            };
        }

        private void OnDoubleClick(JEvent jEvent)
        {
            
            gridView.DoubleClick += (s, e) =>
            {
                if (gridView.FocusedRowHandle < 0)
                    return;

                Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = gridView.CalcHitInfo(pt);
                if (info.RowHandle >= 0)
                {
                    using (Logic.Events ev = new Logic.Events(jEvent, control, form))
                    {
                        ev.RunNotification();
                    }
                }
            };
        }

        private void OnTabChanging(JEvent jEvent)
        {
            DateTime startTime = DateTime.Now;
            (control as XtraTabControl).SelectedPageChanging += (s, e) =>
            {
                using (Logic.Events ev = new Logic.Events(jEvent, control, form))
                {
                    ev.RunNotification();
                }

                if (PreventTabChanging == true)
                {
                    e.Cancel = true;
                    PreventTabChanging = false;
                }
            };
        }

        private void OnTabChanged(JEvent jEvent)
        {
            DateTime startTime = DateTime.Now;
            (control as XtraTabControl).SelectedPageChanged += (s, e) =>
            {
                using (Logic.Events ev = new Logic.Events(jEvent, control, form))
                {
                    ev.RunNotification();
                }
            };
        }






        //izdvajam jer grid na čišćenje DataSource-a zove FocusedRowChanged
        //ovaj dio pozivam nakon punjenja grida
        protected void FocusedRowChanged(DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (gridView.GridControl.DataSource != null)
            {
                if (e != null && (control as JGridControl).Definition.PreventFocusGroup != false)
                    if (PreventFocusGroup(e) == false)
                        return;
                using (Logic.Events ev = new Logic.Events(focusedRowChanged, control, form, gridView.RowCount))
                {
                    ev.RunNotification();
                }
            }
            else if (gridView.GridControl is JGridControl
                && (gridView.GridControl as JGridControl).ComponentExt != null
                    && (gridView.GridControl as JGridControl).ComponentExt.DetailControl != null)
            {
                (gridView.GridControl as JGridControl).ComponentExt.DetailControl.DataSource = null;
                (gridView.GridControl as JGridControl).ComponentExt.DetailControl.ComponentExt.RowsCount = 0;
                (gridView.GridControl as JGridControl).ComponentExt.DetailControl.ComponentExt.SetLabelCount();
            }
        }

        private bool PreventFocusGroup(DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int focusedRowHandle = -1;
            if (e.FocusedRowHandle == GridControl.NewItemRowHandle || e.FocusedRowHandle == GridControl.AutoFilterRowHandle)
                return false;
            if (e.FocusedRowHandle < 0)
            {
                if (e.PrevFocusedRowHandle == GridControl.InvalidRowHandle)
                    focusedRowHandle = 0;
                else if (Control.MouseButtons == MouseButtons.Left || Control.MouseButtons == MouseButtons.Right)
                    focusedRowHandle = e.PrevFocusedRowHandle;
                else
                {
                    int prevRow = gridView.GetVisibleIndex(e.PrevFocusedRowHandle);
                    int currRow = gridView.GetVisibleIndex(e.FocusedRowHandle);
                    if (prevRow > currRow)
                        focusedRowHandle = e.PrevFocusedRowHandle - 1;
                    else
                        focusedRowHandle = e.PrevFocusedRowHandle + 1;
                    if (focusedRowHandle < 0) focusedRowHandle = 0;
                    if (focusedRowHandle >= gridView.DataRowCount) focusedRowHandle = gridView.DataRowCount - 1;
                }
                gridView.FocusedRowHandle = focusedRowHandle < 0 ? 0 : focusedRowHandle;
                return false;
            }
            return true;
        }

        public void FocusedNodeChanged(bool additionalFocus)
        {
            DateTime startTime = DateTime.Now;
            // additionalFocus (true) koristimo kod osvježavanja kontrole sa dodatnim "ručnim" fokusom
            // kroz ugnježđenu akciju "control focus"
            var tree = control as JTreeListControl;
            if (tree == null)
                return;

            if (tree.ComponentExt.Action == "reordering")
                return;

            if (tree.FocusedNode == tree.Nodes.AutoFilterNode && tree.ComponentExt.DetailControl != null)
            { tree.ComponentExt.DetailControl.DataSource = null; }
            else if (tree.DataSource != null && additionalFocus != true)
                using (Logic.Events ev = new Logic.Events(focusedRowChanged, control, form, (control as JTreeListControl).Nodes.Count))
                {
                    ev.RunNotification();
                }
            else if (tree.ComponentExt.DetailControl != null)
            {
                tree.ComponentExt.DetailControl.DataSource = null;
            }
            //form.Logger.InfoData("FocusedNodeChanged ", definition.Name, startTime);
        }

        #endregion CreateEvents

        #region NotifyMe

        public void SetNotify(JEventAction action, object value)
        {
            try
            {
                Logic.Notifications notification = new Notifications(form, control, action, value);
                notification.SetNotify();
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentBase-SetNotify", ex.Message, ex);
            }
        }

        #endregion NotifyMe

        internal virtual void SetValue(IJEditor jEditor)
        {



            if (control is JButtonControl)
                return;
            else if (control is JAutoSuggestControl)
                return;

            object value = null;
            string text = "";
            bool tableExists = false;

            var varsApp = form.CarpoVarsApp.FirstOrDefault(x => x.FormName == form.FormName && x.Name == control.Name);
            var varsSession = form.CarpoVarsSession.FirstOrDefault(x => x.FormName == form.FormName && x.Name == control.Name);

            if (!string.IsNullOrEmpty(definition.DataValue)/* && !string.IsNullOrEmpty(form.MainParameter)*/)
            {
                Logic.Data.Core data = new Logic.Data.Core();
                data.ConnectionString = form.ConnectionString;
                data.CurrentForm = form;

                string tableName = Logic.Parser.ParseParameterGetName(definition.DataValue.Replace("#", "~"), "~");

                //if (definition.DataValue.StartsWith("global", StringComparison.InvariantCultureIgnoreCase) && !tableName.StartsWith("global", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    tableName = "global.~" + tableName.Replace("~", "");
                //}

                if (form.DataList.Count > 0 && form.DataList.ContainsKey(tableName))
                {
                    tableExists = true;
                    value = data.GetDataValue(definition.DataValue);
                    text = GetValueText(data);
                }
            }

            if (tableExists == false && varsApp != null) // ako je definirano snimanje varijable na nivou application --> savevalue="app"
            {
                value = varsApp.ValueObj;
                text = varsApp.ValueText;
            }
            else if (tableExists == false && varsSession != null) //ako je definirano snimanje varijable na nivou sessiona --> savevalue="session"
            {
                value = varsSession.ValueObj;
                text = varsSession.ValueText;
            }
            else if (tableExists == false && !string.IsNullOrEmpty(definition.Value))
            {
                value = definition.Value;
                text = GetValueText(null);
            }

            //lookupcontrol - postavljanje prve vrijednosti iz tablice ako je allownull=false i primljena vrijednost null
            if (jEditor is JLookUpControl && definition.AllowNull == false && value == null)
            {
                try
                {
                    JLookUpControl jLookUpControl = jEditor as JLookUpControl;
                    if (jLookUpControl.Properties.DataSource is DataTable)
                    {
                        DataTable dt = (jLookUpControl.Properties.DataSource as DataTable);
                        if (dt.Rows.Count > 0)
                            value = dt.Rows[0][definition.ValueField];
                    }
                }
                catch (Exception ex)
                {
                    form.Logger.Error("Method: SetValue --> LookUpEditor (AllowNull)", ex.Message, ex);
                }
            }

            //lookupcontrol - postavljanje prve vrijednosti iz tablice ako je allownull=false i primljena vrijednost null
            else if (jEditor is JAutoSuggestControl && definition.AllowNull == false && value == null)
            {
                try
                {
                    JAutoSuggestControl jLookUpControl = jEditor as JAutoSuggestControl;
                    if (jLookUpControl.Properties.DataSource is DataTable)
                    {
                        DataTable dt = (jLookUpControl.Properties.DataSource as DataTable);
                        if (dt.Rows.Count > 0)
                            value = dt.Rows[0][definition.ValueField];
                    }
                }
                catch (Exception ex)
                {
                    form.Logger.Error("Method: SetValue --> LookUpEditor (AllowNull)", ex.Message, ex);
                }
            }

            if (!(jEditor is JAutoSuggestControl))
            {
                jEditor.SetValue(value, text);
            } else
            {
                if (value == null)
                {
                    (jEditor as JAutoSuggestControl).EditValue = null;
                }
            }
            
        }

        private string GetValueText(Logic.Data.Core data)
        {
            if (control is JSearchControl)
            {
                if (data != null)
                {
                    if (!string.IsNullOrEmpty((definition as JSearchEdit).DataText))
                    {
                        object value = data.GetDataValue((definition as JSearchEdit).DataText);
                        if (value != null)
                            return value.ToString();
                    }
                }

                object textObj = data.GetDataValue((definition as JSearchEdit).DataText);
                if (textObj != null)
                    return textObj.ToString();
            }
            return "";
        }

        public void SetItemCaption(object caption, bool attributeRequiredChanged = false)
        {
            if (caption != null && attributeRequiredChanged == false)
            {
                if (form is Forms.Base.JupiterEditor)
                {
                    foreach (LayoutControl layoutControl in (form as Forms.Base.JupiterEditor).LayoutControlList)
                    {
                        LayoutControlItem item = layoutControl.GetItemByControl(control);
                        if (item != null)
                        {
                            bool truncate = form.DefinitionRoot.TranslatorTruncate == false ? false : true;
                            if (item.TextLocation == DevExpress.Utils.Locations.Top) // da li da ovdje dodajem i uvjet && form.DefinitionRoot.TranslatorTruncate != true
                                truncate = false;

                            item.Text = Common.Translator.TranslateString(Convert.ToString(caption), true);
                            break;
                        }
                    }
                }
            }
            else    //ako je promijenjen "required" kontrole
            {
                if (attributeRequiredChanged == true && form is Forms.Base.JupiterEditor)
                {
                    foreach (LayoutControl layoutControl in (form as Forms.Base.JupiterEditor).LayoutControlList)
                    {
                        LayoutControlItem item = layoutControl.GetItemByControl(control);
                        if (item != null)
                        {
                            // ako je kontrolin "required" promijenjen: false->true dodaj zvjezdicu, true->false makni ju s item captiona
                            if (definition.Required == "false")
                                item.Text = item.Text.Replace("<color=200, 0, 0>*</color>", "");
                            else
                                if (!item.Text.Contains("<color=200, 0, 0>*</color>"))
                                item.Text += "<color=200, 0, 0>*</color>";
                            break;
                        }
                    }
                }
            }
        }

        public void SetControlCaption(object caption)
        {
            if (caption != null & control is IJControlCaption)
                control.Text = Common.Translator.TranslateString(caption.ToString(), true);
        }

        public void SetAttribute(JEventAction action)
        {
            if (action == null || action.Parameter.Count == 0)
                return;

            bool checkIfResult = true;
            foreach (JEventActionParameter parameter in action.Parameter)
            {
                if (Conditions.CheckCondition(ref checkIfResult, parameter, form, null) == false)
                    continue;

                Control controlRef = null;
                if (!string.IsNullOrEmpty(action.Value))
                    controlRef = Logic.Controls.FindControl(form, action.Value)?.FirstOrDefault();

                if (!string.IsNullOrEmpty(parameter.Name) && !string.IsNullOrEmpty(parameter.Value))
                    SetAttribute(parameter.Name, parameter.Value, controlRef);
            }
        }

        private void SetAttribute(string attribute, string value, Control control)
        {
            //Evaluate evaluate = new Evaluate(form, value);
            //value = evaluate.ToString();
            object valueObj;

            attribute = attribute.ToLower();

            using (Parser parser = new Parser(form, null))
            {
                valueObj = parser.GetLogicPartValue(value);
            }
            if (valueObj != null || attribute == "localimg") //  attribute=="localimg" - dodano jer atribut value(valueObj) za localimg može biti null
            {
                switch (attribute)
                {
                    case "format":
                        if (this is ComponentTextEdit)
                        {
                            (this as ComponentTextEdit).definition.Format = valueObj.ToString();
                            (this as ComponentTextEdit).SetFormat();
                        }
                        break;

                    case "formattype":
                        if (this is ComponentTextEdit)
                        {
                            (this as ComponentTextEdit).definition.FormatType = valueObj.ToString();
                        }
                        break;

                    case "tabstop":
                        definition.TabStop = Convert.ToBoolean(valueObj);
                        baseEdit.TabStop = Convert.ToBoolean(valueObj);
                        break;

                    case "required":
                        (this as ComponentBase).definition.Required = valueObj.ToString();
                        SetValidation(true);
                        break;

                    case "fontcolor":
                        if ((this is ComponentTextEdit) || (this is ComponentDateEdit) || (this is ComponentLabel)
                            || (this is ComponentSimpleLabel) || (this is ComponentTimeEdit) || (this is ComponentCheckEdit))
                        {
                            setAttributeFontColor(valueObj);
                        }
                        break;

                    case "dbimg":
                        if (this is ComponentPictureEdit)
                        {
                            (this as ComponentPictureEdit).ChangeControlImage(valueObj as byte[]);
                        }
                        break;

                    case "localimg":
                        if (this is ComponentPictureEdit)
                        {
                            if (valueObj == null)
                                (this as ComponentPictureEdit).SetControlLocalImage("");
                            else
                                (this as ComponentPictureEdit).SetControlLocalImage(valueObj.ToString());
                        }
                        break;
                    case "dbdoc":
                        if (this is ComponentDocumentViewer)
                        {
                            (this as ComponentDocumentViewer).LoadDocument();
                        }
                        break;
                    case "dbpdf":
                        if (this is ComponentPdfViewer)
                        {
                            (this as ComponentPdfViewer).ChangeControlPdf(valueObj as byte[]);
                        }
                        break;

                    case "localpdf":
                        if (this is ComponentPdfViewer)
                        {
                            if (valueObj == null)
                                (this as ComponentPdfViewer).SetControlLocalPdf("");
                            else
                                (this as ComponentPdfViewer).SetControlLocalPdf(valueObj.ToString());
                        }
                        break;
                    case "showcheckboxes":
                        if (this is ComponentGrid)
                        {
                            (this as ComponentGrid).SetAttributeShowCheckBoxes(Convert.ToBoolean(valueObj));
                        }
                        break;
                    case "findpanel":
                        {
                            string findRaw = valueObj == null ? "" : valueObj.ToString();
                            Structure.Models.Core.DxUxAttr.ParseFindPanel(findRaw, out bool? show, out string mode);
                            bool enabled = show == true;
                            if (this is ComponentGrid)
                                (this as ComponentGrid).SetAttributeFindPanel(enabled, enabled ? mode : null);
                            else if (this is ComponentTree)
                                (this as ComponentTree).SetAttributeFindPanel(enabled, enabled ? mode : null);
                        }
                        break;
                    case "filtereditor":
                        if (this is ComponentGrid)
                            (this as ComponentGrid).SetAttributeFilterEditor(Convert.ToBoolean(valueObj));
                        break;
                    case "editform":
                        if (this is ComponentGrid)
                        {
                            string mode = valueObj.ToString();
                            bool enabled = !string.Equals(mode, "false", StringComparison.OrdinalIgnoreCase)
                                && !string.Equals(mode, "0", StringComparison.OrdinalIgnoreCase);
                            (this as ComponentGrid).SetAttributeEditForm(enabled, enabled ? mode : null);
                        }
                        break;
                    case "columnchooser":
                        if (this is ComponentGrid)
                            (this as ComponentGrid).SetAttributeColumnChooser(Convert.ToBoolean(valueObj));
                        break;
                }
            }
        }

        public void setAttributeFontColor(object valueObj)
        {
            this.definition.FontColor = valueObj.ToString();

            if ((control is JLabelEditControl))
                (control as JLabelEditControl).Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(definition.FontColor, (control as JLabelEditControl).Appearance.ForeColor);
            else
                this.baseEdit.Properties.Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(this.definition.FontColor, baseEdit.Properties.Appearance.ForeColor);
        }

        public void SetVisibility(JEventAction action)
        {
            if (PermissionVisible == false)
                return;

            object valueObj = Parameters.GetValue(form, action);
            if (valueObj == null)
                return;

            bool value;
            if (Boolean.TryParse(valueObj.ToString(), out value))
                SetVisibility(value);
        }

        public void SetVisibility(bool visible)
        {
            bool controlFound = false;
            if (form is Forms.Base.JupiterEditor)
            {
                foreach (LayoutControl layout in (form as Forms.Base.JupiterEditor).LayoutControlList)
                {
                    var c = layout.GetItemByControl(control);
                    if (c != null)
                    {
                        controlFound = true;
                        if (visible == true)
                            c.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        else
                            c.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
            }
            if (controlFound == false) //znači da kontrola nije unutar layout kontrole
                control.Visible = visible;
        }

        public virtual void RefreshValue()
        {
            if (control is IJEditor)
                SetValue(control as IJEditor);
            else if (control is JGridControl)
                (control as JGridControl).Populate(null, false, false, FocusType.Default);
            else if (control is JTreeListControl)
                (control as JTreeListControl).Populate(null, false, false, FocusType.Default);
        }

        public object GetKeycode()
        {
            return KeyCode;
        }
    }
}