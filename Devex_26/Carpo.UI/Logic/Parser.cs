using Carpo.Structure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Parser : IDisposable
    {
        public bool IsXmlGen { get; set; } = false;
        public Dictionary<string, DataTable> ForwardedDataTablesList { get; set; }

        public enum ParserType { None, Date, Control, Parameter, Method, PubVars, SecondPart }

        public enum DirectType { ControlName, FieldName, Value }

        private Forms.Base.JupiterShared currentForm;
        private Control control;
        private object controlValue;

        private Forms.Base.JupiterShared formRef;

        private string dataTableName = "";
        private string controlName = "";
        private string formParameterName = "";
        private string afterDotNameFirst = "";
        private string methodName = "";

        public ParserType Type { get; set; } = ParserType.None;

        public Parser(Forms.Base.JupiterShared currentForm, Control control, object controlValue)
        {
            this.currentForm = currentForm;
            this.control = control;
            this.controlValue = controlValue;
        }

        public Parser(Forms.Base.JupiterShared currentForm, object controlValue)
        {
            this.currentForm = currentForm;
            this.controlValue = controlValue;
        }

        public Parser(Forms.Base.JupiterShared currentForm, Control control)
        {
            this.currentForm = currentForm;
            this.control = control;
        }

        public Parser(Dictionary<string, DataTable> forwardedDataTablesList) // koristimo za Carpo.XmlGen
        {
            ForwardedDataTablesList = forwardedDataTablesList;
        }

        public object GetLogicPartValue(string logicPart)
        {
            return GetLogicPartValue(logicPart, null);
        }

        public object GetLogicPartValue(string logicPart, Dictionary<string, object> parentFormParameters)
        {
            ResetVariables();

            //ovo izbacujem - this će neprogramerima biti zbunjujuć
            if (logicPart.ToLower() == "this")
            {
                return GetThisValue();
            }

            if (logicPart.Contains("{") && logicPart.Contains("}"))
            {
                if (!IsXmlGen)
                {
                    Evaluate evaluate = new Evaluate(currentForm, logicPart);
                    logicPart = evaluate.ToString();
                }
                else
                {
                    Evaluate evaluate = new Evaluate(ForwardedDataTablesList, logicPart);
                    logicPart = evaluate.ToString();
                }
                return logicPart;
            }
            else if (logicPart.StartsWith("'") && logicPart.Trim().EndsWith("'"))
                return logicPart.Substring(1, logicPart.ToString().Length - 2);

            bool isParent, isGlobal;

            string logicPartOriginal = logicPart;
            logicPart = CheckScope(logicPart, out isParent, out isGlobal);

            List<string> parsStr = logicPart.Split('.').ToList();

            for (int i = 0; i < parsStr.Count; i++)
            {
                string str = parsStr[i];

                if (i == 0 && str.StartsWith("key", StringComparison.InvariantCultureIgnoreCase))
                    return GetKeys();
                else if (i == 0 && str.StartsWith("dialogresult", StringComparison.InvariantCultureIgnoreCase))
                    return GetLastDialogResult();
                else if (i == 0 && str.StartsWith("transaction", StringComparison.InvariantCultureIgnoreCase))
                    return GetLastTransactionResult();
                // Ispravio T.A. 20.06.2018. - uvjet je prvotno namijenjen SAMO za globalni DATASET (ne i njegovo polje), preciznija dorada uvjeta
                else if (isGlobal && !str.StartsWith("$") && !str.StartsWith("@"))
                {
                    // ako se radi o globalnom datasetu
                    if (parsStr.Where(x => x.StartsWith("@")).FirstOrDefault() == null)
                    {
                        Data.Core data = new Data.Core() { CurrentForm = currentForm as Forms.Base.JupiterShared, ConnectionString = currentForm.ConnectionString };
                        return data.GetDataByName(logicPartOriginal);
                    }
                    // ako se radi o POLJU globalnog dataseta
                    else
                        dataTableName = str.Replace("~", "");
                }
                else if (i == 0 && str.StartsWith("@@") && CheckType(ParserType.PubVars))
                    return GetPubVar(str);
                else if (i == 0 && str.StartsWith("$") && CheckType(ParserType.Parameter)) //form parameter
                {
                    formParameterName = str.Replace("$", "");
                    break;
                }
                else if (i == 0 && str.StartsWith("@") && CheckType(ParserType.SecondPart))
                    return FindOutValueBySecondPart(str);
                else if (i == 0 && str.StartsWith("this") && control != null && CheckType(ParserType.Control))
                    controlName = control.Name;
                else if (str.StartsWith("##") && CheckType(ParserType.Method)) //funkcija
                    methodName = str.Replace("##", "");
                else if (str.StartsWith("#") && CheckType(ParserType.Control)) //naziv kontrole
                    controlName = str.Replace("#", "");
                else if (str.StartsWith("~") || Type == ParserType.Date)
                {
                    if (i > 0 && str.ToLower() == "dataset")
                        afterDotNameFirst = str;
                    else
                        dataTableName = str.Replace("~", "").Replace("#", "");
                }
                else if (i == 0 && parsStr.Count == 1) //ako nije kontrola, postavi vrijednost
                {
                    if (str == "null")
                        return null;
                    else
                        return str;
                }
                else if (i > 0) //ako postoji više definicija (npr. #control.level == 2), saznaj o kojoj se konotroli radi;
                {
                    afterDotNameFirst = str;
                    break;
                }
            }

            if (!IsXmlGen)
            {
                if (isParent == true)
                    formRef = FormsVar.GetActiveForm(currentForm).Owner as Forms.Base.JupiterShared;
                else
                    formRef = FormsVar.GetActiveForm(currentForm) as Forms.Base.JupiterShared;
            }

            if (formParameterName != "")
                return GetFormParameter(parentFormParameters, isGlobal);

            if (afterDotNameFirst != "" && controlName != "")
            {
                List<string> helperStr = null;
                if (controlValue is string)
                    helperStr = new List<string> { controlValue.ToString() };
                else if (parsStr.Count > 2)
                    helperStr = parsStr.Skip(2).ToList();

                object value = Controls.GetControlValue(formRef, controlName, afterDotNameFirst, helperStr);
                if (value == null)
                    value = Controls.GetControlValue(currentForm, controlName, afterDotNameFirst, helperStr);
                return value;
            }
            else if (dataTableName != "")
                return GetDataValue();

            if (!string.IsNullOrEmpty(controlName) && !IsXmlGen)
                return GetControlValue();
            else if (!string.IsNullOrEmpty(methodName))
            {
                var method = Methods.GetMethod(formRef, methodName);
                if (method != null)
                    return method.GetValue(afterDotNameFirst.Replace("@", ""));
            }

            return null;
        }

        private void ResetVariables()
        {
            dataTableName = "";
            controlName = "";
            formParameterName = "";
            afterDotNameFirst = "";
            methodName = "";
        }

        private object GetPubVar(string var)
        {
            return Variables.GetPubVar(var, currentForm as Forms.Base.JupiterShared);
        }

        private string GetLastDialogResult()
        {
            if (currentForm == null)
                return "";

            return currentForm.LastDialogResult;
        }

        private string GetLastTransactionResult()
        {
            if (currentForm == null)
                return "";

            return currentForm.LastTransactionSuccess;
        }

        private object GetThisValue()
        {
            if (control != null && control is DevExpress.XtraEditors.BaseEdit)
            {
                if ((control as DevExpress.XtraEditors.BaseEdit).EditValue != null)
                    return (control as DevExpress.XtraEditors.BaseEdit).EditValue.ToString();
                else
                    return null;
            }
            else
            {
                return controlValue;
            }
        }

        private object GetKeys()
        {
            string modifierKey = "", keyCode = "";

            System.Windows.Forms.Keys modKey;

            if (control != null)
            {
                if (Control.ModifierKeys != Keys.None)
                    modKey = Control.ModifierKeys;
                else
                    modKey = this.currentForm.Modifier;
            }
            else
                modKey = this.currentForm.Modifier;

            if (modKey != Keys.None)
            {
                switch (modKey)
                {
                    case (Keys.Control | Keys.Shift | Keys.Alt):
                        modifierKey = "control+shift+alt";
                        break;

                    case (Keys.Control | Keys.Shift):
                        modifierKey = "control+shift";
                        break;

                    case (Keys.Shift | Keys.Alt):
                        modifierKey = "shift+alt";
                        break;

                    case (Keys.Control | Keys.Alt):
                        modifierKey = "control+alt";
                        break;

                    default:
                        modifierKey = modKey.ToString().ToLower();
                        break;
                }
            }

            if (control != null)
                keyCode = (control as IJControl).GetKeyCode().ToString();
            else
                keyCode = this.currentForm.KeyCode.ToString();

            if (modifierKey == "")
                return keyCode;
            else if (keyCode == "")
                return modifierKey;
            else
                return modifierKey + "+" + keyCode;
        }

        private object GetFormParameter(Dictionary<string, object> parentFormParameters, bool isGlobal)
        {
            if (parentFormParameters != null && parentFormParameters.Count > 0)
                return parentFormParameters[formParameterName];

            return FormsVar.GetFormParameterValue(formRef, formParameterName, isGlobal);
        }

        private object GetControlValue()
        {
            object value = Controls.GetControlValue(formRef, controlName, "");

            if (value == null)
            {
                if (formRef is UI.Forms.Base.JupiterManager)
                {
                    List<Structure.Models.IJControlDefinition> controlList = (formRef as UI.Forms.Base.JupiterShared).JControlList;
                    foreach (var controlDefinition in controlList)
                    {
                        if (controlDefinition is Structure.Models.JRepositoryDefinition)
                        {
                            return (controlDefinition as Structure.Models.JRepositoryDefinition).Value;
                        }
                    }
                }
            }

            return value;
        }

        private object GetDataValue()
        {
            Data.Core data = new Data.Core();
            data.CurrentForm = formRef as Forms.Base.JupiterShared;
            if (afterDotNameFirst == "datatable")
                return data.GetDataTableValue(dataTableName, "", true);
            else if (afterDotNameFirst != "" && afterDotNameFirst != "dataset") //get fieldname from datatable
                if (afterDotNameFirst == "rowcount" && !IsXmlGen)
                    return data.GetByAttribute(dataTableName, "rowcount");
                else
                    return data.GetDataTableValue(dataTableName, afterDotNameFirst.Replace("@", ""), isXmlGen: IsXmlGen,
                        dtForwarded: ForwardedDataTablesList);
            else //get JData definition
                return data.GetDataByName(dataTableName);
        }

        //Trenutna vrijednost kontrole prema definiciji
        //@name - naziv kontrole; ako počinje s @ pretpostavka je da je kontrola već evidentirana
        private object FindOutValueBySecondPart(string definition)
        {
            definition = definition.Replace("@", "");

            if (control is IJDataRow)
                return (control as IJDataRow).GetValue(definition, null);

            return null;

            //if (definition.ToLower() == "level" && control is JTreeListControl)
            //{
            //    return (control as JTreeListControl).FocusedNode.Level.ToString();
            //}
            //else if (definition.ToLower() == "rowcount" && control is JGridControl)
            //{
            //    return (control as JGridControl).MainView.RowCount.ToString();
            //}
            //else if (control is IJDataRow)
            //{
            //    if (definition.ToLower() == "selectedids")
            //        return (control as IJDataRow).GetSelectedIds();

            //    return (control as IJDataRow).GetValue(definition);
            //}
            //return null;
        }

        private bool CheckType(ParserType expectedType)
        {
            if (Type == ParserType.None || Type == expectedType)
                return true;

            return false;
        }

        #region statics

        ///<summary>
        ///Direktan dohvat - samo ono što je na prvom mjestu
        ///</summary>
        public static string GetDirectByPart(string logicPart, DirectType type)
        {
            string[] parsStr = logicPart.Split(new string[] { "." }, StringSplitOptions.None);

            for (int i = 0; i < parsStr.Length; i++)
            {
                string str = parsStr[i];
                if (str.StartsWith("@") && type == DirectType.FieldName)
                {
                    return str.Replace("@", "");
                }
                else if (str.StartsWith("#") && type == DirectType.ControlName)
                {
                    return str.Replace("@", "");
                }
                else if (!Regex.IsMatch(str, "^(@|#)") && type == DirectType.Value)
                    return str;
            }

            return null;
        }

        public static string ParseParameterGetName(string parameter, string prefix)
        {
            string[] pars = parameter.Split(new string[] { "." }, StringSplitOptions.None);
            foreach (string str in pars)
            {
                if (str.StartsWith(prefix))
                    return (str.Replace(prefix, ""));
            }
            return null;
        }

        public static string CheckScope(string logicPart, out bool isParent, out bool isGlobal)
        {
            isParent = false;
            isGlobal = false;

            string parentString = "parent.";
            string globalString = "global.";

            if (logicPart.StartsWith(parentString, StringComparison.InvariantCultureIgnoreCase))
            {
                logicPart = logicPart.Substring(parentString.Length);
                isParent = true;
            }
            else if (logicPart.StartsWith(globalString, StringComparison.InvariantCultureIgnoreCase))
            {
                logicPart = logicPart.Substring(globalString.Length);
                isGlobal = true;
            }
            return logicPart;
        }

        #endregion statics

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Parser() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}