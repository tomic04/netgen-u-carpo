using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
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
        /// <summary>Redoslijed prikaza nakon Janus Position premještanja (GridDefinicija). DGR-01.</summary>
        public int Redni;
        public bool Selektor;
        public bool Grupiraj;
        public string EditTip = "NoEdit";
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
        public bool FilterIzXml;
        public bool FilterNamjernoOdstupanje;
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
        public int HeaderLines = 3;
        public bool EmptyRows = true;
        public string EditMode = "EditOff";
        public string SelectionMode = "MultipleSelection";
        public string IncrementalSearch = "AllCharacters";
        public bool RowHeaders = true;
        public string HideSelection = "Highlight";
        public string GridDock = "Fill";
        public bool DxGrid;
        public string SqlGreska;
        /// <summary>Broj SQL greške (SqlException.Number) uz SqlGreska; RAISERROR procedure je &gt;= 50000 (PORUKA u sweepu).</summary>
        public int SqlGreskaBroj;
        public string Poruka;
        public string GroupCount = "[ ]";
        public string PocetniSort = "sql";
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
        public System.Collections.Generic.IDictionary<string, string> Clipboard;
        private NgProject _projekt;

        /// <summary>VB `/` je decimalno dijeljenje, pa se rezultat zaokruži na cijeli (CInt, bankarski).</summary>
        public static int VbJedinica(int jedinica)
        {
            return (int)Math.Round(jedinica / (double)FakPretvorbe, MidpointRounding.ToEven);
        }

        public void Ucitaj(NgProject projekt, DataTable tablica)
        {
            Stanje = new GridStanje();
            _projekt = projekt;
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
                PrimijeniAtribute(projekt);
                NgDnevnik.Akcija("otvara formu " + (Stanje.ImeForme ?? ""));
                return;
            }

            Stanje.IzvorSql = efektivni;
            string prvi = SqlTekst.Sredi(efektivni, Kontekst);
            Stanje.ZadnjiSql = prvi;
            string drugi = SrediPoslije.Petlja(prvi, "", Clipboard, false);
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
                if (tablica != null)
                    SloziKolone(projekt, tablica);
                else
                    PrimijeniAtribute(projekt);
                Fokus(projekt);
            }
            else
            {
                PrimijeniAtribute(projekt);
            }
            Stanje.Redoslijed.Add("kraj");
            OznaciDx(projekt);
            GridIzgled.Popuni(Stanje);
            ZabiljeziOdluku();
        }

        private void ZabiljeziOdluku()
        {
            if (Stanje.OtvaraFormu)
                NgDnevnik.Akcija("otvara formu " + (Stanje.ImeForme ?? ""));
            else if (Stanje.BlokiranoPisanje)
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + (Stanje.SqlZaSlanje ?? ""));
            else if (Stanje.Odluka != "salji")
                NgDnevnik.Preskok(Stanje.SqlZaSlanje);
        }

        /// <summary>
        /// Class1.ExecSql. Uspjeh puni grid. Greška pokaže poruku i ne veže stupce
        /// (Catch vraća Nothing, CreateGridColumns na Nothing odmah izađe).
        /// Greška ide u Carpo Logger (Ctrl+F11) i u spNetGenErrorLog preko ExecSql2 (timeout 100).
        /// </summary>
        public void IzvrsiCitanje(string connectionString)
        {
            if (Stanje.Odluka != "salji" || Stanje.BlokiranoPisanje)
                return;
            if (string.IsNullOrEmpty(connectionString))
            {
                ZabiljeziGresku("nema connection stringa");
                return;
            }
            NgDnevnik.Sql(Stanje.SqlZaSlanje);
            try
            {
                // COMMANDTIMEOUT projekta/forme (TMO-01); sweep ga ograniči kroz VrijemeUpita.Najvise (SWP-01).
                // Ista SQL sesija kao ostali upiti granule (SES-01).
                var dt = NgVeza.Citaj(connectionString, Stanje.SqlZaSlanje ?? "",
                    Carpo.NetGen.Komande.VrijemeUpita.Stvarno(Stanje.TimeoutSekundi));
                Stanje.Tablica = dt;
                Stanje.Kolone.Clear();
                Stanje.SqlGreska = null;
                Stanje.SqlGreskaBroj = 0;
                Stanje.Poruka = null;
                SloziKolone(_projekt, dt);
                Fokus(_projekt);
                Stanje.Redoslijed.Add("exec-ok");
            }
            catch (Exception ex)
            {
                int broj = 0;
                var sqlEx = ex as System.Data.SqlClient.SqlException;
                if (sqlEx != null)
                    broj = sqlEx.Number;
                GreskaSql.Zapisi(broj, ex.Message, Stanje.SqlZaSlanje, connectionString);
                if (TihaGreska(ex.Message))
                    ZabiljeziGresku(ex.Message, false);
                else
                    ZabiljeziGresku(ex.Message, true);
                Stanje.SqlGreskaBroj = broj;
            }
            GridIzgled.Popuni(Stanje);
        }

        public void ZabiljeziGresku(string poruka)
        {
            ZabiljeziGresku(poruka, true);
            GridIzgled.Popuni(Stanje);
        }

        private void ZabiljeziGresku(string poruka, bool prikazi)
        {
            Stanje.SqlGreska = poruka ?? "";
            Stanje.Poruka = prikazi ? ((poruka ?? "") + "\r\n\r\n" + (Stanje.SqlZaSlanje ?? "")) : null;
            if (prikazi)
                NgDnevnik.Greska((poruka ?? "") + " | " + (Stanje.SqlZaSlanje ?? ""));
            Stanje.Tablica = null;
            Stanje.Kolone.Clear();
            Stanje.Id = "0";
            Stanje.FocusRazlog = "sql-greska";
            Stanje.Redoslijed.Add("sql-greska");
        }

        private static bool TihaGreska(string poruka)
        {
            string m = (poruka ?? "").ToLowerInvariant();
            return m.IndexOf("ssoperateri", StringComparison.Ordinal) >= 0
                || m.IndexOf("totalgenlang", StringComparison.Ordinal) >= 0
                || m.IndexOf("sspermission", StringComparison.Ordinal) >= 0
                || m.IndexOf("netgendbfilter", StringComparison.Ordinal) >= 0
                || m.IndexOf("netgendbclipboard", StringComparison.Ordinal) >= 0;
        }

        private void OznaciDx(NgProject projekt)
        {
            string dx = Attr(projekt, "DXGRID");
            Stanje.DxGrid = dx != null && dx.Trim().ToLowerInvariant() == "true";
        }

        public void Osvjezi(DataTable tablica)
        {
            Stanje.Redoslijed.Add("REFRESHMASTERGRID");
            string drugi = SrediPoslije.Petlja(SqlTekst.Sredi(Stanje.ZadnjiSql ?? "", Kontekst), "", Clipboard, false).Replace("~~", ",");
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
            Stanje.Kolone.Add(new GridKolona { Podatak = "Icon", Natpis = "", Sirina = 22, Vidljiva = false, EditTip = "NoEdit" });
            Stanje.Kolone.Add(new GridKolona { Podatak = "Selector", Natpis = "", Sirina = 22, Selektor = true, Vidljiva = true, Uredivanje = true, EditTip = "CheckBox" });
            if (tablica != null)
            {
                foreach (DataColumn c in tablica.Columns)
                    Stanje.Kolone.Add(IzPodatka(c, EditStupciIzUpita ?? EditStupci(projekt)));
            }
            if (projekt != null)
            {
                PrimijeniAtribute(projekt);
                Stanje.Redoslijed.Add("custom-format");
                PrimijeniDefiniciju(projekt.NadjiGrupu("MASTERCOLUMNS"));
            }
        }

        /// <summary>
        /// Form1.vb 3468-3472: EDITCOLUMNS koji počinje sa sp/select izvrši podloga jednom pri čitanju XML-a,
        /// a stupci su prva ćelija rezultata. Null znači da se koristi tekst atributa.
        /// </summary>
        public string EditStupciIzUpita;

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
                // Class1.vb 1066, 1112, 1170: HeaderAlignment Far za boolean, int i decimal/double.
                k.PoravnanjeZaglavlja = "Far";
            }
            else if (tip.IndexOf("int", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 34;
                k.Format = "##0";
                k.Poravnanje = "Far";
                k.PoravnanjeZaglavlja = "Far";
            }
            else if (tip.IndexOf("decimal", StringComparison.Ordinal) >= 0 || tip.IndexOf("double", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 44;
                k.Format = "#,##0.00";
                k.Poravnanje = "Far";
                k.PoravnanjeZaglavlja = "Far";
                // Class1.vb 1171: AggregateFunction.Sum (Janus 2) u ukupnom retku (TOT-01).
                k.Agregat = "2";
            }
            else if (tip.IndexOf("date", StringComparison.Ordinal) >= 0)
            {
                k.Sirina = 100;
                k.Format = "dd.MM.yyyy";
            }
            else if (tip.IndexOf("string", StringComparison.Ordinal) >= 0)
            {
                // CreateGridColumns 1203. 200 je samo hijerarhijski put (Form1.vb 1799).
                k.Sirina = 100;
            }

            string cap = (k.Natpis ?? "").Trim();
            if (cap.Length >= 4)
            {
                int n;
                if (int.TryParse(cap.Substring(cap.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n != 0)
                {
                    k.Sirina = VbJedinica(n);
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
            {
                k.Uredivanje = true;
                k.EditTip = tip.IndexOf("bool", StringComparison.Ordinal) >= 0 ? "CheckBox" : "TextBox";
            }
            return k;
        }

        private void PrimijeniAtribute(NgProject projekt)
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
            if (fm == null)
            {
                // Carpo: ShowAutoFilterRow je uključen osim ako XML eksplicitno kaže false.
                // Janus FilterMode je None. Filter red ne mijenja SQL ni podatke.
                Stanje.FilterRed = true;
                Stanje.FilterNamjernoOdstupanje = true;
            }
            else
            {
                Stanje.FilterIzXml = true;
                Stanje.FilterNamjernoOdstupanje = false;
                Stanje.FilterRed = fm.Trim().ToLowerInvariant() == "true";
                if (Stanje.FilterRed)
                {
                    string ed = Attr(projekt, "EDITCOLUMNS");
                    Stanje.AllowEditKadJeFilter = ed != null && ed.Trim().Length > 0;
                }
            }
            string timeout = Attr(projekt, "COMMANDTIMEOUT");
            int ts;
            if (timeout != null && int.TryParse(timeout.Trim(), out ts))
                Stanje.TimeoutSekundi = ts;
            if (!projekt.Atributi.Ima("EDITCOLUMNS") || string.IsNullOrEmpty(projekt.Atributi.Vrijednost("EDITCOLUMNS")))
                Stanje.EnterTipka = "None";
            if (projekt.Atributi.Ima("ENTERKEYBEHAVIOR"))
                Stanje.EnterTipka = (projekt.Atributi.Vrijednost("ENTERKEYBEHAVIOR") ?? "").Trim();
        }

        /// <summary>Form1.CustomFormatKolona (7451) za MASTERCOLUMNS. HEADERALIGNMENT ide samo na opisane stupce.</summary>
        private void PrimijeniDefiniciju(NgColumnGroup grupa)
        {
            if (grupa == null)
                return;
            bool clear = grupa.ClearColums != null && grupa.ClearColums.ToLowerInvariant() == "true";
            GridDefinicija.Primijeni(Stanje.Kolone, grupa.Kolone, clear, grupa.HeaderAlignment, false);
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
            s.DxPostavke.Add("OptionsFilter.AllowAutoFilterConditionChange=false");
            s.DxPostavke.Add("OptionsView.ShowGroupPanel=" + (s.GroupByBox.HasValue ? (s.GroupByBox.Value ? "true" : "false") : "true"));
            s.DxPostavke.Add("OptionsView.ShowFooter=" + (s.TotalRed ? "true" : "false"));
            s.DxPostavke.Add("OptionsView.ColumnAutoWidth=false");
            s.DxPostavke.Add("OptionsView.ShowIndicator=true");
            s.DxPostavke.Add("OptionsSelection.MultiSelect=true");
            s.DxPostavke.Add("OptionsSelection.EnableAppearanceHideSelection=false");
            s.DxPostavke.Add("OptionsBehavior.AllowIncrementalSearch=true");
            s.DxPostavke.Add("OptionsBehavior.Editable=" + (s.AllowEditKadJeFilter ? "true" : "false"));
            s.DxPostavke.Add("EditMode=" + s.EditMode);
            s.DxPostavke.Add("HeaderLines=" + s.HeaderLines.ToString(CultureInfo.InvariantCulture));
            s.DxPostavke.Add("EmptyRows=" + (s.EmptyRows ? "true" : "false"));
            s.DxPostavke.Add("SelectionMode=" + s.SelectionMode);
            s.DxPostavke.Add("IncrementalSearch=" + s.IncrementalSearch);
            s.DxPostavke.Add("HideSelection=" + s.HideSelection);
            s.DxPostavke.Add("RowHeaders=" + (s.RowHeaders ? "true" : "false"));
            s.DxPostavke.Add("Dock=" + s.GridDock);
            s.DxPostavke.Add("OptionsView.NewItemRowPosition=None");
            s.DxPostavke.Add("OptionsCustomization.AllowSort=" + (s.SortKlikom ? "true" : "false"));
            s.DxPostavke.Add("OptionsCustomization.AllowFilter=false");
            s.DxPostavke.Add("BorderStyle=NoBorder");
            s.DxPostavke.Add("Enter=" + s.EnterTipka);
            s.DxPostavke.Add("Font=Microsoft Sans Serif, 8.25pt");
        }
    }
}
