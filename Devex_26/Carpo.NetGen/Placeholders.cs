using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Carpo.NetGen.Log;
using Carpo.NetGen.Shell;
using DevExpress.XtraPrinting;

namespace Carpo.NetGen.Layout
{
    /// <summary>M1: prvo lijevo stablo + master grid + detail grid. M0 ne crta.</summary>
    public static class LayoutEngine
    {
        public static void Crtaj(ShellLayout raspored)
        {
            throw new NotSupportedException(
                "M1 crta obrazac " + ShellLayout.Obrazac + ". M0 ne otvara formu.");
        }
    }
}

namespace Carpo.NetGen.Commands
{
    /// <summary>M1: ONLOAD koji ne pocinje s ':' ide u grid; ':' otvara formu. Gumbi CMDxCAPTION / BUTTONS.</summary>
    public static class CommandRunner
    {
        public static void Pokreni(string naredba)
        {
            throw new NotSupportedException("M1: komande. M0 samo cuva tekst naredbe.");
        }
    }
}

namespace Carpo.NetGen.Sql
{
    /// <summary>
    /// M1: @@id i @@operateriid, preskok ako @@ preživi. SQL ostaje slobodan tekst.
    /// M0 ne otvara konekciju i ne dira bazu.
    /// </summary>
    public static class SqlBridge
    {
        public static void Exec(string sql)
        {
            throw new NotSupportedException("M1: SQL tekst. M0 ne šalje naredbu serveru.");
        }
    }
}

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Isti poziv kao Carpo.UI: Spin.Printing.v26, klasa Spin.Printing.Start.
    /// Grid (PRINTMASTER, PRINTREPXMASTER, PRINTGRID, PRINTREPXGRID) ide u PrintLink,
    /// kao ComponentGrid.PrintControl. Predložak :repx/ ide u Print, kao PrintDialog.
    /// NetGen bira koji je grid i složi zaglavlje (Form1.vb 4945–6001, Form2.vb 9041–9056).
    /// PageHeaderFooter.Content je Left, Center, Right. Form2.vb 9049–9056.
    /// </summary>
    public static class PrintBridge
    {
        public const string Sklop = "Spin.Printing.v26";
        public const string Klasa = "Spin.Printing.Start";

        public static object Stroj;
        public static object Repx;
        public static string PutanjaDll = "";

        public static string Vrsta(string naredba, bool dijalog)
        {
            string u = (naredba ?? "").Trim();
            if (u.StartsWith(":", StringComparison.Ordinal))
                u = u.Substring(1).Trim();
            if (dijalog && (u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase)))
                return "lodani";
            if (u.StartsWith("PRINTDET", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXDET", StringComparison.OrdinalIgnoreCase))
                return "detail";
            if (u.StartsWith("PRINTMASTER", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXMASTER", StringComparison.OrdinalIgnoreCase)
                || u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase))
                return "master";
            return "";
        }

        public static bool JePredlozak(string naredba)
        {
            string u = (naredba ?? "").ToLowerInvariant();
            return u.IndexOf("repx:", StringComparison.Ordinal) >= 0 || u.IndexOf("repx/", StringComparison.Ordinal) >= 0;
        }

        public static string ImePredloska(string naredba)
        {
            string s = naredba ?? "";
            s = s.Replace("REPX:/", "").Replace("repx:/", "").Replace("REPX/", "").Replace("repx/", "").Replace("repx:", "").Replace("REPX:", "");
            s = s.Replace("/", "");
            if (s.StartsWith(":", StringComparison.Ordinal))
                s = s.Substring(1);
            return s.Trim();
        }

        /// <summary>PrintLink prima bool. LANDSCAPE je true. Prazno je uspravno.</summary>
        public static bool JeLandscape(string pageOrientation)
        {
            return string.Equals((pageOrientation ?? "").Trim(), "LANDSCAPE", StringComparison.OrdinalIgnoreCase);
        }

        public static string Centar(string centar, string naslov, bool dijalog)
        {
            if (!string.IsNullOrWhiteSpace(centar))
                return IspisZaglavlje.Desno(centar, null);
            string t = naslov ?? "";
            if (!dijalog)
            {
                int z = t.IndexOf("v[", StringComparison.Ordinal);
                if (z >= 0)
                    return t.Substring(0, z).Trim();
            }
            return t;
        }

        /// <summary>Form2.vb 9049–9056. Prazan centar je naslov forme. ## i !! su novi red.</summary>
        public static string[] TriZaglavlja(string lijevo, string centar, string desno, string naslov)
        {
            string c = string.IsNullOrWhiteSpace(centar) ? (naslov ?? "") : centar;
            var h = new PageHeaderFooter();
            h.Header.Content = new[] { SrediRed(lijevo), SrediRed(c), SrediRed(desno) };
            var link = new PrintableComponentLink(new PrintingSystem());
            link.PageHeaderFooter = h;
            var natrag = (PageHeaderFooter)link.PageHeaderFooter;
            string[] gotovo = natrag.Header.Content;
            if (gotovo == null || gotovo.Length < 3)
                return new[] { "", "", "" };
            return new[] { gotovo[0] ?? "", gotovo[1] ?? "", gotovo[2] ?? "" };
        }

        private static string SrediRed(string s)
        {
            return (s ?? "").Replace("##", "\r\n").Replace("!!", "\r\n");
        }

        public static Dictionary<string, string> Zaglavlje(string lijevo, string centar, string desno, string naslov, bool dijalog)
        {
            var d = new Dictionary<string, string>();
            d["caption"] = Centar(centar, naslov, dijalog);
            d["captionAlign"] = "";
            d["header"] = IspisZaglavlje.Desno(lijevo, null);
            d["headerAlign"] = "";
            d["subHeader"] = IspisZaglavlje.Desno(desno, null);
            d["subHeaderAlign"] = "";
            d["headerScale"] = "";
            d["imagePath"] = "";
            return d;
        }

        public static bool Ispisi(string naredba, bool dijalog, string id, string operater, string konekcija, string login, string zadnjiSql, string parametriGrida, string parametriSvi, string lijevo, string centar, string desno, string naslov, object grid, string pageOrientation, DataTable tablica)
        {
            if (JePredlozak(naredba) && Vrsta(naredba, dijalog).Length == 0)
                return Predlozak(naredba, konekcija, parametriSvi, id);
            string vrsta = Vrsta(naredba, dijalog);
            if (vrsta.Length == 0)
                return false;
            object o = Ucitaj();
            if (o == null)
                return false;
            int op = 0;
            int.TryParse(operater ?? "", out op);
            var zaglavlje = Zaglavlje(lijevo, centar, desno, naslov, dijalog || vrsta == "lodani");
            string[] tri = TriZaglavlja(lijevo, centar, desno, naslov);
            zaglavlje["header"] = tri[0];
            zaglavlje["caption"] = tri[1];
            zaglavlje["subHeader"] = tri[2];
            var combo = new DataTable();
            var ds = new DataSet();
            if (tablica != null)
                ds.Tables.Add(tablica.Copy());
            var raspored = new MemoryStream();
            bool polozeno = JeLandscape(pageOrientation);
            try
            {
                o.GetType().InvokeMember(
                    "PrintLink",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    o,
                    new object[] { konekcija ?? "", op, grid, zaglavlje, combo, ds, raspored, polozeno });
                return true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod Spin.Printing.PrintLink : " + ex.Message);
                return false;
            }
        }

        public static bool Predlozak(string naredba, string konekcija, string parametri, string id)
        {
            object o = Ucitaj();
            bool slozeno = false;
            if (o != null)
            {
                try
                {
                    o.GetType().InvokeMember(
                        "Print",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        o,
                        new object[] { konekcija ?? "", ImePredloska(naredba), parametri ?? "", "Carpo" });
                    slozeno = true;
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod Spin.Printing.Print : " + ex.Message);
                }
            }
            if (slozeno)
                return true;
            return DevRepx(naredba, konekcija, parametri, id);
        }

        /// <summary>Form6.vb 134–161. isnetgen=1, DoPrint2, putanja SharedDLLs\repx.</summary>
        public static bool DevRepx(string naredba, string konekcija, string parametri, string id)
        {
            object r = Repx;
            if (r == null)
            {
                try
                {
                    Assembly a = Assembly.Load("rptDevRepx");
                    r = a.CreateInstance("rptDevRepx.clsReport");
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod Assembly.Load(rptDevRepx) : " + ex.Message);
                    return false;
                }
            }
            if (r == null)
                return false;
            try
            {
                Type t = r.GetType();
                PropertyInfo prop = t.GetProperty("isnetgen", BindingFlags.Instance | BindingFlags.Public);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(r, 1, null);
                else
                {
                    FieldInfo polje = t.GetField("isnetgen", BindingFlags.Instance | BindingFlags.Public);
                    if (polje != null)
                        polje.SetValue(r, 1);
                }
                string korijen = (PutanjaDll ?? "").Trim();
                if (korijen.Length == 0)
                    korijen = "C:\\JupiterClient";
                string mapa = korijen.TrimEnd('\\') + "\\SharedDLLs\\repx";
                t.InvokeMember(
                    "DoPrint2",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    r,
                    new object[] { konekcija ?? "", id ?? "0", mapa, ImePredloska(naredba), 0, "", parametri ?? "" });
                return true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod rptDevRepx.DoPrint2 : " + ex.Message);
                return false;
            }
        }

        private static object Ucitaj()
        {
            if (Stroj != null)
                return Stroj;
            try
            {
                Assembly a = Assembly.Load(Sklop);
                object o = a.CreateInstance(Klasa);
                if (o == null)
                    NgDnevnik.Greska("Greška kod Assembly.Load(" + Sklop + ") : CreateInstance");
                return o;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod Assembly.Load(" + Sklop + ") : " + ex.Message);
                return null;
            }
        }
    }
}
