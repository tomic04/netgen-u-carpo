using System;
using System.Collections.Generic;
using System.IO;

namespace Carpo.NetGen.Granule
{
    public static class KntDobDosp
    {
        public const string Ime = "kntDobDospRac";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "DOB-01", root, "definitions", "netgen", "BIOVEGA", "kntDobDospRac.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM" && Pet.Attr(p, "COMMANDTIMEOUT") == "120" && Pet.Attr(p, "TOTALROWVISIBLE") == "TRUE"
                ? Qa.Z(Ime, "DOB-02", "Ulaz je FILTERFORM. Timeout je 120. Suma ostaje", "netgen/Form1.vb:3879,3463,2049")
                : Qa.P(Ime, "DOB-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            NgForm f = p.NadjiFormu("FILTERFORM");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE" && (f.OnLoad ?? "").IndexOf("@@datod", StringComparison.Ordinal) >= 0
                && Pet.GumbNaredba(p, 3, "PRINTMASTER") && Pet.GumbNaredba(p, 4, "MULTIBLUEEDIT")
                ? Qa.Z(Ime, "DOB-03", "Filter ceka OK. Ispis je PRINTMASTER. Oznaci je MULTIBLUE", "netgen/Class1.vb:113-118; netgen/Form1.vb:4951")
                : Qa.P(Ime, "DOB-03", "filter", "netgen/Form1.vb:5445", f == null ? "" : f.OnLoad));
            return l;
        }
    }

    public static class KntPrekPartner
    {
        public const string Ime = "KntPreknjizavanjePartnera";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KPP-01", root, "definitions", "netgen", "Agronom granule", "KntPreknjizavanjePartnera.xml");
            if (p == null)
                return l;
            NgForm pro = p.NadjiFormu("MULTISELECTPROKNJIZI");
            string sql = pro == null ? "" : (pro.OnSubmit ?? "");
            l.Add(p.OnLoad == ":FILTERFORM" && Pet.Attr(p, "CHECKBOXVISIBLE") == "TRUE"
                && sql.StartsWith("exec spKntPreknjizavanjePartnera @Action=2", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KPP-02", "Ulaz je filter. Proknjizi je exec action=2 i na jupiteru se ne salje", "netgen/Form1.vb:3879; netgen/Form2.vb:3378")
                : Qa.P(Ime, "KPP-02", "proknjizi", "netgen/Form2.vb:3378", sql));
            l.Add(Pet.GumbNaredba(p, 3, "PRINTMASTER") && Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "KPP-03", "Ispis je PRINTMASTER. LANDSCAPE tu orijentaciju ne mijenja", "netgen/Form1.vb:4951,5930")
                : Qa.P(Ime, "KPP-03", "ispis", "netgen/Form1.vb:4951", Pet.OpisGumba(p, 3)));
            return l;
        }
    }

    public static class Predujmovi
    {
        public const string Ime = "Predujmovi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PRD-01", root, "definitions", "netgen", "Agronom granule", "Predujmovi.xml");
            if (p == null)
                return l;
            string b = p.Buttons ?? "";
            NgForm pro = p.NadjiFormu("MULTISELECTPROKNJIZI");
            l.Add(p.OnLoad == ":FILTERFORM" && b.IndexOf("Pregled:FILTERFORM", StringComparison.Ordinal) >= 0
                && b.IndexOf("MULTISELECTPROKNJIZI", StringComparison.Ordinal) >= 0
                && b.IndexOf("SAMOPREDFORM", StringComparison.Ordinal) >= 0
                && b.IndexOf("PRINTMASTER", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PRD-02", "Ulaz je FILTERFORM. Gumbi su pregled, proknjizi, filter i ispis", "netgen/Form1.vb:3879,3770")
                : Qa.P(Ime, "PRD-02", "buttons", "netgen/Form1.vb:3770", b));
            l.Add(pro != null && (pro.OnSubmit ?? "").IndexOf("@action=2", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PRD-03", "Proknjizi je action=2 i na jupiteru se ne salje", "netgen/Form2.vb:3378")
                : Qa.P(Ime, "PRD-03", "proknjizi", "netgen/Form2.vb:3378", pro == null ? "" : pro.OnSubmit));
            return l;
        }
    }

    public static class PdvPoStjecanju
    {
        public const string Ime = "PDVpoStjecanju";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PDS-01", root, "definitions", "netgen", "BIOVEGA", "PDVpoStjecanju.xml");
            if (p == null)
                return l;
            NgForm pre = p.NadjiFormu("PREBACIFORM");
            l.Add(p.OnLoad == ":FILTERFORM" && Pet.GumbNaredba(p, 1, "FILTERFORM") && Pet.GumbNaredba(p, 3, "PRINTMASTER")
                ? Qa.Z(Ime, "PDS-02", "Prikaz je FILTERFORM. Ispis je PRINTMASTER", "netgen/Form1.vb:3879,4951")
                : Qa.P(Ime, "PDS-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(pre != null && (pre.OnSubmit ?? "").IndexOf("@action=1", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PDS-03", "Prebaci u knjigu je action=1 i na jupiteru se ne salje", "netgen/Form2.vb:3378")
                : Qa.P(Ime, "PDS-03", "prebaci", "netgen/Form2.vb:3378", pre == null ? "" : pre.OnSubmit));
            return l;
        }
    }

    public static class MpPregUtrzak
    {
        public const string Ime = "MPPregUtrzakNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "UTR-01", root, "definitions", "netgen", "BIOVEGA", "MPPregUtrzakNG.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            l.Add(p.OnLoad == ":FORM1" && tree.IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && (p.DetailGrid ?? "").IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "UTR-02", "Ulaz je FORM1. Stablo pocinje s Danas. Detail veze @@id", "netgen/Form1.vb:3879,4347,3373")
                : Qa.P(Ime, "UTR-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 6, "vrstadokid-MaloprodajaGla") && Pet.NemaRedni(p, 3)
                ? Qa.Z(Ime, "UTR-03", "Print dok je vrstadokid-. CMD3 ne postoji", "netgen/Form1.vb:5215,3314")
                : Qa.P(Ime, "UTR-03", "ispis", "netgen/Form1.vb:5215", Pet.OpisGumba(p, 6)));
            return l;
        }
    }

    public static class PdvPromjenaXml
    {
        public const string Ime = "PDVPromjena";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "PDVPromjena.xml");
            string put2 = Path.Combine(root, "definitions", "netgen", "PDVPromjena.xml");
            if (!File.Exists(put) && !File.Exists(put2))
                l.Add(Qa.O(Ime, "PDP-01", "PDVPromjena.xml nije u definitions/netgen", "docs/granule/Prioritet.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(Ime, "PDP-01", "XML je stigao", File.Exists(put) ? put : put2));
            return l;
        }
    }
}
