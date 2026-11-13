using System;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// strParametriSvi. Form2.vb 3432 radio, 3616 search, 3691–3818 grid.
    /// </summary>
    public static class ParametriSvi
    {
        public static string Radio(string ime, bool ukljucen)
        {
            return "@" + (ime ?? "").Trim() + "=" + (ukljucen ? "1" : "0") + ",";
        }

        public static string Search(string ime, string oznaka, string tekst)
        {
            string n = (ime ?? "").Trim();
            string id = (oznaka ?? "").Trim();
            string glavni;
            if (id.Length == 0 || id == "0")
                glavni = "@" + n + "=null,";
            else
                glavni = "@" + n + "=" + id + ",";
            string natpis = (tekst ?? "").Replace(",", " ");
            return glavni + "@searchbox" + n + "='" + natpis + "',";
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
