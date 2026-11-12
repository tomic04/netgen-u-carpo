using System;
using System.Collections.Generic;

namespace Carpo.Data.Set
{
    public class InfoHistory
    {
        public static void Azur (string entity, int id, int operateriId)
        {
            try
            {
                string spName = "spCarpoInfoHistoryAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "azur" },
                    { "@Entity", entity },
                    { "@Id", id },
                    { "@OperateriId", operateriId }
                };

                string sqlText = "";
                Carpo.Data.Common.ExecuteNonQuery(Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }
    }
}