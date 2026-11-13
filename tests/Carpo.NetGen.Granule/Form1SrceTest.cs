using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Ulaz;

namespace Carpo.NetGen.Granule
{
    internal static class Form1SrceTest
    {
        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            l.Add(Upit());
            l.Add(Dozvola());
            l.Add(FormatIRaspored());
            l.Add(DmsIOstalo());
            l.Add(PrevodIPogled());
            return l;
        }

        private static Stavka Upit()
        {
            const string g = "Form1";
            const string id = "F1-33";
            const string opis = "prazan SQL izlazi, DX ne pali timer, grupe su indeks plus 2";
            const string izvor = "netgen/Form1.vb:1825-2180; netgen/Form1.vb:2270-2276";
            Form1Srce.Upit prazan = Form1Srce.Pokazi("", "1", "", false, "", false, "", "", "", 100, false, "", "", "", "", "", "");
            if (!prazan.PrazanIzlaz || !prazan.PunimMaster || prazan.ZadnjiSql.Length > 0)
                return Qa.P(g, id, opis, izvor, "prazan");
            Form1Srce.Upit idNovi = Form1Srce.Pokazi("select 1", "3", "15", false, "", false, "", "", "", 100, false, "", "", "", "", "", "");
            if (idNovi.Id != "15" || !idNovi.Timer || !idNovi.UcitajRaspored || idNovi.Splitter != 50)
                return Qa.P(g, id, opis, izvor, "id=" + idNovi.Id);
            Form1Srce.Upit nula = Form1Srce.Pokazi("select 1", "3", "0", false, "self", false, "FALSE", "FALSE", "", 100, true, "1~", "true", "", "", "", "");
            if (nula.Id != "3" || nula.SelfRoditelj != "id" || nula.SelfDijete != "parentid" || nula.Kvacica || nula.Total || !nula.ZadnjiRed)
                return Qa.P(g, id, opis, izvor, "self");
            if (nula.Grupe.Count != 1 || nula.Grupe[0] != 3 || !nula.Filter || nula.DozvoliUredivanje)
                return Qa.P(g, id, opis, izvor, "grupe");
            Form1Srce.Upit dx = Form1Srce.Pokazi("select 1", "1", "", true, "", false, "", "", ":FORMA", 80, false, "", "", "", "", "", "");
            if (!dx.Dx || dx.Timer || dx.Poruka != "sorkac!" || !dx.DetaljJeForma)
                return Qa.P(g, id, opis, izvor, dx.Poruka);
            if (Form1Srce.Detalj(true, false, "select 1", 100) != "")
                return Qa.P(g, id, opis, izvor, "punim");
            if (Form1Srce.Detalj(false, false, "", 100) != "splitter=50")
                return Qa.P(g, id, opis, izvor, Form1Srce.Detalj(false, false, "", 100));
            var k = new SqlKontekst();
            k.Id = "4";
            if (Form1Srce.Rafalna("x @@id", k) != "x 4")
                return Qa.P(g, id, opis, izvor, Form1Srce.Rafalna("x @@id", k));
            Console.WriteLine("[Z] Form1 F1-33 prazan-izlaz id=15 dx-bez-timera grupe=3");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Dozvola()
        {
            const string g = "Form1";
            const string id = "F1-44";
            const string opis = "True na gumbu ostaje, gumb 8 se na vidljivost true sakrije";
            const string izvor = "netgen/Form1.vb:2742-2961";
            var g1 = new Form1Srce.GumbStanje();
            Form1Srce.Dozvola(1, g1, false, null);
            Form1Srce.Dozvola(1, g1, true, null);
            Form1Srce.Dozvola(1, g1, false, null);
            if (!g1.Ukljucen || !g1.Kvaka)
                return Qa.P(g, id, opis, izvor, "gumb1");
            var g8 = new Form1Srce.GumbStanje();
            Form1Srce.Dozvola(8, g8, null, true);
            if (g8.Vidljiv)
                return Qa.P(g, id, opis, izvor, "gumb8");
            var g3 = new Form1Srce.GumbStanje();
            Form1Srce.Dozvola(3, g3, true, false);
            if (g3.Vidljiv)
                return Qa.P(g, id, opis, izvor, "gumb3");
            Console.WriteLine("[Z] Form1 F1-44 gumb1-ostaje gumb8-skriven gumb3-false");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka FormatIRaspored()
        {
            const string g = "Form1";
            const string id = "F1-140";
            const string opis = "širina kroz 14, DX raspored je xml i briše se";
            const string izvor = "netgen/Form1.vb:7451-7535; netgen/Form1.vb:6246-6372";
            Form1Srce.Format f = Form1Srce.Kolona(0, "Naziv", 140, "A;B", "n", "true", "Red", "", 2, "sum", true, false, "3", false, true, "center");
            if (f.Kljuc != "Naziv" || f.Sirina != 10 || f.Natpis != "A\r\nB" || !f.Prelomi || f.Agregat != "sum" || !f.NegativnoCrveno || f.Boja != "red" || !f.SakrijSve)
                return Qa.P(g, id, opis, izvor, "format " + f.Sirina + " " + f.Natpis);
            string mapa = Path.Combine(Path.GetTempPath(), "ng-raspored-" + Guid.NewGuid().ToString("N"));
            string put = Form1Srce.Datoteka(mapa, "JeziciNG", "hrv", true, false);
            try
            {
                if (!put.EndsWith("\\GridMasterJeziciNG-hrv.xml", StringComparison.Ordinal))
                    return Qa.P(g, id, opis, izvor, put);
                Form1Srce.Spremi(put, "<layout>1</layout>");
                if (Form1Srce.Ucitaj(put, false) != "<layout>1</layout>")
                    return Qa.P(g, id, opis, izvor, "ucitaj");
                if (Form1Srce.Ucitaj(put, true) != "")
                    return Qa.P(g, id, opis, izvor, "reset");
                if (!Form1Srce.Obrisi(put) || File.Exists(put))
                    return Qa.P(g, id, opis, izvor, "brisanje");
            }
            finally
            {
                if (Directory.Exists(mapa))
                    Directory.Delete(mapa, true);
            }
            if (!Form1Srce.Datoteka(mapa, "JeziciNG", "hrv", false, true).EndsWith("\\GridDetJeziciNG-hrv.gxl", StringComparison.Ordinal))
                return Qa.P(g, id, opis, izvor, "det");
            Console.WriteLine("[Z] Form1 F1-140 sirina=10 natpis=A/B raspored=GridMasterJeziciNG-hrv.xml");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka DmsIOstalo()
        {
            const string g = "Form1";
            const string id = "F1-185";
            const string opis = "DmsManagerV1 se učitava, nema sklopa prekida, ostali događaji drže NetGen";
            const string izvor = "netgen/Form1.vb:8488-8665; netgen/Form1.vb:6912-8726";
            string greska = Form1Srce.UcitajDms();
            if (greska.IndexOf("Assembly.Load DMSScan", StringComparison.Ordinal) < 0 || greska.IndexOf("DmsManagerV1", StringComparison.Ordinal) < 0)
                return Qa.P(g, id, opis, izvor, greska);
            if (Form1Srce.DmsKlasa("checkout") != "DmsManager.Db.Documents" || Form1Srce.DmsMetoda("checkout") != "DocumentCheckOut")
                return Qa.P(g, id, opis, izvor, "checkout");
            if (Form1Srce.DmsKlasa("pogledaj") != "DmsManagerV1.Start" || Form1Srce.DmsMetoda("pogledaj") != "OpenDocument")
                return Qa.P(g, id, opis, izvor, "pogledaj");
            if (Form1Srce.DmsMetoda("spremi") != "SaveDocuments" || Form1Srce.DmsMetoda("checkin") != "DocumentCheckIn")
                return Qa.P(g, id, opis, izvor, "metoda");
            if (Form1Srce.SqlDmsBrisi("faktura", "9") != "spNetGenDMSAzur @action='delete',@EntityName='faktura',@selectedids ='9'")
                return Qa.P(g, id, opis, izvor, Form1Srce.SqlDmsBrisi("faktura", "9"));
            if (!Form1Srce.DxPreskaceJanus(true) || Form1Srce.DxPreskaceJanus(false))
                return Qa.P(g, id, opis, izvor, "dx");
            if (!Form1Srce.DesniKlik("Right") || Form1Srce.EnterForma("Enter", "FILTER") != "FILTER")
                return Qa.P(g, id, opis, izvor, "klik");
            if (!Form1Srce.KontrolnaTipka(17) || !Form1Srce.KvacicaNakonZapisa(false) || Form1Srce.KvacicaNakonZapisa(true))
                return Qa.P(g, id, opis, izvor, "tipka");
            if (!Form1Srce.ZatvaranjeOtkazano(true) || Form1Srce.StupacNakonKlika(true) || !Form1Srce.StupacNakonKlika(false))
                return Qa.P(g, id, opis, izvor, "stupac");
            if (Form1Srce.AplikacijaIzTaga("  ") != "" || Form1Srce.AplikacijaIzTaga(@"C:\a.xml") != @"C:\a.xml")
                return Qa.P(g, id, opis, izvor, "tag");
            if (!Form1Srce.TimerKlik(false) || Form1Srce.TimerKlik(true))
                return Qa.P(g, id, opis, izvor, "timer");
            if (Form1Srce.BojaPrijeOdabira() != "White" || Form1Srce.BojaBezFokusa() != "LightGreen")
                return Qa.P(g, id, opis, izvor, "boja");
            if (Form1Srce.StabloSirina(true, 180) != 1 || Form1Srce.StabloSirina(false, 180) != 180)
                return Qa.P(g, id, opis, izvor, "sirina");
            if (!Form1Srce.ResizePaliTimer(true) || Form1Srce.ResizePaliTimer(false))
                return Qa.P(g, id, opis, izvor, "resize");
            int[] rod = Form1Srce.Roditelj(40, 20);
            if (rod[0] != 0 || rod[1] != 0 || rod[2] != 40 || rod[3] != 20)
                return Qa.P(g, id, opis, izvor, "roditelj");
            if (Form1Srce.IzvozIme(true) != "exportmaster.xls" || Form1Srce.IzvozIme(false) != "export.xls")
                return Qa.P(g, id, opis, izvor, "izvoz");
            if (Form1Srce.RedoslijedUcitavanja() != "ulaz,spoj,operater,drvo,jezik,xml,ngparams")
                return Qa.P(g, id, opis, izvor, "redoslijed");
            if (Form1Srce.SpojiParametre("", "@a=1") != "" || Form1Srce.SpojiParametre("@a=1", "@b=2") != "@a=1@b=2")
                return Qa.P(g, id, opis, izvor, "param");
            if (Form1Srce.TreciNivo(true) != 1 || Form1Srce.TreciNivo(false) != 0)
                return Qa.P(g, id, opis, izvor, "nivo");
            if (!Form1Srce.SearchJeStupac(true) || Form1Srce.SearchJeStupac(false) || !Form1Srce.GrupeSeSklope())
                return Qa.P(g, id, opis, izvor, "search");
            if (Form1Srce.Podnozje("", 2, 9) != "Stranica 2 / 9" || Form1Srce.Podnozje("Spin", 2, 9) != "Spin")
                return Qa.P(g, id, opis, izvor, Form1Srce.Podnozje("", 2, 9));
            string[] red = Form1Srce.Red("3", false, "#112233", "#ffffff", false);
            if (red[0] != "3" || red[1] != "#112233" || red[2] != "#ffffff")
                return Qa.P(g, id, opis, izvor, "red");
            if (Form1Srce.Red("", false, "a", "b", true)[1] != "")
                return Qa.P(g, id, opis, izvor, "boja-greska");
            if (Form1Srce.TekstPovlacenja("cvor") != "cvor" || Form1Srce.NoviProzor() != "store-all,dock-fill,row-headers-off")
                return Qa.P(g, id, opis, izvor, "novi");
            Console.WriteLine("[Z] Form1 F1-185 dms=DmsManagerV1 nema-sklopa checkout=DocumentCheckOut brisi=spNetGenDMSAzur");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka PrevodIPogled()
        {
            const string g = "Form1";
            const string id = "F1-114";
            const string opis = "Run učita JupiterTranslator10, pogledi idu na GridView CardView LayoutView";
            const string izvor = "netgen/Form1.vb:1106-1115; netgen/Form1.vb:6161-6184";
            Form1Srce.Pogled tablica = Form1Srce.PogledIz("TableView");
            Form1Srce.Pogled kartica = Form1Srce.PogledIz("CardView");
            Form1Srce.Pogled jedna = Form1Srce.PogledIz("SingleCard");
            if (tablica.Kontrola != "GridView" || !tablica.Ikona || tablica.Navigator || tablica.NoviRed != "Top")
                return Qa.P(g, id, opis, izvor, "table");
            if (kartica.Kontrola != "CardView" || kartica.Ikona || !kartica.ProsiriKartice || kartica.Navigator)
                return Qa.P(g, id, opis, izvor, "card");
            if (jedna.Kontrola != "LayoutView" || jedna.Ikona || jedna.ProsiriKartice || !jedna.Navigator || jedna.NoviRed != "Bottom")
                return Qa.P(g, id, opis, izvor, "single");
            Prevoditelj.Ishod t = Prevoditelj.Ucitaj();
            if (t.Ucitano)
            {
                if (t.Tip != Prevoditelj.Tip || t.Ime != Prevoditelj.Sklop)
                    return Qa.P(g, id, opis, izvor, t.Ime + " " + t.Tip + " " + t.Verzija);
                Console.WriteLine("[Z] Form1 F1-114 JupiterTranslator10 " + t.Verzija + " tip=" + t.Tip + " put=" + t.Putanja);
            }
            else
            {
                Console.WriteLine("[Z] Form1 F1-114 nema JupiterTranslator10.dll Assembly.Load(\"JupiterTranslator10\") tip=JupiterTranslator.Translator susjed=" + t.Susjed + " poruka=" + t.Poruka);
            }
            var loader = new Carpo.NetGen.Loader();
            loader.TestXmlPath = "";
            loader.Run("localhost", "spin", "", "", 1, 0, 0, 0, "");
            if (loader.PrevoditeljUcitano != t.Ucitano)
                return Qa.P(g, id, opis, izvor, "run");
            return Qa.Z(g, id, opis, izvor);
        }
    }
}
