using Carpo.Structure.Models.Core;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Carpo.Structure.Interfaces
{
    public interface IJControl
    {
        void SetEvents(object events);

        void SetNotifications(JEventAction action, object value);

        void SetEnabled(bool enabled);

        void SetVisibility(JEventAction action);

        void RefreshValue();

        void SetAttribute(JEventAction action);

        object GetKeyCode();
    }

    public interface IJEditor
    {
        object GetValue(string helperStr);

        void SetValue(object value, string text);

        void SetItemCaption(object caption);
    }

    public interface IJDataSource
    {
        void ClearDataSource(JEventAction action);

        void DataSourceFilter(JEventAction action);
    }

    public interface IJDataRow
    {
        void SetNotifyForEachRow(JEventAction action, bool onlySelected);

        DataRow DataRowRef { get; set; }

        object GetValue(string field, List<string> helperStr);

        object GetValueSpecial(string name, List<string> helperStr);

        object GetSelectedIds(List<string> helperStr);

        DataTable GetDataSource();
    }

    public interface IFocusRow
    {
        void SetFocus(JEventAction action);
    }

    public interface IControlMenu
    {
        //List<Components.JMenuItemControl> MenuItems {get; set;}
        List<JMenuItemList> MenuItems { get; set; }

        ContextMenuStrip ControlMenu { get; set; }
        //void SetMenu();
    }

    public interface IJCondition
    {
        string If { get; set; }
        string ElseIf { get; set; }
        string Else { get; set; }
    }

    public interface INotifyPrint
    {
        void Print(JEventAction action, bool selected);

        void PrintControl(bool selected);
    }

    public interface IJControlCaption
    {
        void SetControlCaption(object caption);
    }
}