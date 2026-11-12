using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Carpo.NetGen.Model;
using Carpo.NetGen.TestHooks;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    public enum Ishod
    {
        Zeleno,
        Otvoreno,
        Pad
    }

    public sealed class Stavka
    {
        public string Granula;
        public string Id;
        public Ishod Ishod;
        public string Opis;
        public string Izvor;
        public string Detalj;
    }

    public static class Qa
    {
        public const string GranulaNijeIzvedena =
            "Carpo.NetGen.TestHooks.GranulaSesija.Izvrsi je null (Placeholders ne izvode granulu). Stavka je poznato-otvorena.";

        public static int VbCijeli(double d)
        {
            return (int)Math.Round(d, MidpointRounding.ToEven);
        }

        public static int Px(string jedinica)
        {
            return VbCijeli(double.Parse(jedinica, CultureInfo.InvariantCulture) / 14.0);
        }

        public static Stavka Z(string granula, string id, string opis, string izvor)
        {
            return new Stavka { Granula = granula, Id = id, Ishod = Ishod.Zeleno, Opis = opis, Izvor = izvor };
        }

        public static Stavka O(string granula, string id, string opis, string izvor, string razlog)
        {
            return new Stavka { Granula = granula, Id = id, Ishod = Ishod.Otvoreno, Opis = opis, Izvor = izvor, Detalj = razlog };
        }

        public static Stavka P(string granula, string id, string opis, string izvor, string razlog)
        {
            return new Stavka { Granula = granula, Id = id, Ishod = Ishod.Pad, Opis = opis, Izvor = izvor, Detalj = razlog };
        }

        public static Stavka Jednako(string granula, string id, string opis, string izvor, string ocekivano, string dobiveno)
        {
            if (string.Equals(ocekivano ?? "", dobiveno ?? "", StringComparison.Ordinal))
                return Z(granula, id, opis, izvor);
            return P(granula, id, opis, izvor, "ocekivano [" + (ocekivano ?? "") + "] dobiveno [" + (dobiveno ?? "") + "]");
        }

        public static NgProject Ucitaj(string putanja, out string greska)
        {
            greska = null;
            var r = NetGenXmlReader.Ucitaj(putanja);
            if (!r.Uspjeh || r.Projekt == null)
            {
                greska = r.Greska;
                return null;
            }
            return r.Projekt;
        }

        public static Stavka Runtime(string granula, string id, string opis, string izvor, string xml, string akcija, string cilj, string kljuc, string ocekivano)
        {
            if (GranulaSesija.Izvrsi == null)
                return O(granula, id, opis, izvor, GranulaNijeIzvedena);

            Pogled pogled;
            try
            {
                pogled = GranulaSesija.Izvrsi(new Zahtjev
                {
                    XmlPutanja = xml,
                    Akcija = akcija,
                    Cilj = cilj,
                    MasterId = "0",
                    Vrijednosti = ""
                });
            }
            catch (Exception ex)
            {
                return P(granula, id, opis, izvor, "Izvrsi je bacio: " + ex.GetType().Name + ": " + ex.Message);
            }

            if (pogled == null || !pogled.Izvedeno)
            {
                string razlog = pogled == null ? "Izvrsi je vratio null" : (pogled.Napomena ?? "Izvedeno=false");
                return O(granula, id, opis, izvor, razlog);
            }

            if (pogled.Stavke == null || !pogled.Stavke.ContainsKey(kljuc))
                return P(granula, id, opis, izvor, "Izvedeno=true ali nema kljuca " + kljuc);

            return Jednako(granula, id, opis, izvor, ocekivano, pogled.Stavke[kljuc]);
        }

        public static string Tekst(IList<Stavka> sve)
        {
            var sb = new StringBuilder();
            sb.AppendLine("QA granule NetGen u Carpu");
            sb.AppendLine("Zeleno = Carpo.NetGen odgovara NetGen kodu. Otvoreno = jos nije izvedeno. Pad = izvedeno krivo.");
            sb.AppendLine();
            foreach (var g in sve.Select(s => s.Granula).Distinct())
            {
                var dio = sve.Where(s => s.Granula == g).ToList();
                int z = dio.Count(s => s.Ishod == Ishod.Zeleno);
                int o = dio.Count(s => s.Ishod == Ishod.Otvoreno);
                int p = dio.Count(s => s.Ishod == Ishod.Pad);
                sb.AppendLine(g + "  ukupno=" + dio.Count + "  zeleno=" + z + "  otvoreno=" + o + "  pad=" + p);
            }
            int zz = sve.Count(s => s.Ishod == Ishod.Zeleno);
            int oo = sve.Count(s => s.Ishod == Ishod.Otvoreno);
            int pp = sve.Count(s => s.Ishod == Ishod.Pad);
            sb.AppendLine("SVE  ukupno=" + sve.Count + "  zeleno=" + zz + "  otvoreno=" + oo + "  pad=" + pp);
            sb.AppendLine();
            foreach (var s in sve)
            {
                string ozn = s.Ishod == Ishod.Zeleno ? "Z" : s.Ishod == Ishod.Otvoreno ? "O" : "P";
                sb.AppendLine("[" + ozn + "] " + s.Granula + " " + s.Id + "  " + s.Opis);
                sb.AppendLine("    " + s.Izvor);
                if (!string.IsNullOrEmpty(s.Detalj))
                    sb.AppendLine("    " + s.Detalj);
            }
            return sb.ToString();
        }
    }
}
