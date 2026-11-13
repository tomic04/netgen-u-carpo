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

        /// <summary>
        /// Jedan segment Rafalna. Form2.vb 7031–7169 ne izlazi iz petlje na MSGBOX ni na CLOSEPARENT.
        /// </summary>
        public sealed class Korak
        {
            public string Vrsta = "";
            public string Tekst = "";
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

        /// <summary>FILLEXPLORER=onlytree samo NapuniDrvoVrijeme. Form1.vb 5475–5476.</summary>
        public static bool OsvjeziStablo(NgForm forma)
        {
            return string.Equals(FillExplorer(forma), "onlytree", StringComparison.Ordinal);
        }

        /// <summary>FILLEXPLORER=ONLYDET ne dira master. Osvježi detail. Form1.vb 5470–5473.</summary>
        public static bool OsvjeziDetalj(NgForm forma)
        {
            return string.Equals(FillExplorer(forma), "onlydet", StringComparison.Ordinal);
        }

        private static string FillExplorer(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("FILLEXPLORER"))
                return "";
            return (forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim().ToLowerInvariant();
        }

        /// <summary>FILLEXPLORER=TRUE zamijeni master sa ONSUBMIT. Form1.vb 5445–5453.</summary>
        public static string SqlNakonOk(NgForm forma)
        {
            if (!PuniExplorer(forma))
                return "";
            string stora = forma == null ? "" : (forma.OnSubmit ?? "").Trim();
            if (stora.Length == 0)
                return "";
            return Lanac.Lijepi(stora, "");
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
                // Form2.vb 3377: CMD1 bez dvotočke je OK. ONSUBMIT se šalje i kad
                // FILLEXPLORER nije postavljen. DELETEFORM Obriši je takav gumb.
                k.Zatvara = true;
                k.FillExplorer = PuniExplorer(forma);
                if (forma != null)
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
                    bool jeNull = string.Equals(vrijednost, "null", StringComparison.OrdinalIgnoreCase);
                    bool prazan = string.IsNullOrEmpty(vrijednost);
                    // Form2.vb 3616: prazan search id ide kao null, ne kao ''.
                    string ubaci = (jeNull || prazan) && !uNavodniku
                        ? "null"
                        : (uNavodniku ? (vrijednost ?? "").Replace("'", "''") : "'" + (vrijednost ?? "").Replace("'", "''") + "'");
                    s = s.Substring(0, p) + ubaci + s.Substring(p + token.Length);
                    p += ubaci.Length;
                }
            }
            return s;
        }

        /// <summary>
        /// Form2.vb 1195 izvršava ONLOAD bez obzira počinje li sa select.
        /// spStatusiQuery @action=2 puni @@vrstadokid iz rezultata, ne iz kontrole.
        /// </summary>
        public static bool OnLoadJeUpit(string on)
        {
            string t = (on ?? "").TrimStart();
            if (t.Length == 0 || t.StartsWith(":", StringComparison.Ordinal))
                return false;
            // Ime forme nema razmak (INSERT_KLASA). SQL ga ima, i kad počinje sa if.
            // Form2.vb 1195 izvršava ONLOAD, Form1 ne otvara formu iz tog teksta.
            return t.IndexOf(' ') >= 0 || t.IndexOf('\n') >= 0 || t.IndexOf('\r') >= 0;
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

        /// <summary>
        /// Redoslijed je cijeli ONCLICK. MSGBOX ne prekida (7165–7169).
        /// CLOSEPARENT postavi zastavicu i ide u Button2, petlja ide dalje (7035–7037).
        /// </summary>
        public static List<Korak> Koraci(string onClick)
        {
            var lista = new List<Korak>();
            string[] seg = Lanac.Segmenti(onClick ?? "");
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                if (t.Length == 0)
                    continue;
                var k = new Korak();
                k.Tekst = t;
                string u = t.ToUpperInvariant();
                if (ForeachNaredba.Vrsta(t).Length > 0)
                {
                    string tijelo = "";
                    if (i + 1 < seg.Length)
                    {
                        tijelo = (seg[i + 1] ?? "").Trim();
                        i++;
                    }
                    k.Vrsta = "foreach";
                    k.Tekst = ForeachNaredba.Vrsta(t) + "|" + ForeachNaredba.Grid(t) + "|" + tijelo;
                }
                else if (u.IndexOf("MSGBOX(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "poruka";
                    k.Tekst = PorukaNaredba.Tekst(t);
                }
                else if (FiskalNaredba.Tablica(t) != 0)
                    k.Vrsta = "fiskal";
                else if (RafalNaredba.Datoteka(t) != 0)
                    k.Vrsta = "datoteka";
                else if (RafalNaredba.RefreshKontrola(t).Length > 0)
                {
                    k.Vrsta = "refresh";
                    k.Tekst = RafalNaredba.RefreshKontrola(t);
                }
                else if (RafalNaredba.Dock(t).Length > 0)
                {
                    k.Vrsta = "dock";
                    k.Tekst = RafalNaredba.Dock(t) + "|" + RafalNaredba.DockKontrola(t);
                }
                else if (u.IndexOf("RECALCGRID", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "recalc";
                else if (u.IndexOf("TOGGLEGRIDGROUP(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "toggle";
                else if (u.IndexOf("PREPAREMULTISELECTEDPRINT(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "priprema";
                else if (u.IndexOf("PIVOTGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "pivot";
                else if (u.IndexOf("CHARTGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "chart";
                else if (u.IndexOf("COLORPICKER(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "boja";
                else if (PosaljiServer.Vrsta(t).Length > 0)
                    k.Vrsta = "posalji";
                else if (Pokretanje.Je(t))
                    k.Vrsta = "pokretanje";
                else if (u.IndexOf("EVAL(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "eval";
                    k.Tekst = RafalNaredba.EvalLijevo(t) + "=" + RafalNaredba.EvalDesno(t);
                }
                else if (RafalNaredba.MasterSljedeci(t))
                    k.Vrsta = "sljedeci";
                else if (RafalNaredba.MasterPrethodni(t))
                    k.Vrsta = "prethodni";
                else if (RafalNaredba.Slopp(t) || u.IndexOf("CREATEOBJECT(", StringComparison.Ordinal) >= 0
                    || u.IndexOf("ISEMPTYORZERO", StringComparison.Ordinal) >= 0 || PraznoNaredba.Je(t))
                    k.Vrsta = "naredba";
                else if (u == "CLOSE" || u == "CANCEL")
                    k.Vrsta = "odustani";
                else if (u == "CLOSEPARENT")
                    k.Vrsta = "roditelj";
                else if (u == "OK")
                    k.Vrsta = "ok";
                else if (u == "OKSTAY")
                    k.Vrsta = "okstay";
                else if (t.StartsWith(":", StringComparison.Ordinal))
                {
                    k.Vrsta = "forma";
                    k.Tekst = t.Substring(1).Trim();
                }
                else if (KlikNaredba.Kontrola(t).Length > 0)
                {
                    k.Vrsta = "klik";
                    k.Tekst = KlikNaredba.Kontrola(t);
                }
                else
                    k.Vrsta = "sql";
                lista.Add(k);
            }
            return lista;
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
