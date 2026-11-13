using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Događaji koje Dijalog stvarno zove: tražilica, tipka, dvoklik, desni klik, izlaz, label.
    /// </summary>
    public static class Form2Dogadjaj
    {
        /// <summary>Form2.vb 5825–5871. Promjena teksta ide u Form3.Trazilica. Prazan ili isti tekst koji nije * ne ide.</summary>
        public static string TrazilicaKorak(string tekst, string stari)
        {
            if (Carpo.NetGen.Kontrole.Ponasanje.PreskociTrazilicu(tekst, stari))
                return "preskoci";
            return "trazilica";
        }

        public sealed class Tipka
        {
            public bool ValidirajTrazilicu;
            public bool Sljedeca;
            public bool ObrisiStariTekst;
            public bool ControlPritisnut;
            public bool OdaberiSve;
            public bool Kopiraj;
            public string EnterNaredba = "";
        }

        /// <summary>Form2.Glbl_KeyDown 6016–6100. Enter je 13, Ctrl je 17, A je 65, C je 67.</summary>
        public static Tipka KeyDown(bool trazilica, bool grid, bool viselinijski, bool enterKaoTab, bool editPrazan, int tipka, bool control, string onEnter, string onLost, string greska)
        {
            var t = new Tipka();
            if (trazilica && tipka == 13)
            {
                t.ValidirajTrazilicu = true;
                bool fokusGreska = (greska ?? "").ToLowerInvariant().IndexOf("focus(", StringComparison.Ordinal) >= 0;
                bool fokusSql = (onLost ?? "").ToLowerInvariant().IndexOf("focus", StringComparison.Ordinal) >= 0;
                if (!fokusGreska && !fokusSql)
                    t.Sljedeca = true;
                return t;
            }
            if (trazilica)
                t.ObrisiStariTekst = true;
            if (tipka == 13)
            {
                if (grid)
                {
                    if (editPrazan)
                        t.Sljedeca = true;
                    if (!string.IsNullOrWhiteSpace(onEnter))
                        t.EnterNaredba = onEnter.Trim();
                }
                else if (viselinijski)
                {
                    if (enterKaoTab)
                        t.Sljedeca = true;
                }
                else
                    t.Sljedeca = true;
            }
            if (grid && tipka == 17)
                t.ControlPritisnut = true;
            if (grid && control && tipka == 65)
                t.OdaberiSve = true;
            if (grid && control && tipka == 67)
                t.Kopiraj = true;
            return t;
        }

        /// <summary>Form2.Glbl_DoubleClick 6198–6204. Val različit od 0 mijenja zarez u točku.</summary>
        public static string VrijednostDvoklika(string sirovo)
        {
            string v = sirovo ?? "";
            if (VbVal(v) != 0)
                return v.Replace(",", ".");
            return v;
        }

        public static double VbVal(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            int i = 0;
            var sb = new StringBuilder();
            if (s[0] == '-' || s[0] == '+')
            {
                sb.Append(s[0]);
                i = 1;
            }
            bool tocka = false;
            for (; i < s.Length; i++)
            {
                if (char.IsDigit(s[i]))
                    sb.Append(s[i]);
                else if (s[i] == '.' && !tocka)
                {
                    sb.Append('.');
                    tocka = true;
                }
                else
                    break;
            }
            double n;
            if (!double.TryParse(sb.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out n))
                return 0;
            return n;
        }

        /// <summary>Form2.Glbl_GridMouseUp 8704–8731. Desni klik uz Ctrl je izbornik stupaca, inače POPUP caption:naredba.</summary>
        public static string DesniKlik(bool desni, bool control)
        {
            if (!desni)
                return "";
            return control ? "stupci" : "izbornik";
        }

        public static List<string> Izbornik(string popup)
        {
            var lista = new List<string>();
            string[] a = (popup ?? "").Split('#');
            for (int j = 0; j < a.Length; j++)
            {
                string s = a[j] ?? "";
                int d = s.IndexOf(':');
                if (d < 0)
                    continue;
                lista.Add(s.Substring(0, d) + "|" + s.Substring(d + 1));
            }
            return lista;
        }

        /// <summary>Form2.Glbl_Validated2 5916. KeyDown na početku gasi zastavicu (6020). Validated je pali.</summary>
        public sealed class IzlazTrazilice
        {
            public bool OdradioNaKeydown;

            public void KeyDownPocinje()
            {
                OdradioNaKeydown = false;
            }

            public void Validated()
            {
                OdradioNaKeydown = true;
            }

            public bool Validated2Smije()
            {
                return !OdradioNaKeydown;
            }
        }

        /// <summary>Form2.Glbl_ValidatedMultiDrop 5968–5971. Naredba je ErrorText.</summary>
        public static string MultiDropNaredba(string errorText)
        {
            return (errorText ?? "").Trim();
        }

        /// <summary>Form2.Glbl_Click 6149–6151. Label šalje Tag.</summary>
        public static string KlikLabele(string tag)
        {
            return (tag ?? "").Trim();
        }

        /// <summary>Form2.Glbl_CellEdited 8680–8688. Greška vraća staru vrijednost.</summary>
        public static bool VratiStaru(string greska)
        {
            return !string.IsNullOrEmpty(greska);
        }
    }
}
