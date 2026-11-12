using Carpo.Structure.Interfaces;
using Carpo.UI.Components;
using Carpo.UI.Components.Container;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Controls
    {
        #region FindControl

        public static string ControlString(string value)
        {
            return value.Split('.')[0];
        }

        public static Control[] FindControl(Form form, string controlName)
        {
            string subControlName = string.Empty;
            if (controlName.Contains("[")) ///znači da se radi o tabu unutar tabs kontrole... npr. tabs[0]
            {
                
                var split = controlName.Split('[');
                controlName = split[0];
                subControlName = split[1].Split(']')[0];
            }

            Control[] c = form.Controls.Find(controlName, true);

            if (c == null || c.Length == 0)
                return null;


            if (c[0] is JXtraTabControl)
            {
                if (!string.IsNullOrEmpty(subControlName))
                {
                    (c[0] as JXtraTabControl).TabNameHelper = subControlName;
                }
            }
            else if (c[0] is JGridControl)
            {
                if (!string.IsNullOrEmpty(subControlName))
                {
                    (c[0] as JGridControl).ComponentExt.FieldName = subControlName;
                }
            }

            if (form is Forms.Base.JupiterShared)
            {
                if ((form as Forms.Base.JupiterShared).BarButtonItems.Count > 0)
                {
                    var test = (form as Forms.Base.JupiterShared).BarButtonItems.Where(x => x.Name == "controlName").FirstOrDefault();
                }
            }

            if (form is Forms.Info && c.Length > 1 && c.Any(x => x is JSearchControl)) {
                if ((form as Forms.Info).SearchType == Forms.Info.SearchTypeEnums.Default)
                {
                    return Array.FindAll(c, x => x is JAutoSuggestControl);
                }
                else
                {
                    return Array.FindAll(c, x => x is JSearchControl);
                }
                
            }

            return c;
        }

        #endregion FindControl

        #region ControlValue

        public static object GetControlValue(Form form, string controlName)
        {
            return GetControlValue(form, controlName, "");
        }

        public static object GetControlValue(Form form, string controlName, string extension)
        {
            return GetControlValue(form, controlName, extension, null);
        }

        public static object GetControlValue(Form form, string controlName, string extension, List<string> helperStr)
        {
            Control control = GetControl(form, controlName);
            if (control == null)
            {
                //to do log - kontrola nije prionađena
                //pokušaj pronaći unutar DataSeta - #kontrola.@polje
                if (!string.IsNullOrEmpty(extension))
                {
                    Data.Core data = new Data.Core();
                    data.CurrentForm = form as Forms.Base.JupiterShared;
                    return data.GetDataTableValue(controlName, extension.Replace("@", ""));
                }
                return null;
            }

            return GetControlValue(control, extension, helperStr);
        }

        private static object GetControlValue(Control control, string extension, List<string> helperStr)
        {
            //***nakon uvođenja tabobva na manager formi (detalj gridovi).. za provjeru selectedtab
            if (control is JXtraTabControl)
                return (control as JXtraTabControl).GetValue(extension);
            //***

            if (extension.StartsWith("@")) //fieldname
                extension = extension.Replace("@", "");
            else if (extension.Trim() != "")
                return GetBySecondPart(control, extension, helperStr);

            if (control is IJDataRow && extension.Trim() != "")
                return (control as IJDataRow).GetValue(extension, helperStr);
            else if (control is IJEditor)
                return (control as IJEditor).GetValue("");
            //else if (control.GetType() == typeof(JGridControl))
            //    return (control as JGridControl).GetValue(extension);
            //else if (control.GetType() == typeof(JTreeListControl))
            //    return (control as JTreeListControl).GetValue(extension);
            else if (control.GetType() == typeof(JTreeListControl))
            {
                if (extension == "")
                {
                    if (((JTreeListControl)control).ComponentExt.valueField != "")
                        extension = ((JTreeListControl)control).ComponentExt.valueField;
                    //else
                    //    extension = ((JTreeListControl)control).ComponentExt.valueField;
                }
                extension = Logic.Data.Core.FindDataColumn(extension, ((JTreeListControl)control).ComponentExt.DataColumns);
                if (!string.IsNullOrEmpty(extension))
                    return ((JTreeListControl)control).GetValue(extension, null);
            }
            else if (control.GetType() == typeof(JGridControl))
            {
                if (extension == "")
                {
                    if (((JGridControl)control).ComponentExt.keyField != "")
                        extension = ((JGridControl)control).ComponentExt.keyField;
                }
                extension = Logic.Data.Core.FindDataColumn(extension, ((JGridControl)control).ComponentExt.DataColumns);
                if (!string.IsNullOrEmpty(extension))
                    return ((JGridControl)control).GetValue(extension, null);
            }
            else if (Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Any(s => s.ControlName.Equals(control.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var customObject = Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Where(x => x.ControlName == control.Name).FirstOrDefault();
                bool propertyExists = (customObject as JCustomUC).Properties.ContainsKey(extension);
                if (propertyExists)
                    return customObject.GetValue(extension);
            }
            else if (((Forms.Base.JupiterEditor)control.FindForm()).CustomControls.Any(s => s.ControlName.Equals(control.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var customObject = ((Forms.Base.JupiterEditor)control.FindForm()).CustomControls.Where(x => x.ControlName == control.Name).FirstOrDefault();
                bool propertyExists = (customObject as JCustomUC).Properties.ContainsKey(extension);
                if (propertyExists)
                {
                    string sufix = "";
                    if (helperStr != null && helperStr.Count > 0)
                    {
                        sufix = "." + string.Join(".", helperStr);
                    }
                    return customObject.GetValue(extension + sufix);
                }
            }

            return null;
        }

        private static object GetBySecondPart(Control control, string extension, List<string> helperStr)
        {
            if (control is JDateEditControl)
                return (control as IJEditor).GetValue(extension);
            else if (control is JTreeListControl)
                return (control as JTreeListControl).GetValueSpecial(extension, helperStr);
            else if (control is JTreeLookUpControl)
                return (control as JTreeLookUpControl).GetValueSpecial(extension, helperStr);
            else if (extension.ToLower() == "text" && control is JSearchControl)
                return (control as JSearchControl).Text;
            else if (extension.ToLower() == "text" && control is JLookUpControl)
                return (control as JLookUpControl).Text;
            else if (extension.ToLower() == "text" && control is JAutoSuggestControl)
                return (control as JAutoSuggestControl).Text;
            else if (control is JGridControl)
                return (control as JGridControl).GetValueSpecial(extension, helperStr);
            else if (extension.ToLower() == "selectedids" && control is IJDataRow)
                return (control as IJDataRow).GetSelectedIds(helperStr);
            else if (extension.ToLower() == "searchtext" && control is JButtonControl)
                return (control as JButtonControl).GetSearchText();

            return null;
        }

        //TODO - control.Count==0!!!
        public static Control GetControl(Form form, string controlName)
        {
            if (controlName.ToLower().StartsWith("parent.") && form.Owner != null)
                form = form.Owner;

            controlName = controlName.ToLower().Replace("parent.", "").Replace("#", "");
            Control[] controls = FindControl(form, controlName);
            if (controls != null && controls.Count() > 0)
                return controls[0];

            return null;
        }

        public static string GetSearchConotrolStr(string controlStr, Forms.Base.JupiterShared form)
        {
            if (controlStr.Contains("#")) // stari dio, direktni poziv kontrole
            {
                string[] controlPars = controlStr.Split('.');
                return controlPars[controlPars.Count() - 1].ToString().Trim().Replace("#", "");
            }
            else
            {
                using (Parser parser = new Parser(form, null))
                {
                    return parser.GetLogicPartValue(controlStr).ToString();
                }
            }
        }

        #endregion ControlValue
    }
}