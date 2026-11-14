using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Carpo.NetGen.Log
{
    public sealed class NgZapis
    {
        public string Razina;
        public string Opis;
        public string Tekst;
    }

    /// <summary>
    /// Jedini log NetGen elementa. Isti tekst koji je NetGen stavljao u CBLog
    /// ide u Carpo <c>Logger</c> (LogList, Ctrl+F11). Nema drugog preglednika.
    /// </summary>
    public static class NgDnevnik
    {
        /// <summary>
        /// Zadnji zapisi (NetGen CBLog drži 40). Granica je tu da duga sesija u Jupiteru ne raste bez kraja;
        /// cijeli log je u Carpo Loggeru.
        /// </summary>
        public const int NajviseZapisa = 1000;

        public static readonly List<NgZapis> Zapisi = new List<NgZapis>();

        /// <summary>Instanca <c>Carpo.Core.Logger</c>. Postavlja je host (TestShell ili forma).</summary>
        public static object CarpoLogger;

        private static readonly object Brava = new object();
        private static readonly List<string> TajneVrijednosti = new List<string>();
        private static readonly List<string> TajnaImena = new List<string>();
        private static Regex _imena;

        // JupiterLog.Sredi 122-141: pwd= i password= (spojni niz) do ; ili " (VB), ovdje i navodnik.
        private static readonly Regex Spojni = new Regex(@"((?:pwd|password)\s*=\s*N?)(?:'(?:[^']|'')*'|[^;""']+)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static void Spoji(object carpoLogger)
        {
            CarpoLogger = carpoLogger;
        }

        public static void Sql(string tekst)
        {
            Pisi("InfoData", "NetGen SQL", tekst ?? "");
        }

        public static void Preskok(string tekst)
        {
            Pisi("Info", "NetGen preskok", " SKIPPED :   " + (tekst ?? ""));
        }

        public static void Akcija(string tekst)
        {
            Pisi("Info", "NetGen", tekst ?? "");
        }

        public static void Greska(string tekst)
        {
            Pisi("Error", "NetGen SQL", tekst ?? "");
        }

        /// <summary>
        /// Polje s PASSWORDCHAR (MPKase: ServerPass, PassAdmin). NetGen CBLog je lozinku pisao u SQL logu
        /// (plan, faza G); Carpo umjesto vrijednosti parametra <c>@ime='…'</c> piše <c>***</c>.
        /// </summary>
        public static void TajnoIme(string ime)
        {
            string n = (ime ?? "").Trim();
            if (n.Length == 0)
                return;
            lock (Brava)
            {
                foreach (string postoji in TajnaImena)
                    if (string.Equals(postoji, n, StringComparison.OrdinalIgnoreCase))
                        return;
                TajnaImena.Add(n);
                var dijelovi = new List<string>();
                foreach (string t in TajnaImena)
                    dijelovi.Add(Regex.Escape(t));
                _imena = new Regex(@"(@(?:" + string.Join("|", dijelovi) + @")\s*=\s*N?)'(?:[^']|'')*'",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
        }

        /// <summary>
        /// Upisana lozinka: skriva se i kad ide bez imena (exec sp 'server','lozinka'). Kratka vrijednost
        /// (manje od 3 znaka) skriva se samo po imenu, inače bi log izgubio sve jednoznamenkaste brojeve.
        /// Pamte se i oblici iz SQL-a: '' za navodnik, te ¸¸ i ~~ iz PokupiParametre.
        /// </summary>
        public static void Tajna(string vrijednost)
        {
            if (string.IsNullOrEmpty(vrijednost) || vrijednost.Length < 3)
                return;
            lock (Brava)
            {
                foreach (string oblik in new[] { vrijednost, vrijednost.Replace("'", "''"), vrijednost.Replace("'", "¸¸").Replace(",", "~~") })
                    if (!TajneVrijednosti.Contains(oblik))
                        TajneVrijednosti.Add(oblik);
                TajneVrijednosti.Sort((a, b) => b.Length.CompareTo(a.Length));
            }
        }

        /// <summary>Tekst kakav ide u log: lozinke (polja s PASSWORDCHAR, pwd=, password=) su <c>***</c>.</summary>
        public static string Sakrij(string tekst)
        {
            if (string.IsNullOrEmpty(tekst))
                return tekst ?? "";
            tekst = Spojni.Replace(tekst, "$1***");
            lock (Brava)
            {
                if (_imena != null)
                    tekst = _imena.Replace(tekst, "$1'***'");
                foreach (string t in TajneVrijednosti)
                    if (tekst.IndexOf(t, StringComparison.Ordinal) >= 0)
                        tekst = tekst.Replace(t, "***");
            }
            return tekst;
        }

        private static void Pisi(string metoda, string opis, string tekst)
        {
            tekst = Sakrij(tekst);
            lock (Zapisi)
            {
                Zapisi.Add(new NgZapis { Razina = metoda, Opis = opis, Tekst = tekst });
                if (Zapisi.Count > NajviseZapisa)
                    Zapisi.RemoveRange(0, Zapisi.Count - NajviseZapisa);
            }
            object log = CarpoLogger;
            if (log == null)
                return;
            try
            {
                MethodInfo m = log.GetType().GetMethod(
                    metoda,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string), typeof(string), typeof(object[]) },
                    null);
                if (m == null)
                    return;
                m.Invoke(log, new object[] { opis, tekst, new object[0] });
            }
            catch
            {
            }
        }
    }
}
