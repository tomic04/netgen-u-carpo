using System;
using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class Search
    {
        public static string Naziv { get; private set; }

        public static DataSet Partneri(string connectionString, string naziv, out string sqlText)
        {
            try
            {
                string spName = "spPartneriQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 21 },
                    { "@Naziv", naziv },
                    { "@ID", DBNull.Value },
                    { "@IDPartneri", DBNull.Value },
                    { "@DownUnder", DBNull.Value },
                    { "@SortKey", DBNull.Value },
                    { "@Sifra", DBNull.Value },
                    { "@MatBroj", DBNull.Value }
                };

                DataSet ds = Common.ExecuteQuery(connectionString, spName, Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet Roba(string connectionString, string naziv, int parStruId, out string sqlText)
        {
            try
            {
                string spName = "spHelpRoba2";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@mora_biti_na_stanju", 0 },
                    { "@ParStruId", parStruId },
                    { "@Naziv", naziv },
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