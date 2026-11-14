using System;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// strParametriSvi. Form2.vb 3432 radio, 3616 search, 3691–3818 grid.
    /// </summary>
    public static class ParametriSvi
    {
        /// <summary>
        /// Form2.vb 3667-3678 (treeViewMoj): oznaka odabranog čvora pod navodnicima, ' → '' i , → ~~; bez
        /// odabranog čvora null (TRE-03).
        /// </summary>
        public static string Stablo(string ime, string oznakaCvora)
        {
            string n = (ime ?? "").Trim();
            if (oznakaCvora == null)
                return "@" + n + "=null,";
            return "@" + n + "='" + oznakaCvora.Replace("'", "''").Replace(",", "~~") + "',";
        }

        public static string Radio(string ime, bool ukljucen)
        {
            return "@" + (ime ?? "").Trim() + "=" + (ukljucen ? "1" : "0") + ",";
        }

        /// <summary>Form2.vb 3627–3632: search šalje Tag, prazan ili 0 je null. Natpis nije id.</summary>
        public static string SearchId(object tag)
        {
            string id = (tag as string ?? "").Trim();
            if (id.Length == 0 || id == "0")
                return "null";
            return id;
        }

        /// <summary>
        /// Form2.vb 3624-3633: u strParametriSvi (pa i u ONSUBMIT) ide samo @ime=id ili @ime=null.
        /// @searchboxIME je SearchBoxCaptioni i ide samo u CB (3936), vidi <see cref="SearchNatpis"/> (PAR-02).
        /// </summary>
        public static string Search(string ime, string oznaka, string tekst)
        {
            string n = (ime ?? "").Trim();
            string id = (oznaka ?? "").Trim();
            if (id.Length == 0 || id == "0")
                return "@" + n + "=null,";
            return "@" + n + "=" + id + ",";
        }

        /// <summary>Form2.vb 3630/3635: SearchBoxCaptioni, tekst tražilice; zarez postane razmak.</summary>
        public static string SearchNatpis(string ime, string tekst)
        {
            return "@searchbox" + (ime ?? "").Trim() + "='" + (tekst ?? "").Replace(",", " ") + "',";
        }

        public static string Grid(string ime, string id, string plavi, string cekirani, int redova)
        {
            string n = (ime ?? "").Trim();
            string p = Val(plavi);
            string c = Val(cekirani);
            string tid = string.IsNullOrEmpty(id) ? "0" : id;
            return "@" + n + "=" + tid + ","
                + "@arrayofblueids" + n + "='" + p + "',"
                + "@nizplavihajdijeva" + n + "='" + p + "',"
                + "@nizoznacenihajdijeva" + n + "='" + c + "',"
                + "@rowcount" + n + "=" + redova.ToString() + ",";
        }

        /// <summary>
        /// Form2.PokupiParametreIzTrazilice 5926–5953 i Form3.Trazilica 226-246 (jedan red). Petlja ide od
        /// Janus stupca 1, a to je kvacica bez natpisa; stupci podatka pocinju na Janus 2, pa je i prvi
        /// stupac DataTable-a (id) parametar: @SEARCHid (TRZ-02). Ime dobije SEARCH. Zadnje 4 znamenke koje nisu 0 otpadaju.
        /// </summary>
        public static string IzTrazilice(System.Data.DataRow red)
        {
            if (red == null)
                return "";
            var sb = new StringBuilder();
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                System.Data.DataColumn c = red.Table.Columns[i];
                string natpis = string.IsNullOrEmpty(c.Caption) ? c.ColumnName : c.Caption;
                if (natpis.Length == 0)
                    continue;
                if (red[c] == null || red[c] == System.DBNull.Value)
                    continue;
                string ime = (c.ColumnName ?? "").Trim();
                if (ime.Length >= 4)
                {
                    int n;
                    if (int.TryParse(ime.Substring(ime.Length - 4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n) && n != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                ime = ime.Replace(" ", "_");
                string v = System.Convert.ToString(red[c]) ?? "";
                string broj = v.Replace(",", ".");
                double d;
                if (double.TryParse(broj, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                    v = broj;
                sb.Append("@SEARCH").Append(ime).Append("='").Append(v).Append("' , ");
            }
            return sb.ToString();
        }

        /// <summary>Tekst. Apostrof je ¸¸, zarez je ~~. Form2.vb 3656.</summary>
        public static string Tekst(string ime, string tekst)
        {
            string t = (tekst ?? "").Replace("'", "¸¸").Replace(",", "~~");
            return "@" + (ime ?? "").Trim() + "='" + t + "',";
        }

        /// <summary>Prazan broj je 0, zarez je točka. Form2.vb 3610.</summary>
        public static string Broj(string ime, string tekst)
        {
            string t = (tekst ?? "").Trim().Replace(",", ".");
            if (t.Length == 0)
                t = "0";
            return "@" + (ime ?? "").Trim() + "=" + t + ",";
        }

        /// <summary>Kvačica ugašena šalje null. Inače yyyyMMdd, HH:mm, yyyyMM01, yyyyMMdd HH:mm. Form2.vb 3530–3567.</summary>
        public static string Datum(string ime, string format, bool kvacica, System.DateTime? vrijednost)
        {
            string n = (ime ?? "").Trim();
            if (!kvacica || !vrijednost.HasValue)
                return "@" + n + "=null,";
            string f = (format ?? "").ToUpperInvariant();
            string t;
            if (f == "HH:MM")
                t = vrijednost.Value.ToString("HH:mm");
            else if (f == "MM.YYYY")
                t = vrijednost.Value.ToString("yyyyMM") + "01";
            else if (f == "DD.MM.YYYY HH:MM")
                t = vrijednost.Value.ToString("yyyyMMdd HH:mm");
            else
                t = vrijednost.Value.ToString("yyyyMMdd");
            return "@" + n + "='" + t + "',";
        }

        /// <summary>Multidrop je id1~id2 bez razmaka. Form2.vb 3446.</summary>
        public static string Multi(string ime, string zarez)
        {
            string t = (zarez ?? "").Replace(",", "~").Replace(" ", "");
            return "@" + (ime ?? "").Trim() + "='" + t + "',";
        }

        private static string Val(string niz)
        {
            string s = niz ?? "";
            if (s.Length == 0)
                return "";
            if (!s.StartsWith("~", StringComparison.Ordinal))
                s = "~" + s;
            if (!s.EndsWith("~", StringComparison.Ordinal))
                s = s + "~";
            return s;
        }
    }
}
