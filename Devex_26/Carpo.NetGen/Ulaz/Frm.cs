using Carpo.NetGen.Model;

namespace Carpo.NetGen
{
    /// <summary>
    /// Drugi NetGen ulaz. U spin.dbo.ssMeni ih je 9: asm:NetGenDLL.frm:&lt;xml&gt;.
    /// Ista polja kao netgen/frm.vb. Zivi je Run8 (New Form2). Stari Run u VB-u
    /// ne radi New, ali potpis postoji pa ga host i dalje moze pozvati.
    /// Nema strNGParams ni ShellResize. Ima ulazniID i ulaznaForma (Form2.vb 489, 503).
    /// M0 ne otvara dijalog.
    /// Ime klase je frm, malim slovom, kao u VB-u i u Parametar retku.
    /// </summary>
    public class frm
    {
        public string moduleId;
        public int mainHandle;
        public string WCaption;
        public int hWnd;
        public string Ikona;
        public string Koler = "";
        public string ulazniID = "";
        public string ulaznaForma = "";
        public string TestXmlPath;
        public string ZadnjaGreska;

        public NgProject Projekt { get; private set; }

        public object Run(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            return Izvrsi(remoteDllPath);
        }

        public object Run8(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            // ulazniID i ulaznaForma se primjenjuju na dijalog u kasnijoj fazi, ne u M0.
            return Izvrsi(remoteDllPath);
        }

        private object Izvrsi(string remoteDllPath)
        {
            var o = NgUlaz.Pokreni(TestXmlPath, moduleId, remoteDllPath);
            if (o.Ucitano && o.Greska == null)
                WCaption = o.WCaption;
            ZadnjaGreska = o.Greska;
            Projekt = o.Projekt;
            return null;
        }
    }
}
