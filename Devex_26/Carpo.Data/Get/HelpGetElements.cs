using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public static class HelpGetElements
    {
        public static DataTable GetElements()
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getelements" },
                    { "@CarpoDsAttributeId", null}
                };

                string sqlText = "";
                string sqlStrConn = Carpo.Data.Constructor.SetConnectionString("spin-sql", "spin", "", "");
                DataSet ds = Carpo.Data.Common.ExecuteQuery(sqlStrConn, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetOnlyChildElements(int i)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                { "@action", "getOnlyChildElements" },
                { "@CarpoDsElementId", i}
            };

                string sqlText = "";
                string sqlStrConn = Carpo.Data.Constructor.SetConnectionString("spin-sql", "spin", "", "");
                DataSet ds = Carpo.Data.Common.ExecuteQuery(sqlStrConn, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetElementAttributes(int i)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                { "@action", "getElementAttributes" },
                { "@CarpoDsElementId", i}
            };

                string sqlText = "";
                string sqlStrConn = Carpo.Data.Constructor.SetConnectionString("spin-sql", "spin", "", "");
                DataSet ds = Carpo.Data.Common.ExecuteQuery(sqlStrConn, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
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