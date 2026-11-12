using System.Collections.Generic;
using System.Data;

namespace Carpo.Designer.Data
{
    internal class Element
    {
        public static DataTable GetElements()
        {
            try
            {
                string spName = "spCarpoDsElements";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getElements" }
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

        public static DataTable GetLnElements()
        {
            try
            {
                string spName = "spCarpoDsLnElements";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "GetLnElements" }
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