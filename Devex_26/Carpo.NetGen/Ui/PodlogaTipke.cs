using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Carpo.NetGen.Log;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Form1_KeyDown (1349-1380) i ButtonHELP: Escape zatvara podlogu, F1 i gumb "?" pokreću JupiterHelp s HELPKOD.
    /// Carpo JupiterShared_KeyDown čita DefinitionForm, koji podloga nema, pa bi svaka tipka bacila
    /// NullReferenceException (KeyPreview je uključen). Zato podloga sama obrađuje tipke (KEY-01, HLP-01).
    /// </summary>
    public sealed partial class Podloga
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
                return;
            if (e.KeyCode == Keys.Escape)
            {
                // Escape u ćeliji ili filter retku poništava unos, ne zatvara granulu.
                if (UredjujeSe(gvGla) || UredjujeSe(gvDet))
                    return;
                e.Handled = true;
                Close();
                return;
            }
            if (e.KeyCode == Keys.F1 && AtributProjekta("HELPKOD").Trim().Length > 0)
            {
                e.Handled = true;
                PokaziPomoc();
            }
        }

        private static bool UredjujeSe(DevExpress.XtraGrid.Views.Grid.GridView pogled)
        {
            if (pogled == null)
                return false;
            return pogled.IsEditing || pogled.FocusedRowHandle == GridControl.AutoFilterRowHandle;
        }

        /// <summary>3489-3495: ButtonHELP ("?") vidljiv kad postoji HELPKOD.</summary>
        private void DodajGumbPomoci()
        {
            if (AtributProjekta("HELPKOD").Trim().Length == 0)
                return;
            int desno = 0;
            for (int i = 0; i < pnlButtons.Controls.Count; i++)
            {
                Control c = pnlButtons.Controls[i];
                if (c.Visible && c.Right > desno)
                    desno = c.Right;
            }
            var gumb = new SimpleButton();
            gumb.Name = "ButtonHELP";
            gumb.Text = "?";
            gumb.Width = 31;
            gumb.Height = pnlButtons.Height > 8 ? pnlButtons.Height - 8 : 28;
            gumb.Top = 4;
            gumb.Left = desno + 8;
            gumb.Click += delegate { PokaziPomoc(); };
            pnlButtons.Controls.Add(gumb);
        }

        /// <summary>DajPokaziHelp 7781-7799: JupiterHelp.exe -s server -d baza -h HELPKOD -o Str(operater) -p imeXml.</summary>
        private void PokaziPomoc()
        {
            string kod = AtributProjekta("HELPKOD").Trim().ToUpperInvariant();
            if (kod.Length == 0)
                return;
            string xml = _master.Kontekst == null ? "" : _master.Kontekst.XmlName;
            string op = _master.Kontekst == null ? "0" : _master.Kontekst.OperaterId;
            int broj;
            // VB Str() pozitivnom broju doda razmak sprijeda.
            string str = int.TryParse(op, NumberStyles.Integer, CultureInfo.InvariantCulture, out broj) && broj >= 0 ? " " + broj.ToString(CultureInfo.InvariantCulture) : op;
            string argumenti = "-s " + _server + " -d " + _baza + " -h " + kod + " -o " + str + " -p " + xml;
            NgDnevnik.Akcija("JupiterHelp.exe " + argumenti);
            // Klik-pregled ne pokreće vanjske programe.
            if (NgPoruka.Tiho)
                return;
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "JupiterHelp.exe";
                p.StartInfo.Arguments = argumenti;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                p.Start();
            }
            catch (Exception ex)
            {
                // On Error Resume Next: bez JupiterHelp.exe ne javlja ništa korisniku.
                NgDnevnik.Greska("JupiterHelp.exe: " + ex.Message);
            }
        }
    }
}
