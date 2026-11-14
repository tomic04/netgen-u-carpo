using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
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
    public sealed partial class Dijalog : XtraForm
    {
        private Func<string, DataTable> _citaj;
        private SqlKontekst _kontekst;
        private NgForm _forma;
        private static Dijalog _zatvoriRoditelja;
        public static Action<Dijalog> NakonPrikaza;
        public static string TragPutanja;
        private bool _jupiter;
        private readonly Dictionary<string, string> _izUpita = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private int _grupe;
        private string _priprema = "";

        public string OtvorenaForma = "";
        public string MasterSql = "";
        public string Konekcija = "";
        public string Operater = "";
        public string Id = "0";
        public string ZadnjiSql = "";

        public string ParametriVise()
        {
            return _kontekst == null ? "" : (_kontekst.ParametriSaGrida ?? "");
        }
        public Action<string> OtvoriDrugu;

        public void JupiterNeSaljeGumb(bool jupiter)
        {
            _jupiter = jupiter;
        }

        public Dijalog()
        {
            NgPoruka.Pripremi();
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            StartPosition = FormStartPosition.CenterParent;
            Width = 480;
            Height = 320;
            KeyPreview = true;
        }

        /// <summary>
        /// Form2.ProcitajXML 1197-1199 i Form1 5352: greška na ONLOAD forme (ExecSql je pokazao poruku)
        /// znači da se forma ne otvara. "#OK" nije greška (ONL-02).
        /// </summary>
        public string GreskaOnLoad { get; private set; }

        public void Prikazi(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj, bool modal = true, Form vlasnik = null)
        {
            Slozi(forma, kontekst, citaj);
            if (forma == null)
                return;
            if (!string.IsNullOrEmpty(GreskaOnLoad))
            {
                Trag("onload greska, forma se ne otvara " + forma.Ime);
                return;
            }
            Trag("prije-show " + forma.Ime + " modal=" + modal);
            if (modal)
            {
                // Form1.vb 5362: Form2.ShowDialog(Me). Ribbon kao vlasnik ne uđe u petlju.
                // Običan Form kao vlasnik XtraFormu baci KeyNotFound. Skriveni vlasnik
                // iz klika na podlozi ne vrati Show (df927a0). Bez takvog vlasnika ostaje ShowDialog().
                Form nosac = VlasnikXtra(vlasnik);
                if (nosac != null)
                    ShowDialog(nosac);
                else
                    ShowDialog();
            }
            Trag("poslije-show " + forma.Ime);
        }

        /// <summary>
        /// Class1.ExecSql Catch (157-200): greška SQL-a se pokaže korisniku (MsgBox); "#OK"/"+OK" i tihe
        /// greške (ssoperateri, sspermission, NetGenDBFilter...) nisu greška pa lanac ide dalje (MSG-01).
        /// Ostale se bace dalje, pa lanac stane kao na GreskaKodIzvrsavanjaSQLa.
        /// </summary>
        private Func<string, DataTable> Omotaj(Func<string, DataTable> citaj)
        {
            if (citaj == null)
                return null;
            return delegate (string sql)
            {
                try
                {
                    return citaj(sql);
                }
                catch (Exception ex)
                {
                    SqlPoruka.Ishod ish = SqlPoruka.IzIznimke(ex.Message);
                    if (ish.Prikazi)
                        NgPoruka.Info(this, "\r\n" + ish.Tekst + "\r\n\r\n", "Poruka");
                    if (string.IsNullOrEmpty(ish.Greska))
                        return null;
                    throw;
                }
            };
        }

        private static void Trag(string red)
        {
            if (string.IsNullOrEmpty(TragPutanja))
                return;
            try { System.IO.File.AppendAllText(TragPutanja, DateTime.Now.ToString("HH:mm:ss") + " " + red + "\r\n"); }
            catch { }
        }

        /// <summary>Isti raspored kao Prikazi, bez ShowDialog. Test gleda kontrolu.</summary>
        public void Slozi(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            if (forma == null)
                return;
            _slazem = true;
            try
            {
                SloziTijelo(forma, kontekst, citaj);
            }
            finally
            {
                _slazem = false;
            }
        }

        private void SloziTijelo(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            _nakonDjeteta.Clear();
            GreskaOnLoad = "";
            citaj = Omotaj(citaj);
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
            string on = forma.OnLoad ?? "";
            if (DijalogTok.OnLoadJeUpit(on) && citaj != null)
                UcitajOnLoad(on, kontekst, citaj);
            int zadnji = 0;
            var roditelji = new List<KeyValuePair<Control, string>>();
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
                var datumKontrola = c as DateEdit;
                if (datumKontrola != null)
                    PripremiDatum(datumKontrola, k);
                CarpoIzgled.Primijeni(c, k.Grana);
                PrimijeniIzgled(c, k);
                string roditelj = Attr(k, "PARENT");
                if (roditelj.Length > 0)
                    roditelji.Add(new KeyValuePair<Control, string>(c, roditelj));
                var dugme = c as SimpleButton;
                if (dugme != null)
                {
                    dugme.Tag = k.Atributi == null ? "" : (k.Atributi.Vrijednost("ONCLICK") ?? "");
                    dugme.Click += GumbKlik;
                }
                VeziDogadjaj(c, k);
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
                // Form2.SrediKontrolu 3056-3067 i RefreshKontrola 5489/5555: Combo i Janus Grid.
                if (DijalogTok.RefreshAfterChild(k) && (k.Grana == NgGrana.Combo || k.Grana == NgGrana.Grid))
                    _nakonDjeteta.Add(new KeyValuePair<Control, NgControl>(c, k));
            }

            // Form2.vb Glbl.Parent = Controls(PARENT) (npr. 2361, 2414, 3010): kontrola ide u okvir i LEFT/TOP
            // su relativni prema okviru. VB to radi odmah, pa okvir definiran kasnije ostane bez djece; ovdje
            // ide nakon svih kontrola (LAY-01).
            for (int i = 0; i < roditelji.Count; i++)
            {
                Control dijete = roditelji[i].Key;
                Control[] okviri = Controls.Find(roditelji[i].Value, false);
                if (okviri.Length == 0 || okviri[0] == dijete || !(okviri[0] is PanelControl))
                    continue;
                Control[] natpis = Controls.Find("lbl" + dijete.Name, false);
                okviri[0].Controls.Add(dijete);
                if (natpis.Length > 0)
                    okviri[0].Controls.Add(natpis[0]);
            }

            if (forma.Gumbi != null && forma.Gumbi.Count > 0)
            {
                // Form2.designer: Panel1 dno (33) s Button1..6 na stalnim mjestima; prazan natpis skrije gumb
                // (2747-2762), a bez ijednog natpisa nema ni Panel1.
                var ploca = new PanelControl();
                ploca.Name = "pnlGumbi";
                ploca.Dock = DockStyle.Bottom;
                ploca.Height = 33;
                ploca.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                for (int i = 0; i < forma.Gumbi.Count; i++)
                {
                    NgButton g = forma.Gumbi[i];
                    var b = new SimpleButton();
                    b.Name = "cmd" + g.Redni.ToString();
                    b.Text = Raspored.SrediTekst(g.Natpis);
                    b.Tag = g.Redni.ToString() + "|" + (g.Naredba ?? "");
                    b.Left = Form2Izgled.LijevoGumba(g.Redni);
                    b.Top = 4;
                    b.Width = 75;
                    b.Visible = (b.Text ?? "").Trim().Length > 0;
                    b.Click += GumbKlik;
                    CarpoIzgled.Gumb(b);
                    ploca.Controls.Add(b);
                }
                Controls.Add(ploca);
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
            PrimijeniOnLoad2(forma, kontekst, citaj);
            // Form2.vb 1031: MAXIMIZEBOX (CBool), 991 + Timer3 9702: WINDOWSTATE "2" je maksimizirano (FRM-02).
            bool? maks = Form2Rezultat.VbBool(AtributForme("MAXIMIZEBOX"));
            if (maks.HasValue)
                MaximizeBox = maks.Value;
            if (AtributForme("WINDOWSTATE") == "2")
                WindowState = FormWindowState.Maximized;
            // Dock: kontrola straga se slaze prva. Ploca s gumbima ide na dno prva, a Fill (grid, DOCK=5)
            // zadnji, pa popuni ostatak (VB: grid i okvir BringToFront nakon Controls.Add).
            Control[] gumbi = Controls.Find("pnlGumbi", false);
            if (gumbi.Length > 0)
                gumbi[0].SendToBack();
            foreach (Control c in SveKontrole())
            {
                if (c.Dock == DockStyle.Fill)
                    c.BringToFront();
            }

            PrimijeniOnLoad();
            // Form2.vb 1170-1182 i čitanje po kontroli: NetGenDBClipboard nadjača ONLOAD i ONLOAD2 (DBC-01).
            UcitajDbClipboard(forma);
            OsvjeziStatus();
        }

        public bool ProvediTipku(Keys keyData)
        {
            Message m = Message.Create(Handle, 0x100, (IntPtr)keyData, IntPtr.Zero);
            return ProcessCmdKey(ref m, keyData);
        }

        private static Form VlasnikXtra(Form predlozen)
        {
            if (predlozen != null && !predlozen.IsDisposed && JeXtra(predlozen) && !JeRibbon(predlozen))
                return predlozen;
            foreach (Form f in Application.OpenForms)
            {
                if (f == null || f.IsDisposed || !f.IsHandleCreated || !JeXtra(f) || JeRibbon(f))
                    continue;
                return f;
            }
            return null;
        }

        private static bool JeXtra(Control f)
        {
            for (Type t = f.GetType(); t != null && t != typeof(object); t = t.BaseType)
            {
                if (t.Name == "XtraForm")
                    return true;
            }
            return false;
        }

        private static bool JeRibbon(Control f)
        {
            for (Type t = f.GetType(); t != null && t != typeof(object); t = t.BaseType)
            {
                string n = t.FullName ?? "";
                if (n.IndexOf("RibbonForm", StringComparison.Ordinal) >= 0
                    || n.IndexOf("RibbonBase", StringComparison.Ordinal) >= 0)
                    return true;
            }
            return false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // FOCUS forme (1053, Timer1 8510): nakon ucitavanja kontrola dobije fokus.
            string fokus = AtributForme("FOCUS");
            if (fokus.Length > 0)
                Fokusiraj(fokus);
            Action<Dijalog> h = NakonPrikaza;
            if (h == null)
                return;
            BeginInvoke(new Action(delegate ()
            {
                if (!IsDisposed)
                    h(this);
            }));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (_zatvoriRoditelja != null && _zatvoriRoditelja != this)
            {
                _zatvoriRoditelja = null;
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            // Form2_Activated 4025-4030: Rafalno(strONActivated) pri svakoj aktivaciji.
            IzvrsiDogadjaj(AtributForme("ONACTIVATED"));
        }

        /// <summary>Form2_Load 5794-5798: Rafalno(strONLOADED) prije prikaza (FRM-01).</summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IzvrsiDogadjaj(AtributForme("ONLOADED"));
        }

        /// <summary>Form2_FormClosed 4079-4083: Rafalno(strONClosed).</summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            string zatvoreno = AtributForme("ONCLOSED");
            if (zatvoreno.Length > 0)
            {
                try { IzvrsiDogadjaj(zatvoreno); }
                catch (Exception ex) { NgDnevnik.Greska("ONCLOSED: " + ex.Message); }
            }
            base.OnFormClosed(e);
        }

        private string AtributForme(string ime)
        {
            if (_forma == null || _forma.Atributi == null || !_forma.Atributi.Ima(ime))
                return "";
            return (_forma.Atributi.Vrijednost(ime) ?? "").Trim();
        }

        /// <summary>
        /// DTPicker (Form2.vb 2238-2280): Value je Now ili DEFAULT; Checked je True, SHOWCHECKBOX ga ugasi,
        /// CHECKED ga postavi. Neoznacen datum je parametar null (3561-3566), u Carpu EditValue null.
        /// CUSTOMFORMAT je i prikaz i oblik parametra (HH:mm, MM.yyyy, dd.MM.yyyy HH:mm) (DAT-01).
        /// </summary>
        private static void PripremiDatum(DateEdit d, NgControl k)
        {
            bool kvacica = Ponasanje.AllowEmptyJeTrue(Attr(k, "SHOWCHECKBOX")) || Attr(k, "SHOWCHECKBOX") == "1";
            bool oznacen = !kvacica;
            bool? cek = Form2Rezultat.VbBool(Attr(k, "CHECKED"));
            if (cek.HasValue)
                oznacen = cek.Value;
            if (kvacica)
                d.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            if (!oznacen)
            {
                d.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                d.EditValue = null;
            }
            else if (!(d.EditValue is DateTime))
                d.EditValue = DateTime.Now;
            string oblik = Attr(k, "CUSTOMFORMAT");
            if (oblik.Length > 0)
            {
                d.Properties.Mask.EditMask = oblik;
                d.Properties.Mask.UseMaskAsDisplayFormat = true;
                d.Properties.DisplayFormat.FormatString = oblik;
                d.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                d.Properties.EditFormat.FormatString = oblik;
                d.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            }
        }

        /// <summary>
        /// SrediKontrolu 3068-3150: STYLE/SIZE (font), FORECOLOR, BACKCOLOR; pa DOCK i ANCHOR (npr. 2362-2363).
        /// Samo brojevi vrijede (vidi Form2Izgled).
        /// </summary>
        private static void PrimijeniIzgled(Control c, NgControl k)
        {
            FontStyle? stil = Form2Izgled.Stil(Attr(k, "STYLE"));
            float? velicina = Form2Izgled.Velicina(Attr(k, "SIZE"));
            if (stil.HasValue || velicina.HasValue)
            {
                Font f = c.Font;
                try { c.Font = new Font(f.FontFamily, velicina ?? f.Size, stil ?? f.Style); }
                catch (Exception) { }
            }
            Color? prednja = Form2Izgled.Boja(Attr(k, "FORECOLOR"), false);
            if (prednja.HasValue)
                c.ForeColor = prednja.Value;
            Color? pozadina = Form2Izgled.Boja(Attr(k, "BACKCOLOR"), true);
            if (pozadina.HasValue)
                c.BackColor = pozadina.Value;
            // WinForms: Anchor i Dock iskljucuju jedno drugo, vrijedi zadnje postavljeno. VB postavi Dock pa
            // Anchor (labela, okvir, grid 1629/1737), osim dxgrida (1609-1610: Anchor pa Dock).
            DockStyle? dock = Form2Izgled.Dock(Attr(k, "DOCK"));
            AnchorStyles? sidro = Form2Izgled.Anchor(Attr(k, "ANCHOR"));
            if (k.Grana == NgGrana.DxGrid)
            {
                if (sidro.HasValue)
                    c.Anchor = sidro.Value;
                if (dock.HasValue)
                    c.Dock = dock.Value;
                return;
            }
            if (dock.HasValue)
                c.Dock = dock.Value;
            if (sidro.HasValue)
                c.Anchor = sidro.Value;
        }

        private void NapuniKontrolu(Control c, NgControl k)
        {
            NapuniKontrolu(c, k, null);
        }

        /// <summary>
        /// Combo i grid iz SQL atributa. Kod otvaranja SQL ide kroz SqlTekst.Sredi, a SKIPDEFAULTLOAD
        /// preskoce grid. Osvjezavanje (RefreshKontrola 5470, RefreshGrida 5284) dobije vec sredjen SQL
        /// s parametrima dijaloga i ne gleda SKIPDEFAULTLOAD.
        /// </summary>
        private void NapuniKontrolu(Control c, NgControl k, string gotovSql)
        {
            if (_citaj == null || k == null)
                return;
            if (k.Grana != NgGrana.Combo && k.Grana != NgGrana.Grid && k.Grana != NgGrana.DxGrid && k.Grana != NgGrana.MultiDrop)
                return;
            if (gotovSql == null && k.Grana != NgGrana.Combo && DijalogTok.PreskociGrid(k))
                return;
            if (k.Atributi == null || !k.Atributi.Ima("SQL"))
                return;
            string sql = (k.Atributi.Vrijednost("SQL") ?? "").Trim();
            if (sql.Length == 0)
                return;
            if (gotovSql != null)
            {
                // Zastita upisa je u _citaj (Podloga.Citaj), isto kao kod otvaranja.
                sql = gotovSql.Trim();
                if (sql.Length == 0)
                    return;
                NgDnevnik.Sql(sql);
            }
            else if (_kontekst != null)
                sql = SqlTekst.Sredi(sql, _kontekst);
            // MULTIDROP cita ALLOWEMPTY, ali ga ne koristi (2491): nema prazne stavke.
            bool prazan = k.Grana != NgGrana.MultiDrop && ComboDijalog.AllowEmpty(k);
            DataTable dt = null;
            try
            {
                dt = ComboDijalog.Ucitaj(sql, prazan, _citaj);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
            }
            if (dt == null || dt.Columns.Count == 0)
                return;
            var vise = c as CheckedComboBoxEdit;
            if (vise != null)
            {
                // Form2.vb 2478-2520: MULTIDROP, vrijednost stupac 0, natpis stupac 1 (MDR-01).
                string bilo = Oznaceni(vise);
                vise.Properties.DataSource = dt;
                vise.Properties.ValueMember = dt.Columns[0].ColumnName;
                vise.Properties.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
                if (gotovSql == null)
                    vise.Text = "";
                else
                    OznaciVise(vise, bilo);
                return;
            }
            var look = c as LookUpEdit;
            if (look != null)
            {
                look.Properties.DataSource = dt;
                look.Properties.ValueMember = dt.Columns[0].ColumnName;
                look.Properties.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
                // Osvjezavanje ostavi odabir korisnika (LookUpEdit zadrzi EditValue ako ga nova lista ima).
                if (gotovSql != null)
                    return;
                if (!prazan && ComboDijalog.OdabirAkoNista(dt, false) == "1")
                    look.EditValue = 1;
                string odabrano;
                string ostatak;
                if (ComboDijalog.OdvojiHash(c.Tag as string, out odabrano, out ostatak))
                {
                    c.Tag = ostatak;
                    if (odabrano.Length > 0)
                        look.EditValue = odabrano;
                }
                return;
            }
            var grid = c as GridControl;
            if (grid != null)
            {
                grid.DataSource = dt;
                var view = grid.MainView as GridView;
                if (view != null)
                    SloziGridDijaloga(c, k, view, dt);
            }
        }

        private readonly Dictionary<Control, string> _editStupci = new Dictionary<Control, string>();

        /// <summary>Form2.vb 1643-1649: EDITCOLUMNS je popis ili sp/select koji ga vrati (prva celija), jednom.</summary>
        private string EditStupci(Control c, NgControl k)
        {
            string gotovo;
            if (_editStupci.TryGetValue(c, out gotovo))
                return gotovo;
            string e = k == null || k.Atributi == null ? "" : (k.Atributi.Vrijednost("EDITCOLUMNS") ?? "");
            string l = e.Trim().ToLowerInvariant();
            if ((l.StartsWith("sp", StringComparison.Ordinal) || l.StartsWith("select", StringComparison.Ordinal)) && _citaj != null)
            {
                string sql = SrediPoslije.Zamijeni(e, "", _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false);
                string popis = "";
                try
                {
                    DataTable t = _citaj(sql);
                    if (t != null && t.Rows.Count > 0 && t.Columns.Count > 0 && t.Rows[0][0] != DBNull.Value)
                        popis = Convert.ToString(t.Rows[0][0]) ?? "";
                }
                catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sql); }
                e = popis;
            }
            _editStupci[c] = e;
            return e;
        }

        private readonly List<KeyValuePair<Control, NgControl>> _nakonDjeteta = new List<KeyValuePair<Control, NgControl>>();
        private int _dubinaOsvjezavanja;
        private int _tihoGrid;
        private bool _slazem;
        private int _dubinaDogadjaja;

        /// <summary>Form2.RefreshKontrola (5470-5602): combo i grid iz Cdt (REFRESHAFTERCHILD="TRUE").</summary>
        public void OsvjeziNakonDjeteta()
        {
            for (int i = 0; i < _nakonDjeteta.Count; i++)
            {
                Control c = _nakonDjeteta[i].Key;
                NgControl k = _nakonDjeteta[i].Value;
                if (c == null || c.IsDisposed)
                    continue;
                if (k.Grana == NgGrana.Combo)
                    NapuniKontrolu(c, k, SqlDijaloga(k));
                else
                    OsvjeziGrid(c, k, "");
            }
        }

        /// <summary>
        /// RefreshControlSQL (5460) je SrediSQL2, pa SrediSQLPosljeNextForme s parametrima dijaloga
        /// (strPrevParametri = strParametriSvi, 5494 i 5304).
        /// </summary>
        private string SqlDijaloga(NgControl k)
        {
            string sirovo = k == null || k.Atributi == null ? "" : (k.Atributi.Vrijednost("SQL") ?? "");
            string sql = SrediPoslije.Zamijeni(sirovo, PokupiParametre(), _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false);
            return SrediPoslije.Kolone(sql, VrijednostIzGrida);
        }

        /// <summary>
        /// Form2.RefreshGrida (5284-5430): novi podaci, povratak na isti red (prvi stupac podatka,
        /// Janus stupac 2), move* iz vrijednosti, pa AFTERREFRESH kroz Rafalno (5426).
        /// </summary>
        private void OsvjeziGrid(Control c, NgControl k, string naredba)
        {
            var grid = c as GridControl;
            var view = grid == null ? null : grid.MainView as GridView;
            string kljuc = null;
            int stariRed = view == null ? -1 : view.FocusedRowHandle;
            var stara = grid == null ? null : grid.DataSource as DataTable;
            if (view != null && stara != null && stara.Columns.Count > 0 && stariRed >= 0)
            {
                object v = view.GetRowCellValue(stariRed, stara.Columns[0].ColumnName);
                kljuc = v == null || v == DBNull.Value ? null : Convert.ToString(v).Trim();
            }
            _tihoGrid++;
            try
            {
                NapuniKontrolu(c, k, SqlDijaloga(k));
                if (view != null && view.DataRowCount > 0)
                {
                    if (!PomakniNaKljuc(view, kljuc) && stariRed >= 0)
                        view.FocusedRowHandle = Math.Min(stariRed, view.DataRowCount - 1);
                    PomakniGrid(view, naredba);
                }
            }
            finally
            {
                _tihoGrid--;
            }
            // SetDataBinding i MoveFirst/Find u VB-u okinu SelectionChanged, pa ONCLICK grida.
            // Ovdje jednom, kad je red na mjestu (detalj u drugom gridu prati novi red).
            string klikGrida = Attr(k, "ONCLICK");
            if (view != null && klikGrida.Length > 0 && view.DataRowCount > 0 && !_slazem)
                IzvrsiDogadjaj(klikGrida);
            string poslije = Attr(k, "AFTERREFRESH");
            if (poslije.Length > 0 && _dubinaOsvjezavanja < 5)
            {
                _dubinaOsvjezavanja++;
                try { IzvrsiDogadjaj(poslije); }
                finally { _dubinaOsvjezavanja--; }
            }
        }

        private static bool PomakniNaKljuc(GridView view, string kljuc)
        {
            var dt = view.GridControl == null ? null : view.GridControl.DataSource as DataTable;
            if (kljuc == null || dt == null || dt.Columns.Count == 0)
                return false;
            string stupac = dt.Columns[0].ColumnName;
            for (int r = 0; r < view.DataRowCount; r++)
            {
                object v = view.GetRowCellValue(r, stupac);
                if (v != null && v != DBNull.Value && string.Equals(Convert.ToString(v).Trim(), kljuc, StringComparison.Ordinal))
                {
                    view.FocusedRowHandle = r;
                    return true;
                }
            }
            return false;
        }

        /// <summary>RefreshGrida 5351-5365 i RefreshControls 5043-5065: movelast, movefirst, movenext, moveprevious.</summary>
        private static void PomakniGrid(GridView view, string naredba)
        {
            string n = (naredba ?? "").ToLowerInvariant();
            if (n.Length == 0 || view.DataRowCount == 0)
                return;
            if (n.Contains("movelast"))
                view.FocusedRowHandle = view.DataRowCount - 1;
            if (n.Contains("movefirst"))
                view.FocusedRowHandle = 0;
            if (n.Contains("movenext") && view.FocusedRowHandle < view.DataRowCount - 1)
                view.FocusedRowHandle = view.FocusedRowHandle + 1;
            if (n.Contains("moveprevious") && view.FocusedRowHandle > 0)
                view.FocusedRowHandle = view.FocusedRowHandle - 1;
        }

        /// <summary>Lanac je vise koraka ili korak koji nije obican SQL (Form2.Rafalno 6789).</summary>
        private static bool JeLanac(System.Collections.Generic.List<DijalogTok.Korak> koraci)
        {
            if (koraci.Count > 1)
                return true;
            for (int s = 0; s < koraci.Count; s++)
            {
                string v = koraci[s].Vrsta;
                if (v == "poruka" || v == "roditelj" || v == "odustani" || v == "ok" || v == "okstay" || v == "forma"
                    || v == "foreach" || v == "fiskal" || v == "datoteka" || v == "refresh" || v == "dock"
                    || v == "eval" || v == "sljedeci" || v == "prethodni" || v == "naredba"
                    || v == "recalc" || v == "recalcred" || v == "toggle" || v == "priprema" || v == "pivot" || v == "chart" || v == "boja" || v == "posalji" || v == "pokretanje" || v == "beep" || v == "ispisext"
                    || v == "eval" || v == "sljedeci" || v == "prethodni" || v == "svekontrole" || v == "proces" || v == "mapa" || v == "datotekadlg" || v == "ispis" || v == "blagajna"
                    || v == "timer" || v == "timeroff" || v == "progres" || v == "trazi" || v == "stupac" || v == "objekt" || v == "predlozak" || v == "prostor")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Form2.Rafalno (6789-7523): koraci slijeva nadesno. Gumb i svi dogadjaji kontrola idu ovuda.
        /// Vraca "" kad lanac prode ili ga naredba prekine (CANCELBOX, ALERT, zatvaranje), a tekst greske SQL-a
        /// (GreskaKodIzvrsavanjaSQLa). Greska SQL-a prekida lanac (Exit For, 7511-7516).
        /// </summary>
        private string IzvrsiKorake(System.Collections.Generic.List<DijalogTok.Korak> koraci, DijalogTok.Klik klik)
        {
            for (int i = 0; i < koraci.Count; i++)
            {
                DijalogTok.Korak korak = koraci[i];
                // Form2.vb 6812-6819: segment s @@ se prvo zamijeni (PokupiParametre,
                // SrediSQLPosljeNextForme), pa se tek onda prepozna. Tako :@@imereporta1(...)
                // postane ispis, a @@izvjestaj(...) naredba iz upita (LAN-03).
                if (korak.Vrsta != "foreach" && (korak.Izvor ?? "").IndexOf("@@", StringComparison.Ordinal) >= 0)
                {
                    var nakonZamjene = DijalogTok.Koraci(Pripremi(korak.Izvor));
                    if (nakonZamjene.Count > 0)
                        korak = nakonZamjene[0];
                }
                if (korak.Vrsta == "assembly")
                {
                    // Form2.vb 7424-7456: potpis dijaloga. Pad Load/CreateInstance prekida lanac.
                    var poziv = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(korak.Tekst, "dijalog", Id, Operater, Konekcija, ZadnjiSql, "", SviParametri());
                    if (poziv.Prekinuto)
                        return "";
                    continue;
                }
                if (korak.Vrsta == "prostor")
                {
                    // Form2.vb 7489: PoslovniProstor. Kao na podlozi: fiskalni dio se biljezi (R109/R111).
                    NgDnevnik.Akcija("prostor " + ProstorNaredba.Argument(korak.Tekst) + " poziva=" + ProstorNaredba.Poziva(0));
                    continue;
                }
                if (korak.Vrsta == "poruka")
                {
                    PorukaNaredba.Pokazi("MSGBOX(" + korak.Tekst + ")");
                    continue;
                }
                if (korak.Vrsta == "odustani" || korak.Vrsta == "roditelj")
                {
                    if (korak.Vrsta == "roditelj")
                        _zatvoriRoditelja = this;
                    DialogResult = DialogResult.Cancel;
                    Close();
                    continue;
                }
                if (korak.Vrsta == "ok" || korak.Vrsta == "okstay")
                {
                    if (korak.Vrsta == "ok")
                        DialogResult = DialogResult.OK;
                    if (klik.SaljeSql && _citaj != null && klik.MasterSql.Length > 0)
                    {
                        string sqlOk = Lanac.BezKrajnjegZareza(SqlZaGumb(klik.MasterSql));
                        try { _citaj(sqlOk); }
                        catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sqlOk); }
                    }
                    continue;
                }
                if (korak.Vrsta == "klik")
                {
                    Control[] cilj = Controls.Find(korak.Tekst, true);
                    if (cilj.Length > 0)
                    {
                        var dx = cilj[0] as SimpleButton;
                        var wb = cilj[0] as Button;
                        if (dx != null)
                            dx.PerformClick();
                        else if (wb != null)
                            wb.PerformClick();
                        else
                            cilj[0].Focus();
                    }
                    if (IsDisposed)
                        return "";
                    continue;
                }
                if (korak.Vrsta == "forma")
                {
                    OtvorenaForma = korak.Tekst;
                    if (OtvoriDrugu != null)
                        OtvoriDrugu(korak.Tekst);
                    // Form2.NovaGenerickaForma2 4443/4478: kad se dijete zatvori (OK ili ne),
                    // osvjeze se combo i grid s REFRESHAFTERCHILD="TRUE" (RAC-01).
                    if (IsDisposed)
                        return "";
                    if (DijalogTok.OsvjezavaNakonDjeteta(korak.Tekst))
                        OsvjeziNakonDjeteta();
                    continue;
                }
                if (korak.Vrsta == "foreach")
                {
                    IzvrsiForeach(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "fiskal")
                {
                    FiskalTok.Izvrsi(FiskalNaredba.Tablica(korak.Tekst), FiskalNaredba.Argument(korak.Tekst), _citaj);
                    continue;
                }
                if (korak.Vrsta == "sql" && _citaj != null && !_jupiter)
                {
                    string dio = Pripremi(korak.Tekst);
                    if (!DijalogTok.SmijePoslati(dio))
                        continue;
                    NgDnevnik.Sql(dio);
                    DataTable rez;
                    try { rez = _citaj(dio); }
                    catch (Exception ex)
                    {
                        // Form2.vb 7510-7516: RefreshControls postavi GreskaKodIzvrsavanjaSQLa
                        // i Exit For. Ostatak lanca (ispis, OK, nova forma) se ne izvrsi (EVT-03).
                        NgDnevnik.Greska(ex.Message + " | " + dio);
                        // 7512-7514 i Class1.ExecSql 190-193: poruka s "#close"/"+close" zatvara formu (MSG-03).
                        string m = (ex.Message ?? "").ToLowerInvariant();
                        if (m.Contains("#close") || m.Contains("+close"))
                        {
                            DialogResult = DialogResult.Cancel;
                            Close();
                        }
                        return string.IsNullOrEmpty(ex.Message) ? "greska" : ex.Message;
                    }
                    PrimeniRed(rez);
                    continue;
                }
                if (korak.Vrsta == "boja")
                {
                    string html = BojaNaredba.Pokazi();
                    if (_kontekst != null)
                        _kontekst.SelectedColor = html;
                    string unutra = BojaNaredba.Sql(korak.Tekst);
                    if (unutra.Length > 0)
                        IzvrsiSql(unutra);
                    continue;
                }
                if (korak.Vrsta == "pokretanje")
                {
                    Pokretanje.Pokreni(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "posalji")
                {
                    PosaljiNaServer(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "beep")
                {
                    RafalNaredba.Beep();
                    continue;
                }
                if (korak.Vrsta == "refresh")
                {
                    OsvjeziKontrolu(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "dock")
                {
                    int uspravno = korak.Tekst.IndexOf('|');
                    string stil = uspravno < 0 ? korak.Tekst : korak.Tekst.Substring(0, uspravno);
                    string ime = uspravno < 0 ? "" : korak.Tekst.Substring(uspravno + 1);
                    Control[] ciljDock = Controls.Find(ime, true);
                    if (ciljDock.Length > 0)
                        RafalNaredba.PostaviDock(ciljDock[0], stil);
                    continue;
                }
                if (korak.Vrsta == "datoteka")
                {
                    ZapisiDatoteku(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "naredba")
                {
                    if (PraznoNaredba.PreskociSljedeci(korak.Tekst))
                        i++;
                    else if (korak.Tekst.ToUpperInvariant().IndexOf("ISEMPTYORZEROALERT(", StringComparison.Ordinal) >= 0)
                    {
                        if (!RafalNaredba.AlertNastavi(RafalNaredba.UpozorenjeParam(korak.Tekst), RafalNaredba.UpozorenjeTekst(korak.Tekst)))
                            return "";
                    }
                    else if (korak.Tekst.ToUpperInvariant().IndexOf("ISEMPTYORZEROCANCELBOX(", StringComparison.Ordinal) >= 0)
                    {
                        if (!RafalNaredba.OtkazImaPoruku(korak.Tekst))
                            return "";
                        if (RafalNaredba.CancelBoxStani(RafalNaredba.OtkazParam(korak.Tekst), RafalNaredba.OtkazTekst(korak.Tekst)))
                        {
                            string ime = RafalNaredba.OtkazParam(korak.Tekst).TrimStart('@');
                            Control[] f = Controls.Find(ime, true);
                            if (f.Length > 0)
                                f[0].Focus();
                            return "";
                        }
                    }
                    continue;
                }
                if (korak.Vrsta == "eval")
                {
                    int jednako = (korak.Tekst ?? "").IndexOf('=');
                    string ime = jednako < 0 ? (korak.Tekst ?? "") : korak.Tekst.Substring(0, jednako);
                    string izraz = jednako < 0 ? "" : korak.Tekst.Substring(jednako + 1);
                    string v = Izraz.Izracunaj(izraz);
                    Control[] ciljEval = Controls.Find(ime.Trim(), true);
                    if (ciljEval.Length > 0)
                        ciljEval[0].Text = v;
                    continue;
                }
                if (korak.Vrsta == "svekontrole")
                {
                    RafalNaredba.SveKontrole(this, korak.Tekst == "1");
                    continue;
                }
                if (korak.Vrsta == "proces")
                {
                    RafalNaredba.ProcessStart(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "mapa")
                {
                    RafalNaredba.OdaberiMapu();
                    continue;
                }
                if (korak.Vrsta == "datotekadlg")
                {
                    RafalNaredba.OdaberiDatoteku();
                    continue;
                }
                if (korak.Vrsta == "timer")
                {
                    RafalNaredba.TimerOn(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "timeroff")
                {
                    RafalNaredba.TimerOff();
                    continue;
                }
                if (korak.Vrsta == "progres")
                {
                    RafalNaredba.Progres(korak.Tekst == "1");
                    continue;
                }
                if (korak.Vrsta == "trazi")
                {
                    string[] dio = RafalNaredba.StartSearch(korak.Tekst);
                    Control[] ciljT = Controls.Find(dio[0], true);
                    if (ciljT.Length > 0)
                    {
                        ciljT[0].Focus();
                        ciljT[0].Text = dio.Length > 1 ? dio[1] : "";
                    }
                    continue;
                }
                if (korak.Vrsta == "stupac")
                {
                    PomakniNaVrijednost(korak.Tekst);
                    continue;
                }
                if (korak.Vrsta == "objekt")
                {
                    PorukaNaredba.Pokazi("MSGBOX(" + RafalNaredba.CreateObject(korak.Tekst) + ")");
                    continue;
                }
                if (korak.Vrsta == "recalc" || korak.Vrsta == "recalcred")
                {
                    PreracunajGrid(korak.Vrsta == "recalcred");
                    continue;
                }
                if (korak.Vrsta == "toggle")
                {
                    ToggleGrid.Korak(ref _grupe);
                    continue;
                }
                if (korak.Vrsta == "priprema")
                {
                    _priprema = string.Join(",", MultiIspis.Idjevi(OznaceniId()));
                    continue;
                }
                if (korak.Vrsta == "sljedeci" || korak.Vrsta == "prethodni")
                {
                    PomakniRed(korak.Vrsta == "sljedeci");
                    continue;
                }
                if (korak.Vrsta == "blagajna")
                {
                    NgDnevnik.Akcija(BlagajnaNaredba.Upit(BlagajnaNaredba.Id(korak.Tekst)));
                    continue;
                }
                if (korak.Vrsta == "ispis" && korak.Tekst.IndexOf("PRINTGRID(", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Form2.vb 6950-6955: TempGrid = Controls(ime), pa IspisGRIDREPX s njegovim zaglavljima (ISP-07).
                    IspisiGrid(korak.Tekst, GridIzNaredbe(korak.Tekst));
                    continue;
                }
                if (korak.Vrsta == "ispis" || korak.Vrsta == "predlozak")
                {
                    Carpo.NetGen.Print.PrintBridge.Ispisi(korak.Tekst, true, Id, Operater, Konekcija, "", ZadnjiSql, "", SviParametri(), "", "", "", Text, null, "", null);
                    continue;
                }
                if (korak.Vrsta == "pivot" || korak.Vrsta == "chart" || korak.Vrsta == "ispisext")
                {
                    EkstenzijaGrid.Prikazi(korak.Vrsta == "ispisext" ? "ispis" : korak.Vrsta);
                    continue;
                }
                if (korak.Vrsta == "sql")
                    IzvrsiSql(korak.Tekst);
            }
            return "";
        }

        /// <summary>Form2.ToolStripMenuItem1_Click Case Else: s '~' je lanac, inace NovaGenerickaForma2.</summary>
        private void IzvrsiIzbornik(string naredba)
        {
            string lanac = GridIzbornik.Lanac(naredba, false);
            if (lanac.Length > 0)
                IzvrsiDogadjaj(lanac);
            else
                NovaGenerickaForma(naredba);
        }

        /// <summary>
        /// Form2.NovaGenerickaForma2 (4259-4479): ispis (VRSTADOKID-, RSGEN, GG/, RS/, RS:, REPX), objekt
        /// (tocka bez @) ili forma. Nakon forme RefreshKontrola. Ime moze imati (parametri) sa @@.
        /// </summary>
        private void NovaGenerickaForma(string tag)
        {
            string ime = Pripremi((tag ?? "").Trim());
            if (ime.Length == 0)
                return;
            if (!DijalogTok.OsvjezavaNakonDjeteta(ime))
            {
                string u = ime.ToUpperInvariant();
                bool objekt = !(u.StartsWith("VRSTADOKID-", StringComparison.Ordinal) || u.StartsWith("RSGEN", StringComparison.Ordinal)
                    || u.StartsWith("GG/", StringComparison.Ordinal) || u.StartsWith("RS/", StringComparison.Ordinal) || u.StartsWith("RS:", StringComparison.Ordinal)
                    || u.StartsWith("REPX", StringComparison.Ordinal) || ime.ToLowerInvariant().Contains(".clsreport"));
                if (objekt)
                {
                    NgDnevnik.Akcija("objekt " + ime);
                    return;
                }
                Carpo.NetGen.Print.PrintBridge.Ispisi(ime, true, Id, Operater, Konekcija, "", ZadnjiSql, "", SviParametri(), "", "", "", Text, null, "", null);
                return;
            }
            OtvorenaForma = ime;
            if (OtvoriDrugu != null)
                OtvoriDrugu(ime);
            if (!IsDisposed)
                OsvjeziNakonDjeteta();
        }

        /// <summary>
        /// Dogadjaj kontrole (ONCLICK labele i kvacice, ONLOSTFOCUS, ONCHANGE, ONVALUECHANGED, GOTFOCUS,
        /// ONDBLCLICK, ONCELLEDITED, ONENTERKEY, AFTERREFRESH). Form2 ih sve salje u Rafalno
        /// (5638-6228, 8506-8677, 9439-9474), isto kao gumb. Jedan obican SQL ide kao i prije (EVT-01).
        /// </summary>
        private string IzvrsiDogadjaj(string tekst)
        {
            // Dok se dijalog slaze, VB jos nema AddHandler (handleri idu nakon punjenja kontrola).
            if (_slazem || IsDisposed || (tekst ?? "").Trim().Length == 0)
                return "";
            // Postavljanje teksta okine ONCHANGE, refresh okine AFTERREFRESH... VB nema granicu;
            // ovdje lanac koji sam sebe zove stane nakon 8 razina.
            if (_dubinaDogadjaja >= 8)
            {
                NgDnevnik.Greska("dogadjaj se vrti u krug: " + tekst);
                return "";
            }
            _dubinaDogadjaja++;
            try
            {
                return IzvrsiDogadjajTijelo(tekst);
            }
            finally
            {
                _dubinaDogadjaja--;
            }
        }

        private string IzvrsiDogadjajTijelo(string tekst)
        {
            string t = tekst ?? "";
            PokupiParametre();
            var koraci = DijalogTok.Koraci(t);
            if (koraci.Count == 0)
                return "";
            if (koraci.Count == 1 && koraci[0].Vrsta == "sql")
            {
                // Segment s @@ se prepozna tek nakon zamjene (LAN-03): @@izvjestaj moze biti naredba.
                bool ostajeSql = (koraci[0].Izvor ?? "").IndexOf("@@", StringComparison.Ordinal) < 0;
                if (!ostajeSql)
                {
                    var nakon = DijalogTok.Koraci(Pripremi(koraci[0].Izvor));
                    ostajeSql = nakon.Count == 0 || (nakon.Count == 1 && nakon[0].Vrsta == "sql");
                }
                if (ostajeSql)
                    return IzvrsiSqlGreska(t);
            }
            DijalogTok.Klik klik = DijalogTok.Gumb(_forma, 0, Pripremi(t), _jupiter);
            if (!string.IsNullOrEmpty(klik.MasterSql))
            {
                klik.MasterSql = Pripremi(klik.MasterSql);
                if (klik.MasterSql.TrimEnd().EndsWith(",", StringComparison.Ordinal))
                    klik.MasterSql = Lanac.BezKrajnjegZareza(Lanac.Lijepi(klik.MasterSql, SviParametri()));
            }
            return IzvrsiKorake(koraci, klik);
        }

        private void GumbKlik(object sender, EventArgs e)
        {
            PokupiParametre();
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
            // Form2.Button3..6_Click (5800-5818): CMD3-CMD6 zovu NovaGenerickaForma2(Tag) izravno,
            // bez Rafalna. Tag je forma, ispis ili objekt, nikad SQL (BTN-02).
            if (redni >= 3 && b != null && b.Name != null && b.Name.StartsWith("cmd", StringComparison.Ordinal))
            {
                NovaGenerickaForma(naredba);
                return;
            }
            // Lanac ide slijeva nadesno. @@selectedcolor postoji tek nakon COLORPICKER (Form2.vb 7156).
            string sirovo = naredba;
            naredba = Pripremi(naredba);
            DijalogTok.Klik klik = DijalogTok.Gumb(_forma, redni, naredba, _jupiter);
            if (!string.IsNullOrEmpty(klik.MasterSql))
            {
                klik.MasterSql = Pripremi(klik.MasterSql);
                // Form1.vb 5452: ONSUBMIT koji završava zarezom dobije strParametri.
                if (klik.MasterSql.TrimEnd().EndsWith(",", StringComparison.Ordinal))
                    klik.MasterSql = Lanac.BezKrajnjegZareza(Lanac.Lijepi(klik.MasterSql, SviParametri()));
            }
            var koraci = DijalogTok.Koraci(sirovo);
            if (JeLanac(koraci))
            {
                IzvrsiKorake(koraci, klik);
                return;
            }
            if (KlikNaredba.Kontrola(naredba).Length > 0)
            {
                string ime = KlikNaredba.Kontrola(naredba);
                Control[] cilj = Controls.Find(ime, true);
                if (cilj.Length > 0)
                    cilj[0].Focus();
                return;
            }
            MasterSql = _jupiter ? "" : klik.MasterSql;
            if (!DijalogTok.DozvoliUpis && Carpo.NetGen.Print.PrintBridge.Vrsta(naredba, true).Length > 0)
                IspisiGrid(naredba, GridIzNaredbe(naredba));
            for (int i = 0; i < klik.Naredbe.Count; i++)
            {
                string n = klik.Naredbe[i] ?? "";
                if (n.IndexOf("ASSEMBLYLOAD(", StringComparison.Ordinal) < 0)
                    continue;
                string svi = SviParametri();
                var poziv = Carpo.NetGen.Poziv.AssemblyPoziv.Pozovi(n, "dijalog", Id, Operater, Konekcija, ZadnjiSql, "", svi);
                if (poziv.Prekinuto)
                    return;
            }
            if (_citaj != null)
            {
                for (int i = 0; i < klik.Naredbe.Count; i++)
                {
                    string dio = klik.Naredbe[i];
                    if (!DijalogTok.SmijePoslati(dio))
                        continue;
                    NgDnevnik.Sql(dio);
                    DataTable rez;
                    try
                    {
                        rez = _citaj(dio);
                    }
                    catch (Exception ex)
                    {
                        // RefreshControls: greska prekida (GreskaKodIzvrsavanjaSQLa), forma ostaje otvorena.
                        NgDnevnik.Greska(ex.Message + " | " + dio);
                        return;
                    }
                    // Rafalno -> RefreshControls: rezultat gumba postavlja kontrole kao i dogadjaj (BTN-03).
                    PrimeniRed(rez);
                    if (IsDisposed)
                        return;
                }
                if (klik.SaljeSql && !string.IsNullOrEmpty(klik.MasterSql) && DijalogTok.SmijePoslati(klik.MasterSql))
                {
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
            }
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

        private string Pripremi(string naredba)
        {
            string s = ZamijeniSearch(naredba);
            s = ZamijeniIzKontrola(s);
            if (_kontekst != null)
                s = SqlTekst.ZaGumb(s, _kontekst);
            else if (s.IndexOf("@@", StringComparison.Ordinal) >= 0)
                s = SrediPoslije.Petlja(s, "", null, false);
            return SrediPoslije.Kolone(s, VrijednostIzGrida);
        }

        /// <summary>
        /// Form2.NadjiVrijednostuGridu (8457-8487): @(grid..stupac) iz trenutnog reda grida. Nema grida: "".
        /// Nema reda, stupca ili vrijednosti: "null". Datum yyyyMMdd, broj s tockom, tekst Trim.
        /// </summary>
        private string VrijednostIzGrida(string gridIme, string kolona)
        {
            GridControl grid = null;
            foreach (Control c in SveKontrole())
            {
                var g = c as GridControl;
                if (g != null && string.Equals((g.Name ?? "").Trim(), (gridIme ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    grid = g;
                    break;
                }
            }
            if (grid == null)
                return "";
            var view = grid.MainView as GridView;
            var dt = grid.DataSource as DataTable;
            if (view == null || dt == null || view.FocusedRowHandle < 0 || !dt.Columns.Contains(kolona ?? ""))
                return "null";
            object v = view.GetRowCellValue(view.FocusedRowHandle, dt.Columns[kolona].ColumnName);
            if (v == null || v == DBNull.Value)
                return "null";
            if (v is DateTime)
                return ((DateTime)v).ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string tekst = Convert.ToString(v, CultureInfo.CurrentCulture) ?? "";
            double broj;
            if (!(v is string) && v is IFormattable
                || double.TryParse(tekst, NumberStyles.Any, CultureInfo.CurrentCulture, out broj))
                return tekst.Replace(",", ".");
            return tekst.Trim();
        }

        private string ZamijeniIzKontrola(string naredba)
        {
            if (string.IsNullOrEmpty(naredba))
                return "";
            var kontrole = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var par in _izUpita)
                kontrole[par.Key] = par.Value;
            SkupiTekst(this, kontrole);
            string id = _kontekst == null ? "" : _kontekst.Id;
            return DijalogTok.ZamijeniTokene(naredba, id, kontrole);
        }

        private static void SkupiTekst(Control roditelj, Dictionary<string, string> kontrole)
        {
            if (roditelj == null)
                return;
            for (int i = 0; i < roditelj.Controls.Count; i++)
            {
                Control c = roditelj.Controls[i];
                if (!string.IsNullOrEmpty(c.Name))
                {
                    var broj = c as SpinEdit;
                    var kvacica = c as CheckEdit;
                    var look = c as LookUpEdit;
                    var edit = c as TextEdit;
                    var datum = c as DateEdit;
                    var vise = c as CheckedComboBoxEdit;
                    if (vise != null)
                        kontrole[c.Name] = Oznaceni(vise);
                    else if (datum != null)
                    {
                        // strParametriSvi daje datum kao 'yyyyMMdd' ili null (3522-3566), ne prikazani tekst.
                        kontrole[c.Name] = datum.EditValue is DateTime
                            ? ((DateTime)datum.EditValue).ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                            : "null";
                    }
                    else if (broj != null)
                        kontrole[c.Name] = Convert.ToString(broj.Value, CultureInfo.InvariantCulture);
                    else if (kvacica != null)
                        kontrole[c.Name] = kvacica.Checked ? "1" : "0";
                    else if (look != null)
                    {
                        object ev = look.EditValue;
                        kontrole[c.Name] = ev == null || ev == DBNull.Value
                            ? "null"
                            : Convert.ToString(ev, CultureInfo.InvariantCulture);
                    }
                    else if (c.GetType() == typeof(ButtonEdit))
                        kontrole[c.Name] = ParametriSvi.SearchId(c.Tag);
                    else if (edit != null)
                        kontrole[c.Name] = edit.Text ?? "";
                }
                SkupiTekst(c, kontrole);
            }
        }

        /// <summary>
        /// REFRESHCONTROL(ime) (Form2.vb 6911-6915) je RefreshControls("select 'refresh' as ime"):
        /// isti put kao stupac "refresh" u rezultatu. Combo i grid se pune s parametrima dijaloga,
        /// grid zadrzi red i okine AFTERREFRESH.
        /// </summary>
        private void OsvjeziKontrolu(string ime)
        {
            string n = (ime ?? "").Trim();
            if (n.Length == 0)
                return;
            var dt = new DataTable();
            dt.Columns.Add(n, typeof(string));
            dt.Rows.Add("refresh");
            PrimeniRed(dt);
        }

        private void ZapisiDatoteku(string segment)
        {
            int vrsta = RafalNaredba.Datoteka(segment);
            string put = RafalNaredba.DatotekaPut(segment);
            string sql = RafalNaredba.DatotekaSql(segment);
            if (vrsta == 0 || put.Length == 0)
                return;
            var redovi = new System.Collections.Generic.List<string[]>();
            if (_citaj != null && sql.Length > 0 && !ZastitaSql.Tekst(sql).Pise)
            {
                try
                {
                    DataTable dt = _citaj(sql);
                    if (dt != null)
                    {
                        for (int r = 0; r < dt.Rows.Count; r++)
                        {
                            var kol = new string[dt.Columns.Count];
                            for (int c = 0; c < dt.Columns.Count; c++)
                                kol[c] = dt.Rows[r][c] == DBNull.Value ? "" : Convert.ToString(dt.Rows[r][c]);
                            redovi.Add(kol);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska(ex.Message + " | " + sql);
                    return;
                }
            }
            try { RafalNaredba.ZapisiDatoteku(put, vrsta, redovi.ToArray()); }
            catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + put); }
        }

        private string SqlZaGumb(string master)
        {
            string izvor = master ?? "";
            if (_forma != null && !string.IsNullOrEmpty(_forma.OnSubmit))
                izvor = _forma.OnSubmit;
            return SrediPoslije.ZaSlanje(izvor, SviParametri(), _kontekst ?? new SqlKontekst(), null);
        }

        private void IzvrsiSql(string tekst)
        {
            IzvrsiSqlGreska(tekst);
        }

        private string IzvrsiSqlGreska(string tekst)
        {
            if (_citaj == null || _jupiter)
                return "";
            // RefreshControls 4556-4566: PokupiParametre, "++" je strParametri, SrediSQL2 i
            // SrediSQLPosljeNextForme s strParametriSvi. Token koji nije u nizu trazi se u CB.
            // Rafalno 6812-6818: segment s @SEARCH dobije i parametre iz trazilice.
            string svi = PokupiParametre();
            string t = (tekst ?? "").Replace("++", svi);
            if (t.IndexOf("@SEARCH", StringComparison.OrdinalIgnoreCase) >= 0)
                svi = svi + _parametriIzTrazilice;
            string sql = SrediPoslije.Zamijeni(t, svi, _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false);
            sql = SrediPoslije.Kolone(sql, VrijednostIzGrida);
            if (ZastitaSql.Tekst(sql).Pise && !DijalogTok.DozvoliUpis)
                return "";
            try
            {
                DataTable dt = _citaj(sql);
                PrimeniRed(dt);
                return "";
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
                return ex.Message ?? "greska";
            }
        }

        /// <summary>
        /// Form2.RefreshControls (4556-5190): prvi red rezultata postavlja formu i kontrole.
        /// Posebni stupci idu redom kao u VB-u: focus, formcaption, beep, form, formwidth, formheight,
        /// click, setcolorforcolorpicker, id, temp. Onda svaka kontrola ciji stupac postoji (RES-01).
        /// </summary>
        private void PrimeniRed(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0 || IsDisposed)
                return;
            DataRow r = dt.Rows[0];
            string fokus = Form2Rezultat.Celija(r, "focus");
            if (fokus.Length > 0)
                Fokusiraj(fokus);
            string naslov = Form2Rezultat.Celija(r, "formcaption");
            if (naslov.Length > 0)
                Text = naslov;
            int beep = Form2Rezultat.Beep(Form2Rezultat.Celija(r, "beep"));
            for (int i = 0; i < beep; i++)
                RafalNaredba.Beep();
            string dijete = Form2Rezultat.Celija(r, "form");
            if (dijete.Length > 0)
            {
                OtvorenaForma = dijete;
                if (OtvoriDrugu != null)
                    OtvoriDrugu(dijete);
                if (IsDisposed)
                    return;
                if (DijalogTok.OsvjezavaNakonDjeteta(dijete))
                    OsvjeziNakonDjeteta();
            }
            int mjera;
            if (int.TryParse(Form2Rezultat.Celija(r, "formwidth").Trim(), out mjera) && mjera > 0)
                Width = mjera;
            if (int.TryParse(Form2Rezultat.Celija(r, "formheight").Trim(), out mjera) && mjera > 0)
                Height = mjera;
            string klikni = Form2Rezultat.Celija(r, "click");
            if (klikni.Length > 0)
                KlikniKontrolu(klikni.Trim());
            if (IsDisposed)
                return;
            string boja = Form2Rezultat.Celija(r, "setcolorforcolorpicker");
            if (boja.Length > 0 && _kontekst != null)
                _kontekst.SelectedColor = boja;
            // 4710-4744: "id" postavi ID forme (FreezID Carpo ne zna), "temp" takodjer.
            string noviId = Form2Rezultat.Celija(r, "id");
            if (noviId.Length > 0)
                PostaviId(noviId);
            string temp = Form2Rezultat.Celija(r, "temp");
            if (temp.Length > 0)
                PostaviId(temp);
            // 4753-4757: StatusStrip prije petlje po kontrolama (STS-01).
            OsvjeziStatus();

            var kontrole = SveKontrole().ToArray();
            for (int i = 0; i < kontrole.Length; i++)
            {
                Control c = kontrole[i];
                if (c == null || c.IsDisposed || string.IsNullOrEmpty(c.Name))
                    continue;
                string ime = c.Name.Trim();
                if (!Form2Rezultat.Ima(r, ime))
                {
                    // 4785-4793: PictureBox gleda i "<ime>location" kad stupca s imenom nema (PIC-01).
                    var slika = c as PictureEdit;
                    if (slika != null)
                        UcitajSliku(slika, Form2Rezultat.Celija(r, ime + "location"));
                    continue;
                }
                object sirovo = r[ime];
                string v = Form2Rezultat.Celija(r, ime);
                PrimijeniNaKontrolu(c, r, v, sirovo);
                if (IsDisposed)
                    return;
            }
        }

        private void PrimijeniNaKontrolu(Control c, DataRow r, string v, object sirovo)
        {
            var slika = c as PictureEdit;
            if (slika != null)
            {
                // 4766-4795: slika iz baze (byte[]).
                byte[] bajtovi = sirovo as byte[];
                if (bajtovi != null)
                {
                    try
                    {
                        using (var ms = new System.IO.MemoryStream(bajtovi))
                            slika.Image = System.Drawing.Image.FromStream(ms);
                    }
                    catch (Exception ex) { NgDnevnik.Greska("slika " + c.Name + ": " + ex.Message); }
                }
                // 4785-4793: stupac "<ime>location" je put ili URL slike (PIC-01).
                string lok = Form2Rezultat.Celija(r, c.Name.Trim() + "location");
                if (lok.Trim().Length > 0)
                    UcitajSliku(slika, lok);
                return;
            }
            DodatnoPostavi(c, v);
            string mala = (v ?? "").Trim().ToLowerInvariant();
            var grid = c as GridControl;
            if (grid != null)
            {
                if (Form2Rezultat.GridOsvjezi(v))
                {
                    NgControl def = NadjiKontrolu(c.Name);
                    if (def != null)
                        OsvjeziGrid(c, def, v);
                }
                else
                {
                    var view = grid.MainView as GridView;
                    if (view != null)
                        PomakniGrid(view, v);
                }
                return;
            }
            if (c.GetType() == typeof(ButtonEdit))
            {
                // Trazilica 4942-4968: Tag je id, tekst iz stupca CAPTION.
                if (Form2Rezultat.TrazilicaDobivaId(v))
                    c.Tag = v;
                if ((c.Tag as string) == "0")
                {
                    if ((c.Text ?? "").Trim() != "*")
                        c.Text = "";
                }
                else if (!Form2Rezultat.JeStanje(v))
                {
                    NgControl def = NadjiKontrolu(c.Name);
                    string stupac = Form2Rezultat.TrazilicaStupac(Attr(def, "CAPTION"));
                    if (Form2Rezultat.Ima(r, stupac))
                        c.Text = Form2Rezultat.Celija(r, stupac).Trim();
                }
                return;
            }
            var look = c as LookUpEdit;
            if (look != null)
            {
                if (Form2Rezultat.ComboOsvjezi(v))
                {
                    NgControl def = NadjiKontrolu(c.Name);
                    if (def != null)
                        NapuniKontrolu(c, def, SqlDijaloga(def));
                }
                else if (Form2Rezultat.ComboDobivaVrijednost(v))
                    OdaberiUComboju(look, v);
                return;
            }
            var vise = c as CheckedComboBoxEdit;
            if (vise != null)
            {
                // RefreshControls 4806-4830: "refresh" ponovno puni (strMultiDropSQL s ""), inace Tag su
                // oznaceni id-evi (Timer3: id~id).
                if (v.IndexOf("refresh", StringComparison.Ordinal) >= 0)
                {
                    NgControl def = NadjiKontrolu(c.Name);
                    if (def != null)
                        NapuniKontrolu(c, def, SrediPoslije.Zamijeni(Attr(def, "SQL"), "", _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false));
                    vise.SetEditValue(null);
                }
                else if (v.Trim().Length > 0 && !Form2Rezultat.JeStanje(v))
                    OznaciVise(vise, v.Trim());
                return;
            }
            var datum = c as DateEdit;
            if (datum != null)
            {
                if (mala == "checked")
                {
                    if (!(datum.EditValue is DateTime))
                        datum.EditValue = DateTime.Today;
                }
                else if (mala == "unchecked")
                    datum.EditValue = null;
                else if (Form2Rezultat.Akcija(v) == "ostalo")
                {
                    DateTime d;
                    if (sirovo is DateTime)
                        datum.EditValue = (DateTime)sirovo;
                    else if (ProcitajDatum(v, out d))
                        datum.EditValue = d;
                }
                return;
            }
            var spin = c as SpinEdit;
            if (spin != null)
            {
                if (!Form2Rezultat.BrojDobivaVrijednost(v))
                    return;
                decimal broj;
                if (decimal.TryParse(v.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out broj)
                    || decimal.TryParse(v.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out broj))
                    spin.Value = broj;
                return;
            }
            var kvacica = c as CheckEdit;
            if (kvacica != null)
            {
                if (kvacica.Properties.CheckStyle == CheckStyles.Radio)
                {
                    if (Form2Rezultat.RadioUkljuci(v))
                        kvacica.Checked = true;
                }
                else
                {
                    bool? b = Form2Rezultat.VbBool(v);
                    if (b.HasValue)
                        kvacica.Checked = b.Value;
                }
                return;
            }
            var gumb = c as SimpleButton;
            if (gumb != null)
            {
                if (!Form2Rezultat.JeStanje(v) && v.Trim().Length > 0)
                    gumb.Text = v;
                return;
            }
            var edit = c as TextEdit;
            if (edit != null)
            {
                if (!Form2Rezultat.JeStanje(v))
                    edit.Text = v;
                return;
            }
        }

        /// <summary>
        /// DodatnoPostimajKontrolu (4497-4552). visible/invisible vrijede i za natpis "lbl"+ime.
        /// Grid: enable/disable je uredjivanje, ostalo trazi red po prvom stupcu (Janus stupac 2).
        /// </summary>
        private void DodatnoPostavi(Control c, string v)
        {
            string akcija = Form2Rezultat.Akcija(v);
            if (akcija.Length == 0)
                return;
            var grid = c as GridControl;
            if (akcija == "visible" || akcija == "invisible")
            {
                bool vidi = akcija == "visible";
                c.Visible = vidi;
                Control[] natpis = Controls.Find("lbl" + c.Name, true);
                if (natpis.Length > 0)
                    natpis[0].Visible = vidi;
                return;
            }
            if (akcija == "tabstoped" || akcija == "tabnotstoped")
            {
                c.TabStop = akcija == "tabstoped";
                return;
            }
            if (akcija == "enable" || akcija == "disable")
            {
                bool da = akcija == "enable";
                var view = grid == null ? null : grid.MainView as GridView;
                if (view != null)
                    view.OptionsBehavior.Editable = da;
                else
                    c.Enabled = da;
                return;
            }
            if (akcija == "refresh")
            {
                var web = c as WebBrowser;
                if (web != null)
                    web.Refresh();
                return;
            }
            // Case Else
            if (grid != null)
            {
                var view = grid.MainView as GridView;
                if (view != null)
                    PomakniNaKljuc(view, v.Trim());
                return;
            }
            var preglednik = c as WebBrowser;
            if (preglednik != null)
            {
                try { preglednik.Navigate(v); }
                catch (Exception ex) { NgDnevnik.Greska("navigate " + v + ": " + ex.Message); }
                return;
            }
            var label = c as LabelControl;
            if (label != null)
                label.Text = label.Dock == DockStyle.Bottom ? Ponasanje.StatusTekst(v) : v;
        }

        private static bool ProcitajDatum(string v, out DateTime d)
        {
            string t = (v ?? "").Trim();
            string[] oblici = { "dd.MM.yyyy", "d.M.yyyy", "dd.MM.yyyy.", "d.M.yyyy.", "yyyyMMdd", "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss", "d.M.yyyy. H:mm:ss" };
            if (DateTime.TryParseExact(t, oblici, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return true;
            return DateTime.TryParse(t, CultureInfo.CurrentCulture, DateTimeStyles.None, out d);
        }

        /// <summary>
        /// Combo 5137-5146: SelectedValue = vrijednost; ako je nema, prvi red (SelectedIndex = 0).
        /// DataView.Find pretvori tekst u tip stupca, pa se ovdje usporedjuje tekst.
        /// </summary>
        private static void OdaberiUComboju(LookUpEdit look, string v)
        {
            var dt = look.Properties.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
                return;
            string trazeno = (v ?? "").Trim();
            string stupac = string.IsNullOrEmpty(look.Properties.ValueMember) ? dt.Columns[0].ColumnName : look.Properties.ValueMember;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object x = dt.Rows[i][stupac];
                if (x != null && x != DBNull.Value && string.Equals(Convert.ToString(x, CultureInfo.InvariantCulture).Trim(), trazeno, StringComparison.OrdinalIgnoreCase))
                {
                    look.EditValue = x;
                    return;
                }
            }
            look.EditValue = dt.Rows[0][stupac];
        }

        private void Fokusiraj(string ime)
        {
            Control[] n = Controls.Find((ime ?? "").Trim(), true);
            if (n.Length == 0)
                return;
            Control cilj = n[0];
            // 4627-4631: Select() iz dogadjaja. Fokus u Leave/Validated WinForms odbije, pa ide poslije.
            if (IsHandleCreated)
                BeginInvoke(new Action(delegate ()
                {
                    if (!cilj.IsDisposed && cilj.CanFocus)
                        cilj.Focus();
                }));
            else
                cilj.Select();
            ZadnjiFokus = cilj.Name;
        }

        /// <summary>click (4673-4677): Glbl_Click. Gumb pokrene svoj ONCLICK, ostalo ONCLICK ili TAG.</summary>
        private void KlikniKontrolu(string ime)
        {
            Control[] n = Controls.Find(ime, true);
            if (n.Length == 0)
                return;
            var gumb = n[0] as SimpleButton;
            if (gumb != null)
            {
                gumb.PerformClick();
                return;
            }
            NgControl def = NadjiKontrolu(ime);
            string tekst = Attr(def, "ONCLICK");
            if (tekst.Length == 0)
                tekst = Attr(def, "TAG");
            IzvrsiDogadjaj(tekst);
        }

        private void PostaviId(string id)
        {
            Id = id.Trim();
            if (_kontekst != null)
                _kontekst.Id = Id;
        }

        public string ZadnjiFokus = "";

        private NgControl NadjiKontrolu(string ime)
        {
            if (_forma == null || _forma.Kontrole == null)
                return null;
            for (int i = 0; i < _forma.Kontrole.Count; i++)
            {
                if (string.Equals(_forma.Kontrole[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return _forma.Kontrole[i];
            }
            return null;
        }

        private readonly System.Collections.Generic.Dictionary<Control, string> _stariTekst = new System.Collections.Generic.Dictionary<Control, string>();
        private readonly Form2Dogadjaj.IzlazTrazilice _izlazTrazilice = new Form2Dogadjaj.IzlazTrazilice();
        public string ZadnjaTrazilica = "";
        public string ZadnjiIzbornik = "";

        private static string Attr(NgControl k, string ime)
        {
            if (k == null || k.Atributi == null || !k.Atributi.Ima(ime))
                return "";
            return (k.Atributi.Vrijednost(ime) ?? "").Trim();
        }

        private void VeziDogadjaj(Control c, NgControl k)
        {
            string lost = Attr(k, "ONLOSTFOCUS");
            string promjena = Attr(k, "ONCHANGE");
            string vrijednost = Attr(k, "ONVALUECHANGED");
            string klik = Attr(k, "ONCLICK");
            if (k.Grana == NgGrana.Search)
            {
                // Form2.Glbl_Gotfocus (5990): OldSearchString je tekst pri ulasku.
                c.Enter += delegate { _stariTekst[c] = c.Text ?? ""; };
                // Gumb tražilice (DevExpress ButtonEdit) radi isto što i Enter (Glbl_Validated).
                var gumbTrazi = c as ButtonEdit;
                if (gumbTrazi != null)
                {
                    gumbTrazi.ButtonClick += delegate
                    {
                        if (_slazem)
                            return;
                        _izlazTrazilice.Validated();
                        ZadnjaTrazilica = c.Text ?? "";
                        PotvrdiTrazilicu(c, k, lost);
                    };
                }
                string onLeave = Attr(k, "ONLEAVE");
                c.Leave += delegate
                {
                    // Form2.Glbl_Leave (9432-9451). Validated2 je u VB-u iskljucen (2210), pa izlaz
                    // Tabom s upisanim tekstom ne trazi i ne salje ONLOSTFOCUS. Prazan tekst da.
                    var zuta = c as TextEdit;
                    if (zuta != null)
                        zuta.BackColor = System.Drawing.Color.LightYellow;
                    if (_slazem)
                        return;
                    if (onLeave.Length > 0)
                    {
                        OsvjeziNakonDjeteta();
                        IzvrsiDogadjaj(onLeave);
                    }
                    if ((c.Text ?? "").Trim().Length == 0)
                    {
                        c.Tag = "0";
                        if (_izlazTrazilice.Validated2Smije())
                        {
                            _izlazTrazilice.Validated();
                            OsvjeziNakonDjeteta();
                            IzvrsiDogadjaj(lost);
                        }
                    }
                };
            }
            else if (lost.Length > 0)
                c.Leave += delegate { IzvrsiDogadjaj(lost); };
            if (k.Grana == NgGrana.MultiDrop)
            {
                string err = Form2Dogadjaj.MultiDropNaredba(Attr(k, "ERRORTEXT"));
                if (err.Length > 0)
                    c.Leave += delegate { IzvrsiDogadjaj(err); };
            }
            if (k.Grana == NgGrana.Label || k.Grana == NgGrana.Check || k.Grana == NgGrana.Radio)
            {
                string tag = Attr(k, "ONCLICK");
                if (tag.Length == 0)
                    tag = Attr(k, "TAG");
                if (tag.Length > 0)
                    c.Click += delegate { IzvrsiDogadjaj(Form2Dogadjaj.KlikLabele(tag)); };
            }
            if (k.Grana == NgGrana.Combo && klik.Length > 0)
            {
                var look = c as LookUpEdit;
                if (look != null)
                    look.EditValueChanged += delegate { IzvrsiDogadjaj(klik); };
            }
            if (k.Grana == NgGrana.Date && vrijednost.Length > 0)
            {
                var datum = c as DateEdit;
                if (datum != null)
                    datum.EditValueChanged += delegate { IzvrsiDogadjaj(vrijednost); };
            }
            if (promjena.Length > 0)
            {
                var tekst = c as TextEdit;
                if (tekst != null && !(tekst is LookUpEdit) && !(tekst is DateEdit))
                    tekst.TextChanged += delegate { IzvrsiDogadjaj(promjena); };
            }
            // Form2.vb 2051: atribut je ONGOTFOCUS (Glbl_Gotfocus2 -> Rafalno(strGotFocus), 9474).
            string got = Attr(k, "ONGOTFOCUS");
            if (got.Length == 0)
                got = Attr(k, "GOTFOCUS");
            if (got.Length > 0)
                c.Enter += delegate { IzvrsiDogadjaj(got); };
            var tekstFokus = c as TextEdit;
            if (tekstFokus != null && !(tekstFokus is LookUpEdit))
            {
                c.Enter += delegate
                {
                    tekstFokus.BackColor = System.Drawing.Color.LightGreen;
                    tekstFokus.SelectAll();
                };
            }
            string dbl = Attr(k, "ONDBLCLICK");
            string popup = Attr(k, "POPUP");
            if (k.Grana == NgGrana.Grid || k.Grana == NgGrana.DxGrid)
            {
                var gc = c as GridControl;
                var view = gc == null ? null : gc.MainView as GridView;
                if (view != null && dbl.Length > 0)
                {
                    view.DoubleClick += delegate
                    {
                        object celija = null;
                        if (view.FocusedColumn != null && view.FocusedRowHandle >= 0)
                            celija = view.GetFocusedRowCellValue(view.FocusedColumn);
                        if (_kontekst != null)
                        {
                            // Form2.Glbl_DoubleClick 6188-6201: DblClickedColumn (natpis) i TrenutnaVrijednost.
                            _kontekst.DblClickedColumn = view.FocusedColumn == null ? "" : view.FocusedColumn.GetCaption();
                            _kontekst.LastValue = Form2Dogadjaj.VrijednostDvoklika(celija == null ? "" : Convert.ToString(celija));
                        }
                        IzvrsiDogadjaj(dbl);
                    };
                }
                if (view != null && klik.Length > 0)
                {
                    // Form2.DataGridView1_SelectionChanged (5623-5640) i Glbl_Click (6107-6170):
                    // ONCLICK grida ide kad se promijeni red; klik koji je tu promjenu napravio se preskoce
                    // (ZanemariClick). Klik na isti red ide opet.
                    bool zanemariKlik = false;
                    view.FocusedRowChanged += delegate
                    {
                        if (_slazem || _tihoGrid > 0)
                            return;
                        zanemariKlik = true;
                        IzvrsiDogadjaj(klik);
                    };
                    view.Click += delegate
                    {
                        if (_slazem)
                            return;
                        if (zanemariKlik)
                        {
                            zanemariKlik = false;
                            return;
                        }
                        IzvrsiDogadjaj(klik);
                    };
                }
                string prijeSql = Attr(k, "BEFORECELLEDITED");
                if (view != null && prijeSql.Length > 0)
                {
                    view.ShowingEditor += delegate (object s, System.ComponentModel.CancelEventArgs e)
                    {
                        if (_slazem)
                            return;
                        // Form2.Glbl_EditingCell 8532-8614: TrenutnaKolona/TrenutnaVrijednost, stupci reda kao
                        // parametri, red oznacen, Rafalno; greska prekine uredjivanje (e.Cancel).
                        PostaviCeliju(view, view.FocusedColumn, view.FocusedColumn == null ? null : view.GetFocusedRowCellValue(view.FocusedColumn));
                        if (view.FocusedRowHandle >= 0)
                            view.SelectRow(view.FocusedRowHandle);
                        string redParam = Form2Dogadjaj.ParametriCelije(view.GetFocusedDataRow());
                        string sredjen = SrediPoslije.Zamijeni(prijeSql, redParam, _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false);
                        if (Form2Dogadjaj.VratiStaru(IzvrsiDogadjaj(sredjen)))
                            e.Cancel = true;
                    };
                }
                string celijaSql = Attr(k, "ONCELLEDITED");
                if (view != null && celijaSql.Length > 0)
                {
                    string stara = "";
                    bool vracam = false;
                    view.ShowingEditor += delegate
                    {
                        object v = view.FocusedColumn == null ? null : view.GetFocusedRowCellValue(view.FocusedColumn);
                        stara = Form2Dogadjaj.VrijednostDvoklika(v == null ? "" : Convert.ToString(v));
                    };
                    view.CellValueChanged += delegate (object s, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
                    {
                        if (vracam || _slazem)
                            return;
                        // 8616-8640: @@EDITEDCOLUMN je natpis uredjenog stupca, @@LASTVALUE nova vrijednost
                        // (Narudzbe g_ugo_stavke: @kolona='@@editedcolumn', @vrijednost='@@lastvalue').
                        PostaviCeliju(view, e.Column, e.Value);
                        // Form2.Glbl_CellEdited (8616-8688): stupci reda postanu parametri
                        // (ParametriSaGrida), SQL se sredi s njima, pa ide kroz Rafalno.
                        // Greska vrati staru vrijednost celije (CEL-01).
                        // 8624: uredjen red postane oznacen (CheckState = Checked), pa ga FOREACHSELECTED vidi.
                        if (view.FocusedRowHandle >= 0)
                            view.SelectRow(view.FocusedRowHandle);
                        DataRow red = view.GetFocusedDataRow();
                        string redParam = Form2Dogadjaj.ParametriCelije(red);
                        string sredjen = SrediPoslije.Zamijeni(celijaSql, redParam, _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false);
                        string greska = IzvrsiDogadjaj(sredjen);
                        if (!Form2Dogadjaj.VratiStaru(greska) || view.FocusedColumn == null)
                            return;
                        vracam = true;
                        try { view.SetFocusedRowCellValue(view.FocusedColumn, stara); }
                        finally { vracam = false; }
                    };
                }
                // Form2.Glbl_GridMouseUp: desni klik na svaki grid dijaloga (POPUP i stalne stavke) (POP-01).
                if (view != null)
                    GridIzbornik.Vezi(view, delegate { return popup; }, IzvrsiIzbornik, null, delegate { IspisiGrid("PRINTGRID(" + c.Name + ")", gc); });
                if (view != null && popup.Length > 0)
                {
                    view.MouseUp += delegate (object s, MouseEventArgs e)
                    {
                        if (Form2Dogadjaj.DesniKlik(e.Button == MouseButtons.Right, (Control.ModifierKeys & Keys.Control) == Keys.Control) != "izbornik")
                            return;
                        var stavke = Form2Dogadjaj.Izbornik(popup);
                        ZadnjiIzbornik = string.Join(";", stavke.ToArray());
                    };
                }
            }
        }

        /// <summary>SrediSQL2 2882-2883: TrenutnaKolona.Caption i TrenutnaVrijednost (Val &lt;&gt; 0 → točka).</summary>
        private void PostaviCeliju(GridView view, DevExpress.XtraGrid.Columns.GridColumn stupac, object vrijednost)
        {
            if (_kontekst == null || view == null)
                return;
            _kontekst.EditedColumn = stupac == null ? "" : stupac.GetCaption();
            _kontekst.LastValue = Form1Rafal.ZadnjaVrijednost(vrijednost);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            _izlazTrazilice.KeyDownPocinje();
            int kod = (int)(keyData & Keys.KeyCode);
            bool control = (keyData & Keys.Control) == Keys.Control;
            // DevExpress editor u fokusu: ActiveControl forme je njegov unutarnji MaskBox,
            // ne ButtonEdit. Bez penjanja po roditeljima Enter u trazilici nije bio Enter u
            // trazilici i ONLOSTFOCUS je otisao dvaput (Form2.vb KeyDown trazilice).
            // LookUpEdit, DateEdit i SpinEdit su takodjer ButtonEdit, pa je trazilica tocno taj tip
            // (Napravi: NgGrana.Search -> new ButtonEdit()).
            Control aktivna = ActiveControl;
            while (aktivna != null && aktivna != this && !(aktivna is DevExpress.XtraEditors.BaseEdit) && !(aktivna is GridControl))
                aktivna = aktivna.Parent;
            bool trazilica = aktivna != null && aktivna.GetType() == typeof(ButtonEdit);
            bool grid = aktivna is GridControl;
            // Form2_KeyDown 4158-4166 (KeyPreview): Escape ide u ONESCKEY kroz Rafalno, a bez njega zatvori.
            // Otvoren popis comboa ili datuma prvo zatvori sam editor.
            if (kod == (int)Keys.Escape && !control)
            {
                var popup = aktivna as DevExpress.XtraEditors.PopupBaseEdit;
                if (popup == null || !popup.IsPopupOpen)
                {
                    string esc = AtributForme("ONESCKEY");
                    if (esc.Length > 0)
                        IzvrsiDogadjaj(esc);
                    else
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }
                    return true;
                }
            }
            // Form2_KeyDown 4192-4252: F1-F10 i * su HOTKEY (HOT-01).
            if (Form2Izgled.JeHotkey(keyData & Keys.KeyCode) && PokreniHotkey(keyData & Keys.KeyCode))
                return true;
            NgControl def = aktivna == null || aktivna == this ? null : NadjiKontrolu(aktivna.Name);
            string lost = Attr(def, "ONLOSTFOCUS");
            string greska = "";
            if (trazilica && kod == 13)
            {
                // Form2.Glbl_KeyDown 6022-6036: Enter u trazilici je Glbl_Validated (trazi, RefreshKontrola,
                // ONLOSTFOCUS), pa SelectNextControl ako greska ni ONLOSTFOCUS ne traze focus (EVT-02).
                _izlazTrazilice.Validated();
                ZadnjaTrazilica = aktivna.Text ?? "";
                greska = PotvrdiTrazilicu(aktivna, def, lost);
                if (IsDisposed)
                    return true;
            }
            bool viselinijski = aktivna is MemoEdit;
            bool enterKaoTab = string.Equals(Attr(def, "ENTERASTAB"), "true", StringComparison.OrdinalIgnoreCase);
            // 6040-6044: grid bez EDITCOLUMNS ide na sljedecu kontrolu; ONENTERKEY grida ide poslije (6071-6076).
            bool editPrazan = Attr(def, "EDITCOLUMNS").Length == 0;
            string onEnter = grid ? Attr(def, "ONENTERKEY") : "";
            Form2Dogadjaj.Tipka t = Form2Dogadjaj.KeyDown(trazilica, grid, viselinijski, enterKaoTab, editPrazan, kod, control, onEnter, lost, greska);
            bool obradjeno = false;
            if (t.Sljedeca && ActiveControl != null)
            {
                SelectNextControl(trazilica || grid ? aktivna : ActiveControl, true, true, true, true);
                obradjeno = true;
            }
            if (t.EnterNaredba.Length > 0)
            {
                IzvrsiDogadjaj(t.EnterNaredba);
                obradjeno = true;
            }
            if (trazilica && kod == 13)
                obradjeno = true;
            if (obradjeno)
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// HOTKEY (139 granula, gumbi F2/F8/F3, trazilica F5): gumb se klikne ako je ukljucen
        /// (Glbl_ClickButton), combo, broj, trazilica i grid dobiju fokus. Ostale kontrole nista.
        /// </summary>
        private bool PokreniHotkey(Keys tipka)
        {
            bool pogodak = false;
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name))
                    continue;
                NgControl def = NadjiKontrolu(c.Name);
                if (def == null || !Form2Izgled.Pogodak(Attr(def, "HOTKEY"), tipka))
                    continue;
                var gumb = c as SimpleButton;
                if (gumb != null)
                {
                    if (gumb.Enabled)
                    {
                        pogodak = true;
                        gumb.PerformClick();
                        if (IsDisposed)
                            return true;
                    }
                    continue;
                }
                if (c is LookUpEdit || c is SpinEdit || c.GetType() == typeof(ButtonEdit) || c is GridControl)
                {
                    pogodak = true;
                    c.Focus();
                }
            }
            return pogodak;
        }

        private void PosaljiNaServer(string segment)
        {
            if (_citaj == null)
                return;
            DataTable inst = null;
            DataTable kasa = null;
            try
            {
                inst = _citaj("select * from install");
                kasa = _citaj("select * from mpkase");
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message);
                return;
            }
            if (inst == null || inst.Rows.Count == 0 || kasa == null || kasa.Rows.Count == 0)
                return;
            string servis = inst.Columns.Contains("webservice") && inst.Rows[0]["webservice"] != DBNull.Value
                ? Convert.ToString(inst.Rows[0]["webservice"]) : "";
            if (servis.Trim().Length == 0)
                return;
            string trgovina = kasa.Columns.Contains("oznakatrgovine") ? Convert.ToString(kasa.Rows[0]["oznakatrgovine"]) : "";
            string kasaIme = kasa.Columns.Contains("kasa") ? Convert.ToString(kasa.Rows[0]["kasa"]) : "";
            string url = PosaljiServer.Vrsta(segment) == "poruka"
                ? PosaljiServer.UrlPoruke(servis, trgovina, kasaIme, "", "", "")
                : PosaljiServer.UrlRacuna(servis, trgovina, kasaIme, "", "", "", "", "");
            try
            {
                PosaljiServer.Post(url, "<poruka/>");
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + url);
            }
        }

        /// <summary>
        /// FOREACHROW / FOREACHSELECTED / FOREACHBLUEROW (Form2.vb 7040-7048, 8309-8400): za svaki red grida
        /// pomakni grid na taj red i izvrsi sljedeci segment kroz Rafalno. Tijelo vidi red kao @@grid (id reda),
        /// @(grid..stupac) i ostale parametre dijaloga; moze biti i naredba (:FORMA). Greska jednog reda ne
        /// prekida ostale (Rafalno po retku). FOREACHROW ne okida ONCLICK grida (UsaouforeachROW) (FOR-01).
        /// </summary>
        private void IzvrsiForeach(string opis)
        {
            string opisTekst = opis ?? "";
            int crta1 = opisTekst.IndexOf('|');
            int crta2 = crta1 < 0 ? -1 : opisTekst.IndexOf('|', crta1 + 1);
            if (crta1 < 0 || crta2 < 0)
                return;
            string vrsta = opisTekst.Substring(0, crta1);
            string gridIme = opisTekst.Substring(crta1 + 1, crta2 - crta1 - 1);
            string tijelo = opisTekst.Substring(crta2 + 1);
            GridControl grid = null;
            foreach (Control c in SveKontrole())
            {
                var g = c as GridControl;
                if (g != null && string.Equals(g.Name, gridIme, StringComparison.OrdinalIgnoreCase))
                {
                    grid = g;
                    break;
                }
            }
            var pogled = grid == null ? null : grid.MainView as GridView;
            if (pogled == null || tijelo.Trim().Length == 0)
                return;
            var redovi = new List<DataRow>();
            var plavi = new HashSet<int>(pogled.GetSelectedRows());
            DevExpress.XtraGrid.Columns.GridColumn kvacica = null;
            for (int c = 0; c < pogled.Columns.Count; c++)
            {
                if (pogled.Columns[c].ColumnType == typeof(bool))
                {
                    kvacica = pogled.Columns[c];
                    break;
                }
            }
            for (int r = 0; r < pogled.DataRowCount; r++)
            {
                int h = pogled.GetRowHandle(r);
                bool uzmi;
                if (vrsta == "svi")
                    uzmi = true;
                else if (vrsta == "plavi")
                    uzmi = plavi.Contains(h);
                else
                {
                    // GetCheckedRows: kvacica selektora (u Carpu oznaceni red) ili uredjen red (8624).
                    object k = kvacica == null ? null : pogled.GetRowCellValue(h, kvacica);
                    uzmi = plavi.Contains(h) || (k is bool && (bool)k);
                }
                DataRow red = pogled.GetDataRow(h);
                if (uzmi && red != null)
                    redovi.Add(red);
            }
            var dt = grid.DataSource as DataTable;
            for (int i = 0; i < redovi.Count; i++)
            {
                if (IsDisposed)
                    return;
                DataRow red = redovi[i];
                if (red.RowState == DataRowState.Detached || dt == null)
                    continue;
                int h = pogled.GetRowHandle(dt.Rows.IndexOf(red));
                if (h < 0)
                    continue;
                if (vrsta == "svi")
                    _tihoGrid++;
                try { pogled.FocusedRowHandle = h; }
                finally
                {
                    if (vrsta == "svi")
                        _tihoGrid--;
                }
                IzvrsiDogadjaj(tijelo);
            }
        }

        private string _parametriIzTrazilice = "";

        /// <summary>
        /// Form2.PokupiParametre (3406-3983): strParametriSvi, a svaki parametar ide i u CB, uz
        /// @searchboxIME i @comboboxIME (tekst kontrole). Tako @@comboboxIME i vrijednosti dijaloga
        /// vide i SQL-ovi koji ih nemaju u nizu (CB je zajednicki s podlogom).
        /// </summary>
        private string PokupiParametre()
        {
            string svi = SviParametri();
            var cb = _kontekst == null ? null : _kontekst.Clipboard;
            if (cb != null)
            {
                var natpisi = new StringBuilder();
                foreach (Control c in SveKontrole())
                {
                    var look = c as LookUpEdit;
                    if (look != null && !string.IsNullOrEmpty(c.Name))
                        natpisi.Append("@combobox").Append(c.Name.Trim()).Append("='").Append((look.Text ?? "").Replace(",", " ")).Append("',");
                }
                ClipboardNg.UpisiParametre(cb, natpisi.ToString() + svi);
            }
            // 3429-3662: PokupiParametre sprema vrijednosti u NetGenDBClipboard (DBC-01).
            if (_dbCb && !_slazem && !_zatvaram)
                SpremiDbClipboard();
            return svi;
        }

        /// <summary>
        /// Rafalno 6812-6818: @@SEARCH tokeni iz strParametriIzTrazilice (zadnji odabrani red trazilice).
        /// @@searchboxIME nije to (tekst trazilice iz PokupiParametre/CB), pa ostaje za kasnije.
        /// </summary>
        private string ZamijeniSearch(string tekst)
        {
            string s = tekst ?? "";
            if (s.IndexOf("@@SEARCH", StringComparison.OrdinalIgnoreCase) < 0)
                return s;
            return System.Text.RegularExpressions.Regex.Replace(s, @"@@SEARCH(?!BOX)[^\s'%#,)*/\\+\-.(=]*", delegate (System.Text.RegularExpressions.Match m)
            {
                return SrediPoslije.Nadji(m.Value, _parametriIzTrazilice, null, false);
            }, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Form2.Glbl_Validated (5819-5896): prazan tekst je id 0; isti tekst (osim *) ne trazi;
        /// inace trazi (Form3). Pa RefreshKontrola i ONLOSTFOCUS kroz Rafalno. Vraca gresku lanca.
        /// </summary>
        private string PotvrdiTrazilicu(Control c, NgControl def, string lost)
        {
            string stari;
            _stariTekst.TryGetValue(c, out stari);
            string tekst = c.Text ?? "";
            if (tekst.Trim().Length == 0)
            {
                c.Tag = "0";
                OsvjeziNakonDjeteta();
                return IzvrsiDogadjaj(lost);
            }
            if (Form2Dogadjaj.TrazilicaKorak(tekst, stari) == "trazilica")
                PretraziTrazilicu(c, def);
            OsvjeziNakonDjeteta();
            return IzvrsiDogadjaj(lost);
        }

        /// <summary>
        /// Form3.Trazilica (71-332) i Glbl_Validated 5848-5890. SQL je atribut SQL trazilice; sp/select ide
        /// kakav jest (tekst je u @@searchboxIME), a tablica#polje#... postane select ... like.
        /// Nista: id 0 i prazan tekst. Jedan red: odabran bez prozora. Vise: prozor s gridom.
        /// Odustajanje takodjer brise (TrazilicaID je vec "0").
        /// </summary>
        private void PretraziTrazilicu(Control c, NgControl def)
        {
            _parametriIzTrazilice = "";
            string id = "0";
            string naziv = "";
            DataRow red = null;
            string atribut = Attr(def, "SQL");
            if (atribut.Length > 0 && _citaj != null)
            {
                string parametri = SrediPoslije.Zamijeni(atribut, PokupiParametre(), _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, false).Trim();
                string sql = Ponasanje.TrazilicaSql(parametri, c.Text);
                DataTable dt = null;
                if (sql.Length > 0)
                {
                    NgDnevnik.Sql(sql);
                    try { dt = _citaj(sql); }
                    catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sql); dt = null; }
                }
                ZadnjiTraziSql = sql;
                if (dt != null && dt.Rows.Count == 1)
                    red = dt.Rows[0];
                else if (dt != null && dt.Rows.Count > 1)
                    red = TrazilicaProzor.Odaberi(this, dt, c.Text ?? "", Attr(def, "COLUMNCAPTION"), Attr(def, "COLUMNWIDTH"), Attr(def, "COLUMNAUTORESIZE"));
            }
            if (red != null)
            {
                id = TrazilicaTok.Id(red);
                naziv = TrazilicaTok.Naziv(red);
                _parametriIzTrazilice = ParametriSvi.IzTrazilice(red);
            }
            c.Tag = id;
            c.Text = naziv;
        }

        public string ZadnjiTraziSql = "";

        /// <summary>
        /// Kontrole dijaloga, i one u okviru (PARENT, LAY-01). VB ide samo po Me.Controls, pa kontrola u
        /// GroupBoxu nije parametar ni cilj RefreshControls/HOTKEY. U korpusu su u okvirima uglavnom gumbi
        /// (424 od 500); za tekst, broj i radio u okviru Carpo ih ipak vidi (50-50 -> Carpo).
        /// </summary>
        private List<Control> SveKontrole()
        {
            var l = new List<Control>();
            Skupi(this, l);
            return l;
        }

        /// <summary>PokupiParametre 3443-3447: GetCheckedItems, zarez postane ~, bez razmaka.</summary>
        private static string Oznaceni(CheckedComboBoxEdit c)
        {
            // EditValue je popis oznacenih vrijednosti odvojen zarezom (i prije nego se popis otvori).
            object o = c.EditValue;
            if (o == null || o == DBNull.Value)
                return "";
            return (Convert.ToString(o, CultureInfo.InvariantCulture) ?? "").Replace(",", "~").Replace(" ", "");
        }

        /// <summary>Timer3 (9711-9729): oznaci stavke ciji je id u "id~id".</summary>
        private static void OznaciVise(CheckedComboBoxEdit c, string idjevi)
        {
            string niz = (idjevi ?? "").Replace("~", ",").Replace(" ", "");
            c.SetEditValue(niz.Length == 0 ? null : niz);
        }

        private static void Skupi(Control roditelj, List<Control> l)
        {
            foreach (Control c in roditelj.Controls)
            {
                l.Add(c);
                if (c is PanelControl || c is SplitContainerControl)
                    Skupi(c, l);
            }
        }

        private string SviParametri()
        {
            var sb = new StringBuilder();
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name))
                    continue;
                var chk = c as CheckEdit;
                if (chk != null && chk.Properties.CheckStyle == CheckStyles.Radio)
                {
                    sb.Append(ParametriSvi.Radio(c.Name, chk.Checked));
                    continue;
                }
                if (chk != null)
                {
                    sb.Append("@").Append(c.Name.Trim()).Append("=").Append((int)chk.CheckState).Append(",");
                    continue;
                }
                var grid = c as GridControl;
                if (grid != null)
                {
                    sb.Append(GridParametar(grid));
                    continue;
                }
                var look = c as LookUpEdit;
                if (look != null)
                {
                    object v = look.EditValue;
                    string t = v == null || v == DBNull.Value || Convert.ToString(v).Length == 0 ? "null" : Convert.ToString(v);
                    sb.Append("@").Append(c.Name.Trim()).Append("=").Append(t).Append(",");
                    continue;
                }
                if (c.GetType() == typeof(ButtonEdit))
                {
                    var trazi = (ButtonEdit)c;
                    sb.Append(ParametriSvi.Search(c.Name, c.Tag as string, trazi.Text));
                    continue;
                }
                var vise = c as CheckedComboBoxEdit;
                if (vise != null)
                {
                    sb.Append(ParametriSvi.Multi(c.Name, Oznaceni(vise)));
                    continue;
                }
                var txt = c as TextEdit;
                if (txt != null)
                {
                    var spin = txt as SpinEdit;
                    if (spin != null)
                    {
                        // Form2.vb 3601: IIf(Text = "", 0, Value.ToString.Replace(",", ".")).
                        // Salje se vrijednost, ne prikazani tekst: SpinEdit u hr-HR prikaze
                        // 1234,5 kao "1.234,5", a tekst bi u SQL otisao kao 1.234.5 (NUM-01).
                        string broj = string.IsNullOrWhiteSpace(txt.Text)
                            ? ""
                            : Convert.ToString(spin.Value, CultureInfo.InvariantCulture);
                        sb.Append(ParametriSvi.Broj(c.Name, broj));
                        continue;
                    }
                    var datum = txt as DateEdit;
                    if (datum != null)
                    {
                        DateTime d;
                        bool ima = datum.EditValue is DateTime;
                        if (ima)
                            d = (DateTime)datum.EditValue;
                        else
                            d = DateTime.MinValue;
                        // 3522-3549: CUSTOMFORMAT odredi oblik (HH:mm, yyyyMM01, yyyyMMdd HH:mm).
                        NgControl defDatuma = NadjiKontrolu(c.Name);
                        sb.Append(ParametriSvi.Datum(c.Name, Attr(defDatuma, "CUSTOMFORMAT"), ima, ima ? (DateTime?)d : null));
                        continue;
                    }
                    sb.Append(ParametriSvi.Tekst(c.Name, txt.Text));
                }
            }
            return sb.ToString();
        }

        private static string GridParametar(GridControl grid)
        {
            var pogled = grid.MainView as GridView;
            int redova = pogled == null ? 0 : pogled.RowCount;
            string id = "0";
            var plavi = new StringBuilder();
            if (pogled != null && redova > 0)
            {
                id = Celija(pogled, pogled.FocusedRowHandle);
                int[] sel = pogled.GetSelectedRows();
                for (int i = 0; i < sel.Length; i++)
                {
                    if (sel[i] < 0)
                        continue;
                    plavi.Append(Celija(pogled, sel[i]));
                    if (i < sel.Length - 1)
                        plavi.Append("~");
                }
            }
            return ParametriSvi.Grid(grid.Name, id, plavi.ToString(), "", redova);
        }

        private static string Celija(GridView pogled, int red)
        {
            if (red < 0)
                return "0";
            object id = pogled.GetRowCellValue(red, "id");
            if (id == null || id == DBNull.Value)
            {
                if (pogled.Columns.Count == 0)
                    return "0";
                id = pogled.GetRowCellValue(red, pogled.Columns[0]);
            }
            string t = id == null || id == DBNull.Value ? "" : Convert.ToString(id);
            return t.Length == 0 ? "0" : t;
        }

        private GridView Pogled()
        {
            foreach (Control c in SveKontrole())
            {
                var grid = c as GridControl;
                if (grid != null)
                    return grid.MainView as GridView;
            }
            return null;
        }

        private void PreracunajGrid(bool samoRed)
        {
            GridView pogled = Pogled();
            if (pogled == null)
                return;
            var tablica = pogled.GridControl.DataSource as DataTable;
            if (tablica == null)
                return;
            if (samoRed)
                RecalcGrid.IzracunajRed(tablica, pogled.GetDataSourceRowIndex(pogled.FocusedRowHandle));
            else
                RecalcGrid.Izracunaj(tablica);
            pogled.RefreshData();
        }

        private void PomakniRed(bool naprijed)
        {
            GridView pogled = Pogled();
            if (pogled == null)
                return;
            int h = pogled.FocusedRowHandle;
            int sljedeci = naprijed ? pogled.GetNextVisibleRow(h) : pogled.GetPrevVisibleRow(h);
            if (sljedeci >= 0)
                pogled.FocusedRowHandle = sljedeci;
        }

        private void PomakniNaVrijednost(string segment)
        {
            string unutra = (segment ?? "").Trim();
            int a = unutra.IndexOf('(');
            int z = unutra.LastIndexOf(')');
            if (a >= 0 && z > a)
                unutra = unutra.Substring(a + 1, z - a - 1);
            string[] dio = unutra.Split(',');
            if (dio.Length < 3)
                return;
            GridView pogled = Pogled();
            if (pogled == null)
                return;
            var tablica = pogled.GridControl.DataSource as DataTable;
            int red = RafalNaredba.NadjiVrijednost(tablica, dio[1], dio[2]);
            if (red < 0)
                return;
            int h = pogled.GetRowHandle(red);
            if (h >= 0)
            {
                pogled.FocusedRowHandle = h;
                pogled.MakeRowVisible(h);
            }
        }

        private System.Collections.Generic.List<string> OznaceniId()
        {
            var l = new System.Collections.Generic.List<string>();
            GridView pogled = Pogled();
            if (pogled == null)
                return l;
            int[] sel = pogled.GetSelectedRows();
            for (int i = 0; i < sel.Length; i++)
            {
                if (sel[i] >= 0)
                    l.Add(Celija(pogled, sel[i]));
            }
            return l;
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
            {
                // TeksT (Form2.vb 2045-2050): MULTILINE, PASSWORDCHAR, SCROLLBARS, WORDWRAP, MAXLENGTH (TXT-02).
                bool? vise = Form2Rezultat.VbBool(Attr(k, "MULTILINE"));
                TextEdit t = vise == true ? new MemoEdit() : new TextEdit();
                string lozinka = Attr(k, "PASSWORDCHAR");
                if (lozinka.Length > 0)
                    t.Properties.PasswordChar = lozinka[0];
                int duljina;
                if (int.TryParse(Attr(k, "MAXLENGTH"), NumberStyles.Integer, CultureInfo.InvariantCulture, out duljina) && duljina > 0)
                    t.Properties.MaxLength = duljina;
                var memo = t as MemoEdit;
                if (memo != null)
                {
                    int trake;
                    if (int.TryParse(Attr(k, "SCROLLBARS"), NumberStyles.Integer, CultureInfo.InvariantCulture, out trake) && trake >= 0 && trake <= 3)
                        memo.Properties.ScrollBars = (ScrollBars)trake;
                    bool? prelom = Form2Rezultat.VbBool(Attr(k, "WORDWRAP"));
                    if (prelom.HasValue)
                        memo.Properties.WordWrap = prelom.Value;
                }
                return t;
            }
            if (g == NgGrana.Number)
            {
                // Form2.vb 2085-2086: NumericEditBox dobije FORMATMASK i DECIMALS.
                // DevExpress SpinEdit bez maske dopusti samo cijeli broj (N00), pa se
                // DECIMALS mora prenijeti da unos decimala radi kao u NetGenu.
                var spin = new SpinEdit();
                spin.Properties.IsFloatValue = true;
                int dec;
                string decimale = k.Atributi == null ? null : k.Atributi.Vrijednost("DECIMALS");
                if (int.TryParse((decimale ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out dec) && dec >= 0 && dec <= 10)
                {
                    spin.Properties.IsFloatValue = dec > 0;
                    // FORMATMASK bez ',' (npr. "#0" za godinu, 4 granule) nema grupiranje tisućica: 2024, ne 2.024 (NUM-02).
                    string maska = k.Atributi == null ? null : k.Atributi.Vrijednost("FORMATMASK");
                    bool bezGrupa = !string.IsNullOrEmpty(maska) && maska.Trim().Length > 0 && maska.IndexOf(',') < 0;
                    spin.Properties.Mask.EditMask = (bezGrupa ? "f" : "n") + dec.ToString(CultureInfo.InvariantCulture);
                    spin.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
                return spin;
            }
            if (g == NgGrana.Search)
            {
                var b = new ButtonEdit();
                b.Properties.TextEditStyle = TextEditStyles.Standard;
                // Form2.vb 2145: Tag = "0" (nema odabranog id-a).
                b.Tag = "0";
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
                // Form2.vb 1642: SelectionMode.MultipleSelection (plavi redovi, @arrayofblueids).
                pogled.OptionsSelection.MultiSelect = true;
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
            var slikaLok = c as PictureEdit;
            if (slikaLok != null)
            {
                // Form2.vb 2710: ImageLocation = LOCATION.ToUpper (3 granule, UNC put do slike) (PIC-01).
                UcitajSliku(slikaLok, Attr(k, "LOCATION").ToUpperInvariant());
                return;
            }
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
                // DEFAULT ide u Value (VB CDate); CAPTION nije datum pa ne prode (Raspored daje CAPTION kad nema DEFAULT).
                DateTime dt;
                if (k != null && k.Atributi != null && k.Atributi.Ima("DEFAULT") && ProcitajDatum(prikaz, out dt))
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
            {
                // SrediKontrolu (2932) stavi CAPTION u Text, ali TeksT (2008), NumerBox (2084) i SearchBox (2144)
                // ga odmah obrisu. Ostaje DEFAULT; broj ima 0, pa DEFAULT, pa VALUE; "null" je prazno (TXT-01).
                string zadano = k != null && k.Atributi != null && k.Atributi.Ima("DEFAULT") ? (k.Atributi.Vrijednost("DEFAULT") ?? "") : null;
                var spin = c as SpinEdit;
                if (spin != null)
                {
                    string broj = k != null && k.Atributi != null && k.Atributi.Ima("VALUE") ? k.Atributi.Vrijednost("VALUE") : zadano;
                    decimal d;
                    if (broj != null && (decimal.TryParse(broj.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out d)
                        || decimal.TryParse(broj.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out d)))
                        spin.Value = d;
                    else
                        spin.Value = 0;
                    return;
                }
                if (c.GetType() == typeof(ButtonEdit) || c is LookUpEdit || c is CheckedComboBoxEdit || zadano == null)
                {
                    edit.Text = "";
                    return;
                }
                zadano = zadano.Trim();
                edit.Text = zadano.ToLowerInvariant() == "null" ? "" : zadano;
            }
        }

        /// <summary>
        /// ONLOAD2 (Form2.vb 1047, 1147, 1188-1193): drugi upit pri ucitavanju; za svaku kontrolu ciji stupac
        /// postoji DodatnoPostimajKontrolu (enable/disable/visible...), a datum dobije i vrijednost (2297-2315).
        /// 319 formi u 107 granula (FRM-02).
        /// </summary>
        private void PrimijeniOnLoad2(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            string on2 = forma == null || forma.Atributi == null ? "" : (forma.Atributi.Vrijednost("ONLOAD2") ?? "").Trim();
            if (on2.Length == 0 || citaj == null)
                return;
            string sql = SqlTekst.Sredi(on2, kontekst ?? new SqlKontekst());
            DataTable dt = null;
            NgDnevnik.Sql(sql);
            try { dt = citaj(sql); }
            catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sql); return; }
            if (dt == null || dt.Rows.Count == 0)
                return;
            DataRow r = dt.Rows[0];
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name) || !Form2Rezultat.Ima(r, c.Name.Trim()))
                    continue;
                string v = Form2Rezultat.Celija(r, c.Name.Trim());
                DodatnoPostavi(c, v);
                var datum = c as DateEdit;
                DateTime d;
                if (datum != null && !Form2Rezultat.JeStanje(v))
                {
                    object sirovo = r[c.Name.Trim()];
                    if (sirovo is DateTime)
                        datum.EditValue = (DateTime)sirovo;
                    else if (ProcitajDatum(v, out d))
                        datum.EditValue = d;
                }
            }
        }

        private void UcitajOnLoad(string on, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            string sql = SqlTekst.Sredi(on, kontekst);
            // Form2.vb 1146: SrediSQLPosljeNextForme uvijek prođe ostatak @@ (CB ili null). Sredi to radi
            // samo uz parametre grida, pa je @@gridX bez njih ostajao u SQL-u (sintaksna greška, ONL-02).
            if (sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                sql = SrediPoslije.Petlja(sql, kontekst == null ? "" : kontekst.ParametriSaGrida, kontekst == null ? null : kontekst.Clipboard, false);
            NgDnevnik.Sql(sql);
            DataTable dt = null;
            try
            {
                dt = citaj(sql);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
                GreskaOnLoad = string.IsNullOrEmpty(ex.Message) ? "greska" : ex.Message;
            }
            if (dt == null || dt.Rows.Count == 0)
                return;
            foreach (DataColumn col in dt.Columns)
            {
                string v = dt.Rows[0][col] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0][col], CultureInfo.InvariantCulture);
                _izUpita[col.ColumnName] = v;
                if (kontekst != null)
                    kontekst.ParametriSaGrida += "@" + col.ColumnName + "='" + v.Replace("'", "''") + "' , ";
            }
        }

        private void PrimijeniOnLoad()
        {
            foreach (var par in _izUpita)
            {
                Control[] n = Controls.Find(par.Key, true);
                if (n.Length == 0)
                    continue;
                PostaviVrijednost(n[0], par.Value ?? "");
            }
        }

        /// <summary>PictureBox.ImageLocation / Load: put na disku, UNC ili http, učitava se asinkrono.</summary>
        private static void UcitajSliku(PictureEdit slika, string put)
        {
            string p = (put ?? "").Trim();
            if (slika == null || p.Length == 0)
                return;
            try
            {
                slika.LoadAsync(p);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("slika " + slika.Name + " " + p + ": " + ex.Message);
            }
        }

        private static void PostaviVrijednost(Control c, string v)
        {
            var slika = c as PictureEdit;
            if (slika != null)
            {
                // Form2.vb 2713-2724: ONLOAD/ONLOAD2 stupac s imenom slike je put (http ide kroz Load).
                UcitajSliku(slika, v);
                return;
            }
            var vise = c as CheckedComboBoxEdit;
            if (vise != null)
            {
                // 2500-2502: Tag = vrijednost iz ONLOAD-a, Timer3 oznaci te id-eve.
                OznaciVise(vise, v);
                return;
            }
            var broj = c as SpinEdit;
            if (broj != null)
            {
                decimal d;
                if (decimal.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    broj.Value = d;
                return;
            }
            var kvacica = c as CheckEdit;
            if (kvacica != null)
            {
                kvacica.Checked = v == "1" || string.Equals(v, "true", StringComparison.OrdinalIgnoreCase);
                return;
            }
            var look = c as LookUpEdit;
            if (look != null)
            {
                int brojId;
                if (int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out brojId))
                    look.EditValue = brojId;
                else if (!string.IsNullOrEmpty(v))
                    look.EditValue = v;
                return;
            }
            var edit = c as TextEdit;
            if (edit != null)
            {
                edit.Text = v;
                return;
            }
            var label = c as LabelControl;
            if (label != null)
                label.Text = label.Dock == DockStyle.Bottom ? Ponasanje.StatusTekst(v) : v;
        }

        private static bool Odustajanje(string tekst)
        {
            string t = (tekst ?? "").Trim().ToLowerInvariant();
            return t == "odustani" || t == "cancel" || t == "close" || t.StartsWith("odustani", StringComparison.Ordinal);
        }
    }
}
