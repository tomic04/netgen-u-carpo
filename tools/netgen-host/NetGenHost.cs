// Pokretač starog NetGena (NetGenDLL.dll) izvan Jupitera, za usporedbu s Carpo.NetGen.
// Otvori granulu kroz NetGenDLL.Loader.Run8 kao Jupiter, zapiše poruke (MsgBox) i naslove dijaloga koje NetGen
// otvori, zatvori ih, snimi prozor i izađe. NetGen VB se ne mijenja; DLL se uzima iz Jupiter instalacije.
//
// NetGenHost.exe <server> <baza> <operater> <xml> <izlaz-prefiks> [sekundi] [klik]
//   xml            puna putanja XML-a granule; mapa u kojoj je kopira se u <temp>\SharedDLLs\xml (INCLUDE datoteke)
//   izlaz-prefiks  <prefiks>.txt (ishod), .poruke.txt (MsgBox i dijalozi), .png (snimka), .gumbi.txt (gumbi podloge)
//   sekundi        koliko čekati nakon otvaranja prije snimke (zadano 6)
//   klik           natpis gumba podloge koji se klikne nakon otvaranja (bez toga se ništa ne klika)
//
// Okolina: NETGEN_DLL (zadano SharedAssemblies\Spin\NetGenDLL.dll), NETGEN_SHARED (mapa SharedAssemblies),
// NETGEN_TIP (zadano NetGenDLL.Loader). Carpo.NetGen.Loader ima ista polja i Run8, pa isti pokretač otvara i
// Carpo granulu putem kojim je otvara Jupiter: NETGEN_DLL=...\Carpo.NetGen.dll, NETGEN_TIP=Carpo.NetGen.Loader.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NetGenHost
{
    internal static class Program
    {
        private const string ZadaniShared = @"C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies";

        private static string _prefiks = "";
        private static string _shared = ZadaniShared;
        private static string _mapaDll = "";
        private static Dictionary<string, string> _indeks;
        private static readonly object _brava = new object();
        private static IntPtr _glavni = IntPtr.Zero;
        private static IntPtr _granula = IntPtr.Zero;
        private static volatile bool _kraj;
        private static readonly HashSet<IntPtr> _vidjeni = new HashSet<IntPtr>();

        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.Error.WriteLine("NetGenHost.exe <server> <baza> <operater> <xml> <izlaz-prefiks> [sekundi] [klik]");
                return 2;
            }
            string server = args[0];
            string baza = args[1];
            int operater;
            int.TryParse(args[2], out operater);
            string xml = Path.GetFullPath(args[3]);
            _prefiks = Path.GetFullPath(args[4]);
            int sekundi = 6;
            if (args.Length > 5)
                int.TryParse(args[5], out sekundi);
            string klik = args.Length > 6 ? args[6] : "";
            string okolina = Environment.GetEnvironmentVariable("NETGEN_SHARED");
            if (!string.IsNullOrEmpty(okolina))
                _shared = okolina;
            string dll = Environment.GetEnvironmentVariable("NETGEN_DLL");
            if (string.IsNullOrEmpty(dll))
                dll = Path.Combine(_shared, "Spin", "NetGenDLL.dll");

            foreach (string nastavak in new[] { ".txt", ".poruke.txt", ".png", ".gumbi.txt" })
            {
                try { File.Delete(_prefiks + nastavak); } catch { }
            }
            if (!File.Exists(xml))
                return Ishod(2, "nema XML-a " + xml);
            if (!File.Exists(dll))
                return Ishod(2, "nema NetGenDLL.dll " + dll);
            _mapaDll = Path.GetDirectoryName(Path.GetFullPath(dll));
            string tip = Environment.GetEnvironmentVariable("NETGEN_TIP");
            if (string.IsNullOrEmpty(tip))
                tip = "NetGenDLL.Loader";

            AppDomain.CurrentDomain.AssemblyResolve += Razrijesi;
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += delegate (object s, ThreadExceptionEventArgs e)
            {
                Zapisi("IZNIMKA", e.Exception.GetType().Name + ": " + e.Exception.Message);
            };

            // NetGen traži XML u <remoteDllPath>\SharedDLLs\xml\<moduleId>.
            string korijen = Path.Combine(Path.GetTempPath(), "netgen-host", Guid.NewGuid().ToString("N").Substring(0, 8));
            string xmlMapa = Path.Combine(korijen, "SharedDLLs", "xml");
            Directory.CreateDirectory(xmlMapa);
            foreach (string f in Directory.GetFiles(Path.GetDirectoryName(xml), "*.xml"))
            {
                try { File.Copy(f, Path.Combine(xmlMapa, Path.GetFileName(f)), true); } catch { }
            }
            string modul = Path.GetFileName(xml);

            var forma = new Form();
            forma.Text = "NetGenHost " + modul;
            forma.StartPosition = FormStartPosition.Manual;
            forma.Location = new Point(40, 40);
            forma.Size = new Size(1500, 950);
            var ploca = new Panel();
            ploca.Dock = DockStyle.Fill;
            forma.Controls.Add(ploca);

            var cuvar = new Thread(Cuvar);
            cuvar.IsBackground = true;

            forma.Shown += delegate
            {
                _glavni = forma.Handle;
                cuvar.Start();
                forma.BeginInvoke(new Action(delegate
                {
                    string naslov = "";
                    try
                    {
                        Assembly a = Assembly.LoadFrom(dll);
                        Zapisi("DLL", dll + " " + a.GetName().Version);
                        object loader = a.CreateInstance(tip);
                        if (loader == null)
                            throw new InvalidOperationException("nema tipa " + tip);
                        Type t = loader.GetType();
                        t.GetField("moduleId").SetValue(loader, modul);
                        t.GetField("mainHandle").SetValue(loader, (int)forma.Handle);
                        t.GetField("strNGParams").SetValue(loader, "");
                        // Run8 se vrati tek kad se zatvore dijalozi koje granula otvori na ulazu (ONLOAD ":FORMA"); zatvara ih čuvar.
                        t.GetMethod("Run8").Invoke(loader, new object[] { server, baza, "", "", operater, (int)ploca.Handle, (short)0, (short)0, korijen });
                        naslov = Convert.ToString(t.GetField("WCaption").GetValue(loader));
                        _granula = new IntPtr(Convert.ToInt32(t.GetField("hWnd").GetValue(loader)));
                        Zapisi("OTVORENO", "WCaption=" + naslov + " hWnd=" + _granula.ToInt64());
                    }
                    catch (Exception ex)
                    {
                        Exception u = ex.InnerException ?? ex;
                        Zapisi("IZNIMKA", "Run8: " + u.GetType().Name + ": " + u.Message);
                    }
                    var sat = new System.Windows.Forms.Timer();
                    sat.Interval = Math.Max(1, sekundi) * 1000;
                    bool kliknuto = klik.Length == 0;
                    sat.Tick += delegate
                    {
                        if (!kliknuto)
                        {
                            kliknuto = true;
                            PopisiGumbe();
                            Zapisi("KLIK", klik + " -> " + (Klikni(klik) ? "poslano" : "gumb nije nađen"));
                            return;
                        }
                        sat.Stop();
                        PopisiGumbe();
                        Snimi(forma);
                        _kraj = true;
                        Ishod(0, "OK " + naslov);
                        Environment.Exit(0);
                    };
                    sat.Start();
                }));
            };
            Application.Run(forma);
            return 0;
        }

        private static int Ishod(int kod, string tekst)
        {
            try { File.WriteAllText(_prefiks + ".txt", tekst + "\r\n", Encoding.UTF8); } catch { }
            Console.WriteLine(tekst);
            return kod;
        }

        private static void Zapisi(string vrsta, string tekst)
        {
            string red = DateTime.Now.ToString("HH:mm:ss.fff") + "\t" + vrsta + "\t" + (tekst ?? "").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            lock (_brava)
            {
                try { File.AppendAllText(_prefiks + ".poruke.txt", red + "\r\n", Encoding.UTF8); } catch { }
            }
        }

        private static Assembly Razrijesi(object sender, ResolveEventArgs e)
        {
            string ime = new AssemblyName(e.Name).Name;
            if (ime.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) || ime.EndsWith(".XmlSerializers", StringComparison.OrdinalIgnoreCase))
                return null;
            lock (_brava)
            {
                if (_indeks == null)
                {
                    _indeks = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    // Spin prvi: ondje je NetGenDLL i njegove ovisnosti u verziji koju Jupiter stvarno koristi.
                    // Mapa samog DLL-a prva: ondje su ovisnosti u verziji s kojom je izgrađen.
                    foreach (string mapa in new[] { _mapaDll, Path.Combine(_shared, "Spin"), Path.Combine(_shared, "Janus"), Path.Combine(_shared, "DevExpress"), Path.Combine(_shared, "Microsoft"), _shared })
                    {
                        if (string.IsNullOrEmpty(mapa) || !Directory.Exists(mapa))
                            continue;
                        foreach (string f in Directory.GetFiles(mapa, "*.dll", mapa == _mapaDll ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories))
                        {
                            string k = Path.GetFileNameWithoutExtension(f);
                            if (!_indeks.ContainsKey(k))
                                _indeks[k] = f;
                        }
                    }
                }
            }
            string put;
            if (!_indeks.TryGetValue(ime, out put))
                return null;
            try { return Assembly.LoadFrom(put); }
            catch { return null; }
        }

        // ---- čuvar: poruke i dijalozi koje NetGen otvori ----

        private static void Cuvar()
        {
            uint pid = (uint)Process.GetCurrentProcess().Id;
            while (!_kraj)
            {
                Thread.Sleep(250);
                var prozori = new List<IntPtr>();
                EnumWindows(delegate (IntPtr h, IntPtr l)
                {
                    uint p;
                    GetWindowThreadProcessId(h, out p);
                    if (p == pid && IsWindowVisible(h) && h != _glavni)
                        prozori.Add(h);
                    return true;
                }, IntPtr.Zero);
                foreach (IntPtr h in prozori)
                {
                    string klasa = Klasa(h);
                    string naslov = Tekst(h);
                    if (klasa == "#32770")
                    {
                        // MsgBox: tekst je u Static kontrolama. Zatvara se gumbom koji ništa ne potvrđuje.
                        var dijelovi = new List<string>();
                        var gumbi = new List<IntPtr>();
                        EnumChildWindows(h, delegate (IntPtr c, IntPtr l)
                        {
                            string k = Klasa(c);
                            if (k == "Static")
                            {
                                string s = Tekst(c);
                                if (s.Length > 0)
                                    dijelovi.Add(s);
                            }
                            else if (k == "Button")
                                gumbi.Add(c);
                            return true;
                        }, IntPtr.Zero);
                        var natpisi = new List<string>();
                        foreach (IntPtr g in gumbi)
                            natpisi.Add(Tekst(g).Replace("&", ""));
                        lock (_vidjeni)
                        {
                            if (_vidjeni.Add(h))
                                Zapisi("MSGBOX", "[" + naslov + "] " + string.Join(" ", dijelovi.ToArray()) + " {" + string.Join("|", natpisi.ToArray()) + "}");
                        }
                        IntPtr odabran = IntPtr.Zero;
                        foreach (string zeljeni in new[] { "No", "Ne", "Cancel", "Odustani", "OK", "U redu" })
                        {
                            for (int i = 0; i < gumbi.Count && odabran == IntPtr.Zero; i++)
                            {
                                if (string.Equals(natpisi[i], zeljeni, StringComparison.OrdinalIgnoreCase))
                                    odabran = gumbi[i];
                            }
                            if (odabran != IntPtr.Zero)
                                break;
                        }
                        if (odabran == IntPtr.Zero && gumbi.Count > 0)
                            odabran = gumbi[gumbi.Count - 1];
                        if (odabran != IntPtr.Zero)
                            PostMessage(odabran, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                        else
                            PostMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        continue;
                    }
                    // Dijalog granule (Form2) ili drugi prozor NetGena: zabilježi i zatvori, da Run8 može završiti.
                    if (GetWindow(h, GW_OWNER) == IntPtr.Zero && GetParent(h) == IntPtr.Zero && h == _granula)
                        continue;
                    if (naslov.Length == 0 && klasa.IndexOf("WindowsForms", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    bool novi;
                    lock (_vidjeni)
                        novi = _vidjeni.Add(h);
                    if (novi)
                    {
                        Zapisi("DIJALOG", "[" + naslov + "] " + klasa);
                        // Kratko pričekaj da se dijalog napuni (njegov SQL), pa ga zatvori.
                        Thread.Sleep(1200);
                    }
                    PostMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        // ---- gumbi podloge (Form1) ----

        private static List<Control> GumbiPodloge()
        {
            var svi = new List<Control>();
            if (_granula == IntPtr.Zero)
                return svi;
            Control podloga = Control.FromHandle(_granula);
            if (podloga == null)
                return svi;
            var red = new Queue<Control>();
            red.Enqueue(podloga);
            while (red.Count > 0)
            {
                Control c = red.Dequeue();
                if (c is ButtonBase && c.Visible && (c.Text ?? "").Trim().Length > 0)
                    svi.Add(c);
                foreach (Control d in c.Controls)
                    red.Enqueue(d);
            }
            return svi;
        }

        private static void PopisiGumbe()
        {
            try
            {
                var sb = new StringBuilder();
                foreach (Control c in GumbiPodloge())
                    sb.AppendLine(c.Name + "\t" + c.Text.Replace("\r", " ").Replace("\n", " ") + "\t" + (c.Enabled ? "enabled" : "disabled"));
                File.WriteAllText(_prefiks + ".gumbi.txt", sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Zapisi("IZNIMKA", "gumbi: " + ex.Message);
            }
        }

        private static bool Klikni(string natpis)
        {
            foreach (Control c in GumbiPodloge())
            {
                if (string.Equals(c.Text.Replace("&", "").Trim(), natpis.Trim(), StringComparison.OrdinalIgnoreCase) && c.Enabled)
                {
                    // BeginInvoke: klik može otvoriti modalni dijalog, a čuvar ga zatvara s druge niti.
                    Control gumb = c;
                    gumb.BeginInvoke(new Action(delegate { ((ButtonBase)gumb).PerformClickSigurno(); }));
                    return true;
                }
            }
            return false;
        }

        private static void PerformClickSigurno(this ButtonBase gumb)
        {
            var b = gumb as Button;
            if (b != null)
            {
                b.PerformClick();
                return;
            }
            MethodInfo m = typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
            m.Invoke(gumb, new object[] { EventArgs.Empty });
        }

        private static void Snimi(Form forma)
        {
            try
            {
                using (var bmp = new Bitmap(forma.Width, forma.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                        g.CopyFromScreen(forma.Location, Point.Empty, forma.Size);
                    bmp.Save(_prefiks + ".png", ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                Zapisi("IZNIMKA", "snimka: " + ex.Message);
            }
        }

        // ---- Win32 ----

        private const uint WM_CLOSE = 0x0010;
        private const uint BM_CLICK = 0x00F5;
        private const uint GW_OWNER = 4;

        private delegate bool EnumProc(IntPtr h, IntPtr l);

        [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc f, IntPtr l);
        [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr h, EnumProc f, IntPtr l);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr h, uint cmd);
        [DllImport("user32.dll")] private static extern IntPtr GetParent(IntPtr h);
        [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr h, uint msg, IntPtr w, IntPtr l);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr h, StringBuilder sb, int n);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr h, StringBuilder sb, int n);
        [DllImport("user32.dll")] private static extern int GetWindowTextLength(IntPtr h);

        private static string Klasa(IntPtr h)
        {
            var sb = new StringBuilder(256);
            GetClassName(h, sb, sb.Capacity);
            return sb.ToString();
        }

        private static string Tekst(IntPtr h)
        {
            int n = GetWindowTextLength(h);
            var sb = new StringBuilder(n + 2);
            GetWindowText(h, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
