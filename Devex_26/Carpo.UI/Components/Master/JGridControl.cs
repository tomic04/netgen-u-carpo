
using Carpo.Core;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    public partial class JGridControl : GridControl, IJControl, INotifyPopulate, IControlMenu, IJDataRow, INotifyPrint, IFocusRow, IJDataSource
    {
        public Base.ComponentGrid ComponentExt { get; set; }

        public enum Property { ShowFooter, SumsInline, SumsUnder, SumsIfExpanded }
        
        public JGrid Definition { get; set; }

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

        public DataRow DataRowRef { get; set; } //foreachrow

        public JGridControl()
        {
        }

        public JGridControl(JGrid definition)
        {
            this.Definition = definition;
        }

        protected override void OnCreateControl()
        {            
            base.OnCreateControl();
            if (Definition != null && ComponentExt == null)
            {
                ComponentExt = new Base.ComponentGrid(this, Definition);
            }

            if (MenuItems == null)
                MenuItems = new List<JMenuItemList>();            
        }

        //poziv s master forme
        public void SetControl(JGrid definition)
        {            
            if (Definition == null)
                this.Definition = definition;
            if (MenuItems == null)
                MenuItems = new List<JMenuItemList>();
            if (ComponentExt == null)
                ComponentExt = new Base.ComponentGrid(this, Definition);            
        }

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

        public void SetNotificationsForEachRow(JEventAction action, bool onlySelected)
        {
            ComponentExt.SetNotifyForEachRow(action, onlySelected);
        }

        public void Print(JEventAction action, bool selected)
        {
            ComponentExt.Print(action, selected);
        }

        public void PrintControl(bool selected)
        {
            ComponentExt.PrintControl(selected);
        }

        public void Populate(JEventAction action, bool force, bool async, FocusType keepFocus)
        {            
            ComponentExt.Populate(action, force, async, keepFocus);            
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void SetNotifyForEachRow(JEventAction action, bool onlySelected)
        {
            ComponentExt.SetNotifyForEachRow(action, onlySelected);
        }

        public object GetValue(string field, List<string> helperStr)
        {
            return ComponentExt.GetValue(field, helperStr);
        }

        public object GetSelectedIds(List<string> helperStr)
        {
            return ComponentExt.GetSelectedIds(helperStr);
        }

        public void SetFocus(JEventAction action)
        {
            ComponentExt.SetFocus(action);
        }

        public void SetFocusNextRow(JEventAction action)
        {
            ComponentExt.SetFocusNextRow(action);
        }

        public void SetFocusLastRow(JEventAction action)
        {
            ComponentExt.SetFocusLastRow(action);
        }

        public void SetAttribute(JEventAction action)
        {
            ComponentExt.SetAttribute(action);
        }

        public DataTable GetDataSource()
        {
            if (this.DataSource is DataTable)
                return this.DataSource as DataTable;
            return null;
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            return ComponentExt.GetValueSpecial(name, helperStr);
        }

        public void SetVisibility(JEventAction action)
        {
            ComponentExt.SetVisibility(action);
        }

        public void SetFilter(JEventAction action)
        {
            ComponentExt.SetFilter(action);
        }

        public void RefreshValue()
        {
            ComponentExt.RefreshValue();
        }

        public void RefreshRow(JEventAction action)
        {
            ComponentExt.RefreshRow(action);
        }

        public void SearchDoc()
        {
            ComponentExt.SearchDoc();
        }

        public void SetExport(JEventAction action)
        {
            ComponentExt.SetExport(action);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }

        public void SetSelectedRows(JEventAction action)
        {
            ComponentExt.SetSelectedRows(action);
        }

        public void ClearDataSource(JEventAction action)
        {
            this.DataSource = null;

            ComponentExt.ClearTabCaption();

            if (action.Type.ToLower() == "setlayout")
            {
                ComponentExt.ClearColumns();
                return;
            }
            if (action.Parameter.Count > 0)
            {
                var par = action.Parameter[0];
                if (par.Name.ToLower() == "clearcolumns" && par.Value.ToLower() == "false")
                    return;
            }

            ComponentExt.ClearColumns();
        }

        public void DataSourceFilter(JEventAction action)
        {
            ComponentExt.DataSourceFilter(action);
        }
    }
}