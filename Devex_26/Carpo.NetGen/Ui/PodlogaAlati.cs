using System;
using System.Windows.Forms;
using Carpo.NetGen.Log;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Traka alata Carpo Managera iznad master grida (bar2: skupi/raširi grupe, reset, Postavke → sume, izvoz).
    /// Podloga nema Carpo definiciju, pa je <c>gcGla.ComponentExt</c> null i Carpo rukovatelji bi bacili
    /// NullReferenceException (PN-06). Manager.AlatBezDefinicije tada preda radnju ovamo, a Podloga je napravi
    /// na svom gridu. NetGen te alate nema u traci (Janus ih ima u desnom kliku i izborniku), pa je ovo Carpo UX.
    /// </summary>
    public sealed partial class Podloga
    {
        private bool _grupeSkupljene;

        protected override void AlatBezDefinicije(string alat, string argument)
        {
            try
            {
                switch (alat ?? "")
                {
                    case "grupe":
                        if (_grupeSkupljene)
                            gvGla.ExpandAllGroups();
                        else
                            gvGla.CollapseAllGroups();
                        _grupeSkupljene = !_grupeSkupljene;
                        break;
                    case "reset":
                        // Isto kao "Reset grida" u desnom kliku (GRL-01): obriši spremljeni izgled i složi stupce iznova.
                        GridRaspored.Obrisi(DatotekaIzgleda(false));
                        OsvjeziMaster();
                        break;
                    case "suma":
                        gvGla.OptionsView.ShowFooter = !gvGla.OptionsView.ShowFooter;
                        break;
                    case "suma-u-redu":
                        gvGla.OptionsBehavior.AlignGroupSummaryInGroupRow =
                            gvGla.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True ? DefaultBoolean.False : DefaultBoolean.True;
                        break;
                    case "suma-ispod":
                        gvGla.OptionsView.GroupFooterShowMode =
                            gvGla.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways ? GroupFooterShowMode.Hidden : GroupFooterShowMode.VisibleAlways;
                        break;
                    case "izvoz":
                        IzveziMaster(argument ?? "");
                        break;
                }
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("alat " + alat + ": " + ex.Message);
                PostaviStatus(ex.Message);
            }
        }

        /// <summary>Izvoz master grida u datoteku, kao ComponentGrid.GridExport bez izbora označenih redova.</summary>
        private void IzveziMaster(string nastavak)
        {
            string ext = nastavak.Trim().ToLowerInvariant();
            if (ext.Length == 0 || NgPoruka.BezKorisnika())
                return;
            string put;
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "(*." + ext + ")|*." + ext;
                dlg.FileName = (Text ?? "grid").Replace(":", "") + "." + ext;
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                put = dlg.FileName;
            }
            switch (ext)
            {
                case "xlsx": gcGla.ExportToXlsx(put); break;
                case "xls": gcGla.ExportToXls(put); break;
                case "pdf": gcGla.ExportToPdf(put); break;
                case "html": gcGla.ExportToHtml(put); break;
                case "rtf": gcGla.ExportToRtf(put); break;
                case "csv": gcGla.ExportToCsv(put); break;
                default: return;
            }
            NgDnevnik.Akcija("izvoz grida " + put);
        }
    }
}
