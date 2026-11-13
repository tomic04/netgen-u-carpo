using System;
using System.Collections.Generic;
using System.Reflection;

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
        public static readonly List<NgZapis> Zapisi = new List<NgZapis>();

        /// <summary>Instanca <c>Carpo.Core.Logger</c>. Postavlja je host (TestShell ili forma).</summary>
        public static object CarpoLogger;

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

        private static void Pisi(string metoda, string opis, string tekst)
        {
            Zapisi.Add(new NgZapis { Razina = metoda, Opis = opis, Tekst = tekst });
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
