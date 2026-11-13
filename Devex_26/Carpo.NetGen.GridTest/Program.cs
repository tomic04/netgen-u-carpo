using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Poziv;
using Carpo.NetGen.Print;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.TestHooks;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.GridTest
{
    internal static class Program
    {
        private static int fail;

        private static int Main(string[] args)
        {
            SqlPreskoci();
            IntrastatPreskok();
            RucniStart();
            Odabir();
            GumbiButtons();
            Jez57();
            Pregled();
            AssemblyLoad();
            Fiskal();
            Include();
            Ispis();
            Zamjene();
            OznakaBezBaze();
            JeziciBezBaze();
            Kolone();
            PisanjeIServer();
            StabloTestovi();
            KomandeTestovi();
            ZastitaTestovi();
            DijalogKlikTest();
            DijalogRupe();
            ComboDijalogTest();
            OnClickTest();
            GridDijalogTest();
            NextFormTest();
            GridKljucevi();
            OstaloKljucevi();
            StabloKljucevi();
            if (Environment.GetEnvironmentVariable("CARPO_SPIN") == "1")
            {
                Spin();
                SpinDetalj();
                SpinStabloFill();
            }
            else
                Console.WriteLine("PRESKOCENO spin (nema CARPO_SPIN=1)");
            Console.WriteLine("FAIL=" + fail.ToString());
            return fail == 0 ? 0 : 1;
        }

        private static void Pregled()
        {
            T("onload2-disable", Carpo.NetGen.Komande.OnLoad2.Akcija("disable") == "disable"
                && Carpo.NetGen.Komande.OnLoad2.Akcija("enable") == "enable"
                && Carpo.NetGen.Komande.OnLoad2.Akcija("") == ""
                && Carpo.NetGen.Komande.OnLoad2.Akcija("Dodaj") == "natpis", "");
            var statusi = Ucitaj("Statusi.xml");
            Carpo.NetGen.Model.NgForm popuni = null;
            if (statusi != null)
            {
                for (int i = 0; i < statusi.Forme.Count; i++)
                {
                    if (string.Equals(statusi.Forme[i].Ime, "POPUNI_FORM", StringComparison.OrdinalIgnoreCase))
                        popuni = statusi.Forme[i];
                }
            }
            string sql = Carpo.NetGen.Komande.DijalogTok.SqlNakonOk(popuni);
            T("fillexplorer-statusi", sql.IndexOf("spStatusiQuery", StringComparison.OrdinalIgnoreCase) >= 0, sql);
            Carpo.NetGen.Model.NgForm onlydet = null;
            if (statusi != null)
            {
                for (int i = 0; i < statusi.Forme.Count; i++)
                {
                    if (string.Equals(statusi.Forme[i].Ime, "INSERTOPFORM", StringComparison.OrdinalIgnoreCase))
                        onlydet = statusi.Forme[i];
                }
            }
            T("onlydet-nije-master", Carpo.NetGen.Komande.DijalogTok.SqlNakonOk(onlydet).Length == 0, Carpo.NetGen.Komande.DijalogTok.SqlNakonOk(onlydet));
            T("onlydet-detalj", Carpo.NetGen.Komande.DijalogTok.OsvjeziDetalj(onlydet) && !Carpo.NetGen.Komande.DijalogTok.OsvjeziDetalj(popuni), "");
            var rute = Ucitaj("BSRute.xml");
            Carpo.NetGen.Model.NgForm stablo = null;
            if (rute != null)
            {
                for (int i = 0; i < rute.Forme.Count; i++)
                {
                    if (string.Equals(rute.Forme[i].Ime, "REFRESHTREEFORM", StringComparison.OrdinalIgnoreCase))
                        stablo = rute.Forme[i];
                }
            }
            T("onlytree-nije-master", stablo != null && Carpo.NetGen.Komande.DijalogTok.SqlNakonOk(stablo).Length == 0 && Carpo.NetGen.Komande.DijalogTok.OsvjeziStablo(stablo) && !Carpo.NetGen.Komande.DijalogTok.OsvjeziDetalj(stablo), "");
            var jezici = Ucitaj("JeziciNG.xml");
            T("obicna-unesi", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "Unesi") == "forma", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "Unesi"));
            T("obicna-refreshmaster", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "refreshmaster") == "osvjezi", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "refreshmaster"));
            T("obicna-ispis", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "PRINTREPXMASTER") == "ispis", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "PRINTREPXMASTER"));
            T("obicna-grid", Carpo.NetGen.Komande.GumbTok.Akcija(jezici, "REFRESHMASTERGRID") == "osvjezi", "");
            var cbIme = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            cbIme["ime"] = "";
            T("isempty-prazno", Carpo.NetGen.Print.IspisZaglavlje.Desno("ISEMPTY(@@ime)#vise#jedno: @@ime", cbIme) == "vise", Carpo.NetGen.Print.IspisZaglavlje.Desno("ISEMPTY(@@ime)#vise#jedno: @@ime", cbIme));
            cbIme["ime"] = "Glavno";
            T("isempty-vrijednost", Carpo.NetGen.Print.IspisZaglavlje.Desno("ISEMPTY(@@ime)#vise#jedno: @@ime", cbIme) == "jedno: Glavno", Carpo.NetGen.Print.IspisZaglavlje.Desno("ISEMPTY(@@ime)#vise#jedno: @@ime", cbIme));
            T("podnozje-isempty", Carpo.NetGen.Print.IspisZaglavlje.Podnozje("ISEMPTY(@@ime)#lijevo#desno: @@ime", cbIme) == "desno: Glavno", Carpo.NetGen.Print.IspisZaglavlje.Podnozje("ISEMPTY(@@ime)#lijevo#desno: @@ime", cbIme));
            T("prevod-hrv", Carpo.NetGen.Print.Prevod.Tekst("Jezici", "hrv") == "Jezici" && Carpo.NetGen.Print.Prevod.Tekst("Jezici", "") == "Jezici", "");
            T("prevod-bez-dll", Carpo.NetGen.Print.Prevod.Tekst("Jezici", "eng") == "Jezici", Carpo.NetGen.Print.Prevod.Tekst("Jezici", "eng"));
            Carpo.NetGen.Print.Prevod.Vanjski = delegate(string s) { return s == "Jezici" ? "Languages" : s; };
            try
            {
                T("prevod-eng", Carpo.NetGen.Print.IspisZaglavlje.Desno("Jezici", cbIme, "eng") == "Languages", Carpo.NetGen.Print.IspisZaglavlje.Desno("Jezici", cbIme, "eng"));
            }
            finally
            {
                Carpo.NetGen.Print.Prevod.Vanjski = null;
            }
        }

        private static void Ispis()
        {
            T("print-vrste", Carpo.NetGen.Print.PrintBridge.Vrsta("PRINTREPXMASTER", false) == "master"
                && Carpo.NetGen.Print.PrintBridge.Vrsta("PRINTDET", false) == "detail"
                && Carpo.NetGen.Print.PrintBridge.Vrsta("PRINTREPXGRID", true) == "lodani"
                && Carpo.NetGen.Print.PrintBridge.Vrsta("PRINTGRID", false) == "master", "");
            T("print-landscape", Carpo.NetGen.Print.PrintBridge.JeLandscape("LANDSCAPE")
                && !Carpo.NetGen.Print.PrintBridge.JeLandscape(""), "");
            T("print-centar", Carpo.NetGen.Print.PrintBridge.Centar("", "Stanje v[1]", false) == "Stanje"
                && Carpo.NetGen.Print.PrintBridge.Centar("", "Cijeli naslov", true) == "Cijeli naslov", "");
            var stroj = new LazniSpin();
            object masterGrid = new object();
            object detailGrid = new object();
            Carpo.NetGen.Print.PrintBridge.Stroj = stroj;
            try
            {
                bool ok = Carpo.NetGen.Print.PrintBridge.Ispisi("PRINTREPXMASTER", false, "7", "3", "conn", "login", "select 1", "@g=1", "@svi=1", "Lijevo", "", "Desno", "Naslov v[2]", masterGrid, "LANDSCAPE", null);
                T("print-master", ok && LazniSpin.Metoda == "PrintLink" && LazniSpin.Kontrola == masterGrid
                    && LazniSpin.Landscape && LazniSpin.Caption == "Naslov" && LazniSpin.Header == "Lijevo" && LazniSpin.Sub == "Desno"
                    && LazniSpin.Konekcija == "conn", LazniSpin.Metoda + " " + LazniSpin.Caption);
                ok = Carpo.NetGen.Print.PrintBridge.Ispisi("PRINTDET", false, "7", "3", "conn", "login", "select 1", "@g=1", "@svi=1", "L", "C", "D", "Naslov", detailGrid, "", null);
                T("print-detail", ok && LazniSpin.Metoda == "PrintLink" && LazniSpin.Kontrola == detailGrid && !LazniSpin.Landscape, LazniSpin.Metoda);
                ok = Carpo.NetGen.Print.PrintBridge.Ispisi("PRINTREPXGRID", true, "7", "3", "conn", "login", "select 1", "@g=1", "@svi=1", "", "", "", "Dijalog", detailGrid, "", null);
                T("print-lodani", ok && LazniSpin.Metoda == "PrintLink" && LazniSpin.Caption == "Dijalog" && LazniSpin.Kontrola == detailGrid, LazniSpin.Caption);
                ok = Carpo.NetGen.Print.PrintBridge.Ispisi(":repx/rptKartica", false, "7", "3", "conn", "login", "", "", "@id=7", "", "", "", "", null, "", null);
                T("print-repx", ok && LazniSpin.Metoda == "Print" && LazniSpin.Predlozak == "rptKartica" && LazniSpin.Parametri == "@id=7" && LazniSpin.Tko == "Carpo", LazniSpin.Predlozak);
                LazniSpin.Baci = true;
                Carpo.NetGen.Print.PrintBridge.Repx = stroj;
                ok = Carpo.NetGen.Print.PrintBridge.Ispisi(":repx/rptKartica", false, "9", "3", "conn", "login", "", "", "@id=9", "", "", "", "", null, "", null);
                T("print-do2", ok && LazniSpin.Metoda == "DoPrint2" && stroj.isnetgen == 1
                    && LazniSpin.Predlozak == "rptKartica" && LazniSpin.Id == "9"
                    && LazniSpin.Putanja.EndsWith("\\SharedDLLs\\repx", StringComparison.Ordinal)
                    && LazniSpin.Parametri == "@id=9", LazniSpin.Metoda + " " + LazniSpin.Putanja);
            }
            finally
            {
                LazniSpin.Baci = false;
                Carpo.NetGen.Print.PrintBridge.Stroj = null;
                Carpo.NetGen.Print.PrintBridge.Repx = null;
            }
            bool bacio = false;
            try
            {
                Carpo.NetGen.Print.PrintBridge.Ispisi("PRINTMASTER", false, "1", "2", "c", "l", "", "", "", "", "", "", "", null, "", null);
            }
            catch
            {
                bacio = true;
            }
            T("print-nema-dll", bacio == false, "");
            var word = new LazniWord();
            Carpo.NetGen.Print.WordDot.Aplikacija = word;
            try
            {
                bool w = Carpo.NetGen.Print.WordDot.Otvori("WORDDOT1C:\\dot\\racun.dot", "Server=localhost", new[] { "4" });
                T("word-dot", w && word.Visible && word.Dodan == "C:\\dot\\racun.dot" && word.Makro == "Pokreni" && word.Id == "4", word.Dodan);
            }
            finally
            {
                Carpo.NetGen.Print.WordDot.Aplikacija = null;
            }
        }

        private static void GumbiButtons()
        {
            var cmd = new List<NgButton>();
            cmd.Add(new NgButton(6, "Ispis", "repx/rptA"));
            cmd.Add(new NgButton(7, "Report Dob", "repx/rptB"));
            cmd.Add(new NgButton(8, "Report Kup", "repx/rptC"));
            string buttons = "Dodaj:DODAJ_FORM#Izmjeni:IZMJENI_FORM#Obrisi:OBRISI_FORM#Zakljuci:ZAKLJUCI_FORM#Proknjizi:PROKNJIZI_FORM";
            var s = Carpo.NetGen.Shell.GumbiNiz.Slozi(cmd, buttons);
            bool obrisi = false;
            bool repx = false;
            bool dob = false;
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].Redni == 3 && s[i].Naredba == "OBRISI_FORM")
                    obrisi = true;
                if (s[i].Redni == 6 && s[i].Naredba == "repx/rptA")
                    repx = true;
                if (s[i].Redni == 7 && s[i].Naredba == "repx/rptB")
                    dob = true;
            }
            T("buttons-ostaju-cmd", s.Count == 8 && obrisi && repx && dob, s.Count.ToString());
            var sedmi = Carpo.NetGen.Shell.GumbiNiz.Slozi(cmd, "a:A#b:B#c:C#d:D#e:E#f:F#g:G");
            bool cmd7 = false;
            for (int i = 0; i < sedmi.Count; i++)
            {
                if (sedmi[i].Redni == 7 && sedmi[i].Naredba == "repx/rptB")
                    cmd7 = true;
            }
            T("buttons-sedmi-ne-brise-cmd7", cmd7, "");
            var prazan7 = Carpo.NetGen.Shell.GumbiNiz.Slozi(new List<NgButton>(), "a:A#b:B#c:C#d:D#e:E#f:F#g:G");
            bool popunjen = false;
            for (int i = 0; i < prazan7.Count; i++)
            {
                if (prazan7[i].Redni == 7 && prazan7[i].Naredba == "G")
                    popunjen = true;
            }
            T("buttons-sedmi-prazan", popunjen, "");
            string put = Path.Combine("definitions", "netgen", "Agronom granule", "KntKompVerizne.xml");
            if (File.Exists(put))
            {
                var p = NetGenXmlReader.Ucitaj(put).Projekt;
                var zivi = Carpo.NetGen.Shell.GumbiNiz.Slozi(p.Gumbi, p.Buttons);
                bool ima = false;
                bool ispis = false;
                for (int i = 0; i < zivi.Count; i++)
                {
                    if (zivi[i].Naredba == "OBRISI_FORM")
                        ima = true;
                    if (zivi[i].Naredba != null && zivi[i].Naredba.IndexOf("repx/", StringComparison.OrdinalIgnoreCase) >= 0)
                        ispis = true;
                }
                T("buttons-verizne", ima && ispis, zivi.Count.ToString());
            }
        }

        private static void Include()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ng-inc-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            Carpo.NetGen.Xml.IncludeXml.Finder = null;
            try
            {
                string dijete = Path.Combine(dir, "dijete.xml");
                File.WriteAllText(dijete, "<PROJECT><IZTUDEG NAME=\"IZTUDEG\" CAPTION=\"iz\"></IZTUDEG></PROJECT>");
                string host = Path.Combine(dir, "host.xml");
                File.WriteAllText(host, "<PROJECT><MAINFORM NAME=\"MAINFORM\" INCLUDE=\"dijete.xml\" CAPTION=\"h\"></MAINFORM></PROJECT>");
                var p = NetGenXmlReader.Ucitaj(host).Projekt;
                bool ima = false;
                bool ostao = false;
                if (p != null)
                {
                    for (int i = 0; i < p.Forme.Count; i++)
                    {
                        if (string.Equals(p.Forme[i].Ime, "IZTUDEG", StringComparison.OrdinalIgnoreCase))
                            ima = true;
                        if (string.Equals(p.Forme[i].Ime, "MAINFORM", StringComparison.OrdinalIgnoreCase))
                            ostao = true;
                    }
                }
                T("include-ucitaj-ne-miesa", !ima && ostao && p.Forme.Count == 1, ima ? "ima" : "nema");
                var doc = new System.Xml.XmlDocument();
                doc.XmlResolver = null;
                doc.Load(host);
                Carpo.NetGen.Xml.IncludeXml.SpojiFormu(doc, host, "MAINFORM", "");
                T("include-dijete", doc.GetElementsByTagName("IZTUDEG").Count == 1, doc.GetElementsByTagName("IZTUDEG").Count.ToString());
                var docGlobal = new System.Xml.XmlDocument();
                docGlobal.XmlResolver = null;
                docGlobal.Load(host);
                Carpo.NetGen.Xml.IncludeXml.SpojiFormu(docGlobal, host, "MAINFORM", "nema-te-datoteke.xml");
                T("include-global-ne-formu", docGlobal.GetElementsByTagName("IZTUDEG").Count == 0, "");
                string prazan = Path.Combine(dir, "prazan.xml");
                File.WriteAllText(prazan, "<PROJECT><HOST NAME=\"HOST\" INCLUDE=\"nema-te-datoteke.xml\"></HOST></PROJECT>");
                var p2 = NetGenXmlReader.Ucitaj(prazan).Projekt;
                bool hostOstao = false;
                int tudjih = 0;
                if (p2 != null)
                {
                    for (int i = 0; i < p2.Forme.Count; i++)
                    {
                        if (string.Equals(p2.Forme[i].Ime, "HOST", StringComparison.OrdinalIgnoreCase))
                            hostOstao = true;
                        else
                            tudjih++;
                    }
                }
                T("include-nema", hostOstao && tudjih == 0, tudjih.ToString());
                string disk = Path.Combine(dir, "s-diska.xml");
                File.WriteAllText(disk, "<PROJECT><SDISKA NAME=\"SDISKA\"></SDISKA></PROJECT>");
                string dom = Path.Combine(dir, "dom.xml");
                File.WriteAllText(dom, "<PROJECT><DOM NAME=\"DOM\" INCLUDE=\"s-diska.xml\"></DOM></PROJECT>");
                Carpo.NetGen.Xml.IncludeXml.Finder = delegate(string put)
                {
                    var d = new System.Xml.XmlDocument();
                    d.LoadXml("<PROJECT><IZFINDERA NAME=\"IZFINDERA\"></IZFINDERA></PROJECT>");
                    return d;
                };
                var docF = new System.Xml.XmlDocument();
                docF.XmlResolver = null;
                docF.Load(dom);
                Carpo.NetGen.Xml.IncludeXml.SpojiFormu(docF, dom, "DOM", "");
                T("include-finder", docF.GetElementsByTagName("IZFINDERA").Count == 1
                    && docF.GetElementsByTagName("SDISKA").Count == 0, "");
            }
            finally
            {
                Carpo.NetGen.Xml.IncludeXml.Finder = null;
                try { Directory.Delete(dir, true); } catch { }
            }
        }

        private static void Fiskal()
        {
            T("fiskal-raverus", Carpo.NetGen.Poziv.FiskalPoziv.Sklop == "Raverus.FiskalizacijaDEV.COM"
                && Carpo.NetGen.Poziv.FiskalPoziv.Klasa == "Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav"
                && Carpo.NetGen.Poziv.FiskalPoziv.ZastitniKod == "ZastitniKodIzracunCertifikatDatoteka"
                && Carpo.NetGen.Poziv.FiskalPoziv.PosaljiSoap == "PosaljiSoapPorukuCertifikatDatoteka"
                && Carpo.NetGen.Poziv.FiskalPoziv.DohvatiJir == "DohvatiJir", "");
            T("fiskal-slo", Carpo.NetGen.Poziv.FiskalPoziv.JeSlovenija("https://example.fu.gov.si/x")
                && !Carpo.NetGen.Poziv.FiskalPoziv.JeSlovenija("https://cis.example/lokalno")
                && Carpo.NetGen.Poziv.FiskalPoziv.SloRacun == "SLORacun", "");
            var i = Carpo.NetGen.Poziv.FiskalPoziv.Ucitaj("");
            T("fiskal-nema-slanja", i.Poslano == false, i.Greska);
            var cis = new LazniCis();
            Carpo.NetGen.Poziv.FiskalPoziv.Cis = cis;
            try
            {
                var soap = Carpo.NetGen.Poziv.FiskalPoziv.Posalji("<Racun/>", "C:\\cert\\test.pfx", "tajna", "http://127.0.0.1/fiskal-test", "1000", "", "");
                T("fiskal-soap", soap.Poslano && soap.Jir == "JIR-TEST" && cis.CisUrl == "http://127.0.0.1/fiskal-test"
                    && cis.Xml == "<Racun/>" && cis.Pfx.EndsWith("test.pfx", StringComparison.Ordinal)
                    && soap.Url.IndexOf("porezna", StringComparison.OrdinalIgnoreCase) < 0, soap.Jir + " " + cis.CisUrl);
                cis.Baci = true;
                var greska = Carpo.NetGen.Poziv.FiskalPoziv.Posalji("<Racun/>", "C:\\cert\\test.pfx", "tajna", "http://127.0.0.1/fiskal-test", "1000", "", "");
                T("fiskal-greska", !greska.Poslano && greska.Sifra == "55555" && greska.Greska == "opis-cis", greska.Sifra + " " + greska.Greska);
                int prije = cis.Poziva;
                var blok = Carpo.NetGen.Poziv.FiskalPoziv.Posalji("<Racun/>", "C:\\cert\\test.pfx", "tajna", "https://cis.porezna-uprava.hr:8449/FiskalizacijaService", "", "", "");
                T("fiskal-nema-porezne", !blok.Poslano && cis.Poziva == prije, blok.Greska);
            }
            finally
            {
                Carpo.NetGen.Poziv.FiskalPoziv.Cis = null;
            }
            T("param-radio", Carpo.NetGen.Komande.ParametriSvi.Radio("r1", true) == "@r1=1,", "");
            T("param-search", Carpo.NetGen.Komande.ParametriSvi.Search("roba", "0", "Kava, mljevena") == "@roba=null,@searchboxroba='Kava  mljevena',", Carpo.NetGen.Komande.ParametriSvi.Search("roba", "0", "Kava, mljevena"));
            T("param-search-id", Carpo.NetGen.Komande.ParametriSvi.Search("roba", "15", "Kava") == "@roba=15,@searchboxroba='Kava',", "");
            T("param-grid", Carpo.NetGen.Komande.ParametriSvi.Grid("stavke", "8", "8~9", "", 2) == "@stavke=8,@arrayofblueidsstavke='~8~9~',@nizplavihajdijevastavke='~8~9~',@nizoznacenihajdijevastavke='',@rowcountstavke=2,", Carpo.NetGen.Komande.ParametriSvi.Grid("stavke", "8", "8~9", "", 2));
            var sakrij = Carpo.NetGen.Komande.StupciNaredba.Primijeni("HIDECOLUMNS(mastergrid,oznaka)");
            bool oznaka = true;
            bool masterStupac = true;
            bool nasaoO = sakrij.TryGetValue("oznaka", out oznaka);
            bool nasaoM = sakrij.TryGetValue("MASTERGRID", out masterStupac);
            T("stupci-hide", nasaoO && nasaoM && oznaka == false && masterStupac == false, sakrij.Count.ToString());
            var pokazi = Carpo.NetGen.Komande.StupciNaredba.Primijeni("SHOWCOLUMNS(mastergrid,oznaka)");
            bool vidljiv = false;
            pokazi.TryGetValue("oznaka", out vidljiv);
            T("stupci-show", vidljiv, "");
            T("stupci-nije-master", Carpo.NetGen.Komande.StupciNaredba.Primijeni("HIDECOLUMNS(detalj,oznaka)").Count == 0, "");
            T("fiskal-gumb", Carpo.NetGen.Komande.FiskalNaredba.Tablica("FISKALIZACIJAFAKTURE(@@id)") == 2
                && Carpo.NetGen.Komande.FiskalNaredba.Tablica("FISKALIZACIJAN(@@id)") == -1
                && Carpo.NetGen.Komande.FiskalNaredba.Poziva(0) == 1
                && Carpo.NetGen.Komande.FiskalNaredba.Poziva(3) == 3
                && Carpo.NetGen.Komande.FiskalNaredba.Argument("FISKALIZACIJA(@@id)") == "@@ID", "");
            T("klik-podloga", Carpo.NetGen.Komande.KlikNaredba.NaPodlozi() == false
                && Carpo.NetGen.Komande.KlikNaredba.Kontrola("CLICK(ok)") == "OK", "");
            string poruka = "";
            Carpo.NetGen.Komande.PorukaNaredba.Prikazi = delegate(string t) { poruka = t; };
            Carpo.NetGen.Komande.PorukaNaredba.Pokazi("MSGBOX(Pozor)");
            Carpo.NetGen.Komande.PorukaNaredba.Prikazi = null;
            T("msgbox", poruka == "POZOR", poruka);
            bool kljuc = false;
            string kljucTekst = "";
            try
            {
                kljuc = Carpo.NetGen.Log.Spremnik.RegistracijaKaoCarpo();
            }
            catch (System.Exception ex)
            {
                kljucTekst = ex.ToString();
            }
            T("dijalog-kljuc", kljuc && kljucTekst.Length == 0, kljucTekst);
            var msg = Carpo.NetGen.Komande.DijalogTok.Koraci("CREATETEXTFILE3(c:\\a.txt)#msgbox(Pozor)#OK");
            T("dijalog-msgbox-lanac", msg.Count == 3 && msg[0].Vrsta == "sql" && msg[1].Vrsta == "poruka" && msg[1].Tekst == "POZOR" && msg[2].Vrsta == "ok", msg.Count.ToString());
            var rod = Carpo.NetGen.Komande.DijalogTok.Koraci("spMPIzdatnicaSKLMP @action=1#CLOSEPARENT");
            T("dijalog-closeparent-lanac", rod.Count == 2 && rod[0].Vrsta == "sql" && rod[0].Tekst.StartsWith("spMPIzdatnicaSKLMP") && rod[1].Vrsta == "roditelj", rod.Count.ToString());
            T("openform", Carpo.NetGen.Komande.OtvoriNaredba.Ime("OPENFORM(Unesi)") == "UNESI", "");
            T("prostor", Carpo.NetGen.Komande.ProstorNaredba.Je("POSLOVNIPROSTOR(5)")
                && Carpo.NetGen.Komande.ProstorNaredba.Argument("POSLOVNIPROSTOR(5)") == "5"
                && Carpo.NetGen.Komande.ProstorNaredba.Poziva(4) == 4, "");
            T("blagajna", Carpo.NetGen.Komande.BlagajnaNaredba.Upit("12") == "spmpBlagajnaEnable @mpkaseid = 12"
                && Carpo.NetGen.Komande.BlagajnaNaredba.Update("12", false).IndexOf("indenable = 0", StringComparison.Ordinal) >= 0
                && Carpo.NetGen.Komande.BlagajnaNaredba.Update("12", true).IndexOf("indenable = 1", StringComparison.Ordinal) >= 0, "");
            var kvacice = new bool[] { true, true, false };
            Carpo.NetGen.Komande.OdznaciNaredba.Odznaci(kvacice);
            T("uncheck", Carpo.NetGen.Komande.OdznaciNaredba.Je("UNCHECKMASTERGRID") && !kvacice[0] && !kvacice[1], "");
            T("closeparent", Carpo.NetGen.Komande.RoditeljNaredba.Je("CLOSEPARENT")
                && Carpo.NetGen.Komande.GumbTok.Akcija(null, "CLOSEPARENT") == "roditelj", "");
            T("isempty-lanac", Carpo.NetGen.Komande.PraznoNaredba.PreskociSljedeci("ISEMPTY(null)")
                && Carpo.NetGen.Komande.PraznoNaredba.PreskociSljedeci("ISEMPTY()")
                && !Carpo.NetGen.Komande.PraznoNaredba.PreskociSljedeci("ISEMPTY(5)"), "");
            T("grid-column-value", Carpo.NetGen.Komande.StupacVrijednost.Je("GRID_COLUMN_VALUE(g,c,1)")
                && Carpo.NetGen.Komande.StupacVrijednost.Izvrsava() == false
                && Carpo.NetGen.Komande.GumbTok.Akcija(null, "GRID_COLUMN_VALUE(g,c,1)") == "nista", "");
        }

        private static void AssemblyLoad()
        {
            string tip = "ASSEMBLYLOAD(Carpo.NetGen.GridTest.TgSonda)";
            TgSonda.Zadnji = null;
            var shell = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(tip, "shell", "7", "3", "Server=localhost;Database=spin", "select 1 as id", "@id='7' , ", "");
            T("assembly-shell", shell.Pozvan && !shell.Prekinuto
                && shell.Sklop == "Carpo.NetGen.GridTest"
                && TgSonda.Zadnji != null
                && TgSonda.Zadnji[0] == "7" && TgSonda.Zadnji[1] == "3"
                && TgSonda.Zadnji[2] == "Driver={SQL Server};Server=localhost;Database=spin"
                && TgSonda.Zadnji[3] == "select 1 as id" && TgSonda.Zadnji[4] == "@id='7' , "
                && TgSonda.Vratio == "NE", shell.Pozvan ? (TgSonda.Zadnji == null ? "" : string.Join("|", TgSonda.Zadnji)) : shell.Sklop);
            var raf = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(tip, "rafalno", "7", "3", "Server=localhost", "select 1 as id", "@id='7' , ", "@a=1");
            T("assembly-rafalno", raf.Pozvan && TgSonda.Zadnji[2] == "Server=localhost" && TgSonda.Zadnji[4] == "", TgSonda.Zadnji == null ? "" : TgSonda.Zadnji[4]);
            var dij = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(tip, "dijalog", "7", "3", "Server=localhost", "select 1 as id", "grid", "@CarpoForm='x',");
            T("assembly-dijalog", dij.Pozvan && TgSonda.Zadnji[3] == "@CarpoForm='x'," && TgSonda.Zadnji[4] == "@CarpoForm='x',"
                && TgSonda.Zadnji[2].StartsWith("Driver={SQL Server};", StringComparison.Ordinal), TgSonda.Zadnji == null ? "" : TgSonda.Zadnji[3]);
            TgSonda.Zadnji = null;
            var nema = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi("ASSEMBLYLOAD(Nema.Takvog.Tipa)", "shell", "1", "2", "Server=x", "select 1", "", "");
            T("assembly-nema", nema.Prekinuto && !nema.Pozvan && TgSonda.Zadnji == null, nema.Sklop);
            var tab = new System.Data.DataTable();
            tab.Columns.Add("id");
            tab.Columns.Add("Naziv2500");
            tab.Columns.Add("prazno");
            tab.Rows.Add("5", "A B", System.DBNull.Value);
            string red = Carpo.NetGen.Poziv.AssemblyPoziv.ParametriReda(tab.Rows[0]);
            T("assembly-red", red == "@id='5' , @Naziv='A B' , ", red);
        }

        private static void Jez57()
        {
            var cb = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            cb["tkosmomi"] = "Spin d.o.o.";
            T("jez-57-cb", Carpo.NetGen.Print.IspisZaglavlje.Desno("@@tkosmomi", cb) == "Spin d.o.o.", Carpo.NetGen.Print.IspisZaglavlje.Desno("@@tkosmomi", cb));
            var prazan = new System.Collections.Generic.Dictionary<string, string>();
            T("jez-57-prazan", Carpo.NetGen.Print.IspisZaglavlje.Desno("@@tkosmomi", prazan) == "null", Carpo.NetGen.Print.IspisZaglavlje.Desno("@@tkosmomi", prazan));
        }

        private static void Odabir()
        {
            T("odabir-verizne-manager", Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("KntKompVerizne.xml")), "");
            T("odabir-statusi-manager", Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("Statusi.xml")), "");
            T("odabir-blazak-manager", Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("KntBlaZak.xml")), "");
            T("odabir-oznaka-nije", !Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("OznakaKL.xml")), "");
            T("odabir-jezici-nije", !Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("JeziciNG.xml")), "");
            T("odabir-stavke-nije", !Carpo.NetGen.Shell.OdabirForme.JeManager(Ucitaj("RealizacijaStavkeNG.xml")), "");
        }

        private static void RucniStart()
        {
            var prije = GranulaSesija.Izvrsi;
            GranulaSesija.Izvrsi = null;
            try
            {
                T("rucni-oznaka", RucniSql("OznakaKL.xml"), "");
                T("rucni-verizne", RucniSql("KntKompVerizne.xml"), "");
                T("rucni-kuka-null", GranulaSesija.Izvrsi == null, "");
            }
            finally
            {
                GranulaSesija.Izvrsi = prije;
            }
        }

        private static bool RucniSql(string xml)
        {
            NgProject p = Ucitaj(xml);
            if (p == null)
                return false;
            var g = new MasterGrid();
            g.Kontekst.OperaterId = "7";
            g.Kontekst.Node = "null";
            g.Ucitaj(p, null);
            return g.Stanje != null && g.Stanje.Odluka == "salji" && !string.IsNullOrEmpty(g.Stanje.SqlZaSlanje);
        }

        private static void IntrastatPreskok()
        {
            NgProject p = Ucitaj("Intrastat.xml");
            var g = new MasterGrid();
            g.Kontekst.OperaterId = "7";
            if (p != null)
                g.Ucitaj(p, null);
            string sql = g.Stanje == null ? "" : (g.Stanje.SqlZaSlanje ?? "");
            bool preskok = p != null && g.Stanje.Odluka == "ostao-@@" && sql.IndexOf("@@godina", StringComparison.Ordinal) > 0 && g.Stanje.Tablica == null;
            T("intrastat-ostao", preskok, g.Stanje == null ? "" : g.Stanje.Odluka);
        }

        private static void ZastitaTestovi()
        {
            T("zastita-prva-rijec", !SqlTekst.JeZabranaPisanja("spKntZakljucivanjeTem @action=1"), "");
            SqlZastita z = ZastitaSql.Tekst("spKntZakljucivanjeTem @action=1,@datumod='20200101',@datumdo='20200131',@vrstadokid=1");
            T("zastita-zakljucivanje", !z.Pise && z.Procedure.Count == 1 && z.Procedure[0] == "spKntZakljucivanjeTem",
                z.Pise ? z.Razlog : (z.Procedure.Count == 0 ? "nema" : z.Procedure[0]));
            SqlZastita z2 = ZastitaSql.Tekst("exec dbo.spKntPreknjizavanjeNG @action=2, @ssoperateriid=7");
            T("zastita-preknjizi", !z2.Pise && z2.Procedure.Count == 1 && z2.Procedure[0] == "spKntPreknjizavanjeNG",
                z2.Procedure.Count == 0 ? "nema" : z2.Procedure[0]);
            SqlZastita tijelo = ZastitaSql.Tijelo("insert into knttem (a) values (1)\r\nexec spDruga @a=1");
            T("zastita-tijelo-pise", tijelo.Pise, tijelo.Razlog);
            SqlZastita temp = ZastitaSql.Tijelo("insert into #t select 1\r\nupdate #t set a=1\r\ndelete from #t\r\nexec spCita");
            T("zastita-temp", !temp.Pise && temp.Procedure.Count == 1 && temp.Procedure[0] == "spCita", temp.Razlog);
            T("zastita-delete-direktno", ZastitaSql.Tekst("delete from #t").Pise, "");
            T("zastita-insert-temp", !ZastitaSql.Tekst("insert into #t select 1").Pise, ZastitaSql.Tekst("insert into #t select 1").Razlog);
            T("zastita-create-temp", !ZastitaSql.Tekst("create table #t (a int)").Pise, ZastitaSql.Tekst("create table #t (a int)").Razlog);
            T("zastita-create-stalna", ZastitaSql.Tekst("create table knttem (a int)").Pise, "");
            T("zastita-merge", ZastitaSql.Tijelo("merge knttem t using src s on 1=1 when matched then update set a=1;").Pise, "");
            T("zastita-komentar", !ZastitaSql.Tijelo("-- delete from knttem\r\nselect 1").Pise, "");
            T("zastita-string", ZastitaSql.Tijelo("select 'delete from knttem' as x").Pise, "");
            T("zastita-dinamicki", ZastitaSql.Tijelo("exec(@sql)").Pise, "");
            NgProject zak = Ucitaj("ZakljucivanjeTemNG.xml");
            string master;
            List<string> upiti = Otvaranje.Upiti(zak, new SqlKontekst(), "ZakljucivanjeTemNG.xml", out master);
            T("otvaranje-nema-zakljucivanje", zak != null && !Ima(upiti, "spKntZakljucivanjeTem"), upiti.Count.ToString());
            NgProject pre = Ucitaj("KntPreknjizavanjeNG.xml");
            string master2;
            List<string> preUpiti = Otvaranje.Upiti(pre, new SqlKontekst(), "KntPreknjizavanjeNG.xml", out master2);
            T("otvaranje-nema-preknjizi", pre != null && !Ima(preUpiti, "spKntPreknjizavanjeNG"), preUpiti.Count.ToString());
        }

        private static void StabloKljucevi()
        {
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            NgProject sta = Ucitaj("Statusi.xml");
            Stablo.Kljucevi(sta, s);
            T("sta-stablo", sta != null && s["treeStupci"] == "0,1,2,3" && s["preskociId0"] == "true"
                && s["tagBezDvotocke"] == "vrijednost:POPUNI_FORM", s.ContainsKey("tagBezDvotocke") ? s["tagBezDvotocke"] : "");
        }

        private static void OstaloKljucevi()
        {
            string opz = null;
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir != null && opz == null)
            {
                string k = Path.Combine(dir.FullName, "definitions", "netgen", "BIOVEGA", "KntOPZ-STAT-1.xml");
                if (File.Exists(k))
                    opz = k;
                dir = dir.Parent;
            }
            Pogled g = opz == null ? null : GranulaIzvrsenje.Izvrsi(new Zahtjev { XmlPutanja = opz, Akcija = "gumb", Cilj = "Eporezna" });
            string dll = "";
            if (g != null && g.Stavke != null && g.Stavke.ContainsKey("dll"))
                dll = g.Stavke["dll"];
            T("opz-nije-dll", g != null && g.Izvedeno && dll == "ne", dll);
            NgProject bvg = Ucitaj("KUPDostKnNGBvg.xml");
            T("bvg-carpoform", GranulaIzvrsenje.CarpoFormKup(bvg) == "KnjigaUra.frmKnjigaUraKupEdit.xml", GranulaIzvrsenje.CarpoFormKup(bvg));
        }

        private static void GridKljucevi()
        {
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            var master = new GridStanje();
            master.CheckboxVidljiv = false;
            master.TimeoutSekundi = 300;
            NgProject sta = Ucitaj("Statusi.xml");
            DetailGrid.Kljucevi(sta, master, s);
            T("sta-detail", sta != null && s["detailNaSelekciji"] == "true" && s["detailNakonMastera"] == "true"
                && s["detSelector"] == "false" && s["detailKoloneJednom"] == "true", s.ContainsKey("detSelector") ? s["detSelector"] : "");
            s.Clear();
            NgProject bla = Ucitaj("KntBlaZak.xml");
            DetailGrid.Kljucevi(bla, master, s);
            T("bla-sirina", bla != null && s["detClear"] == "true" && s["kolona.VrstaBlagajne.sirina"] == "179" && s["detailNaSelekciji"] == "true",
                s.ContainsKey("kolona.VrstaBlagajne.sirina") ? s["kolona.VrstaBlagajne.sirina"] : "");
            if (bla != null)
            {
                var tab = new DataTable();
                tab.Columns.Add("VrstaBlagajne", typeof(string));
                tab.Rows.Add("blagajna");
                var gm = new MasterGrid();
                gm.Ucitaj(bla, tab);
                GridKolona vb = null;
                for (int i = 0; i < gm.Stanje.Kolone.Count; i++)
                {
                    if (string.Equals(gm.Stanje.Kolone[i].Podatak, "VrstaBlagajne", StringComparison.OrdinalIgnoreCase))
                        vb = gm.Stanje.Kolone[i];
                }
                T("bla-width-prepisuje", vb != null && vb.Sirina == 179, vb == null ? "nema" : vb.Sirina.ToString());
            }
            s.Clear();
            NgProject pl = Ucitaj("PrijemniList.xml");
            DetailGrid.Kljucevi(pl, master, s);
            T("pl-kolicina", pl != null && s["det.Kolicina"] == "71|#,####0.0000" && s["timeout"] == "300" && s["checkboxVidljiv"] == "false",
                s.ContainsKey("det.Kolicina") ? s["det.Kolicina"] : "");
            s.Clear();
            NgProject por = Ucitaj("MPPorKPNG.xml");
            var gp = new MasterGrid();
            if (por != null)
                gp.Ucitaj(por, null);
            DetailGrid.Kljucevi(por, gp.Stanje, s);
            T("mpk-checkbox", por != null && s["checkboxVidljiv"] == "true" && gp.Stanje.SortKlikom && gp.Stanje.TotalRed,
                s.ContainsKey("checkboxVidljiv") ? s["checkboxVidljiv"] : "");
        }

        private static void NextFormTest()
        {
            NgForm kom = Forma(Ucitaj("KomStanjeSkl.xml"), "MAINFORM");
            string sljedeca = DijalogTok.NextForm(kom);
            DijalogTok.Klik ok = DijalogTok.Gumb(kom, 1, "", true);
            T("next-kom", sljedeca == "ButtonKartica:KARTICA" && ok.Zatvara && ok.NextForm == sljedeca && !ok.SaljeSql, sljedeca);
            T("next-prazan", DijalogTok.NextForm(Forma(Ucitaj("MPRealizNG.xml"), "PROMET")) == "", "");
        }

        private static void GridDijalogTest()
        {
            NgForm promet = Forma(Ucitaj("MPRealizNG.xml"), "PROMET");
            DijalogTok.Punjenje stavke = null;
            List<DijalogTok.Punjenje> lista = DijalogTok.PriOtvori(promet);
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i].Ime, "stavke", StringComparison.OrdinalIgnoreCase))
                    stavke = lista[i];
            }
            T("grid-puni", stavke != null && !stavke.Preskoceno && stavke.Sql.IndexOf("@action=2", StringComparison.OrdinalIgnoreCase) >= 0, stavke == null ? "nema" : stavke.Sql);
            NgProject bs = Ucitaj("BSDok.xml");
            DijalogTok.Punjenje preskok = null;
            if (bs != null)
            {
                for (int f = 0; f < bs.Forme.Count && preskok == null; f++)
                {
                    List<DijalogTok.Punjenje> p = DijalogTok.PriOtvori(bs.Forme[f]);
                    for (int i = 0; i < p.Count; i++)
                    {
                        if (p[i].Preskoceno)
                            preskok = p[i];
                    }
                }
            }
            T("grid-skip", preskok != null && preskok.Sql.Length == 0, preskok == null ? "nema" : preskok.Ime);
        }

        private static void OnClickTest()
        {
            NgForm inv = Forma(Ucitaj("MPInventura.xml"), "INSERTFORM");
            NgControl ok = null;
            if (inv != null)
            {
                for (int i = 0; i < inv.Kontrole.Count; i++)
                {
                    string o = inv.Kontrole[i].Atributi == null ? "" : (inv.Kontrole[i].Atributi.Vrijednost("ONCLICK") ?? "");
                    if (o.IndexOf("spMPInventuraNG", StringComparison.OrdinalIgnoreCase) >= 0)
                        ok = inv.Kontrole[i];
                }
            }
            string naredba = ok == null || ok.Atributi == null ? "" : (ok.Atributi.Vrijednost("ONCLICK") ?? "");
            DijalogTok.Klik jup = DijalogTok.Gumb(inv, 0, naredba, true);
            bool imaSp = false;
            for (int i = 0; i < jup.Naredbe.Count; i++)
            {
                if (jup.Naredbe[i].IndexOf("spMPInventuraNG", StringComparison.OrdinalIgnoreCase) >= 0)
                    imaSp = true;
            }
            DijalogTok.Klik cisto = DijalogTok.Gumb(null, 0, "spX @a=1#OK", true);
            T("onclick-ok", cisto.Zatvara && !cisto.SaljeSql && cisto.Naredbe.Count == 1 && cisto.Naredbe[0].IndexOf("spX", StringComparison.Ordinal) >= 0, "");
            T("onclick-jupiter", ok != null && !jup.SaljeSql && imaSp, jup.SaljeSql ? "salje" : "ne");
            NgForm partner = Forma(Ucitaj("KntPreknjizavanjePartnera.xml"), "FILTERFORM");
            string submit = partner == null ? "" : partner.OnSubmit;
            string zalijepljeno = Lanac.Lijepi(submit, "@PartnerSTARIid=9,@PartnerNOVIid=8,");
            T("onclick-exec-ne-lijepi", submit.TrimStart().StartsWith("exec", StringComparison.OrdinalIgnoreCase)
                && zalijepljeno.IndexOf("PartnerSTARIid=9", StringComparison.Ordinal) < 0
                && zalijepljeno.IndexOf("!!", StringComparison.Ordinal) < 0
                && zalijepljeno.IndexOf("@Action=1", StringComparison.Ordinal) >= 0, zalijepljeno);
        }

        private static void ComboDijalogTest()
        {
            NgForm realiz = Forma(Ucitaj("MPRealizNG.xml"), "FORM1");
            NgControl trgovina = Kontrol(realiz, "trgovinaid");
            T("combo-1-prazan", ComboDijalog.AllowEmpty(trgovina), "");
            var k = new SqlKontekst();
            k.OperaterId = "7";
            string sql1 = ComboDijalog.SqlZaSlanje(trgovina.Atributi.Vrijednost("SQL"), k);
            T("combo-1-operater", sql1.IndexOf("@@operateriid", StringComparison.OrdinalIgnoreCase) < 0 && sql1.IndexOf("=7", StringComparison.Ordinal) >= 0, sql1);

            NgForm inv = Forma(Ucitaj("MPInventura.xml"), "INSERTFORM");
            NgControl sort = Kontrol(inv, "sortpoid");
            NgControl trg = Kontrol(inv, "trgovinaid");
            T("combo-2-nema-prazan", sort != null && !ComboDijalog.AllowEmpty(sort), "");
            var tablica = new DataTable();
            tablica.Columns.Add("id", typeof(int));
            tablica.Columns.Add("naziv", typeof(string));
            tablica.Rows.Add(1, "Klasifikacija + naziv robe");
            tablica.Rows.Add(2, "sifra");
            T("combo-2-odabir", ComboDijalog.OdabirAkoNista(tablica, false) == "1", "");
            List<string> osvjezi = ComboDijalog.RefreshImena(trg.Atributi.Vrijednost("ONCLICK"));
            T("combo-3-refresh", osvjezi.Count == 2 && osvjezi[0] == "skladisteid" && osvjezi[1] == "osobeid" && !ComboDijalog.OnClickSeSalje(true), osvjezi.Count.ToString());
            NgControl osobe = Kontrol(inv, "osobeid");
            string sql4 = ComboDijalog.SqlZaSlanje(osobe.Atributi.Vrijednost("SQL"), k);
            T("combo-4-ostao", SqlTekst.Odluka(sql4) == "ostao-@@", sql4);
            string poslije = sql4.Replace("@@trgovinaid", "5");
            T("combo-4-poslije", SqlTekst.Odluka(poslije) == "salji" && poslije.IndexOf("= 5", StringComparison.Ordinal) >= 0, poslije);

            NgForm nacin = Forma(Ucitaj("MPKntNacinPlacAzur.xml"), "FILTERFORM");
            if (nacin == null)
                nacin = Forma(Ucitaj("MPKntNacinPlacAzur.xml"), "FORM1");
            NgControl svi = Kontrol(nacin, "svi");
            T("combo-5-folse", svi != null && !ComboDijalog.AllowEmpty(svi), svi == null ? "nema" : (svi.Atributi.Vrijednost("ALLOWEMPTY") ?? ""));
            var sviTab = new DataTable();
            sviTab.Columns.Add("svi", typeof(int));
            sviTab.Columns.Add("naziv", typeof(string));
            sviTab.Rows.Add(1, "Samo za MP");
            sviTab.Rows.Add(2, "SVI");
            T("combo-5-jedan", ComboDijalog.OdabirAkoNista(sviTab, false) == "1", "");

            NgForm kase = null;
            NgProject kaseP = Ucitaj("MPKaseOperNG.xml");
            if (kaseP != null)
            {
                for (int i = 0; i < kaseP.Forme.Count; i++)
                {
                    if (Kontrol(kaseP.Forme[i], "kasa") != null && kaseP.Forme[i].Kontrole != null)
                    {
                        NgControl kandidat = Kontrol(kaseP.Forme[i], "kasa");
                        if (kandidat.Grana == NgGrana.Combo)
                            kase = kaseP.Forme[i];
                    }
                }
            }
            NgControl kasa = Kontrol(kase, "kasa");
            T("combo-6-nema-atribut", kasa != null && !ComboDijalog.AllowEmpty(kasa), "");

            NgControl multi = null;
            NgProject opz = Ucitaj("KntOPZ-STAT-1.xml");
            if (opz != null)
            {
                for (int i = 0; i < opz.Forme.Count && multi == null; i++)
                    multi = Kontrol(opz.Forme[i], "nizKonto");
            }
            T("combo-7-multidrop", multi != null && multi.Grana == NgGrana.MultiDrop, multi == null ? "nema" : multi.Grana.ToString());

            T("combo-9-nivel", ComboDijalog.AllowEmpty(Kontrol(Forma(Ucitaj("MPNivelacija.xml"), "FILTERFORM"), "xtrgovinaid")), "");

            var nula = new DataTable();
            nula.Columns.Add("namjena", typeof(int));
            nula.Columns.Add("naziv", typeof(string));
            nula.Rows.Add(0, "Roba");
            nula.Rows.Add(1, "Osnovno sredstvo");
            T("combo-10-nula", !ComboDijalog.AllowEmpty((string)null) && ComboDijalog.OdabirAkoNista(nula, false) == "1", "");

            var prazna = new DataTable();
            prazna.Columns.Add("id", typeof(string));
            prazna.Columns.Add("naziv", typeof(string));
            prazna.Rows.Add("5", "A");
            ComboDijalog.UbaciPrazan(prazna);
            T("combo-prazan-red", Convert.ToString(prazna.Rows[0][0]) == "null" && Convert.ToString(prazna.Rows[0][1]) == "", Convert.ToString(prazna.Rows[0][0]));
            var prazanId = new DataTable();
            prazanId.Columns.Add("id", typeof(int));
            prazanId.Columns.Add("naziv", typeof(string));
            prazanId.Rows.Add(5, "A");
            ComboDijalog.UbaciPrazan(prazanId);
            T("combo-prazan-int", prazanId.Rows[0][0] == DBNull.Value && Convert.ToString(prazanId.Rows[0][1]) == "", Convert.ToString(prazanId.Rows[0][0]));
        }

        private static void DijalogRupe()
        {
            NgProject zak = Ucitaj("ZakljucivanjeTemNG.xml");
            NgForm main = Forma(zak, "MAINFORM");
            NgControl zakljuci = Kontrol(main, "buttonOK");
            string klik = zakljuci == null ? "" : (zakljuci.Atributi.Vrijednost("ONCLICK") ?? "");
            DijalogTok.Klik otvara = DijalogTok.Gumb(main, 0, klik, true);
            T("rupa-zakljuci", otvara.OtvaraFormu == "Zakljucavanje" && !otvara.Zatvara && !otvara.SaljeSql, otvara.OtvaraFormu);
            NgForm potvrda = Forma(zak, "Zakljucavanje");
            DijalogTok.Klik ok = DijalogTok.Gumb(potvrda, 0, "OK", true);
            T("rupa-zakljuci-jupiter", ok.MasterSql.IndexOf("spKntZakljucivanjeTem", StringComparison.OrdinalIgnoreCase) >= 0 && !ok.SaljeSql, ok.MasterSql);

            NgProject real = Ucitaj("MPRealizNG.xml");
            NgForm form1 = Forma(real, "FORM1");
            DijalogTok.Klik pokazi = DijalogTok.Gumb(form1, 1, "", true);
            T("rupa-pokazi", pokazi.FillExplorer && pokazi.MasterSql.IndexOf("spMPRealizacijaRobeNG", StringComparison.OrdinalIgnoreCase) >= 0 && !pokazi.SaljeSql, pokazi.MasterSql);
            T("rupa-combo", SqlKontrole(form1, "trgovinaid").IndexOf("trgovina", StringComparison.OrdinalIgnoreCase) >= 0, SqlKontrole(form1, "trgovinaid"));
            NgForm promet = Forma(real, "PROMET");
            T("rupa-grid", SqlKontrole(promet, "stavke").IndexOf("@action=2", StringComparison.OrdinalIgnoreCase) >= 0, SqlKontrole(promet, "stavke"));
            T("rupa-next-prazan", DijalogTok.NextForm(promet) == "", DijalogTok.NextForm(promet));

            NgProject niv = Ucitaj("MPNivelacija.xml");
            NgForm filter = Forma(niv, "FILTERFORM");
            T("rupa-niv-combo", SqlKontrole(filter, "xtrgovinaid").IndexOf("spMPDajTrgovinaNG", StringComparison.OrdinalIgnoreCase) >= 0, SqlKontrole(filter, "xtrgovinaid"));
            DijalogTok.Klik nivPokazi = DijalogTok.Gumb(filter, 1, "", true);
            T("rupa-niv-pokazi", nivPokazi.FillExplorer && !nivPokazi.SaljeSql && nivPokazi.MasterSql.IndexOf("spMPQueryGlaIDetNG", StringComparison.OrdinalIgnoreCase) >= 0, nivPokazi.MasterSql);

            NgForm unesi = Forma(Ucitaj("JeziciNG.xml"), "Unesi");
            T("rupa-next", DijalogTok.NextForm(unesi) == "Dodaj", DijalogTok.NextForm(unesi));
            string master;
            List<string> upiti = Otvaranje.Upiti(real, new SqlKontekst(), "MPRealizNG.xml", out master);
            T("rupa-jupiter-nema-onsubmit", !Ima(upiti, "spMPRealizacijaRobeNG"), upiti.Count.ToString());
        }

        private static string SqlKontrole(NgForm forma, string ime)
        {
            List<DijalogTok.Punjenje> lista = DijalogTok.PriOtvori(forma);
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return lista[i].Sql ?? "";
            }
            return "";
        }

        private static NgForm Forma(NgProject p, string ime)
        {
            if (p == null || p.Forme == null)
                return null;
            for (int i = 0; i < p.Forme.Count; i++)
            {
                if (string.Equals(p.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return p.Forme[i];
            }
            return null;
        }

        private static NgControl Kontrol(NgForm f, string ime)
        {
            if (f == null)
                return null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (string.Equals(f.Kontrole[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return f.Kontrole[i];
            }
            return null;
        }

        private static void DijalogKlikTest()
        {
            NgProject p = Ucitaj("JeziciNG.xml");
            Dictionary<string, string> ok = DijalogKlik.Stavke(p, "Unesi.buttonOK");
            T("dijalog-ostaje", ok != null && ok["ostajeOtvorena"] == "true" && ok["fokus"] == "naziv" && ok["sqlImaZalijep"] == "false",
                ok == null ? "nema" : ok["fokus"]);
            Dictionary<string, string> zatvori = DijalogKlik.Stavke(p, "Unesi.buttonCLOSE");
            T("dijalog-ok", zatvori != null && zatvori["okClick"] == "true" && zatvori["dodajPoruka"] == "", "");
            Dictionary<string, string> odustani = DijalogKlik.Stavke(p, "Izmijeni.buttonCLOSE");
            T("dijalog-close", odustani != null && odustani["refreshMaster"] == "false", "");
            Dictionary<string, string> spremi = DijalogKlik.Stavke(p, "Izmijeni.buttonOK");
            T("dijalog-refresh", spremi != null && spremi["refreshMaster"] == "true" && spremi["ostajeOtvorena"] == "false", "");
            T("jez-next", zatvori != null && zatvori["okClick"] == "true" && zatvori["nextForm"] == "Dodaj" && zatvori["dodajPoruka"] == "",
                zatvori == null ? "" : zatvori["nextForm"]);
            T("jez-refreshmaster", p != null && Lanac.OsvjezavaZadnjiSql("Unesi#refreshmaster") && Lanac.OsvjezavaSegment("REFRESHMASTER")
                && !Lanac.Izvrsi("REFRESHMASTER", true, null).RefreshMasterGrid, "");
            NgProject nef = Ucitaj("MPRACNEFISK.xml");
            string nefNaredba = "";
            if (nef != null)
            {
                for (int i = 0; i < nef.Gumbi.Count; i++)
                {
                    string n = nef.Gumbi[i].Naredba ?? "";
                    if (n.IndexOf("VRIJEMEJIR", StringComparison.OrdinalIgnoreCase) >= 0)
                        nefNaredba = n;
                }
            }
            T("nef-refresh", Lanac.OsvjezavaZadnjiSql(nefNaredba), nefNaredba);
            NgProject oznaka = Ucitaj("OznakaKL.xml");
            NgForm frm = null;
            if (oznaka != null)
            {
                for (int i = 0; i < oznaka.Forme.Count; i++)
                {
                    if (string.Equals(oznaka.Forme[i].Ime, "FRMEDIT", StringComparison.OrdinalIgnoreCase))
                        frm = oznaka.Forme[i];
                }
            }
            var kljucevi = new Dictionary<string, string>();
            Raspored.KljuceviForme(frm, kljucevi);
            T("frmedit-tab", frm != null && kljucevi["tab"] == "Label16,oznaka2,Prihvati,Odustani" && kljucevi["formaNaslov"] == "Izmjena oznake" && kljucevi["acceptButton"] == "",
                kljucevi.ContainsKey("tab") ? kljucevi["tab"] : "");
            Dictionary<string, string> prihvati = DijalogKlik.PoNatpisu(oznaka, "Prihvati", true);
            T("okl-prihvati", prihvati != null && prihvati["refreshMaster"] == "true" && prihvati["sqlImaZalijep"] == "false" && prihvati["sqlApostrof"] == "''",
                prihvati == null ? "nema" : prihvati["sqlImaZalijep"]);
            Dictionary<string, string> od = DijalogKlik.PoNatpisu(oznaka, "Odustani", false);
            T("okl-odustani", od != null && od["refreshMaster"] == "false", "");
            Dictionary<string, string> esc = DijalogKlik.Escape(oznaka);
            T("okl-escape", esc["refreshMaster"] == "false" && esc["okClick"] == "false", "");
            NgForm filter = null;
            NgProject pre = Ucitaj("KntPreknjizavanjeNG.xml");
            if (pre != null)
            {
                for (int i = 0; i < pre.Forme.Count; i++)
                {
                    if (string.Equals(pre.Forme[i].Ime, "FILTER", StringComparison.OrdinalIgnoreCase))
                        filter = pre.Forme[i];
                }
            }
            List<string> combo = Otvaranje.Kontrole(filter, new SqlKontekst());
            T("dijalog-combo-sql", filter != null && combo.Count > 0 && !Ima(combo, "spKntPreknjizavanjeNG"), combo.Count.ToString());
        }

        private static bool Ima(List<string> upiti, string dio)
        {
            for (int i = 0; i < upiti.Count; i++)
            {
                if (upiti[i].IndexOf(dio, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static void T(string id, bool ok, string detalj)
        {
            Console.WriteLine((ok ? "PASS " : "FAIL ") + id + (string.IsNullOrEmpty(detalj) ? "" : " | " + detalj));
            if (!ok)
                fail++;
        }

        private static void SqlPreskoci()
        {
            T("sql-prazno", SqlTekst.Odluka("") == "prazno", SqlTekst.Odluka(""));
            T("sql-at", SqlTekst.Odluka("  @x") == "pocinje-s-@", "");
            T("sql-atat-na-nuli-je-at", SqlTekst.Odluka("@@foo") == "pocinje-s-@", SqlTekst.Odluka("@@foo"));
            T("sql-atat-kasnije", SqlTekst.Odluka("select @@foo") == "ostao-@@", "");
            T("sql-salji", SqlTekst.Odluka("select 1") == "salji", "");
            T("sql-tilde", SqlTekst.Odluka("select 1~~2") == "salji" && SqlTekst.Odluka("select 1~~2").Length > 0, "");
            var k = new SqlKontekst();
            k.Id = "5";
            k.OperaterId = "7";
            k.Node = "null";
            string s = SqlTekst.Sredi("select @@id, @@operateriid, @@node", k);
            T("sql-zamjena", s == "select 5, 7, null", s);
            T("sql-zamjena-nema-atat", SqlTekst.Odluka(s) == "salji", "");
            var forma = SqlTekst.ZaFormu(null);
            string fid = SqlTekst.Sredi("select opis from trgovina where trgovinaid = @@id", forma);
            T("forma-id-0", forma.Id == "0" && forma.DetId == "0" && forma.Node == "null" && fid.IndexOf("trgovinaid = 0", StringComparison.Ordinal) >= 0, fid);
            var red = new SqlKontekst();
            red.Id = "15";
            red.Node = "abc";
            T("forma-id-red", SqlTekst.ZaFormu(red).Id == "15" && SqlTekst.ZaFormu(red).Node == "abc", "");
            string[] lanac = Lanac.OnLoadLanac(":FILTERFORM#select case when '@@izvjestaj'='GFI-POD' then 'disable' else 'enable' end as TSI#select case when '@@izvjestaj'='TSI-POD' then 'disable' else 'enable' end as GFI");
            T("onload-lanac", lanac.Length == 3 && lanac[0] == "FILTERFORM"
                && Lanac.GumbIzCase(lanac[1], "GFI-POD") == "TSI=disable"
                && Lanac.GumbIzCase(lanac[2], "GFI-POD") == "GFI=enable", Lanac.GumbIzCase(lanac[1], "GFI-POD"));
            T("cls-gfipod-nije-ispis", !IspisMotora.JeIspisImena("AOPuExcel.clsGFIPOD(GFI-POD601.xls)")
                && IspisMotora.JeIspisImena("rpt.clsReport")
                && !("AOPuExcel.clsGFIPOD(GFI-POD601.xls)".ToUpperInvariant().IndexOf("ASSEMBLYLOAD(", StringComparison.Ordinal) >= 0), "");
            T("isempty-preskoci", IspisMotora.Zaglavlje("#ISEMPTY()# #Od 1. do 2.") == ""
                && IspisMotora.Zaglavlje("#ISEMPTY(1)# #Od 1. do 2.") == "Od 1. do 2.", IspisMotora.Zaglavlje("#ISEMPTY(1)# #Od 1. do 2."));
            T("dblclick-refresh", Lanac.OsvjezavaZadnjiSql("Unesi#refreshmaster")
                && Lanac.OsvjezavaZadnjiSql("REFRESHMASTERGRID")
                && !Lanac.OsvjezavaZadnjiSql("PRINTREPXMASTER")
                && Lanac.IspisSegment("Ispiši#PRINTREPXMASTER") == "PRINTREPXMASTER", "");
            var filt = Ucitaj("KntPreknjizavanjeNG.xml");
            bool imaFilter = false;
            if (filt != null)
            {
                for (int i = 0; i < filt.Forme.Count; i++)
                {
                    if (string.Equals(filt.Forme[i].Ime, "FILTER", StringComparison.OrdinalIgnoreCase))
                        imaFilter = true;
                }
            }
            T("filter-je-forma", filt != null && imaFilter, filt == null ? "nema xml" : filt.Forme.Count.ToString());
        }

        private static void Zamjene()
        {
            var k = new SqlKontekst();
            k.Id = "9";
            var g = new MasterGrid();
            g.Kontekst = k;
            var p = Projekt("ONLOAD", "select @@id as id");
            g.Ucitaj(p, TablicaId(9));
            T("refresh-ne-mijenja-id", g.Stanje.ZadnjiSql == "select 9 as id", g.Stanje.ZadnjiSql);
            g.Kontekst.Id = "3";
            g.Osvjezi(TablicaId(3));
            T("refresh-koristi-zadnji-tekst", g.Stanje.SqlZaSlanje.IndexOf("@@", StringComparison.Ordinal) < 0 && g.Stanje.SqlZaSlanje.IndexOf("9", StringComparison.Ordinal) >= 0, g.Stanje.SqlZaSlanje);
            T("refresh-redoslijed", g.Stanje.Redoslijed.Contains("REFRESHMASTERGRID"), "");
        }

        private static void OznakaBezBaze()
        {
            var p = Ucitaj("OznakaKL.xml");
            T("oznaka-postoji", p != null, "");
            if (p == null)
                return;
            var g = new MasterGrid();
            g.Ucitaj(p, null);
            T("oznaka-nije-forma", !g.Stanje.OtvaraFormu, "");
            T("oznaka-select", g.Stanje.IzvorSql.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase), g.Stanje.IzvorSql);
            T("oznaka-nema-onload", !p.Atributi.Ima("ONLOAD"), "");
            T("oznaka-checkbox-skriven", !g.Stanje.CheckboxVidljiv, "");
            T("oznaka-total-iskljucen", !g.Stanje.TotalRed, "");
            T("oznaka-sort", g.Stanje.SortKlikom, "");
            T("oznaka-groupby-false", g.Stanje.GroupByBox.HasValue && g.Stanje.GroupByBox.Value == false, "");
            T("oznaka-frozen-0", g.Stanje.ZamrznuteKolone == 0, "");
            T("oznaka-edit-ignorira-gridallowedit", g.Stanje.EnterTipka == "None", g.Stanje.EnterTipka);
            T("oznaka-filter-carpo", g.Stanje.FilterRed && g.Stanje.FilterNamjernoOdstupanje && !g.Stanje.FilterIzXml, "");
            T("oznaka-dx-autofilter", Ima(g, "OptionsView.ShowAutoFilterRow=true"), "");
            T("oznaka-dx-sort-on", Ima(g, "OptionsCustomization.AllowSort=true"), "");
            T("oznaka-redoslijed", g.Stanje.Redoslijed[0] == "startup-sql" && g.Stanje.Redoslijed.Contains("sredi"), string.Join(",", g.Stanje.Redoslijed.ToArray()));
            T("oznaka-stablo-0", p.Raspored != null && !p.Raspored.StabloVidljivo && p.Raspored.LijeviSplitter == 0 && !p.Raspored.GumbDrvoEnabled, "");
            var tg = new DataTable();
            tg.Columns.Add("oznaka", typeof(string));
            tg.Rows.Add("a");
            var gg = new MasterGrid();
            gg.Ucitaj(p, tg);
            GridKolona oz = null;
            for (int i = 0; i < gg.Stanje.Kolone.Count; i++)
            {
                if (string.Equals(gg.Stanje.Kolone[i].Podatak, "oznaka", StringComparison.OrdinalIgnoreCase))
                    oz = gg.Stanje.Kolone[i];
            }
            T("oznaka-gridallowedit-nije-edit", oz != null && oz.EditTip == "NoEdit" && oz.Sirina == 100, oz == null ? "nema" : oz.EditTip + " " + oz.Sirina);
        }

        private static void JeziciBezBaze()
        {
            var p = Ucitaj("JeziciNG.xml");
            T("jezici-postoji", p != null, "");
            if (p == null)
                return;
            var g = new MasterGrid();
            var t = new DataTable();
            t.Columns.Add("Naziv", typeof(string));
            t.Rows.Add("hr");
            g.Ucitaj(p, t);
            T("jezici-onload", g.Stanje.IzvorSql.IndexOf("spJeziciQuery", StringComparison.OrdinalIgnoreCase) >= 0, g.Stanje.IzvorSql);
            T("jezici-clear-nije-true", p.NadjiGrupu("MASTERCOLUMNS").ClearColums.ToLowerInvariant() != "true", "");
            GridKolona naziv = null;
            for (int i = 0; i < g.Stanje.Kolone.Count; i++)
            {
                if (string.Equals(g.Stanje.Kolone[i].Podatak, "Naziv", StringComparison.OrdinalIgnoreCase))
                    naziv = g.Stanje.Kolone[i];
            }
            T("jezici-naziv", naziv != null && naziv.Natpis == "Naziv" && naziv.Sirina == MasterGrid.VbJedinica(2500), naziv == null ? "nema" : naziv.Sirina.ToString());
            T("jezici-vb-179", MasterGrid.VbJedinica(2500) == 179, MasterGrid.VbJedinica(2500).ToString());
            t.Columns.Add("Opis", typeof(string));
            t.Rows[0]["Opis"] = "x";
            g.Ucitaj(p, t);
            GridKolona opis = null;
            naziv = null;
            for (int i = 0; i < g.Stanje.Kolone.Count; i++)
            {
                if (string.Equals(g.Stanje.Kolone[i].Podatak, "Opis", StringComparison.OrdinalIgnoreCase))
                    opis = g.Stanje.Kolone[i];
                if (string.Equals(g.Stanje.Kolone[i].Podatak, "Naziv", StringComparison.OrdinalIgnoreCase))
                    naziv = g.Stanje.Kolone[i];
            }
            T("jezici-clear-false-ne-skriva", opis != null && opis.Vidljiva && opis.Sirina == 100 && naziv != null && naziv.Vidljiva, opis == null ? "nema" : opis.Sirina.ToString());
            T("jezici-edit-noedit", naziv != null && naziv.EditTip == "NoEdit" && g.Stanje.EditMode == "EditOff", "");
            T("jezici-header-prazni-redovi", g.Stanje.HeaderLines == 3 && g.Stanje.EmptyRows && g.Stanje.SelectionMode == "MultipleSelection", "");
            T("jezici-stablo-0", p.Raspored != null && !p.Raspored.StabloVidljivo && p.Raspored.LijeviSplitter == 0 && !p.Raspored.GumbDrvoEnabled, "");
            T("jezici-dva-g-nema-agregat", naziv != null && string.IsNullOrEmpty(naziv.Agregat), "");
            T("jezici-header-1", naziv != null && naziv.PoravnanjeZaglavlja == "1", "");
            T("jezici-checkbox-off", !g.Stanje.CheckboxVidljiv, "");
        }

        private static void Kolone()
        {
            var t = new DataTable();
            t.Columns.Add("klmasterrobaid", typeof(int));
            t.Columns.Add("Iznos1600", typeof(decimal));
            t.Columns.Add("Opis", typeof(string));
            t.Columns.Add("Datum", typeof(DateTime));
            t.Columns.Add("sum!X", typeof(int));
            t.Rows.Add(1, 2.5m, "a", DateTime.Today, 3);
            var k = MasterGrid.IzPodatka(t.Columns["klmasterrobaid"], "");
            T("kol-id-skriven", !k.Vidljiva && k.Format == "##0" && k.Sirina == 34, "");
            var w = MasterGrid.IzPodatka(t.Columns["Iznos1600"], "");
            T("kol-sirina-1600", w.Sirina == 1600 / 14 && w.Natpis == "Iznos" && w.Format == "#,##0.00", w.Natpis + " " + w.Sirina);
            var s = MasterGrid.IzPodatka(t.Columns["Opis"], "");
            T("kol-string-100", s.Sirina == 100 && s.EditTip == "NoEdit", s.Sirina.ToString());
            var d = MasterGrid.IzPodatka(t.Columns["Datum"], "");
            T("kol-datum", d.Format == "dd.MM.yyyy" && d.Sirina == 100, "");
            var z = MasterGrid.IzPodatka(t.Columns["sum!X"], "");
            T("kol-sum-uzvicnik-mrtav", string.IsNullOrEmpty(z.Agregat) && z.Natpis.IndexOf("!") < 0, z.Natpis);
            var e = MasterGrid.IzPodatka(t.Columns["Opis"], "Opis");
            T("kol-edit-contains", e.Uredivanje, "");
            var e0 = MasterGrid.IzPodatka(t.Columns["Opis"], "");
            T("kol-bez-edit", !e0.Uredivanje, "");
            var p = Projekt("ONLOAD", "select 1");
            p.Atributi.GetType();
            var g = new MasterGrid();
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("Naziv", typeof(string));
            dt.Rows.Add(4, "a");
            dt.Rows.Add(8, "b");
            var proj = ProjektS("ONLOAD", "select 1 as id", "MASTERMOVELAST", "true");
            g.Ucitaj(proj, dt);
            T("fokus-zadnji", g.Stanje.FocusRazlog == "MASTERMOVELAST" && g.Stanje.FocusRed == 1 && g.Stanje.Id == "8", g.Stanje.Id);
            var g2 = new MasterGrid();
            g2.Ucitaj(Projekt("ONLOAD", ":FILTERFORM"), null);
            T("dvotocka-nije-sql", g2.Stanje.OtvaraFormu && g2.Stanje.ImeForme == "FILTERFORM" && g2.Stanje.Odluka == "forma", "");
            var g3 = new MasterGrid();
            g3.Ucitaj(Projekt("ONLOAD", ""), null);
            T("prazan-onload", g3.Stanje.Odluka == "prazno", g3.Stanje.Odluka);
            T("log-preskok", NgDnevnik.Zapisi.Count > 0 && NgDnevnik.Zapisi[NgDnevnik.Zapisi.Count - 1].Opis == "NetGen preskok", NgDnevnik.Zapisi.Count == 0 ? "" : NgDnevnik.Zapisi[NgDnevnik.Zapisi.Count - 1].Opis);
            T("checkbox-default-vidljiv", new MasterGrid().Stanje.CheckboxVidljiv, "");
            T("total-default-ukljucen", new GridStanje().TotalRed, "");
            T("sort-default-ukljucen", new GridStanje().SortKlikom, "");
            T("filter-default-iskljucen", !new GridStanje().FilterRed, "");
            var gFilt = new MasterGrid();
            gFilt.Ucitaj(Projekt("ONLOAD", "select 1"), null);
            T("filtermode-nema-carpo-ukljucen", gFilt.Stanje.FilterRed && gFilt.Stanje.FilterNamjernoOdstupanje, "");
            var gFiltOff = new MasterGrid();
            gFiltOff.Ucitaj(ProjektS("ONLOAD", "select 1", "FILTERMODE", "false"), null);
            T("filtermode-false-gasi", !gFiltOff.Stanje.FilterRed && gFiltOff.Stanje.FilterIzXml && !gFiltOff.Stanje.FilterNamjernoOdstupanje, "");
            var gDef = new MasterGrid();
            var dtDef = new DataTable();
            dtDef.Columns.Add("a", typeof(int));
            dtDef.Rows.Add(1);
            gDef.Ucitaj(Projekt("ONLOAD", "select 1 as a"), dtDef);
            T("groupby-janus-default-true", !gDef.Stanje.GroupByBox.HasValue && Ima(gDef, "OptionsView.ShowGroupPanel=true"), "");
            T("dx-header-empty-select", Ima(gDef, "HeaderLines=3") && Ima(gDef, "EmptyRows=true") && Ima(gDef, "SelectionMode=MultipleSelection") && Ima(gDef, "EditMode=EditOff"), "");
            var gErr = new MasterGrid();
            var dtErr = new DataTable();
            dtErr.Columns.Add("Naziv", typeof(string));
            dtErr.Rows.Add("a");
            gErr.Ucitaj(Projekt("ONLOAD", "select oznaka2 from klmasterroba"), dtErr);
            T("prije-greske-ima-stupac", gErr.Stanje.Kolone.Count > 2, gErr.Stanje.Kolone.Count.ToString());
            gErr.ZabiljeziGresku("Invalid column name 'oznaka2'.");
            T("sql-greska-ne-puni", gErr.Stanje.Kolone.Count == 0 && gErr.Stanje.Tablica == null && gErr.Stanje.Poruka != null && gErr.Stanje.Poruka.IndexOf("oznaka2", StringComparison.Ordinal) >= 0, gErr.Stanje.Poruka);
            T("log-greska", NgDnevnik.Zapisi.Count > 0 && NgDnevnik.Zapisi[NgDnevnik.Zapisi.Count - 1].Razina == "Error", "");
        }

        private static void KontroleTestovi()
        {
            T("datum-mjesec-0", Ponasanje.DatumPocetni("firstdayoflastmonth", 2026, 1) == "01.0.2026", Ponasanje.DatumPocetni("firstdayoflastmonth", 2026, 1));
            T("datum-today-literal", Ponasanje.DatumPocetni("TODAY", 2026, 11) == "today", "");
            T("datum-firstday", Ponasanje.DatumPocetni("firstdayofmonth", 2026, 11) == "01.11.2026"
                && Ponasanje.DatumPocetni("firstdayofyear", 2026, 6) == "01.01.2026", "");
            T("datum-checkbox-null", Ponasanje.DatumParametar(true, null, new DateTime(2026, 1, 2)) == "null", "");
            T("datum-nije-allownull", Ponasanje.Dx("date").IndexOf("AllowNull=false", StringComparison.Ordinal) >= 0, "");
            T("cur-isti-broj", Ponasanje.DxBroj() == Ponasanje.Dx("number") && Ponasanje.BrojParametar("") == "0", "");
            T("combo-prazan-je-1", Ponasanje.ComboParametar(false, true) == "1" && Ponasanje.ComboParametar(true, true) == "null", "");
            T("allow-prazan-nije", !Ponasanje.AllowEmptyJeTrue("") && !Ponasanje.AllowEmptyJeTrue("FALSE") && Ponasanje.AllowEmptyJeTrue("TRUE"), "");
            T("multidrop-ne-ubacuje", !Ponasanje.MultidropUbacujePrazanRed(), "");
            T("trazilica-dbo-nije-sp", Ponasanje.TrazilicaOblik("dbo.spNesto @a=1") == "hash"
                && Ponasanje.TrazilicaOblik("spX") == "sp" && Ponasanje.TrazilicaOblik("select 1") == "select", "");
            string like = Ponasanje.TrazilicaSql("roba#robaid#userid%#!cast(userid as varchar)#%naziv%", "12");
            T("trazilica-like", like.IndexOf("userid like '12%'", StringComparison.Ordinal) >= 0
                && like.IndexOf("naziv like '%12%'", StringComparison.Ordinal) >= 0
                && like.IndexOf("cast(userid as varchar) like", StringComparison.Ordinal) < 0, like);
            T("combo-refresh", Ponasanje.ComboOsvjezavanje("refresh", true) == "refresh+allowempty", "");
            T("combo-stupci", Ponasanje.Dx("combo").IndexOf("ValueMember=stupac0", StringComparison.Ordinal) >= 0, "");
            var tipfel = new Dictionary<string, string>();
            tipfel["WUDTH"] = "1500";
            tipfel["TREE33"] = "Danas:FORM";
            tipfel["DEFALUT"] = "today";
            tipfel["GRUOPBYBOXVISIBLE"] = "";
            T("tipfel-ne-popravlja", Ponasanje.Procitano(tipfel, "WIDTH") == null
                && Ponasanje.Procitano(tipfel, "TREE") == null
                && Ponasanje.Procitano(tipfel, "DEFAULT") == null
                && Ponasanje.Procitano(tipfel, "GROUPBYBOXVISIBLE") == null, "");
            T("label-ne-salje", !Ponasanje.LabelSaljeParametar(), "");
            T("status-upper", Ponasanje.StatusTekst("abc") == "ABC", "");
            var ok = Ponasanje.AssemblyShell("1", "7", "cs", "select 1", "@a=1", true, true);
            var lose = Ponasanje.AssemblyShell("1", "7", "cs", "select 1", "@a=1", false, true);
            T("assembly-shell-pet", ok.Argumenata == 5 && ok.ZadnjiSql == "select 1" && ok.ParametriGrida == "@a=1" && !ok.Prekinuto, "");
            T("assembly-prekine", lose.Prekinuto, "");
            T("datetiem-je-datum", NgType.Odredi("datetiem") == NgGrana.Date && NgType.Odredi("CMEditVB6.CurMEdit") == NgGrana.Number, "");
            T("onloaded-u-loadu", Motora.OnLoadedULoadu() && !Motora.OnLoadedPrijePrikaza(1) && Motora.OnLoadedPrijePrikaza(0), "");
            string[] dij = Motora.Totalgen("dijalog", "1", "7", "cs", "sql", "grid", "@CarpoForm='a'");
            string[] sh = Motora.Totalgen("shell", "1", "7", "cs", "sql", "grid", "@CarpoForm='a'");
            string[] raf = Motora.Totalgen("rafalno", "1", "7", "cs", "sql", "grid", "@CarpoForm='a'");
            T("totalgen-dijalog", dij[3] == "@CarpoForm='a'" && dij[4] == dij[3], "");
            T("totalgen-shell", sh[3] == "sql" && sh[4] == "grid" && !Motora.CitaPovrat(), "");
            T("totalgen-rafalno", raf[4].Length == 0 && raf[3] == "sql", "");
            T("tilda-router", Motora.Router("ADAPTER_FORM~REFRESHMASTERGRID") == "lanac:ADAPTER_FORM#REFRESHMASTERGRID"
                && Motora.Router("ASSEMBLYLOAD(x)").StartsWith("jedan:", StringComparison.Ordinal), "");
            T("assembly-ne-cita-povrat", !Motora.CitaPovrat(), "");
            T("print-master-isti", IspisMotora.JeMaster("PRINTMASTER") && IspisMotora.JeMaster("PRINTREPXMASTER"), "");
            T("print-ne-gleda-orijentaciju", !IspisMotora.GledaOrijentaciju(), "");
            T("repx-prefiks", IspisMotora.Prefiks(true, ":repx/rptX") == ":repx:/rptX" && IspisMotora.Prefiks(false, ":repx/rptX") == ":repx:rptX", "");
            T("spin-v26", IspisMotora.Verzija() == "v26" && IspisMotora.Fallback() == "rptDevRepx", "");
            T("vrstadok-nije-forma", IspisMotora.JeVrstaDok(":vrstadokid-mpblagajnagla") && !IspisMotora.JeVrstaDok("FILTERFORM"), "");
            T("multiselect-prije-ispisa", IspisMotora.BriseTemp("MULTISELECTISPIS_FORM") && !IspisMotora.BriseTemp("ISPIS_FORM"), "");
            T("web-url-upper", Rijetke.WebAdresa("http://a", "x") == "HTTP://A", "");
            T("web-refresh-nije", Rijetke.WebAdresa(null, "refresh") == "" && Rijetke.WebAdresa(null, "http://arkod") == "http://arkod", "");
            T("pic-location-upper", Rijetke.LokacijaSlike(@"\\server\a.jpg") == @"\\SERVER\A.JPG" && Rijetke.HttpLoad("http://a") && Rijetke.Stretch() == "stretch", "");
            T("pic-http-load", Rijetke.HttpLoad("https://a") && !Rijetke.HttpLoad(@"C:\a.jpg"), "");
            T("nextform-prva", Rijetke.Sljedeca("IZMJENI_FORM") == "IZMJENI_FORM" && Rijetke.DozvolaIme("INSERT_FORM", "IZMJENI_FORM") == "INSERT_FORM", "");
            T("clipboard-filter", Rijetke.Clipboard("FILTERFORM", "") && !Rijetke.Clipboard("FILTERFORM", "false"), "");
            T("clipboard-pad", !Rijetke.ClipboardPadStvaraTablicu(), "");
            T("grid-dijalog-stupac1", Rijetke.SkriveniSelector() == 1, "");
            T("skip-default-load", Rijetke.PreskociPocetno("TRUE") && !Rijetke.PreskociPocetno("false"), "");
            T("setdefaults-hash", Rijetke.SetDefaults("select 1#select 2").Length == 2, "");
            var dijStablo = new Stablo();
            dijStablo.NaDijalogu = true;
            dijStablo.Ucitaj(Projekt("TREE", "yearmonth:FORM"), null);
            T("stablo-dijalog-yearmonth", dijStablo.Stanje.Korijen.Count == 0, dijStablo.Stanje.Korijen.Count.ToString());
            var tab = new DataTable();
            tab.Columns.Add("a"); tab.Columns.Add("b"); tab.Columns.Add("c"); tab.Columns.Add("d");
            tab.Rows.Add("1", "", "Klase", "abc");
            dijStablo.Jezik = "eng";
            dijStablo.Rjecnik = new Dictionary<string, string>();
            dijStablo.Rjecnik["Klase"] = "Classes";
            dijStablo.Ucitaj(ProjektS("TREE", "spTreeX:FORMA", "TREETRANSLATE", "true"), delegate (string q) { return tab; });
            T("stablo-dijalog-tag", dijStablo.Stanje.Korijen.Count == 1 && dijStablo.Stanje.Korijen[0].Tag == "abc" && dijStablo.Stanje.Korijen[0].Natpis == "Klase" && dijStablo.Stanje.Korijen[0].Ikona == 0, dijStablo.Stanje.Korijen.Count == 0 ? "" : dijStablo.Stanje.Korijen[0].Tag);
            T("node-null-se-salje", SqlTekst.Sredi("spKntAOPOBRASCIQuery @id='@@node'", new SqlKontekst()).IndexOf("@id='null'", StringComparison.Ordinal) >= 0
                && SqlTekst.Odluka(SqlTekst.Sredi("spKntAOPOBRASCIQuery @id='@@node'", new SqlKontekst())) == "salji", "");
            var comboPrazan = new Stablo();
            comboPrazan.Ucitaj(Projekt("COMBO", "select 'spTreeX:FORM' as tree, 'Vrijeme' as opis"), null);
            T("combo-prvi-prolaz", comboPrazan.Stanje.PrviProlaz == "izlaz" && comboPrazan.Stanje.Vidljivo && comboPrazan.Stanje.Splitter == 243 && !comboPrazan.Stanje.GumbDrvo && comboPrazan.Stanje.Korijen.Count == 0, comboPrazan.Stanje.PrviProlaz);
            var comboDt = new DataTable();
            comboDt.Columns.Add("id");
            comboDt.Columns.Add("parentid");
            comboDt.Columns.Add("caption");
            comboDt.Columns.Add("value");
            comboDt.Rows.Add("1", "", "Danas", "1:FORM");
            comboPrazan.IzTeksta("spTreeX:FORM", ProjektS("TREE", "", "SPLITTERDISTANCE", "180"), delegate (string q) { return comboDt; });
            T("combo-puni-drvo", comboPrazan.Stanje.PrviProlaz.Length == 0 && comboPrazan.Stanje.Splitter == 180 && comboPrazan.Stanje.GumbDrvo && comboPrazan.Stanje.Korijen.Count == 1 && comboPrazan.Stanje.Korijen[0].Tag == "1:FORM", comboPrazan.Stanje.Korijen.Count.ToString());
            int z = 0;
            var auto = Raspored.Izracunaj(NgGrana.Text, Bag("CAPTION", "Naziv"), ref z);
            T("raspored-auto", auto.Vrh == 10 && auto.Lijevo == 140 && auto.Natpis && auto.NatpisLijevo == 10 && auto.NatpisTekst == "Naziv", auto.Vrh + "/" + auto.NatpisLijevo);
            var drugi = Raspored.Izracunaj(NgGrana.Text, Bag("CAPTION", "Mjesto"), ref z);
            T("raspored-drugi", drugi.Vrh == 40 && drugi.NatpisLijevo == 10, drugi.Vrh.ToString());
            var sirina = Raspored.Izracunaj(NgGrana.Text, Bag("TOP", "280", "LEFT", "1960", "WIDTH", "2800", "CAPTION", "A"), ref z);
            T("raspored-width", sirina.Vrh == 20 && sirina.Lijevo == 140 && sirina.Sirina == 200 && !sirina.Natpis, sirina.Sirina.ToString());
            var wudth = Raspored.Izracunaj(NgGrana.Number, Bag("WUDTH", "2800", "TOP", "140", "CAPTION", "Iznos"), ref z);
            T("raspored-wudth", wudth.Sirina == 0 && wudth.Vrh == 10, wudth.Sirina.ToString());
            int z2 = 0;
            var zv = Raspored.Izracunaj(NgGrana.Text, Bag("CAPTION", "Naziv*", "TOP", "280", "LEFT", "1960"), ref z2);
            T("raspored-zvjezdica", zv.Vrh == 20 && zv.Lijevo == 140 && zv.Natpis && zv.NatpisTekst == "Naziv" && zv.NatpisLijevo == 100, zv.NatpisLijevo.ToString());
            var rad = Raspored.Izracunaj(NgGrana.Radio, Bag("CAPTION", "Da", "DEFAULT", "1", "TOP", "140"), ref z2);
            T("raspored-radio", rad.Tekst == "Da" && rad.Cekirano, rad.Tekst);
            var tx = Raspored.Izracunaj(NgGrana.Text, Bag("CAPTION", "X", "DEFAULT", "a_b!!c", "TOP", "140"), ref z2);
            T("raspored-default", tx.Tekst == "ab\r\nc", tx.Tekst);
            int z3 = 0;
            var ch = Raspored.Izracunaj(NgGrana.Check, Bag("CAPTION", "Aktivan"), ref z3);
            T("raspored-check", ch.Vrh == 10 && !ch.Natpis, "");
            int z4 = 5;
            var lb = Raspored.Izracunaj(NgGrana.Label, Bag("CAPTION", "Info"), ref z4);
            T("raspored-label", lb.Vrh == 0 && !lb.Natpis && z4 == 5, z4.ToString());
            var en = Raspored.Izracunaj(NgGrana.Text, Bag("ENABLED", "false", "TOP", "140", "CAPTION", "A"), ref z4);
            T("raspored-enabled", en.ImaUkljuceno && !en.Ukljuceno, "");
            int fw;
            int fh;
            Raspored.Velicina(Bag("WIDTH", "6720", "HEIGHT", "4480"), out fw, out fh);
            T("raspored-forma", fw == 480 && fh == 320, fw + "x" + fh);
            T("editor-grana", Raspored.Editor(NgGrana.Text) == "TextEdit"
                && Raspored.Editor(NgGrana.Number) == "SpinEdit"
                && Raspored.Editor(NgGrana.Date) == "DateEdit"
                && Raspored.Editor(NgGrana.Combo) == "LookUpEdit"
                && Raspored.Editor(NgGrana.Radio) == "CheckEdit"
                && Raspored.Editor(NgGrana.Search) == "ButtonEdit"
                && Raspored.Editor(NgGrana.PictureBox) == "PictureEdit"
                && Raspored.Editor(NgGrana.WebBrowser) == "WebBrowser"
                && Raspored.Editor(NgGrana.Button) == "SimpleButton"
                && Raspored.Editor(NgGrana.StatusStrip) == "LabelControl"
                && Raspored.Editor(NgGrana.Nijedna).Length == 0, "");
            T("rub-filter", Raspored.Rub("3") == "FixedDialog" && Raspored.Rub("") == "" && Raspored.Rub("4") == "Sizable", Raspored.Rub("3"));
        }

        private static void KomandeTestovi()
        {
            T("sp-lijepi-parametre", Lanac.Lijepi("spKultureNGAzur @Action=0,", "@Cijena=''")
                .EndsWith("@Cijena=''"), "");
            T("dml-ne-lijepi", Lanac.Lijepi("select 1", "@a=1") == "select 1", "");
            T("uzvicnik-gasi-lijepljenje", Lanac.Lijepi("spX @Action=0 !!", "@a=1").IndexOf("@a") < 0, "");
            T("caption-novi-red", Lanac.Caption("a!!b") == "a\r\nb", "");
            T("prazan-broj", Lanac.PrazanBroj() == "0", "");
            T("prazan-tekst", Lanac.PrazanTekst() == "''", "");
            T("apostrof", Lanac.USql(Lanac.TekstVrijednost("a'b")) == "'a''b'", "");
            T("search-tag", Lanac.SearchParametar("") == "null" && Lanac.SearchParametar("5") == "5", "");
            T("search-tablica", Lanac.SearchTablica("roba#robaid#userid%#!cast(userid as varchar)#%naziv%") == "roba", "");
            T("datum", Lanac.Datum(2026, 1, 1) == "20260101" && Lanac.PrviDanGodine(2026) == "01.01.2026", "");
            T("multidrop", Lanac.MultiDrop(new[] { "1", "2" }) == "1~2", "");
            T("allowempty", Lanac.AllowEmpty("TRUE") && !Lanac.AllowEmpty("false"), "");
            var prekid = Lanac.Izvrsi("spRaiserror 'x'#OK", true, "nema ok");
            T("raiserror-prekida", prekid.Prekinuto && !prekid.Zatvara, "");
            var guta = Lanac.Izvrsi("spRaiserror 'x'#OK", true, "poruka #OK");
            T("hash-ok-guta", !guta.Prekinuto && guta.Zatvara, "");
            var fokus = Lanac.Izvrsi("spRaiserror 'x'#OK", true, "+focus(partneriid)");
            T("focus-nije-ok", fokus.Fokus == "partneriid" && fokus.Prekinuto, "");
            T("prazan-segment", Lanac.Izvrsi("##OK", true, null).Zatvara, "");
            T("okstay", Lanac.Izvrsi("OKSTAY", true, null).Ostaje && !Lanac.Izvrsi("OKSTAY", true, null).Zatvara, "");
            T("close-button2", Lanac.Izvrsi("CLOSE", true, null).Button2 && Lanac.Izvrsi("CLOSEPARENT", true, null).CloseParent, "");
            T("tilde-u-lanacu", Lanac.Izvrsi("REFRESHMASTERGRID~CLOSE", true, null).RefreshMasterGrid, "");
            T("refreshmaster-nije-grid", !Lanac.Izvrsi("REFRESHMASTER", true, null).RefreshMasterGrid, "");
            var fe = Lanac.Izvrsi("FOREACHSELECTED(x)#spAzur", true, null);
            var podloga = Lanac.Izvrsi("FOREACHROW(x)#spAzur", false, null);
            T("foreach-dijalog", fe.Foreach == "cekirani" && fe.Odigrano.Count == 1 && fe.Odigrano[0] == "spAzur", "");
            T("foreach-podloga", podloga.Foreach.Length == 0 && podloga.Odigrano.Count == 0, "");
            T("foreach-plavi", Lanac.Izvrsi("FOREACHBLUEROW(x)#spAzur", true, null).Foreach == "plavi", "");
            T("refresh-sql", Lanac.RefreshSql("ime") == "select 'refresh' as ime" && Lanac.RefreshControl() == "select refresh", "");
            KontroleTestovi();
        }

        private static void StabloTestovi()
        {
            var prazno = new Stablo();
            prazno.Ucitaj(Ucitaj("JeziciNG.xml"), null);
            T("stablo-jezici-skriveno", !prazno.Stanje.Vidljivo && prazno.Stanje.Splitter == 0 && !prazno.Stanje.GumbDrvo && prazno.Stanje.Node == "null", prazno.Stanje.Splitter.ToString());
            T("stablo-obrub-none", prazno.Stanje.Obrub == "None" && prazno.Stanje.AllowDrop && prazno.Stanje.ShowPlusMinus && prazno.Stanje.ShowRootLines, "");

            var stat = new Stablo();
            stat.Ucitaj(Projekt("TREE", "Klase:FILTER#Osvježi drvo:REFRESHTREEFORM"), null);
            T("stablo-static", stat.Stanje.Vidljivo && stat.Stanje.Splitter == 290 && stat.Stanje.Korijen.Count == 2, stat.Stanje.Korijen.Count.ToString());
            T("stablo-static-tag", stat.Stanje.Korijen[0].Natpis == "Klase" && stat.Stanje.Korijen[0].Node == "" && stat.Stanje.Korijen[0].Naredba == "FILTER", stat.Stanje.Korijen[0].Tag);
            stat.Odaberi(stat.Stanje.Korijen[0]);
            T("stablo-afterselect", stat.Stanje.Node == "" && stat.Kontekst.Node == "" && stat.Stanje.ZadnjaNaredba == "FILTER", stat.Stanje.Node);

            var ym = new Stablo();
            ym.Ucitaj(Projekt("TREE", "yearmonth:FORM"), null);
            T("stablo-yearmonth", ym.Stanje.Korijen.Count == 5 && ym.Stanje.Korijen[0].Djeca.Count == 12, ym.Stanje.Korijen.Count.ToString());
            T("stablo-yearmonth-tag", ym.Stanje.Korijen[0].Tag == "20050101#20051231:FORM" && ym.Stanje.Korijen[0].Djeca[0].Tag == "2005101#2005130:FORM", ym.Stanje.Korijen[0].Djeca[0].Tag);

            var dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("parentid", typeof(string));
            dt.Columns.Add("naziv", typeof(string));
            dt.Columns.Add("vrijednost", typeof(string));
            dt.Rows.Add("1", "", "Sve", "1");
            dt.Rows.Add("5", "1", "Dio", "5");
            dt.Rows.Add("0", "1", "nula", "9");
            var store = new Stablo();
            store.Ucitaj(ProjektS("TREE", "spTreeKlMasterRoba:KLAS", "SPLITTERDISTANCE", "200"), delegate (string sql) { return dt; });
            T("stablo-splitter-200", store.Stanje.Splitter == 200, store.Stanje.Splitter.ToString());
            T("stablo-store", store.Stanje.Korijen.Count == 1 && store.Stanje.Korijen[0].Natpis == "Sve" && store.Stanje.Korijen[0].Djeca.Count == 1, store.Stanje.Korijen.Count == 0 ? "nema" : store.Stanje.Korijen[0].Djeca.Count.ToString());
            T("stablo-store-tag", store.Stanje.Korijen[0].Tag == "1:KLAS" && store.Stanje.Korijen[0].Djeca[0].Tag == "5:KLAS" && store.Stanje.Korijen[0].Djeca[0].Oznaka == "5", store.Stanje.Korijen[0].Djeca[0].Tag);
            T("stablo-store-sql", store.Stanje.SqlPozivi.Count > 0 && store.Stanje.SqlPozivi[0].IndexOf("spTreeKlMasterRoba", StringComparison.Ordinal) >= 0, "");

            var prev = new Stablo();
            prev.Ucitaj(Projekt("TREETRANSLATE", "TRUE"), null);
            T("stablo-translate-zastavica", prev.Stanje.Prevedi && !prev.Stanje.Vidljivo, "");

            var drag = new Stablo();
            drag.Ucitaj(Projekt("TREEDRAGDROP", "delete from x"), null);
            T("stablo-drag-ne-pise", drag.SqlOtpusta() == "", "");

            var prevod = new Stablo();
            prevod.Jezik = "eng";
            prevod.Rjecnik = new Dictionary<string, string>();
            prevod.Rjecnik["Klase"] = "Classes";
            prevod.Ucitaj(Projekt("TREE", "Klase:FILTER"), null);
            T("stablo-prevod", prevod.Stanje.Korijen[0].Natpis == "Classes" && prevod.Stanje.Korijen[0].Ikona == 0, prevod.Stanje.Korijen[0].Natpis);
            prevod.Jezik = "hrv";
            prevod.Ucitaj(Projekt("TREE", "Klase:FILTER"), null);
            T("stablo-hrv-ne-prevodi", prevod.Stanje.Korijen[0].Natpis == "Klase", prevod.Stanje.Korijen[0].Natpis);

            var det = new DetailGrid();
            det.Pripremi(ProjektS("DETAILGRID", "spX @id=@@id, @naziv=@@naziv", "DETCHECKBOXVISIBLE", "FALSE"));
            var redovi = new DataTable();
            redovi.Columns.Add("klmasterrobaid", typeof(int));
            redovi.Columns.Add("Naziv", typeof(string));
            redovi.Rows.Add(9, "Kruh");
            redovi.Rows.Add(3, "Mlijeko");
            string detSql = det.Slozi(redovi.Rows[0]);
            T("det-id-prvi", det.Stanje.IdMaster == "9", det.Stanje.IdMaster);
            T("det-zamjena", detSql.IndexOf("@id=9", StringComparison.Ordinal) >= 0 && detSql.IndexOf("@naziv='Kruh'", StringComparison.Ordinal) >= 0, detSql);
            T("det-checkbox-skriven", !det.Stanje.CheckboxVidljiv, "");
            string sql2 = det.Slozi(redovi.Rows[1]);
            T("det-refresh-predlozak", sql2.IndexOf("@id=3", StringComparison.Ordinal) >= 0 && sql2.IndexOf("Mlijeko", StringComparison.Ordinal) >= 0, sql2);
            var nema = new DetailGrid();
            nema.Pripremi(Projekt("ONLOAD", "select 1"));
            T("det-prazan", nema.Stanje.Nema && nema.Slozi(null) == "", "");
            var dvotocka = new DetailGrid();
            dvotocka.Pripremi(Projekt("DETAILGRID", ":FORMA"));
            T("det-dvotocka", dvotocka.Stanje.OtvaraFormu && dvotocka.Slozi(null) == "", "");
            var vise = new DetailGrid();
            vise.Pripremi(ProjektS("DETAILGRID", "select 1 where id=@@id", "SQL2", "select 2 where id=@@id"));
            vise.Slozi(redovi.Rows[0]);
            T("det-sql2", vise.Stanje.DodatniSql.Count == 1 && vise.Stanje.DodatniSql[0].IndexOf("id=9", StringComparison.Ordinal) >= 0, vise.Stanje.DodatniSql.Count == 0 ? "" : vise.Stanje.DodatniSql[0]);

            var imena = new DataTable();
            imena.Columns.Add("ParentId", typeof(string));
            imena.Columns.Add("Id", typeof(string));
            imena.Columns.Add("Caption", typeof(string));
            imena.Columns.Add("ValueField", typeof(string));
            imena.Rows.Add("1", "", "Korijen", "1");
            imena.Rows.Add("5", "1", "Dijete", "5");
            imena.Rows.Add("0", "1", "nula", "0");
            var poImenu = new Stablo();
            poImenu.Ucitaj(Projekt("TREE", "spTreePoPoziciji:FORM"), delegate (string q) { return imena; });
            T("stablo-pozicija-ne-ime", poImenu.Stanje.Korijen.Count == 1
                && poImenu.Stanje.Korijen[0].Oznaka == "1"
                && poImenu.Stanje.Korijen[0].Natpis == "Korijen"
                && poImenu.Stanje.Korijen[0].Djeca.Count == 1
                && poImenu.Stanje.Korijen[0].Djeca[0].Oznaka == "5",
                poImenu.Stanje.Korijen.Count == 0 ? "prazno" : poImenu.Stanje.Korijen[0].Oznaka);

            int fillPoziva = 0;
            string fillSql = "";
            var at = new Stablo();
            at.Ucitaj(Projekt("TREE", "spTreeX @@nepostoji:FORM"), delegate (string q) { fillPoziva++; fillSql = q; return null; });
            T("stablo-fill-ne-preskace-atat", fillPoziva == 1 && fillSql.IndexOf("@@nepostoji", StringComparison.Ordinal) >= 0, fillSql);
        }

        private static void PisanjeIServer()
        {
            T("zabrana-insert", SqlTekst.JeZabranaPisanja(" insert into x values (1)"), "");
            T("select-nije-zabrana", !SqlTekst.JeZabranaPisanja("select * from x"), "");
            bool bacio = false;
            try { SpinVeza.Spoj("jupiter2022", "spin"); }
            catch (InvalidOperationException) { bacio = true; }
            T("ne-jupiter2022", bacio, "");
            T("localhost-ok", SpinVeza.Spoj("localhost", "spin").IndexOf("spin", StringComparison.OrdinalIgnoreCase) >= 0, "");
            var g = new MasterGrid();
            g.Ucitaj(Projekt("ONLOAD", "update klmasterroba set oznaka=1"), null);
            T("update-se-ne-salje", g.Stanje.BlokiranoPisanje && g.Stanje.Odluka == "zabrana-pisanja", g.Stanje.Odluka);
        }

        private static void SpinStabloFill()
        {
            string spoj = SpinVeza.Spoj("localhost", "spin");
            int usporedeno = 0;
            usporedeno += UsporediStablo(spoj, "spTreeVrstaDok", "POPUNI_FORM", "Statusi");
            usporedeno += UsporediStablo(spoj, "spBlagajneZakTreeNG @action=0,@operateriid=0", "MJESEC_FORM", "KntBlaZak-0");
            usporedeno += UsporediStablo(spoj, "spBlagajneZakTreeNG @action=2,@operateriid=0", "MJESEC_FORM0", "KntBlaZak-2");
            UsporediCombo(spoj);
            T("spin-stablo-barem-jedno", usporedeno > 0, "usporedjeno=" + usporedeno.ToString());
        }

        private static int UsporediStablo(string spoj, string sql, string forma, string ime)
        {
            DataTable t = FillAdapter(spoj, sql);
            if (t == null)
            {
                T("spin-stablo-" + ime, false, "Fill nije vratio tablicu");
                return 0;
            }
            var imena = new StringBuilder();
            for (int i = 0; i < t.Columns.Count; i++)
            {
                if (i > 0) imena.Append(",");
                imena.Append(i.ToString()).Append(":").Append(t.Columns[i].ColumnName);
            }
            Console.WriteLine("SPIN-STABLO " + ime + " stupci=" + imena + " redovi=" + t.Rows.Count);
            List<string> ocek = NetGenStablo(t, forma);
            List<string> dobiveno = StabloIzMotora(sql, forma, t);
            bool isto = ListeIste(ocek, dobiveno);
            T("spin-stablo-" + ime, isto, isto ? ("cvorova=" + ocek.Count) : PrvaRazlika(ocek, dobiveno));
            return t.Rows.Count > 0 && isto ? 1 : 0;
        }

        private static void UsporediCombo(string spoj)
        {
            DataTable combo = FillAdapter(spoj, "exec spNg_combo @action='Drvo_NabPri'");
            if (combo == null || combo.Rows.Count == 0 || combo.Columns.Count < 1)
            {
                T("spin-stablo-combo", false, combo == null ? "nema" : ("redovi=" + combo.Rows.Count));
                return;
            }
            string tekst = Convert.ToString(combo.Rows[0][0]) ?? "";
            Console.WriteLine("SPIN-STABLO combo stupac0=" + (tekst.Length > 120 ? tekst.Substring(0, 120) : tekst));
            string[] dijelovi = tekst.Split('#');
            int n = 0;
            for (int i = 0; i < dijelovi.Length; i++)
            {
                int k = dijelovi[i].IndexOf(':');
                if (k < 0)
                    continue;
                string lijevo = dijelovi[i].Substring(0, k).Trim();
                if (lijevo.ToLowerInvariant().IndexOf("tree", StringComparison.Ordinal) < 0)
                    continue;
                string desno = dijelovi[i].Substring(k + 1).Trim();
                n += UsporediStablo(spoj, lijevo, desno, "PrijemniList-" + n);
                if (n > 0)
                    break;
            }
            if (n == 0)
                T("spin-stablo-combo", false, "nema store segmenta u stupcu 0");
        }

        private static List<string> StabloIzMotora(string sql, string forma, DataTable t)
        {
            var s = new Stablo();
            s.Ucitaj(Projekt("TREE", sql + ":" + forma), delegate (string q) { return t; });
            var linije = new List<string>();
            for (int i = 0; i < s.Stanje.Korijen.Count; i++)
                Spljosti(s.Stanje.Korijen[i], "", linije);
            return linije;
        }

        private static void Spljosti(Cvor c, string roditelj, List<string> linije)
        {
            linije.Add(c.Oznaka + "\t" + roditelj + "\t" + c.Natpis + "\t" + c.Tag);
            for (int i = 0; i < c.Djeca.Count; i++)
                Spljosti(c.Djeca[i], c.Oznaka, linije);
        }

        /// <summary>
        /// Neovisna kopija Form1.DazovaIzStore: samo pozicije 0..3, prvi red, dijete "0" se ne crta.
        /// Dupli stupac 0 = relacija padne, stablo prazno.
        /// </summary>
        private static List<string> NetGenStablo(DataTable t, string forma)
        {
            var linije = new List<string>();
            if (t == null || t.Rows.Count == 0 || t.Columns.Count < 3)
                return linije;
            var jednom = new Dictionary<string, bool>(StringComparer.Ordinal);
            for (int i = 0; i < t.Rows.Count; i++)
            {
                string id = Celija(t.Rows[i], 0);
                if (id.Length == 0)
                    continue;
                if (jednom.ContainsKey(id))
                    return new List<string>();
                jednom[id] = true;
            }
            NetGenCvor(t, t.Rows[0], "", forma, linije, new Dictionary<DataRow, bool>());
            return linije;
        }

        private static void NetGenCvor(DataTable t, DataRow red, string roditelj, string forma, List<string> linije, Dictionary<DataRow, bool> vidjeni)
        {
            string id = Celija(red, 0);
            string tag = t.Columns.Count > 3 ? Celija(red, 3) : "";
            if (tag.IndexOf(':') < 0)
                tag = tag + ":" + forma;
            linije.Add(id + "\t" + roditelj + "\t" + Celija(red, 2) + "\t" + tag);
            for (int i = 0; i < t.Rows.Count; i++)
            {
                DataRow dr = t.Rows[i];
                if (dr == red || vidjeni.ContainsKey(dr))
                    continue;
                if (Celija(dr, 1) != id)
                    continue;
                if (Celija(dr, 0) == "0")
                    continue;
                vidjeni[dr] = true;
                NetGenCvor(t, dr, id, forma, linije, vidjeni);
            }
        }

        private static string Celija(DataRow dr, int i)
        {
            if (dr == null || i >= dr.Table.Columns.Count || dr[i] == null || dr[i] == DBNull.Value)
                return "";
            return dr[i].ToString().Trim();
        }

        private static bool ListeIste(List<string> a, List<string> b)
        {
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!string.Equals(a[i], b[i], StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        private static string PrvaRazlika(List<string> a, List<string> b)
        {
            int n = a.Count < b.Count ? a.Count : b.Count;
            for (int i = 0; i < n; i++)
            {
                if (!string.Equals(a[i], b[i], StringComparison.Ordinal))
                    return "idx " + i + " ocek [" + a[i] + "] dobiveno [" + b[i] + "]";
            }
            return "duljina " + a.Count + "/" + b.Count;
        }

        private static DataTable FillAdapter(string spoj, string sql)
        {
            try
            {
                var dt = new DataTable();
                using (var conn = new SqlConnection(spoj))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 60;
                    conn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SPIN-STABLO FILL " + sql + " " + ex.Message);
                return null;
            }
        }

        private static void SpinDetalj()
        {
            string korijen = null;
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir != null && korijen == null)
            {
                string a = Path.Combine(dir.FullName, "definitions", "netgen");
                if (Directory.Exists(a))
                    korijen = dir.FullName;
                dir = dir.Parent;
            }
            if (korijen == null)
            {
                Console.WriteLine("SPIN-DETALJ nema definitions/netgen");
                return;
            }
            string[] kandidati = {
                Path.Combine(korijen, "definitions", "netgen", "BIOVEGA", "DobUgovori.xml"),
                Path.Combine(korijen, "definitions", "netgen", "Agronom granule", "Statusi.xml"),
                Path.Combine(korijen, "definitions", "netgen", "Agronom granule", "KntKompVerizne.xml")
            };
            string spoj = SpinVeza.Spoj("localhost", "spin");
            for (int i = 0; i < kandidati.Length; i++)
            {
                string put = kandidati[i];
                string ime = Path.GetFileName(put);
                if (!File.Exists(put))
                {
                    Console.WriteLine("SPIN-DETALJ " + ime + " nema fajla");
                    continue;
                }
                var r = NetGenXmlReader.Ucitaj(put);
                if (!r.Uspjeh || r.Projekt == null)
                {
                    Console.WriteLine("SPIN-DETALJ " + ime + " XML " + r.Greska);
                    continue;
                }
                var kontekst = new SqlKontekst();
                kontekst.Node = "0";
                kontekst.OperaterId = "0";
                var st = new Stablo();
                st.Kontekst = kontekst;
                st.ImeXml = ime;
                string treeG = "nema";
                if (!string.IsNullOrWhiteSpace(r.Projekt.Tree))
                {
                    st.Ucitaj(r.Projekt, delegate (string sql) { return SpinCitaj(spoj, sql); });
                    treeG = st.Stanje.Korijen.Count > 0 ? ("cvorova=" + st.Stanje.Korijen.Count) : "prazno";
                }
                var g = new MasterGrid();
                g.Kontekst = kontekst;
                g.Ucitaj(r.Projekt, null);
                string masterG = g.Stanje.Odluka;
                if (g.Stanje.Odluka == "salji")
                {
                    g.IzvrsiCitanje(spoj);
                    masterG = string.IsNullOrEmpty(g.Stanje.SqlGreska)
                        ? ("redovi=" + (g.Stanje.Tablica == null ? 0 : g.Stanje.Tablica.Rows.Count))
                        : ("FAIL " + g.Stanje.SqlGreska);
                }
                var d = new DetailGrid();
                d.Kontekst = kontekst;
                d.Pripremi(r.Projekt);
                string detG = d.Stanje.Nema ? "nema" : "ima";
                if (!d.Stanje.Nema && !d.Stanje.OtvaraFormu && g.Stanje.Tablica != null && g.Stanje.Tablica.Rows.Count > 0)
                {
                    string sql = d.Slozi(g.Stanje.Tablica.Rows[0]);
                    DataTable dt = SpinCitaj(spoj, sql);
                    detG = dt == null ? "FAIL" : ("redovi=" + dt.Rows.Count);
                }
                Console.WriteLine("SPIN-DETALJ " + ime + " stablo=" + treeG + " master=" + masterG + " detail=" + detG);
            }
        }

        private static DataTable SpinCitaj(string spoj, string sql)
        {
            if (string.IsNullOrWhiteSpace(sql) || SqlTekst.JeZabranaPisanja(sql))
                return null;
            try
            {
                var dt = new DataTable();
                using (var conn = new SqlConnection(spoj))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 30;
                    conn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SPIN-DETALJ SQL " + ex.Message);
                return null;
            }
        }

        private static void Spin()
        {
            var ozn = Ucitaj("OznakaKL.xml");
            var jez = Ucitaj("JeziciNG.xml");
            T("spin-fajlovi", ozn != null && jez != null, "");
            if (ozn == null || jez == null)
                return;
            ProvjeriSpin(ozn, "oznaka", 5);
            ProvjeriSpin(jez, "jezici", 1);
        }

        private static void ProvjeriSpin(Carpo.NetGen.Model.NgProject p, string ime, int minKolona)
        {
            var g = new MasterGrid();
            g.Ucitaj(p, null);
            T(ime + "-sql-citanje", g.Stanje.Odluka == "salji" && !SqlTekst.JeZabranaPisanja(g.Stanje.SqlZaSlanje), g.Stanje.Odluka + " " + g.Stanje.SqlZaSlanje);
            if (g.Stanje.Odluka != "salji")
                return;
            g.IzvrsiCitanje(SpinVeza.Spoj("localhost", "spin"));
            if (!string.IsNullOrEmpty(g.Stanje.SqlGreska))
            {
                Console.WriteLine("SPIN " + ime + " NETGEN-CATCH " + g.Stanje.SqlGreska);
                T(ime + "-netgen-catch", g.Stanje.Kolone.Count == 0 && g.Stanje.Tablica == null && !string.IsNullOrEmpty(g.Stanje.Poruka), g.Stanje.Poruka);
                using (var c = new SqlConnection(SpinVeza.Spoj("localhost", "spin")))
                {
                    c.Open();
                    IspisiKontekst(c, ime);
                }
                return;
            }
            DataTable dt = g.Stanje.Tablica;
            int podataka = 0;
            for (int i = 0; i < g.Stanje.Kolone.Count; i++)
            {
                if (!g.Stanje.Kolone[i].Selektor && g.Stanje.Kolone[i].Podatak != "Icon")
                    podataka++;
            }
            T(ime + "-redovi", dt.Rows.Count > 0, dt.Rows.Count.ToString());
            T(ime + "-kolone", podataka >= minKolona, podataka.ToString());
            T(ime + "-fokus", g.Stanje.Tablica != null && g.Stanje.Id != "", g.Stanje.FocusRazlog + " id=" + g.Stanje.Id);
            Console.WriteLine("SPIN " + ime + " redovi=" + dt.Rows.Count.ToString() + " kolone=" + podataka.ToString());
        }

        private static void IspisiKontekst(SqlConnection c, string ime)
        {
            try
            {
                using (var cmd = new SqlCommand("select count(*) from klmasterroba", c))
                {
                    object n = cmd.ExecuteScalar();
                    Console.WriteLine("SPIN " + ime + " klmasterroba-redovi=" + n);
                }
                using (var cmd = new SqlCommand("select column_name from information_schema.columns where table_name='klmasterroba' and column_name like '%oznak%' order by column_name", c))
                using (var r = cmd.ExecuteReader())
                {
                    var sb = new StringBuilder();
                    while (r.Read())
                    {
                        if (sb.Length > 0)
                            sb.Append(",");
                        sb.Append(r.GetString(0));
                    }
                    Console.WriteLine("SPIN " + ime + " klmasterroba-oznake=" + sb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SPIN " + ime + " kontekst " + ex.Message);
            }
        }

        private static NgAttributeBag Bag(params string[] kv)
        {
            var d = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i + 1 < kv.Length; i += 2)
                d[kv[i]] = kv[i + 1];
            return new NgAttributeBag(d);
        }

        private static bool Ima(MasterGrid g, string s)
        {
            for (int i = 0; i < g.Stanje.DxPostavke.Count; i++)
            {
                if (g.Stanje.DxPostavke[i] == s)
                    return true;
            }
            return false;
        }

        private static DataTable TablicaId(int id)
        {
            var t = new DataTable();
            t.Columns.Add("id", typeof(int));
            t.Rows.Add(id);
            return t;
        }

        private static Carpo.NetGen.Model.NgProject Projekt(string attr, string vrijednost)
        {
            return ProjektS(attr, vrijednost, null, null);
        }

        private static Carpo.NetGen.Model.NgProject ProjektS(string a, string av, string b, string bv)
        {
            string xml = "<PROJECT " + a + "=\"" + Escape(av) + "\"";
            if (b != null)
                xml += " " + b + "=\"" + Escape(bv) + "\"";
            xml += "><NAME>t</NAME><CAPTION>t</CAPTION></PROJECT>";
            var put = Path.Combine(Path.GetTempPath(), "ng-" + Guid.NewGuid().ToString("N") + ".xml");
            File.WriteAllText(put, xml);
            var r = NetGenXmlReader.Ucitaj(put);
            File.Delete(put);
            return r.Projekt;
        }

        private static string Escape(string s)
        {
            return (s ?? "").Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;");
        }

        private static Carpo.NetGen.Model.NgProject Ucitaj(string ime)
        {
            var starts = new List<string>();
            starts.Add(AppDomain.CurrentDomain.BaseDirectory);
            starts.Add(Environment.CurrentDirectory);
            for (int s = 0; s < starts.Count; s++)
            {
                var dir = new DirectoryInfo(starts[s]);
                while (dir != null)
                {
                    string a = Path.Combine(dir.FullName, "definitions", "netgen");
                    string b = Path.Combine(dir.FullName, "definitions", "Prepisane_granule", "Netgen");
                    string hit = Pronadji(a, ime);
                    if (hit == null)
                        hit = Pronadji(b, ime);
                    if (hit != null)
                        return NetGenXmlReader.Ucitaj(hit).Projekt;
                    dir = dir.Parent;
                }
            }
            return null;
        }

        private static string Pronadji(string korijen, string ime)
        {
            if (string.IsNullOrEmpty(korijen) || !Directory.Exists(korijen))
                return null;
            try
            {
                foreach (string f in Directory.GetFiles(korijen, "*.xml", SearchOption.AllDirectories))
                {
                    if (string.Equals(Path.GetFileName(f), ime, StringComparison.OrdinalIgnoreCase))
                        return f;
                }
                foreach (string f in Directory.GetFiles(korijen, "*.XML", SearchOption.AllDirectories))
                {
                    if (string.Equals(Path.GetFileName(f), ime, StringComparison.OrdinalIgnoreCase))
                        return f;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            return null;
        }
    }

    public sealed class LazniCis
    {
        public string CisUrl { get; set; }
        public string TimeOut { get; set; }
        public string NazivMapeZahtjev { get; set; }
        public string NazivMapeOdgovor { get; set; }
        public bool NazivAutoGeneriranje { get; set; }
        public string OdgovorGreska { get; set; }
        public string OdgovorGreskaStatus { get; set; }
        public string Xml;
        public string Pfx;
        public int Poziva;
        public bool Baci;

        public string PosaljiSoapPorukuCertifikatDatoteka(string xml, string pfx, string lozinka)
        {
            Poziva++;
            Xml = xml;
            Pfx = pfx;
            if (Baci)
            {
                OdgovorGreska = "sifra";
                OdgovorGreskaStatus = "err";
                throw new InvalidOperationException("cis");
            }
            return "<Odgovor><Jir>JIR-TEST</Jir></Odgovor>";
        }

        public string DohvatiJir(string odgovor)
        {
            return "JIR-TEST";
        }

        public string DohvatiGresku(string greska)
        {
            return "opis-cis";
        }
    }

    public sealed class LazniWord
    {
        public bool Visible { get; set; }
        public string Dodan;
        public string Makro;
        public string Id;
        public object Documents { get { return this; } }

        public object Add(object putanja)
        {
            Dodan = Convert.ToString(putanja);
            return this;
        }

        public void Run(object ime, object idjevi, object veza)
        {
            Makro = Convert.ToString(ime);
            var niz = idjevi as string[];
            Id = niz != null && niz.Length > 0 ? niz[0] : "";
        }
    }

    public sealed class LazniSpin
    {
        public static string Metoda;
        public static string Konekcija;
        public static object Kontrola;
        public static bool Landscape;
        public static string Caption;
        public static string Header;
        public static string Sub;
        public static string Predlozak;
        public static string Parametri;
        public static string Tko;
        public static string Putanja;
        public static string Id;
        public static bool Baci;
        public int isnetgen;

        public void PrintLink(object konekcija, object operater, object kontrola, System.Collections.Generic.Dictionary<string, string> zaglavlje, System.Data.DataTable combo, System.Data.DataSet podaci, System.IO.Stream raspored, bool landscape)
        {
            Metoda = "PrintLink";
            Konekcija = Convert.ToString(konekcija);
            Kontrola = kontrola;
            Landscape = landscape;
            Caption = zaglavlje["caption"];
            Header = zaglavlje["header"];
            Sub = zaglavlje["subHeader"];
        }

        public void Print(object konekcija, object predlozak, object parametri, object tko)
        {
            if (Baci)
                throw new InvalidOperationException("pad");
            Metoda = "Print";
            Konekcija = Convert.ToString(konekcija);
            Predlozak = Convert.ToString(predlozak);
            Parametri = Convert.ToString(parametri);
            Tko = Convert.ToString(tko);
        }

        public void DoPrint2(object konekcija, object id, object putanja, object report, object kopija, object pisac, object parametri)
        {
            Metoda = "DoPrint2";
            Konekcija = Convert.ToString(konekcija);
            Id = Convert.ToString(id);
            Putanja = Convert.ToString(putanja);
            Predlozak = Convert.ToString(report);
            Parametri = Convert.ToString(parametri);
        }
    }

    public sealed class TgSonda
    {
        public static string[] Zadnji;
        public static string Vratio = "";

        public string totalgen(object id, object operater, object konekcija, object zadnji, object parametri)
        {
            Zadnji = new[]
            {
                Convert.ToString(id),
                Convert.ToString(operater),
                Convert.ToString(konekcija),
                Convert.ToString(zadnji),
                Convert.ToString(parametri)
            };
            Vratio = "NE";
            return Vratio;
        }
    }
}
