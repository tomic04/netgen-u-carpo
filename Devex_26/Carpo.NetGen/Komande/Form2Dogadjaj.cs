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

        /// <summary>Form2.Glbl_TREE_BeforeSelect 6103–6105.</summary>
        public static string BojaCvoraPrije()
        {
            return "White";
        }

        /// <summary>Form2.Glbl_Click 6149–6151. Label šalje Tag.</summary>
        public static string KlikLabele(string tag)
        {
            return (tag ?? "").Trim();
        }

        /// <summary>Form2.DajPokaziHelp 9551–9552. Ne pokreće proces u testu.</summary>
        public static string HelpArgumenti(string server, string baza, string help, string operater, string xml)
        {
            string o = operater ?? "";
            if (o.Length > 0 && o[0] != ' ')
                o = " " + o;
            return "-s " + (server ?? "") + " -d " + (baza ?? "") + " -h " + (help ?? "") + " -o " + o + " -p " + (xml ?? "");
        }

        /// <summary>Form2.UpaliProgres 6234–6242.</summary>
        public sealed class Progres
        {
            public int Vrijednost;
            public string Stil = "blocks";
            public int Brzina;

            public void Upali()
            {
                Vrijednost = 20;
                Stil = "marquee";
                Brzina = 40;
            }

            public void Ugasi()
            {
                Stil = "blocks";
                Brzina = 0;
            }
        }

        /// <summary>
        /// Form2.PoslovniProstor_COM 6618–6694. Prazan URL ne postaje produkcija (R109).
        /// COM Raverus.FiskalizacijaDEV.COM i SLOFISKAL se ne učitavaju (R111).
        /// </summary>
        public static string ProstorGrana(bool imaRed, string url, string pfx, string lozinka)
        {
            if (!imaRed)
                return "nema";
            if ((url ?? "").ToLowerInvariant().IndexOf("fu.gov.si", StringComparison.Ordinal) >= 0)
                return "slo";
            if (string.IsNullOrWhiteSpace(pfx))
                return "nema-pfx";
            if (string.IsNullOrWhiteSpace(lozinka))
                return "nema-lozinka";
            if (string.IsNullOrWhiteSpace(url) || CisProdukcija(url))
                return "nema-com";
            return "test-url";
        }

        /// <summary>Form2.ProvjeraDostupnosti 7919–7932. Test daje Probe, ne ide na mrežu.</summary>
        public static Func<string, bool> Probe;

        public static bool Dostupan(string url)
        {
            if (Probe != null)
                return Probe(url ?? "");
            return false;
        }

        /// <summary>Form2.Glbl_CellEdited 8646–8661. Stupac 0 se preskače. Zarez postaje točka.</summary>
        public static string ParametriIzmjene(System.Data.DataRow red)
        {
            if (red == null)
                return "";
            var sb = new StringBuilder();
            for (int i = 1; i < red.Table.Columns.Count; i++)
            {
                System.Data.DataColumn c = red.Table.Columns[i];
                if (string.IsNullOrEmpty(c.Caption) && string.IsNullOrEmpty(c.ColumnName))
                    continue;
                if (red[c] == null || red[c] == System.DBNull.Value)
                    continue;
                string ime = (c.ColumnName ?? "").Trim();
                if (ime.Length >= 4)
                {
                    int n;
                    if (int.TryParse(ime.Substring(ime.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                string v = (System.Convert.ToString(red[c]) ?? "").Replace(",", ".");
                sb.Append('@').Append(ime).Append("='").Append(v).Append("' , ");
            }
            return sb.ToString();
        }

        /// <summary>Form2.Glbl_CellEdited 8680–8688. Greška vraća staru vrijednost.</summary>
        public static bool VratiStaru(string greska)
        {
            return !string.IsNullOrEmpty(greska);
        }

        /// <summary>Form2.GetLayoutDirectory 9260–9269 i SaveGridLayout 9210–9213.</summary>
        public static string MapaRasporeda()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\SPIN Informatica\\Jupiter Software\\Grid Settings\\";
        }

        public static string ImeRasporeda(string projekt, string xml, string forma, string grid, string jezik)
        {
            string ime = (xml ?? "").Trim();
            if (ime.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                ime = ime.Substring(0, ime.Length - 4);
            return "GridLodani-" + (projekt ?? "") + "-" + ime + "-" + (forma ?? "") + "-" + (grid ?? "") + "-" + (jezik ?? "") + ".gxl";
        }

        public static void SpremiRaspored(string mapa, string ime, byte[] sadrzaj)
        {
            Directory.CreateDirectory(mapa ?? "");
            File.WriteAllBytes(Path.Combine(mapa ?? "", ime ?? "grid.gxl"), sadrzaj ?? new byte[0]);
        }

        public static string BrisiRasporedSql(string grid)
        {
            return "select 'refresh' as " + (grid ?? "").Trim();
        }

        /// <summary>Form2.AutoCompleteCombo_KeyUp 9285–9320. Back, strelice, Delete ne traže.</summary>
        public static bool PreskociTipku(int tipka)
        {
            return tipka == 8 || tipka == 37 || tipka == 39 || tipka == 38 || tipka == 46 || tipka == 40;
        }

        public static int NadjiStavku(string[] stavke, string tekst)
        {
            if (stavke == null)
                return -1;
            string upis = tekst ?? "";
            for (int razmak = 0; razmak <= 9; razmak++)
            {
                string trazeno = new string(' ', razmak) + upis;
                for (int i = 0; i < stavke.Length; i++)
                {
                    string s = stavke[i] ?? "";
                    if (s.StartsWith(trazeno, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }
            return -1;
        }

        public static string Dopuna(string upisano, string nadjeno)
        {
            string a = upisano ?? "";
            string b = nadjeno ?? "";
            if (b.Length < a.Length)
                return "";
            return b.Substring(a.Length);
        }

        /// <summary>Form2.AutoCompleteCombo_Leave 9412–9420. Točan tekst ostaje, inače prva stavka.</summary>
        public static int IndeksNakonIzlaza(string[] stavke, string tekst)
        {
            if (stavke == null || stavke.Length == 0)
                return -1;
            string t = tekst ?? "";
            for (int i = 0; i < stavke.Length; i++)
            {
                if (string.Equals(stavke[i] ?? "", t, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return 0;
        }

        /// <summary>Form2.SerialPort1_Postimaj 9585–9591 i Pisi 9594–9600. Test koristi lažni port.</summary>
        public sealed class LazniPort
        {
            public string Ime = "";
            public int Brzina;
            public string Paritet = "None";
            public string Stop = "One";
            public int Bitovi;
            public bool Otvoren;
            public string Zapis = "";

            public void Write(string s)
            {
                Zapis = s ?? "";
            }
        }

        public static void PostimajPort(LazniPort p)
        {
            if (p == null)
                return;
            p.Ime = "";
            p.Brzina = 19200;
            p.Paritet = "None";
            p.Stop = "One";
            p.Bitovi = 8;
        }

        public static string PisiPort(LazniPort p)
        {
            if (p == null)
                return "";
            if (!p.Otvoren)
                p.Otvoren = true;
            p.Write("aaaaa\r");
            string s = p.Zapis;
            if (p.Otvoren)
                p.Otvoren = false;
            return s;
        }

        /// <summary>Form2.Timer2_Tick 9687. Form2.Timer3_Tick 9698–9737.</summary>
        public static bool Timer2ImaSql(string sql)
        {
            return !string.IsNullOrWhiteSpace(sql);
        }

        public static bool Maksimiziraj(string windowState)
        {
            return (windowState ?? "").Trim() == "2";
        }

        public static bool CekiranId(string tag, string vrijednost)
        {
            string niz = ",,," + (tag ?? "").Replace("~", ",") + ",";
            return niz.IndexOf("," + (vrijednost ?? "").Trim() + ",", StringComparison.Ordinal) > 0;
        }

        /// <summary>Form2.Glbl_Gotfocus2 9459. Svijetlo zelena i SelectAll.</summary>
        public static string BojaFokusa()
        {
            return "LightGreen";
        }

        /// <summary>Form2.Glbl_LeaveTrazilica 9455.</summary>
        public static string BojaTrazilice()
        {
            return "LightYellow";
        }

        /// <summary>Form2.ContextMenuStrip7_ItemClicked 9516–9533. Prvi natpis, mala slova. Vidljivost i kvačica se okreću.</summary>
        public static int StupacPoNatpisu(string[] natpisi, string klik)
        {
            string t = (klik ?? "").Trim().ToLowerInvariant();
            if (natpisi == null)
                return 0;
            for (int i = 0; i < natpisi.Length; i++)
            {
                if (string.Equals((natpisi[i] ?? "").Trim().ToLowerInvariant(), t, StringComparison.Ordinal))
                    return i;
            }
            return 0;
        }

        public static bool Okreni(bool vrijednost)
        {
            return !vrijednost;
        }

        /// <summary>Form2.ContextMenuStrip7_Closing 9495. dontclose otkazuje zatvaranje i gasi se.</summary>
        public static bool OtkaziZatvaranje(ref bool neZatvaraj)
        {
            if (!neZatvaraj)
                return false;
            neZatvaraj = false;
            return true;
        }
    }
}
