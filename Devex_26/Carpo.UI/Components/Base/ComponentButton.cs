using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using Carpo.UI.Logic;
using Carpo.UI.Properties;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentButton : ComponentBase
    {
        private JButtonControl dropDownButton;
        private PopupMenu popupMenu;

        //private bool isDark = false;

        private new JButton definition;

        public string SearchText { get; set; }

        public ComponentButton() : base()
        {
        }

        public ComponentButton(Control control, object def) : base(control, def)
        {
            definition = def as JButton;
            dropDownButton = (JButtonControl)control;
            Init();
        }

        private void Init()
        {
            dropDownButton.AutoWidthInLayoutControl = true;

            if (string.IsNullOrEmpty(definition.Type))
                dropDownButton.Tag = definition.Type;

            dropDownButton.DropDownArrowStyle = DropDownArrowStyle.Hide;

            //SetDropDown();

            //****create simplebutton from dropdown
            //dropDownButton.ActAsDropDown = false;
            //***

            if (definition.Width != null)
                dropDownButton.Width = (int)definition.Width;
            else
                dropDownButton.Width = 90;

            dropDownButton.Font = new System.Drawing.Font(dropDownButton.Font.FontFamily, 7F, System.Drawing.FontStyle.Bold);
            dropDownButton.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            dropDownButton.Text = Common.Translator.TranslateString(definition.Caption);
            //simpleButton.Dock = DockStyle.Fill;

            dropDownButton.AutoWidthInLayoutControl = true;

            SetSkin(definition.Enabled);

            if (definition.FontSizeDelta != null)
                dropDownButton.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;

            if (!string.IsNullOrEmpty(definition.Type) && definition.Type.ToLower() == "cancel")
                OnCancel();

            //if (definition.FontSizeDelta != null)
            //    dropDownButton.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;

            //if (definition.FontBold == true)
            //    dropDownButton.Appearance.Font = new System.Drawing.Font(dropDownButton.Appearance.Font, System.Drawing.FontStyle.Bold);

            SetImage();
            SetEvents();
        }

        private void SetEvents()
        {
            control.EnabledChanged += (s, e) =>
            {
                SetSkin(control.Enabled);
            };

            dropDownButton.Click += (s, e) =>
            {
                if (definition.Event.Count == 0)
                {
                    bool fromDataSet = false;
                    if (!string.IsNullOrEmpty(definition.DataName))
                        fromDataSet = true;
                    SetDropDownMenu(fromDataSet);
                }
            };
        }

        private void SetSkin(bool? enabled)
        {
            //if ((definition.Color == "dark" || definition.Type == "add") && definition.Color != "light" && enabled != false)
            //{
            //    //dropDownButton.LookAndFeel.SkinName = "DevExpress Dark Style";
            //    dropDownButton.LookAndFeel.UseDefaultLookAndFeel = false;
            //    dropDownButton.LookAndFeel.SkinMaskColor = Color.DarkBlue;
            //    //isDark = true;
            //}
            //else
            //{
            //    dropDownButton.LookAndFeel.UseDefaultLookAndFeel = true;
            //    //isDark = false;
            //}
        }

        //private void ShowDropDownControl(object sender, DevExpress.XtraEditors.ShowDropDownControlEventArgs e)
        //{
        //    SetDropDown();
        //}

        private void SetDropDown()
        {
            if (definition.DataName != null)
            {
                //SetDropDownMenu(true);
                return;
            }

            if (definition.DropDownItemList == null || definition.DropDownItemList.Count == 0)
            {
                dropDownButton.ActAsDropDown = false;
            }
            else
            {
                SetDropDownMenu(false);
            }
        }

        private DataTable GetData(JModelEditorButton definitionItem)
        {
            Cursor.Current = Cursors.WaitCursor;
            //gridView1.BeginUpdate();
            try
            {
                bool stopActions = false;
                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definitionItem);
                DataSet ds = dataDefinition.GetDataSet("punjenje dropDown buttona '" + definitionItem.Name + "'", out stopActions);
                if (ds != null && ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            catch { throw; }
            finally
            {
                //gridView1.EndUpdate();
                Cursor.Current = Cursors.Default;
                Action = "";
            }
            return null;
        }

        public void SetDropDownMenu(bool fromDataSet)
        {
            dropDownButton.DropDownControl = null;

            dropDownButton.ActAsDropDown = true;
            popupMenu = new PopupMenu();
            BarManager barManager = null;
            if (form is Forms.Base.JupiterShared)
            {
                barManager = (form as Forms.Base.JupiterShared).barManagerMain;

                popupMenu.Manager = barManager;
                if (fromDataSet == true)
                    SetDropDownDataSetItems(definition, null);
                else
                    SetDropDownFixedItems(null, definition.DropDownItemList);

                dropDownButton.DropDownControl = popupMenu;
            }
        }

        private void SetDropDownFixedItems(BarSubItem subItem, List<JButtonDropDownItem> dropDownItems)
        {

            bool checkIfResult = true;

            foreach (JButtonDropDownItem dropDownItem in dropDownItems)
            {
                if (Conditions.CheckCondition(ref checkIfResult, dropDownItem, form, null, false) == false)
                    continue;


                if (!string.IsNullOrEmpty(dropDownItem.DataName) && !string.IsNullOrEmpty(dropDownItem.DisplayField))
                {
                    BarSubItem sItem = new BarSubItem(popupMenu.Manager, dropDownItem.Caption);
                    SetDropDownDataSetItems(dropDownItem, sItem);
                    popupMenu.AddItem(sItem);
                }
                else if (dropDownItem.DropDownItemList != null && dropDownItem.DropDownItemList.Count > 0)
                {
                    BarSubItem sItem = new BarSubItem(popupMenu.Manager, dropDownItem.Caption);
                    if (subItem != null)
                        subItem.AddItem(sItem);
                    else
                        popupMenu.AddItem(sItem);
                    SetDropDownFixedItems(sItem, dropDownItem.DropDownItemList);
                }
                else
                {
                    BarButtonItem item = new BarButtonItem(popupMenu.Manager, dropDownItem.Caption);
                    //item.Enabled = false;
                    SetBarButtonItem(item, dropDownItem);
                    AddItemToMenu(subItem, item);
                }
            }
        }

        private void SetBarButtonItem(BarButtonItem item, JButtonDropDownItem dropDownItem)
        {
            if (!string.IsNullOrEmpty(dropDownItem.Name))
            {
                item.Name = dropDownItem.Name;
                form.BarButtonItems.Add(item);
            }
            item.Glyph = Properties.Resources.right_menu_16;
            item.ItemClick += (s, e) =>
            {
                if (dropDownItem.Event.Count > 0)
                    OnClick(dropDownItem.Event[0], item);
            };
        }

        private void AddItemToMenu(BarSubItem subItem, BarButtonItem item)
        {
            if (subItem != null)
                subItem.AddItem(item);
            else
                popupMenu.AddItem(item);
        }

        private string ValueParserToString(string valueStr)
        {
            using (Parser parser = new Parser(form, null))
            {
                object value = parser.GetLogicPartValue(valueStr);
                if (value != null)
                    return value.ToString();
                else
                    return valueStr;
            }
        }

        private void SetDropDownDataSetItems(JModelEditorButton itemDefinition, BarSubItem barSubItem)
        {
            DataTable dt = GetData(itemDefinition);
            if (dt == null)
                return;
            if (dt.Columns[itemDefinition.DisplayField] == null)
            {
                form.Logger.Warn("control: " + control.Name + " - punjenje menia", "Polje '" + itemDefinition.DisplayField + "' nije pronađeno");
                return;
            }

            string oneItemPopup = definition.OneItemPopup != null ? ValueParserToString(definition.OneItemPopup) : "false";
            string treeStructure = definition.TreeStructure != null ? ValueParserToString(definition.TreeStructure) : "false";

            if (dt.Rows.Count == 0)
                return;
            else if (dt.Rows.Count == 1 && oneItemPopup.ToLower() != "true")
            {
                using (BarButtonItem item = SetBarButtonItem(dt.Rows[0], popupMenu.Manager, itemDefinition.DisplayField) as BarButtonItem)
                {
                    OnClick(itemDefinition.Event[0], item);
                }
                return;
            }
            else
            {
                if (treeStructure.ToLower() == "true")
                    CreateTreeButtonDataSet(itemDefinition, dt);
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        BarButtonItem item = SetBarButtonItem(dr, popupMenu.Manager, itemDefinition.DisplayField) as BarButtonItem;
                        if (itemDefinition.Event != null && itemDefinition.Event.Count > 0)
                        {
                            item.ItemClick += (s, e) =>
                            {
                                OnClick(itemDefinition.Event[0], item);
                            };
                        }

                        if (barSubItem != null)
                            barSubItem.AddItem(item);
                        else
                            popupMenu.AddItem(item);
                    }
                }
            }
        }

        private void CreateTreeButtonDataSet(JModelEditorButton itemDefinition, DataTable dt)
        {
            try
            {
                List<MenuItemTreeStructure> buttonList = new List<MenuItemTreeStructure>();

                if (!dt.Columns.Contains("id") || !dt.Columns.Contains("parentId") || !dt.Columns.Contains("displayField") || !dt.Columns.Contains("clickInd"))
                {
                    form.Logger.Warn("CreateTreeButtonDataSet", "Neispravne kolone dataseta koji kreira strukturu 'menuitema' na ''button' kontroli");
                    return;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    MenuItemTreeStructure buttonModel = new MenuItemTreeStructure();
                    buttonModel.Id = (Int32)dr["id"];
                    buttonModel.ParentId = (Int32)dr["parentId"];
                    buttonModel.DisplayField = dr["displayField"].ToString();
                    buttonModel.ClickInd = (Int32)dr["clickInd"];

                    buttonList.Add(buttonModel);
                }

                List<MenuItemTreeStructure> sortedList = buttonList.OrderBy(x => x.ParentId).ThenBy(x => x.DisplayField).ToList();

                int minParentId = sortedList[0].ParentId;

                dt.PrimaryKey = new DataColumn[] { dt.Columns["id"] };

                foreach (var button in sortedList.Where(o => o.ParentId == minParentId))
                {
                    var childList = GetChild(buttonList, button.Id);
                    var ind = button.ClickInd;

                    object barSubTemp = CreateBarButtonItem(dt, button, itemDefinition, null, buttonList, (childList.Count != 0));

                    if (childList.Count != 0)
                    {
                        AddSubButton(childList, buttonList, popupMenu, barSubTemp as BarSubItem, dt, itemDefinition);
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("CreateTreeButtonDataSet", ex.Message, ex);
            }
        }

        private void AddSubButton(List<MenuItemTreeStructure> childList, List<MenuItemTreeStructure> buttonList, PopupMenu popupMenu, BarSubItem sItem, DataTable dt, JModelEditorButton itemDefinition)
        {
            foreach (var button in childList)
            {
                var childChildList = GetChild(buttonList, button.Id);

                object barSubTemp = CreateBarButtonItem(dt, button, itemDefinition, sItem, buttonList, (childChildList.Count != 0));

                if (childChildList.Count != 0)
                {
                    var childsList = GetChild(buttonList, button.Id);
                    AddSubButton(childsList, buttonList, popupMenu, barSubTemp as BarSubItem, dt, itemDefinition);
                }
            }
        }

        private object CreateBarButtonItem(DataTable dt, MenuItemTreeStructure button, JModelEditorButton itemDefinition, BarSubItem sItem, List<MenuItemTreeStructure> buttonList, bool childExist)
        {
            DataRow dtr = dt.Rows.Find(button.Id);
            var item = SetBarButtonItem(dtr, popupMenu.Manager, "displayField", childExist);
            SetItemClickEvent(item, itemDefinition, button.ClickInd);
            if (sItem != null)
            {
                if (item is BarButtonItem)
                    sItem.ItemLinks.Add(item as BarButtonItem);
                else if (item is BarSubItem)
                    sItem.ItemLinks.Add(item as BarSubItem);
            }
            else
            {
                if (item is BarButtonItem)
                    popupMenu.AddItem(item as BarButtonItem);
                else if (item is BarSubItem)
                    popupMenu.AddItem(item as BarSubItem);
            }
            buttonList.Remove(button);

            if (item is BarSubItem)
                return (item as BarSubItem);
            else
                return null;
        }

        private List<MenuItemTreeStructure> GetChild(List<MenuItemTreeStructure> buttonList, int idOfParent)
        {
            var childList = new List<MenuItemTreeStructure>();

            foreach (var button in buttonList)
            {
                if (button.ParentId == idOfParent)
                    childList.Add(button);
            }
            return childList;
        }

        private void SetItemClickEvent(object item, JModelEditorButton itemDefinition, int clickInd)
        {
            if (clickInd == 1 && itemDefinition.Event != null && itemDefinition.Event.Count > 0)
            {
                if (item is BarSubItem)
                {
                    (item as BarSubItem).ItemClick += (s, e) =>
                    {
                        OnClick(itemDefinition.Event[0], item);
                    };
                }
                else
                {
                    (item as BarButtonItem).ItemClick += (s, e) =>
                    {
                        OnClick(itemDefinition.Event[0], item);
                    };
                }
            }
        }

        private object SetBarButtonItem(DataRow dr, BarManager barManager, string displayField, bool isBarSubItem = false)
        {
            string caption = "NULL";

            if (dr[displayField] != null)
            {
                caption = Common.Translator.TranslateString(dr[displayField].ToString());
            }
            if (isBarSubItem == false)
            {
                BarButtonItem item = new BarButtonItem(popupMenu.Manager, caption);
                item.Glyph = Properties.Resources.right_menu_16;
                item.Tag = new Structure.Models.JBarButtonItemTag() { DataRowRef = dr, ControlRef = control };
                return item;
            }
            else
            {
                BarSubItem item = new BarSubItem(popupMenu.Manager, caption);
                item.Glyph = Properties.Resources.right_menu_16;
                item.Tag = new Structure.Models.JBarButtonItemTag() { DataRowRef = dr, ControlRef = control };
                return item;
            }
        }

        private void SetImage()
        {
            string imageStr = definition.Image;
            if (string.IsNullOrEmpty(imageStr))
                return;

            imageStr = imageStr.ToLower();
            if (!string.IsNullOrEmpty(imageStr))
            {
                Image img = null;

                switch (imageStr)
                {
                    case "add": img = Resources.add_12; break; //promijeniti
                    case "bar-chart": img = Resources.icon_bar_chart; break;
                    case "bottom": img = Resources.icon_bottom; break;
                    case "calendar": img = Resources.icon_calendar; break;
                    case "calculator": img = Resources.icon_calculator; break;
                    case "car": img = Resources.icon_car; break;
                    case "check": img = Resources.icon_check; break;
                    case "clock": img = Resources.icon_clock; break;
                    case "edit": img = Resources.edit_12; break; //promijeniti
                    case "home": img = Resources.icon_home; break;
                    case "hourglass": img = Resources.icon_hourglass; break;
                    case "delete": img = Resources.delete_12; break; //promijeniti
                    case "left": img = Resources.left; break; //promijeniti
                    case "leftbegin": img = Resources.leftbegin; break; //promijeniti
                    case "link": img = Resources.icon_link; break;
                    case "list": img = Resources.icon_list; break;
                    case "mail": img = Resources.icon_mail; break;
                    case "money": img = Resources.icon_money; break;
                    case "next": img = Resources.icon_next; break;
                    case "next2": img = Resources.next_gray_16; break; //promijeniti
                    case "note": img = Resources.icon_note; break;
                    case "pie-chart": img = Resources.icon_pie_chart; break;
                    case "pie-chart2": img = Resources.icon_pie_chart2; break;
                    case "preview": img = Resources.icon_preview; break;
                    case "previous": img = Resources.icon_previous; break;
                    case "printer": img = Resources.icon_printer; break;
                    case "refresh": img = Resources.icon_refresh; break;
                    case "return": img = Resources.icon_return; break;
                    case "right": img = Resources.right; break; //promijeniti
                    case "rightend": img = Resources.rightend; break; //promijeniti
                    case "search": img = Resources.icon_search; break;
                    case "star": img = Resources.icon_star; break;
                    case "stats": img = Resources.icon_stats; break;
                    case "sum": img = Resources.icon_sum; break;
                    case "top": img = Resources.icon_top; break;
                    case "truck": img = Resources.icon_truck; break;
                }

                if (img != null)
                {
                    dropDownButton.Image = img;
                    dropDownButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.TopCenter;
                    SetImageLocation();
                }
            }

            //if (imageStr == "next") // JS offers style
            //    dropDownButton.Image = Resources.next_gray_16;
            //else //common action buttons; CRUD operaterions
            //{
            //    if (imageStr == "add")
            //    {
            //        //if (isDark == true)
            //        //    dropDownButton.Image = Resources.add_w_12;
            //        //else
            //        dropDownButton.Image = Resources.add_12;
            //    }
            //    else if (imageStr == "edit")
            //    {
            //        //if (isDark == true)
            //        //    dropDownButton.Image = Resources.edit_w_12;
            //        //else
            //        dropDownButton.Image = Resources.edit_12;
            //    }
            //    else if (imageStr == "delete")
            //    {
            //        //if (isDark == true)
            //        //    dropDownButton.Image = Resources.delete_w_12;
            //        //else
            //        dropDownButton.Image = Resources.delete_12;
            //    }
            //    else if (imageStr == "left")
            //        dropDownButton.Image = Resources.left;
            //    else if (imageStr == "leftbegin")
            //        dropDownButton.Image = Resources.leftbegin;
            //    else if (imageStr == "right")
            //        dropDownButton.Image = Resources.right;
            //    else if (imageStr == "rightend")
            //        dropDownButton.Image = Resources.rightend;
            //    dropDownButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.TopCenter;
            //}
        }

        private void SetImageLocation()
        {
            string imageLocation = definition.ImageLocation;
            if (string.IsNullOrEmpty(imageLocation))
                return;

            dropDownButton.ImageLocation = Common.Helpers.Images.GetDxImageLocationByString(imageLocation);

            if (dropDownButton.ImageLocation == DevExpress.XtraEditors.ImageLocation.MiddleCenter)
            {
                dropDownButton.ToolTip = dropDownButton.Text;
            }
        }

        public object GetValue(string field)
        {
            if (dropDownButton.DataRowRef != null)
                return dropDownButton.DataRowRef[field];
            else
                return null;
        }

        private void OnCancel()
        {
            dropDownButton.Click += (s, e) =>
            {
                if (form.IsMaster == true)
                    form.Close();
                else
                    form.DialogResult = DialogResult.Cancel;
            };
        }
    }
}