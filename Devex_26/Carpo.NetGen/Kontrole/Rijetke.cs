using System;

namespace Carpo.NetGen.Kontrole
{
    /// <summary>
    /// Rijetke kontrole: dijalog stablo, web, slika, NEXTFORM, clipboard, grid na formi.
    /// </summary>
    public static class Rijetke
    {
        public static string WebAdresa(string urlAtribut, string kolona)
        {
            if (!string.IsNullOrWhiteSpace(urlAtribut))
                return urlAtribut.Trim().ToUpperInvariant();
            string k = (kolona ?? "").Trim();
            if (k.Length == 0 || string.Equals(k, "refresh", StringComparison.OrdinalIgnoreCase))
                return "";
            return k;
        }

        public static string LokacijaSlike(string location)
        {
            return (location ?? "").ToUpperInvariant();
        }

        public static bool HttpLoad(string vrijednost)
        {
            return (vrijednost ?? "").StartsWith("http", StringComparison.OrdinalIgnoreCase);
        }

        public static string Stretch()
        {
            return "stretch";
        }

        public static string Sljedeca(string nextForm)
        {
            return (nextForm ?? "").Trim();
        }

        public static string DozvolaIme(string prvaForma, string sljedeca)
        {
            return prvaForma ?? "";
        }

        public static bool Clipboard(string imeForme, string dbClipboard)
        {
            string z = (dbClipboard ?? "").Trim();
            if (string.Equals(z, "false", StringComparison.OrdinalIgnoreCase))
                return false;
            if ((imeForme ?? "").ToLowerInvariant().IndexOf("filter", StringComparison.Ordinal) >= 0)
                return true;
            return string.Equals(z, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool ClipboardPadStvaraTablicu()
        {
            return false;
        }

        public static string[] SetDefaults(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return new string[0];
            return sql.Split('#');
        }

        public static int SkriveniSelector()
        {
            return 1;
        }

        public static bool PreskociPocetno(string skip)
        {
            return string.Equals((skip ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
