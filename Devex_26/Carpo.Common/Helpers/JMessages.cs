using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public class JMessages
    {
        public enum DbTypes { None, Close, Ok, Focus }

        public static void Error(Exception ex, string errText, string errCaption = "Greška", bool isLogLevelFour = false)
        {
            if (isLogLevelFour)
                XtraMessageBox.Show(Translator.TranslateString(errText), Translator.TranslateString(errCaption), MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Carpo.Common.Forms.frmError frmError = new Common.Forms.frmError(ex, Translator.TranslateString(errText), Translator.TranslateString(errCaption));
                frmError.ShowDialog();
            }
        }

        public static void Warning(string text)
        {
            Warning(text, "Upozorenje");
        }

        public static void Warning(string str, string caption)
        {
            XtraMessageBox.Show(Translator.TranslateString(str), Translator.TranslateString(caption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Info(string text)
        {
            Info(text, "Info");
        }

        public static void Info(string str, string caption)
        {
            XtraMessageBox.Show(Translator.TranslateString(str), Translator.TranslateString(caption), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}