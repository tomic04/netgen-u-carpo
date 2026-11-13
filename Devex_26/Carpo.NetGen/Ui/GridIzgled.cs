using System;
using System.Data;
using Carpo.NetGen.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Class1.CreateGridColumns (preko MasterGrid.IzPodatka) na DevExpress GridView: natpis bez sirine
    /// na kraju, sirina, skriveni *ID stupci, format, poravnanje i EDITCOLUMNS. Isto radi Podloga.VeziGrid.
    /// </summary>
    public static class GridIzgled
    {
        /// <summary>
        /// Stupci se slazu iznova samo kad se promijeni popis stupaca (RefreshGrida 5311-5322 ostavlja
        /// postojece stupce), pa korisnikova sirina ostaje nakon osvjezavanja.
        /// </summary>
        public static void Primijeni(GridView pogled, DataTable tablica, string editColumns)
        {
            if (pogled == null || tablica == null)
                return;
            if (!IstiStupci(pogled, tablica))
                pogled.PopulateColumns();
            bool uredjuje = false;
            foreach (DataColumn dc in tablica.Columns)
            {
                GridKolona k = MasterGrid.IzPodatka(dc, editColumns);
                GridColumn c = pogled.Columns.ColumnByFieldName(dc.ColumnName);
                if (c == null)
                    continue;
                c.Visible = k.Vidljiva;
                c.Caption = string.IsNullOrEmpty(k.Natpis) ? dc.ColumnName : k.Natpis;
                if (k.Sirina > 0)
                    c.Width = k.Sirina;
                c.OptionsColumn.AllowEdit = k.Uredivanje;
                if (k.Uredivanje)
                    uredjuje = true;
                if (!string.IsNullOrEmpty(k.Format))
                {
                    c.DisplayFormat.FormatString = k.Format;
                    c.DisplayFormat.FormatType = k.Format.IndexOf("yy", StringComparison.OrdinalIgnoreCase) >= 0
                        || k.Format.IndexOf("dd", StringComparison.OrdinalIgnoreCase) >= 0
                        ? DevExpress.Utils.FormatType.DateTime
                        : DevExpress.Utils.FormatType.Numeric;
                }
                if (string.Equals(k.Poravnanje, "Far", StringComparison.OrdinalIgnoreCase))
                    c.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            pogled.OptionsBehavior.Editable = uredjuje;
        }

        private static bool IstiStupci(GridView pogled, DataTable tablica)
        {
            if (pogled.Columns.Count != tablica.Columns.Count)
                return false;
            foreach (DataColumn dc in tablica.Columns)
            {
                if (pogled.Columns.ColumnByFieldName(dc.ColumnName) == null)
                    return false;
            }
            return true;
        }
    }
}
