using Carpo.Structure.Models.Core;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    /*
     za varijable (variable value) i kontrole (control value)
     vrijednost može biti unutar akcije -> "single push"
       npr. <action type="control value" value="#naziv">
           <paramter value="vrijednost" />
        </action>
     ili možemo nabrajati kontrole/varijable preko parametara
        npr. <action type="control value" >
            <parameter name="#kontola1" value="vrijednost" />
            <parameter name="#kontola2" value="vrijednost" />
            ...
        </action>
    */

    public class ActionValue
    {
        public enum ActionValueType { None, Control, Variable };

        private ActionValueType type;
        private Forms.Base.JupiterShared form;
        private Control control;
        private JEventAction action;

        public ActionValue()
        {
        }

        public void Process(ActionValueType type, Forms.Base.JupiterShared form, JEventAction action)
        {
            Process(null, type, form, action);
        }

        public void Process(Control control, ActionValueType type, Forms.Base.JupiterShared form, JEventAction action)
        {
            this.control = control;
            this.type = type;
            this.form = form;
            this.action = action;

            ProcessAction();
        }

        private void ProcessAction()
        {
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                if (!string.IsNullOrEmpty(action.Value))
                {
                    SetValueByParameter(action.Parameter[0], action.Value);
                }
                else
                {
                    foreach (var par in action.Parameter)
                    {
                        SetValueByParameter(par, "");
                    }
                }
            }
        }

        private void SetValueByParameter(JEventActionParameter par, string parameterName)
        {
            if (form is Forms.Base.JupiterShared)
            {
                if (string.IsNullOrEmpty(parameterName))
                    parameterName = par.Name;

                SetValue(parameterName, par.Value, par.Type);
            }
        }

        private void SetValue(string name, string valueStr, string valueType)
        {
            object value;
            using (Parser parser = new Parser(form, null))
            {
                value = parser.GetLogicPartValue(valueStr);
                value = Common.Helpers.Conversion.Parse(valueType, value);
            }

            if (type == ActionValueType.Variable)
                SetVariableValue(name, value);
            //else if (type == ActionValueType.Control)
            //    SetControlValue(name, value);
        }

        private void SetVariableValue(string name, object value)
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
                dic[name] = value;
            else
                dic.Add(name, value);
        }
    }
}