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
