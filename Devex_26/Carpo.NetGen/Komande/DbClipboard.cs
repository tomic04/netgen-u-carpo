using System;
using System.Collections.Generic;
using System.Data;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// NetGenDBClipboard (Form2.vb 1099-1104, 1170-1182, SpremiuDBClipboard 4000-4023): forma s
    /// DBCLIPBOARD="true", ili s "filter" u imenu ako DBCLIPBOARD nije "false", pamti vrijednosti
    /// kontrola po operateru. Ključ je "ime.xml/kontrola" (DBC-01).
    /// </summary>
    public static class DbClipboard
    {
        public static bool Ukljucen(string imeForme, string dbclipboard)
        {
            string d = (dbclipboard ?? "").Trim().ToLowerInvariant();
            if ((imeForme ?? "").ToLowerInvariant().Contains("filter") && d != "false")
                return true;
            return d == "true";
        }

        public static string CitajSql(string operaterId)
        {
            return "select * from NetGenDBClipboard where operaterid='" + (operaterId ?? "") + "'";
        }

        /// <summary>Čitanje uspoređuje r("key").Trim.ToLower s xml.ToLower + "/" + ime.ToLower.</summary>
        public static string Kljuc(string xml, string ime)
        {
            return (xml ?? "").Trim().ToLowerInvariant() + "/" + (ime ?? "").Trim().ToLowerInvariant();
        }

        /// <summary>Petlja po svim redovima bez izlaza: zadnji red s ključem vrijedi.</summary>
        public static Dictionary<string, string> Mapa(DataTable t)
        {
            var d = new Dictionary<string, string>(StringComparer.Ordinal);
            if (t == null || !t.Columns.Contains("key") || !t.Columns.Contains("value"))
                return d;
            foreach (DataRow r in t.Rows)
            {
                string k = r["key"] == DBNull.Value ? "" : Convert.ToString(r["key"]).Trim().ToLowerInvariant();
                d[k] = r["value"] == DBNull.Value ? "" : Convert.ToString(r["value"]);
            }
            return d;
        }

        /// <summary>4008-4017: ključ se piše kao "ime.xml/Kontrola" (bez ToLower), vrijednost Trim.</summary>
        public static string BrojSql(string operaterId, string xml, string ime)
        {
            return "select count(*) from NetGenDBClipboard where operaterid=" + operaterId + " and [key]='" + KljucZaPis(xml, ime) + "'";
        }

        public static string UbaciSql(string operaterId, string xml, string ime, string vrijednost)
        {
            return "inseRt into NetGenDBClipboard (operaterid,[key],[value]) values(" + operaterId + ",'" + KljucZaPis(xml, ime) + "','" + (vrijednost ?? "").Trim() + "')";
        }

        public static string IzmijeniSql(string operaterId, string xml, string ime, string vrijednost)
        {
            return "update NetGenDBClipboard set [value]='" + (vrijednost ?? "").Trim() + "' where operaterid=" + operaterId + " and [key]='" + KljucZaPis(xml, ime) + "'";
        }

        private static string KljucZaPis(string xml, string ime)
        {
            return (xml ?? "") + "/" + (ime ?? "").Trim();
        }

        /// <summary>CheckBox: CheckState.ToString ("Checked"); čitanje gleda samo "checked".</summary>
        public static bool Kvacica(string v)
        {
            return (v ?? "").Trim().ToLowerInvariant() == "checked";
        }
    }
}
