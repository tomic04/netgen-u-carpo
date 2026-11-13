using System;
using System.Runtime.InteropServices;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Ulaz;

namespace Carpo.NetGen
{
    /// <summary>
    /// Jupiter ssMeni: asm:Carpo.NetGen.Loader:&lt;isti xml&gt;, ukljucujuci podfolder.
    /// Ista polja i Run8 kao NetGenDLL.Loader. Run8 vraca null i puni WCaption.
    /// hWnd ostaje 0 dok M1 ne otvori prozor. AppDomain ostaje ugasen (22.04.2024).
    /// Polja su polja, ne svojstva, jer ih host postavlja po imenu.
    /// </summary>
    public class Loader
    {
        public string moduleId;
        public string strNGParams;
        public int mainHandle;
        public string WCaption;
        public int hWnd;
        public string Koler = "";
        public string Ikona;

        /// <summary>TestShell. Jupiter ovo ne postavlja; on daje moduleId i remoteDllPath.</summary>
        public string TestXmlPath;

        public string ZadnjaGreska;
        public int ZadnjiResizeW;
        public int ZadnjiResizeH;
        public bool PrevoditeljUcitano;
        public string PrevoditeljPoruka = "";
        public string PrevoditeljVerzija = "";
        public string PrevoditeljTip = "";

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        public NgProject Projekt { get; private set; }
        public MasterGrid Grid { get; private set; }

        public string Sazetak
        {
            get
            {
                if (!string.IsNullOrEmpty(ZadnjaGreska))
                    return ZadnjaGreska;
                if (Projekt == null)
                    return "XML nije ucitan.";
                return "forme=" + Projekt.Forme.Count.ToString()
                    + " kontrole=" + Projekt.BrojKontrola().ToString()
                    + " obrazac=" + Projekt.Raspored.ObrazacIme
                    + " stablo=" + (Projekt.Raspored.ImaStablo ? "da" : "ne")
                    + " detail=" + (Projekt.Raspored.ImaDetail ? "da" : "ne");
            }
        }

        /// <summary>Stari Run je vracao formu. M0 nema formu, vraca null. Zivi ulaz je Run8.</summary>
        public object Run(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            return Izvrsi(server, remoteDllPath, operater);
        }

        public object Run8(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            // strNGParams ide u isti router kao klik gumba tek u M1. Ovdje se ne izvrsava.
            return Izvrsi(server, remoteDllPath, operater);
        }

        /// <summary>Loader.vb 22-26. Bez hWnd ne dira prozor (M0 jos nema formu).</summary>
        public void ShellResize(int w, int h)
        {
            ZadnjiResizeW = w;
            ZadnjiResizeH = h;
            if (hWnd == 0)
                return;
            SetWindowPos(new IntPtr(hWnd), new IntPtr(-1), 0, 0, w, h, SWP_NOACTIVATE | SWP_NOZORDER);
        }

        private object Izvrsi(string server, string remoteDllPath, int operater)
        {
            Prevoditelj.Ishod prevod = Prevoditelj.Ucitaj();
            PrevoditeljUcitano = prevod.Ucitano;
            PrevoditeljPoruka = prevod.Poruka;
            PrevoditeljVerzija = prevod.Verzija;
            PrevoditeljTip = prevod.Tip;
            if (!string.IsNullOrEmpty(server) && server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ZadnjaGreska = "jupiter2022 se ne koristi.";
                return null;
            }
            var o = NgUlaz.Pokreni(TestXmlPath, moduleId, remoteDllPath);
            if (o.Ucitano && o.Greska == null)
                WCaption = o.WCaption;
            ZadnjaGreska = o.Greska;
            Projekt = o.Projekt;
            if (Projekt != null)
            {
                Grid = new MasterGrid();
                Grid.Kontekst.OperaterId = operater.ToString();
                Grid.Kontekst.XmlName = moduleId ?? "";
                Grid.Ucitaj(Projekt, null);
            }
            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
    }
}
