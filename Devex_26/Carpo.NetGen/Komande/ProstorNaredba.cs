using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>POSLOVNIPROSTOR. Form1.vb 4783–4801. Isti broj poziva kao fiskalizacija.</summary>
    public static class ProstorNaredba
    {
        public static bool Je(string naredba)
        {
            return (naredba ?? "").ToUpperInvariant().IndexOf("POSLOVNIPROSTOR(", StringComparison.Ordinal) >= 0;
        }

        public static string Argument(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf("POSLOVNIPROSTOR(", StringComparison.Ordinal);
            if (a < 0)
                return "";
            string rest = u.Substring(a + "POSLOVNIPROSTOR(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            return rest.Trim();
        }

        public static int Poziva(int cekiranih)
        {
            return FiskalNaredba.Poziva(cekiranih);
        }
    }
}
