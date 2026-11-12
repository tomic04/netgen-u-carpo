using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Kratki dijalog forme iz taga čvora (id:ime). Nije cijeli Form2.
    /// Natpis je CAPTION. ONLOAD koji je select puni polja istog imena.
    /// </summary>
    public sealed class Dijalog : Form
    {
        public Dijalog()
        {
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Microsoft Sans Serif", 8.25f);
            Width = 480;
            Height = 320;
            KeyPreview = true;
        }

        public void Prikazi(NgForm forma, SqlKontekst kontekst, Func<string, DataTable> citaj)
        {
            if (forma == null)
                return;
            Text = string.IsNullOrEmpty(forma.Atributi.Vrijednost("CAPTION")) ? forma.Ime : forma.Atributi.Vrijednost("CAPTION");
            NgDnevnik.Akcija("otvara formu " + forma.Ime + " node=" + (kontekst == null ? "" : kontekst.Node));
            var tijelo = new Panel();
            tijelo.Dock = DockStyle.Fill;
            tijelo.AutoScroll = true;
            Controls.Add(tijelo);
            int y = 8;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k.Grana == NgGrana.Button || k.Grana == NgGrana.Group)
                    continue;
                string natpis = k.Atributi.Vrijednost("CAPTION");
                if (string.IsNullOrEmpty(natpis))
                    natpis = k.Name;
                var l = new Label();
                l.Text = natpis;
                l.Left = 8;
                l.Top = y + 4;
                l.Width = 140;
                var t = new TextBox();
                t.Name = k.Name;
                t.Left = 152;
                t.Top = y;
                t.Width = 280;
                tijelo.Controls.Add(l);
                tijelo.Controls.Add(t);
                y += 28;
            }
            string on = forma.OnLoad ?? "";
            if (on.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase) && citaj != null)
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
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataColumn c in dt.Columns)
                    {
                        Control[] n = tijelo.Controls.Find(c.ColumnName, true);
                        if (n.Length > 0 && n[0] is TextBox)
                            ((TextBox)n[0]).Text = dt.Rows[0][c] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0][c]);
                    }
                }
            }
            var ok = new Button();
            ok.Text = "OK";
            ok.Dock = DockStyle.Bottom;
            ok.DialogResult = DialogResult.OK;
            Controls.Add(ok);
            AcceptButton = ok;
            ShowDialog();
        }
    }
}
