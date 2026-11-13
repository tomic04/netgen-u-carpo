using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Form2.SrediKontrolu (3068-3150) i kreiranje kontrola: DOCK, ANCHOR, STYLE, SIZE, FORECOLOR, BACKCOLOR,
    /// HOTKEY. VB string dodijeli enumu (Option Strict Off): broj prode, rijec baci iznimku i
    /// On Error Resume Next ostavi staro. Zato vrijedi samo broj.
    /// </summary>
    public static class Form2Izgled
    {
        /// <summary>DockStyle: 0 None, 1 Top, 2 Bottom, 3 Left, 4 Right, 5 Fill.</summary>
        public static DockStyle? Dock(string vrijednost)
        {
            int n;
            if (!int.TryParse((vrijednost ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) || n < 0 || n > 5)
                return null;
            return (DockStyle)n;
        }

        /// <summary>AnchorStyles: 1 Top, 2 Bottom, 4 Left, 8 Right (6 = dolje lijevo, 13 = gore lijevo desno).</summary>
        public static AnchorStyles? Anchor(string vrijednost)
        {
            int n;
            if (!int.TryParse((vrijednost ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) || n < 0 || n > 15)
                return null;
            return (AnchorStyles)n;
        }

        /// <summary>FontStyle: 1 Bold, 2 Italic, 4 Underline, 8 Strikeout. "BOLD" je rijec, pa ne prode.</summary>
        public static FontStyle? Stil(string vrijednost)
        {
            int n;
            if (!int.TryParse((vrijednost ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) || n < 0 || n > 15)
                return null;
            return (FontStyle)n;
        }

        public static float? Velicina(string vrijednost)
        {
            float f;
            if (!float.TryParse((vrijednost ?? "").Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out f) || f <= 0 || f > 72)
                return null;
            return f;
        }

        /// <summary>
        /// FORECOLOR (3084-3115) i BACKCOLOR (3120-3150): "#rrggbb" ili ime s popisa. VB za BACKCOLOR "red"
        /// postavi zutu (3141), pa je i ovdje tako; FORECOLOR "red" je crvena.
        /// </summary>
        public static Color? Boja(string vrijednost, bool pozadina)
        {
            string v = (vrijednost ?? "").Trim();
            if (v.Length == 0)
                return null;
            if (v.StartsWith("#", StringComparison.Ordinal))
            {
                try { return ColorTranslator.FromHtml(v); }
                catch (Exception) { return null; }
            }
            switch (v.ToLowerInvariant())
            {
                case "green": return Color.Green;
                case "white": return Color.White;
                case "black": return Color.Black;
                case "darksalmon": return Color.DarkSalmon;
                case "darkseagreen": return Color.DarkSeaGreen;
                case "limegreen": return Color.LimeGreen;
                case "greenyellow": return Color.GreenYellow;
                case "yellow": return Color.Yellow;
                case "lightyellow": return Color.LightYellow;
                case "red": return pozadina ? Color.Yellow : Color.Red;
                case "silver": return pozadina ? (Color?)Color.Silver : null;
                case "lightgray": return pozadina ? (Color?)Color.LightGray : null;
            }
            return null;
        }

        /// <summary>
        /// Form2_KeyDown 4192-4252: F1-F10 i Multiply. HOTKEY je KeyCode.ToString ("F2"), a "*" je "Multiply".
        /// </summary>
        public static bool JeHotkey(Keys tipka)
        {
            int k = (int)tipka;
            return (k > 111 && k < 122) || tipka == Keys.Multiply;
        }

        public static bool Pogodak(string hotkey, Keys tipka)
        {
            string h = (hotkey ?? "").Trim().ToUpperInvariant();
            if (h.Length == 0)
                return false;
            if (h == "*")
                h = "MULTIPLY";
            return h == tipka.ToString().ToUpperInvariant();
        }

        /// <summary>Form2.designer: Button1..6 u Panel1 (dno, 33), x = 12 + 81 * (n - 1), sirina 75.</summary>
        public static int LijevoGumba(int redni)
        {
            return 12 + 81 * Math.Max(0, redni - 1);
        }
    }
}
