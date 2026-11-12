using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Komande;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Dokaz naredbi s PR #79 na stvarnom XML-u. Kod glavnog agenta se ne mijenja.
    /// </summary>
    internal static class NaredbeDokaz
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Click(root));
            return l;
        }

        private static Stavka Click(string root)
        {
            const string granula = "KntBlaPromet";
            const string id = "NRD-CLICK";
            const string opis = "click(Ok) na ISPISDOK_FORM daje kontrolu OK";
            const string izvor = "netgen/Form2.vb:7151-7154";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntBlaPromet.xml");
            string tekst = Procitaj(xml);
            if (tekst == null || tekst.IndexOf("ONLOADED=\"click(Ok)\"", StringComparison.Ordinal) < 0)
                return Qa.P(granula, id, opis, izvor, "XML nema click(Ok)");
            string kontrola = KlikNaredba.Kontrola("click(Ok)");
            if (kontrola != "OK")
                return Qa.P(granula, id, opis, izvor, "kontrola [" + kontrola + "]");
            Console.WriteLine("[Z] KntBlaPromet NRD-CLICK click(Ok)=OK");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static string Procitaj(string put)
        {
            if (!File.Exists(put))
                return null;
            byte[] raw = File.ReadAllBytes(put);
            if (raw.Length >= 2 && raw[0] == 0xFF && raw[1] == 0xFE)
                return System.Text.Encoding.Unicode.GetString(raw);
            return System.Text.Encoding.GetEncoding(1250).GetString(raw);
        }
    }
}
