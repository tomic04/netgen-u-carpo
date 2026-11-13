using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Form1.Rafalno (6546-6910): lanac na podlozi. Zovu ga ONCELLEDITED (6530), ONCELLEDITEDDET (7122),
    /// BEFORECELLEDITED (7865), ONCLOSED (1306) i desni klik s '~' (5757). Drugačiji je od
    /// NovaGenerickaForma (4543): obični SQL segment se izvršava, a ne otvara formu (RAF-01).
    /// </summary>
    public static class Form1Rafal
    {
        /// <summary>6553-6555: prazan tekst nema segmenata, inače Split po '#'.</summary>
        public static string[] Segmenti(string tekst)
        {
            if (tekst == null || tekst.Trim().Length == 0)
                return new string[0];
            return tekst.Split('#');
        }

        /// <summary>6559-6563: ":HIDECOLUMNS" gubi dvotočku, ":select" postaje "select" (Replace je osjetljiv na mala/velika).</summary>
        public static string Pripremi(string segment)
        {
            string s = (segment ?? "").Replace(":HIDECOLUMNS", "HIDECOLUMNS");
            if (s.Trim().ToLowerInvariant().StartsWith(":select", StringComparison.Ordinal))
                s = s.Replace(":select", "select");
            return s;
        }

        /// <summary>
        /// Grane 6575-6907 istim redom. OK/OKSTAY/CANCEL/CLOSE/CLOSEPARENT zovu Button1_Click/Button2_Click
        /// s grida kao pošiljateljem: NovaGenerickaForma(sender.Tag) s praznim Tagom ne radi ništa.
        /// STARTSEARCH, REFRESHCONTROL, PRINTGRID, GRID_COLUMN_VALUE, PREPAREMULTISELECTEDPRINT i CLICK su
        /// zakomentirani. FOREACH* preskače sljedeći segment bez petlje.
        /// </summary>
        public static string Vrsta(string segment)
        {
            string t = (segment ?? "").Trim();
            if (t.Length == 0)
                return "prazno";
            string u = t.ToUpperInvariant();
            if (u == "NULL")
                return "prazno";
            if (u.Contains("BEEP()"))
                return "beep";
            if (u.Contains("OPENFORM("))
                return "openform";
            if (u.Contains("BLAGAJNAENABLE("))
                return "blagajna";
            if (u.Contains("STARTSEARCH("))
                return "nista";
            if (u.Contains(":ASSEMBLYLOAD("))
                return "assembly";
            if (t.Substring(0, 1) == ":")
                return "forma";
            if (u == "OKSTAY" || u == "OK")
                return "nista";
            if (u == "REFRESHMASTERGRID")
                return "osvjezi";
            if (u.Contains("REFRESHCONTROL("))
                return "nista";
            if (u.Contains("ASSEMBLYLOAD("))
                return "assembly";
            if (u.Contains("HIDECOLUMNS(") || u.Contains("SHOWCOLUMNS("))
                return "stupci";
            if (u.Contains("ISEMPTY("))
                return "isempty";
            if (u.Contains("PRINTGRID(") || u.Contains("GRID_COLUMN_VALUE("))
                return "nista";
            if (u == "CANCEL" || u == "CLOSE")
                return "nista";
            if (t.Contains("COLORPICKER("))
                return "boja";
            if (u == "CLOSEPARENT")
                return "nista";
            if (u.Contains("PREPAREMULTISELECTEDPRINT("))
                return "nista";
            if (u.Contains("FOREACHSELECTED(") || u.Contains("FOREACHBLUEROW(") || u.Contains("FOREACHROW("))
                return "foreach";
            if (u.Contains("CLICK("))
                return "nista";
            return "sql";
        }

        /// <summary>
        /// 6768-6776: param je segment ToUpper.Trim bez "ISEMPTY(" i bez svih ')'. Bez Trim-a unutra:
        /// "ISEMPTY( null )" ne preskače (PraznoNaredba u lancu gumba obreže, ovdje VB ne).
        /// </summary>
        public static bool IsEmptyPreskace(string segment)
        {
            string p = (segment ?? "").ToUpperInvariant().Trim().Replace("ISEMPTY(", "").Replace(")", "");
            return p.ToLowerInvariant() == "null" || p.Trim().Length == 0;
        }

        /// <summary>6583-6586: OPENFORM( i sve ')' van iz cijelog segmenta, velikim slovima.</summary>
        public static string OpenFormIme(string segment)
        {
            return (segment ?? "").ToUpperInvariant().Trim().Replace("OPENFORM(", "").Replace(")", "");
        }

        /// <summary>6825-6832: COLORPICKER( i ')' van iz originalnog segmenta (prije zamjene @@).</summary>
        public static string BojaSql(string original)
        {
            return (original ?? "").Trim().Replace("COLORPICKER(", "").Replace(")", "");
        }

        /// <summary>
        /// GridEX1_CellUpdated 6501-6516 (i GridEX2 7092-7107): svaki stupac s podatkom, ime bez zadnje
        /// 4 znamenke ako nisu 0, razmak postaje '_', vrijednost ToString u kulturi računala sa zarezom u
        /// točku, uvijek u navodnicima. DBNull nije Nothing pa ide kao ''. '!' ostaje (PokupiParametreSaGrida ga skida).
        /// </summary>
        public static string ParametriCelije(DataRow red)
        {
            return ParametriCelije(red, true);
        }

        /// <summary>
        /// GridEX1_EditingCell (7829-7843, BEFORECELLEDITED): zarez u točku samo kad je Val različit od 0,
        /// tekst ostaje sa zarezom (zarezUvijek = false).
        /// </summary>
        public static string ParametriCelije(DataRow red, bool zarezUvijek)
        {
            if (red == null || red.Table == null)
                return "";
            var sb = new StringBuilder();
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                object v = red[i];
                if (v == null)
                    continue;
                string ime = (red.Table.Columns[i].ColumnName ?? "").Trim();
                if (ime.Length >= 4 && Form2Dogadjaj.VbVal(ime.Substring(ime.Length - 4, 4)) != 0)
                    ime = ime.Substring(0, ime.Length - 4);
                string tekst = v == DBNull.Value ? "" : (Convert.ToString(v, CultureInfo.CurrentCulture) ?? "");
                if (zarezUvijek || Form2Dogadjaj.VbVal(tekst) != 0)
                    tekst = tekst.Replace(",", ".");
                sb.Append("@").Append(ime.Replace(" ", "_")).Append("='").Append(tekst).Append("' , ");
            }
            return sb.ToString();
        }

        /// <summary>6488-6496: @@LASTVALUE. Nothing je prazno, broj (Val različit od 0) dobije točku.</summary>
        public static string ZadnjaVrijednost(object v)
        {
            if (v == null)
                return "";
            string s = v == DBNull.Value ? "" : (Convert.ToString(v, CultureInfo.CurrentCulture) ?? "");
            if (Form2Dogadjaj.VbVal(s) != 0)
                return s.Replace(",", ".");
            return s;
        }

        /// <summary>3468-3472: EDITCOLUMNS koji počinje sa sp ili select je upit, stupci su prva ćelija.</summary>
        public static bool EditStupciSuUpit(string editColumns)
        {
            string t = (editColumns ?? "").ToLowerInvariant();
            return t.StartsWith("sp", StringComparison.Ordinal) || t.StartsWith("select", StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Class1.ExecSql Catch (157-200): što korisnik vidi i ostaje li GreskaKodIzvrsavanjaSQLa.
    /// </summary>
    public static class SqlPoruka
    {
        public sealed class Ishod
        {
            public string Greska = "";
            public string Tekst = "";
            public bool Prikazi;
        }

        /// <summary>157-166: poruka o ssoperateri, totalgenlang, sspermission, NetGenDBFilter, NetGenDBClipboard je tiha.</summary>
        public static bool Tiha(string poruka)
        {
            string m = (poruka ?? "").ToLowerInvariant();
            return m.Contains("ssoperateri") || m.Contains("totalgenlang ") || m.Contains("sspermission")
                || m.Contains("netgendbfilter") || m.Contains("netgendbclipboard");
        }

        /// <summary>
        /// 168-200: s '#' ili '+' poruka je tekst prije znaka (InStr - 2), "#OK"/"+OK" briše grešku.
        /// Inače cijela poruka bez rečenice o okidaču.
        /// </summary>
        public static Ishod IzIznimke(string poruka)
        {
            var i = new Ishod();
            string m = poruka ?? "";
            if (Tiha(m))
                return i;
            i.Prikazi = true;
            i.Greska = m;
            if (m.Contains("#") || m.Contains("+"))
            {
                string ff = "";
                if (m.Contains("#"))
                    ff = Lijevo(m, m.IndexOf('#') - 1);
                if (m.Contains("+"))
                    ff = Lijevo(m, m.IndexOf('+') - 1);
                i.Tekst = ff;
                if (m.Contains("#OK") || m.Contains("+OK"))
                    i.Greska = "";
                return i;
            }
            i.Tekst = m.Replace("The transaction ended in the trigger. The batch has been aborted.", "");
            return i;
        }

        private static string Lijevo(string s, int duljina)
        {
            if (duljina <= 0)
                return "";
            return duljina >= s.Length ? s : s.Substring(0, duljina);
        }
    }
}
