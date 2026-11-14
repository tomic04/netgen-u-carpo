using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Poziv;
using Carpo.NetGen.Print;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Ui;
using Carpo.NetGen.TestHooks;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.GridTest
{
    internal static class Program
    {
        private static int fail;

        [STAThread]
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
            BatIPost();
            RazlikeImenom();
            Podaci();
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
                TragaNaStroju();
                LoaderOtvara();
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
                cis.Baci = false;
                var postavke = new System.Data.DataTable();
                postavke.Columns.Add("url");
                postavke.Columns.Add("pfx");
                postavke.Columns.Add("lozinkazapfx");
                postavke.Columns.Add("timeout");
                postavke.Columns.Add("datum", typeof(DateTime));
                postavke.Rows.Add(Carpo.NetGen.Komande.FiskalTok.PravaUrl, "a.pfx", "tajna", "1000", DateTime.Now.AddDays(-1));
                int prijePrave = cis.Poziva;
                var prava = Carpo.NetGen.Komande.FiskalTok.Izvrsi(1, "9", delegate(string sql) { return postavke; });
                T("fiskal-izvrsi-prava", !prava.Poslano && cis.Poziva == prijePrave, prava.Greska);
                postavke.Rows[0]["url"] = Carpo.NetGen.Komande.FiskalTok.TestUrl;
                var xmlTab = new System.Data.DataTable();
                xmlTab.Columns.Add("x");
                xmlTab.Rows.Add("<Racun zki=\"ZZZZZZZZZZ\" uuid=\"UUUUUUUUUU\"/>");
                var jirTab = new System.Data.DataTable();
                jirTab.Columns.Add("jir");
                jirTab.Rows.Add("");
                var testni = Carpo.NetGen.Komande.FiskalTok.Izvrsi(1, "9", delegate(string sql)
                {
                    if (sql.StartsWith("select * from mpfiskalizacija", StringComparison.Ordinal))
                        return postavke;
                    if (sql.StartsWith("select isnull(jir", StringComparison.Ordinal))
                        return jirTab;
                    return xmlTab;
                });
                T("fiskal-izvrsi-test", testni.Poslano && cis.CisUrl == Carpo.NetGen.Komande.FiskalTok.TestUrl
                    && testni.Uuid == "UUID-TEST" && testni.Zki == "ZKI-TEST"
                    && cis.Xml.IndexOf("ZKI-TEST", StringComparison.Ordinal) >= 0
                    && cis.Xml.IndexOf("UUID-TEST", StringComparison.Ordinal) >= 0, testni.Greska + " " + cis.Xml);
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
            FiskalUuidSpin();
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
            T("dijalog-msgbox-lanac", msg.Count == 3 && msg[0].Vrsta == "datoteka" && msg[1].Vrsta == "poruka" && msg[1].Tekst == "POZOR" && msg[2].Vrsta == "ok", msg.Count.ToString());
            var rod = Carpo.NetGen.Komande.DijalogTok.Koraci("spMPIzdatnicaSKLMP @action=1#CLOSEPARENT");
            T("dijalog-closeparent-lanac", rod.Count == 2 && rod[0].Vrsta == "sql" && rod[0].Tekst.StartsWith("spMPIzdatnicaSKLMP") && rod[1].Vrsta == "roditelj", rod.Count.ToString());
            var fe = Carpo.NetGen.Komande.DijalogTok.Koraci("FOREACHSELECTED(gridOsobe)#spAzur @id=@@id#OK");
            string[] petlja = Carpo.NetGen.Komande.ForeachNaredba.Ponovi("spAzur @id=@@id", new[] { "4", "9" });
            T("foreach-dijalog", fe.Count == 2 && fe[0].Vrsta == "foreach" && fe[0].Tekst.StartsWith("cekirani|GRIDOSOBE|")
                && petlja.Length == 2 && petlja[0] == "spAzur @id=4" && petlja[1] == "spAzur @id=9"
                && Carpo.NetGen.Komande.ForeachNaredba.Idjevi("plavi", new[] { "1" }, new[] { "8", "3" }, new string[0]).Length == 2
                && Carpo.NetGen.Komande.ForeachNaredba.Ponovi("sp", new string[0]).Length == 0, fe[0].Tekst);
            T("param-tekst", Carpo.NetGen.Komande.ParametriSvi.Tekst("naziv", "O'Brien, d.o.o.") == "@naziv='O¸¸Brien~~ d.o.o.',", "");
            T("param-broj", Carpo.NetGen.Komande.ParametriSvi.Broj("kolicina", "") == "@kolicina=0,"
                && Carpo.NetGen.Komande.ParametriSvi.Broj("kolicina", "1,5") == "@kolicina=1.5,", "");
            T("param-datum", Carpo.NetGen.Komande.ParametriSvi.Datum("datum", "", false, new DateTime(2026, 3, 1)) == "@datum=null,"
                && Carpo.NetGen.Komande.ParametriSvi.Datum("datum", "MM.YYYY", true, new DateTime(2026, 3, 1)) == "@datum='20260301',", "");
            T("param-multi", Carpo.NetGen.Komande.ParametriSvi.Multi("str", "1, 2,3") == "@str='1~2~3',", "");
            string iz1 = Carpo.NetGen.Komande.Izraz.Izracunaj("1+2*3");
            string iz2 = Carpo.NetGen.Komande.Izraz.Izracunaj("(1+2)*3");
            string iz3 = Carpo.NetGen.Komande.Izraz.Izracunaj("10/4");
            string iz4 = Carpo.NetGen.Komande.Izraz.Izracunaj("abs(-3)+1");
            string iz5 = Carpo.NetGen.Komande.Izraz.Izracunaj("2^3");
            string iz6 = Carpo.NetGen.Komande.Izraz.Izracunaj("min(1,4)");
            string iz7 = Carpo.NetGen.Komande.Izraz.Izracunaj("nije");
            T("izraz-ilcalc", iz1 == "7" && iz2 == "9" && iz3 == "2.5" && iz4 == "4" && iz5 == "8" && iz6 == "1" && iz7 == "",
                iz1 + "|" + iz2 + "|" + iz3 + "|" + iz4 + "|" + iz5 + "|" + iz6 + "|" + iz7);
            var recalc = new System.Data.DataTable();
            recalc.Columns.Add("kolicina");
            recalc.Columns.Add("evalkolicina()");
            recalc.Columns["evalkolicina()"].Caption = "evalkolicina()";
            recalc.Rows.Add("0", "2*3");
            T("recalc-grid", Carpo.NetGen.Komande.RecalcGrid.Izracunaj(recalc) == 1 && Convert.ToString(recalc.Rows[0]["kolicina"]) == "6", Convert.ToString(recalc.Rows[0]["kolicina"]));
            int toggle = 0;
            string t1 = Carpo.NetGen.Komande.ToggleGrid.Korak(ref toggle);
            int nakonSirenja = toggle;
            string t2 = Carpo.NetGen.Komande.ToggleGrid.Korak(ref toggle);
            T("toggle-grid", t1 == "expand" && nakonSirenja == 1 && t2 == "collapse" && toggle == 0, t1 + " " + t2 + " " + nakonSirenja + " " + toggle);
            T("multi-ispis", Carpo.NetGen.Komande.MultiIspis.Prvi(new[] { "8", "9" }) == "8"
                && Carpo.NetGen.Komande.MultiIspis.Idjevi(new[] { "8", "9" }).Length == 2, "");
            T("boja-html", Carpo.NetGen.Komande.BojaNaredba.Html(System.Drawing.Color.Red) == "Red"
                && Carpo.NetGen.Komande.BojaNaredba.Html(System.Drawing.Color.FromArgb(1, 2, 3)) == "#010203"
                && Carpo.NetGen.Komande.BojaNaredba.Sql("COLORPICKER(update x set boja='@@boja')").StartsWith("update"), "");
            T("posalji-url", Carpo.NetGen.Komande.PosaljiServer.UrlRacuna("http://localhost/ws/", "ab", "1", "2026", "5", "", "", "20260301").IndexOf("xmlrec.asp?a=1&v=1", StringComparison.Ordinal) >= 0
                && Carpo.NetGen.Komande.PosaljiServer.Vrsta("POSALJIPORUKENASERVER(select 1)") == "poruka", "");
            T("pokretanje", Carpo.NetGen.Komande.Pokretanje.Datoteka("C:\\a.bat~/q") == "C:\\a.bat"
                && Carpo.NetGen.Komande.Pokretanje.Argument("C:\\a.bat~/q") == "/q"
                && Carpo.NetGen.Komande.EkstenzijaGrid.ChartMetoda == "ChartGRID"
                && Carpo.NetGen.Komande.EkstenzijaGrid.PivotKlasa == "NetGenExtensions.Form1", "");
            T("foreach-podloga-petlja", Carpo.NetGen.Komande.ForeachNaredba.Ponovi("sp @id=@@id", new[] { "1", "2" }).Length == 2
                && Carpo.NetGen.Komande.ForeachNaredba.Ponovi("sp @id=@@id", new[] { "1", "2" })[1] == "sp @id=2", "");
            T("rafal-eval", Carpo.NetGen.Komande.RafalNaredba.EvalLijevo("EVAL(kolicina=1+2)") == "kolicina"
                && Carpo.NetGen.Komande.RafalNaredba.EvalDesno("EVAL(kolicina=1+2)") == "1+2"
                && Carpo.NetGen.Komande.RafalNaredba.Dock("DOCK5(grid)") == "Fill"
                && Carpo.NetGen.Komande.RafalNaredba.RefreshKontrola("REFRESHCONTROL(mjestaid)") == "mjestaid"
                && Carpo.NetGen.Komande.RafalNaredba.PrazanIliNula("0")
                && !Carpo.NetGen.Komande.RafalNaredba.OtkazImaPoruku("ISEMPTYORZEROCANCELBOX(@kolicina)")
                && Carpo.NetGen.Komande.RafalNaredba.UpozorenjeTekst("ISEMPTYORZEROALERT(@k;Nema)") == "Nema"
                && Carpo.NetGen.Komande.RafalNaredba.Slopp("SLOPP(1)")
                && Carpo.NetGen.Komande.RafalNaredba.Datoteka("CREATETEXTFILE1(c:\\a.txt;select 1)") == 1
                && Carpo.NetGen.Komande.RafalNaredba.DatotekaTekst(1, new[] { new[] { "A", "B" } }).EndsWith("\u001a"), "");
            T("fiskal-test-url", Carpo.NetGen.Komande.FiskalTok.UrlZaSlanje("") == ""
                && Carpo.NetGen.Komande.FiskalTok.UrlZaSlanje(Carpo.NetGen.Komande.FiskalTok.PravaUrl) == ""
                && Carpo.NetGen.Komande.FiskalTok.UrlZaSlanje(Carpo.NetGen.Komande.FiskalTok.TestUrl) == Carpo.NetGen.Komande.FiskalTok.TestUrl
                && Carpo.NetGen.Komande.FiskalTok.SqlXml(2, "15").StartsWith("spFaktureXMLGetRacunZahtijev")
                && Carpo.NetGen.Komande.FiskalTok.SqlXml(-1, "4").StartsWith("spMPXMLGetNapojnicaZahtijev"), "");
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
            bool ugradeno = p != null && g.Stanje.Odluka == "salji" && sql.IndexOf("@@godina", StringComparison.Ordinal) < 0 && sql.IndexOf("@godina = null", StringComparison.Ordinal) >= 0 && g.Stanje.Tablica == null;
            T("intrastat-ostao", ugradeno, sql);
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
            T("combo-4-ostao", sql4.IndexOf("@@", StringComparison.Ordinal) < 0 && SqlTekst.Odluka(sql4) == "salji" && sql4.IndexOf("null", StringComparison.Ordinal) >= 0, sql4);
            string poslije = SrediPoslije.Petlja(SqlTekst.Sredi(osobe.Atributi.Vrijednost("SQL"), k), "@trgovinaid=5,", null, false);
            T("combo-4-poslije", SqlTekst.Odluka(poslije) == "salji" && poslije.IndexOf("5", StringComparison.Ordinal) >= 0 && poslije.IndexOf("@@trgovinaid", StringComparison.Ordinal) < 0, poslije);

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
            T("foreach-podloga", podloga.Foreach == "svi" && podloga.Odigrano.Count == 1 && podloga.Odigrano[0] == "spAzur", "");
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
            T("det-zamjena", detSql.IndexOf("@id=9", StringComparison.Ordinal) >= 0 && detSql.IndexOf("@naziv=Kruh", StringComparison.Ordinal) >= 0 && detSql.IndexOf("@naziv='Kruh'", StringComparison.Ordinal) < 0, detSql);
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

        private static void BatIPost()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ng-bat-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                string izlaz = Path.Combine(dir, "out.txt");
                string bat = Path.Combine(dir, "pisi.bat");
                File.WriteAllText(bat, "@echo off\r\necho ok> \"" + izlaz + "\"\r\n");
                int kod = Carpo.NetGen.Komande.Pokretanje.Pokreni(bat);
                string tijelo = "";
                for (int i = 0; i < 20 && !File.Exists(izlaz); i++)
                    Thread.Sleep(100);
                if (File.Exists(izlaz))
                    tijelo = File.ReadAllText(izlaz).Trim();
                T("bat-pokretanje", kod == 0 && tijelo == "ok", kod + " " + tijelo);
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
            var slusac = new System.Net.HttpListener();
            string dobiveno = "";
            try
            {
                slusac.Prefixes.Add("http://127.0.0.1:8765/");
                slusac.Start();
                var nit = new Thread(delegate ()
                {
                    var ctx = slusac.GetContext();
                    using (var sr = new StreamReader(ctx.Request.InputStream, System.Text.Encoding.ASCII))
                        dobiveno = sr.ReadToEnd();
                    byte[] b = System.Text.Encoding.ASCII.GetBytes("1");
                    ctx.Response.OutputStream.Write(b, 0, b.Length);
                    ctx.Response.Close();
                });
                nit.IsBackground = true;
                nit.Start();
                string odg = Carpo.NetGen.Komande.PosaljiServer.Post("http://127.0.0.1:8765/", "<poruka>test</poruka>");
                nit.Join(4000);
                T("posalji-post", odg == "1" && dobiveno.IndexOf("<poruka>test</poruka>", StringComparison.Ordinal) >= 0, odg + " " + dobiveno);
            }
            catch (Exception ex)
            {
                T("posalji-post", false, ex.Message);
            }
            finally
            {
                try { slusac.Close(); } catch { }
            }
            string dll = Carpo.NetGen.Komande.EkstenzijaGrid.Nadji();
            Console.WriteLine(dll.Length == 0
                ? "NETGEN-EXTENSIONS nije na stroju. Treba donijeti NetGenExtensions.dll (NetGenExtensions.frmGridExPrintingREPX, Form1.PivotGRID, Form2.ChartGRID) u Jupiter SharedAssemblies."
                : "NETGEN-EXTENSIONS " + dll);
            int prijeBeep = Carpo.NetGen.Komande.RafalNaredba.BeepPoziva;
            Carpo.NetGen.Komande.RafalNaredba.Beep();
            T("beep", Carpo.NetGen.Komande.RafalNaredba.BeepPoziva == prijeBeep + 1, "");
            var roditelj = new System.Windows.Forms.Control();
            roditelj.Controls.Add(new System.Windows.Forms.Control());
            roditelj.Controls.Add(new System.Windows.Forms.Control());
            int ugaseno = Carpo.NetGen.Komande.RafalNaredba.SveKontrole(roditelj, false);
            T("sve-kontrole", ugaseno == 2 && roditelj.Controls[0].Enabled == false && roditelj.Controls[1].Enabled == false, ugaseno.ToString());
            Carpo.NetGen.Komande.RafalNaredba.SveKontrole(roditelj, true);
            string pokrenuto = "";
            Carpo.NetGen.Komande.RafalNaredba.PokreniProces = delegate (string p) { pokrenuto = p; return 0; };
            int kodProces = Carpo.NetGen.Komande.RafalNaredba.ProcessStart("PROCESSSTART(notepad.exe)");
            Carpo.NetGen.Komande.RafalNaredba.PokreniProces = null;
            T("process-start", kodProces == 0 && pokrenuto == "notepad.exe", pokrenuto);
            Carpo.NetGen.Komande.RafalNaredba.MapaDijalog = delegate () { return "C:\\temp"; };
            Carpo.NetGen.Komande.RafalNaredba.DatotekaDijalog = delegate () { return "C:\\temp\\a.txt"; };
            T("dijalog-mapa", Carpo.NetGen.Komande.RafalNaredba.OdaberiMapu() == "C:\\temp"
                && Carpo.NetGen.Komande.RafalNaredba.OdaberiDatoteku() == "C:\\temp\\a.txt", "");
            Carpo.NetGen.Komande.RafalNaredba.MapaDijalog = null;
            Carpo.NetGen.Komande.RafalNaredba.DatotekaDijalog = null;
            int interval = Carpo.NetGen.Komande.RafalNaredba.TimerOn("TIMERON(250)");
            Carpo.NetGen.Komande.RafalNaredba.TimerOff();
            Carpo.NetGen.Komande.RafalNaredba.Progres(true);
            bool progres = Carpo.NetGen.Komande.RafalNaredba.ProgresUpaljen;
            Carpo.NetGen.Komande.RafalNaredba.Progres(false);
            T("timer-progres", interval == 250 && Carpo.NetGen.Komande.RafalNaredba.TimerUpaljen == false && progres && Carpo.NetGen.Komande.RafalNaredba.ProgresUpaljen == false, interval.ToString());
            string[] trazi = Carpo.NetGen.Komande.RafalNaredba.StartSearch("STARTSEARCH(roba,kava)");
            var koraciOstalo = Carpo.NetGen.Komande.DijalogTok.Koraci("DISABLEALLCONTROLS()#RECALCGRIDROW(g)#GRID_COLUMN_VALUE(g,naziv,kava)#CREATEOBJECT(pero)#STARTSEARCH(roba,kava)");
            T("start-search", trazi.Length == 2 && trazi[0] == "ROBA" && trazi[1] == "KAVA"
                && koraciOstalo.Count == 5 && koraciOstalo[0].Vrsta == "svekontrole" && koraciOstalo[1].Vrsta == "recalcred"
                && koraciOstalo[2].Vrsta == "stupac" && koraciOstalo[3].Vrsta == "objekt" && koraciOstalo[4].Vrsta == "trazi", trazi[0]);
            var recalc = new System.Data.DataTable();
            recalc.Columns.Add("evalx()", typeof(string));
            recalc.Columns.Add("x", typeof(string));
            recalc.Columns[0].Caption = "evalx()";
            recalc.Rows.Add("1+2", "");
            recalc.Rows.Add("3+4", "");
            int jedan = Carpo.NetGen.Komande.RecalcGrid.IzracunajRed(recalc, 1);
            T("recalc-red", jedan == 1 && Convert.ToString(recalc.Rows[1]["x"]) == "7" && Convert.ToString(recalc.Rows[0]["x"]) == "", Convert.ToString(recalc.Rows[1]["x"]));
            var trazenje = new System.Data.DataTable();
            trazenje.Columns.Add("id", typeof(int));
            trazenje.Columns.Add("sifra", typeof(string));
            trazenje.Columns.Add("naziv", typeof(string));
            trazenje.Rows.Add(1, "a", "prvi");
            trazenje.Rows.Add(2, "b", "kava");
            var otvori = Carpo.NetGen.Komande.DijalogTok.Koraci("OPENFORM(FILTERFORM)#BLAGAJNAENABLE(4)#PRINTGRID(stavke)");
            string[] zag = Carpo.NetGen.Print.PrintBridge.TriZaglavlja("Lijevo", "", "Desno!!x", "Naslov");
            T("ispis-zaglavlje", zag.Length == 3 && zag[0] == "Lijevo" && zag[1] == "Naslov" && zag[2] == "Desno\r\nx"
                && Carpo.NetGen.Print.PrintBridge.Vrsta("PRINTREPXGRID(g)", true) == "lodani", zag[1] + "|" + zag[2]);
            T("redni-stupac", Carpo.NetGen.Ui.CarpoIzgled.RedniStupacVidljiv()
                && Carpo.NetGen.Ui.CarpoIzgled.SirinaGumba == 79
                && Carpo.NetGen.Ui.CarpoIzgled.LijevoGumba(1) == 87
                && Carpo.NetGen.Ui.CarpoIzgled.CvorNaSliciNula(), "");
            T("open-blagajna-ispis", otvori.Count == 3 && otvori[0].Vrsta == "forma" && otvori[0].Tekst == "FILTERFORM"
                && otvori[1].Vrsta == "blagajna" && otvori[2].Vrsta == "ispis"
                && Carpo.NetGen.Komande.BlagajnaNaredba.Upit("4").IndexOf("4", StringComparison.Ordinal) >= 0, otvori[0].Tekst);
            T("grid-vrijednost", Carpo.NetGen.Komande.RafalNaredba.NadjiVrijednost(trazenje, "naziv", "kava") == 1
                && Carpo.NetGen.Komande.RafalNaredba.RedStupca2(trazenje, "kava") == 1
                && Carpo.NetGen.Komande.RafalNaredba.PrazniCvorovi(new[] { "a", "", "b" }) == 1
                && Carpo.NetGen.Komande.RafalNaredba.CreateObject("CREATEOBJECT(pero)") == "PERO", "");
            var dock = new System.Windows.Forms.Control();
            Carpo.NetGen.Komande.RafalNaredba.PostaviDock(dock, "Fill");
            Carpo.NetGen.Komande.RafalNaredba.PostaviDock(dock, "Top");
            T("dock-klik", dock.Dock == System.Windows.Forms.DockStyle.Top, dock.Dock.ToString());
            string dat = Path.Combine(Path.GetTempPath(), "ng-txt-" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                Carpo.NetGen.Komande.RafalNaredba.ZapisiDatoteku(dat, 1, new[] { new[] { "A", "B" } });
                byte[] bajt = File.ReadAllBytes(dat);
                T("datoteka-klik", bajt.Length > 0 && bajt[bajt.Length - 1] == 26, bajt.Length.ToString());
            }
            finally
            {
                try { File.Delete(dat); } catch { }
            }
            bool pitao = false;
            Carpo.NetGen.Komande.RafalNaredba.Pitaj = delegate (string p) { pitao = p == "Nema"; return false; };
            bool dalje = Carpo.NetGen.Komande.RafalNaredba.AlertNastavi("0", "Nema");
            Carpo.NetGen.Komande.RafalNaredba.Pitaj = null;
            string up = "";
            Carpo.NetGen.Komande.RafalNaredba.Upozori = delegate (string p) { up = p; };
            bool stani = Carpo.NetGen.Komande.RafalNaredba.CancelBoxStani("", "Stoj");
            Carpo.NetGen.Komande.RafalNaredba.Upozori = null;
            var lanac = Carpo.NetGen.Komande.DijalogTok.Koraci("REFRESHCONTROL(mjesta)#BEEP()#ISEMPTYORZEROALERT(@a;Nema)");
            T("prazan-alert", !dalje && pitao && stani && up == "Stoj" && lanac.Count == 3 && lanac[0].Vrsta == "refresh" && lanac[1].Vrsta == "beep" && lanac[2].Vrsta == "naredba", lanac.Count.ToString());
            T("netgen-extensions", Carpo.NetGen.Komande.EkstenzijaGrid.Sklop == "NetGenExtensions"
                && Carpo.NetGen.Komande.EkstenzijaGrid.IspisKlasa == "NetGenExtensions.frmGridExPrintingREPX", dll);
        }

        private static NgForm FormaIme(NgProject p, string ime)
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

        /// <summary>
        /// Imena iz docs/razlike-netgen-carpo.md koja do sada nisu bila test.
        /// Svaka provjera je NetGen pravilo, ne prilagodba da prođe.
        /// </summary>
        private static void RazlikeImenom()
        {
            string[] on = Lanac.OnLoadLanac(":a#b");
            T("GFI-03", on.Length == 2 && on[0] == "a" && on[1] == "b" && Lanac.OnLoadLanac("select 1#select 2").Length == 0, on.Length.ToString());
            T("GFI-05", !IspisMotora.JeIspisImena("bilanca.clsGFIPOD") && IspisMotora.JeIspisImena("x.clsReport"), "");
            T("KNP-03", !PrintBridge.JeLandscape("PORTAL") && PrintBridge.JeLandscape("LANDSCAPE") && !PrintBridge.JeLandscape(""), "");
            T("REA-05", IspisMotora.Zaglavlje("#ISEMPTY()# #Od 1. do 2.") == ""
                && IspisMotora.Zaglavlje("#ISEMPTY(1)# #Od 1. do 2.") == "Od 1. do 2.", IspisMotora.Zaglavlje("#ISEMPTY(1)# #Od 1. do 2."));
            string gumb = Lanac.GumbIzCase("select case when '1'='1' then 'enable' else 'disable' end as Spremi", "");
            T("MPP-04", gumb == "Spremi=enable" && OnLoad2.Akcija("enable") == "enable", gumb);
            var lvo = DijalogTok.Koraci("IZMJENA(@a=@@x)");
            T("LVO-05", lvo.Count == 1 && lvo[0].Vrsta == "sql" && Lanac.OnLoadLanac("IZMJENA(@a=@@x)").Length == 0, lvo.Count == 0 ? "" : lvo[0].Vrsta);
            string pri = SqlTekst.Sredi("select @robaid", new SqlKontekst());
            T("PRI", pri.IndexOf("@robaid", StringComparison.Ordinal) >= 0 && pri.IndexOf("@@", StringComparison.Ordinal) < 0 && SqlTekst.Odluka(pri) == "salji", pri);

            string dir = Path.Combine(Path.GetTempPath(), "ng-razlike-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string xml = Path.Combine(dir, "R.xml");
            File.WriteAllText(xml,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                + "<PROJECT NAME=\"R\" ONLOAD=\"select 1\" COMMANDTIMEOUT=\"300\" TOTALROW=\"FALSE\" AUTOFITCOLUMNS=\"TRUE\" HELPKOD=\"ABC\" DETAILDBLCLICK=\":DET\" PAGEORIENTATION=\"PORTAL\" REFRESHAFTERCHILD=\"TRUE\" FILLEXPLORER=\"TRUE\" ONENTERKEY=\":X\" CMD6CAPTION=\"Lista\" DETCHECKBOXVISIBLE=\"FALSE\">"
                + "<MASTERCOLUMNS><COLUMN NAME=\"Id\" POSITION=\"2\" /></MASTERCOLUMNS>"
                + "<UNESI FILLEXPLORER=\"TRUE\" ONSUBMIT=\"select 1\" />"
                + "<STABLO FILLEXPLORER=\"onlytree\" />"
                + "<PRAZAN FILLEXPLORER=\"\" />"
                + "<BEZ />"
                + "<DIJETE REFRESHAFTERCHILD=\"TRUE\">"
                + "<INPUT TYPE=\"text\" NAME=\"prvi\" TABINDEX=\"5 \" />"
                + "<INPUT TYPE=\"text\" NAME=\"drugi\" />"
                + "<INPUT TYPE=\"date\" NAME=\"d\" VALUE=\"firstdayofmonth\" CAPTION=\"Datum\" />"
                + "</DIJETE></PROJECT>");
            try
            {
                var ucitano = NetGenXmlReader.Ucitaj(xml);
                NgProject p = ucitano.Projekt;
                T("INS-04", ucitano.Uspjeh && p != null && p.Atributi.Vrijednost("HELPKOD") == "ABC" && !IspisMotora.JeIspisImena("ABC"), ucitano.Greska ?? "");
                if (p == null)
                    throw new InvalidOperationException(ucitano.Greska ?? "XML");
                var tablica = new DataTable();
                tablica.Columns.Add("Id", typeof(int));
                var grid = new MasterGrid();
                grid.Ucitaj(p, tablica);
                int? pozicija = null;
                bool auto = true;
                for (int i = 0; i < grid.Stanje.Kolone.Count; i++)
                {
                    if (string.Equals(grid.Stanje.Kolone[i].Podatak, "Id", StringComparison.OrdinalIgnoreCase))
                    {
                        pozicija = grid.Stanje.Kolone[i].Pozicija;
                        auto = grid.Stanje.Kolone[i].AutoSirina;
                    }
                }
                T("INT-08", grid.Stanje.TimeoutSekundi == 300, grid.Stanje.TimeoutSekundi.ToString());
                T("NPL-04", grid.Stanje.TotalRed, "");
                T("PDV-05", grid.Stanje.EnterTipka == "None", grid.Stanje.EnterTipka ?? "");
                T("OPZ-07", pozicija == 2, pozicija.HasValue ? pozicija.Value.ToString() : "");
                T("MPSNG-05", auto == false && (p.Atributi.Vrijednost("AUTOFITCOLUMNS") ?? "") == "TRUE", "");
                T("KAS-04", (p.MasterDblClick ?? "") == "" && p.Atributi.Vrijednost("DETAILDBLCLICK") == ":DET", p.MasterDblClick ?? "");
                T("KOD-05", p.Atributi.Vrijednost("REFRESHAFTERCHILD") == "TRUE" && !Lanac.OsvjezavaZadnjiSql("REFRESHAFTERCHILD"), "");
                T("MPK-06", p.PreskoceniGumbi != null && p.PreskoceniGumbi.Contains("CMD6CAPTION"), "");
                var det = new DetailGrid();
                det.Pripremi(p);
                T("STA-38", det.Stanje.CheckboxVidljiv == false && grid.Stanje.CheckboxVidljiv, "");
                NgForm unesi = FormaIme(p, "UNESI");
                NgForm stablo = FormaIme(p, "STABLO");
                NgForm prazan = FormaIme(p, "PRAZAN");
                NgForm bez = FormaIme(p, "BEZ");
                NgForm dijete = FormaIme(p, "DIJETE");
                T("STA-08", unesi != null && unesi.Kontrole.Count == 0 && DijalogTok.SqlNakonOk(unesi) == "select 1", DijalogTok.SqlNakonOk(unesi));
                T("LVO-04", stablo != null && DijalogTok.OsvjeziStablo(stablo) && !DijalogTok.PuniExplorer(stablo) && DijalogTok.SqlNakonOk(stablo) == "", "");
                T("MPK-30", prazan != null && !DijalogTok.PuniExplorer(prazan) && DijalogTok.SqlNakonOk(prazan) == "", "");
                T("OPZ-04", bez != null && !DijalogTok.PuniExplorer(bez) && (p.Atributi.Vrijednost("FILLEXPLORER") ?? "") == "TRUE", "");
                var vrc = DijalogTok.Koraci("REFRESHAFTERCHILD");
                T("VRC-05", dijete != null && dijete.Atributi.Vrijednost("REFRESHAFTERCHILD") == "TRUE" && !Lanac.OsvjezavaSegment("REFRESHAFTERCHILD") && vrc.Count == 1 && vrc[0].Vrsta == "sql", vrc.Count == 0 ? "" : vrc[0].Vrsta);
                T("MPK-21", dijete != null && Raspored.TabRed(dijete) == "drugi,d,prvi", dijete == null ? "" : Raspored.TabRed(dijete));
                string datum = "";
                if (dijete != null)
                {
                    for (int i = 0; i < dijete.Kontrole.Count; i++)
                    {
                        if (dijete.Kontrole[i].Name != "d")
                            continue;
                        int z = 0;
                        datum = Raspored.Izracunaj(dijete.Kontrole[i].Grana, dijete.Kontrole[i].Atributi, ref z).Tekst ?? "";
                    }
                }
                T("MPK-22", datum == "Datum", datum);
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }

        private static void Podaci()
        {
            var k = new SqlKontekst();
            k.Id = "5";
            k.TempString = "abc";
            k.Version = "NetGen v1.2.3";
            k.FocusedControl = "naziv";
            T("sredi2-temp", SqlTekst.Sredi2("@@tempstring @@VERSION @@FOCUSEDCONTROL", k) == "abc NetGen v1.2.3 naziv", SqlTekst.Sredi2("@@tempstring @@VERSION @@FOCUSEDCONTROL", k));
            T("sredi2-id-nije-Id", SqlTekst.Sredi2("@@Id", k) == "@@Id" && SqlTekst.Sredi2("@@id", k) == "5", SqlTekst.Sredi2("@@Id", k));
            T("sredi-poslije-roba", SrediPoslije.Zamijeni("select @@robaid", "@robaid=5,", k, null, false) == "select 5", SrediPoslije.Zamijeni("select @@robaid", "@robaid=5,", k, null, false));
            T("sredi-poslije-navodnik", SrediPoslije.Zamijeni("update t set n='@@naziv'", "@naziv='a¸¸b',", k, null, false) == "update t set n='a''b'", SrediPoslije.Zamijeni("update t set n='@@naziv'", "@naziv='a¸¸b',", k, null, false));
            T("sredi-poslije-null", SrediPoslije.Zamijeni("select @@nema", "", k, null, false) == "select null", SrediPoslije.Zamijeni("select @@nema", "", k, null, false));
            T("sredi-poslije-napomena", SrediPoslije.Zamijeni("select '@@napomena'", "", k, null, false) == "select ''", SrediPoslije.Zamijeni("select '@@napomena'", "", k, null, false));
            string niz = SrediPoslije.Zamijeni("select @@nizplavihajdijevaX", "@nizplavihajdijevaX='~1~2~',", k, null, false);
            T("sredi-poslije-ajdijeva", niz.IndexOf("1,2", StringComparison.Ordinal) >= 0 && niz.IndexOf("~", StringComparison.Ordinal) < 0, niz);
            T("sredi-poslije-datum", SrediPoslije.Zamijeni("select @@datumod", "@datumod='20240201',", k, null, false) == "select '20240201'", SrediPoslije.Zamijeni("select @@datumod", "@datumod='20240201',", k, null, false));
            // Kljuc u CB je ime stupca plus '=' (Form1.vb 3980, 3076), Class1.vb 938 ga tako trazi.
            var cb = new Dictionary<string, string>();
            cb["tkosmomi="] = "Glavno";
            cb["kolicina="] = "1,5";
            T("sredi-poslije-cb", SrediPoslije.Zamijeni("@@tkosmomi", "", k, cb, false) == "Glavno"
                && SrediPoslije.Zamijeni("@@kolicina", "", k, cb, false) == "1.5", SrediPoslije.Zamijeni("@@kolicina", "", k, cb, false));
            T("gridrowcount-komentar", SrediPoslije.Zamijeni("@@GRIDROWCOUNT(g)", "", k, null, false) == "null(g)", SrediPoslije.Zamijeni("@@GRIDROWCOUNT(g)", "", k, null, false));
            string dml = SrediPoslije.ZaSlanje("update t set a=@@naziv", "@naziv='K',", k, null);
            T("za-slanje-dml", dml == "update t set a=K" && dml.IndexOf("@naziv=", StringComparison.Ordinal) < 0, dml);
            string sp = SrediPoslije.ZaSlanje("spX", "@a=1,", k, null);
            T("za-slanje-sp", sp.IndexOf("@a=1", StringComparison.Ordinal) >= 0, sp);
            string gas = SrediPoslije.ZaSlanje("spX !!", "@a=1,", k, null);
            T("za-slanje-uzvicnik", gas.IndexOf("@a", StringComparison.Ordinal) < 0 && gas.IndexOf("!!", StringComparison.Ordinal) < 0, gas);
            string kol = SrediPoslije.Kolone("select @(stavke..naziv)", delegate (string g, string c) { return g == "stavke" && c == "naziv" ? "Kava" : "null"; });
            T("kolone-grid", kol == "select Kava", kol);
            var red = new DataTable();
            red.Columns.Add("id", typeof(int));
            red.Columns.Add("Naziv  ", typeof(string));
            red.Columns.Add("Cijena0100", typeof(string));
            red.Rows.Add(3, "A,B", "12");
            string pg = DetailGrid.ParametriIzReda(red.Rows[0]);
            T("parametri-grida", pg.IndexOf("@id='3'", StringComparison.Ordinal) >= 0
                && pg.IndexOf("@Naziv='A,B'", StringComparison.Ordinal) >= 0
                && pg.IndexOf("@Cijena='12'", StringComparison.Ordinal) >= 0
                && pg.IndexOf("0100", StringComparison.Ordinal) < 0, pg);
            var hij = new DataTable();
            hij.Columns.Add("id0100", typeof(string));
            var stupac = hij.Columns.Add("Cijena", typeof(string));
            stupac.Caption = "Cijena";
            hij.Rows.Add("1234", "1,5");
            string hp = DetailGrid.ParametriHijerarhije(hij.Rows[0]);
            T("hijerarhija-grid", DetailGrid.IndeksKartice("t2") == 2 && DetailGrid.IndeksKartice("qwer") == 1
                && hp.IndexOf("@id0100='1234'", StringComparison.Ordinal) >= 0
                && hp.IndexOf("@Cijena='1.5'", StringComparison.Ordinal) >= 0, hp);
            var zadano = new DataTable();
            zadano.Columns.Add("tkosmomi", typeof(string));
            zadano.Rows.Add("'Glavno'");
            var cb2 = ClipboardNg.IzReda(zadano);
            T("setdefaults-cb", cb2.ContainsKey("tkosmomi=") && cb2["tkosmomi="] == "Glavno"
                && SrediPoslije.Zamijeni("@@tkosmomi", "", k, cb2, false) == "Glavno", cb2["tkosmomi="]);
            int poziva = 0;
            var cb3 = ClipboardNg.IzSetDefaults("select 1#select 2", k, delegate (string sql)
            {
                poziva++;
                if (poziva == 1)
                    return zadano;
                return null;
            });
            T("setdefaults-prekida", poziva == 2 && cb3.ContainsKey("tkosmomi="), poziva.ToString());
            string greska = GreskaSql.Poziv(5, "nema 'x'", "select 'a'");
            string uhvaceno = "";
            GreskaSql.Izvrsi = delegate (string sql) { uhvaceno = sql; throw new InvalidOperationException("guta"); };
            GreskaSql.Zapisi(5, "nema 'x'", "select 'a'", "");
            GreskaSql.Izvrsi = null;
            T("execsql2-poziv", greska == "dbo.spNetGenErrorLog @errorNumber=5, @errorDescription='nema ''x''',@ngLog='select ''a'''"
                && uhvaceno == greska && GreskaSql.TimeoutSekundi == 100, uhvaceno);
            string odabrano;
            string ostatak;
            T("combo-hash", ComboDijalog.OdvojiHash("sql##7", out odabrano, out ostatak) && odabrano == "7" && ostatak == "sql", odabrano);
            T("trazilica-preskok", Ponasanje.PreskociTrazilicu("", "x") && Ponasanje.PreskociTrazilicu("Kava", "Kava") && !Ponasanje.PreskociTrazilicu("*", "*") && !Ponasanje.PreskociTrazilicu("nova", "stara"), "");
            DijalogUcinak();
            SqlSlojNetGen();
            BrojNaDijalogu();
            LanacNakonZamjene();
            DogadjajiPravila();
            DogadjajiDijaloga();
            RasporedDijaloga();
            IspisUcinak();
            PokaziUpitUcinak();
            RafalnoPravila();
            PodlogaRafalno();
            GreskeDijaloga();
            DbClipboardDijalog();
            StatusDijaloga();
            GridDijalogaStupci();
            LozinkaULogu();
            IncludeFormi();
            IzvorXmla();
            DozvoleDijaloga();
            IspisGridaDijaloga();
            BojeIzUpita();
            StabloDijaloga();
            IzgledGridaDijaloga();
            OznaceniIzUpita();
            // HLP-02: DajPokaziHelp argumenti; Str(7) ima razmak sprijeda.
            T("hlp02-argumenti", JupiterPomoc.Argumenti("srv", "db", "KOD", "7", "X.xml") == "-s srv -d db -h KOD -o  7 -p X.xml",
                JupiterPomoc.Argumenti("srv", "db", "KOD", "7", "X.xml"));
            var traz = new DataTable();
            traz.Columns.Add("id", typeof(int));
            traz.Columns.Add("Naziv0100", typeof(string));
            traz.Rows.Add(4, "1,5");
            string search = ParametriSvi.IzTrazilice(traz.Rows[0]);
            // TRZ-02: Janus stupac 1 je kvacica, pa je prvi stupac podatka (id) i parametar.
            T("trazilica-search", search.IndexOf("@SEARCHid='4'", StringComparison.Ordinal) >= 0 && search.IndexOf("@SEARCHNaziv='1.5'", StringComparison.Ordinal) >= 0, search);
        }

        private static void DijalogUcinak()
        {
            var kontrole = new List<NgControl>();
            kontrole.Add(new NgControl("text", NgGrana.Text, "naziv", new NgAttributeBag(new Dictionary<string, string>()), true));
            kontrole.Add(new NgControl("search", NgGrana.Search, "trazi", new NgAttributeBag(new Dictionary<string, string> { { "ONLOSTFOCUS", "select lostsearch" } }), true));
            kontrole.Add(new NgControl("label", NgGrana.Label, "natpis", new NgAttributeBag(new Dictionary<string, string> { { "ONCLICK", "select klik" }, { "CAPTION", "Natpis" } }), true));
            var forma = new NgForm("T", new NgAttributeBag(null), "", "", kontrole, new List<NgButton>(), new List<string>());
            var sql = new List<string>();
            var d = new Dijalog();
            try
            {
                d.Slozi(forma, new SqlKontekst(), delegate (string s) { sql.Add(s ?? ""); return null; });
                d.ShowInTaskbar = false;
                d.StartPosition = FormStartPosition.Manual;
                d.Location = new Point(-32000, -32000);
                d.Show();
                Control naziv = d.Controls.Find("naziv", true)[0];
                naziv.Text = "abc";
                typeof(Control).GetMethod("OnEnter", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(naziv, new object[] { EventArgs.Empty });
                bool zelena = naziv.BackColor.ToArgb() == Color.LightGreen.ToArgb();
                var selProp = naziv.GetType().GetProperty("SelectionLength");
                int sel = selProp == null ? -1 : Convert.ToInt32(selProp.GetValue(naziv, null));
                Control trazi = d.Controls.Find("trazi", true)[0];
                trazi.Text = "";
                var onLeave = typeof(Control).GetMethod("OnLeave", BindingFlags.Instance | BindingFlags.NonPublic);
                onLeave.Invoke(trazi, new object[] { EventArgs.Empty });
                bool zuta = trazi.BackColor.ToArgb() == Color.LightYellow.ToArgb();
                bool tag0 = Convert.ToString(trazi.Tag) == "0";
                // Form2.Glbl_Leave 9441-9447: prazna trazilica na izlazu salje ONLOSTFOCUS.
                int prazniLost = Broji(sql, "lostsearch");
                Control natpis = d.Controls.Find("natpis", true)[0];
                typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(natpis, new object[] { EventArgs.Empty });
                bool klik = false;
                for (int i = 0; i < sql.Count; i++)
                {
                    if ((sql[i] ?? "").IndexOf("select klik", StringComparison.Ordinal) >= 0)
                        klik = true;
                }
                trazi.Text = "nova";
                trazi.Focus();
                // Runner nema aktivni prozor, pa Focus() ne postavi ActiveControl.
                // ProcessCmdKey gleda ActiveControl (Form2.vb KeyDown na tražilici).
                d.ActiveControl = trazi;
                int prije = Broji(sql, "lostsearch");
                d.ProvediTipku(Keys.Enter);
                onLeave.Invoke(trazi, new object[] { EventArgs.Empty });
                // Enter u trazilici (Glbl_Validated) salje ONLOSTFOCUS jednom; izlaz nakon toga ne salje opet (EVT-02).
                int nakonEntera = Broji(sql, "lostsearch") - prije;
                bool jednom = prazniLost == 1 && nakonEntera == 1;
                T("dijalog-ucinak", zelena && sel == 3 && zuta && tag0 && klik && jednom, naziv.BackColor.Name + " sel=" + sel + " tag=" + trazi.Tag
                    + " zelena=" + zelena + " zuta=" + zuta + " (" + trazi.BackColor.Name + ") klik=" + klik + " prazniLost=" + prazniLost + " nakonEntera=" + nakonEntera + " sql=" + string.Join(" | ", sql.ToArray()));
            }
            finally
            {
                d.Close();
                d.Dispose();
            }
        }

        /// <summary>
        /// Faza C1 (docs/pregled/C1-sql.md). Ocekivanja su iz doslovnog VB modela tools/diff/vb_oracle.py,
        /// koji je usporeden s Class1.vb 740-977 nad 13 746 SQL tekstova korpusa.
        /// </summary>
        private static void SqlSlojNetGen()
        {
            // SQL-02: ime krace od 4 znaka (@kg) VB tretira kao datum (Substring(0,4) baci, Resume Next).
            string kg = SrediPoslije.Nadji("@@kg", "@kg='abc',", null, false);
            T("sql02-kratko-ime", kg == "'abc'", kg);
            // SQL-03: datum iz medjuspremnika dd.MM.yyyy postane 'yyyyMMdd' (pomaci 6/3/0).
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            cb["datumod="] = "13.11.2026. 0:00:00";
            string dat = SrediPoslije.Nadji("@@datumod", "", cb, false);
            T("sql03-cb-datum", dat == "'20261113'", dat);
            string ispis = SrediPoslije.Nadji("@@datumod", "", cb, true);
            T("sql03-cb-datum-ispis", ispis == "13.11.2026.", ispis);
            // @@@ime u CB se ne nadje (Class1.vb 938).
            var cb3 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            cb3["chk="] = "1";
            string tri = SrediPoslije.Nadji("@@@chk", "", cb3, false);
            T("sql03-cb-tri-at", tri == "null", tri);
            // SQL-05: vise od 80 razlicitih tokena (spMPKase) se sve zamijeni.
            var sb = new StringBuilder("spMPKase @action=5");
            for (int i = 0; i < 95; i++)
                sb.Append(", @p").Append(i).Append("='@@tok").Append(i).Append("'");
            string puno = SrediPoslije.Petlja(sb.ToString(), "", null, false);
            T("sql05-vise-od-80", puno.IndexOf("@@", StringComparison.Ordinal) < 0, puno.Length.ToString());
            // SQL-04: @@ iz retka grida ide po pravilima NadjiVrijednostuPrevParametrima.
            var k = new SqlKontekst();
            k.ParametriSaGrida = "@Naziv='abc' , @DatumOd='13.11.2026. 0:00:00' , @RobaId='15' , ";
            string det = SqlTekst.Sredi("spX @n='@@Naziv', @d=@@DatumOd, @r=@@RobaId\n end", k);
            // Prijelom reda nije granica tokena ni u VB-u (Class1.vb 765-848): "@@RobaId\n" se zamijeni zajedno s \n.
            T("sql04-grid-pravila", det == "spX @n='abc', @d='20261113', @r=15 end", det);
            // SQL-04: redak grida formatira datum u kulturi racunala, kao VB Value.ToString.
            var staro = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hr-HR");
                var t = new DataTable();
                t.Columns.Add("DatumOd", typeof(DateTime));
                t.Columns.Add("Iznos", typeof(decimal));
                t.Rows.Add(new DateTime(2026, 11, 13), 1234.5m);
                string red = SqlTekst.IzReda(t.Rows[0]);
                var k2 = new SqlKontekst();
                k2.ParametriSaGrida = red;
                string s2 = SqlTekst.Sredi("spY @d=@@DatumOd, @i=@@Iznos", k2);
                T("sql04-redak-hr", s2 == "spY @d='20261113', @i=1234.5", red + " => " + s2);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = staro;
            }
        }

        /// <summary>
        /// NUM-01. Form2.vb 2072-2086 i 3601: brojčano polje šalje Value s točkom, prazno je 0.
        /// DECIMALS određuje broj decimala unosa. Carpo je slao prikazani tekst SpinEdita.
        /// </summary>
        private static void BrojNaDijalogu()
        {
            var kontrole = new List<NgControl>();
            kontrole.Add(new NgControl("number", NgGrana.Number, "iznos", new NgAttributeBag(new Dictionary<string, string> { { "DECIMALS", "2" } }), true));
            kontrole.Add(new NgControl("number", NgGrana.Number, "kolicina", new NgAttributeBag(new Dictionary<string, string>()), true));
            kontrole.Add(new NgControl("number", NgGrana.Number, "prazno", new NgAttributeBag(new Dictionary<string, string> { { "DECIMALS", "0" } }), true));
            var forma = new NgForm("T", new NgAttributeBag(null), "", "", kontrole, new List<NgButton>(), new List<string>());
            var staraKultura = Thread.CurrentThread.CurrentCulture;
            var d = new Dijalog();
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hr-HR");
                d.Slozi(forma, new SqlKontekst(), delegate (string s) { return null; });
                var iznos = (DevExpress.XtraEditors.SpinEdit)d.Controls.Find("iznos", true)[0];
                var kolicina = (DevExpress.XtraEditors.SpinEdit)d.Controls.Find("kolicina", true)[0];
                var prazno = (DevExpress.XtraEditors.SpinEdit)d.Controls.Find("prazno", true)[0];
                iznos.Value = 1234.5m;
                kolicina.Value = 2500m;
                prazno.Text = "";
                string sql = Convert.ToString(typeof(Dijalog).GetMethod("SviParametri", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, null));
                bool dobro = sql.IndexOf("@iznos=1234.5,", StringComparison.Ordinal) >= 0
                    && sql.IndexOf("@kolicina=2500,", StringComparison.Ordinal) >= 0
                    && sql.IndexOf("@prazno=0,", StringComparison.Ordinal) >= 0;
                T("broj-vrijednost-hr", dobro, sql + " | tekst=" + iznos.Text + " / " + kolicina.Text);
                T("broj-decimals", iznos.Properties.IsFloatValue && iznos.Properties.Mask.EditMask == "n2"
                    && kolicina.Properties.IsFloatValue
                    && !prazno.Properties.IsFloatValue && prazno.Properties.Mask.EditMask == "n0",
                    iznos.Properties.Mask.EditMask + " / " + kolicina.Properties.Mask.EditMask + " / " + prazno.Properties.Mask.EditMask);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = staraKultura;
                d.Dispose();
            }
        }

        /// <summary>
        /// LAN-01, LAN-03, CB-01 (docs/pregled/C2-lanac.md). Form2.vb 6812-6819: segment se zamijeni
        /// pa tek onda prepozna. @@ime iz medjuspremnika podloge (cls1.CB) vrijedi i u dijalogu.
        /// ASSEMBLYLOAD u lancu je naredba (Form2.vb 7424), ne SQL.
        /// </summary>
        private static int Broji(List<string> sql, string dio)
        {
            int n = 0;
            for (int i = 0; i < sql.Count; i++)
            {
                if ((sql[i] ?? "").IndexOf(dio, StringComparison.Ordinal) >= 0)
                    n++;
            }
            return n;
        }

        /// <summary>Faza C2 (docs/pregled/C2-lanac.md, EVT/RES/TRZ): ciste odluke bez prozora.</summary>
        private static void DogadjajiPravila()
        {
            T("res01-akcija", Form2Rezultat.Akcija(" Disable ") == "disable" && Form2Rezultat.Akcija("") == ""
                && Form2Rezultat.Akcija("refreshmovelast") == "ostalo" && Form2Rezultat.Akcija("tabnotstoped") == "tabnotstoped", "");
            T("res01-stanje", Form2Rezultat.JeStanje("invisible") && !Form2Rezultat.JeStanje("refresh") && Form2Rezultat.BrojDobivaVrijednost("12,5")
                && !Form2Rezultat.BrojDobivaVrijednost("refresh"), "");
            T("res01-vbbool", Form2Rezultat.VbBool("1") == true && Form2Rezultat.VbBool("False") == false && Form2Rezultat.VbBool("0") == false
                && Form2Rezultat.VbBool("disable") == null, "");
            T("res01-radio-beep", Form2Rezultat.RadioUkljuci("true") && !Form2Rezultat.RadioUkljuci("0") && Form2Rezultat.Beep("45") == 30 && Form2Rezultat.Beep("x") == 0, "");
            T("res01-combo-grid", Form2Rezultat.ComboOsvjezi("refresh") && !Form2Rezultat.ComboOsvjezi(" refresh") && Form2Rezultat.GridOsvjezi("refreshmovelast")
                && !Form2Rezultat.ComboDobivaVrijednost("enable") && Form2Rezultat.ComboDobivaVrijednost("5"), "");
            var red = new DataTable();
            red.Columns.Add("id", typeof(int));
            red.Columns.Add("sifra", typeof(string));
            red.Columns.Add("naziv0300", typeof(string));
            red.Columns.Add("napomena", typeof(string));
            red.Rows.Add(7, "S7", "Zagreb, Ilica", DBNull.Value);
            T("trz01-id-naziv", TrazilicaTok.Id(red.Rows[0]) == "7" && TrazilicaTok.Naziv(red.Rows[0]) == "Zagreb, Ilica", TrazilicaTok.Naziv(red.Rows[0]));
            var bezId = new DataTable();
            bezId.Columns.Add("partneriid", typeof(int));
            bezId.Columns.Add("opis", typeof(string));
            bezId.Rows.Add(9, "Opis 9");
            T("trz01-bez-id", TrazilicaTok.Id(bezId.Rows[0]) == "9" && TrazilicaTok.Naziv(bezId.Rows[0]) == "Opis 9", "");
            T("trz01-stupci", TrazilicaTok.Natpis("Sifra,Naziv", 1) == "Sifra" && TrazilicaTok.Natpis("Sifra,Naziv", 3) == null
                && TrazilicaTok.Sirina("40,0", 2) == 0 && TrazilicaTok.Natpis("Sifra", 0) == null, "");
            string celija = Form2Dogadjaj.ParametriCelije(red.Rows[0]);
            T("cel01-parametri", celija == "@id='7' , @sifra='S7' , @naziv='Zagreb. Ilica' , @napomena='' , ", celija);
            T("rac01-djeca", DijalogTok.OsvjezavaNakonDjeteta("UNOSSTAVKE") && DijalogTok.OsvjezavaNakonDjeteta("FORMA(@x=@@id)")
                && !DijalogTok.OsvjezavaNakonDjeteta("REPX:/racun") && !DijalogTok.OsvjezavaNakonDjeteta("rsgen5")
                && !DijalogTok.OsvjezavaNakonDjeteta("Biblioteka.Klasa") && !DijalogTok.OsvjezavaNakonDjeteta("x..y"), "");
            var rac = new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string> { { "REFRESHAFTERCHILD", "TRUE" } }), true);
            var racMalo = new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string> { { "REFRESHAFTERCHILD", "true" } }), true);
            T("rac01-atribut", DijalogTok.RefreshAfterChild(rac) && !DijalogTok.RefreshAfterChild(racMalo), "");
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            cb["NAZIV="] = "staro";
            ClipboardNg.UpisiParametre(cb, "@searchboxpartner='Pero',@comboboxvrsta='A',@naziv='novo',@partner=7,");
            T("cmb01-cb", cb["NAZIV="] == "novo" && cb.Count == 4 && cb["searchboxpartner="] == "Pero" && cb["comboboxvrsta="] == "A" && cb["partner="] == "7",
                string.Join(";", new List<string>(cb.Keys).ToArray()));
        }

        /// <summary>
        /// Faza C2: dogadjaji dijaloga idu kroz lanac (EVT-01), rezultat SQL-a postavlja kontrole (RES-01),
        /// trazilica trazi (TRZ-01), REFRESHAFTERCHILD, AFTERREFRESH, ONCELLEDITED, ONENTERKEY, ONLOADED, ONESCKEY.
        /// </summary>
        private static void DogadjajiDijaloga()
        {
            var kontrole = new List<NgControl>();
            kontrole.Add(new NgControl("text", NgGrana.Text, "naziv", new NgAttributeBag(new Dictionary<string, string>
            {
                { "ONLOSTFOCUS", "select 'disable' as kolicina, 'x1' as napomena#select 'refresh' as stavke" }
            }), true));
            kontrole.Add(new NgControl("number", NgGrana.Number, "kolicina", new NgAttributeBag(new Dictionary<string, string>()), true));
            kontrole.Add(new NgControl("text", NgGrana.Text, "napomena", new NgAttributeBag(new Dictionary<string, string>()), true));
            kontrole.Add(new NgControl("label", NgGrana.Label, "status", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "s" } }), true));
            kontrole.Add(new NgControl("search", NgGrana.Search, "partner", new NgAttributeBag(new Dictionary<string, string>
            {
                { "SQL", "spTrazi '@@searchboxpartner'" }, { "CAPTION", "Partner" }, { "ONLOSTFOCUS", "select 'trazeno' as status" }
            }), true));
            kontrole.Add(new NgControl("grid", NgGrana.Grid, "stavke", new NgAttributeBag(new Dictionary<string, string>
            {
                { "SQL", "select stavkaid, naziv from stavke where p=@@partner" }, { "REFRESHAFTERCHILD", "TRUE" },
                { "AFTERREFRESH", "select 'poslije' as status" }, { "ONCELLEDITED", "update s set naziv='@@naziv' where id=@@stavkaid" },
                { "EDITCOLUMNS", "naziv" }, { "ONENTERKEY", "select 'enter' as status" }
            }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "dodaj", new NgAttributeBag(new Dictionary<string, string> { { "ONCLICK", ":DIJETE" } }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "svi", new NgAttributeBag(new Dictionary<string, string>
            {
                { "ONCLICK", "FOREACHROW(stavke)#update x set k='@(stavke..naziv)' where id=@@stavke" }
            }), true));
            kontrole.Add(new NgControl("multidrop", NgGrana.MultiDrop, "vrste", new NgAttributeBag(new Dictionary<string, string> { { "SQL", "select vrstaid, naziv from vrste" } }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "oznaci", new NgAttributeBag(new Dictionary<string, string> { { "ONCLICK", "select '1~3' as vrste" } }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "salji", new NgAttributeBag(new Dictionary<string, string> { { "ONCLICK", "select '@@vrste' as v" } }), true));
            var atributi = new NgAttributeBag(new Dictionary<string, string>
            {
                { "ONLOADED", "select 'ucitano' as status" }, { "ONESCKEY", "select 'esc' as status" }
            });
            var forma = new NgForm("T2", atributi, "", "", kontrole, new List<NgButton>(), new List<string>());
            var sql = new List<string>();
            Func<string, DataTable> baza = delegate (string s)
            {
                sql.Add(s ?? "");
                string t = s ?? "";
                var dt = new DataTable();
                if (t.IndexOf("'disable' as kolicina", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("kolicina");
                    dt.Columns.Add("napomena");
                    dt.Rows.Add("disable", "x1");
                    return dt;
                }
                if (t.IndexOf("from vrste", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("vrstaid", typeof(int));
                    dt.Columns.Add("naziv", typeof(string));
                    dt.Rows.Add(1, "A");
                    dt.Rows.Add(2, "B");
                    dt.Rows.Add(3, "C");
                    return dt;
                }
                if (t.IndexOf("select '1~3' as vrste", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("vrste");
                    dt.Rows.Add("1~3");
                    return dt;
                }
                if (t.IndexOf("'refresh' as stavke", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("stavke");
                    dt.Rows.Add("refresh");
                    return dt;
                }
                if (t.IndexOf("from stavke", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("stavkaid", typeof(int));
                    dt.Columns.Add("naziv", typeof(string));
                    dt.Rows.Add(1, "prvi");
                    dt.Rows.Add(2, "drugi");
                    return dt;
                }
                if (t.IndexOf("spTrazi", StringComparison.Ordinal) >= 0)
                {
                    dt.Columns.Add("id", typeof(int));
                    dt.Columns.Add("naziv", typeof(string));
                    if (t.IndexOf("'jedan'", StringComparison.Ordinal) >= 0)
                        dt.Rows.Add(7, "Partner Jedan");
                    else if (t.IndexOf("'vise'", StringComparison.Ordinal) >= 0)
                    {
                        dt.Rows.Add(8, "Partner Osam");
                        dt.Rows.Add(9, "Partner Devet");
                    }
                    return dt;
                }
                if (t.IndexOf("update s set", StringComparison.Ordinal) >= 0)
                {
                    if (t.IndexOf("greska", StringComparison.Ordinal) >= 0)
                        throw new InvalidOperationException("greska u bazi");
                    return null;
                }
                int a = t.IndexOf("select '", StringComparison.Ordinal);
                int b = t.IndexOf("' as status", StringComparison.Ordinal);
                if (a >= 0 && b > a)
                {
                    dt.Columns.Add("status");
                    dt.Rows.Add(t.Substring(a + 8, b - a - 8));
                    return dt;
                }
                return null;
            };
            var kontekst = new SqlKontekst();
            kontekst.Clipboard = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var d = new Dijalog();
            string dijete = "";
            d.OtvoriDrugu = delegate (string ime) { dijete = ime; };
            // Upis (update, ONCELLEDITED) ide samo uz DozvoliUpis, kao u klik pregledu na spinu.
            DijalogTok.DozvoliUpis = true;
            try
            {
                d.Slozi(forma, kontekst, baza);
                d.ShowInTaskbar = false;
                d.StartPosition = FormStartPosition.Manual;
                d.Location = new Point(-32000, -32000);
                d.Show();
                Control status = d.Controls.Find("status", true)[0];
                T("frm01-onloaded", status.Text == "ucitano", status.Text + " | " + string.Join(" | ", sql.ToArray()));

                var onLeave = typeof(Control).GetMethod("OnLeave", BindingFlags.Instance | BindingFlags.NonPublic);
                Control naziv = d.Controls.Find("naziv", true)[0];
                Control kolicina = d.Controls.Find("kolicina", true)[0];
                Control napomena = d.Controls.Find("napomena", true)[0];
                int gridPrije = Broji(sql, "from stavke");
                onLeave.Invoke(naziv, new object[] { EventArgs.Empty });
                bool osvjezen = Broji(sql, "from stavke") == gridPrije + 1;
                T("evt01-lanac-rezultat", !kolicina.Enabled && napomena.Text == "x1" && osvjezen && status.Text == "poslije" && Broji(sql, "'poslije'") == 1,
                    "kolicina=" + kolicina.Enabled + " napomena=" + napomena.Text + " osvjezen=" + osvjezen + " status=" + status.Text);

                var partner = (DevExpress.XtraEditors.ButtonEdit)d.Controls.Find("partner", true)[0];
                // Korisnik udje u trazilicu pa upise (OldSearchString je tekst pri ulasku).
                d.ActiveControl = partner;
                partner.Text = "jedan";
                d.ProvediTipku(Keys.Enter);
                bool trazeno = Broji(sql, "spTrazi 'jedan'") == 1;
                bool rac = Broji(sql, "where p=7") == 1;
                T("trz01-jedan-red", Convert.ToString(partner.Tag) == "7" && partner.Text == "Partner Jedan" && trazeno && rac && status.Text == "trazeno",
                    "tag=" + partner.Tag + " text=" + partner.Text + " trazeno=" + trazeno + " rac=" + rac + " status=" + status.Text);
                T("cmb01-cb-dijalog", kontekst.Clipboard.ContainsKey("searchboxpartner=") && kontekst.Clipboard.ContainsKey("partner=")
                    && kontekst.Clipboard["partner="] == "7", string.Join(";", new List<string>(kontekst.Clipboard.Keys).ToArray()));

                TrazilicaProzor.OdabirZaTest = delegate (DataTable dt) { return dt.Rows.Count - 1; };
                try
                {
                    partner.Text = "vise";
                    d.ActiveControl = partner;
                    d.ProvediTipku(Keys.Enter);
                }
                finally
                {
                    TrazilicaProzor.OdabirZaTest = null;
                }
                T("trz01-prozor", Convert.ToString(partner.Tag) == "9" && partner.Text == "Partner Devet", "tag=" + partner.Tag + " text=" + partner.Text);

                int prijeDjeteta = Broji(sql, "from stavke");
                ((DevExpress.XtraEditors.SimpleButton)d.Controls.Find("dodaj", true)[0]).PerformClick();
                T("rac01-nakon-djeteta", dijete == "DIJETE" && Broji(sql, "from stavke") == prijeDjeteta + 1, "dijete=" + dijete);

                // MDR-01: MULTIDROP ima stavke iz SQL-a, rezultat "1~3" ih oznaci, a @@vrste je '1~3'.
                var vise = (DevExpress.XtraEditors.CheckedComboBoxEdit)d.Controls.Find("vrste", true)[0];
                bool stavke = vise.Properties.DataSource is DataTable && ((DataTable)vise.Properties.DataSource).Rows.Count == 3;
                ((DevExpress.XtraEditors.SimpleButton)d.Controls.Find("oznaci", true)[0]).PerformClick();
                ((DevExpress.XtraEditors.SimpleButton)d.Controls.Find("salji", true)[0]).PerformClick();
                T("mdr01-multidrop", stavke && sql.Contains("select '1~3' as v"),
                    "stavke=" + stavke + " oznaceno=" + Convert.ToString(vise.Properties.GetCheckedItems()) + " sql=" + string.Join(" | ", sql.ToArray()));

                // FOR-01: tijelo ide kroz lanac za svaki red; @@stavke je id reda, @(stavke..naziv) stupac reda.
                ((DevExpress.XtraEditors.SimpleButton)d.Controls.Find("svi", true)[0]).PerformClick();
                bool red1 = Broji(sql, "update x set k='prvi' where id=1") == 1;
                bool red2 = Broji(sql, "update x set k='drugi' where id=2") == 1;
                T("for01-foreachrow", red1 && red2, "red1=" + red1 + " red2=" + red2 + " sql=" + string.Join(" | ", sql.ToArray()));

                var grid = (DevExpress.XtraGrid.GridControl)d.Controls.Find("stavke", true)[0];
                var view = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                bool uredi = view.OptionsBehavior.Editable && view.Columns["naziv"].OptionsColumn.AllowEdit && !view.Columns["stavkaid"].Visible;
                view.FocusedRowHandle = 0;
                view.SetRowCellValue(0, "naziv", "novo");
                // @@id bi SrediSQL2 zamijenio ID-om forme (kao u VB-u), pa stupac reda ima drugo ime.
                bool celija = Broji(sql, "update s set naziv='novo' where id=1") == 1;
                T("cel01-grid-red", uredi && celija, "uredi=" + uredi + " sql=" + string.Join(" | ", sql.ToArray()));

                d.ActiveControl = grid;
                d.ProvediTipku(Keys.Enter);
                T("ent01-grid-enter", status.Text == "enter", status.Text);

                d.ActiveControl = naziv;
                d.ProvediTipku(Keys.Escape);
                T("esc01-onesckey", status.Text == "esc" && !d.IsDisposed && d.Visible, status.Text);
            }
            finally
            {
                DijalogTok.DozvoliUpis = false;
                d.Close();
                d.Dispose();
            }
        }

        /// <summary>Faza C4 (LAY-01, HOT-01): PARENT, DOCK, ANCHOR, STYLE, ploca s gumbima i HOTKEY.</summary>
        private static void RasporedDijaloga()
        {
            var kontrole = new List<NgControl>();
            kontrole.Add(new NgControl("group", NgGrana.Group, "GROUP2", new NgAttributeBag(new Dictionary<string, string> { { "DOCK", "2" }, { "HEIGHT", "700" } }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "spremi", new NgAttributeBag(new Dictionary<string, string>
            {
                { "PARENT", "GROUP2" }, { "LEFT", "140" }, { "TOP", "140" }, { "CAPTION", "Spremi" }, { "HOTKEY", "F2" }, { "ONCLICK", "select 'f2' as x" }
            }), true));
            kontrole.Add(new NgControl("grid", NgGrana.Grid, "stavke", new NgAttributeBag(new Dictionary<string, string> { { "DOCK", "5" } }), true));
            kontrole.Add(new NgControl("label", NgGrana.Label, "naslov", new NgAttributeBag(new Dictionary<string, string>
            {
                { "STYLE", "1" }, { "BACKCOLOR", "red" }, { "ANCHOR", "13" }, { "CAPTION", "Naslov" }
            }), true));
            kontrole.Add(new NgControl("text", NgGrana.Text, "opis", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Opis" } }), true));
            kontrole.Add(new NgControl("text", NgGrana.Text, "oznaka", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Oznaka" }, { "DEFAULT", "A_1" } }), true));
            kontrole.Add(new NgControl("number", NgGrana.Number, "kom", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Kom" }, { "DEFAULT", "5" } }), true));
            kontrole.Add(new NgControl("search", NgGrana.Search, "partner", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Partner" } }), true));
            kontrole.Add(new NgControl("date", NgGrana.Date, "datumod", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Od" } }), true));
            kontrole.Add(new NgControl("date", NgGrana.Date, "datumdo", new NgAttributeBag(new Dictionary<string, string> { { "SHOWCHECKBOX", "TRUE" } }), true));
            kontrole.Add(new NgControl("date", NgGrana.Date, "pocetak", new NgAttributeBag(new Dictionary<string, string> { { "DEFAULT", "firstdayofyear" } }), true));
            kontrole.Add(new NgControl("button", NgGrana.Button, "datumi", new NgAttributeBag(new Dictionary<string, string>
            {
                { "ONCLICK", "select '@@datumod' as a, @@datumdo as b" }, { "CAPTION", "D" }
            }), true));
            var gumbi = new List<NgButton> { new NgButton(1, "Unesi", ""), new NgButton(3, "Stavke", "STAVKE") };
            var forma = new NgForm("T3", new NgAttributeBag(null), "", "", kontrole, gumbi, new List<string>());
            var sql = new List<string>();
            var d = new Dijalog();
            try
            {
                d.Slozi(forma, new SqlKontekst(), delegate (string s) { sql.Add(s ?? ""); return null; });
                d.ShowInTaskbar = false;
                d.StartPosition = FormStartPosition.Manual;
                d.Location = new Point(-32000, -32000);
                d.Show();
                Control grupa = d.Controls.Find("GROUP2", true)[0];
                Control spremi = d.Controls.Find("spremi", true)[0];
                Control grid = d.Controls.Find("stavke", true)[0];
                Control naslov = d.Controls.Find("naslov", true)[0];
                Control[] ploca = d.Controls.Find("pnlGumbi", true);
                Control cmd3 = d.Controls.Find("cmd3", true)[0];
                bool okvir = spremi.Parent == grupa && grupa.Dock == DockStyle.Bottom;
                bool fill = grid.Dock == DockStyle.Fill && grid.Bottom <= (grupa.Top + 1);
                bool stil = naslov.Font.Bold && naslov.BackColor.ToArgb() == Color.Yellow.ToArgb()
                    && naslov.Anchor == (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                bool panel = ploca.Length == 1 && ploca[0].Dock == DockStyle.Bottom && cmd3.Parent == ploca[0] && cmd3.Left == 174;
                T("lay01-raspored", okvir && fill && stil && panel, "okvir=" + okvir + " fill=" + fill + " (grid " + grid.Bounds + ", grupa " + grupa.Bounds + ") stil=" + stil + " panel=" + panel);
                d.ActiveControl = grid;
                d.ProvediTipku(Keys.F2);
                T("hot01-f2", Broji(sql, "select 'f2' as x") == 1, string.Join(" | ", sql.ToArray()));

                // TXT-01: CAPTION nije tekst polja; DEFAULT je (bez SrediTekst); broj uzme DEFAULT.
                string opis = d.Controls.Find("opis", true)[0].Text;
                string oznaka = d.Controls.Find("oznaka", true)[0].Text;
                decimal kom = ((DevExpress.XtraEditors.SpinEdit)d.Controls.Find("kom", true)[0]).Value;
                string partner = d.Controls.Find("partner", true)[0].Text;
                T("txt01-tekst", opis == "" && oznaka == "A_1" && kom == 5 && partner == "",
                    "opis=" + opis + " oznaka=" + oznaka + " kom=" + kom + " partner=" + partner);

                // DAT-01: datum bez DEFAULT je danas, SHOWCHECKBOX bez CHECKED je null, DEFAULT firstdayofyear.
                var od = (DevExpress.XtraEditors.DateEdit)d.Controls.Find("datumod", true)[0];
                var dok = (DevExpress.XtraEditors.DateEdit)d.Controls.Find("datumdo", true)[0];
                var poc = (DevExpress.XtraEditors.DateEdit)d.Controls.Find("pocetak", true)[0];
                bool danas = od.EditValue is DateTime && ((DateTime)od.EditValue).Date == DateTime.Today;
                bool prazno = dok.EditValue == null;
                bool godina = poc.EditValue is DateTime && ((DateTime)poc.EditValue) == new DateTime(DateTime.Today.Year, 1, 1);
                ((DevExpress.XtraEditors.SimpleButton)d.Controls.Find("datumi", true)[0]).PerformClick();
                string ocekivano = "select '" + DateTime.Today.ToString("yyyyMMdd") + "' as a, null as b";
                T("dat01-datumi", danas && prazno && godina && Broji(sql, ocekivano) == 1,
                    "danas=" + danas + " prazno=" + prazno + " godina=" + godina + " sql=" + string.Join(" | ", sql.ToArray()));
            }
            finally
            {
                d.Close();
                d.Dispose();
            }
        }

        private static void LanacNakonZamjene()
        {
            string mapa = Path.Combine(Path.GetTempPath(), "ng-lanac-" + Guid.NewGuid().ToString("N"));
            PrintBridge.MapaIzvoza = mapa;
            PrintBridge.ZadnjiPdf = "";
            PrintBridge.ZadnjiIme = "";
            var gumbi = new List<NgButton>();
            gumbi.Add(new NgButton(1, "Ispis", ":@@imereporta1(@id=5)#CLOSE"));
            gumbi.Add(new NgButton(2, "Dms", "select 1 as x#ASSEMBLYLOAD(NePostojiNetGen.clsX)#OK"));
            var forma = new NgForm("T", new NgAttributeBag(null), "", "", new List<NgControl>(), gumbi, new List<string>());
            var k = new SqlKontekst();
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            cb["imereporta1="] = "repx/rptX";
            k.Clipboard = cb;
            var sql = new List<string>();
            var d = new Dijalog();
            try
            {
                d.Id = "5";
                d.Slozi(forma, k, delegate (string s) { sql.Add(s ?? ""); return null; });
                var klik = typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
                Control[] g2 = d.Controls.Find("cmd2", true);
                if (g2.Length > 0)
                    klik.Invoke(g2[0], new object[] { EventArgs.Empty });
                bool bezAsm = true;
                for (int i = 0; i < sql.Count; i++)
                {
                    if (sql[i].IndexOf("ASSEMBLYLOAD", StringComparison.OrdinalIgnoreCase) >= 0)
                        bezAsm = false;
                }
                T("lan01-assembly-nije-sql", g2.Length > 0 && bezAsm, string.Join(" | ", sql.ToArray()));
                Control[] g1 = d.Controls.Find("cmd1", true);
                if (g1.Length > 0)
                    klik.Invoke(g1[0], new object[] { EventArgs.Empty });
                bool bezRepx = true;
                for (int i = 0; i < sql.Count; i++)
                {
                    if (sql[i].IndexOf("repx", StringComparison.OrdinalIgnoreCase) >= 0 || sql[i].IndexOf("imereporta", StringComparison.OrdinalIgnoreCase) >= 0)
                        bezRepx = false;
                }
                T("lan03-token-u-ispis", g1.Length > 0 && bezRepx && PrintBridge.ZadnjiIme == "rptX",
                    "ime=" + PrintBridge.ZadnjiIme + " sql=" + string.Join(" | ", sql.ToArray()));
            }
            finally
            {
                PrintBridge.MapaIzvoza = null;
                try { d.Close(); d.Dispose(); } catch { }
                try { Directory.Delete(mapa, true); } catch { }
            }
        }

        private static void IspisUcinak()
        {
            string mapa = Path.Combine(Path.GetTempPath(), "ng-pdf-" + Guid.NewGuid().ToString("N"));
            PrintBridge.MapaIzvoza = mapa;
            PrintBridge.ZadnjiPdf = "";
            PrintBridge.ZadnjiIme = "";
            var gumbi = new List<NgButton>();
            // CMD1 je u Form2 uvijek OK (Button1_Click). Ispis s gumba je CMD3-CMD6: NovaGenerickaForma2(Tag).
            gumbi.Add(new NgButton(3, "Ispis", "repx/rptX"));
            var forma = new NgForm("T", new NgAttributeBag(null), "", "", new List<NgControl>(), gumbi, new List<string>());
            var d = new Dijalog();
            try
            {
                d.Id = "5";
                d.Slozi(forma, new SqlKontekst(), delegate (string s) { return null; });
                Control[] g = d.Controls.Find("cmd3", true);
                if (g.Length == 0)
                {
                    T("ispis-pdf", false, "nema gumba");
                    return;
                }
                typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(g[0], new object[] { EventArgs.Empty });
                bool dat = File.Exists(PrintBridge.ZadnjiPdf);
                byte[] b = dat ? File.ReadAllBytes(PrintBridge.ZadnjiPdf) : new byte[0];
                bool pdf = b.Length > 200 && b[0] == (byte)'%' && b[1] == (byte)'P' && b[2] == (byte)'D' && b[3] == (byte)'F';
                T("ispis-pdf", pdf && PrintBridge.ZadnjiIme == "rptX" && PrintBridge.ZadnjiParam.IndexOf("@id=5,", StringComparison.Ordinal) >= 0, PrintBridge.ZadnjiIme + " " + PrintBridge.ZadnjiParam + " " + b.Length);
            }
            finally
            {
                PrintBridge.MapaIzvoza = null;
                try { d.Close(); d.Dispose(); } catch { }
                try { Directory.Delete(mapa, true); } catch { }
            }
        }

        /// <summary>Form1.Rafalno, ssPermission i poruka greške bez baze (RAF-01, DOZ-01, MSG-01).</summary>
        private static void RafalnoPravila()
        {
            T("raf-vrsta", Form1Rafal.Vrsta("spX @a=1") == "sql" && Form1Rafal.Vrsta(":frm") == "forma" && Form1Rafal.Vrsta("REFRESHMASTERGRID") == "osvjezi"
                && Form1Rafal.Vrsta("FOREACHROW(x)") == "foreach" && Form1Rafal.Vrsta("OK") == "nista" && Form1Rafal.Vrsta("colorpicker()") == "sql", "");
            T("raf-isempty", Form1Rafal.IsEmptyPreskace("ISEMPTY(null)") && !Form1Rafal.IsEmptyPreskace("ISEMPTY( null )"), "");
            var t = new DataTable();
            t.Columns.Add("id", typeof(int));
            t.Columns.Add("Kalo %1200", typeof(decimal));
            t.Columns.Add("opis", typeof(string));
            t.Rows.Add(7, 1.5m, "a, b");
            string pc = Form1Rafal.ParametriCelije(t.Rows[0]);
            T("raf-parametri-celije", pc == "@id='7' , @Kalo_%='1.5' , @opis='a. b' , ", pc);
            var p = new DataTable();
            p.Columns.Add("objectname");
            p.Columns.Add("functionname");
            p.Columns.Add("smije", typeof(bool));
            p.Columns.Add("Vidljivost", typeof(bool));
            p.Rows.Add("KUPNG.xml", "Obriši", false, false);
            p.Rows.Add("KUPNG.xml", "Izmjeni", false, DBNull.Value);
            p.Rows.Add("KUPNG.xml", "Izmjeni", true, DBNull.Value);
            p.Rows.Add("KUPNG.xml", "UPDATEFORM", false, DBNull.Value);
            Dozvole.Stanje g1 = Dozvole.Gumb(p, "kupng.xml", "&Obriši");
            Dozvole.Stanje g2 = Dozvole.Gumb(p, "KUPNG.xml", "Izmjeni");
            T("doz-gumb", g1.Omogucen == false && g1.Vidljiv == false && g2.Omogucen == true && g2.Vidljiv == null, "");
            T("doz-komanda", !Dozvole.SmijeKomandu(p, ":UPDATEFORM") && Dozvole.SmijeKomandu(p, "INSERTFORM"), "");
            SqlPoruka.Ishod ok = SqlPoruka.IzIznimke("Zadani konto ne postoji #OK");
            SqlPoruka.Ishod tiha = SqlPoruka.IzIznimke("Invalid object name 'ssPermission'.");
            T("msg-execsql", ok.Greska == "" && ok.Tekst == "Zadani konto ne postoji" && !tiha.Prikazi && tiha.Greska == "", ok.Tekst);
            string poruka = null;
            NgPoruka.Zapis = delegate (string x) { poruka = x; };
            try
            {
                PorukaNaredba.Pokazi("MSGBOX(spremljeno)");
            }
            finally
            {
                NgPoruka.Zapis = null;
            }
            T("msg-msgbox-zadano", poruka == "SPREMLJENO", poruka);
            T("tot01-agregat", GridAgregat.Vrsta("2") == "Sum" && GridAgregat.Vrsta("3") == "Average" && GridAgregat.Vrsta("0") == "" && GridAgregat.Vrsta("SUM") == "Sum"
                && GridAgregat.Vrsta("4") == "Min" && GridAgregat.Vrsta("5") == "Max" && GridAgregat.Vrsta("") == "", "");
            T("tot01-format", GridAgregat.Format("Sum", typeof(decimal), "#,##0.000") == "{0:#,##0.00}" && GridAgregat.Format("Sum", typeof(int), "##0") == "{0:##0}"
                && GridAgregat.Format("Count", typeof(decimal), "") == "{0}", "");
            T("tot01-zamrznuti", GridAgregat.ZamrznutiPodaci(5, true) == 3 && GridAgregat.ZamrznutiPodaci(5, false) == 4 && GridAgregat.ZamrznutiPodaci(0, true) == 0 && GridAgregat.ZamrznutiPodaci(null, false) == 0, "");
            var dec = new DataTable();
            dec.Columns.Add("Iznos", typeof(decimal));
            T("tot01-decimal-sum", MasterGrid.IzPodatka(dec.Columns[0], "").Agregat == "2", "");
            // ISP-02: vrstadokid- ide u Spin.Printing (ovdje PDF mapa testa) s istim imenom i @id=.
            string mapaPrije = PrintBridge.MapaIzvoza;
            try
            {
                PrintBridge.MapaIzvoza = Path.Combine(Path.GetTempPath(), "ngvrsta");
                bool vrsta = PrintBridge.Ispisi(":vrstadokid-NabZahGla", false, "17", "7", "", "", "", "", "", "", "", "", "", null, "", null);
                T("isp02-vrstadokid", vrsta && PrintBridge.ZadnjiIme == "vrstadokid-NabZahGla" && PrintBridge.ZadnjiParam == "@id=17,", PrintBridge.ZadnjiIme + " " + PrintBridge.ZadnjiParam);
                PrintBridge.Ispisi(":vrstadokid-NabZahGla(25)", false, "17", "7", "", "", "", "", "", "", "", "", "", null, "", null);
                T("isp02-zagrada-id", PrintBridge.ZadnjiParam == "@id=25,25", PrintBridge.ZadnjiParam);
            }
            finally
            {
                PrintBridge.MapaIzvoza = mapaPrije;
            }
        }

        /// <summary>
        /// Class1.ExecSql u dijalogu (MSG-01): greška se pokaže i prekine lanac, "#OK" pokaže i ide dalje.
        /// ONLOAD s greškom ne otvara formu (Form2.vb 1199, Form1.vb 5352, ONL-02).
        /// </summary>
        private static void GreskeDijaloga()
        {
            var poruke = new List<string>();
            NgPoruka.Zapis = delegate (string x) { poruke.Add(x ?? ""); };
            var sql = new List<string>();
            Func<string, DataTable> baza = delegate (string s)
            {
                sql.Add(s ?? "");
                if ((s ?? "").IndexOf("provjera", StringComparison.OrdinalIgnoreCase) >= 0)
                    throw new InvalidOperationException("Dokument je zaključen #OK");
                if ((s ?? "").IndexOf("pada", StringComparison.OrdinalIgnoreCase) >= 0)
                    throw new InvalidOperationException("Zadani konto ne postoji");
                var dt = new DataTable();
                dt.Columns.Add("status");
                dt.Rows.Add("dalje");
                return dt;
            };
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("label", NgGrana.Label, "status", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "s" } }), true));
                kontrole.Add(new NgControl("button", NgGrana.Button, "ok", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "ONCLICK", "exec spProvjera 1#select 'dalje' as status" }
                }), true));
                kontrole.Add(new NgControl("button", NgGrana.Button, "pad", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "ONCLICK", "exec spPada 1#select 'nakon' as status" }
                }), true));
                var forma = new NgForm("G1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), baza);
                    // PerformClick radi samo na prikazanoj formi (kao DogadjajiDijaloga).
                    d.ShowInTaskbar = false;
                    d.StartPosition = FormStartPosition.Manual;
                    d.Location = new Point(-32000, -32000);
                    d.Show();
                    var ok = d.Controls.Find("ok", true);
                    var pad = d.Controls.Find("pad", true);
                    sql.Clear();
                    poruke.Clear();
                    if (ok.Length > 0)
                        ((DevExpress.XtraEditors.SimpleButton)ok[0]).PerformClick();
                    bool dalje = sql.Exists(delegate (string x) { return x.IndexOf("'dalje'", StringComparison.Ordinal) >= 0; });
                    T("msg01-ok-nastavlja", dalje && poruke.Count == 1 && poruke[0].Contains("Dokument je zaključen") && !poruke[0].Contains("#"),
                        string.Join(" / ", poruke.ToArray()) + " | " + string.Join(" ; ", sql.ToArray()));
                    sql.Clear();
                    poruke.Clear();
                    if (pad.Length > 0)
                        ((DevExpress.XtraEditors.SimpleButton)pad[0]).PerformClick();
                    bool nakon = sql.Exists(delegate (string x) { return x.IndexOf("'nakon'", StringComparison.Ordinal) >= 0; });
                    T("msg01-greska-prekida", !nakon && poruke.Count == 1 && poruke[0].Contains("Zadani konto ne postoji"),
                        string.Join(" / ", poruke.ToArray()) + " | " + string.Join(" ; ", sql.ToArray()));
                }
                var onload = new NgForm("G2", new NgAttributeBag(new Dictionary<string, string>()), "", "exec spPada @id=1", new List<NgControl>(), new List<NgButton>(), new List<string>());
                using (var d2 = new Dijalog())
                {
                    poruke.Clear();
                    d2.Slozi(onload, new SqlKontekst(), baza);
                    bool greska = !string.IsNullOrEmpty(d2.GreskaOnLoad);
                    // Prikazi samo kad je greška zapamćena: tada se vraća bez ShowDialog (inače bi test stao).
                    if (greska)
                        d2.Prikazi(onload, new SqlKontekst(), baza, true, null);
                    T("onl02-onload-greska", greska && !d2.Visible && d2.DialogResult != DialogResult.OK && poruke.Count >= 1, d2.GreskaOnLoad);
                }
            }
            catch (Exception ex)
            {
                T("greske-dijaloga", false, ex.ToString());
            }
            finally
            {
                NgPoruka.Zapis = null;
            }
        }

        /// <summary>NetGenDBClipboard (DBC-01): filter forma čita spremljene vrijednosti i sprema promjenu.</summary>
        private static void DbClipboardDijalog()
        {
            T("dbc-ukljucen", DbClipboard.Ukljucen("FILTER_FORM", "") && DbClipboard.Ukljucen("UNOS", "TRUE") && !DbClipboard.Ukljucen("FilterX", "false") && !DbClipboard.Ukljucen("UNOS", ""), "");
            T("dbc-sql", DbClipboard.CitajSql("7") == "select * from NetGenDBClipboard where operaterid='7'"
                && DbClipboard.UbaciSql("7", "Test.xml", "naziv", " x ") == "inseRt into NetGenDBClipboard (operaterid,[key],[value]) values(7,'Test.xml/naziv','x')", "");
            var sql = new List<string>();
            Func<string, DataTable> baza = delegate (string s)
            {
                sql.Add(s ?? "");
                var dt = new DataTable();
                if ((s ?? "").StartsWith("select * from NetGenDBClipboard", StringComparison.Ordinal))
                {
                    dt.Columns.Add("key");
                    dt.Columns.Add("value");
                    dt.Rows.Add("test.xml/naziv", "abc");
                    dt.Rows.Add("TEST.xml/aktivan ", "Checked");
                    return dt;
                }
                if ((s ?? "").StartsWith("select count(*)", StringComparison.Ordinal))
                {
                    dt.Columns.Add("n", typeof(int));
                    dt.Rows.Add(1);
                    return dt;
                }
                return dt;
            };
            bool bilo = DijalogTok.DozvoliUpis;
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("text", NgGrana.Text, "naziv", new NgAttributeBag(new Dictionary<string, string>()), true));
                kontrole.Add(new NgControl("check", NgGrana.Check, "aktivan", new NgAttributeBag(new Dictionary<string, string>()), true));
                var forma = new NgForm("FILTER1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                var k = new SqlKontekst();
                k.XmlName = "Test.xml";
                k.OperaterId = "7";
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, k, baza);
                    var naziv = d.Controls.Find("naziv", true);
                    var aktivan = d.Controls.Find("aktivan", true);
                    bool procitano = naziv.Length > 0 && naziv[0].Text == "abc" && aktivan.Length > 0 && ((DevExpress.XtraEditors.CheckEdit)aktivan[0]).Checked;
                    T("dbc01-cita", procitano, naziv.Length > 0 ? naziv[0].Text : "nema");
                    DijalogTok.DozvoliUpis = true;
                    sql.Clear();
                    if (naziv.Length > 0)
                        naziv[0].Text = "xyz";
                    Zovi(d, "PokupiParametre");
                    bool izmjena = sql.Exists(delegate (string x) { return x == "update NetGenDBClipboard set [value]='xyz' where operaterid=7 and [key]='Test.xml/naziv'"; });
                    bool bezKvacice = !sql.Exists(delegate (string x) { return x.IndexOf("aktivan", StringComparison.Ordinal) >= 0 && x.StartsWith("update", StringComparison.Ordinal); });
                    T("dbc01-sprema", izmjena && bezKvacice, string.Join(" ; ", sql.ToArray()));
                }
            }
            catch (Exception ex)
            {
                T("dbc01", false, ex.ToString());
            }
            finally
            {
                DijalogTok.DozvoliUpis = bilo;
            }
        }

        /// <summary>STATUSSTRIP TEXT1..3 (STS-01): velika slova, pa @@VERSION i ostali tokeni.</summary>
        private static void StatusDijaloga()
        {
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("statusstrip", NgGrana.StatusStrip, "s1", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "TEXT1", "@@version" }, { "TEXT2", "Broj stavki:" }, { "TEXT3", "" }
                }), true));
                var forma = new NgForm("S1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                var k = new SqlKontekst();
                k.Version = "NetGen v1.2.3";
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, k, delegate (string x) { return new DataTable(); });
                    var s1 = d.Controls.Find("s1", true);
                    string tekst = s1.Length > 0 ? s1[0].Text : "nema";
                    T("sts01-status", tekst == "NetGen v1.2.3   BROJ STAVKI:", tekst);
                }
                T("sts01-verzija", NgVerzija.Tekst().StartsWith("NetGen v", StringComparison.Ordinal), NgVerzija.Tekst());
            }
            catch (Exception ex)
            {
                T("sts01", false, ex.ToString());
            }
        }

        /// <summary>
        /// Faza C4 (DGR-01, DGR-02): grid dijaloga s COLUMN djecom i CLEARCOLUMS (Form2.vb 1798-1957),
        /// GROUPBYBOXVISIBLE, FROZENCOLUMNS, FILTERMODE, TOTALROW s AGREGATEFUNCTION; osvjezavanje ne dira stupce.
        /// </summary>
        /// <summary>XML-02: forma iz INCLUDE datoteke (Form2.ProcitajXML 840-934), glavni XML ima prednost.</summary>
        private static void IncludeFormi()
        {
            string dir = Path.Combine(Path.GetTempPath(), "nginc" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            try
            {
                string glavni = Path.Combine(dir, "Glavni.xml");
                File.WriteAllText(glavni, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT INCLUDE=\"Inc.xml#Nema.xml\">\r\n<NAME>G</NAME>\r\n"
                    + "<A CAPTION=\"iz glavnog\"><INPUT TYPE=\"text\" NAME=\"t\" /></A>\r\n"
                    + "<C INCLUDE=\"Inc2.xml\" CAPTION=\"c\"><INPUT TYPE=\"text\" NAME=\"t\" /></C>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(dir, "Inc.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT INCLUDE=\"Inc3.xml\">\r\n<NAME>I</NAME>\r\n"
                    + "<A CAPTION=\"iz includea\"><INPUT TYPE=\"text\" NAME=\"t\" /></A>\r\n"
                    + "<B_INC CAPTION=\"b\"><INPUT TYPE=\"text\" NAME=\"u\" /></B_INC>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(dir, "Inc2.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT>\r\n<NAME>I2</NAME>\r\n"
                    + "<D CAPTION=\"d\"><INPUT TYPE=\"text\" NAME=\"v\" /></D>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(dir, "Inc3.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT>\r\n<NAME>I3</NAME>\r\n"
                    + "<E CAPTION=\"e\"><INPUT TYPE=\"text\" NAME=\"w\" /></E>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                var r = Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(glavni);
                NgProject p = r.Projekt;
                NgForm a = Carpo.NetGen.Xml.IncludeForme.Nadji(p, "a");
                NgForm b = Carpo.NetGen.Xml.IncludeForme.Nadji(p, "B_INC");
                NgForm d = Carpo.NetGen.Xml.IncludeForme.Nadji(p, "D");
                NgForm e = Carpo.NetGen.Xml.IncludeForme.Nadji(p, "E");
                T("inc01-glavni-prvi", a != null && a.Atributi.Vrijednost("CAPTION") == "iz glavnog", a == null ? "nema" : a.Atributi.Vrijednost("CAPTION"));
                T("inc01-ukljucen", b != null && b.Kontrole.Count == 1 && b.Kontrole[0].Name == "u", "");
                T("inc01-include-forme", d != null, "");
                // VB uvozi samo djecu uključenog PROJECT-a; njegov INCLUDE se ne slijedi.
                T("inc01-ne-slijedi", e == null && Carpo.NetGen.Xml.IncludeForme.Nadji(p, "NEPOSTOJI") == null, "");
                T("inc01-gumb", Carpo.NetGen.Komande.GumbTok.Akcija(p, ":B_INC") == "forma" && Carpo.NetGen.Komande.GumbTok.Akcija(p, ":NEPOSTOJI") == "osvjezi",
                    Carpo.NetGen.Komande.GumbTok.Akcija(p, ":B_INC"));
            }
            catch (Exception ex)
            {
                T("inc01", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
        /// <summary>Kao KriptirajDekriptiraj.Encrypt (DES, Key = IV, StreamWriter.WriteLine): tijelo reda netgenxml.</summary>
        private static byte[] KriptirajKaoNetGen(string tekst)
        {
            byte[] k = Encoding.UTF8.GetBytes(Carpo.NetGen.Xml.XmlIzvor.Kljuc);
            using (var des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                des.Key = k;
                des.IV = k;
                var ms = new MemoryStream();
                using (var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.WriteLine(tekst);
                return ms.ToArray();
            }
        }

        /// <summary>XML-01 (M5): DMS, pa netgenxml (DES "tan123ge"), pa disk (Form1 3160-3274).</summary>
        private static void IzvorXmla()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ngm5" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            int baza = 0;
            try
            {
                T("xml01-dekript", Carpo.NetGen.Xml.XmlIzvor.Dekriptiraj(KriptirajKaoNetGen("abc čž"), "tan123ge") == "abc čž\r\n", "");
                string disk = Path.Combine(dir, "Disk.xml");
                File.WriteAllText(disk, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT>\r\n<NAME>D</NAME>\r\n<CAPTION>S diska</CAPTION>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                T("xml01-iskljucen", Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(disk).Projekt.Naslov == "S diska", "");
                Carpo.NetGen.Xml.XmlIzvor.Ukljuci("");
                Carpo.NetGen.Xml.XmlIzvor.Dms = delegate (string put) { return null; };
                Carpo.NetGen.Xml.XmlIzvor.Baza = delegate (string ime)
                {
                    baza++;
                    return ime == "Baza.xml" ? KriptirajKaoNetGen("<PROJECT><NAME>B</NAME><CAPTION>Iz baze</CAPTION></PROJECT>") : null;
                };
                var r = Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(Path.Combine(dir, "Baza.xml"));
                Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(Path.Combine(dir, "Baza.xml"));
                T("xml01-baza", r.Uspjeh && r.Projekt.Naslov == "Iz baze" && baza == 1, (r.Greska ?? "") + " baza=" + baza);
                T("xml01-disk", Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(disk).Projekt.Naslov == "S diska" && baza == 2, "baza=" + baza);
                Carpo.NetGen.Xml.XmlIzvor.Dms = delegate (string put)
                {
                    var d = new System.Xml.XmlDocument();
                    d.LoadXml("<PROJECT><NAME>M</NAME><CAPTION>Iz DMS-a</CAPTION></PROJECT>");
                    return d;
                };
                var m = Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(Path.Combine(dir, "Dms.xml"));
                T("xml01-dms", m.Uspjeh && m.Projekt.Naslov == "Iz DMS-a" && baza == 2, "baza=" + baza);
            }
            catch (Exception ex)
            {
                T("xml01", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Carpo.NetGen.Xml.XmlIzvor.Ukljucen = false;
                Carpo.NetGen.Xml.XmlIzvor.Dms = null;
                Carpo.NetGen.Xml.XmlIzvor.Baza = null;
                try { Directory.Delete(dir, true); } catch (IOException) { }
            }
        }

        private static string ZadnjiZapis()
        {
            return NgDnevnik.Zapisi.Count == 0 ? "" : NgDnevnik.Zapisi[NgDnevnik.Zapisi.Count - 1].Tekst;
        }

        /// <summary>Plan, faza G: lozinka iz polja s PASSWORDCHAR ne ide u log; log ne raste bez kraja.</summary>
        private static void LozinkaULogu()
        {
            try
            {
                NgDnevnik.TajnoIme("ServerPassTest");
                NgDnevnik.Sql("exec x @ServerPassTest='ab''c',@b=1");
                T("log01-ime", ZadnjiZapis() == "exec x @ServerPassTest='***',@b=1", ZadnjiZapis());
                NgDnevnik.Tajna("Tajna,9'x");
                NgDnevnik.Sql("exec sp 'srv','Tajna,9''x' -- Tajna~~9¸¸x");
                T("log01-vrijednost", ZadnjiZapis() == "exec sp 'srv','***' -- ***", ZadnjiZapis());
                NgDnevnik.Tajna("1");
                NgDnevnik.Sql("select 1");
                T("log01-kratka", ZadnjiZapis() == "select 1", ZadnjiZapis());
                // JupiterLog.Sredi 122-141: pwd= i password= u spojnom nizu.
                NgDnevnik.Greska("Server=a;Database=b;UID=sa;PWD=x9y;Application Name=n; @password='p''q', @c=1");
                T("log01-spojni", ZadnjiZapis() == "Server=a;Database=b;UID=sa;PWD=***;Application Name=n; @password=***, @c=1", ZadnjiZapis());
                for (int i = 0; i < NgDnevnik.NajviseZapisa + 50; i++)
                    NgDnevnik.Akcija("x" + i);
                T("log01-granica", NgDnevnik.Zapisi.Count == NgDnevnik.NajviseZapisa && ZadnjiZapis() == "x" + (NgDnevnik.NajviseZapisa + 49),
                    NgDnevnik.Zapisi.Count.ToString());
                var p = new NgControl("text", NgGrana.Text, "PassAdmin", new NgAttributeBag(new Dictionary<string, string> { { "PASSWORDCHAR", "*" } }), true);
                var forma = new NgForm("L1", new NgAttributeBag(new Dictionary<string, string>()), "", "", new List<NgControl> { p }, new List<NgButton>(), new List<string>());
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), delegate (string x) { return new DataTable(); });
                    var t = (DevExpress.XtraEditors.TextEdit)d.Controls.Find("PassAdmin", true)[0];
                    t.Text = "Admin#2026";
                    string dump = (string)typeof(Dijalog).GetMethod("ParametriZaLog", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, null);
                    NgDnevnik.Akcija(dump);
                    string z = ZadnjiZapis();
                    T("log01-dijalog", t.Properties.PasswordChar == '*' && dump.IndexOf("@PassAdmin='Admin#2026'", StringComparison.Ordinal) >= 0
                        && z.IndexOf("Admin#2026", StringComparison.Ordinal) < 0 && z.IndexOf("@PassAdmin='***'", StringComparison.Ordinal) >= 0, z);
                }
            }
            catch (Exception ex)
            {
                T("log01", false, ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static bool Postavljeno(Control c)
        {
            // Control.Visible na neprikazanoj formi je uvijek false; STATE_VISIBLE (2) je ono što je postavljeno.
            return (bool)typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(c, new object[] { 2 });
        }

        /// <summary>DOZ-02: PERMISIO="true" na formi dijaloga (Form2 ssPermission 5657-5781).</summary>
        private static void DozvoleDijaloga()
        {
            try
            {
                string sql = Dozvole.SqlForme("Obrazac ", "F1", "7");
                T("doz02-sql", sql.IndexOf("objectname='Obrazac.F1'", StringComparison.Ordinal) > 0 && sql.EndsWith("order by sort desc,isgrupa desc,grupeid,operateriid", StringComparison.Ordinal)
                    && sql.IndexOf("OperateriId = 7)", StringComparison.Ordinal) > 0, sql);
                var t = new DataTable();
                t.Columns.Add("objectname", typeof(string));
                t.Columns.Add("functionname", typeof(string));
                t.Columns.Add("smije", typeof(bool));
                t.Columns.Add("Vidljivost", typeof(bool));
                t.Rows.Add("Obrazac.F1", "iznos", false, false);
                t.Rows.Add("obrazac.f1", "Datum", DBNull.Value, DBNull.Value);
                t.Rows.Add("Obrazac.F1", "ZABRANJENA", false, DBNull.Value);
                t.Rows.Add("Obrazac.F1", "Export grid", false, DBNull.Value);
                t.Rows.Add("Obrazac.F1", "skriven", true, false);
                Dozvole.Stanje a = Dozvole.Kontrola(t, "Obrazac.F1", "iznos");
                Dozvole.Stanje b = Dozvole.Kontrola(t, "Obrazac.F1", "datum");
                Dozvole.Stanje c = Dozvole.Kontrola(t, "Obrazac.F1", "skriven");
                T("doz02-kontrola", a.Omogucen == false && a.Vidljiv == false && a.Natpis == false
                    && b.Omogucen == true && !b.Vidljiv.HasValue && b.Natpis == true
                    && c.Omogucen == true && c.Vidljiv == false && c.Natpis == true, "");
                Dozvole.Stanje n = Dozvole.KontrolaNaknadno(t, "iznos");
                Dozvole.Stanje m = Dozvole.KontrolaNaknadno(t, "datum");
                T("doz02-naknadno", n.Omogucen == false && n.Vidljiv == false && m.Omogucen == true && m.Vidljiv == true, "");

                var kontrole = new List<NgControl>
                {
                    new NgControl("text", NgGrana.Text, "iznos", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "Iznos" } }), true),
                    new NgControl("text", NgGrana.Text, "datum", new NgAttributeBag(new Dictionary<string, string>()), true),
                    new NgControl("text", NgGrana.Text, "slobodan", new NgAttributeBag(new Dictionary<string, string>()), true)
                };
                var forma = new NgForm("F1", new NgAttributeBag(new Dictionary<string, string> { { "PERMISIO", "true" } }), "", "", kontrole, new List<NgButton>(), new List<string>());
                var poslano = new List<string>();
                using (var d = new Dijalog())
                {
                    d.Operater = "7";
                    d.Slozi(forma, new SqlKontekst { XmlName = "Obrazac" }, delegate (string x)
                    {
                        poslano.Add(x ?? "");
                        return (x ?? "").IndexOf("ssPermission", StringComparison.Ordinal) >= 0 ? t : new DataTable();
                    });
                    T("doz02-citanje", poslano.Exists(delegate (string x) { return x.IndexOf("objectname='Obrazac.F1'", StringComparison.Ordinal) > 0; }), string.Join(" | ", poslano.ToArray()));
                    typeof(Dijalog).GetMethod("PrimijeniDozvole", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, null);
                    Control iznos = d.Controls.Find("iznos", true)[0];
                    Control[] lbl = d.Controls.Find("lbliznos", true);
                    Control slobodan = d.Controls.Find("slobodan", true)[0];
                    T("doz02-dijalog", !iznos.Enabled && !Postavljeno(iznos) && lbl.Length == 1 && !Postavljeno(lbl[0])
                        && slobodan.Enabled && Postavljeno(slobodan), "iznos=" + iznos.Enabled + "/" + Postavljeno(iznos) + " lbl=" + lbl.Length);
                    var smije = typeof(Dijalog).GetMethod("SmijeOtvoriti", BindingFlags.Instance | BindingFlags.NonPublic);
                    T("doz02-forma", !(bool)smije.Invoke(d, new object[] { ":ZABRANJENA(@a=@@id)" }) && (bool)smije.Invoke(d, new object[] { "DRUGA" }), "");
                    var stavka = (Dozvole.Stanje)typeof(Dijalog).GetMethod("DozvolaStavkeGrida", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(d, new object[] { "Export grid", true });
                    T("doz02-izbornik", stavka != null && stavka.Omogucen == false, "");
                }
                using (var d = new Dijalog())
                {
                    var bez = new NgForm("F1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                    poslano.Clear();
                    d.Slozi(bez, new SqlKontekst { XmlName = "Obrazac" }, delegate (string x) { poslano.Add(x ?? ""); return t; });
                    T("doz02-bez-permisio", !poslano.Exists(delegate (string x) { return x.IndexOf("ssPermission", StringComparison.Ordinal) >= 0; }), "");
                }
            }
            catch (Exception ex)
            {
                Exception u = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                T("doz02", false, u.GetType().Name + ": " + u.Message);
            }
        }

        private static void GridDijalogaStupci()
        {
            try
            {
                var g = new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "SQL", "select a, b, c from t" }, { "CLEARCOLUMS", "TRUE" }, { "GROUPBYBOXVISIBLE", "FALSE" },
                    { "FROZENCOLUMNS", "3" }, { "FILTERMODE", "true" }, { "CHECKBOXVISIBLE", "" }, { "EDITCOLUMNS", "c" },
                    { "ONCELLEDITED", "select '@@editedcolumn' as kolona, '@@lastvalue' as vrijednost" }
                }), true);
                g.Kolone.Add(Stupac("c", "Ce", "TRUE", "3", ""));
                g.Kolone.Add(Stupac("a", "", "FALSE", "0", ""));
                g.Kolone.Add(Stupac("b", "Be", "TRUE", "2", "#,##0.0000"));
                var kontrole = new List<NgControl> { g };
                var forma = new NgForm("G1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                int punjenja = 0;
                var poslano = new List<string>();
                Func<string, DataTable> baza = delegate (string x)
                {
                    poslano.Add(x ?? "");
                    var t = new DataTable();
                    t.Columns.Add("a", typeof(string));
                    t.Columns.Add("b", typeof(decimal));
                    t.Columns.Add("c", typeof(int));
                    t.Rows.Add("x", 1.5m, 2);
                    t.Rows.Add("y", 2.5m, 4);
                    if ((x ?? "").IndexOf("from t", StringComparison.Ordinal) >= 0)
                        punjenja++;
                    return t;
                };
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), baza);
                    var grid = (DevExpress.XtraGrid.GridControl)d.Controls.Find("g", true)[0];
                    var v = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    var a = v.Columns.ColumnByFieldName("a");
                    var b = v.Columns.ColumnByFieldName("b");
                    var c = v.Columns.ColumnByFieldName("c");
                    T("dgr01-clear-redoslijed", a != null && b != null && c != null && !a.Visible && c.VisibleIndex == 0 && b.VisibleIndex == 1
                        && c.Caption == "Ce" && b.Caption == "Be" && b.DisplayFormat.FormatString == "#,##0.0000",
                        a == null ? "nema" : "a=" + a.Visible + " c=" + c.VisibleIndex + " b=" + b.VisibleIndex + " " + c.Caption + "/" + b.Caption
                        + " format=" + b.DisplayFormat.FormatString + "/" + b.DisplayFormat.FormatType);
                    bool prosjek = c != null && c.Summary.Count == 1 && c.Summary[0].SummaryType == DevExpress.Data.SummaryItemType.Average;
                    bool zbroj = b != null && b.Summary.Count == 1 && b.Summary[0].SummaryType == DevExpress.Data.SummaryItemType.Sum;
                    T("dgr01-agregat", v.OptionsView.ShowFooter && prosjek && zbroj, "prosjek=" + prosjek + " zbroj=" + zbroj);
                    // FROZENCOLUMNS 3 = ikona + 2 podatka (kvacicu je CLEARCOLUMS sakrio, Form2.vb 1798).
                    T("dgr02-grid-atributi", !v.OptionsView.ShowGroupPanel && v.OptionsView.ShowAutoFilterRow
                        && v.OptionsSelection.MultiSelectMode == DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect
                        && c.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left && b.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left,
                        "grupe=" + v.OptionsView.ShowGroupPanel + " filter=" + v.OptionsView.ShowAutoFilterRow + " sel=" + v.OptionsSelection.MultiSelectMode + " fixed=" + c.Fixed + "/" + b.Fixed);
                    // Form2.Glbl_CellEdited + SrediSQL2 2882: @@editedcolumn je natpis stupca (Narudzbe g_ugo_stavke).
                    v.FocusedRowHandle = 0;
                    v.SetRowCellValue(0, c, 7);
                    T("dgr03-editedcolumn", poslano.Contains("select 'Ce' as kolona, '7' as vrijednost"), string.Join(" | ", poslano.ToArray()));
                    // Korisnik promijeni stupac; RefreshGrida (5305-5315) ga ne slaze iznova.
                    c.Caption = "Korisnik";
                    int prije = punjenja;
                    Zovi(d, "OsvjeziGrid", grid, g, "");
                    T("dgr01-osvjezi-ostavi-stupce", punjenja == prije + 1 && c.Caption == "Korisnik" && !a.Visible && c.VisibleIndex == 0,
                        "punjenja=" + (punjenja - prije) + " natpis=" + c.Caption);
                }
            }
            catch (Exception ex)
            {
                T("dgr01", false, ex.ToString());
            }
        }

        /// <summary>
        /// ISP-07: PRINTGRID(ime) ispisuje imenovani grid dijaloga s njegovim PAGEHEADER* i PAGEORIENTATION
        /// (Form2.vb 6950-6955, IspisGRIDREPX 9041); prije je išao prvi grid bez zaglavlja.
        /// </summary>
        private static void IspisGridaDijaloga()
        {
            var stroj = new LazniSpin();
            Carpo.NetGen.Print.PrintBridge.Stroj = stroj;
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("text", NgGrana.Text, "broj", new NgAttributeBag(new Dictionary<string, string> { { "DEFAULT", "15" } }), true));
                kontrole.Add(new NgControl("grid", NgGrana.Grid, "prvi", new NgAttributeBag(new Dictionary<string, string> { { "SQL", "select 1 as id" } }), true));
                kontrole.Add(new NgControl("grid", NgGrana.Grid, "kartica", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "SQL", "select 2 as id" }, { "PAGEHEADERCENTER", "Kartica br.@@broj" }, { "PAGEHEADERLEFT", "Lijevo" },
                    { "PAGEORIENTATION", "LANDSCAPE" }
                }), true));
                var forma = new NgForm("P1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), delegate (string x)
                    {
                        var t = new DataTable();
                        t.Columns.Add("id", typeof(int));
                        t.Rows.Add(1);
                        return t;
                    });
                    var kartica = d.Controls.Find("kartica", true)[0];
                    LazniSpin.Metoda = "";
                    Zovi(d, "IzvrsiDogadjaj", "PRINTGRID(kartica)");
                    T("isp07-printgrid-ime", LazniSpin.Metoda == "PrintLink" && LazniSpin.Kontrola == kartica && LazniSpin.Landscape
                        && LazniSpin.Caption == "Kartica br.15" && LazniSpin.Header == "Lijevo",
                        LazniSpin.Metoda + " grid=" + (LazniSpin.Kontrola is Control ? ((Control)LazniSpin.Kontrola).Name : "?")
                        + " land=" + LazniSpin.Landscape + " caption=" + LazniSpin.Caption + " header=" + LazniSpin.Header);
                }
            }
            catch (Exception ex)
            {
                T("isp07", false, ex.ToString());
            }
            finally
            {
                Carpo.NetGen.Print.PrintBridge.Stroj = null;
            }
        }

        /// <summary>
        /// GRB-01: stupci upita RowForeColorID/RowColorID, &lt;ime&gt;CellBackColorID i &lt;ime&gt;MaxLinesID
        /// (Form1.GridEX1_FormattingRow 7875-7945, Form2.glblGRID_FormattingRow).
        /// </summary>
        private static void BojeIzUpita()
        {
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string> { { "SQL", "select 1" } }), true));
                var forma = new NgForm("B1", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), delegate (string x)
                    {
                        var t = new DataTable();
                        t.Columns.Add("id", typeof(int));
                        t.Columns.Add("Naziv", typeof(string));
                        t.Columns.Add("NazivCellBackColorID", typeof(string));
                        t.Columns.Add("NazivMaxLinesID", typeof(int));
                        t.Columns.Add("RowForeColorID", typeof(string));
                        t.Columns.Add("RowColorID", typeof(string));
                        t.Columns.Add("FormatID", typeof(string));
                        t.Rows.Add(1, "dugi naziv", "#FF0000", 2, "Blue", "", "boldgreen");
                        return t;
                    });
                    var grid = (DevExpress.XtraGrid.GridControl)d.Controls.Find("g", true)[0];
                    var v = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    var naziv = v.Columns.ColumnByFieldName("Naziv");
                    T("grb01-maxlines", naziv != null && naziv.AppearanceCell.TextOptions.WordWrap == DevExpress.Utils.WordWrap.Wrap && v.OptionsView.RowAutoHeight
                        && !v.Columns.ColumnByFieldName("NazivCellBackColorID").Visible, naziv == null ? "nema" : naziv.AppearanceCell.TextOptions.WordWrap.ToString());
                    var izgled = new DevExpress.Utils.AppearanceObject();
                    // Konstruktor (rowHandle, stupac, stanje, izgled) preko refleksije, bez imena tipa stanja.
                    ConstructorInfo ctor = typeof(DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs).GetConstructors()[0];
                    ParameterInfo[] ps = ctor.GetParameters();
                    var argumenti = new object[ps.Length];
                    for (int i = 0; i < ps.Length; i++)
                    {
                        Type pt = ps[i].ParameterType;
                        if (pt == typeof(int))
                            argumenti[i] = 0;
                        else if (pt.IsInstanceOfType(naziv))
                            argumenti[i] = naziv;
                        else if (pt.IsInstanceOfType(izgled))
                            argumenti[i] = izgled;
                        else if (pt.IsValueType)
                            argumenti[i] = Activator.CreateInstance(pt);
                    }
                    var arg = (DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs)ctor.Invoke(argumenti);
                    typeof(Carpo.NetGen.Ui.GridIzgled).GetMethod("StilCelije", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { v, arg });
                    T("grb01-boje", arg.Appearance.BackColor.ToArgb() == Color.Red.ToArgb() && arg.Appearance.ForeColor.ToArgb() == Color.Blue.ToArgb(),
                        "back=" + arg.Appearance.BackColor + " fore=" + arg.Appearance.ForeColor);
                    // FMT-01: formatid "boldgreen" podeblja red; RowForeColorID (GRB-01) nadjača zelenu.
                    T("fmt01-red", arg.Appearance.Font.Bold && arg.Appearance.ForeColor.ToArgb() == Color.Blue.ToArgb(),
                        "bold=" + arg.Appearance.Font.Bold + " fore=" + arg.Appearance.ForeColor);
                    var a1 = new DevExpress.Utils.AppearanceObject();
                    Carpo.NetGen.Ui.GridIzgled.FormatReda(a1, "boldorange");
                    var a2 = new DevExpress.Utils.AppearanceObject();
                    Carpo.NetGen.Ui.GridIzgled.FormatReda(a2, "RedGreen Italic Strike");
                    var a3 = new DevExpress.Utils.AppearanceObject();
                    Color prije = a3.ForeColor;
                    Carpo.NetGen.Ui.GridIzgled.FormatReda(a3, DBNull.Value);
                    T("fmt01-uvjeti", a1.ForeColor.ToArgb() == Color.Orange.ToArgb() && a1.Font.Bold && !a1.Font.Italic
                        && a2.ForeColor.ToArgb() == Color.Green.ToArgb() && a2.Font.Italic && a2.Font.Strikeout && !a2.Font.Bold
                        && a3.ForeColor == prije && !a3.Font.Bold, a1.ForeColor + " " + a2.ForeColor + " " + a2.Font.Style);
                }
            }
            catch (Exception ex)
            {
                T("grb01", false, ex.ToString());
            }
        }

        /// <summary>
        /// TRE-03: stablo na dijalogu (Form2.vb 2558-2601, DazovaIzStore 2825): čvorovi iz procedure, parametar
        /// je oznaka odabranog čvora (null dok nije odabran), odabir ide u ONCLICK, "refresh" ga puni iznova.
        /// </summary>
        private static void StabloDijaloga()
        {
            try
            {
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("tree", NgGrana.Tree, "klasa", new NgAttributeBag(new Dictionary<string, string>
                {
                    { "SQL", "spTreeKlase:FORMA" }, { "ONCLICK", "select '@@klasa' as status" }
                }), true));
                kontrole.Add(new NgControl("label", NgGrana.Label, "status", new NgAttributeBag(new Dictionary<string, string> { { "CAPTION", "s" } }), true));
                var forma = new NgForm("T3", new NgAttributeBag(new Dictionary<string, string>()), "", "", kontrole, new List<NgButton>(), new List<string>());
                int stabala = 0;
                Func<string, DataTable> baza = delegate (string x)
                {
                    string q = (x ?? "").Trim();
                    var t = new DataTable();
                    if (q.StartsWith("spTreeKlase", StringComparison.Ordinal))
                    {
                        stabala++;
                        t.Columns.Add("id"); t.Columns.Add("roditelj"); t.Columns.Add("natpis"); t.Columns.Add("oznaka");
                        t.Rows.Add("1", "0", "Sve", "10");
                        t.Rows.Add("2", "1", "Voće", "20");
                        t.Rows.Add("3", "1", "Povrće", "30");
                        return t;
                    }
                    var m = System.Text.RegularExpressions.Regex.Match(q, "^select '([^']*)' as (\\w+)$");
                    if (!m.Success)
                        return null;
                    t.Columns.Add(m.Groups[2].Value);
                    t.Rows.Add(m.Groups[1].Value);
                    return t;
                };
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), baza);
                    var stablo = (DevExpress.XtraTreeList.TreeList)d.Controls.Find("klasa", true)[0];
                    Control status = d.Controls.Find("status", true)[0];
                    bool cvorovi = stablo.Nodes.Count == 1 && stablo.Nodes[0].Nodes.Count == 2 && Convert.ToString(stablo.Nodes[0].GetValue(0)) == "Sve";
                    string prazno = Convert.ToString(Zovi(d, "SviParametri"));
                    T("tre03-cvorovi", cvorovi && stabala == 1 && prazno.IndexOf("@klasa=null,", StringComparison.Ordinal) >= 0, "cvorova=" + stablo.Nodes.Count + " param=" + prazno);
                    stablo.FocusedNode = stablo.Nodes[0].Nodes[0];
                    T("tre03-klik", status.Text == "20" && Convert.ToString(Zovi(d, "SviParametri")).IndexOf("@klasa='20',", StringComparison.Ordinal) >= 0, status.Text);
                    Zovi(d, "IzvrsiDogadjaj", "select 'refresh' as klasa");
                    T("tre03-refresh", stabala == 2 && stablo.Nodes.Count == 1, "stabala=" + stabala);
                }
            }
            catch (Exception ex)
            {
                T("tre03", false, ex.ToString());
            }
        }

        /// <summary>
        /// GRL-01: "Spremi postavke grida" sprema izgled grida dijaloga (SaveGridLayout 9205), novo otvaranje ga
        /// učita (LoadLayout), a "Reset grida" ga obriše i složi stupce iznova (DeleteGridLayout).
        /// </summary>
        private static void IzgledGridaDijaloga()
        {
            string mapa = Path.Combine(Path.GetTempPath(), "nggrl" + Guid.NewGuid().ToString("N").Substring(0, 6));
            GridRaspored.MapaZaTest = mapa;
            try
            {
                var g = new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string> { { "SQL", "select a, b from t" } }), true);
                var forma = new NgForm("L1", new NgAttributeBag(new Dictionary<string, string>()), "", "", new List<NgControl> { g }, new List<NgButton>(), new List<string>());
                Func<string, DataTable> baza = delegate (string x)
                {
                    var t = new DataTable();
                    t.Columns.Add("a", typeof(string));
                    t.Columns.Add("b", typeof(string));
                    t.Rows.Add("1", "2");
                    return t;
                };
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), baza);
                    var grid = (DevExpress.XtraGrid.GridControl)d.Controls.Find("g", true)[0];
                    var v = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    v.Columns.ColumnByFieldName("a").Visible = false;
                    GridRaspored.Spremi(v, Convert.ToString(Zovi(d, "DatotekaIzgleda", grid)));
                }
                bool spremljeno = Directory.Exists(mapa) && Directory.GetFiles(mapa, "GridLodani-*-L1-g-.xml").Length == 1;
                using (var d2 = new Dijalog())
                {
                    d2.Slozi(forma, new SqlKontekst(), baza);
                    var grid = (DevExpress.XtraGrid.GridControl)d2.Controls.Find("g", true)[0];
                    var v = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    bool ucitano = !v.Columns.ColumnByFieldName("a").Visible && v.Columns.ColumnByFieldName("b").Visible;
                    T("grl01-spremi-ucitaj", spremljeno && ucitano, "spremljeno=" + spremljeno + " ucitano=" + ucitano);
                    Zovi(d2, "ResetGrida", grid, g);
                    bool vraceno = v.Columns.ColumnByFieldName("a") != null && v.Columns.ColumnByFieldName("a").Visible
                        && Directory.GetFiles(mapa, "*.xml").Length == 0;
                    T("grl01-reset", vraceno, "a=" + (v.Columns.ColumnByFieldName("a") == null ? "nema" : v.Columns.ColumnByFieldName("a").Visible.ToString()));
                }
            }
            catch (Exception ex)
            {
                T("grl01", false, ex.ToString());
            }
            finally
            {
                GridRaspored.MapaZaTest = null;
                try { if (Directory.Exists(mapa)) Directory.Delete(mapa, true); } catch (IOException) { }
            }
        }

        /// <summary>CHK-01: stupac "selectedid" s 1 označi red grida dijaloga (RucniRadNaGriduzaCheckiranjeRedova).</summary>
        private static void OznaceniIzUpita()
        {
            try
            {
                var g = new NgControl("grid", NgGrana.Grid, "g", new NgAttributeBag(new Dictionary<string, string> { { "SQL", "select 1" } }), true);
                var forma = new NgForm("C1", new NgAttributeBag(new Dictionary<string, string>()), "", "", new List<NgControl> { g }, new List<NgButton>(), new List<string>());
                using (var d = new Dijalog())
                {
                    d.Slozi(forma, new SqlKontekst(), delegate (string x)
                    {
                        var t = new DataTable();
                        t.Columns.Add("id", typeof(int));
                        t.Columns.Add("naziv", typeof(string));
                        t.Columns.Add("selectedid", typeof(int));
                        t.Rows.Add(1, "a", 1);
                        t.Rows.Add(2, "b", 0);
                        return t;
                    });
                    var grid = (DevExpress.XtraGrid.GridControl)d.Controls.Find("g", true)[0];
                    var v = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    T("chk01-selectedid", v.IsRowSelected(v.GetRowHandle(0)) && !v.IsRowSelected(v.GetRowHandle(1)), "sel=" + v.SelectedRowsCount);
                }
            }
            catch (Exception ex)
            {
                T("chk01", false, ex.ToString());
            }
        }

        private static NgColumn Stupac(string ime, string natpis, string vidljiv, string agregat, string format)
        {
            var a = new Dictionary<string, string>
            {
                { "INDEX", "" }, { "NAME", ime }, { "WIDTH", "1400" }, { "CAPTION", natpis }, { "FORMAT", format },
                { "VISIBLE", vidljiv }, { "AGREGATEFUNCTION", agregat }
            };
            return new NgColumn(ime, agregat, false, new NgAttributeBag(a));
        }

        private static object Polje(object o, string ime)
        {
            for (Type t = o.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo f = t.GetField(ime, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (f != null)
                    return f.GetValue(o);
            }
            return null;
        }

        private static object Zovi(object o, string ime, params object[] arg)
        {
            MethodInfo m = o.GetType().GetMethod(ime, BindingFlags.Instance | BindingFlags.NonPublic);
            return m == null ? null : m.Invoke(o, arg);
        }

        /// <summary>
        /// Podloga na spinu: EDITCOLUMNS/ONCELLEDITED (EDT-01/02), lanac desnog klika kroz Rafalno (RAF-01),
        /// "select" u lancu gumba (BTN-03), REFRESHMASTERGRID sa zadnjim SQL-om (REF-01), ONCLOSED.
        /// </summary>
        private static void PodlogaRafalno()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ngraf" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            string xml = Path.Combine(dir, "RafTest.xml");
            File.WriteAllText(xml, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"
                + "<PROJECT CMD1CAPTION=\"Dodaj:frmNema\" CMD2CAPTION=\"Stanje:select &apos;disable&apos; as Dodaj\""
                + " ONLOAD=\"select 1 as id, &apos;abc&apos; as Naziv union all select 2, &apos;def&apos;\""
                + " EDITCOLUMNS=\"Naziv\" CHECKBOXVISIBLE=\"TRUE\""
                + " ONCELLEDITED=\"select &apos;@@Naziv,@@EDITEDCOLUMN,@@LASTVALUE,@@id&apos; as setcolorforcolorpicker\""
                + " ONCLOSED=\"select &apos;zatvoreno&apos; as setcolorforcolorpicker\">\r\n"
                + "<NAME>Projekt</NAME>\r\n<CAPTION>Rafalno test</CAPTION>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
            string greska = null;
            NgPoruka.Zapis = delegate (string x) { greska = x; };
            // Bez Application.DoEvents: Shown baznog Managera bez DI (Logger) u GridTestu baci iznimku i otvori modalni ThreadExceptionDialog.
            var forma = new Podloga();
            MasterGrid master = null;
            try
            {
                forma.ShowInTaskbar = false;
                forma.StartPosition = FormStartPosition.Manual;
                forma.Location = new Point(-32000, -32000);
                forma.Show();
                bool otvoren = forma.Prikazi(xml, "localhost", "spin", 7);
                var gv = Polje(forma, "gvGla") as DevExpress.XtraGrid.Views.Grid.GridView;
                master = Polje(forma, "_master") as MasterGrid;
                if (!otvoren || gv == null || master == null)
                {
                    T("edt01-uredjiv", false, "nije otvoreno");
                    return;
                }
                var naziv = gv.Columns.ColumnByFieldName("Naziv");
                var id = gv.Columns.ColumnByFieldName("id");
                T("edt01-uredjiv", gv.OptionsBehavior.Editable && naziv != null && naziv.OptionsColumn.AllowEdit && id != null && !id.OptionsColumn.AllowEdit,
                    "editable=" + gv.OptionsBehavior.Editable + " rows=" + gv.DataRowCount);
                // DGR-01: Class1.vb 1112 HeaderAlignment Far za int stupac, i na podlozi.
                T("dgr01-zaglavlje-broja", id != null && id.AppearanceHeader.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Far,
                    id == null ? "nema id" : id.AppearanceHeader.TextOptions.HAlignment.ToString());
                gv.FocusedRowHandle = 0;
                // '|' nije granica tokena u SrediSQLPosljeNextForme (Class1 766-820), zato zarez.
                gv.SetRowCellValue(0, naziv, "xyz");
                string boja = master.Kontekst.SelectedColor ?? "";
                T("edt02-oncelledited", boja == "xyz,Naziv,xyz,1" && gv.IsRowSelected(0), boja + " sel=" + gv.IsRowSelected(0));

                FieldInfo projektPolje = typeof(Podloga).GetField("_projekt", BindingFlags.Instance | BindingFlags.NonPublic);
                var projekt = projektPolje.GetValue(forma) as NgProject;
                var mapa = typeof(NgAttributeBag).GetField("_map", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(projekt.Atributi) as Dictionary<string, string>;
                mapa["ONCELLEDITED"] = "select 1/0 as x";
                // Kao korisnik: ShowingEditor pamti staru vrijednost, PostEditor šalje CellValueChanged.
                gv.FocusedRowHandle = 0;
                gv.FocusedColumn = naziv;
                gv.ShowEditor();
                if (gv.ActiveEditor != null)
                {
                    gv.ActiveEditor.EditValue = "pogresno";
                    gv.PostEditor();
                    gv.CloseEditor();
                }
                else
                    gv.SetRowCellValue(0, naziv, "pogresno");
                string vraceno = Convert.ToString(gv.GetRowCellValue(0, naziv));
                T("edt03-greska-vraca", (vraceno == "xyz" || vraceno == "abc") && (greska ?? "").Length > 0,
                    Convert.ToString(gv.GetRowCellValue(0, naziv)) + " | " + greska);

                master.Kontekst.SelectedColor = "";
                Zovi(forma, "IzvrsiIzbornik", "frmNema~select 'q' as setcolorforcolorpicker");
                T("raf01-izbornik-sql", master.Kontekst.SelectedColor == "q", master.Kontekst.SelectedColor);

                var gumbi = Polje(forma, "pnlButtons") as Control;
                DevExpress.XtraEditors.SimpleButton dodaj = null;
                if (gumbi != null)
                {
                    foreach (Control c in gumbi.Controls)
                    {
                        var b = c as DevExpress.XtraEditors.SimpleButton;
                        if (b != null && b.Text == "Dodaj")
                            dodaj = b;
                    }
                }
                Zovi(forma, "IzvrsiLanac", "select 'disable' as Dodaj");
                T("btn03-select-gumbi", dodaj != null && !dodaj.Enabled, dodaj == null ? "nema gumba" : dodaj.Enabled.ToString());

                master.Stanje.SqlZaSlanje = "select 9 as id, 'z' as Naziv";
                master.Stanje.Odluka = "salji";
                Zovi(forma, "IzvrsiRafalno", "REFRESHMASTERGRID");
                T("ref01-zadnji-sql", gv.DataRowCount == 1 && Convert.ToString(gv.GetRowCellValue(0, "id")) == "9", "redova=" + gv.DataRowCount);
                // KEY-01: JupiterShared_KeyDown bez DefinitionForm bacao bi NRE na svaku tipku; Escape zatvara (Form1_KeyDown).
                Zovi(forma, "OnKeyDown", new KeyEventArgs(Keys.Down));
                T("key01-tipka", true, "");
                Zovi(forma, "OnKeyDown", new KeyEventArgs(Keys.Escape));
                T("key01-escape", forma.IsDisposed || !forma.Visible, "visible=" + (!forma.IsDisposed && forma.Visible));
            }
            catch (Exception ex)
            {
                T("podloga-rafalno", false, ex.ToString());
            }
            finally
            {
                forma.Close();
                forma.Dispose();
                NgPoruka.Zapis = null;
            }
            T("onclosed", master != null && master.Kontekst.SelectedColor == "zatvoreno", master == null ? "" : master.Kontekst.SelectedColor);
            try { Directory.Delete(dir, true); } catch { }
        }

        private static void PokaziUpitUcinak()
        {
            string root = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");
            if (string.IsNullOrEmpty(root))
                root = Directory.GetCurrentDirectory();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "JeziciNG.xml");
            if (!File.Exists(xml))
            {
                T("pokazi-upit", false, "nema " + xml);
                return;
            }
            var forma = new Podloga();
            try
            {
                forma.ShowInTaskbar = false;
                forma.StartPosition = FormStartPosition.Manual;
                forma.Location = new Point(-32000, -32000);
                forma.Show();
                bool otvoren = forma.Prikazi(xml, "localhost", "spin", 7);
                FieldInfo gvPolje = typeof(Podloga).GetField("gvGla", BindingFlags.Instance | BindingFlags.NonPublic);
                object gv = gvPolje == null ? null : gvPolje.GetValue(forma);
                int redova = -1;
                if (gv != null)
                {
                    PropertyInfo broj = gv.GetType().GetProperty("DataRowCount");
                    if (broj != null)
                        redova = Convert.ToInt32(broj.GetValue(gv, null));
                }
                FieldInfo masterPolje = typeof(Podloga).GetField("_master", BindingFlags.Instance | BindingFlags.NonPublic);
                MasterGrid master = masterPolje == null ? null : masterPolje.GetValue(forma) as MasterGrid;
                string greska = master == null || master.Stanje == null ? "" : (master.Stanje.SqlGreska ?? "");
                string sql = master == null || master.Stanje == null ? "" : (master.Stanje.SqlZaSlanje ?? "");
                T("pokazi-upit", otvoren && redova > 0 && greska.Length == 0 && sql.IndexOf("spJeziciQuery", StringComparison.Ordinal) >= 0,
                    "redovi=" + redova.ToString() + " " + greska + " " + sql);
            }
            finally
            {
                forma.Close();
                forma.Dispose();
            }
        }

        private static void FiskalUuidSpin()
        {
            string spoj = SpinVeza.Spoj("localhost", "spin");
            try
            {
                using (var conn = new SqlConnection(spoj))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        object id;
                        using (var nadji = new SqlCommand("SELECT TOP 1 mpblagajnaglaid FROM mpblagajnagla", conn, tx))
                            id = nadji.ExecuteScalar();
                        if (id == null || id == DBNull.Value)
                        {
                            T("fiskal-uuid-spin", false, "nema retka u mpblagajnagla");
                            tx.Rollback();
                            return;
                        }
                        string sql = Carpo.NetGen.Komande.FiskalTok.SqlKod(1, Convert.ToString(id), "UUID-NG", "ZKI-NG");
                        using (var upis = new SqlCommand(sql, conn, tx))
                            upis.ExecuteNonQuery();
                        using (var citaj = new SqlCommand("SELECT UUID, KOD FROM mpblagajnagla WHERE mpblagajnaglaid=" + Convert.ToString(id), conn, tx))
                        using (var r = citaj.ExecuteReader())
                        {
                            r.Read();
                            bool ok = Convert.ToString(r[0]) == "UUID-NG" && Convert.ToString(r[1]) == "ZKI-NG";
                            T("fiskal-uuid-spin", ok, Convert.ToString(r[0]) + " " + Convert.ToString(r[1]));
                        }
                        tx.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                T("fiskal-uuid-spin", false, ex.Message);
            }
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
            SpinGreska();
        }

        /// <summary>
        /// M1: Jupiter ulaz Loader.Run8 otvori istu Podlogu kao TestShell (Form1.Run: SetParent na clientHandle,
        /// Show, hWnd). Zadnji test: forma registrira Logger u DependencyContainer.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, uint flags);

        private static void LoaderOtvara()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ngm1" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            string xml = Path.Combine(dir, "LoaderTest.xml");
            File.WriteAllText(xml, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT ONLOAD=\"select 1 as id, &apos;a&apos; as Naziv\">\r\n"
                + "<NAME>L</NAME>\r\n<CAPTION>Loader test</CAPTION>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
            Form forma = null;
            try
            {
                var loader = new Carpo.NetGen.Loader();
                loader.TestXmlPath = xml;
                loader.moduleId = "LoaderTest.xml";
                loader.Run8("localhost", "spin", "", "", 7, 0, 0, 0, "");
                forma = loader.Forma;
                T("m1-run8-podloga", forma is Podloga && loader.hWnd != 0 && loader.WCaption == "Loader test" && string.IsNullOrEmpty(loader.ZadnjaGreska),
                    "forma=" + (forma == null ? "null" : forma.GetType().Name) + " hWnd=" + loader.hWnd + " caption=" + loader.WCaption + " greska=" + loader.ZadnjaGreska);
                // ULZ-02: ulaz frm otvara dijalog ulaznaForma (inace MAINFORM) s ID-om ulazniID (Form2.Run 55-65).
                string dijXml = Path.Combine(dir, "FrmTest.xml");
                File.WriteAllText(dijXml, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<PROJECT>\r\n<NAME>F</NAME>\r\n<CAPTION>Frm test</CAPTION>\r\n"
                    + "<MAINFORM CAPTION=\"Glavna\"><INPUT TYPE=\"label\" NAME=\"lbl\" CAPTION=\"x\" /></MAINFORM>\r\n"
                    + "<DRUGA CAPTION=\"Druga @@id\"><INPUT TYPE=\"text\" NAME=\"t\" /></DRUGA>\r\n</PROJECT>\r\n", new UTF8Encoding(false));
                var f = new Carpo.NetGen.frm();
                f.TestXmlPath = dijXml;
                f.moduleId = "FrmTest.xml";
                f.ulaznaForma = "DRUGA";
                f.ulazniID = "42";
                f.Run8("localhost", "spin", "", "", 7, 0, 0, 0, "");
                var dij = f.Forma as Dijalog;
                T("m1-frm-dijalog", dij != null && dij.Visible && f.hWnd != 0 && dij.Id == "42" && dij.Controls.Find("t", true).Length == 1,
                    "forma=" + (f.Forma == null ? "null" : f.Forma.GetType().Name) + " id=" + (dij == null ? "" : dij.Id) + " greska=" + f.ZadnjaGreska);
                if (dij != null)
                {
                    dij.Close();
                    dij.Dispose();
                }
                // ULZ-04: Form2 PrvaForma 993-1028 (SetParent na clientHandle; Koler Jup10 bez ruba i veličine
                // roditelja) i FormClosing 4112-4113 (SendMessage main_handle 0x7D0).
                using (var domacin = new PrimateljPoruka())
                {
                    var ploca = new Panel();
                    domacin.Controls.Add(ploca);
                    IntPtr hDomacin = domacin.Handle;
                    IntPtr hPloca = ploca.Handle;
                    var f2 = new Carpo.NetGen.frm();
                    f2.TestXmlPath = dijXml;
                    f2.moduleId = "FrmTest.xml";
                    f2.mainHandle = (int)hDomacin;
                    f2.Koler = "Jup10";
                    f2.Run8("localhost", "spin", "", "", 7, (int)hPloca, 0, 0, "");
                    var d2 = f2.Forma as Dijalog;
                    // GetParent za prozor bez WS_CHILD vraća vlasnika; stvarni roditelj je GetAncestor(GA_PARENT).
                    IntPtr roditelj = d2 == null ? IntPtr.Zero : GetAncestor(d2.Handle, 1);
                    bool ugradjen = d2 != null && roditelj == hPloca && d2.FormBorderStyle == FormBorderStyle.None;
                    ploca.Size = new Size(321, 123);
                    // Location prozora bez WS_CHILD WinForms čita u koordinatama ekrana, pa se gleda veličina.
                    bool prati = d2 != null && d2.Size == new Size(321, 123);
                    IntPtr hDijaloga = d2 == null ? IntPtr.Zero : d2.Handle;
                    string opis = d2 == null ? "nema" : "roditelj=" + roditelj + "/" + hPloca + " rub=" + d2.FormBorderStyle + " vel=" + d2.Size;
                    if (d2 != null)
                    {
                        d2.Close();
                        d2.Dispose();
                    }
                    T("ulz04-frm-jupiter", ugradjen && prati && domacin.Zatvoren == hDijaloga && hDijaloga != IntPtr.Zero,
                        opis + " poruka=" + domacin.Zatvoren + " greska=" + f2.ZadnjaGreska);
                }
            }
            catch (Exception ex)
            {
                T("m1-run8", false, ex.ToString());
            }
            finally
            {
                try
                {
                    if (forma != null && !forma.IsDisposed)
                    {
                        forma.Close();
                        forma.Dispose();
                    }
                }
                catch (Exception ex) { Console.WriteLine("m1 zatvaranje: " + ex.Message); }
                try { Directory.Delete(dir, true); } catch (IOException) { }
            }
        }

        private static void TragaNaStroju()
        {
            string izv = TragaDll.Izvjestaj() + TragaDll.Registar();
            Console.WriteLine(izv);
            string donijeti = TragaDll.Donijeti();
            if (donijeti.Length > 0)
                Console.WriteLine("DONIJETI\r\n" + donijeti);
            bool spin = TragaDll.Nadji("Spin.Printing.v26.dll") != null;
            bool prevDat = TragaDll.Nadji("JupiterTranslator10.dll") != null || TragaDll.Nadji("JupiterTranslator.dll") != null;
            Type prevTip = TragaDll.Tip("JupiterTranslator10.dll", "JupiterTranslator.Translator");
            if (prevTip == null)
                prevTip = TragaDll.Tip("JupiterTranslator.dll", "JupiterTranslator.Translator");
            string prevod = "";
            if (prevTip != null)
            {
                try
                {
                    object r = prevTip.InvokeMember("TranslateString", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { "Spremi", "NetGen" });
                    prevod = r == null ? "" : Convert.ToString(r);
                }
                catch (Exception ex)
                {
                    prevod = ex.GetBaseException().Message;
                }
            }
            T("dll-traga", spin && prevDat && izv.IndexOf("Spin.Printing.v26.dll", StringComparison.Ordinal) >= 0, izv + donijeti + " PREVOD=" + prevod);
            Type raverus = TragaDll.Tip("Raverus.FiskalizacijaDEV.COM.dll", "Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav");
            if (raverus != null)
                FiskalPoziv.Cis = Activator.CreateInstance(raverus);
            T("raverus-slo", true, (raverus == null ? "NEMA Raverus" : "IMA Raverus") + " " + (TragaDll.Nadji("SLOFISKAL.dll") ?? "NEMA SLOFISKAL"));
        }

        private static void SpinGreska()
        {
            string spoj = SpinVeza.Spoj("localhost", "spin");
            string tekst = GreskaSql.Poziv(4242, "test", "select 1");
            try
            {
                using (var conn = new SqlConnection(spoj))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var cmd = new SqlCommand(tekst, conn, tx))
                        {
                            cmd.CommandTimeout = GreskaSql.TimeoutSekundi;
                            cmd.ExecuteNonQuery();
                        }
                        object n;
                        using (var citaj = new SqlCommand("SELECT COUNT(*) FROM dbo.NetGenErrorLog WHERE errorNumber=4242", conn, tx))
                            n = citaj.ExecuteScalar();
                        tx.Rollback();
                        T("execsql2-spin", Convert.ToInt32(n) == 1, Convert.ToString(n));
                    }
                }
            }
            catch (Exception ex)
            {
                T("execsql2-spin", false, ex.Message);
            }
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

    /// <summary>Jupiter glavni prozor u testu: pamti wParam poruke 0x7D0 (zatvoren prozor granule).</summary>
    public sealed class PrimateljPoruka : Form
    {
        public IntPtr Zatvoren = IntPtr.Zero;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7D0)
                Zatvoren = m.WParam;
            base.WndProc(ref m);
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

        public string GenerirajUuid()
        {
            return "UUID-TEST";
        }

        public string ZastitniKodIzracunCertifikatDatoteka(string put, string lozinka, string oib, string datum, string broj, string prostor, string uredaj, string iznos)
        {
            return "ZKI-TEST";
        }

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
