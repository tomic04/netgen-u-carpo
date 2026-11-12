using SpinDMSUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                        try
                        {
                            var dnevnik = new Carpo.Core.Logger(Carpo.Core.Log.LogLevel.Info);
                            dnevnik.GranuleName = Path.GetFileNameWithoutExtension(xml);
                            dnevnik.ServerName = server;
                            dnevnik.DatabaseName = database;
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
                            var forma = new Carpo.NetGen.Ui.Podloga();
                            if (!forma.Prikazi(xml, server, database, operateriId))
                                MessageBox.Show(forma.Text, "Carpo.NetGen");
                            forma.Show(this);
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
        public int PokreniUiSmoke(string snimka)
        {
            _uiIshod = 0;
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
                cmdGen_Click(this, EventArgs.Empty);
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
            if (granula != null)
                SnimiFormu(granula, snimka);
            try
            {
                if (granula != null && !granula.IsDisposed)
                    granula.Close();
                Close();
            }
            catch (Exception ex)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL zatvaranje " + ex);
                return 3;
            }
            UpisiIshod(snimka, "OK " + xml);
            return 0;
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
                string naslov = ProzorGreske(pid);
                if (naslov != null)
                {
                    _uiIshod = 1;
                    SnimiEkran(snimka);
                    UpisiIshod(snimka, "FAIL dijalog " + naslov);
                    Environment.Exit(1);
                }
                IntPtr prozor = ProzorGranule(pid);
                if (prozor != IntPtr.Zero)
                {
                    _uiIshod = 2;
                    Thread.Sleep(800);
                    SnimiEkran(snimka);
                    PostMessage(prozor, 0x0010, IntPtr.Zero, IntPtr.Zero);
                }
            }
            if (_uiIshod == 0)
            {
                SnimiEkran(snimka);
                UpisiIshod(snimka, "FAIL timeout");
                Environment.Exit(2);
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
                var sb = new StringBuilder(512);
                GetWindowText(h, sb, sb.Capacity);
                string t = sb.ToString();
                if (t == "Start Error" || t == "Carpo.NetGen" || t == "Greška" || t == "StartXmlGen()")
                {
                    nadjen = t;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return nadjen;
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
                var sb = new StringBuilder(512);
                GetWindowText(h, sb, sb.Capacity);
                string t = sb.ToString();
                if (t.Length == 0 || t == "TestShell")
                    return true;
                if (t == "Start Error" || t == "Carpo.NetGen" || t == "Greška")
                    return true;
                nadjen = h;
                return false;
            }, IntPtr.Zero);
            return nadjen;
        }

        private static void SnimiEkran(string put)
        {
            try
            {
                var ekran = Screen.PrimaryScreen.Bounds;
                using (var bmp = new System.Drawing.Bitmap(ekran.Width, ekran.Height))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                        g.CopyFromScreen(ekran.Location, System.Drawing.Point.Empty, ekran.Size);
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch
            {
            }
        }

        private static void SnimiFormu(Form forma, string put)
        {
            try
            {
                var r = forma.Bounds;
                if (r.Width < 50 || r.Height < 50)
                {
                    SnimiEkran(put);
                    return;
                }
                using (var bmp = new System.Drawing.Bitmap(r.Width, r.Height))
                {
                    forma.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, r.Width, r.Height));
                    bmp.Save(put, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch
            {
                SnimiEkran(put);
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
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

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