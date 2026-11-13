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
        /// <summary>Module1.Translate 266–282. hrv i prazan jezik ostaju. Tuđi jezik ide u vanjski prevoditelj kojeg nema, pa tekst ostaje.</summary>
        public static string Prevedi(string tekst, string jezik)
        {
            return tekst ?? "";
        }

        public static bool TrebaVanjskogPrevoditelja(string jezik)
        {
            string j = (jezik ?? "").Trim().ToLowerInvariant();
            return j.Length > 0 && j != "hrv";
        }

        /// <summary>Module1.Translate_OLD 293–344.</summary>
        public static string PrevediStaro(string tekst, System.Data.DataTable jezik, string visejezicno, string language, string pismo)
        {
            string izvor = tekst ?? "";
            if (visejezicno == "0" || visejezicno == "1")
                return izvor;
            if (string.IsNullOrWhiteSpace(language) || jezik == null || jezik.Rows.Count == 0)
                return izvor;
            foreach (System.Data.DataRow r in jezik.Rows)
            {
                string o = r["original"] == System.DBNull.Value ? "" : Convert.ToString(r["original"]);
                if (string.Equals((o ?? "").Trim(), izvor.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    string t = r["translated"] == System.DBNull.Value ? "" : Convert.ToString(r["translated"]);
                    return pismo == "cir" ? UCirilicu(t) : t;
                }
            }
            if (izvor.IndexOf(' ') >= 0)
            {
                string[] rijeci = izvor.Split(' ');
                var sb = new StringBuilder();
                for (int i = 0; i < rijeci.Length; i++)
                {
                    string tstr2 = rijeci[i];
                    foreach (System.Data.DataRow r in jezik.Rows)
                    {
                        string o = r["original"] == System.DBNull.Value ? "" : Convert.ToString(r["original"]);
                        if (string.Equals((o ?? "").Trim(), (rijeci[i] ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                            tstr2 = r["translated"] == System.DBNull.Value ? "" : Convert.ToString(r["translated"]);
                    }
                    sb.Append(' ').Append(tstr2);
                }
                if (sb.ToString().Trim().Length > 0)
                    izvor = sb.ToString().Trim();
            }
            if (pismo == "cir")
                return UCirilicu(izvor);
            return izvor;
        }

        /// <summary>Module1.ToCiril2 474–505. LJ, NJ i Dž idu prvi.</summary>
        public static string UCirilicu(string tekst)
        {
            const string latinica = "abcdefghijklmnoprstuvzABCDEFGHIJKLMNOPRSTUVZ\u0161\u0107\u0111\u017e\u010d\u0160\u0106\u0110\u017d\u010c";
            const string cirilica = "\u0430\u0431\u0446\u0434\u0435\u0444\u0433\u0445\u0438\u0458\u043a\u043b\u043c\u043d\u043e\u043f\u0440\u0441\u0442\u0443\u0432\u0437\u0410\u0411\u0426\u0414\u0415\u0424\u0413\u0425\u0418\u0408\u041a\u041b\u041c\u041d\u041e\u041f\u0420\u0421\u0422\u0423\u0412\u0417\u0448\u045b\u0452\u0436\u0447\u0428\u040b\u0402\u0416\u0427";
            string[] lat = { "LJ", "Lj", "lj", "NJ", "Nj", "nj", "D\u017d", "D\u017e", "d\u017e" };
            string[] cir = { "\u0409", "\u0409", "\u0459", "\u040a", "\u040a", "\u045a", "\u040f", "\u040f", "\u045f" };
            string c = tekst ?? "";
            for (int i = 0; i < lat.Length; i++)
                c = c.Replace(lat[i], cir[i]);
            var sb = new StringBuilder(c.Length);
            for (int i = 0; i < c.Length; i++)
            {
                int k = latinica.IndexOf(c[i]);
                sb.Append(k >= 0 ? cirilica[k] : c[i]);
            }
            return sb.ToString();
        }

        /// <summary>Form6.GetServerPath 239–254. Nema ini u AppData, čita se C:\JupiterClient\JupiterClient.ini. RemoteDllPath se reže od 14.</summary>
        public static string IniPutanja(bool postojiUAppData, string appData)
        {
            if (postojiUAppData)
                return (appData ?? "") + "JupiterClient.ini";
            return "C:\\JupiterClient\\JupiterClient.ini";
        }

        public static string RemoteDll(string ini, out bool easyPrint)
        {
            easyPrint = false;
            string tekst = ini ?? "";
            int pocetak = 0;
            while (pocetak <= tekst.Length)
            {
                int kraj = tekst.IndexOf('\n', pocetak);
                string linija = kraj < 0 ? tekst.Substring(pocetak) : tekst.Substring(pocetak, kraj - pocetak);
                linija = linija.TrimEnd('\r');
                if (linija.IndexOf("RemoteDllPath", StringComparison.Ordinal) >= 0)
                    return linija.Length > 14 ? linija.Substring(14) : "";
                if (string.Equals(linija.Trim().ToLowerInvariant(), "easyprintworkaround=true", StringComparison.Ordinal))
                    easyPrint = true;
                if (kraj < 0)
                    break;
                pocetak = kraj + 1;
            }
            return "";
        }

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
