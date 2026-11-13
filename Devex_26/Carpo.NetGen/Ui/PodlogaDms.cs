using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Poziv;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// DMS kartica uz stavke (Form1 TabPage4/GridEX3): DMSDET (9 granula) puni popis nakon svakog
    /// osvježavanja detalja (2433), naslov "DMS (n)". Desni klik i dvoklik kao ContextMenuStrip4 (DMS-01).
    /// </summary>
    public sealed partial class Podloga
    {
        private XtraTabControl _detaljTabovi;
        private XtraTabPage _dmsStranica;
        private GridControl _dmsGrid;
        private GridView _dmsPogled;

        /// <summary>2433-2437: bez DMSDET kartica se uklanja.</summary>
        private void PripremiDms()
        {
            string dms = AtributProjekta("DMSDET");
            if (dms.Length == 0 || _detaljTabovi == null)
            {
                if (_dmsStranica != null)
                    _dmsStranica.PageVisible = false;
                return;
            }
            if (_dmsStranica == null)
            {
                _dmsGrid = new GridControl();
                _dmsGrid.Dock = DockStyle.Fill;
                _dmsPogled = new GridView(_dmsGrid);
                _dmsGrid.MainView = _dmsPogled;
                _dmsGrid.ViewCollection.Add(_dmsPogled);
                PodesiPogled(_dmsPogled, false);
                _dmsPogled.OptionsSelection.MultiSelect = false;
                _dmsPogled.PopupMenuShowing += DmsIzbornik;
                _dmsPogled.DoubleClick += delegate { DmsPogledaj(); };
                _dmsStranica = new XtraTabPage();
                _dmsStranica.Controls.Add(_dmsGrid);
                _detaljTabovi.TabPages.Add(_dmsStranica);
            }
            _dmsStranica.Text = "DMS (0)";
            _dmsStranica.PageVisible = true;
        }

        /// <summary>DMS_pOkazi_Upit 2447-2464: SrediSQL(DMSDET).Trim pa parametri reda mastera.</summary>
        private void OsvjeziDms()
        {
            if (_dmsStranica == null || !_dmsStranica.PageVisible)
                return;
            string dms = AtributProjekta("DMSDET");
            _dmsStranica.Text = "DMS (0)";
            var k = _master.Kontekst;
            string sql = SrediPoslije.Zamijeni(SqlTekst.Sredi(dms, k).Trim(), k.ParametriSaGrida, k, k.Clipboard, false);
            DataTable t = Citaj(sql);
            _dmsGrid.DataSource = t;
            _dmsPogled.PopulateColumns();
            _dmsStranica.Text = "DMS (" + (t == null ? 0 : t.Rows.Count).ToString() + ")";
        }

        private void DmsIzbornik(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row && e.MenuType != GridMenuType.User)
                return;
            if (e.Menu == null)
                e.Menu = new GridViewMenu(_dmsPogled);
            DataRow red = _dmsPogled.GetFocusedDataRow();
            bool preuzet = DmsPoziv.Preuzet(red);
            e.Menu.Items.Add(new DXMenuItem("Pogledaj", delegate { DmsPogledaj(); }));
            e.Menu.Items.Add(new DXMenuItem("Spremi na disk", delegate { DmsSpremi(); }));
            e.Menu.Items.Add(new DXMenuItem("Obriši", delegate { DmsObrisi(); }));
            var preuzmi = new DXMenuItem("Preuzmi za potpis", delegate { DmsPotpis(true); });
            preuzmi.BeginGroup = true;
            preuzmi.Enabled = !preuzet;
            e.Menu.Items.Add(preuzmi);
            var vrati = new DXMenuItem("Vrati potpisano", delegate { DmsPotpis(false); });
            vrati.Enabled = preuzet;
            e.Menu.Items.Add(vrati);
            var odustani = new DXMenuItem("Odustani od potpisivanja", delegate { DmsOdustani(); });
            odustani.BeginGroup = true;
            odustani.Enabled = preuzet;
            e.Menu.Items.Add(odustani);
            e.Allow = true;
        }

        private DataRow DmsRed()
        {
            return _dmsPogled == null ? null : _dmsPogled.GetFocusedDataRow();
        }

        /// <summary>DMS_Pogledaj 8640-8675 i GridEX3_RowDoubleClick: DmsManagerV1.Start.OpenDocument.</summary>
        private void DmsPogledaj()
        {
            DataRow red = DmsRed();
            if (red == null)
                return;
            DmsGreska(DmsPoziv.Pozovi(DmsPoziv.Start, "OpenDocument", _server, _baza, "", "", _master.Kontekst.OperaterId, DmsPoziv.Celija(red, "dokid"), ""));
        }

        /// <summary>DMS_SpremiNaDisk 8600-8630.</summary>
        private void DmsSpremi()
        {
            DataRow red = DmsRed();
            if (red == null)
                return;
            DmsGreska(DmsPoziv.Pozovi(DmsPoziv.Start, "SaveDocuments", _server, _baza, "", "", _master.Kontekst.OperaterId, DmsPoziv.Celija(red, "dokid")));
        }

        /// <summary>DMS_preuzmizapotpis (CheckOut) i DMS_vratipotpisano (CheckIn), pa osvježi detalj i DMS.</summary>
        private void DmsPotpis(bool preuzmi)
        {
            DataRow red = DmsRed();
            if (red == null)
                return;
            string greska = preuzmi
                ? DmsPoziv.Pozovi(DmsPoziv.Dokumenti, "DocumentCheckOut", _server, _baza, "", "", _master.Kontekst.OperaterId, DmsPoziv.Celija(red, "dokid"), DmsPoziv.Celija(red, "dokument"))
                : DmsPoziv.Pozovi(DmsPoziv.Dokumenti, "DocumentCheckIn", _server, _baza, "", "", _master.Kontekst.OperaterId, DmsPoziv.Celija(red, "dokid"), "");
            if (DmsGreska(greska))
                return;
            DmsOsvjezi();
        }

        /// <summary>DMS_odustani 8463: dms.spSetDocumentUndoCheckOutOperater; SQL grešku pokaže sam ExecSql.</summary>
        private void DmsOdustani()
        {
            DataRow red = DmsRed();
            if (red == null)
                return;
            if (IzvrsiRafalSql(DmsPoziv.OdustaniSql(DmsPoziv.Celija(red, "dokid"))).Length > 0)
                return;
            DmsOsvjezi();
        }

        /// <summary>DMS_Obrisi 8575-8590: spNetGenDMSAzur @action='delete', pa osvježi i kad nije uspjelo.</summary>
        private void DmsObrisi()
        {
            DataRow red = DmsRed();
            if (red == null)
                return;
            IzvrsiRafalSql(DmsPoziv.ObrisiSql(DmsPoziv.Celija(red, "entityid"), DmsPoziv.Celija(red, "dokid")));
            DmsOsvjezi();
        }

        private void DmsOsvjezi()
        {
            // 8475-8478: If GridEX1.RowCount > 0 Then PokaziUpitDetalj; pa DMS_pOkazi_Upit.
            if (gvGla.DataRowCount > 0)
                OsvjeziDetail();
            else
                OsvjeziDms();
        }

        private bool DmsGreska(string greska)
        {
            if (string.IsNullOrEmpty(greska))
                return false;
            NgPoruka.Info(this, "\r\n" + greska + "\r\n\r\n", "Poruka");
            return true;
        }
    }
}
