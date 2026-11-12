using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// LVO-20: onlytree ne šalje master (Form1.vb 5475). Ime prije zagrade je forma (Form2.vb 4407).
    /// </summary>
    internal static class LVObracun
    {
        public const string Ime = "LVObracun";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "LVObracun.xml");
            const string opis = "onlytree ne salje master. Ime(@a=) nije ime forme";
            const string izvor = "netgen/Form1.vb:5475; netgen/Form2.vb:4407";
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "LVO-20", opis, izvor, "nema XML"));
                return l;
            }
            if (GranulaSesija.Izvrsi == null)
            {
                l.Add(Qa.P(Ime, "LVO-20", opis, izvor, "kuka nije spojena"));
                return l;
            }

            Pogled unos = GranulaSesija.Izvrsi(new Zahtjev { XmlPutanja = xml, Akcija = "gumb", Cilj = "Unos", MasterId = "0", Vrijednosti = "" });
            Pogled drvo = GranulaSesija.Izvrsi(new Zahtjev { XmlPutanja = xml, Akcija = "gumb", Cilj = "Osvježi drvo", MasterId = "0", Vrijednosti = "" });
            string forma = Kljuc(unos, "formaIme");
            string param = Kljuc(unos, "formaParam");
            string only = Kljuc(drvo, "onlytree");
            string master = Kljuc(drvo, "masterSql");
            string stablo = Kljuc(drvo, "stabloSql");
            bool ime = forma == "INSERTLVGLA" && forma.IndexOf('(') < 0;
            bool parametri = param == "@hrkadroviid = 0";
            bool bezMastera = only == "bez-mastera" && master.Length == 0 && stablo.IndexOf("spLVGla_Query", StringComparison.OrdinalIgnoreCase) < 0;
            bool stabloIde = stablo.IndexOf("spLVGla_Tree", StringComparison.OrdinalIgnoreCase) >= 0;
            if (ime && parametri && bezMastera && stabloIde)
            {
                Console.WriteLine("[Z] LVObracun LVO-20 forma=INSERTLVGLA param=@hrkadroviid = 0 onlytree=bez-mastera stablo=spLVGla_Tree");
                l.Add(Qa.Z(Ime, "LVO-20", opis, izvor));
            }
            else
            {
                l.Add(Qa.P(Ime, "LVO-20", opis, izvor,
                    "forma=[" + forma + "] param=[" + param + "] onlytree=[" + only + "] master=[" + master + "] stablo=[" + stablo + "] unos=" + (unos == null ? "null" : unos.Napomena) + " drvo=" + (drvo == null ? "null" : drvo.Napomena)));
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
