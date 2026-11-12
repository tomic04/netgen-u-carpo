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

        /// <summary>Form1.vb 5215. .clsreport je ispis. .clsGFIPOD nije.</summary>
        public static bool JeIspisImena(string ime)
        {
            string u = (ime ?? "").Trim();
            if (u.Length < 3)
                return false;
            string g = u.ToUpperInvariant();
            if (g.StartsWith("VRSTADOKID-", StringComparison.Ordinal) || g.StartsWith("REPX:", StringComparison.Ordinal) || g.StartsWith("REPX/", StringComparison.Ordinal) || g.StartsWith("PR10/", StringComparison.Ordinal) || g.StartsWith("PR10:", StringComparison.Ordinal))
                return true;
            string tri = g.Substring(0, 3);
            if (tri == "RSGEN" || tri == "GG/" || tri == "RS/" || tri == "RS:")
                return true;
            return u.ToLowerInvariant().IndexOf(".clsreport", StringComparison.Ordinal) >= 0;
        }

        /// <summary>Form1.Rafalna_Priprema 7744–7768. Prazan ISEMPTY preskoči sadržaj.</summary>
        public static string Zaglavlje(string tekst)
        {
            string st = tekst ?? "";
            string ts = st.ToUpperInvariant().IndexOf("ISEMPTY(", StringComparison.Ordinal) >= 0 ? "" : st;
            if (st.Trim().Length == 0)
                return ts;
            string[] ar = st.Split('#');
            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].ToUpperInvariant().IndexOf("ISEMPTY(", StringComparison.Ordinal) < 0)
                    continue;
                string param = ar[i].ToUpperInvariant();
                int a = param.IndexOf("ISEMPTY(", StringComparison.Ordinal);
                param = param.Substring(a + "ISEMPTY(".Length);
                int z = param.LastIndexOf(')');
                if (z >= 0)
                    param = param.Substring(0, z);
                if (param.ToLowerInvariant() == "null" || param.Trim().Length == 0)
                {
                    i = i + 1;
                    if (i < ar.Length)
                        ts += ar[i].Trim();
                    i = i + 1;
                }
                else
                {
                    i = i + 2;
                    if (i < ar.Length)
                        ts += ar[i].Trim().Replace("**", " ");
                }
            }
            return ts;
        }

        public static bool BriseTemp(string imeForme)
        {
            string s = (imeForme ?? "").Trim();
            return s.StartsWith("MULTISELECT", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("MULTIBLUE", StringComparison.OrdinalIgnoreCase);
        }
    }
}
