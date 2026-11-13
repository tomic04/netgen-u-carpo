using System;
using System.Data;
using System.Globalization;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Form3 (trazilica): koji red daje id i tekst, i kako COLUMNCAPTION/COLUMNWIDTH idu na stupce.
    /// Janus grid ima stupac 0 (ikona) i 1 (kvacica) prije podataka, pa je Janus 2 prvi stupac DataTable-a.
    /// </summary>
    public static class TrazilicaTok
    {
        /// <summary>Form3.GetID2 (366-372): Cells("id"), a bez njega Janus stupac 2 (prvi stupac podatka).</summary>
        public static string Id(DataRow red)
        {
            if (red == null || red.Table == null || red.Table.Columns.Count == 0)
                return "0";
            object v = red.Table.Columns.Contains("id") ? red["id"] : red[0];
            string id = v == null || v == DBNull.Value ? "" : (Convert.ToString(v, CultureInfo.CurrentCulture) ?? "").Trim();
            return id.Length == 0 ? "0" : id;
        }

        /// <summary>
        /// Form3.GetID2 (373-385): tekst je Janus stupac 3 (drugi stupac podatka). Stupac cije ime
        /// pocinje s "naziv" ima prednost; petlja ide do kraja, pa vrijedi zadnji takav.
        /// </summary>
        public static string Naziv(DataRow red)
        {
            if (red == null || red.Table == null)
                return "";
            int n = red.Table.Columns.Count;
            string tekst = n > 1 ? Tekst(red[1]) : "";
            string naziv = "";
            for (int i = 0; i < n; i++)
            {
                string ime = (red.Table.Columns[i].ColumnName ?? "").ToLowerInvariant();
                if (ime.StartsWith("naziv", StringComparison.Ordinal))
                    naziv = Tekst(red[i]);
            }
            return naziv.Length > 0 ? naziv : tekst;
        }

        /// <summary>
        /// Form3.Trazilica 177-205: COLUMNCAPTION i COLUMNWIDTH (zarezom) krecu od Janus stupca 3,
        /// tj. od drugog stupca podatka. Sirina 0 sakrije stupac. Vraca natpis ili null.
        /// </summary>
        public static string Natpis(string columnCaption, int stupacPodatka)
        {
            return Dio(columnCaption, stupacPodatka);
        }

        public static int? Sirina(string columnWidth, int stupacPodatka)
        {
            string d = Dio(columnWidth, stupacPodatka);
            if (d == null)
                return null;
            return (int)Form2Dogadjaj.VbVal(d.Trim());
        }

        private static string Dio(string popis, int stupacPodatka)
        {
            if (string.IsNullOrWhiteSpace(popis) || stupacPodatka < 1)
                return null;
            string[] dijelovi = popis.Split(',');
            int i = stupacPodatka - 1;
            return i < dijelovi.Length ? dijelovi[i] : null;
        }

        private static string Tekst(object v)
        {
            return v == null || v == DBNull.Value ? "" : (Convert.ToString(v, CultureInfo.CurrentCulture) ?? "").Trim();
        }
    }
}
