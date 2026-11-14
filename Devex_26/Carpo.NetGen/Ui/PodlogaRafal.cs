using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Poziv;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Podloga: Form1.Rafalno (6546), uređivanje ćelija (EDITCOLUMNS/ONCELLEDITED 6470, EDITCOLUMNSDET/
    /// ONCELLEDITEDDET 7063, BEFORECELLEDITED 7803), ONCLOSED (1306), ssPermission (2665-2790) i
    /// NovaGenerickaForma2 za više označenih redova (5003-5054).
    /// </summary>
    public sealed partial class Podloga
    {
        private DataTable _dozvole;
        private string _zadnjaPoruka;
        private object _staraVrijednost;
        private int _staraRed = int.MinValue;
        private DevExpress.XtraGrid.Columns.GridColumn _staraKolona;
        private bool _vracam;
        private bool _zadnjiRed = true;
        private int _dubinaRafala;

        // ---------------------------------------------------------------- Rafalno

        /// <summary>
        /// Form1.Rafalno 6546-6910. Vraća grešku SQL-a (GreskaKodIzvrsavanjaSQLa) ili "".
        /// Obični segment je SQL (ExecSql), ':' i OPENFORM( otvaraju formu, FOREACH* preskače tijelo.
        /// </summary>
        private string IzvrsiRafalno(string tekst)
        {
            if (_dubinaRafala > 8 || IsDisposed)
                return "";
            _dubinaRafala++;
            try
            {
                string[] ar = Form1Rafal.Segmenti(tekst);
                for (int i = 0; i < ar.Length; i++)
                {
                    string s = Form1Rafal.Pripremi(ar[i]);
                    string original = s;
                    if (s.Trim().Contains("@@"))
                    {
                        // 6565-6573: PokupiParametreSaGrida, pa SrediSQL i SrediSQLPosljeNextForme s njima.
                        SpojiId();
                        var k = _master.Kontekst;
                        s = SrediPoslije.Zamijeni(SqlTekst.Sredi(s, k), k.ParametriSaGrida, k, k.Clipboard, false);
                    }
                    string vrsta = Form1Rafal.Vrsta(s);
                    NgDnevnik.Akcija("rafalno " + vrsta + " " + s.Trim());
                    if (vrsta == "prazno" || vrsta == "nista")
                        continue;
                    if (vrsta == "beep")
                        RafalNaredba.Beep();
                    else if (vrsta == "openform")
                        OtvoriZaRedove(Form1Rafal.OpenFormIme(s));
                    else if (vrsta == "blagajna")
                        NgDnevnik.Akcija(BlagajnaNaredba.Upit(BlagajnaNaredba.Id(s)));
                    else if (vrsta == "assembly")
                    {
                        // 6602-6661 (":ASSEMBLYLOAD(", parametri reda, Driver) i 6681-6727 (bez dvotočke, prazni parametri).
                        string put = s.ToUpperInvariant().Contains(":ASSEMBLYLOAD(") ? "kolon" : "rafalno";
                        SpojiId();
                        var poziv = AssemblyPoziv.Pozovi(s.Trim(), put, _master.Stanje.Id, _master.Kontekst.OperaterId, _spoj, _master.Stanje.ZadnjiSql, AssemblyPoziv.ParametriReda(gvGla.GetFocusedDataRow()), "");
                        if (poziv.Prekinuto)
                            return "";
                    }
                    else if (vrsta == "forma")
                        OtvoriZaRedove(s.Replace(":", ""));
                    else if (vrsta == "osvjezi")
                        OsvjeziMaster();
                    else if (vrsta == "stupci")
                        PrimijeniStupce(s);
                    else if (vrsta == "isempty")
                    {
                        if (Form1Rafal.IsEmptyPreskace(s))
                            i++;
                    }
                    else if (vrsta == "boja")
                        IzaberiBoju(original);
                    else if (vrsta == "foreach")
                        i++;
                    else
                    {
                        string greska = IzvrsiRafalSql(s);
                        if (greska.Length > 0)
                            return greska;
                    }
                }
                return "";
            }
            finally
            {
                _dubinaRafala--;
            }
        }

        /// <summary>
        /// 6858-6906: cls1.ExecSql. Poruka greške kao u Class1.ExecSql (SqlPoruka). Stupci
        /// setcolorforcolorpicker (red 0) postavljaju @@SELECTEDCOLOR. Upis samo gdje ga Dijalog smije poslati.
        /// </summary>
        private string IzvrsiRafalSql(string sql)
        {
            string t = (sql ?? "").Trim();
            if (t.Length == 0)
                return "";
            if (!DijalogTok.SmijePoslati(t))
            {
                NgDnevnik.Akcija("rafalno nije poslano (upis zatvoren): " + t);
                return "";
            }
            _zadnjaPoruka = null;
            DataTable rez = Citaj(t);
            if (rez == null && _zadnjaPoruka != null)
            {
                SqlPoruka.Ishod ish = SqlPoruka.IzIznimke(_zadnjaPoruka);
                if (ish.Prikazi)
                    NgPoruka.Info(this, "\r\n" + ish.Tekst + "\r\n\r\n", "Poruka");
                return ish.Greska ?? "";
            }
            if (rez != null && rez.Rows.Count > 0 && rez.Columns.Contains("setcolorforcolorpicker"))
            {
                string boja = rez.Rows[0]["setcolorforcolorpicker"] == DBNull.Value ? "" : Convert.ToString(rez.Rows[0]["setcolorforcolorpicker"]);
                if (boja.Length > 0 && _master.Kontekst != null)
                    _master.Kontekst.SelectedColor = boja;
            }
            return "";
        }

        /// <summary>
        /// Dijalog mora vidjeti grešku SQL-a (Dijalog.Omotaj pokaže poruku i prekine lanac, MSG-01).
        /// Citaj je guta; tiha greška (ssPermission, NetGenDBFilter...) ostaje null kao u ExecSql.
        /// </summary>
        private DataTable CitajZaDijalog(string sql)
        {
            _zadnjaPoruka = null;
            DataTable t = Citaj(sql);
            if (t == null && !string.IsNullOrEmpty(_zadnjaPoruka) && !SqlPoruka.Tiha(_zadnjaPoruka))
            {
                string poruka = _zadnjaPoruka;
                _zadnjaPoruka = null;
                throw new InvalidOperationException(poruka);
            }
            return t;
        }

        /// <summary>6813-6835: ColorDialog, @@SELECTEDCOLOR je HTML boja, pa SQL iz originalnog segmenta.</summary>
        private void IzaberiBoju(string original)
        {
            if (NgPoruka.Tiho)
                return;
            using (var dlg = new ColorDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                if (_master.Kontekst != null)
                    _master.Kontekst.SelectedColor = System.Drawing.ColorTranslator.ToHtml(dlg.Color);
            }
            string param = Form1Rafal.BojaSql(original);
            if (param.Trim().Length == 0)
                return;
            var k = _master.Kontekst;
            IzvrsiRafalSql(SrediPoslije.Zamijeni(SqlTekst.Sredi(param, k), k.ParametriSaGrida, k, k.Clipboard, false));
        }

        // ---------------------------------------------------------------- gumbi i forme

        /// <summary>
        /// NovaGenerickaForma 4875-4899: "select" u lancu gumba se izvrši i prvi red postavi gumbe po natpisu.
        /// Bez veze (offline test) vraća false pa ostaje stara procjena CASE izraza.
        /// </summary>
        private bool IzvrsiGumbSelect(string segment)
        {
            if (string.IsNullOrEmpty(_spoj))
                return false;
            var k = _master.Kontekst;
            string sql = SrediPoslije.Zamijeni(SqlTekst.Sredi(segment, k), "", k, k == null ? null : k.Clipboard, false);
            if (!DijalogTok.SmijePoslati(sql))
                return true;
            DataTable t = Citaj(sql);
            if (t != null && t.Rows.Count > 0)
                PrimijeniStanjeGumba(t.Rows[0]);
            return true;
        }

        /// <summary>
        /// NovaGenerickaForma2 4925-5060: provjera ssPermission, pa forma jednom ili za svaki označeni red
        /// (CheckBoxRowSelect). Grid se osvježava samo nakon zadnjeg reda (Zadnji, 5490/5519).
        /// MULTISELECT/MULTIBLUE ide jednom, redove puni NapuniOdabir.
        /// </summary>
        private void OtvoriZaRedove(string naredba)
        {
            string ime = (naredba ?? "").Trim();
            if (ime.Length == 0)
                return;
            if (_dozvole != null && !Dozvole.SmijeKomandu(_dozvole, ime))
            {
                NgDnevnik.Akcija("ZABRANA : PERMISSIO DENIED " + ime);
                return;
            }
            int[] redovi = OznaceniRedovi();
            if (redovi.Length <= 1 || MultiOdabir.Je(ime))
            {
                OtvoriFormu(ime);
                return;
            }
            for (int i = 0; i < redovi.Length; i++)
            {
                if (IsDisposed)
                    return;
                gvGla.FocusedRowHandle = redovi[i];
                SpojiId();
                _zadnjiRed = i == redovi.Length - 1;
                try
                {
                    OtvoriFormu(ime);
                }
                finally
                {
                    _zadnjiRed = true;
                }
            }
        }

        /// <summary>GridEX1.GetCheckedRows: samo s kvačicama (CHECKBOXVISIBLE), redom kako su u gridu.</summary>
        private int[] OznaceniRedovi()
        {
            if (gvGla == null || gvGla.OptionsSelection.MultiSelectMode != GridMultiSelectMode.CheckBoxRowSelect)
                return new int[0];
            var lista = new List<int>();
            int[] sel = gvGla.GetSelectedRows();
            for (int i = 0; i < sel.Length; i++)
            {
                if (sel[i] >= 0)
                    lista.Add(sel[i]);
            }
            lista.Sort(delegate (int a, int b) { return gvGla.GetVisibleIndex(a).CompareTo(gvGla.GetVisibleIndex(b)); });
            return lista.ToArray();
        }

        // ---------------------------------------------------------------- ssPermission

        /// <summary>Form1.vb 3414: PERMISIO="true" čita ssPermission za ovu granulu i operatera.</summary>
        private void UcitajDozvole()
        {
            _dozvole = null;
            if (!Dozvole.Ukljucene(AtributProjekta("PERMISIO")))
                return;
            string xml = _master.Kontekst == null ? "" : _master.Kontekst.XmlName;
            string op = _master.Kontekst == null ? "0" : _master.Kontekst.OperaterId;
            _dozvole = Citaj(Dozvole.Sql(xml, op));
            NgDnevnik.Akcija("ssPermission redova " + (_dozvole == null ? "greska" : _dozvole.Rows.Count.ToString()));
        }

        /// <summary>ssPermissionProvjeri 2714: gumbi po natpisu bez '&amp;'.</summary>
        private void PrimijeniDozvoleGumba()
        {
            if (_dozvole == null)
                return;
            string xml = _master.Kontekst == null ? "" : _master.Kontekst.XmlName;
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                var gumb = pnlButtons.Controls[i] as DevExpress.XtraEditors.SimpleButton;
                if (gumb == null)
                    continue;
                Dozvole.Stanje st = Dozvole.Gumb(_dozvole, xml, gumb.Text);
                if (st.Omogucen.HasValue)
                    gumb.Enabled = st.Omogucen.Value;
                if (st.Vidljiv.HasValue)
                    gumb.Visible = st.Vidljiv.Value;
            }
        }

        private Dozvole.Stanje DozvolaStavke(string natpis, bool stalna)
        {
            if (_dozvole == null)
                return null;
            if (stalna)
            {
                var st = new Dozvole.Stanje();
                if (!Dozvole.SmijeKomandu(_dozvole, natpis))
                    st.Omogucen = false;
                return st;
            }
            return Dozvole.Stavka(_dozvole, _master.Kontekst == null ? "" : _master.Kontekst.XmlName, natpis);
        }

        /// <summary>Form1.vb 3468-3472: EDITCOLUMNS sa sp/select je upit, stupci su prva ćelija.</summary>
        private void UcitajEditStupce()
        {
            _master.EditStupciIzUpita = null;
            string raw = AtributProjekta("EDITCOLUMNS");
            if (!Form1Rafal.EditStupciSuUpit(raw))
                return;
            var k = _master.Kontekst;
            DataTable t = Citaj(SrediPoslije.Zamijeni(SqlTekst.Sredi(raw, k), "", k, k == null ? null : k.Clipboard, false));
            if (t != null && t.Rows.Count > 0 && t.Columns.Count > 0 && t.Rows[0][0] != DBNull.Value)
                _master.EditStupciIzUpita = Convert.ToString(t.Rows[0][0]);
        }

        // ---------------------------------------------------------------- uređivanje ćelija

        private void VeziUredjivanje()
        {
            gvGla.ShowingEditor += delegate (object s, System.ComponentModel.CancelEventArgs e) { PrijeUredjivanja(gvGla, e, false); };
            gvDet.ShowingEditor += delegate (object s, System.ComponentModel.CancelEventArgs e) { PrijeUredjivanja(gvDet, e, true); };
            gvGla.CellValueChanging += Kvacica_Mijenja;
            gvDet.CellValueChanging += Kvacica_Mijenja;
            gvGla.CellValueChanged += delegate (object s, CellValueChangedEventArgs e) { CelijaPromijenjena(gvGla, e, "ONCELLEDITED", false); };
            gvDet.CellValueChanged += delegate (object s, CellValueChangedEventArgs e) { CelijaPromijenjena(gvDet, e, "ONCELLEDITEDDET", true); };
            // GridEX1_KeyDown 7017-7025: Enter na masteru je NovaGenerickaForma(ONENTERKEY) (ENT-02).
            gvGla.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode != Keys.Enter || gvGla.IsEditing)
                    return;
                string naredba = AtributProjekta("ONENTERKEY");
                if (naredba.Trim().Length == 0)
                    return;
                e.Handled = true;
                IzvrsiLanac(naredba);
            };
        }

        /// <summary>
        /// Janus EditType po stupcu (Class1.CreateGridColumns 1071-1280): grid je uređiv samo ako ima
        /// stupaca iz EDITCOLUMNS / EDITCOLUMNSDET, a ostali stupci su NoEdit (EDT-01).
        /// </summary>
        private void PodesiUredjivanje(GridView pogled, List<GridKolona> kolone)
        {
            var uredjivi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (kolone != null)
            {
                for (int i = 0; i < kolone.Count; i++)
                {
                    GridKolona k = kolone[i];
                    if (k.Uredivanje && !k.Selektor && k.Podatak != "Icon" && !string.IsNullOrEmpty(k.Podatak))
                        uredjivi.Add(k.Podatak);
                }
            }
            bool ima = uredjivi.Count > 0;
            pogled.OptionsBehavior.Editable = ima;
            pogled.OptionsBehavior.ReadOnly = !ima;
            if (!ima)
                return;
            for (int i = 0; i < pogled.Columns.Count; i++)
                pogled.Columns[i].OptionsColumn.AllowEdit = uredjivi.Contains(pogled.Columns[i].FieldName);
        }

        /// <summary>
        /// GridEX1_EditingCell 7803-7865: PredhodnaVrijednost, pa BEFORECELLEDITED kroz Rafalno; greška
        /// otkazuje uređivanje. Detalj samo pamti vrijednost (BEFORECELLEDITEDDET se u NetGenu ne koristi).
        /// </summary>
        private void PrijeUredjivanja(GridView pogled, System.ComponentModel.CancelEventArgs e, bool detalj)
        {
            if (pogled.FocusedRowHandle < 0 || pogled.FocusedColumn == null)
                return;
            _staraVrijednost = pogled.GetFocusedRowCellValue(pogled.FocusedColumn);
            _staraRed = pogled.FocusedRowHandle;
            _staraKolona = pogled.FocusedColumn;
            if (detalj)
                return;
            string prije = AtributProjekta("BEFORECELLEDITED");
            if (prije.Trim().Length == 0)
                return;
            SpojiId();
            OznaciRed(pogled, pogled.FocusedRowHandle);
            string lokalni = Form1Rafal.ParametriCelije(pogled.GetFocusedDataRow(), false);
            var k = KontekstCelije(pogled.FocusedColumn.Caption, null, false);
            string sql = SrediPoslije.Zamijeni(SqlTekst.Sredi(prije, k), lokalni, k, k.Clipboard, false);
            if (IzvrsiRafalno(sql).Trim().Length > 0)
                e.Cancel = true;
        }

        /// <summary>Janus CheckBox šalje CellUpdated odmah; DevExpress tek kad editor izgubi fokus.</summary>
        private void Kvacica_Mijenja(object sender, CellValueChangedEventArgs e)
        {
            var pogled = sender as GridView;
            if (pogled == null || e == null || e.Column == null || e.RowHandle < 0)
                return;
            if (e.Column.ColumnType == typeof(bool))
                pogled.PostEditor();
        }

        /// <summary>
        /// GridEX1_CellUpdated 6470-6536 / GridEX2_CellUpdated 7063-7127: @@EDITEDCOLUMN, @@LASTVALUE,
        /// red dobije kvačicu, SQL = SrediSQLPosljeNextForme(SrediSQL(ONCELLEDITED), parametri ćelije, IdiUclipbord=False),
        /// pa Rafalno. Greška vraća staru vrijednost (EDT-02).
        /// </summary>
        private void CelijaPromijenjena(GridView pogled, CellValueChangedEventArgs e, string atribut, bool detalj)
        {
            if (_vracam || _punim || e == null || e.Column == null || e.RowHandle < 0)
                return;
            SpojiId();
            if (detalj && _master.Kontekst != null)
                _master.Kontekst.DetId = DetaljId();
            var k = KontekstCelije(e.Column.Caption, e.Value, true);
            OznaciRed(pogled, e.RowHandle);
            string tekst = AtributProjekta(atribut);
            if (tekst.Trim().Length == 0)
                return;
            string lokalni = Form1Rafal.ParametriCelije(pogled.GetDataRow(e.RowHandle));
            string sql = SrediPoslije.Zamijeni(SqlTekst.Sredi(tekst, k), lokalni, k, null, false);
            NgDnevnik.Akcija(atribut + " " + sql);
            string greska = IzvrsiRafalno(sql);
            if (greska.Trim().Length == 0 || IsDisposed)
                return;
            object stara = _staraRed == e.RowHandle && _staraKolona == e.Column ? _staraVrijednost : IzvornaVrijednost(pogled, e);
            _vracam = true;
            try
            {
                pogled.SetRowCellValue(e.RowHandle, e.Column, stara ?? DBNull.Value);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("vrati vrijednost: " + ex.Message);
            }
            finally
            {
                _vracam = false;
            }
        }

        /// <summary>Vrijednost nije zapamćena u ShowingEditor (promjena iz koda): izvorna iz DataRow-a.</summary>
        private static object IzvornaVrijednost(GridView pogled, CellValueChangedEventArgs e)
        {
            DataRow red = pogled.GetDataRow(e.RowHandle);
            if (red == null || string.IsNullOrEmpty(e.Column.FieldName) || !red.Table.Columns.Contains(e.Column.FieldName))
                return null;
            if (red.HasVersion(DataRowVersion.Original))
                return red[e.Column.FieldName, DataRowVersion.Original];
            return null;
        }

        /// <summary>
        /// Form1.SrediSQL 2494-2500: @@EDITEDCOLUMN je natpis stupca, @@LASTVALUE vrijednost. Parametri
        /// grida ne ulaze u SrediSQL (to je posao SrediSQLPosljeNextForme s parametrima ćelije).
        /// </summary>
        private SqlKontekst KontekstCelije(string stupac, object vrijednost, bool postaviZadnju)
        {
            if (_master.Kontekst != null)
            {
                _master.Kontekst.EditedColumn = stupac ?? "";
                if (postaviZadnju)
                    _master.Kontekst.LastValue = Form1Rafal.ZadnjaVrijednost(vrijednost);
            }
            SqlKontekst k = SqlTekst.ZaFormu(_master.Kontekst);
            k.ParametriSaGrida = "";
            return k;
        }

        /// <summary>CurrentRow.CheckState = Checked (6522, 7114, 7857) kad grid ima kvačice.</summary>
        private static void OznaciRed(GridView pogled, int red)
        {
            if (pogled.OptionsSelection.MultiSelectMode == GridMultiSelectMode.CheckBoxRowSelect && red >= 0)
                pogled.SelectRow(red);
        }

        // ---------------------------------------------------------------- zatvaranje

        /// <summary>Form1_FormClosed 1298-1306: Rafalno(strONClosed) bez SrediSQL (radi ga Rafalno za @@).</summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                string zatvori = AtributProjekta("ONCLOSED");
                if (zatvori.Trim().Length > 0 && !string.IsNullOrEmpty(_spoj))
                    IzvrsiRafalno(zatvori);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("ONCLOSED: " + ex.Message);
            }
            base.OnFormClosed(e);
        }
    }
}
