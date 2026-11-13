using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Ostale grane Form2.Rafalno koje nisu OK/CLOSE/MSGBOX.
    /// </summary>
    public static class RafalNaredba
    {
        public static int BeepPoziva;

        /// <summary>Form2.vb 6825–6827 i Form1.vb 6579–6581. Beep pa 300 ms.</summary>
        public static void Beep()
        {
            BeepPoziva++;
            try { Console.Beep(); } catch { }
            System.Threading.Thread.Sleep(300);
        }

        public static void PostaviDock(Control c, string stil)
        {
            if (c == null)
                return;
            if (stil == "Fill")
                c.Dock = DockStyle.Fill;
            else if (stil == "Left")
                c.Dock = DockStyle.Left;
            else if (stil == "Right")
                c.Dock = DockStyle.Right;
            else if (stil == "Top")
                c.Dock = DockStyle.Top;
            else if (stil == "Bottom")
                c.Dock = DockStyle.Bottom;
        }

        public static void ZapisiDatoteku(string put, int vrsta, string[][] redovi)
        {
            string tekst = DatotekaTekst(vrsta, redovi);
            File.WriteAllBytes(put, DatotekaKod(vrsta).GetBytes(tekst));
        }

        public static Func<string, bool> Pitaj;
        public static Action<string> Upozori;

        /// <summary>ISEMPTYORZEROALERT: prazno ili 0 pita OK/Cancel. Cancel prekida. Form2.vb 7083–7088.</summary>
        public static bool AlertNastavi(string vrijednost, string poruka)
        {
            if (!PrazanIliNula(vrijednost))
                return true;
            if (Pitaj != null)
                return Pitaj(poruka ?? "");
            return MessageBox.Show(poruka ?? "", "Pitanje", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
        }

        /// <summary>ISEMPTYORZEROCANCELBOX: prazno ili 0 je upozorenje i fokus. Form2.vb 7144–7147.</summary>
        public static bool CancelBoxStani(string vrijednost, string poruka)
        {
            if (!PrazanIliNula(vrijednost))
                return false;
            if (Upozori != null)
                Upozori(poruka ?? "");
            else
                MessageBox.Show(poruka ?? "", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return true;
        }

        public static string RefreshKontrola(string segment)
        {
            return Unutar(segment, "REFRESHCONTROL(");
        }

        /// <summary>DOCK1 vrh, DOCK2 dno, DOCK3 lijevo, DOCK4 desno, DOCK5 fill. Form2.vb 6916–6940.</summary>
        public static string Dock(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            if (u.IndexOf("DOCK5(", StringComparison.Ordinal) >= 0)
                return "Fill";
            if (u.IndexOf("DOCK4(", StringComparison.Ordinal) >= 0)
                return "Right";
            if (u.IndexOf("DOCK3(", StringComparison.Ordinal) >= 0)
                return "Left";
            if (u.IndexOf("DOCK2(", StringComparison.Ordinal) >= 0)
                return "Bottom";
            if (u.IndexOf("DOCK1(", StringComparison.Ordinal) >= 0)
                return "Top";
            return "";
        }

        public static string DockKontrola(string segment)
        {
            string d = Dock(segment);
            if (d.Length == 0)
                return "";
            string u = (segment ?? "").ToUpperInvariant();
            int broj = d == "Fill" ? 5 : d == "Right" ? 4 : d == "Left" ? 3 : d == "Bottom" ? 2 : 1;
            return Unutar(u, "DOCK" + broj.ToString(CultureInfo.InvariantCulture) + "(");
        }

        public static bool MasterSljedeci(string segment)
        {
            return (segment ?? "").ToUpperInvariant().IndexOf("MASTERMOVENEXT", StringComparison.Ordinal) >= 0;
        }

        public static bool MasterPrethodni(string segment)
        {
            return (segment ?? "").ToUpperInvariant().IndexOf("MASTERMOVEPREVIOUS", StringComparison.Ordinal) >= 0;
        }

        /// <summary>EVAL(kontrola=izraz). Zadnji = reže. Form2.vb 7090–7112.</summary>
        public static string EvalLijevo(string segment)
        {
            string p = EvalTijelo(segment);
            int j = p.LastIndexOf('=');
            if (j < 0)
                return p;
            return p.Substring(0, j).Trim();
        }

        public static string EvalDesno(string segment)
        {
            string p = EvalTijelo(segment);
            int j = p.LastIndexOf('=');
            if (j < 0)
                return "";
            return p.Substring(j + 1).Trim();
        }

        public static bool PrazanIliNula(string vrijednost)
        {
            string v = (vrijednost ?? "").Trim();
            return v.Length == 0 || v.Equals("null", StringComparison.OrdinalIgnoreCase) || v == "0";
        }

        /// <summary>ISEMPTYORZEROALERT(param;poruka). Form2.vb 7053–7088.</summary>
        public static string UpozorenjeParam(string segment)
        {
            return PrijeTocke(segment, "ISEMPTYORZEROALERT(");
        }

        public static string UpozorenjeTekst(string segment)
        {
            return IzaTocke(segment, "ISEMPTYORZEROALERT(");
        }

        /// <summary>Bez ; izlazi iz petlje. Form2.vb 7126–7128.</summary>
        public static bool OtkazImaPoruku(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            if (u.IndexOf("ISEMPTYORZEROCANCELBOX(", StringComparison.Ordinal) < 0)
                return false;
            return u.IndexOf(';') >= 0;
        }

        public static string OtkazParam(string segment)
        {
            return PrijeTocke(segment, "ISEMPTYORZEROCANCELBOX(");
        }

        public static string OtkazTekst(string segment)
        {
            return IzaTocke(segment, "ISEMPTYORZEROCANCELBOX(");
        }

        /// <summary>SLOPP je komentar. Form2.vb 7494–7498.</summary>
        public static bool Slopp(string segment)
        {
            return (segment ?? "").ToUpperInvariant().IndexOf("SLOPP(", StringComparison.Ordinal) >= 0;
        }

        /// <summary>1 ASCII i znak 26, 2 ASCII, 3 zadano kodiranje, 8 UTF-8. Form2.vb 7170–7399.</summary>
        public static int Datoteka(string segment)
        {
            string u = (segment ?? "").ToUpperInvariant();
            if (u.IndexOf("CREATETEXTFILEUTF8(", StringComparison.Ordinal) >= 0)
                return 8;
            if (u.IndexOf("CREATETEXTFILE3(", StringComparison.Ordinal) >= 0)
                return 3;
            if (u.IndexOf("CREATETEXTFILE2(", StringComparison.Ordinal) >= 0)
                return 2;
            if (u.IndexOf("CREATETEXTFILE1(", StringComparison.Ordinal) >= 0)
                return 1;
            return 0;
        }

        public static string DatotekaPut(string segment)
        {
            int v = Datoteka(segment);
            if (v == 0)
                return "";
            string kljuc = v == 8 ? "CREATETEXTFILEUTF8(" : "CREATETEXTFILE" + v.ToString(CultureInfo.InvariantCulture) + "(";
            string tijelo = Unutar(segment, kljuc);
            int t = tijelo.LastIndexOf(';');
            if (t < 0)
                return "";
            return tijelo.Substring(0, t).Trim();
        }

        public static string DatotekaSql(string segment)
        {
            int v = Datoteka(segment);
            if (v == 0)
                return "";
            string kljuc = v == 8 ? "CREATETEXTFILEUTF8(" : "CREATETEXTFILE" + v.ToString(CultureInfo.InvariantCulture) + "(";
            string tijelo = Unutar(segment, kljuc);
            int t = tijelo.LastIndexOf(';');
            if (t < 0)
                return "";
            return tijelo.Substring(t + 1).Trim();
        }

        public static Encoding DatotekaKod(int vrsta)
        {
            if (vrsta == 8)
                return new UTF8Encoding(false);
            if (vrsta == 3)
                return Encoding.Default;
            return Encoding.ASCII;
        }

        /// <summary>Stupci se lijepe, red je linija. Vrsta 1 na kraj stavi znak 26.</summary>
        public static string DatotekaTekst(int vrsta, string[][] redovi)
        {
            var sb = new StringBuilder();
            if (redovi != null)
            {
                for (int r = 0; r < redovi.Length; r++)
                {
                    string[] kol = redovi[r] ?? new string[0];
                    for (int c = 0; c < kol.Length; c++)
                        sb.Append(kol[c] ?? "");
                    sb.Append("\r\n");
                }
            }
            if (vrsta == 1)
                sb.Append('\u001a');
            return sb.ToString();
        }

        private static string EvalTijelo(string segment)
        {
            string p = (segment ?? "").Trim();
            int a = p.IndexOf("EVAL(", StringComparison.OrdinalIgnoreCase);
            if (a < 0)
                return "";
            p = p.Substring(a + 5).Trim();
            if (p.EndsWith(")", StringComparison.Ordinal))
                p = p.Substring(0, p.Length - 1);
            return p.Trim();
        }

        private static string PrijeTocke(string segment, string kljuc)
        {
            string t = Unutar(segment, kljuc);
            int z = t.LastIndexOf(';');
            if (z < 0)
                return t;
            return t.Substring(0, z).Trim();
        }

        private static string IzaTocke(string segment, string kljuc)
        {
            string t = Unutar(segment, kljuc);
            int z = t.LastIndexOf(';');
            if (z < 0)
                return "";
            return t.Substring(z + 1).Trim();
        }

        /// <summary>Form2.vb 6828–6835. Samo izravna djeca, ne rekurzija.</summary>
        public static int SveKontrole(Control roditelj, bool ukljuceno)
        {
            if (roditelj == null)
                return 0;
            int n = 0;
            foreach (Control c in roditelj.Controls)
            {
                c.Enabled = ukljuceno;
                n++;
            }
            return n;
        }

        public static Func<string, int> PokreniProces;

        /// <summary>PROCESSSTART Form2.vb 6841–6846.</summary>
        public static int ProcessStart(string segment)
        {
            string p = Unutar(segment, "PROCESSSTART(");
            if (p.Length == 0)
                return -1;
            if (PokreniProces != null)
                return PokreniProces(p);
            try
            {
                Process.Start(p);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public static string OdabranaMapa = "";
        public static string OdabranaDatoteka = "";
        public static Func<string> MapaDijalog;
        public static Func<string> DatotekaDijalog;

        /// <summary>SHOWFOLDERBROWSERDIALOG Form2.vb 6847–6850.</summary>
        public static string OdaberiMapu()
        {
            if (MapaDijalog != null)
            {
                OdabranaMapa = MapaDijalog() ?? "";
                return OdabranaMapa;
            }
            using (var d = new FolderBrowserDialog())
            {
                d.ShowDialog();
                OdabranaMapa = d.SelectedPath ?? "";
            }
            return OdabranaMapa;
        }

        /// <summary>SHOWOPENFILEDIALOG Form2.vb 6851–6855.</summary>
        public static string OdaberiDatoteku()
        {
            if (DatotekaDijalog != null)
            {
                OdabranaDatoteka = DatotekaDijalog() ?? "";
                return OdabranaDatoteka;
            }
            using (var d = new OpenFileDialog())
            {
                d.ShowDialog();
                OdabranaDatoteka = d.FileName ?? "";
            }
            return OdabranaDatoteka;
        }

        public static int TimerInterval;
        public static bool TimerUpaljen;

        /// <summary>TIMERON Form2.vb 6856–6861. Interval je cijeli broj.</summary>
        public static int TimerOn(string segment)
        {
            string p = Unutar(segment, "TIMERON(");
            int n;
            if (!int.TryParse(p, out n) || n < 1)
                n = 100;
            TimerInterval = n;
            TimerUpaljen = true;
            return n;
        }

        /// <summary>TIMEROFF Form2.vb 6862–6863.</summary>
        public static void TimerOff()
        {
            TimerUpaljen = false;
        }

        public static bool ProgresUpaljen;

        /// <summary>PROGRESSON / PROGRESSOFF Form2.vb 6864–6867.</summary>
        public static void Progres(bool upali)
        {
            ProgresUpaljen = upali;
        }

        /// <summary>STARTSEARCH Form2.vb 6873–6881. Form1.vb 6592 je komentar.</summary>
        public static string[] StartSearch(string segment)
        {
            string p = Unutar(segment, "STARTSEARCH(").ToUpperInvariant();
            int z = p.IndexOf(',');
            if (z < 0)
                return new[] { p.Trim(), "" };
            return new[] { p.Substring(0, z).Trim(), p.Substring(z + 1).Trim() };
        }

        /// <summary>CREATEOBJECT Form2.vb 7419–7423. Information, velika slova.</summary>
        public static string CreateObject(string segment)
        {
            return Unutar(segment, "CREATEOBJECT(").ToUpperInvariant();
        }

        /// <summary>GRID_COLUMN_VALUE Form2.vb 7007–7029. Stupac po imenu ili broju, prvi red koji odgovara.</summary>
        public static int NadjiVrijednost(DataTable tablica, string stupac, string vrijednost)
        {
            if (tablica == null || stupac == null)
                return -1;
            int indeks = -1;
            int broj;
            if (int.TryParse(stupac.Trim(), out broj) && broj >= 0 && broj < tablica.Columns.Count)
                indeks = broj;
            else
            {
                for (int c = 0; c < tablica.Columns.Count; c++)
                {
                    if (string.Equals(tablica.Columns[c].ColumnName, stupac.Trim(), StringComparison.OrdinalIgnoreCase)
                        || string.Equals(tablica.Columns[c].Caption, stupac.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        indeks = c;
                        break;
                    }
                }
            }
            if (indeks < 0)
                return -1;
            string trazeno = (vrijednost ?? "").Trim();
            for (int r = 0; r < tablica.Rows.Count; r++)
            {
                string v = tablica.Rows[r][indeks] == DBNull.Value ? "" : Convert.ToString(tablica.Rows[r][indeks]);
                if (string.Equals((v ?? "").Trim(), trazeno, StringComparison.OrdinalIgnoreCase))
                    return r;
            }
            return -1;
        }

        /// <summary>refreshmovetoid Form1.vb 4831–4861. Traži stupac 2.</summary>
        public static int RedStupca2(DataTable tablica, string vrijednost)
        {
            if (tablica == null || tablica.Columns.Count < 3)
                return -1;
            return NadjiVrijednost(tablica, "2", vrijednost);
        }

        /// <summary>findnode Form1.vb 4809–4815. Find praznog imena.</summary>
        public static int PrazniCvorovi(string[] imena)
        {
            if (imena == null)
                return 0;
            int n = 0;
            for (int i = 0; i < imena.Length; i++)
            {
                if ((imena[i] ?? "").Length == 0)
                    n++;
            }
            return n;
        }

        private static string Unutar(string segment, string kljuc)
        {
            string u = (segment ?? "").Trim();
            int a = u.IndexOf(kljuc, StringComparison.OrdinalIgnoreCase);
            if (a < 0)
                return "";
            u = u.Substring(a + kljuc.Length);
            int z = u.LastIndexOf(')');
            if (z >= 0)
                u = u.Substring(0, z);
            return u.Trim();
        }
    }
}
