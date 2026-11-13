using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>OPENFORM. Form1.vb 6582–6585. Ime forme je veliko.</summary>
    public static class OtvoriNaredba
    {
        public static string Ime(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf("OPENFORM(", StringComparison.Ordinal);
            if (a < 0)
                return "";
            string rest = u.Substring(a + "OPENFORM(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            return rest.Trim();
        }
    }
}
