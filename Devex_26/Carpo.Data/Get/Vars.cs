using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class Vars
    {
        public static DataSet GetBySource(string connectionString, int operateriId, string source, string form, out string sqlText)
        {
            try
            {
                string spName = "spCarpoVarsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "getBySource" },
                    { "@OperateriId", operateriId},
                    { "@Source", source },
                    { "@Form", form }
                };

                DataSet ds = Common.ExecuteQuery(connectionString, spName, Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static bool GetDmsExists(string connectionString, out string sqlText)
        {
            try
            {
                string spName = "spCarpoVarsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "getDmsPubvar" }
                };

                DataSet ds = Common.ExecuteQuery(connectionString, spName, Common.DataType.StoredProcedure, parameters, out sqlText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    var varset = ds.Tables[0].Rows[0]["VarSet"].ToString();

                    return varset == "1" ? true : false;
                }
                else
                    return false;
            }
            catch
            {
                throw;
            }
        }
    }
}