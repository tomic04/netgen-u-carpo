using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;

namespace Carpo.NetGen.Kontrole
{
    /// <summary>
    /// Form1.SetDefolte 3945–4009. Prije PokaziUpit (3862–3882).
    /// Ključ u CB je ime stupca plus '='. Jedan vodeći i jedan završni navodnik otpadaju.
    /// </summary>
    public static class ClipboardNg
    {
        public static Dictionary<string, string> IzReda(DataTable tablica)
        {
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (tablica == null || tablica.Rows.Count == 0)
                return cb;
            DataRow red = tablica.Rows[0];
            foreach (DataColumn c in tablica.Columns)
            {
                string ime = string.IsNullOrEmpty(c.Caption) ? c.ColumnName : c.Caption;
                string v = red[c] == null || red[c] == DBNull.Value ? "null" : Convert.ToString(red[c]);
                cb[ime + "="] = Sredi(v);
            }
            return cb;
        }

        /// <summary>Segmenti na #. Greška (null tablica) prekida, kao Exit Sub kad GreskaKodIzvrsavanjaSQLa nije prazan.</summary>
        public static Dictionary<string, string> IzSetDefaults(string sql, SqlKontekst k, Func<string, DataTable> citaj)
        {
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] dijelovi = Rijetke.SetDefaults(sql);
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string dio = (dijelovi[i] ?? "").Trim();
                if (dio.Length == 0)
                    continue;
                string spremno = SrediPoslije.Petlja(SqlTekst.Sredi(dio, k), "", cb, false).Replace("~~", ",");
                if (citaj == null || SqlTekst.Odluka(spremno) != "salji" || SqlTekst.JeZabranaPisanja(spremno))
                    return cb;
                DataTable t = null;
                try
                {
                    t = citaj(spremno);
                }
                catch (Exception)
                {
                    return cb;
                }
                if (t == null)
                    return cb;
                foreach (var par in IzReda(t))
                    cb[par.Key] = par.Value;
            }
            return cb;
        }

        public static string Sredi(string vrijednost)
        {
            string v = vrijednost ?? "null";
            if (v.StartsWith("'", StringComparison.Ordinal) && v.Length > 1)
                v = v.Substring(1);
            if (v.EndsWith("'", StringComparison.Ordinal) && v.Length > 0)
                v = v.Substring(0, v.Length - 1);
            return v.Trim();
        }
    }
}
