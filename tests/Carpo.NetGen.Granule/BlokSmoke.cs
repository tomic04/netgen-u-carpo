using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class PnObracun
    {
        public const string Ime = "PNObracun";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PNO-01", root, "definitions", "netgen", "BIOVEGA", "PNObracun.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.IndexOf("@treenode='otvoreni'", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PNO-02", "Master SQL nosi literal otvoreni", "netgen/Form1.vb:3879-3882")
                : Qa.P(Ime, "PNO-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add((p.MasterDblClick ?? "").IndexOf(":UPDATENALOG(@pnnaloziid=", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PNO-03", "Dvostruki klik je :UPDATENALOG(@pnnaloziid=). Ime forme je lijevo od zagrade", "netgen/Form2.vb:4407-4418")
                : Qa.P(Ime, "PNO-03", "dblclick", "netgen/Form2.vb:4407", p.MasterDblClick ?? ""));
            l.Add((p.Tree ?? "").IndexOf("spPNNalozi_Tree", StringComparison.Ordinal) >= 0 && Pet.Attr(p, "PERMISIO") == "TRUE"
                ? Qa.Z(Ime, "PNO-04", "Stablo ima store. PERMISIO je ukljucen", "netgen/Form1.vb:4350,3414")
                : Qa.P(Ime, "PNO-04", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            return l;
        }
    }

    public static class KntBlaDok
    {
        public const string Ime = "KntBlaDok";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KBD-01", root, "definitions", "netgen", "Agronom granule", "KntBlaDok.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            l.Add(p.OnLoad == "spBLDokumentiQueryNG @action=0" && tree.StartsWith("Blagajne:#", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KBD-02", "Master je action=0. Prvi segment stabla nema dvotocku", "netgen/Form1.vb:3879,4298,4348")
                : Qa.P(Ime, "KBD-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add((p.DetailGrid ?? "").IndexOf("@BlDokumentiId=@@id", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "POPUPDETAIL").IndexOf("STAVKE_UNESI_FORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KBD-03", "Detail veze @@id. POPUPDETAIL puni izbornik detalja", "netgen/Form1.vb:3373,3386,3702")
                : Qa.P(Ime, "KBD-03", "detail", "netgen/Form1.vb:3386", Pet.Attr(p, "POPUPDETAIL")));
            return l;
        }
    }

    public static class Akcije
    {
        public const string Ime = "Akcije";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "AKC-01", root, "definitions", "netgen", "BIOVEGA", "Akcije.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && (p.Tree ?? "").IndexOf("spAkcije_Tree", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "AKC-02", "Nema ONLOAD. Master se ne salje dok cvor ne da SQL", "netgen/Class1.vb:113-116")
                : Qa.P(Ime, "AKC-02", "onload", "netgen/Class1.vb:113", p.OnLoad ?? ""));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "600" && Pet.Attr(p, "TOTALROWVISIBLE").Equals("false", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "AKC-03", "Timeout je 600. false gasi sumu jer se cita velikim slovima", "netgen/Form1.vb:3463,2049-2050")
                : Qa.P(Ime, "AKC-03", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            l.Add(Pet.Attr(p, "POPUPMASTER").IndexOf("#-:#", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "AKC-04", "Segment -: je stavka s natpisom crtice i praznim tagom", "netgen/Form1.vb:3564-3566")
                : Qa.P(Ime, "AKC-04", "popup", "netgen/Form1.vb:3564", Pet.Attr(p, "POPUPMASTER")));
            return l;
        }
    }

    public static class ManagerObaveza
    {
        public const string Ime = "ManagerObaveza";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string mer = Path.Combine(root, "definitions", "netgen", "MER", "ManagerObaveza.xml");
            if (!File.Exists(mer))
                l.Add(Qa.O(Ime, "MOB-00", "MER\\ManagerObaveza.xml nije u repou", "docs/granule/Prioritet.md", mer));
            else
                l.Add(Qa.Z(Ime, "MOB-00", "MER putanja postoji", mer));
            NgProject p = Pet.Ucitaj(l, Ime, "MOB-01", root, "definitions", "netgen", "BIOVEGA", "ManagerObaveza.xml");
            if (p == null)
                return l;
            bool razdoblje = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD3CAPTION")
                    razdoblje = true;
            l.Add((p.Tree ?? "").IndexOf("spTreeKlMasterRoba", StringComparison.Ordinal) >= 0
                && razdoblje && Pet.GumbNaredba(p, 8, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "MOB-02", "Stablo je store. Razdoblje nema dvotocku. Ispis je PRINTREPXMASTER", "netgen/Form1.vb:4350,3320,4945")
                : Qa.P(Ime, "MOB-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            return l;
        }
    }

    public static class PnKatalozi
    {
        public const string Ime = "PNKatalozi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PNK-01", root, "definitions", "netgen", "BIOVEGA", "PNKatalozi.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && (p.Tree ?? "").IndexOf("spPNKatalozi_Tree @action='tree':STATUS", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PNK-02", "Nema ONLOAD. Jedini cvor je store prema STATUS", "netgen/Form1.vb:3879,4350-4351")
                : Qa.P(Ime, "PNK-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "600"
                ? Qa.Z(Ime, "PNK-03", "Timeout je 600", "netgen/Form1.vb:3463")
                : Qa.P(Ime, "PNK-03", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            return l;
        }
    }

    public static class PorukeDocXml
    {
        public const string Ime = "PorukeDoc";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "PorukeDoc.xml");
            string put2 = Path.Combine(root, "definitions", "netgen", "PorukeDoc.xml");
            if (!File.Exists(put) && !File.Exists(put2))
                l.Add(Qa.O(Ime, "PDO-01", "PorukeDoc.xml nije u definitions/netgen", "docs/granule/Prioritet.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(Ime, "PDO-01", "XML je stigao", File.Exists(put) ? put : put2));
            return l;
        }
    }

    public static class SmokePrazno
    {
        public const string Ime = "SmokePrazno";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            Prazno(l, root, "SPZ-01", "BIOVEGA", "Akcije.xml");
            Prazno(l, root, "SPZ-02", "BIOVEGA", "JOPPD.xml");
            Prazno(l, root, "SPZ-03", "BIOVEGA", "MPBlagajna.xml");
            Prazno(l, root, "SPZ-04", "BIOVEGA", "MPPSNG.xml");
            Prazno(l, root, "SPZ-05", "BIOVEGA", "MPPonuda.xml");
            Prazno(l, root, "SPZ-06", "Agronom granule", "PrijemniList.xml");
            Prazno(l, root, "SPZ-07", "BIOVEGA", "MPKnjigaPopisa.xml");
            Prazno(l, root, "SPZ-08", "BIOVEGA", "PNKatalozi.xml");
            Prazno(l, root, "SPZ-09", "BIOVEGA", "KOD_RSPostavke.xml");
            Prazno(l, root, "SPZ-10", "BIOVEGA", "MPInventura.xml");
            Ostao(l, root, "SPZ-11", "BIOVEGA", "kntDobDospRac.xml", "FILTERFORM");
            Ostao(l, root, "SPZ-12", "BIOVEGA", "KntOPZ-STAT-1.xml", "UPIT");
            Ostao(l, root, "SPZ-13", "BIOVEGA", "KntPDVObrasciEU.xml", "FILTER_FORM");
            Ostao(l, root, "SPZ-14", "BIOVEGA", "KntR2racuni1.xml", "FILTERFORM");
            Ostao(l, root, "SPZ-15", "BIOVEGA", "PDVpoStjecanju.xml", "FILTERFORM");
            Ostao(l, root, "SPZ-16", "Agronom granule", "Predujmovi.xml", "FILTERFORM");
            Ostao(l, root, "SPZ-17", "BIOVEGA", "MPRACNEFISK.xml", "FORM1");
            Ostao(l, root, "SPZ-18", "BIOVEGA", "Bilanca-GFI.xml", "FILTERFORM");
            Salji(l, root, "SPZ-19", "Agronom granule", "KntBlaDok.xml", "spBLDokumentiQueryNG @action=0");
            Salji(l, root, "SPZ-20", "BIOVEGA", "LVObracun.xml", "spLVGla_Query @action='master', @treenode='0'");
            return l;
        }

        private static NgProject Ucitaj(List<Stavka> l, string id, string root, string folder, string fajl)
        {
            return Pet.Ucitaj(l, Ime, id, root, "definitions", "netgen", folder, fajl);
        }

        private static void Prazno(List<Stavka> l, string root, string id, string folder, string fajl)
        {
            NgProject p = Ucitaj(l, id, root, folder, fajl);
            if (p == null)
                return;
            string sql = SqlTekst.Sredi(p.OnLoad ?? "", Master());
            l.Add(SqlTekst.Odluka(sql) == "prazno"
                ? Qa.Z(Ime, id, fajl + " nema master SQL. Prazan grid nije rezultat upita", "netgen/Class1.vb:113-116")
                : Qa.P(Ime, id, "prazno", "netgen/Class1.vb:113", sql));
        }

        private static void Ostao(List<Stavka> l, string root, string id, string folder, string fajl, string forma)
        {
            NgProject p = Ucitaj(l, id, root, folder, fajl);
            if (p == null)
                return;
            string on = p.OnLoad ?? "";
            NgForm f = p.NadjiFormu(forma);
            string sql = f == null ? "" : SqlTekst.Sredi(f.OnLoad ?? "", FormaKontekst());
            bool lanac = on.StartsWith(":" + forma, StringComparison.OrdinalIgnoreCase);
            l.Add(lanac && f != null && SqlTekst.Odluka(sql) == "ostao-@@"
                ? Qa.Z(Ime, id, fajl + " otvara " + forma + ". @@ kontrole ostaje i ExecSql preskace", "netgen/Class1.vb:113-118")
                : Qa.P(Ime, id, "ostao", "netgen/Class1.vb:113", on + " | " + sql));
        }

        private static void Salji(List<Stavka> l, string root, string id, string folder, string fajl, string ocekivano)
        {
            NgProject p = Ucitaj(l, id, root, folder, fajl);
            if (p == null)
                return;
            var k = Master();
            string sql = SqlTekst.Sredi(p.OnLoad ?? "", k).Replace("~~", ",");
            l.Add(sql == ocekivano && SqlTekst.Odluka(sql) == "salji"
                ? Qa.Z(Ime, id, fajl + " salje isti SQL kao NetGen. 0 redova je rezultat upita", "netgen/Form1.vb:2470-2502")
                : Qa.P(Ime, id, "salji", "netgen/Form1.vb:2470", sql));
        }

        private static SqlKontekst Master()
        {
            var k = new SqlKontekst();
            k.Id = "";
            k.OperaterId = "7";
            k.Node = "null";
            return k;
        }

        private static SqlKontekst FormaKontekst()
        {
            var k = SqlTekst.ZaFormu(null);
            k.OperaterId = "7";
            return k;
        }
    }
}
