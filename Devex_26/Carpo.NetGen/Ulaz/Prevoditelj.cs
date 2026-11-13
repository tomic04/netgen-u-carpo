using System;
using System.IO;
using System.Reflection

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// Form1.Run 1106–1115. Assembly.Load("JupiterTranslator10"), tip JupiterTranslator.Translator.
    /// Pad učitavanja ne prekida Run (On Error Resume Next). Baza jupiter2019/2022 se ne otvara.
    /// </summary>
    public static class Prevoditelj
    {
        public const string Sklop = "JupiterTranslator10";
        public const string Tip = "JupiterTranslator.Translator";

        public sealed class Ishod
        {
            public bool Ucitano;
            public string Putanja = "";
            public string Ime = "";
            public string Verzija = "";
            public string Token = "";
            public string Tip = "";
            public string Poruka = "";
            public string Susjed = "";
        }

        public static Ishod Ucitaj()
        {
            var i = new Ishod();
            i.Susjed = Susjed();
            Assembly a = null;
            try
            {
                a = Assembly.Load(Sklop);
            }
            catch (Exception ex)
            {
                i.Poruka = ex.Message;
            }
            if (a == null)
            {
                string put = Nadji(Sklop + ".dll");
                if (put.Length > 0)
                {
                    try
                    {
                        a = Assembly.LoadFrom(put);
                        i.Putanja = put;
                        i.Poruka = "";
                    }
                    catch (Exception ex)
                    {
                        i.Poruka = ex.Message;
                        i.Putanja = put;
                    }
                }
            }
            if (a == null)
                return i;
            AssemblyName n = a.GetName();
            i.Ime = n.Name ?? "";
            i.Verzija = n.Version == null ? "" : n.Version.ToString();
            i.Token = Token(n);
            if (!string.Equals(i.Ime, Sklop, StringComparison.OrdinalIgnoreCase))
            {
                i.Poruka = "Assembly.Load je vratio " + i.Ime + " " + i.Verzija;
                return i;
            }
            Type t = a.GetType(Tip);
            i.Tip = t == null ? "" : t.FullName;
            i.Ucitano = true;
            return i;
        }

        private static string Susjed()
        {
            string put = Nadji("JupiterTranslator.dll");
            if (put.Length == 0)
                return "";
            try
            {
                AssemblyName n = AssemblyName.GetAssemblyName(put);
                return put + " " + n.Name + " " + (n.Version == null ? "" : n.Version.ToString()) + " PublicKeyToken=" + Token(n);
            }
            catch (Exception ex)
            {
                return put + " " + ex.Message;
            }
        }

        public static string Nadji(string datoteka)
        {
            string[][] korijeni = new string[][]
            {
                new[] { AppDomain.CurrentDomain.BaseDirectory ?? "", "2" },
                new[] { Environment.GetEnvironmentVariable("SharedAssembliesDir") ?? "", "4" },
                new[] { Path.Combine(Environment.GetEnvironmentVariable("SharedAssembliesDir") ?? "", "Spin"), "2" },
                new[] { @"C:\Users\ntomic\Desktop\Jupiter Client", "6" },
                new[] { @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL", "3" },
                new[] { @"C:\Windows\assembly\GAC_MSIL", "3" }
            };
            for (int i = 0; i < korijeni.Length; i++)
            {
                int dubina;
                if (!int.TryParse(korijeni[i][1], out dubina))
                    dubina = 2;
                string naden = UDubinu(korijeni[i][0], datoteka, dubina);
                if (naden.Length > 0)
                    return naden;
            }
            return "";
        }

        private static string UDubinu(string korijen, string datoteka, int dubina)
        {
            if (dubina < 0 || string.IsNullOrEmpty(korijen) || !Directory.Exists(korijen))
                return "";
            try
            {
                string[] dat = Directory.GetFiles(korijen, datoteka);
                if (dat.Length > 0)
                    return dat[0];
            }
            catch
            {
                return "";
            }
            if (dubina == 0)
                return "";
            string[] mape;
            try
            {
                mape = Directory.GetDirectories(korijen);
            }
            catch
            {
                return "";
            }
            for (int i = 0; i < mape.Length; i++)
            {
                string n = UDubinu(mape[i], datoteka, dubina - 1);
                if (n.Length > 0)
                    return n;
            }
            return "";
        }

        private static string Token(AssemblyName n)
        {
            byte[] b = n.GetPublicKeyToken();
            if (b == null || b.Length == 0)
                return "null";
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < b.Length; i++)
                sb.Append(b[i].ToString("x2"));
            return sb.ToString();
        }
    }
}
