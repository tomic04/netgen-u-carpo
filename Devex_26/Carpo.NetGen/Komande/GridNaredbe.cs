using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// RECALCGRID Form2.vb 8219–8261. Stupac čiji natpis počinje s eval dobije izraz u ćeliji.
    /// Ime odredišta je natpis bez "eval" i bez zadnja dva znaka.
    /// </summary>
    public static class RecalcGrid
    {
        public static int Izracunaj(DataTable tablica)
        {
            if (tablica == null)
                return 0;
            int n = 0;
            for (int r = 0; r < tablica.Rows.Count; r++)
            {
                DataRow red = tablica.Rows[r];
                for (int c = 0; c < tablica.Columns.Count; c++)
                {
                    string natpis = (tablica.Columns[c].Caption ?? "").ToLowerInvariant();
                    if (!natpis.StartsWith("eval", StringComparison.Ordinal) || natpis.Length < 6)
                        continue;
                    string ime = natpis.Substring(4, natpis.Length - 4);
                    ime = ime.Substring(0, ime.Length - 2);
                    if (!tablica.Columns.Contains(ime))
                        continue;
                    string izraz = red[c] == DBNull.Value ? "" : Convert.ToString(red[c]);
                    izraz = (izraz ?? "").Replace("$$", "@@");
                    string v = Izraz.Izracunaj(izraz);
                    if (v.Length == 0)
                        continue;
                    red[ime] = v.Replace(",", ".");
                    n++;
                }
            }
            return n;
        }
    }

    /// <summary>TOGGLEGRIDGROUP Form2.vb 6970–6983. c=0 širi, inače sklapa.</summary>
    public static class ToggleGrid
    {
        public static string Korak(ref int stanje)
        {
            if (stanje == 0)
            {
                stanje = 1;
                return "expand";
            }
            stanje = 0;
            return "collapse";
        }
    }

    /// <summary>PREPAREMULTISELECTEDPRINT Form2.vb 8404–8431. Prvi id postaje ID.</summary>
    public static class MultiIspis
    {
        public static string[] Idjevi(IList<string> cekirani)
        {
            if (cekirani == null || cekirani.Count == 0)
                return new string[0];
            var o = new string[cekirani.Count];
            for (int i = 0; i < cekirani.Count; i++)
                o[i] = (cekirani[i] ?? "").Trim();
            return o;
        }

        public static string Prvi(IList<string> cekirani)
        {
            string[] a = Idjevi(cekirani);
            return a.Length == 0 ? "" : a[0];
        }
    }

    /// <summary>COLORPICKER Form2.vb 7156–7163 i Form1.vb 6813–6831. Boja ide u HTML.</summary>
    public static class BojaNaredba
    {
        public static string Odabrana = "";

        /// <summary>Form2.vb 7160–7162. ShowDialog je 1 (OK) pa HTML boja u @@selectedcolor.</summary>
        public static string Pokazi()
        {
            using (var d = new ColorDialog())
            {
                if (d.ShowDialog() == DialogResult.OK)
                    Odabrana = ColorTranslator.ToHtml(d.Color);
            }
            return Odabrana ?? "";
        }

        public static string Html(Color boja)
        {
            return ColorTranslator.ToHtml(boja);
        }

        public static string Sql(string segment)
        {
            string u = (segment ?? "").Trim();
            int a = u.IndexOf("COLORPICKER(", StringComparison.OrdinalIgnoreCase);
            if (a < 0)
                return "";
            u = u.Substring(a + "COLORPICKER(".Length);
            int z = u.LastIndexOf(')');
            if (z >= 0)
                u = u.Substring(0, z);
            return u.Trim();
        }
    }

    /// <summary>POSALJI*NASERVER Form2.vb 7759 i 8008. URL se slaže, POST ide istim upitom.</summary>
    public static class PosaljiServer
    {
        public static string Vrsta(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            if (u.IndexOf("POSALJIRACUNENASERVER(", StringComparison.Ordinal) >= 0)
                return "racun";
            if (u.IndexOf("POSALJIZAKLJUCKENASERVER(", StringComparison.Ordinal) >= 0)
                return "zakljucak";
            if (u.IndexOf("POSALJIPORUKENASERVER(", StringComparison.Ordinal) >= 0)
                return "poruka";
            return "";
        }

        public static string UrlRacuna(string servis, string trgovina, string kasa, string godina, string broj, string vrijeme, string jir, string datum)
        {
            string v = string.IsNullOrEmpty(vrijeme) ? "null" : "'" + vrijeme + "'";
            string j = string.IsNullOrEmpty(jir) ? "null" : "'" + jir + "'";
            return (servis ?? "").Trim() + "xmlrec.asp?a=1&v=1&g=5555555&oznakatrgovine=" + (trgovina ?? "").ToUpperInvariant()
                + "&kasa=" + (kasa ?? "").ToUpperInvariant() + "&godina=" + godina + "&brojdok=" + broj
                + "&vrijemejir=" + v + "&jir=" + j + "&datumdok=" + datum;
        }

        public static string UrlPoruke(string servis, string trgovina, string kasa, string godina, string broj, string datum)
        {
            return (servis ?? "").Trim() + "xmlrec.asp?a=1&v=3&g=5555555&oznakatrgovine=" + (trgovina ?? "").ToUpperInvariant()
                + "&kasa=" + (kasa ?? "").ToUpperInvariant() + "&godina=" + godina
                + "&vrijemejir=null&jir=null&brojdok=" + broj + "&datumdok=" + datum;
        }

        /// <summary>Form2.vb 7770–7790. POST text/xml, timeout 3000, vraća tijelo odgovora.</summary>
        public static string Post(string url, string tijelo)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(tijelo ?? "");
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "text/xml";
            req.Method = "POST";
            req.ContentLength = bytes.Length;
            req.Timeout = 3000;
            using (Stream os = req.GetRequestStream())
                os.Write(bytes, 0, bytes.Length);
            using (WebResponse resp = req.GetResponse())
            using (var sr = new StreamReader(resp.GetResponseStream()))
                return sr.ReadToEnd().Trim();
        }
    }

    /// <summary>.VBS i .BAT Form2.vb 7402–7418. Jedan ~ dijeli datoteku i argument.</summary>
    public static class Pokretanje
    {
        public static bool Je(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            return u.IndexOf(".VBS", StringComparison.Ordinal) >= 0 || u.IndexOf(".BAT", StringComparison.Ordinal) >= 0;
        }

        public static string Datoteka(string segment)
        {
            string[] a = (segment ?? "").Split('~');
            return a.Length == 0 ? "" : a[0].Trim();
        }

        public static string Argument(string segment)
        {
            string[] a = (segment ?? "").Split('~');
            return a.Length > 1 ? a[1].Trim() : "";
        }

        /// <summary>Form2.vb 7408–7417. Process.Start(datoteka) ili Process.Start(datoteka, argument).</summary>
        public static int Pokreni(string segment)
        {
            string dat = Datoteka(segment);
            string arg = Argument(segment);
            if (dat.Length == 0)
                return -1;
            var info = new ProcessStartInfo();
            info.FileName = dat;
            info.Arguments = arg;
            info.UseShellExecute = true;
            info.CreateNoWindow = true;
            Process p = arg.Length == 0 ? Process.Start(dat) : Process.Start(info);
            if (p == null)
                return -1;
            if (!p.WaitForExit(8000))
                return -2;
            return p.ExitCode;
        }
    }

    /// <summary>CHARTGRID i PIVOTGRID Form2.vb 9062–9102. NetGenExtensions.Form2.ChartGRID i Form1.PivotGRID.</summary>
    public static class EkstenzijaGrid
    {
        public const string Sklop = "NetGenExtensions";
        public const string ChartKlasa = "NetGenExtensions.Form2";
        public const string PivotKlasa = "NetGenExtensions.Form1";
        public const string ChartMetoda = "ChartGRID";
        public const string PivotMetoda = "PivotGRID";
        public const string IspisMetoda = "PrintDocumentREPX";
        public const string IspisKlasa = "NetGenExtensions.frmGridExPrintingREPX";
        public static Form Zadnja;
        public static string ZadnjiTip = "";
        public static string Greska = "";
        public static string Poziv = "";
        public static string Trag = "";
        private static Dictionary<string, string> _datoteke;
        private static readonly HashSet<string> _uTijeku = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static bool _kuka;

        /// <summary>
        /// Form1.vb 5988 i Form2.vb 9044, 9065, 9088: Assembly.Load("NetGenExtensions").
        /// Prazan string ako DLL nije uz exe ni u Jupiter Clientu.
        /// </summary>
        public static string Nadji()
        {
            Indeks();
            return PutDatoteke(Sklop + ".dll");
        }

        public static string PutDatoteke(string ime)
        {
            Indeks();
            string put;
            if (_datoteke != null && _datoteke.TryGetValue(ime, out put))
                return put;
            return "";
        }

        public static string Ucitaj()
        {
            string put = Nadji();
            if (put.Length == 0)
                return "";
            try
            {
                Kuka();
                Assembly.Load(Sklop);
                return put;
            }
            catch (Exception ex)
            {
                return "greska " + ex.Message;
            }
        }

        /// <summary>
        /// Assembly.Load kao NetGen, pa CreateInstance tipa koji ima metodu i poziv te metode.
        /// Form2.vb 9048–9102.
        /// </summary>
        public static Form Prikazi(string vrsta)
        {
            string metoda = vrsta == "pivot" ? PivotMetoda : vrsta == "chart" ? ChartMetoda : IspisMetoda;
            Zadnja = null;
            ZadnjiTip = "";
            Assembly a = UcitajSklop();
            if (a == null)
                return null;
            Type t = TipSMetodom(a, metoda);
            if (t == null)
            {
                Greska = (Greska + " | nema " + metoda + " janus=" + PutDatoteke("Janus.Windows.GridEX.v3.dll")).Trim();
                return null;
            }
            object o;
            try
            {
                o = Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                Greska = (Greska + " | " + t.FullName + " " + Unutra(ex).GetType().Name + " " + Unutra(ex).Message).Trim();
                return null;
            }
            ZadnjiTip = t.FullName ?? "";
            Zadnja = o as Form;
            Pozovi(o, metoda);
            return Zadnja;
        }

        private static Assembly UcitajSklop()
        {
            Indeks();
            Kuka();
            try
            {
                return Assembly.Load(Sklop);
            }
            catch (Exception ex)
            {
                string put = PutDatoteke(Sklop + ".dll");
                if (put.Length == 0)
                {
                    Greska = (Greska + " | " + ex.Message).Trim();
                    return null;
                }
                try
                {
                    return Assembly.LoadFrom(put);
                }
                catch (Exception ex2)
                {
                    Greska = (Greska + " | " + ex2.Message).Trim();
                    return null;
                }
            }
        }

        private static void Kuka()
        {
            if (_kuka)
                return;
            _kuka = true;
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object s, ResolveEventArgs e)
            {
                string ime = new AssemblyName(e.Name).Name;
                if (ime == null || _uTijeku.Contains(ime))
                    return null;
                string put = PutDatoteke(ime + ".dll");
                if (put.Length == 0)
                    return null;
                _uTijeku.Add(ime);
                try
                {
                    return Assembly.LoadFrom(put);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    _uTijeku.Remove(ime);
                }
            };
        }

        private static Type TipSMetodom(Assembly a, string metoda)
        {
            Type[] tipovi;
            try
            {
                tipovi = a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                tipovi = ex.Types ?? new Type[0];
                if (ex.LoaderExceptions != null)
                {
                    for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                    {
                        if (ex.LoaderExceptions[i] != null)
                            Greska = (Greska + " | " + ex.LoaderExceptions[i].Message).Trim();
                    }
                }
            }
            for (int i = 0; i < tipovi.Length; i++)
            {
                Type t = tipovi[i];
                if (t == null)
                    continue;
                try
                {
                    if (t.GetMethod(metoda) != null)
                        return t;
                }
                catch (Exception ex)
                {
                    Greska = (Greska + " | " + t.FullName + " " + ex.Message).Trim();
                }
            }
            return null;
        }

        private static void Pozovi(object o, string metoda)
        {
            MethodInfo m;
            try
            {
                m = o.GetType().GetMethod(metoda);
            }
            catch (Exception ex)
            {
                Poziv = (Poziv + " " + metoda + "=err " + ex.Message).Trim();
                return;
            }
            if (m == null)
            {
                Poziv = (Poziv + " " + metoda + "=nema").Trim();
                return;
            }
            ParameterInfo[] p = m.GetParameters();
            object[] args = new object[p.Length];
            var tablica = new DataTable();
            tablica.Columns.Add("id", typeof(int));
            tablica.Columns.Add("naziv", typeof(string));
            tablica.Rows.Add(1, "a");
            for (int i = 0; i < p.Length; i++)
            {
                Type pt = p[i].ParameterType;
                if (pt == typeof(string))
                    args[i] = i == 0 ? "lodani" : "";
                else if (pt == typeof(DataTable))
                    args[i] = tablica;
                else if (pt == typeof(int))
                    args[i] = 0;
                else if (pt == typeof(bool))
                    args[i] = false;
                else
                    args[i] = null;
            }
            try
            {
                m.Invoke(o, args);
                Poziv = (Poziv + " " + metoda + "=ok").Trim();
            }
            catch (Exception ex)
            {
                Exception u = Unutra(ex);
                Poziv = (Poziv + " " + metoda + "=err " + u.GetType().Name + " " + u.Message).Trim();
                Greska = (Greska + " | " + metoda + " " + u.Message).Trim();
            }
        }

        private static Exception Unutra(Exception ex)
        {
            Exception u = ex;
            while (u.InnerException != null)
                u = u.InnerException;
            return u;
        }

        private static void Indeks()
        {
            if (_datoteke != null)
                return;
            _datoteke = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var mape = new List<string>();
            mape.Add(AppDomain.CurrentDomain.BaseDirectory ?? "");
            string env = Environment.GetEnvironmentVariable("SharedAssembliesDir");
            if (!string.IsNullOrEmpty(env))
            {
                mape.Add(env);
                mape.Add(Path.Combine(env, "Spin"));
            }
            mape.Add(@"C:\Users\ntomic\Desktop\Jupiter Client");
            for (int i = 0; i < mape.Count; i++)
            {
                if (mape[i].Length == 0 || !Directory.Exists(mape[i]))
                    continue;
                if (mape[i].IndexOf("Jupiter Client", StringComparison.OrdinalIgnoreCase) >= 0)
                    Obidi(mape[i], 0);
                else
                    DodajMapu(mape[i]);
            }
            string[] gac = new[]
            {
                @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL",
                @"C:\Windows\Microsoft.NET\assembly\GAC_32",
                @"C:\Windows\Microsoft.NET\assembly\GAC_64",
                @"C:\Windows\assembly\GAC_MSIL",
                @"C:\Windows\assembly\GAC_32",
                @"C:\Windows\assembly\GAC_64"
            };
            for (int i = 0; i < gac.Length; i++)
            {
                if (!Directory.Exists(gac[i]))
                    continue;
                string[] janus;
                try
                {
                    janus = Directory.GetDirectories(gac[i], "Janus*", SearchOption.AllDirectories);
                }
                catch
                {
                    continue;
                }
                for (int j = 0; j < janus.Length; j++)
                    DodajMapu(janus[j]);
            }
            string[] gacDx = new[]
            {
                @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL",
                @"C:\Windows\assembly\GAC_MSIL"
            };
            for (int i = 0; i < gacDx.Length; i++)
            {
                if (!Directory.Exists(gacDx[i]))
                    continue;
                string[] dx;
                try
                {
                    dx = Directory.GetDirectories(gacDx[i], "DevExpress.Reports.v12*", SearchOption.TopDirectoryOnly);
                }
                catch
                {
                    continue;
                }
                for (int j = 0; j < dx.Length; j++)
                    Obidi(dx[j], 0);
            }
            Dopuni();
            string pogodak = "";
            foreach (KeyValuePair<string, string> kv in _datoteke)
            {
                if (kv.Key.IndexOf("Reports.v12", StringComparison.OrdinalIgnoreCase) >= 0)
                    pogodak = pogodak + " " + kv.Value;
            }
            Trag = (Trag + " reports=" + pogodak.Trim()).Trim();
        }

        /// <summary>
        /// NetGen.exe.config privatePath i codebase. Form2.vb 9044 radi Assembly.Load u procesu NetGena,
        /// pa ovisnosti (DevExpress 12.1) stoje uz exe, ne uz Carpo.
        /// </summary>
        private static void Dopuni()
        {
            if (_datoteke.ContainsKey("DevExpress.Reports.v12.1.Designer.dll"))
                return;
            string exe = "";
            string dizajner = "";
            string[] korijeni = new[]
            {
                @"C:\Users\ntomic\Desktop\Jupiter Client",
                @"C:\Users\ntomic\Desktop",
                @"C:\Program Files (x86)\DevExpress 12.1",
                @"C:\Program Files (x86)\DevExpress",
                @"C:\Program Files\DevExpress 12.1"
            };
            for (int k = 0; k < korijeni.Length && (exe.Length == 0 || dizajner.Length == 0); k++)
            {
                if (!Directory.Exists(korijeni[k]))
                    continue;
                var red = new Queue<string>();
                red.Enqueue(korijeni[k]);
                int n = 0;
                while (red.Count > 0 && n < 8000 && (exe.Length == 0 || dizajner.Length == 0))
                {
                    string mapa = red.Dequeue();
                    n++;
                    string[] datoteke;
                    try
                    {
                        datoteke = Directory.GetFiles(mapa);
                    }
                    catch
                    {
                        continue;
                    }
                    for (int i = 0; i < datoteke.Length; i++)
                    {
                        string ime = Path.GetFileName(datoteke[i]);
                        if (exe.Length == 0 && ime.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                            && ime.StartsWith("NetGen", StringComparison.OrdinalIgnoreCase)
                            && ime.IndexOf("Extensions", StringComparison.OrdinalIgnoreCase) < 0)
                            exe = datoteke[i];
                        if (dizajner.Length == 0 && string.Equals(ime, "DevExpress.Reports.v12.1.Designer.dll", StringComparison.OrdinalIgnoreCase))
                            dizajner = datoteke[i];
                    }
                    string[] pod;
                    try
                    {
                        pod = Directory.GetDirectories(mapa);
                    }
                    catch
                    {
                        continue;
                    }
                    for (int i = 0; i < pod.Length; i++)
                    {
                        string ime = Path.GetFileName(pod[i]);
                        if (string.Equals(ime, "node_modules", StringComparison.OrdinalIgnoreCase) || ime.StartsWith(".", StringComparison.Ordinal))
                            continue;
                        red.Enqueue(pod[i]);
                    }
                }
            }
            if (exe.Length > 0)
            {
                string mapaExe = Path.GetDirectoryName(exe) ?? "";
                Obidi(mapaExe, 0);
                Konfig(exe + ".config", mapaExe);
            }
            if (dizajner.Length > 0)
                DodajMapu(Path.GetDirectoryName(dizajner) ?? "");
            Trag = "exe=" + exe + " designer=" + dizajner;
        }

        private static void Konfig(string put, string mapaExe)
        {
            if (!File.Exists(put))
                return;
            string t;
            try
            {
                t = File.ReadAllText(put);
            }
            catch
            {
                return;
            }
            int i = 0;
            while (true)
            {
                int a = t.IndexOf("privatePath", i, StringComparison.OrdinalIgnoreCase);
                if (a < 0)
                    break;
                int q1 = t.IndexOf('"', a);
                int q2 = q1 < 0 ? -1 : t.IndexOf('"', q1 + 1);
                if (q1 < 0 || q2 < 0)
                    break;
                string[] dijelovi = t.Substring(q1 + 1, q2 - q1 - 1).Split(';');
                for (int d = 0; d < dijelovi.Length; d++)
                {
                    string rel = dijelovi[d].Trim();
                    if (rel.Length == 0)
                        continue;
                    string aps = Path.IsPathRooted(rel) ? rel : Path.Combine(mapaExe, rel);
                    if (Directory.Exists(aps))
                        Obidi(aps, 0);
                }
                i = q2 + 1;
            }
            int h = 0;
            while (true)
            {
                int a = t.IndexOf("file:///", h, StringComparison.OrdinalIgnoreCase);
                if (a < 0)
                    break;
                int z = t.IndexOf('"', a);
                if (z < 0)
                    break;
                string dat = t.Substring(a + "file:///".Length, z - a - "file:///".Length).Replace('/', '\\');
                if (File.Exists(dat))
                {
                    string ime = Path.GetFileName(dat);
                    if (!_datoteke.ContainsKey(ime))
                        _datoteke[ime] = dat;
                    DodajMapu(Path.GetDirectoryName(dat) ?? "");
                }
                h = z + 1;
            }
        }

        private static void DodajMapu(string mapa)
        {
            string[] datoteke;
            try
            {
                datoteke = Directory.GetFiles(mapa, "*.dll");
            }
            catch
            {
                return;
            }
            for (int i = 0; i < datoteke.Length; i++)
            {
                string ime = Path.GetFileName(datoteke[i]);
                if (!_datoteke.ContainsKey(ime))
                    _datoteke[ime] = datoteke[i];
            }
        }

        private static void Obidi(string mapa, int dubina)
        {
            if (dubina > 7 || _datoteke.Count > 20000)
                return;
            DodajMapu(mapa);
            string[] mape;
            try
            {
                mape = Directory.GetDirectories(mapa);
            }
            catch
            {
                return;
            }
            for (int i = 0; i < mape.Length; i++)
                Obidi(mape[i], dubina + 1);
        }
    }
}
