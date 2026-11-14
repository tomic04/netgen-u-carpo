using System;
using System.IO;
using Carpo.NetGen.Log;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Desni klik na grid → stalne stavke uz POPUP (Form1 3627-3672, Form2 8776-8820) i spremljeni
    /// izgled grida (GRL-01). Ispis: "Ispis" i "Ispis REPX" (ispisnovi/ispisdx) s PAGEHEADER*;
    /// Spremi/Reset: "Spremi postavke grida" i "Reset grida" (SaveGridLayout/DeleteGridLayout).
    /// Null radnja: Ispis je pregled grida, a Spremi/Reset se ne prikazuju.
    /// </summary>
    public sealed class GridRadnje
    {
        public Action Ispis;
        public Action Spremi;
        public Action Reset;
    }

    /// <summary>
    /// Izgled grida u %APPDATA%\SPIN Informatica\Jupiter Software\Grid Settings (GetLayoutDirectory, Form1 2509,
    /// Form2 9245). Imena kao NetGen: GridMaster/GridDet + ime XML-a (+ forma uz MULTIGRIDLAYOUT) + "-" + jezik, a
    /// grid dijaloga "GridLodani-" + naslov projekta + XML + forma + grid + jezik. NetGen za DevExpress grid
    /// (DXGRID) piše isto ime s .xml (SaveLayoutToXml/RestoreLayoutFromXml, Form1 6206-6220); Janus .gxl Carpo
    /// ne čita. Bez FILTERSAVE "true" učitani filter se briše (RemoveFilters).
    /// </summary>
    public static class GridRaspored
    {
        /// <summary>Za testove: druga mapa umjesto AppData.</summary>
        public static string MapaZaTest;

        public static string Mapa()
        {
            string m = MapaZaTest;
            if (string.IsNullOrEmpty(m))
                m = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SPIN Informatica", "Jupiter Software", "Grid Settings");
            try
            {
                if (!Directory.Exists(m))
                    Directory.CreateDirectory(m);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Grid Settings: " + ex.Message);
            }
            return m;
        }

        public static string ImeXml(string putanja)
        {
            string ime = Path.GetFileName(putanja ?? "") ?? "";
            if (ime.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                ime = ime.Substring(0, ime.Length - 4);
            return ime.Trim();
        }

        public static string Datoteka(string ime)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                ime = (ime ?? "").Replace(c, '_');
            return Path.Combine(Mapa(), ime + ".xml");
        }

        public static void Spremi(GridView pogled, string datoteka)
        {
            if (pogled == null || string.IsNullOrEmpty(datoteka))
                return;
            try
            {
                pogled.SaveLayoutToXml(datoteka);
                NgDnevnik.Akcija("SAVEGRIDLAYOUT : " + datoteka);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("SAVEGRIDLAYOUT " + datoteka + ": " + ex.Message);
            }
        }

        public static bool Ucitaj(GridView pogled, string datoteka, bool filterSave)
        {
            if (pogled == null || string.IsNullOrEmpty(datoteka) || !File.Exists(datoteka))
                return false;
            try
            {
                pogled.RestoreLayoutFromXml(datoteka);
                if (!filterSave)
                    pogled.ActiveFilterString = "";
                NgDnevnik.Akcija("LOADGRIDLAYOUT : " + datoteka);
                return true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("LOADGRIDLAYOUT " + datoteka + ": " + ex.Message);
                return false;
            }
        }

        public static void Obrisi(string datoteka)
        {
            try
            {
                if (!string.IsNullOrEmpty(datoteka) && File.Exists(datoteka))
                    File.Delete(datoteka);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("DELETEGRIDLAYOUT " + datoteka + ": " + ex.Message);
            }
        }
    }
}
