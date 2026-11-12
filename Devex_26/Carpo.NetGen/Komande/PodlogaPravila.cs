using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// FILLEXPLORER nakon OKClick. Form1.vb 5369–5370 i 5445–5519.
    /// Prazan string nije true: ExecSql pa zadnji master.
    /// </summary>
    public static class FillExplorer
    {
        public static string Klasa(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("FILLEXPLORER"))
                return "";
            string v = (forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim();
            string l = v.ToLowerInvariant();
            if (l == "true" || l == "false" || l == "onlydet" || l == "onlytree"
                || l == "treeandrefreshmaster" || l == "treeandmaster")
                return l;
            return "prazan";
        }

        public static bool OtvaraDijalog(NgForm forma)
        {
            return forma != null && forma.Kontrole != null && forma.Kontrole.Count > 0;
        }

        public static bool ZamjenjujeMaster(string klasa)
        {
            return klasa == "true" || klasa == "treeandmaster";
        }

        public static bool PonavljaZadnji(string klasa)
        {
            return klasa == "prazan";
        }

        /// <summary>onlytree zove samo NapuniDrvoVrijeme. Master se ne šalje. Form1.vb 5475–5476.</summary>
        public static bool SamoStablo(string klasa)
        {
            return klasa == "onlytree";
        }

        public static bool SaljeMaster(string klasa)
        {
            return klasa == "true" || klasa == "treeandmaster" || klasa == "treeandrefreshmaster";
        }

        /// <summary>ONSUBMIT, plus parametri ako prvih 6 nije DML i nema !!. Form2.vb 3378–3388.</summary>
        public static string NoviMaster(NgForm forma, string parametri)
        {
            if (forma == null)
                return "";
            string stora = (forma.OnSubmit ?? "").Trim();
            if (stora.Length == 0)
                return "";
            if (stora.IndexOf("!!", StringComparison.Ordinal) >= 0)
                return stora.Replace("!!", "").Trim();
            if (string.IsNullOrWhiteSpace(parametri))
                return stora;
            return Lanac.Lijepi(stora, parametri).Trim();
        }

        /// <summary>
        /// Čvor čija forma nema kontrola i FILLEXPLORER je true: master postaje ONSUBMIT.
        /// </summary>
        public static string CvorSql(NgProject projekt)
        {
            if (projekt == null || string.IsNullOrEmpty(projekt.Tree))
                return "";
            string[] dijelovi = projekt.Tree.Split('#');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string seg = dijelovi[i] ?? "";
                int k = seg.IndexOf(':');
                if (k < 0 || k >= seg.Length - 1)
                    continue;
                NgForm forma = Nadji(projekt, seg.Substring(k + 1).Trim());
                if (forma == null || OtvaraDijalog(forma) || !ZamjenjujeMaster(Klasa(forma)))
                    continue;
                return NoviMaster(forma, "");
            }
            return "";
        }

        private static NgForm Nadji(NgProject projekt, string ime)
        {
            if (projekt.Forme == null || ime.Length == 0)
                return null;
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                if (string.Equals(projekt.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return projekt.Forme[i];
            }
            return null;
        }
    }

    /// <summary>
    /// ONLOAD2 indeksira red po Button.Text. Form1.vb 3914–3921. NAME i Tag nisu ključ.
    /// BUTTONS prepisuje prvih N gumba. Form1.vb 3770–3819. Samo j=6 čeka prazan Button7.
    /// </summary>
    public static class GumbNatpis
    {
        public static List<NgButton> Efektivni(NgProject projekt)
        {
            var po = new Dictionary<int, NgButton>();
            if (projekt != null && projekt.Gumbi != null)
            {
                for (int i = 0; i < projekt.Gumbi.Count; i++)
                {
                    NgButton g = projekt.Gumbi[i];
                    po[g.Redni] = g;
                }
            }
            string buttons = projekt == null ? "" : (projekt.Buttons ?? "");
            if (buttons.Trim().Length > 0)
            {
                string[] a = buttons.Split('#');
                for (int j = 0; j < a.Length && j < 10; j++)
                {
                    string dio = a[j] ?? "";
                    int kolon = dio.IndexOf(':');
                    if (kolon < 0)
                        continue;
                    int redni = j + 1;
                    NgButton postojeci;
                    if (redni == 7 && po.TryGetValue(7, out postojeci) && (postojeci.Natpis ?? "").Trim().Length > 0)
                        continue;
                    po[redni] = new NgButton(redni, dio.Substring(0, kolon), dio.Substring(kolon + 1));
                }
            }
            var lista = new List<NgButton>();
            for (int r = 1; r <= 10; r++)
            {
                NgButton g;
                if (po.TryGetValue(r, out g))
                    lista.Add(g);
            }
            return lista;
        }

        /// <summary>Stupac koji se zove kao natpis. Stupac koji se zove kao naredba ne dira gumb.</summary>
        public static string PoStupcu(IList<NgButton> gumbi, string stupac)
        {
            if (gumbi == null || string.IsNullOrEmpty(stupac))
                return "";
            for (int i = 0; i < gumbi.Count; i++)
            {
                if (string.Equals((gumbi[i].Natpis ?? "").Trim(), stupac.Trim(), StringComparison.OrdinalIgnoreCase))
                    return (gumbi[i].Natpis ?? "").Trim();
            }
            return "";
        }

        public static string Prvi(IList<NgButton> gumbi, IList<string> stupci)
        {
            if (stupci == null)
                return "";
            for (int i = 0; i < stupci.Count; i++)
            {
                string n = PoStupcu(gumbi, stupci[i]);
                if (n.Length > 0)
                    return n;
            }
            return "";
        }

        /// <summary>'Dodaj' = izraz. Literal iza kojeg nije = nije alias.</summary>
        public static List<string> Aliasi(string sql)
        {
            var lista = new List<string>();
            if (string.IsNullOrEmpty(sql))
                return lista;
            int i = 0;
            while (i < sql.Length)
            {
                int a = sql.IndexOf('\'', i);
                if (a < 0)
                    break;
                int b = sql.IndexOf('\'', a + 1);
                if (b < 0)
                    break;
                int j = b + 1;
                while (j < sql.Length && char.IsWhiteSpace(sql[j]))
                    j++;
                if (j < sql.Length && sql[j] == '=')
                    lista.Add(sql.Substring(a + 1, b - a - 1));
                i = b + 1;
            }
            return lista;
        }

        public static string Vrijednost(DataRow red, string natpis)
        {
            if (red == null || red.Table == null || string.IsNullOrEmpty(natpis))
                return "";
            if (!red.Table.Columns.Contains(natpis))
                return "";
            object v = red[natpis];
            if (v == null || v == DBNull.Value)
                return "";
            return Convert.ToString(v);
        }
    }

    /// <summary>
    /// Combo podloge: ValueMember je stupac 0, DisplayMember stupac 1. Form1.vb 3342–3344.
    /// SelectedValue postaje TREE. Form1.vb 7732 i 4344.
    /// </summary>
    public static class ComboStablo
    {
        public const int IndeksVrijednosti = 0;
        public const int IndeksPrikaza = 1;

        public static string Klasa(string vrijednost)
        {
            if (string.IsNullOrEmpty(vrijednost))
                return "";
            if (vrijednost.IndexOf('#') >= 0 && vrijednost.IndexOf(':') >= 0)
                return "tree";
            return "nije-tree";
        }
    }

    /// <summary>
    /// SETDEFAULTS stupac ide u CB kao ime=. ONLOAD2 onda mijenja @@ime prije ExecSql.
    /// Form1.vb 3976–3998 i Class1.vb 765–849, 932–964.
    /// </summary>
    public static class Dozvola
    {
        public static Dictionary<string, string> IzReda(DataRow red)
        {
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (red == null || red.Table == null)
                return cb;
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                object v = red[i];
                string t = v == null || v == DBNull.Value ? "null" : Convert.ToString(v);
                t = (t ?? "").Trim();
                if (t.StartsWith("'", StringComparison.Ordinal) && t.Length > 0)
                    t = t.Substring(1);
                if (t.EndsWith("'", StringComparison.Ordinal) && t.Length > 0)
                    t = t.Substring(0, t.Length - 1);
                cb[red.Table.Columns[i].ColumnName] = t.Trim();
            }
            return cb;
        }

        public static string Poslije(string sql, IDictionary<string, string> cb)
        {
            if (sql == null)
                sql = "";
            int zastita = 0;
            while (sql.IndexOf("@@", StringComparison.Ordinal) >= 0 && zastita++ < 40)
            {
                int poc = sql.IndexOf("@@", StringComparison.Ordinal);
                string sql2 = sql.Substring(poc) + " ";
                int kraj = sql2.IndexOf(' ');
                char[] rub = { '\'', '%', '#', ',', ')', '*', '/', '\\', '+', '-', '.', '(', '=' };
                for (int i = 0; i < rub.Length; i++)
                {
                    int p = sql2.IndexOf(rub[i]);
                    if (p > -1 && p < kraj)
                        kraj = p;
                }
                if (kraj <= 0)
                    break;
                string token = sql2.Substring(0, kraj);
                string vrijednost = IzCb(token, cb);
                sql = sql.Substring(0, poc) + vrijednost + sql.Substring(poc + token.Length);
            }
            return sql;
        }

        private static string IzCb(string token, IDictionary<string, string> cb)
        {
            string param = (token ?? "").Trim().Replace("@@", "@");
            string ime = param.StartsWith("@", StringComparison.Ordinal) ? param.Substring(1) : param;
            string v = null;
            if (cb != null)
                cb.TryGetValue(ime, out v);
            if (string.IsNullOrWhiteSpace(v))
                return "null";
            double broj;
            string norm = v.Replace(",", ".");
            if (double.TryParse(norm, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out broj))
                return norm;
            return v;
        }
    }

    /// <summary>
    /// IMEFORME(@a=@@x) nije ime forme. Form2.vb 4407–4418.
    /// Lijevo od prve zagrade je ime. Unutrašnjost ide u parametre. -freezid zabrani novi id.
    /// </summary>
    public static class PozivForme
    {
        public sealed class Poziv
        {
            public string Ime = "";
            public string Parametri = "";
            public bool FreezId;
        }

        public static Poziv Rastavi(string token)
        {
            var p = new Poziv();
            if (string.IsNullOrWhiteSpace(token))
                return p;
            string t = token.Trim();
            if (t.StartsWith(":", StringComparison.Ordinal))
                t = t.Substring(1).Trim();
            int z = t.IndexOf('(');
            if (z <= 0)
            {
                p.Ime = t;
                return p;
            }
            p.Ime = t.Substring(0, z).Trim();
            int duljina = t.Length - z - 2;
            string unutra = duljina > 0 ? t.Substring(z + 1, duljina) : "";
            if (unutra.IndexOf("-freezid", StringComparison.Ordinal) >= 0)
            {
                p.FreezId = true;
                unutra = unutra.Replace("-freezid", "");
            }
            p.Parametri = unutra;
            return p;
        }
    }
}
