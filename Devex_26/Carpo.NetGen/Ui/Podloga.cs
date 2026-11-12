using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Print;
using Carpo.NetGen.Xml;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// NetGen podloga na gotovom rasporedu Carpo Managera (Manager ponuda):
    /// RibbonBase preko JupiterManager, lijevo combo i stablo, desno traka i master,
    /// ispod detail, dolje flat gumbi, status Broj slogova.
    /// </summary>
    public sealed class Podloga : Carpo.UI.Forms.Master.Manager
    {
        private readonly Stablo _stablo = new Stablo();
        private readonly MasterGrid _master = new MasterGrid();
        private readonly DetailGrid _detail = new DetailGrid();
        private NgProject _projekt;
        private string _spoj = "";
        private string _server = "";
        private string _sqlGreska = "";
        private bool _punim;
        private int _redovaDetail;
        private DataTable _comboRedovi;
        private readonly System.Collections.Generic.Dictionary<string, string> _polja = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Podloga()
        {
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            Text = "NetGen";
            WindowState = FormWindowState.Maximized;
            KeyPreview = true;
            if (ribbonControl1 != null)
            {
                ribbonControl1.Pages.Clear();
                ribbonControl1.ShowApplicationButton = DefaultBoolean.False;
                ribbonControl1.ShowPageHeadersMode = ShowPageHeadersMode.Hide;
                ribbonControl1.Toolbar.ShowCustomizeItem = false;
            }
            PodesiPogled(gvGla, true);
            PodesiPogled(gvDet, false);
            gvGla.FocusedRowChanged += Pogled_FocusedRowChanged;
            gvGla.DoubleClick += Pogled_DoubleClick;
            trMain.OptionsView.ShowColumns = false;
            trMain.OptionsView.ShowIndicator = false;
            trMain.OptionsBehavior.Editable = false;
            trMain.FocusedNodeChanged += Drvo_FocusedNodeChanged;
            var stavke = new XtraTabPage();
            stavke.Text = "Stavke";
            splitGrid.Panel2.Controls.Remove(gcDet);
            stavke.Controls.Add(gcDet);
            var tab = new XtraTabControl();
            tab.Dock = DockStyle.Fill;
            tab.TabPages.Add(stavke);
            splitGrid.Panel2.Controls.Add(tab);
            bbiLookUpFilter.EditValueChanged += Combo_EditValueChanged;
            bbiLookUpFilter.Visibility = BarItemVisibility.Never;
            bbiBrojSlogova.Caption = "Broj slogova:";
            bbiBrojSlogovaMasterGrid.Caption = "master grid: <b>0</b>";
            bbiBrojSlogovaDetailsGrid.Caption = "detalj grid: <b>0</b>";
        }

        private static void PodesiPogled(GridView pogled, bool master)
        {
            pogled.OptionsBehavior.Editable = false;
            pogled.OptionsBehavior.ReadOnly = true;
            pogled.OptionsView.ShowAutoFilterRow = true;
            pogled.OptionsView.ColumnAutoWidth = false;
            pogled.OptionsView.ShowGroupPanel = true;
            pogled.OptionsSelection.MultiSelect = true;
            pogled.OptionsSelection.EnableAppearanceFocusedCell = false;
            pogled.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            pogled.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            if (master)
                pogled.OptionsView.ShowIndicator = true;
            CarpoIzgled.Pogled(pogled);
        }

        public bool Prikazi(string xml, string server, string baza, int operater)
        {
            var r = NetGenXmlReader.Ucitaj(xml);
            if (!r.Uspjeh || r.Projekt == null)
            {
                PostaviStatus(r.Greska ?? "XML nije učitan.");
                return false;
            }
            _projekt = r.Projekt;
            _server = server ?? "";
            Text = string.IsNullOrEmpty(_projekt.Naslov) ? "NetGen" : _projekt.Naslov;
            bbiVerzija.Caption = (server ?? "") + " · " + (baza ?? "") + " · " + operater.ToString();
            var kontekst = new SqlKontekst();
            kontekst.OperaterId = operater.ToString();
            kontekst.XmlName = Path.GetFileName(xml ?? "");
            kontekst.Node = "null";
            _stablo.Kontekst = kontekst;
            _stablo.ImeXml = kontekst.XmlName;
            _master.Kontekst = kontekst;
            _detail.Kontekst = kontekst;
            try
            {
                _spoj = SpinVeza.Spoj(server, baza);
            }
            catch (Exception ex)
            {
                PostaviStatus(ex.Message);
                return false;
            }

            _stablo.Ucitaj(_projekt, Citaj);
            NapuniCombo();
            NapuniDrvo();
            _detail.Pripremi(_projekt);
            _master.Ucitaj(_projekt, null);
            NapuniGrid();
            NapuniGumbe();
            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F11))
            {
                OtvoriCarpoLog();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NapuniDrvo()
        {
            trMain.BeginUnboundLoad();
            trMain.Nodes.Clear();
            for (int i = 0; i < _stablo.Stanje.Korijen.Count; i++)
                DodajCvor(_stablo.Stanje.Korijen[i], null);
            trMain.EndUnboundLoad();
            trMain.ExpandAll();
        }

        private void DodajCvor(Cvor cvor, TreeListNode roditelj)
        {
            TreeListNode n = trMain.AppendNode(new object[] { cvor.Natpis ?? "" }, roditelj);
            n.ImageIndex = 0;
            n.SelectImageIndex = 0;
            n.Tag = cvor;
            for (int i = 0; i < cvor.Djeca.Count; i++)
                DodajCvor(cvor.Djeca[i], n);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!_stablo.Stanje.Vidljivo || _stablo.Stanje.Splitter <= 0)
                splitMain.PanelVisibility = SplitPanelVisibility.Panel2;
            else
            {
                splitMain.PanelVisibility = SplitPanelVisibility.Both;
                int d = _stablo.Stanje.Splitter;
                if (d > 40 && d < Width - 120)
                    splitMain.SplitterPosition = d;
            }
            if (_detail.Stanje.Nema || _detail.Stanje.OtvaraFormu)
                splitGrid.PanelVisibility = SplitPanelVisibility.Panel1;
            else
            {
                splitGrid.PanelVisibility = SplitPanelVisibility.Both;
                int visina = splitGrid.Height;
                if (visina > 160)
                    splitGrid.SplitterPosition = (int)(visina * 0.62);
            }
            if (_master.Stanje.OtvaraFormu)
                IzvrsiLanac(_master.Stanje.ImeForme ?? "");
        }

        private void NapuniCombo()
        {
            bbiLookUpFilter.Visibility = BarItemVisibility.Never;
            cmbComboEditFilter.Items.Clear();
            _comboRedovi = null;
            if (_projekt == null || _projekt.Atributi == null || !_projekt.Atributi.Ima("COMBO"))
                return;
            string sql = (_projekt.Atributi.Vrijednost("COMBO") ?? "").Trim();
            if (sql.Length == 0 || SqlTekst.JeZabranaPisanja(sql))
                return;
            DataTable t = Citaj(SqlTekst.Sredi(sql, _master.Kontekst));
            if (t == null || t.Columns.Count == 0 || t.Rows.Count == 0)
                return;
            _comboRedovi = t;
            int prikaz = t.Columns.Count > 1 ? 1 : 0;
            for (int i = 0; i < t.Rows.Count; i++)
                cmbComboEditFilter.Items.Add(Convert.ToString(t.Rows[i][prikaz]));
            bbiLookUpFilter.Visibility = BarItemVisibility.Always;
            bbiLookUpFilter.EditValue = Convert.ToString(t.Rows[0][prikaz]);
        }

        private void Combo_EditValueChanged(object sender, EventArgs e)
        {
            if (bbiLookUpFilter.EditValue == null || _comboRedovi == null)
                return;
            string prikaz = Convert.ToString(bbiLookUpFilter.EditValue);
            int stupac = _comboRedovi.Columns.Count > 1 ? 1 : 0;
            string vrijednost = "";
            for (int i = 0; i < _comboRedovi.Rows.Count; i++)
            {
                if (Convert.ToString(_comboRedovi.Rows[i][stupac]) == prikaz)
                {
                    vrijednost = Convert.ToString(_comboRedovi.Rows[i][0]);
                    break;
                }
            }
            if (vrijednost.Length == 0)
                return;
            _stablo.IzTeksta(vrijednost, _projekt, Citaj);
            NapuniDrvo();
            if (_stablo.Stanje.Vidljivo && _stablo.Stanje.Splitter > 0)
            {
                splitMain.PanelVisibility = SplitPanelVisibility.Both;
                int d = _stablo.Stanje.Splitter;
                if (d > 40 && d < Width - 120)
                    splitMain.SplitterPosition = d;
            }
        }

        private void Drvo_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            var cvor = e.Node == null ? null : e.Node.Tag as Cvor;
            if (cvor == null)
                return;
            _stablo.Odaberi(cvor);
            string naredba = (cvor.Naredba ?? "").Trim();
            string sqlNaredba = naredba;
            if (sqlNaredba.StartsWith("==", StringComparison.Ordinal))
                sqlNaredba = sqlNaredba.Substring(2).Trim();
            bool jeSql = sqlNaredba.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                || sqlNaredba.StartsWith("sp", StringComparison.OrdinalIgnoreCase)
                || sqlNaredba.StartsWith("exec", StringComparison.OrdinalIgnoreCase);
            if (jeSql)
                IzvrsiSql(sqlNaredba);
            else
            {
                string master = _projekt == null ? "" : (_projekt.OnLoad ?? "");
                if (master.IndexOf("@@node", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _master.Ucitaj(_projekt, null);
                    NapuniGrid();
                }
                OtvoriFormu(naredba);
            }
        }

        private int _dubinaForme;

        private void OtvoriFormu(string naredba)
        {
            if (string.IsNullOrWhiteSpace(naredba) || _projekt == null || _dubinaForme > 8)
                return;
            string trazeno = naredba.Trim();
            if (trazeno.StartsWith(":", StringComparison.Ordinal))
                trazeno = trazeno.Substring(1).Trim();
            int zagrada = trazeno.IndexOf('(');
            string ime = zagrada > 0 ? trazeno.Substring(0, zagrada).Trim() : trazeno;
            if (Lanac.OsvjezavaSegment(ime))
            {
                OsvjeziMaster();
                return;
            }
            NgForm forma = null;
            for (int i = 0; i < _projekt.Forme.Count; i++)
            {
                if (string.Equals(_projekt.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                {
                    forma = _projekt.Forme[i];
                    break;
                }
            }
            if (forma == null)
            {
                NgDnevnik.Akcija("nema forme " + ime);
                OsvjeziMaster();
                return;
            }
            _dubinaForme++;
            try
            {
                var dij = new Dijalog();
                dij.JupiterNeSaljeGumb(DijalogTok.JeJupiter(_server));
                dij.OtvoriDrugu = OtvoriFormu;
                dij.Prikazi(forma, SqlTekst.ZaFormu(_master.Kontekst), Citaj);
                string iz = dij.Vrijednost("izvjestaj");
                if (!string.IsNullOrEmpty(iz))
                    _polja["izvjestaj"] = iz;
                if (dij.DialogResult != DialogResult.OK)
                    return;
                string sljedeca = DijalogTok.NextForm(forma);
                if (sljedeca.Length > 0)
                    OtvoriFormu(sljedeca);
                else
                    OsvjeziMaster();
            }
            finally
            {
                _dubinaForme--;
            }
        }

        private void OsvjeziMaster()
        {
            if (DijalogTok.JeJupiter(_server) || _projekt == null)
                return;
            _master.Ucitaj(_projekt, null);
            NapuniGrid();
        }

        private void IzvrsiLanac(string naredba)
        {
            string[] seg = Lanac.Segmenti(naredba ?? "");
            for (int i = 0; i < seg.Length; i++)
            {
                string s = (seg[i] ?? "").Trim();
                if (s.StartsWith(":", StringComparison.Ordinal))
                    s = s.Substring(1).Trim();
                if (s.Length == 0)
                    continue;
                if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    string stanje = Lanac.GumbIzCase(s, Polje("izvjestaj"));
                    int jednako = stanje.IndexOf('=');
                    if (jednako > 0)
                        PostaviGumb(stanje.Substring(0, jednako), stanje.Substring(jednako + 1) != "disable");
                }
                else if (IspisMotora.JeIspisImena(s) || s.StartsWith("PRINT", StringComparison.OrdinalIgnoreCase))
                    NgDnevnik.Akcija("ispis " + s);
                else if (s.ToUpperInvariant().IndexOf("ASSEMBLYLOAD(", StringComparison.Ordinal) >= 0)
                    NgDnevnik.Akcija("assembly " + s);
                else
                    OtvoriFormu(s);
            }
        }

        private string Polje(string ime)
        {
            string v;
            if (_polja.TryGetValue(ime, out v))
                return v ?? "";
            return "";
        }

        private void PostaviGumb(string natpis, bool ukljucen)
        {
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                var gumb = pnlButtons.Controls[i] as SimpleButton;
                if (gumb == null)
                    continue;
                string t = (gumb.Text ?? "").Replace("\r\n", "").Trim();
                if (string.Equals(t, natpis, StringComparison.OrdinalIgnoreCase))
                    gumb.Enabled = ukljucen;
            }
        }

        private void Pogled_DoubleClick(object sender, EventArgs e)
        {
            if (_projekt == null || string.IsNullOrWhiteSpace(_projekt.MasterDblClick))
                return;
            IzvrsiLanac(_projekt.MasterDblClick);
        }

        private void Pogled_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (_punim)
                return;
            SpojiId();
            OsvjeziDetail();
        }

        private void SpojiId()
        {
            string id = "0";
            DataRow row = gvGla.GetFocusedDataRow();
            if (row != null)
            {
                if (row.Table.Columns.Contains("id"))
                    id = Convert.ToString(row["id"]);
                else if (row.Table.Columns.Count > 0)
                    id = Convert.ToString(row[0]);
                if (string.IsNullOrEmpty(id))
                    id = "0";
            }
            else if (!string.IsNullOrEmpty(_master.Stanje.Id))
                id = _master.Stanje.Id;
            _master.Stanje.Id = id;
            _master.Kontekst.Id = id;
        }

        private void NapuniGrid()
        {
            _punim = true;
            _master.IzvrsiCitanje(_spoj);
            VeziGrid(gvGla, gcGla, _master.Stanje.Tablica, _master.Stanje.Kolone, _master.Stanje.Poruka, _master.Stanje.CheckboxVidljiv, _master.Stanje.GroupByBox, _master.Stanje.FilterRed || _master.Stanje.FilterNamjernoOdstupanje || !_master.Stanje.FilterIzXml);
            SpojiId();
            _punim = false;
            OsvjeziDetail();
        }

        private void IzvrsiSql(string sql)
        {
            string spremno = SqlTekst.Sredi(sql, _master.Kontekst).Replace("~~", ",");
            if (SqlTekst.JeZabranaPisanja(spremno))
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + spremno);
                PostaviStatus("Upis nije poslan.");
                return;
            }
            NgDnevnik.Sql(spremno);
            DataTable t = Citaj(spremno);
            gcGla.DataSource = t;
            PostaviStatus(t == null ? "Greška SQL-a." : (t.Rows.Count.ToString() + " redova"));
            OsvjeziDetail();
        }

        private void OsvjeziDetail()
        {
            if (_detail.Stanje.Nema || _detail.Stanje.OtvaraFormu)
            {
                gcDet.DataSource = null;
                return;
            }
            DataRow red = gvGla.GetFocusedDataRow();
            if (red == null && _master.Stanje.Tablica != null && _master.Stanje.Tablica.Rows.Count > 0)
                red = _master.Stanje.Tablica.Rows[0];
            string sql = _detail.Slozi(red);
            if (string.IsNullOrWhiteSpace(sql))
                return;
            if (SqlTekst.JeZabranaPisanja(sql))
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + sql);
                return;
            }
            NgDnevnik.Sql(sql);
            DataTable t = Citaj(sql);
            NgColumnGroup def = _projekt == null ? null : _projekt.NadjiGrupu("DETAILCOLUMNS");
            _detail.PrimijeniTablicu(t, def);
            VeziGrid(gvDet, gcDet, t, _detail.Stanje.Kolone, _detail.Stanje.Poruka, _detail.Stanje.CheckboxVidljiv, null, true);
            if (_detail.Stanje.FokusZadnji && gvDet.RowCount > 0)
                gvDet.FocusedRowHandle = gvDet.RowCount - 1;
        }

        private void VeziGrid(GridView pogled, GridControl grid, DataTable tablica, System.Collections.Generic.List<GridKolona> kolone, string poruka, bool checkbox, bool? grupa, bool filter)
        {
            grid.DataSource = tablica;
            pogled.PopulateColumns();
            if (grupa.HasValue)
                pogled.OptionsView.ShowGroupPanel = grupa.Value;
            pogled.OptionsView.ShowAutoFilterRow = filter;
            if (checkbox)
                pogled.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            if (!string.IsNullOrEmpty(poruka))
            {
                PostaviStatus(poruka.Replace("\r\n", " "));
                return;
            }
            int redova = tablica == null ? 0 : tablica.Rows.Count;
            if (pogled == gvDet)
                _redovaDetail = redova;
            else
                PostaviStatus(redova.ToString());
            if (kolone == null)
                return;
            for (int i = 0; i < kolone.Count; i++)
            {
                GridKolona k = kolone[i];
                if (k.Selektor || k.Podatak == "Icon")
                    continue;
                var c = pogled.Columns.ColumnByFieldName(k.Podatak);
                if (c == null)
                    continue;
                c.Visible = k.Vidljiva;
                c.Caption = k.Natpis ?? k.Podatak;
                if (k.Sirina > 0)
                    c.Width = k.Sirina;
                c.OptionsColumn.AllowEdit = k.Uredivanje;
                if (!string.IsNullOrEmpty(k.Format))
                {
                    c.DisplayFormat.FormatString = k.Format;
                    c.DisplayFormat.FormatType = k.Format.IndexOf("yy", System.StringComparison.OrdinalIgnoreCase) >= 0
                        || k.Format.IndexOf("dd", System.StringComparison.OrdinalIgnoreCase) >= 0
                        ? DevExpress.Utils.FormatType.DateTime
                        : DevExpress.Utils.FormatType.Numeric;
                }
                if (string.Equals(k.Poravnanje, "Far", System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(k.Poravnanje, "Right", System.StringComparison.OrdinalIgnoreCase))
                    c.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else if (string.Equals(k.Poravnanje, "Center", System.StringComparison.OrdinalIgnoreCase))
                    c.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                if (string.Equals(k.PoravnanjeZaglavlja, "Far", System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(k.PoravnanjeZaglavlja, "Right", System.StringComparison.OrdinalIgnoreCase))
                    c.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else if (string.Equals(k.PoravnanjeZaglavlja, "Center", System.StringComparison.OrdinalIgnoreCase))
                    c.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                else if (string.Equals(k.PoravnanjeZaglavlja, "Near", System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(k.PoravnanjeZaglavlja, "Left", System.StringComparison.OrdinalIgnoreCase))
                    c.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            }
            if (pogled.RowCount > 0)
                pogled.FocusedRowHandle = 0;
        }

        private void NapuniGumbe()
        {
            pnlButtons.Controls.Clear();
            if (_projekt == null)
                return;
            for (int i = _projekt.Gumbi.Count - 1; i >= 0; i--)
            {
                NgButton b = _projekt.Gumbi[i];
                var gumb = new SimpleButton();
                gumb.Text = (b.Natpis ?? "").Replace("!!", "\r\n");
                gumb.Tag = b.Naredba;
                gumb.Dock = DockStyle.Left;
                gumb.Width = Math.Max(90, gumb.Text.Replace("\r\n", "").Length * 8);
                CarpoIzgled.Gumb(gumb);
                gumb.Click += Gumb_Click;
                pnlButtons.Controls.Add(gumb);
            }
        }

        private void Gumb_Click(object sender, EventArgs e)
        {
            var gumb = sender as SimpleButton;
            string naredba = gumb == null || gumb.Tag == null ? "" : gumb.Tag.ToString();
            NgDnevnik.Akcija("gumb " + (gumb == null ? "" : gumb.Text) + " " + naredba);
            IzvrsiLanac(naredba);
        }

        private void PostaviStatus(string tekst)
        {
            int n;
            if (int.TryParse(tekst, out n))
                bbiBrojSlogovaMasterGrid.Caption = "master grid: <b>" + n.ToString() + "</b>";
            else
                bbiBrojSlogovaMasterGrid.Caption = tekst ?? "";
            bbiBrojSlogovaDetailsGrid.Caption = "detalj grid: <b>" + _redovaDetail.ToString() + "</b>";
        }

        private DataTable Citaj(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql) || SqlTekst.JeZabranaPisanja(sql))
                return null;
            try
            {
                var dt = new DataTable();
                using (var conn = new System.Data.SqlClient.SqlConnection(_spoj))
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 30;
                    conn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                _sqlGreska = ex.Message;
                NgDnevnik.Greska(ex.Message + " | " + sql);
                PostaviStatus(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Klik na podlozi. Upis i fiskalizacija se ne šalju. Dijalog se zatvara kao Button2.
        /// </summary>
        public List<string> ProvediKlikove()
        {
            var redovi = new List<string>();
            string granula = _projekt == null ? Text : (_projekt.Naslov ?? Text);
            int master = _master.Stanje.Tablica == null ? 0 : _master.Stanje.Tablica.Rows.Count;
            bool sqlMaster = MasterPuni();
            if (!string.IsNullOrEmpty(_sqlGreska))
                redovi.Add(Linija(granula, "master", "grid", "GRESKA", _sqlGreska, "netgen/Form1.vb:3882"));
            else if (sqlMaster && master == 0)
                redovi.Add(Linija(granula, "master", "grid", "PRAZAN", "NetGen puni master, grid je prazan", "netgen/Form1.vb:3882"));
            else
                redovi.Add(Linija(granula, "master", "grid", "OK", master.ToString() + " redova", "netgen/Form1.vb:3882"));

            if (!_detail.Stanje.Nema && !_detail.Stanje.OtvaraFormu)
            {
                int det = _detail.Stanje.Tablica == null ? 0 : _detail.Stanje.Tablica.Rows.Count;
                if (!string.IsNullOrEmpty(_sqlGreska) && det == 0)
                    redovi.Add(Linija(granula, "detail", "grid", "GRESKA", _sqlGreska, "netgen/Form1.vb:2258"));
                else if (master > 0 && det == 0)
                    redovi.Add(Linija(granula, "detail", "grid", "PRAZAN", "master ima red, detail je prazan", "netgen/Form1.vb:2258"));
                else
                    redovi.Add(Linija(granula, "detail", "grid", "OK", det.ToString() + " redova", "netgen/Form1.vb:2258"));
            }

            var tab = NadjiTab();
            if (tab != null)
            {
                for (int i = 0; i < tab.TabPages.Count; i++)
                {
                    try
                    {
                        tab.SelectedTabPageIndex = i;
                        redovi.Add(Linija(granula, "tab", tab.TabPages[i].Text, "OK", "", "netgen/Form1.vb:2258"));
                    }
                    catch (Exception ex)
                    {
                        redovi.Add(Linija(granula, "tab", tab.TabPages[i].Text, "GRESKA", ex.Message, "netgen/Form1.vb:2258"));
                    }
                }
            }

            if (trMain.Nodes.Count > 0)
            {
                _sqlGreska = "";
                try
                {
                    PokreniZatvaranje();
                    trMain.FocusedNode = trMain.Nodes[0];
                    Application.DoEvents();
                    if (!string.IsNullOrEmpty(_sqlGreska))
                        redovi.Add(Linija(granula, "stablo", "cvor", "GRESKA", _sqlGreska, "netgen/Form1.vb:7426"));
                    else
                        redovi.Add(Linija(granula, "stablo", "cvor", "OK", "prvi cvor", "netgen/Form1.vb:7426"));
                }
                catch (Exception ex)
                {
                    redovi.Add(Linija(granula, "stablo", "cvor", "GRESKA", ex.Message, "netgen/Form1.vb:7426"));
                }
            }

            var gumbi = new List<SimpleButton>();
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                var g = pnlButtons.Controls[i] as SimpleButton;
                if (g != null)
                    gumbi.Add(g);
            }
            for (int i = 0; i < gumbi.Count; i++)
            {
                SimpleButton g = gumbi[i];
                string naredba = g.Tag == null ? "" : g.Tag.ToString();
                string natpis = (g.Text ?? "").Replace("\r\n", " ").Trim();
                if (Destruktivan(naredba, natpis))
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", naredba, "netgen/Form1.vb:5664"));
                    continue;
                }
                _sqlGreska = "";
                try
                {
                    PokreniZatvaranje();
                    g.PerformClick();
                    Application.DoEvents();
                    if (!string.IsNullOrEmpty(_sqlGreska))
                        redovi.Add(Linija(granula, "gumb", natpis, "GRESKA", _sqlGreska, "netgen/Form1.vb:5664"));
                    else
                        redovi.Add(Linija(granula, "gumb", natpis, "OK", naredba, "netgen/Form1.vb:5664"));
                }
                catch (Exception ex)
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "GRESKA", ex.Message, "netgen/Form1.vb:5664"));
                }
            }
            ZaustaviZatvaranje();
            return redovi;
        }

        private bool _zatvaraj;

        private void PokreniZatvaranje()
        {
            if (_zatvaraj)
                return;
            _zatvaraj = true;
            var nit = new Thread(ZatvarajDijaloge);
            nit.IsBackground = true;
            nit.Start();
        }

        private void ZaustaviZatvaranje()
        {
            _zatvaraj = false;
        }

        private void ZatvarajDijaloge()
        {
            while (_zatvaraj)
            {
                Thread.Sleep(200);
                Form[] otvorene;
                try
                {
                    otvorene = new Form[Application.OpenForms.Count];
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                        otvorene[i] = Application.OpenForms[i];
                }
                catch
                {
                    continue;
                }
                for (int i = 0; i < otvorene.Length; i++)
                {
                    Form f = otvorene[i];
                    if (f == null || f == this || f.IsDisposed || !(f is Dijalog))
                        continue;
                    try
                    {
                        f.BeginInvoke(new Action(delegate ()
                        {
                            if (!f.IsDisposed)
                            {
                                f.DialogResult = DialogResult.Cancel;
                                f.Close();
                            }
                        }));
                    }
                    catch
                    {
                    }
                }
            }
        }

        private bool MasterPuni()
        {
            if (_projekt == null)
                return false;
            string on = (_projekt.OnLoad ?? "").Trim();
            if (on.Length > 0 && !on.StartsWith(":", StringComparison.Ordinal))
                return true;
            return !string.IsNullOrWhiteSpace(_master.Stanje.IzvorSql);
        }

        private bool Destruktivan(string naredba, string natpis)
        {
            if (Pise(naredba) || Pise(natpis))
                return true;
            if (naredba != null && naredba.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (_projekt == null || string.IsNullOrEmpty(naredba))
                return false;
            string ime = naredba.Trim();
            int hash = ime.IndexOf('#');
            if (hash >= 0)
                ime = ime.Substring(0, hash);
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1);
            for (int i = 0; i < _projekt.Forme.Count; i++)
            {
                if (!string.Equals(_projekt.Forme[i].Ime, ime.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;
                if (Pise(_projekt.Forme[i].OnSubmit) || Pise(_projekt.Forme[i].OnLoad))
                    return true;
            }
            return false;
        }

        private static bool Pise(string tekst)
        {
            if (string.IsNullOrEmpty(tekst))
                return false;
            return ImaRijec(tekst, "insert") || ImaRijec(tekst, "update") || ImaRijec(tekst, "delete")
                || tekst.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool ImaRijec(string tekst, string rijec)
        {
            string t = tekst.ToLowerInvariant();
            int i = 0;
            while (i <= t.Length - rijec.Length)
            {
                int n = t.IndexOf(rijec, i, StringComparison.Ordinal);
                if (n < 0)
                    return false;
                bool lijevo = n == 0 || !char.IsLetter(t[n - 1]);
                int kraj = n + rijec.Length;
                bool desno = kraj >= t.Length || !char.IsLetter(t[kraj]);
                if (lijevo && desno)
                    return true;
                i = n + 1;
            }
            return false;
        }

        private XtraTabControl NadjiTab()
        {
            for (int i = 0; i < splitGrid.Panel2.Controls.Count; i++)
            {
                var t = splitGrid.Panel2.Controls[i] as XtraTabControl;
                if (t != null)
                    return t;
            }
            return null;
        }

        private static string Linija(string granula, string dio, string ime, string ishod, string poruka, string izvor)
        {
            return granula + "\t" + dio + "\t" + ime + "\t" + ishod + "\t" + (poruka ?? "").Replace("\t", " ").Replace("\r", " ").Replace("\n", " ") + "\t" + izvor;
        }

        private void OtvoriCarpoLog()
        {
            object logger = NgDnevnik.CarpoLogger;
            if (logger == null)
            {
                PostaviStatus("Carpo logger nije spojen.");
                return;
            }
            try
            {
                Type tip = logger.GetType().Assembly.GetType("Carpo.Core.Log.Log");
                if (tip == null)
                {
                    PostaviStatus("Carpo.Core.Log.Log nije pronađen.");
                    return;
                }
                object frm = Activator.CreateInstance(tip, logger);
                MethodInfo show = tip.GetMethod("ShowDialog", new[] { typeof(IWin32Window) });
                if (show != null)
                    show.Invoke(frm, new object[] { this });
                else
                    tip.GetMethod("ShowDialog", Type.EmptyTypes).Invoke(frm, null);
            }
            catch (Exception ex)
            {
                PostaviStatus("Ctrl+F11: " + ex.Message);
            }
        }
    }
}
