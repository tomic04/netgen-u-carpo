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
        public string TempString = "";
        public string DblClickedColumn = "";
        public string Version = "";
        public string OpisniParametri = "";
        public string FocusedControl = "";
        public string SelectedFile = "";
        public string SelectedPath = "";
        /// <summary>cls1.strProjectCaption = naslov podloge (Form1.vb 3530); ime spremljenog izgleda grida dijaloga.</summary>
        public string ProjektNaslov = "";
        /// <summary>strHelpKod (HELPKOD projekta, velikim slovima): dijalog bez svog HELPKOD-a koristi njega (Form2.vb 1050).</summary>
        public string HelpKod = "";
        /// <summary>strXML (put XML-a, remotedllpath\SharedDLLs\xml\Mid) i Version1 (VERSION projekta): ključ veličine dijaloga (VEL-01).</summary>
        public string XmlPut = "";
        public string ProjektVerzija = "";
        /// <summary>
        /// Medjuspremnik podloge (SETDEFAULTS, Form1.vb 3945-4009). U NetGenu je cls1.CB globalan,
        /// pa ga NadjiVrijednostuPrevParametrima vidi i u dijalogu i na gumbu (CB-01).
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> Clipboard;
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
            // Form1.vb 2304 i Class1.vb 740-853: @@ iz retka grida rjesava ista petlja kao
            // nakon forme (NadjiVrijednostuPrevParametrima skida navodnike, slaze datum).
            // Prije je ovdje bila DetailGrid.ZamijeniOstatak s drugim pravilima: '@@x' je
            // postao ''x'', datum je ostao 13.11.2026. 0:00:00, a token pred prijelomom
            // reda je progutao SQL do sljedeceg razmaka (SQL-04).
            if (!string.IsNullOrEmpty(k.ParametriSaGrida))
                sql = Carpo.NetGen.Komande.SrediPoslije.Petlja(sql, k.ParametriSaGrida, k.Clipboard, false);
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
                // Form1.vb 2567-2571: Value.ToString u kulturi racunala (datum 13.11.2026. 0:00:00,
                // koji NadjiVrijednostuPrevParametrima slozi u '20261113'), a broj dobije tocku.
                // Invarijantna kultura je davala 11/13/2026 00:00:00 (SQL-04).
                string tekst = Convert.ToString(v, System.Globalization.CultureInfo.CurrentCulture) ?? "";
                double broj;
                if ((v is IFormattable && !(v is string) && !(v is DateTime))
                    || (v is string && double.TryParse(tekst, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out broj)))
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
                s = Carpo.NetGen.Komande.SrediPoslije.Petlja(s, k == null ? "" : k.ParametriSaGrida, k == null ? null : k.Clipboard, false);
            return s;
        }

        /// <summary>
        /// Form2.SrediSQL2 (2876–2920). Nema @@Id ni @@table. @@GRIDROWCOUNT( je zakomentiran (2899–2905).
        /// </summary>
        public static string Sredi2(string sql, SqlKontekst k)
        {
            if (sql == null)
                return "";
            if (k == null)
                k = new SqlKontekst();
            sql = Zamijeni(sql, "@@ID", k.Id);
            sql = Zamijeni(sql, "@@id", k.Id);
            sql = Zamijeni(sql, "@@operateriid", k.OperaterId);
            sql = Zamijeni(sql, "@@OPERATERIID", k.OperaterId);
            sql = Zamijeni(sql, "@@DETID", k.DetId);
            sql = Zamijeni(sql, "@@detid", k.DetId);
            sql = Zamijeni(sql, "@@DetId", k.DetId);
            sql = Zamijeni(sql, "@@node", k.Node);
            sql = Zamijeni(sql, "@@NODE", k.Node);
            sql = Zamijeni(sql, "@@tempstring", k.TempString);
            sql = Zamijeni(sql, "@@TEMPSTRING", k.TempString);
            sql = Zamijeni(sql, "@@editedcolumn", k.EditedColumn);
            sql = Zamijeni(sql, "@@EDITEDCOLUMN", k.EditedColumn);
            sql = Zamijeni(sql, "@@lastvalue", k.LastValue);
            sql = Zamijeni(sql, "@@LASTVALUE", k.LastValue);
            sql = Zamijeni(sql, "@@dblclickedcolumn", k.DblClickedColumn);
            sql = Zamijeni(sql, "@@DBLCLICKEDCOLUMN", k.DblClickedColumn);
            sql = Zamijeni(sql, "@@VERSION", k.Version);
            sql = Zamijeni(sql, "@@OPISNIPARAMETRI", k.OpisniParametri);
            sql = Zamijeni(sql, "@@FOCUSEDCONTROL", k.FocusedControl);
            sql = Zamijeni(sql, "@@selectedfile", k.SelectedFile);
            sql = Zamijeni(sql, "@@selectedpath", k.SelectedPath);
            if (sql.IndexOf("@@xmlname", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sql = Zamijeni(sql, "@@xmlname", k.XmlName);
                sql = Zamijeni(sql, "@@XMLNAME", k.XmlName);
            }
            if (sql.IndexOf("@@jupitersrvpath", StringComparison.OrdinalIgnoreCase) >= 0)
                sql = Zamijeni(sql, "@@jupitersrvpath", k.JupiterSrvPath);
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
                k.TempString = izvor.TempString;
                k.DblClickedColumn = izvor.DblClickedColumn;
                k.Version = izvor.Version;
                k.OpisniParametri = izvor.OpisniParametri;
                k.FocusedControl = izvor.FocusedControl;
                k.SelectedFile = izvor.SelectedFile;
                k.SelectedPath = izvor.SelectedPath;
                k.ProjektNaslov = izvor.ProjektNaslov;
                k.HelpKod = izvor.HelpKod;
                k.XmlPut = izvor.XmlPut;
                k.ProjektVerzija = izvor.ProjektVerzija;
                k.Clipboard = izvor.Clipboard;
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
            // B-JUP: u pravom Jupiter ulazu nema zabrane pisanja (DijalogTok.Produkcija).
            if (sql == null || Carpo.NetGen.Komande.DijalogTok.Produkcija)
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
    /// Novija vrijednost ide prva jer SrediPoslije.Nadji uzima prvi pogodak (Class1.vb 863-884).
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
        private static string _korisnik = "";
        private static string _lozinka = "";

        /// <summary>
        /// ULZ-07: korisnik i lozinka iz Run/Run8 (Jupiter). Form1 1142 i 1563-1568: s korisnikom NetGen spaja
        /// SQL prijavom (UID/PWD), bez njega Windows prijavom; isto Carpo Starter (Constructor.SetConnectionString).
        /// Lozinka se nigdje ne zapisuje (NgDnevnik skriva pwd= i password=).
        /// </summary>
        public static void Prijava(string korisnik, string lozinka)
        {
            _korisnik = korisnik ?? "";
            _lozinka = lozinka ?? "";
        }

        /// <summary>Ima li SQL prijave (korisnik iz Run8).</summary>
        public static bool ImaSqlPrijavu
        {
            get { return _korisnik.Length > 0; }
        }

        /// <summary>Isti način prijave za drugi spoj (JupiterXML, tstr6 u Form1 1564-1568).</summary>
        public static void PostaviPrijavu(System.Data.SqlClient.SqlConnectionStringBuilder b)
        {
            if (_korisnik.Length == 0)
            {
                b.IntegratedSecurity = true;
                return;
            }
            b.IntegratedSecurity = false;
            b.UserID = _korisnik;
            b.Password = _lozinka;
        }

        public static string Spoj(string server, string baza)
        {
            if (!Carpo.NetGen.Komande.DijalogTok.Produkcija && !string.IsNullOrEmpty(server)
                && server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0)
                throw new InvalidOperationException("jupiter2022 se ne koristi.");
            if (_korisnik.Length == 0)
                return "Server=" + server + ";Database=" + baza + ";Integrated Security=true;TrustServerCertificate=True;";
            var b = new System.Data.SqlClient.SqlConnectionStringBuilder();
            b.DataSource = server ?? "";
            b.InitialCatalog = baza ?? "";
            PostaviPrijavu(b);
            b.TrustServerCertificate = true;
            return b.ConnectionString + ";";
        }

        /// <summary>
        /// Ime aplikacije u SQL Profileru i sp_who (APP-01). NetGen: "NetGenDLL-" + ime XML-a (Form1 1176-1177,
        /// 1564). Carpo granula: ime sklopa + "." + modul (Starter 109). Port: "Carpo.NetGen." + ime XML-a bez nastavka.
        /// </summary>
        public static string SaAplikacijom(string spoj, string xmlPut)
        {
            string ime = "";
            try
            {
                ime = System.IO.Path.GetFileNameWithoutExtension((xmlPut ?? "").Trim()) ?? "";
            }
            catch (ArgumentException)
            {
                ime = "";
            }
            var sb = new System.Text.StringBuilder("Carpo.NetGen");
            if (ime.Length > 0)
                sb.Append('.');
            for (int i = 0; i < ime.Length && sb.Length < 120; i++)
            {
                char c = ime[i];
                sb.Append(c == ';' || c == '=' || c == '\'' || c == '"' ? '_' : c);
            }
            string s = spoj ?? "";
            if (s.Length > 0 && !s.EndsWith(";", StringComparison.Ordinal))
                s += ";";
            return s + "Application Name=" + sb.ToString() + ";";
        }
    }
}
