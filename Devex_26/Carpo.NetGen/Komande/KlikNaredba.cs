using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// CLICK. Form2.vb 7151–7155 zove kontrolu. Form1.vb 6852–6856 je komentirano.
    /// </summary>
    public static class KlikNaredba
    {
        public static bool NaPodlozi()
        {
            return false;
        }

        public static string Kontrola(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf("CLICK(", StringComparison.Ordinal);
            if (a < 0)
                return "";
            string rest = u.Substring(a + "CLICK(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            return rest.Trim();
        }
    }
}
