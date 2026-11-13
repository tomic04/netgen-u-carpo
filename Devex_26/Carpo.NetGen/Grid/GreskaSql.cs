using System;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// Class1.ExecSql2 (252–267), timeout 100, Catch vraća Nothing.
    /// Poziv iz ExecSql Catch (230): dbo.spNetGenErrorLog.
    /// </summary>
    public static class GreskaSql
    {
        public const int TimeoutSekundi = 100;

        public static string Poziv(int broj, string opis, string sql)
        {
            return "dbo.spNetGenErrorLog @errorNumber=" + broj.ToString()
                + ", @errorDescription='" + Navodnik(opis)
                + "',@ngLog='" + Navodnik(sql) + "'";
        }

        public static Func<string, DataTable> Izvrsi;

        public static void Zapisi(int broj, string opis, string sql, string spoj)
        {
            string tekst = Poziv(broj, opis, sql);
            try
            {
                if (Izvrsi != null)
                {
                    Izvrsi(tekst);
                    return;
                }
                if (string.IsNullOrEmpty(spoj))
                    return;
                using (var conn = new SqlConnection(spoj))
                using (var cmd = new SqlCommand(tekst, conn))
                {
                    cmd.CommandTimeout = TimeoutSekundi;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
            }
        }

        private static string Navodnik(string s)
        {
            return (s ?? "").Replace("'", "''");
        }
    }
}
