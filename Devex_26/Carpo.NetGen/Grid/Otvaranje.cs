using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// Samo put otvaranja: ONLOAD, master, stablo, detail, combo i SQL filtera.
    /// Nema ONSUBMIT, nema ONCLICK gumba, nema ispisa.
    /// </summary>
    public static class Otvaranje
    {
        public static List<string> Upiti(NgProject projekt, SqlKontekst kontekst, string imeXml, out string master)
        {
            var lista = new List<string>();
            master = "";
            if (projekt == null)
                return lista;
            if (kontekst == null)
                kontekst = new SqlKontekst();
            var grid = new MasterGrid();
            grid.Kontekst = kontekst;
            grid.Ucitaj(projekt, null);
            if (grid.Stanje.Odluka == "salji" && !grid.Stanje.BlokiranoPisanje)
            {
                Dodaj(lista, grid.Stanje.SqlZaSlanje, kontekst);
                if (lista.Count > 0)
                    master = lista[0];
            }
            if (grid.Stanje.OtvaraFormu)
                Forma(projekt, grid.Stanje.ImeForme, kontekst, lista);
            Stablo(projekt, kontekst, lista);
            string combo = Atr(projekt, "COMBO");
            Dodaj(lista, combo, kontekst);
            if (!string.IsNullOrEmpty(imeXml))
                Dodaj(lista, "select * from NetGenDBFilter where granula='" + imeXml.Replace("'", "''") + "'", kontekst);
            return lista;
        }

        public static string Detail(NgProject projekt, SqlKontekst kontekst)
        {
            if (projekt == null)
                return "";
            var d = new DetailGrid();
            d.Kontekst = kontekst ?? new SqlKontekst();
            d.Pripremi(projekt);
            if (d.Stanje.Nema || d.Stanje.OtvaraFormu)
                return "";
            string sql = SqlTekst.Sredi(d.Stanje.Predlozak, d.Kontekst).Replace("~~", ",");
            if (SqlTekst.Odluka(sql) != "salji")
                return "";
            return sql;
        }

        private static void Forma(NgProject projekt, string ime, SqlKontekst kontekst, List<string> lista)
        {
            if (string.IsNullOrEmpty(ime) || projekt.Forme == null)
                return;
            int hash = ime.IndexOf('#');
            if (hash >= 0)
                ime = ime.Substring(0, hash);
            ime = ime.Trim();
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1);
            NgForm forma = null;
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                if (string.Equals(projekt.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                {
                    forma = projekt.Forme[i];
                    break;
                }
            }
            if (forma == null)
                return;
            var formaKontekst = SqlTekst.ZaFormu(kontekst);
            Dodaj(lista, forma.OnLoad, formaKontekst);
            if (forma.Kontrole == null)
                return;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k.Grana == NgGrana.Button || k.Grana == NgGrana.Help)
                    continue;
                if (k.Atributi == null || !k.Atributi.Ima("SQL"))
                    continue;
                Dodaj(lista, k.Atributi.Vrijednost("SQL"), formaKontekst);
            }
        }

        private static void Stablo(NgProject projekt, SqlKontekst kontekst, List<string> lista)
        {
            string tree = projekt.Tree ?? "";
            string[] dijelovi = tree.Split('#');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string seg = dijelovi[i] ?? "";
                int kolon = seg.IndexOf(':');
                if (kolon < 0)
                    continue;
                string lijevo = seg.Substring(0, kolon).Trim();
                if (lijevo.ToLowerInvariant().Contains("tree"))
                    Dodaj(lista, lijevo, kontekst);
            }
        }

        private static void Dodaj(List<string> lista, string predlozak, SqlKontekst kontekst)
        {
            if (string.IsNullOrWhiteSpace(predlozak))
                return;
            string t = predlozak.Trim();
            if (t.StartsWith(":", StringComparison.Ordinal))
                return;
            if (t.IndexOf('#') >= 0 && t.IndexOf("select", StringComparison.OrdinalIgnoreCase) < 0
                && t.IndexOf("exec", StringComparison.OrdinalIgnoreCase) < 0)
                return;
            string sql = SqlTekst.Sredi(t, kontekst).Replace("~~", ",");
            if (SqlTekst.Odluka(sql) != "salji")
                return;
            lista.Add(sql);
        }

        private static string Atr(NgProject projekt, string ime)
        {
            if (projekt.Atributi == null || !projekt.Atributi.Ima(ime))
                return "";
            return projekt.Atributi.Vrijednost(ime) ?? "";
        }
    }
}
