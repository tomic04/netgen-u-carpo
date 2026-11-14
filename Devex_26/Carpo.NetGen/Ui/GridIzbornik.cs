using System;
using System.IO;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Desni klik na grid (Form2.Glbl_GridMouseUp 9500+, Form1 POPUPMASTER 3560 / POPUPDETAIL 3700):
    /// stavke "Natpis:naredba#..." pa stalne stavke (Spremi postavke grida, Reset grida, Export grid, Grupiraj / razgrupiraj, Ispis).
    /// DevExpress izbornik retka (PopupMenuShowing) umjesto Janus ContextMenuStrip (POP-01).
    /// </summary>
    public static class GridIzbornik
    {
        /// <summary>
        /// dozvola(natpis, stalna): Form1 3577-3610 za stavke iz POPUP-a i ssPermissionProvjeriRafalnuKomandu
        /// (3627-3672) za stalne stavke. Null ne mijenja ništa (DOZ-01).
        /// radnje: GridRadnje (Ispis s PAGEHEADER*, Spremi/Reset izgleda). Carpo ima jednu stavku ispisa.
        /// </summary>
        public static void Vezi(GridView pogled, Func<string> popup, Action<string> naredba, Func<string, bool, Dozvole.Stanje> dozvola, GridRadnje radnje)
        {
            if (radnje == null)
                radnje = new GridRadnje();
            if (pogled == null)
                return;
            bool grupe = false;
            pogled.PopupMenuShowing += delegate (object s, PopupMenuShowingEventArgs e)
            {
                if (e.MenuType != GridMenuType.Row && e.MenuType != GridMenuType.User)
                    return;
                // Form1 6984-6998 i Form2.Glbl_GridMouseUp 8704-8716: Ctrl + desni klik je izbor stupaca
                // (kvačica je vidljivost stupca). Carpo za to ima svoj izbornik stupaca (ComponentGrid
                // SetAttributeColumnChooser), pa se otvori on umjesto popisa.
                if (Form2Dogadjaj.DesniKlik(true, (Control.ModifierKeys & Keys.Control) == Keys.Control) == "stupci")
                {
                    e.Allow = false;
                    pogled.OptionsCustomization.CustomizationFormSearchBoxVisible = true;
                    pogled.ShowCustomization();
                    return;
                }
                if (e.Menu == null)
                    e.Menu = new GridViewMenu(pogled);
                var stavke = Form2Dogadjaj.Izbornik(popup == null ? "" : popup());
                for (int i = 0; i < stavke.Count; i++)
                {
                    string[] dio = stavke[i].Split(new[] { '|' }, 2);
                    string cilj = dio.Length > 1 ? dio[1] : "";
                    var stavka = new DXMenuItem(dio[0], delegate
                    {
                        NgDnevnik.Akcija("izbornik " + cilj);
                        if (naredba != null)
                            naredba(cilj);
                    });
                    Primijeni(stavka, dozvola, dio[0], false);
                    e.Menu.Items.Add(stavka);
                }
                bool prva = true;
                if (radnje.Spremi != null)
                {
                    var spremi = new DXMenuItem("Spremi postavke grida", delegate { radnje.Spremi(); });
                    spremi.BeginGroup = stavke.Count > 0;
                    Primijeni(spremi, dozvola, "Spremi postavke grida", true);
                    e.Menu.Items.Add(spremi);
                    prva = false;
                }
                if (radnje.Reset != null)
                {
                    var reset = new DXMenuItem("Reset grida", delegate { radnje.Reset(); });
                    Primijeni(reset, dozvola, "Reset grida", true);
                    e.Menu.Items.Add(reset);
                }
                var export = new DXMenuItem("Export grid", delegate { Izvezi(pogled.GridControl); });
                export.BeginGroup = prva && stavke.Count > 0;
                Primijeni(export, dozvola, "Export grid", true);
                e.Menu.Items.Add(export);
                var grupiraj = new DXMenuItem("Grupiraj / razgrupiraj", delegate
                {
                    // ToolStripMenuItem1_Click "groupungropu": izmjenicno ExpandGroups / CollapseGroups.
                    if (grupe)
                        pogled.CollapseAllGroups();
                    else
                        pogled.ExpandAllGroups();
                    grupe = !grupe;
                });
                Primijeni(grupiraj, dozvola, "Grupiraj / razgrupiraj", true);
                e.Menu.Items.Add(grupiraj);
                var stavkaIspis = new DXMenuItem("Ispis", delegate
                {
                    if (radnje.Ispis != null)
                    {
                        radnje.Ispis();
                        return;
                    }
                    var g = pogled.GridControl;
                    if (g != null && g.IsPrintingAvailable)
                        g.ShowPrintPreview();
                });
                Primijeni(stavkaIspis, dozvola, "Ispis", true);
                e.Menu.Items.Add(stavkaIspis);
                e.Allow = true;
            };
        }

        private static void Primijeni(DXMenuItem stavka, Func<string, bool, Dozvole.Stanje> dozvola, string natpis, bool stalna)
        {
            if (dozvola == null || stavka == null)
                return;
            Dozvole.Stanje st = dozvola(natpis, stalna);
            if (st == null)
                return;
            if (st.Omogucen.HasValue)
                stavka.Enabled = st.Omogucen.Value;
            if (st.Vidljiv.HasValue)
                stavka.Visible = st.Vidljiv.Value;
        }

        /// <summary>"exportgrid": spremi pa otvori (NetGen .xls; DevExpress .xlsx).</summary>
        private static void Izvezi(GridControl grid)
        {
            if (grid == null)
                return;
            using (var dlg = new SaveFileDialog())
            {
                dlg.FileName = "exportdetail.xlsx";
                dlg.Filter = "Excel (*.xlsx)|*.xlsx";
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    if (File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);
                    grid.ExportToXlsx(dlg.FileName);
                    System.Diagnostics.Process.Start(dlg.FileName);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("export grida: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// ToolStripMenuItem1_Click Case Else: naredba s '~' je lanac ('~' postaje '#'), inace forma.
        /// Na podlozi (Form1 5743-5760) lanac dobije ':' sprijeda.
        /// </summary>
        public static string Lanac(string naredba, bool podloga)
        {
            string n = naredba ?? "";
            if (n.IndexOf('~') < 0)
                return "";
            n = n.Replace("~", "#");
            return podloga ? ":" + n : n;
        }
    }
}
