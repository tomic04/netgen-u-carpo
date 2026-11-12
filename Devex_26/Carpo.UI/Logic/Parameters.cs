using Carpo.Structure.Models.Core;
using System.Data;
using System.Linq;

namespace Carpo.UI.Logic
{
    internal class Parameters
    {
        //Forms.Base.JupiterShared form;
        //List<JEventActionParameter> parameter;
        //object controlValue;
        //string valueName;

        //public Parameters(Forms.Base.JupiterShared form, List<JEventActionParameter> parameter, string valueName)
        //{
        //    this.form = form;
        //    this.parameter = parameter;
        //    this.valueName = valueName;
        //}

        //public Parameters(object controlValue, string valueName)
        //{
        //    this.controlValue = controlValue;
        //    this.valueName = valueName;
        //}

        public static object GetValue(Forms.Base.JupiterShared form, object controlValue, string value)
        {
            if (controlValue is DataRow)
            {
                if (value.Contains("{"))
                {
                    Evaluate evaluate = new Evaluate(controlValue as DataRow, value);
                    return evaluate.ToString();
                }
                else
                {
                    string[] valueArr = value.Split('.');
                    string field = valueArr.Last().Replace("@", "");
                    var dataValue = (controlValue as DataRow)[field];
                    return dataValue;
                }
            }
            else if (value.Contains("{"))
            {
                Evaluate evaluate = new Evaluate(form as Forms.Base.JupiterShared, value);
                return evaluate.ToString();
            }
            else if (controlValue != null)
            {
                return controlValue;
            }
            else
            {
                return ParserDirect.GetValue(form, value);
            }
        }

        public static object GetValue(Forms.Base.JupiterShared form, object controlValue, JEventAction action, out string parameterText)
        {
            parameterText = null;
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                bool checkIfResult = true;
                foreach (JEventActionParameter parameter in action.Parameter)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, parameter, form, controlValue, true) == false)
                        continue;

                    string parText = parameter.Text;
                    string parValue = parameter.Value;
                    //if (parValue.Contains("this"))
                    //    parValue = parValue.Replace("this.", "").Replace("this", "");
                    //else
                    if (!string.IsNullOrEmpty(parText))
                    {
                        object parTextobj = ParserDirect.GetValue(form, parText);
                        if (parTextobj != null)
                            parameterText = parTextobj.ToString();
                    }

                    object value = ParserDirect.GetValue(form, parValue);
                    value = Common.Helpers.Conversion.Parse(action.Parameter[0].Type, value); // dodao Ognjen
                    return value;

                    //return GetValue(form, controlValue, parValue);
                }
            }
            return null;
        }

        public static object GetValue(Forms.Base.JupiterShared form, JEventAction action)
        {
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                bool checkIfResult = true;
                foreach (JEventActionParameter parameter in action.Parameter)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, parameter, form, null, true) == false)
                        continue;

                    string parValue = parameter.Value;
                    return ParserDirect.GetValue(form, parValue);
                }
            }
            return null;
        }

        // dodao Ognjen - u dogovoru s Igorom trenutno ostaje u ovom obliku
        public static JEventActionParameter GetParameter(Forms.Base.JupiterShared form, JEventAction action)
        {
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                bool checkIfResult = true;
                foreach (JEventActionParameter parameter in action.Parameter)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, parameter, form, null, true) == false)
                        continue;

                    return parameter;
                }
            }

            return null;
        }
    }
}