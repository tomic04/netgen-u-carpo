using System;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// Zamjene iz Form1.SrediSQL (2470) i preskok iz Class1.ExecSql (85-118).
    /// @@ na poziciji 0 ne hvata uvjet IndexOf &gt; 0; hvata ga grana "pocinje s @".
    /// </summary>
    public sealed class SqlKontekst
    {
        public string Id = "";
        public string DetId = "";
        public string Node = "null";
        public string OperaterId = "";
        public string Tablica = "t1";
        public string XmlName = "";
        public string JupiterSrvPath = "";
        public string EditedColumn = "";
        public string LastValue = "";
        public string SelectedColor = "";
    }

    public static class SqlTekst
    {
        public static string Sredi(string sql, SqlKontekst k)
        {
            if (sql == null)
                return "";
            if (k == null)
                k = new SqlKontekst();
            sql = Zamijeni(sql, "@@Id", k.Id);
            sql = Zamijeni(sql, "@@ID", k.Id);
            sql = Zamijeni(sql, "@@id", k.Id);
            sql = Zamijeni(sql, "@@operateriid", k.OperaterId);
            sql = Zamijeni(sql, "@@OPERATERIID", k.OperaterId);
            sql = Zamijeni(sql, "@@Operateriid", k.OperaterId);
            sql = Zamijeni(sql, "@@DETID", k.DetId);
            sql = Zamijeni(sql, "@@detid", k.DetId);
            sql = Zamijeni(sql, "@@DetId", k.DetId);
            sql = Zamijeni(sql, "@@node", k.Node);
            sql = Zamijeni(sql, "@@NODE", k.Node);
            sql = Zamijeni(sql, "@@table", k.Tablica);
            sql = Zamijeni(sql, "@@TABLE", k.Tablica);
            if (sql.IndexOf("@@xmlname", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sql = Zamijeni(sql, "@@xmlname", k.XmlName);
                sql = Zamijeni(sql, "@@XMLNAME", k.XmlName);
            }
            if (sql.IndexOf("@@jupitersrvpath", StringComparison.OrdinalIgnoreCase) >= 0)
                sql = Zamijeni(sql, "@@jupitersrvpath", k.JupiterSrvPath);
            sql = Zamijeni(sql, "@@editedcolumn", k.EditedColumn);
            sql = Zamijeni(sql, "@@EDITEDCOLUMN", k.EditedColumn);
            sql = Zamijeni(sql, "@@lastvalue", k.LastValue);
            sql = Zamijeni(sql, "@@LASTVALUE", k.LastValue);
            sql = Zamijeni(sql, "@@selectedcolor", k.SelectedColor);
            sql = Zamijeni(sql, "@@SELECTEDCOLOR", k.SelectedColor);
            return sql;
        }

        /// <summary>
        /// Nakon zamjene ~~ u zarez. "prazno" | "pocinje-s-@" | "ostao-@@" | "salji".
        /// </summary>
        public static string Odluka(string sql)
        {
            if (sql == null)
                sql = "";
            sql = sql.Replace("~~", ",");
            if (sql.Trim().StartsWith("@", StringComparison.Ordinal))
                return "pocinje-s-@";
            if (sql.Length == 0 || sql.IndexOf("@@", StringComparison.Ordinal) > 0)
                return sql.Length == 0 ? "prazno" : "ostao-@@";
            return "salji";
        }

        public static bool JeZabranaPisanja(string sql)
        {
            if (sql == null)
                return false;
            string t = sql.TrimStart();
            int i = 0;
            while (i < t.Length && char.IsWhiteSpace(t[i]))
                i++;
            string rijec = "";
            while (i < t.Length && char.IsLetter(t[i]))
            {
                rijec += char.ToLowerInvariant(t[i]);
                i++;
            }
            return rijec == "insert" || rijec == "update" || rijec == "delete"
                || rijec == "drop" || rijec == "alter" || rijec == "truncate"
                || rijec == "merge" || rijec == "create";
        }

        private static string Zamijeni(string sql, string token, string vrijednost)
        {
            if (vrijednost == null)
                vrijednost = "";
            return sql.Replace(token, vrijednost);
        }
    }

    public static class SpinVeza
    {
        public static string Spoj(string server, string baza)
        {
            if (!string.IsNullOrEmpty(server) && server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0)
                throw new InvalidOperationException("jupiter2022 se ne koristi.");
            return "Server=" + server + ";Database=" + baza + ";Integrated Security=true;";
        }
    }
}
