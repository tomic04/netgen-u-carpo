using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Grid dijaloga (INPUT TYPE="grid", Form2.vb 1630-1990): stupci iz Class1.CreateGridColumns i COLUMN
    /// definicija (DGR-01), CLEARCOLUMS, TOTALROW, CHECKBOXVISIBLE, COLUMNHEADERS, GROUPBYBOXVISIBLE
    /// (236 granula), AUTOMATICSORT, FROZENCOLUMNS, FILTERMODE i ENTERKEYBEHAVIOR (DGR-02).
    /// </summary>
    public sealed partial class Dijalog
    {
        private void SloziGridDijaloga(Control c, NgControl k, GridView view, DataTable dt)
        {
            // Form2.vb 1739-1746: TOTALROW (ToUpper, samo "FALSE" gasi).
            bool ukupno = Attr(k, "TOTALROW").ToUpperInvariant() != "FALSE";
            bool clear = Attr(k, "CLEARCOLUMS").ToLowerInvariant() == "true";
            bool slozeno = GridIzgled.Primijeni(view, dt, EditStupci(c, k), ukupno, k == null ? null : k.Kolone, clear);
            // Form2.vb 1748-1755 CHECKBOXVISIBLE, pa 1798 CLEARCOLUMS sakrije sve stupce, i kvacicu (Columns(1)).
            bool kvacica = Attr(k, "CHECKBOXVISIBLE").ToUpperInvariant() != "FALSE" && !clear;
            view.OptionsSelection.MultiSelectMode = kvacica
                ? GridMultiSelectMode.CheckBoxRowSelect
                : GridMultiSelectMode.RowSelect;
            if (!slozeno)
                return;
            // Form2.vb 1708: Janus ColumnHeaders = "2" (InheritableBoolean.False, 1713) skriva zaglavlja (3 grida, GRD-03).
            string zaglavlja = Attr(k, "COLUMNHEADERS").ToLowerInvariant();
            if (zaglavlja == "2" || zaglavlja == "false")
                view.OptionsView.ShowColumnHeaders = false;
            // Form2.vb 1707: GroupByBoxVisible = GROUPBYBOXVISIBLE (CBool; prazno ostavi Janus True). Tekst
            // ploce ostaje Carpov (VB ga brise, 1762).
            bool? grupe = Form2Rezultat.VbBool(Attr(k, "GROUPBYBOXVISIBLE"));
            view.OptionsView.ShowGroupPanel = !grupe.HasValue || grupe.Value;
            // Form2.vb 1686, 1726, 1737: FrozenColumns broji stupac ikone i kvacicu.
            int frozen;
            if (int.TryParse(Attr(k, "FROZENCOLUMNS"), NumberStyles.Integer, CultureInfo.InvariantCulture, out frozen))
                GridIzgled.Zamrzni(view, frozen, kvacica);
            // Form2.vb 1690-1693: AUTOMATICSORT "false" gasi sortiranje klikom na zaglavlje.
            if (Attr(k, "AUTOMATICSORT").ToLowerInvariant() == "false")
                view.OptionsCustomization.AllowSort = false;
            // Form2.vb 1981-1990: FILTERMODE "true" je red za filtriranje (FilterMode.Automatic).
            view.OptionsView.ShowAutoFilterRow = Attr(k, "FILTERMODE").ToLowerInvariant() == "true";
            // Form2.vb 1659-1662: ENTERKEYBEHAVIOR 1 (NextCell) vodi Enter na sljedeci stupac dok se uredjuje.
            view.OptionsNavigation.EnterMoveNextColumn = Attr(k, "ENTERKEYBEHAVIOR") == "1";
            // Form2.vb 1730 LoadLayout: spremljeni izgled; FILTERSAVE je atribut forme (GRL-01).
            GridRaspored.Ucitaj(view, DatotekaIzgleda(c), AtributForme("FILTERSAVE").ToLowerInvariant() == "true");
        }

        /// <summary>
        /// Form2.vb 9205-9243: "GridLodani-" + naslov projekta + "-" + ime XML-a + "-" + forma + "-" + grid + "-" + jezik.
        /// </summary>
        private string DatotekaIzgleda(Control grid)
        {
            string xml = GridRaspored.ImeXml(_kontekst == null ? "" : _kontekst.XmlName);
            string naslov = _kontekst == null ? "" : (_kontekst.ProjektNaslov ?? "");
            string forma = _forma == null ? "" : (_forma.Ime ?? "");
            return GridRaspored.Datoteka("GridLodani-" + naslov + "-" + xml + "-" + forma + "-" + (grid == null ? "" : grid.Name.Trim()) + "-");
        }

        /// <summary>DeleteGridLayout 9226-9238: obriše datoteku, složi stupce iznova i osvježi grid.</summary>
        private void ResetGrida(GridControl grid, NgControl k)
        {
            GridRaspored.Obrisi(DatotekaIzgleda(grid));
            var view = grid == null ? null : grid.MainView as GridView;
            if (view == null || k == null)
                return;
            GridIzgled.Zaboravi(view);
            OsvjeziGrid(grid, k, "");
        }
    }
}
