using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// FISKALIZACIJA* na gumbu. Form1.vb 4643–4781.
    /// 0 ili 1 označen red: jedan poziv. Inače po jedan na svaki označeni red.
    /// </summary>
    public static class FiskalNaredba
    {
        public static int Tablica(string naredba)
        {
            string u = (naredba ?? "").ToUpperInvariant();
            if (u.IndexOf("FISKALIZACIJAN(", StringComparison.Ordinal) >= 0)
                return -1;
            if (u.IndexOf("FISKALIZACIJAFAKTURE(", StringComparison.Ordinal) >= 0)
                return 2;
            if (u.IndexOf("FISKALIZACIJABSRACUNI(", StringComparison.Ordinal) >= 0)
                return 3;
            if (u.IndexOf("FISKALIZACIJAPD(", StringComparison.Ordinal) >= 0)
                return 4;
            if (u.IndexOf("FISKALIZACIJAPONUDE(", StringComparison.Ordinal) >= 0)
                return 5;
            if (u.IndexOf("FISKALIZACIJA(", StringComparison.Ordinal) >= 0)
                return 1;
            return 0;
        }

        public static string Argument(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.LastIndexOf('(');
            int z = u.LastIndexOf(')');
            if (a < 0 || z <= a)
                return "";
            return u.Substring(a + 1, z - a - 1).Trim();
        }

        public static int Poziva(int cekiranih)
        {
            if (cekiranih <= 1)
                return 1;
            return cekiranih;
        }
    }
}
