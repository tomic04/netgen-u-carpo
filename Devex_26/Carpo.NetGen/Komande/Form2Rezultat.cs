using System;
using System.Data;
using System.Globalization;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Prvi red rezultata SQL-a iz lanca dijaloga. Form2.RefreshControls (4556-5190) i
    /// DodatnoPostimajKontrolu (4497-4552). Ovdje je samo odluka; Dijalog.PrimeniRed je primijeni.
    /// </summary>
    public static class Form2Rezultat
    {
        /// <summary>
        /// VB cita Trs2.Rows(0)("ime") pod On Error Resume Next. Nema stupca ili je DBNull:
        /// dodjela u String baci iznimku i vrijednost ostane "". DataRow trazi ime bez obzira na velika slova.
        /// </summary>
        public static bool Ima(DataRow red, string ime)
        {
            if (red == null || red.Table == null || string.IsNullOrEmpty(ime))
                return false;
            if (!red.Table.Columns.Contains(ime))
                return false;
            object v = red[ime];
            return v != null && v != DBNull.Value;
        }

        public static string Celija(DataRow red, string ime)
        {
            if (!Ima(red, ime))
                return "";
            object v = red[ime];
            if (v is byte[])
                return "";
            return Convert.ToString(v, CultureInfo.CurrentCulture) ?? "";
        }

        /// <summary>
        /// DodatnoPostimajKontrolu: visible, invisible, tabstoped, tabnotstoped, enable, disable, refresh.
        /// Prazno ne radi nista. Sve ostalo je "ostalo" (Case Else: tekst labele, red grida, adresa preglednika).
        /// </summary>
        public static string Akcija(string vrijednost)
        {
            string t = (vrijednost ?? "").Trim().ToLowerInvariant();
            if (t.Length == 0)
                return "";
            if (t == "visible" || t == "invisible" || t == "tabstoped" || t == "tabnotstoped"
                || t == "enable" || t == "disable" || t == "refresh")
                return t;
            return "ostalo";
        }

        /// <summary>
        /// RefreshControls 4948, 4975, 4992, 5121: ove cetiri rijeci ne idu u tekst ni u vrijednost.
        /// tabstoped/tabnotstoped VB upise u tekst (nije u popisu); Carpo ih ne upisuje.
        /// </summary>
        public static bool JeStanje(string vrijednost)
        {
            string t = (vrijednost ?? "").Trim().ToLowerInvariant();
            return t == "disable" || t == "enable" || t == "visible" || t == "invisible"
                || t == "tabstoped" || t == "tabnotstoped";
        }

        /// <summary>
        /// Broj (NumerBox, 4879): ni "refresh" ne ide u vrijednost.
        /// </summary>
        public static bool BrojDobivaVrijednost(string vrijednost)
        {
            string t = (vrijednost ?? "").Trim().ToLowerInvariant();
            return t.Length > 0 && !JeStanje(t) && t != "refresh";
        }

        /// <summary>
        /// CheckBox.Checked = vrijednost (5008). VB CBool: "True"/"False" i broj (nije 0 je True).
        /// Ostalo baci iznimku, pa kvacica ostane kakva je (null).
        /// </summary>
        public static bool? VbBool(string vrijednost)
        {
            string t = (vrijednost ?? "").Trim();
            if (t.Length == 0)
                return null;
            if (string.Equals(t, "true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(t, "false", StringComparison.OrdinalIgnoreCase))
                return false;
            double d;
            if (double.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out d)
                || double.TryParse(t, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                return d != 0;
            return null;
        }

        /// <summary>RadioButton (5017): samo "1" ili "true" ukljuci.</summary>
        public static bool RadioUkljuci(string vrijednost)
        {
            string t = (vrijednost ?? "").Trim();
            return t == "1" || string.Equals(t, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>beep (4651-4661): Val, najvise 30.</summary>
        public static int Beep(string vrijednost)
        {
            double n = Form2Dogadjaj.VbVal((vrijednost ?? "").Trim());
            if (n > 30)
                n = 30;
            return n < 1 ? 0 : (int)n;
        }

        /// <summary>
        /// Combo (5125-5150): vrijednost postane SelectedValue osim cetiri rijeci i tocno "refresh"
        /// (malim slovima, bez Trim).
        /// </summary>
        public static bool ComboDobivaVrijednost(string vrijednost)
        {
            string v = vrijednost ?? "";
            if (v.Length == 0 || JeStanje(v))
                return false;
            return v.ToLowerInvariant() != "refresh";
        }

        /// <summary>Grid (5043): InStr(Tstr, "refresh"), pa i "refreshmovelast".</summary>
        public static bool GridOsvjezi(string vrijednost)
        {
            return (vrijednost ?? "").ToLowerInvariant().IndexOf("refresh", StringComparison.Ordinal) >= 0;
        }

        /// <summary>Combo (5151): Tstr = "refresh", malim slovima, bez Trim.</summary>
        public static bool ComboOsvjezi(string vrijednost)
        {
            return (vrijednost ?? "").ToLowerInvariant() == "refresh";
        }

        /// <summary>Trazilica (4942-4968): Tag dobije vrijednost osim cetiri rijeci. Tag "0" brise tekst osim "*".</summary>
        public static bool TrazilicaDobivaId(string vrijednost)
        {
            string v = vrijednost ?? "";
            return v.Length > 0 && !JeStanje(v);
        }

        /// <summary>Tooltip trazilice je CAPTION bez "_" (2147-2156), pa "#"; stupac za tekst je taj dio bez "*".</summary>
        public static string TrazilicaStupac(string caption)
        {
            return (caption ?? "").Replace("_", "").Replace("*", "");
        }
    }
}
