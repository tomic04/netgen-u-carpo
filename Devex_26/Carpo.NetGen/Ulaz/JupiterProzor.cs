using System;
using System.Windows.Forms;

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// Koler "Jup10" (Jupiter 10): prozor granule je bez ruba i iste veličine kao Jupiterov panel, a prati
    /// njegovu promjenu veličine i pomak forme. Form1.Run 1190-1205 (podloga, odmah i na promjenu) i
    /// Form2 PrvaForma 995-1027 (ulaz frm, samo na promjenu); ParentV_Resize i ParentForm_LocationChanged
    /// (Form1 8710-8726).
    /// </summary>
    internal static class JupiterProzor
    {
        public static bool JeJup10(string koler, int clientHandle)
        {
            return clientHandle != 0 && string.Equals((koler ?? "").Trim(), "Jup10", StringComparison.Ordinal);
        }

        /// <summary>Rub ide prije SetParent: promjena stila nakon njega može ponovo stvoriti prozor bez roditelja.</summary>
        public static void BezRuba(Form f)
        {
            f.FormBorderStyle = FormBorderStyle.None;
        }

        public static void PratiRoditelja(Form f, int clientHandle, bool odmah)
        {
            Control roditelj = Control.FromHandle(new IntPtr(clientHandle));
            if (roditelj == null)
                return;
            Form forma = roditelj.FindForm();
            EventHandler prati = delegate
            {
                if (f.IsDisposed)
                    return;
                f.WindowState = FormWindowState.Normal;
                f.Location = System.Drawing.Point.Empty;
                f.Size = roditelj.Size;
            };
            roditelj.Resize += prati;
            if (forma != null)
                forma.LocationChanged += prati;
            f.FormClosed += delegate
            {
                roditelj.Resize -= prati;
                if (forma != null)
                    forma.LocationChanged -= prati;
            };
            if (odmah)
                prati(roditelj, EventArgs.Empty);
        }
    }
}
