using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Dijalog forme. XtraForm i DevExpress editor po grani iz Form2.
    /// Raspored je SrediKontrolu (TOP/LEFT/WIDTH/HEIGHT / 14). WebBrowser ostaje
    /// WinForms jer DevExpress nema preglednik.
    /// </summary>
    public sealed class Dijalog : XtraForm
    {
        private Func<string, DataTable> _citaj;
        private SqlKontekst _kontekst;
        private NgForm _forma;
        private bool _jupiter;

        public string OtvorenaForma = "";
        public string MasterSql = "";
        public Action<string> OtvoriDrugu;

        public void JupiterNeSaljeGumb(bool jupiter)
        {
            _jupiter = jupiter;
        }

        public Dijalog()
        {
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            StartPosition = FormStartPosition.CenterParent;
            Width = 480;
            Height = 320;
            KeyPreview = true;
        }

        public void Prikazi(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj, bool modal = true)
        {
            if (forma == null)
                return;
            _citaj = citaj;
            _kontekst = kontekst;
            _forma = forma;
            OtvorenaForma = "";
            MasterSql = "";
            string naslov = forma.Atributi == null ? null : forma.Atributi.Vrijednost("CAPTION");
            if (string.IsNullOrEmpty(naslov))
                naslov = forma.Ime;
            Text = Raspored.SrediTekst(naslov);
            int sirina;
            int visina;
            Raspored.Velicina(forma.Atributi, out sirina, out visina);
            if (sirina > 0)
                Width = sirina;
            if (visina > 0)
                Height = visina;
            string rub = Raspored.Rub(forma.Atributi == null ? null : forma.Atributi.Vrijednost("BORDERSTYLE"));
            if (rub == "FixedDialog")
                FormBorderStyle = FormBorderStyle.FixedDialog;
            else if (rub == "FixedSingle")
                FormBorderStyle = FormBorderStyle.FixedSingle;
            else if (rub == "Fixed3D")
                FormBorderStyle = FormBorderStyle.Fixed3D;
            else if (rub == "None")
                FormBorderStyle = FormBorderStyle.None;
            else if (rub == "FixedToolWindow")
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
            else if (rub == "SizableToolWindow")
                FormBorderStyle = FormBorderStyle.SizableToolWindow;

            NgDnevnik.Akcija("otvara formu " + forma.Ime + " node=" + (kontekst == null ? "" : kontekst.Node));
            int zadnji = 0;
            bool imaGumb = forma.Gumbi != null && forma.Gumbi.Count > 0;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k.Grana == NgGrana.Button)
                    imaGumb = true;
                Control c = Napravi(k);
                if (c == null)
                    continue;
                Mjesto m = Raspored.Izracunaj(k.Grana, k.Atributi, ref zadnji);
                c.Name = k.Name ?? "";
                c.Left = m.Lijevo;
                c.Top = m.Vrh;
                if (m.Sirina > 0)
                    c.Width = m.Sirina;
                if (m.Visina > 0)
                    c.Height = m.Visina;
                if (m.ImaUkljuceno)
                    c.Enabled = m.Ukljuceno;
                if (m.ImaVidljivo)
                    c.Visible = m.Vidljivo;
                PostaviTekst(c, k, m);
                CarpoIzgled.Primijeni(c, k.Grana);
                var dugme = c as SimpleButton;
                if (dugme != null)
                {
                    dugme.Tag = k.Atributi == null ? "" : (k.Atributi.Vrijednost("ONCLICK") ?? "");
                    dugme.Click += GumbKlik;
                }
                if (m.Natpis)
                {
                    var l = new LabelControl();
                    l.Name = "lbl" + c.Name;
                    l.Text = m.NatpisTekst ?? "";
                    l.Left = m.NatpisLijevo;
                    l.Top = m.NatpisVrh;
                    l.AutoSizeMode = LabelAutoSizeMode.Horizontal;
                    Controls.Add(l);
                }
                Controls.Add(c);
                NapuniKontrolu(c, k);
            }

            if (forma.Gumbi != null)
            {
                int x = 8;
                for (int i = 0; i < forma.Gumbi.Count; i++)
                {
                    NgButton g = forma.Gumbi[i];
                    var b = new SimpleButton();
                    b.Name = "cmd" + g.Redni.ToString();
                    b.Text = Raspored.SrediTekst(g.Natpis);
                    b.Tag = g.Redni.ToString() + "|" + (g.Naredba ?? "");
                    b.Left = x;
                    b.Top = ClientSize.Height - 36;
                    b.Width = 96;
                    b.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    b.Click += GumbKlik;
                    CarpoIzgled.Gumb(b);
                    Controls.Add(b);
                    x += 104;
                }
            }
            if (!imaGumb)
            {
                var ok = new SimpleButton();
                ok.Text = "OK";
                ok.DialogResult = DialogResult.OK;
                ok.Dock = DockStyle.Bottom;
                CarpoIzgled.Gumb(ok);
                Controls.Add(ok);
                AcceptButton = ok;
            }

            string on = forma.OnLoad ?? "";
            if (on.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase) && citaj != null)
                NapuniIzOnLoad(on, kontekst, citaj);
            if (modal)
                ShowDialog();
        }

        private void NapuniKontrolu(Control c, NgControl k)
        {
            if (_citaj == null || k == null)
                return;
            if (k.Grana != NgGrana.Combo && k.Grana != NgGrana.Grid && k.Grana != NgGrana.DxGrid)
                return;
            if (k.Grana != NgGrana.Combo && DijalogTok.PreskociGrid(k))
                return;
            if (k.Atributi == null || !k.Atributi.Ima("SQL"))
                return;
            string sql = (k.Atributi.Vrijednost("SQL") ?? "").Trim();
            if (sql.Length == 0)
                return;
            SqlZastita z = ZastitaSql.Tekst(sql);
            if (z.Pise || z.Procedure.Count > 0)
                return;
            sql = ComboDijalog.SqlZaSlanje(sql, _kontekst);
            DataTable dt = null;
            try
            {
                dt = _citaj(sql);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
            }
            if (dt == null || dt.Columns.Count == 0)
                return;
            var look = c as LookUpEdit;
            if (look != null)
            {
                bool prazan = ComboDijalog.AllowEmpty(k);
                if (prazan)
                    ComboDijalog.UbaciPrazan(dt);
                look.Properties.DataSource = dt;
                look.Properties.ValueMember = dt.Columns[0].ColumnName;
                look.Properties.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
                if (!prazan && ComboDijalog.OdabirAkoNista(dt, false) == "1")
                    look.EditValue = 1;
                return;
            }
            var grid = c as GridControl;
            if (grid != null)
                grid.DataSource = dt;
        }

        private void GumbKlik(object sender, EventArgs e)
        {
            var b = sender as SimpleButton;
            string tag = b == null ? "" : (b.Tag as string ?? "");
            int redni = 0;
            string naredba = tag;
            int crta = tag.IndexOf('|');
            if (crta > 0 && tag.StartsWith("cmd", StringComparison.Ordinal) == false)
            {
                int.TryParse(tag.Substring(0, crta), out redni);
                naredba = tag.Substring(crta + 1);
            }
            if (b != null && b.Name != null && b.Name.StartsWith("cmd", StringComparison.Ordinal))
            {
                int.TryParse(b.Name.Substring(3), out redni);
                if (crta >= 0)
                    naredba = tag.Substring(crta + 1);
            }
            DijalogTok.Klik klik = DijalogTok.Gumb(_forma, redni, naredba, _jupiter);
            if (klik.OtvaraFormu.Length > 0)
            {
                OtvorenaForma = klik.OtvaraFormu;
                if (b != null)
                    b.DialogResult = DialogResult.None;
                if (OtvoriDrugu != null)
                    OtvoriDrugu(klik.OtvaraFormu);
                return;
            }
            if (klik.Odustani)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            if (klik.SaljeSql && _citaj != null)
            {
                MasterSql = klik.MasterSql;
                NgDnevnik.Sql(klik.MasterSql);
                try
                {
                    _citaj(klik.MasterSql);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska(ex.Message + " | " + klik.MasterSql);
                }
            }
            else
                MasterSql = _jupiter ? "" : klik.MasterSql;
            string fokus = DijalogKlik.Fokus(naredba);
            if (fokus.Length > 0)
            {
                Control[] nadjeni = Controls.Find(fokus, true);
                if (nadjeni.Length > 0)
                    nadjeni[0].Focus();
            }
            if (klik.Zatvara)
                DialogResult = DialogResult.OK;
            else if (b != null)
                b.DialogResult = DialogResult.None;
        }

        public string Vrijednost(string ime)
        {
            if (string.IsNullOrEmpty(ime))
                return "";
            Control[] n = Controls.Find(ime, true);
            if (n.Length == 0)
                return "";
            var look = n[0] as LookUpEdit;
            if (look != null && look.EditValue != null)
                return Convert.ToString(look.EditValue);
            var edit = n[0] as TextEdit;
            if (edit != null)
                return edit.Text ?? "";
            return n[0].Text ?? "";
        }

        private static Control Napravi(NgControl k)
        {
            NgGrana g = k.Grana;
            if (g == NgGrana.Text)
                return new TextEdit();
            if (g == NgGrana.Number)
                return new SpinEdit();
            if (g == NgGrana.Search)
            {
                var b = new ButtonEdit();
                b.Properties.TextEditStyle = TextEditStyles.Standard;
                return b;
            }
            if (g == NgGrana.Date)
            {
                var d = new DateEdit();
                d.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
                return d;
            }
            if (g == NgGrana.Combo)
                return new LookUpEdit();
            if (g == NgGrana.MultiDrop)
                return new CheckedComboBoxEdit();
            if (g == NgGrana.Check)
                return new CheckEdit();
            if (g == NgGrana.Radio)
            {
                var r = new CheckEdit();
                r.Properties.CheckStyle = CheckStyles.Radio;
                r.Properties.RadioGroupIndex = 1;
                return r;
            }
            if (g == NgGrana.Label || g == NgGrana.StatusStrip)
            {
                var l = new LabelControl();
                if (g == NgGrana.StatusStrip)
                    l.Dock = DockStyle.Bottom;
                return l;
            }
            if (g == NgGrana.Group)
                return new GroupControl();
            if (g == NgGrana.Panel)
                return new PanelControl();
            if (g == NgGrana.Button || g == NgGrana.Help)
                return new SimpleButton();
            if (g == NgGrana.PictureBox)
            {
                var p = new PictureEdit();
                p.Properties.SizeMode = PictureSizeMode.Stretch;
                return p;
            }
            if (g == NgGrana.WebBrowser)
                return new WebBrowser();
            if (g == NgGrana.Grid || g == NgGrana.DxGrid)
            {
                var grid = new GridControl();
                var pogled = new GridView();
                grid.MainView = pogled;
                pogled.GridControl = grid;
                pogled.OptionsBehavior.Editable = false;
                return grid;
            }
            if (g == NgGrana.Tree)
                return new DevExpress.XtraTreeList.TreeList();
            if (g == NgGrana.SplitContainer)
                return new SplitContainerControl();
            return null;
        }

        private static void PostaviTekst(Control c, NgControl k, Mjesto m)
        {
            string tekst = m.Tekst ?? "";
            var check = c as CheckEdit;
            if (check != null)
            {
                check.Text = tekst;
                check.Checked = m.Cekirano;
                return;
            }
            var gumb = c as SimpleButton;
            if (gumb != null)
            {
                gumb.Text = tekst;
                return;
            }
            var label = c as LabelControl;
            if (label != null)
            {
                if (k.Grana == NgGrana.StatusStrip)
                    label.Text = Ponasanje.StatusTekst(tekst);
                else
                    label.Text = tekst;
                return;
            }
            var grupa = c as GroupControl;
            if (grupa != null)
            {
                grupa.Text = tekst;
                return;
            }
            var datum = c as DateEdit;
            if (datum != null)
            {
                string prikaz = tekst;
                if (prikaz.StartsWith("firstday", StringComparison.OrdinalIgnoreCase))
                    prikaz = Ponasanje.DatumPocetni(prikaz, DateTime.Now.Year, DateTime.Now.Month) ?? "";
                DateTime dt;
                if (DateTime.TryParseExact(prikaz, "dd.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    datum.DateTime = dt;
                return;
            }
            if (c is PictureEdit || c is WebBrowser)
            {
                var web = c as WebBrowser;
                if (web != null)
                {
                    string url = k.Atributi == null ? null : Rijetke.WebAdresa(k.Atributi.Vrijednost("URL"), null);
                    if (!string.IsNullOrEmpty(url))
                        web.Navigate(url);
                }
                return;
            }
            var edit = c as TextEdit;
            if (edit != null)
                edit.Text = tekst;
        }

        private void NapuniIzOnLoad(string on, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            string sql = SqlTekst.Sredi(on, kontekst);
            NgDnevnik.Sql(sql);
            DataTable dt = null;
            try
            {
                dt = citaj(sql);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
            }
            if (dt == null || dt.Rows.Count == 0)
                return;
            foreach (DataColumn col in dt.Columns)
            {
                Control[] n = Controls.Find(col.ColumnName, true);
                if (n.Length == 0)
                    continue;
                string v = dt.Rows[0][col] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0][col]);
                var edit = n[0] as TextEdit;
                if (edit != null && !(edit is LookUpEdit))
                    edit.Text = v;
                var look = n[0] as LookUpEdit;
                if (look != null)
                    look.EditValue = v;
                var label = n[0] as LabelControl;
                if (label != null && n[0].Name == col.ColumnName)
                    label.Text = label.Dock == DockStyle.Bottom ? Ponasanje.StatusTekst(v) : v;
            }
        }

        private static bool Odustajanje(string tekst)
        {
            string t = (tekst ?? "").Trim().ToLowerInvariant();
            return t == "odustani" || t == "cancel" || t == "close" || t.StartsWith("odustani", StringComparison.Ordinal);
        }
    }
}
