using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// ISEMPTY u lancu. Form1.vb 6768–6776.
    /// null ili prazno preskoči sljedeći segment. Inače ga izvrši.
    /// </summary>
    public static class PraznoNaredba
    {
        public static bool Je(string segment)
        {
            return (segment ?? "").ToUpperInvariant().IndexOf("ISEMPTY(", StringComparison.Ordinal) >= 0;
        }

        public static bool PreskociSljedeci(string segment)
        {
            if (!Je(segment))
                return false;
            string u = segment.Trim().ToUpperInvariant();
            int a = u.IndexOf("ISEMPTY(", StringComparison.Ordinal);
            string rest = u.Substring(a + "ISEMPTY(".Length);
            int z = rest.LastIndexOf(')');
            if (z >= 0)
                rest = rest.Substring(0, z);
            rest = rest.Trim();
            return rest == "NULL" || rest.Length == 0;
        }
    }
}
