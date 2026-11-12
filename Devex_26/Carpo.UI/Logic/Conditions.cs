using Carpo.Structure.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Conditions
    {
        public static string GetIfString(IJCondition element)
        {
            if (!string.IsNullOrEmpty(element.If))
                return element.If;
            else if (!string.IsNullOrEmpty(element.ElseIf))
                return element.ElseIf;
            else if (!string.IsNullOrEmpty(element.Else))
                return element.Else;
            return null;
        }

        public static bool CheckCondition(ref bool checkIfResult, IJCondition element, Forms.Base.JupiterShared form, Control control)
        {
            return CheckCondition(ref checkIfResult, element, form, control, null);
        }

        public static bool CheckCondition(ref bool checkIfResult, IJCondition element, Forms.Base.JupiterShared form, object controlValue, bool fake)
        {
            return CheckCondition(ref checkIfResult, element, form, null, controlValue);
        }

        private static bool CheckCondition(ref bool checkIfResult, IJCondition element, Forms.Base.JupiterShared form, Control control, object controlValue)
        {
            if (checkIfResult == true && (!string.IsNullOrEmpty(element.ElseIf) || element.Else != null))
                return false;

            if (element.Else == null)
            {
                string ifString = GetIfString(element);

                Logic.Statemants statemants;
                if (control != null)
                    statemants = new Logic.Statemants(ifString, form, control);
                else
                    statemants = new Logic.Statemants(ifString, form, controlValue);

                checkIfResult = statemants.CheckIf();
                if (checkIfResult == false)
                    return false;
            }

            return true;
        }

        public static bool CheckConditionXmlGen(ref bool checkIfResult, string conditionString, Dictionary<string, DataTable> dtList)
        {
            try
            {
                if (checkIfResult == true)
                    return false;

                Logic.Statemants statemants = new Statemants(conditionString);

                checkIfResult = statemants.CheckIf(true, dtList);

                return checkIfResult;
            }
            catch
            {
                throw;
            }
        }
    }
}