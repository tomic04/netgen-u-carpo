using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Logic
{
    public class Notifications
    {
        //private List<string> atributi = new List<string>();

        private string[] keyWords = new string[] {
            "async",
            "attribute",
            "caption",
            "control",
            "data",
            "default",
            "drag",
            "enable",
            "empty",
            "each",
            "focus",
            "for",
            "item",
            "keep",
            "mode",
            "populate",
            "popup",
            "print",
            "refresh",
            "row",
            "selected",
            "source",
            "value",
            "visible"
        };

        private const string controlPrefix = "control";

        private enum Behavior
        {
            none,
            attribute, controlcaption,
            datasourceempty, datasourcefilter, defaultmode, disable, dragmode,
            enable, expand, export,
            filter, focus, focusnextrow, focuslastrow, foreachrow, foreachrowselected, itemcaption,
            populate, populateasync, populatekeepfocus,
            populateasynckeepfocus, refreshrow, popup, print, printcontrol, printcontrolselected,

            //refreshrow, refreshvalue,
            refreshvalue,

            selectedrows,
            value, visible
        }

        private enum BehaviorLevel2
        {
            sync, keepfocus
        }

        private Forms.Base.JupiterShared form;
        private Control control;
        private JEventAction action;
        private object value;

        public Notifications(Forms.Base.JupiterShared form, Control control, JEventAction action, object value)
        {
            this.form = form;
            this.control = control;
            this.action = action;
            this.value = value;
        }

        #region NotifyMe

        public void SetNotify()
        {
            if (action.Type == null)
                return;

            string type = action.Type.ToLower();
            if (type.StartsWith(controlPrefix))
            {
                type = GetTextAfterPrefix(controlPrefix, type);
            }

            Behavior behavior = GetBehaviour(type);
            SetBehaviour(behavior);
        }

        private string GetTextAfterPrefix(string prefix, string type)
        {
            return type.Remove(0, prefix.Length).Trim();
        }

        private Behavior GetBehaviour(string type)
        {
            type = type.Replace(" ", "");
            switch (type)
            {
                case "attribute": return Behavior.attribute;
                case "caption": return Behavior.controlcaption;
                case "datasourceempty": return Behavior.datasourceempty;
                case "datasourcefilter": return Behavior.datasourcefilter;
                case "defaultmode": return Behavior.defaultmode; //tree self dragdrop
                case "disable": return Behavior.disable;
                case "dragmode": return Behavior.dragmode; //tree self dragdrop
                case "enable": return Behavior.enable;
                case "expand": return Behavior.expand;
                case "export": return Behavior.export;
                case "filter": return Behavior.filter;
                case "focus": return Behavior.focus;
                case "focuslastrow": return Behavior.focuslastrow;
                case "focusnextrow": return Behavior.focusnextrow;
                case "foreachrow": return Behavior.foreachrow;
                case "foreachrowselected": return Behavior.foreachrowselected;
                case "itemcaption": return Behavior.itemcaption;
                case "populate": return Behavior.populate;
                case "populateasync": return Behavior.populateasync;
                case "populateasynckeepfocus": return Behavior.populateasynckeepfocus;
                case "populatekeepfocus": return Behavior.populatekeepfocus;
                case "refreshrow": return Behavior.refreshrow;
                case "popup": return Behavior.popup;
                case "print": return Behavior.print;
                case "printcontrol": return Behavior.printcontrol;
                case "printcontrolselected": return Behavior.printcontrolselected;
                //case "refreshrow": return Behavior.refreshrow;
                case "refreshvalue": return Behavior.refreshvalue; //tree self dragdrop
                case "selectedrows": return Behavior.selectedrows;
                case "setlayout": return Behavior.datasourceempty;
                case "value": return Behavior.value;
                case "visible": return Behavior.visible;
                default: return Behavior.none;
            }
        }

        private void SetBehaviour(Behavior behaviour)
        {
            string parameterText = null;
            switch (behaviour)
            {
                case Behavior.itemcaption:
                    if (control is IJEditor)
                        (control as IJEditor).SetItemCaption(Logic.Parameters.GetValue(form, value, action, out parameterText));
                    return;

                case Behavior.controlcaption:
                    if (control is IJControlCaption)
                        (control as IJControlCaption).SetControlCaption(Logic.Parameters.GetValue(form, value, action, out parameterText));
                    return;

                case Behavior.datasourceempty:
                    if (control is IJDataSource)
                        (control as IJDataSource).ClearDataSource(action);
                    return;

                case Behavior.datasourcefilter:
                    if (control is IJDataSource)
                        (control as IJDataSource).DataSourceFilter(action);
                    return;

                case Behavior.disable:
                    if (control is IJControl)
                        (control as IJControl).SetEnabled(false);
                    return;

                case Behavior.enable:
                    if (control is IJControl)
                        (control as IJControl).SetEnabled(true);
                    return;

                case Behavior.expand:
                    if (control is JTreeListControl)
                        (control as JTreeListControl).SetExpand(action);
                    return;

                case Behavior.export:
                    if (control is JGridControl)
                        (control as JGridControl).SetExport(action);
                    return;

                case Behavior.filter:
                    if (control is JGridControl)
                        (control as JGridControl).SetFilter(action);
                    return;

                case Behavior.focus:
                    if (control is IFocusRow)
                        (control as IFocusRow).SetFocus(action);
                    else
                        control.Focus();
                    return;

                case Behavior.focusnextrow:
                    if (control is JGridControl)
                        (control as JGridControl).SetFocusNextRow(action);
                    return;

                case Behavior.focuslastrow:
                    if (control is JGridControl)
                        (control as JGridControl).SetFocusLastRow(action);
                    return;

                case Behavior.foreachrow:
                    if (control is IJDataRow)
                        (control as IJDataRow).SetNotifyForEachRow(action, false);
                    return;

                case Behavior.foreachrowselected:
                    if (control is IJDataRow)
                        (control as IJDataRow).SetNotifyForEachRow(action, true);
                    return;

                case Behavior.populate:
                    if (control is INotifyPopulate)
                        (control as INotifyPopulate).Populate(action, true, false, FocusType.Default);
                    return;

                case Behavior.populatekeepfocus:
                    if (control is INotifyPopulate)
                        (control as INotifyPopulate).Populate(action, true, false, FocusType.KeepFocus);
                    return;

                case Behavior.populateasync:
                    if (control is INotifyPopulate)
                        (control as INotifyPopulate).Populate(action, true, true, FocusType.Default);
                    return;

                case Behavior.populateasynckeepfocus:
                    if (control is INotifyPopulate)
                        (control as INotifyPopulate).Populate(action, true, true, FocusType.KeepFocus);
                    return;

                case Behavior.print:
                    if (control is INotifyPrint)
                        (control as INotifyPrint).Print(action, true);
                    return;

                case Behavior.printcontrol:
                    if (control is INotifyPrint)
                        (control as INotifyPrint).PrintControl(false);
                    return;

                case Behavior.printcontrolselected:
                    if (control is INotifyPrint)
                        (control as INotifyPrint).PrintControl(true);
                    return;

                case Behavior.popup:
                    if (control is INotifyPopup)
                        (control as INotifyPopup).OpenPopup();
                    return;

                case Behavior.refreshvalue:
                    if (control is IJControl)
                        (control as IJControl).RefreshValue();
                    return;

                case Behavior.refreshrow:
                    if (control is JGridControl)
                        (control as JGridControl).RefreshRow(action);
                    return;

                case Behavior.selectedrows:
                    if (control is JGridControl)
                        (control as JGridControl).SetSelectedRows(action);
                    return;

                case Behavior.value:
                    if (control is JSearchControl)
                    {
                        (control as IJEditor).SetValue(Logic.Parameters.GetValue(form, value, action, out parameterText), parameterText);
                    }
                    else if (control is IJEditor)
                        (control as IJEditor).SetValue(Logic.Parameters.GetValue(form, value, action, out parameterText), parameterText);
                    return;

                case Behavior.visible:
                    if (control is IJControl)
                        (control as IJControl).SetVisibility(action);
                    return;

                case Behavior.attribute:
                    if (control is IJControl)
                        (control as IJControl).SetAttribute(action);
                    return;

                case Behavior.dragmode:
                case Behavior.defaultmode:
                    if (control is JTreeListControl)
                        (control as JTreeListControl).SetMode(Behavior.defaultmode.ToString());
                    return;
            }
        }

        private void SetKeyWords(string type)
        {
            keyWords = type.Split(' ');
        }

        #endregion NotifyMe
    }
}