using Carpo.Structure.Models.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    internal class FormsVar
    {
        public static object GetFormParameterValue(Forms.Base.JupiterShared form, string parameter, bool isGlobal)
        {
            if (form is Forms.Base.JupiterShared)
            {
                Dictionary<string, object> dic;
                if (isGlobal == true)
                    dic = form.VariablesGlobal;
                else
                    dic = form.VariablesForm;

                return dic.Where((KeyValuePair<string, object> x) => x.Key.ToLower() == parameter.ToLower()).FirstOrDefault().Value;
            }
            return null;
        }

        public static void SetVariableValue(Forms.Base.JupiterShared form, JEventAction action)
        {
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                if (!string.IsNullOrEmpty(action.Value))
                {
                    JEventActionParameter param = Carpo.UI.Logic.Parameters.GetParameter(form, action);
                    SetVariableByParameter(form, param, action.Value);
                }
                else
                {
                    foreach (var par in action.Parameter)
                    {
                        SetVariableByParameter(form, par, "");
                    }
                }
            }
        }

        public static void SetVariableByParameter(Forms.Base.JupiterShared form, JEventActionParameter par, string parameterName)
        {
            if (form is Forms.Base.JupiterShared)
            {
                if (string.IsNullOrEmpty(parameterName))
                    parameterName = par.Name;

                SetVariable(form, parameterName, par.Value, par.Type);
            }
        }

        private static void SetVariable(Forms.Base.JupiterShared form, string name, string valueStr, string valueType)
        {
            object value;
            using (Parser parser = new Parser(form, null))
            {
                value = parser.GetLogicPartValue(valueStr);
                value = Common.Helpers.Conversion.Parse(valueType, value);
            }

            SetVariable(form, name, value);
        }

        public static void SetVariable(Forms.Base.JupiterShared form, string name, object value, bool? deleteTransactionVariables = null)
        {
            bool isParent = false, isGlobal = false;
            name = Parser.CheckScope(name, out isParent, out isGlobal);

            if (name.StartsWith("$"))
                name = name.Substring(1);

            if (isParent == true)
            {
                form = form.Owner as Forms.Base.JupiterShared;
            }

            Dictionary<string, object> dic;
            if (isGlobal == true)
                dic = (form as Forms.Base.JupiterShared).VariablesGlobal;
            else
                dic = (form as Forms.Base.JupiterShared).VariablesForm;

            if (dic.ContainsKey(name))
            {
                if (deleteTransactionVariables != null && deleteTransactionVariables == true)
                    dic.Remove(name);
                else
                    dic[name] = value;
            }
            else
                dic.Add(name, value);
        }

        public static void SetFormParametersOnClosing(Forms.Base.JupiterShared form, Forms.Base.JupiterEditor editForm, JEventAction action, Dictionary<string, object> parentVariablesForm)
        {
            if (action.Parameter == null)
                return;
            if (editForm == null)
                return;

            bool passParentFormParameters = false;

            if (!string.IsNullOrEmpty(action.Type))
            {
                string type = action.Type.ToLower();
                if (type.Contains("form") && type.Contains("open"))
                {
                    passParentFormParameters = true;
                }
            }

            foreach (JEventActionParameter parameter in action.Parameter)
            {
                using (Parser parser = new Parser(form, null))
                {
                    object value = parser.GetLogicPartValue(parameter.Value, passParentFormParameters == true ? parentVariablesForm : null);

                    if (editForm.VariablesForm.ContainsKey(parameter.Name))
                        editForm.VariablesForm[parameter.Name] = value;
                    else
                        editForm.VariablesForm.Add(parameter.Name, value);
                }
            }
        }

        public static string SetScalarParameter(Forms.Base.JupiterShared form, JEventAction action, object value)
        {
            if (action.Parameter == null || action.Parameter.Count == 0)
                return "";
            if (form == null)
                return "";

            string name = action.Parameter[0].Name;

            SetVariable(form, name, value);
            return name;
        }

        public static Form GetSearchForm(Form form, string controlStr)
        {
            string[] controlPars = controlStr.Split('.');
            if (controlPars.Length > 1 && controlPars[0].ToLower() == "parent")
                return form.Owner;
            return form;
        }

        public static Forms.Base.JupiterShared GetActiveForm(Forms.Base.JupiterShared form)
        {
            //provjeri owned forme - ako se referenciramo na parent kontrolu (npr. parent.#grid)
            Forms.Base.JupiterShared formActive;
            if (form.OwnedForms != null &&
                form.OwnedForms.Length > 0 &&
                form.OwnedForms[0] is Forms.Base.JupiterShared) //ako je lookup otvoren - jer predstavlja formu
                formActive = form.OwnedForms[0] as Forms.Base.JupiterShared;
            else
                formActive = form;

            return formActive;
        }

        public static void FormClear(Form form)
        {
            //u izradi
        }
    }
}