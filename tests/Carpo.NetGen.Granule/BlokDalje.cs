using System;
using System.Collections.Generic;
using System.IO;

namespace Carpo.NetGen.Granule
{
    public static class MpRealizNg
    {
        public const string Ime = "MPRealizNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "REA-01", root, "definitions", "netgen", "Agronom granule", "MPRealizNG.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("FORM1");
            l.Add(p.OnLoad == ":FORM1" && Pet.GumbNaredba(p, 1, "FORM1") && Pet.Attr(p, "COMMANDTIMEOUT") == "600"
                ? Qa.Z(Ime, "REA-02", "Ulaz i Parametri otvaraju FORM1. Timeout je 600", "netgen/Form1.vb:3879,3463")
                : Qa.P(Ime, "REA-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            string on = f == null ? "" : (f.OnLoad ?? "");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE" && on.IndexOf("@@datumod", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "REA-03", "ONLOAD forme zadrzi @@datumod i ExecSql ga preskoci. Master ceka FILLEXPLORER", "netgen/Class1.vb:113-118")
                : Qa.P(Ime, "REA-03", "forma", "netgen/Class1.vb:113", on));
            return l;
        }
    }

    public static class PartneriAudit
    {
        public const string Ime = "PartneriAuditNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PAU-01", root, "definitions", "netgen", "BIOVEGA", "PartneriAuditNG.xml");
            if (p == null)
                return l;
            bool filter = p.NadjiFormu("FILTER") != null;
            l.Add(p.OnLoad == ":FILTER" && p.MasterDblClick == "DETALJI"
                ? Qa.Z(Ime, "PAU-02", "Ulaz i dvostruki klik. Filter je element FILTER", "netgen/Form1.vb:3879,6935")
                : Qa.P(Ime, "PAU-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(filter
                ? Qa.Z(Ime, "PAU-03", "Element FILTER je forma", "netgen/Form2.vb:957")
                : Qa.O(Ime, "PAU-03", "Element FILTER jos nije forma. NetGen trazi /PROJECT/FILTER", "netgen/Form2.vb:957", "NetGenXmlReader drzi FILTER strukturnim"));
            return l;
        }
    }

    public static class ZakljucivanjeTem
    {
        public const string Ime = "ZakljucivanjeTemNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "ZKT-01", root, "definitions", "netgen", "Agronom granule", "ZakljucivanjeTemNG.xml");
            if (p == null)
                return l;
            NgForm main = p.NadjiFormu("MAINFORM");
            NgForm zak = p.NadjiFormu("Zakljucavanje");
            l.Add(p.OnLoad == ":MAINFORM" && main != null && Pet.AttrF(main, "MAXIMIZEBOX") == "TRUE"
                ? Qa.Z(Ime, "ZKT-02", "Ulaz otvara MAINFORM. MAXIMIZEBOX TRUE pali povecanje", "netgen/Form1.vb:3879; netgen/Form2.vb:1031")
                : Qa.P(Ime, "ZKT-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            string sql = zak == null ? "" : (zak.OnSubmit ?? "");
            l.Add(sql.StartsWith("spKntZakljucivanjeTem @action=1", StringComparison.Ordinal)
                ? Qa.Z(Ime, "ZKT-03", "Zakljuci salje spKntZakljucivanjeTem action=1. Na jupiteru se taj sp ne izvrsava", "netgen/Form2.vb:3378")
                : Qa.P(Ime, "ZKT-03", "submit", "netgen/Form2.vb:3378", sql));
            return l;
        }
    }

    public static class MpNivelacija
    {
        public const string Ime = "MPNivelacija";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "NIV-01", root, "definitions", "netgen", "BIOVEGA", "MPNivelacija.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            l.Add(string.IsNullOrEmpty(p.OnLoad) && tree.IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && tree.IndexOf("spMPTreeNG", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NIV-02", "Nema ONLOAD. Stablo ima Danas i store spMPTreeNG", "netgen/Form1.vb:3879,4350")
                : Qa.P(Ime, "NIV-02", "tree", "netgen/Form1.vb:4347", tree));
            l.Add((p.Buttons ?? "").IndexOf("vrstadokid-MaloprodajaGla", StringComparison.Ordinal) >= 0
                && (p.DetailGrid ?? "").IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NIV-03", "Ispis je vrstadokid-. Detail veze @@id", "netgen/Form1.vb:5215,3373")
                : Qa.P(Ime, "NIV-03", "ispis", "netgen/Form1.vb:5215", p.Buttons ?? ""));
            l.Add(Pet.Attr(p, "ONLOAD2").IndexOf("'Dodaj'", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NIV-04", "ONLOAD2 trazi stupac po natpisu gumba", "netgen/Form1.vb:3914-3921")
                : Qa.P(Ime, "NIV-04", "onload2", "netgen/Form1.vb:3914", ""));
            return l;
        }
    }

    public static class KntPreknjizavanje
    {
        public const string Ime = "KntPreknjizavanjeNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PRK-01", root, "definitions", "netgen", "BIOVEGA", "KntPreknjizavanjeNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTER" && (p.Buttons ?? "").IndexOf("MULTISELECT_Proknjizi", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "CMD5CAPTION").IndexOf("PRINTMASTER#REFRESHMASTER", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PRK-02", "Ulaz je FILTER. CMD5 je PRINTMASTER pa REFRESHMASTER", "netgen/Form1.vb:3879,4951,4570")
                : Qa.P(Ime, "PRK-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.NadjiFormu("FILTER") != null
                ? Qa.Z(Ime, "PRK-03", "Element FILTER je forma", "netgen/Form2.vb:957")
                : Qa.O(Ime, "PRK-03", "Element FILTER jos nije forma. NetGen trazi /PROJECT/FILTER", "netgen/Form2.vb:957", "NetGenXmlReader drzi FILTER strukturnim"));
            l.Add(Pet.Attr(p, "CHECKBOXVISIBLE") == "TRUE"
                ? Qa.Z(Ime, "PRK-04", "CHECKBOXVISIBLE TRUE ostavlja stupac 1 vidljiv", "netgen/Form1.vb:2043-2046")
                : Qa.P(Ime, "PRK-04", "checkbox", "netgen/Form1.vb:2043", Pet.Attr(p, "CHECKBOXVISIBLE")));
            return l;
        }
    }

    public static class KntBlaVrsteXml
    {
        public const string Ime = "KntBlaVrste";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "KntBlaVrste.xml");
            if (!File.Exists(put))
                l.Add(Qa.O(Ime, "KBV-01", "KntBlaVrste.xml nije u definitions/netgen", "docs/granule/Prioritet.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(Ime, "KBV-01", "XML je stigao", put));
            return l;
        }
    }

    public static class MpTrgovineXml
    {
        public const string Ime = "MPTrgovine";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string put = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPTrgovine.xml");
            if (!File.Exists(put))
                l.Add(Qa.O(Ime, "MTR-01", "MPTrgovine.xml nije u definitions/netgen", "docs/granule/Prioritet.md", "otvoreno dok XML ne stigne"));
            else
                l.Add(Qa.Z(Ime, "MTR-01", "XML je stigao", put));
            return l;
        }
    }
}
