using System;
using System.Data;
using System.Globalization;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Upiti i URL iz Form1.Fiskalizacija 286–434 i Form2.FiskalizacijaNapojnica 7527–7609.
    /// Prazan URL se ne zamjenjuje produkcijskim cis.porezna-uprava.hr.
    /// Šalje se samo testni CIS cistest.apis-it.hr.
    /// </summary>
    public static class FiskalTok
    {
        public const string TestUrl = "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest";
        public const string PravaUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";

        public static bool JeTest(string url)
        {
            return (url ?? "").IndexOf("cistest.apis-it.hr", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>Prazan i prava porezna daju prazan URL. Testni CIS ostaje.</summary>
        public static string UrlZaSlanje(string url)
        {
            string u = (url ?? "").Trim();
            if (u.Length == 0)
                return "";
            if (Carpo.NetGen.Poziv.FiskalPoziv.JePravaPorezna(u))
                return "";
            if (!JeTest(u))
                return "";
            return u;
        }

        public static string SqlPostavke(int tablica, string id)
        {
            string i = (id ?? "").Trim();
            if (tablica == 2 || tablica == 3 || tablica == 5)
                return "select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')";
            if (tablica == 4)
                return "select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpponudagla where mpponudaglaid = " + i + " ))";
            return "select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpblagajnagla where mpblagajnaglaid = " + i + "  ))";
        }

        public static string SqlXml(int tablica, string id)
        {
            string i = (id ?? "").Trim();
            if (tablica == 2)
                return "spFaktureXMLGetRacunZahtijev @faktureid=" + i;
            if (tablica == 3)
                return "spBSracuniXMLGetRacunZahtijev @action=2,@bsracuniglaid=" + i;
            if (tablica == 5)
                return "spPonudaGetXMLZahtijev @ProPonGlaId=" + i;
            if (tablica == 4)
                return "spMPXMLGetPDZahtijev @mpponudaglaid=" + i;
            if (tablica == -1)
                return "spMPXMLGetNapojnicaZahtijev @mpblagajnaglaid=" + i;
            return "spMPXMLGetRacunZahtijev @mpblagajnaglaid=" + i;
        }

        public static string SqlJir(int tablica, string id)
        {
            string i = (id ?? "").Trim();
            if (tablica == 2)
                return "select isnull(jir,'') as jir from fakture  where faktureid=" + i;
            if (tablica == 3)
                return "select isnull(jir,'') as jir from BSRacuniGla  where BSRacuniGlaid=" + i;
            if (tablica == 5)
                return "select isnull(jir,'') as jir from ProPonGla  where ProPonGlaId=" + i;
            if (tablica == 4)
                return "select isnull(jir,'') as jir from MPPonudagla  where MPPonudaglaid=" + i;
            return "select isnull(jir,'') as jir from mpblagajnagla   where mpblagajnaglaid=" + i;
        }

        public static bool DatumPusta(DataRow red, DateTime sada)
        {
            if (red == null || red.Table == null || !red.Table.Columns.Contains("datum"))
                return true;
            object v = red["datum"];
            if (v == null || v == DBNull.Value)
                return true;
            DateTime d;
            if (v is DateTime)
                d = (DateTime)v;
            else if (!DateTime.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return true;
            return d <= sada;
        }

        /// <summary>Čita postavke i XML. SOAP ide samo na testni CIS.</summary>
        public static Carpo.NetGen.Poziv.FiskalPoziv.Ishod Izvrsi(int tablica, string id, Func<string, DataTable> citaj)
        {
            var i = new Carpo.NetGen.Poziv.FiskalPoziv.Ishod();
            if (citaj == null)
            {
                i.Greska = "Nema čitanja.";
                return i;
            }
            DataTable post = null;
            try
            {
                post = citaj(SqlPostavke(tablica, id));
            }
            catch (Exception ex)
            {
                i.Greska = ex.Message;
                return i;
            }
            if (post == null || post.Rows.Count == 0)
            {
                i.Greska = "Nema ništa u MPFISKALIZACIJA !!!";
                return i;
            }
            DataRow red = post.Rows[0];
            if (!DatumPusta(red, DateTime.Now))
            {
                i.Greska = "DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!! NEMA FISKALIZACIJE ! ";
                return i;
            }
            string url = UrlZaSlanje(Polje(red, "url"));
            i.Url = url;
            if (url.Length == 0)
            {
                i.Greska = "URL nije testni CIS. Zahtjev nije poslan.";
                return i;
            }
            string pfx = Polje(red, "pfx");
            string lozinka = Polje(red, "lozinkazapfx");
            if (pfx.Length == 0)
            {
                i.Greska = "Nije definiran PFX !";
                return i;
            }
            if (lozinka.Length == 0)
            {
                i.Greska = "Nije definirana lozinka za  PFX !";
                return i;
            }
            DataTable jir = null;
            try
            {
                jir = citaj(SqlJir(tablica, id));
            }
            catch (Exception ex)
            {
                i.Greska = ex.Message;
                return i;
            }
            if (jir != null && jir.Rows.Count > 0 && Polje(jir.Rows[0], "jir").Length > 0)
            {
                i.Greska = "RAČUN VEĆ IMA JIR, fiskalizacija prekinuta !! ID=" + (id ?? "").Trim();
                return i;
            }
            DataTable xml = null;
            try
            {
                xml = citaj(SqlXml(tablica, id));
            }
            catch (Exception ex)
            {
                i.Greska = ex.Message;
                return i;
            }
            string tijelo = "";
            if (xml != null && xml.Rows.Count > 0 && xml.Columns.Count > 0 && xml.Rows[0][0] != DBNull.Value)
                tijelo = Convert.ToString(xml.Rows[0][0]);
            if (tablica == -1 && tijelo.Length == 0)
            {
                i.Greska = "Nema XML napojnice.";
                return i;
            }
            return Carpo.NetGen.Poziv.FiskalPoziv.Posalji(
                tijelo,
                pfx,
                lozinka,
                url,
                Polje(red, "timeout"),
                Polje(red, "PutanjaZaZahtjeveiOdgovoreOdPorezne"),
                Polje(red, "PutanjaZaZahtjeveiOdgovoreOdPorezne"));
        }

        private static string Polje(DataRow red, string ime)
        {
            if (red == null || red.Table == null || !red.Table.Columns.Contains(ime) || red[ime] == DBNull.Value)
                return "";
            return Convert.ToString(red[ime]) ?? "";
        }
    }
}
