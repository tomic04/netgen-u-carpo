using System;
using System.Windows.Forms;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using DevExpress.XtraEditors;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// MsgBox iz NetGena (Class1.ExecSql 186/200/203, Form2 MSGBOX 7165) kao XtraMessageBox.
    /// Na CI-u (GITHUB_ACTIONS) i u klik-pregledu (Tiho) poruka ide samo u dnevnik,
    /// jer modalni prozor zaustavi test (MSG-01).
    /// </summary>
    public static class NgPoruka
    {
        public static bool Tiho;
        public static Action<string> Zapis;

        static NgPoruka()
        {
            // Form2.vb 7165-7169: MSGBOX se prikazuje. Bez ovoga PorukaNaredba.Prikazi je null u Carpu i poruka se gubi.
            PorukaNaredba.Zadano = delegate (string t) { Info(null, t, "Poruka"); };
            // ISEMPTYORZEROALERT/CANCELBOX kao XtraMessageBox (Carpo). Bez korisnika (CI, klik-pregled) pitanje je
            // Cancel: modalni prozor bi zaustavio test. Datoteke i mape ostaju WinForms dijalozi, kao u Carpu.
            RafalNaredba.ZadanoPitaj = delegate (string t)
            {
                NgDnevnik.Akcija("pitanje: " + (t ?? ""));
                if (BezKorisnika())
                    return false;
                return XtraMessageBox.Show(t ?? "", "Pitanje", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
            };
            RafalNaredba.ZadanoUpozori = delegate (string t)
            {
                NgDnevnik.Akcija("upozorenje: " + (t ?? ""));
                if (!BezKorisnika())
                    XtraMessageBox.Show(t ?? "", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }

        private static bool BezKorisnika()
        {
            return Tiho || string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS") ?? "", "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Statički konstruktor postavi PorukaNaredba.Zadano.</summary>
        public static void Pripremi()
        {
        }

        public static void Info(IWin32Window vlasnik, string tekst, string naslov)
        {
            NgDnevnik.Akcija("poruka: " + (tekst ?? ""));
            if (Zapis != null)
            {
                Zapis(tekst ?? "");
                return;
            }
            if (BezKorisnika())
                return;
            try
            {
                if (vlasnik != null)
                    XtraMessageBox.Show(vlasnik, tekst ?? "", naslov ?? "Poruka", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    XtraMessageBox.Show(tekst ?? "", naslov ?? "Poruka", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("poruka: " + ex.Message);
            }
        }
    }
}
