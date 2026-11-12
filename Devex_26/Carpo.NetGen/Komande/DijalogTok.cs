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
            string naredba = ProsiriClick(forma, (onClick ?? "").Trim());
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
                k.SaljeSql = !jupiter && k.MasterSql.Length > 0 && !ZastitaSql.Tekst(k.MasterSql).Pise;
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
                    k.OtvaraFormu = t.Replace(":", "").Trim();
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
                    if (!jupiter && k.MasterSql.Length > 0 && !ZastitaSql.Tekst(k.MasterSql).Pise)
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
                    if (!jupiter && !ZastitaSql.Tekst(svedeno).Pise)
                        k.SaljeSql = true;
                }
            }
            if (jupiter)
                k.SaljeSql = false;
            return k;
        }

        /// <summary>CLICK(ime) zamijeni ONCLICK te kontrole. Gumb ide preko Tag = ONCLICK. Form2.vb 6143–6146.</summary>
        public static string ProsiriClick(NgForm forma, string naredba)
        {
            string[] seg = Lanac.Segmenti(naredba);
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                string ime = Lanac.ImeKlika(t);
                if (ime.Length > 0)
                {
                    string onclick = OnClickKontrole(forma, ime);
                    if (onclick.Length > 0)
                        t = onclick;
                }
                if (sb.Length > 0)
                    sb.Append('#');
                sb.Append(t);
            }
            return sb.ToString();
        }

        public static string OnClickKontrole(NgForm forma, string ime)
        {
            if (forma == null || forma.Kontrole == null || string.IsNullOrEmpty(ime))
                return "";
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (!string.Equals(k.Name, ime, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (k.Grana != NgGrana.Button || k.Atributi == null)
                    return "";
                return (k.Atributi.Vrijednost("ONCLICK") ?? "").Trim();
            }
            return "";
        }

        /// <summary>
        /// Pokaži je Button1. FILLEXPLORER true šalje ONSUBMIT. Form2.vb 3367, Form1.vb 5445.
        /// </summary>
        public static string SqlNakonOk(NgForm forma, SqlKontekst kontekst, IDictionary<string, string> cb)
        {
            return SqlNakonOk(forma, kontekst, cb, "");
        }

        public static string SqlNakonOk(NgForm forma, SqlKontekst kontekst, IDictionary<string, string> cb, string parametri)
        {
            Klik klik = Gumb(forma, 1, "", false);
            if (!klik.FillExplorer)
                return "";
            string sql = klik.MasterSql ?? "";
            if (!string.IsNullOrWhiteSpace(parametri) && !Lanac.Prvih6JeDml(sql) && sql.IndexOf("!!", StringComparison.Ordinal) < 0)
                sql = sql.TrimEnd() + " " + parametri.Trim();
            sql = SqlTekst.Sredi(sql, kontekst);
            return Dozvola.Poslije(sql, cb);
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
