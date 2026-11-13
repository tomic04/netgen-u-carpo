using System;
using System.Collections.Generic;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// FOREACH na dijalogu vrti sljedeći segment po retku. Form2.vb 7040–7048, 8310–8394.
    /// Na podlozi petlja ne postoji, ali i = i + 1 pojede sljedeći segment. Form1.vb 6843–6850.
    /// </summary>
    public static class ForeachNaredba
    {
        public static string Vrsta(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            if (u.IndexOf("FOREACHSELECTED(", StringComparison.Ordinal) >= 0)
                return "cekirani";
            if (u.IndexOf("FOREACHBLUEROW(", StringComparison.Ordinal) >= 0)
                return "plavi";
            if (u.IndexOf("FOREACHROW(", StringComparison.Ordinal) >= 0)
                return "svi";
            return "";
        }

        public static string Grid(string segment)
        {
            string u = (segment ?? "").Trim().ToUpperInvariant();
            int a = u.IndexOf('(');
            int z = u.LastIndexOf(')');
            if (a < 0 || z <= a)
                return "";
            return u.Substring(a + 1, z - a - 1).Trim();
        }

        public static string[] Idjevi(string vrsta, IList<string> svi, IList<string> plavi, IList<string> cekirani)
        {
            IList<string> izvor = null;
            if (vrsta == "svi")
                izvor = svi;
            else if (vrsta == "plavi")
                izvor = plavi;
            else if (vrsta == "cekirani")
                izvor = cekirani;
            if (izvor == null || izvor.Count == 0)
                return new string[0];
            var o = new string[izvor.Count];
            for (int i = 0; i < izvor.Count; i++)
                o[i] = izvor[i] ?? "";
            return o;
        }

        /// <summary>Jedan Rafalno po retku. @@id je trenutni red, kao MoveToRowIndex pa sljedeći segment.</summary>
        public static string[] Ponovi(string tijelo, IList<string> idjevi)
        {
            var o = new List<string>();
            if (string.IsNullOrEmpty(tijelo) || idjevi == null)
                return o.ToArray();
            for (int i = 0; i < idjevi.Count; i++)
                o.Add(Zamijeni(tijelo, idjevi[i] ?? ""));
            return o.ToArray();
        }

        private static string Zamijeni(string tijelo, string id)
        {
            string s = tijelo;
            int p = 0;
            while (p < s.Length)
            {
                int n = s.IndexOf("@@id", p, StringComparison.OrdinalIgnoreCase);
                if (n < 0)
                    break;
                s = s.Substring(0, n) + id + s.Substring(n + 4);
                p = n + id.Length;
            }
            return s;
        }
    }
}
