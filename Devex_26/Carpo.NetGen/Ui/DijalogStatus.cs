using System;
using System.Text;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;
using DevExpress.XtraEditors;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// STATUSSTRIP (Form2.vb 1256-1260, RefreshControls 4753-4757): TEXT1..3 velikim slovima, a na svakom
    /// osvježavanju idu kroz SrediSQL2, SrediSQLPosljeNextForme i stupce grida (@@VERSION,
    /// @@ROWCOUNTGRIDX, @@GRIDX). Carpo je pokazivao predložak doslovno (STS-01).
    /// </summary>
    public sealed partial class Dijalog
    {
        private void OsvjeziStatus()
        {
            if (_forma == null || _forma.Kontrole == null || IsDisposed)
                return;
            for (int i = 0; i < _forma.Kontrole.Count; i++)
            {
                NgControl k = _forma.Kontrole[i];
                if (k.Grana != NgGrana.StatusStrip || string.IsNullOrEmpty(k.Name))
                    continue;
                Control[] nadjeno = Controls.Find(k.Name, true);
                var label = nadjeno.Length > 0 ? nadjeno[0] as LabelControl : null;
                if (label == null)
                    continue;
                var sb = new StringBuilder();
                string[] dijelovi = { "TEXT1", "TEXT2", "TEXT3" };
                for (int d = 0; d < dijelovi.Length; d++)
                {
                    string t = StatusDio(Attr(k, dijelovi[d]));
                    if (t.Trim().Length == 0)
                        continue;
                    if (sb.Length > 0)
                        sb.Append("   ");
                    sb.Append(t);
                }
                label.Text = sb.ToString();
            }
        }

        /// <summary>
        /// Form2.ProcitajXML 1240: Me.Text = SrediSQLPosljeNextForme(SrediSQL2(Me.Text), , True) (CAP-01). 752 naslova
        /// formi u korpusu imaju @@ ("Izmjena dokumenta : @@broj [ @@id ]"); datumi su za ispis (dd.MM.yyyy).
        /// Prazan prevParametri u VB-u čita strSviParametriuModulu, a to je ParametriSaGrida.
        /// </summary>
        private void SrediNaslov()
        {
            SqlKontekst k = _kontekst ?? new SqlKontekst();
            string t = Text ?? "";
            if (t.IndexOf("@@", StringComparison.Ordinal) >= 0)
                Text = SrediPoslije.Zamijeni(t, k.ParametriSaGrida, k, k.Clipboard, true);
            SrediNatpise(k);
        }

        /// <summary>
        /// Form2.ProcitajXML 1324 (label), 1369 (group/frame), 1399 (panel): tekst iz CAPTION ili ONLOAD-a ide kroz
        /// SrediSQL2 i SrediSQLPosljeNextForme; labela "null" postane prazna (LBL-01). U korpusu 64 labele i 2 okvira
        /// imaju @@ ("Želite li obrisati tablu @@Šifra ?").
        /// </summary>
        private void SrediNatpise(SqlKontekst k)
        {
            if (_forma == null || _forma.Kontrole == null)
                return;
            for (int i = 0; i < _forma.Kontrole.Count; i++)
            {
                NgControl d = _forma.Kontrole[i];
                if ((d.Grana != NgGrana.Label && d.Grana != NgGrana.Group && d.Grana != NgGrana.Panel) || string.IsNullOrEmpty(d.Name))
                    continue;
                Control[] nadjeno = Controls.Find(d.Name, true);
                if (nadjeno.Length == 0)
                    continue;
                Control c = nadjeno[0];
                string t = c.Text ?? "";
                if (t.IndexOf("@@", StringComparison.Ordinal) < 0)
                    continue;
                string s = SrediPoslije.Zamijeni(t, k.ParametriSaGrida, k, k.Clipboard, false);
                if (d.Grana == NgGrana.Label && s.Trim().ToLowerInvariant() == "null")
                    s = "";
                c.Text = s;
            }
        }

        private string StatusDio(string predlozak)
        {
            string t = Ponasanje.StatusTekst(predlozak);
            if (t.IndexOf("@@", StringComparison.Ordinal) < 0 && t.IndexOf("@(", StringComparison.Ordinal) < 0)
                return t;
            SqlKontekst k = _kontekst ?? new SqlKontekst();
            string s = SrediPoslije.Zamijeni(t, k.ParametriSaGrida, k, k.Clipboard, false);
            return SrediPoslije.Kolone(s, VrijednostIzGrida);
        }
    }
}
