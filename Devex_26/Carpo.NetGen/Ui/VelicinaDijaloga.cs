using System;
using Microsoft.Win32;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// VEL-01. Form2 pamti veličinu dijaloga kroz VB SaveSetting/GetSetting (HKCU\Software\VB and VBA Program
    /// Settings\Jupiter, odjeljci FormHeight i FormWidth), ključ je put XML-a, ime forme, VERSION projekta i
    /// VERSION forme odvojeni s "-" (Form2.vb 2785-2790 i 4108-4109). Isti ključ znači da Carpo vrati veličinu
    /// koju je korisnik namjestio još u NetGenu.
    /// </summary>
    public static class VelicinaDijaloga
    {
        /// <summary>Gdje VB SaveSetting piše za aplikaciju "Jupiter". Test ga preusmjeri.</summary>
        public static string Korijen = @"Software\VB and VBA Program Settings\Jupiter";

        public static string Kljuc(string xmlPut, string forma, string verzijaProjekta, string verzijaForme)
        {
            return (xmlPut ?? "") + "-" + (forma ?? "") + "-" + (verzijaProjekta ?? "") + "-" + (verzijaForme ?? "");
        }

        /// <summary>GetSetting za oba odjeljka. Nema vrijednosti ili nije broj: false (VB greška pod On Error).</summary>
        public static bool Procitaj(string kljuc, out int visina, out int sirina)
        {
            visina = Broj("FormHeight", kljuc);
            sirina = Broj("FormWidth", kljuc);
            return visina > 0 && sirina > 0;
        }

        public static void Spremi(string kljuc, int visina, int sirina)
        {
            if (string.IsNullOrEmpty(kljuc) || visina <= 0 || sirina <= 0)
                return;
            try
            {
                using (RegistryKey h = Registry.CurrentUser.CreateSubKey(Korijen + @"\FormHeight"))
                    h.SetValue(kljuc, visina.ToString(System.Globalization.CultureInfo.InvariantCulture));
                using (RegistryKey w = Registry.CurrentUser.CreateSubKey(Korijen + @"\FormWidth"))
                    w.SetValue(kljuc, sirina.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                if (!(ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is System.IO.IOException))
                    throw;
                NgDnevnik.Greska("SaveSetting FormHeight/FormWidth: " + ex.Message);
            }
        }

        private static int Broj(string odjeljak, string kljuc)
        {
            try
            {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(Korijen + @"\" + odjeljak))
                {
                    object v = k == null ? null : k.GetValue(kljuc);
                    int n;
                    if (v != null && int.TryParse(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture), out n))
                        return n;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is System.IO.IOException))
                    throw;
                NgDnevnik.Greska("GetSetting " + odjeljak + ": " + ex.Message);
            }
            return 0;
        }
    }
}
