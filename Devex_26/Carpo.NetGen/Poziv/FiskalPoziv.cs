using System;
using System.Reflection;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Poziv
{
    /// <summary>
    /// Fiskalizacija ide kroz vanjski DLL, isto kao Form1.Fiskalizacija 286–611.
    /// Carpo ne računa ZKI, ne slaže SOAP i ne zove poreznu. Samo učita isti sklop.
    /// Slanje je PosaljiSoapPorukuCertifikatDatoteka i ovdje se ne zove.
    /// </summary>
    public static class FiskalPoziv
    {
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
