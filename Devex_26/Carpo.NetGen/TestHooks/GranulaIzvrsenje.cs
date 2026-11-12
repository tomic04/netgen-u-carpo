using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
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
            if (zahtjev == null || !string.Equals(zahtjev.Akcija, "otvori", StringComparison.OrdinalIgnoreCase))
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

            DodajKomande(s);
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
            s["refreshControl"] = Lanac.RefreshControl();
            var zatvori = Lanac.Izvrsi("CLOSE", true, null);
            s["cancelClose"] = zatvori.Button2 ? "button2" : "";
            var ostani = Lanac.Izvrsi("OKSTAY", true, null);
            s["okStay"] = ostani.Ostaje && !ostani.Zatvara ? "ne-zatvara" : "";
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
