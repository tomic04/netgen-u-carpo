using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Carpo.NetGen.Ulaz;

namespace Carpo.NetGen.Granule
{
    /// <summary>Form1.vb funkcije 1–50 koje su nedostajale. Očekivano je NetGen, ne prilagodba.</summary>
    internal static class Form1Blok
    {
        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            l.Add(Alatna());
            l.Add(Tipka());
            l.Add(Dozvola());
            l.Add(Ulaz());
            l.Add(Prostor());
            l.Add(Stavke());
            l.Add(Format());
            l.Add(StabloMeni());
            l.Add(Rafal());
            l.Add(Pomoc());
            return l;
        }

        private static Stavka Alatna()
        {
            const string g = "Form1";
            const string id = "F1-06";
            const string opis = "alatna traka šalje samo vidljive gumbe 1-5";
            const string izvor = "netgen/Form1.vb:233-260";
            string xml = Form1Ulaz.AlatnaTraka(
                new[] { true, false, true, false, false },
                new[] { "Unesi", "Izmijeni", "Obriši", "", "" },
                new[] { true, true, false, false, false });
            string ocek = "<buttons><button type=\"button\" name=\"button1\" caption=\"Unesi\" icon=\" \" enabled=\"True\" visible=\"True\" /><button type=\"button\" name=\"button3\" caption=\"Obriši\" icon=\" \" enabled=\"False\" visible=\"True\" /></buttons>";
            if (xml != ocek || Form1Ulaz.KlikAlatne("button3") != "button3" || Form1Ulaz.KlikAlatne("button9") != "")
                return Qa.P(g, id, opis, izvor, xml);
            if (!Form1Ulaz.ResizeAkoJeMaksimiziran(true) || Form1Ulaz.ResizeAkoJeMaksimiziran(false))
                return Qa.P(g, id, opis, izvor, "resize");
            if (!Form1Ulaz.TrebaPoravnati(0x7E) || !Form1Ulaz.TrebaPoravnati(0x2E0) || Form1Ulaz.TrebaPoravnati(0x7D1))
                return Qa.P(g, id, opis, izvor, "poruka");
            Console.WriteLine("[Z] Form1 F1-06 alatna=button1,button3 klik=button3");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Tipka()
        {
            const string g = "Form1";
            const string id = "F1-25";
            const string opis = "Escape zatvara, F1 samo uz HELPKOD, Ctrl+F11 parametri";
            const string izvor = "netgen/Form1.vb:1350-1392";
            Form1Ulaz.TipkaIshod esc = Form1Ulaz.Tipka("Escape", false, "");
            Form1Ulaz.TipkaIshod f1prazan = Form1Ulaz.Tipka("F1", false, "  ");
            Form1Ulaz.TipkaIshod f1 = Form1Ulaz.Tipka("F1", true, "MER005");
            Form1Ulaz.TipkaIshod f11 = Form1Ulaz.Tipka("F11", true, "");
            Form1Ulaz.TipkaIshod f9 = Form1Ulaz.Tipka("F9", false, "");
            if (!esc.Zatvori || f1prazan.Pomoc || !f1.Pomoc || f1.Parametri || !f11.Parametri || f9.Jezici)
                return Qa.P(g, id, opis, izvor, "tipka");
            if (Form1Ulaz.Zatvoreno("spKraj") != "spKraj")
                return Qa.P(g, id, opis, izvor, "onclosed");
            if (Form1Ulaz.DajParametre("@id=1") != "PARAMATERISAMASTERIDETAILGRIDA=@id=1")
                return Qa.P(g, id, opis, izvor, "param");
            if (Form1Ulaz.DajSql(new[] { "a", "b" }) != "a\r\n\r\nb\r\n\r\n")
                return Qa.P(g, id, opis, izvor, "dajsql");
            Console.WriteLine("[Z] Form1 F1-25 escape zatvara, F1 uz HELPKOD, F11 parametri");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Dozvola()
        {
            const string g = "Form1";
            const string id = "F1-42";
            const string opis = "True na dozvoli izlazi iz petlje i pobjeđuje";
            const string izvor = "netgen/Form1.vb:2665-2707";
            var t = new DataTable();
            t.Columns.Add("functionname");
            t.Columns.Add("smije");
            t.Rows.Add("brisi", false);
            t.Rows.Add("brisi", true);
            t.Rows.Add("brisi", false);
            if (!Form1Ulaz.DozvolaKomande(":BRISI", t))
                return Qa.P(g, id, opis, izvor, "true nije pobijedio");
            t.Rows.Clear();
            t.Rows.Add("brisi", false);
            if (Form1Ulaz.DozvolaKomande("brisi", t))
                return Qa.P(g, id, opis, izvor, "false je prošao");
            if (!Form1Ulaz.DozvolaKomande("nema", null))
                return Qa.P(g, id, opis, izvor, "prazno nije true");
            string sql = Form1Ulaz.SqlDozvole(@"C:\xml\JeziciNG.xml", 7);
            if (sql.IndexOf("objectname='JeziciNG.xml'", StringComparison.Ordinal) < 0 || sql.IndexOf("order by sort ,isgrupa desc", StringComparison.Ordinal) < 0)
                return Qa.P(g, id, opis, izvor, sql);
            string op = Form1Ulaz.SqlOperatera(" Spin ", "s");
            if (op != "select * from ssoperateri where naziv ='Spin' and password='s'")
                return Qa.P(g, id, opis, izvor, op);
            Console.WriteLine("[Z] Form1 F1-42 dozvola true izlazi, sql=ssPermission");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Ulaz()
        {
            const string g = "Form1";
            const string id = "F1-31";
            const string opis = "naredbeni red puni server, bazu i xml";
            const string izvor = "netgen/Form1.vb:1707-1774; netgen/Form1.vb:1563-1568";
            Form1Ulaz.UlazArg u = Form1Ulaz.Ulaz(new[] { "exe", "-xml", "\"C:\\a\\Jezici.xml\"", "-s", "localhost", "-d", "spin", "-u", "Spin", "-o", "4" }, @"C:\prazno");
            if (u.Xml != @"C:\a\Jezici.xml" || u.Server != "localhost" || u.Baza != "spin" || u.AppKorisnik != "Spin" || u.Operater != "4")
                return Qa.P(g, id, opis, izvor, u.Xml);
            Form1Ulaz.UlazArg prazan = Form1Ulaz.Ulaz(new[] { "exe" }, @"C:\startup");
            if (prazan.Xml != @"C:\startup")
                return Qa.P(g, id, opis, izvor, "prazan xml");
            string ime = Form1Ulaz.ImeAplikacije(@"C:\xml\JeziciNG.xml");
            string spoj = Form1Ulaz.Spoj("localhost", "spin", "", "", ime);
            if (spoj != "Server=localhost;Database=spin;Trusted_Connection=true;Application Name=NetGenDLL-JeziciNG.xml;")
                return Qa.P(g, id, opis, izvor, spoj);
            if (Form1Ulaz.TabVidljiv(false, false) || !Form1Ulaz.TabVidljiv(false, true))
                return Qa.P(g, id, opis, izvor, "tab");
            if (Form1Ulaz.PrvoStablo(@"C:\xml\a.xml") || !Form1Ulaz.PrvoStablo(@"C:\xml"))
                return Qa.P(g, id, opis, izvor, "stablo");
            Form1Ulaz.JezikMp hrv = Form1Ulaz.JezikIz(true, "", "hrv", true, "en", true);
            Form1Ulaz.JezikMp en = Form1Ulaz.JezikIz(true, "", "", false, "en", true);
            Form1Ulaz.JezikMp ugašen = Form1Ulaz.JezikIz(false, "", "en", true, "en", true);
            if (hrv.Inicijalizira || hrv.Jezik != "hrv" || !en.Inicijalizira || en.Jezik != "en" || ugašen.Inicijalizira)
                return Qa.P(g, id, opis, izvor, "jezik");
            if (Form1Ulaz.SqlMultilingual() != " select * from Pubvars where VarIme = 'Multilingual'")
                return Qa.P(g, id, opis, izvor, "pubvars");
            if (Form1Ulaz.SqlPismo(" en ") != " select * from jezici where oznaka = 'en'")
                return Qa.P(g, id, opis, izvor, "pismo");
            Console.WriteLine("[Z] Form1 F1-31 xml=JeziciNG.xml spoj=Trusted_Connection hrv-ne-inicira");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Prostor()
        {
            const string g = "Form1";
            const string id = "F1-13";
            const string opis = "prazan URL je produkcija, šalje se samo testni CIS";
            const string izvor = "netgen/Form1.vb:849-934";
            Form1Ulaz.ProstorIshod prazno = Form1Ulaz.Prostor("4", null);
            if (!prazno.Izlaz || prazno.Poruka.IndexOf("MPFISKALIZACIJA", StringComparison.Ordinal) < 0)
                return Qa.P(g, id, opis, izvor, prazno.Poruka);
            var t = new DataTable();
            t.Columns.Add("url");
            t.Columns.Add("timeout");
            t.Columns.Add("pfx");
            t.Columns.Add("lozinkazapfx");
            t.Rows.Add("", "", "a.pfx", "tajna");
            Form1Ulaz.ProstorIshod prod = Form1Ulaz.Prostor("4", t);
            if (prod.Url != Form1Ulaz.ProdukcijskiCis || prod.Salje || prod.Timeout != "5000")
                return Qa.P(g, id, opis, izvor, prod.Url + " salje=" + prod.Salje);
            t.Rows.Clear();
            t.Rows.Add("https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest", "1000", "a.pfx", "tajna");
            Form1Ulaz.ProstorIshod test = Form1Ulaz.Prostor("4", t);
            if (!test.Salje || test.Timeout != "1000")
                return Qa.P(g, id, opis, izvor, "test se ne šalje");
            t.Rows.Clear();
            t.Rows.Add("https://fu.gov.si/x", "", "", "");
            Form1Ulaz.ProstorIshod slo = Form1Ulaz.Prostor("4", t);
            if (!slo.Slo || !slo.Izlaz)
                return Qa.P(g, id, opis, izvor, "slo");
            if (prod.Sql != "select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=4 )")
                return Qa.P(g, id, opis, izvor, prod.Sql);
            Console.WriteLine("[Z] Form1 F1-13 prostor produkcija-ne-salje test-salje slo-izlaz");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Stavke()
        {
            const string g = "Form1";
            const string id = "F1-35";
            const string opis = "total red se ne broji, prijevod preskače broj i stupac 0";
            const string izvor = "netgen/Form1.vb:2189-2255";
            string tekst = Form1Ulaz.BrojStavki(3, true, 2, false, "filter", "");
            if (tekst != "Pronađeno : 2 / 2   filter")
                return Qa.P(g, id, opis, izvor, tekst);
            if (Form1Ulaz.NaslovStavki(0, true) != "Stavke (0)")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.NaslovStavki(0, true));
            string[] gotovo = Form1Ulaz.PrevediStupce("naziv", new[] { "id", "naziv1400", "kolicina" }, new[] { "x", "  sir ", "12" }, "en");
            Carpo.NetGen.Print.Prevod.Vanjski = delegate(string s) { return "EN:" + s; };
            try
            {
                gotovo = Form1Ulaz.PrevediStupce("naziv", new[] { "id", "naziv1400", "kolicina" }, new[] { "x", "  sir ", "12" }, "en");
            }
            finally
            {
                Carpo.NetGen.Print.Prevod.Vanjski = null;
            }
            if (gotovo.Length != 3 || gotovo[0] != "x" || gotovo[1] != "EN:sir" || gotovo[2] != "12")
                return Qa.P(g, id, opis, izvor, string.Join("|", gotovo));
            if (Form1Ulaz.DmsNaslov(4) != "DMS (4)")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.DmsNaslov(4));
            string cb = Form1Ulaz.DajCb("8.3.6", "srv", "spin", "7", "a.xml", "danas", "9", new[] { new[] { "filter", "1" }, new[] { "", "" } });
            if (cb.IndexOf("NetGen DLL v8.3.6", StringComparison.Ordinal) < 0 || cb.IndexOf("0.  @@filter1", StringComparison.Ordinal) < 0)
                return Qa.P(g, id, opis, izvor, cb);
            Console.WriteLine("[Z] Form1 F1-35 pronadjeno=2/2 prijevod=EN:sir dms=4");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Format()
        {
            const string g = "Form1";
            const string id = "F1-32";
            const string opis = "širina natpisa je zadnje 4 znamenke kroz 14, ID se skriva";
            const string izvor = "netgen/Form1.vb:1777-1822";
            Form1Ulaz.StupacFormat s = Form1Ulaz.FormatStupca("string", "Naziv1400");
            Form1Ulaz.StupacFormat idStupac = Form1Ulaz.FormatStupca("int", "PartnerId");
            if (s.Sirina != 100 || s.Natpis != "Naziv" || s.Sakriven)
                return Qa.P(g, id, opis, izvor, "sirina=" + s.Sirina + " natpis=" + s.Natpis);
            if (!idStupac.Sakriven || idStupac.Sirina != 34 || idStupac.Format != "##0")
                return Qa.P(g, id, opis, izvor, "id");
            string mapa = Path.Combine(Path.GetTempPath(), "ng-form1-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(mapa);
            File.WriteAllText(Path.Combine(mapa, "a.xml"), "<PROJECT><CAPTION>Jezici</CAPTION></PROJECT>");
            try
            {
                List<string> stavke = Form1Ulaz.IzbornikMape(mapa);
                if (stavke.Count < 3 || stavke[0] != "XML-ovi" || stavke[1] != "BUILDIN 1 : XMLFile1" || stavke[2] != "Jezici")
                    return Qa.P(g, id, opis, izvor, string.Join("|", stavke.ToArray()));
            }
            finally
            {
                Directory.Delete(mapa, true);
            }
            var meni = new DataTable();
            meni.Columns.Add("id", typeof(int));
            meni.Columns.Add("parentid", typeof(object));
            meni.Columns.Add("caption", typeof(string));
            meni.Rows.Add(1, DBNull.Value, "Root");
            meni.Rows.Add(2, 1, "Dijete");
            List<string> korijen = Form1Ulaz.IzbornikBaza(meni);
            if (korijen.Count != 1 || korijen[0] != "Root" || Form1Ulaz.SqlIzbornika(null) != "select * from netgenmenu")
                return Qa.P(g, id, opis, izvor, "meni");
            if (!Form1Ulaz.MapaRasporeda().EndsWith("\\SPIN Informatica\\Jupiter Software\\Grid Settings\\", StringComparison.Ordinal))
                return Qa.P(g, id, opis, izvor, Form1Ulaz.MapaRasporeda());
            Console.WriteLine("[Z] Form1 F1-32 sirina=100 id-sakriven izbornik=Jezici");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka StabloMeni()
        {
            const string g = "Form1";
            const string id = "F1-51";
            const string opis = "menu.xml slaže tag put\\xml, id bez stupca ide na stupac 2";
            const string izvor = "netgen/Form1.vb:4217-4282; netgen/Form1.vb:5566-5599";
            if (Form1Ulaz.PutMeniXml(@"C:\xml") != @"C:\xml\menu.xml")
                return Qa.P(g, id, opis, izvor, "disk");
            if (Form1Ulaz.PutMeniXml("http://host/xml") != "http://host/xml/menu.xml")
                return Qa.P(g, id, opis, izvor, "http");
            Form1Ulaz.CvorMeni c = Form1Ulaz.MeniIzXml("<menu caption=\"Aplikacije\"><item caption=\"Jezici\" xml=\"JeziciNG.xml\"/></menu>", @"C:\xml");
            if (c == null || c.Djeca.Count != 1 || c.Djeca[0].Tag != @"C:\xml\JeziciNG.xml" || c.Djeca[0].Natpis != "Jezici")
                return Qa.P(g, id, opis, izvor, "cvor");
            if (Form1Ulaz.MeniIzXml("<menu caption=\"A\"><item xml=\"x.xml\"/></menu>", @"C:\xml") != null)
                return Qa.P(g, id, opis, izvor, "bez caption je prošao");
            if (Form1Ulaz.NadjiPoTekstu("  Jezici ", new[] { "Ostalo", "JEZICI" }) != 1)
                return Qa.P(g, id, opis, izvor, "trazi");
            string cvor;
            string forma;
            Form1Ulaz.RastaviTag("0:INSERT", out cvor, out forma);
            if (cvor != "0" || forma != "INSERT")
                return Qa.P(g, id, opis, izvor, cvor + "/" + forma);
            if (Form1Ulaz.Id(false, false, "5", "9") != "0")
                return Qa.P(g, id, opis, izvor, "prazan red");
            if (Form1Ulaz.Id(true, true, "", "9") != "9")
                return Qa.P(g, id, opis, izvor, "stupac2");
            if (Form1Ulaz.Id(true, false, "", "9") != "")
                return Qa.P(g, id, opis, izvor, "prazan id");
            if (Form1Ulaz.DxId(true, true, null, "") != "0")
                return Qa.P(g, id, opis, izvor, "dx");
            if (Form1Ulaz.DatotekaRasporeda(@"C:\a\JeziciNG.xml", "", false, "", "hrv", false) != "GridMasterJeziciNG-hrv.gxl")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.DatotekaRasporeda(@"C:\a\JeziciNG.xml", "", false, "", "hrv", false));
            if (Form1Ulaz.DatotekaRasporeda(@"C:\a\JeziciNG.xml", "Posebno", true, "FILTER", "en", true) != "GridMasterPosebnoFILTER-en.xml")
                return Qa.P(g, id, opis, izvor, "dx layout");
            Console.WriteLine("[Z] Form1 F1-51 tag=C:\\xml\\JeziciNG.xml id-stupac2=9 raspored=GridMasterJeziciNG-hrv.gxl");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Rafal()
        {
            const string g = "Form1";
            const string id = "F1-107";
            const string opis = ":select male zamijeni, :SELECT ostaje, Val staje na zarezu";
            const string izvor = "netgen/Form1.vb:6559-6562; netgen/Form1.vb:6491-6495; netgen/Form1.vb:7348-7362";
            if (Form1Ulaz.SegmentRafala(":HIDECOLUMNS(mastergrid,a)") != "HIDECOLUMNS(mastergrid,a)")
                return Qa.P(g, id, opis, izvor, "hide");
            if (Form1Ulaz.SegmentRafala(":select 1") != "select 1")
                return Qa.P(g, id, opis, izvor, "select");
            if (Form1Ulaz.SegmentRafala(":SELECT 1") != ":SELECT 1")
                return Qa.P(g, id, opis, izvor, "SELECT");
            if (Form1Ulaz.VrijednostCelije("12,5") != "12.5")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.VrijednostCelije("12,5"));
            if (Form1Ulaz.VrijednostCelije("0,5") != "0,5")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.VrijednostCelije("0,5"));
            int interval;
            string natpis;
            if (!Form1Ulaz.TimerUpaljen("5000", out interval, out natpis) || interval != 5000 || natpis != "Timer ON 5")
                return Qa.P(g, id, opis, izvor, natpis);
            if (Form1Ulaz.TimerUpaljen("", out interval, out natpis) || natpis != "Timer OFF")
                return Qa.P(g, id, opis, izvor, "prazan timer");
            if (!Form1Ulaz.SljedeciRastvoreno(false) || Form1Ulaz.SljedeciRastvoreno(true))
                return Qa.P(g, id, opis, izvor, "grupe");
            Console.WriteLine("[Z] Form1 F1-107 :select ostaje veliko, 12,5=12.5, timer=5");
            return Qa.Z(g, id, opis, izvor);
        }

        private static Stavka Pomoc()
        {
            const string g = "Form1";
            const string id = "F1-158";
            const string opis = "pomoć ide u JupiterHelp, Str broja ima vodeći razmak";
            const string izvor = "netgen/Form1.vb:7781-8023; netgen/Form1.vb:8446-8468";
            string arg = Form1Ulaz.ArgumentiPomoci("localhost", "spin", "MER005", 7, @"C:\xml\JeziciNG.xml");
            if (arg != "-s localhost -d spin -h MER005 -o  7 -p JeziciNG.xml")
                return Qa.P(g, id, opis, izvor, arg);
            string jezik;
            string multi;
            Form1Ulaz.JezikIzbornika("EN", out jezik, out multi);
            if (jezik != "EN" || multi != "2")
                return Qa.P(g, id, opis, izvor, jezik);
            Form1Ulaz.JezikIzbornika("mk", out jezik, out multi);
            if (jezik != "MK" || multi != "2")
                return Qa.P(g, id, opis, izvor, "mk");
            Form1Ulaz.JezikIzbornika("original", out jezik, out multi);
            if (jezik != "" || multi != "0")
                return Qa.P(g, id, opis, izvor, "original");
            if (Form1Ulaz.SqlDmsOdustani(" 12 ") != "exec dms.spSetDocumentUndoCheckOutOperater @dokId='12'")
                return Qa.P(g, id, opis, izvor, Form1Ulaz.SqlDmsOdustani(" 12 "));
            bool preuzmi;
            bool vrati;
            bool odustani;
            Form1Ulaz.DmsIzbornik(true, out preuzmi, out vrati, out odustani);
            if (!preuzmi || vrati || odustani)
                return Qa.P(g, id, opis, izvor, "prazan checkout");
            Form1Ulaz.DmsIzbornik(false, out preuzmi, out vrati, out odustani);
            if (preuzmi || !vrati || !odustani)
                return Qa.P(g, id, opis, izvor, "checkout");
            if (Form1Ulaz.SmijeSpustiti("a", new[] { "b", "a" }) || !Form1Ulaz.SmijeSpustiti("a", new[] { "b", "c" }))
                return Qa.P(g, id, opis, izvor, "drop");
            Console.WriteLine("[Z] Form1 F1-158 pomoc=-o  7 jezik=EN dms-odustani checkout-prazan-preuzmi");
            return Qa.Z(g, id, opis, izvor);
        }
    }
}
