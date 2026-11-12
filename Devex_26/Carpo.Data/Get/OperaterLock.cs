using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class OperaterLock
    {
        public static DataSet OperaterLockCheck(string connectionString, string operaterlocktable, string operaterlockkey, int operaterlockkeyvalue, int ssoperateriid, out string sqlText)
        {
            try
            {
                string spName = "spCarpoOperaterLock";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "open" },
                    { "@OperaterLockTable", operaterlocktable },
                    { "@OperaterLockKey", operaterlockkey },
                    { "@OperaterLockKeyValue", operaterlockkeyvalue },
                    { "@SSOperateriId", ssoperateriid },
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }
    }
}