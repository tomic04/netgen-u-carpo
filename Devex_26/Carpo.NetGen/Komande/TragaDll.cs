using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Traži Raverus, SLOFISKAL i JupiterTranslator na sql-ai.
    /// GAC, Jupiter Client, SharedAssemblies. Ne kopira i ne briše.
    /// </summary>
    public static class TragaDll
    {
        public static readonly string[] Imena = new string[]
        {
            "Raverus.FiskalizacijaDEV.COM.dll",
            "SLOFISKAL.dll",
            "JupiterTranslator.dll",
            "JupiterTranslator10.dll",
            "Spin.Printing.v26.dll"
        };

        public static readonly string[] Korijeni = new string[]
        {
            @"C:\Users\ntomic\Desktop\Jupiter Client",
            @"C:\Windows\Microsoft.NET\assembly",
            @"C:\Windows\assembly",
            @"C:\Program Files\Jupiter Software",
            @"C:\Program Files (x86)\Jupiter Software"
        };

        public static string Izvjestaj()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Imena.Length; i++)
            {
                string put = Nadji(Imena[i]);
                sb.AppendLine((put == null ? "NEMA " : "IMA ") + Imena[i] + (put == null ? "" : " " + put));
            }
            return sb.ToString();
        }

        public static string Nadji(string ime)
        {
            string uz = AppDomain.CurrentDomain.BaseDirectory ?? "";
            string lokal = Path.Combine(uz, ime);
            if (File.Exists(lokal))
                return lokal;
            for (int i = 0; i < Korijeni.Length; i++)
            {
                if (!Directory.Exists(Korijeni[i]))
                    continue;
                try
                {
                    string[] f = Directory.GetFiles(Korijeni[i], ime, SearchOption.AllDirectories);
                    if (f.Length > 0)
                        return f[0];
                }
                catch
                {
                }
            }
            return null;
        }

        public static Type Tip(string dllIme, string punoIme)
        {
            string put = Nadji(dllIme);
            if (put == null)
                return null;
            try
            {
                Assembly a = Assembly.LoadFrom(put);
                return a.GetType(punoIme, false);
            }
            catch
            {
                return null;
            }
        }

        public static string Registar()
        {
            return Kljuc("Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav")
                + Kljuc("SLOFISKAL.Program")
                + Kljuc("JupiterTranslator.Translator");
        }

        private static string Kljuc(string prog)
        {
            try
            {
                Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(prog);
                if (k == null)
                    return "NEMA HKCR\\" + prog + "\r\n";
                Microsoft.Win32.RegistryKey clsid = k.OpenSubKey("CLSID");
                string id = clsid == null ? "" : Convert.ToString(clsid.GetValue(""));
                return "IMA HKCR\\" + prog + " " + (id ?? "") + "\r\n";
            }
            catch (Exception ex)
            {
                return "REG " + prog + " " + ex.Message + "\r\n";
            }
        }

        public static string Donijeti()
        {
            var sb = new StringBuilder();
            if (Nadji("Raverus.FiskalizacijaDEV.COM.dll") == null)
                sb.AppendLine("Raverus.FiskalizacijaDEV.COM.dll, klasa Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav, metode GenerirajUuid, PosaljiSoapPorukuCertifikatDatoteka, DohvatiJir. U SharedAssemblies\\Spin. Fiskal ide samo na https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest.");
            if (Nadji("SLOFISKAL.dll") == null)
                sb.AppendLine("SLOFISKAL.dll, klasa SLOFISKAL.Program, metode SLORacun i SLOPP. U SharedAssemblies\\Spin.");
            if (Nadji("JupiterTranslator10.dll") == null && Nadji("JupiterTranslator.dll") == null)
                sb.AppendLine("JupiterTranslator10.dll, klasa JupiterTranslator.Translator, TranslateString. Carpo.Common već zove tu klasu. U SharedAssemblies\\Spin.");
            if (sb.Length == 0)
                return "";
            return sb.ToString();
        }
    }
}
