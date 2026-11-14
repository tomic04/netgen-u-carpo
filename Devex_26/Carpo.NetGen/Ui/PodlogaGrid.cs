using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
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
                GridIzgled.Ukupno(pogled, tablica, kolone);
            pogled.OptionsView.ShowFooter = ukupno;
            if (!master)
                return;
            pogled.OptionsCustomization.AllowSort = _master.Stanje.SortKlikom;
            GridIzgled.Zamrzni(pogled, _master.Stanje.ZamrznuteKolone, _master.Stanje.CheckboxVidljiv);
        }
    }
}
