using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Form3 (trazilica) u DevExpressu: grid rezultata, Odaberi i Odustani. Enter, dvoklik ili Odaberi
    /// vrate red; Escape ili Odustani vrate null (Form3.Button2, Form3_KeyDown 340-344, GridEX1_KeyDown 407-415).
    /// </summary>
    public sealed class TrazilicaProzor : XtraForm
    {
        /// <summary>Za testove bez prozora: dobije rezultat i vrati indeks reda ili -1 (odustani).</summary>
        public static Func<DataTable, int> OdabirZaTest;

        private readonly GridControl _grid = new GridControl();
        private readonly GridView _pogled = new GridView();
        private readonly DataTable _tablica;
        private DataRow _odabran;

        /// <summary>
        /// imeIzgleda: "GridTrazilica-xml-forma" (Form3 spremipostavke 38-48). Prazno znači bez pamćenja izgleda.
        /// </summary>
        public static DataRow Odaberi(IWin32Window vlasnik, DataTable tablica, string trazeno, string columnCaption, string columnWidth, string autoResize, string imeIzgleda)
        {
            if (tablica == null || tablica.Rows.Count == 0)
                return null;
            if (OdabirZaTest != null)
            {
                int i = OdabirZaTest(tablica);
                return i >= 0 && i < tablica.Rows.Count ? tablica.Rows[i] : null;
            }
            using (var p = new TrazilicaProzor(tablica, trazeno, columnCaption, columnWidth, autoResize, imeIzgleda))
            {
                if (vlasnik != null)
                    p.ShowDialog(vlasnik);
                else
                    p.ShowDialog();
                return p._odabran;
            }
        }

        private TrazilicaProzor(DataTable tablica, string trazeno, string columnCaption, string columnWidth, string autoResize, string imeIzgleda)
        {
            _tablica = tablica;
            // Form3.Trazilica 105: naslov je trazeni tekst.
            Text = trazeno ?? "";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            KeyPreview = true;
            MinimizeBox = false;
            Size = new Size(720, 420);

            _grid.MainView = _pogled;
            _pogled.GridControl = _grid;
            _grid.Dock = DockStyle.Fill;
            _grid.DataSource = tablica;
            GridIzgled.Primijeni(_pogled, tablica, "");
            _pogled.OptionsBehavior.Editable = false;
            _pogled.OptionsView.ShowGroupPanel = false;
            _pogled.OptionsSelection.EnableAppearanceFocusedCell = false;
            CarpoIzgled.RedniStupac(_pogled);
            // TRZ-02, Form3.Trazilica 84-94: spremljeni izgled (GridTrazilica-xml-forma) se učita prije
            // COLUMNCAPTION i COLUMNWIDTH, koji ga nadjačaju. Janus .gxl se ne čita; Carpo piše .xml (R115).
            string datoteka = string.IsNullOrEmpty(imeIzgleda) ? "" : GridRaspored.Datoteka(imeIzgleda);
            if (datoteka.Length > 0)
                GridRaspored.Ucitaj(_pogled, datoteka, false);
            for (int i = 1; i < tablica.Columns.Count; i++)
            {
                var c = _pogled.Columns.ColumnByFieldName(tablica.Columns[i].ColumnName);
                if (c == null)
                    continue;
                string natpis = TrazilicaTok.Natpis(columnCaption, i);
                if (natpis != null)
                    c.Caption = natpis;
                int? sirina = TrazilicaTok.Sirina(columnWidth, i);
                if (sirina.HasValue)
                {
                    if (sirina.Value == 0)
                        c.Visible = false;
                    else
                        c.Width = sirina.Value;
                }
            }
            // Form3 160-166: COLUMNAUTORESIZE="false" ostavlja sirine, inace se stupci rasire na grid.
            _pogled.OptionsView.ColumnAutoWidth = !string.Equals((autoResize ?? "").Trim(), "false", StringComparison.OrdinalIgnoreCase);
            _pogled.DoubleClick += delegate { Uzmi(); };
            // Form3 butonzarazgrupiranje (spremipostavke) i Button11 (DeleteGridLayout, pa Close).
            if (datoteka.Length > 0)
            {
                GridIzbornik.Vezi(_pogled, null, null, null, new GridRadnje
                {
                    Spremi = delegate { GridRaspored.Spremi(_pogled, datoteka); },
                    Reset = delegate
                    {
                        GridRaspored.Obrisi(datoteka);
                        _odabran = null;
                        Close();
                    }
                });
            }
            _pogled.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    Uzmi();
                }
            };

            var dolje = new PanelControl();
            dolje.Dock = DockStyle.Bottom;
            dolje.Height = 40;
            var ok = new SimpleButton();
            ok.Text = "Odaberi";
            ok.Width = CarpoIzgled.SirinaGumba;
            ok.Left = 8;
            ok.Top = 8;
            ok.Click += delegate { Uzmi(); };
            CarpoIzgled.Gumb(ok);
            var odustani = new SimpleButton();
            odustani.Text = "Odustani";
            odustani.Width = CarpoIzgled.SirinaGumba;
            odustani.Left = 8 + CarpoIzgled.SirinaGumba + CarpoIzgled.RazmakGumba;
            odustani.Top = 8;
            odustani.Click += delegate { _odabran = null; Close(); };
            CarpoIzgled.Gumb(odustani);
            dolje.Controls.Add(ok);
            dolje.Controls.Add(odustani);
            Controls.Add(_grid);
            Controls.Add(dolje);
            Shown += delegate
            {
                _grid.Focus();
                if (_pogled.RowCount > 0)
                    _pogled.FocusedRowHandle = 0;
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                _odabran = null;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Uzmi()
        {
            if (_pogled.FocusedRowHandle >= 0)
                _odabran = _pogled.GetDataRow(_pogled.FocusedRowHandle);
            Close();
        }
    }
}
