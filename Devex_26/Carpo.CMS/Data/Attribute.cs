using System.Collections.Generic;
using System.Data;

namespace Carpo.CMS.Data
{
    internal class Attribute
    {
        public static DataTable GetAttributes()
        {
            try
            {
                string spName = "spCarpoDsAttribute";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getAttributes" }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters);
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