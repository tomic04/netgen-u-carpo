using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Lanac naredbi kao NovaGenerickaForma i Form2.Rafalno.
    /// Tilda postaje # tek ovdje (Form1.vb 4558), ne u parseru.
    /// </summary>
    public static class Lanac
    {
        public const string Dml = "INSERTDELETEUPDATESELECT";

        public sealed class Ishod
        {
            public bool Prekinuto;
            public bool Zatvara;
            public bool Ostaje;
            public bool CloseParent;
            public bool Button2;
            public bool RefreshMasterGrid;
            public string Fokus = "";
            public string Foreach = "";
            public readonly List<string> Odigrano = new List<string>();
        }

        public static string[] Segmenti(string naredba)
        {
            return (naredba ?? "").Replace("~", "#").Split('#');
        }

        /// <summary>
        /// REFRESHMASTERGRID ponovi zadnji SQL. Segment refreshmaster nije ta riječ,
        /// ali prazna forma i dalje ima OKClick pa PokaziUpit (Form1.vb 4570, 5354–5370, 5519).
        /// </summary>
        public static bool OsvjezavaZadnjiSql(string naredba)
        {
            string[] seg = Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string u = (seg[i] ?? "").Trim();
                if (u.StartsWith(":", StringComparison.Ordinal))
                    u = u.Substring(1).Trim();
                if (u.Equals("REFRESHMASTERGRID", StringComparison.OrdinalIgnoreCase)
                    || u.Equals("REFRESHMASTER", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string IspisSegment(string naredba)
        {
            string[] seg = Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string s = (seg[i] ?? "").Trim();
                if (s.StartsWith("PRINT", StringComparison.OrdinalIgnoreCase))
                    return s;
            }
            return "";
        }

        /// <summary>ONLOAD koji počinje s : ide u NovaGenerickaForma i reže se na #. Form1.vb 3879, 4558.</summary>
        public static string[] OnLoadLanac(string onload)
        {
            string s = (onload ?? "").Trim();
            if (!s.StartsWith(":", StringComparison.Ordinal))
                return new string[0];
            return s.Substring(1).Replace("~", "#").Split('#');
        }

        /// <summary>select case ... then 'disable' else 'enable' end as Natpis. Vrijednost je @@izvjestaj.</summary>
        public static string GumbIzCase(string segment, string izvjestaj)
        {
            string s = (segment ?? "").Replace("@@izvjestaj", izvjestaj ?? "").Replace("@@IZVJESTAJ", izvjestaj ?? "");
            var navodnici = new List<string>();
            int i = 0;
            while (i < s.Length)
            {
                int a = s.IndexOf('\'', i);
                if (a < 0)
                    break;
                int b = s.IndexOf('\'', a + 1);
                if (b < 0)
                    break;
                navodnici.Add(s.Substring(a + 1, b - a - 1));
                i = b + 1;
            }
            if (navodnici.Count < 4)
                return "";
            string vrijednost = string.Equals(navodnici[0], navodnici[1], StringComparison.Ordinal) ? navodnici[2] : navodnici[3];
            int asAt = s.LastIndexOf(" as ", StringComparison.OrdinalIgnoreCase);
            if (asAt < 0)
                return "";
            return s.Substring(asAt + 4).Trim() + "=" + vrijednost;
        }

        public static bool Prvih6JeDml(string sql)
        {
            if (sql == null)
                return false;
            string s = sql.Trim();
            if (s.Length < 6)
                return false;
            return Dml.IndexOf(s.Substring(0, 6).ToUpperInvariant(), StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// sp... dobije drugi niz @ime= iako već ima parametre. !! na kraju to gasi.
        /// </summary>
        public static string Lijepi(string stora, string parametri)
        {
            string sql = stora ?? "";
            string bez = sql.Replace("!!", "");
            if (sql.Contains("!!"))
                return bez;
            string pocetak = bez.TrimStart();
            if (pocetak.StartsWith("exec", StringComparison.OrdinalIgnoreCase)
                || pocetak.StartsWith("execute", StringComparison.OrdinalIgnoreCase))
                return bez;
            if (Prvih6JeDml(bez))
                return bez;
            if (string.IsNullOrWhiteSpace(parametri))
                return sql;
            return sql + " " + parametri.Trim();
        }

        public static string Caption(string tekst)
        {
            return (tekst ?? "").Replace("!!", "\r\n");
        }

        public static string PrazanBroj()
        {
            return "0";
        }

        public static string PrazanTekst()
        {
            return "''";
        }

        public static string TekstVrijednost(string tekst)
        {
            return "'" + (tekst ?? "").Replace("'", "¸¸").Replace(",", "~~") + "'";
        }

        public static string USql(string vrijednost)
        {
            return (vrijednost ?? "").Replace("¸¸", "''");
        }

        /// <summary>Tag tražilice. Prazan ili 0 je null. Inače sirovi tag.</summary>
        public static string SearchParametar(string tag)
        {
            string t = (tag ?? "").Trim();
            if (t.Length == 0 || t == "0")
                return "null";
            return t;
        }

        /// <summary>roba#robaid#userid%#!cast(...)#%naziv% → tablica je prvi komad.</summary>
        public static string SearchTablica(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return "";
            string[] p = sql.Split('#');
            return p.Length == 0 ? "" : p[0];
        }

        public static string MultiDrop(IList<string> id)
        {
            if (id == null || id.Count == 0)
                return "";
            var sb = new StringBuilder();
            for (int i = 0; i < id.Count; i++)
            {
                if (i > 0)
                    sb.Append('~');
                sb.Append(id[i]);
            }
            return sb.ToString();
        }

        public static string Datum(int godina, int mjesec, int dan)
        {
            return new DateTime(godina, mjesec, dan).ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string PrviDanGodine(int godina)
        {
            return "01.01." + godina.ToString(CultureInfo.InvariantCulture);
        }

        public static bool AllowEmpty(string vrijednost)
        {
            return string.Equals((vrijednost ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        public static string Radio(bool ukljucen)
        {
            return ukljucen ? "1" : "0";
        }

        /// <summary>Form2.vb 6915: select 'refresh' as ime.</summary>
        public static string RefreshSql(string ime)
        {
            return "select 'refresh' as " + (ime ?? "").Trim();
        }

        public static string RefreshControl()
        {
            return "select refresh";
        }

        public static string MultiTemp()
        {
            return "delete-dva-pa-insert";
        }

        public static Ishod Izvrsi(string naredba, bool dijalog, string porukaGreske)
        {
            var ishod = new Ishod();
            string[] seg = Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string s = seg[i] ?? "";
                string t = s.Trim();
                if (t.Length == 0)
                {
                    ishod.Odigrano.Add("");
                    continue;
                }
                string u = t.ToUpperInvariant();
                if (u == "OKSTAY")
                {
                    ishod.Ostaje = true;
                    ishod.Zatvara = false;
                    ishod.Odigrano.Add("OKSTAY");
                    continue;
                }
                if (u == "OK")
                {
                    ishod.Zatvara = true;
                    ishod.Odigrano.Add("OK");
                    continue;
                }
                if (u == "CANCEL" || u == "CLOSE")
                {
                    ishod.Button2 = true;
                    ishod.Zatvara = true;
                    ishod.Odigrano.Add(u);
                    continue;
                }
                if (u == "CLOSEPARENT")
                {
                    ishod.CloseParent = true;
                    ishod.Button2 = true;
                    ishod.Zatvara = true;
                    ishod.Odigrano.Add("CLOSEPARENT");
                    continue;
                }
                if (u.Contains("REFRESHMASTERGRID"))
                {
                    ishod.RefreshMasterGrid = true;
                    ishod.Odigrano.Add("REFRESHMASTERGRID");
                    continue;
                }
                if (u.Contains("REFRESHMASTER"))
                {
                    ishod.Odigrano.Add("REFRESHMASTER");
                    continue;
                }
                if (u.Contains("FOREACHSELECTED") || u.Contains("FOREACHBLUEROW") || u.Contains("FOREACHROW"))
                {
                    if (dijalog)
                    {
                        if (u.Contains("FOREACHSELECTED"))
                            ishod.Foreach = "cekirani";
                        else if (u.Contains("FOREACHBLUEROW"))
                            ishod.Foreach = "plavi";
                        else
                            ishod.Foreach = "svi";
                        if (i + 1 < seg.Length)
                        {
                            ishod.Odigrano.Add(seg[i + 1]);
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                    continue;
                }
                if (t.IndexOf("spRaiserror", StringComparison.OrdinalIgnoreCase) >= 0 || !string.IsNullOrEmpty(porukaGreske) && t.IndexOf("raiserror", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string poruka = porukaGreske ?? "";
                    string fokus = Fokus(poruka);
                    if (fokus.Length > 0)
                        ishod.Fokus = fokus;
                    bool guta = poruka.IndexOf("#OK", StringComparison.OrdinalIgnoreCase) >= 0
                        || poruka.IndexOf("+OK", StringComparison.OrdinalIgnoreCase) >= 0;
                    ishod.Odigrano.Add("greska");
                    if (!guta)
                    {
                        ishod.Prekinuto = true;
                        break;
                    }
                    continue;
                }
                ishod.Odigrano.Add(t);
            }
            return ishod;
        }

        public static string Fokus(string poruka)
        {
            if (string.IsNullOrEmpty(poruka))
                return "";
            int p = poruka.IndexOf("+focus(", StringComparison.OrdinalIgnoreCase);
            if (p < 0)
                p = poruka.IndexOf("#focus(", StringComparison.OrdinalIgnoreCase);
            if (p < 0)
                return "";
            int a = poruka.IndexOf('(', p);
            int b = poruka.IndexOf(')', a + 1);
            if (a < 0 || b < 0)
                return "";
            return poruka.Substring(a + 1, b - a - 1).Trim();
        }
    }
}
