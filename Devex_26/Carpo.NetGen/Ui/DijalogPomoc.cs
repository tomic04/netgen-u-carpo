using System;
using System.Data.SqlClient;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Pomoć na dijalogu (HLP-02): Form2_KeyDown F1 uz HELPKOD (4161-4166) i Glbl_ClickButton (6221-6229): gumb
    /// bez ONCLICK ili s "help" (TYPE help, 2684) zove DajPokaziHelp. strHelpKod2 je HELPKOD forme, inače
    /// HELPKOD projekta (1048-1050). Bez ijednog koda Carpo ne pokreće JupiterHelp (VB bi ga pokrenuo s praznim -h).
    /// </summary>
    public sealed partial class Dijalog
    {
        private string HelpKod()
        {
            string kod = AtributForme("HELPKOD").ToUpperInvariant();
            if (kod.Length == 0 && _kontekst != null)
                kod = (_kontekst.HelpKod ?? "").Trim();
            return kod;
        }

        private bool PokaziPomoc()
        {
            string kod = HelpKod();
            if (kod.Length == 0)
                return false;
            string server = "";
            string baza = "";
            try
            {
                var b = new SqlConnectionStringBuilder(Konekcija ?? "");
                server = b.DataSource;
                baza = b.InitialCatalog;
            }
            catch (ArgumentException)
            {
            }
            JupiterPomoc.Pokreni(server, baza, kod, Operater, _kontekst == null ? "" : _kontekst.XmlName);
            return true;
        }
    }
}
