using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Klik gumba na dijalogu. Form2.Rafalno (6884–6890) i prazna forma (Form1.vb 5354–5370).
    /// Procedura koja piše ne ide u sqlImaZalijep: ONCLICK je Rafalno, ne drugi niz @ime=.
    /// </summary>
    public static class DijalogKlik
    {
        public static Dictionary<string, string> Stavke(NgProject projekt, string cilj)
        {
            NgForm forma;
            NgControl kontrola;
            if (!Nadji(projekt, cilj, out forma, out kontrola))
                return null;
            string naredba = "";
            if (kontrola.Atributi != null && kontrola.Atributi.Ima("ONCLICK"))
                naredba = kontrola.Atributi.Vrijednost("ONCLICK") ?? "";
            Lanac.Ishod ishod = Lanac.Izvrsi(naredba, true, null);
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            s["ostajeOtvorena"] = ishod.Ostaje ? "true" : "false";
            s["refreshMaster"] = ishod.Zatvara && !ishod.Button2 && !ishod.Ostaje ? "true" : "false";
            s["okClick"] = string.Equals(naredba.Trim(), "ok", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
            s["fokus"] = Fokus(naredba);
            s["sqlImaZalijep"] = "false";
            s["dodajPoruka"] = "";
            return s;
        }

        public static string Fokus(string naredba)
        {
            string[] seg = Lanac.Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string t = seg[i] ?? "";
                int p = t.IndexOf(" as focus", StringComparison.OrdinalIgnoreCase);
                if (p < 0)
                    continue;
                string lijevo = t.Substring(0, p);
                int zatvoren = lijevo.LastIndexOf('\'');
                if (zatvoren <= 0)
                    continue;
                int otvoren = lijevo.LastIndexOf('\'', zatvoren - 1);
                if (otvoren < 0)
                    continue;
                return lijevo.Substring(otvoren + 1, zatvoren - otvoren - 1);
            }
            return "";
        }

        private static bool Nadji(NgProject projekt, string cilj, out NgForm forma, out NgControl kontrola)
        {
            forma = null;
            kontrola = null;
            if (projekt == null || string.IsNullOrEmpty(cilj))
                return false;
            int tocka = cilj.IndexOf('.');
            if (tocka <= 0 || tocka >= cilj.Length - 1)
                return false;
            string imeForme = cilj.Substring(0, tocka);
            string ime = cilj.Substring(tocka + 1);
            if (projekt.Forme == null)
                return false;
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                if (!string.Equals(projekt.Forme[i].Ime, imeForme, StringComparison.OrdinalIgnoreCase))
                    continue;
                forma = projekt.Forme[i];
                if (forma.Kontrole == null)
                    return false;
                for (int k = 0; k < forma.Kontrole.Count; k++)
                {
                    if (string.Equals(forma.Kontrole[k].Name, ime, StringComparison.OrdinalIgnoreCase))
                    {
                        kontrola = forma.Kontrole[k];
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
