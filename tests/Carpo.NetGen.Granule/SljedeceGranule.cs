using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class MpFiskalPubVars
    {
        public const string Ime = "MPFiskalPubVars";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "FIS-01", root, "definitions", "netgen", "BIOVEGA", "MPFiskalPubVars.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":UPDATEFORM"
                ? Qa.Z(Ime, "FIS-02", "Ulaz otvara UPDATEFORM", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "FIS-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add((p.Buttons ?? "").IndexOf("Izmjeni:UPDATEFORM", StringComparison.Ordinal) >= 0 && p.Gumbi.Count == 0
                ? Qa.Z(Ime, "FIS-03", "Jedini gumb dolazi iz BUTTONS. Prazni CMD se ne vide", "netgen/Form1.vb:3770-3788")
                : Qa.P(Ime, "FIS-03", "BUTTONS", "netgen/Form1.vb:3770", p.Buttons ?? ""));
            l.Add(Pet.Attr(p, "TOTALROW") == "FALSE" && !p.Atributi.Ima("TOTALROWVISIBLE")
                ? Qa.Z(Ime, "FIS-04", "TOTALROW se ne cita. Suma ostaje", "netgen/Form1.vb:2049-2053")
                : Qa.P(Ime, "FIS-04", "TOTALROW", "netgen/Form1.vb:2049", Pet.Attr(p, "TOTALROW")));
            l.Add(string.IsNullOrEmpty(p.MasterDblClick) && string.IsNullOrEmpty(p.DetailGrid)
                ? Qa.Z(Ime, "FIS-05", "Nema dvostrukog klika ni detaila", "netgen/Form1.vb:3389,3373")
                : Qa.P(Ime, "FIS-05", "dbl/detail", "netgen/Form1.vb:3389", p.MasterDblClick ?? ""));
            return l;
        }
    }

    public static class BilancaGfi
    {
        public const string Ime = "Bilanca-GFI";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "GFI-01", root, "definitions", "netgen", "BIOVEGA", "Bilanca-GFI.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad.StartsWith(":FILTERFORM#", StringComparison.Ordinal)
                ? Qa.Z(Ime, "GFI-02", "ONLOAD je lanac. Prvi segment otvara FILTERFORM", "netgen/Form1.vb:3879-3880,4558-4561,4862")
                : Qa.P(Ime, "GFI-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.OnLoad.IndexOf("disable", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "GFI-03", "Iza filtera select case gasi TSI ili GFI", "netgen/Form1.vb:4558-4561")
                : Qa.P(Ime, "GFI-03", "lanac", "netgen/Form1.vb:4558", ""));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "120" && Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "GFI-04", "Timeout 120. LANDSCAPE ne vrijedi za PRINTMASTER", "netgen/Form1.vb:3463,3458,4951-4955")
                : Qa.P(Ime, "GFI-04", "timeout/orijentacija", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            l.Add(Pet.Gumb(p, 2, "GFI", "GFIPODFORM#AOPuExcel.clsGFIPOD(GFI-POD601.xls)")
                && Pet.Gumb(p, 7, "AOPuExcel", "ASSEMBLYLOAD(AOPuExcel64.Class1)")
                && Pet.Gumb(p, 4, "Ispis", "PRINTMASTER")
                ? Qa.Z(Ime, "GFI-05", "clsGFIPOD nije clsreport. DLL je samo ASSEMBLYLOAD", "netgen/Form1.vb:5215,5369-5370,4584")
                : Qa.P(Ime, "GFI-05", "gumbi", "netgen/Form1.vb:5215", Pet.OpisGumba(p, 2)));
            return l;
        }
    }

    public static class MpPdv
    {
        public const string Ime = "MPPDV";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "MPD-01", root, "definitions", "netgen", "BIOVEGA", "MPPDV.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":Filter"
                ? Qa.Z(Ime, "MPD-02", "Ulaz otvara Filter", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "MPD-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.MasterDblClick == ":MP_PDV"
                ? Qa.Z(Ime, "MPD-03", "Dvostruki klik ima vodecu dvotocku i otvara MP_PDV", "netgen/Form1.vb:6935,4862")
                : Qa.P(Ime, "MPD-03", "dblclick", "netgen/Form1.vb:6935", p.MasterDblClick));
            l.Add(Pet.Gumb(p, 4, null, "Find") && Pet.GumbNaredba(p, 6, "IzvrsiJob") && Pet.Attr(p, "PERMISIO") == "TRUE"
                ? Qa.Z(Ime, "MPD-04", "Find i IzvrsiJob su forme. PERMISIO je ukljucen", "netgen/Form1.vb:3314,3414")
                : Qa.P(Ime, "MPD-04", "gumbi", "netgen/Form1.vb:3314", Pet.OpisGumba(p, 4) + " / " + Pet.OpisGumba(p, 6)));
            l.Add(Pet.Attr(p, "VERSION") == "1.0" && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "TOTALROWVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "MPD-05", "Verzija 1.0. Checkbox i suma su ugaseni", "netgen/Form1.vb:3524,2043-2053")
                : Qa.P(Ime, "MPD-05", "verzija", "netgen/Form1.vb:3524", Pet.Attr(p, "VERSION")));
            return l;
        }
    }

    public static class KntR2Racuni
    {
        public const string Ime = "KntR2racuni1";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "R2-01", root, "definitions", "netgen", "BIOVEGA", "KntR2racuni1.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM"
                ? Qa.Z(Ime, "R2-02", "Ulaz otvara FILTERFORM", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "R2-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 3, "PRINTMASTER") && Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "R2-03", "PRINTMASTER ne gleda LANDSCAPE", "netgen/Form1.vb:4951-4955,5930")
                : Qa.P(Ime, "R2-03", "ispis", "netgen/Form1.vb:4951", Pet.OpisGumba(p, 3)));
            l.Add(Pet.Attr(p, "PAGEFOOTERLEFT").IndexOf("##", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && !p.Atributi.Ima("TOTALROWVISIBLE")
                ? Qa.Z(Ime, "R2-04", "## u podnozju je novi red. Suma ostaje jer TOTALROWVISIBLE nema", "netgen/Form1.vb:2049-2053")
                : Qa.P(Ime, "R2-04", "podnozje", "netgen/Form1.vb:2049", Pet.Attr(p, "PAGEFOOTERLEFT")));
            return l;
        }
    }

    public static class TecRazOtv
    {
        public const string Ime = "TecRazOTV";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "TecRazOTV.xml");
            string agr = Path.Combine(root, "definitions", "netgen", "Agronom granule", "TecRazOTV.xml");
            if (!File.Exists(put) && !File.Exists(agr))
            {
                l.Add(Qa.O(Ime, "TRZ-01", "TecRazOTV.xml nije u definitions/netgen", "docs/granule/ssMeni-svi-serveri.md", "nema datoteke, checklista ponašanja čeka XML"));
                return l;
            }
            l.Add(Qa.Z(Ime, "TRZ-01", "XML je stigao", put));
            return l;
        }
    }

    public static class RealizacijaStavke
    {
        public const string Ime = "RealizacijaStavkeNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string mer = Path.Combine(root, "definitions", "netgen", "MER", "RealizacijaStavkeNG.xml");
            if (!File.Exists(mer))
                l.Add(Qa.O(Ime, "REA-00", "MER\\RealizacijaStavkeNG.xml nije u repou. Provjera je Agronom primjerak", "docs/granule/Prioritet.md", mer));
            NgProject p = Pet.Ucitaj(l, Ime, "REA-01", root, "definitions", "netgen", "Agronom granule", "RealizacijaStavkeNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":Parametri" && Pet.Attr(p, "COMMANDTIMEOUT") == "180"
                ? Qa.Z(Ime, "REA-02", "Ulaz otvara Parametri. Timeout je 180", "netgen/Form1.vb:3879,3463")
                : Qa.P(Ime, "REA-02", "onload/timeout", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 3, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "REA-03", "Ispis je PRINTREPXMASTER", "netgen/Form1.vb:4945-4948")
                : Qa.P(Ime, "REA-03", "ispis", "netgen/Form1.vb:4945", Pet.OpisGumba(p, 3)));
            l.Add(Pet.Attr(p, "TOTALROWVISIBLE").Equals("false", StringComparison.OrdinalIgnoreCase)
                && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "REA-04", "Suma i checkbox stupac su ugaseni", "netgen/Form1.vb:2043-2053")
                : Qa.P(Ime, "REA-04", "total", "netgen/Form1.vb:2049", Pet.Attr(p, "TOTALROWVISIBLE")));
            l.Add(Pet.Attr(p, "PAGEHEADERLEFT").IndexOf("ISEMPTY(", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "REA-05", "#ISEMPTY u lijevom zaglavlju bira segment", "netgen/Form1.vb:7744-7768")
                : Qa.P(Ime, "REA-05", "ISEMPTY", "netgen/Form1.vb:7744", Pet.Attr(p, "PAGEHEADERLEFT")));
            return l;
        }
    }
}
