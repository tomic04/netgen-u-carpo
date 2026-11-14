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

        /// <summary>Form1.vb 4931: prvo brisanje po SQL korisniku (suser_sname), pa po operateru (4932) (MULTI-01).</summary>
        public static string ObrisiKorisnika()
        {
            return "if object_id('TotalGenMultiSelectTemp') is not null delete TotalGenMultiSelectTemp where susersname= suser_sname()";
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

    /// <summary>
    /// Form2.vb 3367 šalje ONCLICK gumba koji je kliknut.
    /// Gumb :WEBBROWSER nema natpis Spremi i ne smije pojesti potvrdu.
    /// </summary>
    public static class PotvrdaGumba
    {
        public static bool JeNatpis(string tekst)
        {
            string n = (tekst ?? "").Trim().ToLowerInvariant();
            return n.StartsWith("spremi") || n.StartsWith("prihvati") || n.StartsWith("obri")
                || n.StartsWith("dodaj") || n.StartsWith("ukloni") || n.StartsWith("kopiraj")
                || n.StartsWith("izm");
        }

        public static int Indeks(string[] natpisi, string[] tagovi)
        {
            if (natpisi == null)
                return -1;
            for (int i = 0; i < natpisi.Length; i++)
            {
                if (!JeNatpis(natpisi[i]))
                    continue;
                string tag = tagovi != null && i < tagovi.Length ? (tagovi[i] ?? "") : "";
                if (tag.Length == 0 || tag.StartsWith(":", StringComparison.Ordinal))
                    continue;
                return i;
            }
            return -1;
        }
    }
}
