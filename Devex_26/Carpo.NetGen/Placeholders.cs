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
        public static string[] ZadnjeZaglavlje;
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
            while (h.Header.Content.Count < 3)
                h.Header.Content.Add("");
            h.Header.Content[0] = SrediRed(lijevo);
            h.Header.Content[1] = SrediRed(c);
            h.Header.Content[2] = SrediRed(desno);
            var link = new PrintableComponentLink(new PrintingSystem());
            link.PageHeaderFooter = h;
            var natrag = (PageHeaderFooter)link.PageHeaderFooter;
            var gotovo = natrag.Header.Content;
            return new[]
            {
                gotovo.Count > 0 ? gotovo[0] ?? "" : "",
                gotovo.Count > 1 ? gotovo[1] ?? "" : "",
                gotovo.Count > 2 ? gotovo[2] ?? "" : ""
            };
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
            ZadnjeZaglavlje = TriZaglavlja(lijevo, centar, desno, naslov);
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

        /// <summary>Prazno: Print otvara Spin.Printing. Postavljeno: isti predložak ide u PDF, bez dijaloga pisača.</summary>
        public static string MapaIzvoza;
        public static string ZadnjiPdf;
        public static string ZadnjiIme;
        public static string ZadnjiParam;

        /// <summary>Class1.IspisiMePrekoSpinPrintinga 320–328. @id= u parametru se ne prepisuje. Više od dva id-a ide u PrintMulti.</summary>
        public static System.Collections.Generic.List<string> ParametriIzvjestaja(string[] id, string param)
        {
            var lista = new System.Collections.Generic.List<string>();
            string p = param ?? "";
            string nize = p.ToLowerInvariant();
            bool ima = nize.IndexOf("@id=", StringComparison.Ordinal) >= 0 || nize.IndexOf("@id =", StringComparison.Ordinal) >= 0;
            if (id == null || id.Length == 0)
            {
                lista.Add(p);
                return lista;
            }
            for (int i = 0; i < id.Length; i++)
                lista.Add(ima ? p : "@id=" + (id[i] ?? "") + "," + p);
            return lista;
        }

        public static bool ViseOdDva(int broj)
        {
            return broj > 2;
        }

        public static string Pdf(string mapa, string ime, string parametri)
        {
            Directory.CreateDirectory(mapa ?? "");
            string put = Path.Combine(mapa ?? "", "ispis.pdf");
            var ps = new PrintingSystem();
            var link = new Link(ps);
            string tijelo = (ime ?? "") + "\r\n" + (parametri ?? "");
            link.CreateDetailArea += delegate (object sender, CreateAreaEventArgs e)
            {
                e.Graph.DrawString(tijelo, System.Drawing.Color.Black, new System.Drawing.RectangleF(0, 0, 480, 40), BorderSide.None);
            };
            link.CreateDocument();
            ps.ExportToPdf(put);
            ps.Dispose();
            return put;
        }

        public static bool Predlozak(string naredba, string konekcija, string parametri, string id)
        {
            ZadnjiIme = ImePredloska(naredba);
            var lista = ParametriIzvjestaja(string.IsNullOrEmpty(id) ? new string[0] : new[] { id }, parametri ?? "");
            ZadnjiParam = lista.Count == 0 ? "" : (ViseOdDva(lista.Count) ? string.Join("|", lista.ToArray()) : lista[0]);
            if (!string.IsNullOrEmpty(MapaIzvoza))
            {
                ZadnjiPdf = Pdf(MapaIzvoza, ZadnjiIme, ZadnjiParam);
                return File.Exists(ZadnjiPdf);
            }
            object o = Ucitaj();
            bool slozeno = false;
            if (o != null)
            {
                try
                {
                    string metoda = ViseOdDva(lista.Count) ? "PrintMulti" : "Print";
                    object tijelo = ViseOdDva(lista.Count) ? (object)lista : ZadnjiParam;
                    o.GetType().InvokeMember(
                        metoda,
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        o,
                        new object[] { konekcija ?? "", ZadnjiIme, tijelo, "Carpo" });
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
