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
            string sljedeca = DijalogTok.NextForm(forma);
            s["nextForm"] = sljedeca;
            s["dodajPoruka"] = "";
            s["otvaraFormu"] = ishod.Otvara ?? "";
            s["saljeSubmit"] = ishod.Otvara.Length > 0 ? "false" : "nije-ova-grana";
            return s;
        }

        /// <summary>
        /// Prihvati je Button1: OKClick, pa ONSUBMIT, pa zadnji master (Form1.vb 5519).
        /// Odustani je Button2: nema SQL-a i nema refresha. Escape bez ONESCKEY samo zatvara.
        /// </summary>
        public static Dictionary<string, string> PoNatpisu(NgProject projekt, string natpis, bool potvrda)
        {
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            NgForm forma = null;
            NgButton gumb = null;
            if (projekt != null && projekt.Forme != null && !string.IsNullOrEmpty(natpis))
            {
                for (int i = 0; i < projekt.Forme.Count && gumb == null; i++)
                {
                    NgForm f = projekt.Forme[i];
                    if (f.Gumbi == null)
                        continue;
                    for (int g = 0; g < f.Gumbi.Count; g++)
                    {
                        if (string.Equals(f.Gumbi[g].Natpis, natpis, StringComparison.OrdinalIgnoreCase))
                        {
                            forma = f;
                            gumb = f.Gumbi[g];
                            break;
                        }
                    }
                }
            }
            if (!potvrda)
            {
                s["refreshMaster"] = "false";
                s["okClick"] = "false";
                s["sqlImaZalijep"] = "false";
                s["sqlApostrof"] = "";
                return gumb == null && !string.IsNullOrEmpty(natpis) ? null : s;
            }
            if (gumb == null)
                return null;
            string submit = forma == null ? "" : (forma.OnSubmit ?? "");
            bool lijepi = submit.Trim().Length > 0 && !Lanac.Prvih6JeDml(submit) && submit.IndexOf("!!", StringComparison.Ordinal) < 0;
            s["refreshMaster"] = "true";
            s["okClick"] = "true";
            s["sqlImaZalijep"] = lijepi ? "true" : "false";
            s["sqlApostrof"] = "''";
            s["nextForm"] = DijalogTok.NextForm(forma);
            return s;
        }

        public static Dictionary<string, string> Escape(NgProject projekt)
        {
            var s = new Dictionary<string, string>(StringComparer.Ordinal);
            bool ima = false;
            if (projekt != null && projekt.Forme != null)
            {
                for (int i = 0; i < projekt.Forme.Count && !ima; i++)
                {
                    NgForm f = projekt.Forme[i];
                    if (f.Atributi != null && f.Atributi.Ima("ONESCKEY")
                        && (f.Atributi.Vrijednost("ONESCKEY") ?? "").Trim().Length > 0)
                        ima = true;
                }
            }
            s["refreshMaster"] = "false";
            s["okClick"] = ima ? "true" : "false";
            s["sqlImaZalijep"] = "false";
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
