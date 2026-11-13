using System;
using System.Globalization;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Ostale grane Form2.Rafalno koje nisu OK/CLOSE/MSGBOX.
    /// </summary>
    public static class RafalNaredba
    {
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

        public static string CreateObject(string segment)
        {
            return Unutar(segment, "CREATEOBJECT(");
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
