using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Događaji Form2 koji su bili rupa: tražilica, tipka, dvoklik, desni klik, timer, datoteka odgovora, JIR.
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

        /// <summary>Form2.vb 5875–5881. ShowDialog samo kad redova nije 0 i nije 1.</summary>
        public static bool PokaziTrazilicu(int redova)
        {
            return redova != 0 && redova != 1;
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

        /// <summary>Form2.Timer1_Tick 8510–8514. Nakon tražilice fokus, pa se timer gasi.</summary>
        public sealed class TimerFokusa
        {
            public bool Upaljen;

            public void NakonTrazilice()
            {
                Upaljen = true;
            }

            public bool Tick()
            {
                if (!Upaljen)
                    return false;
                Upaljen = false;
                return true;
            }
        }

        /// <summary>Form2.SpremiResponse 7900–7906 i SpremiRacunResponse 7909–7915.</summary>
        public static string Datoteka(string vrsta)
        {
            if (string.Equals(vrsta, "racun", StringComparison.OrdinalIgnoreCase))
                return "PosaljiRacunnaServerResponse.txt";
            return "PosaljiZakljucaknaServerResponse.txt";
        }

        public static void SpremiOdgovor(string mapa, string vrsta, string odgovor, string broj, string godina)
        {
            string put = Path.Combine(mapa ?? "", Datoteka(vrsta));
            var sb = new StringBuilder();
            sb.Append("Response od servera , brojdokumenta: ").Append(broj).Append(", godina").Append(godina);
            sb.Append("\r\n");
            sb.Append(odgovor ?? "");
            sb.Append("\r\n");
            File.WriteAllText(put, sb.ToString());
        }

        /// <summary>Form2.PosaljiSamoJIRServer 8097–8167. Tijelo je &lt;doc&gt;&lt;/doc&gt;. Produkcijski CIS se ne zove.</summary>
        public const string JirTijelo = "<doc></doc>";

        public static bool CisProdukcija(string url)
        {
            return (url ?? "").IndexOf("cis.porezna-uprava.hr", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string UrlJir(string servis, string trgovina, string kasa, string godina, string broj, string vrijeme, string jir, string datum)
        {
            if (string.IsNullOrWhiteSpace(servis) || CisProdukcija(servis))
                return "";
            string v = string.IsNullOrEmpty(vrijeme) ? "null" : "'" + vrijeme + "'";
            string j = string.IsNullOrEmpty(jir) ? "null" : "'" + jir + "'";
            return servis.Trim() + "xmlrec.asp?a=2&v=1&g=5555555&oznakatrgovine=" + (trgovina ?? "")
                + "&kasa=" + (kasa ?? "") + "&godina=" + (godina ?? "") + "&brojdok=" + (broj ?? "")
                + "&vrijemejir=" + v + "&jir=" + j + "&datumdok=" + (datum ?? "");
        }
    }
}
