using System;

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Module1.Translate 266–284. Prazan jezik i hrv ostaju izvorni tekst.
    /// Inače se zove vanjski prevoditelj. Ako ga nema ili baci, On Error Resume Next
    /// ostavlja već dodijeljeni izvorni tekst.
    /// </summary>
    public static class Prevod
    {
        public static Func<string, string> Vanjski;

        public static string Tekst(string izvor, string jezik)
        {
            string t = izvor ?? "";
            string j = (jezik ?? "").Trim().ToLowerInvariant();
            if (j.Length == 0 || j == "hrv")
                return t;
            if (Vanjski == null)
                return t;
            try
            {
                string r = Vanjski(t);
                if (string.IsNullOrEmpty(r))
                    return t;
                return r;
            }
            catch
            {
                return t;
            }
        }
    }
}
