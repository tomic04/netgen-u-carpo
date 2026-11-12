using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class MpKaseOper
    {
        public const string Ime = "MPKaseOperNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KAS-01", root, "definitions", "netgen", "BIOVEGA", "MPKaseOperNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.IndexOf("spMPKaseOper @action=1", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KAS-02", "ONLOAD je master SQL, razmak oko = ne mijenja ime", "netgen/Form1.vb:3879-3882")
                : Qa.P(Ime, "KAS-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@trgovinaid=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KAS-03", "Detail veze @@id kao trgovinaid", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "KAS-03", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(Pet.Attr(p, "DETAILDBLCLICK") == "UPDATE_FORM"
                ? Qa.Z(Ime, "KAS-04", "Dvostruki klik detaila otvara UPDATE_FORM", "netgen/Form1.vb:3390,7201-7202")
                : Qa.P(Ime, "KAS-04", "DETAILDBLCLICK", "netgen/Form1.vb:3390", Pet.Attr(p, "DETAILDBLCLICK")));
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            l.Add(lista && Pet.NemaRedni(p, 7)
                ? Qa.Z(Ime, "KAS-05", "Lista nema dvotocku i ostaje skrivena", "netgen/Form1.vb:3320,3834")
                : Qa.P(Ime, "KAS-05", "CMD7", "netgen/Form1.vb:3320", string.Join(",", p.PreskoceniGumbi.ToArray())));
            l.Add(string.IsNullOrEmpty(p.Tree)
                ? Qa.Z(Ime, "KAS-06", "TREE je prazan i stablo se sklanja", "netgen/Form1.vb:4310-4316")
                : Qa.P(Ime, "KAS-06", "tree", "netgen/Form1.vb:4310", p.Tree));
            return l;
        }
    }

    public static class KodRsPostavke
    {
        public const string Ime = "KOD_RSPostavke";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KOD-01", root, "definitions", "netgen", "BIOVEGA", "KOD_RSPostavke.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            l.Add(tree.IndexOf("REFRESHTREEFORM", StringComparison.Ordinal) >= 0
                && tree.IndexOf("spKOD_RSPostavke_tree", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KOD-02", "Stablo ima staticki cvor i dva store cvora prema TREEFORM", "netgen/Form1.vb:4347-4356")
                : Qa.P(Ime, "KOD-02", "tree", "netgen/Form1.vb:4347", tree));
            string b = p.Buttons ?? "";
            l.Add(b.IndexOf("IZMJENA(@tablica=", StringComparison.Ordinal) >= 0
                && b.IndexOf("BRISANJE(@tablica=", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KOD-03", "Ime forme je lijevo od zagrade, parametri idu u strParametriSvi", "netgen/Form2.vb:4407-4418")
                : Qa.P(Ime, "KOD-03", "BUTTONS", "netgen/Form2.vb:4407", b));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "600" && Pet.Attr(p, "PERMISIO") == "TRUE"
                ? Qa.Z(Ime, "KOD-04", "Timeout je 600. PERMISIO je ukljucen", "netgen/Form1.vb:3463,3414")
                : Qa.P(Ime, "KOD-04", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            l.Add(Pet.Attr(p, "REFRESHAFTERCHILD") == "TRUE" && Pet.Attr(p, "FILLEXPLORER").Equals("true", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KOD-05", "REFRESHAFTERCHILD na projektu se samo upise. FILLEXPLORER projekta se ne cita", "netgen/Form1.vb:3396")
                : Qa.P(Ime, "KOD-05", "refresh", "netgen/Form1.vb:3396", Pet.Attr(p, "REFRESHAFTERCHILD")));
            l.Add(Pet.Attr(p, "TOTALROWVISIBLE").Equals("false", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KOD-06", "Suma je ugasena", "netgen/Form1.vb:2049-2050")
                : Qa.P(Ime, "KOD-06", "total", "netgen/Form1.vb:2049", Pet.Attr(p, "TOTALROWVISIBLE")));
            return l;
        }
    }

    public static class RealizacijaFakture
    {
        public const string Ime = "RealizacijaFakture";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string mer = Path.Combine(root, "definitions", "netgen", "MER", "RealizacijaFakture.xml");
            if (!File.Exists(mer))
                l.Add(Qa.O(Ime, "RFA-00", "MER\\RealizacijaFakture.xml nije u repou", "docs/granule/Prioritet.md", mer));
            NgProject p = Pet.Ucitaj(l, Ime, "RFA-01", root, "definitions", "netgen", "Agronom granule", "RealizacijaFakture.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":Parametri" && Pet.Attr(p, "COMMANDTIMEOUT") == "180"
                ? Qa.Z(Ime, "RFA-02", "Ulaz otvara Parametri. Timeout je 180", "netgen/Form1.vb:3879,3463")
                : Qa.P(Ime, "RFA-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 3, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "RFA-03", "Ispis je PRINTREPXMASTER", "netgen/Form1.vb:4945-4948")
                : Qa.P(Ime, "RFA-03", "ispis", "netgen/Form1.vb:4945", Pet.OpisGumba(p, 3)));
            l.Add(Pet.Attr(p, "PAGEHEADERLEFT").IndexOf("ISEMPTY(", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "RFA-04", "#ISEMPTY bira segment zaglavlja", "netgen/Form1.vb:7744-7768")
                : Qa.P(Ime, "RFA-04", "ISEMPTY", "netgen/Form1.vb:7744", ""));
            return l;
        }
    }

    public static class MpKnjigaPopisa
    {
        public const string Ime = "MPKnjigaPopisa";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KNP-01", root, "definitions", "netgen", "BIOVEGA", "MPKnjigaPopisa.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && string.IsNullOrEmpty(p.Buttons) && p.Gumbi.Count == 0
                ? Qa.Z(Ime, "KNP-02", "Nema ONLOAD ni gumba. MAINFORM se s podloge ne otvara", "netgen/Form1.vb:3879")
                : Qa.P(Ime, "KNP-02", "ulaz", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add(Pet.Attr(p, "PAGEORIENTATION") == "PORTAL"
                ? Qa.Z(Ime, "KNP-03", "PORTAL nije LANDSCAPE. Orijentacija ostaje uspravna", "netgen/Form1.vb:50,3458,5930")
                : Qa.P(Ime, "KNP-03", "orijentacija", "netgen/Form1.vb:5930", Pet.Attr(p, "PAGEORIENTATION")));
            l.Add(p.MasterDblClick == "KARTICA" && p.NadjiFormu("KARTICA") == null && p.NadjiFormu("MAINFORM") != null
                ? Qa.Z(Ime, "KNP-04", "KARTICA ne postoji. MAINFORM je u XML-u, ali je nitko ne zove", "netgen/Form1.vb:5369-5370")
                : Qa.P(Ime, "KNP-04", "forme", "netgen/Form1.vb:6935", p.MasterDblClick));
            NgForm f = p.NadjiFormu("MAINFORM");
            l.Add(f != null && Pet.AttrF(f, "COMMANDTIMEOUT") == "600"
                ? Qa.Z(Ime, "KNP-05", "Timeout forme je 600", "netgen/Form2.vb:1035")
                : Qa.P(Ime, "KNP-05", "timeout forme", "netgen/Form2.vb:1035", f == null ? "" : Pet.AttrF(f, "COMMANDTIMEOUT")));
            return l;
        }
    }

    public static class SkladisteNg
    {
        public const string Ime = "SkladisteNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "SKL-01", root, "definitions", "netgen", "BIOVEGA", "SkladisteNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.StartsWith("spSkladisteQueryNG @action=8", StringComparison.Ordinal)
                ? Qa.Z(Ime, "SKL-02", "ONLOAD je master SQL", "netgen/Form1.vb:3879-3882")
                : Qa.P(Ime, "SKL-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            string b = p.Buttons ?? "";
            l.Add(b.IndexOf("Dodaj:INSERT", StringComparison.Ordinal) >= 0
                && b.IndexOf("Ispis:PRINTMASTER", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "UPDATE"
                ? Qa.Z(Ime, "SKL-03", "BUTTONS ima INSERT, UPDATE, DELETE i PRINTMASTER. Dvostruki klik je UPDATE", "netgen/Form1.vb:3770,4951,6935")
                : Qa.P(Ime, "SKL-03", "BUTTONS", "netgen/Form1.vb:3770", b));
            l.Add(Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "TOTALROWVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "SKL-04", "Checkbox stupac i suma su ugaseni", "netgen/Form1.vb:2043-2053")
                : Qa.P(Ime, "SKL-04", "grid", "netgen/Form1.vb:2043", ""));
            return l;
        }
    }

    public static class PorukeXml
    {
        public const string Ime = "Poruke";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "Poruke.xml");
            if (!File.Exists(put))
                l.Add(Qa.O(Ime, "POR-01", "Poruke.xml nije u definitions/netgen", "docs/granule/ssMeni-svi-serveri.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(Ime, "POR-01", "XML je stigao", put));
            return l;
        }
    }
}
