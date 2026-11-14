using System;
using System.Reflection;
using System.Windows.Forms;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Ctrl+F11. NetGen otvara Form5 s CBLog-om i parametrima (Form1.vb 1376-1378, Form2.vb 4167-4171).
    /// Carpo ima svoj log (Carpo.Core.Log.Log), pa se parametri upišu u njega i otvori se taj prozor
    /// (pregled-funkcija: NapuniCBLog/PokaziCBLog idu u Carpo logger).
    /// </summary>
    internal static class CarpoLogProzor
    {
        /// <summary>Upiše parametre i otvori log. Vraća poruku greške ili null.</summary>
        public static string Pokazi(IWin32Window vlasnik, string parametri)
        {
            if (!string.IsNullOrEmpty(parametri))
                NgDnevnik.Akcija(parametri);
            object logger = NgDnevnik.CarpoLogger;
            if (logger == null)
                return "Carpo logger nije spojen.";
            try
            {
                Type tip = logger.GetType().Assembly.GetType("Carpo.Core.Log.Log");
                if (tip == null)
                    return "Carpo.Core.Log.Log nije pronađen.";
                object frm = Activator.CreateInstance(tip, logger);
                MethodInfo show = tip.GetMethod("ShowDialog", new[] { typeof(IWin32Window) });
                if (show != null)
                    show.Invoke(frm, new object[] { vlasnik });
                else
                    tip.GetMethod("ShowDialog", Type.EmptyTypes).Invoke(frm, null);
                return null;
            }
            catch (Exception ex)
            {
                Exception u = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                return "Ctrl+F11: " + u.Message;
            }
        }
    }
}
