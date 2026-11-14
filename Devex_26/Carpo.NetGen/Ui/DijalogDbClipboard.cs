using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// NetGenDBClipboard (DBC-01): forma s DBCLIPBOARD="true" ili s "filter" u imenu pamti vrijednosti
    /// kontrola po operateru. Čitanje ide nakon ONLOAD i ONLOAD2 (Form2.vb 1482, 2030, 2112, 2184, 2346,
    /// 2455, 2523), pisanje u PokupiParametre (3429-3662, SpremiuDBClipboard 4000).
    /// </summary>
    public sealed partial class Dijalog
    {
        private bool _dbCb;
        private readonly Dictionary<string, string> _dbCbZapisano = new Dictionary<string, string>(StringComparer.Ordinal);

        private string XmlDb()
        {
            return _kontekst == null ? "" : (_kontekst.XmlName ?? "");
        }

        private string OperaterDb()
        {
            if (!string.IsNullOrEmpty(Operater))
                return Operater;
            return _kontekst == null ? "0" : (_kontekst.OperaterId ?? "0");
        }

        /// <summary>1170-1182: select iz NetGenDBClipboard; greška (nema tablice) gasi pamćenje.</summary>
        private void UcitajDbClipboard(NgForm forma)
        {
            _dbCb = false;
            _dbCbZapisano.Clear();
            if (_citaj == null || forma == null || !DbClipboard.Ukljucen(forma.Ime, AtributForme("DBCLIPBOARD")))
                return;
            DataTable t;
            try
            {
                t = _citaj(DbClipboard.CitajSql(OperaterDb()));
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("NetGenDBClipboard: " + ex.Message);
                return;
            }
            if (t == null)
                return;
            _dbCb = true;
            Dictionary<string, string> mapa = DbClipboard.Mapa(t);
            foreach (var par in mapa)
                _dbCbZapisano[par.Key] = (par.Value ?? "").Trim();
            string xml = XmlDb();
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name))
                    continue;
                string v;
                bool ima = mapa.TryGetValue(DbClipboard.Kljuc(xml, c.Name), out v);
                if (c.GetType() == typeof(ButtonEdit))
                {
                    // 2184-2200: Tag je id, "ime" + caption je tekst; "null" je prazno / 0.
                    string natpis;
                    if (ima)
                        c.Tag = v;
                    if (mapa.TryGetValue(DbClipboard.Kljuc(xml, c.Name + "caption"), out natpis))
                        ((ButtonEdit)c).Text = natpis;
                    if (((ButtonEdit)c).Text == "null")
                        ((ButtonEdit)c).Text = "";
                    if (c.Tag as string == "null")
                        c.Tag = "0";
                    continue;
                }
                if (!ima)
                    continue;
                PrimijeniDbVrijednost(c, v ?? "");
            }
            NgDnevnik.Akcija("NetGenDBClipboard " + forma.Ime + " kljuceva=" + mapa.Count.ToString());
        }

        private void PrimijeniDbVrijednost(Control c, string v)
        {
            var chk = c as CheckEdit;
            if (chk != null)
            {
                if (chk.Properties.CheckStyle != CheckStyles.Radio)
                    chk.Checked = DbClipboard.Kvacica(v);
                return;
            }
            var vise = c as CheckedComboBoxEdit;
            if (vise != null)
            {
                // 2516-2530: samo kad ONLOAD nije dao vrijednost.
                string iz;
                if (!_izUpita.TryGetValue(c.Name, out iz) || string.IsNullOrEmpty(iz))
                    OznaciVise(vise, v);
                return;
            }
            var look = c as LookUpEdit;
            if (look != null)
            {
                PostaviVrijednost(c, v);
                return;
            }
            var spin = c as SpinEdit;
            if (spin != null)
            {
                decimal d;
                if (decimal.TryParse(v, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    spin.Value = d;
                return;
            }
            var datum = c as DateEdit;
            if (datum != null)
            {
                DateTime d;
                if (DateTime.TryParse(v, CultureInfo.CurrentCulture, DateTimeStyles.None, out d))
                    datum.EditValue = d;
                return;
            }
            var txt = c as TextEdit;
            if (txt != null)
            {
                // 2033-2035: "null" je prazno. ¸¸ je navodnik spremljen u 3662 (VB ga ostavi; ovdje se vrati ').
                txt.Text = v.Trim().ToLowerInvariant() == "null" ? "" : v.Replace("¸¸", "'");
            }
        }

        /// <summary>Vrijednosti koje PokupiParametre sprema (3429-3662), redom kontrola.</summary>
        private List<KeyValuePair<string, string>> DbVrijednosti()
        {
            var l = new List<KeyValuePair<string, string>>();
            foreach (Control c in SveKontrole())
            {
                if (string.IsNullOrEmpty(c.Name))
                    continue;
                var chk = c as CheckEdit;
                if (chk != null)
                {
                    if (chk.Properties.CheckStyle != CheckStyles.Radio)
                        l.Add(new KeyValuePair<string, string>(c.Name, chk.CheckState.ToString()));
                    continue;
                }
                var vise = c as CheckedComboBoxEdit;
                if (vise != null)
                {
                    l.Add(new KeyValuePair<string, string>(c.Name, Oznaceni(vise)));
                    continue;
                }
                var look = c as LookUpEdit;
                if (look != null)
                {
                    object v = look.EditValue;
                    l.Add(new KeyValuePair<string, string>(c.Name, v == null || v == DBNull.Value ? "0" : Convert.ToString(v, CultureInfo.CurrentCulture)));
                    continue;
                }
                if (c.GetType() == typeof(ButtonEdit))
                {
                    l.Add(new KeyValuePair<string, string>(c.Name, c.Tag as string ?? ""));
                    l.Add(new KeyValuePair<string, string>(c.Name + "caption", ((ButtonEdit)c).Text ?? ""));
                    continue;
                }
                var spin = c as SpinEdit;
                if (spin != null)
                {
                    l.Add(new KeyValuePair<string, string>(c.Name, Convert.ToString(spin.Value, CultureInfo.CurrentCulture)));
                    continue;
                }
                var datum = c as DateEdit;
                if (datum != null)
                {
                    if (datum.EditValue is DateTime)
                        l.Add(new KeyValuePair<string, string>(c.Name, ((DateTime)datum.EditValue).ToString(CultureInfo.CurrentCulture)));
                    continue;
                }
                var txt = c as TextEdit;
                if (txt != null)
                    l.Add(new KeyValuePair<string, string>(c.Name, (txt.Text ?? "").Trim().Replace("'", "¸¸")));
            }
            return l;
        }

        /// <summary>
        /// SpremiuDBClipboard 4000-4023: count, pa insert ili update. Piše se samo vrijednost koja se razlikuje
        /// od one u tablici (isti krajnji sadržaj kao VB, bez upita na svaki događaj).
        /// </summary>
        private void SpremiDbClipboard()
        {
            if (!_dbCb || _citaj == null || IsDisposed)
                return;
            string op = OperaterDb();
            string xml = XmlDb();
            foreach (var par in DbVrijednosti())
            {
                string k = DbClipboard.Kljuc(xml, par.Key);
                string nova = (par.Value ?? "").Trim();
                string stara;
                if (_dbCbZapisano.TryGetValue(k, out stara) && stara == nova)
                    continue;
                _dbCbZapisano[k] = nova;
                string ubaci = DbClipboard.UbaciSql(op, xml, par.Key, nova);
                if (!DijalogTok.SmijePoslati(ubaci))
                    continue;
                try
                {
                    DataTable n = _citaj(DbClipboard.BrojSql(op, xml, par.Key));
                    if (n == null || n.Rows.Count == 0 || n.Columns.Count == 0)
                        continue;
                    bool nema = Convert.ToInt32(n.Rows[0][0], CultureInfo.InvariantCulture) == 0;
                    _citaj(nema ? ubaci : DbClipboard.IzmijeniSql(op, xml, par.Key, nova));
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("NetGenDBClipboard upis: " + ex.Message);
                }
            }
        }
    }
}
