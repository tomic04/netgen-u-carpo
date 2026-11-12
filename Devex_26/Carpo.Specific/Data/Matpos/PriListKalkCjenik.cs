using System.Collections.Generic;
using System.Data;

namespace Carpo.Specific.Data.Matpos
{
    public class PriListKalkCjenik
    {
        public static DataTable GetCjenikData(int robaId, int skladisteId, decimal? nabCijena)
        {
            try
            {
                string spName = "spNabPlkCarpoNKUCjenikQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 0 },
                    { "@NabCijena", nabCijena },
                    { "@RobaId", robaId },
                    { "@skladisteid", skladisteId }
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

        public static void SpremiCjenik(int action, int? Id, decimal NettoCijenaKn, decimal MPCijena, int robaId, int skladisteId, int operateriid, int VrCjenikId, int nabpriglaid, out string sqlText)
        {
            try
            {
                string spName = "spNabPlkCarpoNKUCjenikAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", Id },
                    { "@NetoCijenaKn", NettoCijenaKn },
                    { "@MPCijena", MPCijena },
                    { "@Robaid", robaId },
                    { "@skladisteid", skladisteId },
                    { "@operateriid", operateriid },
                    { "@VrCjenikId", VrCjenikId },
                    { "@nabpriglaid", nabpriglaid }
                };

                Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }
    }
}