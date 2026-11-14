using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// ssPermission na dijalogu (DOZ-02). PERMISIO="true" na FORM (Form2.vb 1097) čita ssPermission za
    /// objekt xml.forma (ssPermissionProcitaj 5657, 2801-2804), poslije ONLOADED primijeni na kontrole
    /// (Form2_Load 5797, ssPermissionProvjeri 5674), provjeri formu prije otvaranja (NovaGenerickaForma2
    /// 4423/4459), kontrolu nakon visible/enable iz rezultata (DodatnoPostimajKontrolu 4489/4513) i stalne
    /// stavke desnog klika grida (8787-8826). VB gleda samo izravnu djecu forme; Carpo sve kontrole.
    /// </summary>
    public sealed partial class Dijalog
    {
        private DataTable _dozvole;

        private string ObjektDozvola()
        {
            string xml = _kontekst == null ? "" : (_kontekst.XmlName ?? "");
            return Dozvole.Objekt(xml, _forma == null ? "" : _forma.Ime);
        }

        private void UcitajDozvole(NgForm forma, Func<string, DataTable> citaj)
        {
            _dozvole = null;
            if (forma == null || forma.Kontrole.Count == 0 || citaj == null || !Dozvole.Ukljucene(AtributForme("PERMISIO")))
                return;
            string op = (Operater ?? "").Trim();
            if (op.Length == 0 && _kontekst != null)
                op = _kontekst.OperaterId ?? "";
            string xml = _kontekst == null ? "" : (_kontekst.XmlName ?? "");
            try
            {
                _dozvole = citaj(Dozvole.SqlForme(xml, forma.Ime, op));
            }
            catch (Exception ex)
            {
                // On Error Resume Next: bez tablice nema zabrana.
                NgDnevnik.Greska("ssPermission: " + ex.Message);
                _dozvole = null;
            }
            NgDnevnik.Akcija("ssPermission " + forma.Ime + " redova " + (_dozvole == null ? "greska" : _dozvole.Rows.Count.ToString()));
        }

        /// <summary>ssPermissionProvjeri 5674-5721, poslije ONLOADED.</summary>
        private void PrimijeniDozvole()
        {
            if (_dozvole == null || _dozvole.Rows.Count == 0)
                return;
            string objekt = ObjektDozvola();
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name) || c.Name.StartsWith("lbl", StringComparison.OrdinalIgnoreCase))
                    continue;
                Postavi(c, Dozvole.Kontrola(_dozvole, objekt, c.Name));
            }
        }

        /// <summary>ssPermissionProvjeriKontrolu 5723-5758: nakon visible/enable iz rezultata.</summary>
        private void DozvolaKontrole(Control c)
        {
            if (_dozvole == null || c == null || string.IsNullOrEmpty(c.Name))
                return;
            Postavi(c, Dozvole.KontrolaNaknadno(_dozvole, c.Name));
        }

        private void Postavi(Control c, Dozvole.Stanje st)
        {
            if (st == null)
                return;
            if (st.Omogucen.HasValue)
                c.Enabled = st.Omogucen.Value;
            if (st.Vidljiv.HasValue)
                c.Visible = st.Vidljiv.Value;
            if (st.Natpis.HasValue)
            {
                Control[] natpis = Controls.Find("lbl" + c.Name, true);
                if (natpis.Length > 0)
                    natpis[0].Visible = st.Natpis.Value;
            }
        }

        /// <summary>NovaGenerickaForma2 4423-4426: zabranjena forma se ne otvara, samo zapis u log.</summary>
        private bool SmijeOtvoriti(string ime)
        {
            if (_dozvole == null)
                return true;
            // 4401-4404: provjerava se ime bez (parametara).
            string n = (ime ?? "").Trim();
            int z = n.IndexOf('(');
            if (z > 0)
                n = n.Substring(0, z);
            if (Dozvole.SmijeKomandu(_dozvole, n))
                return true;
            NgDnevnik.Akcija("ZABRANA : PERMISSIO DENIED LOADING " + ime);
            return false;
        }

        /// <summary>
        /// Desni klik grida (8730-8826). Stalne stavke idu kroz ssPermissionProvjeriRafalnuKomandu; stavke
        /// POPUP-a VB uspoređuje s objektom bez forme, a pročitani redovi imaju xml.forma, pa ne djeluju.
        /// </summary>
        private Dozvole.Stanje DozvolaStavkeGrida(string natpis, bool stalna)
        {
            if (_dozvole == null)
                return null;
            if (stalna)
            {
                var st = new Dozvole.Stanje();
                if (!Dozvole.SmijeKomandu(_dozvole, natpis))
                {
                    st.Omogucen = false;
                    NgDnevnik.Akcija("ZABRANA : PERMISSIO DENIED " + natpis);
                }
                return st;
            }
            return Dozvole.Stavka(_dozvole, _kontekst == null ? "" : (_kontekst.XmlName ?? ""), natpis);
        }
    }
}
