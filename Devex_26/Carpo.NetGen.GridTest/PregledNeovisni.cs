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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.GridTest
{
    /// <summary>
    /// Neovisni pregled 14. 11. 2026. (docs/pregled/pregled-neovisni-2026-11-14.md): testovi učinka koji
    /// pokazuju razliku prema NetGenu. Svaki test navodi VB retke koje provjerava. Testovi `pn-*` padaju dok se
    /// nalaz ne ispravi.
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
                Korak("lijepi-exec", LijepiExec);
                Korak("veza-nakon-greske", VezaNakonGreske);
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
                Korak("manager-alati", ManagerAlati);
                Korak("klik-master-greska", KlikMasterGreska);
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

        // ------------------------------------------------------------------ N3 (R94)

        /// <summary>
        /// Form2.Button1_Click 3377: samo prvih 6 znakova iz "INSERTDELETEUPDATESELECT" gasi lijepljenje;
        /// Form1 5450: PokaziUpit(strStora &amp; " " &amp; strParametri). "exec" nema iznimku, pa 26 ONSUBMIT-a u
        /// korpusu (exec spMPUvjetiKase @action = 13 , @operateriid = @@operateriid ,) u NetGenu dobije parametre
        /// dijaloga. Lanac.Lijepi 142-144 ih za exec ne lijepi.
        /// </summary>
        private static void LijepiExec()
        {
            string s = Lanac.Lijepi("exec spMPUvjetiKase @action = 13 , @operateriid = 7 ,", "@mpakcijeid=5,");
            T("pn-lijepi-exec", s.IndexOf("@mpakcijeid=5", StringComparison.Ordinal) >= 0, s);
        }

        // ------------------------------------------------------------------ N4

        /// <summary>
        /// Class1.ExecSql 241-242: nakon svake greške koja nije RAISERROR s #/+ (i nakon tihih) NetGen radi
        /// Conn.Close() i Conn.Open(): transakcija koju je procedura ostavila otvorenom se poništi i #temp
        /// tablice nestanu. NgVeza to radi samo za timeout (R124), pa otvorena transakcija ostaje na vezi
        /// granule do zatvaranja i svaki kasniji upis ide u nju.
        /// </summary>
        private static void VezaNakonGreske()
        {
            string spoj = SpinVeza.SaAplikacijom(Spoj(), "pn-veza.xml");
            try
            {
                try { NgVeza.Citaj(spoj, "begin tran; raiserror('pn greska', 16, 1); select 1 as a", 30); }
                catch (SqlException) { }
                DataTable t = NgVeza.Citaj(spoj, "select @@trancount as n", 30);
                int n = t == null || t.Rows.Count == 0 ? -1 : Convert.ToInt32(t.Rows[0][0]);
                T("pn-veza-nakon-greske", n == 0, "@@trancount=" + n.ToString() + " (NetGen: 0, Close/Open)");
                if (n > 0)
                    NgVeza.Citaj(spoj, "while @@trancount > 0 rollback", 30);
            }
            catch (Exception ex)
            {
                T("pn-veza-nakon-greske", false, ex.Message);
            }
            finally
            {
                NgVeza.Otpusti(spoj);
            }
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
                + "<FILT CAPTION=\"Filter\" FILLEXPLORER=\"TRUE\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledNeovisni @k='jednom', @t='fiksno'\">\r\n"
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
                T("pn-onsubmit-jednom", z.Rows.Count == 1 && redova == 1,
                    "ONSUBMIT izvršen " + z.Rows.Count.ToString() + " puta (NetGen 1), master redova=" + redova.ToString());
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

        // ------------------------------------------------------------------ MGR-01

        /// <summary>
        /// Manager bar2 ("Tools" iznad master grida: skupi/raširi grupe, reset, Postavke → sume, izvoz) ostaje
        /// vidljiv u Podlozi (Pages.Clear() prazni samo ribbon). Rukovatelji zovu gcGla.ComponentExt, koji je u
        /// Podlozi null (JGridControl ga stvara samo uz Definition), pa je klik NullReferenceException.
        /// </summary>
        private static void ManagerAlati()
        {
            string xml = Xml("PnAlati.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\">\r\n<NAME>PnAlati</NAME>\r\n<CAPTION>Pregled alati</CAPTION>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                var gc = Polje(forma, "gcGla") as GridControl;
                object ext = gc == null ? null : gc.GetType().GetProperty("ComponentExt").GetValue(gc, null);
                var prosiri = Polje(forma, "bbiGridExpand") as BarItem;
                var dock = Polje(forma, "standaloneBarDockControl2") as Control;
                bool vidljiv = dock != null && dock.Visible;
                string greska = "";
                if (prosiri != null)
                {
                    try { prosiri.PerformClick(); }
                    catch (Exception ex) { greska = ex.GetType().Name; }
                }
                T("pn-manager-alati", otvoren && prosiri != null && greska.Length == 0,
                    "ComponentExt=" + (ext == null ? "null" : "ima") + " bar2 vidljiv=" + vidljiv.ToString() + " klik=" + (greska.Length == 0 ? "ok" : greska));
            }
            catch (Exception ex)
            {
                T("pn-manager-alati", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ SWP-11

        /// <summary>
        /// MasterGrid.IzvrsiCitanje ide kroz NgVeza izravno, pa greška mastera ne ulazi u Podloga._sqlGreska.
        /// ProvediKlikove tada javi "PRAZAN", a TestShell sweep broji samo GRESKA/HANG/PORUKA: granula čiji
        /// ONLOAD mastera pukne je CISTO. NetGen (Form1 3882 → ExecSql) bi pokazao grešku korisniku.
        /// </summary>
        private static void KlikMasterGreska()
        {
            string xml = Xml("PnGreska.xml",
                "<PROJECT ONLOAD=\"select * from dbo.PregledNeovisniNemaTablice\">\r\n<NAME>PnGreska</NAME>\r\n<CAPTION>Pregled greska</CAPTION>\r\n</PROJECT>\r\n");
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            try
            {
                List<string> redovi = forma.ProvediKlikove(true);
                string master = "";
                for (int i = 0; i < redovi.Count; i++)
                {
                    string[] c = redovi[i].Split('\t');
                    if (c.Length >= 4 && c[1] == "master")
                        master = c[3];
                }
                T("pn-klik-master-greska", master == "GRESKA", "master=" + master + " (sweep bi ovu granulu brojao kao CISTO)");
            }
            catch (Exception ex)
            {
                T("pn-klik-master-greska", false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                Zatvori(forma);
            }
        }
    }
}
