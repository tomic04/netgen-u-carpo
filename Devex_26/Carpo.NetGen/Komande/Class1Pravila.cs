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
    }
}
