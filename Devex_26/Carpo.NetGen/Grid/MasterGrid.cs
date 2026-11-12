using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Grid
{
    public sealed class GridKolona
    {
        public string Podatak;
        public string Natpis;
        public int Sirina;
        public string Format;
        public bool Vidljiva = true;
        public bool Uredivanje;
        public string Poravnanje;
        public string Agregat;
        public string BojaTeksta;
        public string BojaPozadine;
        public bool NegativnoCrveno;
        public bool NotZeroCrveno;
        public bool Prelomi;
        public int MaxLinija;
        public bool AutoSirina;
        public string PoravnanjeZaglavlja;
        public int? Pozicija;
        public bool Selektor;
        public bool Grupiraj;
    }

    public sealed class GridStanje
    {
        public string IzvorSql = "";
        public string ZadnjiSql = "";
        public string SqlZaSlanje = "";
        public string Odluka = "";
        public bool OtvaraFormu;
        public string ImeForme = "";
        public bool CheckboxVidljiv = true;
        public bool TotalRed = true;
        public bool SortKlikom = true;
        public bool FilterRed;
        public bool AllowEditKadJeFilter;
        public string EnterTipka = "None";
        public int? ZamrznuteKolone;
        public bool? GroupByBox;
        public int TimeoutSekundi = 30;
        public string FocusRazlog = "";
        public int FocusRed;
        public string Id = "0";
        public bool BlokiranoPisanje;
        public readonly List<string> Redoslijed = new List<string>();
        public readonly List<GridKolona> Kolone = new List<GridKolona>();
        public DataTable Tablica;
        public readonly List<string> DxPostavke = new List<string>();
    }

    /// <summary>
    /// Master grid podloge, Janus put (DXGRID nije u korpusu).
    /// Redoslijed je PokaziUpit + DodatnoFormatiraj + CustomFormatKolona + CreateGridColumns.
    /// </summary>
    public sealed class MasterGrid
    {
        public const int FakPretvorbe = 14;
        public SqlKontekst Kontekst = new SqlKontekst();
        public GridStanje Stanje = new GridStanje();

        public void Ucitaj(NgProject projekt, DataTable tablica)
        {
            Stanje = new GridStanje();
            if (projekt == null)
                return;
            string onLoad = projekt.Atributi.Ima("ONLOAD") ? (projekt.Atributi.Vrijednost("ONLOAD") ?? "").Trim() : null;
            string stora = projekt.Atributi.Ima("STARTUPSTORA") ? (projekt.Atributi.Vrijednost("STARTUPSTORA") ?? "").Trim() : null;
            string efektivni = stora ?? "";
            if (onLoad != null)
                efektivni = onLoad.Trim();

            Stanje.Redoslijed.Add("startup-sql");
            if (efektivni.StartsWith(":", StringComparison.Ordinal))
            {
                Stanje.OtvaraFormu = true;
                Stanje.ImeForme = efektivni.Substring(1);
                Stanje.Odluka = "forma";
                Stanje.Redoslijed.Add("nije-sql");
                PrimijeniAtribute(projekt, null);
                return;
            }

            Stanje.IzvorSql = efektivni;
            string prvi = SqlTekst.Sredi(efektivni, Kontekst);
            Stanje.ZadnjiSql = prvi;
            string drugi = SqlTekst.Sredi(prvi, Kontekst);
            drugi = drugi.Replace("~~", ",");
            Stanje.SqlZaSlanje = drugi;
            Stanje.Odluka = SqlTekst.Odluka(drugi);
            Stanje.Redoslijed.Add("sredi");
            Stanje.Redoslijed.Add("odluka:" + Stanje.Odluka);
            if (Stanje.Odluka == "salji" && SqlTekst.JeZabranaPisanja(drugi))
            {
                Stanje.BlokiranoPisanje = true;
                Stanje.Odluka = "zabrana-pisanja";
                Stanje.Redoslijed.Add("zabrana-pisanja");
            }
            if (Stanje.Odluka == "salji")
            {
                Stanje.Tablica = tablica;
                Stanje.Redoslijed.Add("exec");
                SloziKolone(projekt, tablica);
                Fokus(projekt);
            }
            else
            {
                PrimijeniAtribute(projekt, null);
            }
            Stanje.Redoslijed.Add("kraj");
            GridIzgled.Popuni(Stanje);
        }

        public void Osvjezi(DataTable tablica)
        {
            Stanje.Redoslijed.Add("REFRESHMASTERGRID");
            string drugi = SqlTekst.Sredi(Stanje.ZadnjiSql ?? "", Kontekst).Replace("~~", ",");
            Stanje.SqlZaSlanje = drugi;
            Stanje.Odluka = SqlTekst.Odluka(drugi);
            if (Stanje.Odluka == "salji" && !SqlTekst.JeZabranaPisanja(drugi))
            {
                Stanje.Tablica = tablica;
                Stanje.Kolone.Clear();
                SloziKolone(null, tablica);
            }
            Stanje.Redoslijed.Add("kraj-refresh");
        }

        private void SloziKolone(NgProject projekt, DataTable tablica)
        {
            Stanje.Redoslijed.Add("tip-format");
            Stanje.Kolone.Add(new GridKolona { Podatak = "Icon", Natpis = "", Sirina = 22, Vidljiva = false });
            Stanje.Kolone.Add(new GridKolona { Podatak = "Selector", Natpis = "", Sirina = 22, Selektor = true, Vidljiva = true, Uredivanje = true });
            if (tablica != null)
            {
                foreach (DataColumn c in tablica.Columns)
                    Stanje.Kolone.Add(IzPodatka(c, EditStupci(projekt)));
            }
            if (projekt != null)
            {
                PrimijeniAtribute(projekt, projekt.NadjiGrupu("MASTERCOLUMNS"));
                Stanje.Redoslijed.Add("custom-format");
                PrimijeniDefiniciju(projekt.NadjiGrupu("MASTERCOLUMNS"));
            }
        }

        private static string EditStupci(NgProject projekt)
        {
            if (projekt == null || !projekt.Atributi.Ima("EDITCOLUMNS"))
                return "";
            return projekt.Atributi.Vrijednost("EDITCOLUMNS") ?? "";
        }

        public static GridKolona IzPodatka(DataColumn c, string editColumns)
        {
            var k = new GridKolona();
            k.Podatak = c.ColumnName;
            k.Natpis = c.ColumnName ?? "";
            string tip = c.DataType == null ? "" : c.DataType.ToString().ToLowerInvariant();
            if (tip.IndexOf("bool", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 34;
                k.Poravnanje = "Far";
            }
            else if (tip.IndexOf("int", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 34;
                k.Format = "##0";
                k.Poravnanje = "Far";
            }
            else if (tip.IndexOf("decimal", StringComparison.Ordinal) >= 0 || tip.IndexOf("double", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 44;
                k.Format = "#,##0.00";
                k.Poravnanje = "Far";
            }
            else if (tip.IndexOf("date", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 100;
                k.Format = "dd.MM.yyyy";
            }
            else if (tip.IndexOf("string", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 200;
            }

            string cap = (k.Natpis ?? "").Trim();
            if (cap.Length >= 4)
            {
                int n;
                if (int.TryParse(cap.Substring(cap.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n != 0)
                {
                    k.Sirina = n / FakPretvorbe;
                    k.Natpis = cap.Substring(0, cap.Length - 4);
                }
            }
            string prije = (k.Natpis ?? "").Trim();
            if (prije.StartsWith("!", StringComparison.Ordinal))
                k.Grupiraj = true;
            // Class1 1107 brise '!' prije provjere sum! (1116), pa sum! nikad ne upali. Ne vezujemo.
            k.Natpis = (k.Natpis ?? "").Replace(";", "\r\n").Replace("!", "");
            string izvor = c.ColumnName ?? "";
            if (izvor.Length >= 2 && izvor.Substring(izvor.Length - 2).ToUpperInvariant() == "ID")
                k.Vidljiva = false;
            if (editColumns != null && prije.Length > 0 && editColumns.ToLowerInvariant().Contains(prije.ToLowerInvariant()))
                k.Uredivanje = true;
            return k;
        }

        private void PrimijeniAtribute(NgProject projekt, NgColumnGroup master)
        {
            string cb = Attr(projekt, "CHECKBOXVISIBLE");
            if (cb != null)
                Stanje.CheckboxVidljiv = cb.ToUpperInvariant() != "FALSE";
            for (int i = 0; i < Stanje.Kolone.Count; i++)
            {
                if (Stanje.Kolone[i].Selektor)
                    Stanje.Kolone[i].Vidljiva = Stanje.CheckboxVidljiv;
            }
            string total = Attr(projekt, "TOTALROWVISIBLE");
            if (total != null)
                Stanje.TotalRed = total.ToUpperInvariant() != "FALSE";
            string sort = Attr(projekt, "AUTOMATICSORT");
            if (sort != null && sort.Trim().ToLowerInvariant() == "false")
                Stanje.SortKlikom = false;
            string gb = Attr(projekt, "GROUPBYBOXVISIBLE");
            if (gb != null)
                Stanje.GroupByBox = gb.Trim().ToLowerInvariant() != "false";
            string fr = Attr(projekt, "FROZENCOLUMNS");
            int fz;
            if (fr != null && int.TryParse(fr.Trim(), out fz))
                Stanje.ZamrznuteKolone = fz;
            string fm = Attr(projekt, "FILTERMODE");
            if (fm != null && fm.Trim().ToLowerInvariant() == "true")
            {
                Stanje.FilterRed = true;
                string ed = Attr(projekt, "EDITCOLUMNS");
                Stanje.AllowEditKadJeFilter = ed != null && ed.Trim().Length > 0;
            }
            string timeout = Attr(projekt, "COMMANDTIMEOUT");
            int ts;
            if (timeout != null && int.TryParse(timeout.Trim(), out ts))
                Stanje.TimeoutSekundi = ts;
            if (!projekt.Atributi.Ima("EDITCOLUMNS") || string.IsNullOrEmpty(projekt.Atributi.Vrijednost("EDITCOLUMNS")))
                Stanje.EnterTipka = "None";
            if (projekt.Atributi.Ima("ENTERKEYBEHAVIOR"))
                Stanje.EnterTipka = (projekt.Atributi.Vrijednost("ENTERKEYBEHAVIOR") ?? "").Trim();
            if (master != null && !string.IsNullOrEmpty(master.HeaderAlignment))
            {
                for (int i = 0; i < Stanje.Kolone.Count; i++)
                    Stanje.Kolone[i].PoravnanjeZaglavlja = master.HeaderAlignment;
            }
        }

        private void PrimijeniDefiniciju(NgColumnGroup grupa)
        {
            if (grupa == null)
                return;
            bool clear = grupa.ClearColums != null && grupa.ClearColums.ToLowerInvariant() == "true";
            if (clear)
            {
                for (int i = 0; i < Stanje.Kolone.Count; i++)
                    Stanje.Kolone[i].Vidljiva = false;
            }
            int pos = 0;
            for (int n = 0; n < grupa.Kolone.Count; n++)
            {
                var def = grupa.Kolone[n];
                pos++;
                string indexRaw = def.Atributi.Vrijednost("INDEX");
                int index = 0;
                if (!string.IsNullOrEmpty(indexRaw))
                    int.TryParse(indexRaw.Trim(), out index);
                string ime = def.Name ?? "";
                if (index <= 0 && ime.Length == 0)
                    continue;
                GridKolona kol = null;
                if (index > 0)
                {
                    int vidljivi = 0;
                    for (int i = 0; i < Stanje.Kolone.Count; i++)
                    {
                        if (vidljivi == index)
                        {
                            kol = Stanje.Kolone[i];
                            break;
                        }
                        vidljivi++;
                    }
                }
                else
                {
                    for (int i = 0; i < Stanje.Kolone.Count; i++)
                    {
                        if (string.Equals(Stanje.Kolone[i].Podatak, ime, StringComparison.OrdinalIgnoreCase))
                        {
                            kol = Stanje.Kolone[i];
                            break;
                        }
                    }
                }
                if (kol == null)
                    continue;
                if (!string.IsNullOrEmpty(def.AgregateFunction))
                    kol.Agregat = def.AgregateFunction;
                string width = def.Atributi.Vrijednost("WIDTH");
                int w;
                if (!string.IsNullOrEmpty(width) && int.TryParse(width.Trim(), out w) && w > 0)
                    kol.Sirina = w / FakPretvorbe;
                string cap = def.Atributi.Vrijednost("CAPTION");
                if (!string.IsNullOrEmpty(cap) && cap.Trim().Length > 0)
                    kol.Natpis = cap.Trim();
                string fmt = def.Atributi.Vrijednost("FORMAT");
                if (!string.IsNullOrEmpty(fmt) && fmt.Trim().Length > 0)
                    kol.Format = fmt.Trim();
                string vis = def.Atributi.Vrijednost("VISIBLE");
                if (!string.IsNullOrEmpty(vis) && vis.Trim().Length > 0)
                    kol.Vidljiva = vis.Trim().ToLowerInvariant() != "false";
                if (clear)
                {
                    pos++;
                    kol.Pozicija = pos;
                    kol.Vidljiva = true;
                }
                string poz = def.Atributi.Vrijednost("POSITION");
                int pz;
                if (!string.IsNullOrEmpty(poz) && int.TryParse(poz.Trim(), out pz))
                    kol.Pozicija = pz;
                kol.Natpis = (kol.Natpis ?? "").Replace(";", "\r\n");
                string auto = def.Atributi.Vrijednost("AUTOSIZE");
                if (auto != null && auto.Trim().ToLowerInvariant() == "true")
                    kol.AutoSirina = true;
                string neg = def.Atributi.Vrijednost("NEGATIVERED");
                if (neg != null && neg.Trim().ToLowerInvariant() == "true")
                    kol.NegativnoCrveno = true;
                string nz = def.Atributi.Vrijednost("NOTZERORED");
                if (nz != null && nz.Trim().ToLowerInvariant() == "true")
                    kol.NotZeroCrveno = true;
                string fg = def.Atributi.Vrijednost("FORECOLOR");
                if (!string.IsNullOrEmpty(fg) && fg.Trim().Length > 0 && Boja(fg) != null)
                    kol.BojaTeksta = Boja(fg);
                string bg = def.Atributi.Vrijednost("BACKCOLOR");
                if (!string.IsNullOrEmpty(bg) && bg.Trim().Length > 0 && Boja(bg) != null)
                    kol.BojaPozadine = Boja(bg);
                string ml = def.Atributi.Vrijednost("MAXLINES");
                int m;
                if (!string.IsNullOrEmpty(ml) && int.TryParse(ml.Trim(), out m) && m > 0)
                {
                    kol.MaxLinija = m;
                    kol.Prelomi = true;
                }
                if (!string.IsNullOrEmpty(grupa.HeaderAlignment))
                    kol.PoravnanjeZaglavlja = grupa.HeaderAlignment;
            }
        }

        private static string Boja(string ime)
        {
            switch ((ime ?? "").Trim().ToLowerInvariant())
            {
                case "green":
                case "white":
                case "black":
                case "darksalmon":
                case "darkseagreen":
                case "limegreen":
                case "greenyellow":
                case "yellow":
                case "lightyellow":
                case "red":
                case "brown":
                case "blue":
                case "ghostwhite":
                case "gray":
                case "orange":
                case "orangered":
                    return ime.Trim().ToLowerInvariant();
                default:
                    return null;
            }
        }

        private void Fokus(NgProject projekt)
        {
            Stanje.Redoslijed.Add("fokus");
            if (Stanje.Tablica == null || Stanje.Tablica.Rows.Count == 0)
            {
                Stanje.Id = "0";
                Stanje.FocusRazlog = "nema-redova";
                return;
            }
            Stanje.FocusRed = 0;
            Stanje.FocusRazlog = "prvi-red";
            int idCol = -1;
            for (int i = 0; i < Stanje.Tablica.Columns.Count; i++)
            {
                if (string.Equals(Stanje.Tablica.Columns[i].ColumnName, "id", StringComparison.OrdinalIgnoreCase))
                    idCol = i;
            }
            // Janus GetID: Cells("id"), inace Cells(2). U gridu su 0 ikona i 1 selector, pa je 2 prvi podatak.
            if (idCol < 0 && Stanje.Tablica.Columns.Count > 0)
                idCol = 0;
            if (idCol >= 0)
                Stanje.Id = Convert.ToString(Stanje.Tablica.Rows[0][idCol]);
            string move = Attr(projekt, "MASTERMOVELAST");
            if (move != null && move.Trim().ToLowerInvariant() == "true")
            {
                Stanje.FocusRed = Stanje.Tablica.Rows.Count - 1;
                Stanje.FocusRazlog = "MASTERMOVELAST";
                if (idCol >= 0)
                    Stanje.Id = Convert.ToString(Stanje.Tablica.Rows[Stanje.FocusRed][idCol]);
            }
            if (string.IsNullOrEmpty(Stanje.Id))
                Stanje.Id = "0";
        }

        private static string Attr(NgProject projekt, string ime)
        {
            if (projekt == null || !projekt.Atributi.Ima(ime))
                return null;
            return projekt.Atributi.Vrijednost(ime);
        }
    }

    public static class GridIzgled
    {
        /// <summary>
        /// Eksplicitne DX postavke. Ne prepuštaju se DevExpress defaultu.
        /// GroupByBox bez XML-a ostaje nepostavljen dok mjerenje Janusa ne kaže vrijednost.
        /// </summary>
        public static void Popuni(GridStanje s)
        {
            s.DxPostavke.Clear();
            s.DxPostavke.Add("OptionsView.ShowAutoFilterRow=" + (s.FilterRed ? "true" : "false"));
            s.DxPostavke.Add("OptionsView.ShowGroupPanel=" + (s.GroupByBox.HasValue ? (s.GroupByBox.Value ? "true" : "false") : "true"));
            s.DxPostavke.Add("OptionsView.ShowFooter=" + (s.TotalRed ? "true" : "false"));
            s.DxPostavke.Add("OptionsView.ColumnAutoWidth=false");
            s.DxPostavke.Add("OptionsView.ShowIndicator=true");
            s.DxPostavke.Add("OptionsSelection.MultiSelect=true");
            s.DxPostavke.Add("OptionsSelection.EnableAppearanceHideSelection=false");
            s.DxPostavke.Add("OptionsBehavior.AllowIncrementalSearch=true");
            s.DxPostavke.Add("OptionsBehavior.Editable=" + (s.AllowEditKadJeFilter ? "true" : "false"));
            s.DxPostavke.Add("OptionsCustomization.AllowSort=" + (s.SortKlikom ? "true" : "false"));
            s.DxPostavke.Add("OptionsCustomization.AllowFilter=false");
            s.DxPostavke.Add("BorderStyle=NoBorder");
            s.DxPostavke.Add("Enter=" + s.EnterTipka);
            s.DxPostavke.Add("Font=Microsoft Sans Serif, 8.25pt");
        }
    }
}
