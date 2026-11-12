using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Poziv Carpo subforme (CarpoForm, tilda, tri potpisa totalgen) i ispis
    /// PRINTMASTER / :repx / vrstadokid-. Kuka bez kljuca je otvorena.
    /// </summary>
    internal static class Ispis
    {
        public static NgButton Gumb(NgProject p, int redni)
        {
            if (p == null)
                return null;
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == redni)
                    return p.Gumbi[i];
            }
            return null;
        }

        public static string Atr(NgForm f, string ime)
        {
            if (f == null || f.Atributi == null || !f.Atributi.Ima(ime))
                return null;
            return f.Atributi.Vrijednost(ime) ?? "";
        }

        public static string RepxShell(string s)
        {
            if (s == null)
                return "";
            return s.Replace("repx/", "repx:").Replace("REPX/", "repx:");
        }

        public static string RepxDijalog(string s)
        {
            if (s == null)
                return "";
            return s.Replace("REPX/", "REPX:/").Replace("repx/", "repx:/");
        }

        public static bool VecImaId(string param)
        {
            if (param == null)
                return false;
            string p = param.ToLowerInvariant();
            return p.Contains("@id=") || p.Contains("@id =");
        }

        public static void K(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Kljuc(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }
    }

    public static class PozivPravila
    {
        public const string Ime = "PozivIspis";

        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            l.Add(Ispis.RepxShell("repx/rptX") == "repx:rptX" && Ispis.RepxShell("REPX/rptX") == "repx:rptX"
                ? Qa.Z(Ime, "ISP-01", "Shell reze repx/ na repx: bez kose crte", "netgen/Form1.vb:5238-5239")
                : Qa.P(Ime, "ISP-01", "repx shell", "netgen/Form1.vb:5238", Ispis.RepxShell("repx/rptX")));
            l.Add(Ispis.RepxDijalog("repx/rptX") == "repx:/rptX"
                ? Qa.Z(Ime, "ISP-02", "Dijalog reze repx/ na repx:/", "netgen/Form2.vb:4296-4297")
                : Qa.P(Ime, "ISP-02", "repx dijalog", "netgen/Form2.vb:4297", Ispis.RepxDijalog("repx/rptX")));
            l.Add(Ispis.VecImaId("@DatumOd=1") == false && Ispis.VecImaId("@id=5") && Ispis.VecImaId("@id =5")
                ? Qa.Z(Ime, "ISP-03", "Drugi @id se ne lijepi ako parametar vec ima @id=", "netgen/Class1.vb:320-327")
                : Qa.P(Ime, "ISP-03", "@id", "netgen/Class1.vb:320", ""));
            return l;
        }
    }

    public static class KupCarpo
    {
        public const string Ime = "KUPNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KUPNG.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KUC-01", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            l.Add(Qa.Z(Ime, "KUC-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgButton cmd8 = Ispis.Gumb(p, 8);
            NgButton cmd10 = Ispis.Gumb(p, 10);
            string n8 = cmd8 == null ? "" : cmd8.Naredba;
            l.Add(n8.IndexOf('~') >= 0 && n8.IndexOf("ADAPTER_FORM", StringComparison.Ordinal) >= 0 && n8.IndexOf("REFRESHMASTERGRID", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KUC-02", "CMD8 tilda otvori ADAPTER_FORM pa REFRESHMASTERGRID", "netgen/Form1.vb:4558,4570")
                : Qa.P(Ime, "KUC-02", "CMD8", "netgen/Form1.vb:4558", n8));
            string n10 = cmd10 == null ? "" : cmd10.Naredba;
            l.Add(n10.IndexOf('~') < 0 && n10.IndexOf("ASSEMBLYLOAD(KUPPreglednikDMS.ScanPreview)", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KUC-03", "CMD10 bez tildu je shell ASSEMBLYLOAD, DMS ne Carpo", "netgen/Form1.vb:4584-4623,5759")
                : Qa.P(Ime, "KUC-03", "CMD10", "netgen/Form1.vb:4584", n10));
            NgForm ad = p.NadjiFormu("ADAPTER_FORM");
            string on = Ispis.Atr(ad, "ONLOADED") ?? "";
            NgControl carpo = Kontrole.Nadji(ad, "CarpoForm");
            l.Add(ad != null && ad.Kontrole.Count == 1 && on.StartsWith("ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KUC-04", "Jedna kontrola: ONLOADED ne ide u granu 2798, ali Load 5796 ga zove", "netgen/Form2.vb:2798,3069,5796")
                : Qa.P(Ime, "KUC-04", "ONLOADED", "netgen/Form2.vb:5796", on));
            l.Add(carpo != null && Kontrole.A(carpo, "DEFAULT") == "KnjigaUra.frmKnjigaUraInsert.xml"
                && string.Equals(Kontrole.A(carpo, "VISIBLE"), "false", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KUC-05", "CarpoForm DEFAULT ide u strParametriSvi dijaloga", "netgen/Form2.vb:7424-7456")
                : Qa.P(Ime, "KUC-05", "CarpoForm", "netgen/Form2.vb:7456", Kontrole.A(carpo, "DEFAULT") ?? ""));
            string pop = p.Popup ?? "";
            l.Add(pop.IndexOf("ADAPTER_FORM~REFRESHMASTERGRID", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KUC-06", "Popup s tildom ide u Rafalno, ne u NovaGenerickaForma", "netgen/Form1.vb:5743-5757")
                : Qa.P(Ime, "KUC-06", "popup", "netgen/Form1.vb:5743", ""));
            l.Add(Stablo.Meni(Ime, "KUC-07", "KUPNG.xml"));
            Ispis.K(l, Ime, xml, "KUC-20", "Dijalog totalgen: id, operater, konekcija, strParametriSvi, strParametriSvi", "netgen/Form2.vb:7424-7456", "totalgen.dijalog", "parametri-dva");
            Ispis.K(l, Ime, xml, "KUC-21", "Shell bez tildu: zadnji SQL mastera i ParametriSaGrida. Povrat se ne cita", "netgen/Form1.vb:4623,5275-5278", "totalgen.shell", "master-sql");
            return l;
        }
    }

    public static class KupDostKn
    {
        public const string Ime = "KUPDostKN";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KUPDostKN.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KND-01", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            l.Add(Qa.Z(Ime, "KND-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            string dbl = p.MasterDblClick ?? "";
            l.Add(dbl.IndexOf('~') < 0 && dbl.IndexOf("ASSEMBLYLOAD(KUPPreglednikDMS.ScanPreview)", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KND-02", "Dvostruki klik bez tildu je shell ASSEMBLYLOAD", "netgen/Form1.vb:4584-4623")
                : Qa.P(Ime, "KND-02", "MASTERDBLCLICK", "netgen/Form1.vb:4584", dbl));
            NgButton cmd10 = Ispis.Gumb(p, 10);
            string n10 = cmd10 == null ? "" : cmd10.Naredba;
            l.Add(n10 == dbl
                ? Qa.Z(Ime, "KND-03", "CMD10 je isti DMS poziv kao dvostruki klik", "netgen/Form1.vb:5759")
                : Qa.P(Ime, "KND-03", "CMD10", "netgen/Form1.vb:5759", n10));
            string pop = p.Popup ?? "";
            l.Add(pop.IndexOf("ADAPTER_FORM~REFRESHMASTERGRID", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KND-04", "Popup s tildom otvori ADAPTER_FORM pa refresh", "netgen/Form1.vb:5743-5757,4558")
                : Qa.P(Ime, "KND-04", "popup", "netgen/Form1.vb:5743", ""));
            NgForm ad = p.NadjiFormu("ADAPTER_FORM");
            NgControl carpo = Kontrole.Nadji(ad, "CarpoForm");
            l.Add(carpo != null && (Ispis.Atr(ad, "ONLOADED") ?? "").IndexOf("clsCarpoKnjigaUra", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KND-05", "CarpoForm i ONLOADED su dijalog potpis, ne shell", "netgen/Form2.vb:5796,7424-7456")
                : Qa.P(Ime, "KND-05", "adapter", "netgen/Form2.vb:7456", ""));
            l.Add(Stablo.Meni(Ime, "KND-06", "KUPDostKN.xml"));
            return l;
        }
    }

    public static class KupDostBvg
    {
        public const string Ime = "KUPDostKnNGBvg";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "KUPDostKnNGBvg.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "BVG-01", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            l.Add(Qa.Z(Ime, "BVG-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgForm a = p.NadjiFormu("ADAPTER_FORM");
            NgForm edit = p.NadjiFormu("MULTISELECT_ADAPTER_FORM_KUPURAEDIT");
            NgForm ura = p.NadjiFormu("ADAPTER_FORM_KUPURA");
            NgForm odo = p.NadjiFormu("ADAPTER_FORM_KUPOdobravanje");
            l.Add(Kontrole.A(Kontrole.Nadji(a, "CarpoForm"), "DEFAULT") == "KnjigaUra.frmKnjigaUraInsert.xml"
                && (Ispis.Atr(a, "ONLOADED") ?? "").IndexOf("clsCarpoKnjigaUra", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "BVG-02", "Insert adapter: clsCarpoKnjigaUra i frmKnjigaUraInsert", "netgen/Form2.vb:5796")
                : Qa.P(Ime, "BVG-02", "insert", "netgen/Form2.vb:5796", ""));
            l.Add(Kontrole.A(Kontrole.Nadji(edit, "CarpoForm"), "DEFAULT") == "KnjigaUra.frmKnjigaUraKupEdit.xml"
                && (Ispis.Atr(edit, "ONLOADED") ?? "").IndexOf("clsCarpoKupUra", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "BVG-03", "clsCarpoKupUra nosi drugi CarpoForm", "netgen/Form2.vb:7424-7456")
                : Qa.P(Ime, "BVG-03", "kup ura", "netgen/Form2.vb:7456", Ispis.Atr(edit, "ONLOADED") ?? ""));
            l.Add(edit != null && edit.Ime.StartsWith("MULTISELECT", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "BVG-04", "Ime forme MULTISELECT brise TotalGenMultiSelectTemp prije otvaranja", "netgen/Form1.vb:4932-4936")
                : Qa.P(Ime, "BVG-04", "MULTISELECT ime", "netgen/Form1.vb:4932", edit == null ? "" : edit.Ime));
            l.Add(Kontrole.A(Kontrole.Nadji(ura, "CarpoForm"), "DEFAULT") == "KnjigaUra.frmKnjigaUraKupInsert.xml"
                ? Qa.Z(Ime, "BVG-05", "Treci CarpoForm je frmKnjigaUraKupInsert", "netgen/Form2.vb:7456")
                : Qa.P(Ime, "BVG-05", "kup insert", "netgen/Form2.vb:7456", ""));
            NgControl gumb = Kontrole.Nadji(odo, "Odobravanje");
            string klik = Kontrole.A(gumb, "ONCLICK") ?? "";
            string on = Ispis.Atr(odo, "ONLOADED") ?? "";
            l.Add(klik.IndexOf("ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok", StringComparison.Ordinal) >= 0
                && on.IndexOf("ASSEMBLYLOAD", StringComparison.OrdinalIgnoreCase) < 0
                && Kontrole.A(Kontrole.Nadji(odo, "CarpoForm"), "DEFAULT") == "KUP_Odobravanje.xml"
                ? Qa.Z(Ime, "BVG-06", "Odobravanje: assembly je na gumbu, ONLOADED je SQL i +close", "netgen/Form2.vb:7424; netgen/Class1.vb:187-204")
                : Qa.P(Ime, "BVG-06", "gumb", "netgen/Form2.vb:7424", klik));
            l.Add(Stablo.Meni(Ime, "BVG-07", "KUPDostKnNGBvg.xml"));
            Ispis.K(l, Ime, xml, "BVG-20", "clsCarpoKupUra i clsCarpoKnjigaUra dijele totalgen, razlikuje se CarpoForm", "netgen/Form2.vb:7456", "carpoform.kup", "KnjigaUra.frmKnjigaUraKupEdit.xml");
            return l;
        }
    }

    public static class MpBlagajnaIspis
    {
        public const string Ime = "MPBlagajna";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPBlagajna.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "MPB-01", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            l.Add(Qa.Z(Ime, "MPB-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgButton cmd5 = Ispis.Gumb(p, 5);
            string n5 = cmd5 == null ? "" : cmd5.Naredba;
            l.Add(n5.Equals("PRINTMASTER", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "MPB-02", "PRINTMASTER je IspisGRIDREPX, ne Button7. Orijentacija se ne cita", "netgen/Form1.vb:4951-4955,5984-6003")
                : Qa.P(Ime, "MPB-02", "PRINTMASTER", "netgen/Form1.vb:4951", n5));
            NgButton cmd4 = Ispis.Gumb(p, 4);
            string n4 = cmd4 == null ? "" : cmd4.Naredba;
            l.Add(n4.StartsWith("MULTISELECT", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "MPB-05", "Ime forme brise temp tablicu prije ispisa dokumenta", "netgen/Form1.vb:4932-4936")
                : Qa.P(Ime, "MPB-05", "MULTISELECT", "netgen/Form1.vb:4932", n4));
            NgForm f = p.NadjiFormu("MULTISELECTISPIS_FORM");
            string on = Ispis.Atr(f, "ONLOADED") ?? "";
            l.Add(f != null && f.Kontrole.Count == 0 && on == ":vrstadokid-mpblagajnagla"
                ? Qa.Z(Ime, "MPB-03", "Prazna forma: ONLOADED odmah, ShowDialog se preskace", "netgen/Form2.vb:2798,4430")
                : Qa.P(Ime, "MPB-03", "prazna forma", "netgen/Form2.vb:2798", on + " n=" + (f == null ? -1 : f.Kontrole.Count)));
            l.Add(on.TrimStart().StartsWith(":", StringComparison.Ordinal) && on.IndexOf("vrstadokid-", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "MPB-04", "vrstadokid- nije forma. PrintRpt trazi template", "netgen/Form2.vb:6882-6883; netgen/Class1.vb:443-486")
                : Qa.P(Ime, "MPB-04", "vrstadokid", "netgen/Class1.vb:443", on));
            string levo = p.Atributi.Ima("PAGEHEADERLEFT") ? (p.Atributi.Vrijednost("PAGEHEADERLEFT") ?? "") : "";
            string centar = p.Atributi.Ima("PAGEHEADERCENTER") ? (p.Atributi.Vrijednost("PAGEHEADERCENTER") ?? "") : null;
            l.Add(levo.IndexOf("@@trgovina", StringComparison.OrdinalIgnoreCase) >= 0 && centar == ""
                ? Qa.Z(Ime, "MPB-06", "Prazan centar i nema v[ ostave centar prazan. Lijevo je @@trgovina", "netgen/Form1.vb:5996-5997")
                : Qa.P(Ime, "MPB-06", "zaglavlje", "netgen/Form1.vb:5996", levo));
            l.Add(Stablo.Meni(Ime, "MPB-07", "MPBlagajna.xml"));
            Ispis.K(l, Ime, xml, "MPB-20", "Spin.Printing.v23 pa v9.1. Port zove v26, ne v23", "netgen/Class1.vb:297-304; Devex_26/Carpo.NetGen/Placeholders.cs:46-52", "spin.printing", "v26");
            return l;
        }
    }

    public static class KarticaIspis
    {
        public const string Ime = "KarticaPartnera";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KarticaPartnera.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KAR-30", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            NgButton cmd3 = Ispis.Gumb(p, 3);
            NgButton cmd4 = Ispis.Gumb(p, 4);
            string n3 = cmd3 == null ? "" : cmd3.Naredba;
            string n4 = cmd4 == null ? "" : cmd4.Naredba;
            l.Add(n3.Equals("PRINTMASTER", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KAR-30", "PRINTMASTER je isti poziv kao PRINTREPXMASTER", "netgen/Form1.vb:4945-4955")
                : Qa.P(Ime, "KAR-30", "PRINTMASTER", "netgen/Form1.vb:4951", n3));
            string ori = p.Atributi.Ima("PAGEORIENTATION") ? (p.Atributi.Vrijednost("PAGEORIENTATION") ?? "") : "";
            l.Add(ori.Equals("LANDSCAPE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KAR-31", "LANDSCAPE stoji na projektu, IspisGRIDREPX ga ne cita", "netgen/Form1.vb:5984-6003")
                : Qa.P(Ime, "KAR-31", "orijentacija", "netgen/Form1.vb:5984", ori));
            l.Add(n4.StartsWith(":repx/", StringComparison.OrdinalIgnoreCase) && n4.IndexOf("@DatumOd", StringComparison.Ordinal) >= 0
                && n4.IndexOf("#CLOSE", StringComparison.OrdinalIgnoreCase) >= 0 && !Ispis.VecImaId(n4)
                ? Qa.Z(Ime, "KAR-32", ":repx/ nosi parametre i #CLOSE. Nema @id=, engine ga doda", "netgen/Form2.vb:4276,4322; netgen/Class1.vb:320-327; netgen/Form2.vb:7033")
                : Qa.P(Ime, "KAR-32", "repx", "netgen/Form2.vb:4276", n4));
            l.Add(Ispis.RepxDijalog(n4.TrimStart(':')) .StartsWith("repx:/", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KAR-33", "Dijalog od :repx/ napravi repx:/ prije PrintRpt", "netgen/Form2.vb:4296-4297")
                : Qa.P(Ime, "KAR-33", "prefiks", "netgen/Form2.vb:4297", ""));
            Ispis.K(l, Ime, xml, "KAR-40", "Ako Spin.Printing padne, Form6 zove rptDevRepx.DoPrint2", "netgen/Form6.vb:134-161; netgen/Class1.vb:606-632", "repx.fallback", "rptDevRepx");
            return l;
        }
    }

    public static class JeziciIspis
    {
        public const string Ime = "JeziciNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "JeziciNG.xml");
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "JEP-01", "Projekt se ucita", "netgen/Form1.vb:3263", g ?? "nema"));
                return l;
            }
            NgButton cmd5 = Ispis.Gumb(p, 5);
            string n5 = cmd5 == null ? "" : cmd5.Naredba;
            l.Add(n5.Equals("PRINTREPXMASTER", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "JEP-01", "PRINTREPXMASTER izlazi u IspisGRIDREPX, ne trazi formu", "netgen/Form1.vb:4945-4948")
                : Qa.P(Ime, "JEP-01", "PRINTREPXMASTER", "netgen/Form1.vb:4945", n5));
            bool nemaCentar = p.Atributi == null || !p.Atributi.Ima("PAGEHEADERCENTER");
            string desno = p.Atributi != null && p.Atributi.Ima("PAGEHEADERRIGHT") ? (p.Atributi.Vrijednost("PAGEHEADERRIGHT") ?? "") : "";
            l.Add(nemaCentar && desno.IndexOf("@@tkosmomi", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "JEP-02", "Nema PAGEHEADERCENTER: centar je naslov do v[, desno je @@tkosmomi", "netgen/Form1.vb:5996-6001")
                : Qa.P(Ime, "JEP-02", "zaglavlje", "netgen/Form1.vb:5996", desno));
            return l;
        }
    }
}
