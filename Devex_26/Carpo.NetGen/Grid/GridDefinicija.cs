using System;
using System.Collections.Generic;
using System.Globalization;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// COLUMN definicije na stupce grida, redom kako su napisane. Podloga (MASTERCOLUMNS, DETAILCOLUMNS):
    /// Form1.CustomFormatKolona 7451-7570. Grid dijaloga (INPUT TYPE="grid" s COLUMN djecom, 47 granula):
    /// Form2.vb 1798-1957 (DGR-01). Popis ima Janus stupce 0 (ikona) i 1 (kvačica), pa INDEX znači isto
    /// što i Janus Columns(index). Janus Position premješta stupac (ostali se pomaknu), a Redni je
    /// redoslijed prikaza nakon svih premještanja.
    /// </summary>
    public static class GridDefinicija
    {
        public static void Primijeni(List<GridKolona> kolone, IList<NgColumn> defs, bool clear, string zaglavlje, bool dijalog)
        {
            if (kolone == null)
                return;
            var prikaz = new List<GridKolona>(kolone);
            if (defs != null)
            {
                if (clear)
                {
                    for (int i = 0; i < kolone.Count; i++)
                        kolone[i].Vidljiva = false;
                }
                int pos = 0;
                for (int n = 0; n < defs.Count; n++)
                {
                    NgColumn def = defs[n];
                    pos++;
                    int index = Broj(Atr(def, "INDEX"));
                    string ime = def.Name ?? "";
                    if (index <= 0 && ime.Length == 0)
                        continue;
                    GridKolona kol = Nadji(kolone, index, ime);
                    if (dijalog)
                        pos = Dijalog(kol, def, clear, pos, prikaz);
                    else if (kol != null)
                        pos = Podloga(kol, def, clear, pos, zaglavlje, prikaz);
                }
            }
            for (int i = 0; i < prikaz.Count; i++)
                prikaz[i].Redni = i;
        }

        /// <summary>Form1.vb 7503-7568: agregat, MAXLINES, CLEARCOLUMS pozicija, WIDTH, CAPTION, FORMAT, VISIBLE, POSITION.</summary>
        private static int Podloga(GridKolona kol, NgColumn def, bool clear, int pos, string zaglavlje, List<GridKolona> prikaz)
        {
            if (!string.IsNullOrEmpty(def.AgregateFunction))
                kol.Agregat = def.AgregateFunction;
            MaxLinija(kol, def);
            if (clear)
            {
                pos++;
                Premjesti(prikaz, kol, pos);
            }
            Osnovno(kol, def);
            int? poz = BrojIliNista(Atr(def, "POSITION"));
            if (poz.HasValue)
                Premjesti(prikaz, kol, poz.Value);
            if (!string.IsNullOrEmpty(zaglavlje) && zaglavlje.Trim().Length > 0)
                kol.PoravnanjeZaglavlja = zaglavlje.Trim();
            kol.Natpis = (kol.Natpis ?? "").Replace(";", "\r\n");
            if (Atr(def, "AUTOSIZE").ToLowerInvariant() == "true")
                kol.AutoSirina = true;
            if (Atr(def, "NEGATIVERED").ToLowerInvariant() == "true")
                kol.NegativnoCrveno = true;
            if (Atr(def, "NOTZERORED").ToLowerInvariant() == "true")
                kol.NotZeroCrveno = true;
            Boje(kol, def);
            return pos;
        }

        /// <summary>
        /// Form2.vb 1828-1957: POSITION prvi, AUTOSIZE, MAXLINES, pa WIDTH/CAPTION/FORMAT/VISIBLE, ALIGN, agregat,
        /// CLEARCOLUMS pozicija (zadnja, pa nadjača POSITION) i boje. Uz On Error Resume Next pozicija se broji
        /// i kad stupca nema.
        /// </summary>
        private static int Dijalog(GridKolona kol, NgColumn def, bool clear, int pos, List<GridKolona> prikaz)
        {
            if (kol != null)
            {
                int? poz = BrojIliNista(Atr(def, "POSITION"));
                if (poz.HasValue)
                    Premjesti(prikaz, kol, poz.Value);
                if (Atr(def, "AUTOSIZE").ToLowerInvariant() == "true")
                    kol.AutoSirina = true;
                MaxLinija(kol, def);
                Osnovno(kol, def);
                switch (Atr(def, "ALIGN").ToUpperInvariant())
                {
                    case "RIGHT":
                        kol.Poravnanje = "Far";
                        break;
                    case "CENTER":
                        kol.Poravnanje = "Center";
                        break;
                    case "LEFT":
                        kol.Poravnanje = "Near";
                        break;
                }
                if (Atr(def, "AGREGATEFUNCTION").Length > 0)
                    kol.Agregat = Atr(def, "AGREGATEFUNCTION");
            }
            if (clear)
            {
                pos++;
                if (kol != null)
                    Premjesti(prikaz, kol, pos);
            }
            if (kol != null)
                Boje(kol, def);
            return pos;
        }

        private static void Osnovno(GridKolona kol, NgColumn def)
        {
            int w = Broj(Atr(def, "WIDTH"));
            if (w > 0)
                kol.Sirina = MasterGrid.VbJedinica(w);
            string cap = Atr(def, "CAPTION");
            if (cap.Length > 0)
                kol.Natpis = cap;
            string fmt = Atr(def, "FORMAT");
            if (fmt.Length > 0)
                kol.Format = fmt;
            bool? vis = Form2Rezultat.VbBool(Atr(def, "VISIBLE"));
            if (vis.HasValue)
                kol.Vidljiva = vis.Value;
        }

        private static void MaxLinija(GridKolona kol, NgColumn def)
        {
            int m = Broj(Atr(def, "MAXLINES"));
            if (m > 0)
            {
                kol.MaxLinija = m;
                kol.Prelomi = true;
            }
        }

        private static void Boje(GridKolona kol, NgColumn def)
        {
            string fg = Boja(Atr(def, "FORECOLOR"));
            if (fg != null)
                kol.BojaTeksta = fg;
            string bg = Boja(Atr(def, "BACKCOLOR"));
            if (bg != null)
                kol.BojaPozadine = bg;
        }

        /// <summary>Janus Columns(index) ili Columns(ime) (IIf(tIndex = 0, tName, tIndex)).</summary>
        private static GridKolona Nadji(List<GridKolona> kolone, int index, string ime)
        {
            if (index > 0)
                return index < kolone.Count ? kolone[index] : null;
            for (int i = 0; i < kolone.Count; i++)
            {
                if (string.Equals(kolone[i].Podatak, ime, StringComparison.OrdinalIgnoreCase))
                    return kolone[i];
            }
            return null;
        }

        /// <summary>Janus Column.Position: stupac ide na to mjesto među svim stupcima, ostali se pomaknu.</summary>
        private static void Premjesti(List<GridKolona> prikaz, GridKolona kol, int pozicija)
        {
            kol.Pozicija = pozicija;
            if (!prikaz.Remove(kol))
                return;
            int p = pozicija < 0 ? 0 : pozicija;
            if (p > prikaz.Count)
                p = prikaz.Count;
            prikaz.Insert(p, kol);
        }

        /// <summary>
        /// Boje iz Select Case u CustomFormatKolona i Form2 (ostale vrijednosti se ne vežu). Vraća ime malim
        /// slovima ili null.
        /// </summary>
        public static string Boja(string ime)
        {
            switch ((ime ?? "").Trim().ToLowerInvariant())
            {
                case "green":
                case "white":
                case "black":
                case "darksalmon":
                case "darkseagreen":
                case "limegreen":
                case "greenyellow":
                case "yellow":
                case "lightyellow":
                case "red":
                case "brown":
                case "blue":
                case "ghostwhite":
                case "gray":
                case "orange":
                case "orangered":
                    return ime.Trim().ToLowerInvariant();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Janus TextAlignment iz HEADERALIGNMENT: 1 Near, 2 Center, 3 Far (Form2.vb 1873-1881 piše isto za
        /// ALIGN). Prazno ili nepoznato: null.
        /// </summary>
        public static string Poravnanje(string janus)
        {
            switch ((janus ?? "").Trim().ToLowerInvariant())
            {
                case "1":
                case "near":
                case "left":
                    return "Near";
                case "2":
                case "center":
                    return "Center";
                case "3":
                case "far":
                case "right":
                    return "Far";
                default:
                    return null;
            }
        }

        private static string Atr(NgColumn def, string ime)
        {
            if (def == null || def.Atributi == null)
                return "";
            return (def.Atributi.Vrijednost(ime) ?? "").Trim();
        }

        private static int Broj(string v)
        {
            return BrojIliNista(v) ?? 0;
        }

        private static int? BrojIliNista(string v)
        {
            int n;
            return int.TryParse((v ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : (int?)null;
        }
    }
}
