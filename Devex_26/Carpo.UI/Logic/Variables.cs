using Carpo.UI.Connector.Abstract;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Logic
{
    public class Variables
    {
        private static List<string> pubVars = new List<string>()
        {
            "@@operateriid", "@@moduleid", "@@connection", "@@connectionres", "@@database", "@@server", "@@uid", "@@pwd", "@@login" , "@@imeFirme"
        };

        public static object GetPubVar(string var, Forms.Base.JupiterShared form)
        {
            if (var.StartsWith("@@operateriid", StringComparison.InvariantCultureIgnoreCase) ||
                    var.StartsWith("@@operaterid", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Core.PubVars.OperateriId;
            else if (var.StartsWith("@@database", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.DataBase;
            else if (var.StartsWith("@@formName", StringComparison.InvariantCultureIgnoreCase))
            {
                if (form != null && (!string.IsNullOrEmpty(form.FormName)))
                    return form.FormName;
            }

            else if (var.StartsWith("@@imeFirme", StringComparison.InvariantCultureIgnoreCase))
            {

                return Starter.ImeFirme;
            }
            else if (var.StartsWith("@@connectionres", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.ConnStrResources;
            else if (var.StartsWith("@@connection", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.ConnectionString;
            else if (var.StartsWith("@@server", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.Server;
            else if (var.StartsWith("@@uid", StringComparison.InvariantCultureIgnoreCase) ||
                    var.StartsWith("@@username", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.Uid;
            else if (var.StartsWith("@@pwd", StringComparison.InvariantCultureIgnoreCase) ||
                    var.StartsWith("@@password", StringComparison.InvariantCultureIgnoreCase))
                return Carpo.Data.Connection.Pwd;
            else if (var.StartsWith("@@login", StringComparison.InvariantCultureIgnoreCase))
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            else if (var.StartsWith("@@moduleid", StringComparison.InvariantCultureIgnoreCase))
            {
                if (form != null && (!string.IsNullOrEmpty(form.ModuleId)))
                    return form.ModuleId;
            }
            return null;
        }

        public static Dictionary<string, object> GetAllPubVars(Forms.Base.JupiterShared form)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (string s in pubVars)
            {
                dic.Add(s, GetPubVar(s, form));
            }
            return dic;
        }
    }
}