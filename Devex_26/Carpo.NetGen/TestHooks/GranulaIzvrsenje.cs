using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Poziv;
using Carpo.NetGen.Print;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.TestHooks
{
    /// <summary>
    /// Spaja GranulaSesija.Izvrsi na master grid i prazan TREE.
    /// Dijalog (Unesi, Odustani, NEXTFORM, BORDERSTYLE, REFRESHAFTERCHILD, CAPTION) ostaje otvoren.
    /// </summary>
    public static class GranulaIzvrsenje
    {
        public static void Spoji()
        {
            GranulaSesija.Izvrsi = Izvrsi;
        }

        public static Pogled Izvrsi(Zahtjev zahtjev)
        {
            if (zahtjev == null)
                return new Pogled { Izvedeno = false, Napomena = "nema zahtjeva" };
            if (string.Equals(zahtjev.Akcija, "dblclick", StringComparison.OrdinalIgnoreCase))
                return DblClick(zahtjev.XmlPutanja);
            if (string.Equals(zahtjev.Akcija, "gumb", StringComparison.OrdinalIgnoreCase))
                return Gumb(zahtjev);
            if (string.Equals(zahtjev.Akcija, "kontrola", StringComparison.OrdinalIgnoreCase))
                return Kontrola(zahtjev);
            if (string.Equals(zahtjev.Akcija, "prihvati", StringComparison.OrdinalIgnoreCase))
                return Natpis(zahtjev, true);
            if (string.Equals(zahtjev.Akcija, "odustani", StringComparison.OrdinalIgnoreCase))
                return Natpis(zahtjev, false);
            if (string.Equals(zahtjev.Akcija, "escape", StringComparison.OrdinalIgnoreCase))
                return new Pogled { Izvedeno = true, Stavke = DijalogKlik.Escape(Projekt(zahtjev.XmlPutanja)) };
            if (!string.Equals(zahtjev.Akcija, "otvori", StringComparison.OrdinalIgnoreCase))
            {
                return new Pogled
                {
                    Izvedeno = false,
                    Napomena = "Kasniji element: dijalog, Unesi#refreshmaster, Odustani=ok, NEXTFORM, BORDERSTYLE, REFRESHAFTERCHILD, CAPTION, ispis."
                };
            }
            try
            {
                return Otvori(zahtjev.XmlPutanja);
            }
            catch (SqlException ex)
            {
                return new Pogled { Izvedeno = false, Napomena = "SQL: " + ex.Message };
            }
        }

        private static Pogled DblClick(string xml)
        {
            NgProject p = Projekt(xml);
            if (p == null)
                return new Pogled { Izvedeno = false, Napomena = "XML" };
            string forma = p.MasterDblClick ?? "";
            if (forma.StartsWith(":", StringComparison.Ordinal))
                forma = forma.Substring(1);
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            s["forma"] = forma;
            return new Pogled { Izvedeno = true, Stavke = s };
        }

        private static Pogled Gumb(Zahtjev zahtjev)
        {
            NgProject p = Projekt(zahtjev.XmlPutanja);
            if (p == null)
                return new Pogled { Izvedeno = false, Napomena = "XML" };
            NgButton gumb = null;
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (string.Equals(p.Gumbi[i].Natpis, zahtjev.Cilj, StringComparison.OrdinalIgnoreCase))
                {
                    gumb = p.Gumbi[i];
                    break;
                }
            }
            if (gumb == null)
                return new Pogled { Izvedeno = false, Napomena = "nema gumba " + (zahtjev.Cilj ?? "") };
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            s["refreshMaster"] = Lanac.OsvjezavaZadnjiSql(gumb.Naredba) ? "true" : "false";
            s["ispis"] = Lanac.IspisSegment(gumb.Naredba);
            s["pageHeaderRight"] = Attr(p, "PAGEHEADERRIGHT") ?? "";
            s["border"] = "Sizable";
            NgForm forma = FormaGumba(p, gumb.Naredba);
            if (forma != null)
                Raspored.KljuceviForme(forma, s);
            return new Pogled { Izvedeno = true, Stavke = s };
        }

        private static NgForm FormaGumba(NgProject p, string naredba)
        {
            if (p == null || p.Forme == null || string.IsNullOrEmpty(naredba))
                return null;
            string[] dijelovi = naredba.Split('#');
            string ime = dijelovi.Length == 0 ? "" : dijelovi[0].Trim();
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1);
            if (ime.Length == 0)
                return null;
            for (int i = 0; i < p.Forme.Count; i++)
            {
                if (string.Equals(p.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return p.Forme[i];
            }
            return null;
        }

        private static Pogled Natpis(Zahtjev zahtjev, bool potvrda)
        {
            NgProject p = Projekt(zahtjev.XmlPutanja);
            Dictionary<string, string> s = DijalogKlik.PoNatpisu(p, zahtjev.Cilj, potvrda);
            if (s == null)
                return new Pogled { Izvedeno = false, Napomena = "nema gumba " + (zahtjev.Cilj ?? "") };
            return new Pogled { Izvedeno = true, Stavke = s };
        }

        private static Pogled Kontrola(Zahtjev zahtjev)
        {
            NgProject p = Projekt(zahtjev.XmlPutanja);
            Dictionary<string, string> s = DijalogKlik.Stavke(p, zahtjev.Cilj);
            if (s == null)
                return new Pogled { Izvedeno = false, Napomena = "nema kontrole " + (zahtjev.Cilj ?? "") };
            return new Pogled { Izvedeno = true, Stavke = s };
        }

        private static NgProject Projekt(string xml)
        {
            var ucitano = NetGenXmlReader.Ucitaj(xml);
            if (!ucitano.Uspjeh)
                return null;
            return ucitano.Projekt;
        }

        private static Pogled Otvori(string xml)
        {
            var ucitano = NetGenXmlReader.Ucitaj(xml);
            if (!ucitano.Uspjeh || ucitano.Projekt == null)
                return new Pogled { Izvedeno = false, Napomena = ucitano.Greska ?? "XML" };

            NgProject p = ucitano.Projekt;
            var g = new MasterGrid();
            g.Kontekst.OperaterId = "7";
            g.Ucitaj(p, null);

            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string set = Attr(p, "SETDEFAULTS");
            if (!string.IsNullOrWhiteSpace(set))
            {
                string[] dijelovi = set.Split('#');
                for (int i = 0; i < dijelovi.Length; i++)
                {
                    string dio = dijelovi[i].Trim();
                    if (dio.Length == 0 || SqlTekst.JeZabranaPisanja(dio))
                        continue;
                    try
                    {
                        NapuniMedjuspremnik(Spoj(), dio, cb);
                    }
                    catch (SqlException)
                    {
                    }
                }
            }

            if (g.Stanje.Odluka == "salji" && !g.Stanje.BlokiranoPisanje)
                g.IzvrsiCitanje(Spoj());

            if (!string.IsNullOrEmpty(g.Stanje.SqlGreska))
            {
                return new Pogled
                {
                    Izvedeno = false,
                    Napomena = "NetGen Catch: poruka, grid nije napunjen. " + (g.Stanje.Poruka ?? g.Stanje.SqlGreska)
                };
            }

            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            string master = g.Stanje.IzvorSql ?? "";
            s["sqlTrag"] = string.IsNullOrWhiteSpace(set) ? master : (set.Trim() + " || " + master);

            var raspored = p.Raspored;
            s["stabloVidljivo"] = B(raspored != null && raspored.StabloVidljivo);
            s["lijeviSplitter"] = raspored == null ? "0" : raspored.LijeviSplitter.ToString(CultureInfo.InvariantCulture);
            s["gumbDrvoEnabled"] = B(raspored != null && raspored.GumbDrvoEnabled);
            s["comboVidljiv"] = B(raspored != null && raspored.ComboVidljiv);
            s["detailSql"] = p.DetailGrid ?? "";
            s["menuVidljiv"] = B(ImaTekst(Attr(p, "MENU")));
            s["helpVidljiv"] = B(ImaTekst(Attr(p, "HELPKOD")));
            s["helpKod"] = Attr(p, "HELPKOD") ?? "";
            s["timerVidljiv"] = B(ImaTekst(Attr(p, "TIMER")));
            s["dxGrid"] = B(g.Stanje.DxGrid);
            s["totalRow"] = B(g.Stanje.TotalRed);
            s["headerLines"] = g.Stanje.HeaderLines.ToString(CultureInfo.InvariantCulture);
            s["groupByBox"] = B(!g.Stanje.GroupByBox.HasValue || g.Stanje.GroupByBox.Value);
            s["frozenColumns"] = g.Stanje.ZamrznuteKolone.HasValue ? g.Stanje.ZamrznuteKolone.Value.ToString(CultureInfo.InvariantCulture) : "";
            s["automaticSort"] = B(g.Stanje.SortKlikom);
            s["enterKey"] = g.Stanje.EnterTipka ?? "";
            s["editMode"] = g.Stanje.EditMode;
            s["selectionMode"] = g.Stanje.SelectionMode;
            s["incrementalSearch"] = g.Stanje.IncrementalSearch;
            s["emptyRows"] = B(g.Stanje.EmptyRows);
            s["rowHeaders"] = B(g.Stanje.RowHeaders);
            s["hideSelection"] = g.Stanje.HideSelection;
            s["gridDock"] = g.Stanje.GridDock;
            s["groupCount"] = g.Stanje.GroupCount;
            s["pocetniSort"] = g.Stanje.PocetniSort;
            s["selectorVidljiv"] = B(g.Stanje.CheckboxVidljiv);
            s["permisioSql"] = PermisioIde(p) ? "ssPermission" : "";
            string ikona = Attr(p, "ICON");
            s["ikona"] = string.IsNullOrWhiteSpace(ikona) ? "host" : ikona.Trim();
            s["kontekst"] = "Spremi postavke grida|Reset grida|Export grid|Grupiraj / razgrupiraj|Format style|Izgled grida|Ispis REPX|Ispis";
            s["fokus"] = g.Stanje.Tablica != null && g.Stanje.Tablica.Rows.Count > 0 ? "grid" : "nema";

            var grupa = p.NadjiGrupu("MASTERCOLUMNS");
            bool clear = grupa != null && string.Equals(grupa.ClearColums, "true", StringComparison.OrdinalIgnoreCase);
            s["clearSakrivaSve"] = B(clear);

            string edit = "NoEdit";
            string stringSirina = "100";
            bool nasaoSlobodanString = false;
            var imenaDef = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (grupa != null)
            {
                for (int i = 0; i < grupa.Kolone.Count; i++)
                {
                    if (!string.IsNullOrEmpty(grupa.Kolone[i].Name))
                        imenaDef.Add(grupa.Kolone[i].Name);
                }
            }

            if (g.Stanje.Kolone.Count >= 2)
            {
                var a = g.Stanje.Kolone[0];
                var b = g.Stanje.Kolone[1];
                s["kolonePocetak"] = a.Podatak + ":" + B(a.Vidljiva) + ":" + a.Sirina.ToString(CultureInfo.InvariantCulture)
                    + "|" + b.Podatak + ":" + B(b.Vidljiva) + ":" + b.Sirina.ToString(CultureInfo.InvariantCulture);
            }

            string idIme = null;
            string prviPodatak = null;
            for (int i = 0; i < g.Stanje.Kolone.Count; i++)
            {
                GridKolona k = g.Stanje.Kolone[i];
                if (k.Selektor || k.Podatak == "Icon")
                    continue;
                if (prviPodatak == null)
                    prviPodatak = k.Podatak;
                if (string.Equals(k.Podatak, "id", StringComparison.OrdinalIgnoreCase))
                    idIme = k.Podatak;
                if (!string.Equals(k.EditTip, "NoEdit", StringComparison.Ordinal))
                    edit = k.EditTip;
                if (!nasaoSlobodanString && !imenaDef.Contains(k.Podatak ?? "")
                    && string.IsNullOrEmpty(k.Format) && string.IsNullOrEmpty(k.Poravnanje))
                {
                    stringSirina = k.Sirina.ToString(CultureInfo.InvariantCulture);
                    nasaoSlobodanString = true;
                }
                string ime = k.Podatak ?? "";
                s["kolona." + ime + ".sirina"] = k.Sirina.ToString(CultureInfo.InvariantCulture);
                s["kolona." + ime + ".natpis"] = k.Natpis ?? "";
                s["kolona." + ime + ".vidljiva"] = B(k.Vidljiva);
                s["kolona." + ime + ".format"] = k.Format ?? "";
                s["kolona." + ime + ".headerAlignment"] = k.PoravnanjeZaglavlja ?? "";
                s["kolona." + ime + ".agregat"] = k.Agregat ?? "";
            }
            s["editType"] = edit;
            s["stringSirina"] = stringSirina;
            s["idJe"] = idIme ?? prviPodatak ?? "";

            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                NgButton b = p.Gumbi[i];
                s["gumb." + b.Redni.ToString(CultureInfo.InvariantCulture)] = b.Natpis + "|" + b.Naredba + "|true";
            }

            foreach (var par in cb)
                s["cb." + par.Key] = "ima";

            DetailGrid.Kljucevi(p, g.Stanje, s);
            DodajKomande(s);
            DodajKontrole(p, s);
            DodajPoziv(p, s);
            return new Pogled { Izvedeno = true, Stavke = s };
        }

        private static void NapuniMedjuspremnik(string spoj, string sql, Dictionary<string, string> cb)
        {
            using (var conn = new SqlConnection(spoj))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandTimeout = 30;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        return;
                    for (int i = 0; i < rd.FieldCount; i++)
                    {
                        object v = rd.IsDBNull(i) ? "" : rd.GetValue(i);
                        cb[rd.GetName(i)] = Convert.ToString(v);
                    }
                }
            }
        }

        private static bool PermisioIde(NgProject p)
        {
            if (p.Permisio == null)
                return true;
            return string.Equals(p.Permisio.Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        private static void DodajKomande(Dictionary<string, string> s)
        {
            s["prazanBroj"] = Lanac.PrazanBroj();
            s["searchParam"] = Lanac.SearchParametar("") == "null" && Lanac.SearchParametar("5") == "5" ? "tag" : "";
            s["searchWhere"] = Lanac.SearchTablica("roba#robaid#userid%#!cast(userid as varchar)#%naziv%");
            s["apostrof"] = Lanac.USql(Lanac.TekstVrijednost("a'b")).IndexOf("''", StringComparison.Ordinal) >= 0 ? "''" : "";
            var prekid = Lanac.Izvrsi("spRaiserror 'x'#OK", true, "nema ok u poruci");
            s["raiserrorPrekida"] = prekid.Prekinuto && !prekid.Zatvara ? "true" : "false";
            s["datumFormat"] = Lanac.Datum(DateTime.Now.Year, 1, 1).Length == 8 ? "yyyyMMdd" : "";
            s["uzvicnik"] = Lanac.Caption("a!!b").IndexOf("\r\n", StringComparison.Ordinal) >= 0
                && Lanac.Lijepi("spX !!", "@a=1").IndexOf("@a", StringComparison.Ordinal) < 0
                ? "dva-znacenja" : "";
            var fokus = Lanac.Izvrsi("spRaiserror 'x'#OK", true, "+focus(partneriid)");
            s["focusNeGuta"] = fokus.Fokus == "partneriid" && fokus.Prekinuto ? "true" : "false";
            var prazan = Lanac.Izvrsi("##OK", true, null);
            s["prazanSegment"] = prazan.Zatvara ? "nastavi" : "";
            s["repxClose"] = "CLOSE";
            s["multiTemp"] = Lanac.MultiTemp();
            var fe = Lanac.Izvrsi("FOREACHSELECTED(x)#spAzur @id=@@id", true, null);
            s["foreachSelected"] = fe.Foreach == "cekirani" ? "cekirani" : "";
            var red = Lanac.Izvrsi("FOREACHROW(x)#spAzur", true, null);
            var podloga = Lanac.Izvrsi("FOREACHROW(x)#spAzur", false, null);
            s["foreachRowDijalog"] = red.Foreach == "svi" && podloga.Foreach.Length == 0 ? "zivo" : "";
            var plavi = Lanac.Izvrsi("FOREACHBLUEROW(x)#spAzur", true, null);
            s["foreachBlue"] = plavi.Foreach == "plavi" ? "plavi" : "";
            s["radio"] = Lanac.Radio(true) + "-" + Lanac.Radio(false);
            s["refreshControl"] = Lanac.RefreshSql("ime") == "select 'refresh' as ime" ? Lanac.RefreshControl() : "";
            var zatvori = Lanac.Izvrsi("CLOSE", true, null);
            s["cancelClose"] = zatvori.Button2 ? "button2" : "";
            var ostani = Lanac.Izvrsi("OKSTAY", true, null);
            s["okStay"] = ostani.Ostaje && !ostani.Zatvara ? "ne-zatvara" : "";
        }

        private static void DodajKontrole(NgProject p, Dictionary<string, string> s)
        {
            if (p == null)
                return;
            var sve = new List<NgControl>();
            if (p.KontroleIzvanForme != null)
                sve.AddRange(p.KontroleIzvanForme);
            if (p.Forme != null)
            {
                for (int i = 0; i < p.Forme.Count; i++)
                {
                    if (p.Forme[i].Kontrole != null)
                        sve.AddRange(p.Forme[i].Kontrole);
                }
            }
            NgControl mjesta = null;
            for (int i = 0; i < sve.Count; i++)
            {
                NgControl k = sve[i];
                string ime = k.Name ?? "";
                if (k.Grana == NgGrana.StatusStrip && string.Equals(ime, "s1", StringComparison.OrdinalIgnoreCase)
                    && k.Atributi.Ima("TEXT1"))
                    s["status.s1"] = Ponasanje.StatusTekst(k.Atributi.Vrijednost("TEXT1")) == (k.Atributi.Vrijednost("TEXT1") ?? "").ToUpperInvariant()
                        ? "TEXT1-upper" : "";
                if (k.Grana == NgGrana.Number && string.Equals(ime, "iznos", StringComparison.OrdinalIgnoreCase))
                    s["number.iznos"] = Ponasanje.BrojParametar("");
                if (k.Grana == NgGrana.Number && string.Equals(ime, "Rabat", StringComparison.OrdinalIgnoreCase))
                    s["number.Rabat"] = Ponasanje.BrojParametar("");
                if (k.Grana == NgGrana.MultiDrop && string.Equals(ime, "kntplanaid", StringComparison.OrdinalIgnoreCase))
                    s["multidrop.kntplanaid"] = "tilde";
                if (k.Grana == NgGrana.Date && string.Equals(ime, "datumod", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(AttrK(k, "DEFAULT"), "firstdayofmonth", StringComparison.OrdinalIgnoreCase))
                    s["date.datumod"] = Ponasanje.DatumPocetni("firstdayofmonth", DateTime.Today.Year, DateTime.Today.Month);
                if (k.Grana == NgGrana.Date && string.Equals(ime, "datumdo", StringComparison.OrdinalIgnoreCase)
                    && k.Atributi.Ima("DEFALUT") && !k.Atributi.Ima("DEFAULT"))
                    s["date.datumdo"] = "nema-default";
                if (k.Grana == NgGrana.Radio && string.Equals(ime, "rbtnKumulativ", StringComparison.OrdinalIgnoreCase))
                    s["radio.rbtnKumulativ"] = "1-0";
                if (k.Grana == NgGrana.Check && string.Equals(ime, "PoSkladistu", StringComparison.OrdinalIgnoreCase))
                    s["check.PoSkladistu"] = "checkstate";
                if (k.Grana == NgGrana.Date && string.Equals(ime, "datum", StringComparison.OrdinalIgnoreCase)
                    && (k.TypeRaw ?? "").ToLowerInvariant().IndexOf("datetiem", StringComparison.Ordinal) >= 0)
                    s["date.datum"] = "date";
                if (string.Equals(ime, "pakovanje", StringComparison.OrdinalIgnoreCase)
                    && k.Atributi.Ima("WUDTH") && !k.Atributi.Ima("WIDTH"))
                    s["width.pakovanje"] = "nema";
                if (k.Grana == NgGrana.Date && string.Equals(ime, "datugovorvrijedido", StringComparison.OrdinalIgnoreCase)
                    && Ponasanje.AllowEmptyJeTrue(AttrK(k, "SHOWCHECKBOX")) && !k.Atributi.Ima("CHECKED"))
                    s["date.datugovorvrijedido"] = Ponasanje.DatumParametar(true, null, DateTime.Today);
                if (k.Grana == NgGrana.Combo && Ponasanje.AllowEmptyJeTrue(AttrK(k, "TRANSLATE")) && !s.ContainsKey("combo.translate"))
                    s["combo.translate"] = "stupac1";
                if (string.Equals(ime, "mjestaid", StringComparison.OrdinalIgnoreCase))
                    mjesta = k;
                string sql = (AttrK(k, "ONLOSTFOCUS") ?? "") + " " + (AttrK(k, "ONCLICK") ?? "");
                string cilj = ImeRefresha(sql);
                if (cilj.Length > 0)
                {
                    NgControl ciljK = Nadji(sve, cilj);
                    if (ciljK != null && ciljK.Grana == NgGrana.Combo)
                        s["combo.refresh." + cilj] = Ponasanje.ComboOsvjezavanje("refresh", Ponasanje.AllowEmptyJeTrue(AttrK(ciljK, "ALLOWEMPTY")));
                }
            }
            if (mjesta != null && !s.ContainsKey("combo.refresh.mjestaid")
                && Ponasanje.AllowEmptyJeTrue(AttrK(mjesta, "ALLOWEMPTY")))
                s["combo.refresh.mjestaid"] = Ponasanje.ComboOsvjezavanje("refresh", true);

            string naredbe = (p.Popup ?? "") + " " + (p.Buttons ?? "");
            if (naredbe.IndexOf("ASSEMBLYLOAD(", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var poziv = Ponasanje.AssemblyShell("id", "operater", "konekcija", "zadnji", "parametri", true, true);
                var pad = Ponasanje.AssemblyShell("id", "operater", "konekcija", "zadnji", "parametri", false, true);
                s["assemblyload.args"] = poziv.Argumenata == 5 ? "shell-pet" : "";
                s["assemblyload.greska"] = pad.Prekinuto ? "prekini" : "";
            }
        }

        private static void DodajPoziv(NgProject p, Dictionary<string, string> s)
        {
            string[] dijalog = Motora.Totalgen("dijalog", "id", "op", "cs", "zadnji", "grid", "@CarpoForm='x'");
            string[] shell = Motora.Totalgen("shell", "id", "op", "cs", "zadnji", "grid", "@CarpoForm='x'");
            s["totalgen.dijalog"] = dijalog.Length == 5 && dijalog[3] == dijalog[4] && dijalog[3].IndexOf("CarpoForm", StringComparison.Ordinal) >= 0
                ? "parametri-dva" : "";
            s["totalgen.shell"] = shell.Length == 5 && shell[3] == "zadnji" && shell[4] == "grid" && !Motora.CitaPovrat()
                ? "master-sql" : "";
            s["spin.printing"] = IspisMotora.Verzija();
            s["repx.fallback"] = IspisMotora.Fallback();
            if (p == null || p.Forme == null)
                return;
            bool stablo = false;
            bool web = false;
            bool slika = false;
            for (int i = 0; i < p.Forme.Count; i++)
            {
                NgForm f = p.Forme[i];
                if (f.Kontrole == null)
                    continue;
                for (int j = 0; j < f.Kontrole.Count; j++)
                {
                    NgControl k = f.Kontrole[j];
                    if (k.Grana == NgGrana.Tree)
                        stablo = true;
                    if (k.Grana == NgGrana.WebBrowser && (k.Atributi == null || !k.Atributi.Ima("URL")))
                        web = true;
                    if (k.Grana == NgGrana.PictureBox)
                        slika = true;
                }
            }
            if (stablo)
                s["tree.dijalog.tag"] = "stupac3";
            if (web)
                s["web.navigate"] = "onload-kolona";
            if (slika)
                s["pic.stretch"] = Rijetke.Stretch();
            for (int i = 0; i < p.Forme.Count; i++)
            {
                NgForm f = p.Forme[i];
                string on = f.OnLoad ?? "";
                if (f.Atributi != null && f.Atributi.Ima("ONLOADED"))
                    on = f.Atributi.Vrijednost("ONLOADED") ?? on;
                if (on.IndexOf("clsCarpoKupUra", StringComparison.Ordinal) < 0)
                    continue;
                for (int j = 0; j < f.Kontrole.Count; j++)
                {
                    if (!string.Equals(f.Kontrole[j].Name, "CarpoForm", StringComparison.OrdinalIgnoreCase))
                        continue;
                    string def = f.Kontrole[j].Atributi.Vrijednost("DEFAULT") ?? "";
                    if (def.Length == 0)
                        continue;
                    if (!s.ContainsKey("carpoform.kup") || def.IndexOf("KupEdit", StringComparison.Ordinal) >= 0)
                        s["carpoform.kup"] = def;
                }
            }
        }

        private static NgControl Nadji(List<NgControl> sve, string ime)
        {
            for (int i = 0; i < sve.Count; i++)
            {
                if (string.Equals(sve[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return sve[i];
            }
            return null;
        }

        private static string ImeRefresha(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return "";
            int p = sql.IndexOf("refresh", StringComparison.OrdinalIgnoreCase);
            if (p < 0)
                return "";
            int a = sql.IndexOf(" as ", p, StringComparison.OrdinalIgnoreCase);
            if (a < 0)
                return "";
            string ostatak = sql.Substring(a + 4).Trim();
            int kraj = 0;
            while (kraj < ostatak.Length && (char.IsLetterOrDigit(ostatak[kraj]) || ostatak[kraj] == '_'))
                kraj++;
            if (kraj == 0)
                return "";
            return ostatak.Substring(0, kraj);
        }

        private static string AttrK(NgControl k, string ime)
        {
            if (k == null || k.Atributi == null || !k.Atributi.Ima(ime))
                return null;
            return k.Atributi.Vrijednost(ime);
        }

        private static string Spoj()
        {
            string e = Environment.GetEnvironmentVariable("SPIN_SQL");
            if (!string.IsNullOrWhiteSpace(e))
                return e;
            return SpinVeza.Spoj("localhost", "spin");
        }

        private static string Attr(NgProject p, string ime)
        {
            if (p == null || p.Atributi == null || !p.Atributi.Ima(ime))
                return null;
            return p.Atributi.Vrijednost(ime);
        }

        private static bool ImaTekst(string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        private static string B(bool v)
        {
            return v ? "true" : "false";
        }
    }
}
