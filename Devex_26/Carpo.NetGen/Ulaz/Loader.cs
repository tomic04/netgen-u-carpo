using System;
using System.Runtime.InteropServices;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Ulaz;

namespace Carpo.NetGen
{
    /// <summary>
    /// Jupiter ssMeni: asm:Carpo.NetGen.Loader:&lt;isti xml&gt;, ukljucujuci podfolder.
    /// Ista polja i Run8 kao NetGenDLL.Loader. Run8 vraca null, puni WCaption i (M1) otvara Podlogu u
    /// Jupiterovom panelu (clientHandle); hWnd je njen prozor. AppDomain ostaje ugasen (22.04.2024).
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

        /// <summary>
        /// TestShell sam otvara prozor (i klik-pregled), pa Run8 samo učita model. Jupiter ovo ne postavlja.
        /// </summary>
        public bool BezProzora;

        /// <summary>Podloga koju je Run8 otvorio u Jupiterovom panelu (M1).</summary>
        public System.Windows.Forms.Form Forma { get; private set; }

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

        /// <summary>Stari Run je vracao formu; ovdje vraca null kao Run8. Zivi ulaz je Run8.</summary>
        public object Run(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            Carpo.NetGen.Grid.SpinVeza.Prijava(userId, password);
            return Izvrsi(server, database, remoteDllPath, operater, clientHandle);
        }

        public object Run8(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            // strNGParams ide u isti router kao klik gumba, kad se Podloga pokaze (Form1_Load 1682, ULZ-03).
            // Form1 1142: s korisnikom iz Jupitera SQL prijava, bez njega Windows (ULZ-07).
            Carpo.NetGen.Grid.SpinVeza.Prijava(userId, password);
            return Izvrsi(server, database, remoteDllPath, operater, clientHandle);
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

        private object Izvrsi(string server, string database, string remoteDllPath, int operater, int clientHandle)
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
            // M5 (XML-01): Jupiter traži XML u DMS-u i bazi JupiterXML prije diska; test ide s diska.
            if (string.IsNullOrWhiteSpace(TestXmlPath))
                Carpo.NetGen.Xml.XmlIzvor.Ukljuci(server);
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
            if (!BezProzora && Projekt != null && o.Greska == null)
                OtvoriPodlogu(o.Putanja, server, database, operater, clientHandle);
            return null;
        }

        /// <summary>
        /// M1 (docs/analiza/08): Run8 otvara istu Podlogu kao TestShell. Form1.Run (Form1.vb Run): SetParent na
        /// clientHandle pa Show; hWnd je prozor granule. Kao Carpo Starter.RegisterForm: Logger u
        /// DependencyContainer prije forme, InitLogger, MainHandle/ParentHandle, pa Base.Start() (SetParent + Show).
        /// </summary>
        private void OtvoriPodlogu(string putanja, string server, string database, int operater, int clientHandle)
        {
            Carpo.Core.Logger dnevnik = null;
            try
            {
                dnevnik = new Carpo.Core.Logger(Carpo.Core.Log.LogLevel.Debug);
                dnevnik.GranuleName = System.IO.Path.GetFileNameWithoutExtension(putanja ?? "");
                dnevnik.ServerName = server;
                dnevnik.DatabaseName = database;
                Carpo.Core.Log.DependencyContainer.Add(dnevnik);
                Carpo.NetGen.Log.NgDnevnik.Spoji(dnevnik);
            }
            catch (Exception ex)
            {
                Carpo.NetGen.Log.NgDnevnik.Greska("Logger: " + ex.Message);
            }
            string greska;
            System.Windows.Forms.Form forma = Carpo.NetGen.Ui.Prozor.Otvori(putanja, server, database, operater, out greska);
            if (!string.IsNullOrEmpty(greska))
                ZadnjaGreska = greska;
            if (forma == null)
                return;
            Forma = forma;
            var podloga = forma as Carpo.NetGen.Ui.Podloga;
            if (podloga != null)
                podloga.NgParametri = strNGParams;
            var baza = forma as Carpo.UI.Forms.Jupiter.Base;
            try
            {
                // Form1.Run 1190-1205: Koler "Jup10" je bez ruba i veličine Jupiterovog panela (ULZ-06).
                bool jup10 = Carpo.NetGen.Ulaz.JupiterProzor.JeJup10(Koler, clientHandle);
                if (jup10)
                    Carpo.NetGen.Ulaz.JupiterProzor.BezRuba(forma);
                if (baza != null)
                {
                    baza.InitLogger(dnevnik);
                    baza.ModuleId = moduleId;
                    baza.MainHandle = mainHandle;
                    baza.ParentHandle = clientHandle;
                    baza.Start();
                }
                else
                    forma.Show();
                if (jup10)
                    Carpo.NetGen.Ulaz.JupiterProzor.PratiRoditelja(forma, clientHandle, true);
                hWnd = (int)forma.Handle;
            }
            catch (Exception ex)
            {
                ZadnjaGreska = ex.Message;
                Carpo.NetGen.Log.NgDnevnik.Greska("Run8: " + ex.Message);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
    }
}
