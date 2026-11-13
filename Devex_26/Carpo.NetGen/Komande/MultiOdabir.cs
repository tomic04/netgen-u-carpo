using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// MULTIBLUE i MULTISELECT prije forme pune TotalGenMultiSelectTemp.
    /// Form1.vb 4932–4935 briše po operateru, 5022–5032 ubacuje GetID svakog plavog reda.
    /// </summary>
    public static class MultiOdabir
    {
        public static bool Je(string ime)
        {
            string t = (ime ?? "").Trim().ToLowerInvariant();
            return t.StartsWith("multiblue", StringComparison.Ordinal)
                || t.StartsWith("multiselect", StringComparison.Ordinal);
        }

        public static bool Plavi(string ime)
        {
            return (ime ?? "").Trim().ToLowerInvariant().StartsWith("multiblue", StringComparison.Ordinal);
        }

        public static string Obrisi(string operaterId)
        {
            string op = Broj(operaterId);
            return "if object_id('TotalGenMultiSelectTemp') is not null delete TotalGenMultiSelectTemp where operateriid=" + op;
        }

        public static string Ubaci(string id, string operaterId)
        {
            return "insert into TotalGenMultiSelectTemp (id, hostname, susersname, operateriid) select "
                + Broj(id) + ", host_name(), suser_sname(), " + Broj(operaterId);
        }

        private static string Broj(string tekst)
        {
            string t = (tekst ?? "").Trim();
            int n;
            if (!int.TryParse(t, out n))
                return "0";
            return n.ToString();
        }
    }
}
