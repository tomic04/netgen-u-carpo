using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>Čista pravila iz Class1.vb koja ne trebaju Janus ni Spin.Printing.</summary>
    public static class Class1Pravila
    {
        /// <summary>Class1.FormatirajGrid 271–279. Stupac 0 se ne dira. Ime koje završava na id, a nije id, se skriva.</summary>
        public static bool SakrijId(string ime)
        {
            string n = (ime ?? "").Trim().ToLowerInvariant();
            return n.EndsWith("id", StringComparison.Ordinal) && n != "id";
        }

        /// <summary>Zadnje 4 znamenke su širina / 14. Inače 0.</summary>
        public static int Sirina(string ime)
        {
            string n = (ime ?? "").Trim();
            if (n.Length < 4)
                return 0;
            int broj;
            if (!int.TryParse(n.Substring(n.Length - 4), out broj))
                return 0;
            return broj / 14;
        }

        public static string ImeBezSirine(string ime)
        {
            string n = (ime ?? "").Trim();
            if (Sirina(n) == 0 && (n.Length < 4 || !JeBroj(n.Substring(n.Length - 4))))
                return n;
            if (n.Length < 4)
                return n;
            return n.Substring(0, n.Length - 4);
        }

        private static bool JeBroj(string s)
        {
            int n;
            return int.TryParse(s, out n);
        }

        /// <summary>Class1.AddConditionalFormatting 1379–1421. formatid sadrži token.</summary>
        public static string FormatUvjeta(string formatid)
        {
            string f = (formatid ?? "").ToLowerInvariant();
            string s = "";
            if (f.IndexOf("bold", StringComparison.Ordinal) >= 0) s += "bold ";
            if (f.IndexOf("gray", StringComparison.Ordinal) >= 0) s += "gray ";
            if (f.IndexOf("red", StringComparison.Ordinal) >= 0) s += "red ";
            if (f.IndexOf("green", StringComparison.Ordinal) >= 0) s += "green ";
            if (f.IndexOf("blue", StringComparison.Ordinal) >= 0) s += "blue ";
            if (f.IndexOf("brown", StringComparison.Ordinal) >= 0) s += "brown ";
            if (f.IndexOf("yellow", StringComparison.Ordinal) >= 0) s += "yellow ";
            if (f.IndexOf("orange", StringComparison.Ordinal) >= 0) s += "orange ";
            if (f.IndexOf("strike", StringComparison.Ordinal) >= 0) s += "strike ";
            if (f.IndexOf("italic", StringComparison.Ordinal) >= 0) s += "italic ";
            if (f.IndexOf("underline", StringComparison.Ordinal) >= 0) s += "underline ";
            return s.Trim();
        }

        /// <summary>Class1.DajInstall 1454–1456. Oba polja puna, upit se ne šalje.</summary>
        public static bool InstallVecIma(string web, string baza)
        {
            return !string.IsNullOrEmpty(web) && !string.IsNullOrEmpty(baza);
        }

        public static string InstallSql()
        {
            return "select * from Install ";
        }

        /// <summary>Class1.GlipiMS 1605–1611.</summary>
        public static string Glipi(string tekst)
        {
            string s = tekst ?? "";
            s = s.Replace("Ć", "CCC").Replace("ć", "ccc");
            s = s.Replace("Ž", "ZZZ").Replace("ž", "zzz");
            s = s.Replace("Č", "YYY").Replace("č", "yyy");
            s = s.Replace("Š", "SSS").Replace("š", "sss");
            s = s.Replace("Đ", "DDD").Replace("đ", "ddd");
            return s;
        }

        /// <summary>Class1.GetDXID 1960–1971.</summary>
        public static string DxId(bool imaRedova, string id, bool idNedostaje, string prvi)
        {
            if (!imaRedova)
                return "0";
            string v = idNedostaje ? (prvi ?? "") : (id ?? "");
            if (v.Length == 0)
                return "0";
            return v;
        }

        /// <summary>Class1.BlagajnaEnableZaMlinar 1977–1985.</summary>
        public static string BlagajnaUpit(string id)
        {
            return "spmpBlagajnaEnable @mpkaseid = " + (id ?? "");
        }

        public static string BlagajnaAzur(string id, bool procitano)
        {
            return "update mpkase set indenable = " + (procitano ? "1" : "0") + " where mpkaseid = " + (id ?? "");
        }

        /// <summary>Class1.NapuniDXGrid 1716. Caption koji završava na id se skriva, uključujući sam id.</summary>
        public static bool SakrijCaptionId(string caption)
        {
            return (caption ?? "").Trim().ToLowerInvariant().EndsWith("id", StringComparison.Ordinal);
        }

        /// <summary>Class1.NapuniDXGrid 1720–1722. Zadnje 4 znamenke / 14, VB dijeljenje.</summary>
        public static int SirinaNatpisa(string caption)
        {
            int broj;
            if (!ZadnjeCetiri(caption, out broj))
                return 0;
            return (int)Math.Round(broj / 14.0, MidpointRounding.ToEven);
        }

        public static string NatpisBezSirine(string caption)
        {
            int broj;
            string n = (caption ?? "").Trim();
            if (!ZadnjeCetiri(n, out broj))
                return n;
            return n.Substring(0, n.Length - 4);
        }

        /// <summary>Class1.NapuniDXGrid 1735. IndexOf &gt; 0, pa pogodak na početku nije edit.</summary>
        public static bool SmijeEdit(string editStupci, string caption)
        {
            return (editStupci ?? "").IndexOf(caption ?? "", StringComparison.Ordinal) > 0;
        }

        public const string StupacOdaberi = "rcb555";
        public const string NatpisOdaberi = "Odaberi";
        public const string BojaEdita = "Azure";
        public const string MaskaN2 = "N2";

        /// <summary>Class1.CustomFormatKolonaDX 1903–1921. Index 0 bira ime, inače index.</summary>
        public static string KljucKolone(int index, string ime)
        {
            if (index == 0)
                return ime ?? "";
            return index.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static int SirinaXml(int width)
        {
            if (width <= 0)
                return 0;
            return (int)Math.Round(width / 14.0, MidpointRounding.ToEven);
        }

        public static string Natpis(string caption)
        {
            return (caption ?? "").Replace(";", "\r\n");
        }

        public static bool SakrijSve(string clearColumns)
        {
            return string.Equals((clearColumns ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Petlja diže iPosition, a CLEARCOLUMS ga diže još jednom. Prvi čvor dobije 2.</summary>
        public static int IndeksVidljivosti(int redni, bool clear)
        {
            if (!clear || redni < 1)
                return 0;
            return redni * 2;
        }

        /// <summary>Class1.UkljuciSum 1937–1952. Samo decimalni stupac dobiva zbroj.</summary>
        public static bool Zbroj(bool ukljuci, bool decimalni)
        {
            return ukljuci && decimalni;
        }

        public const string FormatN2 = "{0:N2}";

        private static bool ZadnjeCetiri(string caption, out int broj)
        {
            broj = 0;
            string n = (caption ?? "").Trim();
            if (n.Length < 4)
                return false;
            return int.TryParse(n.Substring(n.Length - 4), out broj);
        }
    }
}
