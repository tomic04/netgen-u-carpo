using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Grid
{
    public sealed class Cvor
    {
        public string Natpis = "";
        public string Oznaka = "";
        public string Tag = "";
        public string Node = "";
        public string Naredba = "";
        public int Ikona;
        public readonly List<Cvor> Djeca = new List<Cvor>();
    }

    public sealed class StabloStanje
    {
        public bool Vidljivo;
        public int Splitter;
        public bool GumbDrvo;
        public bool ShowRootLines = true;
        public bool ShowPlusMinus = true;
        public bool AllowDrop = true;
        public string Obrub = "None";
        public string Boja = "ControlLightLight";
        public string Node = "null";
        public bool Prevedi;
        public string DragDropSql = "";
        public string ZadnjaNaredba = "";
        public readonly List<Cvor> Korijen = new List<Cvor>();
        public readonly List<string> SqlPozivi = new List<string>();
    }

    /// <summary>
    /// Lijevo stablo podloge, TreeView2. NapuniDrvoVrijeme, DazovaIzStore, DazovaVremenska.
    /// TreeView1 (izbornik XML-ova) nije ovo stablo: u dll-u je sakriven.
    /// </summary>
    public sealed class Stablo
    {
        public StabloStanje Stanje = new StabloStanje();
        public SqlKontekst Kontekst = new SqlKontekst();
        public string ImeXml = "";
        public string Jezik = "";
        public Dictionary<string, string> Rjecnik;

        public void Ucitaj(NgProject projekt, Func<string, DataTable> citaj)
        {
            Stanje = new StabloStanje();
            NgDnevnik.Akcija(" Idem puniti drvo ...");
            if (projekt == null)
                return;
            string tree = projekt.Tree ?? "";
            string combo = Atr(projekt, "COMBO") ?? "";
            Stanje.Prevedi = string.Equals((Atr(projekt, "TREETRANSLATE") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
            Stanje.DragDropSql = (Atr(projekt, "TREEDRAGDROP") ?? "").Trim();
            if (tree.Trim().Length == 0)
            {
                if (combo.Trim().Length == 0)
                {
                    Stanje.Vidljivo = false;
                    Stanje.Splitter = 0;
                    Stanje.GumbDrvo = false;
                }
                else
                {
                    Stanje.Vidljivo = true;
                    Stanje.Splitter = 243;
                    Stanje.GumbDrvo = true;
                }
                return;
            }

            Stanje.Vidljivo = true;
            Stanje.GumbDrvo = true;
            int splitter = 290;
            string raw = Atr(projekt, "SPLITTERDISTANCE");
            int parsed;
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                splitter = parsed;
            Stanje.Splitter = splitter;

            string[] dijelovi = tree.Split('#');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string seg = dijelovi[i] ?? "";
                int kolon = seg.IndexOf(':');
                if (kolon < 0)
                    continue;
                string lijevo = seg.Substring(0, kolon);
                string desno = seg.Substring(kolon + 1);
                if (string.Equals(lijevo.Trim(), "yearmonth", StringComparison.OrdinalIgnoreCase))
                    GodinaMjesec(desno.Trim());
                else if (lijevo.ToLowerInvariant().Contains("tree"))
                    IzStore(lijevo, desno, citaj);
                else
                {
                    var c = new Cvor();
                    c.Natpis = Prevedi(lijevo);
                    c.Ikona = 0;
                    c.Tag = ":" + desno;
                    Raspakiraj(c);
                    Stanje.Korijen.Add(c);
                }
            }
            Filtri(citaj);
        }

        public string Prevedi(string tekst)
        {
            if (tekst == null)
                return "";
            if (string.IsNullOrWhiteSpace(Jezik) || string.Equals(Jezik.Trim(), "hrv", StringComparison.OrdinalIgnoreCase))
                return tekst;
            if (Rjecnik != null)
            {
                string v;
                if (Rjecnik.TryGetValue(tekst, out v))
                    return v;
            }
            NgDnevnik.Akcija("prijevod nema riječ: " + tekst);
            return tekst;
        }

        public void Odaberi(Cvor cvor)
        {
            if (cvor == null)
                return;
            Stanje.Node = cvor.Node ?? "";
            Kontekst.Node = Stanje.Node;
            Stanje.ZadnjaNaredba = cvor.Naredba ?? "";
            NgDnevnik.Akcija("AfterSelect " + (cvor.Tag ?? ""));
        }

        /// <summary>TREEDRAGDROP se ne šalje ako je upis. Inače ostaje tekst za host.</summary>
        public string SqlOtpusta()
        {
            string sql = SqlTekst.Sredi(Stanje.DragDropSql ?? "", Kontekst);
            if (string.IsNullOrWhiteSpace(sql))
                return "";
            if (SqlTekst.JeZabranaPisanja(sql))
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + sql);
                return "";
            }
            NgDnevnik.Sql(sql);
            return sql;
        }

        private void GodinaMjesec(string imeForme)
        {
            for (int g = 2005; g <= 2009; g++)
            {
                var godina = new Cvor();
                godina.Natpis = g.ToString(CultureInfo.InvariantCulture);
                godina.Ikona = 0;
                godina.Tag = g.ToString(CultureInfo.InvariantCulture) + "0101#" + g.ToString(CultureInfo.InvariantCulture) + "1231:" + imeForme;
                Raspakiraj(godina);
                for (int j = 1; j <= 12; j++)
                {
                    var mjesec = new Cvor();
                    mjesec.Natpis = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(j);
                    mjesec.Ikona = 0;
                    string js = j.ToString(CultureInfo.InvariantCulture);
                    string gs = g.ToString(CultureInfo.InvariantCulture);
                    mjesec.Tag = gs + js + "01#" + gs + js + "30:" + imeForme;
                    Raspakiraj(mjesec);
                    godina.Djeca.Add(mjesec);
                }
                Stanje.Korijen.Add(godina);
            }
        }

        private void IzStore(string stora, string imeForme, Func<string, DataTable> citaj)
        {
            string sql = SqlTekst.Sredi(stora, Kontekst);
            Stanje.SqlPozivi.Add(sql);
            NgDnevnik.Akcija("Punjenje drveta sa storom : " + sql);
            NgDnevnik.Sql(sql);
            DataTable tablica = null;
            try
            {
                if (citaj != null && !SqlTekst.JeZabranaPisanja(sql))
                    tablica = citaj(sql);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(" TREE PROBLEM : " + ex.Message);
                return;
            }
            if (tablica == null || tablica.Rows.Count == 0 || tablica.Columns.Count < 3)
            {
                if (tablica == null)
                    NgDnevnik.Greska(" TREE PROBLEM : nema tablice");
                return;
            }
            if (!IdStupacJedinstven(tablica))
            {
                NgDnevnik.Greska(" TREE PROBLEM : stupac 0 nije jedinstven");
                return;
            }
            var vidjeni = new Dictionary<DataRow, bool>();
            var root = IzReda(tablica.Rows[0], imeForme);
            DodajDjecu(tablica, tablica.Rows[0], root, imeForme, vidjeni);
            Stanje.Korijen.Add(root);
        }

        private static bool IdStupacJedinstven(DataTable tablica)
        {
            var vidjeni = new Dictionary<string, bool>(StringComparer.Ordinal);
            for (int i = 0; i < tablica.Rows.Count; i++)
            {
                string id = Celija(tablica.Rows[i], 0);
                if (id.Length == 0)
                    continue;
                if (vidjeni.ContainsKey(id))
                    return false;
                vidjeni[id] = true;
            }
            return true;
        }

        private void DodajDjecu(DataTable tablica, DataRow roditelj, Cvor cvor, string imeForme, Dictionary<DataRow, bool> vidjeni)
        {
            string id = Celija(roditelj, 0);
            for (int i = 0; i < tablica.Rows.Count; i++)
            {
                DataRow dr = tablica.Rows[i];
                if (dr == roditelj || vidjeni.ContainsKey(dr))
                    continue;
                if (Celija(dr, 1) != id)
                    continue;
                if (Celija(dr, 0) == "0")
                    continue;
                vidjeni[dr] = true;
                var dijete = IzReda(dr, imeForme);
                dijete.Oznaka = Celija(dr, 0);
                cvor.Djeca.Add(dijete);
                DodajDjecu(tablica, dr, dijete, imeForme, vidjeni);
            }
        }

        private Cvor IzReda(DataRow dr, string imeForme)
        {
            var c = new Cvor();
            c.Oznaka = Celija(dr, 0);
            string natpis = Celija(dr, 2);
            c.Natpis = Stanje.Prevedi ? Prevedi(natpis) : natpis;
            c.Ikona = 0;
            string t = dr.Table.Columns.Count > 3 ? Celija(dr, 3) : "";
            if (t.IndexOf(':') >= 0)
                c.Tag = t;
            else
                c.Tag = t + ":" + imeForme;
            Raspakiraj(c);
            return c;
        }

        private void Filtri(Func<string, DataTable> citaj)
        {
            if (string.IsNullOrWhiteSpace(ImeXml) || citaj == null)
                return;
            string sql = "select * from NetGenDBFilter where granula='" + ImeXml.Replace("'", "''") + "'";
            Stanje.SqlPozivi.Add(sql);
            DataTable t = null;
            try
            {
                t = citaj(sql);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(" TREE PROBLEM : " + ex.Message);
                return;
            }
            if (t == null)
                return;
            int iNaslov = t.Columns.IndexOf("naslov");
            int iSql = t.Columns.IndexOf("sql");
            if (iNaslov < 0 || iSql < 0)
                return;
            for (int i = 0; i < t.Rows.Count; i++)
            {
                var c = new Cvor();
                c.Natpis = Convert.ToString(t.Rows[i][iNaslov]) ?? "";
                c.Tag = ":==" + (Convert.ToString(t.Rows[i][iSql]) ?? "");
                Raspakiraj(c);
                Stanje.Korijen.Add(c);
            }
        }

        private static void Raspakiraj(Cvor c)
        {
            string tag = c.Tag ?? "";
            int k = tag.IndexOf(':');
            if (k < 0)
            {
                c.Node = tag;
                c.Naredba = "";
                return;
            }
            c.Node = tag.Substring(0, k);
            c.Naredba = tag.Substring(k + 1);
        }

        private static string Celija(DataRow dr, int i)
        {
            if (dr == null || i >= dr.Table.Columns.Count || dr[i] == DBNull.Value || dr[i] == null)
                return "";
            return dr[i].ToString().Trim();
        }

        private static string Atr(NgProject projekt, string ime)
        {
            if (projekt == null || projekt.Atributi == null || !projekt.Atributi.Ima(ime))
                return null;
            return projekt.Atributi.Vrijednost(ime);
        }
    }
}
