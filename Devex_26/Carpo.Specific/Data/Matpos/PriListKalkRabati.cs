using Carpo.Specific.Matpos;
using System.Collections.Generic;
using System.Data;

namespace Carpo.Specific.Data.Matpos
{
    public class PriListKalkRabati
    {
        public static DataSet GetStavkaPLK(int? ID)
        {
            try
            {
                string spName = "spNabPlkCarpoDetQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "stavkeDetalji" },
                    { "@ID", PriListKalkUrednikRabata.SendIntParamToDatabase(ID) }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static void SpremiStavku(int action, int? NabPriDetId, decimal NCijena, decimal Rabat1P, decimal Rabat1I, decimal Rabat2P, decimal Rabat2I, decimal Rabat3P, decimal Rabat3I, decimal TrosakI, decimal Ncijena, decimal NVrijednost, decimal CarinaI, out string sqlText)
        {
            try
            {
                string spName = "spNabPlkCarpoDetAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", Kontor.Virmani.Virmani.SendIntParamToDatabase(NabPriDetId) },
                    { "@CijenaKn", NCijena },
                    { "@Rabat1P", Rabat1P },
                    { "@Rabat1I", Rabat1I },
                    { "@Rabat2P", Rabat2P },
                    { "@Rabat2I", Rabat2I },
                    { "@TrosakI", TrosakI },
                    { "@Ncijena", NCijena },
                    { "@NVrijednost", NVrijednost },
                    { "@Rabat3P", Rabat3P },
                    { "@Rabat3I", Rabat3I },
                    { "@CarinaI", CarinaI }
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