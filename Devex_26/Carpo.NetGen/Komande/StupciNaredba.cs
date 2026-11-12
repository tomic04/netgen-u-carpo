using System;
using System.Collections.Generic;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// HIDECOLUMNS i SHOWCOLUMNS. Form1.vb 6730–6766.
    /// Prvi token je ime grida. Samo mastergrid mijenja stupce.
    /// Ista petlja sakrije i taj prvi token, jer je i on Ar88(i2).
    /// </summary>
    public static class StupciNaredba
    {
        public static Dictionary<string, bool> Primijeni(string naredba)
        {
            var d = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            bool pokazi = u.IndexOf("SHOWCOLUMNS(", StringComparison.Ordinal) >= 0;
            string oznaka = pokazi ? "SHOWCOLUMNS(" : "HIDECOLUMNS(";
            int a = u.IndexOf(oznaka, StringComparison.Ordinal);
            if (a < 0)
                return d;
            string rest = u.Substring(a + oznaka.Length).Replace(")", "");
            string[] ar = rest.Split(',');
            if (ar.Length == 0 || ar[0].Trim() != "MASTERGRID")
                return d;
            for (int i = 0; i < ar.Length; i++)
            {
                string ime = ar[i].Trim();
                if (ime.Length == 0)
                    continue;
                d[ime] = pokazi;
            }
            return d;
        }
    }
}
