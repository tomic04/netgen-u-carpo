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
    /// Ocekivano ponasanje je iz netgen/Form1.vb, Form2.vb, Class1.vb i XML-a.
    /// Parser se usporeduje odmah. Crtež i SQL tok idu kroz GranulaSesija i dok
    /// kuka nije spojena ostaju otvoreni.
    /// </summary>
    public static class OznakaKL
    {
        public const string Ime = "OznakaKL";
        public const string MasterSql =
            "select klmasterrobaid, sortkey, Naziv, oznaka, oznaka2 as DodatnaOznaka from klmasterroba order by sortkey";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "OznakaKL.xml");
            string kopija = Path.Combine(root, "NetGenBot", "granule", "Biovega", "OznakaKL.xml");

            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "OKL-01", "XML postoji", "definitions/netgen/BIOVEGA/OznakaKL.xml", "nema datoteke"));
                return l;
            }

            if (File.Exists(kopija))
            {
                l.Add(IstiHash(xml, kopija)
                    ? Qa.Z(Ime, "OKL-00", "NetGenBot kopija je isti sadrzaj", "definitions/netgen/BIOVEGA/OznakaKL.xml")
                    : Qa.P(Ime, "OKL-00", "NetGenBot kopija je isti sadrzaj", "definitions/netgen/BIOVEGA/OznakaKL.xml", "SHA-256 se razlikuje"));
            }

            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "OKL-01", "XmlDocument ucita PROJECT", "netgen/Form1.vb:3263", greska));
                return l;
            }
            l.Add(Qa.Z(Ime, "OKL-01", "XmlDocument ucita PROJECT", "netgen/Form1.vb:3263"));

            l.Add(Qa.Jednako(Ime, "OKL-02", "Naslov je tekst CAPTION, windows-1250", "netgen/Form1.vb:3522-3528",
                "Pregled i ažuriranje dodatne oznake klase robe", p.Naslov));
            l.Add(p.Naslov != null && p.Naslov.IndexOf("DODKLAZUR", StringComparison.Ordinal) < 0
                ? Qa.Z(Ime, "OKL-03", "Element NAME nije naslov prozora", "netgen/Form1.vb:3522")
                : Qa.P(Ime, "OKL-03", "Element NAME nije naslov prozora", "netgen/Form1.vb:3522", p.Naslov));
            l.Add(p.Naslov != null && p.Naslov.IndexOf("v[", StringComparison.Ordinal) < 0
                ? Qa.Z(Ime, "OKL-04", "Nema VERSION pa nema sufiksa v[]", "netgen/Form1.vb:3524-3528")
                : Qa.P(Ime, "OKL-04", "Nema VERSION pa nema sufiksa v[]", "netgen/Form1.vb:3524-3528", p.Naslov));

            l.Add(Qa.Jednako(Ime, "OKL-05", "Nema ONLOAD pa vrijedi STARTUPSTORA", "netgen/Form1.vb:3367-3368",
                MasterSql, p.OnLoad));
            l.Add(Qa.Jednako(Ime, "OKL-06", "Master SQL je tocan tekst, bez dodanih parametara", "netgen/Form1.vb:3882",
                MasterSql, (p.Raspored == null ? "" : p.Raspored.MasterSql)));
            l.Add(string.IsNullOrEmpty(p.Tree)
                ? Qa.Z(Ime, "OKL-07", "Nema TREE", "netgen/Form1.vb:3387")
                : Qa.P(Ime, "OKL-07", "Nema TREE", "netgen/Form1.vb:3387", p.Tree));
            l.Add(string.IsNullOrEmpty(p.DetailGrid)
                ? Qa.Z(Ime, "OKL-08", "Nema DETAILGRID", "netgen/Form1.vb:3373")
                : Qa.P(Ime, "OKL-08", "Nema DETAILGRID", "netgen/Form1.vb:3373", p.DetailGrid));
            l.Add(Qa.Jednako(Ime, "OKL-09", "MASTERDBLCLICK je FRMEDIT", "netgen/Form1.vb:3388-3389",
                "FRMEDIT", p.MasterDblClick));

            if (p.Gumbi.Count == 1 && p.Gumbi[0].Natpis == "Izmjena" && p.Gumbi[0].Naredba == "FRMEDIT")
                l.Add(Qa.Z(Ime, "OKL-10", "CMD1CAPTION se lomi na prvoj dvotocki", "netgen/Form1.vb:3314-3325"));
            else
                l.Add(Qa.P(Ime, "OKL-10", "CMD1CAPTION se lomi na prvoj dvotocki", "netgen/Form1.vb:3314-3325",
                    p.Gumbi.Count == 0 ? "(nema)" : p.Gumbi[0].Natpis + "/" + p.Gumbi[0].Naredba));
            l.Add(p.Gumbi.Count == 1
                ? Qa.Z(Ime, "OKL-11", "Nema CMD2-CMD10", "netgen/Form1.vb:3827-3837")
                : Qa.P(Ime, "OKL-11", "Nema CMD2-CMD10", "netgen/Form1.vb:3827-3837", "gumbi=" + p.Gumbi.Count));

            l.Add(Qa.Jednako(Ime, "OKL-12", "GROUPBYBOXVISIBLE, FROZENCOLUMNS, CHECKBOXVISIBLE, TOTALROWVISIBLE ostaju kako su napisani",
                "netgen/Form1.vb:3350,3362,3459-3460",
                "FALSE|0|FALSE|FALSE",
                Attr(p, "GROUPBYBOXVISIBLE") + "|" + Attr(p, "FROZENCOLUMNS") + "|" + Attr(p, "CHECKBOXVISIBLE") + "|" + Attr(p, "TOTALROWVISIBLE")));
            l.Add(GridAllowEditNijeUkljucen(p));

            NgForm f = p.NadjiFormu("FRMEDIT");
            if (f == null)
            {
                l.Add(Qa.P(Ime, "OKL-14", "Jedna forma FRMEDIT", "definitions/netgen/BIOVEGA/OznakaKL.xml:7", "nema forme"));
                return l;
            }
            l.Add(p.Forme.Count == 1
                ? Qa.Z(Ime, "OKL-14", "Jedna forma FRMEDIT", "definitions/netgen/BIOVEGA/OznakaKL.xml:7")
                : Qa.P(Ime, "OKL-14", "Jedna forma FRMEDIT", "definitions/netgen/BIOVEGA/OznakaKL.xml:7", "forme=" + p.Forme.Count));

            string onSubmit = Sirovi(xml, "FRMEDIT", "ONSUBMIT").Trim();
            l.Add(Qa.Jednako(Ime, "OKL-15", "ONSUBMIT je update i nije mu zalijepljen niz @ime=", "netgen/Form2.vb:1036-1039,3378-3388",
                onSubmit, f.OnSubmit));
            string spremljeno = f.OnSubmit ?? "";
            bool updateGrana = spremljeno.Length >= 6
                && "INSERTDELETEUPDATESELECT".IndexOf(spremljeno.Substring(0, 6).ToUpperInvariant(), StringComparison.Ordinal) >= 0;
            l.Add(updateGrana && spremljeno.IndexOf("@oznaka2=", StringComparison.Ordinal) < 0
                ? Qa.Z(Ime, "OKL-15b", "Prvih 6 slova je update pa se parametri ne lijepe na tekst", "netgen/Form2.vb:3378-3388")
                : Qa.P(Ime, "OKL-15b", "Prvih 6 slova je update pa se parametri ne lijepe na tekst", "netgen/Form2.vb:3378-3388", spremljeno));

            string onLoad = Sirovi(xml, "FRMEDIT", "ONLOAD").Trim();
            l.Add(Qa.Jednako(Ime, "OKL-16", "ONLOAD forme je select oznaka2", "netgen/Form2.vb:1041-1042",
                onLoad, f.OnLoad));

            if (f.Gumbi.Count == 2 && f.Gumbi[0].Natpis == "Prihvati" && f.Gumbi[0].Naredba == ""
                && f.Gumbi[1].Natpis == "Odustani" && f.Gumbi[1].Naredba == "")
                l.Add(Qa.Z(Ime, "OKL-17", "CMD1 i CMD2 na dijalogu su cijeli natpis, bez loma na dvotocki", "netgen/Form2.vb:1061-1066"));
            else
                l.Add(Qa.P(Ime, "OKL-17", "CMD1 i CMD2 na dijalogu su cijeli natpis, bez loma na dvotocki", "netgen/Form2.vb:1061-1066", "gumbi=" + f.Gumbi.Count));

            NgControl label = f.Kontrole.Count > 0 ? f.Kontrole[0] : null;
            NgControl tekst = f.Kontrole.Count > 1 ? f.Kontrole[1] : null;
            l.Add(label != null && label.Name == "Label16" && label.Grana == NgGrana.Label
                && label.Atributi.Vrijednost("CAPTION") == "Dodatna oznaka"
                && label.Atributi.Vrijednost("TEXT") == "Label16"
                ? Qa.Z(Ime, "OKL-19", "Label ide u granu label; natpis je CAPTION, TEXT se cuva ali ga kod ne cita", "netgen/Form2.vb:1296,2932")
                : Qa.P(Ime, "OKL-19", "Label ide u granu label; natpis je CAPTION, TEXT se cuva ali ga kod ne cita", "netgen/Form2.vb:1296,2932", label == null ? "(nema)" : label.Name));
            l.Add(tekst != null && tekst.Name == "oznaka2" && tekst.Grana == NgGrana.Text && tekst.TypeRaw == "text"
                ? Qa.Z(Ime, "OKL-20", "Polje oznaka2 je text", "netgen/Form2.vb:1996")
                : Qa.P(Ime, "OKL-20", "Polje oznaka2 je text", "netgen/Form2.vb:1996", tekst == null ? "(nema)" : tekst.TypeRaw));
            l.Add(label != null && label.Atributi.Vrijednost("BOLD") == "FALSE" && NemaSvojstvo(label, "Bold")
                ? Qa.Z(Ime, "OKL-21", "BOLD se ne mapira na font (kod cita STYLE, ne BOLD)", "netgen/Form2.vb:3079-3081")
                : Qa.P(Ime, "OKL-21", "BOLD se ne mapira na font (kod cita STYLE, ne BOLD)", "netgen/Form2.vb:3079-3081", "BOLD=" + (label == null ? "" : label.Atributi.Vrijednost("BOLD"))));

            l.Add(Qa.Jednako(Ime, "OKL-22", "Sirove dimenzije forme i kontrola, dioba s 14 jos nije primijenjena u modelu",
                "netgen/Form2.vb:58,1093-1094,2945-2950",
                "5112|2189|389|158|2002|2650",
                AttrF(f, "WIDTH") + "|" + AttrF(f, "HEIGHT") + "|" + AttrC(label, "TOP") + "|" + AttrC(label, "LEFT") + "|" + AttrC(tekst, "LEFT") + "|" + AttrC(tekst, "WIDTH")));

            l.Add(p.Raspored != null && p.Raspored.ImaMasterSql && !p.Raspored.ImaStablo && !p.Raspored.ImaDetail
                ? Qa.Z(Ime, "OKL-24", "Model: master SQL postoji, stablo i detail SQL ne", "netgen/Form1.vb:3367,3373,3387")
                : Qa.P(Ime, "OKL-24", "Model: master SQL postoji, stablo i detail SQL ne", "netgen/Form1.vb:3367,3373,3387", ""));

            DodajCitanje(l);
            DodajRuntime(l, xml);
            return l;
        }

        private static void DodajCitanje(List<Stavka> l)
        {
            var shema = Citanje.Select(MasterSql, 5000);
            if (!shema.Uspjeh)
            {
                string razlog = "spin samo citanje nije uspjelo: " + shema.Greska;
                l.Add(Qa.O(Ime, "OKL-25", "SELECT mastera vraca stupce redom", "netgen/Form1.vb:3882", razlog));
                l.Add(Qa.O(Ime, "OKL-26", "ORDER BY sortkey je poredak koji grid mora pokazati", "netgen/Class1.vb:1360", razlog));
                l.Add(Qa.O(Ime, "OKL-27", "Tipovi stupaca odreduju sirinu, format i skrivanje *id", "netgen/Class1.vb:1087-1244", razlog));
                return;
            }
            string imena = string.Join(",", shema.Imena.ToArray());
            l.Add(Qa.Jednako(Ime, "OKL-25", "SELECT mastera vraca stupce redom", "netgen/Form1.vb:3882",
                "klmasterrobaid,sortkey,Naziv,oznaka,DodatnaOznaka", imena));
            l.Add(shema.SortkeyNeopada
                ? Qa.Z(Ime, "OKL-26", "ORDER BY sortkey je poredak koji grid mora pokazati (uzorak " + shema.Redova + " redova)", "netgen/Class1.vb:1360")
                : Qa.O(Ime, "OKL-26", "ORDER BY sortkey je poredak koji grid mora pokazati", "netgen/Class1.vb:1360",
                    "uzorak nije monoton po string usporedbi; grid i dalje mora poslati SQL kakav jest"));
            l.Add(shema.Tipovi.Count == 5
                ? Qa.Z(Ime, "OKL-27", "Tipovi sa servera: " + string.Join(",", shema.Tipovi.ToArray()), "netgen/Class1.vb:1087-1244")
                : Qa.P(Ime, "OKL-27", "Tipovi stupaca", "netgen/Class1.vb:1087-1244", imena));
        }

        private static void DodajRuntime(List<Stavka> l, string xml)
        {
            R(l, xml, "OKL-30", "Redoslijed: nema SETDEFAULTS, drvo, pa master SQL, detail se ne zove",
                "netgen/Form1.vb:3862-3883,2104-2113", "otvori", "", "sqlTrag",
                MasterSql);
            R(l, xml, "OKL-31", "Prazan TREE i prazan COMBO skrivaju TreeView2",
                "netgen/Form1.vb:4310-4316,3345-3347", "otvori", "", "stabloVidljivo", "false");
            R(l, xml, "OKL-32", "Lijevi splitter je 0 i gumb drveta je ugasen",
                "netgen/Form1.vb:4313-4315", "otvori", "", "lijeviSplitter", "0");
            R(l, xml, "OKL-33", "Gumb za drvo je disabled",
                "netgen/Form1.vb:4315", "otvori", "", "gumbDrvoEnabled", "false");
            R(l, xml, "OKL-34", "Combo na podlozi je skriven",
                "netgen/Form1.vb:3345-3347", "otvori", "", "comboVidljiv", "false");
            R(l, xml, "OKL-35", "Nema detail SQL pa se donji splitter spusti na visinu-50",
                "netgen/Form1.vb:2111-2113", "otvori", "", "detailSql", "");
            R(l, xml, "OKL-36", "Izbornik je skriven (nema MENU)",
                "netgen/Form1.vb:3552-3553,3449-3450,3491-3494", "otvori", "", "menuVidljiv", "false");
            R(l, xml, "OKL-37", "Help gumb nije vidljiv (nema HELPKOD)",
                "netgen/Form1.vb:3491-3494", "otvori", "", "helpVidljiv", "false");
            R(l, xml, "OKL-81", "Nema TIMER: statusna oznaka timera je skrivena",
                "netgen/Form1.vb:3449-3450", "otvori", "", "timerVidljiv", "false");
            R(l, xml, "OKL-38", "DXGRID nije zadan pa se crta Janus grid, ne DevExpress",
                "netgen/Form1.vb:1861,1906-1908", "otvori", "", "dxGrid", "false");
            R(l, xml, "OKL-39", "Prve dvije kolone su skriveni Icon i Selector (CHECKBOXVISIBLE FALSE)",
                "netgen/Class1.vb:1010-1027; netgen/Form1.vb:2043-2046", "otvori", "", "kolonePocetak",
                "Icon:false:22|Selector:false:22");
            R(l, xml, "OKL-40", "klmasterrobaid je skriven jer ime zavrsava na ID",
                "netgen/Class1.vb:1124-1126", "otvori", "", "kolona.klmasterrobaid.vidljiva", "false");
            R(l, xml, "OKL-41", "Int stupac: sirina 34, format ##0, poravnanje Far, EditType NoEdit",
                "netgen/Class1.vb:1087-1139", "otvori", "", "kolona.klmasterrobaid.format", "##0");
            R(l, xml, "OKL-42", "String stupac bez WIDTH u MASTERCOLUMNS ima sirinu 100 (nije 200; to je samo hijerarhijski put)",
                "netgen/Class1.vb:1201-1204", "otvori", "", "kolona.Naziv.sirina", "100");
            R(l, xml, "OKL-43", "Natpis stupca DodatnaOznaka je alias iz SQL-a, ne oznaka2",
                "netgen/Class1.vb:1037", "otvori", "", "kolona.DodatnaOznaka.natpis", "DodatnaOznaka");
            R(l, xml, "OKL-44", "TotalRow na kraju FALSE iako CreateGridColumns prvo stavi True",
                "netgen/Class1.vb:1363; netgen/Form1.vb:2049-2050", "otvori", "", "totalRow", "false");
            R(l, xml, "OKL-45", "HeaderLines je 3",
                "netgen/Class1.vb:1364", "otvori", "", "headerLines", "3");
            R(l, xml, "OKL-46", "GroupByBox je skriven, FrozenColumns je 0",
                "netgen/Form1.vb:3350,3362", "otvori", "", "groupByBox", "false");
            R(l, xml, "OKL-47", "FrozenColumns je 0",
                "netgen/Form1.vb:3362", "otvori", "", "frozenColumns", "0");
            R(l, xml, "OKL-48", "AUTOMATICSORT nije false pa sort klikom na zaglavlje ostaje ukljucen",
                "netgen/Form1.vb:3401-3404", "otvori", "", "automaticSort", "true");
            R(l, xml, "OKL-49", "EDITCOLUMNS prazan i GRIDALLOWEDIT se ne cita: nema editiranja celije",
                "netgen/Form1.vb:3466; netgen/Class1.vb:1137-1139; netgen/Resource1.Designer.vb:75", "otvori", "", "editType", "NoEdit");
            R(l, xml, "OKL-50", "EditMode je EditOff",
                "netgen/Class1.vb:1003", "otvori", "", "editMode", "EditOff");
            R(l, xml, "OKL-51", "Enter na gridu ne otvara formu (EnterKeyBehavior None, nema ONENTERKEY)",
                "netgen/Form1.vb:3419-3423", "otvori", "", "enterKey", "None");
            R(l, xml, "OKL-52", "Selekcija je visestruka (designer), ne DX default jednog reda",
                "netgen/Form1.designer.vb:654", "otvori", "", "selectionMode", "MultipleSelection");
            R(l, xml, "OKL-53", "IncrementalSearch je AllCharacters",
                "netgen/Form1.designer.vb:641", "otvori", "", "incrementalSearch", "AllCharacters");
            R(l, xml, "OKL-54", "EmptyRows je true",
                "netgen/Class1.vb:1000", "otvori", "", "emptyRows", "true");
            R(l, xml, "OKL-55", "RowHeaders su vidljivi, HideSelection je Highlight",
                "netgen/Form1.designer.vb:640,655", "otvori", "", "rowHeaders", "true");
            R(l, xml, "OKL-56", "HideSelection je Highlight",
                "netgen/Form1.designer.vb:640", "otvori", "", "hideSelection", "Highlight");
            R(l, xml, "OKL-57", "Grid je Dock Fill",
                "netgen/Form1.vb:1906", "otvori", "", "gridDock", "Fill");
            R(l, xml, "OKL-58", "Ako ima redova, fokus je na gridu",
                "netgen/Form1.vb:2059-2072", "otvori", "", "fokus", "grid");
            R(l, xml, "OKL-59", "Nema kolone id pa je @@id vrijednost Cells(2), prvi podatak (klmasterrobaid)",
                "netgen/Form1.vb:5566-5582", "otvori", "", "idJe", "klmasterrobaid");
            R(l, xml, "OKL-60", "Vidljiv je samo gumb Izmjena, naredba FRMEDIT",
                "netgen/Form1.vb:3827-3837,5664-5667", "otvori", "", "gumb.1", "Izmjena|FRMEDIT|true");
            R(l, xml, "OKL-61", "Dvostruki klik otvara FRMEDIT",
                "netgen/Form1.vb:6935-6936", "dblclick", "", "forma", "FRMEDIT");
            R(l, xml, "OKL-62", "Naslov dijaloga je Izmjena oznake",
                "netgen/Form2.vb:969", "gumb", "Izmjena", "formaNaslov", "Izmjena oznake");
            R(l, xml, "OKL-63", "Sirina dijaloga je 5112/14, visina 2189/14, prije GetSetting registra",
                "netgen/Form2.vb:1093-1094,2785-2788", "gumb", "Izmjena", "formaSirina", Qa.Px("5112").ToString());
            R(l, xml, "OKL-64", "Visina dijaloga je 2189/14",
                "netgen/Form2.vb:1094", "gumb", "Izmjena", "formaVisina", Qa.Px("2189").ToString());
            R(l, xml, "OKL-65", "Label prikazuje CAPTION, ne TEXT=Label16",
                "netgen/Form2.vb:2932-2938", "gumb", "Izmjena", "kontrola.Label16.tekst", "Dodatna oznaka");
            R(l, xml, "OKL-66", "Label TOP/LEFT/W/H su jedinice/14, VB zaokruzivanje na parni",
                "netgen/Form2.vb:2945-2950", "gumb", "Izmjena", "kontrola.Label16.top", Qa.Px("389").ToString());
            R(l, xml, "OKL-67", "oznaka2 LEFT je 2002/14",
                "netgen/Form2.vb:2947", "gumb", "Izmjena", "kontrola.oznaka2.left", Qa.Px("2002").ToString());
            R(l, xml, "OKL-68", "ONLOAD upise oznaka2 iz reda; tekst null postaje prazan",
                "netgen/Form2.vb:2016-2018", "gumb", "Izmjena", "kontrola.oznaka2.izOnLoada", "true");
            R(l, xml, "OKL-69", "Prihvati izvrsi update tek nakon zatvaranja i onda osvjezi master",
                "netgen/Form2.vb:3367-3396; netgen/Form1.vb:5510-5519", "prihvati", "Prihvati", "refreshMaster", "true");
            R(l, xml, "OKL-70", "Update ne dobije zalijepljene parametre; @@oznaka2 i @@ID su zamijenjeni",
                "netgen/Form2.vb:3378-3388; netgen/Class1.vb:740-851", "prihvati", "Prihvati", "sqlImaZalijep", "false");
            R(l, xml, "OKL-71", "Apostrof u tekstu ide kao '' (preko ¸¸), zarez se vrati iz ~~",
                "netgen/Form2.vb:3655; netgen/Class1.vb:87,850", "prihvati", "Prihvati", "sqlApostrof", "''");
            R(l, xml, "OKL-72", "Odustani ne salje SQL i ne osvjezava master",
                "netgen/Form2.vb:3400-3402; netgen/Form1.vb:5429", "odustani", "Odustani", "refreshMaster", "false");
            R(l, xml, "OKL-73", "Escape zatvara bez OKClick i bez SQL-a",
                "netgen/Form2.vb:4161-4166", "escape", "", "refreshMaster", "false");
            R(l, xml, "OKL-74", "Nema AcceptButton: Enter u tekstu ne radi Prihvati",
                "netgen/Form2.vb:3367", "gumb", "Izmjena", "acceptButton", "");
            R(l, xml, "OKL-75", "Tab: label (auto), pa oznaka2, pa Prihvati, pa Odustani",
                "netgen/Form2.vb:3027-3043,2775-2776", "gumb", "Izmjena", "tab", "Label16,oznaka2,Prihvati,Odustani");
            R(l, xml, "OKL-76", "PERMISIO nije u XML-u pa default ostaje true i salje se select ssPermission",
                "netgen/Form1.vb:3153,3395,3414-3416,2706", "otvori", "", "permisioSql",
                "ssPermission");
            R(l, xml, "OKL-77", "Kontekstni izbornik ima standardne stavke iako POPUP ne postoji",
                "netgen/Form1.vb:3620-3673", "otvori", "", "kontekst",
                "Spremi postavke grida|Reset grida|Export grid|Grupiraj / razgrupiraj|Format style|Izgled grida|Ispis REPX|Ispis");
            R(l, xml, "OKL-78", "Group header total Count s prefiksom [ i sufiksom  ]",
                "netgen/Form1.vb:2136-2142", "otvori", "", "groupCount", "[ ]");
            R(l, xml, "OKL-79", "Pocetni poredak redova je poredak SQL-a, grid sam ne sortira",
                "netgen/Form1.vb:1999", "otvori", "", "pocetniSort", "sql");
            R(l, xml, "OKL-80", "Nema ICON pa ikona dolazi iz hosta (Ikona), ne iz Graphics\\Icons",
                "netgen/Form1.vb:3430-3440", "otvori", "", "ikona", "host");
        }

        private static void R(List<Stavka> l, string xml, string id, string opis, string izvor, string akcija, string cilj, string kljuc, string ocekivano)
        {
            l.Add(Qa.Runtime(Ime, id, opis, izvor, xml, akcija, cilj, kljuc, ocekivano));
        }

        private static Stavka GridAllowEditNijeUkljucen(NgProject p)
        {
            if (Attr(p, "GRIDALLOWEDIT") != "TRUE" || Attr(p, "GRIDALLOWEDITCOLUMNS") != "oznaka2,oznaka")
                return Qa.P(Ime, "OKL-13", "GRIDALLOWEDIT se cuva sirov i ne ukljucuje edit", "netgen/Form1.vb:3466", Attr(p, "GRIDALLOWEDIT"));
            var props = p.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                string n = props[i].Name;
                if (n.IndexOf("AllowEdit", StringComparison.OrdinalIgnoreCase) < 0
                    && n.IndexOf("GridAllow", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                object v = props[i].GetValue(p, null);
                if (v is bool && (bool)v)
                    return Qa.P(Ime, "OKL-13", "GRIDALLOWEDIT se cuva sirov i ne ukljucuje edit", "netgen/Form1.vb:3466", n + "=true");
            }
            return Qa.Z(Ime, "OKL-13", "GRIDALLOWEDIT se cuva sirov i ne ukljucuje edit", "netgen/Form1.vb:3466");
        }

        private static bool NemaSvojstvo(object o, string ime)
        {
            return o.GetType().GetProperty(ime) == null;
        }

        private static string Attr(NgProject p, string ime)
        {
            return p.Atributi.Vrijednost(ime) ?? "";
        }

        private static string AttrF(NgForm f, string ime)
        {
            return f == null ? "" : (f.Atributi.Vrijednost(ime) ?? "");
        }

        private static string AttrC(NgControl c, string ime)
        {
            return c == null ? "" : (c.Atributi.Vrijednost(ime) ?? "");
        }

        private static string Sirovi(string xml, string element, string atribut)
        {
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xml);
            XmlNode n = doc.SelectSingleNode("/PROJECT/" + element);
            if (n == null || n.Attributes == null || n.Attributes[atribut] == null)
                return "";
            return n.Attributes[atribut].Value ?? "";
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
