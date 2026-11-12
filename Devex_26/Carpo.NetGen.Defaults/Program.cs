using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Carpo.NetGen.Defaults
{
    /// <summary>
    /// Na runneru: refleksija svjezeg Janus GridEX i DevExpress GridView.
    /// Ne pokrece granulu i ne dira bazu.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            string jupiter = Arg(args, "--jupiter");
            string dx = Arg(args, "--dx");
            string izlaz = Arg(args, "--izlaz");
            if (string.IsNullOrEmpty(jupiter))
                jupiter = @"C:\Users\ntomic\Desktop\Jupiter Client";
            if (string.IsNullOrEmpty(dx))
                dx = @"C:\Program Files\DevExpress 26.1\Components\Bin\Framework";
            if (string.IsNullOrEmpty(izlaz))
                izlaz = "grid-defaults.tsv";

            var sb = new StringBuilder();
            sb.AppendLine("kontrola\tsvojstvo\tdefault\tgreska");
            var dirs = new List<string>();
            dirs.Add(dx);
            dirs.Add(jupiter);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object s, ResolveEventArgs e)
            {
                string ime = new AssemblyName(e.Name).Name + ".dll";
                for (int i = 0; i < dirs.Count; i++)
                {
                    string hit = Pronadji(dirs[i], ime);
                    if (hit != null)
                        return Assembly.LoadFrom(hit);
                }
                return null;
            };

            string janus = Pronadji(jupiter, "Janus.Windows.GridEX.v3.dll");
            if (janus == null)
                janus = Pronadji(jupiter, "Janus.Windows.GridEX.dll");
            sb.AppendLine("trazeno\tJanus.dll\t" + (janus ?? "(nema)") + "\t");
            if (janus != null)
            {
                Ispisi(sb, janus, "Janus.Windows.GridEX.GridEX");
                Ispisi(sb, janus, "Janus.Windows.GridEX.EditControls.NumericEditBox");
            }

            string dxGrid = Path.Combine(dx, "DevExpress.XtraGrid.v26.1.dll");
            sb.AppendLine("trazeno\tDX.dll\t" + dxGrid + "\t" + (File.Exists(dxGrid) ? "" : "nema"));
            if (File.Exists(dxGrid))
            {
                Ispisi(sb, dxGrid, "DevExpress.XtraGrid.GridControl");
                Ispisi(sb, dxGrid, "DevExpress.XtraGrid.Views.Grid.GridView");
            }

            string asmIzv = Path.Combine(Path.GetDirectoryName(izlaz) ?? ".", "asm-sonda.txt");
            File.WriteAllText(asmIzv, SondaAsm(jupiter), new UTF8Encoding(false));
            string dir = Path.GetDirectoryName(Path.GetFullPath(izlaz));
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(izlaz, sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine("defaults " + izlaz);
            Console.WriteLine("asm " + asmIzv);
            return 0;
        }

        private static void Ispisi(StringBuilder sb, string dll, string tip)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(dll);
                Type t = a.GetType(tip, false);
                if (t == null)
                {
                    sb.AppendLine(tip + "\t(tip)\t\tnije u " + dll);
                    return;
                }
                object inst = Activator.CreateInstance(t);
                PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Array.Sort(props, delegate(PropertyInfo x, PropertyInfo y) { return string.Compare(x.Name, y.Name, StringComparison.Ordinal); });
                for (int i = 0; i < props.Length; i++)
                {
                    if (!props[i].CanRead || props[i].GetIndexParameters().Length > 0)
                        continue;
                    string val = "";
                    string err = "";
                    try
                    {
                        object v = props[i].GetValue(inst, null);
                        val = v == null ? "(null)" : v.ToString();
                    }
                    catch (Exception ex)
                    {
                        err = ex.GetBaseException().GetType().Name + ": " + ex.GetBaseException().Message;
                    }
                    val = val.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                    err = err.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                    sb.AppendLine(tip + "\t" + props[i].Name + "\t" + val + "\t" + err);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(tip + "\t(instanca)\t\t" + ex.GetBaseException().Message.Replace("\t", " "));
            }
        }

        private static string SondaAsm(string korijen)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Trazim UTF-16 'asm:' u " + korijen);
            if (!Directory.Exists(korijen))
            {
                sb.AppendLine("Nema mape.");
                return sb.ToString();
            }
            byte[] uzorak = Encoding.Unicode.GetBytes("asm:");
            int nadeno = 0;
            foreach (string f in Directory.GetFiles(korijen, "*.*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(f).ToLowerInvariant();
                if (ext != ".dll" && ext != ".exe")
                    continue;
                try
                {
                    byte[] b = File.ReadAllBytes(f);
                    if (IndexOf(b, uzorak) < 0)
                        continue;
                    nadeno++;
                    sb.AppendLine("HIT " + f);
                    if (nadeno <= 8)
                        ProbajTipove(sb, f);
                }
                catch (Exception ex)
                {
                    sb.AppendLine("SKIP " + f + " " + ex.GetBaseException().Message);
                }
            }
            sb.AppendLine("HIT_COUNT=" + nadeno.ToString());
            sb.AppendLine("Zakljucak se cita iz metoda koje u imenu imaju asm/Parametar/Menu. Ako Load uzme prvi segment, assembly je lijevo od prve tocke. Ako uzme zadnju, Carpo.NetGen.Loader je assembly Carpo.NetGen i klasa Loader.");
            return sb.ToString();
        }

        private static void ProbajTipove(StringBuilder sb, string dll)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(dll);
                Type[] tipovi;
                try { tipovi = a.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { tipovi = ex.Types; }
                int n = 0;
                for (int i = 0; tipovi != null && i < tipovi.Length; i++)
                {
                    if (tipovi[i] == null)
                        continue;
                    string tn = tipovi[i].FullName ?? "";
                    if (tn.IndexOf("asm", StringComparison.OrdinalIgnoreCase) < 0
                        && tn.IndexOf("Meni", StringComparison.OrdinalIgnoreCase) < 0
                        && tn.IndexOf("Parametar", StringComparison.OrdinalIgnoreCase) < 0
                        && tn.IndexOf("Granul", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    sb.AppendLine("  TIP " + tn);
                    MethodInfo[] ms = tipovi[i].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    for (int m = 0; m < ms.Length && n < 40; m++)
                    {
                        sb.AppendLine("    " + ms[m].Name);
                        n++;
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("  LOAD " + ex.GetBaseException().Message);
            }
        }

        private static int IndexOf(byte[] hay, byte[] needle)
        {
            for (int i = 0; i + needle.Length <= hay.Length; i++)
            {
                int j = 0;
                for (; j < needle.Length; j++)
                {
                    if (hay[i + j] != needle[j])
                        break;
                }
                if (j == needle.Length)
                    return i;
            }
            return -1;
        }

        private static string Pronadji(string korijen, string ime)
        {
            if (string.IsNullOrEmpty(korijen) || !Directory.Exists(korijen))
                return null;
            string direktno = Path.Combine(korijen, ime);
            if (File.Exists(direktno))
                return direktno;
            foreach (string f in Directory.GetFiles(korijen, ime, SearchOption.AllDirectories))
                return f;
            return null;
        }

        private static string Arg(string[] args, string ime)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], ime, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            }
            return null;
        }
    }
}
