using Carpo.Common.Models;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public class JConfirm
    {
        public static DialogResult Confirm(string text, string caption, string buttonYesValueStr, string buttonNoValueStr)
        {
            if (!string.IsNullOrEmpty(buttonYesValueStr) && !string.IsNullOrEmpty(buttonNoValueStr))
            {
                ButtonCaptions.yesCaption = buttonYesValueStr;
                ButtonCaptions.noCaption = buttonNoValueStr;
            }
            else
                ButtonCaptions.SetValuesToDefault();

            return Confirm(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static DialogResult Confirm(string text, string caption, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            return XtraMessageBox.Show(Translator.TranslateString(text), Translator.TranslateString(caption), messageBoxButtons, messageBoxIcon);
        }
    }
}