using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Ulaz;

namespace Carpo.NetGen.Granule
{
    internal static class Form1SrceTest
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Punjenje(root));
            l.Add(FormatIRaspored());
            l.Add(PrevodIPogled());
            return l;
        }

        private static Stavka Punjenje(string root)
        {
            const string g = "JeziciNG";
            const string id = "FILL";
            const string opis = "MasterGrid pošalje spJeziciQuery i grid ima redove. PokaziUpit time, id i grupe još nisu tu";
            const string izvor = "netgen/Form1.vb:1855-1859";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "JeziciNG.xml");
            string greska;
            Carpo.NetGen.Model.NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(g, id, opis, izvor, greska ?? "xml");
            var grid = new MasterGrid();
            grid.Kontekst.OperaterId = "7";
            grid.Ucitaj(p, null);
            if (grid.Stanje.Odluka != "salji")
                return Qa.P(g, id, opis, izvor, "odluka=" + grid.Stanje.Odluka + " sql=" + grid.Stanje.SqlZaSlanje);
            grid.IzvrsiCitanje(Citanje.Spoj);
            if (!string.IsNullOrEmpty(grid.Stanje.SqlGreska))
                return Qa.P(g, id, opis, izvor, grid.Stanje.SqlGreska);
            int redova = grid.Stanje.Tablica == null ? 0 : grid.Stanje.Tablica.Rows.Count;
            if (redova < 1)
                return Qa.P(g, id, opis, izvor, "redovi=0 sql=" + grid.Stanje.SqlZaSlanje);
            if ((grid.Stanje.SqlZaSlanje ?? "").IndexOf("spJeziciQuery", StringComparison.Ordinal) < 0)
                return Qa.P(g, id, opis, izvor, grid.Stanje.SqlZaSlanje);
            Console.WriteLine("[Z] JeziciNG FILL redovi=" + redova.ToString() + " sql=spJeziciQuery");
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
