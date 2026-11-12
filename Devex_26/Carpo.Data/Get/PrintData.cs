using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class PrintData
    {
        public static DataSet GetPrintData(string connectionString, int operateriid, out string sqlText)
        {
            try
            {
                string spName = "spCarpoPrint";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "GetPrintData"},
                    { "@OperateriId", operateriid }
                };

                DataSet ds = Common.ExecuteQuery(connectionString, spName, Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }
    }
}