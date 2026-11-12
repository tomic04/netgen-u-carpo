using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Podloga kao Form1: lijevo drvo, desno master, splitter.
    /// Ctrl+F11 otvara Carpo.Core.Log.Log nad loggerom spojenim u NgDnevnik.
    /// </summary>
    public sealed class Podloga : Form
    {
        private readonly SplitContainer _split;
        private readonly TreeView _drvo;
        private readonly DataGridView _grid;
        private readonly FlowLayoutPanel _gumbi;
        private readonly Label _status;
        private readonly Stablo _stablo = new Stablo();
        private readonly MasterGrid _master = new MasterGrid();
        private NgProject _projekt;
        private string _spoj = "";

        public Podloga()
        {
            Text = "NetGen";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Font = new Font("Microsoft Sans Serif", 8.25f);

            _status = new Label();
            _status.Dock = DockStyle.Bottom;
            _status.Height = 22;
            _status.TextAlign = ContentAlignment.MiddleLeft;

            _gumbi = new FlowLayoutPanel();
            _gumbi.Dock = DockStyle.Bottom;
            _gumbi.Height = 40;
            _gumbi.FlowDirection = FlowDirection.LeftToRight;
            _gumbi.WrapContents = false;

            _drvo = new TreeView();
            _drvo.Dock = DockStyle.Fill;
            _drvo.ShowRootLines = true;
            _drvo.ShowPlusMinus = true;
            _drvo.ShowLines = true;
            _drvo.HideSelection = true;
            _drvo.AllowDrop = true;
            _drvo.Sorted = false;
            _drvo.BorderStyle = BorderStyle.None;
            _drvo.BackColor = SystemColors.ControlLightLight;
            _drvo.AfterSelect += Drvo_AfterSelect;

            _grid = new DataGridView();
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = true;
            _grid.RowHeadersVisible = true;
            _grid.BorderStyle = BorderStyle.None;
            _grid.BackgroundColor = SystemColors.Window;
            _grid.Font = new Font("Microsoft Sans Serif", 8.25f);
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.ColumnHeadersHeight = 48;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.EditMode = DataGridViewEditMode.EditProgrammatically;

            _split = new SplitContainer();
            _split.Dock = DockStyle.Fill;
            _split.Orientation = Orientation.Vertical;
            _split.Panel1.Controls.Add(_drvo);
            _split.Panel2.Controls.Add(_grid);

            Controls.Add(_status);
            Controls.Add(_gumbi);
            Controls.Add(_split);
        }

        public bool Prikazi(string xml, string server, string baza, int operater)
        {
            var r = NetGenXmlReader.Ucitaj(xml);
            if (!r.Uspjeh || r.Projekt == null)
            {
                _status.Text = r.Greska ?? "XML nije učitan.";
                return false;
            }
            _projekt = r.Projekt;
            Text = string.IsNullOrEmpty(_projekt.Naslov) ? "NetGen" : _projekt.Naslov;
            var kontekst = new SqlKontekst();
            kontekst.OperaterId = operater.ToString();
            kontekst.XmlName = Path.GetFileName(xml ?? "");
            kontekst.Node = "null";
            _stablo.Kontekst = kontekst;
            _stablo.ImeXml = kontekst.XmlName;
            _master.Kontekst = kontekst;
            try
            {
                _spoj = SpinVeza.Spoj(server, baza);
            }
            catch (Exception ex)
            {
                _status.Text = ex.Message;
                return false;
            }

            _stablo.Ucitaj(_projekt, Citaj);
            NapuniDrvo();
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
            _drvo.BeginUpdate();
            _drvo.Nodes.Clear();
            for (int i = 0; i < _stablo.Stanje.Korijen.Count; i++)
                DodajCvor(_stablo.Stanje.Korijen[i], _drvo.Nodes);
            _drvo.EndUpdate();
        }

        private void DodajCvor(Cvor cvor, TreeNodeCollection kamo)
        {
            var n = new TreeNode(cvor.Natpis ?? "");
            n.Tag = cvor;
            n.Name = cvor.Oznaka ?? "";
            kamo.Add(n);
            for (int i = 0; i < cvor.Djeca.Count; i++)
                DodajCvor(cvor.Djeca[i], n.Nodes);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!_stablo.Stanje.Vidljivo || _stablo.Stanje.Splitter <= 0)
            {
                _split.Panel1Collapsed = true;
                return;
            }
            _split.Panel1Collapsed = false;
            int d = _stablo.Stanje.Splitter;
            int max = _split.Width - _split.SplitterWidth - 80;
            if (d > 40 && d < max)
                _split.SplitterDistance = d;
        }

        private void Drvo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var cvor = e.Node == null ? null : e.Node.Tag as Cvor;
            if (cvor == null)
                return;
            _stablo.Odaberi(cvor);
            string naredba = (cvor.Naredba ?? "").Trim();
            if (naredba.StartsWith("==", StringComparison.Ordinal))
                naredba = naredba.Substring(2).Trim();
            if (naredba.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                || naredba.StartsWith("sp", StringComparison.OrdinalIgnoreCase)
                || naredba.StartsWith("exec", StringComparison.OrdinalIgnoreCase))
            {
                IzvrsiSql(naredba);
                return;
            }
            string master = _projekt == null ? "" : (_projekt.OnLoad ?? "");
            if (master.IndexOf("@@node", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _master.Ucitaj(_projekt, null);
                NapuniGrid();
                return;
            }
            if (naredba.Length > 0)
                NgDnevnik.Akcija("otvara formu " + naredba);
        }

        private void NapuniGrid()
        {
            _master.IzvrsiCitanje(_spoj);
            VeziGrid();
        }

        private void IzvrsiSql(string sql)
        {
            string spremno = SqlTekst.Sredi(sql, _master.Kontekst).Replace("~~", ",");
            if (SqlTekst.JeZabranaPisanja(spremno))
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + spremno);
                _status.Text = "Upis nije poslan.";
                return;
            }
            NgDnevnik.Sql(spremno);
            DataTable t = Citaj(spremno);
            _grid.DataSource = t;
            _status.Text = t == null ? "Greška SQL-a." : (t.Rows.Count.ToString() + " redova");
        }

        private void VeziGrid()
        {
            _grid.DataSource = _master.Stanje.Tablica;
            if (!string.IsNullOrEmpty(_master.Stanje.Poruka))
            {
                _status.Text = _master.Stanje.Poruka.Replace("\r\n", " ");
                return;
            }
            int redova = _master.Stanje.Tablica == null ? 0 : _master.Stanje.Tablica.Rows.Count;
            _status.Text = redova.ToString() + " redova";
            for (int i = 0; i < _master.Stanje.Kolone.Count; i++)
            {
                GridKolona k = _master.Stanje.Kolone[i];
                if (k.Selektor || k.Podatak == "Icon")
                    continue;
                if (!_grid.Columns.Contains(k.Podatak))
                    continue;
                DataGridViewColumn c = _grid.Columns[k.Podatak];
                c.Visible = k.Vidljiva;
                c.HeaderText = k.Natpis ?? k.Podatak;
                if (k.Sirina > 0)
                    c.Width = k.Sirina;
                c.ReadOnly = !k.Uredivanje;
            }
            if (_grid.Rows.Count > 0)
                _grid.Rows[0].Selected = true;
        }

        private void NapuniGumbe()
        {
            _gumbi.Controls.Clear();
            if (_projekt == null)
                return;
            for (int i = 0; i < _projekt.Gumbi.Count; i++)
            {
                NgButton b = _projekt.Gumbi[i];
                var btn = new Button();
                btn.Text = b.Natpis;
                btn.Tag = b.Naredba;
                btn.AutoSize = true;
                btn.Click += Gumb_Click;
                _gumbi.Controls.Add(btn);
            }
        }

        private void Gumb_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string naredba = btn == null || btn.Tag == null ? "" : btn.Tag.ToString();
            NgDnevnik.Akcija("gumb " + (btn == null ? "" : btn.Text) + " " + naredba);
            if (string.Equals(naredba.Trim(), "REFRESHMASTERGRID", StringComparison.OrdinalIgnoreCase))
            {
                _master.Ucitaj(_projekt, null);
                NapuniGrid();
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
                _status.Text = ex.Message;
                return null;
            }
        }

        private void OtvoriCarpoLog()
        {
            object logger = NgDnevnik.CarpoLogger;
            if (logger == null)
            {
                _status.Text = "Carpo logger nije spojen.";
                return;
            }
            try
            {
                Type tip = logger.GetType().Assembly.GetType("Carpo.Core.Log.Log");
                if (tip == null)
                {
                    _status.Text = "Carpo.Core.Log.Log nije pronađen.";
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
                _status.Text = "Ctrl+F11: " + ex.Message;
            }
        }
    }
}
