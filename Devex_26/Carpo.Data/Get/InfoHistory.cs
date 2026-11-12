using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class InfoHistory
    {
        public static DataTable Get(string entity, int operateriId)
        {
            try
            {
                string spName = "spCarpoInfoHistoryQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "get" },
                    { "@Entity", entity },
                    { "@OperateriId", operateriId }
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
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