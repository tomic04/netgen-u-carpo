using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;
using Carpo.NetGen.Ui;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.GridTest
{
    /// <summary>
    /// Treći neovisni pregled, 15. 11. 2026. (docs/pregled/pregled-fable52-2026-11-15.md). Testovi učinka za
    /// nalaze PF-xx, svaki uz VB retke koje provjerava.
    /// </summary>
    internal static class PregledFable52
    {
        private static Action<string, bool, string> _t;
        private static volatile string _korak = "";

        private static void T(string id, bool ok, string detalj)
        {
            _t(id, ok, detalj);
        }

        private static volatile bool _cuvar;

        public static void Izvrsi(Action<string, bool, string> t)
        {
            _t = t;
            Korak("onload-datum-hr", OnLoadDatumHr);
            // Podloga u GridTestu nema Carpo Logger, pa JupiterShared_Shown baci iznimku na UI niti (kao u PregledNeovisni).
            ThreadExceptionEventHandler iznimka = delegate (object s, ThreadExceptionEventArgs e)
            {
                Console.WriteLine("pf: iznimka na UI niti u koraku " + _korak + ": " + e.Exception.GetType().Name + " " + e.Exception.Message);
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
                    T("pf-spin-priprema", false, ex.Message);
                    return;
                }
                Korak("onsubmit-poruka", OnSubmitPoruka);
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
            Console.WriteLine("pf: " + ime);
            radnja();
        }

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
                Console.WriteLine("pf: korak " + _korak + " stoji 90 s");
                try
                {
                    var forme = new List<Form>();
                    foreach (Form f in Application.OpenForms)
                        forme.Add(f);
                    foreach (Form f in forme)
                    {
                        if (f is Dijalog && !f.IsDisposed)
                        {
                            Form z = f;
                            z.BeginInvoke(new Action(delegate () { z.DialogResult = DialogResult.Cancel; z.Close(); }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("pf:   " + ex.Message);
                }
            }
        }

        private static string Spoj()
        {
            return SpinVeza.Spoj("localhost", "spin");
        }

        private static void PripremiSpin()
        {
            using (var c = new SqlConnection(Spoj()))
            {
                c.Open();
                Izvrsi(c, "if object_id('dbo.spPregledFableGreska') is not null drop procedure dbo.spPregledFableGreska");
                Izvrsi(c, "create procedure dbo.spPregledFableGreska @k varchar(50) as\r\n"
                    + "if @k = 'poruka' raiserror('Dokument je zakljucen #', 16, 1)\r\n"
                    + "else raiserror('Obicna greska procedure', 16, 1)");
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

        private static string Xml(string ime, string sadrzaj)
        {
            string dir = Path.Combine(Path.GetTempPath(), "pf" + Guid.NewGuid().ToString("N").Substring(0, 6));
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

        /// <summary>Otvori podlogu, klikni gumb, u dijalogu klikni CMD1 (OK).</summary>
        private static Podloga KlikOk(string xml, string gumbNatpis, out string greska)
        {
            greska = "";
            bool otvoren;
            Podloga forma = OtvoriPodlogu(xml, out otvoren);
            SimpleButton gumb = GumbPodloge(forma, gumbNatpis);
            if (!otvoren || gumb == null)
            {
                greska = "podloga se nije otvorila ili nema gumba";
                return forma;
            }
            Dijalog.NakonPrikaza = delegate (Dijalog d) { KlikniCmd(d, 1); };
            try { gumb.PerformClick(); }
            finally { Dijalog.NakonPrikaza = null; }
            Application.DoEvents();
            return forma;
        }

        // ------------------------------------------------------------------ PF-02

        /// <summary>
        /// Class1.ExecSql 157-205: svaka ne-tiha greška SQL-a pokaže MsgBox (poruka procedure prije '#', inače
        /// cijela poruka i SQL). ONSUBMIT nakon dijaloga izvršava Form1 (5501-5507) kroz cls1.ExecSql, dakle s
        /// porukom. Carpo: Podloga.IzvrsiOnSubmit -> Citaj proguta grešku u statusnu traku (koju odmah prebriše
        /// broj redova nakon OsvjeziMaster), pa korisnik ne zna da procedura nije spremila.
        /// </summary>
        private static void OnSubmitPoruka()
        {
            JednaPoruka("pf02-onsubmit-poruka", "poruka", "Dokument je zakljucen");
            JednaPoruka("pf02-onsubmit-greska", "greska", "Obicna greska procedure");
        }

        private static void JednaPoruka(string id, string k, string ocekivano)
        {
            string xml = Xml("PfPoruka.xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Spremi:UNOS\">\r\n"
                + "<NAME>PfPoruka</NAME>\r\n<CAPTION>Pregled poruka</CAPTION>\r\n"
                + "<UNOS CAPTION=\"Unos\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\"dbo.spPregledFableGreska @k='" + k + "' !!\">\r\n"
                + "<INPUT TYPE=\"text\" NAME=\"t\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                + "</UNOS>\r\n</PROJECT>\r\n");
            var poruke = new List<string>();
            Action<string> staro = NgPoruka.Zapis;
            NgPoruka.Zapis = delegate (string tekst) { poruke.Add(tekst ?? ""); };
            Podloga forma = null;
            try
            {
                string greska;
                forma = KlikOk(xml, "Spremi", out greska);
                if (greska.Length > 0)
                {
                    T(id, false, greska);
                    return;
                }
                bool ima = false;
                for (int i = 0; i < poruke.Count; i++)
                {
                    if (poruke[i].IndexOf(ocekivano, StringComparison.OrdinalIgnoreCase) >= 0)
                        ima = true;
                }
                T(id, ima, "poruka korisniku: " + poruke.Count.ToString() + " [" + string.Join(" | ", poruke.ToArray()).Replace("\r", " ").Replace("\n", " ") + "] (NetGen: MsgBox '" + ocekivano + "')");
            }
            catch (Exception ex)
            {
                T(id, false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                NgPoruka.Zapis = staro;
                if (forma != null)
                    Zatvori(forma);
            }
        }

        // ------------------------------------------------------------------ PF-03

        /// <summary>
        /// Form2.ProcitajXML: ONLOAD stupac datuma ide u DTPicker kao Date (Value), a u CB kao CStr(Date) u kulturi
        /// računala, pa Class1.NadjiVrijednostuPrevParametrima za @DAT... da 'yyyyMMdd'. Carpo (Dijalog.UcitajOnLoad)
        /// pretvori datum u tekst InvariantCulture ("11/05/2026 00:00:00") i taj tekst upiše u DateEdit.Text: na
        /// računalu s hrvatskim postavkama DateEdit ga ne razumije (EditValue ostane tekst), pa SviParametri na OK
        /// pošalje @datum=null. Run 43756059751 (grana pregled52/dokaz) pokazuje stanje prije ispravka. Bez baze.
        /// </summary>
        private static void OnLoadDatumHr()
        {
            CultureInfo stara = Thread.CurrentThread.CurrentCulture;
            Dijalog d = null;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("hr-HR");
                var kontrole = new List<NgControl>();
                kontrole.Add(new NgControl("date", NgGrana.Date, "datumdok", new NgAttributeBag(new Dictionary<string, string>()), true));
                kontrole.Add(new NgControl("date", NgGrana.Date, "datumdo", new NgAttributeBag(new Dictionary<string, string>()), true));
                kontrole.Add(new NgControl("date", NgGrana.Date, "datumtekst", new NgAttributeBag(new Dictionary<string, string>()), true));
                var forma = new NgForm("T", new NgAttributeBag(null), "", "select 1 as x", kontrole, new List<NgButton>(), new List<string>());
                Func<string, DataTable> citaj = delegate (string sql)
                {
                    if ((sql ?? "").IndexOf("select 1 as x", StringComparison.OrdinalIgnoreCase) < 0)
                        return null;
                    var t = new DataTable();
                    t.Columns.Add("datumdok", typeof(DateTime));
                    t.Columns.Add("datumdo", typeof(DateTime));
                    t.Columns.Add("datbez", typeof(DateTime));
                    // Datum kao tekst (select '20261203' as datumtekst): VB 2273-2275 ga složi u yyyy-MM-dd.
                    t.Columns.Add("datumtekst", typeof(string));
                    t.Rows.Add(new DateTime(2026, 11, 5), new DateTime(2026, 11, 25), new DateTime(2026, 11, 5), "20261203");
                    return t;
                };
                d = new Dijalog();
                d.Slozi(forma, new SqlKontekst(), citaj);
                var prvi = (DateEdit)d.Controls.Find("datumdok", true)[0];
                var drugi = (DateEdit)d.Controls.Find("datumdo", true)[0];
                string a = prvi.EditValue is DateTime ? ((DateTime)prvi.EditValue).ToString("yyyyMMdd") : "nema (" + prvi.Text + ")";
                string b = drugi.EditValue is DateTime ? ((DateTime)drugi.EditValue).ToString("yyyyMMdd") : "nema (" + drugi.Text + ")";
                T("pf03-onload-datum-hr", a == "20261105" && b == "20261125",
                    "hr-HR: ONLOAD 5.11.2026 -> " + a + ", 25.11.2026 -> " + b + " (NetGen: 20261105, 20261125)");
                // Form2.PokupiParametre 3530-3567: učitani datum na OK ide kao 'yyyyMMdd'; prije je išao null.
                string svi = Convert.ToString(typeof(Dijalog).GetMethod("SviParametri", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, null));
                T("pf03-onload-datum-parametar", svi.IndexOf("@datumdok='20261105'", StringComparison.Ordinal) >= 0
                    && svi.IndexOf("@datumdo='20261125'", StringComparison.Ordinal) >= 0
                    && svi.IndexOf("@datumtekst='20261203'", StringComparison.Ordinal) >= 0, svi);
                string token = Convert.ToString(typeof(Dijalog).GetMethod("Pripremi", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(d, new object[] { "spX @d=@@datbez" }));
                T("pf03-onload-datum-token", token == "spX @d='20261105'", token + " (NetGen: spX @d='20261105')");
            }
            catch (Exception ex)
            {
                Exception u = ex.InnerException ?? ex;
                T("pf03-onload-datum-hr", false, u.GetType().Name + ": " + u.Message);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = stara;
                if (d != null)
                    d.Dispose();
            }
        }
    }
}
