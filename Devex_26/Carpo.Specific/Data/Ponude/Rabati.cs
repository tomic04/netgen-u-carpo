using System;
using System.Collections.Generic;
using System.Data;

namespace Carpo.Specific.Data.Ponude
{
    internal class Rabati
    {
        public static decimal ZbrojiRabate(string connectionString, decimal rabat1, decimal rabat2, decimal rabat3, decimal rabat4, decimal rabat5, out string sqlText)
        {
            try
            {
                string spName = "spZbrojiRabate";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@rabat1", rabat1 },
                    { "@rabat2", rabat2 },
                    { "@rabat3", rabat3 },
                    { "@rabat4", rabat4 },
                    { "@rabat5", rabat5 }
                };

                return Convert.ToDecimal(Carpo.Data.Common.ExecuteScalar(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetRabatiNaziv(string connectionString, out string sqlText)
        {
            try
            {
                string spName = "spProd_DodatniRabati";
                Dictionary<string, object> parameters = new Dictionary<string, object>() { };

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