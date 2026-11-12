using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;
using DevExpress.Skins;
using DevExpress.UserSkins;
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
    /// NetGen podloga u Carpo ljusci: RibbonBase, ribbon gumbi, DevExpress grid i stablo.
    /// Raspored i naredbe ostaju NetGen. Boja i stil gumba više nisu WinForms.
    /// </summary>
    public sealed class Podloga : RibbonBase.RibbonBase
    {
        private readonly SplitContainerControl _split;
        private readonly SplitContainerControl _detailSplit;
        private readonly XtraTabControl _tabLijevo;
        private readonly XtraTabControl _tabDetalj;
        private readonly TreeList _drvo;
        private readonly LookUpEdit _combo;
        private readonly GridControl _grid;
        private readonly GridView _pogled;
        private readonly GridControl _detailGrid;
        private readonly GridView _detailPogled;
        private readonly RibbonPageGroup _naredbe;
        private readonly BarStaticItem _status;
        private readonly BarStaticItem _veza;
        private readonly Stablo _stablo = new Stablo();
        private readonly MasterGrid _master = new MasterGrid();
        private readonly DetailGrid _detail = new DetailGrid();
        private NgProject _projekt;
        private string _spoj = "";
        private bool _punim;

        public Podloga()
        {
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            Text = "NetGen";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;

            var stranica = new RibbonPage("Početna");
            _naredbe = new RibbonPageGroup("Naredbe");
            _naredbe.ShowCaptionButton = false;
            stranica.Groups.Add(_naredbe);
            if (ribbonControl1 != null)
            {
                ribbonControl1.Pages.Add(stranica);
                ribbonControl1.Toolbar.ShowCustomizeItem = false;
            }

            _status = new BarStaticItem();
            _status.Caption = "";
            _status.AutoSize = BarStaticItemSize.Spring;
            _veza = new BarStaticItem();
            _veza.Caption = "";
            if (ribbonControl1 != null && ribbonControl1.StatusBar != null)
            {
                ribbonControl1.StatusBar.ItemLinks.Add(_status);
                ribbonControl1.StatusBar.ItemLinks.Add(_veza);
            }

            _drvo = new TreeList();
            _drvo.Dock = DockStyle.Fill;
            var stupac = _drvo.Columns.Add();
            stupac.Caption = "";
            stupac.Visible = true;
            stupac.VisibleIndex = 0;
            _drvo.OptionsView.ShowColumns = false;
            _drvo.OptionsView.ShowIndicator = false;
            _drvo.OptionsView.ShowHorzLines = false;
            _drvo.OptionsView.ShowVertLines = false;
            _drvo.OptionsBehavior.Editable = false;
            _drvo.FocusedNodeChanged += Drvo_FocusedNodeChanged;

            _combo = new LookUpEdit();
            _combo.Dock = DockStyle.Top;
            _combo.Properties.NullText = "";
            _combo.Properties.ShowHeader = false;
            _combo.Properties.ShowFooter = false;
            CarpoIzgled.Combo(_combo);
            _combo.Visible = false;
            _combo.EditValueChanged += Combo_EditValueChanged;

            _pogled = new GridView();
            _grid = new GridControl();
            _grid.Dock = DockStyle.Fill;
            _grid.MainView = _pogled;
            _pogled.GridControl = _grid;
            PodesiPogled(_pogled, true);
            _pogled.FocusedRowChanged += Pogled_FocusedRowChanged;

            _detailPogled = new GridView();
            _detailGrid = new GridControl();
            _detailGrid.Dock = DockStyle.Fill;
            _detailGrid.MainView = _detailPogled;
            _detailPogled.GridControl = _detailGrid;
            PodesiPogled(_detailPogled, false);

            var stavke = new XtraTabPage();
            stavke.Text = "Stavke";
            stavke.Controls.Add(_detailGrid);
            _tabDetalj = new XtraTabControl();
            _tabDetalj.Dock = DockStyle.Fill;
            _tabDetalj.TabPages.Add(stavke);

            _detailSplit = new SplitContainerControl();
            _detailSplit.Dock = DockStyle.Fill;
            _detailSplit.Horizontal = true;
            _detailSplit.Panel1.Controls.Add(_grid);
            _detailSplit.Panel2.Controls.Add(_tabDetalj);

            var programi = new XtraTabPage();
            programi.Text = "Programs";
            var explorer = new XtraTabPage();
            explorer.Text = "Explorer";
            explorer.Controls.Add(_drvo);
            explorer.Controls.Add(_combo);
            _tabLijevo = new XtraTabControl();
            _tabLijevo.Dock = DockStyle.Fill;
            _tabLijevo.TabPages.Add(programi);
            _tabLijevo.TabPages.Add(explorer);
            _tabLijevo.SelectedTabPage = explorer;

            _split = new SplitContainerControl();
            _split.Dock = DockStyle.Fill;
            _split.Horizontal = false;
            _split.Panel1.Controls.Add(_tabLijevo);
            _split.Panel2.Controls.Add(_detailSplit);
            Controls.Add(_split);
            if (ribbonControl1 != null)
                Controls.SetChildIndex(ribbonControl1, 0);
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
            Text = string.IsNullOrEmpty(_projekt.Naslov) ? "NetGen" : _projekt.Naslov;
            _veza.Caption = (server ?? "") + " · " + (baza ?? "") + " · " + operater.ToString();
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
            _drvo.BeginUnboundLoad();
            _drvo.Nodes.Clear();
            for (int i = 0; i < _stablo.Stanje.Korijen.Count; i++)
                DodajCvor(_stablo.Stanje.Korijen[i], null);
            _drvo.EndUnboundLoad();
            _drvo.ExpandAll();
        }

        private void DodajCvor(Cvor cvor, TreeListNode roditelj)
        {
            TreeListNode n = _drvo.AppendNode(new object[] { cvor.Natpis ?? "" }, roditelj);
            n.Tag = cvor;
            for (int i = 0; i < cvor.Djeca.Count; i++)
                DodajCvor(cvor.Djeca[i], n);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!_stablo.Stanje.Vidljivo || _stablo.Stanje.Splitter <= 0)
                _split.PanelVisibility = SplitPanelVisibility.Panel2;
            else
            {
                _split.PanelVisibility = SplitPanelVisibility.Both;
                if (_tabLijevo.TabPages.Count > 1)
                    _tabLijevo.SelectedTabPageIndex = 1;
                int d = _stablo.Stanje.Splitter;
                if (d > 40 && d < Width - 120)
                    _split.SplitterPosition = d;
            }
            if (_detail.Stanje.Nema || _detail.Stanje.OtvaraFormu)
                _detailSplit.PanelVisibility = SplitPanelVisibility.Panel1;
            else
            {
                _detailSplit.PanelVisibility = SplitPanelVisibility.Both;
                int visina = _detailSplit.Height;
                if (visina > 160)
                    _detailSplit.SplitterPosition = (int)(visina * 0.62);
            }
            if (_master.Stanje.OtvaraFormu)
            {
                string ime = _master.Stanje.ImeForme ?? "";
                int hash = ime.IndexOf('#');
                if (hash >= 0)
                    ime = ime.Substring(0, hash);
                OtvoriFormu(ime.Trim());
            }
        }

        private void NapuniCombo()
        {
            _combo.Visible = false;
            _combo.Properties.DataSource = null;
            if (_projekt == null || _projekt.Atributi == null || !_projekt.Atributi.Ima("COMBO"))
                return;
            string sql = (_projekt.Atributi.Vrijednost("COMBO") ?? "").Trim();
            if (sql.Length == 0 || SqlTekst.JeZabranaPisanja(sql))
                return;
            DataTable t = Citaj(SqlTekst.Sredi(sql, _master.Kontekst));
            if (t == null || t.Columns.Count == 0 || t.Rows.Count == 0)
                return;
            _combo.Visible = true;
            _combo.Properties.ValueMember = t.Columns[0].ColumnName;
            _combo.Properties.DisplayMember = t.Columns.Count > 1 ? t.Columns[1].ColumnName : t.Columns[0].ColumnName;
            _combo.Properties.DataSource = t;
            _combo.EditValue = t.Rows[0][0];
        }

        private void Combo_EditValueChanged(object sender, EventArgs e)
        {
            if (_combo.EditValue == null || _combo.EditValue is DataRowView)
                return;
            string vrijednost = Convert.ToString(_combo.EditValue);
            _stablo.IzTeksta(vrijednost, _projekt, Citaj);
            NapuniDrvo();
            if (_stablo.Stanje.Vidljivo && _stablo.Stanje.Splitter > 0)
            {
                _split.PanelVisibility = SplitPanelVisibility.Both;
                int d = _stablo.Stanje.Splitter;
                if (d > 40 && d < Width - 120)
                    _split.SplitterPosition = d;
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

        private void OtvoriFormu(string naredba)
        {
            if (string.IsNullOrWhiteSpace(naredba) || _projekt == null)
                return;
            NgForm forma = null;
            string trazeno = naredba.Trim();
            for (int i = 0; i < _projekt.Forme.Count; i++)
            {
                if (string.Equals(_projekt.Forme[i].Ime, trazeno, StringComparison.OrdinalIgnoreCase))
                {
                    forma = _projekt.Forme[i];
                    break;
                }
            }
            if (forma == null)
            {
                NgDnevnik.Akcija("nema forme " + trazeno);
                return;
            }
            var dij = new Dijalog();
            dij.Prikazi(forma, _master.Kontekst, Citaj);
        }

        private void Pogled_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (_punim)
                return;
            OsvjeziDetail();
        }

        private void NapuniGrid()
        {
            _punim = true;
            _master.IzvrsiCitanje(_spoj);
            VeziGrid(_pogled, _grid, _master.Stanje.Tablica, _master.Stanje.Kolone, _master.Stanje.Poruka, _master.Stanje.CheckboxVidljiv, _master.Stanje.GroupByBox, _master.Stanje.FilterRed || _master.Stanje.FilterNamjernoOdstupanje || !_master.Stanje.FilterIzXml);
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
            _grid.DataSource = t;
            PostaviStatus(t == null ? "Greška SQL-a." : (t.Rows.Count.ToString() + " redova"));
            OsvjeziDetail();
        }

        private void OsvjeziDetail()
        {
            if (_detail.Stanje.Nema || _detail.Stanje.OtvaraFormu)
            {
                _detailGrid.DataSource = null;
                return;
            }
            DataRow red = _pogled.GetFocusedDataRow();
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
            VeziGrid(_detailPogled, _detailGrid, t, _detail.Stanje.Kolone, _detail.Stanje.Poruka, _detail.Stanje.CheckboxVidljiv, null, true);
            if (_detail.Stanje.FokusZadnji && _detailPogled.RowCount > 0)
                _detailPogled.FocusedRowHandle = _detailPogled.RowCount - 1;
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
            PostaviStatus(redova.ToString() + " redova");
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
            _naredbe.ItemLinks.Clear();
            if (_projekt == null || ribbonControl1 == null)
                return;
            for (int i = 0; i < _projekt.Gumbi.Count; i++)
            {
                NgButton b = _projekt.Gumbi[i];
                var item = new BarButtonItem(ribbonControl1.Manager, b.Natpis ?? "");
                item.Tag = b.Naredba;
                item.ImageOptions.ImageUri.Uri = Ikona(b.Natpis);
                item.ItemAppearance.Normal.Font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.FontFamily, 7f, System.Drawing.FontStyle.Bold);
                item.ItemClick += Gumb_ItemClick;
                _naredbe.ItemLinks.Add(item);
            }
        }

        private static string Ikona(string natpis)
        {
            string n = (natpis ?? "").ToLowerInvariant();
            if (n.IndexOf("unesi", StringComparison.Ordinal) >= 0 || n.IndexOf("dodaj", StringComparison.Ordinal) >= 0)
                return "Add";
            if (n.IndexOf("izmjen", StringComparison.Ordinal) >= 0 || n.IndexOf("izmijen", StringComparison.Ordinal) >= 0)
                return "Edit";
            if (n.IndexOf("obri", StringComparison.Ordinal) >= 0 || n.IndexOf("briš", StringComparison.Ordinal) >= 0 || n.IndexOf("bris", StringComparison.Ordinal) >= 0)
                return "Delete";
            if (n.IndexOf("ispi", StringComparison.Ordinal) >= 0 || n.IndexOf("print", StringComparison.Ordinal) >= 0)
                return "Print";
            if (n.IndexOf("osvje", StringComparison.Ordinal) >= 0 || n.IndexOf("refresh", StringComparison.Ordinal) >= 0)
                return "Refresh";
            return "";
        }

        private void Gumb_ItemClick(object sender, ItemClickEventArgs e)
        {
            string naredba = e.Item == null || e.Item.Tag == null ? "" : e.Item.Tag.ToString();
            NgDnevnik.Akcija("gumb " + (e.Item == null ? "" : e.Item.Caption) + " " + naredba);
            if (string.Equals(naredba.Trim(), "REFRESHMASTERGRID", StringComparison.OrdinalIgnoreCase))
            {
                _master.Ucitaj(_projekt, null);
                NapuniGrid();
            }
        }

        private void PostaviStatus(string tekst)
        {
            _status.Caption = tekst ?? "";
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
                NgDnevnik.Greska(ex.Message + " | " + sql);
                PostaviStatus(ex.Message);
                return null;
            }
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
