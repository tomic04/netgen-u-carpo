using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Grid
{
    public sealed class DetailStanje
    {
        public string Predlozak = "";
        public string SqlZaSlanje = "";
        public bool Nema = true;
        public bool OtvaraFormu;
        public bool CheckboxVidljiv = true;
        public bool TotalRed = true;
        public bool FokusZadnji;
        public string IdMaster = "0";
        public string Parametri = "";
        public DataTable Tablica;
        public string Poruka;
        public readonly List<GridKolona> Kolone = new List<GridKolona>();
        public readonly List<string> DodatniPredlosci = new List<string>();
        public readonly List<string> DodatniSql = new List<string>();
    }

    /// <summary>
    /// Donji grid. PokaziUpitDetalj i GridEX1.SelectionChanged.
    /// Prazan DETAILGRID ne šalje SQL. Vodeće : ne šalje SQL (NetGen kaže sorkac).
    /// </summary>
    public sealed class DetailGrid
    {
        public DetailStanje Stanje = new DetailStanje();
        public SqlKontekst Kontekst = new SqlKontekst();

        public void Pripremi(NgProject projekt)
        {
            Stanje = new DetailStanje();
            if (projekt == null)
                return;
            Stanje.Predlozak = (projekt.DetailGrid ?? "").Trim();
            string cb = Atr(projekt, "DETCHECKBOXVISIBLE");
            if (cb != null)
                Stanje.CheckboxVidljiv = cb.Trim().ToUpperInvariant() != "FALSE";
            string total = Atr(projekt, "DETTOTALROWVISIBLE");
            if (total != null)
                Stanje.TotalRed = total.Trim().ToUpperInvariant() != "FALSE";
            Stanje.FokusZadnji = string.Equals((Atr(projekt, "DETMOVELAST") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
            Dodaj(Atr(projekt, "SQL2"));
            Dodaj(Atr(projekt, "SQL3"));
            Dodaj(Atr(projekt, "SQL4"));
            if (Stanje.Predlozak.Length == 0)
            {
                Stanje.Nema = true;
                return;
            }
            Stanje.Nema = false;
            if (Stanje.Predlozak.StartsWith(":", StringComparison.Ordinal))
            {
                Stanje.OtvaraFormu = true;
                NgDnevnik.Akcija("DETAILGRID otvara formu " + Stanje.Predlozak.Substring(1));
            }
        }

        public string Slozi(DataRow red)
        {
            if (Stanje.Nema || Stanje.OtvaraFormu)
                return "";
            Stanje.IdMaster = IdReda(red);
            Kontekst.Id = Stanje.IdMaster;
            Stanje.Parametri = ParametriIzReda(red);
            string sql = SqlTekst.Sredi(Stanje.Predlozak, Kontekst);
            sql = ZamijeniOstatak(sql, Stanje.Parametri);
            sql = sql.Replace("~~", ",");
            Stanje.SqlZaSlanje = sql;
            Stanje.DodatniSql.Clear();
            for (int i = 0; i < Stanje.DodatniPredlosci.Count; i++)
            {
                string d = SqlTekst.Sredi(Stanje.DodatniPredlosci[i], Kontekst);
                Stanje.DodatniSql.Add(ZamijeniOstatak(d, Stanje.Parametri).Replace("~~", ","));
            }
            return sql;
        }

        public void PrimijeniTablicu(DataTable tablica, NgColumnGroup definicija)
        {
            Stanje.Tablica = tablica;
            Stanje.Kolone.Clear();
            Stanje.Poruka = null;
            if (tablica == null)
                return;
            Stanje.Kolone.Add(new GridKolona { Podatak = "Icon", Sirina = 22, Vidljiva = false, EditTip = "NoEdit" });
            var sel = new GridKolona { Podatak = "Selector", Sirina = 22, Selektor = true, Vidljiva = Stanje.CheckboxVidljiv, EditTip = "CheckBox" };
            Stanje.Kolone.Add(sel);
            foreach (DataColumn c in tablica.Columns)
                Stanje.Kolone.Add(MasterGrid.IzPodatka(c, ""));
            if (definicija == null)
                return;
            bool clear = definicija.ClearColums != null && definicija.ClearColums.Trim().ToLowerInvariant() == "true";
            if (clear)
            {
                for (int i = 0; i < Stanje.Kolone.Count; i++)
                    Stanje.Kolone[i].Vidljiva = false;
            }
            for (int n = 0; n < definicija.Kolone.Count; n++)
            {
                var def = definicija.Kolone[n];
                GridKolona kol = null;
                for (int i = 0; i < Stanje.Kolone.Count; i++)
                {
                    if (string.Equals(Stanje.Kolone[i].Podatak, def.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        kol = Stanje.Kolone[i];
                        break;
                    }
                }
                if (kol == null)
                    continue;
                if (clear)
                    kol.Vidljiva = true;
                string width = def.Atributi.Vrijednost("WIDTH");
                int w;
                if (!string.IsNullOrEmpty(width) && int.TryParse(width.Trim(), out w) && w > 0)
                    kol.Sirina = MasterGrid.VbJedinica(w);
                string cap = def.Atributi.Vrijednost("CAPTION");
                if (!string.IsNullOrEmpty(cap) && cap.Trim().Length > 0)
                    kol.Natpis = cap.Trim();
                string vis = def.Atributi.Vrijednost("VISIBLE");
                if (!string.IsNullOrEmpty(vis) && vis.Trim().Length > 0)
                    kol.Vidljiva = vis.Trim().ToLowerInvariant() != "false";
            }
        }

        public static string IdReda(DataRow red)
        {
            if (red == null || red.Table == null || red.Table.Columns.Count == 0)
                return "0";
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                if (string.Equals(red.Table.Columns[i].ColumnName, "id", StringComparison.OrdinalIgnoreCase))
                    return red[i] == DBNull.Value ? "0" : Convert.ToString(red[i]);
            }
            return red[0] == DBNull.Value ? "0" : Convert.ToString(red[0]);
        }

        public static string ParametriIzReda(DataRow red)
        {
            if (red == null)
                return "";
            var sb = new StringBuilder();
            foreach (DataColumn c in red.Table.Columns)
            {
                if (red[c] == null || red[c] == DBNull.Value)
                    continue;
                string ime = c.ColumnName.Trim();
                if (ime.Length >= 4)
                {
                    int n;
                    if (int.TryParse(ime.Substring(ime.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                ime = ime.Replace(" ", "_").Replace("!", "");
                string v = Convert.ToString(red[c]) ?? "";
                double broj;
                if (double.TryParse(v, NumberStyles.Any, CultureInfo.CurrentCulture, out broj))
                    v = v.Replace(",", ".");
                sb.Append('@').Append(ime).Append("='").Append(v).Append("' , ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Ključevi detaila koji ne ovise o retcima: selekcija ponovo zove DETAILGRID,
        /// stupci se slože jednom, DETCHECKBOXVISIBLE i CLEARCOLUMS, širina WIDTH/14.
        /// </summary>
        public static void Kljucevi(NgProject p, GridStanje master, Dictionary<string, string> s)
        {
            if (p == null || s == null)
                return;
            bool cb = master == null || master.CheckboxVidljiv;
            s["checkboxVidljiv"] = cb ? "true" : "false";
            int timeout = master == null ? 30 : master.TimeoutSekundi;
            s["timeout"] = timeout.ToString(CultureInfo.InvariantCulture);
            bool ima = !string.IsNullOrWhiteSpace(p.DetailGrid);
            s["detailNaSelekciji"] = ima ? "true" : "false";
            s["detailNakonMastera"] = ima ? "true" : "false";
            s["detailKoloneJednom"] = ima ? "true" : "false";
            NgColumnGroup det = p.NadjiGrupu("DETAILCOLUMNS");
            bool clear = det != null && string.Equals(det.ClearColums, "true", StringComparison.OrdinalIgnoreCase);
            s["detClear"] = clear ? "true" : "false";
            string detCb = "";
            if (p.Atributi != null && p.Atributi.Ima("DETCHECKBOXVISIBLE"))
                detCb = p.Atributi.Vrijednost("DETCHECKBOXVISIBLE") ?? "";
            bool detSel = detCb.Trim().Length == 0 || detCb.Trim().ToUpperInvariant() != "FALSE";
            s["detSelector"] = ima && detSel ? "true" : "false";
            UpisiSirine(det, "det.", s);
            UpisiSirine(p.NadjiGrupu("MASTERCOLUMNS"), "kolona.", s);
        }

        private static void UpisiSirine(NgColumnGroup grupa, string prefiks, Dictionary<string, string> s)
        {
            if (grupa == null)
                return;
            bool detail = prefiks == "det.";
            for (int i = 0; i < grupa.Kolone.Count; i++)
            {
                NgColumn k = grupa.Kolone[i];
                string ime = k.Name ?? "";
                if (ime.Length == 0)
                    continue;
                // WIDTH iz XML-a prepisuje sirinu koju je SQL vec slozio (Form1.vb 7522).
                int w = 100;
                string width = k.Atributi == null ? null : k.Atributi.Vrijednost("WIDTH");
                int n;
                if (!string.IsNullOrEmpty(width) && int.TryParse(width.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n > 0)
                    w = MasterGrid.VbJedinica(n);
                if (detail)
                {
                    string fmt = k.Atributi == null ? "" : (k.Atributi.Vrijednost("FORMAT") ?? "").Trim();
                    s["det." + ime] = w.ToString(CultureInfo.InvariantCulture) + "|" + fmt;
                }
                else
                    s["kolona." + ime + ".sirina"] = w.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string ZamijeniOstatak(string sql, string parametri)
        {
            if (sql == null)
                return "";
            int guard = 0;
            while (sql.IndexOf("@@", StringComparison.Ordinal) >= 0 && guard < 40)
            {
                guard++;
                int poc = sql.IndexOf("@@", StringComparison.Ordinal);
                int kraj = sql.Length;
                string ostatak = sql.Substring(poc);
                char[] rez = { ' ', '\'', '%', '#', ',', ')', '*', '/', '\\', '+', '-', '.', '(', '=' };
                for (int i = 0; i < rez.Length; i++)
                {
                    int p = ostatak.IndexOf(rez[i]);
                    if (p >= 0 && poc + p < kraj)
                        kraj = poc + p;
                }
                if (kraj <= poc)
                    break;
                string token = sql.Substring(poc, kraj - poc);
                string vrijednost = Nadji(token, parametri);
                sql = sql.Substring(0, poc) + vrijednost + sql.Substring(kraj);
            }
            return sql;
        }

        private static string Nadji(string token, string parametri)
        {
            string kljuc = (token ?? "").Trim();
            if (kljuc.StartsWith("@@", StringComparison.Ordinal))
                kljuc = "@" + kljuc.Substring(2);
            if (string.IsNullOrEmpty(parametri) || kljuc.Length == 0)
                return "";
            string[] dijelovi = parametri.Split(',');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string dio = dijelovi[i].Trim();
                if (dio.Length < kljuc.Length)
                    continue;
                if (!dio.Substring(0, kljuc.Length).Equals(kljuc, StringComparison.OrdinalIgnoreCase))
                    continue;
                int eq = dio.IndexOf('=');
                if (eq < 0)
                    continue;
                return dio.Substring(eq + 1).Trim();
            }
            return "";
        }

        private void Dodaj(string sql)
        {
            if (!string.IsNullOrWhiteSpace(sql))
                Stanje.DodatniPredlosci.Add(sql.Trim());
        }

        private static string Atr(NgProject projekt, string ime)
        {
            if (projekt == null || projekt.Atributi == null || !projekt.Atributi.Ima(ime))
                return null;
            return projekt.Atributi.Vrijednost(ime);
        }
    }
}
