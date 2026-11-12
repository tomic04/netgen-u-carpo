using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class HrOsobeUcinak
    {
        public const string Ime = "HROsobeUcinak";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "HRU-01", root, "definitions", "netgen", "BIOVEGA", "HROsobeUcinak.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == "spHROsobeUcinakQuery @Action=0" && p.MasterDblClick == ":UPDATE_FORM" && Pet.GumbNaredba(p, 1, "UPDATE_FORM")
                ? Qa.Z(Ime, "HRU-02", "Master je action=0. Dvostruki klik i Izmjena otvaraju UPDATE_FORM", "netgen/Form1.vb:3879,4862")
                : Qa.P(Ime, "HRU-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(string.IsNullOrEmpty(p.Tree) && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "HRU-03", "Nema TREE. Checkbox je ugasen", "netgen/Form1.vb:4310,2043")
                : Qa.P(Ime, "HRU-03", "tree", "netgen/Form1.vb:4310", p.Tree ?? ""));
            return l;
        }
    }

    public static class KupVrstPn
    {
        public const string Ime = "KUPVrstPN";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KVP-01", root, "definitions", "netgen", "Agronom granule", "KUPVrstPN.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                && Pet.GumbNaredba(p, 1, "INSERTFORM") && Pet.GumbNaredba(p, 3, "DELETEFORM") && p.MasterDblClick == "UPDATEFORM"
                ? Qa.Z(Ime, "KVP-02", "ONLOAD je select. Gumbi su unos, izmjena i brisanje", "netgen/Form1.vb:3879,3314")
                : Qa.P(Ime, "KVP-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm d = p.NadjiFormu("DELETEFORM");
            l.Add(d != null && (d.OnSubmit ?? "").StartsWith("spKUPVrstaPosteAzur @action=2", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KVP-03", "Brisanje je sp action=2. Na jupiteru se taj sp ne izvrsava", "netgen/Form2.vb:3378")
                : Qa.P(Ime, "KVP-03", "brisanje", "netgen/Form2.vb:3378", d == null ? "" : d.OnSubmit));
            return l;
        }
    }

    public static class MpIzdVp
    {
        public const string Ime = "MPIzdVPNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "IZD-01", root, "definitions", "netgen", "BIOVEGA", "MPIzdVPNG.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD5CAPTION")
                    lista = true;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && tree.IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "IZD-02", "Nema ONLOAD. Stablo pocinje s Danas", "netgen/Form1.vb:3879,4347")
                : Qa.P(Ime, "IZD-02", "tree", "netgen/Form1.vb:4347", tree));
            l.Add(lista && Pet.GumbNaredba(p, 6, "vrstadokid-MaloprodajaGla")
                && (p.DetailGrid ?? "").IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "IZD-03", "Lista je skrivena. Ispis je vrstadokid-. Detail veze @@id", "netgen/Form1.vb:3320,5215,3373")
                : Qa.P(Ime, "IZD-03", "cmd", "netgen/Form1.vb:3320", string.Join(",", p.PreskoceniGumbi.ToArray())));
            return l;
        }
    }

    public static class PnNalozi
    {
        public const string Ime = "PNNalozi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PNN-01", root, "definitions", "netgen", "BIOVEGA", "PNNalozi.xml");
            if (p == null)
                return l;
            l.Add((p.OnLoad ?? "").IndexOf("@treenode='otvoreni'", StringComparison.Ordinal) >= 0
                && (p.OnLoad ?? "").IndexOf("@@operateriid", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PNN-02", "Master SQL nosi literal otvoreni i @@operateriid", "netgen/Form1.vb:2482,3879")
                : Qa.P(Ime, "PNN-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm osv = p.NadjiFormu("REFRESHTREEFORM");
            l.Add(osv != null && Pet.AttrF(osv, "FILLEXPLORER").Equals("onlytree", StringComparison.OrdinalIgnoreCase)
                && (p.MasterDblClick ?? "").IndexOf(":UPDATENALOG(@pnnaloziid=", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PNN-03", "Osvjezi drvo je onlytree. Dvostruki klik je UPDATENALOG s parametrom", "netgen/Form1.vb:5475; netgen/Form2.vb:4407")
                : Qa.P(Ime, "PNN-03", "tree", "netgen/Form1.vb:5475", osv == null ? "" : Pet.AttrF(osv, "FILLEXPLORER")));
            return l;
        }
    }

    public static class KntShemaKonta
    {
        public const string Ime = "kntShemaKonta";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "SHK-01", root, "definitions", "netgen", "Agronom granule", "kntShemaKonta.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == "exec spKntShema 2" && (p.Tree ?? "").IndexOf("spKntShemaTree:TREEROBAFORM", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "COMMANDTIMEOUT") == "120"
                ? Qa.Z(Ime, "SHK-02", "Master je exec spKntShema 2. Stablo je store prema TREEROBAFORM. Timeout je 120", "netgen/Form1.vb:3879,4350,3463")
                : Qa.P(Ime, "SHK-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 4, "PRINTMASTER") && Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "SHK-03", "Ispis je PRINTMASTER. LANDSCAPE tu orijentaciju ne mijenja", "netgen/Form1.vb:4951,5930")
                : Qa.P(Ime, "SHK-03", "ispis", "netgen/Form1.vb:4951", Pet.OpisGumba(p, 4)));
            return l;
        }
    }

    public static class NedostajeXml
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            Jedan(l, root, "KntIzvodTijek", "KIZ-01", "KntIzvodTijek.xml");
            Jedan(l, root, "KntPriGen", "KPG-01", "KntPriGen.xml");
            Jedan(l, root, "KntZatvaranjeNG", "KZT-01", "KntZatvaranjeNG.xml");
            Jedan(l, root, "ManPokazateljiGrupe", "MPG-01", "ManPokazateljiGrupe.xml");
            return l;
        }

        private static void Jedan(List<Stavka> l, string root, string ime, string id, string fajl)
        {
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", fajl);
            string put2 = Path.Combine(root, "definitions", "netgen", fajl);
            if (!File.Exists(put) && !File.Exists(put2))
                l.Add(Qa.O(ime, id, fajl + " nije u definitions/netgen", "docs/granule/Prioritet.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(ime, id, "XML je stigao", File.Exists(put) ? put : put2));
        }
    }
}
