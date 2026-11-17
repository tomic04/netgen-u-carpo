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

        /// <summary>
        /// Smije li se greška zapisati u dbo.spNetGenErrorLog (PN-10). Ne piše se u sweepu i klik-pregledu (tiho),
        /// za poruku procedure s '#' ili '+', ni na jupiter2019/jupiter2022 izvan produkcije.
        /// Isto pravilo za master grid i za ostale upite podloge.
        /// </summary>
        public static bool Smije(string spoj, string poruka, bool tiho)
        {
            if (tiho)
                return false;
            string p = poruka ?? "";
            if (p.IndexOf('#') >= 0 || p.IndexOf('+') >= 0)
                return false;
            return !(Carpo.NetGen.Komande.DijalogTok.JeJupiter(Server(spoj)) && !Carpo.NetGen.Komande.DijalogTok.Produkcija);
        }

        private static string Server(string spoj)
        {
            if (string.IsNullOrEmpty(spoj))
                return "";
            try { return new SqlConnectionStringBuilder(spoj).DataSource ?? ""; }
            catch (Exception) { return spoj; }
        }

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
