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
        public string ParametriSaGrida = "";
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
            if (!string.IsNullOrEmpty(k.ParametriSaGrida))
                sql = DetailGrid.ZamijeniOstatak(sql, k.ParametriSaGrida);
            return sql;
        }

        /// <summary>
        /// Form1.vb 2556-2576: stupci od drugog, zadnje 4 znamenke se skidaju ako nisu 0.
        /// Broj i tekst idu u navodnike. Zarez u broju postaje točka.
        /// </summary>
        public static string IzReda(System.Data.DataRow red)
        {
            if (red == null || red.Table == null)
                return "";
            var sb = new System.Text.StringBuilder();
            // Janus ima stupac 0 koji nije podatak, pa Form1.vb 2557 krene od 1.
            // DataTable nema taj stupac: prvi podatak je indeks 0, i to je statusiid.
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                object v = red[i];
                if (v == null || v == System.DBNull.Value)
                    continue;
                string ime = red.Table.Columns[i].ColumnName.Trim();
                if (ime.Length >= 4)
                {
                    int n;
                    if (int.TryParse(ime.Substring(ime.Length - 4), out n) && n != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                ime = ime.Replace(" ", "_").Replace("!", "");
                string tekst = Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture) ?? "";
                if (v is IFormattable && !(v is string))
                    tekst = tekst.Replace(",", ".");
                sb.Append("@").Append(ime).Append("='").Append(tekst.Replace("'", "''")).Append("' , ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gumb šalje SQL. Class1.vb 915: token kojeg nema ni u kontroli ni u međuspremniku je null.
        /// Combo SQL ostaje na Sredi da @@trgovinaid pričeka odabir.
        /// </summary>
        public static string ZaGumb(string sql, SqlKontekst k)
        {
            string s = Sredi(sql, k);
            if (s.IndexOf("@@", StringComparison.Ordinal) >= 0)
                s = DetailGrid.ZamijeniOstatak(s, k == null ? "" : k.ParametriSaGrida);
            return s;
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

        /// <summary>
        /// Form2 prije ProcitajXML: ID = GetID() (prazan grid je "0"), DetID = GetDetID() ("0"),
        /// Node ostaje onaj s podloge ("null" dok se čvor ne odabere). Form1.vb 5293–5298.
        /// </summary>
        public static SqlKontekst ZaFormu(SqlKontekst izvor)
        {
            var k = new SqlKontekst();
            if (izvor != null)
            {
                k.Id = izvor.Id;
                k.DetId = izvor.DetId;
                k.Node = izvor.Node;
                k.OperaterId = izvor.OperaterId;
                k.Tablica = izvor.Tablica;
                k.XmlName = izvor.XmlName;
                k.JupiterSrvPath = izvor.JupiterSrvPath;
                k.EditedColumn = izvor.EditedColumn;
                k.LastValue = izvor.LastValue;
                k.SelectedColor = izvor.SelectedColor;
                k.ParametriSaGrida = izvor.ParametriSaGrida;
            }
            if (string.IsNullOrEmpty(k.Id))
                k.Id = "0";
            if (string.IsNullOrEmpty(k.DetId))
                k.DetId = "0";
            if (k.Node == null)
                k.Node = "null";
            return k;
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

    /// <summary>
    /// Form2.vb 3906 strSviParametriuModulu. Dijete vidi ONLOAD stupce roditelja.
    /// Novija vrijednost ide prva jer DetailGrid.Nadji uzima prvi pogodak.
    /// </summary>
    public static class ParametriModula
    {
        public static string Dodaj(string roditelj, string dijete)
        {
            if (string.IsNullOrEmpty(dijete))
                return roditelj ?? "";
            if (string.IsNullOrEmpty(roditelj))
                return dijete;
            if (roditelj.IndexOf(dijete, StringComparison.Ordinal) >= 0)
                return roditelj;
            return dijete + roditelj;
        }
    }

    public static class SpinVeza
    {
        public static string Spoj(string server, string baza)
        {
            if (!string.IsNullOrEmpty(server) && server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0)
                throw new InvalidOperationException("jupiter2022 se ne koristi.");
            return "Server=" + server + ";Database=" + baza + ";Integrated Security=true;TrustServerCertificate=True;";
        }
    }
}
