using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.UI.Logic
{
    public class Print
    {
        public static string GetVariables(Forms.Base.JupiterShared form, List<JEventActionParameter> parameterList)
        {
            Data.Core data = new Data.Core() { ConnectionString = form.ConnectionString, ParameterControlValue = "", CurrentForm = form };
            Dictionary<string, object> parameters = GetParameters(form, parameterList);

            string parStr = "";
            int i = 0;
            foreach (KeyValuePair<string, object> par in parameters)
            {
                parStr += (i > 0 ? ", " : "") + "@" + par.Key + "=" + GetValue(par.Value);
                i++;
            }
            return parStr;
        }

        private static Dictionary<string, object> GetParameters(Forms.Base.JupiterShared form, List<JEventActionParameter> dataParameterList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            if (dataParameterList != null && dataParameterList.Count > 0)
            {
                foreach (JEventActionParameter par in dataParameterList)
                {
                    Statemants statemants = new Statemants(par.If, form, null);
                    if (statemants.CheckIf() == true)
                        ParseParameter(form, parameters, par);
                }
            }
            return parameters;
        }

        private static void ParseParameter(Forms.Base.JupiterShared form, Dictionary<string, object> parameters, JEventActionParameter par)
        {
            object value = null;
            value = ParseParameter(form, par.Value);
            value = Common.Helpers.Conversion.Parse(par.Type, value);

            parameters.Add(par.Name, value);
        }

        private static object ParseParameter(Forms.Base.JupiterShared form, string parameter)
        {
            using (Parser parser = new Parser(form, null))
            {
                return parser.GetLogicPartValue(parameter);
            }
        }

        private static string GetValue(object value)
        {
            if (value == null)
                return "null";
            if (value is int)
                return value.ToString();
            else if (value is decimal)
                return "'" + string.Format("{0:F}", value).Replace(",", ".") + "'";
            else if (value is DateTime)
                return "'" + string.Format("{0:yyyyMMdd}", value) + "'";
            else
                return "'" + value.ToString().Trim() + "'";
        }

        public static string GetFooterText(Forms.Base.JupiterShared form)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(form.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spPartneriQuery100", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                return ds.Tables[0].Rows[0].Field<string>("Naziv") + "\n" + ds.Tables[0].Rows[0].Field<string>("Mjesto");
            }
            catch
            {
                throw;
            }
        }
    }
}