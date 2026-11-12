using System.Collections.Generic;

namespace Carpo.Data.Set
{
    public class OperaterLock
    {
        public static void Unlock(string connectionString, string operaterlocktable, string operaterlockkey, int operaterlockkeyvalue, out string sqlText)
        {
            try
            {
                string spName = "spCarpoOperaterLock";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "close" },
                    { "@OperaterLockTable", operaterlocktable },
                    { "@OperaterLockKey", operaterlockkey },
                    { "@OperaterLockKeyValue", operaterlockkeyvalue }
                };

                Common.ExecuteNonQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }
    }
}