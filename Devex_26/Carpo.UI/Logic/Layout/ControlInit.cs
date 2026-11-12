using Carpo.Core;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using Carpo.UI.Components;
using Carpo.UI.Components.Base;
using Carpo.UI.Components.Container;
using Carpo.UI.Components.Editor;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic.Layout
{
    public class ControlInit
    {
        //public static List<Component> CustomControls { get; set; } = new List<Component>();
        private Forms.Base.JupiterEditor form;        

        private string connectionString;
        private LayoutControl layoutControl;
        private JLayout jLayout;
        private DevExpress.Utils.Locations defaultTextLocation;

        private int defaultItemPadding = 3;
        private int defaultItemSpacing = 3;
        private int defaultGroupPadding = 3;
        private int defaultGroupSpacing = 0;

        private enum PaddingType { Padding, Spacing }

        private enum ItemType { Item, Group }

        public ControlInit(Forms.Base.JupiterEditor form, LayoutControl layoutControl, JLayout jLayout, string connectionString)
        {
            this.form = form;
            this.layoutControl = layoutControl;
            this.jLayout = jLayout;
            this.connectionString = connectionString;
            
            defaultTextLocation = SetTextPosition(jLayout.TextPosition, true);
            defaultItemPadding = SetItemPadding(jLayout.ItemPadding);
        }

        private DevExpress.Utils.Locations SetTextPosition(string textPosition, bool isInit)
        {
            DevExpress.Utils.Locations location;
            switch (textPosition)
            {
                case "left":
                    location = DevExpress.Utils.Locations.Left;
                    break;

                case "bottom":
                    location = DevExpress.Utils.Locations.Bottom;
                    break;

                case "right":
                    location = DevExpress.Utils.Locations.Right;
                    break;

                default:
                    location = DevExpress.Utils.Locations.Top;
                    break;
            }
            return location;
        }

        private int SetItemPadding(int? itemPadding)
        {
            if (itemPadding == null)
                return defaultItemPadding;

            return (int)itemPadding;
        }

        //****first call
        public void CreateLayout()
        {            
            try
            {
                layoutControl.Root.OptionsTableLayoutGroup.Add(new RowDefinition() { Height = 100, SizeType = SizeType.Percent });
                layoutControl.Root.OptionsTableLayoutGroup.Add(new ColumnDefinition() { Width = 100, SizeType = SizeType.Percent });

                List<JLayoutGroup> groups = jLayout.Group;
                bool checkIfResult_1 = true;
                foreach (JLayoutGroup group in groups)
                {
                    if (Conditions.CheckCondition(ref checkIfResult_1, group, form, null) == false)
                        continue;

                    List<JLayoutGroup> groupList = new List<JLayoutGroup>() { group };
                    CreateGroups(groupList, layoutControl.Root, true);
                }
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }            
        }

        #region Control

        private System.ComponentModel.Component CreateControl(JLayoutItem item, out bool? isRequired)
        {
            DateTime startTime = DateTime.Now;
            isRequired = false;
            System.ComponentModel.Component control = null;
            string controlName = Guid.NewGuid().ToString();
            
            if (item.Container != null)
                return CreateContainerControl(item, out isRequired);

            if (item.Grid != null)
            {
                control = GetGridControl(item.Grid);
                item.TextVisible = false;
            }
            else if (item.Tree != null)
            {
                control = GetTreeControl(item.Tree);
                item.TextVisible = false;
            }
            else if (item.ComboEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.ComboEdit.Required)))
                    isRequired = Convert.ToBoolean(item.ComboEdit.Required);
                control = GetLookUpControl(item.ComboEdit, connectionString, form.EventList);
            }
            else if (item.ComboMultiEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.ComboMultiEdit.Required)))
                    isRequired = Convert.ToBoolean(item.ComboMultiEdit.Required);
                control = GetComboMultiEditControl(item.ComboMultiEdit, connectionString, form.EventList);
            }
            else if (item.TokenEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.TokenEdit.Required)))
                    isRequired = Convert.ToBoolean(item.TokenEdit.Required);
                control = GetTokenEditControl(item.TokenEdit);
            }
            else if (item.ComboTreeEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.ComboTreeEdit.Required)))
                    isRequired = Convert.ToBoolean(item.ComboTreeEdit.Required);
                control = GetTreeLookUpControl(item.ComboTreeEdit, connectionString, form.EventList);
            }
            else if (item.TextEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.TextEdit.Required)))
                    isRequired = Convert.ToBoolean(item.TextEdit.Required);
                control = GetTextEditControl(item.TextEdit);
            }
            else if (item.HyperLinkEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.HyperLinkEdit.Required)))
                    isRequired = Convert.ToBoolean(item.HyperLinkEdit.Required);
                control = GetHyperLinkEditControl(item.HyperLinkEdit);
            }
            else if (item.MemoEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.MemoEdit.Required)))
                    isRequired = Convert.ToBoolean(item.MemoEdit.Required);
                control = GetMemoEditControl(item.MemoEdit);
            }
            else if (item.RichEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.RichEdit.Required)))
                    isRequired = Convert.ToBoolean(item.RichEdit.Required);
                control = GetRichEditControl(item.RichEdit);
            }
            else if (item.DateEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.DateEdit.Required)))
                    isRequired = Convert.ToBoolean(item.DateEdit.Required);
                //control = new JDateEditControl(item.DateEdit);
                control = GetDateEditControl(item.DateEdit);
            }
            else if (item.RadioEdit != null)
            {
                control = GetRadioEditControl(item.RadioEdit);
            }
            else if (item.CheckEdit != null)
            {
                control = GetCheckEditControl(item.CheckEdit);
            }
            else if (item.LabelEdit != null)
            {
                control = GetLabelControl(item.LabelEdit);
                //control = new JSimpleLabelItem(layoutControl, item.LabelEdit);
            }
            else if (item.ButtonTextEdit != null)
            {
                control = GetTextButtonControl(item.ButtonTextEdit);
            }
            else if (item.Button != null)
            {
                control = GetJButtonControl(item.Button);
            }
            else if (item.SearchEdit != null)
            {
                if (!(string.IsNullOrEmpty(item.SearchEdit.Required)))
                    isRequired = Convert.ToBoolean(item.SearchEdit.Required);
                control = GetSearchEditControl(item.SearchEdit);
            }
            else if (item.DaysEdit != null)
            {
                control = GetDaysEditControl(item.DaysEdit);
            }
            else if (item.TimeEdit != null)
            {
                control = GetTimeEditControl(item.TimeEdit);
            }
            else if (item.PictureEdit != null)
            {
                control = GetPictureEditControl(item.PictureEdit);
            }
            else if (item.PdfViewer != null)
            {
                control = GetPdfViewerControl(item.PdfViewer);
            }
            else if (item.DocumentViewer != null)
            {
                control = GetDocumentViewerControl(item.DocumentViewer);
            }
            //else if (item.Chart != null)
            //{
            //    control = GetChartControl(item.Chart);
            //}
            else if (item.BrowseEdit != null)
            {
                control = GetBrowseEditControl(item.BrowseEdit);
            }
            else if (item.Chart != null)
            {
                control = GetChartControlControl(item.Chart);
            }
            else if (item.Separator != null)
            {
                control = GetSeparatorControl(item.Separator);
            }
            else if (item.CustomControl != null)
            {
                if (Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Exists(x => x.ControlName == item.CustomControl.Name))
                {
                    control = Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.FirstOrDefault(x => x.ControlName == item.CustomControl.Name).Control;
                }
                else
                {
                    var specificControlLocal = new UI.Components.JCustomUC(item.CustomControl);
                    form.Logger.InfoData("Inicijalizacija lokalne custom kontrole", "Kontrola '" + specificControlLocal.definition.Name +
                    "' uspješno incijalizirana ", DateTime.Now);

                    form.CustomControls.Add(specificControlLocal);
                    control = specificControlLocal.Control;
                }
            }

            if (control == null)
            {
                item.TextVisible = false;
            }
            //else
            //{
            //    if (control is Control)
            //    {
            //        if ((control as Control).Name == "")
            //            (control as Control).Name = controlName;
            //        form.ControlList.Add(new Structure.Models.JControl() { ControlName = controlName, Control = control as Control, ControlType = control.GetType() });
            //    }
            //}

            //form.Logger.InfoData("CreateControl","" , startTime);
            return control;
        }

        #endregion Control

        #region LayoutGroups

        //private LayoutControlGroup CreateLayoutControlGroup(BaseLayoutItem parentGroup)
        //{
        //    LayoutControlGroup groupNew = new LayoutControlGroup();
        //    InitLayoutGroup(groupNew);
        //    groupNew.TextVisible = false;
        //    groupNew.Parent = (LayoutControlGroup)parentGroup;
        //    groupNew.OptionsTableLayoutGroup.AddColumn();
        //    groupNew.OptionsTableLayoutGroup.AddRow();
        //    return

        //}

        private LayoutControlGroup CreateLayoutControlGroup(JLayoutGroup item, BaseLayoutItem parentGroup)
        {
            LayoutControlGroup groupNew = new LayoutControlGroup();

            if (!string.IsNullOrEmpty(item.BackgroundColor))
            {
                //groupNew.GroupBordersVisible = false;
                groupNew.AppearanceGroup.BackColor = Common.Helpers.Colors.HtmlToColor(item.BackgroundColor, groupNew.AppearanceGroup.BackColor);
            }

            InitLayoutGroup(groupNew);

            //var test = item.ItemList.Where(x => x.Type == "splitter");

            if (!string.IsNullOrEmpty(item.Name))
            {
                groupNew.Name = item.Name;
            }

            if (item.Caption != null)
            {
                groupNew.TextVisible = true;
                groupNew.Text = SetCaption(item.Caption);
            }
            else
            {
                groupNew.TextVisible = false;
            }

            if ((item.GroupBorderVisible == false) || (item.BorderVisible == false))
                groupNew.GroupBordersVisible = false;

            SetLayoutGroup gr = new SetLayoutGroup(groupNew, form);
            gr.SetLayoutGroupDefinition(item);
            if (parentGroup.IsGroup == true)
            {
                groupNew.Parent = (parentGroup as LayoutControlGroup);
            }
            else
            {
                ((TabbedControlGroup)parentGroup).AddTabPage(groupNew);
            }

            string itemPadding = GetPaddingStr(item.Padding, PaddingType.Padding, ItemType.Group);
            string itemSpacing = GetPaddingStr(item.Spacing, PaddingType.Spacing, ItemType.Group);

            SetPadding(groupNew, itemPadding, PaddingType.Padding, ItemType.Group);
            SetPadding(groupNew, itemSpacing, PaddingType.Spacing, ItemType.Group);

            if (item.AlignMode == "local")
                groupNew.OptionsItemText.TextAlignMode = TextAlignModeGroup.AlignLocal;

            CreateTabs(item.TabsList, groupNew);
            CreateGroups(item.GroupList, groupNew, false);
            CreateItems(item.ItemList.OrderBy(x => x.Row).ThenBy(x => x.Col).ToList(), item.ItemDefault, groupNew);

            return groupNew;
        }

        private string SetCaption(string caption)
        {
            if (!(string.IsNullOrEmpty(caption) && caption.Contains("{")))
            {
                Evaluate evaluate = new Evaluate(form, caption);
                return evaluate.ToString();
            }

            return SetLabelCaption(caption);
        }

        private string SetLabelCaption(string caption)
        {
            bool truncate = form.DefinitionRoot.TranslatorTruncate == false ? false : true;
            if (jLayout.TextPosition == "top") // da li da ovdje dodajem i uvjet && form.DefinitionRoot.TranslatorTruncate != true
                truncate = false;

            return Common.Translator.TranslateString(caption, truncate);
        }

        private void InitLayoutGroup(LayoutControlGroup group)
        {
            group.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            group.OptionsTableLayoutGroup.RowDefinitions.Clear();
            group.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
            group.Name = Guid.NewGuid().ToString();
        }

        private void CreateGroups(List<JLayoutGroup> list, LayoutControlGroup parentGroup, bool isRoot)
        {
            DateTime startTime = DateTime.Now;
            if (list != null)
            {
                bool checkIfResult = true;
                foreach (JLayoutGroup item in list)
                {
                    if (isRoot == false && Conditions.CheckCondition(ref checkIfResult, item, form, null) == false)
                        continue;
                    CreateLayoutControlGroup(item, parentGroup);
                }
            }
           // form.Logger.InfoData("CreateGroups", "", startTime);
        }

        private void CreateTabs(List<JLayoutTabs> tabs, LayoutControlGroup parentGroup)
        {
            DateTime startTime = DateTime.Now;
            if (tabs != null)
                foreach (JLayoutTabs tabItem in tabs)
                {
                    TabbedControlGroup tabsNew = CreateTabbedGroup(tabItem, parentGroup);
                    tabsNew.BeginUpdate();

                    if (tabs[0].MultiLine == true)
                        tabsNew.MultiLine = DevExpress.Utils.DefaultBoolean.True;

                    //if (!string.IsNullOrEmpty(tabItem.Name))
                    //    tabsNew.Name = tabItem.Name;

                    bool checkIfResult = true;

                    foreach (JLayoutGroup item in tabItem.GroupList)
                    {
                        if (Conditions.CheckCondition(ref checkIfResult, item, form, null) == false)
                            continue;

                        CreateLayoutControlGroup(item, tabsNew);
                    }

                    SetSelectedTabPageIndex(tabItem.SelectedTabIndex, tabsNew);
                    SetTabEvents(tabsNew, tabItem);
                    form.Logger.InfoData("CreateTabs", "", startTime);
                    tabsNew.EndUpdate();
                }            
        }

        private void SetSelectedTabPageIndex(string selectedTabPageIndex, TabbedControlGroup tabGroup)
        {
            try
            {
                int selectedTabPageIndexInt = 0;
                if (!string.IsNullOrEmpty(selectedTabPageIndex))
                {
                    using (Parser parser = new Parser(form, null))
                    {
                        object selectedtabPageIndexValue = parser.GetLogicPartValue(selectedTabPageIndex.ToString(), null);
                        if (selectedtabPageIndexValue != null)
                        {
                            string x = selectedtabPageIndexValue.ToString();
                            int value;
                            if (int.TryParse(x, out value))
                                selectedTabPageIndexInt = value;
                        }
                    }
                }

                tabGroup.SelectedTabPageIndex = selectedTabPageIndexInt;
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ControlInit.SetSelectedTabPageIndex", ex.Message, ex);
            }
        }

        private void SetTabEvents(TabbedControlGroup tabControl, JLayoutTabs tabItem)
        {
            var onSelectedPageChanged = tabItem.Events.Where(x => x.Type.Contains("selectedtabchanged")).FirstOrDefault();
            if (onSelectedPageChanged != null)
                tabControl.SelectedPageChanged += (s, e) =>
                {
                    using (Logic.Events ev = new Logic.Events(onSelectedPageChanged, null, this.form))
                    {
                        ev.RunNotification();
                    }
                };
        }

        private TabbedControlGroup CreateTabbedGroup(JLayoutTabs group, LayoutControlGroup parentGroup)
        {
            TabbedControlGroup tabbedControlGroup = new TabbedControlGroup();

            if (group.Col != null && group.Row != null)
            {
                using (Parser parser = new Parser(form, null))
                {
                    object objCol = parser.GetLogicPartValue(group.Col, null);
                    if (objCol != null)
                        tabbedControlGroup.OptionsTableLayoutItem.ColumnIndex = Convert.ToInt32(objCol) - 1;

                    object objRow = parser.GetLogicPartValue(group.Row, null);
                    if (objRow != null)
                        tabbedControlGroup.OptionsTableLayoutItem.RowIndex = Convert.ToInt32(objRow) - 1;
                }
            }

            if (group.RowSpan != null && group.RowSpan > 1)
                tabbedControlGroup.OptionsTableLayoutItem.RowSpan = (int)group.RowSpan;
            if (group.ColSpan != null && group.ColSpan > 1)
                tabbedControlGroup.OptionsTableLayoutItem.ColumnSpan = (int)group.ColSpan;

            string itemPadding = GetPaddingStr(group.Padding, PaddingType.Padding, ItemType.Group);
            string itemSpacing = GetPaddingStr(group.Spacing, PaddingType.Spacing, ItemType.Group);

            SetPadding(tabbedControlGroup, itemPadding, PaddingType.Padding, ItemType.Group);
            SetPadding(tabbedControlGroup, itemSpacing, PaddingType.Spacing, ItemType.Group);

            tabbedControlGroup.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.True;
            tabbedControlGroup.Parent = parentGroup;
            return tabbedControlGroup;
        }

        #endregion LayoutGroups

        #region LayoutItems

        private void CreateItems(List<JLayoutItem> list, JLayoutItem itemDefault, LayoutControlGroup parentGroup)
        {
            try
            {
                if (list != null)
                    foreach (JLayoutItem item in list)
                    {
                        CreateLayoutControlItem(item, itemDefault, parentGroup);
                    }
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        private BaseLayoutItem CreateLayoutControlItem(JLayoutItem item, JLayoutItem itemDefault, LayoutControlGroup parentGroup)
        {
            BaseLayoutItem itemNew;
            //if (item.LabelEdit != null)
            //{
            //    itemNew = CreateControl(item) as SimpleLabelItem;
            //    parentGroup.Add(itemNew);
            //}
            //else
            itemNew = new CustomLayoutControlItem(parentGroup);

            (itemNew as CustomLayoutControlItem).AllowHtmlStringInCaption = true;

            itemNew.Name = Guid.NewGuid().ToString();
            //itemNew.TextVisible = false;

            //if (!string.IsNullOrEmpty(item.Spacing))
            //SetPadding(itemNew, item.Spacing, PaddingType.Spacing);
            //itemNew.Spacing = SetSpacing(itemNew, new DevExpress.XtraLayout.Utils.Padding(defaultItemSpacing);

            if (item.FontBold == true)
                itemNew.AppearanceItemCaption.Font = new Font(itemNew.AppearanceItemCaption.Font, FontStyle.Bold);

            if (!string.IsNullOrEmpty(item.TextPosition))
                itemNew.TextLocation = SetTextPosition(item.TextPosition, false);
            else
                itemNew.TextLocation = defaultTextLocation;

            if (item.FontSizeDelta != null)
                itemNew.AppearanceItemCaption.FontSizeDelta = (int)item.FontSizeDelta;

            //itemNew.AppearanceItemCaption.BackColor = Color.FromArgb(240, 240, 240);

            string itemPadding = GetPaddingStr(item.Padding, PaddingType.Padding, ItemType.Item);
            string itemSpacing = GetPaddingStr(item.Spacing, PaddingType.Spacing, ItemType.Item);

            if (item.Container != null || item.Grid != null)
            {
                if (string.IsNullOrEmpty(itemPadding))
                    itemPadding = "0";
                if (string.IsNullOrEmpty(itemSpacing))
                    itemSpacing = "0";
            }
            else if (item.Button != null && itemNew is CustomLayoutControlItem)
            {
                (itemNew as CustomLayoutControlItem).SizeConstraintsType = SizeConstraintsType.Custom;
            }

            //if (item.Padding != null)
            SetPadding(itemNew, itemPadding, PaddingType.Padding, ItemType.Item);

            //if (item.Spacing != null)
            SetPadding(itemNew, itemSpacing, PaddingType.Spacing, ItemType.Item);

            Component component = null;
            bool? isRequired = false;
            if (itemNew is CustomLayoutControlItem)
                component = CreateControl(item, out isRequired);

            if (itemNew is CustomLayoutControlItem)
            {
                if (string.IsNullOrEmpty(item.Caption))
                    itemNew.TextVisible = false;
                else
                {
                    string requiredStr = "";
                    if (isRequired == true)
                        requiredStr = "<color=200, 0, 0>*</color>";
                    Parser p = new Parser(form, item.Caption);
                    object caption = p.GetLogicPartValue(item.Caption);
                    if (caption != null)
                        itemNew.Text = SetLabelCaption(caption.ToString()) + requiredStr;
                    else
                        itemNew.Text = SetLabelCaption(item.Caption.ToString()) + requiredStr;
                }
            }

            if (component is Control && itemNew is CustomLayoutControlItem)
            {
                (itemNew as CustomLayoutControlItem).Control = component as Control;

                (itemNew as CustomLayoutControlItem).HighlightFocusedItemEx = PropertyItemHighlight(item, itemDefault);

                if (!string.IsNullOrEmpty(item.TextAlignMode) && item.TextAlignMode == "auto")
                    (itemNew as CustomLayoutControlItem).TextAlignMode = TextAlignModeItem.AutoSize;

                //if (item.LabelEdit != null)
                //(itemNew as CustomLayoutControlItem).SizeConstraintsType = SizeConstraintsType.Custom;
            }
            else if (component is SimpleLabelItem)
            {
                (itemNew as SimpleLabelItem).TextAlignMode = TextAlignModeItem.AutoSize;
            }

            //itemNew.Control.TabIndex = 0;

            if (item.Col != null && item.Row != null)
            {
                using (Parser parser = new Parser(form, null))
                {
                    object objCol = parser.GetLogicPartValue(item.Col, null);
                    if (objCol != null)
                        itemNew.OptionsTableLayoutItem.ColumnIndex = Convert.ToInt32(objCol) - 1;

                    object objRow = parser.GetLogicPartValue(item.Row, null);
                    if (objRow != null)
                        itemNew.OptionsTableLayoutItem.RowIndex = Convert.ToInt32(objRow) - 1;
                }
            }

            if (item.RowSpan != null && item.RowSpan > 1)
                itemNew.OptionsTableLayoutItem.RowSpan = (int)item.RowSpan;
            if (item.ColSpan != null && item.ColSpan > 1)
                itemNew.OptionsTableLayoutItem.ColumnSpan = (int)item.ColSpan;

            itemNew.Parent = parentGroup;

            return itemNew;
        }

        private bool PropertyItemHighlight(JLayoutItem item, JLayoutItem itemDefault)
        {
            if (item.Highlight != null)
                return (bool)item.Highlight;

            if (item.Container != null ||
                item.Button != null ||
                item.LabelEdit != null)
                return false;

            if (itemDefault != null && itemDefault.Highlight == false)
                return false;

            return true;
        }

        #endregion LayoutItems

        #region LayoutSpacing/Padding

        private void SetPadding(BaseLayoutItem item, string spacing, PaddingType paddingType, ItemType itemType)
        {
            string[] values = spacing.Split(new string[] { "," }, StringSplitOptions.None);
            bool allNumeric = Array.TrueForAll(values, value =>
            {
                int s;
                return Int32.TryParse(value.Substring(value.Length - 1), out s);
            });

            if (allNumeric == false)
                return;
            DevExpress.XtraLayout.Utils.Padding padding = new DevExpress.XtraLayout.Utils.Padding();
            if (values.Length == 1)
            {
                padding = new DevExpress.XtraLayout.Utils.Padding(Convert.ToInt32(values[0]));
            }
            else if (values.Length == 4)
            {
                padding = new DevExpress.XtraLayout.Utils.Padding(
                    GetPadding(Convert.ToInt32(values[0]), paddingType, itemType),
                    GetPadding(Convert.ToInt32(values[1]), paddingType, itemType),
                    GetPadding(Convert.ToInt32(values[2]), paddingType, itemType),
                    GetPadding(Convert.ToInt32(values[3]), paddingType, itemType)
               );
            }

            //if (typeof(TabbedControlGroup) == item.GetType() || item.IsGroup == true)
            //    SetPadding(item, padding);
            //else
            //    SetSpacing(item, padding);

            if (paddingType == PaddingType.Padding)
                SetPadding(item, padding);
            else
                SetSpacing(item, padding);
        }

        private string GetPaddingStr(string value, PaddingType paddingType, ItemType itemType)
        {
            int defPadd = GetDefaultPadding(paddingType, itemType);

            if (string.IsNullOrEmpty(value))
                return defPadd.ToString();

            return value;
        }

        private int GetPadding(int value, PaddingType paddingType, ItemType itemType)
        {
            if (value < 0)
                return GetDefaultPadding(paddingType, itemType);

            return value;
        }

        private int GetDefaultPadding(PaddingType paddingType, ItemType itemType)
        {
            int defPadd = 0;
            if (paddingType == PaddingType.Spacing && itemType == ItemType.Item)
                defPadd = defaultItemSpacing;
            else if (paddingType == PaddingType.Spacing && itemType == ItemType.Group)
                defPadd = defaultGroupSpacing;
            else if (paddingType == PaddingType.Padding && itemType == ItemType.Item)
                defPadd = defaultItemPadding;
            else if (paddingType == PaddingType.Padding && itemType == ItemType.Group)
                defPadd = defaultGroupPadding;

            return defPadd;
        }

        private void SetPadding(BaseLayoutItem item, DevExpress.XtraLayout.Utils.Padding padding)
        {
            item.Padding = padding;
        }

        private void SetSpacing(BaseLayoutItem item, DevExpress.XtraLayout.Utils.Padding padding)
        {
            item.Spacing = padding;
        }

        #endregion LayoutSpacing/Padding

        #region ControlContainer

        private Control CreateContainerControl(JLayoutItem item, out bool? isRequired)
        {
            isRequired = false;
            Panel panel = new Panel() { Padding = new Padding(0), BorderStyle = BorderStyle.None };

            JButtons buttons = item.Container.Buttons;
            if (buttons != null)
            {
                List<JButton> buttonList = item.Container.Buttons.ButtonList;
                if (buttonList != null)
                {
                    foreach (JButton buttonDef in buttonList)
                    {
                        panel.Controls.Add(CreateButtonSpacer(4, buttonDef));
                        panel.Controls.Add(CreateButtonControl(buttonDef));
                    }
                }
            }
            return panel;
        }

        private JButtonControl CreateButtonControl(JButton def)
        {
            JButtonControl button = new JButtonControl(def)
            {
                Name = Guid.NewGuid().ToString(),
                Width = 65,
            };

            if (def.Dock != null && def.Dock == "right")
            {
                button.Dock = DockStyle.Right;
            }
            else
            {
                button.Dock = DockStyle.Left;
            }

            if (def.Caption != null)
                button.Text = def.Caption;

            button.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            button.Font = new Font(button.Font.FontFamily, 7F);
            return button;
        }

        private Panel CreateButtonSpacer(int spaceWidth, JButton def)
        {
            Panel panel = new Panel()
            {
                Width = 4,
                BorderStyle = BorderStyle.None
            };
            if (def.Dock != null && def.Dock == "right")
                panel.Dock = DockStyle.Right;
            else
                panel.Dock = DockStyle.Left;
            return panel;
        }

        #endregion ControlContainer

        #region ControlCollections

        private JButtonControl GetJButtonControl(JButton definition)
        {
            JButtonControl control = new Components.JButtonControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JSearchControl GetSearchEditControl(JSearchEdit definition)
        {
            JSearchControl control = new Components.JSearchControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private Component GetDateEditControl(JDateEdit definition)
        {
            JDateEditControl control = new JDateEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JTextEditControl GetTextEditControl(JTextEdit definition)
        {
            JTextEditControl control = new JTextEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JHyperLinkEditControl GetHyperLinkEditControl(JHyperlinkEdit definition)
        {
            JHyperLinkEditControl control = new JHyperLinkEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JCheckEditControl GetCheckEditControl(JCheckEdit definition)
        {
            JCheckEditControl control = new JCheckEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JRadioEditControl GetRadioEditControl(JRadioEdit definition)
        {
            JRadioEditControl control = new JRadioEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JMemoEditControl GetMemoEditControl(JMemoEdit definition)
        {
            JMemoEditControl control = new JMemoEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }
        private JRichEditControl GetRichEditControl(JRichEdit definition)
        {
            JRichEditControl control = new JRichEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }
        private JLookUpControl GetLookUpControl(JComboEdit definition, string connectionString, List<Structure.Models.ControlEvents> eventList)
        {
            JLookUpControl control = new JLookUpControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
            //control.Notifications = notificationsList;
        }

        private JAutoSuggestControl GetAutoSuggestControl(JAutoSuggest definition, string connectionString, List<Structure.Models.ControlEvents> eventList)
        {
            JAutoSuggestControl control = new JAutoSuggestControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
            //control.Notifications = notificationsList;
        }

        private JComboMultiEditControl GetComboMultiEditControl(JComboMultiEdit definition, string connectionString, List<Structure.Models.ControlEvents> eventList)
        {
            JComboMultiEditControl control = new JComboMultiEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JTokenEditControl GetTokenEditControl(JTokenEdit definition)
        {
            JTokenEditControl control = new JTokenEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JTreeLookUpControl GetTreeLookUpControl(JComboTreeEdit definition, string connectionString, List<Structure.Models.ControlEvents> eventList)
        {
            JTreeLookUpControl control = new JTreeLookUpControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
            //control.Notifications = notificationsList;
        }

        private JButtonEditControl GetTextButtonControl(JButtonTextEdit definition)
        {
            JButtonEditControl control = new JButtonEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JLabelEditControl GetLabelControl(JLabelEdit definition)
        {
            JLabelEditControl control = new JLabelEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JDaysEditControl GetDaysEditControl(JDaysEdit definition)
        {
            JDaysEditControl control = new JDaysEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JBrowseEditControl GetBrowseEditControl(JBrowseEdit definition)
        {
            JBrowseEditControl control = new JBrowseEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JChartControl GetChartControlControl(JChart definition)
        {
            JChartControl control = new JChartControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private JSeparatorControl GetSeparatorControl(JSeparator definition)
        {
            JSeparatorControl control = new JSeparatorControl(definition);
            control.TabStop = false;
            return control;
        }

        public JGridControl GetGridControl(JGrid definition)
        {
            JGridControl control = new JGridControl(definition);
            AddToControlList(control, definition);
            return control;
        }

        public JTreeListControl GetTreeControl(JTree definition)
        {
            JTreeListControl control = new JTreeListControl(definition);
            AddToControlList(control, definition);
            return control;
        }

        private Component GetTimeEditControl(JTimeEdit definition)
        {
            JTimeEditControl control = new JTimeEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private Component GetPictureEditControl(JPictureEdit definition)
        {
            JPictureEditControl control = new JPictureEditControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        private Component GetPdfViewerControl(JPdfViewer definition)
        {
            JPdfViewerControl control = new JPdfViewerControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }
        private Component GetDocumentViewerControl(JDocumentViewer definition)
        {
            var control = new JDocumentViewControl(definition);
            AddToControlList(control, definition);
            SetNotification(definition, control);
            return control;
        }

        //private Component GetChartControl(JChart definition)
        //{
        //    JChartControl control = new JChartControl(definition);
        //    AddToControlList(control, definition);
        //    SetNotification(definition, control);
        //    return control;
        //}

        private void AddToControlList(Object control, IJModelEditorBase definition)
        {
            if (control is Control)
            {
                bool isNameDefined = false;
                string controlName = "";
                if (definition != null)
                    if (!string.IsNullOrEmpty(definition.Name))
                    {
                        isNameDefined = true;
                        controlName = definition.Name;
                    }
                    else
                        controlName = Guid.NewGuid().ToString();

                bool saveValueSession = (definition.SaveValue == "session");
                bool saveValueApp = (definition.SaveValue == "app");

                form.ControlList.Add(new Structure.Models.JControl() { ControlName = definition.Name, Control = control as Control, ControlType = control.GetType(), IsNameDefinied = isNameDefined, SaveValueSession = saveValueSession, SaveValueApp = saveValueApp });
            }
        }

        private void SetNotification(JModelEditorDataBase definition, Control control)
        {
            if (definition.Event != null && definition.Event.Count > 0)
                form.EventList.Add(new Structure.Models.ControlEvents() { Control = control, Events = definition.Event });
        }

        #endregion ControlCollections
    }
}