using SpinDMSUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Carpo.NetGen.Ui;
using DevExpress.XtraEditors;
using System.Xml.Linq;
using Gen = Carpo.Gen;

namespace Carpo.TestShell
{
    public partial class Main : Form
    {
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_ACTIVATE = 1;
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool SetForegroundWindow(IntPtr hWnd);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
            private const int SW_SHOWNORMAL = 1;

        protected override void WndProc(ref Message m)
        {
            // ensure that mouse clicks on controls are processed even when form is inactive
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_ACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        private string server = "";
        private string database = "";
        private string xml = "";
        private string username = "";
        private string password = "";
        private int operateriId = 0;
        private string operater = "";
        private bool isXmlGen = false;
        private string paramList = "";
        private bool _autoMode = false;
        private int _uiIshod;
        private int _uiVidjen;
        private bool _uiKlikGotov;
        private bool _extCuvaj;

        
        public Main()
        {
            InitializeComponent();
            // activate form when mouse moves over it (helps when form is inactive)
            try { this.MouseMove += Main_MouseMove; toolStrip1.MouseEnter += Main_MouseEnter; } catch { }
        }

        public Main(string server, string database, string xml, int operateriId)
        {
            this.server = server;
            this.database = database;
            this.xml = xml;
            this.operateriId = operateriId;
            InitializeComponent();
            try { this.MouseMove += Main_MouseMove; toolStrip1.MouseEnter += Main_MouseEnter; } catch { }
        }

        /// <summary>
        /// Konstruktor za automatski mod (batch testiranje granula).
        /// </summary>
        public Main(string server, string database, string xml, int operateriId, string username, string password, bool autoMode)
        {
            this.server = server;
            this.database = database;
            this.xml = xml;
            this.operateriId = operateriId;
            this.username = username ?? "";
            this.password = password ?? "";
            this._autoMode = autoMode;
            this.operater = operateriId.ToString();
            InitializeComponent();
            try { this.MouseMove += Main_MouseMove; toolStrip1.MouseEnter += Main_MouseEnter; } catch { }
        }

        private void Main_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                if (Form.ActiveForm != this)
                    this.Activate();
            }
            catch { }
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Form.ActiveForm != this)
                    this.Activate();
            }
            catch { }
        }

        public void Test()
        {
            //string str = "2323 343 {#test} 4324 23432 4324 32423 {4234 234}  234 32423 43ret {er dfg} ert erotewtmwekotnwe o";

            //List<Match> matches = GetSubStrings(str, "{", "}").ToList();

            //foreach (Match match in matches.OrderByDescending(x => x.Index))
            //{
            //    str = SubsituteString(str, match.Index, match.Length, "222");
            //}
            //MessageBox.Show(str);
        }

        //public static string ReplaceAt(this string str, int index, int length, string replace)
        //{
        //    return str.Remove(index, Math.Min(length, str.Length - index))
        //            .Insert(index, replace);
        //}

        public static string SubsituteString(string OriginalStr, int index, int length, string SubsituteStr)
        {
            return new StringBuilder(OriginalStr).Remove(index, length).Insert(index, SubsituteStr).ToString();
        }

        //private IEnumerable<string> GetSubStrings(string input, string start, string end)
        //{
        //    Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
        //    MatchCollection matches = r.Matches(input);
        //    foreach (Match match in matches)
        //        yield return match.Groups[1].Value;
        //}

        private IEnumerable<Match> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            //Regex r = new Regex(start + "(.*?)" + end);
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match;
        }

        //private void cmdStart_Click(object sender, EventArgs e)
        //{
        //    Test();
        //    //Carpo.Granula.Start start = new Carpo.Granula.Start();
        //    //start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
        //}

        //private void cmdUrednik_Click(object sender, EventArgs e)
        //{
        //    Carpo.Urednik.Start start = new Carpo.Urednik.Start();
        //    start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
        //}

        //private void toolStripButton1_Click(object sender, EventArgs e)
        //{
        //    Carpo.Urednik2.Start start = new Carpo.Urednik2.Start();
        //    start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
        //}

        //private void toolStripButton2_Click(object sender, EventArgs e)
        //{
        //    Carpo.Urednik3.Start start = new Carpo.Urednik3.Start();
        //    start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
        //}

        private void cmdGen_Click(object sender, EventArgs e)
        {
            // Prevent accidental reentrancy / first-click focus issue by disabling the button immediately,
            // add lightweight diagnostics and surface exceptions so first-click failures aren't silent.
            if (!cmdGen.Enabled)
                return;

            cmdGen.Enabled = false;
            try
            {
                System.Diagnostics.Debug.WriteLine("cmdGen_Click invoked: isXmlGen=" + isXmlGen.ToString());

                if (!isXmlGen)
                {
                    // <root> ostaje Carpo. <PROJECT> ide u Carpo.NetGen (isti server, baza, operater).
                    if (Carpo.NetGen.Xml.NetGenXmlReader.JeNetGenProjekt(xml))
                    {
                        var loader = new Carpo.NetGen.Loader();
                        Carpo.Core.Logger dnevnik = null;
                        try
                        {
                            // Starter.Run 81–84: Logger u DependencyContainer prije forme.
                            dnevnik = new Carpo.Core.Logger(Carpo.Core.Log.LogLevel.Debug);
                            dnevnik.GranuleName = Path.GetFileNameWithoutExtension(xml);
                            dnevnik.ServerName = server;
                            dnevnik.DatabaseName = database;
                            Carpo.Core.Log.DependencyContainer.Add(dnevnik);
                            Carpo.NetGen.Log.NgDnevnik.Spoji(dnevnik);
                        }
                        catch
                        {
                        }
                        loader.TestXmlPath = xml;
                        loader.moduleId = Path.GetFileName(xml);
                        loader.mainHandle = (int)pnlShell.Handle;
                        loader.Run8(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
                        if (!string.IsNullOrEmpty(loader.ZadnjaGreska))
                            MessageBox.Show(loader.ZadnjaGreska, "Carpo.NetGen");
                        else
                        {
                            string greska;
                            var forma = Carpo.NetGen.Ui.Prozor.Otvori(xml, server, database, operateriId, out greska);
                            if (forma == null || !string.IsNullOrEmpty(greska))
                                MessageBox.Show(string.IsNullOrEmpty(greska) ? "Granula se nije otvorila." : greska, "Carpo.NetGen");
                            if (forma != null)
                            {
                                // RegisterForm zove InitLogger prije prikaza. Shown cita isti Logger.
                                var jupiter = forma as Carpo.UI.Forms.Jupiter.Base;
                                if (jupiter != null && dnevnik != null)
                                    jupiter.InitLogger(dnevnik);
                                forma.Show(this);
                            }
                        }
                    }
                    else
                    {
                        Gen.Start start = new Gen.Start();
                        start.TestXmlPath = xml;
                        start.ModuleId = Path.GetFileName(xml);
                        start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
                    }
                }
                else
                {
                    StartXmlGen();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("cmdGen_Click exception: " + ex.ToString());
                MessageBox.Show(ex.ToString(), "Start Error");
            }
            finally
            {
                // restore enabled state so user can retry if needed
                try
                {
                    cmdGen.Enabled = true;
                }
                catch { }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (_autoMode)
                return;
            Postavke();
        }

        /// <summary>
        /// Isti klik na Start kao korisnik. Ne spaja GranulaSesija.
        /// </summary>
        public int PokreniEkstenzije(string xml, string snimka)
        {
            var r = Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(xml);
            Carpo.NetGen.Model.NgForm forma = null;
            if (r.Projekt != null && r.Projekt.Forme != null)
            {
                for (int i = 0; i < r.Projekt.Forme.Count; i++)
                {
                    if (string.Equals(r.Projekt.Forme[i].Ime, "EXT_FORM", StringComparison.OrdinalIgnoreCase))
                        forma = r.Projekt.Forme[i];
                }
            }
            if (forma == null)
            {
                UpisiIshod(snimka, "FAIL nema EXT_FORM");
                return 5;
            }
            var vidjeno = new List<string>();
            Carpo.NetGen.Komande.EkstenzijaGrid.Greska = "";
            Carpo.NetGen.Komande.EkstenzijaGrid.Poziv = "";
            Dijalog.NakonPrikaza = delegate (Dijalog d)
            {
                string[] gumbi = new[] { "Pivot", "Graf", "Ispis" };
                for (int i = 0; i < gumbi.Length; i++)
                {
                    SimpleButton gumb = NadjiGumb(d, gumbi[i]);
                    if (gumb != null)
                        gumb.PerformClick();
                    string tip = Carpo.NetGen.Komande.EkstenzijaGrid.ZadnjiTip ?? "";
                    if (tip.Length > 0)
                        vidjeno.Add(tip);
                    Form f = Carpo.NetGen.Komande.EkstenzijaGrid.Zadnja;
                    if (f != null && !f.IsDisposed)
                    {
                        try { f.Close(); } catch { }
                    }
                }
                try { d.Close(); } catch { }
            };
            _extCuvaj = true;
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            var cuvar = new Thread(delegate () { CuvajEkstenziju(pid); });
            cuvar.IsBackground = true;
            cuvar.Start();
            var dij = new Dijalog();
            dij.Prikazi(forma, new Carpo.NetGen.Grid.SqlKontekst(), delegate (string sql) { return new DataTable(); }, true, this);
            _extCuvaj = false;
            Dijalog.NakonPrikaza = null;
            string poziv = Carpo.NetGen.Komande.EkstenzijaGrid.Poziv ?? "";
            bool chart = poziv.IndexOf("ChartGRID=ok", StringComparison.Ordinal) >= 0;
            bool pivot = poziv.IndexOf("PivotGRID=ok", StringComparison.Ordinal) >= 0;
            bool ispis = poziv.IndexOf("PrintDocumentREPX=ok", StringComparison.Ordinal) >= 0;
            string nema = Carpo.NetGen.Komande.EkstenzijaGrid.Nedostaje ?? "";
            string linija;
            int kod;
            if (chart && pivot && ispis)
            {
                linija = "OK ext " + string.Join(",", vidjeno.ToArray()) + " " + poziv;
                kod = 0;
            }
            else if (nema.Length > 0)
            {
                linija = "OPEN ext PivotGRID ChartGRID nedostaje " + nema
                    + " ispis=" + (ispis ? "ok" : "ne")
                    + " ref=" + Carpo.NetGen.Komande.EkstenzijaGrid.Referenca;
                kod = 0;
            }
            else
            {
                linija = "FAIL ext " + string.Join(",", vidjeno.ToArray()) + " " + poziv + " " + Carpo.NetGen.Komande.EkstenzijaGrid.Greska
                    + " janus=" + Carpo.NetGen.Komande.EkstenzijaGrid.PutDatoteke("Janus.Windows.GridEX.v3.dll")
                    + " " + Carpo.NetGen.Komande.EkstenzijaGrid.Trag;
                kod = 5;
            }
            UpisiIshod(snimka, linija);
            return kod;
        }

        private void CuvajEkstenziju(int pid)
        {
            int vidjen = 0;
            while (_extCuvaj)
            {
                Thread.Sleep(400);
                if (!_extCuvaj)
                    return;
                if (ImaTudjiProzor(pid))
                    vidjen++;
                else
                    vidjen = 0;
                if (vidjen >= 3)
                    ZatvoriTudje(pid);
            }
        }

        private static bool ImaTudjiProzor(int pid)
        {
            bool ima = false;
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                if (!TudjiProzor(h, pid))
                    return true;
                ima = true;
                return false;
            }, IntPtr.Zero);
            return ima;
        }

        private static void ZatvoriTudje(int pid)
        {
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                if (TudjiProzor(h, pid))
                    PostMessage(h, 0x0010, IntPtr.Zero, IntPtr.Zero);
                return true;
            }, IntPtr.Zero);
        }

        private static bool TudjiProzor(IntPtr h, int pid)
        {
            uint wpid;
            GetWindowThreadProcessId(h, out wpid);
            if (wpid != (uint)pid || !IsWindowVisible(h))
                return false;
            string t = NaslovProzora(h);
            if (t == "TestShell" || t == "Ekstenzije")
                return false;
            return true;
        }

        public int PokreniBoju(string xml, string snimka)
        {
            var r = Carpo.NetGen.Xml.NetGenXmlReader.Ucitaj(xml);
            Carpo.NetGen.Model.NgForm forma = null;
            if (r.Projekt != null && r.Projekt.Forme != null)
            {
                for (int i = 0; i < r.Projekt.Forme.Count; i++)
                {
                    if (string.Equals(r.Projekt.Forme[i].Ime, "GRUPEDODAJ_FORM", StringComparison.OrdinalIgnoreCase))
                        forma = r.Projekt.Forme[i];
                }
            }
            if (forma == null)
            {
                UpisiIshod(snimka, "FAIL nema GRUPEDODAJ_FORM");
                return 5;
            }
            bool vidjen = false;
            var nit = new Thread(delegate ()
            {
                for (int i = 0; i < 50; i++)
                {
                    Thread.Sleep(200);
                    if (ZatvoriDijalogBoje())
                    {
                        vidjen = true;
                        return;
                    }
                }
            });
            nit.IsBackground = true;
            nit.SetApartmentState(ApartmentState.STA);
            nit.Start();
            var kontekst = new Carpo.NetGen.Grid.SqlKontekst();
            kontekst.OperaterId = operateriId.ToString();
            string ishod = "FAIL boja";
            Dijalog.NakonPrikaza = delegate (Dijalog d)
            {
                SimpleButton gumb = NadjiGumb(d, "Odaberi boju");
                if (gumb != null)
                    gumb.PerformClick();
                string tekst = "";
                Control[] n = d.Controls.Find("boja", true);
                if (n.Length > 0)
                    tekst = n[0].Text ?? "";
                bool ok = vidjen && tekst.Length > 0 && tekst == Carpo.NetGen.Komande.BojaNaredba.Odabrana;
                ishod = ok ? "OK boja=" + tekst : "FAIL boja vidjen=" + vidjen + " tekst=" + tekst + " odabrana=" + Carpo.NetGen.Komande.BojaNaredba.Odabrana;
                try { d.Close(); } catch { }
            };
            var dij = new Dijalog();
            dij.Prikazi(forma, kontekst, CitajBoju, true, this);
            Dijalog.NakonPrikaza = null;
            UpisiIshod(snimka, ishod);
            return ishod.StartsWith("OK", StringComparison.Ordinal) ? 0 : 5;
        }

        private static DataTable CitajBoju(string sql)
        {
            var dt = new DataTable();
            using (var conn = new System.Data.SqlClient.SqlConnection(Carpo.NetGen.Grid.SpinVeza.Spoj("localhost", "spin")))
            using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
            using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = 30;
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        private static bool ZatvoriDijalogBoje()
        {
            bool nasao = false;
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                uint wpid;
                GetWindowThreadProcessId(h, out wpid);
                if (wpid != (uint)pid || !IsWindowVisible(h))
                    return true;
                var klasa = new StringBuilder(256);
                GetClassName(h, klasa, klasa.Capacity);
                if (klasa.ToString().IndexOf("32770", StringComparison.Ordinal) < 0)
                    return true;
                nasao = true;
                PostMessage(h, 0x0111, (IntPtr)1, IntPtr.Zero);
                return false;
            }, IntPtr.Zero);
            return nasao;
        }

        public int PokreniUiSmoke(string snimka)
        {
            _uiIshod = 0;
            _uiVidjen = 0;
            _uiKlikGotov = false;
            var dir = Path.GetDirectoryName(snimka);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            var cudar = new Thread(delegate () { CuvajDijalog(snimka); });
            cudar.IsBackground = true;
            cudar.SetApartmentState(ApartmentState.STA);
            cudar.Start();
            try
            {
                operater = operateriId.ToString();
                cmdGen.Enabled = true;
                try
                {
                    Carpo.Core.Logger.BindJupiterLogs(server, operateriId, operater, database, null, Path.GetFileNameWithoutExtension(xml));
                }
                catch (Exception ex)
                {
                    UpisiIshod(snimka, "FAIL bind " + ex);
                    return 1;
                }
                Faza(snimka, "prije-klika");
                cmdGen_Click(this, EventArgs.Empty);
                _uiKlikGotov = true;
                Faza(snimka, "poslije-klika");
            }
            catch (Exception ex)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL start " + ex);
                return 1;
            }
            if (_uiIshod == 1)
                return 1;
            Form granula = NadjiGranulu();
            if (granula == null && _uiIshod != 2)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL nema prozora granule");
                return 2;
            }
            if (!ImaSnimku(snimka) && granula != null && !granula.IsDisposed)
            {
                try { granula.Update(); } catch { }
                SnimiProzor(granula.Handle, snimka);
            }
            if (!ImaSnimku(snimka))
            {
                UpisiIshod(snimka, "FAIL nema screenshota " + TekstSnimke(snimka));
                return 4;
            }
            string klik = "";
            if (granula != null && !granula.IsDisposed)
            {
                int k = Klikni(granula, snimka, out klik);
                if (k != 0)
                    return k;
            }
            try
            {
                Faza(snimka, "prije-zatvaranja");
                if (granula != null && !granula.IsDisposed)
                    granula.Close();
                Close();
                Faza(snimka, "poslije-zatvaranja");
            }
            catch (Exception ex)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL zatvaranje " + ex);
                return 3;
            }
            UpisiIshod(snimka, "OK " + xml + (klik.Length == 0 ? "" : " " + klik));
            return 0;
        }

        /// <summary>
        /// Tri tipa: obična forma (Izmjena), Manager (Unesi), ulazni dijalog (Odustani).
        /// Odustani je CLOSE i ne šalje SQL (OznakaKL FRMEDIT CMD2, Statusi INSERTFORM ONCLICK close).
        /// </summary>
        private int Klikni(Form granula, string snimka, out string trag)
        {
            trag = "";
            string ime = Path.GetFileName(xml ?? "");
            if (ime.Equals("RealizacijaStavkeNG.xml", StringComparison.OrdinalIgnoreCase))
            {
                var o = NadjiGumb(granula, "Odustani");
                if (o == null)
                {
                    UpisiIshod(snimka, "FAIL nema gumba Odustani");
                    return 5;
                }
                o.PerformClick();
                trag = "klik=Odustani";
                return 0;
            }
            string otvori = null;
            string tag = null;
            if (ime.Equals("OznakaKL.xml", StringComparison.OrdinalIgnoreCase))
                otvori = "Izmjena";
            else if (ime.Equals("KntKompVerizne.xml", StringComparison.OrdinalIgnoreCase))
                tag = "OBRISI_FORM";
            else if (ime.Equals("Statusi.xml", StringComparison.OrdinalIgnoreCase))
                otvori = "Unesi";
            if (otvori == null && tag == null)
                return 0;
            var gumb = tag != null ? NadjiPoTag(granula, tag) : NadjiGumb(granula, otvori);
            if (otvori == null)
                otvori = tag;
            if (gumb == null)
            {
                UpisiIshod(snimka, "FAIL nema gumba " + otvori);
                return 5;
            }
            string pod = snimka + ".podforma.png";
            bool odustao = false;
            bool klikGotov = false;
            string razlog = "";
            int pokusaj = 0;
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 400;
            Dijalog.NakonPrikaza = delegate (Dijalog dij)
            {
                if (odustao || dij == null || dij.IsDisposed)
                    return;
                var o = NadjiGumb(dij, "Odustani");
                if (o == null)
                {
                    razlog = dij.Text + " " + TekstGumba(dij);
                    try { dij.Close(); } catch { }
                    return;
                }
                try { dij.Update(); } catch { }
                SnimiProzor(dij.Handle, pod);
                if (!ImaSnimku(pod))
                    SnimiKontrolu(dij, pod);
                odustao = true;
                try
                {
                    o.DialogResult = DialogResult.Cancel;
                    o.PerformClick();
                    if (!dij.IsDisposed)
                        dij.Close();
                }
                catch (Exception ex)
                {
                    razlog = ex.ToString();
                    try { File.WriteAllText(snimka + ".iznimka.txt", ex.ToString()); } catch { }
                    try { if (!dij.IsDisposed) dij.Close(); } catch { }
                }
            };
            timer.Tick += delegate
            {
                if (odustao)
                {
                    timer.Stop();
                    return;
                }
                pokusaj++;
                Form dij = null;
                foreach (Form f in Application.OpenForms)
                {
                    if (f == null || f.IsDisposed || !f.Visible || f == this || f == granula)
                        continue;
                    dij = f;
                    break;
                }
                if (dij == null)
                {
                    if (pokusaj > 30)
                    {
                        timer.Stop();
                        razlog = "nema podforme";
                    }
                    return;
                }
                var o = NadjiGumb(dij, "Odustani");
                if (o == null)
                {
                    if (pokusaj > 30)
                    {
                        timer.Stop();
                        razlog = dij.Text + " " + TekstGumba(dij);
                        try { dij.Close(); } catch { }
                    }
                    return;
                }
                timer.Stop();
                try { dij.Update(); } catch { }
                SnimiProzor(dij.Handle, pod);
                odustao = true;
                o.PerformClick();
            };
            Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
            {
                try { File.AppendAllText(snimka + ".iznimka.txt", e.Exception.ToString() + "\r\n"); } catch { }
            };
            timer.Start();
            var cekaj = new Thread(delegate () { SnimiZastoj(snimka, granula, delegate () { return klikGotov; }); });
            cekaj.IsBackground = true;
            cekaj.Start();
            try
            {
                gumb.PerformClick();
                klikGotov = true;
            }
            catch (Exception ex)
            {
                klikGotov = true;
                timer.Stop();
                timer.Dispose();
                Dijalog.NakonPrikaza = null;
                UpisiIshod(snimka, "FAIL klik " + ex.Message);
                return 5;
            }
            timer.Stop();
            timer.Dispose();
            Dijalog.NakonPrikaza = null;
            if (!odustao || !ImaSnimku(pod))
            {
                UpisiIshod(snimka, "FAIL podforma " + otvori + " " + razlog);
                return 5;
            }
            trag = "podforma=" + otvori;
            return 0;
        }

        private static void SnimiKontrolu(Control c, string put)
        {
            try
            {
                if (c == null || c.Width < 50 || c.Height < 50)
                    return;
                using (var bmp = new System.Drawing.Bitmap(c.Width, c.Height))
                {
                    c.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, c.Width, c.Height));
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch
            {
            }
        }

        private void SnimiZastoj(string snimka, Form granula, Func<bool> gotovo)
        {
            Thread.Sleep(20000);
            if (gotovo())
                return;
            var sb = new StringBuilder();
            sb.AppendLine("20s prozori");
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                uint wpid;
                GetWindowThreadProcessId(h, out wpid);
                if (wpid != (uint)pid)
                    return true;
                var klasa = new StringBuilder(256);
                GetClassName(h, klasa, klasa.Capacity);
                sb.Append(IsWindowVisible(h) ? "vidljiv " : "skriven ");
                sb.Append(klasa).Append(" | ").Append(NaslovProzora(h));
                sb.AppendLine();
                EnumChildWindows(h, delegate (IntPtr c, IntPtr p)
                {
                    var ck = new StringBuilder(256);
                    GetClassName(c, ck, ck.Capacity);
                    string t = NaslovProzora(c);
                    if (t.Length == 0 && ck.ToString().IndexOf("32770", StringComparison.Ordinal) < 0)
                        return true;
                    sb.Append("  dijete ").Append(ck).Append(" | ").Append(t).AppendLine();
                    return true;
                }, IntPtr.Zero);
                return true;
            }, IntPtr.Zero);
            string stack = "";
            var ev = new ManualResetEvent(false);
            try
            {
                BeginInvoke(new Action(delegate ()
                {
                    stack = Environment.StackTrace;
                    ev.Set();
                }));
            }
            catch (Exception ex)
            {
                stack = ex.Message;
                ev.Set();
            }
            if (!ev.WaitOne(3000))
                stack = "UI nit ne crta: BeginInvoke nije odrađen za 3 s. ShowDialog nije u petlji poruka.";
            sb.AppendLine(stack);
            try
            {
                foreach (Form f in Application.OpenForms)
                    sb.Append("OpenForm ").Append(f.GetType().Name).Append(" ").Append(f.Text).Append(" visible=").Append(f.Visible).AppendLine();
            }
            catch (Exception ex)
            {
                sb.AppendLine("OpenForms " + ex.Message);
            }
            if (granula != null)
                sb.Append("granula ").Append(granula.GetType().FullName).AppendLine();
            try { File.WriteAllText(snimka + ".prozori.txt", sb.ToString()); } catch { }
        }

        private static string TekstGumba(Control korijen)
        {
            var sb = new StringBuilder();
            SkupiGumbe(korijen, sb);
            return sb.ToString();
        }

        private static void SkupiGumbe(Control korijen, StringBuilder sb)
        {
            if (korijen == null)
                return;
            foreach (Control c in korijen.Controls)
            {
                var b = c as SimpleButton;
                if (b != null)
                    sb.Append("[").Append(b.Text).Append("]");
                SkupiGumbe(c, sb);
            }
        }

        private static SimpleButton NadjiPoTag(Control korijen, string tag)
        {
            if (korijen == null)
                return null;
            foreach (Control c in korijen.Controls)
            {
                var b = c as SimpleButton;
                if (b != null && b.Tag != null && string.Equals(b.Tag.ToString(), tag, StringComparison.OrdinalIgnoreCase))
                    return b;
                var dublje = NadjiPoTag(c, tag);
                if (dublje != null)
                    return dublje;
            }
            return null;
        }

        private static SimpleButton NadjiGumb(Control korijen, string natpis)
        {
            if (korijen == null)
                return null;
            foreach (Control c in korijen.Controls)
            {
                var b = c as SimpleButton;
                if (b != null)
                {
                    string t = (b.Text ?? "").Replace("\r", "").Replace("\n", "");
                    if (t.IndexOf(natpis, StringComparison.OrdinalIgnoreCase) >= 0)
                        return b;
                }
                var dublje = NadjiGumb(c, natpis);
                if (dublje != null)
                    return dublje;
            }
            return null;
        }

        private Form NadjiGranulu()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f == null || f == this || !f.Visible)
                    continue;
                return f;
            }
            return null;
        }

        private void CuvajDijalog(string snimka)
        {
            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            for (int i = 0; i < 40; i++)
            {
                Thread.Sleep(500);
                if (_uiKlikGotov)
                    continue;
                string naslov = ProzorGreske(pid);
                if (naslov != null)
                {
                    SnimiEkran(snimka);
                    UpisiIshod(snimka, "FAIL dijalog " + naslov);
                    _uiIshod = 1;
                    Environment.Exit(1);
                }
                IntPtr prozor = ProzorGranule(pid);
                if (prozor != IntPtr.Zero)
                {
                    _uiIshod = 2;
                    _uiVidjen++;
                    if (!ImaSnimku(snimka))
                        SnimiProzor(prozor, snimka);
                    // WM_CLOSE usred Show zna zaglaviti modeless prozor. Zatvori tek modal koji ne pusti klik.
                    if (_uiVidjen >= 6 && ImaSnimku(snimka))
                        PostMessage(prozor, 0x0010, IntPtr.Zero, IntPtr.Zero);
                }
            }
            if (_uiKlikGotov)
                return;
            if (_uiIshod == 0)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL timeout " + TekstSnimke(snimka));
                Environment.Exit(2);
            }
            if (_uiIshod == 2 && !ImaSnimku(snimka) && !_uiKlikGotov)
            {
                UpisiIshod(snimka, "FAIL screenshot " + TekstSnimke(snimka));
                Environment.Exit(4);
            }
        }

        private static string ProzorGreske(int pid)
        {
            string nadjen = null;
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                uint wpid;
                GetWindowThreadProcessId(h, out wpid);
                if (wpid != (uint)pid || !IsWindowVisible(h))
                    return true;
                string t = NaslovProzora(h);
                if (t == "Start Error" || t == "Carpo.NetGen" || t == "Greška" || t == "StartXmlGen()")
                {
                    nadjen = t + TekstDijaloga(h);
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return nadjen;
        }

        private static string TekstDijaloga(IntPtr hwnd)
        {
            var sb = new StringBuilder();
            EnumChildWindows(hwnd, delegate (IntPtr h, IntPtr l)
            {
                string t = NaslovProzora(h);
                if (t.Length > 0)
                    sb.Append(" | ").Append(t);
                return true;
            }, IntPtr.Zero);
            return sb.ToString();
        }

        private static IntPtr ProzorGranule(int pid)
        {
            IntPtr nadjen = IntPtr.Zero;
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                uint wpid;
                GetWindowThreadProcessId(h, out wpid);
                if (wpid != (uint)pid || !IsWindowVisible(h))
                    return true;
                string t = NaslovProzora(h);
                if (t.Length == 0 || t == "TestShell")
                    return true;
                if (t == "Start Error" || t == "Carpo.NetGen" || t == "Greška")
                    return true;
                nadjen = h;
                return false;
            }, IntPtr.Zero);
            return nadjen;
        }

        private static bool ImaSnimku(string put)
        {
            try
            {
                return File.Exists(put) && new FileInfo(put).Length >= 500;
            }
            catch
            {
                return false;
            }
        }

        private static string TekstSnimke(string put)
        {
            try
            {
                string extra = put + ".snimka.txt";
                if (File.Exists(extra))
                    return File.ReadAllText(extra);
            }
            catch
            {
            }
            return "";
        }

        private static void ZapisiSnimku(string put, string tekst)
        {
            try { File.WriteAllText(put + ".snimka.txt", tekst); } catch { }
        }

        private static void SnimiProzor(IntPtr hwnd, string put)
        {
            if (hwnd == IntPtr.Zero || ImaSnimku(put))
                return;
            RECT r;
            if (!GetWindowRect(hwnd, out r))
            {
                ZapisiSnimku(put, "GetWindowRect nije uspio");
                return;
            }
            int w = r.Right - r.Left;
            int h = r.Bottom - r.Top;
            if (w < 50 || h < 50)
            {
                ZapisiSnimku(put, "prozor " + w + "x" + h);
                return;
            }
            if (SnimiPikselima(r.Left, r.Top, w, h, put))
                return;
            if (!SnimiBitBlt(hwnd, w, h, put))
                ZapisiSnimku(put, "ekran bez sadrzaja " + w + "x" + h);
        }

        private static void Faza(string snimka, string tekst)
        {
            try { File.WriteAllText(snimka + ".faza.txt", tekst); } catch { }
        }

        private static string NaslovProzora(IntPtr hwnd)
        {
            var sb = new StringBuilder(512);
            IntPtr rezultat;
            IntPtr odg = SendMessageTimeout(hwnd, 0x000D, (IntPtr)sb.Capacity, sb, 0x0002, 200, out rezultat);
            if (odg == IntPtr.Zero)
                return "";
            return sb.ToString();
        }

        private static bool SnimiBitBlt(IntPtr hwnd, int w, int h, string put)
        {
            try
            {
                using (var bmp = new System.Drawing.Bitmap(w, h))
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    IntPtr src = GetWindowDC(hwnd);
                    try
                    {
                        IntPtr hdc = g.GetHdc();
                        bool ok;
                        try
                        {
                            ok = BitBlt(hdc, 0, 0, w, h, src, 0, 0, 0x00CC0020);
                        }
                        finally
                        {
                            g.ReleaseHdc(hdc);
                        }
                        if (!ok || !JeSadrzaj(bmp))
                            return false;
                    }
                    finally
                    {
                        ReleaseDC(hwnd, src);
                    }
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ZapisiSnimku(put, ex.ToString());
                return false;
            }
        }

        private static bool SnimiPikselima(int lijevo, int gore, int w, int h, string put)
        {
            try
            {
                var ekran = System.Windows.Forms.SystemInformation.VirtualScreen;
                int x = Math.Max(lijevo, ekran.Left);
                int y = Math.Max(gore, ekran.Top);
                int sirina = Math.Min(lijevo + w, ekran.Right) - x;
                int visina = Math.Min(gore + h, ekran.Bottom) - y;
                if (sirina < 50 || visina < 50)
                {
                    ZapisiSnimku(put, "piksel izvan ekrana " + sirina + "x" + visina);
                    return false;
                }
                using (var bmp = new System.Drawing.Bitmap(sirina, visina))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                        g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(sirina, visina));
                    if (!JeSadrzaj(bmp))
                    {
                        ZapisiSnimku(put, "ekran prazan " + sirina + "x" + visina);
                        return false;
                    }
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ZapisiSnimku(put, ex.ToString());
                return false;
            }
        }

        private static bool JeSadrzaj(System.Drawing.Bitmap bmp)
        {
            int korakX = Math.Max(1, bmp.Width / 40);
            int korakY = Math.Max(1, bmp.Height / 40);
            var vjedra = new HashSet<int>();
            for (int y = 0; y < bmp.Height; y += korakY)
            {
                for (int x = 0; x < bmp.Width; x += korakX)
                {
                    var c = bmp.GetPixel(x, y);
                    vjedra.Add((c.R / 16) << 8 | (c.G / 16) << 4 | (c.B / 16));
                    if (vjedra.Count >= 8)
                        return true;
                }
            }
            return false;
        }

        private static void SnimiEkran(string put)
        {
            if (ImaSnimku(put))
                return;
            try
            {
                var ekran = Screen.PrimaryScreen == null ? System.Drawing.Rectangle.Empty : Screen.PrimaryScreen.Bounds;
                if (ekran.Width < 50 || ekran.Height < 50)
                {
                    ZapisiSnimku(put, "nema ekrana " + ekran.Width + "x" + ekran.Height);
                    return;
                }
                using (var bmp = new System.Drawing.Bitmap(ekran.Width, ekran.Height))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                        g.CopyFromScreen(ekran.Location, System.Drawing.Point.Empty, ekran.Size);
                    if (!JeSadrzaj(bmp))
                    {
                        ZapisiSnimku(put, "cijeli ekran prazan " + ekran.Width + "x" + ekran.Height);
                        return;
                    }
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                ZapisiSnimku(put, ex.ToString());
            }
        }

        private static void SnimiFormu(Form forma, string put)
        {
            if (ImaSnimku(put))
                return;
            try
            {
                var r = forma.Bounds;
                if (r.Width < 50 || r.Height < 50)
                {
                    ZapisiSnimku(put, "forma " + r.Width + "x" + r.Height);
                    return;
                }
                using (var bmp = new System.Drawing.Bitmap(r.Width, r.Height))
                {
                    forma.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, r.Width, r.Height));
                    if (!JeSadrzaj(bmp))
                    {
                        ZapisiSnimku(put, "DrawToBitmap prazan " + r.Width + "x" + r.Height);
                        return;
                    }
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                ZapisiSnimku(put, ex.ToString());
            }
        }

        private static void UpisiIshod(string snimka, string tekst)
        {
            try
            {
                File.WriteAllText(snimka + ".txt", tekst);
                Console.Error.WriteLine(tekst);
            }
            catch
            {
            }
        }

        private delegate bool EnumProc(IntPtr hWnd, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumProc lpEnumFunc, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumProc lpEnumFunc, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam, uint flags, uint timeout, out IntPtr result);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        private void Postavke()
        {
            using (Postavke postavke = new Postavke())
            {
                if (postavke.ShowDialog() == DialogResult.OK)
                {
                    server = postavke.server;
                    database = postavke.database;
                    username = postavke.username;
                    password = postavke.password;
                    xml = postavke.xml;

                    operateriId = postavke.operateriId;
                    operater = postavke.operater;

                    lblKonekcija.Text = string.Format("server: {0}; baza: {1}; xml file: {2}; operater: {3} ", server, database, xml, operater.Trim());
                    cmdGen.Enabled = true;
                    isXmlGen = postavke.isXmlGen;

                    Carpo.Core.Logger.BindJupiterLogs(server, operateriId, operater, database, null, System.IO.Path.GetFileNameWithoutExtension(xml));

                    if (isXmlGen)
                        paramList = postavke.xmlGenParamList;
                    else
                        paramList = "";

                    // Force focus to the ToolStrip so the first mouse click is not consumed by focus change
                    try
                    {
                        toolStrip1.Focus();
                        toolStrip1.Refresh();
                    }
                    catch { }
                }
            }
        }

        private void cmdPostavke_Click(object sender, EventArgs e)
        {
            Postavke();
        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {
            var frmHelp = new Forms.Help();
            frmHelp.Show();
        }

        private void StartXmlGen()
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("Započinjem proces generiranja XML-a");
            splashScreenManager1.SetWaitFormCaption("Carpo.XmlGen :: Generiranje XML-a");

            try
            {
                FileFinder fileFinder = new FileFinder("Carpo.TestShell");
                XDocument xDoc = null;
                if (fileFinder.CheckFileExist(xml))
                {
                    object file = fileFinder.GetFile(xml);
                    string fileString = Encoding.UTF8.GetString((byte[])file);
                    fileString = fileString.Remove(0, fileString.IndexOf("<"));

                    xDoc = XDocument.Parse(fileString);
                }

                if (xDoc == null)
                {
                    MessageBox.Show("Neuspješno učitavanje XML-a sa putanje '" + xml + "'");
                    return;
                }

                string connStr = Carpo.Data.Constructor.SetConnectionString(this.server, this.database, this.username, this.password, "Carpo.XmlGen");

                var xmlGenFrm = new Forms.XmlGen(connStr, operateriId.ToString(), paramList, pnlShell.Handle, splashScreenManager1, XmlSchemePath: xml, XmlScheme: xDoc);

                if (xmlGenFrm.Start())
                    xmlGenFrm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "StartXmlGen()");
            }
            finally
            {
                if (splashScreenManager1.IsSplashFormVisible == true)
                {
                    System.Threading.Thread.Sleep(500);
                    splashScreenManager1.CloseWaitForm();
                }
            }
        }
    }
}