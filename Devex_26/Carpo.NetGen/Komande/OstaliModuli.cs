using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>Čista pravila iz modula koji nisu Form1 ni Form2.</summary>
    public static class OstaliModuli
    {
        /// <summary>GetSpecialFolderLocationDotnet.vb 48–78. Grid Settings mapa se vrati, a NetGen stvara roditelja.</summary>
        public static string SpinMapa(string appData)
        {
            string p = (appData ?? "").TrimEnd('\\') + "\\SPIN Informatica\\";
            Directory.CreateDirectory(p);
            return p;
        }

        public static string JupiterMapa(string appData)
        {
            string p = SpinMapa(appData) + "Jupiter Software\\";
            Directory.CreateDirectory(p);
            return p;
        }

        public static string GridSettingsMapa(string appData)
        {
            string jupiter = JupiterMapa(appData);
            Directory.CreateDirectory(jupiter);
            return jupiter + "Grid Settings\\";
        }

        /// <summary>GetSpecialFolderLocationDotnet.vb 136–160. Lokalni disk je slovo:, UNC je do trećeg backslasha.</summary>
        public static string PutanjaSlike(string direktorij, string tvrtka)
        {
            string dir = direktorij ?? "";
            string firma = tvrtka ?? "";
            if (dir.Length == 0)
                return "";
            if (dir[0] != '\\')
                return dir.Substring(0, 1) + ":\\JupiterClient\\" + firma + "\\logos\\";
            int udaljenost = dir.IndexOf('\\', 2);
            if (udaljenost < 0)
                return "";
            return dir.Substring(0, udaljenost + 1) + "JupiterClient\\" + firma + "\\logos\\";
        }

        /// <summary>JupiterLog.Sredi 122–136. Lozinka postaje ***. Duljina se reže na 2000.</summary>
        public const int MaxPoruka = 2000;

        public static string SrediLog(string tekst)
        {
            if (tekst == null)
                return "";
            string s = Sakrij(tekst, "pwd");
            s = Sakrij(s, "password");
            if (s.Length > MaxPoruka)
                s = s.Substring(0, MaxPoruka) + " ... [skraceno, puni tekst je u Ctrl+F11]";
            return s;
        }

        private static string Sakrij(string s, string kljuc)
        {
            string rezultat = s ?? "";
            string trazi = (kljuc ?? "").ToLowerInvariant() + "=";
            int poz = 0;
            while (poz < rezultat.Length)
            {
                int i = rezultat.ToLowerInvariant().IndexOf(trazi, poz, StringComparison.Ordinal);
                if (i < 0)
                    break;
                int pocetak = i + trazi.Length;
                if (pocetak >= rezultat.Length)
                    break;
                int kraj = rezultat.Length;
                for (int j = pocetak; j < rezultat.Length; j++)
                {
                    char c = rezultat[j];
                    if (c == ';' || c == '"')
                    {
                        kraj = j;
                        break;
                    }
                }
                rezultat = rezultat.Substring(0, pocetak) + "***" + rezultat.Substring(kraj);
                poz = pocetak + 3;
            }
            return rezultat;
        }

        /// <summary>KriptirajDekriptiraj.vb 57–71. Ključ je 8 bajtova, kraći se puni s X.</summary>
        public static byte[] Kljuc(string k)
        {
            string s = (k ?? "").Trim();
            if (s.Length < 8)
                s = s.PadRight(8, 'X').Substring(0, 8);
            if (s.Length > 8)
                s = s.Substring(0, 8);
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] Kriptiraj(string tekst, string kljuc)
        {
            byte[] bts = Kljuc(kljuc);
            using (var key = new DESCryptoServiceProvider())
            {
                key.Key = bts;
                key.IV = bts;
                using (var ms = new MemoryStream())
                using (var enc = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(enc))
                {
                    sw.WriteLine(tekst ?? "");
                    sw.Flush();
                    enc.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public static string Dekriptiraj(byte[] sifra, string kljuc)
        {
            byte[] bts = Kljuc(kljuc);
            using (var key = new DESCryptoServiceProvider())
            {
                key.Key = bts;
                key.IV = bts;
                using (var ms = new MemoryStream(sifra ?? new byte[0]))
                using (var enc = new CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(enc))
                    return sr.ReadToEnd();
            }
        }

        /// <summary>FormCollectionExtensions.GetLast 16–30. Zadnji namespace koji sadrži ime. Bez MessageBox.</summary>
        public static string ZadnjiNamespace(string[] novi, string ime)
        {
            string zadnji = null;
            if (novi == null)
                return null;
            string trazeno = ime ?? "";
            for (int i = 0; i < novi.Length; i++)
            {
                string n = novi[i];
                if (n != null && n.IndexOf(trazeno, StringComparison.Ordinal) >= 0)
                    zadnji = n;
            }
            return zadnji;
        }
    }
}
