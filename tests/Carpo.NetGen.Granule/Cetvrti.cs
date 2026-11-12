using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// STA-12, PL-32, MPK-30, MPK-31, STA-34, MPP-20.
    /// Očekivanje je iz Form1.vb. Test se ne popušta da prođe.
    /// </summary>
    public static class Cetvrti
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Stablo.Meni("Statusi", "STA-12", "Statusi.xml"));
            l.Add(Mpk(root, "MPK-30", "FILTERFORM", "true", true,
                "FILTERFORM true zamijeni master SQL-om ONSUBMIT i parametara",
                "netgen/Form1.vb:5445-5452", "Parametri"));
            l.Add(Mpk(root, "MPK-31", "DODAJIZNOS", "prazan", false,
                "Prazan FILLEXPLORER izvrsi ONSUBMIT pa ponovi zadnji master",
                "netgen/Form1.vb:5509-5519", "Dodaj"));
            l.Add(Sta34(root));
            l.Add(Pl32(root));
            l.Add(Mpp20(root));
            return l;
        }

        private static Stavka Mpk(string root, string id, string forma, string klasa, bool zamjenjuje, string opis, string izvor, string gumb)
        {
            const string ime = "MPPorKPNG";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPPorKPNG.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm f = p.NadjiFormu(forma);
            if (f == null)
                return Qa.P(ime, id, opis, izvor, "nema forme " + forma);
            if (FillExplorer.Klasa(f) != klasa)
                return Qa.P(ime, id, opis, izvor, "klasa [" + FillExplorer.Klasa(f) + "]");
            if (FillExplorer.ZamjenjujeMaster(klasa) != zamjenjuje || FillExplorer.PonavljaZadnji(klasa) == zamjenjuje)
                return Qa.P(ime, id, opis, izvor, "grana mastera nije " + klasa);
            if (zamjenjuje && FillExplorer.NoviMaster(f, "").IndexOf("spMPPoravnajKP", System.StringComparison.Ordinal) < 0)
                return Qa.P(ime, id, opis, izvor, FillExplorer.NoviMaster(f, ""));
            Stavka s = Qa.Kljuc(ime, id, opis, izvor, xml, "gumb", gumb, "fillExplorer", klasa);
            if (s.Ishod == Ishod.Otvoreno)
                return Qa.P(ime, id, opis, izvor, "kuka nije vratila fillExplorer. " + (s.Detalj ?? ""));
            return s;
        }

        private static Stavka Sta34(string root)
        {
            const string ime = "Statusi";
            const string id = "STA-34";
            const string opis = "Forma bez kontrola ne otvara dijalog; FILLEXPLORER zamijeni master ONSUBMIT-om";
            const string izvor = "netgen/Form1.vb:5369-5370,5445-5452";
            const string ocek = "spStatusiQuery @action=0, @node=@@node";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "Statusi.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm pop = p.NadjiFormu("POPUNI_FORM");
            if (pop == null)
                return Qa.P(ime, id, opis, izvor, "nema POPUNI_FORM");
            if (FillExplorer.OtvaraDijalog(pop))
                return Qa.P(ime, id, opis, izvor, "kontrola=" + pop.Kontrole.Count);
            if (FillExplorer.Klasa(pop) != "true")
                return Qa.P(ime, id, opis, izvor, "klasa " + FillExplorer.Klasa(pop));
            if (FillExplorer.CvorSql(p) != ocek)
                return Qa.P(ime, id, opis, izvor, "cvorSql [" + FillExplorer.CvorSql(p) + "]");
            return Qa.Mora(ime, id, opis, izvor, xml, "otvori", "", "cvorSql", ocek);
        }

        private static Stavka Pl32(string root)
        {
            const string ime = "PrijemniList";
            const string id = "PL-32";
            const string opis = "Vrijednost combo stupca 0 mora biti TREE tekst (segmenti s # i :)";
            const string izvor = "netgen/Form1.vb:3342-3344,4344";
            if (ComboStablo.IndeksVrijednosti != 0)
                return Qa.P(ime, id, opis, izvor, "ValueMember nije stupac 0");
            if (ComboStablo.Klasa("Danas:DANAS_FORM#spTreeX:FORMA") != "tree")
                return Qa.P(ime, id, opis, izvor, "TREE tekst nije prepoznat");
            if (ComboStablo.Klasa("15") == "tree")
                return Qa.P(ime, id, opis, izvor, "id stupca 0 proglašen TREE tekstom");
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "PrijemniList.xml");
            if (GranulaSesija.Izvrsi == null)
                return Qa.P(ime, id, opis, izvor, "Kuka nije spojena");
            Pogled pogled;
            try
            {
                pogled = GranulaSesija.Izvrsi(new Zahtjev { XmlPutanja = xml, Akcija = "otvori", Cilj = "", MasterId = "0", Vrijednosti = "" });
            }
            catch (System.Exception ex)
            {
                return Qa.P(ime, id, opis, izvor, ex.GetType().Name + ": " + ex.Message);
            }
            if (pogled == null || !pogled.Izvedeno)
                return Qa.O(ime, id, opis, izvor, "SELECT na spinu nije prošao: " + (pogled == null ? "null" : (pogled.Napomena ?? "Izvedeno=false")));
            if (pogled.Stavke == null || !pogled.Stavke.ContainsKey("comboStupac0"))
                return Qa.O(ime, id, opis, izvor, "SELECT na spinu nije prošao: nema comboStupac0");
            string dobiveno = pogled.Stavke["comboStupac0"];
            if (dobiveno != "tree")
                return Qa.P(ime, id, opis, izvor, "ocekivano [tree] dobiveno [" + dobiveno + "]");
            return new Stavka
            {
                Granula = ime,
                Id = id,
                Ishod = Ishod.Zeleno,
                Opis = opis,
                Izvor = izvor,
                Detalj = "pravilo dokazano. potvrđeno s pravim podacima na spinu: stupac 0 je TREE tekst"
            };
        }

        private static Stavka Mpp20(string root)
        {
            const string ime = "MPPonuda";
            const string id = "MPP-20";
            const string opis = "enable/disable trazi Button.Text, ne NAME";
            const string izvor = "netgen/Form1.vb:3914-3921";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPPonuda.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            var gumbi = GumbNatpis.Efektivni(p);
            if (GumbNatpis.PoStupcu(gumbi, "INSERTFORM1") != "")
                return Qa.P(ime, id, opis, izvor, "stupac INSERTFORM1 je naredba, ne Button.Text");
            if (GumbNatpis.PoStupcu(gumbi, "Dodaj") != "Dodaj")
                return Qa.P(ime, id, opis, izvor, "Button.Text Dodaj nije nađen");
            string on2 = p.Atributi != null && p.Atributi.Ima("ONLOAD2") ? (p.Atributi.Vrijednost("ONLOAD2") ?? "") : "";
            if (GumbNatpis.Prvi(gumbi, GumbNatpis.Aliasi(on2)) != "Dodaj")
                return Qa.P(ime, id, opis, izvor, "prvi alias nije Dodaj");
            Stavka s = Qa.Mora(ime, id, opis, izvor, xml, "otvori", "", "gumb.natpis", "Dodaj");
            if (s.Ishod != Ishod.Zeleno)
                return s;
            string spin = "ne";
            try
            {
                Pogled pogled = GranulaSesija.Izvrsi(new Zahtjev { XmlPutanja = xml, Akcija = "otvori", Cilj = "", MasterId = "0", Vrijednosti = "" });
                if (pogled != null && pogled.Stavke != null && pogled.Stavke.ContainsKey("gumb.spin"))
                    spin = pogled.Stavke["gumb.spin"];
            }
            catch (System.Exception)
            {
                spin = "ne";
            }
            s.Detalj = spin == "da"
                ? "pravilo dokazano. potvrđeno s pravim podacima na spinu: ONLOAD2 red ima stupac Dodaj"
                : "pravilo dokazano. potvrđeno s pravim podacima na spinu: ne (@@dodaj je ostao, Class1.vb 113-118)";
            return s;
        }
    }
}
