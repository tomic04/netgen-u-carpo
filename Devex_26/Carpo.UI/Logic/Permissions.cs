using System;
using System.Collections.Generic;
using System.Data;

namespace Carpo.UI.Logic
{
    public class Permissions
    {

        public static bool PermissionsAreSet { get; set; }
        //form.PermissionList - punjenje
        public static void SetPermissions(Forms.Base.JupiterShared form, string moduleId = "")
        {
            //if (PermissionsAreSet == true)
            //    return;

            string sqlText = "";

            

            try
            {
                //string moduleStr = form.ModuleId.Replace(".xml", "");
                //string objectPrefix = "cr#" + moduleStr + "#";

                if (string.IsNullOrEmpty(moduleId))
                    moduleId = form.ModuleId;

                string objectName = "asm:Carpo.Start:" + moduleId;

                using (DataTable dt = Carpo.Data.Get.Permission.GetPermissions(form.ConnectionString, Carpo.Core.PubVars.OperateriId, objectName, out sqlText))
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<Structure.Models.Permission> permissionList = new List<Structure.Models.Permission>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            permissionList.Add(new Structure.Models.Permission
                            {
                                ObjectName = dr["ObjectName"].ToString().Replace(objectName, ""),
                                Function = dr["FunctionName"].ToString(),
                                Enabled = dr["Enabled"] != DBNull.Value ? Convert.ToBoolean(dr["Enabled"]) : false,
                                Visible = dr["Visible"] != DBNull.Value ? Convert.ToBoolean((dr["Visible"])) : false,
                                SubObjects = null
                            });
                        }
                        form.PermissionList = permissionList;
                        PermissionsAreSet = true;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}