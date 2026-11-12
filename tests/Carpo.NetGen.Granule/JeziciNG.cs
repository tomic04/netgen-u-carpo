using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// JeziciNG: podloga s gumbima, MASTERCOLUMNS i tri dijaloga.
    /// Ocekivanje je iz Form1/Form2/Class1, ne iz Carpo.NetGen grida.
    /// </summary>
    public static class JeziciNG
    {
        public const string Ime = "JeziciNG";
        public const string OnLoad = "spJeziciQuery @action=0, @id=''";
        public const string SetDefaults =
            "select naziv as tkosmomi from partneri where PartneriId=(select varset from pubvars where VarIme='nasid')";
        public const string UnesiClick =
            "spJeziciAzur @action=0, @id=@@id, @Naziv='@@Naziv'#OKStay#select '' as naziv, 'naziv' as focus";
        public const string IzmijeniClick =
            "spJeziciAzur @action=1, @ID=@@ID, @Naziv='@@Naziv'#OK";
        public const string BrisiClick =
            "spJeziciAzur @action=2, @ID=@@ID, @Naziv=''#OK";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "JeziciNG.xml");
            string kopija = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "JeziciNG.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "JEZ-01", "XML postoji", xml, "nema datoteke"));
                return l;
            }
            if (File.Exists(kopija))
            {
                l.Add(IstiHash(xml, kopija)
                    ? Qa.Z(Ime, "JEZ-00", "BIOVEGA kopija je isti sadrzaj", "definitions/netgen/Agronom granule/JeziciNG.xml")
                    : Qa.P(Ime, "JEZ-00", "BIOVEGA kopija je isti sadrzaj", "definitions/netgen/BIOVEGA/JeziciNG.xml", "SHA-256 se razlikuje"));
            }

            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "JEZ-01", "Projekt se ucita", "netgen/Form1.vb:3263", greska));
                return l;
            }
            l.Add(Qa.Z(Ime, "JEZ-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(Qa.Jednako(Ime, "JEZ-02", "Naslov je Jezici, bez verzije", "netgen/Form1.vb:3522-3528", "Jezici", p.Naslov));
            l.Add(Qa.Jednako(Ime, "JEZ-03", "ONLOAD je spJeziciQuery, nema STARTUPSTORA", "netgen/Form1.vb:3367-3368", OnLoad, p.OnLoad));
            l.Add(Qa.Jednako(Ime, "JEZ-04", "MASTERDBLCLICK je Izmijeni", "netgen/Form1.vb:3389", "Izmijeni", p.MasterDblClick));
            l.Add(string.IsNullOrEmpty(p.Tree) && string.IsNullOrEmpty(p.DetailGrid)
                ? Qa.Z(Ime, "JEZ-05", "Nema TREE ni DETAILGRID", "netgen/Form1.vb:3373,3387")
                : Qa.P(Ime, "JEZ-05", "Nema TREE ni DETAILGRID", "netgen/Form1.vb:3373,3387", (p.Tree ?? "") + "/" + (p.DetailGrid ?? "")));

            l.Add(Gumbi(p));
            l.Add(Qa.Jednako(Ime, "JEZ-07", "SETDEFAULTS, PAGEHEADERRIGHT, HELPKOD i ICON su sirovi",
                "netgen/Form1.vb:3371,3452,3489,3392",
                SetDefaults + "|@@tkosmomi|UTPT007|Jezici.ico",
                Attr(p, "SETDEFAULTS") + "|" + Attr(p, "PAGEHEADERRIGHT") + "|" + Attr(p, "HELPKOD") + "|" + Attr(p, "ICON")));
            l.Add(Qa.Jednako(Ime, "JEZ-08", "Checkbox, total, group i frozen ostaju kako su napisani",
                "netgen/Form1.vb:3350,3362,3459-3462",
                "FALSE|FALSE|FALSE|FALSE|FALSE|0",
                Attr(p, "CHECKBOXVISIBLE") + "|" + Attr(p, "TOTALROWVISIBLE") + "|" + Attr(p, "DETCHECKBOXVISIBLE") + "|" + Attr(p, "DETTOTALROWVISIBLE") + "|" + Attr(p, "GROUPBYBOXVISIBLE") + "|" + Attr(p, "FROZENCOLUMNS")));

            var grupa = p.NadjiGrupu("MASTERCOLUMNS");
            if (grupa == null || grupa.Kolone.Count != 1)
            {
                l.Add(Qa.P(Ime, "JEZ-09", "MASTERCOLUMNS ima jednu kolonu Naziv", "netgen/Form1.vb:3294-3296", "nema grupe"));
            }
            else
            {
                var k = grupa.Kolone[0];
                l.Add(grupa.ClearColums == "false" && grupa.HeaderAlignment == "1" && k.Name == "Naziv"
                    && k.ImaDvostrukoG && k.AgregateFunction == null
                    && k.Atributi.Vrijednost("WIDTH") == "2500"
                    && k.Atributi.Vrijednost("AGGREGATEFUNCTION") == ""
                    && !k.Atributi.Ima("AGREGATEFUNCTION")
                    ? Qa.Z(Ime, "JEZ-09", "CLEARCOLUMS=false, HEADERALIGNMENT=1, AGGREGATEFUNCTION s dva G se ne veze", "netgen/Form1.vb:3295-3296,7496; netgen/Grid.vb")
                    : Qa.P(Ime, "JEZ-09", "CLEARCOLUMS i agregat", "netgen/Form1.vb:3295",
                        "clear=" + grupa.ClearColums + " align=" + grupa.HeaderAlignment + " dvaG=" + k.ImaDvostrukoG + " agg=" + (k.AgregateFunction ?? "(null)")));
            }

            Forme(l, p, xml);
            CitanjeBaze(l);
            Runtime(l, xml);
            return l;
        }

        private static Stavka Gumbi(NgProject p)
        {
            if (p.Gumbi.Count != 4)
                return Qa.P(Ime, "JEZ-06", "Cetiri gumba podloge, CMD4 ne postoji", "netgen/Form1.vb:3314-3335", "gumbi=" + p.Gumbi.Count);
            bool ok = p.Gumbi[0].Redni == 1 && p.Gumbi[0].Natpis == "Unesi" && p.Gumbi[0].Naredba == "Unesi#refreshmaster"
                && p.Gumbi[1].Redni == 2 && p.Gumbi[1].Natpis == "Izmijeni" && p.Gumbi[1].Naredba == "Izmijeni"
                && p.Gumbi[2].Redni == 3 && p.Gumbi[2].Natpis == "Obriši" && p.Gumbi[2].Naredba == "Brisanje"
                && p.Gumbi[3].Redni == 5 && p.Gumbi[3].Natpis == "Ispiši" && p.Gumbi[3].Naredba == "PRINTREPXMASTER";
            return ok
                ? Qa.Z(Ime, "JEZ-06", "CMD1-3 i CMD5; hash u Unesi#refreshmaster se ne lomi u parseru", "netgen/Form1.vb:3314-3335,4560")
                : Qa.P(Ime, "JEZ-06", "CMD1-3 i CMD5", "netgen/Form1.vb:3314-3335",
                    p.Gumbi[0].Naredba + " | " + p.Gumbi[3].Natpis + "/" + p.Gumbi[3].Naredba);
        }

        private static void Forme(List<Stavka> l, NgProject p, string xml)
        {
            if (p.Forme.Count != 3)
            {
                l.Add(Qa.P(Ime, "JEZ-10", "Tri forme: Unesi, Izmijeni, Brisanje", xml, "forme=" + p.Forme.Count));
                return;
            }
            l.Add(p.Forme[0].Ime == "Unesi" && p.Forme[1].Ime == "Izmijeni" && p.Forme[2].Ime == "Brisanje"
                ? Qa.Z(Ime, "JEZ-10", "Tri forme tim redom", xml)
                : Qa.P(Ime, "JEZ-10", "Tri forme tim redom", xml, p.Forme[0].Ime));

            NgForm unesi = p.NadjiFormu("Unesi");
            NgForm izm = p.NadjiFormu("Izmijeni");
            NgForm br = p.NadjiFormu("Brisanje");
            l.Add(unesi.OnSubmit == "" && unesi.Atributi.Vrijednost("NEXTFORM") == "Dodaj"
                ? Qa.Z(Ime, "JEZ-11", "Unesi nema ONSUBMIT, NEXTFORM je Dodaj", "netgen/Form2.vb:1039,1052")
                : Qa.P(Ime, "JEZ-11", "Unesi nema ONSUBMIT, NEXTFORM je Dodaj", "netgen/Form2.vb:1052", unesi.OnSubmit));
            l.Add(Qa.Jednako(Ime, "JEZ-12", "Spremi na Unesi je azur, OKStay, pa select za fokus", "netgen/Form2.vb:2618,6884-6890,7510",
                UnesiClick, Klik(unesi, "buttonOK")));
            l.Add(Qa.Jednako(Ime, "JEZ-13", "Odustani na Unesi je naredba ok, ne close", "netgen/Form2.vb:6888",
                "ok", Klik(unesi, "buttonCLOSE")));
            l.Add(Qa.Jednako(Ime, "JEZ-14", "Izmijeni ONLOAD je select * po @@id", "netgen/Form2.vb:1042",
                "select * from jezici where jeziciid=@@id", izm.OnLoad));
            l.Add(Qa.Jednako(Ime, "JEZ-15", "Spremi na Izmijeni je azur pa OK", "netgen/Form2.vb:6888",
                IzmijeniClick, Klik(izm, "buttonOK")));
            l.Add(Qa.Jednako(Ime, "JEZ-16", "Odustani na Izmijeni je CLOSE", "netgen/Form2.vb:7033-7034",
                "CLOSE", Klik(izm, "buttonCLOSE")));
            l.Add(Qa.Jednako(Ime, "JEZ-17", "Obriši šalje action 2 pa OK", "netgen/Form2.vb:7510,6888",
                BrisiClick, Klik(br, "Obriši")));
            l.Add(Qa.Jednako(Ime, "JEZ-18", "Odustani na brisanju je close", "netgen/Form2.vb:7033",
                "close", Klik(br, "odustani")));
            l.Add(Qa.Jednako(Ime, "JEZ-19", "Potvrda brisanja je tocan natpis", "netgen/Form2.vb:2932",
                "Jeste li sigurni da želite obrisati označeni jezik?",
                Natpis(br, "tekst")));
            l.Add(unesi.Atributi.Vrijednost("BORDERSTYLE") == "3" && unesi.GetType().GetProperty("BorderStyle") == null
                ? Qa.Z(Ime, "JEZ-20", "BORDERSTYLE=3 se cuva, kod ga ne cita (nema GetNamedItem)", "netgen/Form2.vb:1030-1130")
                : Qa.P(Ime, "JEZ-20", "BORDERSTYLE se ne mapira", "netgen/Form2.vb:1030", unesi.Atributi.Vrijednost("BORDERSTYLE")));
            l.Add(unesi.Atributi.Vrijednost("REFRESHAFTERCHILD") == "TRUE"
                ? Qa.Z(Ime, "JEZ-21", "REFRESHAFTERCHILD na formi je sirov; SrediKontrolu ga cita samo na INPUT-u", "netgen/Form2.vb:3058")
                : Qa.P(Ime, "JEZ-21", "REFRESHAFTERCHILD", "netgen/Form2.vb:3058", ""));
            NgControl zv = Nadji(unesi, "LabelZvjezdica");
            NgControl spremi = Nadji(unesi, "buttonOK");
            NgControl frame = Nadji(unesi, "FrmJezici");
            l.Add(zv != null && zv.Atributi.Vrijednost("SIZE") == "13" && zv.Atributi.Vrijednost("CAPTION") == "*"
                ? Qa.Z(Ime, "JEZ-22", "Zvjezdica ima SIZE 13; kod cita SIZE, ne BOLD", "netgen/Form2.vb:3077-3081")
                : Qa.P(Ime, "JEZ-22", "SIZE 13", "netgen/Form2.vb:3078", zv == null ? "" : zv.Atributi.Vrijednost("SIZE")));
            l.Add(spremi != null && spremi.Atributi.Vrijednost("CAPTION") == "Spremi" && spremi.Atributi.Vrijednost("TEXT") == "ButtonNG0"
                ? Qa.Z(Ime, "JEZ-23", "Natpis gumba je CAPTION, TEXT=ButtonNG0 se ne cita", "netgen/Form2.vb:2932")
                : Qa.P(Ime, "JEZ-23", "CAPTION gumba", "netgen/Form2.vb:2932", ""));
            l.Add(frame != null && frame.Grana == NgGrana.Group && frame.Atributi.Vrijednost("TEXT") == "FrmJezici" && !frame.Atributi.Ima("CAPTION")
                ? Qa.Z(Ime, "JEZ-24", "frame ide u grupu; TEXT se cuva, natpis iz CAPTION ne postoji", "netgen/Form2.vb:1354,2932")
                : Qa.P(Ime, "JEZ-24", "frame", "netgen/Form2.vb:1354", frame == null ? "" : frame.Grana.ToString()));
        }

        private static void CitanjeBaze(List<Stavka> l)
        {
            var s = Citanje.Select(SetDefaults, 5);
            if (!s.Uspjeh)
                l.Add(Qa.O(Ime, "JEZ-30", "SETDEFAULTS je select i vraca stupac tkosmomi", "netgen/Form1.vb:3945-4002", s.Greska));
            else
                l.Add(s.Imena.Count == 1 && string.Equals(s.Imena[0], "tkosmomi", StringComparison.OrdinalIgnoreCase)
                    ? Qa.Z(Ime, "JEZ-30", "SETDEFAULTS vraca tkosmomi (" + s.Redova + " red)", "netgen/Form1.vb:3976-3998")
                    : Qa.P(Ime, "JEZ-30", "SETDEFAULTS vraca tkosmomi", "netgen/Form1.vb:3976", string.Join(",", s.Imena.ToArray())));

            string defGreska;
            string def = Citanje.PrviTekst("select object_definition(object_id('dbo.spJeziciQuery'))", out defGreska);
            if (def == null)
            {
                l.Add(Qa.O(Ime, "JEZ-31", "spJeziciQuery se smije citati samo ako tijelo nema upis", "netgen/Class1.vb:85", defGreska));
                return;
            }
            if (!CistoTijelo(def))
            {
                l.Add(Qa.O(Ime, "JEZ-31", "spJeziciQuery se ne izvrsava jer tijelo nije dokazano samo citanje", "netgen/Class1.vb:85", "tijelo sadrzi upis ili poziv"));
                return;
            }
            var q = Citanje.Select(OnLoad, 50);
            if (!q.Uspjeh)
                l.Add(Qa.O(Ime, "JEZ-31", "spJeziciQuery @action=0 vraca retke", "netgen/Form1.vb:3882", q.Greska));
            else
                l.Add(q.Redova > 0
                    ? Qa.Z(Ime, "JEZ-31", "spJeziciQuery @action=0 je citanje, stupci: " + string.Join(",", q.Imena.ToArray()) + ", redova u uzorku " + q.Redova, "netgen/Form1.vb:3882")
                    : Qa.O(Ime, "JEZ-31", "spJeziciQuery nije vratio red", "netgen/Form1.vb:3882", "0 redova"));
        }

        private static bool CistoTijelo(string def)
        {
            if (string.IsNullOrWhiteSpace(def))
                return false;
            string l = " " + def.ToLowerInvariant() + " ";
            string[] z = { " insert ", " update ", " delete ", " merge ", " drop ", " alter ", " truncate ", " exec ", " execute ", " into " };
            for (int i = 0; i < z.Length; i++)
            {
                if (l.Contains(z[i]))
                    return false;
            }
            return true;
        }

        private static void Runtime(List<Stavka> l, string xml)
        {
            R(l, xml, "JEZ-40", "Prije mastera ide SETDEFAULTS, pa spJeziciQuery", "netgen/Form1.vb:3862-3882", "otvori", "", "sqlTrag", SetDefaults + " || " + OnLoad);
            R(l, xml, "JEZ-41", "Prazan TREE sakrije stablo i splitter je 0", "netgen/Form1.vb:4310-4316", "otvori", "", "stabloVidljivo", "false");
            R(l, xml, "JEZ-42", "Nema detail SQL-a", "netgen/Form1.vb:2111-2113", "otvori", "", "detailSql", "");
            R(l, xml, "JEZ-43", "Help je vidljiv, kod UTPT007", "netgen/Form1.vb:3491-3494,7781-7799", "otvori", "", "helpKod", "UTPT007");
            R(l, xml, "JEZ-44", "Ikona je Jezici.ico iz Graphics\\Icons", "netgen/Form1.vb:3440", "otvori", "", "ikona", "Jezici.ico");
            R(l, xml, "JEZ-45", "Selector je skriven", "netgen/Form1.vb:2043-2046", "otvori", "", "selectorVidljiv", "false");
            R(l, xml, "JEZ-46", "Total red je ugašen", "netgen/Form1.vb:2049-2050", "otvori", "", "totalRow", "false");
            R(l, xml, "JEZ-47", "Group box je skriven, frozen je 0", "netgen/Form1.vb:3350,3362", "otvori", "", "groupByBox", "false");
            R(l, xml, "JEZ-48", "Naziv: širina 2500/14 = 179 (VB dijeljenje, ne cjelobrojno)", "netgen/Form1.vb:7522", "otvori", "", "kolona.Naziv.sirina", "179");
            R(l, xml, "JEZ-49", "Zaglavlje Naziv ima HeaderAlignment 1", "netgen/Form1.vb:7530-7531", "otvori", "", "kolona.Naziv.headerAlignment", "1");
            R(l, xml, "JEZ-50", "Dvo-G agregat ne zbraja", "netgen/Form1.vb:7496", "otvori", "", "kolona.Naziv.agregat", "");
            R(l, xml, "JEZ-51", "CLEARCOLUMS=false ne sakrije ostale stupce iz rezultata", "netgen/Form1.vb:7459-7462", "otvori", "", "clearSakrivaSve", "false");
            R(l, xml, "JEZ-52", "String stupac koji nije u COLUMN ima širinu 100, ne 200", "netgen/Class1.vb:1201-1204", "otvori", "", "stringSirina", "100");
            R(l, xml, "JEZ-53", "Nema edit stupaca", "netgen/Class1.vb:1237-1239", "otvori", "", "editType", "NoEdit");
            R(l, xml, "JEZ-54", "HeaderLines je 3, selekcija višestruka, Enter None", "netgen/Class1.vb:1364; netgen/Form1.designer.vb:654; netgen/Form1.vb:3419", "otvori", "", "headerLines", "3");
            R(l, xml, "JEZ-55", "Gumb 4 ne postoji, Ispiši je redni 5", "netgen/Form1.vb:3827-3848", "otvori", "", "gumb.5", "Ispiši|PRINTREPXMASTER|true");
            R(l, xml, "JEZ-56", "Ispiši zove ispis mastera, ne formu tog imena", "netgen/Form1.vb:4945-4948,5984", "gumb", "Ispiši", "ispis", "PRINTREPXMASTER");
            R(l, xml, "JEZ-57", "Desno zaglavlje ispisa dolazi iz @@tkosmomi (SETDEFAULTS u CB)", "netgen/Form1.vb:6001; netgen/Class1.vb:932-966", "gumb", "Ispiši", "pageHeaderRight", "@@tkosmomi");
            R(l, xml, "JEZ-58", "Dvostruki klik otvara Izmijeni", "netgen/Form1.vb:6935", "dblclick", "", "forma", "Izmijeni");
            R(l, xml, "JEZ-59", "Unesi: okvir nema natpis FrmJezici", "netgen/Form2.vb:2932-2938", "gumb", "Unesi", "kontrola.FrmJezici.tekst", "");
            R(l, xml, "JEZ-60", "Zvjezdica je font veličine 13", "netgen/Form2.vb:3077-3081", "gumb", "Unesi", "kontrola.LabelZvjezdica.velicina", "13");
            R(l, xml, "JEZ-61", "Spremi na Unesi ostaje na formi (OKStay), Naziv se isprazni", "netgen/Form2.vb:6884-6887,4627-4633,4970-4979", "kontrola", "Unesi.buttonOK", "ostajeOtvorena", "true");
            R(l, xml, "JEZ-62", "Nakon OKStay fokus je kontrola naziv", "netgen/Form2.vb:4627-4632", "kontrola", "Unesi.buttonOK", "fokus", "naziv");
            R(l, xml, "JEZ-63", "Odustani na Unesi je naredba ok: OKClick true, pa NEXTFORM Dodaj", "netgen/Form2.vb:6888; netgen/Form1.vb:5381", "kontrola", "Unesi.buttonCLOSE", "okClick", "true");
            R(l, xml, "JEZ-64", "Forma Dodaj ne postoji: nema poruke, 0 kontrola i dalje OKClick", "netgen/Form1.vb:5354-5370", "kontrola", "Unesi.buttonCLOSE", "dodajPoruka", "");
            R(l, xml, "JEZ-65", "Segment refreshmaster nije REFRESHMASTERGRID, ali prazna forma ipak osvježi zadnji SQL", "netgen/Form1.vb:4570,4926-5370,5519", "gumb", "Unesi", "refreshMaster", "true");
            R(l, xml, "JEZ-66", "Izmijeni ONLOAD napuni Naziv iz stupca", "netgen/Form2.vb:2016", "gumb", "Izmijeni", "kontrola.Naziv.izOnLoada", "true");
            R(l, xml, "JEZ-67", "Izmijeni Odustani je CLOSE: nema SQL-a i nema refresha", "netgen/Form2.vb:7033; netgen/Form1.vb:5429", "kontrola", "Izmijeni.buttonCLOSE", "refreshMaster", "false");
            R(l, xml, "JEZ-68", "Izmijeni Spremi zatvori formu i osvježi master (Zadnji=true)", "netgen/Form2.vb:6888; netgen/Form1.vb:5519", "kontrola", "Izmijeni.buttonOK", "refreshMaster", "true");
            R(l, xml, "JEZ-69", "BORDERSTYLE se ne primijeni: ostaje Sizable", "netgen/Form2.vb:999-1124", "gumb", "Unesi", "border", "Sizable");
            R(l, xml, "JEZ-70", "Azur na Unesi ide kroz Rafalno, ne kroz lijepljenje ONSUBMIT; @@ se zamijeni, drugi niz @ime= se ne dodaje, upis se ne šalje u testu", "netgen/Form2.vb:7510,3655", "kontrola", "Unesi.buttonOK", "sqlImaZalijep", "false");
            R(l, xml, "JEZ-71", "PERMISIO default true šalje select ssPermission", "netgen/Form1.vb:3153,2706", "otvori", "", "permisioSql", "ssPermission");
            R(l, xml, "JEZ-72", "Enter na gridu je None", "netgen/Form1.vb:3419-3421", "otvori", "", "enterKey", "None");
            R(l, xml, "JEZ-73", "tkosmomi iz SETDEFAULTS je u međuspremniku prije ispisa", "netgen/Form1.vb:3995-3998", "otvori", "", "cb.tkosmomi", "ima");
        }

        private static void R(List<Stavka> l, string xml, string id, string opis, string izvor, string akcija, string cilj, string kljuc, string ocekivano)
        {
            l.Add(Qa.Runtime(Ime, id, opis, izvor, xml, akcija, cilj, kljuc, ocekivano));
        }

        private static string Klik(NgForm f, string ime)
        {
            NgControl c = Nadji(f, ime);
            return c == null ? "" : (c.Atributi.Vrijednost("ONCLICK") ?? "");
        }

        private static string Natpis(NgForm f, string ime)
        {
            NgControl c = Nadji(f, ime);
            return c == null ? "" : (c.Atributi.Vrijednost("CAPTION") ?? "");
        }

        private static NgControl Nadji(NgForm f, string ime)
        {
            if (f == null)
                return null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (f.Kontrole[i].Name == ime)
                    return f.Kontrole[i];
            }
            return null;
        }

        private static string Attr(NgProject p, string ime)
        {
            return p.Atributi.Vrijednost(ime) ?? "";
        }

        private static bool IstiHash(string a, string b)
        {
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
