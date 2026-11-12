using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.GridTest
{
    internal static class Program
    {
        private static int fail;

        private static int Main(string[] args)
        {
            SqlPreskoci();
            Zamjene();
            OznakaBezBaze();
            JeziciBezBaze();
            Kolone();
            PisanjeIServer();
            StabloTestovi();
            KomandeTestovi();
            if (Environment.GetEnvironmentVariable("CARPO_SPIN") == "1")
            {
                Spin();
                SpinDetalj();
                SpinStabloFill();
            }
            else
                Console.WriteLine("PRESKOCENO spin (nema CARPO_SPIN=1)");
            Console.WriteLine("FAIL=" + fail.ToString());
            return fail == 0 ? 0 : 1;
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
            T("foreach-podloga", podloga.Foreach.Length == 0 && podloga.Odigrano.Count == 0, "");
            T("foreach-plavi", Lanac.Izvrsi("FOREACHBLUEROW(x)#spAzur", true, null).Foreach == "plavi", "");
            T("refresh-sql", Lanac.RefreshSql("ime") == "select 'refresh' as ime" && Lanac.RefreshControl() == "select refresh", "");
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
            T("det-zamjena", detSql.IndexOf("@id=9", StringComparison.Ordinal) >= 0 && detSql.IndexOf("@naziv='Kruh'", StringComparison.Ordinal) >= 0, detSql);
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
}
