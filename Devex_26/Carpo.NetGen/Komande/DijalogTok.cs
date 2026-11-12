using System;
using System.Collections.Generic;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Dijalog kako Form2 radi: ONCLICK, CMD1 = OK, FILLEXPLORER, combo/grid SQL, NEXTFORM.
    /// Na jupiter2019 i jupiter2022 gumb i ONSUBMIT se ne šalju.
    /// </summary>
    public static class DijalogTok
    {
        public sealed class Punjenje
        {
            public string Ime = "";
            public string Sql = "";
            public bool Preskoceno;
        }

        public sealed class Klik
        {
            public string OtvaraFormu = "";
            public bool Zatvara;
            public bool Odustani;
            public bool FillExplorer;
            public string MasterSql = "";
            public bool SaljeSql;
            public string NextForm = "";
            public readonly List<string> Naredbe = new List<string>();
        }

        public static bool JeJupiter(string server)
        {
            if (string.IsNullOrEmpty(server))
                return false;
            return server.IndexOf("jupiter2019", StringComparison.OrdinalIgnoreCase) >= 0
                || server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Upis smije samo klik-pregled na bazi spin, server localhost na sql-ai.
        /// jupiter2019, jupiter2022 i svaka druga baza ostaju zatvorene.
        /// </summary>
        public static bool UpisNaSpinu(string server, string baza)
        {
            if (!string.Equals((baza ?? "").Trim(), "spin", StringComparison.OrdinalIgnoreCase))
                return false;
            if (JeJupiter(server))
                return false;
            string s = (server ?? "").Trim();
            bool lokal = s.Length == 0
                || string.Equals(s, "localhost", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, ".", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, "(local)", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
                || s.IndexOf("sql-ai", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!lokal)
                return false;
            string akcija = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") ?? "";
            string stroj = Environment.MachineName ?? "";
            return string.Equals(akcija, "true", StringComparison.OrdinalIgnoreCase)
                || stroj.IndexOf("sql-ai", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool DozvoliUpis;
        public static string TekstUpisa = "";

        public static string NextForm(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("NEXTFORM"))
                return "";
            return (forma.Atributi.Vrijednost("NEXTFORM") ?? "").Trim();
        }

        public static bool PuniExplorer(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("FILLEXPLORER"))
                return false;
            return string.Equals((forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        public static List<Punjenje> PriOtvori(NgForm forma)
        {
            var lista = new List<Punjenje>();
            if (forma == null || forma.Kontrole == null)
                return lista;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k.Grana != NgGrana.Combo && k.Grana != NgGrana.Grid && k.Grana != NgGrana.DxGrid)
                    continue;
                var p = new Punjenje();
                p.Ime = k.Name ?? "";
                if (k.Grana != NgGrana.Combo && Skip(k))
                {
                    p.Preskoceno = true;
                    lista.Add(p);
                    continue;
                }
                if (k.Atributi != null && k.Atributi.Ima("SQL"))
                    p.Sql = (k.Atributi.Vrijednost("SQL") ?? "").Trim();
                lista.Add(p);
            }
            return lista;
        }

        public static Klik Gumb(NgForm forma, int redni, string onClick, bool jupiter)
        {
            var k = new Klik();
            k.NextForm = NextForm(forma);
            string naredba = (onClick ?? "").Trim();
            if (naredba.Length == 0 && redni == 2)
            {
                k.Odustani = true;
                k.Zatvara = true;
                k.NextForm = "";
                return k;
            }
            if (naredba.Length == 0 && redni == 1)
            {
                k.Zatvara = true;
                k.FillExplorer = PuniExplorer(forma);
                if (k.FillExplorer && forma != null)
                    k.MasterSql = Lanac.Lijepi((forma.OnSubmit ?? "").Trim(), "");
                k.SaljeSql = !jupiter && k.MasterSql.Length > 0 && SmijePoslati(k.MasterSql);
                return k;
            }
            string[] seg = Lanac.Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                if (t.Length == 0)
                    continue;
                if (t.StartsWith(":", StringComparison.Ordinal))
                {
                    k.OtvaraFormu = t.Substring(1).Trim();
                    k.Zatvara = false;
                    k.SaljeSql = false;
                    continue;
                }
                string u = t.ToUpperInvariant();
                if (u == "CLOSE" || u == "CANCEL")
                {
                    k.Odustani = true;
                    k.Zatvara = true;
                    k.NextForm = "";
                    k.SaljeSql = false;
                }
                else if (u == "OK")
                {
                    k.Zatvara = true;
                    if (forma != null)
                        k.MasterSql = Lanac.Lijepi((forma.OnSubmit ?? "").Trim(), "");
                    if (!jupiter && k.MasterSql.Length > 0 && SmijePoslati(k.MasterSql))
                        k.SaljeSql = true;
                }
                else if (u == "OKSTAY")
                {
                    k.Zatvara = false;
                }
                else
                {
                    string svedeno = Lanac.Lijepi(t, "");
                    k.Naredbe.Add(svedeno);
                    if (!jupiter && SmijePoslati(svedeno))
                        k.SaljeSql = true;
                }
            }
            if (jupiter)
                k.SaljeSql = false;
            return k;
        }

        /// <summary>
        /// Form2.vb 3655 stavi tekst kontrole kao @Ime='vrijednost'.
        /// Class1.vb 765-868 zamijeni @@Ime tom vrijednošću. XML već ima navodnike
        /// oko @@Naziv, pa se u taj par stavlja sirovi tekst kontrole.
        /// </summary>
        public static string ZamijeniTokene(string sql, string id, IDictionary<string, string> kontrole)
        {
            if (string.IsNullOrEmpty(sql))
                return "";
            string s = sql;
            s = s.Replace("@@id", id ?? "");
            s = s.Replace("@@ID", id ?? "");
            s = s.Replace("@@Id", id ?? "");
            if (kontrole != null)
            {
                foreach (var par in kontrole)
                {
                    if (string.IsNullOrEmpty(par.Key))
                        continue;
                    s = ZamijeniIme(s, par.Key, par.Value ?? "");
                }
            }
            // Form2.vb 3818: grid bez kvačica da prazan niz. Inače @@ ostane u navodniku i stora ga dobije kao tekst.
            s = ZamijeniIme(s, "nizoznacenihajdijevaslijedeci", "");
            return s;
        }

        private static string ZamijeniIme(string sql, string ime, string vrijednost)
        {
            string[] tokeni = new string[] { "@@" + ime, "@@" + ime.ToUpperInvariant(), "@@" + ime.ToLowerInvariant() };
            string s = sql;
            for (int i = 0; i < tokeni.Length; i++)
            {
                string token = tokeni[i];
                int p = 0;
                while ((p = s.IndexOf(token, p, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    bool uNavodniku = p > 0 && s[p - 1] == '\'';
                    string ubaci = uNavodniku ? (vrijednost ?? "").Replace("'", "''") : "'" + (vrijednost ?? "").Replace("'", "''") + "'";
                    s = s.Substring(0, p) + ubaci + s.Substring(p + token.Length);
                    p += ubaci.Length;
                }
            }
            return s;
        }

        public static bool SmijePoslati(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;
            if (sql.IndexOf("cis.porezna-uprava.hr", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            if (!ZastitaSql.Tekst(sql).Pise)
                return true;
            return DozvoliUpis;
        }

        public static bool PreskociGrid(NgControl k)
        {
            return Skip(k);
        }

        private static bool Skip(NgControl k)
        {
            if (k.Atributi == null || !k.Atributi.Ima("SKIPDEFAULTLOAD"))
                return false;
            return string.Equals((k.Atributi.Vrijednost("SKIPDEFAULTLOAD") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
