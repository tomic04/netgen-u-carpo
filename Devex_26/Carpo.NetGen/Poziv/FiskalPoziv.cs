using System;
using System.Reflection;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Poziv
{
    /// <summary>
    /// Fiskalizacija ide kroz Raverus, Form1.vb 493–548.
    /// PosaljiSoapPorukuCertifikatDatoteka(xml, pfx, lozinka) vraća SOAP.
    /// Greška daje šifru 55555 i DohvatiGresku(OdgovorGreska). Inače DohvatiJir.
    /// URL s porezna-uprava.hr se ne zove.
    /// </summary>
    public static class FiskalPoziv
    {
        public static object Cis;
        public const string Sklop = "Raverus.FiskalizacijaDEV.COM";
        public const string Klasa = "Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav";
        public const string SklopSlo = "SLOFISKAL";
        public const string KlasaSlo = "SLOFISKAL.Program";

        public const string GenerirajUuid = "GenerirajUuid";
        public const string ZastitniKod = "ZastitniKodIzracunCertifikatDatoteka";
        public const string PosaljiSoap = "PosaljiSoapPorukuCertifikatDatoteka";
        public const string DohvatiJir = "DohvatiJir";
        public const string DohvatiGresku = "DohvatiGresku";
        public const string SloRacun = "SLORacun";
        public const string SloProstor = "SLOPP";

        public static bool JeSlovenija(string url)
        {
            return (url ?? "").ToLowerInvariant().IndexOf("fu.gov.si", StringComparison.Ordinal) >= 0;
        }

        public sealed class Ishod
        {
            public bool Ucitano;
            public bool Poslano;
            public string Sklop = "";
            public string Klasa = "";
            public string Greska = "";
            public string Odgovor = "";
            public string Jir = "";
            public string Sifra = "";
            public string Url = "";
        }

        public static bool JePravaPorezna(string url)
        {
            string u = (url ?? "").ToLowerInvariant();
            return u.IndexOf("porezna-uprava.hr", StringComparison.Ordinal) >= 0;
        }

        /// <summary>Form1.vb 493–548. Prazan PFX ili prava porezna ne šalju.</summary>
        public static Ishod Posalji(string xml, string pfx, string lozinka, string url, string timeout, string mapaZahtjev, string mapaOdgovor)
        {
            var i = new Ishod();
            i.Url = (url ?? "").Trim();
            if (i.Url.Length == 0)
            {
                i.Greska = "Nema CIS URL-a. NetGen bi uzeo produkcijski cis.porezna-uprava.hr.";
                return i;
            }
            if (JePravaPorezna(i.Url))
            {
                i.Greska = "URL je prava porezna. Zahtjev nije poslan.";
                NgDnevnik.Greska(i.Greska + " " + i.Url);
                return i;
            }
            if (string.IsNullOrWhiteSpace(pfx))
            {
                i.Greska = "Nije definiran PFX !";
                return i;
            }
            if (string.IsNullOrWhiteSpace(lozinka))
            {
                i.Greska = "Nije definirana lozinka za  PFX !";
                return i;
            }
            object fiskal = Cis;
            if (fiskal == null)
            {
                Ishod ucitan = Ucitaj(i.Url);
                i.Ucitano = ucitan.Ucitano;
                i.Greska = ucitan.Greska;
                if (!ucitan.Ucitano)
                    return i;
                fiskal = Cis;
            }
            if (fiskal == null)
            {
                i.Greska = "Nema Raverus.FiskalizacijaDEV.COM.dll ni testnog CIS objekta.";
                return i;
            }
            string cekanje = string.IsNullOrWhiteSpace(timeout) ? "5000" : timeout.Trim();
            string zahtjev = string.IsNullOrWhiteSpace(mapaZahtjev) ? "C:\\FISKALIZACIJA\\RACUNI\\ZAHTJEVI" : mapaZahtjev;
            string odgovorMapa = string.IsNullOrWhiteSpace(mapaOdgovor) ? "C:\\FISKALIZACIJA\\RACUNI\\ODGOVORI" : mapaOdgovor;
            Postavi(fiskal, "CisUrl", i.Url);
            Postavi(fiskal, "TimeOut", cekanje);
            Postavi(fiskal, "NazivMapeZahtjev", zahtjev);
            Postavi(fiskal, "NazivMapeOdgovor", odgovorMapa);
            Postavi(fiskal, "NazivAutoGeneriranje", true);
            try
            {
                object odg = fiskal.GetType().InvokeMember(
                    PosaljiSoap,
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    fiskal,
                    new object[] { xml ?? "", pfx, lozinka.Trim() });
                i.Odgovor = odg == null ? "" : Convert.ToString(odg);
                i.Poslano = true;
                object jir = fiskal.GetType().InvokeMember(
                    DohvatiJir,
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    fiskal,
                    new object[] { i.Odgovor });
                i.Jir = jir == null ? "" : Convert.ToString(jir);
            }
            catch (Exception ex)
            {
                i.Poslano = false;
                i.Sifra = "55555";
                string greska = Citaj(fiskal, "OdgovorGreska");
                string status = Citaj(fiskal, "OdgovorGreskaStatus");
                string opis = "";
                try
                {
                    object t = fiskal.GetType().InvokeMember(
                        DohvatiGresku,
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        fiskal,
                        new object[] { greska });
                    opis = t == null ? "" : Convert.ToString(t);
                }
                catch
                {
                    opis = "";
                }
                i.Greska = opis.Length > 0 ? opis : ex.Message;
                NgDnevnik.Greska("GREŠKA KOD FISKALIZACIJE RAČUNA : " + ex.Message + " " + status + " " + greska);
            }
            return i;
        }

        private static void Postavi(object o, string ime, object vrijednost)
        {
            try
            {
                o.GetType().InvokeMember(ime, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null, o, new object[] { vrijednost });
            }
            catch
            {
            }
        }

        private static string Citaj(object o, string ime)
        {
            try
            {
                object v = o.GetType().InvokeMember(ime, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, o, null);
                return v == null ? "" : Convert.ToString(v);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Assembly.Load i CreateInstance. Ne postavlja CisUrl i ne zove nijednu metodu.
        /// Form1.vb 341–354 (Slovenija) ili 360–368 (Raverus).
        /// </summary>
        public static Ishod Ucitaj(string url)
        {
            var i = new Ishod();
            i.Poslano = false;
            bool slo = JeSlovenija(url);
            i.Sklop = slo ? SklopSlo : Sklop;
            i.Klasa = slo ? KlasaSlo : Klasa;
            try
            {
                Assembly a = Assembly.Load(i.Sklop);
                object o = a.CreateInstance(i.Klasa);
                if (o == null)
                    throw new InvalidOperationException("CreateInstance je vratio null");
                Cis = o;
                i.Ucitano = true;
            }
            catch (Exception ex)
            {
                i.Ucitano = false;
                i.Greska = ex.Message;
                NgDnevnik.Greska("Greška kod Assembly.Load(" + i.Sklop + ") : " + ex.Message);
            }
            return i;
        }
    }
}
