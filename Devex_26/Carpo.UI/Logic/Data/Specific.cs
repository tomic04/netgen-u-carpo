using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.UI.Logic.Data
{
    //privremeno ovdje: Treba premjestiti
    internal class Specific
    {
        public static DataSet SearchPartners(string connectionString, string naziv)
        {
            try
            {
                string spName = "spPartneriQuery";
                //Dictionary<string, object> parameters = data.DataParameterList.ToDictionary(x => x.Name, x => (Object)x.Value, StringComparer.OrdinalIgnoreCase);
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 21 },
                    { "@Naziv", naziv },
                };

                SqlParameter[] spPars = Carpo.Data.Parameters.GetStoredProcedureParameters(connectionString, spName, parameters);

                DataSet ds = Carpo.Data.SqlHelper.ExecuteDataset(connectionString, spName, spPars);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet SearchRoba(string connectionString, string naziv, int parStruId)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("spHelpRoba2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("mora_biti_na_stanju", 0);
                        cmd.Parameters.AddWithValue("ParStruId", parStruId);
                        cmd.Parameters.AddWithValue("Naziv", naziv);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch
            {
                throw;
            }
        }
    }
}