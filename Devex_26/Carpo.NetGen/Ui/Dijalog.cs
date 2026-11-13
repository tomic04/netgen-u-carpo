using System;
using System.Collections.Generic;
using System.Data;
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
    public sealed class Dijalog : XtraForm
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
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            StartPosition = FormStartPosition.CenterParent;
            Width = 480;
            Height = 320;
            KeyPreview = true;
        }

        public void Prikazi(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj, bool modal = true, Form vlasnik = null)
        {
            Slozi(forma, kontekst, citaj);
            if (forma == null)
                return;
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

            PrimijeniOnLoad();
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
            }
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
            if (_kontekst != null)
                sql = SqlTekst.Sredi(sql, _kontekst);
            bool prazan = ComboDijalog.AllowEmpty(k);
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
            var look = c as LookUpEdit;
            if (look != null)
            {
                look.Properties.DataSource = dt;
                look.Properties.ValueMember = dt.Columns[0].ColumnName;
                look.Properties.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
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
            bool lanac = koraci.Count > 1;
            for (int s = 0; s < koraci.Count; s++)
            {
                string v = koraci[s].Vrsta;
                if (v == "poruka" || v == "roditelj" || v == "odustani" || v == "ok" || v == "okstay" || v == "forma"
                    || v == "foreach" || v == "fiskal" || v == "datoteka" || v == "refresh" || v == "dock"
                    || v == "eval" || v == "sljedeci" || v == "prethodni" || v == "naredba"
                    || v == "recalc" || v == "recalcred" || v == "toggle" || v == "priprema" || v == "pivot" || v == "chart" || v == "boja" || v == "posalji" || v == "pokretanje" || v == "beep" || v == "ispisext"
                    || v == "eval" || v == "sljedeci" || v == "prethodni" || v == "svekontrole" || v == "proces" || v == "mapa" || v == "datotekadlg" || v == "ispis" || v == "blagajna"
                    || v == "timer" || v == "timeroff" || v == "progres" || v == "trazi" || v == "stupac" || v == "objekt" || v == "predlozak")
                    lanac = true;
            }
            if (lanac)
            {
                for (int i = 0; i < koraci.Count; i++)
                {
                    DijalogTok.Korak korak = koraci[i];
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
                            return;
                        continue;
                    }
                    if (korak.Vrsta == "forma")
                    {
                        OtvorenaForma = korak.Tekst;
                        if (OtvoriDrugu != null)
                            OtvoriDrugu(korak.Tekst);
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
                        try { PrimeniRed(_citaj(dio)); }
                        catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + dio); }
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
                                return;
                        }
                        else if (korak.Tekst.ToUpperInvariant().IndexOf("ISEMPTYORZEROCANCELBOX(", StringComparison.Ordinal) >= 0)
                        {
                            if (!RafalNaredba.OtkazImaPoruku(korak.Tekst))
                                return;
                            if (RafalNaredba.CancelBoxStani(RafalNaredba.OtkazParam(korak.Tekst), RafalNaredba.OtkazTekst(korak.Tekst)))
                            {
                                string ime = RafalNaredba.OtkazParam(korak.Tekst).TrimStart('@');
                                Control[] f = Controls.Find(ime, true);
                                if (f.Length > 0)
                                    f[0].Focus();
                                return;
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
            {
                DevExpress.XtraGrid.GridControl grid = null;
                foreach (Control c in Controls)
                {
                    grid = c as DevExpress.XtraGrid.GridControl;
                    if (grid != null)
                        break;
                }
                string orij = _forma != null && _forma.Atributi != null && _forma.Atributi.Ima("PAGEORIENTATION")
                    ? _forma.Atributi.Vrijednost("PAGEORIENTATION") : "";
                DataTable tab = grid == null ? null : grid.DataSource as DataTable;
                Carpo.NetGen.Print.PrintBridge.Ispisi(naredba, true, Id, Operater, Konekcija, "", ZadnjiSql, "", SviParametri(), "", "", "", Text, grid, orij, tab);
            }
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
                    try
                    {
                        _citaj(dio);
                    }
                    catch (Exception ex)
                    {
                        NgDnevnik.Greska(ex.Message + " | " + dio);
                    }
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
            string s = ZamijeniIzKontrola(naredba);
            if (_kontekst != null)
                s = SqlTekst.ZaGumb(s, _kontekst);
            else if (s.IndexOf("@@", StringComparison.Ordinal) >= 0)
                s = DetailGrid.ZamijeniOstatak(s, "");
            return s;
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
                    if (broj != null)
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

        private void OsvjeziKontrolu(string ime)
        {
            if (_forma == null || _forma.Kontrole == null || _citaj == null)
                return;
            for (int i = 0; i < _forma.Kontrole.Count; i++)
            {
                NgControl k = _forma.Kontrole[i];
                if (k == null || !string.Equals(k.Name, ime, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (k.Atributi == null || !k.Atributi.Ima("SQL"))
                    return;
                string sql = SqlTekst.Sredi(k.Atributi.Vrijednost("SQL") ?? "", _kontekst ?? new SqlKontekst());
                if (ZastitaSql.Tekst(sql).Pise)
                    return;
                DataTable dt = null;
                try { dt = _citaj(sql); }
                catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sql); return; }
                Control[] n = Controls.Find(k.Name, true);
                if (n.Length == 0 || dt == null)
                    return;
                var look = n[0] as LookUpEdit;
                if (look == null || dt.Columns.Count == 0)
                    return;
                look.Properties.DataSource = dt;
                look.Properties.ValueMember = dt.Columns[0].ColumnName;
                look.Properties.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
                return;
            }
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
            string sql = SrediPoslije.Zamijeni(tekst ?? "", SviParametri(), _kontekst ?? new SqlKontekst(), null, false);
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

        private void PrimeniRed(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            foreach (DataColumn col in dt.Columns)
            {
                Control[] n = Controls.Find(col.ColumnName, true);
                if (n.Length == 0)
                    continue;
                string v = dt.Rows[0][col] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0][col]);
                if (string.Equals((v ?? "").Trim(), "refresh", StringComparison.OrdinalIgnoreCase))
                {
                    NgControl def = NadjiKontrolu(col.ColumnName);
                    if (def != null)
                        NapuniKontrolu(n[0], def);
                    continue;
                }
                var edit = n[0] as TextEdit;
                if (edit != null && !(edit is LookUpEdit))
                    edit.Text = v;
            }
        }

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
                c.Enter += delegate { _stariTekst[c] = c.Text ?? ""; };
                c.Leave += delegate
                {
                    string stari;
                    _stariTekst.TryGetValue(c, out stari);
                    string korak = Form2Dogadjaj.TrazilicaKorak(c.Text, stari);
                    if (korak == "preskoci" && (c.Text ?? "").Trim().Length == 0)
                        c.Tag = "0";
                    if (korak == "trazilica")
                        ZadnjaTrazilica = c.Text ?? "";
                    if (lost.Length > 0 && _izlazTrazilice.Validated2Smije())
                        IzvrsiSql(lost);
                    var zuta = c as TextEdit;
                    if (zuta != null)
                        zuta.BackColor = System.Drawing.Color.LightYellow;
                };
            }
            else if (lost.Length > 0)
                c.Leave += delegate { IzvrsiSql(lost); };
            if (k.Grana == NgGrana.MultiDrop)
            {
                string err = Form2Dogadjaj.MultiDropNaredba(Attr(k, "ERRORTEXT"));
                if (err.Length > 0)
                    c.Leave += delegate { IzvrsiSql(err); };
            }
            if (k.Grana == NgGrana.Label || k.Grana == NgGrana.Check || k.Grana == NgGrana.Radio)
            {
                string tag = Attr(k, "ONCLICK");
                if (tag.Length == 0)
                    tag = Attr(k, "TAG");
                if (tag.Length > 0)
                    c.Click += delegate { IzvrsiSql(Form2Dogadjaj.KlikLabele(tag)); };
            }
            if (k.Grana == NgGrana.Combo && klik.Length > 0)
            {
                var look = c as LookUpEdit;
                if (look != null)
                    look.EditValueChanged += delegate { IzvrsiSql(klik); };
            }
            if (k.Grana == NgGrana.Date && vrijednost.Length > 0)
            {
                var datum = c as DateEdit;
                if (datum != null)
                    datum.EditValueChanged += delegate { IzvrsiSql(vrijednost); };
            }
            if (promjena.Length > 0)
            {
                var tekst = c as TextEdit;
                if (tekst != null && !(tekst is LookUpEdit) && !(tekst is DateEdit))
                    tekst.TextChanged += delegate { IzvrsiSql(promjena); };
            }
            string got = Attr(k, "GOTFOCUS");
            if (got.Length > 0)
                c.Enter += delegate { IzvrsiSql(got); };
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
                            _kontekst.LastValue = Form2Dogadjaj.VrijednostDvoklika(celija == null ? "" : Convert.ToString(celija));
                        IzvrsiSql(dbl);
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
                    view.CellValueChanged += delegate
                    {
                        if (vracam)
                            return;
                        string greska = IzvrsiSqlGreska(celijaSql);
                        if (!Form2Dogadjaj.VratiStaru(greska) || view.FocusedColumn == null)
                            return;
                        vracam = true;
                        try { view.SetFocusedRowCellValue(view.FocusedColumn, stara); }
                        finally { vracam = false; }
                    };
                }
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
            Form2Dogadjaj.Tipka t = Form2Dogadjaj.KeyDown(trazilica, grid, false, false, true, kod, control, "", "", "");
            if (t.ValidirajTrazilicu && trazilica)
            {
                _izlazTrazilice.Validated();
                ZadnjaTrazilica = aktivna.Text ?? "";
            }
            if (t.Sljedeca && ActiveControl != null)
            {
                SelectNextControl(trazilica || grid ? aktivna : ActiveControl, true, true, true, true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
            foreach (Control c in Controls)
            {
                var g = c as GridControl;
                if (g != null && string.Equals(g.Name, gridIme, StringComparison.OrdinalIgnoreCase))
                {
                    grid = g;
                    break;
                }
            }
            var pogled = grid == null ? null : grid.MainView as GridView;
            var svi = new System.Collections.Generic.List<string>();
            var plavi = new System.Collections.Generic.List<string>();
            var cek = new System.Collections.Generic.List<string>();
            if (pogled != null)
            {
                for (int r = 0; r < pogled.DataRowCount; r++)
                    svi.Add(Celija(pogled, pogled.GetRowHandle(r)));
                int[] sel = pogled.GetSelectedRows();
                for (int i = 0; i < sel.Length; i++)
                {
                    if (sel[i] >= 0)
                        plavi.Add(Celija(pogled, sel[i]));
                }
                DevExpress.XtraGrid.Columns.GridColumn kol = null;
                for (int c = 0; c < pogled.Columns.Count; c++)
                {
                    if (pogled.Columns[c].ColumnType == typeof(bool))
                    {
                        kol = pogled.Columns[c];
                        break;
                    }
                }
                if (kol != null)
                {
                    for (int r = 0; r < pogled.DataRowCount; r++)
                    {
                        int h = pogled.GetRowHandle(r);
                        object kvaka = pogled.GetRowCellValue(h, kol);
                        if (kvaka is bool && (bool)kvaka)
                            cek.Add(Celija(pogled, h));
                    }
                }
            }
            string[] sqlovi = ForeachNaredba.Ponovi(tijelo, ForeachNaredba.Idjevi(vrsta, svi, plavi, cek));
            for (int i = 0; i < sqlovi.Length; i++)
            {
                if (_citaj == null || _jupiter || !DijalogTok.SmijePoslati(sqlovi[i]))
                    continue;
                try { _citaj(sqlovi[i]); }
                catch (Exception ex) { NgDnevnik.Greska(ex.Message + " | " + sqlovi[i]); }
            }
        }

        private string SviParametri()
        {
            var sb = new StringBuilder();
            foreach (Control c in Controls)
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
                var txt = c as TextEdit;
                if (txt != null)
                {
                    var spin = txt as SpinEdit;
                    if (spin != null)
                    {
                        sb.Append(ParametriSvi.Broj(c.Name, txt.Text));
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
                        sb.Append(ParametriSvi.Datum(c.Name, "", ima, ima ? (DateTime?)d : null));
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
            foreach (Control c in Controls)
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

        private void UcitajOnLoad(string on, SqlKontekst kontekst, Func<string, DataTable> citaj)
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

        private static void PostaviVrijednost(Control c, string v)
        {
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
