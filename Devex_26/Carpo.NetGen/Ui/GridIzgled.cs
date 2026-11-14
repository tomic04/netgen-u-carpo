using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using DevExpress.Data;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Class1.CreateGridColumns (preko MasterGrid.IzPodatka) i COLUMN definicije (GridDefinicija) na
    /// DevExpress GridView: natpis, sirina, vidljivost, format, poravnanje stupca i zaglavlja, redoslijed
    /// (Janus Position), prelamanje, boje, crveni brojevi, grupiranje '!' i EDITCOLUMNS. Isto radi
    /// Podloga.VeziGrid za master i detail.
    /// </summary>
    public static class GridIzgled
    {
        public static void Primijeni(GridView pogled, DataTable tablica, string editColumns)
        {
            Primijeni(pogled, tablica, editColumns, false, null, false);
        }

        /// <summary>
        /// Grid dijaloga. Form2.RefreshGrida 5305-5315 ostavlja postojece stupce (i sve sto su COLUMN
        /// definicije i korisnik na njima promijenili), pa se stupci slazu samo kad se popis stupaca
        /// promijeni. Vraca true kad su stupci slozeni iznova.
        /// ukupno: TotalRow (Form2.vb 1739-1746, TOTALROW nije "FALSE"); AggregateFunction iz Class1.vb
        /// 1171 (decimal Sum) ili COLUMN AGREGATEFUNCTION (Form2.vb 1886).
        /// </summary>
        public static bool Primijeni(GridView pogled, DataTable tablica, string editColumns, bool ukupno, IList<NgColumn> defs, bool clear)
        {
            if (pogled == null || tablica == null)
                return false;
            string potpis = Potpis(tablica);
            string bilo;
            if (Slozeno.TryGetValue(pogled, out bilo) && bilo == potpis && IstiStupci(pogled, tablica))
                return false;
            Slozeno.Remove(pogled);
            Slozeno.Add(pogled, potpis);
            pogled.PopulateColumns();
            var kolone = new List<GridKolona>();
            kolone.Add(new GridKolona { Podatak = "Icon", Natpis = "", Sirina = 22, Vidljiva = false });
            kolone.Add(new GridKolona { Podatak = "Selector", Natpis = "", Sirina = 22, Selektor = true });
            foreach (DataColumn dc in tablica.Columns)
                kolone.Add(MasterGrid.IzPodatka(dc, editColumns));
            GridDefinicija.Primijeni(kolone, defs, clear, null, true);
            PrimijeniKolone(pogled, kolone);
            Ukupno(pogled, tablica, kolone);
            pogled.OptionsView.ShowFooter = ukupno;
            bool uredjuje = false;
            for (int i = 0; i < kolone.Count; i++)
            {
                if (!kolone[i].Selektor && kolone[i].Uredivanje)
                    uredjuje = true;
            }
            pogled.OptionsBehavior.Editable = uredjuje;
            return true;
        }

        /// <summary>GridKolona na stupce pogleda. Ikona i kvacica nisu DevExpress stupci.</summary>
        public static void PrimijeniKolone(GridView pogled, List<GridKolona> kolone)
        {
            if (pogled == null || kolone == null)
                return;
            bool premjesteno = false;
            bool prelama = false;
            int grupa = 0;
            pogled.RowCellStyle -= CrveniBroj;
            for (int i = 0; i < kolone.Count; i++)
            {
                GridKolona k = kolone[i];
                if (k.Selektor || k.Podatak == "Icon" || string.IsNullOrEmpty(k.Podatak))
                    continue;
                GridColumn c = pogled.Columns.ColumnByFieldName(k.Podatak);
                if (c == null)
                    continue;
                if (k.Pozicija.HasValue)
                    premjesteno = true;
                c.Visible = k.Vidljiva;
                c.Caption = k.Natpis ?? k.Podatak;
                if (k.Sirina > 0)
                    c.Width = k.Sirina;
                c.OptionsColumn.AllowEdit = k.Uredivanje;
                if (!string.IsNullOrEmpty(k.Format))
                {
                    // FormatType prvi: promjena tipa u FormatInfo vrati FormatString na zadani.
                    c.DisplayFormat.FormatType = k.Format.IndexOf("yy", StringComparison.OrdinalIgnoreCase) >= 0
                        || k.Format.IndexOf("dd", StringComparison.OrdinalIgnoreCase) >= 0
                        ? DevExpress.Utils.FormatType.DateTime
                        : DevExpress.Utils.FormatType.Numeric;
                    c.DisplayFormat.FormatString = k.Format;
                }
                DevExpress.Utils.HorzAlignment h;
                if (Vodoravno(k.Poravnanje, out h))
                    c.AppearanceCell.TextOptions.HAlignment = h;
                if (Vodoravno(k.PoravnanjeZaglavlja, out h))
                    c.AppearanceHeader.TextOptions.HAlignment = h;
                if (k.Prelomi)
                {
                    c.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    prelama = true;
                }
                Color boja;
                if (Boja(k.BojaTeksta, out boja))
                    c.AppearanceCell.ForeColor = boja;
                if (Boja(k.BojaPozadine, out boja))
                    c.AppearanceCell.BackColor = boja;
                c.Tag = k.NegativnoCrveno ? Crveno.Negativno : k.NotZeroCrveno ? Crveno.NijeNula : (object)null;
                if (k.Grupiraj)
                    c.GroupIndex = grupa++;
            }
            if (prelama)
                pogled.OptionsView.RowAutoHeight = true;
            pogled.RowCellStyle += CrveniBroj;
            if (premjesteno)
                Poredaj(pogled, kolone);
            for (int i = 0; i < kolone.Count; i++)
            {
                if (!kolone[i].AutoSirina || string.IsNullOrEmpty(kolone[i].Podatak))
                    continue;
                GridColumn c = pogled.Columns.ColumnByFieldName(kolone[i].Podatak);
                if (c != null && c.Visible)
                    c.BestFit();
            }
        }

        /// <summary>Ukupni red: GridAgregat iz Agregat stupca (kao Podloga.PrimijeniUkupno).</summary>
        public static void Ukupno(GridView pogled, DataTable tablica, List<GridKolona> kolone)
        {
            for (int i = 0; i < kolone.Count; i++)
            {
                GridKolona k = kolone[i];
                if (k.Selektor || string.IsNullOrEmpty(k.Podatak))
                    continue;
                GridColumn c = pogled.Columns.ColumnByFieldName(k.Podatak);
                if (c == null)
                    continue;
                c.Summary.Clear();
                string vrsta = GridAgregat.Vrsta(k.Agregat);
                if (vrsta.Length == 0)
                    continue;
                Type tip = tablica != null && tablica.Columns.Contains(k.Podatak) ? tablica.Columns[k.Podatak].DataType : null;
                c.Summary.Add((SummaryItemType)Enum.Parse(typeof(SummaryItemType), vrsta), k.Podatak, GridAgregat.Format(vrsta, tip, k.Format));
            }
        }

        /// <summary>FROZENCOLUMNS: Janus broji stupac ikone i kvacicu (GridAgregat.ZamrznutiPodaci).</summary>
        public static void Zamrzni(GridView pogled, int? frozen, bool kvacica)
        {
            int zamrznuti = GridAgregat.ZamrznutiPodaci(frozen, kvacica);
            for (int i = 0; i < pogled.VisibleColumns.Count; i++)
                pogled.VisibleColumns[i].Fixed = i < zamrznuti ? FixedStyle.Left : FixedStyle.None;
        }

        /// <summary>Redoslijed prikaza (Redni) nakon Janus Position; jednaki Redni ostaju kako su bili.</summary>
        private static void Poredaj(GridView pogled, List<GridKolona> kolone)
        {
            var redom = new List<KeyValuePair<int, GridKolona>>();
            for (int i = 0; i < kolone.Count; i++)
                redom.Add(new KeyValuePair<int, GridKolona>(i, kolone[i]));
            redom.Sort(delegate(KeyValuePair<int, GridKolona> a, KeyValuePair<int, GridKolona> b)
            {
                int r = a.Value.Redni.CompareTo(b.Value.Redni);
                return r != 0 ? r : a.Key.CompareTo(b.Key);
            });
            int vi = 0;
            for (int i = 0; i < redom.Count; i++)
            {
                GridKolona k = redom[i].Value;
                if (k.Selektor || string.IsNullOrEmpty(k.Podatak))
                    continue;
                GridColumn c = pogled.Columns.ColumnByFieldName(k.Podatak);
                if (c == null || !c.Visible)
                    continue;
                c.VisibleIndex = vi++;
            }
        }

        private enum Crveno
        {
            Negativno,
            NijeNula
        }

        /// <summary>Form1.vb 7543-7563: NEGATIVERED (vrijednost &lt; 0) i NOTZERORED (&lt;&gt; 0) crvenim slovima.</summary>
        private static void CrveniBroj(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column == null || !(e.Column.Tag is Crveno) || e.CellValue == null || e.CellValue == DBNull.Value)
                return;
            double v;
            try { v = Convert.ToDouble(e.CellValue, System.Globalization.CultureInfo.CurrentCulture); }
            catch (FormatException) { return; }
            catch (InvalidCastException) { return; }
            bool crveno = (Crveno)e.Column.Tag == Crveno.Negativno ? v < 0 : v != 0;
            if (crveno)
                e.Appearance.ForeColor = Color.Red;
        }

        private static bool Vodoravno(string poravnanje, out DevExpress.Utils.HorzAlignment h)
        {
            h = DevExpress.Utils.HorzAlignment.Default;
            switch (GridDefinicija.Poravnanje(poravnanje))
            {
                case "Near":
                    h = DevExpress.Utils.HorzAlignment.Near;
                    return true;
                case "Center":
                    h = DevExpress.Utils.HorzAlignment.Center;
                    return true;
                case "Far":
                    h = DevExpress.Utils.HorzAlignment.Far;
                    return true;
                default:
                    return false;
            }
        }

        private static bool Boja(string ime, out Color boja)
        {
            boja = Color.Empty;
            if (string.IsNullOrEmpty(ime))
                return false;
            Color c = Color.FromName(ime);
            if (!c.IsKnownColor)
                return false;
            boja = c;
            return true;
        }

        /// <summary>Popis stupaca za koji je pogled zadnji put slozen (DataSource sam doda stupce, pa to nije znak).</summary>
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<GridView, string> Slozeno =
            new System.Runtime.CompilerServices.ConditionalWeakTable<GridView, string>();

        private static string Potpis(DataTable tablica)
        {
            var sb = new System.Text.StringBuilder();
            foreach (DataColumn dc in tablica.Columns)
                sb.Append(dc.ColumnName).Append('|').Append(dc.DataType == null ? "" : dc.DataType.Name).Append(';');
            return sb.ToString();
        }

        private static bool IstiStupci(GridView pogled, DataTable tablica)
        {
            if (pogled.Columns.Count != tablica.Columns.Count)
                return false;
            foreach (DataColumn dc in tablica.Columns)
            {
                if (pogled.Columns.ColumnByFieldName(dc.ColumnName) == null)
                    return false;
            }
            return true;
        }
    }
}
