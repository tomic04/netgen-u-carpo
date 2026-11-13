using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.XtraPrinting;

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Class1.IspisiMePrekoSpinPrintinga 291–374, IspisiMe7 284 i PrintRpt grana repx 594–606.
    /// Poziv je Carpo.UI PrintDialog: Spin.Printing.v26, Print i PrintMulti, četvrti argument "Carpo".
    /// Test ne zove Print (otvorio bi dijalog). Dokument je PDF iz istog PrintingSystema.
    /// </summary>
    public static class SpinIspis
    {
        public const string Aplikacija = "Carpo";

        public static Action<string, string, string, string> PrintKuka;
        public static Action<string, string, IList<string>, string> MultiKuka;

        /// <summary>Class1.vb 331–333. REPX prefiks i kosa crta otpadaju.</summary>
        public static string Ime(string izvjestaj)
        {
            string s = izvjestaj ?? "";
            s = s.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "");
            return s.Replace("/", "");
        }

        /// <summary>IspisiMe7 287. Dvotočka postaje kosa crta, zatim Ime.</summary>
        public static string IzTaga(string tag)
        {
            return Ime((tag ?? "").Replace(":", "/"));
        }

        /// <summary>Class1.vb 320–328. @id= u parametru se ne prepisuje.</summary>
        public static List<string> Parametri(int[] id, string param)
        {
            var lista = new List<string>();
            string p = param ?? "";
            string nize = p.ToLowerInvariant();
            bool ima = nize.IndexOf("@id=", StringComparison.Ordinal) >= 0 || nize.IndexOf("@id =", StringComparison.Ordinal) >= 0;
            if (id == null || id.Length == 0)
            {
                lista.Add(p);
                return lista;
            }
            for (int i = 0; i < id.Length; i++)
                lista.Add(ima ? p : "@id=" + id[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + p);
            return lista;
        }

        /// <summary>Class1.vb 349. UBound &gt; 1 znači barem tri id-a.</summary>
        public static bool Vise(int broj)
        {
            return broj > 2;
        }

        /// <summary>PrintRpt 388 i 594. repx ide na Spin.Printing. vrstadokid isto, uz predložak iz baze ako Spin ne prođe.</summary>
        public static string Grana(string ime)
        {
            string s = (ime ?? "").ToLowerInvariant();
            if (s.IndexOf("repx:", StringComparison.Ordinal) >= 0 || s.IndexOf("repx/", StringComparison.Ordinal) >= 0)
                return "spin";
            if (s.IndexOf("vrstadokid-", StringComparison.Ordinal) >= 0)
                return "vrsta";
            if (s.IndexOf(".clsreport", StringComparison.Ordinal) >= 0)
                return "cls";
            if (s.IndexOf("pr10:", StringComparison.Ordinal) >= 0)
                return "pr10";
            if (s.IndexOf("rs:/", StringComparison.Ordinal) >= 0)
                return "rs";
            if (s.IndexOf("gg/", StringComparison.Ordinal) >= 0)
                return "gg";
            return "dlg";
        }

        public static bool Pozovi(string konekcija, string izvjestaj, int[] id, string param)
        {
            List<string> lista = Parametri(id, param);
            string ime = Ime(izvjestaj);
            if (Vise(lista.Count))
            {
                if (MultiKuka != null)
                {
                    MultiKuka(konekcija ?? "", ime, lista, Aplikacija);
                    return true;
                }
                return Zovi("PrintMulti", konekcija, ime, lista, null);
            }
            string jedan = lista.Count == 0 ? "" : lista[0];
            if (PrintKuka != null)
            {
                PrintKuka(konekcija ?? "", ime, jedan, Aplikacija);
                return true;
            }
            return Zovi("Print", konekcija, ime, null, jedan);
        }

        /// <summary>PDF bez pisača. Isti PrintingSystem koji Spin.Printing.v26 hrani.</summary>
        public static string Pdf(string mapa, string izvjestaj, string parametri)
        {
            Directory.CreateDirectory(mapa ?? "");
            string put = Path.Combine(mapa ?? "", "ispis.pdf");
            var ps = new PrintingSystem();
            var link = new Link(ps);
            string tekst = (izvjestaj ?? "") + "\r\n" + (parametri ?? "");
            link.CreateDetailArea += delegate (object sender, CreateAreaEventArgs e)
            {
                e.Graph.Font = new Font("Arial", 10);
                e.Graph.DrawString(tekst, Color.Black, new RectangleF(0, 0, 480, 60), BorderSide.None);
            };
            link.CreateDocument();
            ps.ExportToPdf(put);
            return put;
        }

        private static bool Zovi(string metoda, string konekcija, string ime, IList<string> vise, string jedan)
        {
            object o = PrintBridge.Stroj;
            if (o == null)
                o = Ucitaj();
            if (o == null)
                return false;
            try
            {
                if (vise != null)
                {
                    o.GetType().InvokeMember(metoda, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, o,
                        new object[] { konekcija ?? "", ime, vise, Aplikacija });
                }
                else
                {
                    o.GetType().InvokeMember(metoda, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, o,
                        new object[] { konekcija ?? "", ime, jedan ?? "", Aplikacija });
                }
                return true;
            }
            catch (Exception ex)
            {
                Carpo.NetGen.Log.NgDnevnik.Greska("Greška kod Spin.Printing." + metoda + " : " + ex.Message);
                return false;
            }
        }

        public static object Ucitaj()
        {
            try
            {
                Assembly a = Assembly.Load(PrintBridge.Sklop);
                object o = a.CreateInstance(PrintBridge.Klasa);
                PrintBridge.Stroj = o;
                return o;
            }
            catch (Exception ex)
            {
                Carpo.NetGen.Log.NgDnevnik.Greska("Greška kod Assembly.Load(" + PrintBridge.Sklop + ") : " + ex.Message);
                return null;
            }
        }
    }
}
