using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Ui;
using DevExpress.XtraEditors;

namespace Carpo.NetGen.GridTest
{
    /// <summary>
    /// PAR-03: što se lijepi na ONSUBMIT (Form2.PokupiParametre strParametri, 3406-3901). Kontrola s Tagom "_"
    /// (CAPTION "_broj") ne ide; grid daje samo ",@ime=id"; forma bez NEXTFORM reže od ",@grid".
    /// Spin: dbo.spParOnSubmit zapiše što je stvarno primila u dbo.ParOnSubmitLog.
    /// </summary>
    internal static class ParametriOnSubmit
    {
        private static Action<string, bool, string> _t;

        public static void Izvrsi(Action<string, bool, string> t)
        {
            _t = t;
            ThreadExceptionEventHandler iznimka = delegate (object s, ThreadExceptionEventArgs e)
            {
                Console.WriteLine("par03: iznimka na UI niti: " + e.Exception.GetType().Name + " " + e.Exception.Message);
            };
            Application.ThreadException += iznimka;
            try
            {
                try
                {
                    PripremiSpin();
                }
                catch (Exception ex)
                {
                    _t("par03-spin-priprema", false, ex.Message);
                    return;
                }
                // ZAKLJUČI: spmpzakljucidokNG @id=@@id, jedina kontrola _broj. NetGen: strParametri prazan.
                Provjeri("par03-crta-bez-zareza", "crta", "dbo.spParOnSubmit @k='crta'",
                    "<INPUT TYPE=\"text\" NAME=\"broj\" CAPTION=\"_broj\" DEFAULT=\"x\" ENABLED=\"FALSE\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />",
                    "", "");
                // IZMJENISTAVKUTERM: ONSUBMIT sa zarezom, uz _kontrolu: proceduri ide samo kontrola bez "_".
                Provjeri("par03-crta-zarez", "zarez", "dbo.spParOnSubmit @k='zarez',",
                    "<INPUT TYPE=\"text\" NAME=\"t\" CAPTION=\"Tekst\" DEFAULT=\"abc\" TOP=\"100\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />\r\n"
                    + "<INPUT TYPE=\"text\" NAME=\"broj\" CAPTION=\"_broj\" DEFAULT=\"x\" TOP=\"500\" LEFT=\"100\" WIDTH=\"2000\" HEIGHT=\"300\" />",
                    "abc", "");
                // Grid daje samo @stavke=id (3811), ne i @arrayofblueidsstavke, @rowcountstavke...
                Provjeri("par03-grid-zarez", "grid", "dbo.spParOnSubmit @k='grid',",
                    "<INPUT TYPE=\"grid\" NAME=\"stavke\" SQL=\"select 5 as id, 'a' as naziv\" TOP=\"100\" LEFT=\"100\" WIDTH=\"4000\" HEIGHT=\"2000\" />",
                    "", "5");
                // Bez zareza: NetGen ",@stavke=5" iza store je ispravan SQL.
                Provjeri("par03-grid-bez-zareza", "gridb", "dbo.spParOnSubmit @k='gridb'",
                    "<INPUT TYPE=\"grid\" NAME=\"stavke\" SQL=\"select 6 as id, 'a' as naziv\" TOP=\"100\" LEFT=\"100\" WIDTH=\"4000\" HEIGHT=\"2000\" />",
                    "", "6");
                // Grid imena grid...: forma bez NEXTFORM odreže od ",@grid" (Form1 5427-5431), pa grid ne ide.
                Provjeri("par03-gridime-rez", "rez", "dbo.spParOnSubmit @k='rez'",
                    "<INPUT TYPE=\"grid\" NAME=\"gridtip\" SQL=\"select 7 as id, 'a' as naziv\" TOP=\"100\" LEFT=\"100\" WIDTH=\"4000\" HEIGHT=\"2000\" />",
                    "", "");
            }
            finally
            {
                Application.ThreadException -= iznimka;
            }
        }

        private static string Spoj()
        {
            return SpinVeza.Spoj("sql-ai", "spin");
        }

        private static void PripremiSpin()
        {
            using (var c = new SqlConnection(Spoj()))
            {
                c.Open();
                Izvrsi(c, "if object_id('dbo.ParOnSubmitLog') is null create table dbo.ParOnSubmitLog (kad datetime not null default getdate(), k varchar(50) not null, t varchar(100) null, stavke int null)");
                Izvrsi(c, "if object_id('dbo.spParOnSubmit') is not null drop procedure dbo.spParOnSubmit");
                Izvrsi(c, "create procedure dbo.spParOnSubmit @k varchar(50), @t varchar(100) = null, @stavke int = null as\r\n"
                    + "insert into dbo.ParOnSubmitLog (k, t, stavke) values (@k, @t, @stavke);\r\n"
                    + "select 1 as id, @k as oznaka");
                Izvrsi(c, "delete from dbo.ParOnSubmitLog");
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

        private static DataTable Zapisi(string k)
        {
            var dt = new DataTable();
            using (var c = new SqlConnection(Spoj()))
            using (var cmd = new SqlCommand("select isnull(t, '') as t, isnull(convert(varchar(20), stavke), '') as stavke from dbo.ParOnSubmitLog where k = @k", c))
            {
                cmd.Parameters.AddWithValue("@k", k);
                c.Open();
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Podloga s gumbom Filter otvori formu FILT (FILLEXPLORER=TRUE, Form1 5445-5453), OK, pa ONSUBMIT ide
        /// jednom i puni master. Očekuje se jedan zapis s t i stavke kako ih NetGen pošalje, i master bez greške.
        /// </summary>
        private static void Provjeri(string test, string k, string onSubmit, string kontrole, string ocekT, string ocekStavke)
        {
            string xml = Xml("ParOn" + k + ".xml",
                "<PROJECT ONLOAD=\"select 1 as id, 'a' as naziv\" CMD1CAPTION=\"Filter:FILT\">\r\n"
                + "<NAME>ParOn" + k + "</NAME>\r\n<CAPTION>Parametri " + k + "</CAPTION>\r\n"
                + "<FILT CAPTION=\"Filter\" FILLEXPLORER=\"TRUE\" CMD1CAPTION=\"OK\" CMD2CAPTION=\"Odustani\" ONSUBMIT=\""
                + onSubmit.Replace("'", "&apos;") + "\">\r\n"
                + kontrole + "\r\n</FILT>\r\n</PROJECT>\r\n");
            var forma = new Podloga();
            try
            {
                forma.ShowInTaskbar = false;
                forma.StartPosition = FormStartPosition.Manual;
                forma.Location = new Point(-32000, -32000);
                forma.Show();
                bool otvoren = forma.Prikazi(xml, "sql-ai", "spin", 7);
                Application.DoEvents();
                SimpleButton gumb = Gumb(forma, "Filter");
                if (!otvoren || gumb == null)
                {
                    _t(test, false, "podloga se nije otvorila ili nema gumba");
                    return;
                }
                Dijalog.NakonPrikaza = delegate (Dijalog d)
                {
                    Control[] c = d.Controls.Find("cmd1", true);
                    if (c.Length > 0)
                        ((SimpleButton)c[0]).PerformClick();
                    else
                    {
                        d.DialogResult = DialogResult.Cancel;
                        d.Close();
                    }
                };
                try { gumb.PerformClick(); }
                finally { Dijalog.NakonPrikaza = null; }
                Application.DoEvents();
                var master = Polje(forma, "_master") as MasterGrid;
                string greska = master == null ? "" : (master.Stanje.SqlGreska ?? "");
                string sql = master == null ? "" : (master.Stanje.SqlZaSlanje ?? "");
                DataTable z = Zapisi(k);
                string t = z.Rows.Count == 1 ? Convert.ToString(z.Rows[0]["t"]) : "";
                string stavke = z.Rows.Count == 1 ? Convert.ToString(z.Rows[0]["stavke"]) : "";
                _t(test, greska.Length == 0 && z.Rows.Count == 1 && t == ocekT && stavke == ocekStavke,
                    "zapisa=" + z.Rows.Count.ToString() + " t='" + t + "' stavke='" + stavke + "' (NetGen t='" + ocekT
                    + "' stavke='" + ocekStavke + "') greska='" + greska + "' sql=" + sql);
            }
            catch (Exception ex)
            {
                _t(test, false, ex.GetType().Name + ": " + ex.Message);
            }
            finally
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
        }

        private static string Xml(string ime, string sadrzaj)
        {
            string dir = Path.Combine(Path.GetTempPath(), "par03" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(dir);
            string put = Path.Combine(dir, ime);
            File.WriteAllText(put, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" + sadrzaj, new UTF8Encoding(false));
            return put;
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

        private static SimpleButton Gumb(Podloga forma, string natpis)
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
    }
}
