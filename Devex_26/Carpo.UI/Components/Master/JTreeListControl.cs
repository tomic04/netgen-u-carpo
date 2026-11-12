using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JTreeListControl : TreeList, IJControl, INotifyPopulate, IControlMenu, IFocusRow, IJDataRow, INotifyPrint
    {
        public Base.ComponentTree ComponentExt { get; set; }
        public JTree Definition { get; set; }

        public DataRow DataRowRef { get; set; } //foreachrow; u pripremi; za sada ne koristim

        public ImageCollection ImgCollection { get; set; }

        //public ContextMenuStrip MenuDragDrop { get; set; } = new ContextMenuStrip();

        private List<JMenuItemList> menuItems;

        public List<JMenuItemList> MenuItems
        {
            get
            {
                return menuItems;
            }
            set
            {
                if (!DesignMode)
                    menuItems = value;
                else
                    menuItems = null;
            }
        }

        public ContextMenuStrip ControlMenu { get; set; } = new ContextMenuStrip();

        //protected override bool DoFilterNode(DevExpress.XtraTreeList.Nodes.TreeListNode node)
        //{
        //    node.Visible = true;
        //    return true;
        //    if (node.Level == 0 && (base.GetNodeIndex(node) == 0 | base.GetNodeIndex(node) == 1 | base.GetNodeIndex(node) == 2))
        //    {
        //        node.Visible = true;
        //        return false;
        //    }
        //    else
        //    {
        //        if (node.Nodes != null && node.Nodes.Count > 0)
        //        {
        //            foreach (TreeListNode childNode in node.Nodes)
        //            {
        //                if (!DoFilterNode(childNode))
        //                {
        //                    node.Visible = true;
        //                    return false;
        //                }
        //            }
        //        }
        //        //Else
        //        //    If node.ParentNode IsNot Nothing AndAlso Not MyBase.DoFilterNode(node.ParentNode) Then
        //        //        'node.Visible = True
        //        //        node.ParentNode.Visible = True
        //        //        Return False
        //        //    End If
        //    }

        //    return base.DoFilterNode(node);
        //}

        public JTreeListControl()
        {
        }

        public JTreeListControl(JTree definition)
        {
            this.Definition = definition;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                if (ComponentExt == null)
                    ComponentExt = new Base.ComponentTree(this, Definition);

                ImgCollection = new ImageCollection();
                ImgCollection.AddImage(Properties.Resources.circle_gray_16);
                ImgCollection.AddImage(Properties.Resources.circle_yellow_16);
                ImgCollection.AddImage(Properties.Resources.today_16);
                ImgCollection.AddImage(Properties.Resources.week_16);
                ImgCollection.AddImage(Properties.Resources.yesterday_16);
                ImgCollection.AddImage(Properties.Resources.tommorow_16);

                this.SelectImageList = ImgCollection;
            }
        }

        //poziv s master forme
        public void SetControl(JTree definition)
        {
            if (this.Definition == null)
                this.Definition = definition;

            if (ComponentExt == null)
                ComponentExt = new Base.ComponentTree(this, definition);
        }

        public void SetSearchField(string searchField)
        {
            ComponentExt.SearchField = searchField;
        }

        #region IJControl

        public void SetEvents(object ev)
        {
            if (ev is List<JEvent>)
                ComponentExt.SetEvents(ev as List<JEvent>);
            else if (ev is JEvent)
                ComponentExt.SetEvents(ev as JEvent);
        }

        public void SetNotifications(JEventAction action, object value)
        {
            ComponentExt.SetNotify(action, value);
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void Populate(JEventAction action, bool force, bool isAsync, FocusType focusType)
        {
            bool saveState = false, additionalFocus = false;

            if (focusType == FocusType.KeepFocus)
                saveState = true;

            if (action != null && action.Actions.Count > 0)
            {
                if (action.Actions[0].Type == "control focus" || action.Actions[0].Type == "control expand")
                    additionalFocus = true;
            }

            // B8: honor populate async flag or tree async="true"
            bool useAsync = isAsync || (Definition != null && Definition.Async == true);
            ComponentExt.PopulateTreeList(saveState, additionalFocus, useAsync);
        }

        public object GetValue(string field, List<string> helperStr)
        {
            return ComponentExt.GetValue(field, helperStr);
        }

        public int GetNodeLevel(List<string> helperStr)
        {
            return ComponentExt.GetNodeLevel(helperStr);
        }

        public void ForceFocusedNodeChanged()
        {
            ComponentExt.FocusedNodeChanged(false);
        }

        public void SetFocus(JEventAction action)
        {
            ComponentExt.SetFocus(action);
        }

        public void SetNotifyForEachRow(JEventAction action, bool onlySelected)
        {
            throw new NotImplementedException();
        }

        public object GetSelectedIds(List<string> helperStr)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataSource()
        {
            return ComponentExt.GetDataSource();
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            return ComponentExt.GetValueSpecial(name, helperStr);
        }

        public void SetMode(string mode)
        {
            ComponentExt.SetMode(mode);
        }

        public void Print(JEventAction action, bool selected)
        {
            throw new NotImplementedException();
        }

        public void PrintControl(bool selected)
        {
            ComponentExt.PrintControl(selected);
        }

        public void SetVisibility(JEventAction action)
        {
            ComponentExt.SetVisibility(action);
        }

        public void RefreshValue()
        {
            ComponentExt.RefreshValue();
        }

        public void SetAttribute(JEventAction action)
        {
            ComponentExt.SetAttribute(action);
        }

        public void SearchDoc(DataRow dr)
        {
            ComponentExt.SearchDoc(dr);
        }

        public void SetExpand(JEventAction action)
        {
            ComponentExt.SetExpand(action);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }

        #endregion IJControl
    }
}