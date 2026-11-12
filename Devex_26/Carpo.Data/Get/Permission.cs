using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class Permission
    {
        public static DataTable GetPermissions(string connectionString, int operateriId, string objectName, out string sqlText)
        {
            try
            {
                string spName = "spCarpoPermissionQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "getPermissions" },
                    { "@OperaterId", operateriId},
                    { "@ObjectName", objectName }
                };

                DataSet ds = Common.ExecuteQuery(connectionString, spName, Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }
    }
}