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
    /// stavke "Natpis:naredba#..." pa stalne stavke (Export grid, Grupiraj / razgrupiraj, Ispis).
    /// DevExpress izbornik retka (PopupMenuShowing) umjesto Janus ContextMenuStrip (POP-01).
    /// </summary>
    public static class GridIzbornik
    {
        public static void Vezi(GridView pogled, Func<string> popup, Action<string> naredba)
        {
            if (pogled == null)
                return;
            bool grupe = false;
            pogled.PopupMenuShowing += delegate (object s, PopupMenuShowingEventArgs e)
            {
                if (e.MenuType != GridMenuType.Row && e.MenuType != GridMenuType.User)
                    return;
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
                    e.Menu.Items.Add(stavka);
                }
                var export = new DXMenuItem("Export grid", delegate { Izvezi(pogled.GridControl); });
                export.BeginGroup = stavke.Count > 0;
                e.Menu.Items.Add(export);
                e.Menu.Items.Add(new DXMenuItem("Grupiraj / razgrupiraj", delegate
                {
                    // ToolStripMenuItem1_Click "groupungropu": izmjenicno ExpandGroups / CollapseGroups.
                    if (grupe)
                        pogled.CollapseAllGroups();
                    else
                        pogled.ExpandAllGroups();
                    grupe = !grupe;
                }));
                e.Menu.Items.Add(new DXMenuItem("Ispis", delegate
                {
                    var g = pogled.GridControl;
                    if (g != null && g.IsPrintingAvailable)
                        g.ShowPrintPreview();
                }));
                e.Allow = true;
            };
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
