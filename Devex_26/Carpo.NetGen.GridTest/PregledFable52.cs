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

        public static void Izvrsi(Action<string, bool, string> t)
        {
            _t = t;
            Korak("onload-datum-hr", OnLoadDatumHr);
        }

        private static void Korak(string ime, Action radnja)
        {
            _korak = ime;
            Console.WriteLine("pf: " + ime);
            radnja();
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
