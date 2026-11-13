using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// BLAGAJNAENABLE. Form1.vb 4635–4640, Class1.vb 1975–1986.
    /// Procedura koja ne vrati vrijednost gasi kasu (0), inače je pali (1).
    /// </summary>
    public static class BlagajnaNaredba
    {
        public static string Id(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf("BLAGAJNAENABLE(", StringComparison.Ordinal);
            if (a < 0)
                return "";
            string rest = u.Substring(a + "BLAGAJNAENABLE(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            return rest.Trim();
        }

        public static string Upit(string id)
        {
            return "spmpBlagajnaEnable @mpkaseid = " + (id ?? "");
        }

        public static string Update(string id, bool proceduraVratila)
        {
            return "update mpkase set indenable = " + (proceduraVratila ? "1" : "0") + " where mpkaseid = " + (id ?? "");
        }
    }
}
