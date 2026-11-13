using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Postavke grida podloge koje je MasterGrid/DetailGrid izračunao, a VeziGrid nije primjenjivao:
    /// TotalRow (TOTALROWVISIBLE/DETTOTALROWVISIBLE, 236 granula) sa zbrojem decimala i AGREGATEFUNCTION,
    /// AUTOMATICSORT i FROZENCOLUMNS (Form1.vb 3362, 3401-3409, 2009-2013, 7507). TOT-01.
    /// </summary>
    public sealed partial class Podloga
    {
        private void PrimijeniUkupno(GridView pogled, DataTable tablica, List<GridKolona> kolone)
        {
            bool master = pogled == gvGla;
            bool ukupno = master ? _master.Stanje.TotalRed : _detail.Stanje.TotalRed;
            if (kolone != null)
            {
                for (int i = 0; i < kolone.Count; i++)
                {
                    GridKolona k = kolone[i];
                    if (k.Selektor || string.IsNullOrEmpty(k.Podatak))
                        continue;
                    GridColumn c = pogled.Columns.ColumnByFieldName(k.Podatak);
                    if (c == null)
                        continue;
                    c.Summary.Clear();
                    string vrsta = GridAgregat.Vrsta(k.Agregat);
                    if (vrsta.Length == 0)
                        continue;
                    Type tip = tablica != null && tablica.Columns.Contains(k.Podatak) ? tablica.Columns[k.Podatak].DataType : null;
                    c.Summary.Add((SummaryItemType)Enum.Parse(typeof(SummaryItemType), vrsta), k.Podatak, GridAgregat.Format(vrsta, tip, k.Format));
                }
            }
            pogled.OptionsView.ShowFooter = ukupno;
            if (!master)
                return;
            pogled.OptionsCustomization.AllowSort = _master.Stanje.SortKlikom;
            int zamrznuti = GridAgregat.ZamrznutiPodaci(_master.Stanje.ZamrznuteKolone, _master.Stanje.CheckboxVidljiv);
            int n = 0;
            for (int i = 0; i < pogled.VisibleColumns.Count && n < zamrznuti; i++)
            {
                pogled.VisibleColumns[i].Fixed = FixedStyle.Left;
                n++;
            }
        }
    }
}
