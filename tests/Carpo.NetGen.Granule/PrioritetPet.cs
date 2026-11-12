using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Pet granula s 10 baza i Biovegom. SQL se ne salje. jupiter2022 se ne otvara.
    /// </summary>
    public static class Intrastat
    {
        public const string Ime = "Intrastat";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "INT-01", root, "definitions", "netgen", "BIOVEGA", "Intrastat.xml");
            if (p == null)
                return l;
            l.Add(p.Naslov == "Intrastat  v[2.4]"
                ? Qa.Z(Ime, "INT-02", "Naslov ima dva razmaka i v[2.4]", "netgen/Form1.vb:3524-3525")
                : Qa.P(Ime, "INT-02", "naslov", "netgen/Form1.vb:3524", p.Naslov));
            l.Add(p.OnLoad.StartsWith("spIntrastat_Query @action = 0", StringComparison.Ordinal)
                ? Qa.Z(Ime, "INT-03", "ONLOAD je query, ne forma", "netgen/Form1.vb:3879-3882")
                : Qa.P(Ime, "INT-03", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@id = @@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "INT-04", "Detail veze @@id, ne natpis stupca", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "INT-04", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(Pet.Attr(p, "SETDEFAULTS").IndexOf("#if exists", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "INT-05", "SETDEFAULTS se reze na #. #if exists je drugi ExecSql", "netgen/Form1.vb:3862-3866")
                : Qa.P(Ime, "INT-05", "setdefaults", "netgen/Form1.vb:3862", ""));
            l.Add(Pet.Gumb(p, 7, "&Ispiši", "repx/rptIntrastat(@id=@@id)")
                ? Qa.Z(Ime, "INT-06", "Ispis je repx s parametrom u zagradi", "netgen/Form1.vb:5215-5239")
                : Qa.P(Ime, "INT-06", "gumb 7", "netgen/Form1.vb:3320", Pet.OpisGumba(p, 7)));
            l.Add(Pet.Gumb(p, 1, "&Unesi", "Unos") && Pet.Gumb(p, 9, "&Filter", "Filter") && Pet.NemaRedni(p, 4) && Pet.NemaRedni(p, 8)
                ? Qa.Z(Ime, "INT-07", "CMD4 i CMD8 nisu napisani. & ostaje u natpisu", "netgen/Form1.vb:3314-3836")
                : Qa.P(Ime, "INT-07", "gumbi", "netgen/Form1.vb:3314", p.Gumbi.Count.ToString()));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "300"
                ? Qa.Z(Ime, "INT-08", "Timeout SQL-a je 300, ne 30", "netgen/Form1.vb:3463; netgen/Class1.vb:99")
                : Qa.P(Ime, "INT-08", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            l.Add(Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "TOTALROWVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "INT-09", "Checkbox stupac i suma su ugaseni", "netgen/Form1.vb:2043-2053")
                : Qa.P(Ime, "INT-09", "checkbox/total", "netgen/Form1.vb:2043", ""));
            NgColumnGroup det = Pet.Grupa(p, "DETAILCOLUMNS");
            NgColumn sifra = det == null ? null : Pet.Kolona(det, "SifraRobe");
            l.Add(det != null && det.ClearColums == "false" && sifra != null && sifra.Atributi.Vrijednost("WIDTH") == "900"
                ? Qa.Z(Ime, "INT-10", "CLEARCOLUMS=false ne sakrije ostale. Sirina SifraRobe je 900/14", "netgen/Form1.vb:7459-7462,7522")
                : Qa.P(Ime, "INT-10", "detail stupci", "netgen/Form1.vb:7522", det == null ? "nema" : det.ClearColums));
            l.Add(Pet.Izgled("INT-11", "Row header, prazni retci i HeaderLines su izgled. Kontrola je DevExpress."));
            l.Add(Qa.Mora(Ime, "INT-20", "Master timeout je 300 i detail salje @@id", "netgen/Class1.vb:99", Pet.Putanja(root, "Intrastat.xml"), "otvori", "", "timeout", "300"));
            return l;
        }
    }

    public static class KntPdvObrasciEu
    {
        public const string Ime = "KntPDVObrasciEU";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string a = Pet.Putanja(root, "KntPDVObrasciEU.xml");
            string b = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntPDVObrasciEU.xml");
            l.Add(Pet.Isti(a, b)
                ? Qa.Z(Ime, "PDV-00", "Agronom kopija je isti sadrzaj", a)
                : Qa.P(Ime, "PDV-00", "kopija", b, "SHA-256"));
            NgProject p = Pet.Ucitaj(l, Ime, "PDV-01", root, "definitions", "netgen", "BIOVEGA", "KntPDVObrasciEU.xml");
            if (p == null)
                return l;
            l.Add(p.Naslov != null && p.Naslov.StartsWith("PDV Obrasci EU 2", StringComparison.Ordinal)
                ? Qa.Z(Ime, "PDV-02", "Naslov je caption, ne NAME", "netgen/Form1.vb:3522-3528")
                : Qa.P(Ime, "PDV-02", "naslov", "netgen/Form1.vb:3522", p.Naslov));
            l.Add(p.OnLoad == ":FILTER_FORM"
                ? Qa.Z(Ime, "PDV-03", "ONLOAD otvara filter prije mastera", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "PDV-03", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.Attr(p, "ONENTERKEY") == "IZMIJENI_FORM"
                ? Qa.Z(Ime, "PDV-04", "Enter otvara IZMIJENI_FORM iako je EnterKeyBehavior None", "netgen/Form1.vb:3364,3419-3421,7021-7023")
                : Qa.P(Ime, "PDV-04", "ONENTERKEY", "netgen/Form1.vb:3364", Pet.Attr(p, "ONENTERKEY")));
            string gumbi = p.Buttons ?? "";
            l.Add(gumbi.IndexOf("ASSEMBLYLOAD(EPoreznaObrasci.Program)", StringComparison.Ordinal) >= 0
                && gumbi.IndexOf("repx/@@Template", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PDV-05", "Sedmi gumb je ASSEMBLYLOAD, sesti je repx/@@Template", "netgen/Form1.vb:3770-3788,4584,5238")
                : Qa.P(Ime, "PDV-05", "BUTTONS", "netgen/Form1.vb:3770", gumbi));
            l.Add(Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "PDV-06", "LANDSCAPE ide u Janus IspisNovi. repx/ ga ne gleda", "netgen/Form1.vb:3458,5930,5215")
                : Qa.P(Ime, "PDV-06", "orijentacija", "netgen/Form1.vb:3458", Pet.Attr(p, "PAGEORIENTATION")));
            l.Add(Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "GROUPBYBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "PDV-07", "Checkbox i group box su ugaseni", "netgen/Form1.vb:2043,3350")
                : Qa.P(Ime, "PDV-07", "checkbox", "netgen/Form1.vb:2043", ""));
            l.Add(Pet.Izgled("PDV-08", "FORECOLOR i BACKCOLOR prazni ne boje stupac. Boja je izgled."));
            l.Add(Qa.Kljuc(Ime, "PDV-20", "Enter otvara IZMIJENI_FORM", "netgen/Form1.vb:7021-7023", a, "otvori", "", "enterForma", "IZMIJENI_FORM"));
            return l;
        }
    }

    public static class MpKntNacinPlac
    {
        public const string Ime = "MPKntNacinPlacAzur";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "NPL-01", root, "definitions", "netgen", "BIOVEGA", "MPKntNacinPlacAzur.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM"
                ? Qa.Z(Ime, "NPL-02", "Filter se otvara prije mastera", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "NPL-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@kntnacinplacid = @@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NPL-03", "Detail veze @@id", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "NPL-03", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(Pet.Attr(p, "TOTALROW") == "FALSE" && !p.Atributi.Ima("TOTALROWVISIBLE")
                ? Qa.Z(Ime, "NPL-04", "TOTALROW se ne cita. Suma ostaje ukljucena", "netgen/Form1.vb:2049-2053")
                : Qa.P(Ime, "NPL-04", "TOTALROW", "netgen/Form1.vb:2049", Pet.Attr(p, "TOTALROW")));
            string b = p.Buttons ?? "";
            l.Add(b.IndexOf("Parametri:FILTERFORM", StringComparison.Ordinal) >= 0
                && b.IndexOf("Provizija:PROVIZIJA", StringComparison.Ordinal) >= 0
                && p.Gumbi.Count == 0
                ? Qa.Z(Ime, "NPL-05", "BUTTONS prepisuje pet gumba. Prazni CMD ne prave gumb", "netgen/Form1.vb:3770-3788")
                : Qa.P(Ime, "NPL-05", "BUTTONS", "netgen/Form1.vb:3770", b));
            NgForm f = p.NadjiFormu("FILTERFORM");
            NgControl svi = f == null ? null : Pet.Kontrola(f, "svi");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE" && svi != null && svi.Atributi.Vrijednost("ALLOWEMPTY") == "FOLSE"
                ? Qa.Z(Ime, "NPL-06", "FOLSE nije true. Prazan red se ne ubaci", "netgen/Form2.vb:2402-2404")
                : Qa.P(Ime, "NPL-06", "ALLOWEMPTY", "netgen/Form2.vb:2402", svi == null ? "" : svi.Atributi.Vrijednost("ALLOWEMPTY")));
            NgForm dodaj = p.NadjiFormu("DODAJFORM");
            l.Add(dodaj != null && Pet.AttrF(dodaj, "FOCUS") == "Naziv"
                ? Qa.Z(Ime, "NPL-07", "FOCUS=Naziv aktivira tu kontrolu nakon loada", "netgen/Form2.vb:1053,2792-2794")
                : Qa.P(Ime, "NPL-07", "FOCUS", "netgen/Form2.vb:1053", dodaj == null ? "" : Pet.AttrF(dodaj, "FOCUS")));
            l.Add(Pet.Attr(p, "SETDEFAULTS").IndexOf("fnTkoSmoMi", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NPL-08", "SETDEFAULTS je jedan select, nema #", "netgen/Form1.vb:3862-3866")
                : Qa.P(Ime, "NPL-08", "setdefaults", "netgen/Form1.vb:3862", ""));
            l.Add(Qa.Kljuc(Ime, "NPL-20", "Suma mastera ostaje ukljucena", "netgen/Form1.vb:2049-2053", Pet.Putanja(root, "MPKntNacinPlacAzur.xml"), "otvori", "", "totalRow", "true"));
            return l;
        }
    }

    public static class KntOpzStat
    {
        public const string Ime = "KntOPZ-STAT-1";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string a = Pet.Putanja(root, "KntOPZ-STAT-1.xml");
            string b = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntOPZ-STAT-1.xml");
            l.Add(Pet.Isti(a, b)
                ? Qa.Z(Ime, "OPZ-00", "Agronom kopija je isti sadrzaj", a)
                : Qa.P(Ime, "OPZ-00", "kopija", b, "SHA-256"));
            NgProject p = Pet.Ucitaj(l, Ime, "OPZ-01", root, "definitions", "netgen", "BIOVEGA", "KntOPZ-STAT-1.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":UPIT"
                ? Qa.Z(Ime, "OPZ-02", "ONLOAD otvara UPIT", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "OPZ-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.Gumb(p, 4, "Eporezna", "OpzStatExport.Start(@@datumDoParam ; @@datdospparam)")
                ? Qa.Z(Ime, "OPZ-03", "Eporezna je ime forme s tockom, ne ASSEMBLYLOAD", "netgen/Form2.vb:4407-4418; netgen/Form1.vb:5369-5370")
                : Qa.P(Ime, "OPZ-03", "gumb 4", "netgen/Form1.vb:3329", Pet.OpisGumba(p, 4)));
            l.Add(Pet.Attr(p, "FILLEXPLORER") == "TRUE"
                ? Qa.Z(Ime, "OPZ-04", "FILLEXPLORER na projektu se ne cita. Puni ga forma UPIT", "netgen/Form2.vb:1092; netgen/Form1.vb:5445")
                : Qa.P(Ime, "OPZ-04", "fillexplorer", "netgen/Form1.vb:5445", Pet.Attr(p, "FILLEXPLORER")));
            l.Add(Pet.Gumb(p, 3, "Ispis", "repx/rptOpzStat1(@DatumDo=@@DatumDoParam, @DatDosp=@@DatDospParam)")
                ? Qa.Z(Ime, "OPZ-05", "Ispis je repx s dva parametra", "netgen/Form1.vb:5215-5239")
                : Qa.P(Ime, "OPZ-05", "ispis", "netgen/Form1.vb:5215", Pet.OpisGumba(p, 3)));
            NgForm upit = p.NadjiFormu("UPIT");
            NgForm ispravi = p.NadjiFormu("ISPRAVI");
            l.Add(upit != null && Pet.AttrF(upit, "FILLEXPLORER") == "TRUE"
                && ispravi != null && Pet.AttrF(ispravi, "FILLEXPLORER") == "treeandrefreshmaster"
                ? Qa.Z(Ime, "OPZ-06", "UPIT puni master. ISPRAVI je treeandrefreshmaster", "netgen/Form1.vb:5445,5478-5490")
                : Qa.P(Ime, "OPZ-06", "forme", "netgen/Form1.vb:5445", ""));
            NgColumnGroup master = Pet.Grupa(p, "MASTERCOLUMNS");
            NgColumn fmt = master == null ? null : Pet.Kolona(master, "FormatId");
            l.Add(master != null && master.ClearColums == "TRUE" && fmt != null
                && fmt.Atributi.Vrijednost("POSITION") == "-1" && fmt.Atributi.Vrijednost("VISIBLE") == "FALSE"
                && fmt.Atributi.Vrijednost("AGREGATEFUNCTION") == "0"
                ? Qa.Z(Ime, "OPZ-07", "POSITION -1 prepisuje red. Agregat 0 nije prazan. Stupac je skriven", "netgen/Form1.vb:7516-7526,7506")
                : Qa.P(Ime, "OPZ-07", "FormatId", "netgen/Form1.vb:7526", fmt == null ? "nema" : fmt.Atributi.Vrijednost("POSITION")));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "240"
                ? Qa.Z(Ime, "OPZ-08", "Timeout je 240", "netgen/Form1.vb:3463")
                : Qa.P(Ime, "OPZ-08", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            l.Add(Pet.Izgled("OPZ-09", "HEADERALIGNMENT=2 je poravnanje zaglavlja, izgled."));
            l.Add(Qa.Kljuc(Ime, "OPZ-20", "Eporezna ne otvara dijalog i ne ucitava DLL", "netgen/Form1.vb:5369-5370", a, "gumb", "Eporezna", "dll", "ne"));
            return l;
        }
    }

    public static class MpRacNeFisk
    {
        public const string Ime = "MPRACNEFISK";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "NEF-01", root, "definitions", "netgen", "BIOVEGA", "MPRACNEFISK.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FORM1"
                ? Qa.Z(Ime, "NEF-02", "Parametri se otvaraju prije mastera", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "NEF-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("spmpPregRacNG @action=2", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NEF-03", "DETAILGRID s razmakom ispred = i dalje veze @@id", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "NEF-03", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(Pet.Gumb(p, 3, "FISKALIZACIJA ", " FISKALIZACIJA(@@id)#:VRIJEMEJIR#REFRESHMASTER")
                ? Qa.Z(Ime, "NEF-04", "Natpis ima razmak. REFRESHMASTER nije REFRESHMASTERGRID", "netgen/Form1.vb:3316,4558-4570,5519")
                : Qa.P(Ime, "NEF-04", "gumb 3", "netgen/Form1.vb:3316", Pet.OpisGumba(p, 3)));
            l.Add(p.MasterDblClick == "PROMET" && p.NadjiFormu("PROMET") == null
                ? Qa.Z(Ime, "NEF-05", "PROMET nije u XML-u. Dvostruki klik ne otvara dijalog", "netgen/Form1.vb:5369-5370")
                : Qa.P(Ime, "NEF-05", "dblclick", "netgen/Form1.vb:3389", p.MasterDblClick));
            l.Add(Pet.Attr(p, "REPORT").IndexOf(".clsReport", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "NEF-06", "REPORT s clsReport ide u PrintRpt, ne u formu", "netgen/Form1.vb:5215")
                : Qa.P(Ime, "NEF-06", "report", "netgen/Form1.vb:5215", Pet.Attr(p, "REPORT")));
            NgForm f = p.NadjiFormu("FORM1");
            NgControl dat = f == null ? null : Pet.Kontrola(f, "datum");
            NgControl chk = f == null ? null : Pet.Kontrola(f, "dadatum");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE"
                && dat != null && dat.Grana == NgGrana.Date && dat.Atributi.Vrijednost("ALLOWEMPTY") == "true" && !dat.Atributi.Ima("SHOWCHECKBOX")
                && chk != null && chk.Grana == NgGrana.Check
                ? Qa.Z(Ime, "NEF-07", "ALLOWEMPTY na datumu nije SHOWCHECKBOX. Check salje 1/0", "netgen/Form2.vb:2276")
                : Qa.P(Ime, "NEF-07", "kontrole", "netgen/Form2.vb:2276", dat == null ? "" : dat.Grana.ToString()));
            l.Add(f != null && f.OnLoad.IndexOf("@@datumod", StringComparison.Ordinal) >= 0 && Pet.Kontrola(f, "datumod") == null
                ? Qa.Z(Ime, "NEF-08", "ONLOAD trazi @@datumod, kontrola se zove datum. @@ koji ostane se ne salje", "netgen/Class1.vb:113-118")
                : Qa.P(Ime, "NEF-08", "onload forme", "netgen/Class1.vb:113", f == null ? "" : f.OnLoad));
            NgForm vrijeme = p.NadjiFormu("VRIJEMEJIR");
            l.Add(vrijeme != null && Pet.AttrF(vrijeme, "FILLEXPLORER") == ""
                ? Qa.Z(Ime, "NEF-09", "Prazan FILLEXPLORER izvrsi ONSUBMIT i ponovi zadnji master", "netgen/Form1.vb:5509-5519")
                : Qa.P(Ime, "NEF-09", "VRIJEMEJIR", "netgen/Form1.vb:5509", ""));
            l.Add(!p.Atributi.Ima("CHECKBOXVISIBLE") && !p.Atributi.Ima("TOTALROWVISIBLE")
                ? Qa.Z(Ime, "NEF-10", "Checkbox stupac i suma ostaju ukljuceni", "netgen/Form1.vb:2043-2053")
                : Qa.P(Ime, "NEF-10", "default grida", "netgen/Form1.vb:2043", ""));
            l.Add(Qa.Mora(Ime, "NEF-20", "Treci segment osvjezi zadnji master iako rijec nije REFRESHMASTERGRID", "netgen/Form1.vb:4570,5519", Pet.Putanja(root, "MPRACNEFISK.xml"), "gumb", "FISKALIZACIJA ", "refreshMaster", "true"));
            return l;
        }
    }

    internal static class Pet
    {
        public static NgProject Ucitaj(List<Stavka> l, string ime, string id, string root, params string[] dijelovi)
        {
            string put = Path.Combine(dijelovi);
            put = Path.Combine(root, put);
            if (!File.Exists(put))
            {
                l.Add(Qa.P(ime, id, "XML postoji", put, "nema datoteke"));
                return null;
            }
            string greska;
            NgProject p = Qa.Ucitaj(put, out greska);
            if (p == null)
            {
                l.Add(Qa.P(ime, id, "Projekt se ucita", "netgen/Form1.vb:3263", greska));
                return null;
            }
            l.Add(Qa.Z(ime, id, "Projekt se ucita", "netgen/Form1.vb:3263"));
            return p;
        }

        public static string Putanja(string root, string fajl)
        {
            return Path.Combine(root, "definitions", "netgen", "BIOVEGA", fajl);
        }

        public static string Attr(NgProject p, string ime)
        {
            return p.Atributi.Vrijednost(ime) ?? "";
        }

        public static string AttrF(NgForm f, string ime)
        {
            return f.Atributi.Vrijednost(ime) ?? "";
        }

        public static bool Gumb(NgProject p, int redni, string natpis, string naredba)
        {
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == redni)
                    return (natpis == null || p.Gumbi[i].Natpis == natpis) && p.Gumbi[i].Naredba == naredba;
            }
            return false;
        }

        public static bool GumbNaredba(NgProject p, int redni, string naredba)
        {
            return Gumb(p, redni, null, naredba);
        }

        public static bool NemaRedni(NgProject p, int redni)
        {
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == redni)
                    return false;
            }
            return true;
        }

        public static string OpisGumba(NgProject p, int redni)
        {
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == redni)
                    return p.Gumbi[i].Natpis + "|" + p.Gumbi[i].Naredba;
            }
            return "nema";
        }

        public static NgColumnGroup Grupa(NgProject p, string ime)
        {
            for (int i = 0; i < p.GrupeKolona.Count; i++)
            {
                if (string.Equals(p.GrupeKolona[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return p.GrupeKolona[i];
            }
            return null;
        }

        public static NgColumn Kolona(NgColumnGroup g, string ime)
        {
            for (int i = 0; i < g.Kolone.Count; i++)
            {
                if (string.Equals(g.Kolone[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return g.Kolone[i];
            }
            return null;
        }

        public static NgControl Kontrola(NgForm f, string ime)
        {
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (f.Kontrole[i].Name == ime)
                    return f.Kontrole[i];
            }
            return null;
        }

        public static Stavka Izgled(string id, string opis)
        {
            return Qa.Z("izgled", id, "izgled – Carpo. " + opis, "docs/razlike-netgen-carpo.md R59");
        }

        public static bool Isti(string a, string b)
        {
            if (!File.Exists(a) || !File.Exists(b))
                return false;
            using (var sha = SHA256.Create())
            {
                byte[] ha = sha.ComputeHash(File.ReadAllBytes(a));
                byte[] hb = sha.ComputeHash(File.ReadAllBytes(b));
                if (ha.Length != hb.Length)
                    return false;
                for (int i = 0; i < ha.Length; i++)
                {
                    if (ha[i] != hb[i])
                        return false;
                }
                return true;
            }
        }
    }
}
