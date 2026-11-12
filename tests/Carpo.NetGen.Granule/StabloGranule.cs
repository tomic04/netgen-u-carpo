using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// TREE i master-detail. Ocekivanje je iz Form1.NapuniDrvoVrijeme, DazovaIzStore,
    /// TreeView2_AfterSelect, PokaziUpit i PokaziUpitDetalj. Kuka koja nema kljuc
    /// ostavlja stavku otvorenu.
    /// </summary>
    internal static class Stablo
    {
        public static string Atribut(string xml, string ime)
        {
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xml);
            XmlNode n = doc.DocumentElement;
            if (n == null || n.Attributes == null || n.Attributes[ime] == null)
                return null;
            return n.Attributes[ime].Value ?? "";
        }

        public static string Caption(string xml)
        {
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(xml);
            XmlNode n = doc.SelectSingleNode("/PROJECT/CAPTION");
            if (n == null || n.ChildNodes.Count == 0)
                return "";
            return n.ChildNodes[0].InnerText ?? "";
        }

        public static string VerzijaNaslova(string xml, NgProject p)
        {
            string cap = Caption(xml);
            string v = "";
            if (p.Atributi.Ima("VERSION"))
                v = (p.Atributi.Vrijednost("VERSION") ?? "").Trim();
            if (v.Length > 0)
                return cap + "  v[" + v + "]";
            return cap;
        }

        public static Stavka Meni(string granula, string id, string fajl)
        {
            string greska;
            string safe = fajl.Replace("'", "");
            string sql = "select Parametar from ssMeni where Parametar like '%:" + safe
                + "' or Parametar like '%\\" + safe + "' or Parametar like '%/" + safe + "'";
            List<string> redovi = Citanje.Stupac(sql, out greska);
            if (redovi == null)
                return Qa.O(granula, id, "ssMeni (samo citanje) za " + fajl, "docs/analiza/06-prijedlog-arhitekture.md", greska);
            if (redovi.Count == 0)
                return Qa.O(granula, id, fajl + " nije u ssMeni.Parametar", "dbo.ssMeni", "0 redova, asm:NetGenDLL.loader/frm");
            return Qa.Z(granula, id, fajl + " je u ssMeni: " + string.Join(" ; ", redovi.ToArray()), "dbo.ssMeni.Parametar");
        }

        public static void R(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Runtime(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }

        public static void K(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Kljuc(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }

        public static void M(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Mora(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }
    }

    public static class Statusi
    {
        public const string Ime = "Statusi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "Statusi.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "STA-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "STA-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "STA-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(Qa.Jednako(Ime, "STA-02", "Naslov ima v[verzija] s dva razmaka", "netgen/Form1.vb:3524-3525", Stablo.VerzijaNaslova(xml, p), p.Naslov));
            string tree = (Stablo.Atribut(xml, "TREE") ?? "").Trim();
            string det = (Stablo.Atribut(xml, "DETAILGRID") ?? "").Trim();
            string on = (Stablo.Atribut(xml, "ONLOAD") ?? "").Trim();
            l.Add(Qa.Jednako(Ime, "STA-03", "TREE je spTreeVrstaDok:POPUNI_FORM", "netgen/Form1.vb:3387,4347-4351", tree, p.Tree ?? ""));
            l.Add(tree.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0 && tree.EndsWith(":POPUNI_FORM", StringComparison.Ordinal)
                ? Qa.Z(Ime, "STA-04", "Lijevo od dvotocke sadrzi tree pa ide DazovaIzStore, forma je POPUNI_FORM", "netgen/Form1.vb:4350-4351")
                : Qa.P(Ime, "STA-04", "Grana SQL stabla", "netgen/Form1.vb:4350", tree));
            l.Add(Qa.Jednako(Ime, "STA-05", "DETAILGRID je spStatusiQuery action 1 po @@ID", "netgen/Form1.vb:3373,2104-2109", det, p.DetailGrid ?? ""));
            l.Add(Qa.Jednako(Ime, "STA-06", "Pocetni master je ONLOAD s literalnim @node=0, ne @@node", "netgen/Form1.vb:3368,3882", on, p.OnLoad));
            l.Add(p.Raspored != null && p.Raspored.ImaStablo && p.Raspored.ImaDetail
                ? Qa.Z(Ime, "STA-07", "Model vidi i stablo i detail SQL", "netgen/Form1.vb:3387,3373")
                : Qa.P(Ime, "STA-07", "Model vidi i stablo i detail SQL", "netgen/Form1.vb:3387", ""));

            NgForm pop = p.NadjiFormu("POPUNI_FORM");
            l.Add(pop != null && pop.OnSubmit.IndexOf("@@node", StringComparison.Ordinal) >= 0
                && string.Equals(pop.Atributi.Vrijednost("FILLEXPLORER"), "TRUE", StringComparison.OrdinalIgnoreCase)
                && pop.Kontrole.Count == 0
                ? Qa.Z(Ime, "STA-08", "POPUNI_FORM nema INPUT, FILLEXPLORER=TRUE, ONSUBMIT nosi @@node", "netgen/Form1.vb:5354-5370,5445-5452")
                : Qa.P(Ime, "STA-08", "POPUNI_FORM", "netgen/Form1.vb:5445", pop == null ? "nema" : pop.OnSubmit));
            l.Add(p.MasterDblClick == "UPDATEFORM"
                ? Qa.Z(Ime, "STA-09", "Dvostruki klik mastera je UPDATEFORM", "netgen/Form1.vb:6935")
                : Qa.P(Ime, "STA-09", "MASTERDBLCLICK", "netgen/Form1.vb:6935", p.MasterDblClick));
            string detDbl = p.Atributi.Vrijednost("DETAILDBLCLICK") ?? "";
            l.Add(Qa.Jednako(Ime, "STA-10", "Dvostruki klik detaila je OPFORM", "netgen/Form1.vb:7201-7202", "OPFORM", detDbl));
            l.Add(p.Gumbi.Count >= 1 && p.Gumbi[0].Natpis == "Unesi" && p.Gumbi[0].Naredba == "INSERTFORM"
                ? Qa.Z(Ime, "STA-11", "CMD1 je Unesi:INSERTFORM", "netgen/Form1.vb:3314")
                : Qa.P(Ime, "STA-11", "CMD1", "netgen/Form1.vb:3314", p.Gumbi.Count == 0 ? "" : p.Gumbi[0].Naredba));

            l.Add(Stablo.Meni(Ime, "STA-12", "Statusi.xml"));

            Stablo.R(l, Ime, xml, "STA-20", "TREE nije prazan: stablo je vidljivo", "netgen/Form1.vb:4322-4330", "stabloVidljivo", "true");
            Stablo.R(l, Ime, xml, "STA-21", "Nema SPLITTERDISTANCE: lijevi splitter je 290", "netgen/Form1.vb:4323-4326", "lijeviSplitter", "290");
            Stablo.R(l, Ime, xml, "STA-22", "Gumb drveta je ukljucen", "netgen/Form1.vb:4328", "gumbDrvoEnabled", "true");
            Stablo.R(l, Ime, xml, "STA-23", "Nema COMBO", "netgen/Form1.vb:3345-3347", "comboVidljiv", "false");
            Stablo.R(l, Ime, xml, "STA-24", "Detail SQL je onaj iz DETAILGRID", "netgen/Form1.vb:3373", "detailSql", det);
            Stablo.R(l, Ime, xml, "STA-25", "CHECKBOXVISIBLE FALSE sakrije selector i na masteru", "netgen/Form1.vb:2043-2046", "selectorVidljiv", "false");
            Stablo.R(l, Ime, xml, "STA-26", "TOTALROWVISIBLE FALSE", "netgen/Form1.vb:2049-2050", "totalRow", "false");
            Stablo.R(l, Ime, xml, "STA-27", "Pocetni SQL trag je ONLOAD, prije klika na cvor", "netgen/Form1.vb:3879-3882", "sqlTrag", on);
            Stablo.K(l, Ime, xml, "STA-30", "Stablo iz store: stupci 0 id, 1 parent, 2 natpis, 3 tag; relacija 0->1", "netgen/Form1.vb:4450,4461-4478", "treeStupci", "0,1,2,3");
            Stablo.K(l, Ime, xml, "STA-31", "Korijen je samo prvi red. Dijete kojem je id 0 se preskace", "netgen/Form1.vb:4455,4498", "preskociId0", "true");
            Stablo.K(l, Ime, xml, "STA-32", "Tag cvora bez dvotocke u stupcu 3 je vrijednost:POPUNI_FORM", "netgen/Form1.vb:4474-4477", "tagBezDvotocke", "vrijednost:POPUNI_FORM");
            Stablo.K(l, Ime, xml, "STA-33", "AfterSelect: @@node je dio taga prije dvotocke, forma iza", "netgen/Form1.vb:7182-7188", "cvorForma", "POPUNI_FORM");
            Stablo.K(l, Ime, xml, "STA-34", "Forma bez kontrola ne otvara dijalog; FILLEXPLORER zamijeni master ONSUBMIT-om", "netgen/Form1.vb:5369-5370,5445-5452", "cvorSql", "spStatusiQuery @action=0, @node=@@node");
            Stablo.K(l, Ime, xml, "STA-35", "Punjenje stabla ide SqlDataAdapterom, ne ExecSql: prezivjeli @@ se ipak salje", "netgen/Form1.vb:4446-4447", "treeZaobilaziExecSql", "true");
            Stablo.M(l, Ime, xml, "STA-36", "Promjena reda mastera ponovo zove DETAILGRID", "netgen/Form1.vb:7413-7416", "detailNaSelekciji", "true");
            Stablo.M(l, Ime, xml, "STA-37", "Tijekom punjenja mastera detail se preskace (PunimMasterGrid), pa jednom na kraju", "netgen/Form1.vb:2270,2099-2109", "detailNakonMastera", "true");
            Stablo.M(l, Ime, xml, "STA-38", "DETCHECKBOXVISIBLE FALSE sakrije selector detaila (stupac 1)", "netgen/Form1.vb:2402-2406", "detSelector", "false");
            Stablo.M(l, Ime, xml, "STA-39", "Drugi detail upit ne radi CreateGridColumns ako RootTable vec postoji", "netgen/Form1.vb:2334-2336", "detailKoloneJednom", "true");
            return l;
        }
    }

    public static class KntBlaZak
    {
        public const string Ime = "KntBlaZak";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "KntBlaZak.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "BLA-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "BLA-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "BLA-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(Qa.Jednako(Ime, "BLA-02", "Naslov je CAPTION, nema VERSION", "netgen/Form1.vb:3526-3527", Stablo.VerzijaNaslova(xml, p), p.Naslov));
            string tree = (Stablo.Atribut(xml, "TREE") ?? "").Trim();
            string det = (Stablo.Atribut(xml, "DETAILGRID") ?? "").Trim();
            l.Add(Qa.Jednako(Ime, "BLA-03", "TREE se cuva cijeli, segmenti ostaju odvojeni s #", "netgen/Form1.vb:3387,4344", tree, p.Tree ?? ""));
            l.Add(tree.StartsWith("Blagajne:#", StringComparison.Ordinal)
                ? Qa.Z(Ime, "BLA-04", "Prvi segment Blagajne: nema tree u lijevom dijelu: staticki cvor, tag je samo dvotocka", "netgen/Form1.vb:4352-4355")
                : Qa.P(Ime, "BLA-04", "Staticki cvor Blagajne", "netgen/Form1.vb:4352", tree));
            int store = 0;
            string[] seg = tree.Split('#');
            for (int i = 0; i < seg.Length; i++)
            {
                int c = seg[i].IndexOf(':');
                if (c < 0)
                    continue;
                if (seg[i].Substring(0, c).IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0)
                    store++;
            }
            l.Add(store == 3
                ? Qa.Z(Ime, "BLA-05", "Tri segmenta imaju tree lijevo od dvotocke: tri korijena iz store, nisu djeca cvora Blagajne", "netgen/Form1.vb:4347-4356")
                : Qa.P(Ime, "BLA-05", "Tri SQL stabla", "netgen/Form1.vb:4350", "store=" + store));
            l.Add(Qa.Jednako(Ime, "BLA-06", "Detail trazi @@blagajneid i @@id", "netgen/Form1.vb:2301-2304", det, p.DetailGrid ?? ""));
            l.Add(det.IndexOf("@@blagajneid", StringComparison.OrdinalIgnoreCase) >= 0 && det.IndexOf("@@id", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "BLA-07", "@@id ide iz GetID, @@blagajneid iz parametara retka (DataMember)", "netgen/Form1.vb:2482,2564-2572,2304")
                : Qa.P(Ime, "BLA-07", "Parametri detaila", "netgen/Form1.vb:2304", det));
            var master = p.NadjiGrupu("MASTERCOLUMNS");
            var detail = p.NadjiGrupu("DETAILCOLUMNS");
            l.Add(master != null && string.Equals(master.ClearColums, "TRUE", StringComparison.OrdinalIgnoreCase)
                && detail != null && string.Equals(detail.ClearColums, "TRUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "BLA-08", "CLEARCOLUMS=TRUE na masteru i detailu: prvo sve sakriti, pa vratiti navedene", "netgen/Form1.vb:7459-7462")
                : Qa.P(Ime, "BLA-08", "CLEARCOLUMS", "netgen/Form1.vb:7459", master == null ? "" : master.ClearColums));
            l.Add(master != null && master.HeaderAlignment == "2"
                ? Qa.Z(Ime, "BLA-09", "HEADERALIGNMENT=2 ide na svaku navedenu kolonu", "netgen/Form1.vb:7530-7531")
                : Qa.P(Ime, "BLA-09", "HEADERALIGNMENT", "netgen/Form1.vb:7530", master == null ? "" : master.HeaderAlignment));
            NgColumn col = master != null && master.Kolone.Count > 0 ? master.Kolone[0] : null;
            l.Add(col != null && col.AgregateFunction == "0" && !col.ImaDvostrukoG
                ? Qa.Z(Ime, "BLA-10", "AGREGATEFUNCTION=0 (jedno G) nije prazan pa se dodjela izvrsi", "netgen/Form1.vb:7496-7507")
                : Qa.P(Ime, "BLA-10", "Agregat 0", "netgen/Form1.vb:7506", col == null ? "" : (col.AgregateFunction ?? "null")));
            string buttons = p.Atributi.Vrijednost("BUTTONS") ?? "";
            l.Add(buttons.IndexOf("ISPISZAK_FORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "BLA-11", "BUTTONS prepisuje CMD gumbe nakon razdvajanja CMD", "netgen/Form1.vb:3770-3776")
                : Qa.P(Ime, "BLA-11", "BUTTONS", "netgen/Form1.vb:3770", buttons));
            NgForm m0 = p.NadjiFormu("MJESEC_FORM0");
            l.Add(m0 != null && string.Equals(m0.Atributi.Vrijednost("FILLEXPLORER"), "TRUE", StringComparison.OrdinalIgnoreCase)
                && m0.OnSubmit.IndexOf("@@node", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "BLA-12", "MJESEC_FORM0 puni master iz ONSUBMIT, @@node u navodnicima", "netgen/Form1.vb:5445-5452")
                : Qa.P(Ime, "BLA-12", "MJESEC_FORM0", "netgen/Form1.vb:5452", m0 == null ? "" : m0.OnSubmit));
            l.Add(string.Equals(p.Permisio, "TRUE", StringComparison.OrdinalIgnoreCase) || string.Equals((p.Atributi.Vrijednost("PERMISIO") ?? ""), "TRUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "BLA-13", "PERMISIO=TRUE: select ssPermission se zove", "netgen/Form1.vb:3414-3416,2706")
                : Qa.P(Ime, "BLA-13", "PERMISIO", "netgen/Form1.vb:3395", p.Permisio ?? ""));
            l.Add(Stablo.Meni(Ime, "BLA-14", "KntBlaZak.xml"));

            Stablo.R(l, Ime, xml, "BLA-20", "Stablo je vidljivo", "netgen/Form1.vb:4322-4330", "stabloVidljivo", "true");
            Stablo.R(l, Ime, xml, "BLA-21", "Splitter 290", "netgen/Form1.vb:4326", "lijeviSplitter", "290");
            Stablo.R(l, Ime, xml, "BLA-22", "Detail SQL je spBLPrometGlaQueryNG", "netgen/Form1.vb:3373", "detailSql", det);
            Stablo.R(l, Ime, xml, "BLA-23", "Nema TOTALROWVISIBLE: total ostaje ukljucen", "netgen/Form1.vb:2049-2053", "totalRow", "true");
            Stablo.R(l, Ime, xml, "BLA-24", "CHECKBOXVISIBLE FALSE", "netgen/Form1.vb:2043", "selectorVidljiv", "false");
            Stablo.R(l, Ime, xml, "BLA-25", "PERMISIO trazi ssPermission", "netgen/Form1.vb:2706", "permisioSql", "ssPermission");
            Stablo.K(l, Ime, xml, "BLA-30", "Klik na Blagajne: tag je ':' pa je ime forme prazno i NovaGenerickaForma izadje", "netgen/Form1.vb:4354,7185-7187,4546", "klikBlagajne", "prazno");
            Stablo.K(l, Ime, xml, "BLA-31", "@@operateriid u TREE store se zamijeni prije SqlDataAdapter.Fill", "netgen/Form1.vb:4442-4446", "treeOperater", "zamijenjen");
            Stablo.K(l, Ime, xml, "BLA-32", "Tri storea su tri korijena, ne podgrane cvora Blagajne", "netgen/Form1.vb:4347-4480", "treeKorijeni", "4");
            Stablo.M(l, Ime, xml, "BLA-33", "Detail se ponovi na SelectionChanged mastera", "netgen/Form1.vb:7413-7416", "detailNaSelekciji", "true");
            Stablo.M(l, Ime, xml, "BLA-34", "CLEARCOLUMS detaila sakrije stupce koji nisu u DETAILCOLUMNS", "netgen/Form1.vb:2347,7459", "detClear", "true");
            Stablo.M(l, Ime, xml, "BLA-35", "Sirina VrstaBlagajne je 2500/14 = 179", "netgen/Form1.vb:7522", "kolona.VrstaBlagajne.sirina", "179");
            return l;
        }
    }

    public static class PrijemniList
    {
        public const string Ime = "PrijemniList";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "PrijemniList.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "PL-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "PL-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "PL-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(string.IsNullOrEmpty(p.Tree)
                ? Qa.Z(Ime, "PL-02", "Nema atributa TREE. Lijevo stablo dolazi iz COMBO", "netgen/Form1.vb:3387,7730-7733")
                : Qa.P(Ime, "PL-02", "Nema TREE", "netgen/Form1.vb:3387", p.Tree));
            string combo = (Stablo.Atribut(xml, "COMBO") ?? "").Trim();
            l.Add(combo.StartsWith("exec spNg_combo", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "PL-03", "COMBO je exec spNg_combo Drvo_NabPri. Stupac 0 je vrijednost (TREE tekst), stupac 1 natpis", "netgen/Form1.vb:3340-3344")
                : Qa.P(Ime, "PL-03", "COMBO", "netgen/Form1.vb:3340", combo));
            string det = (Stablo.Atribut(xml, "DETAILGRID") ?? "").Trim();
            l.Add(Qa.Jednako(Ime, "PL-04", "DETAILGRID je batch declare + exec, @@id", "netgen/Form1.vb:3373,2304", det, p.DetailGrid ?? ""));
            l.Add(det.StartsWith("declare", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "PL-05", "Detail ne pocinje s ':' pa se ne javlja sorkac, ide u PokaziUpitDetalj", "netgen/Form1.vb:2105-2109")
                : Qa.P(Ime, "PL-05", "Detail nije :", "netgen/Form1.vb:2105", det));
            l.Add(Qa.Jednako(Ime, "PL-06", "SPLITTERDISTANCE je 180", "netgen/Form1.vb:3394,4323-4324", "180", (p.Atributi.Vrijednost("SPLITTERDISTANCE") ?? "").Trim()));
            string dbl = p.MasterDblClick ?? "";
            l.Add(dbl.StartsWith(":IzmijeniPL", StringComparison.Ordinal) && dbl.IndexOf("REFRESHMASTER", StringComparison.Ordinal) >= 0 && dbl.IndexOf("REFRESHMASTERGRID", StringComparison.Ordinal) < 0
                ? Qa.Z(Ime, "PL-07", "MASTERDBLCLICK je :IzmijeniPL#REFRESHMASTER. REFRESHMASTER nije REFRESHMASTERGRID", "netgen/Form1.vb:4570,6935")
                : Qa.P(Ime, "PL-07", "MASTERDBLCLICK", "netgen/Form1.vb:3389", dbl));
            l.Add(p.Gumbi.Count >= 2 && p.Gumbi[1].Naredba.IndexOf("REFRESHMASTER", StringComparison.Ordinal) >= 0 && p.Gumbi[1].Naredba.IndexOf("REFRESHMASTERGRID", StringComparison.Ordinal) < 0
                ? Qa.Z(Ime, "PL-08", "CMD2 naredba nosi #REFRESHMASTER, parser ne lomi hash", "netgen/Form1.vb:3326,4560")
                : Qa.P(Ime, "PL-08", "CMD2", "netgen/Form1.vb:3315", p.Gumbi.Count < 2 ? "" : p.Gumbi[1].Naredba));
            l.Add(string.Equals(p.Atributi.Vrijednost("GROUPBYBOXVISIBLE"), "TRUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "PL-09", "GROUPBYBOXVISIBLE TRUE ostaje ukljucen", "netgen/Form1.vb:3350")
                : Qa.P(Ime, "PL-09", "GROUPBYBOX", "netgen/Form1.vb:3350", p.Atributi.Vrijednost("GROUPBYBOXVISIBLE")));
            var detKol = p.NadjiGrupu("DETAILCOLUMNS");
            l.Add(detKol != null && detKol.Kolone.Count > 0 && detKol.Kolone[0].Name == "Kolicina" && detKol.Kolone[0].Atributi.Vrijednost("WIDTH") == "1000"
                ? Qa.Z(Ime, "PL-10", "Prva detail kolona Kolicina, WIDTH 1000, dvo-G agregat se ne veze", "netgen/Form1.vb:2347,7496")
                : Qa.P(Ime, "PL-10", "DETAILCOLUMNS", "netgen/Form1.vb:2347", detKol == null ? "" : detKol.Kolone[0].Name));
            l.Add(Stablo.Meni(Ime, "PL-11", "PrijemniList.xml"));

            Stablo.R(l, Ime, xml, "PL-20", "COMBO drzi stablo vidljivim iako TREE atribut ne postoji", "netgen/Form1.vb:4310-4320,7730-7733", "stabloVidljivo", "true");
            Stablo.R(l, Ime, xml, "PL-21", "Combo je vidljiv", "netgen/Form1.vb:3340-3347", "comboVidljiv", "true");
            Stablo.R(l, Ime, xml, "PL-22", "Splitter 180 tek kad TREE tekst postoji (nakon SelectedValue)", "netgen/Form1.vb:4323-4324", "lijeviSplitter", "180");
            Stablo.M(l, Ime, xml, "PL-23", "Detail SQL je declare/exec", "netgen/Form1.vb:3373", "detailSql", det);
            Stablo.M(l, Ime, xml, "PL-24", "CHECKBOXVISIBLE FALSE", "netgen/Form1.vb:2043", "selectorVidljiv", "false");
            Stablo.R(l, Ime, xml, "PL-25", "Nema ONLOAD: pocetni master SQL je prazan", "netgen/Form1.vb:3367-3368,1830", "sqlTrag", "");
            Stablo.K(l, Ime, xml, "PL-30", "SelectedValueChanged upise SelectedValue u strTreeNodes i ponovo zove NapuniDrvoVrijeme", "netgen/Form1.vb:7730-7733", "comboPuniDrvo", "true");
            Stablo.K(l, Ime, xml, "PL-31", "Dok je TREE jos prazan, NapuniDrvo izadje prije splitter-a i prije Enable gumba", "netgen/Form1.vb:4310-4320", "comboPrviProlaz", "izlaz");
            Stablo.K(l, Ime, xml, "PL-32", "Vrijednost combo stupca 0 mora biti TREE tekst (segmenti s # i :)", "netgen/Form1.vb:3342-3344,4344", "comboStupac0", "tree");
            Stablo.M(l, Ime, xml, "PL-33", "Detail se veze na trenutni @@id nakon master reda", "netgen/Form1.vb:2294-2304,7413", "detailNaSelekciji", "true");
            Stablo.M(l, Ime, xml, "PL-34", "Kolicina na detailu: sirina 1000/14 = 71, format #,####0.0000", "netgen/Form1.vb:7522-7524", "det.Kolicina", "71|#,####0.0000");
            Stablo.K(l, Ime, xml, "PL-35", "REFRESHMASTER na dvostrukom kliku ne osvjezava preko rijeci REFRESHMASTERGRID", "netgen/Form1.vb:4570", "dblRefresh", "nije-grid");
            return l;
        }
    }
}
