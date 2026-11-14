using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Ui;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.GridTest
{
    /// <summary>
    /// Neovisni pregled 14. 11. 2026. (docs/pregled/pregled-neovisni-2026-11-14.md): testovi učinka za tok
    /// ONSUBMIT-a nakon dijaloga (PN-01, PN-02, PN-05, PN-07, PN-12), svaki uz VB retke koje provjerava.
    /// Stanje prije ispravka je u PR #133 (run 43605973837).
    /// Spin: tablica dbo.PregledNeovisniLog i procedura dbo.spPregledNeovisni broje koliko je puta ONSUBMIT
    /// stvarno izvršen i s kojim parametrima.
    /// </summary>
    internal static class PregledNeovisni
    {
        private static Action<string, bool, string> _t;

        private static void T(string id, bool ok, string detalj)
        {
            _t(id, ok, detalj);
        }

        private static volatile bool _cuvar;
        private static volatile string _korak = "";

        public static void Izvrsi(Action<string, bool, string> t)
        {
            _t = t;
            // Podloga u GridTestu nema Carpo Logger, pa JupiterShared_Shown (Events.CreateEvents) baci iznimku na UI
            // niti; bez ovoga bi ThreadExceptionDialog zaustavio test (vidi i PodlogaRafalno).
            ThreadExceptionEventHandler iznimka = delegate (object s, ThreadExceptionEventArgs e)
            {
                Console.WriteLine("pn: iznimka na UI niti u koraku " + _korak + ": " + e.Exception.GetType().Name + " " + e.Exception.Message);
            };
            Application.ThreadException += iznimka;
            _cuvar = true;
            var cuvar = new Thread(Cuvar);
            cuvar.IsBackground = true;
            cuvar.Start();
            try
            {
                try
                {
                    PripremiSpin();
                }
                catch (Exception ex)
                {
                    T("pn-spin-priprema", false, ex.Message);
                    return;
                }
                Korak("onsubmit-jednom", OnSubmitJednom);
                Korak("fillexplorer-parametri", FillExplorerParametri);
                Korak("nextform-id", NextFormId);
                Korak("odustani-nije-ok", OdustaniNijeOk);
                Korak("stablo-nakon-dijaloga", StabloNakonDijaloga);
                Korak("forma-parametri", FormaParametri);
                Korak("onload-jedna-rijec", OnLoadJednaRijec);
            }
            finally
            {
                _cuvar = false;
                Application.ThreadException -= iznimka;
            }
        }

        private static void Korak(string ime, Action radnja)
        {
            _korak = ime;
            Console.WriteLine("pn: " + ime);
            radnja();
        }

        /// <summary>Ako korak stoji dulje od 90 s, ispiše otvorene forme i zatvori dijaloge, da CI ne visi.</summary>
        private static void Cuvar()
        {
            string zadnji = _korak;
            int sekundi = 0;
            while (_cuvar)
            {
                Thread.Sleep(1000);
                if (_korak != zadnji)
                {
                    zadnji = _korak;
                    sekundi = 0;
                    continue;
                }
                sekundi++;
                if (sekundi < 90)
                    continue;
                sekundi = 0;
                Console.WriteLine("pn: korak " + _korak + " stoji 90 s, otvorene forme:");
                try
                {
                    var forme = new List<Form>();
                    foreach (Form f in Application.OpenForms)
                        forme.Add(f);
                    foreach (Form f in forme)
                    {
                        Console.WriteLine("pn:   " + f.GetType().Name + " '" + f.Text + "' modal=" + f.Modal);
                        if (f is Dijalog && !f.IsDisposed)
                        {
                            Form z = f;
                            z.BeginInvoke(new Action(delegate () { z.DialogResult = DialogResult.Cancel; z.Close(); }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("pn:   " + ex.Message);
                }
            }
        }

        private static string Spoj()
        {
            return SpinVeza.Spoj("localhost", "spin");
        }

        // ------------------------------------------------------------------ spin pomoćni objekti

        private static void PripremiSpin()
        {
            using (var c = new SqlConnection(Spoj()))
            {
                c.Open();
                Izvrsi(c, "if object_id('dbo.PregledNeovisniLog') is null create table dbo.PregledNeovisniLog (kad datetime not null default getdate(), oznaka varchar(50) not null, t varchar(100) null)");
                Izvrsi(c, "if object_id('dbo.spPregledNeovisni') is not null drop procedure dbo.spPregledNeovisni");
                Izvrsi(c, "create procedure dbo.spPregledNeovisni @k varchar(50), @t varchar(100) = null as\r\n"
                    + "insert into dbo.PregledNeovisniLog (oznaka, t) values (@k, @t);\r\n"
                    + "select 1 as id, @k as oznaka, isnull(@t, '') as t");
                Izvrsi(c, "if object_id('dbo.spPregledNeovisniTree') is not null drop procedure dbo.spPregledNeovisniTree");
                Izvrsi(c, "create procedure dbo.spPregledNeovisniTree @k varchar(50) = null as\r\n"
                    + "if @k is not null insert into dbo.PregledNeovisniLog (oznaka, t) values (@k, 'stablo');\r\n"
                    + "select 1 as id, 0 as roditelj, 'Korijen' as natpis");
                Izvrsi(c, "delete from dbo.PregledNeovisniLog");
            }
        }

        private static void Izvrsi(SqlConnection c, string sql)
        {
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.CommandTimeout = 30;
                cmd.ExecuteNonQuery();
            }
        }

        private static DataTable Zapisi(string oznaka)
        {
            var dt = new DataTable();
            using (var c = new SqlConnection(Spoj()))
            using (var cmd = new SqlCommand("select t from dbo.PregledNeovisniLog where oznaka = @o order by kad", c))
            {
                cmd.Parameters.AddWithValue("@o", oznaka);
                c.Open();
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            return dt;
        }

        private static string Xml(string ime, string sadrzaj)
        {
            string dir = Path.Combine(Path.GetTempPath(), "pn" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            string put = Path.Combine(dir, ime);
            File.WriteAllText(put, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + sadrzaj, new UTF8Encoding(false));
            return put;
        }

        private static Podloga OtvoriPodlogu(string xml, out bool otvoren)
        {
            var forma = new Podloga();
            forma.ShowInTaskbar = false;
            forma.StartPosition = FormStartPosition.Manual;
            forma.Location = new Point(-32000, -32000);
            forma.Show();
            otvoren = forma.Prikazi(xml, "localhost", "spin", 7);
            // Shown Managera (bez Carpo Loggera baci iznimku, uhvaćenu gore) prođe prije samog testa.
            Application.DoEvents();
            return forma;
        }

        private static object Polje(object o, string ime)
        {
            for (Type t = o.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo f = t.GetField(ime, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (f != null)
                    return f.GetValue(o);
            }
            return null;
        }

        private static SimpleButton GumbPodloge(Podloga forma, string natpis)
        {
            var ploca = Polje(forma, "pnlButtons") as Control;
            if (ploca == null)
                return null;
            foreach (Control c in ploca.Controls)
            {
                var b = c as SimpleButton;
                if (b != null && string.Equals((b.Text ?? "").Trim(), natpis, StringComparison.OrdinalIgnoreCase))
                    return b;
            }
            return null;
        }

        private static void KlikniCmd(Dijalog d, int redni)
        {
            Control[] c = d.Controls.Find("cmd" + redni.ToString(), true);
            if (c.Length > 0)
                ((SimpleButton)c[0]).PerformClick();
            else
            {
                d.DialogResult = DialogResult.Cancel;
                d.Close();
            }
        }

        private static void Zatvori(Podloga forma)
        {
            try
            {
                forma.Close();
                forma.Dispose();
            }
            catch
            {
            }
        }

        // ------------------------------------------------------------------ N1

        /// <summary>
        /// Form2.Button1_Click 3367-3388 samo pripremi strStora; Form1 5445-5453 (FILLEXPLORER=true) izvrši
        /// ONSUBMIT JEDNOM kroz PokaziUpit, i to je rezultat mastera. Carpo: Dijalog.GumbKlik (1340) pošalje
        /// ONSUBMIT, pa Podloga.OtvoriFormu (542) isti ONSUBMIT pošalje još jednom da napuni master. Procedura
        /// koja piše (spRedokPotvrdeAzur, spmpCopyDok, spKntKompVerizneAzur @action=0...) piše dvaput.
        /// </summary>
        private static void OnSubmitJednom()
        {
            string xml = Xml("PnJednom.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Filter:FILT\">\r\n"
                + "<NAME>PnJednom</NAME>\r\n<CAPTION>Pregled jednom</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\" FILLEXPLORER=\"TRUE\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledNeovisni @k='jednom', @t='fiksno' !!\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</FILT>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Filter");
                if (!otvoren || gumb == null)
                {
                    T("pn-onsubmit-jednom", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                Dijalog.NakonPrikaza = delegate (Dijalog d) { KlikniCmd(d, 1); };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                DataTable z = Zapisi("jednom");
                var gv = Polje(forma, "gvGla") as GridView;
                int redova = gv == null ? -1 : gv.DataRowCount;
                // !! na kraju: strParametri se ne lijepi (Form1 5447), @t ostaje 'fiksno'.
                string t = z.Rows.Count == 1 ? Convert.ToString(z.Rows[0][0]) : "";
                T("pn-onsubmit-jednom", z.Rows.Count == 1 && redova == 1 && t == "fiksno",
                    "ONSUBMIT izvršen " + z.Rows.Count.ToString() + " puta (NetGen 1), t='" + t + "', master redova=" + redova.ToString());
            }
            catch (Exception ex)
            {
                T("pn-onsubmit-jednom", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ N2

        /// <summary>
        /// Form1 5450: PokaziUpit(SrediSQLPosljeNextForme(SrediSQL(cf.strStora &amp; " " &amp; cf.strParametri))):
        /// ONSUBMIT koji završava zarezom (390 formi FILLEXPLORER=TRUE u korpusu, npr. spKntPreknjizavanjeNG
        /// @action=1,) dobije parametre dijaloga i puni master. Podloga.OtvoriFormu 542-545 šalje ONSUBMIT bez
        /// parametara i sa zarezom, pa master dobije grešku sintakse ili proceduru bez filtera.
        /// </summary>
        private static void FillExplorerParametri()
        {
            string xml = Xml("PnZarez.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Filter:FILT\">\r\n"
                + "<NAME>PnZarez</NAME>\r\n<CAPTION>Pregled zarez</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\" FILLEXPLORER=\"TRUE\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledNeovisni @k='zarez',\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</FILT>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Filter");
                if (!otvoren || gumb == null)
                {
                    T("pn-fillexplorer-parametri", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                Dijalog.NakonPrikaza = delegate (Dijalog d) { KlikniCmd(d, 1); };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                var master = Polje(forma, "_master") as MasterGrid;
                var gv = Polje(forma, "gvGla") as GridView;
                string greska = master == null ? "" : (master.Stanje.SqlGreska ?? "");
                string sql = master == null ? "" : (master.Stanje.SqlZaSlanje ?? "");
                string t = "";
                if (gv != null && gv.DataRowCount > 0 && gv.Columns.ColumnByFieldName("t") != null)
                    t = Convert.ToString(gv.GetRowCellValue(0, "t"));
                T("pn-fillexplorer-parametri", greska.Length == 0 && t == "abc",
                    "master t='" + t + "' greska='" + greska + "' sql=" + sql);
            }
            catch (Exception ex)
            {
                T("pn-fillexplorer-parametri", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ NXT-01

        /// <summary>
        /// Form1 5381-5415 (NEXTFORM): Tid = prva ćelija rezultata ONSUBMIT-a (5390), PokaziUpit (5409), pa
        /// Form2c.ID = Tid (5414). Tako INSERTFORM → UPDATE_FORM (26 formi: MPIntDok, MPIzdVPMP, MPNarIzdSkl,
        /// KntKompVerizne...) otvori izmjenu novog dokumenta. Podloga.OtvoriFormu 566-568 otvara sljedeću formu s
        /// _master.Stanje.Id, a rezultat ONSUBMIT-a dijalog odbaci (Dijalog.cs 1345).
        /// </summary>
        private static void NextFormId()
        {
            string xml = Xml("PnNext.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Unesi:INS\">\r\n"
                + "<NAME>PnNext</NAME>\r\n<CAPTION>Pregled nextform</CAPTION>\r\n"
                + "<INS CAPTION=\"Unos\" NEXTFORM=\"UPD\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"select 777 as id\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</INS>\r\n"
                + "<UPD CAPTION=\"Izmjena\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"u\" DEFAULT=\"x\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</UPD>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Unesi");
                if (!otvoren || gumb == null)
                {
                    T("pn-nextform-id", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                var otvorene = new List<string>();
                Dijalog.NakonPrikaza = delegate (Dijalog d)
                {
                    otvorene.Add(d.Text + "/" + d.Id);
                    if (d.Text.StartsWith("Unos", StringComparison.Ordinal))
                        KlikniCmd(d, 1);
                    else
                        KlikniCmd(d, 2);
                };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                string niz = string.Join(",", otvorene.ToArray());
                T("pn-nextform-id", niz == "Unos/1,Izmjena/777", niz + " (NetGen: Unos/1,Izmjena/777)");
            }
            catch (Exception ex)
            {
                T("pn-nextform-id", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ PN-12

        /// <summary>
        /// Form2.Button2_Click 3395-3398: Odustani zatvara s OKClick = False, pa Form1 ne izvrši ONSUBMIT ni ne
        /// osvježi master. Carpo je za CMD2 bez naredbe davao DialogResult.OK (Dijalog.GumbKlik), pa je podloga nakon
        /// Odustani punila master ONSUBMIT-om.
        /// </summary>
        private static void OdustaniNijeOk()
        {
            string xml = Xml("PnOdustani.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Filter:FILT\">\r\n"
                + "<NAME>PnOdustani</NAME>\r\n<CAPTION>Pregled odustani</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\" FILLEXPLORER=\"TRUE\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledNeovisni @k='odustani', @t='x'\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</FILT>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Filter");
                if (!otvoren || gumb == null)
                {
                    T("pn-odustani-nije-ok", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                string ishod = "";
                Dijalog.NakonPrikaza = delegate (Dijalog d)
                {
                    d.FormClosed += delegate { ishod = d.DialogResult.ToString(); };
                    KlikniCmd(d, 2);
                };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                DataTable z = Zapisi("odustani");
                T("pn-odustani-nije-ok", z.Rows.Count == 0 && ishod == "Cancel",
                    "ONSUBMIT izvršen " + z.Rows.Count.ToString() + " puta (NetGen 0), DialogResult=" + ishod);
            }
            catch (Exception ex)
            {
                T("pn-odustani-nije-ok", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ PN-07

        /// <summary>
        /// Form1 5475-5486: FILLEXPLORER="treeandrefreshmaster" = NapuniDrvoVrijeme + ExecSql(ONSUBMIT) + zadnji
        /// master (75 formi u korpusu); "treeandmaster" 5487-5500 (8). Carpo je grane poznavao samo za true, onlydet
        /// i onlytree, pa se stablo nakon unosa nije osvježilo.
        /// </summary>
        private static void StabloNakonDijaloga()
        {
            string xml = Xml("PnStablo.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" TREE=\"dbo.spPregledNeovisniTree @k='stablo':FILT\" CMD1CAPTION=\"Unesi:UNOS\">\r\n"
                + "<NAME>PnStablo</NAME>\r\n<CAPTION>Pregled stablo</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\"><INPUT TYPE=\"text\" NAME=\"f\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" /></FILT>\r\n"
                + "<UNOS CAPTION=\"Unos\" FILLEXPLORER=\"treeandrefreshmaster\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledNeovisni @k='unos',\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</UNOS>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Unesi");
                int stabloPrije = Zapisi("stablo").Rows.Count;
                if (!otvoren || gumb == null)
                {
                    T("pn-stablo-nakon-dijaloga", false, "podloga se nije otvorila ili nema gumba (stablo=" + stabloPrije + ")");
                    return;
                }
                Dijalog.NakonPrikaza = delegate (Dijalog d) { KlikniCmd(d, 1); };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                int stabloPoslije = Zapisi("stablo").Rows.Count;
                DataTable unos = Zapisi("unos");
                string t = unos.Rows.Count == 1 ? Convert.ToString(unos.Rows[0][0]) : "";
                T("pn-stablo-nakon-dijaloga", stabloPrije == 1 && stabloPoslije == 2 && unos.Rows.Count == 1 && t == "abc",
                    "stablo puni " + stabloPrije + "->" + stabloPoslije + " (NetGen 1->2), ONSUBMIT " + unos.Rows.Count + "x t='" + t + "'");
            }
            catch (Exception ex)
            {
                T("pn-stablo-nakon-dijaloga", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }
        // ------------------------------------------------------------------ PN-14

        /// <summary>
        /// Form1.NovaGenerickaForma2 5298-5313: IMEFORME(@pero=@@id): parametar iz zagrade ide ispred strPrevParametri
        /// dijaloga, pa ONLOAD vidi @@pero (KOD_Diskete IZMJENA(@oopdiskprijenosglaid=@@id) → ONLOAD
        /// "select @@oopdiskprijenosglaid as ..."). Carpo je zagradu odbacivao.
        /// </summary>
        private static void FormaParametri()
        {
            string xml = Xml("PnParam.xml",
                "<PROJECT ONLOAD=\"select 7 as id, 'a' as naziv\" CMD1CAPTION=\"Izmjena:IZM(@kljuc=@@id,@drugi=@@naziv)\">\r\n"
                + "<NAME>PnParam</NAME>\r\n<CAPTION>Pregled parametri</CAPTION>\r\n"
                + "<IZM CAPTION=\"Izmjena\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONLOAD=\"select @@kljuc as k, '@@drugi' as d\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"k\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"d\" TOP=\"500\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</IZM>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Izmjena");
                if (!otvoren || gumb == null)
                {
                    T("pn-forma-parametri", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                string k = "", d = "";
                Dijalog.NakonPrikaza = delegate (Dijalog dij)
                {
                    Control[] ck = dij.Controls.Find("k", true);
                    Control[] cd = dij.Controls.Find("d", true);
                    k = ck.Length > 0 ? ck[0].Text : "nema";
                    d = cd.Length > 0 ? cd[0].Text : "nema";
                    KlikniCmd(dij, 2);
                };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                T("pn-forma-parametri", k == "7" && d == "a", "k='" + k + "' d='" + d + "' (NetGen: k='7' d='a')");
            }
            catch (Exception ex)
            {
                T("pn-forma-parametri", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ PN-13

        /// <summary>
        /// Form2.vb 1195: svaki neprazan ONLOAD forme se izvrši, i procedura bez parametara (OOPKnjigaR FILTERFORM
        /// ONLOAD="spHRInterneObustave"). DijalogTok.OnLoadJeUpit je tražio razmak, pa ga je preskakao.
        /// </summary>
        private static void OnLoadJednaRijec()
        {
            string xml = Xml("PnJedna.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Filter:FILT\">\r\n"
                + "<NAME>PnJedna</NAME>\r\n<CAPTION>Pregled jedna rijec</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONLOAD=\"dbo.spPregledNeovisniTree\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"natpis\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</FILT>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                SimpleButton gumb = GumbPodloge(forma, "Filter");
                if (!otvoren || gumb == null)
                {
                    T("pn-onload-jedna-rijec", false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                string natpis = "";
                Dijalog.NakonPrikaza = delegate (Dijalog dij)
                {
                    Control[] c = dij.Controls.Find("natpis", true);
                    natpis = c.Length > 0 ? c[0].Text : "nema";
                    KlikniCmd(dij, 2);
                };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                T("pn-onload-jedna-rijec", natpis == "Korijen", "natpis='" + natpis + "' (NetGen: 'Korijen')");
            }
            catch (Exception ex)
            {
                T("pn-onload-jedna-rijec", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }
    }
}
