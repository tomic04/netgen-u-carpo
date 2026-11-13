using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>MSGBOX. Form2.vb 7165–7169. Tekst ide velikim slovima, Information.</summary>
    public static class PorukaNaredba
    {
        public static Action<string> Prikazi;

        public static string Tekst(string naredba)
        {
            string u = (naredba ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf("MSGBOX(", StringComparison.Ordinal);
            if (a < 0)
                return "";
            string rest = u.Substring(a + "MSGBOX(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            return rest.Trim();
        }

        public static void Pokazi(string naredba)
        {
            string t = Tekst(naredba);
            if (Prikazi != null)
                Prikazi(t);
        }
    }
}
