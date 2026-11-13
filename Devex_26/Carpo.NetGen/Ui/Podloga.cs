using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Poziv;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Print;
using Carpo.NetGen.Shell;
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
    /// Samo granule čiji NetGen raspored jest stablo lijevo, master-detail i gumbi dolje.
    /// Ostale idu u ObicnaForma ili u Dijalog. Odabir je OdabirForme.JeManager.
    /// </summary>
    public sealed class Podloga : Carpo.UI.Forms.Master.Manager
    {
        private readonly Stablo _stablo = new Stablo();
        private readonly MasterGrid _master = new MasterGrid();
        private readonly DetailGrid _detail = new DetailGrid();
        private NgProject _projekt;
        private string _spoj = "";
        private string _server = "";
        private string _baza = "";
        private string _sqlGreska = "";
        private string _zadnjiSql = "";
        private string _testId = "";
        private string _imeUnosa = "";
        private readonly List<string> _poslano = new List<string>();
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
            CarpoIzgled.SlikeCvorova(trMain);
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
            CarpoIzgled.RedniStupac(pogled);
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
            _baza = baza ?? "";
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
            string zadano = _projekt.Atributi != null && _projekt.Atributi.Ima("SETDEFAULTS")
                ? _projekt.Atributi.Vrijednost("SETDEFAULTS") : "";
            _master.Clipboard = Carpo.NetGen.Kontrole.ClipboardNg.IzSetDefaults(zadano, kontekst, Citaj);
            // Isti CB vidi i dijalog (SqlTekst.ZaFormu) i gumb, kao cls1.CB u NetGenu (CB-01).
            kontekst.Clipboard = _master.Clipboard;
            _master.Ucitaj(_projekt, null);
            NapuniGrid();
            NapuniGumbe();
            PrimijeniOnLoad2();
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
            if (MultiOdabir.Je(ime))
                NapuniOdabir(ime);
            if (forma.Kontrole == null || forma.Kontrole.Count == 0)
            {
                // Form1.vb 5369: nema kontrola, OKClick je true i dijalog se ne otvara.
                string prazna = DijalogTok.SqlNakonOk(forma);
                if (prazna.Length > 0)
                {
                    string sqlPrazna = SqlTekst.Sredi(prazna, _master.Kontekst);
                    _master.Stanje.IzvorSql = prazna;
                    _master.Stanje.SqlZaSlanje = sqlPrazna;
                    _master.Stanje.ZadnjiSql = sqlPrazna;
                    _master.Stanje.Odluka = SqlTekst.Odluka(sqlPrazna);
                    _master.Stanje.BlokiranoPisanje = SqlTekst.JeZabranaPisanja(sqlPrazna);
                    NapuniGrid();
                    return;
                }
            }
            _dubinaForme++;
            try
            {
                var dij = new Dijalog();
                dij.JupiterNeSaljeGumb(DijalogTok.JeJupiter(_server));
                dij.Konekcija = _spoj;
                dij.Operater = _master.Kontekst.OperaterId;
                dij.Id = _master.Stanje.Id;
                dij.ZadnjiSql = _master.Stanje.ZadnjiSql;
                dij.OtvoriDrugu = delegate (string imeDjeteta)
                {
                    // Form2.vb 3906: strSviParametriuModulu vrijedi i u djetetu.
                    if (_master.Kontekst != null)
                        _master.Kontekst.ParametriSaGrida = ParametriModula.Dodaj(_master.Kontekst.ParametriSaGrida, dij.ParametriVise());
                    OtvoriFormu(imeDjeteta);
                };
                dij.Prikazi(forma, SqlTekst.ZaFormu(_master.Kontekst), Citaj, true, this);
                string iz = dij.Vrijednost("izvjestaj");
                if (!string.IsNullOrEmpty(iz))
                    _polja["izvjestaj"] = iz;
                if (dij.DialogResult != DialogResult.OK)
                    return;
                if (_master.Kontekst != null)
                    _master.Kontekst.ParametriSaGrida = ParametriModula.Dodaj(_master.Kontekst.ParametriSaGrida, dij.ParametriVise());
                ZapamtiNoviTrosak();
                string explorer = DijalogTok.SqlNakonOk(forma);
                if (explorer.Length > 0)
                {
                    string sql = SqlTekst.Sredi(explorer, _master.Kontekst).Replace("~~", ",");
                    _master.Stanje.IzvorSql = explorer;
                    _master.Stanje.ZadnjiSql = sql;
                    _master.Stanje.SqlZaSlanje = sql;
                    _master.Stanje.Odluka = SqlTekst.Odluka(sql);
                    if (_master.Stanje.Odluka == "salji" && !SqlTekst.JeZabranaPisanja(sql))
                        NapuniGrid();
                    return;
                }
                if (DijalogTok.OsvjeziDetalj(forma))
                {
                    OsvjeziDetail();
                    return;
                }
                if (DijalogTok.OsvjeziStablo(forma))
                {
                    _stablo.Ucitaj(_projekt, Citaj);
                    NapuniDrvo();
                    return;
                }
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

        private void PrimijeniStupce(string naredba)
        {
            var mapa = StupciNaredba.Primijeni(naredba);
            foreach (var par in mapa)
            {
                var c = gvGla.Columns.ColumnByFieldName(par.Key);
                if (c == null)
                {
                    for (int i = 0; i < gvGla.Columns.Count; i++)
                    {
                        if (string.Equals(gvGla.Columns[i].Caption, par.Key, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(gvGla.Columns[i].FieldName, par.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            c = gvGla.Columns[i];
                            break;
                        }
                    }
                }
                if (c != null)
                    c.Visible = par.Value;
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
                if (PraznoNaredba.PreskociSljedeci(s))
                {
                    i++;
                    continue;
                }
                if (PraznoNaredba.Je(s) || StupacVrijednost.Je(s))
                    continue;
                if (ForeachNaredba.Vrsta(s).Length > 0)
                {
                    string tijelo = "";
                    if (i + 1 < seg.Length)
                    {
                        tijelo = (seg[i + 1] ?? "").Trim();
                        i++;
                    }
                    string[] sqlovi = ForeachNaredba.Ponovi(tijelo, IdjeviMaster(ForeachNaredba.Vrsta(s)));
                    for (int k = 0; k < sqlovi.Length; k++)
                    {
                        if (ZastitaSql.Tekst(sqlovi[k]).Pise)
                            continue;
                        try { Citaj(sqlovi[k]); }
                        catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sqlovi[k]); }
                    }
                    continue;
                }
                if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    string stanje = Lanac.GumbIzCase(s, Polje("izvjestaj"));
                    int jednako = stanje.IndexOf('=');
                    if (jednako > 0)
                        PostaviGumb(stanje.Substring(0, jednako), stanje.Substring(jednako + 1) != "disable");
                }
                else if (GumbTok.Akcija(_projekt, s) == "fiskal")
                {
                    NgDnevnik.Akcija("fiskal tablica=" + FiskalNaredba.Tablica(s));
                    FiskalTok.Izvrsi(FiskalNaredba.Tablica(s), FiskalNaredba.Argument(s), Citaj);
                }
                else if (GumbTok.Akcija(_projekt, s) == "prostor")
                    NgDnevnik.Akcija("prostor " + ProstorNaredba.Argument(s));
                else if (GumbTok.Akcija(_projekt, s) == "blagajna")
                    NgDnevnik.Akcija(BlagajnaNaredba.Upit(BlagajnaNaredba.Id(s)));
                else if (GumbTok.Akcija(_projekt, s) == "odznaci")
                    NgDnevnik.Akcija("uncheck");
                else if (GumbTok.Akcija(_projekt, s) == "roditelj")
                {
                    Close();
                    return;
                }
                else if (GumbTok.Akcija(_projekt, s) == "poruka")
                {
                    if (!DijalogTok.DozvoliUpis)
                        PorukaNaredba.Pokazi(s);
                }
                else if (GumbTok.Akcija(_projekt, s) == "openform")
                    OtvoriFormu(OtvoriNaredba.Ime(s));
                else if (GumbTok.Akcija(_projekt, s) == "stupci")
                    PrimijeniStupce(s);
                else if (GumbTok.Akcija(_projekt, s) == "word")
                {
                    if (DijalogTok.DozvoliUpis)
                        continue;
                    if (!WordDot.Otvori(s, _spoj, new[] { _master.Stanje.Id }))
                        return;
                }
                else if (IspisMotora.JeIspisImena(s) || s.StartsWith("PRINT", StringComparison.OrdinalIgnoreCase))
                {
                    if (DijalogTok.DozvoliUpis)
                    {
                        NgDnevnik.Akcija("ispis preskocen " + s);
                        continue;
                    }
                    string vrsta = PrintBridge.Vrsta(s, false);
                    object grid = vrsta == "detail" ? (object)gcDet : gcGla;
                    DataTable tab = null;
                    var gc = grid as GridControl;
                    if (gc != null)
                        tab = gc.DataSource as DataTable;
                    string orij = _projekt != null && _projekt.Atributi != null && _projekt.Atributi.Ima("PAGEORIENTATION")
                        ? _projekt.Atributi.Vrijednost("PAGEORIENTATION") : "";
                    PrintBridge.Ispisi(s, false, _master.Stanje.Id, _master.Kontekst.OperaterId, _spoj, _spoj, _master.Stanje.ZadnjiSql, AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), "", "", "", "", Text, grid, orij, tab);
                }
                else if (s.ToUpperInvariant().IndexOf("BEEP()", StringComparison.Ordinal) >= 0)
                    RafalNaredba.Beep();
                else if (s.StartsWith("==", StringComparison.Ordinal))
                {
                    try { Citaj(s.Substring(2)); }
                    catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + s); }
                }
                else if (s.IndexOf("findnode", StringComparison.OrdinalIgnoreCase) >= 0)
                    OznaciPrazneCvorove();
                else if (s.IndexOf("refreshmovetoid", StringComparison.OrdinalIgnoreCase) >= 0)
                    PomakniNaId(s);
                else if (s.ToUpperInvariant().IndexOf("ASSEMBLYLOAD(", StringComparison.Ordinal) >= 0)
                {
                    var poziv = AssemblyPoziv.Pozovi(s, "shell", _master.Stanje.Id, _master.Kontekst.OperaterId, _spoj, _master.Stanje.ZadnjiSql, AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), "");
                    if (poziv.Prekinuto)
                        return;
                }
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
            _master.Kontekst.ParametriSaGrida = SqlTekst.IzReda(row);
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
            var slozeni = GumbiNiz.Slozi(_projekt.Gumbi, _projekt.Buttons);
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 36;
            for (int i = 0; i < slozeni.Count; i++)
            {
                NgButton b = slozeni[i];
                var gumb = new SimpleButton();
                gumb.Text = (b.Natpis ?? "").Replace("!!", "\r\n");
                gumb.Tag = b.Naredba;
                gumb.Dock = DockStyle.None;
                CarpoIzgled.Gumb(gumb);
                gumb.Width = CarpoIzgled.SirinaGumba;
                gumb.Left = CarpoIzgled.LijevoGumba(i);
                gumb.Top = 4;
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

        private void PrimijeniOnLoad2()
        {
            if (_projekt == null || _projekt.Atributi == null || !_projekt.Atributi.Ima("ONLOAD2"))
                return;
            string raw = (_projekt.Atributi.Vrijednost("ONLOAD2") ?? "").Trim();
            if (raw.Length == 0)
                return;
            string sql = SqlTekst.Sredi(raw, _master.Kontekst).Replace("~~", ",");
            if (SqlTekst.Odluka(sql) != "salji" || SqlTekst.JeZabranaPisanja(sql))
                return;
            DataTable t = Citaj(sql);
            if (t == null || t.Rows.Count == 0)
                return;
            DataRow row = t.Rows[0];
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                var gumb = pnlButtons.Controls[i] as SimpleButton;
                if (gumb == null)
                    continue;
                string natpis = (gumb.Text ?? "").Replace("\r\n", "").Trim();
                if (!row.Table.Columns.Contains(natpis))
                    continue;
                string v = row[natpis] == DBNull.Value ? "" : Convert.ToString(row[natpis]);
                string akcija = OnLoad2.Akcija(v);
                if (akcija == "visible")
                    gumb.Visible = true;
                else if (akcija == "invisible")
                    gumb.Visible = false;
                else if (akcija == "enable")
                    gumb.Enabled = true;
                else if (akcija == "disable")
                    gumb.Enabled = false;
                else if (akcija == "natpis")
                    gumb.Text = v;
            }
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

        private System.Collections.Generic.List<string> IdjeviMaster(string vrsta)
        {
            var svi = new System.Collections.Generic.List<string>();
            var plavi = new System.Collections.Generic.List<string>();
            var cek = new System.Collections.Generic.List<string>();
            if (gvGla == null)
                return svi;
            DevExpress.XtraGrid.Columns.GridColumn kol = null;
            for (int c = 0; c < gvGla.Columns.Count; c++)
            {
                if (gvGla.Columns[c].ColumnType == typeof(bool))
                {
                    kol = gvGla.Columns[c];
                    break;
                }
            }
            for (int r = 0; r < gvGla.DataRowCount; r++)
            {
                int h = gvGla.GetRowHandle(r);
                string id = IdCelije(h);
                svi.Add(id);
                if (kol != null)
                {
                    object b = gvGla.GetRowCellValue(h, kol);
                    if (b is bool && (bool)b)
                        cek.Add(id);
                }
            }
            int[] sel = gvGla.GetSelectedRows();
            for (int i = 0; i < sel.Length; i++)
            {
                if (sel[i] >= 0)
                    plavi.Add(IdCelije(sel[i]));
            }
            string[] iz = ForeachNaredba.Idjevi(vrsta, svi, plavi, cek);
            return new System.Collections.Generic.List<string>(iz);
        }

        private void OznaciPrazneCvorove()
        {
            if (trMain == null)
                return;
            var prazni = new System.Collections.Generic.List<DevExpress.XtraTreeList.Nodes.TreeListNode>();
            SkupiPrazne(trMain.Nodes, prazni);
            for (int i = 0; i < prazni.Count; i++)
                prazni[i].Expanded = true;
            if (prazni.Count > 0)
                trMain.FocusedNode = prazni[0];
        }

        private static void SkupiPrazne(DevExpress.XtraTreeList.Nodes.TreeListNodes cvorovi, System.Collections.Generic.List<DevExpress.XtraTreeList.Nodes.TreeListNode> prazni)
        {
            if (cvorovi == null)
                return;
            for (int i = 0; i < cvorovi.Count; i++)
            {
                DevExpress.XtraTreeList.Nodes.TreeListNode n = cvorovi[i];
                string ime = n.GetDisplayText(0) ?? "";
                if (ime.Length == 0)
                    prazni.Add(n);
                SkupiPrazne(n.Nodes, prazni);
            }
        }

        private void PomakniNaId(string naredba)
        {
            string ostatak = (naredba ?? "").ToLowerInvariant().Replace("refreshmovetoid", "").Trim();
            if (_master != null && _master.Stanje != null && (_master.Stanje.ZadnjiSql ?? "").Length > 0)
            {
                try { Citaj(_master.Stanje.ZadnjiSql); }
                catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + _master.Stanje.ZadnjiSql); }
            }
            var tablica = gcGla == null ? null : gcGla.DataSource as DataTable;
            int red = RafalNaredba.RedStupca2(tablica, ostatak);
            if (red < 0 || gvGla == null)
                return;
            int h = gvGla.GetRowHandle(red);
            if (h >= 0)
            {
                gvGla.FocusedRowHandle = h;
                gvGla.MakeRowVisible(h);
            }
        }

        private string IdCelije(int red)
        {
            if (red < 0)
                return "0";
            object id = null;
            if (gvGla.Columns.ColumnByFieldName("id") != null)
                id = gvGla.GetRowCellValue(red, "id");
            if (id == null || id == DBNull.Value)
            {
                if (gvGla.Columns.Count > 0)
                    id = gvGla.GetRowCellValue(red, gvGla.Columns[0]);
            }
            return id == null || id == DBNull.Value ? "0" : Convert.ToString(id);
        }

        private DataTable Citaj(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return null;
            if (DateTime.UtcNow > RokUtc || _brojUpita >= MaxUpita)
            {
                _sqlGreska = "sweep rok";
                return null;
            }
            _brojUpita++;
            if (sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                sql = SrediPoslije.Petlja(sql, "", null, false);
            if (sql.IndexOf("cis.porezna-uprava.hr", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _sqlGreska = "nije poslano na produkcijski URL Porezne";
                return null;
            }
            if (SqlTekst.JeZabranaPisanja(sql) && !DijalogTok.DozvoliUpis)
                return null;
            bool filter = sql.IndexOf("NetGenDBFilter", StringComparison.OrdinalIgnoreCase) >= 0;
            _zadnjiSql = sql;
            if (!filter)
                _poslano.Add(sql);
            try
            {
                var dt = new DataTable();
                using (var conn = new System.Data.SqlClient.SqlConnection(_spoj))
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = TimeoutUpita;
                    conn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                if (!filter)
                    _sqlGreska = ex.Message + " | " + sql;
                NgDnevnik.Greska(ex.Message + " | " + sql);
                if (!filter)
                    PostaviStatus(ex.Message);
                return null;
            }
        }

        public static int TimeoutUpita = 30;
        public static int MaxUpita = int.MaxValue;
        public static DateTime RokUtc = DateTime.MaxValue;
        private int _brojUpita;

        /// <summary>
        /// Klik na podlozi. Upis ide samo na spin na sql-ai. Fiskalizacija ne ide na Poreznu.
        /// samoCitanje preskače Unesi/Izmjeni/Obriši. Podforma se zatvara Odustani.
        /// </summary>
        public List<string> ProvediKlikove()
        {
            return ProvediKlikove(false);
        }

        public List<string> ProvediKlikove(bool samoCitanje)
        {
            var redovi = new List<string>();
            bool upis = !samoCitanje && DijalogTok.UpisNaSpinu(_server, _baza);
            DijalogTok.DozvoliUpis = upis;
            _imeUnosa = "kp" + DateTime.Now.ToString("HHmmss");
            DijalogTok.TekstUpisa = _imeUnosa;
            _korisceno.Clear();
            if (upis)
            {
                try { System.IO.File.AppendAllText(Dijalog.TragPutanja ?? "", DateTime.Now.ToString("HH:mm:ss") + " klikovi\r\n"); } catch { }
                Dijalog.NakonPrikaza = delegate (Dijalog d)
                {
                    if (d == null || d.IsDisposed)
                        return;
                    PotvrdiUpis(d);
                };
            }
            try
            {
            return ProvediKlikoveUnutra(redovi, upis);
            }
            finally
            {
                if (upis)
                    OcistiTest();
                Dijalog.NakonPrikaza = null;
                DijalogTok.DozvoliUpis = false;
                ZaustaviZatvaranje();
            }
        }

        private List<string> ProvediKlikoveUnutra(List<string> redovi, bool upis)
        {
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
            gumbi.Sort(delegate (SimpleButton a, SimpleButton b) { return Redoslijed(a).CompareTo(Redoslijed(b)); });
            for (int i = 0; i < gumbi.Count; i++)
            {
                if (DateTime.UtcNow > RokUtc)
                {
                    redovi.Add(Linija(granula, "gumb", "rok", "HANG", "isteklo vrijeme granule", "netgen/Form2.vb:3367"));
                    break;
                }
                SimpleButton g = gumbi[i];
                string naredba = g.Tag == null ? "" : g.Tag.ToString();
                string natpis = (g.Text ?? "").Replace("\r\n", " ").Trim();
                if (naredba.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0
                    || natpis.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", "nije poslano na https://cis.porezna-uprava.hr:8449/FiskalizacijaService ; test URL je https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest", "netgen/Form2.vb:6342"));
                    continue;
                }
                if (upis && (naredba.IndexOf("WEBBROWSER", StringComparison.OrdinalIgnoreCase) >= 0
                    || natpis.Equals("Web", StringComparison.OrdinalIgnoreCase)))
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", "preglednik nije dio upisa", "netgen/Form2.vb:3367"));
                    continue;
                }
                if (!upis && Destruktivan(naredba, natpis))
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", naredba, "netgen/Form2.vb:3367"));
                    continue;
                }
                _sqlGreska = "";
                _zadnjiSql = "";
                _poslano.Clear();
                try
                {
                    if (upis && (naredba.IndexOf("Izm", StringComparison.OrdinalIgnoreCase) >= 0
                        || naredba.IndexOf("UPDATE", StringComparison.OrdinalIgnoreCase) >= 0))
                        DijalogTok.TekstUpisa = _imeUnosa + "-izmjena";
                    else if (upis)
                        DijalogTok.TekstUpisa = _imeUnosa;
                    if (upis && _testId.Length > 0)
                    {
                        _master.Kontekst.Id = _testId;
                        _master.Kontekst.Node = _testId;
                        if (_master.Kontekst.ParametriSaGrida.IndexOf("@statusiid=", StringComparison.OrdinalIgnoreCase) < 0)
                            _master.Kontekst.ParametriSaGrida += "@statusiid='" + _testId + "' , ";
                    }
                    if (upis && naredba.IndexOf("INSERTFORM", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        DataTable vd = Citaj("select top 1 cast(VrstaDokId as varchar(20)) as id from VrstaDok where Opis = N'klik-proba'");
                        if (vd != null && vd.Rows.Count > 0)
                            _master.Kontekst.Node = Convert.ToString(vd.Rows[0]["id"]);
                    }
                    try { System.IO.File.AppendAllText(Dijalog.TragPutanja ?? "", DateTime.Now.ToString("HH:mm:ss") + " gumb " + natpis + "\r\n"); } catch { }
                    if (MultiOdabir.Je(naredba))
                    {
                        if (gvGla.DataRowCount == 0)
                        {
                            _master.Stanje.SqlZaSlanje = "exec spLnRobaWebKlase_Query @action='SVA', @klrobawebklaseid=0";
                            _master.Stanje.Odluka = "salji";
                            _master.Stanje.BlokiranoPisanje = false;
                            NapuniGrid();
                        }
                        OznaciRedove();
                    }
                    PokreniZatvaranje();
                    g.PerformClick();
                    Application.DoEvents();
                    string provjera = upis ? ProvjeriUpis(naredba) : "";
                    if (!string.IsNullOrEmpty(_sqlGreska))
                        redovi.Add(Linija(granula, "gumb", natpis, "GRESKA", _sqlGreska, "netgen/Form2.vb:3377"));
                    else if (provjera.StartsWith("GRESKA", StringComparison.Ordinal))
                        redovi.Add(Linija(granula, "gumb", natpis, "GRESKA", provjera.Substring(6).Trim(), "netgen/Form2.vb:3406"));
                    else if (provjera.Length > 0)
                        redovi.Add(Linija(granula, "gumb", natpis, "OK", provjera, "netgen/Form2.vb:3367"));
                    else
                        redovi.Add(Linija(granula, "gumb", natpis, "OK", naredba, "netgen/Form1.vb:5664"));
                }
                catch (Exception ex)
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "GRESKA", ex.Message, "netgen/Form2.vb:3367"));
                }
            }
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
                            if (f.IsDisposed)
                                return;
                            if (DijalogTok.DozvoliUpis && PotvrdiUpis(f))
                                return;
                            f.DialogResult = DialogResult.Cancel;
                            f.Close();
                        }));
                    }
                    catch
                    {
                    }
                }
            }
        }

        private bool PotvrdiUpis(Form f)
        {
            if (f.Tag as string == "potvrdeno")
                return true;
            PopuniTest(f);
            var gumbi = new List<SimpleButton>();
            SkupiGumbe(f, gumbi);
            if (KlikniPotvrdu(f, gumbi, true))
                return true;
            for (int i = 0; i < gumbi.Count; i++)
            {
                SimpleButton b = gumbi[i];
                string tag = b.Tag as string ?? "";
                string tekst = (b.Text ?? "").Trim();
                if (tag.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                if (tag.StartsWith(":", StringComparison.Ordinal))
                    continue;
                if (JeOdustani(tag, tekst))
                    continue;
                if (tag.Length == 0)
                    continue;
                if (JeTudjiUpis(tag))
                    continue;
                f.Tag = "potvrdeno";
                b.PerformClick();
                if (!f.IsDisposed)
                {
                    f.DialogResult = DialogResult.OK;
                    f.Close();
                }
                return true;
            }
            return false;
        }

        private bool KlikniPotvrdu(Form f, List<SimpleButton> gumbi, bool samoNatpis)
        {
            for (int i = 0; i < gumbi.Count; i++)
            {
                SimpleButton b = gumbi[i];
                string tag = b.Tag as string ?? "";
                string tekst = (b.Text ?? "").Trim();
                if (!PotvrdaGumba.JeNatpis(tekst))
                    continue;
                if (tag.IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                if (JeOdustani(tag, tekst))
                    continue;
                if (tag.Length == 0)
                    continue;
                if (JeTudjiUpis(tag))
                    continue;
                f.Tag = "potvrdeno";
                b.PerformClick();
                if (!f.IsDisposed)
                {
                    f.DialogResult = DialogResult.OK;
                    f.Close();
                }
                return true;
            }
            return samoNatpis ? false : false;
        }

        private void ZapamtiNoviTrosak()
        {
            string sql = PoslanoKoje("spPrnTroskoviGlaAzur");
            if (sql.Length == 0 || !Akcija(sql, "0") || _master.Kontekst == null)
                return;
            DataTable t = Citaj("select top 1 cast(PrnTroskoviGlaId as varchar(20)) as id from PrnTroskoviGla where KlMasterRobaId is null and RobaId is null order by PrnTroskoviGlaId desc");
            if (t == null || t.Rows.Count == 0)
                return;
            _testId = Convert.ToString(t.Rows[0]["id"]);
            _master.Kontekst.Id = _testId;
            _master.Stanje.Id = _testId;
            _master.Kontekst.ParametriSaGrida = ParametriModula.Dodaj(_master.Kontekst.ParametriSaGrida, "@PrnTroskoviGlaId='" + _testId + "' , ");
        }

        private static void SkupiGumbe(Control roditelj, List<SimpleButton> gumbi)
        {
            if (roditelj == null)
                return;
            for (int i = 0; i < roditelj.Controls.Count; i++)
            {
                var b = roditelj.Controls[i] as SimpleButton;
                if (b != null)
                    gumbi.Add(b);
                SkupiGumbe(roditelj.Controls[i], gumbi);
            }
        }

        private static void PopuniTest(Form f)
        {
            string tekst = DijalogTok.TekstUpisa ?? "";
            if (tekst.Length == 0)
                return;
            string[] imena = new string[] { "Naziv", "Korak", "Sifra", "Oznaka", "HPTPoziv" };
            for (int i = 0; i < imena.Length; i++)
            {
                Control[] n = f.Controls.Find(imena[i], true);
                if (n.Length == 0)
                    continue;
                var edit = n[0] as TextEdit;
                if (edit != null && !(edit is LookUpEdit))
                    edit.Text = string.Equals(imena[i], "HPTPoziv", StringComparison.OrdinalIgnoreCase) ? "01" : tekst;
            }
            OdaberiLook(f);
        }

        private static readonly System.Collections.Generic.HashSet<string> _korisceno = new System.Collections.Generic.HashSet<string>();

        private static void OdaberiLook(Control roditelj)
        {
            if (roditelj == null)
                return;
            for (int i = 0; i < roditelj.Controls.Count; i++)
            {
                var look = roditelj.Controls[i] as LookUpEdit;
                if (look != null && (look.EditValue == null || look.EditValue == DBNull.Value))
                {
                    var dt = look.Properties.DataSource as DataTable;
                    if (dt != null && dt.Columns.Count > 0)
                    {
                        // Isti combo u istom klik pregledu dobije vrijednost koju jos nije imao. Izmjena
                        // PrnTroskovi (ADDITEMS -> ADDITEM) dodaje stavku istom zaglavlju; ista vrsta kao kod
                        // Unesi granula sama odbije ("Postoji vec takva vrsta troska"), kao i NetGen.
                        string kljuc = (look.Name ?? "").ToLowerInvariant();
                        object prva = null;
                        object nova = null;
                        for (int r = 0; r < dt.Rows.Count; r++)
                        {
                            object v = dt.Rows[r][0];
                            if (v == null || v == DBNull.Value)
                                continue;
                            string s = Convert.ToString(v);
                            if (s.Length == 0 || s == "null")
                                continue;
                            if (prva == null)
                                prva = v;
                            if (!_korisceno.Contains(kljuc + "=" + s))
                            {
                                nova = v;
                                break;
                            }
                        }
                        object izbor = nova ?? prva;
                        if (izbor != null)
                        {
                            look.EditValue = izbor;
                            _korisceno.Add(kljuc + "=" + Convert.ToString(izbor));
                        }
                    }
                }
                OdaberiLook(roditelj.Controls[i]);
            }
        }

        private bool JeTudjiUpis(string tag)
        {
            string t = tag.ToLowerInvariant();
            bool brise = t.Contains("@action=2") || t.Contains("@action = 2") || ImaRijec(t, "delete");
            bool mijenja = t.Contains("@action=1") || t.Contains("@action = 1") || ImaRijec(t, "update");
            if ((brise || mijenja) && _testId.Length == 0)
                return true;
            return false;
        }

        private static bool JeOdustani(string tag, string tekst)
        {
            string u = (tag ?? "").Trim().ToUpperInvariant();
            if (u == "CLOSE" || u == "CANCEL" || u.EndsWith("|CLOSE") || u.EndsWith("|CANCEL"))
                return true;
            string n = (tekst ?? "").ToLowerInvariant();
            return n.StartsWith("odustani") || n.StartsWith("cancel");
        }

        private void OznaciRedove()
        {
            gvGla.ClearSelection();
            for (int r = 0; r < gvGla.DataRowCount; r++)
                gvGla.SelectRow(gvGla.GetRowHandle(r));
        }

        private void NapuniOdabir(string ime)
        {
            if (!MultiOdabir.Je(ime))
                return;
            string op = _master.Kontekst == null ? "0" : _master.Kontekst.OperaterId;
            Citaj(MultiOdabir.Obrisi(op));
            var ids = new System.Collections.Generic.List<string>();
            int[] sel = gvGla.GetSelectedRows();
            for (int i = 0; i < sel.Length; i++)
            {
                if (sel[i] < 0)
                    continue;
                string id = IdCelije(sel[i]);
                if (id.Length > 0 && id != "0")
                    ids.Add(id);
            }
            for (int i = 0; i < ids.Count; i++)
                Citaj(MultiOdabir.Ubaci(ids[i], op));
        }

        private static int Redoslijed(SimpleButton g)
        {
            string n = ((g.Tag as string) ?? "") + " " + (g.Text ?? "");
            if (n.IndexOf("Unesi", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("INSERT", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("@action=0", StringComparison.OrdinalIgnoreCase) >= 0)
                return 0;
            if (n.IndexOf("Izm", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("UPDATE", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("@action=1", StringComparison.OrdinalIgnoreCase) >= 0)
                return 1;
            if (n.IndexOf("MULTIBLUE_ADD", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("MULTI_INSERT", StringComparison.OrdinalIgnoreCase) >= 0)
                return 2;
            if (n.IndexOf("MULTIBLUE_DELETE", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("MULTI_DELETE", StringComparison.OrdinalIgnoreCase) >= 0)
                return 3;
            if (n.IndexOf("Obri", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("DELETE", StringComparison.OrdinalIgnoreCase) >= 0
                || n.IndexOf("@action=2", StringComparison.OrdinalIgnoreCase) >= 0)
                return 4;
            return 5;
        }

        private string PoslanoKoje(string dio)
        {
            for (int i = 0; i < _poslano.Count; i++)
            {
                if (_poslano[i].IndexOf(dio, StringComparison.OrdinalIgnoreCase) >= 0)
                    return _poslano[i];
            }
            return "";
        }

        private string ProvjeriUpis(string naredba)
        {
            string sql = PoslanoKoje("spJeziciAzur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spStatusiAzur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spZupanijeAzur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spPrnVrstaTroskaAzur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spKlRobaWebKlase_Azur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spLnRobaWebKlase_Azur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spRatArkod_Azur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spPrnTroskoviDetAzur");
            if (sql.Length == 0)
                sql = PoslanoKoje("spPrnTroskoviGlaAzur");
            if (sql.Length == 0)
                sql = _zadnjiSql ?? "";
            if (sql.Length == 0)
                return "";
            if (sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                return "GRESKA Form2.vb:3406 zamijeni @@ iz kontrole, poslano je " + sql;
            string tekst = (DijalogTok.TekstUpisa ?? "").Replace("'", "''");
            if (sql.IndexOf("spJeziciAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("@action=0", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable t = Citaj("select top 1 cast(JeziciId as varchar(20)) as id from jezici where Naziv = N'" + tekst + "'");
                if (t == null || t.Rows.Count == 0)
                    return "GRESKA nema reda u jezici.Naziv=" + DijalogTok.TekstUpisa + " sql=" + sql;
                _testId = Convert.ToString(t.Rows[0]["id"]);
                _master.Kontekst.Id = _testId;
                return "jezici.Naziv=" + DijalogTok.TekstUpisa + " id=" + _testId;
            }
            if (sql.IndexOf("spJeziciAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("@action=1", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable t = Citaj("select Naziv from jezici where JeziciId = " + (_testId.Length == 0 ? "0" : _testId));
                string naziv = t == null || t.Rows.Count == 0 ? "" : Convert.ToString(t.Rows[0]["Naziv"]);
                if (naziv != DijalogTok.TekstUpisa)
                    return "GRESKA jezici.Naziv je '" + naziv + "', očekivano " + DijalogTok.TekstUpisa;
                return "jezici id=" + _testId + " Naziv=" + naziv;
            }
            if (sql.IndexOf("spJeziciAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("@action=2", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DataTable t = Citaj("select count(*) as n from jezici where Naziv like N'klik-proba%'");
                int n = t == null || t.Rows.Count == 0 ? -1 : Convert.ToInt32(t.Rows[0]["n"]);
                if (n != 0)
                    return "GRESKA brisanje je ostavilo " + n.ToString() + " testnih redova";
                _testId = "";
                return "testni jezik obrisan";
            }
            if (sql.IndexOf("spStatusiAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && Akcija(sql, "0"))
            {
                DataTable t = Citaj("select top 1 cast(statusiid as varchar(20)) as id from statusi where Korak = N'" + tekst + "'");
                if (t == null || t.Rows.Count == 0)
                    return "GRESKA nema reda u statusi.Korak=" + DijalogTok.TekstUpisa;
                _testId = Convert.ToString(t.Rows[0]["id"]);
                _master.Kontekst.Id = _testId;
                return "statusi.Korak=" + DijalogTok.TekstUpisa + " id=" + _testId;
            }
            if (sql.IndexOf("spStatusiAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && Akcija(sql, "1"))
            {
                string id = _testId.Length == 0 ? "0" : _testId;
                DataTable t = Citaj("select Korak from statusi where statusiid = " + id);
                string korak = t == null || t.Rows.Count == 0 ? "" : Convert.ToString(t.Rows[0]["Korak"]);
                if (korak != DijalogTok.TekstUpisa)
                    return "GRESKA statusi.Korak je '" + korak + "', očekivano " + DijalogTok.TekstUpisa + " sql=" + sql;
                return "statusi id=" + id + " Korak=" + korak;
            }
            if (sql.IndexOf("spStatusiAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && Akcija(sql, "2"))
            {
                string id = _testId.Length == 0 ? "0" : _testId;
                DataTable t = Citaj("select count(*) as n from statusi where statusiid = " + id);
                int n = t == null || t.Rows.Count == 0 ? -1 : Convert.ToInt32(t.Rows[0]["n"]);
                if (n != 0)
                    return "GRESKA brisanje statusa ostavilo " + n.ToString() + " sql=" + sql;
                _testId = "";
                return "testni status obrisan";
            }
            if (sql.IndexOf("spZupanijeAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "0"))
                return ProvjeriNaziv("Zupanije", "ZupanijeId", tekst, true);
            if (sql.IndexOf("spZupanijeAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "1"))
                return ProvjeriNaziv("Zupanije", "ZupanijeId", tekst, false);
            if (sql.IndexOf("spZupanijeAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "2"))
                return ProvjeriObrisano("Zupanije", "ZupanijeId");
            if (sql.IndexOf("spPrnVrstaTroskaAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "0"))
                return ProvjeriNaziv("PrnVrstaTroska", "PrnVrstaTroskaId", tekst, true);
            if (sql.IndexOf("spPrnVrstaTroskaAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "1"))
                return ProvjeriNaziv("PrnVrstaTroska", "PrnVrstaTroskaId", tekst, false);
            if (sql.IndexOf("spPrnVrstaTroskaAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "2"))
                return ProvjeriObrisano("PrnVrstaTroska", "PrnVrstaTroskaId");
            if (sql.IndexOf("spKlRobaWebKlase_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'INSERT'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriNaziv("KlRobaWebKlase", "KlRobaWebKlaseId", tekst, true);
            if (sql.IndexOf("spKlRobaWebKlase_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'UPDATE'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriNaziv("KlRobaWebKlase", "KlRobaWebKlaseId", tekst, false);
            if (sql.IndexOf("spKlRobaWebKlase_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'DELETE'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriObrisano("KlRobaWebKlase", "KlRobaWebKlaseId");
            if (sql.IndexOf("spLnRobaWebKlase_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("MULTI_INSERT", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriVeze(true);
            if (sql.IndexOf("spLnRobaWebKlase_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("MULTI_DELETE", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriVeze(false);
            if (sql.IndexOf("spRatArkod_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'insert'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriArkod(true);
            if (sql.IndexOf("spRatArkod_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'update'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriNaziv("RatArkod", "RatArkodId", tekst, false);
            if (sql.IndexOf("spRatArkod_Azur", StringComparison.OrdinalIgnoreCase) >= 0
                && sql.IndexOf("'delete'", StringComparison.OrdinalIgnoreCase) >= 0)
                return ProvjeriObrisano("RatArkod", "RatArkodId");
            if (sql.IndexOf("spPrnTroskoviGlaAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "0"))
                return ProvjeriTrosak(true);
            if (sql.IndexOf("spPrnTroskoviDetAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "0"))
            {
                if (_testId.Length == 0)
                    ProvjeriTrosak(true);
                return ProvjeriStavku();
            }
            if (sql.IndexOf("spPrnTroskoviGlaAzur", StringComparison.OrdinalIgnoreCase) >= 0 && Akcija(sql, "2"))
                return ProvjeriObrisano("PrnTroskoviGla", "PrnTroskoviGlaId");
            return "poslano: " + sql;
        }

        private static bool Akcija(string sql, string broj)
        {
            int p = sql.IndexOf("@action=" + broj, StringComparison.OrdinalIgnoreCase);
            if (p < 0)
                return false;
            int kraj = p + "@action=".Length + broj.Length;
            if (kraj < sql.Length && char.IsDigit(sql[kraj]))
                return false;
            return true;
        }

        private string ProvjeriNaziv(string tablica, string kljuc, string tekst, bool novi)
        {
            if (novi)
            {
                DataTable t = Citaj("select top 1 cast(" + kljuc + " as varchar(20)) as id from " + tablica + " where Naziv = N'" + tekst + "'");
                if (t == null || t.Rows.Count == 0)
                    return "GRESKA nema reda u " + tablica + ".Naziv=" + DijalogTok.TekstUpisa;
                _testId = Convert.ToString(t.Rows[0]["id"]);
                _master.Kontekst.Id = _testId;
                return tablica + ".Naziv=" + DijalogTok.TekstUpisa + " id=" + _testId;
            }
            string id = _testId.Length == 0 ? "0" : _testId;
            DataTable red = Citaj("select Naziv from " + tablica + " where " + kljuc + " = " + id);
            string naziv = red == null || red.Rows.Count == 0 ? "" : (Convert.ToString(red.Rows[0]["Naziv"]) ?? "").TrimEnd();
            if (naziv != (DijalogTok.TekstUpisa ?? "").TrimEnd())
                return "GRESKA " + tablica + ".Naziv je '" + naziv + "', očekivano " + DijalogTok.TekstUpisa;
            return tablica + " id=" + id + " Naziv=" + naziv;
        }

        private string ProvjeriVeze(bool dodane)
        {
            string id = _testId.Length == 0 ? "0" : _testId;
            DataTable t = Citaj("select count(*) as n from LnRobaWebKlase where KlRobaWebKlaseId = " + id);
            int n = t == null || t.Rows.Count == 0 ? 0 : Convert.ToInt32(t.Rows[0]["n"]);
            if (dodane && n == 0)
                return "GRESKA multiselect nije ubacio robu u klasu " + id;
            if (!dodane && n != 0)
                return "GRESKA uklanjanje je ostavilo " + n.ToString() + " veza";
            return (dodane ? "dodano " : "uklonjeno ") + "veza=" + n.ToString() + " klasa=" + id;
        }

        private string ProvjeriArkod(bool novi)
        {
            string tekst = (DijalogTok.TekstUpisa ?? "").Replace("'", "''");
            DataTable t = Citaj("select top 1 cast(RatArkodId as varchar(20)) as id from RatArkod where Naziv = N'" + tekst + "'");
            if (t == null || t.Rows.Count == 0)
                return "GRESKA nema reda u RatArkod.Naziv=" + DijalogTok.TekstUpisa;
            _testId = Convert.ToString(t.Rows[0]["id"]);
            _master.Kontekst.Id = _testId;
            DataTable k = Citaj("select count(*) as n from RatArkodKat where RatArkodId = " + _testId);
            int n = k == null || k.Rows.Count == 0 ? 0 : Convert.ToInt32(k.Rows[0]["n"]);
            if (novi && n == 0)
                return "GRESKA arkod " + _testId + " nema katastarsku česticu";
            return "RatArkod.Naziv=" + DijalogTok.TekstUpisa + " id=" + _testId + " cestice=" + n.ToString();
        }

        private string ProvjeriTrosak(bool novi)
        {
            DataTable t = Citaj("select top 1 cast(PrnTroskoviGlaId as varchar(20)) as id from PrnTroskoviGla where KlMasterRobaId is null and RobaId is null order by PrnTroskoviGlaId desc");
            if (t == null || t.Rows.Count == 0)
                return "GRESKA nema zaglavlja troška";
            if (novi)
            {
                _testId = Convert.ToString(t.Rows[0]["id"]);
                _master.Kontekst.Id = _testId;
            }
            return "PrnTroskoviGla id=" + Convert.ToString(t.Rows[0]["id"]);
        }

        private string ProvjeriStavku()
        {
            string id = _testId.Length == 0 ? "0" : _testId;
            DataTable t = Citaj("select count(*) as n from PrnTroskoviDet where PrnTroskoviGlaId = " + id);
            int n = t == null || t.Rows.Count == 0 ? 0 : Convert.ToInt32(t.Rows[0]["n"]);
            if (n == 0)
                return "GRESKA podforma nije ubacila stavku za gla " + id;
            return "PrnTroskoviDet gla=" + id + " stavki=" + n.ToString();
        }

        private string ProvjeriObrisano(string tablica, string kljuc)
        {
            string id = _testId.Length == 0 ? "0" : _testId;
            DataTable t = Citaj("select count(*) as n from " + tablica + " where " + kljuc + " = " + id);
            int n = t == null || t.Rows.Count == 0 ? -1 : Convert.ToInt32(t.Rows[0]["n"]);
            if (n != 0)
                return "GRESKA brisanje " + tablica + " ostavilo " + n.ToString();
            _testId = "";
            return "testni red u " + tablica + " obrisan";
        }

        private void OcistiTest()
        {
            try
            {
                Citaj("delete from jezici where Naziv like N'kp[0-9]%' or Naziv like N'klik-proba%'");
                Citaj("delete from statusi where Korak like N'kp[0-9]%' or Korak like N'klik-proba%'");
                Citaj("delete from Zupanije where Naziv like N'kp[0-9]%' or Naziv like N'klik-proba%'");
                Citaj("delete from PrnVrstaTroska where Naziv like N'kp[0-9]%' or Naziv like N'klik-proba%'");
            }
            catch
            {
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
