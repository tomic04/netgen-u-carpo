using System;
using System.Collections.Generic;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Xml
{
    /// <summary>
    /// INCLUDE (XML-02). Form1 3383: INCLUDE na PROJECT je cls1.strIncludeGlobal. Form2.ProcitajXML 840-934:
    /// bez globalnog vrijedi INCLUDE forme koja se otvara; svaka datoteka (odvojene s #, iz mape glavnog XML-a)
    /// doda svu djecu PROJECT-a na kraj dokumenta, pa se forma traži redom: glavni XML, zatim uključeni.
    /// Uključeni XML-ovi se ne slijede dalje (VB uvozi samo djecu). U korpusu 46 INCLUDE-a: 34 granule BS*
    /// dijele ~190 formi iz BSUgovori_INC.xml, MP* iz MPKarticaRobe.xml i MPAmbalaza.xml.
    /// Carpo uzima uniju INCLUDE-a projekta i formi (VB bi uzeo formin tek kad se ta forma otvori),
    /// pa forma iz uključenog XML-a postoji od prvog klika.
    /// </summary>
    public static class IncludeForme
    {
        private const int SekundiPamcenja = 60;

        private sealed class Zapis
        {
            public DateTime Kad;
            public NgProject Projekt;
        }

        private static readonly Dictionary<string, Zapis> Ucitani = new Dictionary<string, Zapis>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Forma po imenu (bez razlike velikih slova, kao ostatak Carpa): glavni XML, pa INCLUDE.</summary>
        public static NgForm Nadji(NgProject projekt, string ime)
        {
            string n = (ime ?? "").Trim();
            if (projekt == null || projekt.Forme == null || n.Length == 0)
                return null;
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                if (string.Equals(projekt.Forme[i].Ime, n, StringComparison.OrdinalIgnoreCase))
                    return projekt.Forme[i];
            }
            foreach (string datoteka in Datoteke(projekt))
            {
                NgProject ukljuceni = Ucitaj(IncludeXml.Putanja(projekt.Putanja, datoteka));
                if (ukljuceni == null)
                    continue;
                for (int i = 0; i < ukljuceni.Forme.Count; i++)
                {
                    if (string.Equals(ukljuceni.Forme[i].Ime, n, StringComparison.OrdinalIgnoreCase))
                        return ukljuceni.Forme[i];
                }
            }
            return null;
        }

        /// <summary>INCLUDE projekta pa INCLUDE formi, redom, bez ponavljanja.</summary>
        private static List<string> Datoteke(NgProject projekt)
        {
            var lista = new List<string>();
            var vidjeno = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Dodaj(lista, vidjeno, projekt.Include);
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                NgAttributeBag a = projekt.Forme[i].Atributi;
                if (a != null && a.Ima("INCLUDE"))
                    Dodaj(lista, vidjeno, a.Vrijednost("INCLUDE"));
            }
            return lista;
        }

        private static void Dodaj(List<string> lista, HashSet<string> vidjeno, string vrijednost)
        {
            string[] komadi = (vrijednost ?? "").Split('#');
            for (int k = 0; k < komadi.Length; k++)
            {
                string d = (komadi[k] ?? "").Trim();
                if (d.Length > 0 && vidjeno.Add(d))
                    lista.Add(d);
            }
        }

        private static NgProject Ucitaj(string put)
        {
            if (string.IsNullOrWhiteSpace(put))
                return null;
            lock (Ucitani)
            {
                Zapis z;
                if (Ucitani.TryGetValue(put, out z) && (DateTime.UtcNow - z.Kad).TotalSeconds < SekundiPamcenja)
                    return z.Projekt;
            }
            NgLoadResult r = NetGenXmlReader.Ucitaj(put);
            NgProject p = r.Uspjeh ? r.Projekt : null;
            if (p == null)
                NgDnevnik.Greska(" m_xmldINCLUDE2.Load  , " + (r.Greska ?? put));
            else
                NgDnevnik.Akcija(" stINCL = " + put);
            lock (Ucitani)
                Ucitani[put] = new Zapis { Kad = DateTime.UtcNow, Projekt = p };
            return p;
        }
    }
}
