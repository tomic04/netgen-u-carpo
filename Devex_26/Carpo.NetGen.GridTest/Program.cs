using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Carpo.NetGen.Grid;
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
            if (Environment.GetEnvironmentVariable("CARPO_SPIN") == "1")
                Spin();
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
