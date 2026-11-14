using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
    /// Form1: podloga svake granule (Prozor, RUT-01). Stablo lijevo i detalj se sakriju kad ih nema
    /// (OnShown), a ONLOAD ":Forma" otvori dijalog nad podlogom.
    /// </summary>
    public sealed partial class Podloga : Carpo.UI.Forms.Master.Manager
    {
        private readonly Stablo _stablo = new Stablo();
        private readonly MasterGrid _master = new MasterGrid();
        private readonly DetailGrid _detail = new DetailGrid();
        private NgProject _projekt;
        private string _spoj = "";
        private string _server = "";
        private string _baza = "";
        private string _sqlGreska = "";
        // SWP-04: _sqlGreska je poruka iz baze (RAISERROR/spRaiserror, broj >= 50000), npr. "Niste odabrali
        // dokument za ispravak!". NetGen je prikaže korisniku; klik pregled je bilježi kao PORUKA, ne GRESKA.
        private bool _sqlPoruka;
        private bool _vezaPrijavljena;

        private string StatusSqlGreske
        {
            get { return _sqlPoruka ? "PORUKA" : "GRESKA"; }
        }

        /// <summary>Greška master grida koju bi NetGen pokazao (MasterGrid.IzvrsiCitanje kroz NgVeza; tiha greška je bez Poruke).</summary>
        private string MasterGreska()
        {
            if (_master == null || _master.Stanje == null || string.IsNullOrEmpty(_master.Stanje.SqlGreska) || _master.Stanje.Poruka == null)
                return "";
            return _master.Stanje.SqlGreska + " | " + (_master.Stanje.SqlZaSlanje ?? "");
        }

        private string StatusMasterGreske
        {
            get { return _master.Stanje.SqlGreskaBroj >= 50000 ? "PORUKA" : "GRESKA"; }
        }
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
            NgPoruka.Pripremi();
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
            // Form1 3560 (POPUP/POPUPMASTER, 204 granule) i 3700 (POPUPDETAIL): desni klik (POP-01).
            // Form1 3577-3610 i 3627-3672: s PERMISIO stavke i stalne stavke slijede ssPermission (DOZ-01).
            GridIzbornik.Vezi(gvGla, delegate { return _projekt == null ? "" : (_projekt.Popup ?? ""); }, IzvrsiIzbornik, DozvolaStavke, new GridRadnje
            {
                Ispis = delegate { IspisiGrid("PRINTMASTER", false); },
                Spremi = delegate { GridRaspored.Spremi(gvGla, DatotekaIzgleda(false)); },
                Reset = delegate { GridRaspored.Obrisi(DatotekaIzgleda(false)); OsvjeziMaster(); }
            });
            GridIzbornik.Vezi(gvDet, delegate { return AtributProjekta("POPUPDETAIL"); }, IzvrsiIzbornik, DozvolaStavke, new GridRadnje
            {
                Ispis = delegate { IspisiGrid("PRINTDET", true); },
                Spremi = delegate { GridRaspored.Spremi(gvDet, DatotekaIzgleda(true)); },
                Reset = delegate { GridRaspored.Obrisi(DatotekaIzgleda(true)); OsvjeziDetail(); }
            });
            // Form1.GridEX2_DoubleClick (7200): NovaGenerickaForma(strDBlClickDetForm), lanac kao gumb (DBL-02).
            gvDet.DoubleClick += delegate
            {
                string f = AtributProjekta("DETAILDBLCLICK");
                if (f.Length > 0)
                    IzvrsiLanac(f);
            };
            VeziUredjivanje();
            CarpoIzgled.SlikeCvorova(trMain);
            trMain.OptionsView.ShowColumns = false;
            trMain.OptionsView.ShowIndicator = false;
            trMain.OptionsBehavior.Editable = false;
            trMain.FocusedNodeChanged += Drvo_FocusedNodeChanged;
            // Form1.TreeView2_DoubleClick 5656-5662: dvoklik ponovi radnju odabranog čvora (forma ili upit),
            // pa ponovni klik na isti čvor osvježi (TRE-02).
            trMain.DoubleClick += delegate
            {
                if (trMain.FocusedNode != null)
                    IzvrsiCvor(trMain.FocusedNode.Tag as Cvor);
            };
            var stavke = new XtraTabPage();
            stavke.Text = "Stavke";
            splitGrid.Panel2.Controls.Remove(gcDet);
            stavke.Controls.Add(gcDet);
            var tab = new XtraTabControl();
            _detaljTabovi = tab;
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
            kontekst.ProjektNaslov = Text;
            kontekst.HelpKod = AtributProjekta("HELPKOD").ToUpperInvariant();
            // Form1 3463: COMMANDTIMEOUT projekta vrijedi za svaki SQL (TMO-01).
            _vrijemeUpita = VrijemeUpita.Iz(_projekt.Atributi, VrijemeUpita.Zadano);
            kontekst.XmlPut = xml ?? "";
            kontekst.ProjektVerzija = AtributProjekta("VERSION");
            kontekst.Version = NgVerzija.Tekst();
            kontekst.Node = "null";
            _stablo.Kontekst = kontekst;
            _stablo.ImeXml = kontekst.XmlName;
            _master.Kontekst = kontekst;
            _detail.Kontekst = kontekst;
            try
            {
                _spoj = SpinVeza.SaAplikacijom(SpinVeza.Spoj(server, baza), xml);
                if (!_vezaPrijavljena)
                {
                    NgVeza.Prijavi(_spoj);
                    _vezaPrijavljena = true;
                }
            }
            catch (Exception ex)
            {
                PostaviStatus(ex.Message);
                return false;
            }

            // SWP-03: faze otvaranja u trag sweepa (TragPutanja je prazan izvan TestShella).
            Dijalog.Trag("podloga stablo");
            _stablo.Ucitaj(_projekt, Citaj);
            Dijalog.Trag("podloga combo");
            NapuniCombo();
            NapuniDrvo();
            _detail.Pripremi(_projekt);
            Dijalog.Trag("podloga dms");
            PripremiDms();
            Dijalog.Trag("podloga setdefaults");
            string zadano = _projekt.Atributi != null && _projekt.Atributi.Ima("SETDEFAULTS")
                ? _projekt.Atributi.Vrijednost("SETDEFAULTS") : "";
            _master.Clipboard = Carpo.NetGen.Kontrole.ClipboardNg.IzSetDefaults(zadano, kontekst, Citaj);
            // Isti CB vidi i dijalog (SqlTekst.ZaFormu) i gumb, kao cls1.CB u NetGenu (CB-01).
            kontekst.Clipboard = _master.Clipboard;
            // Form1.vb 3964-3968: stupac formcaption iz SETDEFAULTS je naslov podloge (KntIzv306090).
            string naslovIzZadanog;
            if (_master.Clipboard != null && _master.Clipboard.TryGetValue("formcaption=", out naslovIzZadanog)
                && !string.IsNullOrEmpty(naslovIzZadanog) && naslovIzZadanog != "null")
            {
                // strProjectCaption (Form1 3530, ime izgleda grida dijaloga) ostaje CAPTION projekta.
                Text = naslovIzZadanog;
            }
            Dijalog.Trag("podloga dozvole");
            UcitajDozvole();
            UcitajEditStupce();
            _master.Ucitaj(_projekt, null);
            Dijalog.Trag("podloga master");
            NapuniGrid();
            Dijalog.Trag("podloga gumbi");
            NapuniGumbe();
            // Form1.vb 3873 ssPermissionProvjeri prije ONLOAD2 (3906).
            PrimijeniDozvoleGumba();
            Dijalog.Trag("podloga onload2");
            PrimijeniOnLoad2();
            DodajGumbPomoci();
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
            Dijalog.Trag("drvo dodano, ExpandAll");
            trMain.ExpandAll();
        }

        private static int BrojCvorova(System.Collections.Generic.List<Cvor> cvorovi)
        {
            int n = 0;
            if (cvorovi == null)
                return 0;
            for (int i = 0; i < cvorovi.Count; i++)
                n += 1 + BrojCvorova(cvorovi[i].Djeca);
            return n;
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
            // Form1_Load 1682-1686: strNGParams iz Jupiterovog retka ide u NovaGenerickaForma (ULZ-03).
            string ng = (NgParametri ?? "").Trim();
            if (ng.Length > 0 && !IsDisposed)
            {
                NgDnevnik.Akcija(" strNGParams = " + ng);
                IzvrsiLanac(ng);
            }
        }

        /// <summary>Loader.strNGParams (Jupiter): naredba kao klik gumba nakon otvaranja (ULZ-03).</summary>
        public string NgParametri { get; set; }

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
            Dijalog.Trag("combo redova " + (t == null ? "null" : t.Rows.Count.ToString()));
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
            Dijalog.Trag("combo stablo " + vrijednost);
            _stablo.IzTeksta(vrijednost, _projekt, Citaj);
            Dijalog.Trag("combo cvorova " + BrojCvorova(_stablo.Stanje.Korijen).ToString());
            NapuniDrvo();
            Dijalog.Trag("combo drvo gotovo");
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
            IzvrsiCvor(e.Node == null ? null : e.Node.Tag as Cvor);
        }

        private void IzvrsiCvor(Cvor cvor)
        {
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

        /// <summary>
        /// PRINTMASTER/PRINTDET i "Ispis" u izborniku grida (Form1 IspisNovi, IspisGRIDREPX): Form1.vb 5934-5938,
        /// 6001: PAGEHEADERLEFT/CENTER/RIGHT kroz SrediSQL i SrediSQLPosljeNextForme s ParametriSaGrida i datumom
        /// za ispis; prazan centar je naslov (PRN-01, ISP-07).
        /// </summary>
        private void IspisiGrid(string naredba, bool detail)
        {
            GridControl gc = detail ? gcDet : gcGla;
            DataTable tab = gc == null ? null : gc.DataSource as DataTable;
            string orij = AtributProjekta("PAGEORIENTATION");
            PrintBridge.Ispisi(naredba, false, _master.Stanje.Id, _master.Kontekst.OperaterId, _spoj, _spoj, _master.Stanje.ZadnjiSql, AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), "",
                ZaglavljeIspisa("PAGEHEADERLEFT"), ZaglavljeIspisa("PAGEHEADERCENTER"), ZaglavljeIspisa("PAGEHEADERRIGHT"), Text, gc, orij, tab);
        }

        /// <summary>
        /// GridMaster/GridDet + ime XML-a (+ forma koja je napunila master, uz MULTIGRIDLAYOUT) + "-" + jezik
        /// (Form1 6206, 6237). Jezik je JupiterTranslatorMPJezik, prazan bez prevoditelja.
        /// </summary>
        private string DatotekaIzgleda(bool detail)
        {
            string xml = GridRaspored.ImeXml(_master.Kontekst == null ? "" : _master.Kontekst.XmlName);
            // Form1.vb 6198-6201, 5688: MASTERGRIDLAYOUTFILE (3 granule) je ime datoteke mastera, sređeno
            // SrediSQL + SrediSQLPosljeNextForme s parametrima reda.
            string vlastito = detail ? "" : AtributProjekta("MASTERGRIDLAYOUTFILE");
            if (vlastito.Length > 0 && _master.Kontekst != null)
                xml = SrediPoslije.Petlja(SqlTekst.Sredi(vlastito, _master.Kontekst), AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), _master.Kontekst.Clipboard, false).Trim();
            bool vise = AtributProjekta("MULTIGRIDLAYOUT").ToLowerInvariant() == "true";
            string forma = !detail && vise ? _aktivnaFormaMastera : "";
            return GridRaspored.Datoteka((detail ? "GridDet" : "GridMaster") + xml + forma + "-");
        }

        /// <summary>Form1 5448, 5489: AktivnaFormaMaster je forma s FILLEXPLORER koja je napunila master.</summary>
        private string _aktivnaFormaMastera = "";

        private string ZaglavljeIspisa(string atribut)
        {
            string sirovo = AtributProjekta(atribut);
            if (sirovo.Length == 0 || _master.Kontekst == null)
                return "";
            string t = SqlTekst.Sredi(sirovo, _master.Kontekst);
            return SrediPoslije.Petlja(t, _master.Kontekst.ParametriSaGrida, _master.Kontekst.Clipboard, true);
        }

        private string AtributProjekta(string ime)
        {
            if (_projekt == null || _projekt.Atributi == null || !_projekt.Atributi.Ima(ime))
                return "";
            return (_projekt.Atributi.Vrijednost(ime) ?? "").Trim();
        }

        /// <summary>
        /// Form1.ToolStripMenuItem1_Click/2_Click Case Else (5740-5763): s '~' je lanac (':' + naredba,
        /// '~' postaje '#'), inace NovaGenerickaForma.
        /// </summary>
        private void IzvrsiIzbornik(string naredba)
        {
            // Form1 5757: lanac s '~' ide u Rafalno (izvršava SQL), ne u NovaGenerickaForma (RAF-01).
            string lanac = GridIzbornik.Lanac(naredba, true);
            if (lanac.Length > 0)
                IzvrsiRafalno(lanac);
            else
                IzvrsiLanac(naredba);
        }

        /// <summary>Form1.GetDetID (5601-5615): "id" detail reda, inace prvi stupac podatka.</summary>
        private string DetaljId()
        {
            if (gvDet == null || gvDet.FocusedRowHandle < 0 || gvDet.RowCount == 0)
                return "0";
            object v = gvDet.Columns.ColumnByFieldName("id") != null ? gvDet.GetFocusedRowCellValue("id") : null;
            if ((v == null || v == DBNull.Value) && gvDet.Columns.Count > 0)
                v = gvDet.GetFocusedRowCellValue(gvDet.Columns[0]);
            string t = v == null || v == DBNull.Value ? "" : Convert.ToString(v).Trim();
            return t.Length == 0 ? "0" : t;
        }

        private void OtvoriFormu(string naredba)
        {
            OtvoriFormu(naredba, null, false);
        }

        /// <summary>
        /// Form1.NovaGenerickaForma (5290-5520). <paramref name="idSljedece"/> je Tid iz NEXTFORM petlje (5414):
        /// sljedeća forma se otvara s ID-om iz rezultata ONSUBMIT-a prethodne, ne s fokusiranim redom.
        /// <paramref name="izDijaloga"/>: dijete otvoreno iz dijaloga ide kao Form2.NovaGenerickaForma2 (4384-4479):
        /// ONSUBMIT jednom, NEXTFORM, bez FILLEXPLORER grana i bez osvježavanja mastera (roditelj radi RefreshKontrola).
        /// </summary>
        private void OtvoriFormu(string naredba, string idSljedece, bool izDijaloga)
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
            // Glavni XML, pa INCLUDE (Form2.ProcitajXML 840-934, XML-02).
            NgForm forma = IncludeForme.Nadji(_projekt, ime);
            if (forma == null)
            {
                NgDnevnik.Akcija("nema forme " + ime);
                OsvjeziMaster();
                return;
            }
            // Form2.NovaGenerickaForma2 4398 postavi strPrevParametri djeteta prije nego tpP uđe u strParametriSvi (4406),
            // pa dijete otvoreno iz dijaloga te parametre ne vidi; iz podloge (Form1 5313-5314) vidi.
            if (zagrada > 0 && !izDijaloga)
                ParametriIzImenaForme(trazeno, zagrada);
            // Form2 1035: COMMANDTIMEOUT forme zamijeni strCommandTimeout i ostane (TMO-01).
            int rok = VrijemeUpita.Iz(forma.Atributi, _vrijemeUpita);
            if (rok != _vrijemeUpita)
            {
                _vrijemeUpita = rok;
                _master.Stanje.TimeoutSekundi = rok;
            }
            if (MultiOdabir.Je(ime))
                NapuniOdabir(ime);
            if (!string.IsNullOrEmpty(idSljedece))
            {
                // 5414-5415: Form2c.ID = Tid, cls1.MasterID = Tid.
                _master.Stanje.Id = idSljedece;
                _master.Kontekst.Id = idSljedece;
            }
            if (forma.Kontrole == null || forma.Kontrole.Count == 0)
            {
                // Form1.vb 5369-5375: bez kontrola dijalog se ne otvara, OKClick je True i ide se odmah na NEXTFORM
                // i FILLEXPLORER grane sa strStora bez parametara (PokupiParametre nema što skupiti).
                string bezKontrola = Lanac.Lijepi((forma.OnSubmit ?? "").Trim(), "");
                if (bezKontrola.Length > 0)
                    bezKontrola = Lanac.BezKrajnjegZareza(SqlTekst.ZaGumb(bezKontrola, _master.Kontekst));
                _dubinaForme++;
                try { NakonDijaloga(forma, ime, bezKontrola, izDijaloga); }
                finally { _dubinaForme--; }
                return;
            }
            _dubinaForme++;
            try
            {
                var dij = new Dijalog();
                dij.JupiterNeSaljeGumb(DijalogTok.JeJupiter(_server) && !DijalogTok.Produkcija);
                // Form1 5381-5520: ONSUBMIT izvršava podloga, jednom, nakon zatvaranja dijaloga (PN-01).
                dij.OnSubmitSaljePodloga = true;
                dij.Konekcija = _spoj;
                dij.Operater = _master.Kontekst.OperaterId;
                dij.Id = _master.Stanje.Id;
                dij.ZadnjiSql = _master.Stanje.ZadnjiSql;
                dij.OtvoriDrugu = delegate (string imeDjeteta)
                {
                    // Form2.vb 3906: strSviParametriuModulu vrijedi i u djetetu.
                    if (_master.Kontekst != null)
                        _master.Kontekst.ParametriSaGrida = ParametriModula.Dodaj(_master.Kontekst.ParametriSaGrida, dij.ParametriVise());
                    OtvoriFormu(imeDjeteta, null, true);
                };
                // Form1.NovaGenerickaForma 5295: Form2b.DetID = GetDetID() (@@DETID u dijalogu, DET-01).
                if (_master.Kontekst != null)
                    _master.Kontekst.DetId = DetaljId();
                dij.Prikazi(forma, SqlTekst.ZaFormu(_master.Kontekst), CitajZaDijalog, true, this);
                string iz = dij.Vrijednost("izvjestaj");
                if (!string.IsNullOrEmpty(iz))
                    _polja["izvjestaj"] = iz;
                if (dij.DialogResult != DialogResult.OK)
                    return;
                if (_master.Kontekst != null)
                    _master.Kontekst.ParametriSaGrida = ParametriModula.Dodaj(_master.Kontekst.ParametriSaGrida, dij.ParametriVise());
                // strStora & " " & strParametri, sređen (Form2.Button1_Click 3367-3388, Form1 5450); dijalog ga nije slao.
                string stora = (dij.OnSubmitPripremljen ?? "").Trim();
                NakonDijaloga(forma, ime, stora, izDijaloga);
            }
            finally
            {
                _dubinaForme--;
            }
        }

        /// <summary>
        /// Form1.NovaGenerickaForma2 5298-5313 (1.10.2009.): IMEFORME(@pero=@@id,@djuro=@@gridX). Parametri u zagradi
        /// se srede kao SQL s parametrima reda, dio s @id= postavi ID dijaloga, a cijeli niz ide ispred
        /// ParametriSaGrida, pa ih dijalog vidi kao @@pero u ONLOAD-u, naslovu i ONSUBMIT-u (PN-14). 24 gumba u
        /// korpusu (KOD_Diskete IZMJENA(@oopdiskprijenosglaid=@@id), LVObracun :UPDATELVDET(@lvdetid=@@lvdetid)...).
        /// </summary>
        private void ParametriIzImenaForme(string trazeno, int zagrada)
        {
            string unutra = trazeno.Substring(zagrada + 1);
            if (unutra.EndsWith(")", StringComparison.Ordinal))
                unutra = unutra.Substring(0, unutra.Length - 1);
            SqlKontekst k = _master.Kontekst;
            if (k == null)
                return;
            string tpP = SrediPoslije.Petlja(SqlTekst.Sredi(unutra, k), k.ParametriSaGrida ?? "", k.Clipboard, false);
            if (tpP.Trim().Length == 0)
                return;
            string[] dijelovi = tpP.Trim().Split(',');
            for (int j = 0; j < dijelovi.Length; j++)
            {
                string dio = dijelovi[j].Trim();
                if (dio.StartsWith("@id=", StringComparison.OrdinalIgnoreCase))
                {
                    string id = dio.Substring(4).Trim();
                    _master.Stanje.Id = id;
                    k.Id = id;
                }
            }
            k.ParametriSaGrida = tpP + "," + (k.ParametriSaGrida ?? "");
        }

        /// <summary>
        /// Form1.NovaGenerickaForma 5381-5520 nakon OK: NEXTFORM petlja (5381-5415), pa FILLEXPLORER grane
        /// (5445-5507). ONSUBMIT ide serveru jednom (PN-01), s parametrima dijaloga (PN-02); sljedeća forma dobije
        /// ID iz rezultata (PN-05); treeandrefreshmaster, treeandmaster i false kao u VB-u (PN-07).
        /// </summary>
        private void NakonDijaloga(NgForm forma, string ime, string stora, bool izDijaloga)
        {
            string sljedeca = DijalogTok.NextForm(forma);
            if (sljedeca.Length > 0)
            {
                // 5385-5390 (Form2 4432-4440): Tid = cf.ID, a ako strStora vrati red, prva ćelija.
                string tid = _master.Stanje.Id;
                if (stora.Length > 0)
                {
                    DataTable t = IzvrsiOnSubmit(stora);
                    if (t != null && t.Rows.Count > 0 && t.Columns.Count > 0 && t.Rows[0][0] != DBNull.Value)
                        tid = Convert.ToString(t.Rows[0][0]).Trim();
                    else if (!izDijaloga)
                    {
                        // Klik pregled na spinu: stub spPrnTroskoviGlaAzur ne vraća id, pa ga uzme iz tablice.
                        ZapamtiNoviTrosak();
                        tid = _master.Stanje.Id;
                    }
                }
                // 5409: PokaziUpit(strZadnjiSQLsakojimjepunjenGrid), pa 5414 Form2c.ID = Tid. Iz dijaloga (4442) samo RefreshKontrola.
                if (!izDijaloga)
                    OsvjeziMaster();
                OtvoriFormu(sljedeca, tid, izDijaloga);
                return;
            }
            if (izDijaloga)
            {
                // Form2 4468-4477: ExecSql(strStora & strParametri), pa RefreshKontrola roditelja (RAC-01 u Dijalog).
                if (stora.Length > 0)
                    IzvrsiOnSubmit(stora);
                return;
            }
            string fill = DijalogTok.FillExplorerVrijednost(forma);
            if (fill == "true")
            {
                // 5445-5453: master je rezultat ONSUBMIT-a.
                if (stora.Length > 0)
                    NapuniMasterIz(stora, ime);
                return;
            }
            if (fill == "false")
            {
                // 5455-5466: ExecSql, ili forma kad tekst počinje s ':'; master se ne osvježava.
                if (stora.StartsWith(":", StringComparison.Ordinal))
                    IzvrsiLanac(stora);
                else if (stora.Length > 0)
                    IzvrsiOnSubmit(stora);
                return;
            }
            if (DijalogTok.OsvjeziDetalj(forma))
            {
                // 5467-5471: PokaziUpitDetalj(strStartupStoraDetalj).
                OsvjeziDetail();
                return;
            }
            if (DijalogTok.OsvjeziStablo(forma))
            {
                // 5472-5474: NapuniDrvoVrijeme.
                OsvjeziStabloPodloge();
                return;
            }
            if (fill == "treeandrefreshmaster")
            {
                // 5475-5486: stablo, ExecSql(ONSUBMIT), zadnji master, fokus stabla.
                OsvjeziStabloPodloge();
                if (stora.Length > 0)
                    IzvrsiOnSubmit(stora);
                _aktivnaFormaMastera = ime;
                if (_zadnjiRed)
                    OsvjeziMaster();
                trMain.Focus();
                return;
            }
            if (fill == "treeandmaster")
            {
                // 5487-5500: stablo, pa master iz ONSUBMIT-a.
                OsvjeziStabloPodloge();
                if (stora.Length > 0)
                    NapuniMasterIz(stora, ime);
                trMain.Focus();
                return;
            }
            // 5501-5507: ExecSql(ONSUBMIT), pa zadnji master ako je ovo zadnji red.
            if (stora.Length > 0)
            {
                IzvrsiOnSubmit(stora);
                ZapamtiNoviTrosak();
            }
            _aktivnaFormaMastera = ime;
            if (_zadnjiRed)
                OsvjeziMaster();
        }

        /// <summary>cls1.ExecSql(strStora): jednom, kroz vezu granule; greška je u statusu, lanac ide dalje kao u VB-u.</summary>
        private DataTable IzvrsiOnSubmit(string stora)
        {
            // Kao JupiterNeSaljeGumb: na jupiter2019/2022 izvan produkcije gumb i ONSUBMIT se ne šalju (R89).
            if (DijalogTok.JeJupiter(_server) && !DijalogTok.Produkcija)
            {
                NgDnevnik.Akcija("jupiter: ONSUBMIT se ne šalje: " + stora);
                return null;
            }
            if (!DijalogTok.SmijePoslati(stora))
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + stora);
                return null;
            }
            NgDnevnik.Sql(stora);
            return Citaj(stora);
        }

        /// <summary>PokaziUpit(ONSUBMIT): master grid se puni rezultatom ONSUBMIT-a (5450, 5497).</summary>
        private void NapuniMasterIz(string stora, string ime)
        {
            string sql = stora.Replace("~~", ",");
            _master.Stanje.IzvorSql = stora;
            _master.Stanje.ZadnjiSql = sql;
            _master.Stanje.SqlZaSlanje = sql;
            _master.Stanje.Odluka = SqlTekst.Odluka(sql);
            _master.Stanje.BlokiranoPisanje = !DijalogTok.SmijePoslati(sql);
            _aktivnaFormaMastera = ime;
            if (_master.Stanje.Odluka == "salji" && !_master.Stanje.BlokiranoPisanje)
                NapuniGrid();
        }

        private void OsvjeziStabloPodloge()
        {
            _stablo.Ucitaj(_projekt, Citaj);
            NapuniDrvo();
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

        /// <summary>
        /// Form1.PokaziUpit(strZadnjiSQLsakojimjepunjenGrid) (4577, 5409, 5519, 6675): zadnji SQL kojim je
        /// grid napunjen, s filtrom forme, a ne ONLOAD ispočetka (REF-01). Bez zadnjeg SQL-a ONLOAD.
        /// </summary>
        private void OsvjeziMaster()
        {
            if ((DijalogTok.JeJupiter(_server) && !DijalogTok.Produkcija) || _projekt == null)
                return;
            if (_master.Stanje != null && _master.Stanje.Odluka == "salji" && !_master.Stanje.BlokiranoPisanje
                && !string.IsNullOrEmpty(_master.Stanje.SqlZaSlanje))
            {
                NapuniGrid();
                return;
            }
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
                        // FOR-02: NetGen na podlozi ne vrti FOREACH (tijelo se preskoči), pa Carpo ni u
                        // Produkcija ne šalje upis iz petlje dok se o FOR-02 ne odluči.
                        if (ZastitaSql.Tekst(sqlovi[k]).Pise)
                            continue;
                        try { Citaj(sqlovi[k]); }
                        catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sqlovi[k]); }
                    }
                    continue;
                }
                if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    // Form1.vb 4875-4899: izvrši SQL, prvi red po natpisu gumba (DodatnoPostimajKontrolu, BTN-03).
                    if (!IzvrsiGumbSelect(s))
                    {
                        string stanje = Lanac.GumbIzCase(s, Polje("izvjestaj"));
                        int jednako = stanje.IndexOf('=');
                        if (jednako > 0)
                            PostaviGumb(stanje.Substring(0, jednako), stanje.Substring(jednako + 1) != "disable");
                    }
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
                    OtvoriZaRedove(OtvoriNaredba.Ime(s));
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
                    // NovaGenerickaForma2 5003-5054: ispis dokumenta (ne PRINTMASTER/PRINTDET, 4945-4962) ide
                    // za svaki označeni red, kao forma (ISP-06).
                    int[] oznaceni = vrsta.Length == 0 ? OznaceniRedovi() : new int[0];
                    if (oznaceni.Length > 1)
                    {
                        for (int r = 0; r < oznaceni.Length && !IsDisposed; r++)
                        {
                            gvGla.FocusedRowHandle = oznaceni[r];
                            SpojiId();
                            PrintBridge.Ispisi(s, false, _master.Stanje.Id, _master.Kontekst.OperaterId, _spoj, _spoj, _master.Stanje.ZadnjiSql, AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), "",
                                "", "", "", Text, gcGla, "", null);
                        }
                        continue;
                    }
                    IspisiGrid(s, vrsta == "detail");
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
                    OtvoriZaRedove(s);
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
            // Form1.PokaziUpit 1837-1839: tID = GetID() prije punjenja.
            string staro = gvGla.FocusedRowHandle >= 0 && gvGla.DataRowCount > 0 ? IdCelije(gvGla.FocusedRowHandle) : "";
            _punim = true;
            _master.IzvrsiCitanje(_spoj);
            VeziGrid(gvGla, gcGla, _master.Stanje.Tablica, _master.Stanje.Kolone, _master.Stanje.Poruka, _master.Stanje.CheckboxVidljiv, _master.Stanje.GroupByBox, _master.Stanje.FilterRed || _master.Stanje.FilterNamjernoOdstupanje || !_master.Stanje.FilterIzXml);
            PozicionirajMaster(staro);
            SpojiId();
            _punim = false;
            OsvjeziDetail();
        }

        /// <summary>
        /// Form1.PokaziUpit 2016-2047: red s istim prvim stupcem kao prije (Find na Columns(2)), pa movetoid
        /// trenutnog reda u stupcu id, pa MASTERMOVELAST (FOK-01).
        /// </summary>
        private void PozicionirajMaster(string staro)
        {
            DataTable t = _master.Stanje.Tablica;
            if (t == null || t.Columns.Count == 0 || gvGla.DataRowCount == 0)
                return;
            string prvi = t.Columns[0].ColumnName;
            if (!string.IsNullOrEmpty(staro) && staro != "0")
            {
                int h = NadjiRed(prvi, staro);
                if (h >= 0)
                    gvGla.FocusedRowHandle = h;
            }
            if (t.Columns.Contains("id") && t.Columns.Contains("movetoid") && gvGla.FocusedRowHandle >= 0)
            {
                object m = gvGla.GetRowCellValue(gvGla.FocusedRowHandle, "movetoid");
                string cilj = m == null || m == DBNull.Value ? "" : Convert.ToString(m).Trim();
                int h = cilj.Length == 0 ? -1 : NadjiRed("id", cilj);
                if (h >= 0)
                    gvGla.FocusedRowHandle = h;
            }
            if (_master.Stanje.FocusRazlog == "MASTERMOVELAST")
                gvGla.MoveLast();
        }

        private int NadjiRed(string stupac, string vrijednost)
        {
            for (int r = 0; r < gvGla.DataRowCount; r++)
            {
                int h = gvGla.GetRowHandle(r);
                object v = gvGla.GetRowCellValue(h, stupac);
                if (v != null && v != DBNull.Value && Convert.ToString(v).Trim() == vrijednost)
                    return h;
            }
            return -1;
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
            NgColumnGroup mst = _projekt == null ? null : _projekt.NadjiGrupu("MASTERCOLUMNS");
            _detail.PrimijeniTablicu(t, def, mst == null ? null : mst.HeaderAlignment);
            VeziGrid(gvDet, gcDet, t, _detail.Stanje.Kolone, _detail.Stanje.Poruka, _detail.Stanje.CheckboxVidljiv, null, true);
            if (_detail.Stanje.FokusZadnji && gvDet.RowCount > 0)
                gvDet.FocusedRowHandle = gvDet.RowCount - 1;
            // PokaziUpitDetalj 2433: DMS kartica se puni nakon stavki (DMS-01).
            OsvjeziDms();
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
            // Class1.CreateGridColumns i Form1.CustomFormatKolona: zajedno s gridom dijaloga (DGR-01).
            GridIzgled.PrimijeniKolone(pogled, kolone);
            PodesiUredjivanje(pogled, kolone);
            PrimijeniUkupno(pogled, tablica, kolone);
            // Form1.vb 2009 / LoadLayout2: spremljeni izgled grida nakon CustomFormatKolona (GRL-01).
            GridRaspored.Ucitaj(pogled, DatotekaIzgleda(pogled == gvDet), AtributProjekta("FILTERSAVE").ToLowerInvariant() == "true");
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
            PrimijeniStanjeGumba(t.Rows[0]);
        }

        /// <summary>Form1.DodatnoPostimajKontrolu (4013) za Button1..10 po natpisu: stupac s imenom gumba.</summary>
        private void PrimijeniStanjeGumba(DataRow row)
        {
            if (row == null)
                return;
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                var gumb = pnlButtons.Controls[i] as SimpleButton;
                if (gumb == null)
                    continue;
                string natpis = (gumb.Text ?? "").Replace("\r\n", "").Trim();
                if (natpis.Length == 0 || !row.Table.Columns.Contains(natpis))
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
                _sqlPoruka = false;
                return null;
            }
            _brojUpita++;
            if (sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                sql = SrediPoslije.Petlja(sql, "", null, false);
            if (sql.IndexOf("cis.porezna-uprava.hr", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _sqlGreska = "nije poslano na produkcijski URL Porezne";
                _sqlPoruka = false;
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
                // Class1.ExecSql: svaki upit kroz istu otvorenu vezu (SES-01).
                return NgVeza.Citaj(_spoj, sql, VrijemeUpita.Stvarno(_vrijemeUpita));
            }
            catch (Exception ex)
            {
                _zadnjaPoruka = ex.Message ?? "";
                // Class1.ExecSql 157-166: greška o ssoperateri, sspermission, NetGenDBFilter... je tiha.
                bool tiho = filter || SqlPoruka.Tiha(ex.Message);
                if (!tiho)
                {
                    _sqlGreska = ex.Message + " | " + sql;
                    var sqlEx = ex as System.Data.SqlClient.SqlException;
                    _sqlPoruka = sqlEx != null && sqlEx.Number >= 50000;
                }
                NgDnevnik.Greska(ex.Message + " | " + sql);
                if (!tiho)
                    PostaviStatus(ex.Message);
                return null;
            }
        }

        /// <summary>Gornja granica roka za sweep (TestShell); 0 je bez granice. COMMANDTIMEOUT je TMO-01.</summary>
        public static int TimeoutUpita
        {
            get { return VrijemeUpita.Najvise; }
            set { VrijemeUpita.Najvise = value; }
        }

        /// <summary>strCommandTimeout ove podloge: PROJECT COMMANDTIMEOUT, pa zadnja forma koja ga ima (TMO-01).</summary>
        private int _vrijemeUpita = VrijemeUpita.Zadano;
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
            NgPoruka.Tiho = true;
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
                NgPoruka.Tiho = false;
                ZaustaviZatvaranje();
            }
        }

        private List<string> ProvediKlikoveUnutra(List<string> redovi, bool upis)
        {
            string granula = _projekt == null ? Text : (_projekt.Naslov ?? Text);
            int master = _master.Stanje.Tablica == null ? 0 : _master.Stanje.Tablica.Rows.Count;
            bool sqlMaster = MasterPuni();
            string masterGreska = MasterGreska();
            if (!string.IsNullOrEmpty(_sqlGreska))
                redovi.Add(Linija(granula, "master", "grid", StatusSqlGreske, _sqlGreska, "netgen/Form1.vb:3882"));
            else if (masterGreska.Length > 0)
                // PN-08: greška master grida ide kroz NgVeza u _master.Stanje, ne u _sqlGreska; prije je bila PRAZAN (= CISTO u sweepu).
                redovi.Add(Linija(granula, "master", "grid", StatusMasterGreske, masterGreska, "netgen/Form1.vb:3882"));
            else if (sqlMaster && master == 0)
                redovi.Add(Linija(granula, "master", "grid", "PRAZAN", "NetGen puni master, grid je prazan", "netgen/Form1.vb:3882"));
            else
                redovi.Add(Linija(granula, "master", "grid", "OK", master.ToString() + " redova", "netgen/Form1.vb:3882"));

            if (!_detail.Stanje.Nema && !_detail.Stanje.OtvaraFormu)
            {
                int det = _detail.Stanje.Tablica == null ? 0 : _detail.Stanje.Tablica.Rows.Count;
                if (!string.IsNullOrEmpty(_sqlGreska) && det == 0)
                    redovi.Add(Linija(granula, "detail", "grid", StatusSqlGreske, _sqlGreska, "netgen/Form1.vb:2258"));
                else if (master > 0 && det == 0)
                    redovi.Add(Linija(granula, "detail", "grid", "PRAZAN", "master ima red, detail je prazan", "netgen/Form1.vb:2258"));
                else
                    redovi.Add(Linija(granula, "detail", "grid", "OK", det.ToString() + " redova", "netgen/Form1.vb:2258"));
            }

            Dijalog.Trag("klik kartice");
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
                _sqlPoruka = false;
                try
                {
                    PokreniZatvaranje();
                    Dijalog.Trag("klik stablo");
                    trMain.FocusedNode = trMain.Nodes[0];
                    Application.DoEvents();
                    if (!string.IsNullOrEmpty(_sqlGreska))
                        redovi.Add(Linija(granula, "stablo", "cvor", StatusSqlGreske, _sqlGreska, "netgen/Form1.vb:7426"));
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
                if (!upis && !DijalogTok.UpisNaSpinu(_server, _baza) && !OtvaraFormu(naredba))
                {
                    // SWP-02: izvan spina (npr. backup klijenta) klikne se samo gumb koji otvara formu granule;
                    // forma se zatvori s Odustani. Izravan SQL ili procedura se ne šalje.
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", "nije spin: izravan SQL se ne šalje", "netgen/Form2.vb:3367"));
                    continue;
                }
                if (!upis && Destruktivan(naredba, natpis))
                {
                    redovi.Add(Linija(granula, "gumb", natpis, "PRESKOCEN", naredba, "netgen/Form2.vb:3367"));
                    continue;
                }
                _sqlGreska = "";
                _sqlPoruka = false;
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
                    else if (gvGla.OptionsSelection.MultiSelectMode == GridMultiSelectMode.CheckBoxRowSelect)
                    {
                        // NovaGenerickaForma2 otvara formu za svaki označeni red (OtvoriZaRedove). Klik-pregled
                        // ne smije naslijediti kvačice prethodnog MULTISELECT gumba.
                        gvGla.ClearSelection();
                    }
                    PokreniZatvaranje();
                    string masterPrije = MasterGreska();
                    g.PerformClick();
                    Application.DoEvents();
                    string provjera = upis ? ProvjeriUpis(naredba) : "";
                    string masterPoslije = MasterGreska();
                    if (!string.IsNullOrEmpty(_sqlGreska))
                        redovi.Add(Linija(granula, "gumb", natpis, StatusSqlGreske, _sqlGreska, "netgen/Form2.vb:3377"));
                    else if (masterPoslije.Length > 0 && masterPoslije != masterPrije)
                        // PN-08: master napunjen nakon dijaloga (FILLEXPLORER, REFRESHMASTER) s greškom.
                        redovi.Add(Linija(granula, "gumb", natpis, StatusMasterGreske, masterPoslije, "netgen/Form1.vb:5450"));
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
            Citaj(MultiOdabir.ObrisiKorisnika());
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
            // Form1.vb 5003-5006: MULTISELECT bez kvačica (0 ili 1) ubaci GetID() trenutnog reda.
            if (ids.Count == 0 && !MultiOdabir.Plavi(ime) && gvGla.FocusedRowHandle >= 0)
            {
                string id = IdCelije(gvGla.FocusedRowHandle);
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

        /// <summary>SWP-02: prvi dio naredbe gumba (do #) je forma ove granule ili uključenog XML-a.</summary>
        private bool OtvaraFormu(string naredba)
        {
            if (_projekt == null || string.IsNullOrWhiteSpace(naredba))
                return false;
            string ime = naredba.Trim();
            int hash = ime.IndexOf('#');
            if (hash >= 0)
                ime = ime.Substring(0, hash);
            ime = ime.Trim();
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1).Trim();
            int zagrada = ime.IndexOf('(');
            if (zagrada > 0)
                ime = ime.Substring(0, zagrada).Trim();
            return ime.Length > 0 && IncludeForme.Nadji(_projekt, ime) != null;
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
            NgForm f = IncludeForme.Nadji(_projekt, ime.Trim());
            return f != null && (Pise(f.OnSubmit) || Pise(f.OnLoad));
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

        /// <summary>Form1_KeyDown 1375-1378: PokupiParametreSaGrida, pa CBLog s PARAMATERISAMASTERIDETAILGRIDA.</summary>
        private void OtvoriCarpoLog()
        {
            string grid = _master == null || _master.Kontekst == null ? "" : (_master.Kontekst.ParametriSaGrida ?? "");
            string greska = CarpoLogProzor.Pokazi(this, "PARAMATERISAMASTERIDETAILGRIDA=" + grid);
            if (greska != null)
                PostaviStatus(greska);
        }
    }
}
