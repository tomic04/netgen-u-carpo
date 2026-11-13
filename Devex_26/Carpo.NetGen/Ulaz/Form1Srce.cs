using System;
using System.Globalization;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Print;

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// Širina stupca i datoteka rasporeda. PokaziUpit, dozvola gumba i DMS nisu ovdje:
    /// pomoćna funkcija koja ne zove ExecSql i ne puni grid nije port.
    /// </summary>
    public static class Form1Srce
    {
        public sealed class Format
        {
            public string Kljuc = "";
            public int Sirina;
            public string Natpis = "";
            public string FormatTekst = "";
            public string Vidljiva = "";
            public string Boja = "";
            public string Pozadina = "";
            public int MaxLinija;
            public bool Prelomi;
            public string Agregat = "";
            public bool NegativnoCrveno;
            public bool NulaNijeCrveno;
            public string Pozicija = "";
            public bool Auto;
            public string Zaglavlje = "";
            public bool SakrijSve;
        }

        /// <summary>Form1.vb 7459–7535. WIDTH ide kroz 14. Točka-zarez u natpisu je novi red.</summary>
        public static Format Kolona(int index, string ime, int sirina, string natpis, string format, string vidljiva, string boja, string pozadina, int maxLinija, string agregat, bool negativno, bool nijeNula, string pozicija, bool auto, bool clear, string zaglavlje)
        {
            var f = new Format();
            f.SakrijSve = clear;
            if (index == 0 && (ime ?? "").Length == 0)
                return f;
            f.Kljuc = index == 0 ? ime : index.ToString(CultureInfo.InvariantCulture);
            if (sirina > 0)
                f.Sirina = (int)Math.Round(sirina / 14.0, MidpointRounding.ToEven);
            f.Natpis = (natpis ?? "").Replace(";", "\r\n");
            f.FormatTekst = format ?? "";
            f.Vidljiva = vidljiva ?? "";
            f.Boja = (boja ?? "").Trim().ToLowerInvariant();
            f.Pozadina = (pozadina ?? "").Trim().ToLowerInvariant();
            if (maxLinija > 0)
            {
                f.MaxLinija = maxLinija;
                f.Prelomi = true;
            }
            f.Agregat = agregat ?? "";
            f.NegativnoCrveno = negativno;
            f.NulaNijeCrveno = nijeNula;
            f.Pozicija = pozicija ?? "";
            f.Auto = auto;
            f.Zaglavlje = (zaglavlje ?? "").Trim();
            return f;
        }

        /// <summary>Form1.vb 6246–6372. DX piše xml. Janus piše gxl.</summary>
        public static string Datoteka(string mapa, string ime, string jezik, bool dx, bool detalj)
        {
            string prefiks = detalj ? "GridDet" : "GridMaster";
            string ext = !detalj && dx ? ".xml" : ".gxl";
            string korijen = mapa ?? "";
            if (!korijen.EndsWith("\\", StringComparison.Ordinal))
                korijen += "\\";
            return korijen + "\\" + prefiks + (ime ?? "") + "-" + (jezik ?? "") + ext;
        }

        public static void Spremi(string put, string xml)
        {
            string dir = Path.GetDirectoryName(put);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(put, xml ?? "");
        }

        public static string Ucitaj(string put, bool reset)
        {
            if (reset || string.IsNullOrEmpty(put) || !File.Exists(put))
                return "";
            return File.ReadAllText(put);
        }

        public static bool Obrisi(string put)
        {
            if (string.IsNullOrEmpty(put) || !File.Exists(put))
                return false;
            File.Delete(put);
            return true;
        }

        /// <summary>Form1.vb 7773. ISEMPTY, pa SrediSQL.</summary>
        public static string Rafalna(string tekst, SqlKontekst kontekst)
        {
            return SqlTekst.Sredi(IspisMotora.Zaglavlje(tekst), kontekst);
        }

        public sealed class Pogled
        {
            public string Kontrola = "";
            public bool Ikona;
            public bool Navigator;
            public bool ProsiriKartice;
            public string NoviRed = "";
        }

        /// <summary>Form1.vb 6161–6184. TableView je GridView, CardView je CardView, SingleCard je LayoutView.</summary>
        public static Pogled PogledIz(string janus)
        {
            var p = new Pogled();
            string u = (janus ?? "").Trim();
            if (string.Equals(u, "CardView", StringComparison.OrdinalIgnoreCase))
            {
                p.Kontrola = "CardView";
                p.Ikona = false;
                p.ProsiriKartice = true;
                p.Navigator = false;
                return p;
            }
            if (string.Equals(u, "SingleCard", StringComparison.OrdinalIgnoreCase))
            {
                p.Kontrola = "LayoutView";
                p.Ikona = false;
                p.ProsiriKartice = false;
                p.Navigator = true;
                p.NoviRed = "Bottom";
                return p;
            }
            p.Kontrola = "GridView";
            p.Ikona = true;
            p.Navigator = false;
            p.NoviRed = "Top";
            return p;
        }
    }
}
