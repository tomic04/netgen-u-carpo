using System;

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Ispis NetGen liste i repx-a. Pisač se ne zove. Verzija je v26, fallback rptDevRepx.
    /// </summary>
    public static class IspisMotora
    {
        public static bool JeMaster(string naredba)
        {
            string u = (naredba ?? "").Trim();
            return u.Equals("PRINTMASTER", StringComparison.OrdinalIgnoreCase)
                || u.Equals("PRINTREPXMASTER", StringComparison.OrdinalIgnoreCase)
                || u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase)
                || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase);
        }

        public static bool GledaOrijentaciju()
        {
            return false;
        }

        public static string Prefiks(bool dijalog, string naredba)
        {
            string s = naredba ?? "";
            if (dijalog)
                return s.Replace("REPX/", "repx:/").Replace("repx/", "repx:/");
            return s.Replace("REPX/", "repx:").Replace("repx/", "repx:");
        }

        public static string Verzija()
        {
            return "v26";
        }

        public static string Fallback()
        {
            return "rptDevRepx";
        }

        public static bool JeVrstaDok(string ime)
        {
            string s = (ime ?? "").Trim();
            if (s.StartsWith(":", StringComparison.Ordinal))
                s = s.Substring(1);
            return s.StartsWith("vrstadokid-", StringComparison.OrdinalIgnoreCase);
        }

        public static bool BriseTemp(string imeForme)
        {
            string s = (imeForme ?? "").Trim();
            return s.StartsWith("MULTISELECT", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("MULTIBLUE", StringComparison.OrdinalIgnoreCase);
        }
    }
}
