using Carpo.Common.Models;
using DevExpress.XtraEditors.Controls;

namespace Carpo.Common.Localization
{
    public class XtraEditorsLocalizer : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            if (id == StringId.XtraMessageBoxYesButtonText)
                return ButtonCaptions.yesCaption;
            if (id == StringId.XtraMessageBoxNoButtonText)
                return ButtonCaptions.noCaption;
            if (id == StringId.FilterShowAll)
                return Translator.TranslateString("(Označi sve)");
            if (id == StringId.Cancel)
                return Translator.TranslateString("Odustani");
            if (id == StringId.DateEditToday)
                return Translator.TranslateString("Danas");

            return base.GetLocalizedString(id);
        }
    }
}