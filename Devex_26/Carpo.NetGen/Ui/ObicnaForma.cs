using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
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
    /// Klik gumba ide u NovaGenerickaForma (Form1.vb 5664–5667).
    /// </summary>
    public sealed class ObicnaForma : XtraForm
    {
        private readonly GridControl _grid = new GridControl();
        private readonly GridView _pogled = new GridView();
        private readonly Panel _gumbi = new Panel();
        private NgProject _projekt;
        private MasterGrid _master;
        private string _spoj = "";
        private string _server = "";
        private int _dubina;

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
            _projekt = projekt;
            _server = server ?? "";
            Text = string.IsNullOrEmpty(projekt.Naslov) ? "NetGen" : projekt.Naslov;
            try
            {
                _spoj = SpinVeza.Spoj(server, baza);
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
            _master = new MasterGrid();
            _master.Kontekst = kontekst;
            _master.Ucitaj(projekt, null);
            Osvjezi();
            Gumbi(projekt);
            return true;
        }

        private void Vezi()
        {
            DataTable tablica = _master.Stanje.Tablica;
            _grid.DataSource = tablica;
            _pogled.PopulateColumns();
            bool? grupa = _master.Stanje.GroupByBox;
            _pogled.OptionsView.ShowGroupPanel = !grupa.HasValue || grupa.Value;
            _pogled.OptionsView.ShowAutoFilterRow = _master.Stanje.FilterRed;
            if (_master.Stanje.Kolone == null)
                return;
            for (int i = 0; i < _master.Stanje.Kolone.Count; i++)
            {
                GridKolona k = _master.Stanje.Kolone[i];
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

        private void Osvjezi()
        {
            if (_master == null)
                return;
            _master.IzvrsiCitanje(_spoj);
            Vezi();
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
                gumb.Click += Gumb_Click;
                _gumbi.Controls.Add(gumb);
            }
        }

        private void Gumb_Click(object sender, EventArgs e)
        {
            var gumb = sender as SimpleButton;
            string naredba = gumb == null || gumb.Tag == null ? "" : gumb.Tag.ToString();
            NgDnevnik.Akcija("gumb " + (gumb == null ? "" : gumb.Text) + " " + naredba);
            string[] seg = Lanac.Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string s = (seg[i] ?? "").Trim();
                if (s.Length == 0)
                    continue;
                string vrsta = GumbTok.Akcija(_projekt, s);
                if (vrsta == "prazno")
                {
                    if (PraznoNaredba.PreskociSljedeci(s))
                        i++;
                    continue;
                }
                if (vrsta == "nista")
                    continue;
                if (vrsta == "fiskal" || vrsta == "prostor" || vrsta == "blagajna")
                {
                    IzvrsiVanjsko(vrsta, s);
                    continue;
                }
                if (vrsta == "odznaci")
                    continue;
                if (vrsta == "roditelj")
                {
                    Close();
                    return;
                }
                if (vrsta == "poruka")
                {
                    PorukaNaredba.Pokazi(s);
                    continue;
                }
                if (vrsta == "openform")
                {
                    Otvori(OtvoriNaredba.Ime(s));
                    continue;
                }
                if (vrsta == "osvjezi")
                    Osvjezi();
                else if (vrsta == "forma")
                    Otvori(s);
                else if (vrsta == "stupci")
                    PrimijeniStupce(s);
                else if (vrsta == "word")
                {
                    if (!Carpo.NetGen.Print.WordDot.Otvori(s, _spoj, new[] { IdReda() }))
                        break;
                }
                else if (vrsta == "ispis")
                    Carpo.NetGen.Print.PrintBridge.Ispisi(s, false, IdReda(), _master.Kontekst.OperaterId, _spoj, _spoj, _master.Stanje.ZadnjiSql, ParametriReda(), "", "", "", "", Text, _grid, Orijentacija(), _master.Stanje.Tablica);
                else if (vrsta == "assembly")
                {
                    var poziv = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(s, "shell", IdReda(), _master.Kontekst.OperaterId, _spoj, _master.Stanje.ZadnjiSql, ParametriReda(), "");
                    if (poziv.Prekinuto)
                        break;
                }
                else if (vrsta == "zatvori")
                    Close();
                else if (vrsta == "sql")
                    PrimijeniSelect(s);
            }
        }

        private void IzvrsiVanjsko(string vrsta, string s)
        {
            if (vrsta == "fiskal")
            {
                NgDnevnik.Akcija("fiskal tablica=" + FiskalNaredba.Tablica(s) + " poziva=" + FiskalNaredba.Poziva(0));
                Carpo.NetGen.Poziv.FiskalPoziv.Posalji(FiskalNaredba.Argument(s), "", "", "", "", "", "");
            }
            else if (vrsta == "prostor")
                NgDnevnik.Akcija("prostor " + ProstorNaredba.Argument(s) + " poziva=" + ProstorNaredba.Poziva(0));
            else if (vrsta == "blagajna")
                NgDnevnik.Akcija(BlagajnaNaredba.Update(BlagajnaNaredba.Id(s), false));
        }

        private string IdReda()
        {
            DataRow red = _pogled.GetFocusedDataRow();
            if (red == null)
                return "0";
            if (red.Table.Columns.Contains("id") && red["id"] != DBNull.Value)
            {
                string id = Convert.ToString(red["id"]);
                if (!string.IsNullOrEmpty(id))
                    return id;
            }
            if (red.Table.Columns.Count > 0 && red[0] != DBNull.Value)
            {
                string id = Convert.ToString(red[0]);
                if (!string.IsNullOrEmpty(id))
                    return id;
            }
            return "0";
        }

        private void PrimijeniStupce(string naredba)
        {
            var mapa = Carpo.NetGen.Komande.StupciNaredba.Primijeni(naredba);
            foreach (var par in mapa)
            {
                var c = _pogled.Columns.ColumnByFieldName(par.Key);
                if (c == null)
                    continue;
                c.Visible = par.Value;
            }
        }

        private string Orijentacija()
        {
            if (_projekt == null || _projekt.Atributi == null || !_projekt.Atributi.Ima("PAGEORIENTATION"))
                return "";
            return _projekt.Atributi.Vrijednost("PAGEORIENTATION") ?? "";
        }

        private string ParametriReda()
        {
            return Carpo.NetGen.Poziv.AssemblyPoziv.ParametriReda(_pogled.GetFocusedDataRow());
        }

        private void PrimijeniSelect(string sql)
        {
            string spremno = SqlTekst.Sredi(sql, _master.Kontekst).Replace("~~", ",");
            if (SqlTekst.Odluka(spremno) != "salji" || SqlTekst.JeZabranaPisanja(spremno))
                return;
            DataTable t = Citaj(spremno);
            if (t == null || t.Rows.Count == 0)
                return;
            DataRow row = t.Rows[0];
            for (int i = 0; i < _gumbi.Controls.Count; i++)
            {
                var gumb = _gumbi.Controls[i] as SimpleButton;
                if (gumb == null)
                    continue;
                string natpis = (gumb.Text ?? "").Replace("\r\n", "").Trim();
                if (!row.Table.Columns.Contains(natpis))
                    continue;
                string v = row[natpis] == DBNull.Value ? "" : Convert.ToString(row[natpis]);
                string akcija = OnLoad2.Akcija(v);
                if (akcija == "enable")
                    gumb.Enabled = true;
                else if (akcija == "disable")
                    gumb.Enabled = false;
                else if (akcija == "visible")
                    gumb.Visible = true;
                else if (akcija == "invisible")
                    gumb.Visible = false;
                else if (akcija == "natpis")
                    gumb.Text = v;
            }
        }

        private void Otvori(string naredba)
        {
            if (string.IsNullOrWhiteSpace(naredba) || _projekt == null || _dubina > 8)
                return;
            string trazeno = naredba.Trim();
            if (trazeno.StartsWith(":", StringComparison.Ordinal))
                trazeno = trazeno.Substring(1).Trim();
            int zagrada = trazeno.IndexOf('(');
            string ime = zagrada > 0 ? trazeno.Substring(0, zagrada).Trim() : trazeno;
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
                Osvjezi();
                return;
            }
            _dubina++;
            try
            {
                var dij = new Dijalog();
                dij.JupiterNeSaljeGumb(DijalogTok.JeJupiter(_server));
                dij.Konekcija = _spoj;
                dij.Operater = _master.Kontekst.OperaterId;
                dij.Id = IdReda();
                dij.ZadnjiSql = _master.Stanje.ZadnjiSql;
                dij.OtvoriDrugu = Otvori;
                dij.Prikazi(forma, SqlTekst.ZaFormu(_master.Kontekst), Citaj);
                if (dij.DialogResult != DialogResult.OK)
                    return;
                string explorer = DijalogTok.SqlNakonOk(forma);
                if (explorer.Length > 0)
                {
                    string sql = SqlTekst.Sredi(explorer, _master.Kontekst).Replace("~~", ",");
                    _master.Stanje.IzvorSql = explorer;
                    _master.Stanje.ZadnjiSql = sql;
                    _master.Stanje.SqlZaSlanje = sql;
                    _master.Stanje.Odluka = SqlTekst.Odluka(sql);
                    if (_master.Stanje.Odluka == "salji" && !SqlTekst.JeZabranaPisanja(sql))
                        Osvjezi();
                    return;
                }
                string sljedeca = DijalogTok.NextForm(forma);
                if (sljedeca.Length > 0)
                    Otvori(sljedeca);
                else
                    Osvjezi();
            }
            finally
            {
                _dubina--;
            }
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
                return null;
            }
        }
    }
}
