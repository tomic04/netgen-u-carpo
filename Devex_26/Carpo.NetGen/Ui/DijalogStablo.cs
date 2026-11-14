using System;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Stablo na dijalogu (INPUT TYPE="tree", 11 granula: klase robe, AOP obrasci, JOPPD katalozi), TRE-03.
    /// Form2.vb 2558-2601: SQL je "sp:forma#tekst:naredba"; segment s "tree" puni DazovaIzStore 2825-2858
    /// (stupci id, roditelj, natpis, oznaka), ostali su čvorovi s oznakom ":naredba", yearmonth se preskače.
    /// Odabir čvora (AfterSelect → Glbl_Click) ide u ONCLICK, parametar je oznaka čvora (3667-3678), desni
    /// klik je POPUP (Glbl_TreeMouseUp), a "refresh" u rezultatu ga puni iznova (RefreshControls 5071-5088).
    /// Čvorove slaže isti Stablo kao podloga (NaDijalogu).
    /// </summary>
    public sealed partial class Dijalog
    {
        private bool _punimStablo;

        private void NapuniStablo(Control c, NgControl k, bool osvjezi)
        {
            var stablo = c as TreeList;
            if (stablo == null || k == null)
                return;
            string sirovo = Attr(k, "SQL");
            if (sirovo.Length == 0)
                return;
            // 2565 kod otvaranja SrediSQLPosljeNextForme s parametrima ulaza; kod osvježavanja DazovaIzStore
            // sredi izvorni SQL s parametrima dijaloga.
            SqlKontekst kontekst = _kontekst ?? new SqlKontekst();
            string sql = osvjezi
                ? SrediPoslije.Zamijeni(sirovo, PokupiParametre(), kontekst, kontekst.Clipboard, false)
                : SqlTekst.Sredi(sirovo, kontekst);
            var izvor = new Stablo();
            izvor.NaDijalogu = true;
            izvor.Kontekst = kontekst;
            try
            {
                izvor.IzTeksta(sql, null, _citaj);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska(" TREE PROBLEM : " + ex.Message);
                return;
            }
            _punimStablo = true;
            try
            {
                PripremiStablo(stablo);
                stablo.BeginUnboundLoad();
                stablo.Nodes.Clear();
                for (int i = 0; i < izvor.Stanje.Korijen.Count; i++)
                    DodajCvor(stablo, izvor.Stanje.Korijen[i], null);
                stablo.EndUnboundLoad();
                stablo.ExpandAll();
                // TreeView nema odabran čvor dok ga korisnik ne odabere: parametar je null.
                stablo.FocusedNode = null;
            }
            finally
            {
                _punimStablo = false;
            }
        }

        private static void PripremiStablo(TreeList stablo)
        {
            if (stablo.Columns.Count > 0)
                return;
            var stupac = stablo.Columns.Add();
            stupac.Caption = "";
            stupac.Visible = true;
            stupac.VisibleIndex = 0;
            stablo.OptionsView.ShowColumns = false;
            stablo.OptionsView.ShowIndicator = false;
            stablo.OptionsBehavior.Editable = false;
            CarpoIzgled.SlikeCvorova(stablo);
        }

        private static void DodajCvor(TreeList stablo, Cvor cvor, TreeListNode roditelj)
        {
            TreeListNode n = stablo.AppendNode(new object[] { cvor.Natpis ?? "" }, roditelj);
            n.ImageIndex = 0;
            n.SelectImageIndex = 0;
            n.Tag = cvor;
            for (int i = 0; i < cvor.Djeca.Count; i++)
                DodajCvor(stablo, cvor.Djeca[i], n);
        }

        /// <summary>3667-3678: @ime='oznaka' odabranog čvora, inače null.</summary>
        private static string ParametarStabla(TreeList stablo)
        {
            Cvor cvor = stablo.FocusedNode == null ? null : stablo.FocusedNode.Tag as Cvor;
            return ParametriSvi.Stablo(stablo.Name, cvor == null ? null : (cvor.Tag ?? ""));
        }

        private void VeziStablo(TreeList stablo, NgControl k)
        {
            string klik = Attr(k, "ONCLICK");
            stablo.FocusedNodeChanged += delegate
            {
                if (_slazem || _punimStablo || stablo.FocusedNode == null)
                    return;
                IzvrsiDogadjaj(klik);
            };
            string popup = Attr(k, "POPUP");
            if (popup.Length == 0)
                return;
            stablo.MouseUp += delegate (object s, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Right)
                    return;
                var meni = new ContextMenuStrip();
                var stavke = Form2Dogadjaj.Izbornik(popup);
                for (int i = 0; i < stavke.Count; i++)
                {
                    string[] dio = stavke[i].Split(new[] { '|' }, 2);
                    string cilj = dio.Length > 1 ? dio[1] : "";
                    meni.Items.Add(dio[0], null, delegate { IzvrsiIzbornik(cilj); });
                }
                meni.Show(stablo, e.Location);
            };
        }
    }
}
