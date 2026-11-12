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

        public sealed class Razrada
        {
            public bool OkClick;
            public bool Zatvara;
            public bool Prekinuto;
            public bool ImaUpis;
            public bool Salje;
            public bool MasterIzSubmita;
            public bool SamoStablo;
            public bool PonoviZadnjiMaster;
            public string OtvaraFormu = "";
            public string Fill = "";
            public string Submit = "";
            public readonly List<string> PrijeOk = new List<string>();
            public readonly List<string> NijeSql = new List<string>();
        }

        public static Razrada Razradi(NgForm forma, string onclick, bool jupiter, string parametri, IDictionary<string, string> vrijednosti)
        {
            var k = new Razrada();
            string[] seg = Lanac.Segmenti(onclick);
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                if (t.Length == 0)
                    continue;
                t = Zamijeni(t, vrijednosti);
                if (t.StartsWith(":", StringComparison.Ordinal))
                {
                    string ime = t.Substring(1).Trim();
                    int z = ime.IndexOf('(');
                    if (z >= 0)
                        ime = ime.Substring(0, z).Trim();
                    k.OtvaraFormu = ime;
                    k.Zatvara = false;
                    continue;
                }
                if (t.StartsWith("BLAGAJNAENABLE", StringComparison.OrdinalIgnoreCase))
                {
                    k.NijeSql.Add(t);
                    continue;
                }
                string u = t.ToUpperInvariant();
                if (u == "CLOSE" || u == "CANCEL" || u == "CLOSEPARENT")
                {
                    k.OkClick = false;
                    k.Zatvara = true;
                    k.Salje = false;
                    return k;
                }
                if (u == "OKSTAY")
                {
                    k.OkClick = true;
                    k.Zatvara = false;
                    continue;
                }
                if (u == "OK")
                {
                    k.OkClick = true;
                    k.Zatvara = true;
                    PrimijeniSubmit(k, forma, parametri, vrijednosti, jupiter);
                    return k;
                }
                if (ZastitaSql.Tekst(t).Pise)
                    k.ImaUpis = true;
                if (t.IndexOf("@@", StringComparison.Ordinal) >= 0)
                {
                    k.PrijeOk.Add(t);
                    k.Prekinuto = true;
                    k.Zatvara = false;
                    k.OkClick = false;
                    k.Salje = false;
                    return k;
                }
                k.PrijeOk.Add(t);
            }
            if (jupiter || k.ImaUpis)
                k.Salje = false;
            return k;
        }

        private static void PrimijeniSubmit(Razrada k, NgForm forma, string parametri, IDictionary<string, string> vrijednosti, bool jupiter)
        {
            string fill = "";
            if (forma != null && forma.Atributi != null && forma.Atributi.Ima("FILLEXPLORER"))
                fill = (forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim().ToLowerInvariant();
            k.Fill = fill;
            string submit = forma == null ? "" : (forma.OnSubmit ?? "");
            submit = Zamijeni(submit, vrijednosti);
            submit = Lanac.Lijepi(submit, parametri);
            k.Submit = submit.Trim();
            if (fill == "true")
                k.MasterIzSubmita = k.Submit.Length > 0;
            else if (fill == "onlytree")
                k.SamoStablo = true;
            else if (fill.Length == 0 && k.Submit.Length == 0)
                k.PonoviZadnjiMaster = true;
            bool submitPise = k.Submit.Length > 0 && ZastitaSql.Tekst(k.Submit).Pise;
            if (submitPise)
                k.ImaUpis = true;
            k.Salje = !jupiter && !k.ImaUpis && (k.PrijeOk.Count > 0 || (k.Submit.Length > 0 && !k.SamoStablo));
        }

        private static string Zamijeni(string sql, IDictionary<string, string> vrijednosti)
        {
            if (string.IsNullOrEmpty(sql) || vrijednosti == null)
                return sql ?? "";
            foreach (KeyValuePair<string, string> kv in vrijednosti)
            {
                string token = "@@" + kv.Key;
                int i = 0;
                while (i < sql.Length)
                {
                    int p = sql.IndexOf(token, i, StringComparison.OrdinalIgnoreCase);
                    if (p < 0)
                        break;
                    string v = kv.Value ?? "";
                    sql = sql.Substring(0, p) + v + sql.Substring(p + token.Length);
                    i = p + v.Length;
                }
            }
            return sql;
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
