using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// KBP-20: CLICK(Ok) izvrši ONCLICK gumba, close ne guta REPX. Form2.vb 7151–7155, 7033, 6882.
    /// </summary>
    internal static class KntBlaPromet
    {
        public const string Ime = "KntBlaPromet";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntBlaPromet.xml");
            const string opis = "CLICK(Ok) izvrsi ONCLICK, ne samo zatvaranje";
            const string izvor = "netgen/Form2.vb:7151-7155";
            const string ocek = "close|REPX/@@template (@id=@@BLPrometGlaId)";
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KBP-20", opis, izvor, "nema XML"));
                return l;
            }
            if (GranulaSesija.Izvrsi == null)
            {
                l.Add(Qa.P(Ime, "KBP-20", opis, izvor, "kuka nije spojena"));
                return l;
            }
            Pogled p = GranulaSesija.Izvrsi(new Zahtjev
            {
                XmlPutanja = xml,
                Akcija = "gumb",
                Cilj = "Ispiši",
                MasterId = "0",
                Vrijednosti = ""
            });
            string ime = Kljuc(p, "click.ime");
            string lanac = Kljuc(p, "click.lanac");
            bool ok = ime == "OK" && lanac == ocek && lanac != "close";
            if (ok)
            {
                Console.WriteLine("[Z] KntBlaPromet KBP-20 click=OK lanac=close|REPX/@@template (@id=@@BLPrometGlaId)");
                l.Add(Qa.Z(Ime, "KBP-20", opis, izvor));
            }
            else
            {
                l.Add(Qa.P(Ime, "KBP-20", opis, izvor,
                    "ime=[" + ime + "] lanac=[" + lanac + "] " + (p == null ? "null" : (p.Napomena ?? ""))));
            }
            return l;
        }

        private static string Kljuc(Pogled p, string kljuc)
        {
            if (p == null || !p.Izvedeno || p.Stavke == null || !p.Stavke.ContainsKey(kljuc))
                return "";
            return p.Stavke[kljuc] ?? "";
        }
    }
}
