using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class LVObracun
    {
        public const string Ime = "LVObracun";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "LVObracun.xml");
            NgProject p = Otvori(l, "LVO-01", xml);
            if (p == null)
                return l;
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@@lvglaid", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "LVO-02", "Detail veze @@lvglaid, ne @@id", "netgen/Form1.vb:2564-2572")
                : Qa.P(Ime, "LVO-02", "detail", "netgen/Form1.vb:2564", p.DetailGrid ?? ""));
            l.Add(p.Tree != null && p.Tree.IndexOf("spLVGla_Tree", StringComparison.Ordinal) >= 0 && p.Tree.IndexOf("REFRESHTREEFORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "LVO-03", "TREE: staticki REFRESHTREEFORM i store spLVGla_Tree", "netgen/Form1.vb:4347-4351")
                : Qa.P(Ime, "LVO-03", "TREE", "netgen/Form1.vb:4347", p.Tree ?? ""));
            NgForm osv = p.NadjiFormu("REFRESHTREEFORM");
            l.Add(osv != null && osv.Kontrole.Count == 0 && string.Equals(Ispis.Atr(osv, "FILLEXPLORER"), "onlytree", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "LVO-04", "onlytree samo ponovo puni stablo", "netgen/Form1.vb:5475-5476; netgen/Form2.vb:4430")
                : Qa.P(Ime, "LVO-04", "onlytree", "netgen/Form1.vb:5475", Ispis.Atr(osv, "FILLEXPLORER") ?? ""));
            string gumbi = p.Buttons ?? "";
            l.Add(gumbi.IndexOf("INSERTLVGLA(@hrkadroviid = 0)", StringComparison.Ordinal) >= 0
                && (p.MasterDblClick ?? "").StartsWith(":UPDATELVGLA(", StringComparison.Ordinal)
                ? Qa.Z(Ime, "LVO-05", "Ime forme je do zagrade. Parametri idu u strParametriSvi", "netgen/Form2.vb:4407-4418; netgen/Form1.vb:4862")
                : Qa.P(Ime, "LVO-05", "parametri forme", "netgen/Form2.vb:4407", gumbi));
            string sd = p.Atributi.Ima("SETDEFAULTS") ? (p.Atributi.Vrijednost("SETDEFAULTS") ?? "") : "";
            l.Add(sd.StartsWith("spLVGla_DefaultsNG", StringComparison.Ordinal) && sd.IndexOf('#') < 0
                ? Qa.Z(Ime, "LVO-06", "SETDEFAULTS je jedan ExecSql", "netgen/Form1.vb:3862-3866")
                : Qa.P(Ime, "LVO-06", "SETDEFAULTS", "netgen/Form1.vb:3862", sd));
            l.Add(Stablo.Meni(Ime, "LVO-07", "LVObracun.xml"));
            Ispis.K(l, Ime, xml, "LVO-20", "onlytree ne salje master. Ime(@a=) nije ime forme", "netgen/Form1.vb:5475; netgen/Form2.vb:4407", "onlytree.param", "samo-stablo");
            return l;
        }

        private static NgProject Otvori(List<Stavka> l, string id, string xml)
        {
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, id, "XML postoji", xml, "nema"));
                return null;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, id, "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return null;
            }
            l.Add(Qa.Z(Ime, id, "Projekt se ucita", "netgen/Form1.vb:3263"));
            return p;
        }
    }

    public static class Joppd
    {
        public const string Ime = "JOPPD";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "JOPPD.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "JOP-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "JOP-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "JOP-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.Tree != null && p.Tree.IndexOf("spKOD_JOPPDTree", StringComparison.Ordinal) >= 0 && p.Tree.IndexOf("REFRESHTREEFORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "JOP-02", "TREE je osvjezi plus vremenska podjela", "netgen/Form1.vb:4350-4351")
                : Qa.P(Ime, "JOP-02", "TREE", "netgen/Form1.vb:4350", p.Tree ?? ""));
            NgForm osv = p.NadjiFormu("REFRESHTREEFORM");
            l.Add(osv != null && string.Equals(Ispis.Atr(osv, "FILLEXPLORER"), "onlytree", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "JOP-03", "Osvjezi drvo je onlytree", "netgen/Form1.vb:5475-5476")
                : Qa.P(Ime, "JOP-03", "onlytree", "netgen/Form1.vb:5475", ""));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@@id", StringComparison.OrdinalIgnoreCase) >= 0
                && (p.Popup ?? "").IndexOf("@@kodjoppdaid", StringComparison.OrdinalIgnoreCase) >= 0
                && (p.Popup ?? "").IndexOf('~') >= 0
                ? Qa.Z(Ime, "JOP-04", "Detail je @@id. Popup je @@kodjoppdaid i tilda pa REFRESHMASTERGRID", "netgen/Form1.vb:4558,4570")
                : Qa.P(Ime, "JOP-04", "id", "netgen/Form1.vb:4558", p.DetailGrid ?? ""));
            string detSum = p.Atributi.Ima("DETTOTALROWVISIBLE") ? (p.Atributi.Vrijednost("DETTOTALROWVISIBLE") ?? "") : "";
            string masSum = p.Atributi.Ima("TOTALROWVISIBLE") ? (p.Atributi.Vrijednost("TOTALROWVISIBLE") ?? "") : "";
            l.Add(detSum.Equals("TRUE", StringComparison.OrdinalIgnoreCase) && masSum.Equals("false", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "JOP-05", "Suma detaila je upaljena, suma mastera ugasena", "netgen/Form1.vb:2049")
                : Qa.P(Ime, "JOP-05", "suma", "netgen/Form1.vb:2049", masSum + "/" + detSum));
            l.Add(Stablo.Meni(Ime, "JOP-06", "JOPPD.xml"));
            return l;
        }
    }

    public static class KntKompVerizne
    {
        public const string Ime = "KntKompVerizne";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntKompVerizne.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KOV-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KOV-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "KOV-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.Tree != null && p.Tree.IndexOf("sptreeMjesec", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "KOV-02", "sptree sadrzi tree pa je store, ne staticki cvor", "netgen/Form1.vb:4350-4355")
                : Qa.P(Ime, "KOV-02", "TREE", "netgen/Form1.vb:4350", p.Tree ?? ""));
            NgForm mj = p.NadjiFormu("MJESEC_FORM");
            l.Add(mj != null && mj.Kontrole.Count == 0 && (mj.OnSubmit ?? "").IndexOf("@@node", StringComparison.OrdinalIgnoreCase) < 0
                ? Qa.Z(Ime, "KOV-03", "MJESEC_FORM nema @@node. Ostale forme ga imaju", "netgen/Form1.vb:5445-5452")
                : Qa.P(Ime, "KOV-03", "mjesec", "netgen/Form1.vb:5445", mj == null ? "" : mj.OnSubmit));
            string b = p.Buttons ?? "";
            int seg = b.Length == 0 ? 0 : b.Split('#').Length;
            l.Add(seg == 5 && b.StartsWith("Dodaj:DODAJ_FORM", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KOV-04", "BUTTONS ima 5 stavki i prepisuje gumbe 1-5", "netgen/Form1.vb:3770-3788")
                : Qa.P(Ime, "KOV-04", "BUTTONS", "netgen/Form1.vb:3770", b));
            NgButton cmd6 = null;
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == 6)
                    cmd6 = p.Gumbi[i];
            }
            string n6 = cmd6 == null ? "" : cmd6.Naredba;
            l.Add(n6.StartsWith("repx/", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KOV-05", "CMD6 repx ostaje jer BUTTONS nema sessti segment", "netgen/Form1.vb:3793-3796,5238")
                : Qa.P(Ime, "KOV-05", "CMD6", "netgen/Form1.vb:3319", n6));
            l.Add(Stablo.Meni(Ime, "KOV-06", "KntKompVerizne.xml"));
            return l;
        }
    }

    public static class KntBlaPromet
    {
        public const string Ime = "KntBlaPromet";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntBlaPromet.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KBP-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KBP-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "KBP-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("spBLPrometDetQueryNG", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KBP-02", "DETAILGRID s razmakom ispred = i dalje je detail. Veza je @@id", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "KBP-02", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(p.Tree != null && p.Tree.IndexOf("spBlagajnePrometTreeNG", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KBP-03", "Store stabla je spBlagajnePrometTreeNG, forma MJESEC_FORM", "netgen/Form1.vb:4350-4351")
                : Qa.P(Ime, "KBP-03", "TREE", "netgen/Form1.vb:4350", p.Tree ?? ""));
            NgForm isp = p.NadjiFormu("ISPISDOK_FORM");
            NgControl ok = Kontrole.Nadji(isp, "ok");
            string klik = Kontrole.A(ok, "ONCLICK") ?? "";
            l.Add(string.Equals(Ispis.Atr(isp, "ONLOADED"), "click(Ok)", StringComparison.OrdinalIgnoreCase)
                && klik.IndexOf(":REPX/@@template", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "KBP-04", "click(Ok) pali gumb. Ime izvjestaja je @@template", "netgen/Form2.vb:5796,7151-7155,4296")
                : Qa.P(Ime, "KBP-04", "click", "netgen/Form2.vb:7151", klik));
            l.Add(Stablo.Meni(Ime, "KBP-05", "KntBlaPromet.xml"));
            Ispis.K(l, Ime, xml, "KBP-20", "CLICK(Ok) izvrsi ONCLICK, ne samo zatvaranje", "netgen/Form2.vb:7151-7155", "click.ok", "repx-template");
            return l;
        }
    }

    public static class MPPonuda
    {
        public const string Ime = "MPPonuda";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPPonuda.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "MPP-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "MPP-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "MPP-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(string.IsNullOrEmpty(p.OnLoad) && p.DetailGrid != null && p.DetailGrid.IndexOf("sifradok='118'", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "MPP-02", "Nema ONLOAD. Detail je MP ponuda 118", "netgen/Form1.vb:3879; netgen/Form1.vb:3373")
                : Qa.P(Ime, "MPP-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            string b = p.Buttons ?? "";
            l.Add(b.IndexOf("vrstadokid-MPPonudaGla", StringComparison.OrdinalIgnoreCase) >= 0
                && b.IndexOf("FISKALIZIRAJ", StringComparison.Ordinal) < 0
                && p.Atributi.Ima("BUTTONSoldNEkoristitiFiskal")
                ? Qa.Z(Ime, "MPP-03", "Zivi BUTTONS nema fiskal. Stari atribut se ne cita. Zadnji gumb je vrstadokid-", "netgen/Form1.vb:3391,3770; netgen/Class1.vb:443")
                : Qa.P(Ime, "MPP-03", "BUTTONS", "netgen/Form1.vb:3770", b));
            string o2 = p.Atributi.Ima("ONLOAD2") ? (p.Atributi.Vrijednost("ONLOAD2") ?? "") : "";
            l.Add(o2.IndexOf("'Dodaj'", StringComparison.Ordinal) >= 0 || o2.IndexOf("Dodaj", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MPP-04", "ONLOAD2 enable trazi stupac po natpisu gumba", "netgen/Form1.vb:3914-3921")
                : Qa.P(Ime, "MPP-04", "ONLOAD2", "netgen/Form1.vb:3914", ""));
            l.Add(p.Tree != null && p.Tree.IndexOf("spMPTreeNG", StringComparison.Ordinal) >= 0 && p.Tree.IndexOf("DANAS_FORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MPP-05", "TREE mijesa staticke cvorove i spMPTreeNG", "netgen/Form1.vb:4350-4355")
                : Qa.P(Ime, "MPP-05", "TREE", "netgen/Form1.vb:4350", p.Tree ?? ""));
            l.Add(Stablo.Meni(Ime, "MPP-06", "MPPonuda.xml"));
            Ispis.K(l, Ime, xml, "MPP-20", "enable/disable trazi Button.Text, ne NAME", "netgen/Form1.vb:3914-3921", "gumb.natpis", "Dodaj");
            return l;
        }
    }
}
