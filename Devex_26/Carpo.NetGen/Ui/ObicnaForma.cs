using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Granula koja nije Manager: jedan grid i gumbi na dnu, bez ribbona,
    /// bez lijevog stabla i bez detaila. Koordinate grida su dock, kao Form1
    /// kad je lijevi splitter 0 (Form1.vb 4310–4316, 2111–2113).
    /// </summary>
    public sealed class ObicnaForma : XtraForm
    {
        private readonly GridControl _grid = new GridControl();
        private readonly GridView _pogled = new GridView();
        private readonly Panel _gumbi = new Panel();

        public ObicnaForma()
        {
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            Text = "NetGen";
            Width = 960;
            Height = 640;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            _gumbi.Dock = DockStyle.Bottom;
            _gumbi.Height = 40;
            _grid.Dock = DockStyle.Fill;
            _grid.MainView = _pogled;
            _grid.ViewCollection.Add(_pogled);
            _pogled.OptionsBehavior.Editable = false;
            _pogled.OptionsBehavior.ReadOnly = true;
            _pogled.OptionsView.ColumnAutoWidth = false;
            _pogled.OptionsView.ShowIndicator = true;
            _pogled.OptionsSelection.MultiSelect = true;
            _pogled.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            CarpoIzgled.Pogled(_pogled);
            Controls.Add(_grid);
            Controls.Add(_gumbi);
        }

        public bool Prikazi(NgProject projekt, string server, string baza, int operater, string xml)
        {
            if (projekt == null)
            {
                Text = "XML nije učitan.";
                return false;
            }
            Text = string.IsNullOrEmpty(projekt.Naslov) ? "NetGen" : projekt.Naslov;
            string spoj;
            try
            {
                spoj = SpinVeza.Spoj(server, baza);
            }
            catch (Exception ex)
            {
                Text = ex.Message;
                return false;
            }
            var kontekst = new SqlKontekst();
            kontekst.OperaterId = operater.ToString();
            kontekst.XmlName = System.IO.Path.GetFileName(xml ?? "");
            kontekst.Node = "null";
            var master = new MasterGrid();
            master.Kontekst = kontekst;
            master.Ucitaj(projekt, null);
            master.IzvrsiCitanje(spoj);
            Vezi(master);
            Gumbi(projekt);
            return true;
        }

        private void Vezi(MasterGrid master)
        {
            DataTable tablica = master.Stanje.Tablica;
            _grid.DataSource = tablica;
            _pogled.PopulateColumns();
            bool? grupa = master.Stanje.GroupByBox;
            _pogled.OptionsView.ShowGroupPanel = !grupa.HasValue || grupa.Value;
            _pogled.OptionsView.ShowAutoFilterRow = master.Stanje.FilterRed;
            if (master.Stanje.Kolone == null)
                return;
            for (int i = 0; i < master.Stanje.Kolone.Count; i++)
            {
                GridKolona k = master.Stanje.Kolone[i];
                if (k.Selektor || k.Podatak == "Icon")
                    continue;
                var c = _pogled.Columns.ColumnByFieldName(k.Podatak);
                if (c == null)
                    continue;
                c.Visible = k.Vidljiva;
                c.Caption = k.Natpis ?? k.Podatak;
                if (k.Sirina > 0)
                    c.Width = k.Sirina;
                c.OptionsColumn.AllowEdit = k.Uredivanje;
            }
            if (_pogled.RowCount > 0)
                _pogled.FocusedRowHandle = 0;
            NgDnevnik.Akcija("obicna forma redova=" + (tablica == null ? 0 : tablica.Rows.Count).ToString());
        }

        private void Gumbi(NgProject projekt)
        {
            _gumbi.Controls.Clear();
            if (projekt.Gumbi == null)
                return;
            for (int i = projekt.Gumbi.Count - 1; i >= 0; i--)
            {
                NgButton b = projekt.Gumbi[i];
                var gumb = new SimpleButton();
                gumb.Text = (b.Natpis ?? "").Replace("!!", "\r\n");
                gumb.Tag = b.Naredba;
                gumb.Dock = DockStyle.Left;
                gumb.Width = Math.Max(90, gumb.Text.Replace("\r\n", "").Length * 8);
                CarpoIzgled.Gumb(gumb);
                gumb.Click += delegate
                {
                    NgDnevnik.Akcija("gumb " + gumb.Text);
                };
                _gumbi.Controls.Add(gumb);
            }
        }
    }
}
